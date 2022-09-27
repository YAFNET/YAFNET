/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Types.Objects;
using YAF.Types.Models;

/// <summary>
/// The post message Page.
/// </summary>
public partial class PostMessage : ForumPage
{
    /// <summary>
    ///   The forum editor.
    /// </summary>
    private ForumEditor forumEditor;

    /// <summary>
    ///   The Spam Approved Indicator
    /// </summary>
    private bool spamApproved = true;

    /// <summary>
    /// The edit or quoted message.
    /// </summary>
    private Message quotedMessage;

    /// <summary>
    ///   The forum.
    /// </summary>
    private Topic topic;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostMessage"/> class.
    /// </summary>
    public PostMessage()
        : base("POSTMESSAGE", ForumPages.PostMessage)
    {
    }

    /// <summary>
    ///   Gets or sets the PollId if the topic has a poll attached
    /// </summary>
    protected int? PollId { get; set; }

    /// <summary>
    ///   Gets Quoted Message ID.
    /// </summary>
    protected int? QuotedMessageId => this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("q");

    /// <summary>
    /// Canceling Posting New Message Or editing Message.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        // reply to existing topic or editing of existing topic
        this.Get<LinkBuilder>().Redirect(
            ForumPages.Posts,
            new {t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });
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

        // see if they've past that delay point
        if (this.Get<ISession>().LastPost
            <= DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
        {
            return false;
        }

        this.PageBoardContext.Notify(
            this.GetTextFormatted(
                "wait",
                (this.Get<ISession>().LastPost
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
        var postedMessage = HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text.Trim()));

        if (postedMessage.IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("ISEMPTY"), MessageTypes.warning);
            return false;
        }

        // No need to check whitespace if they are actually posting something
        if (this.PageBoardContext.BoardSettings.MaxPostSize > 0
            && this.forumEditor.Text.Length >= this.PageBoardContext.BoardSettings.MaxPostSize)
        {
            this.PageBoardContext.Notify(this.GetText("ISEXCEEDED"), MessageTypes.warning);
            return false;
        }

        // Check if the Entered Guest Username is not too long
        if (this.FromRow.Visible && this.From.Text.Trim().Length > 100)
        {
            this.PageBoardContext.Notify(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.warning);

            this.From.Text = this.From.Text.Substring(100);
            return false;
        }

        if ((!this.PageBoardContext.IsGuest || !this.PageBoardContext.BoardSettings.EnableCaptchaForGuests)
            && (!this.PageBoardContext.BoardSettings.EnableCaptchaForPost || this.PageBoardContext.PageUser.UserFlags.IsCaptchaExcluded)
            || CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
        {
            return true;
        }

        this.PageBoardContext.Notify(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
        return false;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnInit([NotNull] EventArgs e)
    {
        if (this.PageBoardContext.UploadAccess)
        {
            this.PageBoardContext.PageElements.AddScriptReference("FileUploadScript");

#if DEBUG
            this.PageBoardContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.css");
#else
                this.PageBoardContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.min.css");
#endif
        }

        this.forumEditor = ForumEditorHelper.GetCurrentForumEditor();
        this.forumEditor.MaxCharacters = this.PageBoardContext.BoardSettings.MaxPostSize;

        this.EditorLine.Controls.Add(this.forumEditor);

        base.OnInit(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.PageBoardContext.PageForumID == 0)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                                                     && !this.PageBoardContext.ForumPostAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (this.Get<HttpRequestBase>()["t"] != null && !this.PageBoardContext.ForumReplyAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        this.topic = this.PageBoardContext.PageTopic;

        // we reply to a post with a quote
        if (this.QuotedMessageId.HasValue)
        {
            this.quotedMessage =
                this.GetRepository<Message>().GetMessage(this.QuotedMessageId.Value);

            if (this.quotedMessage != null)
            {
                if (this.Get<HttpRequestBase>().QueryString.Exists("text"))
                {
                    var quotedMessageText =
                        this.Server.UrlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("text"));

                    this.quotedMessage.MessageText =
                        HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(HtmlHelper.CleanHtmlString(quotedMessageText)));
                }

                if (this.quotedMessage.TopicID != this.PageBoardContext.PageTopicID)
                {
                    this.Get<LinkBuilder>().AccessDenied();
                }

                if (!this.CanQuotePostCheck(this.topic))
                {
                    this.Get<LinkBuilder>().AccessDenied();
                }
            }
        }

        this.HandleUploadControls();

        this.LastPosts1.TopicID = this.PageBoardContext.PageTopicID;

        if (!this.IsPostBack)
        {
            // update options...
            this.PostOptions1.Visible = !this.PageBoardContext.IsGuest;
            this.PostOptions1.PersistentOptionVisible =
                this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess;
            this.PostOptions1.WatchOptionVisible = !this.PageBoardContext.IsGuest;
            this.PostOptions1.PollOptionVisible = false;

            if (!this.PageBoardContext.IsGuest)
            {
                this.PostOptions1.WatchChecked = this.PageBoardContext.PageTopicID > 0
                                                     ? this.GetRepository<WatchTopic>().Check(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID).HasValue
                                                     : this.PageBoardContext.PageUser.AutoWatchTopics;
            }

            if (this.PageBoardContext.IsGuest && this.PageBoardContext.BoardSettings.EnableCaptchaForGuests
                || this.PageBoardContext.BoardSettings.EnableCaptchaForPost && !this.PageBoardContext.PageUser.UserFlags.IsCaptchaExcluded)
            {
                this.imgCaptcha.ImageUrl = CaptchaHelper.GetCaptcha();
                this.tr_captcha1.Visible = true;
                this.tr_captcha2.Visible = true;
            }

            this.PageBoardContext.PageLinks.AddRoot();
            this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

            this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

            // check if it's a reply to a topic...
            this.InitReplyToTopic();

            if (this.quotedMessage != null)
            {
                if (this.QuotedMessageId.HasValue)
                {
                    if (this.Get<ISession>().MultiQuoteIds != null)
                    {
                        var quoteId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("q");
                        var multiQuote = new MultiQuote { MessageID = quoteId, TopicID = this.PageBoardContext.PageTopicID };

                        if (
                            !this.Get<ISession>()
                                .MultiQuoteIds.Any(m => m.MessageID.Equals(quoteId)))
                        {
                            this.Get<ISession>()
                                .MultiQuoteIds.Add(
                                    multiQuote);
                        }

                        var messages = this.GetRepository<Message>().GetByIds(
                            this.Get<ISession>().MultiQuoteIds.Select(i => i.MessageID));

                        messages.ForEach(this.InitQuotedReply);

                        // Clear Multi-quotes
                        this.Get<ISession>().MultiQuoteIds = null;
                    }
                    else
                    {
                        this.InitQuotedReply(this.quotedMessage);
                    }
                }
            }

            // form user is only for "Guest"
            if (this.PageBoardContext.IsGuest)
            {
                this.From.Text = this.PageBoardContext.PageUser.DisplayOrUserName();
                this.FromRow.Visible = false;
            }
        }

        // Set Poll
        this.PollId = this.topic.PollID;
        this.PollList.TopicId = this.PageBoardContext.PageTopicID;
        this.PollList.PollId = this.PollId;
    }

    /// <summary>
    /// The post reply handle reply to topic.
    /// </summary>
    /// <param name="isSpamApproved">
    /// The is Spam Approved.
    /// </param>
    /// <returns>
    /// Returns the new Message.
    /// </returns>
    protected Message PostReplyHandleReplyToTopic(bool isSpamApproved)
    {
        if (!this.PageBoardContext.ForumReplyAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

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

        var replyTo = this.QuotedMessageId;

        // make message flags
        var messageFlags = new MessageFlags
                               {
                                   IsHtml = this.forumEditor.UsesHTML,
                                   IsBBCode = this.forumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistentChecked,
                                   IsApproved = isSpamApproved
                               };

        var message = this.GetRepository<Message>().SaveNew(
            this.PageBoardContext.PageForum,
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageUser,
            HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text)),
            this.User != null ? null : this.From.Text,
            this.Get<HttpRequestBase>().GetUserRealIPAddress(),
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
    /// <param name="sender">
    /// The Sender Object.
    /// </param>
    /// <param name="e">
    /// The Event Arguments.
    /// </param>
    protected void PostReply_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (!this.IsPostReplyVerified())
        {
            return;
        }

        if (this.IsPostReplyDelay())
        {
            return;
        }

        var isPossibleSpamMessage = false;

        var message = HtmlHelper.StripHtml(this.forumEditor.Text);

        // Check for SPAM
        if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
        {
            // Check content for spam
            if (
                this.Get<ISpamCheck>().CheckPostForSpam(
                    this.PageBoardContext.IsGuest ? this.From.Text : this.PageBoardContext.PageUser.DisplayOrUserName(),
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.forumEditor.Text)))
                        .RemoveMultipleWhitespace(),
                    this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                    out var spamResult))
            {
                var description =
                    $@"Spam Check detected possible SPAM ({spamResult}) Original message: [{this.forumEditor.Text}]
                           posted by PageUser: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}";

                switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                {
                    case SpamPostHandling.DoNothing:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            description);
                        break;
                    case SpamPostHandling.FlagMessageUnapproved:
                        this.spamApproved = false;
                        isPossibleSpamMessage = true;
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, it was flagged as unapproved post.");
                        break;
                    case SpamPostHandling.RejectMessage:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, post was rejected");
                        this.PageBoardContext.Notify(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                        return;
                    case SpamPostHandling.DeleteBanUser:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            $"{description}, user was deleted and banned");

                        this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                            this.PageBoardContext.PageUserID,
                            this.PageBoardContext.MembershipUser,
                            this.PageBoardContext.PageUser.IP);

