/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
   using YAF.Web;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The post message Page.
    /// </summary>
    public partial class postmessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The _forum editor.
        /// </summary>
        protected ForumEditor ForumEditor;

        /// <summary>
        ///   The original message.
        /// </summary>
        protected string _originalMessage;

        /// <summary>
        ///   The _owner user id.
        /// </summary>
        protected int OwnerUserId;

        /// <summary>
        ///   Table with choices
        /// </summary>
        protected DataTable Choices;

        /// <summary>
        ///   The Spam Approved Indicator
        /// </summary>
        private bool spamApproved = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="postmessage" /> class.
        /// </summary>
        public postmessage()
            : base("POSTMESSAGE")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets EditMessageID.
        /// </summary>
        protected long? EditMessageId => this.PageContext.QueryIDs["m"];

        /// <summary>
        ///   Gets or sets OriginalMessage.
        /// </summary>
        protected string OriginalMessage
        {
            get => this._originalMessage;

            set => this._originalMessage = value;
        }

        /// <summary>
        ///   Gets or sets the PollGroupId if the topic has a poll attached
        /// </summary>
        protected int? PollGroupId { get; set; }

        /// <summary>
        ///   Gets Quoted Message ID.
        /// </summary>
        protected long? QuotedMessageId => this.PageContext.QueryIDs["q"];

        /// <summary>
        ///   Gets TopicID.
        /// </summary>
        protected long? TopicId => this.PageContext.QueryIDs["t"];

        #endregion

        #region Methods

        /// <summary>
        /// Canceling Posting New Message Or editing Message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.TopicId != null || this.EditMessageId != null)
            {
                // reply to existing topic or editing of existing topic
                YafBuildLink.Redirect(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
            }
            else
            {
                // new topic -- cancel back to forum
                YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.PageContext.PageForumID);
            }
        }

        /// <summary>
        /// The get poll group id.
        /// </summary>
        /// <returns>
        /// Returns the PollGroup Id
        /// </returns>
        protected int? GetPollGroupID()
        {
            return this.PollGroupId;
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
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess
                || this.PageContext.BoardSettings.PostFloodDelay <= 0)
            {
                return false;
            }

            // see if they've past that delay point
            if (this.Get<IYafSession>().LastPost
                <= DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)
                || this.EditMessageId != null)
            {
                return false;
            }

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "wait",
                    (this.Get<IYafSession>().LastPost
                     - DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds),
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
            var postedMessage = this.ForumEditor.Text.Trim();

            if (postedMessage.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("ISEMPTY"), MessageTypes.warning);
                return false;
            }

            // No need to check whitespace if they are actually posting something
            if (this.PageContext.BoardSettings.MaxPostSize > 0
                && this.ForumEditor.Text.Length >= this.PageContext.BoardSettings.MaxPostSize)
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

            // Check if the topic name is not too long
            if (this.PageContext.BoardSettings.MaxWordLength > 0
                && this.TopicSubjectTextBox.Text.Trim()
                    .AreAnyWordsOverMaxLength(this.PageContext.BoardSettings.MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_NAME_WORDTOOLONG", this.PageContext.BoardSettings.MaxWordLength),
                    MessageTypes.warning);

                try
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.PageContext.BoardSettings.MaxWordLength)
                            .Substring(255);
                }
                catch (Exception)
                {
                    this.TopicSubjectTextBox.Text =
                        this.TopicSubjectTextBox.Text.Substring(this.PageContext.BoardSettings.MaxWordLength);
                }

                return false;
            }

            // Check if the topic description words are not too long
            if (this.PageContext.BoardSettings.MaxWordLength > 0
                && this.TopicDescriptionTextBox.Text.Trim()
                    .AreAnyWordsOverMaxLength(this.PageContext.BoardSettings.MaxWordLength))
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("TOPIC_DESCRIPTION_WORDTOOLONG", this.PageContext.BoardSettings.MaxWordLength),
                    MessageTypes.warning);

                try
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.PageContext.BoardSettings.MaxWordLength)
                            .Substring(255);
                }
                catch (Exception)
                {
                    this.TopicDescriptionTextBox.Text =
                        this.TopicDescriptionTextBox.Text.Substring(this.PageContext.BoardSettings.MaxWordLength);
                }

                return false;
            }

            if (this.SubjectRow.Visible && this.TopicSubjectTextBox.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.PageContext.BoardSettings.AllowCreateTopicsSameName)
                && this.GetRepository<Topic>().CheckForDuplicateTopic(this.TopicSubjectTextBox.Text.Trim()) && this.TopicId == null
                && this.EditMessageId == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
                return false;
            }

            if ((this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests
                 || this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)
                && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The new topic.
        /// </summary>
        /// <returns>
        /// Returns if New Topic
        /// </returns>
        protected bool NewTopic()
        {
            return !(this.PollGroupId > 0);
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

            this.ForumEditor = ForumEditorHelper.GetCurrentForumEditor();

            this.EditorLine.Controls.Add(this.ForumEditor);

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q" }, false);

            TypedMessageList currentMessage = null;
            var topicInfo = this.GetRepository<Topic>().GetById(this.PageContext.PageTopicID);

            // we reply to a post with a quote
            if (this.QuotedMessageId != null)
            {
                currentMessage =
                    this.GetRepository<Message>().MessageList(this.QuotedMessageId.ToType<int>())
                        .FirstOrDefault();

                if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("text") != null)
                {
                    var quotedMessage =
                        this.Get<IBBCode>()
                            .ConvertHtmltoBBCodeForEdit(
                                this.Server.UrlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("text")));

                    currentMessage.Message =
                        HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(quotedMessage));
                }
                else if (currentMessage != null)
                {
                    if (currentMessage.TopicID.ToType<int>() != this.PageContext.PageTopicID)
                    {
                        YafBuildLink.AccessDenied();
                    }

                    if (!this.CanQuotePostCheck(topicInfo))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                                           ? 0
                                           : currentMessage.PollID;

                    // if this is a quoted message (a new reply with a quote)  we should transfer the TopicId value only to return
                    this.PollList.TopicId = this.TopicId.ToType<int>();

                    if (this.TopicId == null)
                    {
                        this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                                                    ? 0
                                                    : currentMessage.TopicID.ToType<int>();
                    }
                }
            }
            else if (this.EditMessageId != null)
            {
                currentMessage = this.GetRepository<Message>().MessageList(this.EditMessageId.ToType<int>()).FirstOrDefault();

                if (currentMessage != null)
                {
                    this.OriginalMessage = currentMessage.Message;

                    this.OwnerUserId = currentMessage.UserID.ToType<int>();

                    if (!this.CanEditPostCheck(currentMessage, topicInfo))
                    {
                        YafBuildLink.AccessDenied();
                    }

                    this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                                           ? 0
                                           : currentMessage.PollID;

                    // we edit message and should transfer both the message ID and TopicID for PageLinks.
                    this.PollList.EditMessageId = this.EditMessageId.ToType<int>();

                    if (this.TopicId == null)
                    {
                        this.PollList.TopicId = currentMessage.TopicID.ToType<int>().IsNullOrEmptyDBField()
                                                    ? 0
                                                    : currentMessage.TopicID.ToType<int>();
                    }
                }
            }

            if (this.PageContext.PageForumID == 0)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                && !this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Message.EnableRTE = PageContext.BoardSettings.AllowRichEdit;
            this.ForumEditor.BaseDir = $"{YafForumInfo.ForumClientFileRoot}Scripts";

            this.Title.Text = this.GetText("NEWTOPIC");

            if (this.PageContext.BoardSettings.MaxPostSize == 0)
            {
                this.LocalizedLblMaxNumberOfPost.Visible = false;
                this.maxCharRow.Visible = false;
            }
            else
            {
                this.LocalizedLblMaxNumberOfPost.Visible = true;
                this.LocalizedLblMaxNumberOfPost.Param0 =
                    this.PageContext.BoardSettings.MaxPostSize.ToString(CultureInfo.InvariantCulture);
                this.maxCharRow.Visible = true;
            }

            this.HandleUploadControls();

            if (!this.IsPostBack)
            {
                if (this.PageContext.BoardSettings.EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }

                // helper bool -- true if this is a completely new topic...
                var isNewTopic = this.TopicId == null && this.QuotedMessageId == null
                                  && this.EditMessageId == null;

                this.Priority.Items.Add(new ListItem(this.GetText("normal"), "0"));
                this.Priority.Items.Add(new ListItem(this.GetText("sticky"), "1"));
                this.Priority.Items.Add(new ListItem(this.GetText("announcement"), "2"));
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
                this.PostOptions1.PersistentOptionVisible = this.PageContext.ForumPriorityAccess;
                this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
                this.PostOptions1.PollOptionVisible = this.PageContext.ForumPollAccess && isNewTopic;

                if (!this.PageContext.IsGuest)
                {
                    this.PostOptions1.WatchChecked = this.PageContext.PageTopicID > 0
                        ? this.GetRepository<WatchTopic>().Check(this.PageContext.PageUserID, this.PageContext.PageTopicID).HasValue
                        : new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
                }

                if (this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests
                    || this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded)
                {
                    this.imgCaptcha.ImageUrl = $"{YafForumInfo.ForumClientFileRoot}resource.ashx?c=1";
                    this.tr_captcha1.Visible = true;
                    this.tr_captcha2.Visible = true;
                }

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddRoot();
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        YafBuildLink.GetLink(ForumPages.forum, "c={0}", this.PageContext.PageCategoryID));
                }

                this.PageLinks.AddForum(this.PageContext.PageForumID);

                // check if it's a reply to a topic...
                if (this.TopicId != null)
                {
                    this.InitReplyToTopic();

                    this.PollList.TopicId = this.TopicId.ToType<int>();
                }

                // If currentRow != null, we are quoting a post in a new reply, or editing an existing post
                if (currentMessage != null)
                {
                    this.OriginalMessage = currentMessage.Message;

                    if (this.QuotedMessageId != null)
                    {
                        if (this.Get<IYafSession>().MultiQuoteIds != null)
                        {
                            var quoteId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>();
                            var multiQuote = new MultiQuote { MessageID = quoteId, TopicID = this.PageContext.PageTopicID };

                            if (
                                !this.Get<IYafSession>()
                                    .MultiQuoteIds.Any(m => m.MessageID.Equals(quoteId)))
                            {
                                this.Get<IYafSession>()
                                    .MultiQuoteIds.Add(
                                        multiQuote);
                            }

                            var messages = this.GetRepository<Message>().PostListAsDataTable(
                                this.TopicId,
                                this.PageContext.PageUserID,
                                this.PageContext.PageUserID,
                                0,
                                false,
                                false,
                                false,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                DateTimeHelper.SqlDbMinTime(),
                                DateTime.UtcNow,
                                0,
                                500,
                                1,
                                0,
                                0,
                                false,
                                0);

                            // quoting a reply to a topic...
                            foreach (
                                var msg in
                                    this.Get<IYafSession>()
                                        .MultiQuoteIds.Select(
                                            item =>
                                            messages.AsEnumerable()
                                                .Select(t => new TypedMessageList(t))
                                                .Where(m => m.MessageID == item.MessageID))
                                        .SelectMany(quotedMessage => quotedMessage))
                            {
                                this.InitQuotedReply(msg);
                            }

                            // Clear Multiquotes
                            this.Get<IYafSession>().MultiQuoteIds = null;
                        }
                        else
                        {
                            this.InitQuotedReply(currentMessage);
                        }

                        this.PollList.TopicId = this.TopicId.ToType<int>();
                    }
                    else if (this.EditMessageId != null)
                    {
                        // editing a message...
                        this.InitEditedPost(currentMessage);
                        this.PollList.EditMessageId = this.EditMessageId.ToType<int>();
                    }

                    this.PollGroupId = currentMessage.PollID.ToType<int>().IsNullOrEmptyDBField()
                        ? 0
                        : currentMessage.PollID.ToType<int>();
                }

                // add the "New Topic" page link last...
                if (isNewTopic)
                {
                    this.PageLinks.AddLink(this.GetText("NEWTOPIC"));

                    // enable similar topics search
                    this.TopicSubjectTextBox.CssClass += " searchSimilarTopics";
                }

                // form user is only for "Guest"
                this.From.Text = this.Get<IUserDisplayName>().GetName(this.PageContext.PageUserID);
                if (this.User != null)
                {
                    this.FromRow.Visible = false;
                }

                /*   if (this.TopicID == null)
                                {
                                        this.PollList.TopicId = (currentRow["TopicID"].IsNullOrEmptyDBField() ? 0 : Convert.ToInt32(currentRow["TopicID"]));
                                } */
            }

            this.PollList.PollGroupId = this.PollGroupId;
        }

        /// <summary>
        /// The post reply handle edit post.
        /// </summary>
        /// <returns>
        /// Returns the Message Id
        /// </returns>
        protected long PostReplyHandleEditPost()
        {
            if (!this.PageContext.ForumEditAccess)
            {
                YafBuildLink.AccessDenied();
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

            var editMessage = this.GetRepository<Message>().ListTyped(this.EditMessageId.ToType<int>())
                .FirstOrDefault();

            // Remove Message Attachments
            if (editMessage.HasAttachments.HasValue && editMessage.HasAttachments.Value)
            {
                var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == this.EditMessageId.ToType<int>());

                attachments.ForEach(
                    attach =>
                        {
                            // Rename filename
                            if (attach.FileData == null)
                            {
                                var oldFilePath = this.Get<HttpRequestBase>().MapPath(
                                    $"{YafBoardFolders.Current.Uploads}/{attach.MessageID.ToString()}.{attach.FileName}.yafupload");

                                var newFilePath =
                                    this.Get<HttpRequestBase>().MapPath(
                                        $"{YafBoardFolders.Current.Uploads}/u{attach.UserID}.{attach.FileName}.yafupload");

                                File.Move(oldFilePath, newFilePath);
                            }


                            attach.MessageID = 0;
                            this.GetRepository<Attachment>().Update(attach);
                        });
            }

            // Mek Suggestion: This should be removed, resetting flags on edit is a bit lame.
            // Ederon : now it should be better, but all this code around forum/topic/message flags needs revamp
            // retrieve message flags
            var messageFlags = new MessageFlags(editMessage.Flags)
            {
                IsHtml =
                    this
                    .ForumEditor
                    .UsesHTML,
                IsBBCode
                    =
                    this
                    .ForumEditor
                    .UsesBBCode,
                IsPersistent
                    =
                    this
                    .PostOptions1
                    .PersistantChecked
            };

            var isModeratorChanged = this.PageContext.PageUserID != this.OwnerUserId;

            this.GetRepository<Message>().Update(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("m"),
                this.Priority.SelectedValue.ToType<int>(),
                this.ForumEditor.Text.Trim(),
                descriptionSave.Trim(),
                string.Empty,
                stylesSave.Trim(),
                subjectSave.Trim(),
                messageFlags.BitValue,
                this.HtmlEncode(this.ReasonEditor.Text),
                isModeratorChanged,
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                this.OriginalMessage,
                this.PageContext.PageUserID);

            var messageId = this.EditMessageId.Value;

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            // remove cache if it exists...
            this.Get<IDataCache>()
                .Remove(string.Format(Constants.Cache.FirstPostCleaned, this.PageContext.PageBoardID,this.TopicId));

            return messageId;
        }

        /// <summary>
        /// The post reply handle new post.
        /// </summary>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// Returns the Message Id.
        /// </returns>
        protected long PostReplyHandleNewPost(out long topicId)
        {
            long messageId = 0;

            if (!this.PageContext.ForumPostAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            var isForumModerated = false;

            var forumInfo = this.GetRepository<Types.Models.Forum>()
                .List(this.PageContext.PageBoardID, this.PageContext.PageForumID).FirstOrDefault();
            
            if (forumInfo != null)
            {
                isForumModerated = this.CheckForumModerateStatus(forumInfo, true);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                this.spamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                this.spamApproved = true;
            }

            // make message flags
            var messageFlags = new MessageFlags
                               {
                                   IsHtml = this.ForumEditor.UsesHTML,
                                   IsBBCode = this.ForumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistantChecked,
                                   IsApproved = this.spamApproved
                               };

            // Save to Db
            topicId = this.GetRepository<Topic>().Save(
                this.PageContext.PageForumID,
                this.TopicSubjectTextBox.Text.Trim(),
                string.Empty,
                this.TopicStylesTextBox.Text.Trim(),
                this.TopicDescriptionTextBox.Text.Trim(),
                this.ForumEditor.Text,
                this.PageContext.PageUserID,
                this.Priority.SelectedValue.ToType<int>(),
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                string.Empty,
                messageFlags.BitValue,
                ref messageId);

            this.UpdateWatchTopic(this.PageContext.PageUserID, (int)topicId);

            // clear caches as stats changed
            if (!messageFlags.IsApproved)
            {
                return messageId;
            }

            this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

            return messageId;
        }

        /// <summary>
        /// The post reply handle reply to topic.
        /// </summary>
        /// <param name="isSpamApproved">
        /// The is Spam Approved.
        /// </param>
        /// <returns>
        /// Returns the Message Id.
        /// </returns>
        protected long PostReplyHandleReplyToTopic(bool isSpamApproved)
        {
            long messageId = 0;

            if (!this.PageContext.ForumReplyAccess)
            {
                YafBuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            var isForumModerated = false;

            var forumInfo = this.GetRepository<Types.Models.Forum>()
                .List(this.PageContext.PageBoardID, this.PageContext.PageForumID).FirstOrDefault();

            if (forumInfo != null)
            {
                isForumModerated = this.CheckForumModerateStatus(forumInfo, false);
            }

            // If Forum is Moderated
            if (isForumModerated)
            {
                isSpamApproved = false;
            }

            // Bypass Approval if Admin or Moderator
            if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
            {
                isSpamApproved = true;
            }

            object replyTo = this.QuotedMessageId ?? -1;

            // make message flags
            var messageFlags = new MessageFlags
                               {
                                   IsHtml = this.ForumEditor.UsesHTML,
                                   IsBBCode = this.ForumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistantChecked,
                                   IsApproved = isSpamApproved
                               };

            this.GetRepository<Message>().Save(
               this.TopicId.Value,
                this.PageContext.PageUserID,
                this.ForumEditor.Text,
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                replyTo.ToType<int>(),
                messageFlags.BitValue,
                ref messageId);

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            if (messageFlags.IsApproved)
            {
                this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
                this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
            }

            return messageId;
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

            // Check for SPAM
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess
                && !this.PageContext.BoardSettings.SpamServiceType.Equals(0))
            {
                string spamResult;

                // Check content for spam
                if (
                    this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                        this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.ForumEditor.Text)))
                            .RemoveMultipleWhitespace(),
                        this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        out spamResult))
                {
                    switch (this.PageContext.BoardSettings.SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string.Format(
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}", this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post.",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and banned",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);

                            var userIp =
                                new CombinedUserDataHelper(
                                    this.PageContext.CurrentUserData.Membership,
                                    this.PageContext.PageUserID).LastIP;

                            UserMembershipHelper.DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // Check posts for urls if the user has only x posts
            if (YafContext.Current.CurrentUserData.NumPosts
                <= YafContext.Current.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount &&
                !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
            {
                var urlCount = UrlHelper.CountUrls(this.ForumEditor.Text);

                if (urlCount > this.PageContext.BoardSettings.AllowedNumberOfUrls)
                {
                    var spamResult =
                        $"The user posted {urlCount} urls but allowed only {this.PageContext.BoardSettings.AllowedNumberOfUrls}";

                    switch (this.PageContext.BoardSettings.SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string.Format(
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}",
                                    this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                        spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post.",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and banned",
                                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);

                            var userIp =
                                new CombinedUserDataHelper(
                                    this.PageContext.CurrentUserData.Membership,
                                    this.PageContext.PageUserID).LastIP;

                            UserMembershipHelper.DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // update the last post time...
            this.Get<IYafSession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            long messageId;
            long newTopic = 0;

            if (this.TopicId != null)
            {
                // Reply to topic
                messageId = this.PostReplyHandleReplyToTopic(this.spamApproved);
                newTopic = this.TopicId.ToType<long>();
            }
            else if (this.EditMessageId != null)
            {
                // Edit existing post
                messageId = this.PostReplyHandleEditPost();
            }
            else
            {
                // New post
                messageId = this.PostReplyHandleNewPost(out newTopic);
            }

            // Check if message is approved
            var isApproved = false;
            using (var dt = this.GetRepository<Message>().ListAsDataTable(messageId.ToType<int>()))
            {
                foreach (DataRow row in dt.Rows)
                {
                    isApproved = row["Flags"].BinaryAnd(MessageFlags.Flags.IsApproved);
                }
            }

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            var attachPollParameter = string.Empty;
            var retforum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible && newTopic > 0)
            {
                // new topic poll token
                attachPollParameter = $"&t={newTopic}";

                // new return forum poll token
                retforum = $"&f={this.PageContext.PageForumID}";
            }

            // Create notification emails
            if (isApproved)
            {
                this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                if (Config.IsDotNetNuke && this.EditMessageId == null && !this.PageContext.IsGuest)
                {
                    if (this.TopicId != null)
                    {
                        this.Get<IActivityStream>()
                            .AddReplyToStream(
                                this.PageContext.PageForumID,
                                newTopic,
                                messageId.ToType<int>(),
                                this.PageContext.PageTopicName,
                                this.ForumEditor.Text);
                    }
                    else
                    {
                        this.Get<IActivityStream>()
                            .AddTopicToStream(
                                this.PageContext.PageForumID,
                                newTopic,
                                messageId.ToType<int>(),
                                this.TopicSubjectTextBox.Text,
                                this.ForumEditor.Text);
                    }
                }

                if (attachPollParameter.IsNotSet() || !this.PostOptions1.PollChecked)
                {
                    // regular redirect...
                    YafBuildLink.Redirect(ForumPages.posts, "m={0}#post{0}", messageId);
                }
                else
                {
                    // poll edit redirect...
                    YafBuildLink.Redirect(ForumPages.polledit, "{0}", attachPollParameter);
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
                var url = YafBuildLink.GetLink(ForumPages.topics, "f={0}", this.PageContext.PageForumID);

                if (this.PageContext.PageTopicID > 0)
                {
                    url = YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);
                }

                if (attachPollParameter.Length <= 0)
                {
                    YafBuildLink.Redirect(ForumPages.info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
                else
                {
                    YafBuildLink.Redirect(ForumPages.polledit, "&ra=1{0}{1}", attachPollParameter, retforum);
                }

                if (Config.IsRainbow)
                {
                    YafBuildLink.Redirect(ForumPages.info, "i=1");
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
                                                           IsHtml = this.ForumEditor.UsesHTML,
                                                           IsBBCode = this.ForumEditor.UsesBBCode
                                                       };

            this.PreviewMessagePost.Message = this.ForumEditor.Text;

            if (!this.PageContext.BoardSettings.AllowSignatures)
            {
                return;
            }

            var userSig = this.GetRepository<User>().GetSignature(this.PageContext.PageUserID);

            if (userSig.IsSet())
            {
                this.PreviewMessagePost.Signature = userSig;
            }
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
        private bool CanEditPostCheck([NotNull] TypedMessageList message, Topic topicInfo)
        {
            var postLocked = false;

            if (!this.PageContext.IsAdmin && this.PageContext.BoardSettings.LockPosts > 0)
            {
                var edited = message.Edited.ToType<DateTime>();

                if (edited.AddDays(this.PageContext.BoardSettings.LockPosts) < DateTime.UtcNow)
                {
                    postLocked = true;
                }
            }

            // get  forum information
            var forumInfo = this.GetRepository<Types.Models.Forum>()
                .List(this.PageContext.PageBoardID, this.PageContext.PageForumID).FirstOrDefault();

            // Ederon : 9/9/2007 - moderator can edit in locked topics
            return !postLocked && !forumInfo.ForumFlags.IsLocked
                               && !topicInfo.TopicFlags.IsLocked
                               && message.UserID.ToType<int>() == this.PageContext.PageUserID
                    || this.PageContext.ForumModeratorAccess && this.PageContext.ForumEditAccess;
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
            var forumInfo = this.GetRepository<Types.Models.Forum>()
                .List(this.PageContext.PageBoardID, this.PageContext.PageForumID).FirstOrDefault();

            if (topicInfo == null || forumInfo == null)
            {
                return false;
            }

            // Ederon : 9/9/2007 - moderator can reply to locked topics
            return !forumInfo.ForumFlags.IsLocked
                    && !topicInfo.TopicFlags.IsLocked || this.PageContext.ForumModeratorAccess
                   && this.PageContext.ForumReplyAccess;
        }

        /// <summary>
        /// The init edited post.
        /// </summary>
        /// <param name="currentMessage">
        /// The current message.
        /// </param>
        private void InitEditedPost([NotNull] TypedMessageList currentMessage)
        {
            if (this.ForumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                currentMessage.Message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(currentMessage.Message);
            }

            if (this.ForumEditor.UsesBBCode && currentMessage.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                currentMessage.Message = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(currentMessage.Message);
            }

            this.ForumEditor.Text = currentMessage.Message;

            // Convert Message Attachments to new [Attach] BBCode Attachments
            if (currentMessage.HasAttachments.HasValue && currentMessage.HasAttachments.Value)
            {
                var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == currentMessage.MessageID);

                attachments.ForEach(
                    attach =>
                        {
                            this.ForumEditor.Text += $" [ATTACH]{attach.ID}[/Attach] ";
                        });
            }

            this.Title.Text = this.GetText("EDIT");
            this.PostReply.TextLocalizedTag = "SAVE";
            this.PostReply.TextLocalizedPage = "COMMON";

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(currentMessage.Topic),
                YafBuildLink.GetLink(ForumPages.posts, "m={0}", this.EditMessageId));

            // editing..
            this.PageLinks.AddLink(this.GetText("EDIT"));

            this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(currentMessage.Topic);
            this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(currentMessage.Description);

            if (currentMessage.TopicOwnerID.ToType<int>() == currentMessage.UserID.ToType<int>()
                || this.PageContext.ForumModeratorAccess)
            {
                // allow editing of the topic subject
                this.TopicSubjectTextBox.Enabled = true;
                if (this.PageContext.BoardSettings.EnableTopicDescription)
                {
                    this.DescriptionRow.Visible = true;
                }
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

            this.TopicStylesTextBox.Text = currentMessage.Styles;

            this.Priority.SelectedItem.Selected = false;
            this.Priority.Items.FindByValue(currentMessage.Priority.ToString()).Selected = true;

            this.EditReasonRow.Visible = true;
            this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
            this.PostOptions1.PersistantChecked = currentMessage.Flags.IsPersistent;
        }

        /// <summary>
        /// The init quoted reply.
        /// </summary>
        /// <param name="message">
        /// The current TypedMessage.
        /// </param>
        private void InitQuotedReply(TypedMessageList message)
        {
            var messageContent = message.Message;

            if (this.PageContext.BoardSettings.RemoveNestedQuotes)
            {
                messageContent = this.Get<IFormatMessage>().RemoveNestedQuotes(messageContent);
            }

            if (this.ForumEditor.UsesHTML && message.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                messageContent = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(messageContent);
            }

            if (this.ForumEditor.UsesBBCode && message.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                messageContent = this.Get<IBBCode>().ConvertHtmltoBBCodeForEdit(messageContent);
            }

            // Ensure quoted replies have bad words removed from them
            messageContent = this.Get<IBadWordReplace>().Replace(messageContent);

            // Remove HIDDEN Text
            messageContent = this.Get<IFormatMessage>().RemoveHiddenBBCodeContent(messageContent);

            // Quote the original message
            this.ForumEditor.Text +=
                $"[quote={this.Get<IUserDisplayName>().GetName(message.UserID.ToType<int>())};{message.MessageID}]{messageContent}[/quote]\n\n"
                    .TrimStart();
        }

        /// <summary>
        /// The init reply to topic.
        /// </summary>
        private void InitReplyToTopic()
        {
            var topic = this.GetRepository<Topic>().GetById(this.TopicId.ToType<int>());

            // Ederon : 9/9/2007 - moderators can reply in locked topics
            if (topic.TopicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
            {
                this.Response.Redirect(this.Get<HttpRequestBase>().UrlReferrer.ToString());
            }

            this.PriorityRow.Visible = false;
            this.SubjectRow.Visible = false;
            this.DescriptionRow.Visible = false;
            this.StyleRow.Visible = false;
            this.Title.Text = this.GetText("reply");

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(topic.TopicName),
                YafBuildLink.GetLink(ForumPages.posts, "t={0}", this.TopicId));

            // add "reply" text...
            this.PageLinks.AddLink(this.GetText("reply"));

            this.HandleUploadControls();

            // show the last posts AJAX frame...
            this.LastPosts1.Visible = true;
            this.LastPosts1.TopicID = this.TopicId.Value;
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
        private bool CheckForumModerateStatus(Types.Models.Forum forumInfo, bool isNewTopic)
        {
            var forumModerated = forumInfo.ForumFlags.IsModerated;

            if (!forumModerated)
            {
                return false;
            }

            if (forumInfo.IsModeratedNewTopicOnly && !isNewTopic)
            {
                return false;
            }

            if (!forumInfo.ModeratedPostCount.HasValue || this.PageContext.IsGuest)
            {
                return true;
            }

            var moderatedPostCount = forumInfo.ModeratedPostCount;

            return !(this.PageContext.CurrentUserData.NumPosts >= moderatedPostCount);
        }

        /// <summary>
        /// Handles the upload controls.
        /// </summary>
        private void HandleUploadControls()
        {
            this.ForumEditor.UserCanUpload = this.PageContext.ForumUploadAccess;
            this.UploadDialog.Visible = this.PageContext.ForumUploadAccess;

            this.PostAttachments1.Visible = !this.ForumEditor.AllowsUploads && this.PageContext.ForumUploadAccess;
        }

        #endregion
    }
}