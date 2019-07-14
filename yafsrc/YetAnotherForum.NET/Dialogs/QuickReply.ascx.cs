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
namespace YAF.Dialogs
{
    #region Using

    using System;
    using System.Linq;
    using System.Threading;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
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
            this.quickReplyEditor = new BasicBBCodeEditor();

            base.OnInit(e: e);
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
                this.imgCaptcha.ImageUrl = $"{YafForumInfo.ForumClientFileRoot}resource.ashx?c=1";
                this.CaptchaDiv.Visible = true;
            }

            this.quickReplyEditor.BaseDir = $"{YafForumInfo.ForumClientFileRoot}Scripts";

            this.QuickReplyWatchTopic.Visible = !this.PageContext.IsGuest;

            if (!this.PageContext.IsGuest)
            {
                this.TopicWatch.Checked = this.PageContext.PageTopicID > 0
                                              ? this.GetRepository<WatchTopic>().Check(
                                                  userId: this.PageContext.PageUserID,
                                                  topicId: this.PageContext.PageTopicID).HasValue
                                              : new CombinedUserDataHelper(userId: this.PageContext.PageUserID).AutoWatchTopics;
            }

            this.QuickReplyLine.Controls.Add(child: this.quickReplyEditor);
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
                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        name: "openModalJs",
                        script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(message: this.GetText(tag: "EMPTY_MESSAGE"), messageType: MessageTypes.warning);

                    return;
                }

                // No need to check whitespace if they are actually posting something
                if (this.Get<YafBoardSettings>().MaxPostSize > 0
                    && this.quickReplyEditor.Text.Length >= this.Get<YafBoardSettings>().MaxPostSize)
                {
                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        name: "openModalJs",
                        script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(message: this.GetText(tag: "ISEXCEEDED"), messageType: MessageTypes.warning);

                    return;
                }

                if (this.EnableCaptcha() && !CaptchaHelper.IsValid(captchaText: this.tbCaptcha.Text.Trim()))
                {
                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                        name: "openModalJs",
                        script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                    this.PageContext.AddLoadMessage(message: this.GetText(tag: "BAD_CAPTCHA"), messageType: MessageTypes.warning);

                    return;
                }

                if (!(this.PageContext.IsAdmin || this.PageContext.ForumModeratorAccess)
                    && this.Get<YafBoardSettings>().PostFloodDelay > 0)
                {
                    if (YafContext.Current.Get<IYafSession>().LastPost
                        > DateTime.UtcNow.AddSeconds(value: -this.Get<YafBoardSettings>().PostFloodDelay))
                    {
                        YafContext.Current.PageElements.RegisterJsBlockStartup(
                            name: "openModalJs",
                            script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                        this.PageContext.AddLoadMessage(
                            message: this.GetTextFormatted(
                                tag: "wait",
                                (YafContext.Current.Get<IYafSession>().LastPost
                                 - DateTime.UtcNow.AddSeconds(value: -this.Get<YafBoardSettings>().PostFloodDelay)).Seconds),
                            messageType: MessageTypes.warning);

                        return;
                    }
                }

                YafContext.Current.Get<IYafSession>().LastPost = DateTime.UtcNow;

                // post message...
                long messageId = 0;
                object replyTo = -1;
                var message = this.quickReplyEditor.Text;
                long topicId = this.PageContext.PageTopicID;

                // SPAM Check

                // Check if Forum is Moderated
                var isForumModerated = false;

                var dt = this.GetRepository<Forum>().List(
                    boardId: this.PageContext.PageBoardID,
                    forumId: this.PageContext.PageForumID);

                var forumInfo = dt.FirstOrDefault();

                if (forumInfo != null)
                {
                    isForumModerated = this.CheckForumModerateStatus(forumInfo: forumInfo);
                }

                var spamApproved = true;
                var isPossibleSpamMessage = false;

                // Check for SPAM
                if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess
                                              && !this.Get<YafBoardSettings>().SpamServiceType.Equals(obj: 0))
                {
                    string spamResult;

                    // Check content for spam
                    if (this.Get<ISpamCheck>().CheckPostForSpam(
                        userName: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                        ipAddress: YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        postMessage: this.quickReplyEditor.Text,
                        emailAddress: this.PageContext.IsGuest ? null : this.PageContext.User.Email,
                        result: out spamResult))
                    {
                        switch (this.Get<YafBoardSettings>().SpamMessageHandling)
                        {
                            case 0:
                                this.Logger.Log(
                                    userId: this.PageContext.PageUserID,
                                    source: "Spam Message Detected",
                                    description: string.Format(
                                        format: "Spam Check detected possible SPAM ({1}) posted by User: {0}",
                                        arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                            arg1: spamResult),
                                    eventType: EventLogTypes.SpamMessageDetected);
                                break;
                            case 1:
                                spamApproved = false;
                                isPossibleSpamMessage = true;
                                this.Logger.Log(
                                    userId: this.PageContext.PageUserID,
                                    source: "Spam Message Detected",
                                    description: string
                                        .Format(
                                            format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post",
                                            arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                arg1: spamResult),
                                    eventType: EventLogTypes.SpamMessageDetected);
                                break;
                            case 2:
                                this.Logger.Log(
                                    userId: this.PageContext.PageUserID,
                                    source: "Spam Message Detected",
                                    description: string
                                        .Format(
                                            format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected",
                                            arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                arg1: spamResult),
                                    eventType: EventLogTypes.SpamMessageDetected);

                                YafContext.Current.PageElements.RegisterJsBlockStartup(
                                    name: "openModalJs",
                                    script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                                this.PageContext.AddLoadMessage(message: this.GetText(tag: "SPAM_MESSAGE"), messageType: MessageTypes.danger);

                                return;
                            case 3:
                                this.Logger.Log(
                                    userId: this.PageContext.PageUserID,
                                    source: "Spam Message Detected",
                                    description: string
                                        .Format(
                                            format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and bannded",
                                            arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                arg1: spamResult),
                                    eventType: EventLogTypes.SpamMessageDetected);

                                var userIp = new CombinedUserDataHelper(
                                    membershipUser: this.PageContext.CurrentUserData.Membership,
                                    userId: this.PageContext.PageUserID).LastIP;

                                UserMembershipHelper.DeleteAndBanUser(
                                    userID: this.PageContext.PageUserID,
                                    user: this.PageContext.CurrentUserData.Membership,
                                    userIpAddress: userIp);

                                return;
                        }
                    }

                    // Check posts for urls if the user has only x posts
                    if (YafContext.Current.CurrentUserData.NumPosts
                        <= YafContext.Current.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount
                        && !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                    {
                        var urlCount = UrlHelper.CountUrls(message: this.quickReplyEditor.Text);

                        if (urlCount > this.PageContext.BoardSettings.AllowedNumberOfUrls)
                        {
                            spamResult =
                                $"The user posted {urlCount} urls but allowed only {this.PageContext.BoardSettings.AllowedNumberOfUrls}";

                            switch (this.Get<YafBoardSettings>().SpamMessageHandling)
                            {
                                case 0:
                                    this.Logger.Log(
                                        userId: this.PageContext.PageUserID,
                                        source: "Spam Message Detected",
                                        description: string.Format(
                                            format: "Spam Check detected possible SPAM ({1}) posted by User: {0}",
                                            arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                arg1: spamResult),
                                        eventType: EventLogTypes.SpamMessageDetected);
                                    break;
                                case 1:
                                    spamApproved = false;
                                    isPossibleSpamMessage = true;
                                    this.Logger.Log(
                                        userId: this.PageContext.PageUserID,
                                        source: "Spam Message Detected",
                                        description: string
                                            .Format(
                                                format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post",
                                                arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                    arg1: spamResult),
                                        eventType: EventLogTypes.SpamMessageDetected);
                                    break;
                                case 2:
                                    this.Logger.Log(
                                        userId: this.PageContext.PageUserID,
                                        source: "Spam Message Detected",
                                        description: string
                                            .Format(
                                                format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected",
                                                arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                    arg1: spamResult),
                                        eventType: EventLogTypes.SpamMessageDetected);

                                    YafContext.Current.PageElements.RegisterJsBlockStartup(
                                        name: "openModalJs",
                                        script: JavaScriptBlocks.OpenModalJs(clientId: "QuickReplyDialog"));

                                    this.PageContext.AddLoadMessage(message: this.GetText(tag: "SPAM_MESSAGE"), messageType: MessageTypes.danger);

                                    return;
                                case 3:
                                    this.Logger.Log(
                                        userId: this.PageContext.PageUserID,
                                        source: "Spam Message Detected",
                                        description: string
                                            .Format(
                                                format: "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and bannded",
                                                arg0: this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                                    arg1: spamResult),
                                        eventType: EventLogTypes.SpamMessageDetected);

                                    var userIp = new CombinedUserDataHelper(
                                        membershipUser: this.PageContext.CurrentUserData.Membership,
                                        userId: this.PageContext.PageUserID).LastIP;

                                    UserMembershipHelper.DeleteAndBanUser(
                                        userID: this.PageContext.PageUserID,
                                        user: this.PageContext.CurrentUserData.Membership,
                                        userIpAddress: userIp);

                                    return;
                            }
                        }
                    }

                    if (!this.PageContext.IsGuest)
                    {
                        this.UpdateWatchTopic(userId: this.PageContext.PageUserID, topicId: this.PageContext.PageTopicID);
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
                this.GetRepository<Message>().Save(
                    topicId: topicId,
                    userId: this.PageContext.PageUserID,
                    message: message,
                    guestUserName: null,
                    ip: this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    posted: DateTime.UtcNow, 
                    replyTo: replyTo.ToType<int>(),
                    flags: messageFlags.BitValue,
                    messageID: ref messageId);

                // Check to see if the user has enabled "auto watch topic" option in his/her profile.
                if (this.PageContext.CurrentUserData.AutoWatchTopics)
                {
                    var watchTopicId = this.GetRepository<WatchTopic>().Check(
                        userId: this.PageContext.PageUserID,
                        topicId: this.PageContext.PageTopicID);

                    if (!watchTopicId.HasValue)
                    {
                        // subscribe to this topic
                        this.GetRepository<WatchTopic>().Add(userID: this.PageContext.PageUserID, topicID: this.PageContext.PageTopicID);
                    }
                }

                if (messageFlags.IsApproved)
                {
                    // send new post notification to users watching this topic/forum
                    this.Get<ISendNotification>().ToWatchingUsers(newMessageId: messageId.ToType<int>());

                    if (Config.IsDotNetNuke && !this.PageContext.IsGuest)
                    {
                        this.Get<IActivityStream>().AddReplyToStream(
                            forumID: this.PageContext.PageForumID,
                            topicID: this.PageContext.PageTopicID,
                            messageID: messageId.ToType<int>(),
                            topicTitle: this.PageContext.PageTopicName,
                            message: message);
                    }

                    // redirect to newly posted message
                    YafBuildLink.Redirect(page: ForumPages.posts, format: "m={0}&#post{0}", messageId);
                }
                else
                {
                    if (this.Get<YafBoardSettings>().EmailModeratorsOnModeratedPost)
                    {
                        // not approved, notifiy moderators
                        this.Get<ISendNotification>().ToModeratorsThatMessageNeedsApproval(
                            forumId: this.PageContext.PageForumID,
                            newMessageId: messageId.ToType<int>(),
                            isSpamMessage: isPossibleSpamMessage);
                    }

                    var url = YafBuildLink.GetLink(page: ForumPages.topics, format: "f={0}", this.PageContext.PageForumID);
                    if (Config.IsRainbow)
                    {
                        YafBuildLink.Redirect(page: ForumPages.info, format: "i=1");
                    }
                    else
                    {
                        YafBuildLink.Redirect(page: ForumPages.info, format: "i=1&url={0}", this.Server.UrlEncode(s: url));
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.GetType() != typeof(ThreadAbortException))
                {
                    this.Logger.Log(userId: this.PageContext.PageUserID, source: this, exception: exception);
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
            var topicWatchedId = this.GetRepository<WatchTopic>().Check(userId: userId, topicId: topicId);

            if (topicWatchedId.HasValue && !this.TopicWatch.Checked)
            {
                // unsubscribe...
                this.GetRepository<WatchTopic>().DeleteById(id: topicWatchedId.Value);
            }
            else if (!topicWatchedId.HasValue && this.TopicWatch.Checked)
            {
                // subscribe to this topic...
                this.GetRepository<WatchTopic>().Add(userID: userId, topicID: topicId);
            }
        }

        /// <summary>
        /// Checks the forum moderate status.
        /// </summary>
        /// <param name="forumInfo">The forum information.</param>
        /// <returns>Returns if the forum needs to be moderated</returns>
        private bool CheckForumModerateStatus(Forum forumInfo)
        {
            var forumModerated = forumInfo.Flags.BinaryAnd(checkAgainst: ForumFlags.Flags.IsModerated);

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