                        return;
                }
            }
        }

        // update the last post time...
        this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

        // Reply to topic
        var newMessage = this.PostReplyHandleReplyToTopic(this.spamApproved);

        var isApproved = newMessage.MessageFlags.IsApproved;

        // Create notification emails
        if (isApproved)
        {
            this.Get<ISendNotification>().ToWatchingUsers(newMessage);

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
                    Config.IsDotNetNuke ? this.PageBoardContext.PageForumID : this.PageBoardContext.PageUserID,
                    this.PageBoardContext.PageTopicID,
                    newMessage.ID,
                    this.PageBoardContext.PageTopic.TopicName,
                    this.forumEditor.Text);
            }

            this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { m = newMessage.ID, name = this.PageBoardContext.PageTopic.TopicName });
        }
        else
        {
            // Not Approved
            if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
            {
                // not approved, notify moderators
                this.Get<ISendNotification>()
                    .ToModeratorsThatMessageNeedsApproval(
                        this.PageBoardContext.PageForumID,
                        newMessage.ID,
                        isPossibleSpamMessage);
            }

            // Tell user that his message will have to be approved by a moderator
            var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

            if (this.PageBoardContext.PageTopicID > 0 && this.topic.NumPosts > 1)
            {
                url = this.Get<LinkBuilder>().GetTopicLink(this.PageBoardContext.PageTopicID, this.PageBoardContext.PageTopic.TopicName);
            }

            this.Get<LinkBuilder>().Redirect(ForumPages.Info, new { i = 1, url = this.Server.UrlEncode(url) });
        }
    }

    /// <summary>
    /// Previews the new Message
    /// </summary>
    /// <param name="sender">
    /// The Sender Object.
    /// </param>
    /// <param name="e">
    /// The Event Arguments.
    /// </param>
    protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.PreviewRow.Visible = true;

        this.PreviewMessagePost.MessageFlags = new MessageFlags
                                                   {
                                                       IsHtml = this.forumEditor.UsesHTML,
                                                       IsBBCode = this.forumEditor.UsesBBCode
                                                   };

        this.PreviewMessagePost.MessageID = 0;

        this.PreviewMessagePost.Message = this.forumEditor.Text;
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

        if (topicInfo == null || forumInfo == null)
        {
            return false;
        }

        // Ederon : 9/9/2007 - moderator can reply to locked topics
        return !forumInfo.ForumFlags.IsLocked
               && !topicInfo.TopicFlags.IsLocked || this.PageBoardContext.ForumModeratorAccess
               && this.PageBoardContext.ForumReplyAccess;
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

        /*if (this.forumEditor.UsesHTML && message.Flags.IsBBCode)
        {
            // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
            messageContent = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(messageContent);
        }*/

        if (this.forumEditor.UsesBBCode && message.MessageFlags.IsHtml)
        {
            // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
            messageContent = this.Get<IBBCode>().ConvertHtmlToBBCodeForEdit(messageContent);
        }

        // Ensure quoted replies have bad words removed from them
        messageContent = this.Get<IBadWordReplace>().Replace(messageContent);

        // Remove HIDDEN Text
        messageContent = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(messageContent);

        // Quote the original message
        this.forumEditor.Text +=
            $"[quote={this.Get<IUserDisplayName>().GetNameById(message.UserID.ToType<int>())};{message.ID}]{messageContent}[/quote]\r\n"
                .TrimStart();

        /*if (this.forumEditor.UsesHTML && message.Flags.IsBBCode)
        {
            // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
            this.forumEditor.Text = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(this.forumEditor.Text);

            this.forumEditor.Text = this.Get<IBBCode>().FormatMessageWithCustomBBCode(
                this.forumEditor.Text,
                message.Flags,
                message.UserID,
                message.MessageID);
        }*/
    }

    /// <summary>
    /// Initializes a reply to the current topic.
    /// </summary>
    private void InitReplyToTopic()
    {
        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (this.topic.TopicFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            var urlReferrer = this.Get<HttpRequestBase>().UrlReferrer;

            if (urlReferrer != null)
            {
                this.Get<HttpResponseBase>().Redirect(urlReferrer.ToString());
            }
        }

        // add topic link...
        this.PageBoardContext.PageLinks.AddTopic(this.topic.TopicName, this.PageBoardContext.PageTopicID);

        this.Title.Text = this.GetText("reply");

        // add "reply" text...
        this.PageBoardContext.PageLinks.AddLink(this.GetText("reply"));
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
        var topicWatchedID = this.GetRepository<WatchTopic>().Check(userId, topicId);

        if (topicWatchedID.HasValue && !this.PostOptions1.WatchChecked)
        {
            // unsubscribe...
            this.GetRepository<WatchTopic>().DeleteById(topicWatchedID.Value);
        }
        else if (!topicWatchedID.HasValue && this.PostOptions1.WatchChecked)
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

    /// <summary>
    /// Handles the upload controls.
    /// </summary>
    private void HandleUploadControls()
    {
        this.forumEditor.UserCanUpload = this.PageBoardContext.UploadAccess;
        this.UploadDialog.Visible = this.PageBoardContext.UploadAccess;
    }
}