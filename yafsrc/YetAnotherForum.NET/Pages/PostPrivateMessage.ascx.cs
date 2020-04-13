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
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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
    /// The Post Private Message Page
    /// </summary>
    public partial class PostPrivateMessage : ForumPage
    {
        #region Constants and Fields

        /// <summary>
        ///   message body editor
        /// </summary>
        private ForumEditor editor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "PostPrivateMessage" /> class. 
        ///   Default constructor.
        /// </summary>
        public PostPrivateMessage()
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
            if (this.Get<BoardSettings>().AllowPrivateMessageAttachments)
            {
                this.PageContext.PageElements.AddScriptReference("FileUploadScript");

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
            usersFound.Rows.Cast<DataRow>().ForEach(row =>
            {
                friendsString.AppendFormat("{0};", row["Name"]);
            });

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
            BuildLink.Redirect(ForumPages.PM);
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
            this.AllUsers.Visible = BoardContext.Current.IsAdmin;
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
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));

            // private messages
            this.PageLinks.AddLink(
                this.GetText(ForumPages.PM.ToString(), "TITLE"),
                BuildLink.GetLink(ForumPages.PM));

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
                // need at least 2 letters of user's name
                BoardContext.Current.AddLoadMessage(this.GetText("NEED_MORE_LETTERS"), MessageTypes.warning);
                return;
            }

            // try to find users by user name
            var usersFound = this.Get<IUserDisplayName>().Find(this.To.Text.Trim())
                .Where(u => !u.Block.BlockPMs && u.IsApproved == true).ToList();

            if (usersFound.Count > 0)
            {
                // we found a user(s)
                this.ToList.DataSource = usersFound;
                this.ToList.DataValueField = "ID";
                this.ToList.DataTextField = this.Get<BoardSettings>().EnableDisplayName ? "DisplayName" : "Name";
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
                BoardContext.Current.AddLoadMessage(this.GetText("USER_NOTFOUND"), MessageTypes.danger);
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
            this.editor = ForumEditorHelper.GetCurrentForumEditor();

            this.editor.MaxCharacters = this.PageContext.BoardSettings.MaxPostSize;

            this.EditorLine.Controls.Add(this.editor);

            this.editor.UserCanUpload = this.Get<BoardSettings>().AllowPrivateMessageAttachments;

            // add editor to the page
            this.EditorLine.Controls.Add(this.editor);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // if user isn't authenticated, redirect him to login page
            if (this.User == null || BoardContext.Current.IsGuest)
            {
                this.RedirectNoAccess();
            }

            // this needs to be done just once, not during post-backs
            if (this.IsPostBack)
            {
                return;
            }

            // only administrators can send messages to all users
            this.AllUsers.Visible = BoardContext.Current.IsAdmin;

            this.AllBuddies.Visible = this.PageContext.UserHasBuddies;

            // Is Reply
            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p").IsSet())
            {
                // PM is a reply or quoted reply (isQuoting)
                // to the given message id "p"
                var isQuoting = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("q") == "1";

                var isReport = this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("report") == "1";

                // get quoted message
                var row =
                    this.GetRepository<PMessage>().ListAsDataTable(
                        Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p"))).GetFirstRow();

                // there is such a message
                if (row == null)
                {
                    return;
                }

                // get message sender/recipient
                var toUserId = row["ToUserID"].ToType<int>();
                var fromUserId = row["FromUserID"].ToType<int>();

                // verify access to this PM
                if (toUserId != BoardContext.Current.PageUserID && fromUserId != BoardContext.Current.PageUserID)
                {
                    BuildLink.AccessDenied();
                }

                // handle subject
                var subject = row["Subject"].ToType<string>();
                if (!subject.StartsWith("Re: "))
                {
                    subject = $"Re: {subject}";
                }

                this.PmSubjectTextBox.Text = subject;

                var displayName = this.Get<IUserDisplayName>().GetName(fromUserId);

                // set "To" user and disable changing...
                this.To.Text = displayName;
                this.To.Enabled = false;
                this.FindUsers.Visible = false;
                this.AllUsers.Visible = false;
                this.AllBuddies.Visible = false;

                if (!isQuoting)
                {
                    return;
                }

                // PM is a quoted reply
                var body = row["Body"].ToString();

                if (this.Get<BoardSettings>().RemoveNestedQuotes)
                {
                    body = this.Get<IFormatMessage>().RemoveNestedQuotes(body);
                }

                // Ensure quoted replies have bad words removed from them
                body = this.Get<IBadWordReplace>().Replace(body);

                // Quote the original message
                body = $"[QUOTE={displayName}]{body}[/QUOTE]";

                // we don't want any whitespaces at the beginning of message
                this.editor.Text = body.TrimStart();

                if (!isReport)
                {
                    return;
                }

                var users = this.GetRepository<User>().UserList(
                    BoardContext.Current.PageBoardID,
                    null,
                    true,
                    null,
                    null,
                    null).ToList();

                var hostUser = users.FirstOrDefault(u => u.IsHostAdmin > 0);

                if (hostUser != null)
                {
                    this.To.Text = this.Get<BoardSettings>().EnableDisplayName
                                       ? hostUser.DisplayName
                                       : hostUser.Name;

                    this.PmSubjectTextBox.Text = this.GetTextFormatted("REPORT_SUBJECT", displayName);

                    var bodyReport = $"[QUOTE={displayName}]{row["Body"]}[/QUOTE]";

                    // Quote the original message
                    bodyReport = this.GetTextFormatted("REPORT_BODY", bodyReport);

                    // we don't want any whitespaces at the beginning of message
                    this.editor.Text = bodyReport.TrimStart();
                }
                else
                {
                    BuildLink.AccessDenied();
                }
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
                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"), out _)
                    || !int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r"), out _))
                {
                    return;
                }

                // get quoted message
                var messagesRow =
                        this.GetRepository<Message>().ListReportersAsDataTable(
                            Security.StringToIntOrRedirect(
                                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r")),
                            Security.StringToIntOrRedirect(
                                this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u")))
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
                this.FindUsers.Visible = false;
                this.AllUsers.Visible = false;
                this.AllBuddies.Visible = false;

                // Parse content with delimiter '|'  
                var quoteList = messagesRow.Field<string>("ReportText").Split('|');

                // Quoted replies should have bad words in them
                // Reply to report PM is always a quoted reply
                // Quote the original message in a cycle
                for (var i = 0; i < quoteList.Length; i++)
                {
                    // Add quote codes
                    quoteList[i] = $"[QUOTE={displayName}]{quoteList[i]}[/QUOTE]";

                    // Replace DateTime delimiter '??' by ': ' 
                    // we don't want any whitespaces at the beginning of message
                    this.editor.Text = quoteList[i].Replace("??", ": ") + this.editor.Text.TrimStart();
                }
            }
            else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").IsSet())
            {
                // PM is being send as a reply to a reported post

                // find user
                if (!int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"), out var toUserId))
                {
                    return;
                }

                var currentRow =
                    this.GetRepository<User>().ListAsDataTable(BoardContext.Current.PageBoardID, toUserId, true).GetFirstRow();

                if (currentRow == null)
                {
                    return;
                }

                this.To.Text = this.Get<IUserDisplayName>().GetName(currentRow.Field<int>("UserID"));
                this.To.Enabled = false;

                // hide find user/all users buttons
                this.FindUsers.Visible = false;
                this.AllUsers.Visible = false;
                this.AllBuddies.Visible = false;
            }
            else
            {
                // Blank PM

                // multi-receiver info is relevant only when sending blank PM
                if (this.Get<BoardSettings>().PrivateMessageMaxRecipients <= 1)
                {
                    return;
                }

                // format localized string
                this.MultiReceiverInfo.Text =
                    $"<br />{string.Format(this.GetText("MAX_RECIPIENT_INFO"), this.Get<BoardSettings>().PrivateMessageMaxRecipients)}<br />{this.GetText("MULTI_RECEIVER_INFO")}";

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

            this.PreviewMessagePost.MessageFlags.IsHtml = this.editor.UsesHTML;
            this.PreviewMessagePost.MessageFlags.IsBBCode = this.editor.UsesBBCode;
            this.PreviewMessagePost.Message = this.editor.Text;

            if (!this.Get<BoardSettings>().AllowSignatures)
            {
                return;
            }

            using (
                var userDT = this.GetRepository<User>().ListAsDataTable(
                    BoardContext.Current.PageBoardID,
                    BoardContext.Current.PageUserID,
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
                BoardContext.Current.AddLoadMessage(this.GetText("need_to"), MessageTypes.warning);
                return;
            }

            // subject is required
            if (this.PmSubjectTextBox.Text.Trim().Length <= 0)
            {
                BoardContext.Current.AddLoadMessage(this.GetText("need_subject"), MessageTypes.warning);
                return;
            }

            // message is required
            if (this.editor.Text.Trim().Length <= 0)
            {
                BoardContext.Current.AddLoadMessage(this.GetText("need_message"), MessageTypes.warning);
                return;
            }

            if (this.ToList.SelectedItem != null && this.ToList.SelectedItem.Value == "0")
            {
                // administrator is sending PMs to all users           
                var body = this.editor.Text;
                var messageFlags = new MessageFlags
                                       {
                                           IsHtml = this.editor.UsesHTML,
                                           IsBBCode = this.editor.UsesBBCode
                                       };
                
                // test user's PM count
                if (!this.VerifyMessageAllowed(1, body))
                {
                    return;
                }

                this.GetRepository<PMessage>().SendMessage(
                    BoardContext.Current.PageUserID,
                    0,
                    this.PmSubjectTextBox.Text,
                    body,
                    messageFlags.BitValue,
                    replyTo);

                // redirect to outbox (sent items), not control panel
                BuildLink.Redirect(ForumPages.PM, "v={0}", "out");
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

                if (recipients.Count > this.Get<BoardSettings>().PrivateMessageMaxRecipients
                    && !BoardContext.Current.IsAdmin && this.Get<BoardSettings>().PrivateMessageMaxRecipients != 0)
                {
                    // to many recipients
                    BoardContext.Current.AddLoadMessage(
                        this.GetTextFormatted(
                            "TOO_MANY_RECIPIENTS",
                            this.Get<BoardSettings>().PrivateMessageMaxRecipients),
                        MessageTypes.warning);

                    return;
                }

                if (!this.VerifyMessageAllowed(recipients.Count, this.editor.Text))
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
                        BoardContext.Current.AddLoadMessage(
                            this.GetTextFormatted("NO_SUCH_USER", recipient),
                            MessageTypes.warning);
                        return;
                    }

                    if (UserMembershipHelper.IsGuestUser(userId.Value))
                    {
                        BoardContext.Current.AddLoadMessage(this.GetText("NOT_GUEST"), MessageTypes.danger);
                        return;
                    }

                    // get recipient's ID from the database
                    if (!recipientIds.Contains(userId.Value))
                    {
                        recipientIds.Add(userId.Value);
                    }

                    var receivingPMInfo = this.GetRepository<PMessage>().UserMessageCount(userId.Value).Rows[0];

                    // test receiving user's PM count
                    if (receivingPMInfo["NumberTotal"].ToType<int>() + 1
                        < receivingPMInfo["NumberAllowed"].ToType<int>() || BoardContext.Current.IsAdmin
                        || (bool)
                           Convert.ChangeType(
                               UserMembershipHelper.GetUserRowForID(userId.Value, true)["IsAdmin"],
                               typeof(bool)))
                    {
                        continue;
                    }

                    // recipient has full PM box
                    BoardContext.Current.AddLoadMessage(
                        this.GetTextFormatted("RECIPIENTS_PMBOX_FULL", recipient),
                        MessageTypes.danger);
                    return;
                }

                // send PM to all recipients
                recipientIds.ForEach(
                    userId =>

                        {
                            var body = this.editor.Text;

                            var messageFlags = new MessageFlags
                                                   {
                                                       IsHtml = this.editor.UsesHTML,
                                                       IsBBCode = this.editor.UsesBBCode
                                                   };

                            this.GetRepository<PMessage>().SendMessage(
                                BoardContext.Current.PageUserID,
                                userId,
                                this.PmSubjectTextBox.Text,
                                body,
                                messageFlags.BitValue,
                                replyTo);

                            // reset lazy data as he should be informed at once
                            this.Get<IDataCache>().Remove(string.Format(Constants.Cache.ActiveUserLazyData, userId));

                            if (this.Get<BoardSettings>().AllowPMEmailNotification)
                            {
                                this.Get<ISendNotification>().ToPrivateMessageRecipient(
                                    userId,
                                    this.PmSubjectTextBox.Text.Trim());
                            }
                        });

                // redirect to outbox (sent items), not control panel
                BuildLink.Redirect(ForumPages.PM, "v={0}", "out");
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
            if (!this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess && !this.Get<BoardSettings>().SpamServiceType.Equals(0))
            {
                // Check content for spam
                if (this.Get<ISpamCheck>().CheckPostForSpam(
                    this.PageContext.IsGuest ? "Guest" : this.PageContext.PageUserName,
                    BoardContext.Current.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    message,
                    this.PageContext.User.Email,
                    out var spamResult))
                {
                    switch (this.Get<BoardSettings>().SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}",
                                        this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                string
                                    .Format(
                                        "Spam Check detected possible SPAM ({1}) posted by User: {0}, it was flagged as unapproved post",
                                        this.PageContext.PageUserName,
                                            spamResult),
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 2:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {this.PageContext.PageUserName}, post was rejected",
                                EventLogTypes.SpamMessageDetected);

                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                            break;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {this.PageContext.PageUserName}, user was deleted and bannded",
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
                if (BoardContext.Current.CurrentUserData.NumPosts
                    <= BoardContext.Current.Get<BoardSettings>().IgnoreSpamWordCheckPostCount &&
                    !this.PageContext.IsAdmin && !this.PageContext.ForumModeratorAccess)
                {
                    var urlCount = UrlHelper.CountUrls(message);

                    if (urlCount <= this.PageContext.BoardSettings.AllowedNumberOfUrls)
                    {
                        return true;
                    }

                    spamResult =
                        $"The user posted {urlCount} urls but allowed only {this.PageContext.BoardSettings.AllowedNumberOfUrls}";

                    switch (this.Get<BoardSettings>().SpamMessageHandling)
                    {
                        case 0:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {this.PageContext.PageUserName}",
                                EventLogTypes.SpamMessageDetected);
                            break;
                        case 1:
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
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {this.PageContext.PageUserName}, post was rejected",
                                EventLogTypes.SpamMessageDetected);

                            this.PageContext.AddLoadMessage(this.GetText("SPAM_MESSAGE"), MessageTypes.danger);

                            break;
                        case 3:
                            this.Logger.Log(
                                this.PageContext.PageUserID,
                                "Spam Message Detected",
                                $"Spam Check detected possible SPAM ({spamResult}) posted by User: {this.PageContext.PageUserName}, user was deleted and bannded",
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

                return true;
            }

            ///////////////////////////////

            // test sending user's PM count
            // get user's name
            var drPMInfo = this.GetRepository<PMessage>().UserMessageCount(BoardContext.Current.PageUserID).Rows[0];

            if (drPMInfo["NumberTotal"].ToType<int>() + count <= drPMInfo["NumberAllowed"].ToType<int>()
                || BoardContext.Current.IsAdmin)
            {
                return true;
            }

            // user has full PM box
            BoardContext.Current.AddLoadMessage(
                this.GetTextFormatted("OWN_PMBOX_FULL", drPMInfo["NumberAllowed"]),
                MessageTypes.danger);

            return false;
        }

        #endregion
    }
}