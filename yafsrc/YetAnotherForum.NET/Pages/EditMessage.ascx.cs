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

using YAF.Types.Models;

namespace YAF.Pages;

/// <summary>
/// The Edit message Page.
/// </summary>
public partial class EditMessage : ForumPage
{
    /// <summary>
    ///   The forum editor.
    /// </summary>
    private ForumEditor forumEditor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditMessage"/> class.
    /// </summary>
    public EditMessage()
        : base("POSTMESSAGE", ForumPages.EditMessage)
    {
    }

    /// <summary>
    ///   Gets or sets the PollId if the topic has a poll attached
    /// </summary>
    protected int? PollId { get; set; }

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
            new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });
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

        if (this.TopicSubjectTextBox.Text.IsNotSet())
        {
            this.PageBoardContext.Notify(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
            return false;
        }

        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.AllowCreateTopicsSameName)
            && this.GetRepository<Topic>().CheckForDuplicate(this.TopicSubjectTextBox.Text.Trim())
            && this.PageBoardContext.PageMessage == null)
        {
            this.PageBoardContext.Notify(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
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
    /// Registers the java scripts
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        // setup jQuery and Jquery Ui Tabs.
        this.PageBoardContext.PageElements.RegisterJsBlock(
            nameof(JavaScriptBlocks.GetBoardTagsJs),
            JavaScriptBlocks.GetBoardTagsJs("Tags", this.TagsValue.ClientID));

        base.OnPreRender(e);
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

        if (this.Get<HttpRequestBase>()["m"] == null)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (!this.PageBoardContext.ForumPostAccess && !this.PageBoardContext.ForumReplyAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        if (!this.CanEditPostCheck(this.PageBoardContext.PageMessage, this.PageBoardContext.PageTopic))
        {
            this.Get<LinkBuilder>().AccessDenied();
        }
        
        // we edit message and should transfer both the message ID and TopicID for PageLinks.
        this.PollList.EditMessageId = this.PageBoardContext.PageMessage.ID;

        this.HandleUploadControls();

        if (!this.IsPostBack)
        {
            var normal = new ListItem(this.GetText("normal"), "0");

            normal.Attributes.Add(
                "data-content",
                $"<span class='select2-image-select-icon'><i class='far fa-comment fa-fw text-secondary me-1'></i>{this.GetText("normal")}</span>");

            this.Priority.Items.Add(normal);

            var sticky = new ListItem(this.GetText("sticky"), "1");

            sticky.Attributes.Add(
                "data-content",
                $"<span class='select2-image-select-icon'><i class='fas fa-thumbtack fa-fw text-secondary me-1'></i>{this.GetText("sticky")}</span>");

            this.Priority.Items.Add(sticky);

            var announcement = new ListItem(this.GetText("announcement"), "2");

            announcement.Attributes.Add(
                "data-content",
                $"<span class='select2-image-select-icon'><i class='fas fa-bullhorn fa-fw text-secondary me-1'></i>{this.GetText("announcement")}</span>");

            this.Priority.Items.Add(announcement);

            this.Priority.SelectedIndex = 0;

            // Allow the Styling of Topic Titles only for Mods or Admins
            if (this.PageBoardContext.BoardSettings.UseStyledTopicTitles
                && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
            {
                this.StyleRow.Visible = true;
            }
            else
            {
                this.StyleRow.Visible = false;
            }

            this.PriorityRow.Visible = this.PageBoardContext.ForumPriorityAccess;

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
            }

            this.PageBoardContext.PageLinks.AddRoot();
            this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

            this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

            // editing a message...
            this.InitEditedPost(this.PageBoardContext.PageMessage);
            this.PollList.EditMessageId = this.PageBoardContext.PageMessage.ID;

            // form user is only for "Guest"
            if (this.PageBoardContext.IsGuest)
            {
                this.From.Text = this.PageBoardContext.PageUser.DisplayOrUserName();
                this.FromRow.Visible = true;
            }
        }

        // Set Poll
        this.PollId = this.PageBoardContext.PageTopic.PollID;
        this.PollList.TopicId = this.PageBoardContext.PageTopicID;
        this.PollList.PollId = this.PollId;
    }

    /// <summary>
    /// The post reply handle edit post.
    /// </summary>
    /// <returns>
    /// Returns the Message Id
    /// </returns>
    private void PostReplyHandleEditPost()
    {
        if (!this.PageBoardContext.ForumEditAccess)
        {
            this.Get<LinkBuilder>().AccessDenied();
        }

        var subjectSave = string.Empty;
        var descriptionSave = string.Empty;
        var stylesSave = string.Empty;

        if (this.TopicSubjectTextBox.Enabled)
        {
            subjectSave = this.TopicSubjectTextBox.Text;
        }

        if (this.TopicDescriptionTextBox.Enabled)
        {
            descriptionSave = this.TopicDescriptionTextBox.Text;
        }

        if (this.TopicStylesTextBox.Enabled)
        {
            stylesSave = this.TopicStylesTextBox.Text;
        }

        // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
        // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
        // retrieve message flags
        var messageFlags = new MessageFlags(this.PageBoardContext.PageMessage.Flags)
                               {
                                   IsHtml =
                                       false,
                                   IsBBCode
                                       =
                                       this
                                           .forumEditor
                                           .UsesBBCode,
                                   IsPersistent
                                       =
                                       this
                                           .PostOptions1
                                           .PersistentChecked
                               };

        this.PageBoardContext.PageMessage.Flags = messageFlags.BitValue;

        var isModeratorChanged = this.PageBoardContext.PageUserID != this.PageBoardContext.PageMessage.UserID;

        var messageUser = this.GetRepository<User>().GetById(this.PageBoardContext.PageMessage.UserID);

        this.GetRepository<Message>().Update(
            this.Priority.SelectedValue.ToType<short>(),
            HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text)),
            descriptionSave.Trim(),
            string.Empty,
            stylesSave.Trim(),
            subjectSave.Trim(),
            this.HtmlEncode(this.ReasonEditor.Text),
            isModeratorChanged,
            this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess,
            this.PageBoardContext.PageTopic,
            this.PageBoardContext.PageMessage,
            this.PageBoardContext.PageForum,
            messageUser,
            this.PageBoardContext.PageUserID);

        // Update Topic Tags?!
        this.GetRepository<TopicTag>().Delete(x => x.TopicID == this.PageBoardContext.PageTopicID);

        this.GetRepository<TopicTag>().AddTagsToTopic(this.TagsValue.Value, this.PageBoardContext.PageTopicID);

        this.UpdateWatchTopic(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);
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

        var isPossibleSpamMessage = false;

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
                           posted by User: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}";

                switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                {
                    case SpamPostHandling.DoNothing:
                        this.Logger.SpamMessageDetected(
                            this.PageBoardContext.PageUserID,
                            description);
                        break;
                    case SpamPostHandling.FlagMessageUnapproved:
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

        // Edit existing post
        this.PostReplyHandleEditPost();

        // Check if message is approved
        var isApproved = this.PageBoardContext.PageMessage.MessageFlags.IsApproved;

        var messageId = this.PageBoardContext.PageMessage.ID;

        // Create notification emails
        if (isApproved)
        {
            // regular redirect...
            this.Get<LinkBuilder>().Redirect(ForumPages.Posts, new { m = messageId, name = this.PageBoardContext.PageTopic.TopicName });
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
                        messageId.ToType<int>(),
                        isPossibleSpamMessage);
            }

            // Tell user that his message will have to be approved by a moderator
            var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForum);

            if (this.PageBoardContext.PageTopicID > 0 && this.PageBoardContext.PageTopic.NumPosts > 1)
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
                                                       IsHtml = false,
                                                       IsBBCode = this.forumEditor.UsesBBCode
                                                   };

        this.PreviewMessagePost.MessageID = this.PageBoardContext.PageMessage.ID;

        this.PreviewMessagePost.Message = this.forumEditor.Text;
    }

    /// <summary>
    /// The can edit post check.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="topicInfo">
    /// The topic Info.
    /// </param>
    /// <returns>
    /// Returns if user can edit post check.
    /// </returns>
    private bool CanEditPostCheck([NotNull] Message message, [NotNull] Topic topicInfo)
    {
        var postLocked = false;

        if (!this.PageBoardContext.IsAdmin && this.PageBoardContext.BoardSettings.LockPosts > 0)
        {
            var edited = message.Edited ?? message.Posted;

            if (edited.AddDays(this.PageBoardContext.BoardSettings.LockPosts) < DateTime.UtcNow)
            {
                postLocked = true;
            }
        }

        // get  forum information
        var forumInfo = this.PageBoardContext.PageForum;

        // Ederon : 9/9/2007 - moderator can edit in locked topics
        return !postLocked && !forumInfo.ForumFlags.IsLocked
                           && !topicInfo.TopicFlags.IsLocked
                           && message.UserID == this.PageBoardContext.PageUserID
               || this.PageBoardContext.ForumModeratorAccess && this.PageBoardContext.ForumEditAccess;
    }

    /// <summary>
    /// The initializes the edited post.
    /// </summary>
    /// <param name="currentMessage">
    /// The current message.
    /// </param>
    private void InitEditedPost([NotNull] Message currentMessage)
    {
        if (this.forumEditor.UsesBBCode && currentMessage.MessageFlags.IsHtml)
        {
            // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
            currentMessage.MessageText = this.Get<IBBCode>().ConvertHtmlToBBCodeForEdit(currentMessage.MessageText);
        }

        this.forumEditor.Text = BBCodeHelper.DecodeCodeBlocks(currentMessage.MessageText);

        this.Title.Text = this.GetText("EDIT");
        this.PostReply.TextLocalizedTag = "SAVE";
        this.PostReply.TextLocalizedPage = "COMMON";

        // add topic link...
        this.PageBoardContext.PageLinks.AddTopic(this.PageBoardContext.PageTopic);

        // editing..
        this.PageBoardContext.PageLinks.AddLink(this.GetText("EDIT"));

        this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(this.PageBoardContext.PageTopic.TopicName);
        this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(this.PageBoardContext.PageTopic.Description);

        if (this.PageBoardContext.PageTopic.UserID == currentMessage.UserID
            || this.PageBoardContext.ForumModeratorAccess)
        {
            // allow editing of the topic subject
            this.TopicSubjectTextBox.Enabled = true;
        }
        else
        {
            this.TopicSubjectTextBox.Enabled = false;
            this.TopicDescriptionTextBox.Enabled = false;
        }

        // Allow the Styling of Topic Titles only for Mods or Admins
        if (this.PageBoardContext.BoardSettings.UseStyledTopicTitles
            && (this.PageBoardContext.ForumModeratorAccess || this.PageBoardContext.IsAdmin))
        {
            this.StyleRow.Visible = true;
        }
        else
        {
            this.StyleRow.Visible = false;
            this.TopicStylesTextBox.Enabled = false;
        }

        this.TopicStylesTextBox.Text = this.PageBoardContext.PageTopic.Styles;

        this.Priority.SelectedItem.Selected = false;
        this.Priority.Items.FindByValue(this.PageBoardContext.PageTopic.Priority.ToString()).Selected = true;

        this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
        this.PostOptions1.PersistentChecked = currentMessage.MessageFlags.IsPersistent;

        var topicsList = this.GetRepository<TopicTag>().List(this.PageBoardContext.PageTopicID);

        if (topicsList.Any())
        {
            this.TagsValue.Value = topicsList.Select(t => t.Item2.TagName).ToDelimitedString(",");
        }

        // Ederon : 9/9/2007 - moderators can reply in locked topics
        if (this.PageBoardContext.PageTopic.TopicFlags.IsLocked && !this.PageBoardContext.ForumModeratorAccess)
        {
            var urlReferrer = this.Get<HttpRequestBase>().UrlReferrer;

            if (urlReferrer != null)
            {
                this.Get<HttpResponseBase>().Redirect(urlReferrer.ToString());
            }
        }

        this.HandleUploadControls();
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
    /// Handles the upload controls.
    /// </summary>
    private void HandleUploadControls()
    {
        this.forumEditor.UserCanUpload = this.PageBoardContext.UploadAccess;
        this.UploadDialog.Visible = this.PageBoardContext.UploadAccess;
    }
}