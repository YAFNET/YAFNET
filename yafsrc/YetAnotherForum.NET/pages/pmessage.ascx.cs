﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// Summary description for pmessage.
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
        /// Handles all users button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void AllUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // create one entry to show in dropdown
            var li = new ListItem("All Users", "0");

            // bind the list to dropdown
            this.ToList.Items.Add(li);
            this.ToList.Visible = true;
            this.To.Text = "All Users";

            // hide To text box
            this.To.Visible = false;

            // hide find users/all users buttons
            this.FindUsers.Visible = false;
            this.AllUsers.Visible = false;

            // we need clear button now
            this.Clear.Visible = true;
        }

        /// <summary>
        /// Handles cancel button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // redirect user back to his PM inbox
            YafBuildLink.Redirect(ForumPages.cp_pm);
        }

        /// <summary>
        /// Handles clear button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
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

            // clear button is not necessary now
            this.Clear.Visible = false;
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddLink(this.Get<YafBoardSettings>().Name, YafBuildLink.GetLink(ForumPages.forum));

            // users control panel
            string displayName = UserMembershipHelper.GetDisplayNameFromID(this.PageContext.PageUserID);
            this.PageLinks.AddLink(
              !string.IsNullOrEmpty(displayName) ? displayName : this.PageContext.PageUserName,
              YafBuildLink.GetLink(ForumPages.cp_profile));

            // private messages
            this.PageLinks.AddLink(
              this.GetText(ForumPages.cp_pm.ToString(), "TITLE"),
              YafBuildLink.GetLink(ForumPages.cp_pm));

            // post new message
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        /// <summary>
        /// Handles find users button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void FindUsers_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.To.Text.Length < 2)
            {
                // need at least 2 latters of user's name
                YafContext.Current.AddLoadMessage(this.GetText("NEED_MORE_LETTERS"));
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
                YafContext.Current.AddLoadMessage(this.GetText("USER_NOTFOUND"));
                return;
            }

            // re-bind data to the controls
            this.DataBind();
        }

        /// <summary>
        /// Page initialization handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // create editor based on administrator's settings
            // get the forum editor based on the settings
            string editorId = this.Get<YafBoardSettings>().ForumEditor;
            if (this.Get<YafBoardSettings>().AllowUsersTextEditor)
            {
                // Text editor
                editorId = !string.IsNullOrEmpty(this.PageContext.TextEditor)
                                        ? this.PageContext.TextEditor
                                        : this.Get<YafBoardSettings>().ForumEditor;
            }

            // Check if Editor exists, if not fallback to default editorid=1
            this._editor =
               this.Get<IModuleManager<ForumEditor>>().GetBy(editorId, false) ??
               this.Get<IModuleManager<ForumEditor>>().GetBy("1");

            // Override Editor when mobile device with default Yaf BBCode Editor
            if (PageContext.IsMobileDevice)
            {
                this._editor = this.Get<IModuleManager<ForumEditor>>().GetBy("1");
            }

            // add editor to the page
            this.EditorLine.Controls.Add(this._editor);
        }

        /// <summary>
        /// Handles page load event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // if user isn't authenticated, redirect him to login page
            if (this.User == null || YafContext.Current.IsGuest)
            {
                this.RedirectNoAccess();
            }

            // set attributes of editor
            this._editor.BaseDir = YafForumInfo.ForumClientFileRoot + "editors";
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
            this.Clear.Text = this.GetText("CLEAR");

            // only administrators can send messages to all users
            this.AllUsers.Visible = YafContext.Current.IsAdmin;

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p").IsSet())
            {
                // PM is a reply or quoted reply (isQuoting)
                // to the given message id "p"
                bool isQuoting = this.Request.QueryString.GetFirstOrDefault("q") == "1";

                // get quoted message
                DataRow row =
                    LegacyDb.pmessage_list(Security.StringToLongOrRedirect(this.Request.QueryString.GetFirstOrDefault("p"))).GetFirstRow();

                // there is such a message
                if (row != null)
                {
                    // get message sender/recipient
                    var toUserId = (int)row["ToUserID"];
                    var fromUserId = (int)row["FromUserID"];

                    // verify access to this PM
                    if (toUserId != YafContext.Current.PageUserID && fromUserId != YafContext.Current.PageUserID)
                    {
                        YafBuildLink.AccessDenied();
                    }

                    // handle subject
                    var subject = (string)row["Subject"];
                    if (!subject.StartsWith("Re: "))
                    {
                        subject = "Re: {0}".FormatWith(subject);
                    }

                    this.PmSubjectTextBox.Text = subject;

                    string displayName = this.Get<IUserDisplayName>().GetName(fromUserId);

                    // set "To" user and disable changing...
                    this.To.Text = displayName;
                    this.To.Enabled = false;
                    this.FindUsers.Enabled = false;
                    this.AllUsers.Enabled = false;

                    if (isQuoting)
                    {
                        // PM is a quoted reply
                        string body = row["Body"].ToString();

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
                }
            }
            else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").IsSet() &&
                     this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r").IsSet())
            {
                // We check here if the user have access to the option
                if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
                {
                    // PM is being sent to a predefined user
                    int toUser;
                    int reportMessage;

                    if (int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"), out toUser) &&
                        int.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r"), out reportMessage))
                    {
                        // get quoted message
                        DataRow messagesRow =
                            LegacyDb.message_listreporters(
                                Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("r")).
                                    ToType<int>(),
                                Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u")).
                                    ToType<int>()).GetFirstRow();

                        // there is such a message
                        // message info should be always returned as 1 row 
                        if (messagesRow != null)
                        {
                            // handle subject                                           
                            this.PmSubjectTextBox.Text = this.GetText("REPORTED_SUBJECT");

                            string displayName = this.Get<IUserDisplayName>().GetName(
                                messagesRow.Field<int>("UserID"));

                            // set "To" user and disable changing...
                            this.To.Text = displayName;
                            this.To.Enabled = false;
                            this.FindUsers.Enabled = false;
                            this.AllUsers.Enabled = false;

                            // Parse content with delimiter '|'  
                            string[] quoteList = messagesRow.Field<string>("ReportText").Split('|');

                            // Quoted replies should have bad words in them
                            // Reply to report PM is always a quoted reply
                            // Quote the original message in a cycle
                            for (int i = 0; i < quoteList.Length; i++)
                            {
                                // Add quote codes
                                quoteList[i] = "[QUOTE={0}]{1}[/QUOTE]".FormatWith(displayName, quoteList[i]);

                                // Replace DateTime delimiter '??' by ': ' 
                                // we don't want any whitespaces at the beginning of message
                                this._editor.Text = quoteList[i].Replace("??", ": ") + this._editor.Text.TrimStart();
                            }
                        }
                    }
                }
            }
            else if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u").IsSet())
            {
                // PM is being send as a reply to a reported post

                // find user
                int toUserId;

                if (int.TryParse(this.Request.QueryString.GetFirstOrDefault("u"), out toUserId))
                {
                    DataRow currentRow = LegacyDb.user_list(YafContext.Current.PageBoardID, toUserId, true).GetFirstRow();

                    if (currentRow != null)
                    {
                        this.To.Text = this.Get<IUserDisplayName>().GetName(currentRow.Field<int>("UserID"));
                        this.To.Enabled = false;

                        // hide find user/all users buttons
                        this.FindUsers.Enabled = false;
                        this.AllUsers.Enabled = false;
                    }
                }
            }
            else
            {
                // Blank PM

                // multi-receiver info is relevant only when sending blank PM
                if (this.Get<YafBoardSettings>().PrivateMessageMaxRecipients > 1)
                {
                    // format localized string
                    this.MultiReceiverInfo.Text =
                        "<br />{0}<br />{1}".FormatWith(
                            this.GetText("MAX_RECIPIENT_INFO").FormatWith(
                                this.Get<YafBoardSettings>().PrivateMessageMaxRecipients),
                            this.GetText("MULTI_RECEIVER_INFO"));

                    // display info
                    this.MultiReceiverInfo.Visible = true;
                }
            }
        }

        /// <summary>
        /// Handles preview button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // make preview row visible
            this.PreviewRow.Visible = true;

            this.PreviewMessagePost.MessageFlags.IsHtml = this._editor.UsesHTML;
            this.PreviewMessagePost.MessageFlags.IsBBCode = this._editor.UsesBBCode;
            this.PreviewMessagePost.Message = this._editor.Text;

            // set message flags
            var tFlags = new MessageFlags { IsHtml = this._editor.UsesHTML, IsBBCode = this._editor.UsesBBCode };

            if (!this.Get<YafBoardSettings>().AllowSignatures)
            {
                return;
            }

            using (DataTable userDT = LegacyDb.user_list(YafContext.Current.PageBoardID, YafContext.Current.PageUserID, true))
            {
                if (!userDT.Rows[0].IsNull("Signature"))
                {
                    this.PreviewMessagePost.Signature = userDT.Rows[0]["Signature"].ToString();
                }
            }
        }

        /// <summary>
        /// Handles save button click event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // recipient was set in dropdown
            if (this.ToList.Visible)
            {
                this.To.Text = this.ToList.SelectedItem.Text;
            }

            if (this.To.Text.Length <= 0)
            {
                // recipient is required field
                YafContext.Current.AddLoadMessage(this.GetText("need_to"));
                return;
            }

            // subject is required
            if (this.PmSubjectTextBox.Text.Trim().Length <= 0)
            {
                YafContext.Current.AddLoadMessage(this.GetText("need_subject"));
                return;
            }

            // message is required
            if (this._editor.Text.Trim().Length <= 0)
            {
                YafContext.Current.AddLoadMessage(this.GetText("need_message"));
                return;
            }

            if (this.ToList.SelectedItem != null && this.ToList.SelectedItem.Value == "0")
            {
                // administrator is sending PMs tp all users           
                string body = this._editor.Text;
                var messageFlags = new MessageFlags { IsHtml = this._editor.UsesHTML, IsBBCode = this._editor.UsesBBCode };

                LegacyDb.pmessage_save(YafContext.Current.PageUserID, 0, this.PmSubjectTextBox.Text, body, messageFlags.BitValue);

                // redirect to outbox (sent items), not control panel
                YafBuildLink.Redirect(ForumPages.cp_pm, "v={0}", "out");
            }
            else
            {
                // remove all abundant whitespaces and separators
                this.To.Text.Trim();
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

                if (recipients.Count > this.Get<YafBoardSettings>().PrivateMessageMaxRecipients &&
                    !YafContext.Current.IsAdmin && this.Get<YafBoardSettings>().PrivateMessageMaxRecipients != 0)
                {
                    // to many recipients
                    YafContext.Current.AddLoadMessage(
                      this.GetTextFormatted("TOO_MANY_RECIPIENTS", this.Get<YafBoardSettings>().PrivateMessageMaxRecipients));
                    return;
                }

                // test sending user's PM count
                // get user's name
                DataRow drPMInfo = LegacyDb.user_pmcount(YafContext.Current.PageUserID).Rows[0];

                if ((drPMInfo["NumberTotal"].ToType<int>() > drPMInfo["NumberAllowed"].ToType<int>() + recipients.Count) &&
                    !YafContext.Current.IsAdmin)
                {
                    // user has full PM box
                    YafContext.Current.AddLoadMessage(this.GetTextFormatted("OWN_PMBOX_FULL", drPMInfo["NumberAllowed"]));
                    return;
                }

                // list of recipient's ids
                var recipientIds = new List<int>();

                // get recipients' IDs
                foreach (string recipient in recipients)
                {
                    int? userId = this.Get<IUserDisplayName>().GetId(recipient);

                    if (!userId.HasValue)
                    {
                        YafContext.Current.AddLoadMessage(this.GetTextFormatted("NO_SUCH_USER", recipient));
                        return;
                    }

                    if (UserMembershipHelper.IsGuestUser(userId.Value))
                    {
                        YafContext.Current.AddLoadMessage(this.GetText("NOT_GUEST"));
                        return;
                    }

                    // get recipient's ID from the database
                    if (!recipientIds.Contains(userId.Value))
                    {
                        recipientIds.Add(userId.Value);
                    }

                    // test receiving user's PM count
                    if ((LegacyDb.user_pmcount(userId.Value).Rows[0]["NumberTotal"].ToType<int>() <
                         LegacyDb.user_pmcount(userId.Value).Rows[0]["NumberAllowed"].ToType<int>()) ||
                        YafContext.Current.IsAdmin ||
                        (bool)
                        Convert.ChangeType(
                            UserMembershipHelper.GetUserRowForID(userId.Value, true)["IsAdmin"], typeof(bool)))
                    {
                        continue;
                    }

                    // recipient has full PM box
                    YafContext.Current.AddLoadMessage(this.GetTextFormatted("RECIPIENTS_PMBOX_FULL", recipient));
                    return;
                }

                // send PM to all recipients
                foreach (var userId in recipientIds)
                {
                    string body = this._editor.Text;

                    var messageFlags = new MessageFlags { IsHtml = this._editor.UsesHTML, IsBBCode = this._editor.UsesBBCode };

                    LegacyDb.pmessage_save(
                    YafContext.Current.PageUserID, userId, this.PmSubjectTextBox.Text, body, messageFlags.BitValue);

                    // reset reciever's lazy data as he should be informed at once
                    this.Get<IDataCache>().Remove(
                      Constants.Cache.ActiveUserLazyData.FormatWith(userId));

                    if (this.Get<YafBoardSettings>().AllowPMEmailNotification)
                    {
                        this.Get<ISendNotification>().ToPrivateMessageRecipient(userId, this.PmSubjectTextBox.Text.Trim());
                    }
                }

                // redirect to outbox (sent items), not control panel
                YafBuildLink.Redirect(ForumPages.cp_pm, "v={0}", "out");
            }
        }

        #endregion
    }
}