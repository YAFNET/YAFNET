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
namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Linq;
    using System.Threading;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.BaseModules;
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
    using YAF.Web.Editors;

    #endregion

    /// <summary>
    /// The Quick Reply Dialog.
    /// </summary>
    public partial class QuickReply : BaseUserControl
    {
        /// <summary>
        ///   The _quick reply editor.
        /// </summary>
        private ForumEditor quickReplyEditor;

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // Quick Reply Modification Begin
            this.quickReplyEditor =
                new CKEditorBBCodeEditorBasic { MaxCharacters = this.PageContext.BoardSettings.MaxPostSize };

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.Page.IsPostBack)
            {
                // return;
            }

            if (this.EnableCaptcha())
            {
                this.imgCaptcha.ImageUrl = $"{BoardInfo.ForumClientFileRoot}resource.ashx?c=1";
                this.CaptchaDiv.Visible = true;
            }

            this.QuickReplyWatchTopic.Visible = !this.PageContext.IsGuest;

            if (!this.PageContext.IsGuest)
            {
                this.TopicWatch.Checked = this.PageContext.PageTopicID > 0
                                              ? this.GetRepository<WatchTopic>().Check(
                                                  this.PageContext.PageUserID,
                                                  this.PageContext.PageTopicID).HasValue
                                              : this.PageContext.User.AutoWatchTopics;
            }

            this.QuickReplyLine.Controls.Add(this.quickReplyEditor);
        }

        /// <summary>
        /// The quick reply_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void QuickReplyClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            try
            {
                if (this.quickReplyEditor.Text.Length <= 0)
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("EMPTY_MESSAGE"), MessageTypes.warning);

                    return;
                }

                // No need to check whitespace if they are actually posting something
                if (this.PageContext.BoardSettings.MaxPostSize > 0
                    && this.quickReplyEditor.Text.Length >= this.PageContext.BoardSettings.MaxPostSize)
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.warning);

                    return;
                }

                if (this.EnableCaptcha() && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
                {
                    this.PageContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.warning);

                    return;
                }

                if (!(this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                    && this.PageContext.BoardSettings.PostFloodDelay > 0)
                {
                    if (this.PageContext.Get<ISession>().LastPost
                        > DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay))
                    {
                        this.PageContext.PageElements.RegisterJsBlockStartup(
                            "openModalJs",
                            JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "wait",
                                (this.PageContext.Get<ISession>().LastPost
                                 - DateTime.UtcNow.AddSeconds(-this.PageContext.BoardSettings.PostFloodDelay)).Seconds),
                            MessageTypes.warning);

                        return;
                    }
                }

                this.PageContext.Get<ISession>().LastPost = DateTime.UtcNow;

                // post message...
                var message = this.quickReplyEditor.Text;

                // SPAM Check

                // Check if Forum is Moderated
                var isForumModerated = false;

                var dt = this.GetRepository<Forum>().List(
                    this.PageContext.PageBoardID,
                    this.PageContext.PageForumID);

                var forumInfo = dt.FirstOrDefault();

                if (forumInfo != null)
                {
                    isForumModerated = this.CheckForumModerateStatus(forumInfo);
                }

                var spamApproved = true;
                var isPossibleSpamMessage = false;

                // Check for SPAM
                if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                {
                    // Check content for spam
                    if (this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? "Guest" : this.PageContext.User.DisplayOrUserName(),
                        this.PageContext.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        this.quickReplyEditor.Text,
                        this.PageContext.IsGuest ? null : this.PageContext.MembershipUser.Email,
                        out var spamResult))
                    {
                        var description =
                            $@"Spam Check detected possible SPAM ({spamResult}) Original message: [{this.quickReplyEditor.Text}]
                               posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.User.DisplayOrUserName())}";

                        switch (this.PageContext.BoardSettings.SpamPostHandling)
                        {
                            case SpamPostHandling.DoNothing:
                                this.Logger.SpamMessageDetected(
                                    this.PageContext.PageUserID,
                                    description);
                                break;
                            case SpamPostHandling.FlagMessageUnapproved:
                                spamApproved = false;
                                isPossibleSpamMessage = true;
                                this.Logger.SpamMessageDetected(
                                    this.PageContext.PageUserID,
                                    $"{description}, it was flagged as unapproved post");
                                break;
                            case SpamPostHandling.RejectMessage:
                                this.Logger.SpamMessageDetected(
                                    this.PageContext.PageUserID,
                                    $"{description}, post was rejected");

                                this.PageContext.PageElements.RegisterJsBlockStartup(
                                    "openModalJs",
                                    JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                                this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                                return;
                            case SpamPostHandling.DeleteBanUser:
                                this.Logger.SpamMessageDetected(
                                    this.PageContext.PageUserID,
                                    $"{description}, user was deleted and bannded");

                                this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                    this.PageContext.PageUserID,
                                    this.PageContext.MembershipUser,
                                    this.PageContext.User.IP);

                                return;
                        }
                    }

                    if (!this.PageContext.IsGuest)
                    {
                        this.UpdateWatchTopic(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                    }
                }

                // If Forum is Moderated
                if (isForumModerated)
                {
                    spamApproved = false;
                }

                // Bypass Approval if Admin or Moderator
                if (this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                {
                    spamApproved = true;
                }

                var messageFlags = new MessageFlags
                {
                    IsHtml = this.quickReplyEditor.UsesHTML,
                    IsBBCode = this.quickReplyEditor.UsesBBCode,
                    IsApproved = spamApproved
                };

                // Bypass Approval if Admin or Moderator.
                var messageId = this.GetRepository<Message>().SaveNew(
                    this.PageContext.PageForumID,
                    this.PageContext.PageTopicID,
                    this.PageContext.PageTopic.TopicName,
                    this.PageContext.PageUserID,
                    message,
                    null,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    null,
                    messageFlags);

                // Check to see if the user has enabled "auto watch topic" option in his/her profile.
                if (this.PageContext.User.AutoWatchTopics)
                {
                    var watchTopicId = this.GetRepository<WatchTopic>().Check(
                        this.PageContext.PageUserID,
                        this.PageContext.PageTopicID);

                    if (!watchTopicId.HasValue)
                    {
                        // subscribe to this topic
                        this.GetRepository<WatchTopic>().Add(this.PageContext.PageUserID, this.PageContext.PageTopicID);
                    }
                }

                if (messageFlags.IsApproved)
                {
                    // send new post notification to users watching this topic/forum
                    this.Get<ISendNotification>().ToWatchingUsers(messageId.ToType<int>());

                    if (!this.PageContext.IsGuest && this.PageContext.User.Activity)
                    {
                        this.Get<IActivityStream>().AddReplyToStream(
                            this.PageContext.PageForumID,
                            this.PageContext.PageTopicID,
                            messageId.ToType<int>(),
                            this.PageContext.PageTopic.TopicName,
                            message);
                    }

                    // redirect to newly posted message
                    this.Get<LinkBuilder>().Redirect(
                        ForumPages.Posts,
                        "m={0}&name={1}&",
                        messageId,
                        this.PageContext.PageTopic.TopicName);
                }
                else
                {
                    if (this.PageContext.BoardSettings.EmailModeratorsOnModeratedPost)
                    {
                        // not approved, notify moderators
                        this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                            this.PageContext.PageForumID,
                            messageId.ToType<int>(),
                            isPossibleSpamMessage);
                    }

                    var url = this.Get<LinkBuilder>().GetForumLink(this.PageContext.PageForumID, this.PageContext.PageForum.Name);

                    this.Get<LinkBuilder>().Redirect(ForumPages.Info, "i=1&url={0}", this.Server.UrlEncode(url));
                }
            }
            catch (Exception exception)
            {
                if (exception.GetType() != typeof(ThreadAbortException))
                {
                    this.Logger.Log(this.PageContext.PageUserID, this, exception);
                }
            }
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

            if (topicWatchedId.HasValue && !this.TopicWatch.Checked)
            {
                // unsubscribe...
                this.GetRepository<WatchTopic>().DeleteById(topicWatchedId.Value);
            }
            else if (!topicWatchedId.HasValue && this.TopicWatch.Checked)
            {
                // subscribe to this topic...
                this.GetRepository<WatchTopic>().Add(userId, topicId);
            }
        }

        /// <summary>
        /// Checks the forum moderate status.
        /// </summary>
        /// <param name="forumInfo">The forum information.</param>
        /// <returns>Returns if the forum needs to be moderated</returns>
        private bool CheckForumModerateStatus(Forum forumInfo)
        {
            // User Moderate override
            if (this.PageContext.User.UserFlags.Moderated)
            {
                return true;
            }

            var forumModerated = forumInfo.ForumFlags.IsModerated;

            if (!forumModerated)
            {
                return false;
            }

            if (forumInfo.IsModeratedNewTopicOnly)
            {
                return false;
            }

            if (!forumInfo.ModeratedPostCount.HasValue || this.PageContext.IsGuest)
            {
                return true;
            }

            var moderatedPostCount = forumInfo.ModeratedPostCount.Value;

            return !(this.PageContext.User.NumPosts >= moderatedPostCount);
        }

        /// <summary>
        /// Enables the captcha.
        /// </summary>
        /// <returns>Returns if Captcha is enabled or not</returns>
        private bool EnableCaptcha()
        {
            if (this.PageContext.IsGuest && this.PageContext.BoardSettings.EnableCaptchaForGuests)
            {
                return true;
            }

            return this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.User.UserFlags.IsCaptchaExcluded;
        }

        #endregion
    }
}