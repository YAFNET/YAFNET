/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Controllers.Modals;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Logging;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Model;
using YAF.Types.Attributes;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Quick Reply Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[EnableRateLimiting("fixed")]
[CamelCaseOutput]
[Produces(MediaTypeNames.Application.Json)]
[Route("api/[controller]")]
[ApiController]
public class PostsModalController : ForumBaseController
{
    /// <summary>
    /// Move Topic
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("MoveTopic")]
    [Authorization(AuthorizationAccess.ModeratorAccess)]
    public IActionResult MoveTopic([FromBody] MoveTopicModal model)
    {
        if (model.ForumListSelected == this.PageBoardContext.PageForum.ID)
        {
            return this.Ok(new MessageModalNotification(
                this.GetText("MODERATE", "MOVE_TO_DIFFERENT"),
                MessageTypes.danger));
        }

        // Ederon : 7/14/2007
        this.GetRepository<Topic>().Move(
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageForum.ID,
            model.ForumListSelected,
            model.LeavePointer,
            model.LinkDays);

        this.Get<IDataCache>().Remove("TopicID");

        return this.Ok(
            this.Get<LinkBuilder>().GetLink(
                ForumPages.Topics,
                new { f = this.PageBoardContext.PageForum.ID, name = this.PageBoardContext.PageForum.Name }));
    }

    /// <summary>
    /// Quick Reply
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Reply")]
    public async Task<IActionResult> ReplyAsync([FromBody] QuickReplyModal model)
    {
        try
        {
            if (model.QuickReplyEditor.IsNotSet())
            {
                return this.Ok(new MessageModalNotification(this.GetText("EMPTY_MESSAGE"), MessageTypes.warning));
            }

            // No need to check whitespace if they are actually posting something
            if (this.PageBoardContext.BoardSettings.MaxPostSize > 0
                && model.QuickReplyEditor.Length >= this.PageBoardContext.BoardSettings.MaxPostSize)
            {
                return this.Ok(new MessageModalNotification(this.GetText("ISEXCEEDED"), MessageTypes.warning));
            }

            if (!(this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
                && this.PageBoardContext.BoardSettings.PostFloodDelay > 0
                && this.PageBoardContext.LastPosted.HasValue && this.PageBoardContext.LastPosted
                > DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
            {
                return this.Ok(
                    new MessageModalNotification(
                        this.GetTextFormatted(
                            "wait",
                            (this.PageBoardContext.LastPosted.Value
                             - DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
                            .Seconds),
                        MessageTypes.warning));
            }

            // post message...
            var message = model.QuickReplyEditor;

            // SPAM Check

            // Check if Forum is Moderated
            var isForumModerated = this.CheckForumModerateStatus(this.PageBoardContext.PageForum);

            var spamApproved = true;
            var isPossibleSpamMessage = false;

            // Check for SPAM
            if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
            {
                // Check content for spam
                if (this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName(),
                        this.HttpContext.GetUserRealIPAddress(),
                        message,
                        this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                        out var spamResult))
                {
                    var description =
                        $"""
                         Spam Check detected possible SPAM ({spamResult}) Original message: [{message}]
                                                        posted by User: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}
                         """;

                    switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                    {
                        case SpamPostHandling.DoNothing:
                            this.Get<ILogger<PostsModalController>>().SpamMessageDetected(
                                this.PageBoardContext.PageUserID,
                                description);
                            break;
                        case SpamPostHandling.FlagMessageUnapproved:
                            spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Get<ILogger<PostsModalController>>().SpamMessageDetected(
                                this.PageBoardContext.PageUserID,
                                $"{description}, it was flagged as unapproved post");
                            break;
                        case SpamPostHandling.RejectMessage:
                            this.Get<ILogger<PostsModalController>>().SpamMessageDetected(
                                this.PageBoardContext.PageUserID,
                                $"{description}, post was rejected");

                            return this.Ok(new MessageModalNotification(this.GetText("SPAM_MESSAGE"), MessageTypes.danger));
                        case SpamPostHandling.DeleteBanUser:
                            this.Get<ILogger<PostsModalController>>().SpamMessageDetected(
                                this.PageBoardContext.PageUserID,
                                $"{description}, user was deleted and banned");

                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageBoardContext.PageUser,
                                this.PageBoardContext.MembershipUser,
                                this.PageBoardContext.PageUser.IP);
                            return this.Ok();
                    }
                }

                if (!this.PageBoardContext.IsGuest)
                {
                    this.UpdateWatchTopic(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID, model.TopicWatch);
                }
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
            {
                spamApproved = true;
            }

            var messageFlags = new MessageFlags
            {
                IsHtml = false,
                IsBBCode = true,
                IsApproved = spamApproved
            };

            // Bypass Approval if Admin or Moderator.
            var newMessage = this.GetRepository<Message>().SaveNew(
                this.PageBoardContext.PageForum,
                this.PageBoardContext.PageTopic,
                this.PageBoardContext.PageUser,
                message,
                null,
                this.HttpContext.GetUserRealIPAddress(),
                DateTime.UtcNow,
                null,
                messageFlags);

            newMessage.Topic = this.PageBoardContext.PageTopic;

            // Check to see if the user has enabled "auto watch topic" option in his/her profile.
            if (this.PageBoardContext.PageUser.AutoWatchTopics)
            {
                var watchTopicId = this.GetRepository<WatchTopic>().Check(
                    this.PageBoardContext.PageUserID,
                    this.PageBoardContext.PageTopicID);

                if (!watchTopicId.HasValue)
                {
                    // subscribe to this topic
                    this.GetRepository<WatchTopic>().Add(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);
                }
            }

            if (messageFlags.IsApproved)
            {
                // send new post notification to users watching this topic/forum
                await this.Get<ISendNotification>().ToWatchingUsersAsync(newMessage);

                if (!this.PageBoardContext.IsGuest && this.PageBoardContext.PageUser.Activity)
                {
                    this.Get<IActivityStream>().AddReplyToStream(
                        this.PageBoardContext.PageUser,
                        this.PageBoardContext.PageTopicID,
                        newMessage.ID,
                        this.PageBoardContext.PageTopic.TopicName,
                        message);
                }

                // redirect to newly posted message
                return this.Ok(this.Get<LinkBuilder>().GetMessageLink(this.PageBoardContext.PageTopic, newMessage.ID));
            }

            if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
            {
                // not approved, notify moderators
                await this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApprovalAsync(
                    this.PageBoardContext.PageForumID,
                    newMessage.ID,
                    isPossibleSpamMessage);
            }

            var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

            return this.Ok(this.Get<LinkBuilder>().GetLink(ForumPages.Info, new { i = 1, url = HttpUtility.UrlEncode(url) }));
        }
        catch (Exception exception)
        {
            if (exception.GetType() != typeof(ThreadAbortException))
            {
                this.Get<ILogger<PostsModalController>>().Log(this.PageBoardContext.PageUserID, this, exception);
            }

            return this.Ok();
        }
    }

    /// <summary>
    /// Checks the forum moderate status.
    /// </summary>
    /// <param name="forumInfo">The forum information.</param>
    /// <returns>Returns if the forum needs to be moderated</returns>
    private bool CheckForumModerateStatus(Forum forumInfo)
    {
        // User Moderate override
        if (this.PageBoardContext.PageUser.UserFlags.Moderated)
        {
            return true;
        }

        var forumModerated = forumInfo.ForumFlags.IsModerated;

        if (!forumModerated)
        {
            return false;
        }

        if (forumInfo.IsModeratedNewTopicOnly)
        {
            return false;
        }

        if (!forumInfo.ModeratedPostCount.HasValue || this.PageBoardContext.IsGuest)
        {
            return true;
        }

        var moderatedPostCount = forumInfo.ModeratedPostCount.Value;

        return this.PageBoardContext.PageUser.NumPosts < moderatedPostCount;
    }

    /// <summary>
    /// Updates Watch Topic based on controls/settings for user...
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <param name="topicWatch"></param>
    private void UpdateWatchTopic(int userId, int topicId, bool topicWatch)
    {
        var topicWatchedId = this.GetRepository<WatchTopic>().Check(userId, topicId);

        if (topicWatchedId.HasValue && !topicWatch)
        {
            // unsubscribe...
            this.GetRepository<WatchTopic>().DeleteById(topicWatchedId.Value);
        }
        else if (!topicWatchedId.HasValue && topicWatch)
        {
            // subscribe to this topic...
            this.GetRepository<WatchTopic>().Add(userId, topicId);
        }
    }
}