/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
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
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Post Private Message Page
    /// </summary>
    public partial class pmessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   message body editor
        /// </summary>
        protected ForumEditor _editor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "pmessage" /> class. 
        ///   Default constructor.
        /// </summary>
        public pmessage()
            : base("PMESSAGE")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            if (this.Get<YafBoardSettings>().AllowPrivateMessageAttachments)
            {
                this.PageContext.PageElements.RegisterJsScriptsInclude(
                    "FileUploadScriptJs",
#if DEBUG
                    "jquery.fileupload.comb.js");
#else
                    "jquery.fileupload.comb.min.js");
#endif

#if DEBUG
                this.PageContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.css");
#else
                this.PageContext.PageElements.RegisterCssIncludeContent("jquery.fileupload.comb.min.css");
#endif
            }

            base.OnInit(e);
        }

        /// <summary>
        /// Send pm to all users
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AllUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // create one entry to show in dropdown
            var li = new ListItem(this.GetText("ALLUSERS"), "0");

            // bind the list to dropdown
            this.ToList.Items.Add(li);
            this.ToList.Visible = true;
            this.To.Text = this.GetText("ALLUSERS");

            // hide To text box
            this.To.Visible = false;

            // hide find users/all users buttons
            this.FindUsers.Visible = false;
            this.AllUsers.Visible = false;
            this.AllBuddies.Visible = false;

            // we need clear button now
            this.Clear.Visible = true;
        }

        /// <summary>
        /// Send PM to all Buddies
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void AllBuddies_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // try to find users by user name
            var usersFound = this.Get<IBuddy>().All();

            var friendsString = new StringBuilder();

            if (!usersFound.HasRows())
            {
                return;
            }

            // we found a user(s)
            foreach (DataRow row in usersFound.Rows)
            {
                friendsString.AppendFormat("{0};", row["Name"]);
            }

            this.To.Text = friendsString.ToString();

            // hide find users/all users buttons
            this.FindUsers.Visible = false;
            this.AllUsers.Visible = false;
            this.AllBuddies.Visible = false;

            // we need clear button now
            this.Clear.Visible = true;
        }

        /// <summary>
        /// Redirect user back to his PM inbox
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_pm);
        }

        /// <summary>
        /// Clears the User List
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Clear_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // clear drop down
            this.ToList.Items.Clear();

            // hide it and show empty To text box
            this.ToList.Visible = false;
            this.To.Text = string.Empty;
            this.To.Visible = true;

            // show find users and all users (if user is admin)
            this.FindUsers.Visible = true;
            this.AllUsers.Visible = YafContext.Current.IsAdmin;
            this.AllBuddies.Visible = this.PageContext.UserHasBuddies;

            // clear button is not necessary now
            this.Clear.Visible = false;
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            // users control panel
            this.PageLinks.AddLink(
                this.Get<YafBoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                YafBuildLink.GetLink(ForumPages.cp_profile));

            // private messages
            this.PageLinks.AddLink(
                this.GetText(ForumPages.cp_pm.ToString(), "TITLE"),
                YafBuildLink.GetLink(ForumPages.cp_pm));

            // post new message
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        /// <summary>
        /// Find Users
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void FindUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.To.Text.Length < 2)
            {
                // need at least 2 latters of user's name
                YafContext.Current.AddLoadMessage(this.GetText("NEED_MORE_LETTERS"), MessageTypes.warning);
                return;
            }

            // try to find users by user name
            var usersFound = this.Get<IUserDisplayName>().Find(this.To.Text.Trim());

            if (usersFound.Count > 0)
            {
                // we found a user(s)
                this.ToList.DataSource = usersFound;
                this.ToList.DataValueField = "Key";
                this.ToList.DataTextField = "Value";
                this.ToList.DataBind();

                // ToList.SelectedIndex = 0;
                // hide To text box and show To drop down
                this.ToList.Visible = true;
                this.To.Visible = false;

                // find is no more needed
                this.FindUsers.Visible = false;

                // we need clear button displayed now
                this.Clear.Visible = true;
            }
            else
            {
                // user not found
                YafContext.Current.AddLoadMessage(this.GetText("USER_NOTFOUND"), MessageTypes.danger);
                return;
            }

            // re-bind data to the controls
            this.DataBind();
        }

        /// <summary>
        /// Handles the Initialization event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // create editor based on administrator's settings
            // get the forum editor based on the settings
            this._editor = ForumEditorHelper.GetCurrentForumEditor();

            this.EditorLine.Controls.Add(this._editor);

            this._editor.UserCanUpload = this.Get<YafBoardSettings>().AllowPrivateMessageAttachments;

            // add editor to the page
            this.EditorLine.Controls.Add(this._editor);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // if user isn't authenticated, redirect him to login page
            if (this.User == null || YafContext.Current.IsGuest)
            {
                this.RedirectNoAccess();
            }

            // set attributes of editor
            this._editor.BaseDir = "{0}Scripts".FormatWith(YafForumInfo.ForumClientFileRoot);
            this._editor.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

            // this needs to be done just once, not during postbacks
            if (this.IsPostBack)
            {
                return;
            }

            // create page links
            this.CreatePageLinks();

            // localize button labels
            this.FindUsers.Text = this.GetText("FINDUSERS");
            this.AllUsers.Text = this.GetText("ALLUSERS");
            this.AllBuddies.Text = this.GetText("ALLBUDDIES");
            this.Clear.Text = this.GetText("CLEAR");

            // only administrators can send messages to all users
            this.AllUsers.Visible = YafContext.Current.IsAdmin;

            this.AllBuddies.Visible = this.PageContext.UserHasBuddies;

            // Is Reply
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p").IsSet())
            {
                // PM is a reply or quoted reply (isQuoting)
                // to the given message id "p"
                var isQuoting = this.Request.QueryString.GetFirstOrDefault("q") == "1";

                // get quoted message
                var row =
                    LegacyDb.pmessage_list(
                        Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("p"))).GetFirstRow();

                // there is such a message
                if (row == null)
                {
                    return;
                }

                // get message sender/recipient
                var toUserId = row["ToUserID"].ToType<int>();
                var fromUserId = row["FromUserID"].ToType<int>();

                // verify access to this PM
                if (toUserId != YafContext.Current.PageUserID && fromUserId != YafContext.Current.PageUserID)
                {
                    YafBuildLink.AccessDenied();
                }

                // handle subject
                var subject = row["Subject"].ToType<string>();
                if (!subject.StartsWith("Re: "))
                {
                    subject = "Re: {0}".FormatWith(subject);
                }

                this.PmSubjectTextBox.Text = subject;

                var displayName = this.Get<IUserDisplayName>().GetName(fromUserId);

                // set "To" user and disable changing...
                this.To.Text = displayName;
                this.To.Enabled = false;
                this.FindUsers.Enabled = false;
                this.AllUsers.Enabled = false;
                this.AllBuddies.Enabled = false;

                if (!isQuoting)
                {
                    return;
                }

                // PM is a quoted reply
                var body = row["Body"].ToString();

                if (this.Get<YafBoardSettings>().RemoveNestedQuotes)
                {
                    body = this.Get<IFormatMessage>().RemoveNestedQuotes(body);
                }

                // Ensure quoted replies have bad words removed from them
                body = this.Get<IBadWordReplace>().Replace(body);

                // Quote the original message
                body = "[QUOTE={0}]{1}[/QUOTE]".FormatWith(displayName, body);

                // we don't want any whitespaces at the beginning of message
                this._editor.Text = body.TrimStart();
            }
            else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").IsSet()
                     && this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r").IsSet())
            {
                // We check here if the user have access to the option
                if (!this.PageContext.IsModeratorInAnyForum && !this.PageContext.IsForumModerator)
                {
                    return;
                }

                // PM is being sent to a predefined user
                int toUser;
                int reportMessage;

                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"), out toUser)
                    || !int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r"), out reportMessage))
                {
                    return;
                }

                // get quoted message
                var messagesRow =
                    LegacyDb.message_listreporters(
                        Security.StringToLongOrRedirect(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r")).ToType<int>(),
                        Security.StringToLongOrRedirect(
                            this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u")).ToType<int>())
                        .GetFirstRow();

                // there is such a message
                // message info should be always returned as 1 row 
                if (messagesRow == null)
                {
                    return;
                }

                // handle subject                                           
                this.PmSubjectTextBox.Text = this.GetText("REPORTED_SUBJECT");

                var displayName =
                    this.Get<IUserDisplayName>().GetName(messagesRow.Field<int>("UserID"));

                // set "To" user and disable changing...
                this.To.Text = displayName;
                this.To.Enabled = false;
                this.FindUsers.Enabled = false;
                this.AllUsers.Enabled = false;
                this.AllBuddies.Enabled = false;

                // Parse content with delimiter '|'  
                var quoteList = messagesRow.Field<string>("ReportText").Split('|');

                // Quoted replies should have bad words in them
                // Reply to report PM is always a quoted reply
                // Quote the original message in a cycle
                for (var i = 0; i < quoteList.Length; i++)
                {
                    // Add quote codes
                    quoteList[i] = "[QUOTE={0}]{1}[/QUOTE]".FormatWith(displayName, quoteList[i]);

                    // Replace DateTime delimiter '??' by ': ' 
                    // we don't want any whitespaces at the beginning of message
                    this._editor.Text = quoteList[i].Replace("??", ": ") + this._editor.Text.TrimStart();
                }
            }
            else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").IsSet())
            {
                // PM is being send as a reply to a reported post

                // find user
                int toUserId;

                if (!int.TryParse(this.Request.QueryString.GetFirstOrDefault("u"), out toUserId))
                {
                    return;
                }

                var currentRow =
                    LegacyDb.user_list(YafContext.Current.PageBoardID, toUserId, true).GetFirstRow();

                if (currentRow == null)
                {
                    return;
                }

                this.To.Text = this.Get<IUserDisplayName>().GetName(currentRow.Field<int>("UserID"));
                this.To.Enabled = false;

                // hide find user/all users buttons
                this.FindUsers.Enabled = false;
                this.AllUsers.Enabled = false;
                this.AllBuddies.Enabled = false;
            }
            else
            {
                // Blank PM

                // multi-receiver info is relevant only when sending blank PM
                if (this.Get<YafBoardSettings>().PrivateMessageMaxRecipients <= 1)
                {
                    return;
                }

                // format localized string
                this.MultiReceiverInfo.Text =
                    "<br />{0}<br />{1}".FormatWith(
                        this.GetText("MAX_RECIPIENT_INFO")
                            .FormatWith(this.Get<YafBoardSettings>().PrivateMessageMaxRecipients),
                        this.GetText("MULTI_RECEIVER_INFO"));

                // display info
                this.MultiReceiverInfo.Visible = true;
            }
        }

        /// <summary>
        /// Previews the Message Output
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // make preview row visible
            this.PreviewRow.Visible = true;

            this.PreviewMessagePost.MessageFlags.IsHtml = this._editor.UsesHTML;
            this.PreviewMessagePost.MessageFlags.IsBBCode = this._editor.UsesBBCode;
            this.PreviewMessagePost.Message = this._editor.Text;

            if (!this.Get<YafBoardSettings>().AllowSignatures)
            {
                return;
            }

            using (
                var userDT = LegacyDb.user_list(
                    YafContext.Current.PageBoardID,
                    YafContext.Current.PageUserID,
                    true))
            {
                if (!userDT.Rows[0].IsNull("Signature"))
                {
                    this.PreviewMessagePost.Signature = userDT.Rows[0]["Signature"].ToString();
                }
            }
        }

        /// <summary>
        /// Send Private Message
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var replyTo = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p").IsSet()
                              ? this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p").ToType<int>()
                              : -1;

            // recipient was set in dropdown
            if (this.ToList.Visible)
            {
                this.To.Text = this.ToList.SelectedItem.Text;
            }

            if (this.To.Text.Length <= 0)
            {
                // recipient is required field
                YafContext.Current.AddLoadMessage(this.GetText("need_to"), MessageTypes.warning);
                return;
            }

            // subject is required
            if (this.PmSubjectTextBox.Text.Trim().Length <= 0)
            {
                YafContext.Current.AddLoadMessage(this.GetText("need_subject"), MessageTypes.warning);
                return;
            }

            // message is required
            if (this._editor.Text.Trim().Length <= 0)
            {
                YafContext.Current.AddLoadMessage(this.GetText("need_message"), MessageTypes.warning);
                return;
            }

            if (this.ToList.SelectedItem != null && this.ToList.SelectedItem.Value == "0")
            {
                // administrator is sending PMs to all users           
                var body = this._editor.Text;
                var messageFlags = new MessageFlags
                                       {
                                           IsHtml = this._editor.UsesHTML,
                                           IsBBCode = this._editor.UsesBBCode
                                       };
                
                // test user's PM count
                if (!this.VerifyMessageAllowed(1, body))
                {
                    return;
                }

                LegacyDb.pmessage_save(
                    YafContext.Current.PageUserID,
                    0,
                    this.PmSubjectTextBox.Text,
                    body,
                    messageFlags.BitValue,
                    replyTo);

                // redirect to outbox (sent items), not control panel
                YafBuildLink.Redirect(ForumPages.cp_pm, "v={0}", "out");
            }
            else
            {
                // remove all abundant whitespaces and separators
                var rx = new Regex(@";(\s|;)*;");
                this.To.Text = rx.Replace(this.To.Text, ";");

                if (this.To.Text.StartsWith(";"))
                {
                    this.To.Text = this.To.Text.Substring(1);
                }

                if (this.To.Text.EndsWith(";"))
                {
                    this.To.Text = this.To.Text.Substring(0, this.To.Text.Length - 1);
                }

                rx = new Regex(@"\s*;\s*");
                this.To.Text = rx.Replace(this.To.Text, ";");

                // list of recipients
                var recipients = new List<string>(this.To.Text.Trim().Split(';'));

                if (recipients.Count > this.Get<YafBoardSettings>().PrivateMessageMaxRecipients
                    && !YafContext.Current.IsAdmin && this.Get<YafBoardSettings>().PrivateMessageMaxRecipients != 0)
                {
                    // to many recipients
                    YafContext.Current.AddLoadMessage(
                        this.GetTextFormatted(
                            "TOO_MANY_RECIPIENTS",
                            this.Get<YafBoardSettings>().PrivateMessageMaxRecipients),
                        MessageTypes.warning);

                    return;
                }

                if (!this.VerifyMessageAllowed(recipients.Count, this._editor.Text))
                {
                    return;
                }

                // list of recipient's ids
                var recipientIds = new List<int>();

                // get recipients' IDs
                foreach (var recipient in recipients)
                {
                    var userId = this.Get<IUserDisplayName>().GetId(recipient);

                    if (!userId.HasValue)
                    {
                        YafContext.Current.AddLoadMessage(
                            this.GetTextFormatted("NO_SUCH_USER", recipient),
                            MessageTypes.warning);
                        return;
                    }

                    if (UserMembershipHelper.IsGuestUser(userId.Value))
                    {
                        YafContext.Current.AddLoadMessage(this.GetText("NOT_GUEST"), MessageTypes.danger);
                        return;
                    }

                    // get recipient's ID from the database
                    if (!recipientIds.Contains(userId.Value))
                    {
                        recipientIds.Add(userId.Value);
                    }

                    var receivingPMInfo = LegacyDb.user_pmcount(userId.Value).Rows[0];

                    // test receiving user's PM count
                    if ((receivingPMInfo["NumberTotal"].ToType<int>() + 1
                         < receivingPMInfo["NumberAllowed"].ToType<int>()) || YafContext.Current.IsAdmin
                        || (bool)
                           Convert.ChangeType(
                               UserMembershipHelper.GetUserRowForID(userId.Value, true)["IsAdmin"],
                               typeof(bool)))
                    {
                        continue;
                    }

                    // recipient has full PM box
                    YafContext.Current.AddLoadMessage(
                        this.GetTextFormatted("RECIPIENTS_PMBOX_FULL", recipient),
                        MessageTypes.danger);
                    return;
                }

                // send PM to all recipients
                foreach (var userId in recipientIds)
                {
                    var body = this._editor.Text;

                    var messageFlags = new MessageFlags
                                           {
                                               IsHtml = this._editor.UsesHTML,
                                               IsBBCode = this._editor.UsesBBCode
                                           };

                    LegacyDb.pmessage_save(
                        YafContext.Current.PageUserID,
                        userId,
                        this.PmSubjectTextBox.Text,
                        body,
                        messageFlags.BitValue,
                        replyTo);

                    // reset reciever's lazy data as he should be informed at once
                    this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userId));

                    if (this.Get<YafBoardSettings>().AllowPMEmailNotification)
                    {
                        this.Get<ISendNotification>()
                            .ToPrivateMessageRecipient(userId, this.PmSubjectTextBox.Text.Trim());
                    }
                }

                // redirect to outbox (sent items), not control panel
                YafBuildLink.Redirect(ForumPages.cp_pm, "v={0}", "out");
            }
        }

        /// <summary>
        /// Verifies the message allowed.
        /// </summary>
        /// <param name="count">The recipients count.</param>
        /// <param name="message">The message.</param>
        /// <returns>
        /// Returns if the user is allowed to send a message or not
        /// </returns>
        private bool VerifyMessageAllowed(int count, string message)
        {
            // Check if SPAM Message first...
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess && !this.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                var spamChecker = new YafSpamCheck();
                string spamResult;

                // Check content for spam
                if (spamChecker.CheckPostForSpam(
                    this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                    YafContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    message,
                    this.PageContext.User.Email,
                    out spamResult))
                {
                    switch (this.Get<YafBoardSettings>().SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                "Spam Check detected possible SPAM ({1}) posted by User: {0}"
                                    .FormatWith(
                                        this.PageContext.PageUserName,
                                        spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post"
                                    .FormatWith(
                                        this.PageContext.PageUserName,
                                        spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected"
                                    .FormatWith(
                                        this.PageContext.PageUserName,
                                        spamResult),
                                EventLogTypes.SpamMessageDetected);

                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                            break;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and bannded"
                                    .FormatWith(
                                        this.PageContext.PageUserName,
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

                            break;
                    }

                    return false;
                }

                // Check posts for urls if the user has only x posts
                if (YafContext.Current.CurrentUserData.NumPosts
                    <= YafContext.Current.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount &&
                    !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                {
                    var urlCount = UrlHelper.CountUrls(message);

                    if (urlCount > this.PageContext.BoardSettings.AllowedNumberOfUrls)
                    {
                        spamResult = "The user posted {0} urls but allowed only {1}".FormatWith(
                            urlCount,
                            this.PageContext.BoardSettings.AllowedNumberOfUrls);

                        switch (this.Get<YafBoardSettings>().SpamMessageHandling)
                        {
                            case 0:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}".FormatWith(
                                        this.PageContext.PageUserName,
                                        spamResult),
                                    EventLogTypes.SpamMessageDetected);
                                break;
                            case 1:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post"
                                        .FormatWith(
                                            this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                                            spamResult),
                                    EventLogTypes.SpamMessageDetected);
                                break;
                            case 2:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}, post was rejected"
                                        .FormatWith(
                                            this.PageContext.PageUserName,
                                            spamResult),
                                    EventLogTypes.SpamMessageDetected);

                                this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                                break;
                            case 3:
                                this.Logger.Log(
                                    this.PageContext.PageUserID,
                                    "Spam Message Detected",
                                    "Spam Check detected possible SPAM ({1}) posted by User: {0}, user was deleted and bannded"
                                        .FormatWith(
                                            this.PageContext.PageUserName,
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

                                break;
                        }

                        return false;
                    }
                }

                return true;
            }

            ///////////////////////////////


            // test sending user's PM count
            // get user's name
            var drPMInfo = LegacyDb.user_pmcount(YafContext.Current.PageUserID).Rows[0];

            if ((drPMInfo["NumberTotal"].ToType<int>() + count <= drPMInfo["NumberAllowed"].ToType<int>())
                || YafContext.Current.IsAdmin)
            {
                return true;
            }

            // user has full PM box
            YafContext.Current.AddLoadMessage(
                this.GetTextFormatted("OWN_PMBOX_FULL", drPMInfo["NumberAllowed"]),
                MessageTypes.danger);

            return false;
        }

        #endregion
    }
}