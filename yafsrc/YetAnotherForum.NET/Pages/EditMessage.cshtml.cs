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

using YAF.Types.Models;

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;
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

/// <summary>
/// The Edit message Page.
/// </summary>
public class EditMessageModel : ForumPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditMessageInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the Topic Priorities.
    /// </summary>
    public IReadOnlyCollection<SelectListItem> Priorities => StaticDataHelper.TopicPriorities();

    /// <summary>
    /// Initializes a new instance of the <see cref="EditMessageModel"/> class.
    /// </summary>
    public EditMessageModel()
        : base("POSTMESSAGE", ForumPages.EditMessage)
    {
        this.PageBoardContext.CurrentForumPage.PageTitle = this.GetText("EDIT");
    }

    /// <summary>
    /// Canceling Posting New Message Or editing Message.
    /// </summary>
    public IActionResult OnPostCancel()
    {
        // reply to existing topic or editing of existing topic
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Posts,
            new {t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName});
    }

    /// <summary>
    /// Handles verification of the PostReply. Adds javascript message if there is a problem.
    /// </summary>
    /// <returns>
    /// true if everything is verified
    /// </returns>
    private bool IsPostReplyVerified()
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
            this.Input.From = this.Input.From[100..];

            this.PageBoardContext.Notify(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.warning);

            return false;
        }

        if (this.PageBoardContext.PageTopic.UserID == this.PageBoardContext.PageMessage.UserID ||
            this.PageBoardContext.ForumModeratorAccess)
        {
            if (this.Input.TopicSubject.IsNotSet())
            {
                this.PageBoardContext.Notify(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.AllowCreateTopicsSameName)
                && this.GetRepository<Topic>().CheckForDuplicate(this.Input.TopicSubject.Trim())
                && this.PageBoardContext.PageMessage is null)
            {
                this.PageBoardContext.Notify(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        this.Input = new EditMessageInputModel();

        if (this.PageBoardContext.PageMessage is null)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (!this.PageBoardContext.ForumPostAccess && !this.PageBoardContext.ForumReplyAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (!this.CanEditPostCheck())
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (this.PageBoardContext.PageTopic.TopicFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Posts,
                new {t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName});
        }

        this.Input.Priority = 0;

        // update options...
        if (!this.PageBoardContext.IsGuest)
        {
            if (this.PageBoardContext.PageTopicID > 0)
            {
                var item = await this.GetRepository<WatchTopic>().CheckAsync(
                    this.PageBoardContext.PageUserID,
                    this.PageBoardContext.PageTopicID);

                this.Input.TopicWatch = item.HasValue;
            }
            else
            {
                this.Input.TopicWatch = this.PageBoardContext.PageUser.AutoWatchTopics;
            }
        }

        // editing a message...
        this.InitEditedPost();

        return this.Page();
    }

    /// <summary>
    /// The create page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        // add topic link...
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        // editing..
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EDIT"));
    }

    /// <summary>
    /// The post reply handle edit post.
    /// </summary>
    /// <returns>
    /// Returns the Message Id
    /// </returns>
    private async Task<Message> PostReplyHandleEditPostAsync()
    {
        var subjectSave = string.Empty;
        var descriptionSave = string.Empty;
        var stylesSave = string.Empty;

        if (this.PageBoardContext.PageTopic.UserID == this.PageBoardContext.PageMessage.UserID
            || this.PageBoardContext.ForumModeratorAccess)
        {
            subjectSave = this.Input.TopicSubject;
            descriptionSave = this.Input.TopicDescription;
        }

        if (this.PageBoardContext.BoardSettings.UseStyledTopicTitles
            && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
        {
            stylesSave = this.Input.TopicStyles;
        }

        // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
        // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
        // retrieve message flags
        var messageFlags = new MessageFlags(this.PageBoardContext.PageMessage.Flags) {
                               IsHtml = false,
                               IsBBCode = true,
                               IsPersistent = this.Input.Persistent
                           };

        this.PageBoardContext.PageMessage.Flags = messageFlags.BitValue;

        var isModeratorChanged = this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID;

        var messageUser = await this.GetRepository<User>().GetByIdAsync(this.PageBoardContext.PageMessage.UserID);

        var messageText = this.Input.Editor;

        await this.GetRepository<Message>().UpdateAsync(
            this.Input.Priority.ToType<short>(),
            HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(messageText)),
            descriptionSave,
            string.Empty,
            stylesSave,
            subjectSave,
            this.HtmlEncode(this.Input.ReasonEditor),
            isModeratorChanged,
            this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess,
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageMessage,
            this.PageBoardContext.PageForum,
            messageUser,
            this.PageBoardContext.PageUserID);

        // Update Topic Tags?!
        await this.GetRepository<TopicTag>().DeleteAsync(x => x.TopicID == this.PageBoardContext.PageTopicID);

        await this.GetRepository<TopicTag>().AddTagsToTopicAsync(this.Input.TagsValue, this.PageBoardContext.PageTopicID);

        await this.UpdateWatchTopicAsync(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);

        return this.PageBoardContext.PageMessage;
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

        if (!this.PageBoardContext.ForumEditAccess)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        var isPossibleSpamMessage = false;

        // Check for SPAM
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
        {
            // Check content for spam
            if (this.Get<ISpamCheck>().CheckPostForSpam(
                    this.PageBoardContext.IsGuest
                        ? this.Input.From
                        : this.PageBoardContext.PageUser.DisplayOrUserName(),
                    this.HttpContext.GetUserRealIPAddress(),
                    BBCodeHelper.StripBBCode(HtmlTagHelper.StripHtml(HtmlTagHelper.CleanHtmlString(this.Input.Editor)))
                        .RemoveMultipleWhitespace(),
                    this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                    out var spamResult))
            {
                var description =
                    $"""
                     Spam Check detected possible SPAM ({spamResult}) Original message: [{this.Input.Editor}]
                                                posted by User: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}
                     """;

                switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                {
                    case SpamPostHandling.DoNothing:
                        this.Get<Logger<EditMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            description);
                        break;
                    case SpamPostHandling.FlagMessageUnapproved:
                        isPossibleSpamMessage = true;
                        this.Get<Logger<EditMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, it was flagged as unapproved post.");
                        break;
                    case SpamPostHandling.RejectMessage:
                        this.Get<Logger<EditMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, post was rejected");
                        return this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                    case SpamPostHandling.DeleteBanUser:
                        this.Get<Logger<EditMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, user was deleted and banned");

                        await this.Get<IAspNetUsersHelper>().DeleteAndBanUserAsync(
                            this.PageBoardContext.PageUser,
                            this.PageBoardContext.MembershipUser,
                            this.PageBoardContext.PageUser.IP);

                        return this.Page();
                }
            }
        }

        // Edit existing post
        var editMessage = await this.PostReplyHandleEditPostAsync();

        // Check if message is approved
        var isApproved = editMessage.MessageFlags.IsApproved;

        var messageId = editMessage.ID;

        // Create notification emails
        if (isApproved)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Post,
                new {m = messageId, name = this.PageBoardContext.PageTopic.TopicName});
        }

        // Not Approved
        if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
        {
            // not approved, notify moderators
            await this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApprovalAsync(
                this.PageBoardContext.PageForumID,
                messageId.ToType<int>(),
                isPossibleSpamMessage);
        }

        // Tell user that his message will have to be approved by a moderator
        var url = this.Get<ILinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

        if (this.PageBoardContext.PageTopicID > 0 && this.PageBoardContext.PageTopic.NumPosts > 1)
        {
            url = this.Get<ILinkBuilder>().GetTopicLink(
                this.PageBoardContext.PageTopic);
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Info, new {i = 1, url = HttpUtility.UrlEncode(url)});
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
    /// The can edit post check.
    /// </summary>
    /// <returns>
    /// Returns if user can edit post check.
    /// </returns>
    private bool CanEditPostCheck()
    {
        var postLocked = false;

        if (!this.PageBoardContext.IsAdmin && this.PageBoardContext.BoardSettings.LockPosts > 0)
        {
            var edited = this.PageBoardContext.PageMessage.Edited ?? this.PageBoardContext.PageMessage.Posted;

            if (edited.AddDays(this.PageBoardContext.BoardSettings.LockPosts) < DateTime.UtcNow)
            {
                postLocked = true;
            }
        }

        // get  forum information
        var forumInfo = this.PageBoardContext.PageForum;

        // Ederon : 9/9/2007 - moderator can edit in locked topics
        return !postLocked && !forumInfo.ForumFlags.IsLocked && !this.PageBoardContext.PageTopic.TopicFlags.IsLocked
               && this.PageBoardContext.PageMessage.UserID == this.PageBoardContext.PageUserID
               || this.PageBoardContext.ForumModeratorAccess && this.PageBoardContext.ForumEditAccess;
    }

    /// <summary>
    /// The initializes the edited post.
    /// </summary>
    private void InitEditedPost()
    {
        if (this.PageBoardContext.PageMessage.MessageFlags.IsHtml)
        {
            // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
            this.PageBoardContext.PageMessage.MessageText = this.Get<IBBCodeService>()
                .ConvertHtmlToBBCodeForEdit(this.PageBoardContext.PageMessage.MessageText);
        }

        this.Input.Editor = BBCodeHelper.DecodeCodeBlocks(this.PageBoardContext.PageMessage.MessageText);

        this.Input.TopicSubject = HttpUtility.HtmlDecode(this.PageBoardContext.PageTopic.TopicName);
        this.Input.TopicDescription = HttpUtility.HtmlDecode(this.PageBoardContext.PageTopic.Description);

        this.Input.TopicStyles = this.PageBoardContext.PageTopic.Styles;

        this.Input.Priority = this.PageBoardContext.PageTopic.Priority;

        this.Input.ReasonEditor = HttpUtility.HtmlDecode(this.PageBoardContext.PageMessage.EditReason);
        this.Input.Persistent = this.PageBoardContext.PageMessage.MessageFlags.IsPersistent;

        var topicsList = this.GetRepository<TopicTag>().List(this.PageBoardContext.PageTopicID);

        if (topicsList.Count == 0)
        {
            return;
        }

        this.Input.TagsValue = topicsList.Select(t => t.Item2.TagName).ToDelimitedString(",");
        this.Input.Tags = topicsList.Select(t => t.Item2.TagName);
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
        var topicWatchedId = await this.GetRepository<WatchTopic>().CheckAsync(userId, topicId);

        if (topicWatchedId.HasValue && !this.Input.TopicWatch)
        {
            // unsubscribe...
            await this.GetRepository<WatchTopic>().DeleteByIdAsync(topicWatchedId.Value);
        }
        else if (!topicWatchedId.HasValue && this.Input.TopicWatch)
        {
            // subscribe to this topic...
            await this.GetRepository<WatchTopic>().AddAsync(userId, topicId);
        }
    }
}