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
                                              : new CombinedUserDataHelper(this.PageContext.PageUserID).AutoWatchTopics;
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
                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("EMPTY_MESSAGE"), MessageTypes.warning);

                    return;
                }

                // No need to check whitespace if they are actually posting something
                if (this.Get<BoardSettings>().MaxPostSize > 0
                    && this.quickReplyEditor.Text.Length >= this.Get<BoardSettings>().MaxPostSize)
                {
                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("ISEXCEEDED"), MessageTypes.warning);

                    return;
                }

                if (this.EnableCaptcha() && !CaptchaHelper.IsValid(this.tbCaptcha.Text.Trim()))
                {
                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                        "openModalJs",
                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(this.GetText("BAD_CAPTCHA"), MessageTypes.warning);

                    return;
                }

                if (!(this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                    && this.Get<BoardSettings>().PostFloodDelay > 0)
                {
                    if (BoardContext.Current.Get<ISession>().LastPost
                        > DateTime.UtcNow.AddSeconds(-this.Get<BoardSettings>().PostFloodDelay))
                    {
                        BoardContext.Current.PageElements.RegisterJsBlockStartup(
                            "openModalJs",
                            JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                        this.PageContext.AddLoadMessage(
                            this.GetTextFormatted(
                                "wait",
                                (BoardContext.Current.Get<ISession>().LastPost
                                 - DateTime.UtcNow.AddSeconds(-this.Get<BoardSettings>().PostFloodDelay)).Seconds),
                            MessageTypes.warning);

                        return;
                    }
                }

                BoardContext.Current.Get<ISession>().LastPost = DateTime.UtcNow;

                // post message...
                object replyTo = -1;
                var message = this.quickReplyEditor.Text;
                long topicId = this.PageContext.PageTopicID;

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
                if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess
                                              && !this.Get<BoardSettings>().SpamServiceType.Equals(0))
                {
                    // Check content for spam
                    if (this.Get<ISpamCheck>().CheckPostForSpam(
                        this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                        BoardContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        this.quickReplyEditor.Text,
                        this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        out var spamResult))
                    {
                        switch (this.Get<BoardSettings>().SpamMessageHandling)
                        {
                            case 0:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}",
                                    EventLogTypes.SpamMessageDetected);
                                break;
                            case 1:
                                spamApproved = false;
                                isPossibleSpamMessage = true;
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, it was flagged as unapproved post",
                                    EventLogTypes.SpamMessageDetected);
                                break;
                            case 2:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, post was rejected",
                                    EventLogTypes.SpamMessageDetected);

                                BoardContext.Current.PageElements.RegisterJsBlockStartup(
                                    "openModalJs",
                                    JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                                this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                                return;
                            case 3:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, user was deleted and bannded",
                                    EventLogTypes.SpamMessageDetected);

                                var userIp = new CombinedUserDataHelper(
                                    this.PageContext.CurrentUserData.Membership,
                                    this.PageContext.PageUserID).LastIP;

                                UserMembershipHelper.DeleteAndBanUser(
                                    this.PageContext.PageUserID,
                                    this.PageContext.CurrentUserData.Membership,
                                    userIp);

                                return;
                        }
                    }

                    // Check posts for urls if the user has only x posts
                    if (BoardContext.Current.CurrentUserData.NumPosts
                        <= BoardContext.Current.Get<BoardSettings>().IgnoreSpamWordCheckPostCount
                        && !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                    {
                        var urlCount = UrlHelper.CountUrls(this.quickReplyEditor.Text);

                        if (urlCount > this.PageContext.BoardSettings.AllowedNumberOfUrls)
                        {
                            spamResult =
                                $"The user posted {urlCount} urls but allowed only {this.PageContext.BoardSettings.AllowedNumberOfUrls}";

                            switch (this.Get<BoardSettings>().SpamMessageHandling)
                            {
                                case 0:
                                    this.Logger.Log(
                                        this.PageContext.PageUserID,
                                        "Spam Message Detected",
                                        $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}",
                                        EventLogTypes.SpamMessageDetected);
                                    break;
                                case 1:
                                    spamApproved = false;
                                    isPossibleSpamMessage = true;
                                    this.Logger.Log(
                                        this.PageContext.PageUserID,
                                        "Spam Message Detected",
                                        $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, it was flagged as unapproved post",
                                        EventLogTypes.SpamMessageDetected);
                                    break;
                                case 2:
                                    this.Logger.Log(
                                        this.PageContext.PageUserID,
                                        "Spam Message Detected",
                                        $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, post was rejected",
                                        EventLogTypes.SpamMessageDetected);

                                    BoardContext.Current.PageElements.RegisterJsBlockStartup(
                                        "openModalJs",
                                        JavaScriptBlocks.OpenModalJs("QuickReplyDialog"));

                                    this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                                    return;
                                case 3:
                                    this.Logger.Log(
                                        this.PageContext.PageUserID,
                                        "Spam Message Detected",
                                        $"Spam Check detected possible SPAM ({spamResult}) posted by User: {(this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName)}, user was deleted and bannded",
                                        EventLogTypes.SpamMessageDetected);

                                    var userIp = new CombinedUserDataHelper(
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
                    topicId,
                    this.PageContext.PageUserID,
                    message,
                    null,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    DateTime.UtcNow, 
                    replyTo.ToType<int>(),
                    messageFlags);

                // Check to see if the user has enabled "auto watch topic" option in his/her profile.
                if (this.PageContext.CurrentUserData.AutoWatchTopics)
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

                    if (!this.PageContext.IsGuest && this.PageContext.CurrentUserData.Activity)
                    {
                        this.Get<IActivityStream>().AddReplyToStream(
                            this.PageContext.PageForumID,
                            this.PageContext.PageTopicID,
                            messageId.ToType<int>(),
                            this.PageContext.PageTopicName,
                            message);
                    }

                    // redirect to newly posted message
                    BuildLink.Redirect(ForumPages.Posts, "m={0}&#post{0}", messageId);
                }
                else
                {
                    if (this.Get<BoardSettings>().EmailModeratorsOnModeratedPost)
                    {
                        // not approved, notify moderators
                        this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                            this.PageContext.PageForumID,
                            messageId.ToType<int>(),
                            isPossibleSpamMessage);
                    }

                    var url = BuildLink.GetLink(ForumPages.Topics, "f={0}", this.PageContext.PageForumID);

                    if (Config.IsRainbow)
                    {
                        BuildLink.Redirect(ForumPages.Info, "i=1");
                    }
                    else
                    {
                        BuildLink.Redirect(ForumPages.Info, "i=1&url={0}", this.Server.UrlEncode(url));
                    }
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
            if (this.PageContext.Moderated)
            {
                return true;
            }

            var forumModerated = forumInfo.Flags.BinaryAnd(ForumFlags.Flags.IsModerated);

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

            return !(this.PageContext.CurrentUserData.NumPosts >= moderatedPostCount);
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

            return this.PageContext.BoardSettings.EnableCaptchaForPost && !this.PageContext.IsCaptchaExcluded;
        }

        #endregion
    }
}