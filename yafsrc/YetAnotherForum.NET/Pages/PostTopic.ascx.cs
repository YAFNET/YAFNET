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
    using System.Linq;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseModules;
    using YAF.Core.BasePages;
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The post New Topic Page.
    /// </summary>
    public partial class PostTopic : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   The forum editor.
        /// </summary>
        private ForumEditor forumEditor;

        /// <summary>
        ///   The Spam Approved Indicator
        /// </summary>
        private bool spamApproved = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PostTopic"/> class.
        /// </summary>
        public PostTopic()
            : base("POSTTOPIC")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Canceling Posting New Message Or editing Message.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // new topic -- cancel back to forum
            BuildLink.Redirect(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);
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
                <= DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay))
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
                && this.GetRepository<Topic>().CheckForDuplicateTopic(this.TopicSubjectTextBox.Text.Trim()))
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
            BoardContext.Current.PageElements.RegisterJsBlock(
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
                BuildLink.AccessDenied();
            }

            if (!this.PageContext.ForumPostAccess)
            {
                BuildLink.AccessDenied();
            }

            this.Title.Text = this.GetText("NEWTOPIC");

            this.HandleUploadControls();

            if (this.IsPostBack)
            {
                return;
            }

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

            this.PriorityRow.Visible = this.PageContext.ForumPriorityAccess;

            // update options...
            this.PostOptions1.Visible = !this.PageContext.IsGuest;
            this.PostOptions1.PersistentOptionVisible =
                this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess;
            this.PostOptions1.WatchOptionVisible = !this.PageContext.IsGuest;
            this.PostOptions1.PollOptionVisible = this.PageContext.ForumPollAccess;

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

            // enable similar topics search
            this.TopicSubjectTextBox.CssClass += " searchSimilarTopics";

            // form user is only for "Guest"
            this.From.Text = this.Get<IUserDisplayName>().GetName(this.PageContext.PageUserID);
            if (this.User != null)
            {
                this.FromRow.Visible = false;
            }
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            if (this.PageContext.Settings.LockedForum == 0)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.PageContext.PageCategoryName,
                    BuildLink.GetLink(ForumPages.Board, "c={0}", this.PageContext.PageCategoryID));
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID);

            // add the "New Topic" page link last...
            this.PageLinks.AddLink(this.GetText("NEWTOPIC"));
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

                            UserMembershipHelper.DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // Check posts for urls if the user has only x posts
            if (BoardContext.Current.CurrentUserData.NumPosts
                <= BoardContext.Current.Get<BoardSettings>().IgnoreSpamWordCheckPostCount &&
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

                            UserMembershipHelper.DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.CurrentUserData.Membership,
                                userIp);

                            return;
                    }
                }
            }

            // update the last post time...
            this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            // New Topic
            var messageId = this.PostReplyHandleNewPost(out var newTopic);

            // Check if message is approved
            var isApproved = this.GetRepository<Message>().GetById(messageId.ToType<int>()).MessageFlags.IsApproved;

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            var attachPollParameter = string.Empty;
            var returnForum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible)
            {
                // new topic poll token
                attachPollParameter = $"&t={newTopic}";

                // new return forum poll token
                returnForum = $"&f={this.PageContext.PageForumID}";
            }

            // Create notification emails
            if (isApproved)
            {
                this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                if (!this.PageContext.IsGuest && this.PageContext.CurrentUserData.Activity)
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

                    this.Get<IActivityStream>().AddTopicToStream(
                        Config.IsDotNetNuke ? this.PageContext.PageForumID : this.PageContext.PageUserID,
                        newTopic,
                        messageId.ToType<int>(),
                        this.TopicSubjectTextBox.Text,
                        this.forumEditor.Text);

                    // Add tags
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
                                            newTopic.ToType<int>());
                                    }
                                    else
                                    {
                                        // save new Tag
                                        var newTagId = this.GetRepository<Tag>().Add(tag);

                                        // add to topic
                                        this.GetRepository<TopicTag>().Add(newTagId, newTopic.ToType<int>());
                                    }
                                });
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

                if (attachPollParameter.Length <= 0)
                {
                    BuildLink.Redirect(ForumPages.Info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
                else
                {
                    BuildLink.Redirect(ForumPages.PollEdit, "&ra=1{0}{1}", attachPollParameter, returnForum);
                }

                if (Config.IsRainbow)
                {
                    BuildLink.Redirect(ForumPages.Info, "i=1");
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