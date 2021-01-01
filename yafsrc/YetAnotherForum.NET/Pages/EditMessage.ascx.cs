/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Linq;
    using System.Web;
    
    using YAF.Core.BaseModules;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Core.Utilities.Helpers;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Web.Extensions;

    using DateTime = System.DateTime;
    using ListItem = System.Web.UI.WebControls.ListItem;

    #endregion

    /// <summary>
    /// The Edit message Page.
    /// </summary>
    public partial class EditMessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The forum editor.
        /// </summary>
        private ForumEditor forumEditor;

        /// <summary>
        ///   The owner user id.
        /// </summary>
        private int ownerUserId;

        /// <summary>
        /// The edit or quoted message.
        /// </summary>
        private Tuple<Topic, Message, User, Forum> editedMessage;

        /// <summary>
        ///   The forum.
        /// </summary>
        private Topic topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditMessage"/> class.
        /// </summary>
        public EditMessage()
            : base("POSTMESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets EditMessageID.
        /// </summary>
        protected int? EditMessageId => this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAsInt("m");

        /// <summary>
        ///   Gets or sets the PollId if the topic has a poll attached
        /// </summary>
        protected int? PollId { get; set; }

        #endregion

        #region Methods

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
                "t={0}&name={1}",
                this.PageContext.PageTopicID,
                this.PageContext.PageTopicName);
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
            var postedMessage = this.forumEditor.Text.Trim();

            if (postedMessage.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEMPTY"), MessageTypes.warning);
                return false;
            }

            // No need to check whitespace if they are actually posting something
            if (this.PageContext.BoardSettings.MaxPostSize > 0
                && this.forumEditor.Text.Length >= this.PageContext.BoardSettings.MaxPostSize)
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.warning);
                return false;
            }

            // Check if the Entered Guest Username is not too long
            if (this.FromRow.Visible && this.From.Text.Trim().Length > 100)
            {
                this.PageContext.AddLoadMessage(this.GetText("GUEST_NAME_TOOLONG"), MessageTypes.warning);

                this.From.Text = this.From.Text.Substring(100);
                return false;
            }

            if (this.SubjectRow.Visible && this.TopicSubjectTextBox.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.PageContext.BoardSettings.AllowCreateTopicsSameName)
                && this.GetRepository<Topic>().CheckForDuplicate(this.TopicSubjectTextBox.Text.Trim())
                && !this.EditMessageId.HasValue)
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
                return false;
            }

            if ((!this.PageContext.IsGuest || !this.PageContext.BoardSettings.EnableCaptchaForGuests)
                && (!this.PageContext.BoardSettings.EnableCaptchaForPost || this.PageContext.User.UserFlags.IsCaptchaExcluded)
                || CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                return true;
            }

            this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            if (this.PageContext.ForumUploadAccess)
            {
                this.PageContext.PageElements.AddScriptReference("FileUploadScript");

#if DEBUG
                this.PageContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.css");
#else
                this.PageContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.min.css");
#endif
            }

            this.forumEditor = ForumEditorHelper.GetCurrentForumEditor();
            this.forumEditor.MaxCharacters = this.PageContext.BoardSettings.MaxPostSize;

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
            this.PageContext.PageElements.RegisterJsBlock(
                "GetBoardTagsJs",
                JavaScriptBlocks.GetBoardTagsJs(this.Tags.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.PageContext.PageForumID == 0)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                                                         && !this.PageContext.ForumPostAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
            }

            this.topic = this.GetRepository<Topic>().GetById(this.PageContext.PageTopicID);

            if (this.EditMessageId.HasValue)
            {
                var editMessage = this.GetRepository<Message>().GetMessage(this.EditMessageId.Value);

                if (editMessage != null)
                {
                    this.ownerUserId = editMessage.Item1.UserID;

                    if (!this.CanEditPostCheck(editMessage.Item2, this.topic))
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }
                }

                this.editedMessage = editMessage;

                // we edit message and should transfer both the message ID and TopicID for PageLinks.
                this.PollList.EditMessageId = this.EditMessageId.Value;
            }

            this.HandleUploadControls();

            if (!this.IsPostBack)
            {
                var normal = new ListItem(this.GetText("normal"), "0");

                normal.Attributes.Add(
                    "data-content",
                    $"<span class='select2-image-select-icon'><i class='far fa-comment fa-fw text-secondary'></i>&nbsp;{this.GetText("normal")}</span>");

                this.Priority.Items.Add(normal);

                var sticky = new ListItem(this.GetText("sticky"), "1");

                sticky.Attributes.Add(
                    "data-content",
                    $"<span class='select2-image-select-icon'><i class='fas fa-thumbtack fa-fw text-secondary'></i>&nbsp;{this.GetText("sticky")}</span>");

                this.Priority.Items.Add(sticky);

                var announcement = new ListItem(this.GetText("announcement"), "2");

                announcement.Attributes.Add(
                    "data-content",
                    $"<span class='select2-image-select-icon'><i class='fas fa-bullhorn fa-fw text-secondary'></i>&nbsp;{this.GetText("announcement")}</span>");

                this.Priority.Items.Add(announcement);

                this.Priority.SelectedIndex = 0;

                // Allow the Styling of Topic Titles only for Mods or Admins
                if (this.PageContext.BoardSettings.UseStyledTopicTitles
                    && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
                {
                    this.StyleRow.Visible = true;
                }
                else
                {
                    this.StyleRow.Visible = false;
                }

                this.EditReasonRow.Visible = false;

                this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;

                // update options...
                this.PostOptions1.Visible = !this.PageContext.IsGuest;
                this.PostOptions1.PersistentOptionVisible =
                    this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess;
                this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
                this.PostOptions1.PollOptionVisible = false;

                if (!this.PageContext.IsGuest)
                {
                    this.PostOptions1.WatchChecked = this.PageContext.PageTopicID > 0
                        ? this.GetRepository<WatchTopic>().Check(this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue
                        : this.PageContext.User.AutoWatchTopics;
                }

                if (this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests
                    || this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.User.UserFlags.IsCaptchaExcluded)
                {
                    this.imgCaptcha.ImageUrl = $"{BoardInfo.ForumClientFileRoot}resource.ashx?c=1";
                    this.tr_captcha1.Visible = true;
                    this.tr_captcha2.Visible = true;
                }

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddRoot();
                    this.PageLinks.AddCategory(this.PageContext.PageCategoryName, this.PageContext.PageCategoryID);
                }

                this.PageLinks.AddForum(this.PageContext.PageForumID);

                // editing a message...
                this.InitEditedPost(this.editedMessage.Item2);
                this.PollList.EditMessageId = this.EditMessageId.Value;

                // form user is only for "Guest"
                if (this.PageContext.IsGuest)
                {
                    this.From.Text = this.PageContext.User.DisplayOrUserName();
                    this.FromRow.Visible = false;
                }
            }

            // Set Poll
            this.PollId = this.topic.PollID;
            this.PollList.TopicId = this.PageContext.PageTopicID;
            this.PollList.PollId = this.PollId;
        }

        /// <summary>
        /// The post reply handle edit post.
        /// </summary>
        /// <returns>
        /// Returns the Message Id
        /// </returns>
        protected Message PostReplyHandleEditPost()
        {
            if (!this.PageContext.ForumEditAccess)
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
            var messageFlags = new MessageFlags(this.editedMessage.Item2.Flags)
            {
                IsHtml =
                    this
                    .forumEditor
                    .UsesHTML,
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

            this.editedMessage.Item2.Flags = messageFlags.BitValue;

            var isModeratorChanged = this.PageContext.PageUserID != this.ownerUserId;

            this.GetRepository<Message>().Update(
                this.Priority.SelectedValue.ToType<short>(),
                this.forumEditor.Text.Trim(),
                descriptionSave.Trim(),
                string.Empty,
                stylesSave.Trim(),
                subjectSave.Trim(),
                this.HtmlEncode(this.ReasonEditor.Text),
                isModeratorChanged,
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                this.editedMessage,
                this.PageContext.PageUserID);

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            // remove cache if it exists...
            this.Get<IDataCache>()
                .Remove(string.Format(Constants.Cache.FirstPostCleaned, this.PageContext.PageBoardID, this.PageContext.PageTopicID));

            return this.editedMessage.Item2;
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
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess
                && !this.PageContext.BoardSettings.SpamServiceType.Equals(0))
            {
                // Check content for spam
                if (
                    this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.User.DisplayOrUserName(),
                        this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.forumEditor.Text)))
                            .RemoveMultipleWhitespace(),
                        this.PageContext.IsGuest ? null : this.PageContext.MembershipUser.Email,
                        out var spamResult))
                {
                    var description =
                        $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.User.DisplayOrUserName())}";

                    switch (this.PageContext.BoardSettings.SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                description);
                            break;
                        case 1:
                            isPossibleSpamMessage = true;
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"{description}, it was flagged as unapproved post.");
                            break;
                        case 2:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"{description}, post was rejected");
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case 3:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"{description}, user was deleted and banned");

                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.MembershipUser,
                                this.PageContext.User.IP);

                            return;
                    }
                }
            }

            if (this.Get<ISpamCheck>().ContainsSpamUrls(this.forumEditor.Text))
            {
                return;
            }

            // update the last post time...
            this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            // Edit existing post
            var editMessage = this.PostReplyHandleEditPost();

            // Check if message is approved
            var isApproved = editMessage.MessageFlags.IsApproved;

            var messageId = editMessage.ID;

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            var attachPollParameter = string.Empty;
            var returnForum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible)
            {
                // new topic poll token
                attachPollParameter = $"&t={this.PageContext.PageTopicID}";

                // new return forum poll token
                returnForum = $"&f={this.PageContext.PageForumID}";
            }

            // Create notification emails
            if (isApproved)
            {
                if (attachPollParameter.IsNotSet() || !this.PostOptions1.PollChecked)
                {
                    // regular redirect...
                    this.Get<LinkBuilder>().Redirect(ForumPages.Posts, "m={0}&name={1}#post{0}", messageId, this.PageContext.PageTopicName);
                }
                else
                {
                    // poll edit redirect...
                    this.Get<LinkBuilder>().Redirect(ForumPages.PollEdit, "{0}", attachPollParameter);
                }
            }
            else
            {
                // Not Approved
                if (this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost)
                {
                    // not approved, notify moderators
                    this.Get<ISendNotification>()
                        .ToModeratorsThatMessageNeedsApproval(
                            this.PageContext.PageForumID,
                            messageId.ToType<int>(),
                            isPossibleSpamMessage);
                }

                // 't' variable is required only for poll and this is a attach poll token for attachments page
                if (!this.PostOptions1.PollChecked)
                {
                    attachPollParameter = string.Empty;
                }

                // Tell user that his message will have to be approved by a moderator
                var url = this.Get<LinkBuilder>().GetForumLink(this.PageContext.PageForumID, this.PageContext.PageForumName);

                if (this.PageContext.PageTopicID > 0 && this.topic.NumPosts > 1)
                {
                    url = this.Get<LinkBuilder>().GetTopicLink(this.PageContext.PageTopicID, this.PageContext.PageTopicName);
                }

                if (attachPollParameter.Length <= 0)
                {
                    this.Get<LinkBuilder>().Redirect(ForumPages.Info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
                else
                {
                    this.Get<LinkBuilder>().Redirect(ForumPages.PollEdit, "&ra=1{0}{1}", attachPollParameter, returnForum);
                }
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
        private bool CanEditPostCheck([NotNull] Message message, Topic topicInfo)
        {
            var postLocked = false;

            if (!this.PageContext.IsAdmin && this.PageContext.BoardSettings.LockPosts > 0)
            {
                var edited = message.Edited.Value;

                if (edited.AddDays(this.PageContext.BoardSettings.LockPosts) < DateTime.UtcNow)
                {
                    postLocked = true;
                }
            }

            // get  forum information
            var forumInfo = this.GetRepository<Forum>().GetById(this.PageContext.PageForumID);

            // Ederon : 9/9/2007 - moderator can edit in locked topics
            return !postLocked && !forumInfo.ForumFlags.IsLocked
                               && !topicInfo.TopicFlags.IsLocked
                               && message.UserID == this.PageContext.PageUserID
                    || this.PageContext.ForumModeratorAccess && this.PageContext.ForumEditAccess;
        }

        /// <summary>
        /// The initializes the edited post.
        /// </summary>
        /// <param name="currentMessage">
        /// The current message.
        /// </param>
        private void InitEditedPost([NotNull] Message currentMessage)
        {
            /*if (this.forumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                currentMessage.Message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(currentMessage.Message);
            }*/

            if (this.forumEditor.UsesBBCode && currentMessage.MessageFlags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                currentMessage.MessageText = this.Get<IBBCode>().ConvertHtmlToBBCodeForEdit(currentMessage.MessageText);
            }

            this.forumEditor.Text = currentMessage.MessageText;

            if (this.forumEditor.UsesHTML && currentMessage.MessageFlags.IsBBCode)
            {
                this.forumEditor.Text = this.Get<IBBCode>().FormatMessageWithCustomBBCode(
                    this.forumEditor.Text,
                    currentMessage.MessageFlags,
                    currentMessage.UserID,
                    currentMessage.ID);
            }

            this.Title.Text = this.GetText("EDIT");
            this.PostReply.TextLocalizedTag = "SAVE";
            this.PostReply.TextLocalizedPage = "COMMON";

            // add topic link...
            this.PageLinks.AddTopic(this.topic.TopicName, this.PageContext.PageTopicID);

            // editing..
            this.PageLinks.AddLink(this.GetText("EDIT"));

            this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(this.PageContext.PageTopicName);
            this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(this.topic.Description);

            if (this.topic.UserID == currentMessage.UserID.ToType<int>()
                || this.PageContext.ForumModeratorAccess)
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
            if (this.PageContext.BoardSettings.UseStyledTopicTitles
                && (this.PageContext.ForumModeratorAccess || this.PageContext.IsAdmin))
            {
                this.StyleRow.Visible = true;
            }
            else
            {
                this.StyleRow.Visible = false;
                this.TopicStylesTextBox.Enabled = false;
            }

            this.TopicStylesTextBox.Text = this.topic.Styles;

            this.Priority.SelectedItem.Selected = false;
            this.Priority.Items.FindByValue(this.topic.Priority.ToString()).Selected = true;

            this.EditReasonRow.Visible = true;
            this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
            this.PostOptions1.PersistentChecked = currentMessage.MessageFlags.IsPersistent;

            var topicsList = this.GetRepository<TopicTag>().List(this.PageContext.PageTopicID);

            if (topicsList.Any())
            {
                this.Tags.Text = topicsList.Select(t => t.Item2.TagName).ToDelimitedString(",");
            }

            // Ederon : 9/9/2007 - moderators can reply in locked topics
            if (this.topic.TopicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
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
            this.forumEditor.UserCanUpload = this.PageContext.ForumUploadAccess;
            this.UploadDialog.Visible = this.PageContext.ForumUploadAccess;
        }

#endregion
    }
}