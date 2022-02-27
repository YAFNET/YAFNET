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
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
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
            : base("POSTTOPIC", ForumPages.PostTopic)
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
            this.Get<LinkBuilder>().Redirect(
                ForumPages.Topics,
                "f={0}&name={1}",
                this.PageContext.PageForumID,
                this.PageContext.PageForum.Name);
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
            var postedMessage = HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text.Trim()));

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

            if (this.SubjectRow.Visible && HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text).IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("NEED_SUBJECT"), MessageTypes.warning);
                return false;
            }

            if (!this.Get<IPermissions>().Check(this.PageContext.BoardSettings.AllowCreateTopicsSameName)
                && this.GetRepository<Topic>().CheckForDuplicate(HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text).Trim()))
            {
                this.PageContext.AddLoadMessage(this.GetText("SUBJECT_DUPLICATE"), MessageTypes.warning);
                return false;
            }

            if ((!this.PageContext.IsGuest || !this.PageContext.BoardSettings.EnableCaptchaForGuests)
                && (!this.PageContext.BoardSettings.EnableCaptchaForPost || this.PageContext.PageUser.UserFlags.IsCaptchaExcluded)
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
            if (this.PageContext.UploadAccess)
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

            if (!this.PageContext.ForumPostAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
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
                                                     : this.PageContext.PageUser.AutoWatchTopics;
            }

            if (this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests
                || this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.PageUser.UserFlags.IsCaptchaExcluded)
            {
                this.imgCaptcha.ImageUrl = CaptchaHelper.GetCaptcha();
                this.tr_captcha1.Visible = true;
                this.tr_captcha2.Visible = true;
            }

            // enable similar topics search
            this.TopicSubjectTextBox.CssClass += " searchSimilarTopics";

            if (!this.PageContext.IsGuest)
            {
                return;
            }

            // form user is only for "Guest"
            this.From.Text = this.PageContext.PageUser.DisplayOrUserName();

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
                this.PageLinks.AddCategory(this.PageContext.PageCategory.Name, this.PageContext.PageCategoryID);
            }

            this.PageLinks.AddForum(this.PageContext.PageForumID);

            // add the "New Topic" page link last...
            this.PageLinks.AddLink(this.GetText("NEWTOPIC"));
        }

        /// <summary>
        /// The post reply handle new post.
        /// </summary>
        /// <returns>
        /// Returns the Message Id.
        /// </returns>
        protected Message PostReplyHandleNewTopic()
        {
            if (!this.PageContext.ForumPostAccess)
            {
                this.Get<LinkBuilder>().AccessDenied();
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
            var newTopic = this.GetRepository<Topic>().SaveNew(
                this.PageContext.PageForum,
                HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text.Trim()),
                string.Empty,
                HtmlHelper.StripHtml(this.TopicStylesTextBox.Text.Trim()),
                HtmlHelper.StripHtml(this.TopicDescriptionTextBox.Text.Trim()),
                HtmlHelper.StripHtml(BBCodeHelper.EncodeCodeBlocks(this.forumEditor.Text)),
                this.PageContext.PageUser,
                this.Priority.SelectedValue.ToType<short>(),
                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUser.Name,
                this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUser.DisplayName,
                this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                DateTime.UtcNow,
                messageFlags,
                out var message);

            message.Topic = newTopic;

            this.UpdateWatchTopic(this.PageContext.PageUserID, newTopic.ID);

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
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
            {
                // Check content for spam
                if (
                    this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? this.From.Text : this.PageContext.PageUser.DisplayOrUserName(),
                        this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        BBCodeHelper.StripBBCode(
                            HtmlHelper.StripHtml(HtmlHelper.CleanHtmlString(this.forumEditor.Text)))
                            .RemoveMultipleWhitespace(),
                        this.PageContext.IsGuest ? null : this.PageContext.MembershipUser.Email,
                        out var spamResult))
                {
                    var description =
                        $@"Spam Check detected possible SPAM ({spamResult}) 
                           posted by PageUser: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUser.DisplayOrUserName())}";

                    switch (this.PageContext.BoardSettings.SpamPostHandling)
                    {
                        case SpamPostHandling.DoNothing:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                description);
                            break;
                        case SpamPostHandling.FlagMessageUnapproved:
                            this.spamApproved = false;
                            isPossibleSpamMessage = true;
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"{description}, it was flagged as unapproved post.");
                            break;
                        case SpamPostHandling.RejectMessage:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"S{description}, post was rejected");
                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);
                            return;
                        case SpamPostHandling.DeleteBanUser:
                            this.Logger.SpamMessageDetected(
                                this.PageContext.PageUserID,
                                $"{description}, user was deleted and banned");

                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                this.PageContext.PageUserID,
                                this.PageContext.MembershipUser,
                                this.PageContext.PageUser.IP);

                            return;
                    }
                }
            }

            // update the last post time...
            this.Get<ISession>().LastPost = DateTime.UtcNow.AddSeconds(30);

            // New Topic
            var newMessage = this.PostReplyHandleNewTopic();

            // Check if message is approved
            var isApproved = newMessage.MessageFlags.IsApproved;

            // vzrus^ the poll access controls are enabled and this is a new topic - we add the variables
            var attachPollParameter = string.Empty;
            var returnForum = string.Empty;

            if (this.PageContext.ForumPollAccess && this.PostOptions1.PollOptionVisible)
            {
                // new topic poll token
                attachPollParameter = $"&t={newMessage.TopicID}";

                // new return forum poll token
                returnForum = $"&f={this.PageContext.PageForumID}";
            }

            // Create notification emails
            if (isApproved)
            {
                this.Get<ISendNotification>().ToWatchingUsers(newMessage, true);

                if (!this.PageContext.IsGuest && this.PageContext.PageUser.Activity)
                {
                    // Handle Mentions
                    BBCodeHelper.FindMentions(message).ForEach(
                        user =>
                            {
                                var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                                if (userId != this.PageContext.PageUserID)
                                {
                                    this.Get<IActivityStream>().AddMentionToStream(
                                        userId,
                                        newMessage.TopicID,
                                        newMessage.ID,
                                        this.PageContext.PageUserID);
                                }
                            });

                    // Handle User Quoting
                    BBCodeHelper.FindUserQuoting(message).ForEach(
                        user =>
                            {
                                var userId = this.Get<IUserDisplayName>().FindUserByName(user).ID;

                                if (userId != this.PageContext.PageUserID)
                                {
                                    this.Get<IActivityStream>().AddQuotingToStream(
                                        userId,
                                        newMessage.TopicID,
                                        newMessage.ID,
                                        this.PageContext.PageUserID);
                                }
                            });

                    this.Get<IActivityStream>().AddTopicToStream(
                        Config.IsDotNetNuke ? this.PageContext.PageForumID : this.PageContext.PageUserID,
                        newMessage.TopicID,
                        newMessage.ID,
                        HtmlHelper.StripHtml(this.TopicSubjectTextBox.Text),
                        message);

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
                                            newMessage.TopicID);
                                    }
                                    else
                                    {
                                        // save new Tag
                                        var newTagId = this.GetRepository<Tag>().Add(tag);

                                        // add to topic
                                        this.GetRepository<TopicTag>().Add(newTagId, newMessage.TopicID);
                                    }
                                });
                    }
                }

                if (attachPollParameter.IsNotSet() || !this.PostOptions1.PollChecked)
                {
                    // regular redirect...
                    this.Get<LinkBuilder>().Redirect(ForumPages.Posts, "m={0}&name={1}", newMessage.ID, newMessage.TopicID);
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
                            newMessage.ID,
                            isPossibleSpamMessage);
                }

                // 't' variable is required only for poll and this is a attach poll token for attachments page
                if (!this.PostOptions1.PollChecked)
                {
                    attachPollParameter = string.Empty;
                }

                // Tell user that his message will have to be approved by a moderator
                var url = this.Get<LinkBuilder>().GetForumLink(this.PageContext.PageForumID, this.PageContext.PageForum.Name);

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

            this.PreviewMessagePost.MessageID = null;

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
            if (this.PostOptions1.WatchChecked)
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
            if (this.PageContext.PageUser.UserFlags.Moderated)
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

            return !(this.PageContext.PageUser.NumPosts >= moderatedPostCount);
        }

        /// <summary>
        /// Handles the upload controls.
        /// </summary>
        private void HandleUploadControls()
        {
            this.forumEditor.UserCanUpload = this.PageContext.UploadAccess;
            this.UploadDialog.Visible = this.PageContext.UploadAccess;
        }

        #endregion
    }
}