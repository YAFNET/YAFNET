/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// The post New Topic Page.
/// </summary>
public class PostTopicModel : ForumPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public PostTopicInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the Topic Priorities.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> Priorities => StaticDataHelper.TopicPriorities();

    /// <summary>
    ///   The Spam Approved Indicator
    /// </summary>
    private bool spamApproved = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostTopicModel"/> class.
    /// </summary>
    public PostTopicModel()
        : base("POSTTOPIC", ForumPages.PostTopic)
    {
        this.PageBoardContext.CurrentForumPage.PageTitle = this.GetText("NEWTOPIC");
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        // add the "New Topic" page link last...
        this.PageBoardContext.PageLinks.AddLink(this.GetText("NEWTOPIC"));
    }

    /// <summary>
    /// Canceling Posting New Message Or editing Message.
    /// </summary>
    public IActionResult OnPostCancel()
    {
        // new topic -- cancel back to forum
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Topics,
            new {f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name});
    }

    /// <summary>
    /// Verifies the user isn't posting too quickly, if so, tells them to wait.
    /// </summary>
    /// <returns>
    /// True if there is a delay in effect.
    /// </returns>
    protected bool IsPostReplyDelay()
    {
        // see if there is a post delay
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess
                                          || this.PageBoardContext.BoardSettings.PostFloodDelay <= 0)
        {
            return false;
        }

        var lastPosted = this.GetRepository<Message>().GetUserLastPosted(this.PageBoardContext.PageUserID);

        if (!lastPosted.HasValue)
        {
            return false;
        }

        // see if they've past that delay point
        if (lastPosted
            <= DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
        {
            return false;
        }

        this.PageBoardContext.Notify(
            this.GetTextFormatted(
                "wait",
                (lastPosted.Value
                 - DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay)).Seconds),
            MessageTypes.warning);
        return true;
    }

    /// <summary>
    /// Handles verification of the PostReply. Adds java script message if there is a problem.
    /// </summary>
    /// <returns>
    /// true if everything is verified
    /// </returns>
    protected bool IsPostReplyVerified()
    {
        // To avoid posting whitespace(s) or empty messages
        var postedMessage = HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.Input.Editor ?? string.Empty));

        if (postedMessage.IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("ISEMPTY"), MessageTypes.warning);
            return false;
        }

        // No need to check whitespace if they are actually posting something
        if (this.PageBoardContext.BoardSettings.MaxPostSize > 0
            && this.Input.Editor.Length >= this.PageBoardContext.BoardSettings.MaxPostSize)
        {
            this.PageBoardContext.Notify(this.GetText("ISEXCEEDED"), MessageTypes.warning);
            return false;
        }

        // Check if the Entered Guest Username is not too long
        if (this.PageBoardContext.IsGuest && this.Input.From.Length > 100)
        {
            this.PageBoardContext.Notify(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.warning);

            this.Input.From = this.Input.From[100..];
            return false;
        }

        if (HtmlTagHelper.StripHtml(this.Input.TopicSubject).IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
            return false;
        }

        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.AllowCreateTopicsSameName) && this
                .GetRepository<Topic>().CheckForDuplicate(HtmlTagHelper.StripHtml(this.Input.TopicSubject).Trim()))
        {
            this.PageBoardContext.Notify(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        this.Input = new PostTopicInputModel {
            Persistent = true
        };

        if (this.PageBoardContext.PageForumID == 0)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (!this.PageBoardContext.ForumPostAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input.Priority = 0;

        // update options...
        if (this.PageBoardContext.IsGuest)
        {
            return this.Page();
        }

        if (this.PageBoardContext.PageTopicID > 0)
        {
            var check = await this.GetRepository<WatchTopic>().CheckAsync(
                this.PageBoardContext.PageUserID,
                this.PageBoardContext.PageTopicID);

            this.Input.TopicWatch = check.HasValue;
        }
        else
        {
            this.Input.TopicWatch = this.PageBoardContext.PageUser.AutoWatchTopics;
        }

        return this.Page();
    }

    /// <summary>
    /// The post reply handle new post.
    /// </summary>
    /// <returns>
    /// Returns the Message Id.
    /// </returns>
    async protected Task<Message> PostReplyHandleNewTopicAsync()
    {
        // Check if Forum is Moderated
        var isForumModerated = this.CheckForumModerateStatus(this.PageBoardContext.PageForum, true);

        // If Forum is Moderated
        if (isForumModerated)
        {
            this.spamApproved = false;
        }

        // Bypass Approval if Admin or Moderator
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
        {
            this.spamApproved = true;
        }

        // make message flags
        var messageFlags = new MessageFlags {
                                                IsHtml = false,
                                                IsBBCode = true,
                                                IsPersistent = this.Input.Persistent,
                                                IsApproved = this.spamApproved
                                            };

        var messageText = this.Input.Editor;

        // Save to Db
        var (topic, message) = await this.GetRepository<Topic>().SaveNewAsync(
            this.PageBoardContext.PageForum,
            HtmlTagHelper.StripHtml(this.Input.TopicSubject),
            string.Empty,
            HtmlTagHelper.StripHtml(this.Input.TopicStyles),
            HtmlTagHelper.StripHtml(this.Input.TopicDescription),
            HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(messageText)),
            this.PageBoardContext.PageUser,
            this.Input.Priority.ToType<short>(),
            this.PageBoardContext.IsGuest ? this.Input.From : this.PageBoardContext.PageUser.Name,
            this.PageBoardContext.IsGuest ? this.Input.From : this.PageBoardContext.PageUser.DisplayName,
            this.HttpContext.GetUserRealIPAddress(),
            DateTime.UtcNow,
            messageFlags);

        message.Topic = topic;

        await this.UpdateWatchTopicAsync(this.PageBoardContext.PageUserID, topic.ID);

        return message;
    }

    /// <summary>
    /// Handles the PostReply click including: Replying, Editing and New post.
    /// </summary>
    public async Task<IActionResult> OnPostPostReplyAsync()
    {
        if (!this.IsPostReplyVerified())
        {
            return this.Page();
        }

        if (this.IsPostReplyDelay())
        {
            return this.Page();
        }

        var isPossibleSpamMessage = false;

        var message = HtmlTagHelper.StripHtml(this.Input.Editor);

        // Check for SPAM
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess && this.Get<ISpamCheck>().CheckPostForSpam(
                this.PageBoardContext.IsGuest
                    ? this.Input.From
                    : this.PageBoardContext.PageUser.DisplayOrUserName(),
                this.HttpContext.GetUserRealIPAddress(),
                BBCodeHelper.StripBBCode(HtmlTagHelper.StripHtml(HtmlTagHelper.CleanHtmlString(this.Input.Editor)))
                    .RemoveMultipleWhitespace(),
                this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                out var spamResult))
        {
            // Check content for spam
            var description = $"""
                               Spam Check detected possible SPAM ({spamResult})
                                                          posted by PageUser: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}
                               """;

            switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
            {
                case SpamPostHandling.DoNothing:
                    this.Get<Logger<PostMessageModel>>().SpamMessageDetected(
                        this.PageBoardContext.PageUserID,
                        description);
                    break;
                case SpamPostHandling.FlagMessageUnapproved:
                    this.spamApproved = false;
                    isPossibleSpamMessage = true;
                    this.Get<Logger<PostMessageModel>>().SpamMessageDetected(
                        this.PageBoardContext.PageUserID,
                        $"{description}, it was flagged as unapproved post.");
                    break;
                case SpamPostHandling.RejectMessage:
                    this.Get<Logger<PostMessageModel>>().SpamMessageDetected(
                        this.PageBoardContext.PageUserID,
                        $"S{description}, post was rejected");
                    return this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                case SpamPostHandling.DeleteBanUser:
                    this.Get<Logger<PostMessageModel>>().SpamMessageDetected(
                        this.PageBoardContext.PageUserID,
                        $"{description}, user was deleted and banned");

                    await this.Get<IAspNetUsersHelper>().DeleteAndBanUserAsync(
                        this.PageBoardContext.PageUser,
                        this.PageBoardContext.MembershipUser,
                        this.PageBoardContext.PageUser.IP);

                    return this.Page();
            }
        }

        // New Topic
        var newMessage = await this.PostReplyHandleNewTopicAsync();

        // Check if message is approved
        var isApproved = newMessage.MessageFlags.IsApproved;

        // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
        var attachPollParameter = this.PageBoardContext.ForumPollAccess;

        // Create notification emails
        if (isApproved)
        {
            await this.Get<ISendNotification>().ToWatchingUsersAsync(newMessage, true);

            if (!this.PageBoardContext.IsGuest && this.PageBoardContext.PageUser.Activity)
            {
                // Handle Mentions
                BBCodeHelper.FindMentions(message).ForEach(
                    user =>
                    {
                        var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                        if (userId != this.PageBoardContext.PageUserID)
                        {
                            this.Get<IActivityStream>().AddMentionToStream(
                                userId,
                                newMessage.TopicID,
                                newMessage.ID,
                                this.PageBoardContext.PageUserID);
                        }
                    });

                // Handle User Quoting
                BBCodeHelper.FindUserQuoting(message).ForEach(
                    user =>
                    {
                        var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                        if (userId != this.PageBoardContext.PageUserID)
                        {
                            this.Get<IActivityStream>().AddQuotingToStream(
                                userId,
                                newMessage.TopicID,
                                newMessage.ID,
                                this.PageBoardContext.PageUserID);
                        }
                    });

                this.Get<IActivityStream>().AddTopicToStream(
                    this.PageBoardContext.PageUser,
                    newMessage.TopicID,
                    newMessage.ID,
                    HtmlTagHelper.StripHtml(this.Input.TopicSubject),
                    message);

                // Add tags
                if (this.Input.TagsValue.IsSet())
                {
                    await this.GetRepository<TopicTag>().AddTagsToTopicAsync(this.Input.TagsValue, newMessage.TopicID);
                }
            }

            if (!attachPollParameter || !this.Input.AddPoll)
            {
                // regular redirect...
                return this.Get<ILinkBuilder>().Redirect(
                    ForumPages.Post,
                    new {m = newMessage.ID, name = newMessage.Topic.TopicName});
            }

            // poll edit redirect...
            return this.Get<ILinkBuilder>().Redirect(ForumPages.PollEdit, new {t = newMessage.TopicID});
        }

        // Not Approved
        if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
        {
            // not approved, notify moderators
            await this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApprovalAsync(
                this.PageBoardContext.PageForumID,
                newMessage.ID,
                isPossibleSpamMessage);
        }

        // 't' variable is required only for poll and this is a attach poll token for attachments page
        if (!this.Input.AddPoll)
        {
            attachPollParameter = false;
        }

        // Tell user that his message will have to be approved by a moderator
        var url = this.Get<ILinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

        if (!attachPollParameter)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Info,
                new {i = 1, url = HttpUtility.UrlEncode(url)});
        }

        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.PollEdit,
            new {ra = 1, t = newMessage.TopicID, f = this.PageBoardContext.PageForumID});
    }

    /// <summary>
    /// Previews the new Message
    /// </summary>
    public IActionResult OnPostPreview()
    {
        this.Input.PreviewMessage = this.Input.Editor;

        return this.Page();
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
    private async Task UpdateWatchTopicAsync(int userId, int topicId)
    {
        if (this.Input.TopicWatch)
        {
            // subscribe to this topic...
            await this.GetRepository<WatchTopic>().AddAsync(userId, topicId);
        }
    }

    /// <summary>
    /// Checks the forum moderate status.
    /// </summary>
    /// <param name="forumInfo">The forum information.</param>
    /// <param name="isNewTopic">if set to <c>true</c> [is new topic].</param>
    /// <returns>
    /// Returns if the forum needs to be moderated
    /// </returns>
    private bool CheckForumModerateStatus(Forum forumInfo, bool isNewTopic)
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

        if (forumInfo.IsModeratedNewTopicOnly && !isNewTopic)
        {
            return false;
        }

        if (!forumInfo.ModeratedPostCount.HasValue || this.PageBoardContext.IsGuest)
        {
            return true;
        }

        var moderatedPostCount = forumInfo.ModeratedPostCount;

        return !(this.PageBoardContext.PageUser.NumPosts >= moderatedPostCount);
    }
}