
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

namespace YAF.Pages;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Objects;
using YAF.Types.Models;

using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Extensions.Logging;

/// <summary>
/// The post message Page.
/// </summary>
public class PostMessageModel : ForumPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public PostMessageInputModel Input { get; set; }

    /// <summary>
    ///   The Spam Approved Indicator
    /// </summary>
    private bool spamApproved = true;

    /// <summary>
    /// The edit or quoted message.
    /// </summary>
    private Message quotedMessage;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostMessageModel"/> class.
    /// </summary>
    public PostMessageModel()
        : base("POSTMESSAGE", ForumPages.PostMessage)
    {
        this.PageBoardContext.CurrentForumPage.PageTitle = this.GetText("reply");
    }

    /// <summary>
    /// Canceling Posting New Message Or editing Message.
    /// </summary>
    public IActionResult OnPostCancel()
    {
        // reply to existing topic or editing of existing topic
        return this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
           new {t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName});
    }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        // add topic link...
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        // add "reply" text...
        this.PageBoardContext.PageLinks.AddLink(this.GetText("reply"));
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
    /// Handles verification of the PostReply. Adds javascript message if there is a problem.
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

        return true;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? q = null, string text = null)
    {
        // in case topic is deleted or not existent
        if (this.PageBoardContext.PageTopic is null)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Input = new PostMessageInputModel();

        if (this.PageBoardContext.PageForumID == 0)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        if (!this.PageBoardContext.ForumPostAccess && !this.PageBoardContext.ForumReplyAccess)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.PageBoardContext.PageTopic.TopicFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        // we reply to a post with a quote
        if (q.HasValue)
        {
            this.quotedMessage = this.GetRepository<Message>().GetMessage(q.Value);

            if (this.quotedMessage != null)
            {
                if (text.IsSet())
                {
                    var quotedMessageText = HttpUtility.UrlDecode(text);

                    this.quotedMessage.MessageText = HtmlTagHelper.StripHtml(
                        BBCodeHelper.EncodeCodeBlocks(HtmlTagHelper.CleanHtmlString(quotedMessageText)));
                }

                if (this.quotedMessage.TopicID != this.PageBoardContext.PageTopicID)
                {
                    return this.Get<LinkBuilder>().AccessDenied();
                }

                if (!this.CanQuotePostCheck(this.PageBoardContext.PageTopic))
                {
                    return this.Get<LinkBuilder>().AccessDenied();
                }
            }
        }

        // update options...
        if (!this.PageBoardContext.IsGuest)
        {
            this.Input.TopicWatch = this.PageBoardContext.PageTopicID > 0
                                        ? this.GetRepository<WatchTopic>().Check(
                                            this.PageBoardContext.PageUserID,
                                            this.PageBoardContext.PageTopicID).HasValue
                                        : this.PageBoardContext.PageUser.AutoWatchTopics;
        }

        if (this.quotedMessage != null && q.HasValue)
        {
            if (this.Get<ISessionService>().MultiQuoteIds != null)
            {
                var quoteId = q;
                var multiQuote = new MultiQuote
                    { MessageID = quoteId.Value, TopicID = this.PageBoardContext.PageTopicID };

                if (!this.Get<ISessionService>().MultiQuoteIds.Exists(m => m.MessageID.Equals(quoteId)))
                {
                    this.Get<ISessionService>().MultiQuoteIds.Add(multiQuote);
                }

                var messages = this.GetRepository<Message>().GetByIds(
                    this.Get<ISessionService>().MultiQuoteIds.Select(i => i.MessageID));

                messages.ForEach(this.InitQuotedReply);

                // Clear Multi-quotes
                this.Get<ISessionService>().MultiQuoteIds = null;
            }
            else
            {
                this.InitQuotedReply(this.quotedMessage);
            }
        }

        return this.Page();
    }

    /// <summary>
    /// The post reply handle reply to topic.
    /// </summary>
    /// <param name="isSpamApproved">
    /// The is Spam Approved.
    /// </param>
    /// <param name="replyTo"></param>
    /// <returns>
    /// Returns the new Message.
    /// </returns>
    protected Message PostReplyHandleReplyToTopic(bool isSpamApproved, int? replyTo = null)
    {
        // Check if Forum is Moderated
        var isForumModerated = this.CheckForumModerateStatus(this.PageBoardContext.PageForum, false);

        // If Forum is Moderated
        if (isForumModerated)
        {
            isSpamApproved = false;
        }

        // Bypass Approval if Admin or Moderator
        if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
        {
            isSpamApproved = true;
        }

        // make message flags
        var messageFlags = new MessageFlags {
                                                IsHtml = false,
                                                IsBBCode = true,
                                                IsPersistent = this.Input.Persistent,
                                                IsApproved = isSpamApproved
                                            };

        var messageText = this.Input.Editor;

        var message = this.GetRepository<Message>().SaveNew(
            this.PageBoardContext.PageForum,
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageUser,
            HtmlTagHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(messageText)),
            this.AspNetUser != null ? null : this.Input.From,
            this.HttpContext.GetUserRealIPAddress(),
            DateTime.UtcNow,
            replyTo,
            messageFlags);

        message.Topic = this.PageBoardContext.PageTopic;

        this.UpdateWatchTopic(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);

        return message;
    }

    /// <summary>
    /// Handles the PostReply click including: Replying, Editing and New post.
    /// </summary>
    public async Task<IActionResult> OnPostPostReplyAsync(int? q = null)
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
                            $"{description}, post was rejected");
                        return this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                    case SpamPostHandling.DeleteBanUser:
                        this.Get<Logger<PostMessageModel>>().SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, user was deleted and banned");

                        this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                            this.PageBoardContext.PageUser,
                            this.PageBoardContext.MembershipUser,
                            this.PageBoardContext.PageUser.IP);

                        return this.Page();
                }
            }
        }

        // Reply to topic
        var newMessage = this.PostReplyHandleReplyToTopic(this.spamApproved, q);

        var isApproved = newMessage.MessageFlags.IsApproved;

        // Create notification emails
        if (isApproved)
        {
            await this.Get<ISendNotification>().ToWatchingUsersAsync(newMessage);

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
                                this.PageBoardContext.PageTopicID,
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
                                this.PageBoardContext.PageTopicID,
                                newMessage.ID,
                                this.PageBoardContext.PageUserID);
                        }
                    });

                this.Get<IActivityStream>().AddReplyToStream(
                    this.PageBoardContext.PageUser,
                    this.PageBoardContext.PageTopicID,
                    newMessage.ID,
                    this.PageBoardContext.PageTopic.TopicName,
                    this.Input.Editor);
            }

            // regular redirect...
            return this.Get<LinkBuilder>().Redirect(
                ForumPages.Post,
                new {m = newMessage.ID, name = this.PageBoardContext.PageTopic.TopicName});
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

        // Tell user that his message will have to be approved by a moderator
        var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

        if (this.PageBoardContext.PageTopicID > 0 && this.PageBoardContext.PageTopic.NumPosts > 1)
        {
            url = this.Get<LinkBuilder>().GetTopicLink(
                this.PageBoardContext.PageTopic);
        }

        return this.Get<LinkBuilder>().Redirect(ForumPages.Info, new {i = 1, url = HttpUtility.UrlEncode(url)});
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
    /// Determines whether this instance [can quote post check] the specified topic info.
    /// </summary>
    /// <param name="topicInfo">
    /// The topic info.
    /// </param>
    /// <returns>
    /// The can quote post check.
    /// </returns>
    private bool CanQuotePostCheck(Topic topicInfo)
    {
        // get topic and forum information
        var forumInfo = this.PageBoardContext.PageForum;

        if (topicInfo is null || forumInfo is null)
        {
            return false;
        }

        // Ederon : 9/9/2007 - moderator can reply to locked topics
        return !forumInfo.ForumFlags.IsLocked && !topicInfo.TopicFlags.IsLocked
               || this.PageBoardContext.ForumModeratorAccess && this.PageBoardContext.ForumReplyAccess;
    }

    /// <summary>
    /// Initializes the quoted reply.
    /// </summary>
    /// <param name="message">
    /// The current TypedMessage.
    /// </param>
    private void InitQuotedReply(Message message)
    {
        var messageContent = message.MessageText;

        if (this.PageBoardContext.BoardSettings.RemoveNestedQuotes)
        {
            messageContent = this.Get<IFormatMessage>().RemoveNestedQuotes(messageContent);
        }

        if (message.MessageFlags.IsHtml)
        {
            // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
            messageContent = this.Get<IBBCodeService>().ConvertHtmlToBBCodeForEdit(messageContent);
        }

        // Ensure quoted replies have bad words removed from them
        messageContent = this.Get<IBadWordReplace>().Replace(messageContent);

        // Remove HIDDEN Text
        messageContent = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(messageContent);

        // Quote the original message
        this.Input.Editor +=
            $"[quote={this.Get<IUserDisplayName>().GetNameById(message.UserID.ToType<int>())};{message.ID}]{messageContent}[/quote]\r\n"
                .TrimStart();
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
    private void UpdateWatchTopic(int userId, int topicId)
    {
        var topicWatchedId = this.GetRepository<WatchTopic>().Check(userId, topicId);

        if (topicWatchedId.HasValue && !this.Input.TopicWatch)
        {
            // unsubscribe...
            this.GetRepository<WatchTopic>().DeleteById(topicWatchedId.Value);
        }
        else if (!topicWatchedId.HasValue && this.Input.TopicWatch)
        {
            // subscribe to this topic...
            this.GetRepository<WatchTopic>().Add(userId, topicId);
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