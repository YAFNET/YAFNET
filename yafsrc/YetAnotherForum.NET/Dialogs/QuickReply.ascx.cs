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
    using YAF.Types.Interfaces.Services;
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
                new CKEditorBBCodeEditorBasic { MaxCharacters = this.PageBoardContext.BoardSettings.MaxPostSize };

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
                this.imgCaptcha.ImageUrl = this.imgCaptcha.ImageUrl = CaptchaHelper.GetCaptcha();
                this.CaptchaDiv.Visible = true;
            }

            this.QuickReplyWatchTopic.Visible = !this.PageBoardContext.IsGuest;

            if (!this.PageBoardContext.IsGuest)
            {
                this.TopicWatch.Checked = this.PageBoardContext.PageTopicID > 0
                                              ? this.GetRepository<WatchTopic>().Check(
                                                  this.PageBoardContext.PageUserID,
                                                  this.PageBoardContext.PageTopicID).HasValue
                                              : this.PageBoardContext.PageUser.AutoWatchTopics;
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
                    this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageBoardContext.AddLoadMessage(this.GetText("EMPTY_MESSAGE"), MessageTypes.warning);

                    return;
                }

                // No need to check whitespace if they are actually posting something
                if (this.PageBoardContext.BoardSettings.MaxPostSize > 0
                    && this.quickReplyEditor.Text.Length >= this.PageBoardContext.BoardSettings.MaxPostSize)
                {
                    this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageBoardContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.warning);

                    return;
                }

                if (this.EnableCaptcha() && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
                {
                    this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageBoardContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.warning);

                    return;
                }

                if (!(this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
                    && this.PageBoardContext.BoardSettings.PostFloodDelay > 0)
                {
                    if (this.PageBoardContext.Get<ISession>().LastPost
                        > DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay))
                    {
                        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                            "openModalJs",
                            JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                        this.PageBoardContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "wait",
                                (this.PageBoardContext.Get<ISession>().LastPost
                                 - DateTime.UtcNow.AddSeconds(-this.PageBoardContext.BoardSettings.PostFloodDelay)).Seconds),
                            MessageTypes.warning);

                        return;
                    }
                }

                this.PageBoardContext.Get<ISession>().LastPost = DateTime.UtcNow;

                // post message...
                var message = this.quickReplyEditor.Text;

                // SPAM Check

                // Check if Forum is Moderated
                var isForumModerated = false;

                var dt = this.GetRepository<Forum>().List(
                    this.PageBoardContext.PageBoardID,
                    this.PageBoardContext.PageForumID);

                var forumInfo = dt.FirstOrDefault();

                if (forumInfo != null)
                {
                    isForumModerated = this.CheckForumModerateStatus(forumInfo);
                }

                var spamApproved = true;
                var isPossibleSpamMessage = false;

                // Check for SPAM
                if (!this.PageBoardContext.IsAdmin && !this.PageBoardContext.ForumModeratorAccess)
                {
                    // Check content for spam
                    if (this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName(),
                        this.PageBoardContext.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        this.quickReplyEditor.Text,
                        this.PageBoardContext.IsGuest ? null : this.PageBoardContext.MembershipUser.Email,
                        out var spamResult))
                    {
                        var description =
                            $@"Spam Check detected possible SPAM ({spamResult}) Original message: [{this.quickReplyEditor.Text}]
                               posted by User: {(this.PageBoardContext.IsGuest ? "Guest" : this.PageBoardContext.PageUser.DisplayOrUserName())}";

                        switch (this.PageBoardContext.BoardSettings.SpamPostHandling)
                        {
                            case SpamPostHandling.DoNothing:
                                this.Logger.SpamMessageDetected(
                                    this.PageBoardContext.PageUserID,
                                    description);
                                break;
                            case SpamPostHandling.FlagMessageUnapproved:
                                spamApproved = false;
                                isPossibleSpamMessage = true;
                                this.Logger.SpamMessageDetected(
                                    this.PageBoardContext.PageUserID,
                                    $"{description}, it was flagged as unapproved post");
                                break;
                            case SpamPostHandling.RejectMessage:
                                this.Logger.SpamMessageDetected(
                                    this.PageBoardContext.PageUserID,
                                    $"{description}, post was rejected");

                                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                                    "openModalJs",
                                    JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                                this.PageBoardContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

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

                    if (!this.PageBoardContext.IsGuest)
                    {
                        this.UpdateWatchTopic(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);
                    }
                }

                // If Forum is Moderated
                if (isForumModerated)
                {
                    spamApproved = false;
                }

                // Bypass Approval if Admin or Moderator
                if (this.PageBoardContext.IsAdmin || this.PageBoardContext.ForumModeratorAccess)
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
                var newMessage = this.GetRepository<Message>().SaveNew(
                    this.PageBoardContext.PageForum,
                    this.PageBoardContext.PageTopic,
                    this.PageBoardContext.PageUser,
                    message,
                    null,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    null,
                    messageFlags);

                newMessage.Topic = this.PageBoardContext.PageTopic;

                // Check to see if the user has enabled "auto watch topic" option in his/her profile.
                if (this.PageBoardContext.PageUser.AutoWatchTopics)
                {
                    var watchTopicId = this.GetRepository<WatchTopic>().Check(
                        this.PageBoardContext.PageUserID,
                        this.PageBoardContext.PageTopicID);

                    if (!watchTopicId.HasValue)
                    {
                        // subscribe to this topic
                        this.GetRepository<WatchTopic>().Add(this.PageBoardContext.PageUserID, this.PageBoardContext.PageTopicID);
                    }
                }

                if (messageFlags.IsApproved)
                {
                    // send new post notification to users watching this topic/forum
                    this.Get<ISendNotification>().ToWatchingUsers(newMessage);

                    if (!this.PageBoardContext.IsGuest && this.PageBoardContext.PageUser.Activity)
                    {
                        this.Get<IActivityStream>().AddReplyToStream(
                            Config.IsDotNetNuke ? this.PageBoardContext.PageForumID : this.PageBoardContext.PageUserID,
                            this.PageBoardContext.PageTopicID,
                            newMessage.ID,
                            this.PageBoardContext.PageTopic.TopicName,
                            message);
                    }

                    // redirect to newly posted message
                    this.Get<LinkBuilder>().Redirect(
                        ForumPages.Posts,
                        new {m = newMessage.ID, name = this.PageBoardContext.PageTopic.TopicName });
                }
                else
                {
                    if (this.PageBoardContext.BoardSettings.EmailModeratorsOnModeratedPost)
                    {
                        // not approved, notify moderators
                        this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                            this.PageBoardContext.PageForumID,
                            newMessage.ID,
                            isPossibleSpamMessage);
                    }

                    var url = this.Get<LinkBuilder>().GetForumLink(this.PageBoardContext.PageForumID, this.PageBoardContext.PageForum.Name);

                    this.Get<LinkBuilder>().Redirect(ForumPages.Info, new { i = 1, url = this.Server.UrlEncode(url) });
                }
            }
            catch (Exception exception)
            {
                if (exception.GetType() != typeof(ThreadAbortException))
                {
                    this.Logger.Log(this.PageBoardContext.PageUserID, this, exception);
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
            if (this.PageBoardContext.PageUser.UserFlags.Moderated)
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

            if (!forumInfo.ModeratedPostCount.HasValue || this.PageBoardContext.IsGuest)
            {
                return true;
            }

            var moderatedPostCount = forumInfo.ModeratedPostCount.Value;

            return !(this.PageBoardContext.PageUser.NumPosts >= moderatedPostCount);
        }

        /// <summary>
        /// Enables the captcha.
        /// </summary>
        /// <returns>Returns if Captcha is enabled or not</returns>
        private bool EnableCaptcha()
        {
            if (this.PageBoardContext.IsGuest && this.PageBoardContext.BoardSettings.EnableCaptchaForGuests)
            {
                return true;
            }

            return this.PageBoardContext.BoardSettings.EnableCaptchaForPost && !this.PageBoardContext.PageUser.UserFlags.IsCaptchaExcluded;
        }

        #endregion
    }
}