/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;
    
    using YAF.Configuration;
    using YAF.Core.BaseModules;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    using ListItem = System.Web.UI.WebControls.ListItem;

    #endregion

    /// <summary>
    /// The post message Page.
    /// </summary>
    public partial class PostMessage : ForumPage
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
        ///   The Spam Approved Indicator
        /// </summary>
        private bool spamApproved = true;

        /// <summary>
        ///   Gets or sets OriginalMessage.
        /// </summary>
        private TypedMessageList originalMessage;

        /// <summary>
        ///   The forum.
        /// </summary>
        private Topic topic;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostMessage"/> class.
        /// </summary>
        public PostMessage()
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
                BuildLink.Redirect(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID);
            }
            else
            {
                // new topic -- cancel back to forum
                BuildLink.Redirect(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);
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
            if (this.Get<ISession>().LastPost
                <= DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)
                || this.EditMessageId != null)
            {
                return false;
            }

            this.PageContext.AddLoadMessage(
                this.GetTextFormatted(
                    "wait",
                    (this.Get<ISession>().LastPost
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
                && this.GetRepository<Topic>().CheckForDuplicateTopic(this.TopicSubjectTextBox.Text.Trim()) && this.TopicId == null
                && this.EditMessageId == null)
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
                return false;
            }

            if ((!this.PageContext.IsGuest || !this.PageContext.BoardSettings.EnableCaptchaForGuests)
                && (!this.PageContext.BoardSettings.EnableCaptchaForPost || this.PageContext.IsCaptchaExcluded)
                || CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
            {
                return true;
            }

            this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.danger);
            return false;
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
            this.PageContext.QueryIDs = new QueryStringIDHelper(new[] { "m", "t", "q" }, false);

            TypedMessageList currentMessage = null;

            this.topic = this.GetRepository<Topic>().GetById(this.PageContext.PageTopicID);

            // we reply to a post with a quote
            if (this.QuotedMessageId != null)
            {
                currentMessage =
                    this.GetRepository<Message>().MessageList(this.QuotedMessageId.ToType<int>())
                        .FirstOrDefault();

                if (currentMessage != null)
                {
                    if (this.Get<HttpRequestBase>().QueryString.Exists("text"))
                    {
                        var quotedMessage =
                            this.Server.UrlDecode(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("text"));

                        currentMessage.Message =
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(quotedMessage));
                    }

                    if (currentMessage.TopicID.ToType<int>() != this.PageContext.PageTopicID)
                    {
                        BuildLink.AccessDenied();
                    }

                    if (!this.CanQuotePostCheck(this.topic))
                    {
                        BuildLink.AccessDenied();
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
                    this.originalMessage = currentMessage;

                    this.ownerUserId = currentMessage.UserID.ToType<int>();

                    if (!this.CanEditPostCheck(currentMessage, this.topic))
                    {
                        BuildLink.AccessDenied();
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
                BuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] == null && this.Get<HttpRequestBase>()["m"] == null
                && !this.PageContext.ForumPostAccess)
            {
                BuildLink.AccessDenied();
            }

            if (this.Get<HttpRequestBase>()["t"] != null && !this.PageContext.ForumReplyAccess)
            {
                BuildLink.AccessDenied();
            }

            this.Title.Text = this.GetText("NEWTOPIC");

            this.HandleUploadControls();

            if (!this.IsPostBack)
            {
                // helper bool -- true if this is a completely new topic...
                var isNewTopic = this.TopicId == null && this.QuotedMessageId == null
                                  && this.EditMessageId == null;

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
                    this.imgCaptcha.ImageUrl = $"{BoardInfo.ForumClientFileRoot}resource.ashx?c=1";
                    this.tr_captcha1.Visible = true;
                    this.tr_captcha2.Visible = true;
                }

                if (this.PageContext.Settings.LockedForum == 0)
                {
                    this.PageLinks.AddRoot();
                    this.PageLinks.AddLink(
                        this.PageContext.PageCategoryName,
                        BuildLink.GetLink(ForumPages.Board, "c={0}", this.PageContext.PageCategoryID));
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
                    this.originalMessage = currentMessage;

                    if (this.QuotedMessageId != null)
                    {
                        if (this.Get<ISession>().MultiQuoteIds != null)
                        {
                            var quoteId = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q").ToType<int>();
                            var multiQuote = new MultiQuote { MessageID = quoteId, TopicID = this.PageContext.PageTopicID };

                            if (
                                !this.Get<ISession>()
                                    .MultiQuoteIds.Any(m => m.MessageID.Equals(quoteId)))
                            {
                                this.Get<ISession>()
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
                            this.Get<ISession>().MultiQuoteIds
                                .Select(
                                    item => messages.AsEnumerable().Select(t => new TypedMessageList(t))
                                        .Where(m => m.MessageID == item.MessageID))
                                .SelectMany(quotedMessage => quotedMessage).ForEach(
                                    this.InitQuotedReply);

                            // Clear Multi-quotes
                            this.Get<ISession>().MultiQuoteIds = null;
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
                if (!this.PageContext.IsGuest)
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
                BuildLink.AccessDenied();
            }

            // Update Tags
            if (this.TagsHolder.Visible)
            {
                this.GetRepository<TopicTag>().Delete(t => t.TopicID == this.PageContext.PageTopicID);

                if (this.Tags.Text.IsSet())
                {
                    var tags = this.Tags.Text.Split(',');

                    var boardTags = this.GetRepository<Tag>().GetByBoardId();

                    tags.ForEach(
                        tag =>
                            {
                                var existTag = boardTags.FirstOrDefault(t => t.TagName == tag);

                                if (existTag != null)
                                {
                                    // add to topic
                                    this.GetRepository<TopicTag>().Add(
                                        existTag.ID,
                                        this.PageContext.PageTopicID);
                                }
                                else
                                {
                                    // save new Tag
                                    var newTagId = this.GetRepository<Tag>().Add(tag);

                                    // add to topic
                                    this.GetRepository<TopicTag>().Add(newTagId, this.PageContext.PageTopicID);
                                }
                            });
                }
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
            if (editMessage?.HasAttachments != null && editMessage.HasAttachments.Value)
            {
                var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == this.EditMessageId.ToType<int>());

                attachments.ForEach(
                    attach =>
                        {
                            // Rename filename
                            if (attach.FileData == null)
                            {
                                var oldFilePath = this.Get<HttpRequestBase>().MapPath(
                                    $"{BoardFolders.Current.Uploads}/{attach.MessageID.ToString()}.{attach.FileName}.yafupload");

                                var newFilePath =
                                    this.Get<HttpRequestBase>().MapPath(
                                        $"{BoardFolders.Current.Uploads}/u{attach.UserID}.{attach.FileName}.yafupload");

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

            var isModeratorChanged = this.PageContext.PageUserID != this.ownerUserId;

            this.GetRepository<Message>().Update(
                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("m"),
                this.Priority.SelectedValue.ToType<int>(),
                this.forumEditor.Text.Trim(),
                descriptionSave.Trim(),
                string.Empty,
                stylesSave.Trim(),
                subjectSave.Trim(),
                messageFlags.BitValue,
                this.HtmlEncode(this.ReasonEditor.Text),
                isModeratorChanged,
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess,
                this.originalMessage,
                this.PageContext.PageUserID);

            var messageId = this.EditMessageId.Value;

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            // remove cache if it exists...
            this.Get<IDataCache>()
                .Remove(string.Format(Constants.Cache.FirstPostCleaned, this.PageContext.PageBoardID, this.TopicId));

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
                BuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            var isForumModerated = false;

            var forumInfo = this.GetRepository<Forum>()
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
                                   IsHtml = this.forumEditor.UsesHTML,
                                   IsBBCode = this.forumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistentChecked,
                                   IsApproved = this.spamApproved
                               };

            // Save to Db
            topicId = this.GetRepository<Topic>().Save(
                this.PageContext.PageForumID,
                this.TopicSubjectTextBox.Text.Trim(),
                string.Empty,
                this.TopicStylesTextBox.Text.Trim(),
                this.TopicDescriptionTextBox.Text.Trim(),
                this.forumEditor.Text,
                this.PageContext.PageUserID,
                this.Priority.SelectedValue.ToType<int>(),
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                string.Empty,
                messageFlags.BitValue,
                this.Tags.Text,
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
            if (!this.PageContext.ForumReplyAccess)
            {
                BuildLink.AccessDenied();
            }

            // Check if Forum is Moderated
            var isForumModerated = false;

            var forumInfo = this.GetRepository<Forum>()
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
                                   IsHtml = this.forumEditor.UsesHTML,
                                   IsBBCode = this.forumEditor.UsesBBCode,
                                   IsPersistent = this.PostOptions1.PersistentChecked,
                                   IsApproved = isSpamApproved
                               };

            var messageId = this.GetRepository<Message>().SaveNew(
                this.TopicId.Value,
                this.PageContext.PageUserID,
                this.forumEditor.Text,
                this.User != null ? null : this.From.Text,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                replyTo.ToType<int>(),
                messageFlags);

            this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);

            if (!messageFlags.IsApproved)
            {
                return messageId;
            }

            this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
            this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);

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
                // Check content for spam
                if (
                    this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName,
                        this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.forumEditor.Text)))
                            .RemoveMultipleWhitespace(),
                        this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        out var spamResult))
                {
                    switch (this.PageContext.BoardSettings.SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}",
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, it was flagged as unapproved post.",
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, post was rejected",
                                EventLogTypes.SpamMessageDetected);
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, user was deleted and banned",
                                EventLogTypes.SpamMessageDetected);

                            var userIp =
                                new CombinedUserDataHelper(
                                    this.PageContext.CurrentUserData.Membership,
                                    this.PageContext.PageUserID).LastIP;

                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // Check posts for urls if the user has only x posts
            if (this.PageContext.CurrentUserData.NumPosts
                <= this.PageContext.Get<BoardSettings>().IgnoreSpamWordCheckPostCount &&
                !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
            {
                var urlCount = UrlHelper.CountUrls(this.forumEditor.Text);

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
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}",
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, it was flagged as unapproved post.",
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, post was rejected",
                                EventLogTypes.SpamMessageDetected);
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUserName)}, user was deleted and banned",
                                EventLogTypes.SpamMessageDetected);

                            var userIp =
                                new CombinedUserDataHelper(
                                    this.PageContext.CurrentUserData.Membership,
                                    this.PageContext.PageUserID).LastIP;

                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // update the last post time...
            this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

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
            var isApproved = this.GetRepository<Message>().GetById(messageId.ToType<int>()).MessageFlags.IsApproved;

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            var attachPollParameter = string.Empty;
            var returnForum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible && newTopic > 0)
            {
                // new topic poll token
                attachPollParameter = $"&t={newTopic}";

                // new return forum poll token
                returnForum = $"&f={this.PageContext.PageForumID}";
            }

            // Create notification emails
            if (isApproved)
            {
                if (this.EditMessageId == null)
                {
                    this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());
                }

                if (this.EditMessageId == null && !this.PageContext.IsGuest && this.PageContext.CurrentUserData.Activity)
                {
                    // Handle Mentions
                    BBCodeHelper.FindMentions(this.forumEditor.Text).ForEach(
                        user =>
                            {
                                var userId = this.Get<IUserDisplayName>().GetId(user).Value;

                                if (userId != this.PageContext.PageUserID)
                                {
                                    this.Get<IActivityStream>().AddMentionToStream(
                                        userId,
                                        newTopic.ToType<int>(),
                                        messageId.ToType<int>(),
                                        this.PageContext.PageUserID);
                                }
                            });

                    // Handle User Quoting
                    BBCodeHelper.FindUserQuoting(this.forumEditor.Text).ForEach(
                        user =>
                            {
                                var userId = this.Get<IUserDisplayName>().GetId(user).Value;

                                if (userId != this.PageContext.PageUserID)
                                {
                                    this.Get<IActivityStream>().AddQuotingToStream(
                                        userId,
                                        newTopic.ToType<int>(),
                                        messageId.ToType<int>(),
                                        this.PageContext.PageUserID);
                                }
                            });

                    if (this.TopicId != null)
                    {
                        this.Get<IActivityStream>().AddReplyToStream(
                            Config.IsDotNetNuke ? this.PageContext.PageForumID : this.PageContext.PageUserID,
                            newTopic,
                            messageId.ToType<int>(),
                            this.PageContext.PageTopicName,
                            this.forumEditor.Text);
                    }
                }

                if (attachPollParameter.IsNotSet() || !this.PostOptions1.PollChecked)
                {
                    // regular redirect...
                    BuildLink.Redirect(ForumPages.Posts, "m={0}#post{0}", messageId);
                }
                else
                {
                    // poll edit redirect...
                    BuildLink.Redirect(ForumPages.PollEdit, "{0}", attachPollParameter);
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
                var url = BuildLink.GetLink(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);

                if (this.PageContext.PageTopicID > 0 && this.topic.NumPosts > 1)
                {
                    url = BuildLink.GetLink(ForumPages.Posts, "t={0}", this.PageContext.PageTopicID);
                }

                if (attachPollParameter.Length <= 0)
                {
                    BuildLink.Redirect(ForumPages.Info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
                else
                {
                    BuildLink.Redirect(ForumPages.PollEdit, "&ra=1{0}{1}", attachPollParameter, returnForum);
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
            var forumInfo = this.GetRepository<Forum>().GetById(this.PageContext.PageForumID);

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
            var forumInfo = this.GetRepository<Forum>()
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
        /// The initializes the edited post.
        /// </summary>
        /// <param name="currentMessage">
        /// The current message.
        /// </param>
        private void InitEditedPost([NotNull] TypedMessageList currentMessage)
        {
            /*if (this.forumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                currentMessage.Message = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(currentMessage.Message);
            }*/

            if (this.forumEditor.UsesBBCode && currentMessage.Flags.IsHtml)
            {
                // If the message is in HTML but the editor uses YafBBCode, convert the message text to BBCode
                currentMessage.Message = this.Get<IBBCode>().ConvertHtmlToBBCodeForEdit(currentMessage.Message);
            }

            this.forumEditor.Text = currentMessage.Message;

            // Convert Message Attachments to new [Attach] BBCode Attachments
            if (currentMessage.HasAttachments.HasValue && currentMessage.HasAttachments.Value)
            {
                var attachments = this.GetRepository<Attachment>().Get(a => a.MessageID == currentMessage.MessageID);

                attachments.ForEach(
                    attach => this.forumEditor.Text += $" [ATTACH]{attach.ID}[/Attach] ");
            }

            if (this.forumEditor.UsesHTML && currentMessage.Flags.IsBBCode)
            {
                this.forumEditor.Text = this.Get<IBBCode>().FormatMessageWithCustomBBCode(
                    this.forumEditor.Text,
                    currentMessage.Flags,
                    currentMessage.UserID,
                    currentMessage.MessageID);
            }

            this.Title.Text = this.GetText("EDIT");
            this.PostReply.TextLocalizedTag = "SAVE";
            this.PostReply.TextLocalizedPage = "COMMON";

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(currentMessage.Topic),
                BuildLink.GetLink(ForumPages.Posts, "m={0}", this.EditMessageId));

            // editing..
            this.PageLinks.AddLink(this.GetText("EDIT"));

            this.TopicSubjectTextBox.Text = this.Server.HtmlDecode(currentMessage.Topic);
            this.TopicDescriptionTextBox.Text = this.Server.HtmlDecode(currentMessage.Description);

            if (currentMessage.TopicOwnerID.ToType<int>() == currentMessage.UserID.ToType<int>()
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

            this.TopicStylesTextBox.Text = currentMessage.Styles;

            this.Priority.SelectedItem.Selected = false;
            this.Priority.Items.FindByValue(currentMessage.Priority.ToString()).Selected = true;

            this.EditReasonRow.Visible = true;
            this.ReasonEditor.Text = this.Server.HtmlDecode(currentMessage.EditReason);
            this.PostOptions1.PersistentChecked = currentMessage.Flags.IsPersistent;

            var topicsList = this.GetRepository<TopicTag>().List(this.PageContext.PageTopicID);

            if (topicsList.Any())
            {
                this.Tags.Text = topicsList.Select(t => t.Item2.TagName).ToDelimitedString(",");
            }
        }

        /// <summary>
        /// Initializes the quoted reply.
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

            /*if (this.forumEditor.UsesHTML && message.Flags.IsBBCode)
            {
                // If the message is in YafBBCode but the editor uses HTML, convert the message text to HTML
                messageContent = this.Get<IBBCode>().ConvertBBCodeToHtmlForEdit(messageContent);
            }*/

            if (this.forumEditor.UsesBBCode && message.Flags.IsHtml)
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
                $"[quote={this.Get<IUserDisplayName>().GetName(message.UserID.ToType<int>())};{message.MessageID}]{messageContent}[/quote]\r\n"
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
            if (this.topic.TopicFlags.IsLocked && !this.PageContext.ForumModeratorAccess)
            {
                var urlReferrer = this.Get<HttpRequestBase>().UrlReferrer;

                if (urlReferrer != null)
                {
                    this.Get<HttpResponseBase>().Redirect(urlReferrer.ToString());
                }
            }

            this.PriorityRow.Visible = false;
            this.SubjectRow.Visible = false;
            this.DescriptionRow.Visible = false;
            this.StyleRow.Visible = false;
            this.TagsHolder.Visible = false;

            this.Title.Text = this.GetText("reply");

            // add topic link...
            this.PageLinks.AddLink(
                this.Server.HtmlDecode(this.topic.TopicName),
                BuildLink.GetLink(ForumPages.Posts, "t={0}", this.TopicId));

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
        private bool CheckForumModerateStatus(Forum forumInfo, bool isNewTopic)
        {
            // User Moderate override
            if (this.PageContext.Moderated)
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
            this.forumEditor.UserCanUpload = this.PageContext.ForumUploadAccess;
            this.UploadDialog.Visible = this.PageContext.ForumUploadAccess;
        }

#endregion
    }
}