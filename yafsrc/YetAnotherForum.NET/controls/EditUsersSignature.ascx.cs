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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Text;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Editors;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users signature.
    /// </summary>
    public partial class EditUsersSignature : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The string with allowed BBCodes info.
        /// </summary>
        private string _allowedBbcodes;

        /// <summary>
        ///   The string with allowed HTML tags info.
        /// </summary>
        private string _allowedHtml;

        /// <summary>
        ///   The number of characters which is allowed in user signature.
        /// </summary>
        private int _allowedNumberOfCharacters;

        /// <summary>
        ///   The _sig.
        /// </summary>
        private ForumEditor _sig;

        /// <summary>
        ///  The signature Preview
        /// </summary>
        private SignaturePreview signaturePreview;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether InModeratorMode.
        /// </summary>
        public bool InModeratorMode { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether ShowHeader.
        /// </summary>
        public bool ShowHeader
        {
            get
            {
                return this.ViewState["ShowHeader"] == null || Convert.ToBoolean(this.ViewState["ShowHeader"]);
            }

            set
            {
                this.ViewState["ShowHeader"] = value;
            }
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        public int CurrentUserID
        {
            get
            {
                if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin && this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return this.PageContext.QueryIDs["u"].ToType<int>();
                }

                if (this.InModeratorMode && (this.PageContext.IsAdmin || this.PageContext.IsForumModerator) &&
                    this.PageContext.QueryIDs.ContainsKey("u"))
                {
                    return this.PageContext.QueryIDs["u"].ToType<int>();
                }

                return this.PageContext.PageUserID;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        protected void BindData()
        {
            this._sig.Text = this.GetRepository<User>().GetSignature(this.CurrentUserID);

            this.signaturePreview.Signature = this._sig.Text;
            this.signaturePreview.DisplayUserID = this.CurrentUserID;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            // since signatures are so small only allow YafBBCode in them...
            this._sig = new BBCodeEditor { UserCanUpload = false };

            this.EditorLine.Controls.Add(this._sig);

            this.signaturePreview = new SignaturePreview();
            this.PreviewLine.Controls.Add(this.signaturePreview);
       
            this.save.Click += this.Save_Click;
            this.preview.Click += this.Preview_Click;
            this.cancel.Click += this.Cancel_Click;

            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper("u");

            this._sig.BaseDir = "{0}Scripts".FormatWith(YafForumInfo.ForumClientFileRoot);
            this._sig.StyleSheet = this.Get<ITheme>().BuildThemePath("theme.css");

            var sigData = LegacyDb.user_getsignaturedata(this.CurrentUserID, this.PageContext.PageBoardID);

            if (sigData.HasRows())
            {
                this._allowedBbcodes = sigData.Rows[0]["UsrSigBBCodes"].ToString().Trim().Trim(',').Trim();

                this._allowedHtml = sigData.Rows[0]["UsrSigHTMLTags"].ToString().Trim().Trim(',').Trim();

                this._allowedNumberOfCharacters = sigData.Rows[0]["UsrSigChars"].ToType<int>();
            }

            if (this.IsPostBack)
            {
                return;
            }

            this.save.Text = "<i class=\"fa fa-save fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "SAVE"));
            this.preview.Text = "<i class=\"fa fa-image fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "PREVIEW"));
            this.cancel.Text = "<i class=\"fa fa-trash fa-fw\"></i>&nbsp;{0}".FormatWith(this.GetText("COMMON", "CANCEL"));

            var warningMessage = new StringBuilder();

            warningMessage.Append("<ul>");

            if (this._allowedBbcodes.IsSet())
            {
                warningMessage.AppendFormat(
                  "<li>{0}</li>",
                  this._allowedBbcodes.Contains("ALL")
                    ? this.GetText("BBCODE_ALLOWEDALL")
                    : this.GetTextFormatted("BBCODE_ALLOWEDLIST", this._allowedBbcodes));
            }
            else
            {
                warningMessage.AppendFormat("<li>{0}</li>", this.GetText("BBCODE_FORBIDDEN"));
            }

            if (this._allowedHtml.IsSet())
            {
                warningMessage.AppendFormat(
                  "<li>{0}</li>",
                  this._allowedHtml.Contains("ALL")
                    ? this.GetText("HTML_ALLOWEDALL")
                    : this.GetTextFormatted("HTML_ALLOWEDLIST", this._allowedHtml));
            }
            else
            {
                warningMessage.AppendFormat("<li>{0}</li>", this.GetText("HTML_FORBIDDEN"));
            }

            warningMessage.AppendFormat(
                "<li>{0}</li>",
                this._allowedNumberOfCharacters > 0
                    ? this.GetTextFormatted("SIGNATURE_CHARSMAX", this._allowedNumberOfCharacters)
                    : this.GetText("SIGNATURE_NOEDIT"));

            warningMessage.Append("</ul>");

            this.TagsAllowedWarning.Text = warningMessage.ToString();

            this.BindData();
        }

        /// <summary>
        /// The page_ pre render.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.trHeader.Visible = this.ShowHeader;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        ///   the contents of this method with the code editor.
        /// </summary>
        private static void InitializeComponent()
        {
        }

        /// <summary>
        /// The do redirect.
        /// </summary>
        private void DoRedirect()
        {
            if (this.InModeratorMode)
            {
                YafBuildLink.Redirect(ForumPages.profile, "u={0}", this.CurrentUserID);
            }
            else
            {
                YafBuildLink.Redirect(ForumPages.cp_profile);
            }
        }

        /// <summary>
        /// The save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var body = this._sig.Text;

            // find forbidden BBcodes in signature
            var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this._allowedBbcodes, ',');
            if (this._allowedBbcodes.IndexOf("ALL") < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"));
                    return;
                }
            }

            // find forbidden HTMLTags in signature
            if (!this.PageContext.IsAdmin && this._allowedHtml.IndexOf("ALL") < 0)
            {
                var detectedHtmlTag = this.Get<IFormatMessage>().CheckHtmlTags(body, this._allowedHtml, ',');
                if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
                {
                    this.PageContext.AddLoadMessage(detectedHtmlTag);
                    return;
                }

                if (detectedHtmlTag.IsSet() && detectedHtmlTag == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("HTML_FORBIDDEN"));
                    return;
                }
            }

            // body = this.Get<IFormatMessage>().RepairHtml(this,body,false);
            if (this._sig.Text.Length > 0)
            {
                if (this._sig.Text.Length <= this._allowedNumberOfCharacters)
                {
                    var userData = new CombinedUserDataHelper(this.CurrentUserID);

                    if (userData.NumPosts < this.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount)
                    {
                        // Check for spam
                        string result;
                        if (this.Get<ISpamWordCheck>().CheckForSpamWord(body, out result))
                        {
                            var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID);
                            var userId = this.CurrentUserID;

                            // Log and Send Message to Admins
                            if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(1))
                            {
                                this.Logger.Log(
                                    null,
                                    "Bot Detected",
                                    "Internal Spam Word Check detected a SPAM BOT: (user name : '{0}', user id : '{1}') after the user included a spam word in his/her signature: {2}"
                                        .FormatWith(user.UserName, this.CurrentUserID, result),
                                    EventLogTypes.SpamBotDetected);
                            }
                            else if (this.Get<YafBoardSettings>().BotHandlingOnRegister.Equals(2))
                            {
                                this.Logger.Log(
                                    null,
                                    "Bot Detected",
                                    "Internal Spam Word Check detected a SPAM BOT: (user name : '{0}', user id : '{1}') after the user included a spam word in his/her signature: {2}, user was deleted and the name, email and IP Address are banned."
                                        .FormatWith(user.UserName, this.CurrentUserID, result),
                                    EventLogTypes.SpamBotDetected);

                                // Kill user
                                if (!this.PageContext.CurrentForumPage.IsAdminPage)
                                {
                                    var userIp = new CombinedUserDataHelper(user, userId).LastIP;

                                    UserMembershipHelper.DeleteAndBanUser(this.CurrentUserID, user, userIp);
                                }
                            }
                        }
                    }

                    this.GetRepository<User>().SaveSignature(this.CurrentUserID, this.Get<IBadWordReplace>().Replace(body));
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("SIGNATURE_MAX", this._allowedNumberOfCharacters));

                    return;
                }
            }
            else
            {
                this.GetRepository<User>().SaveSignature(this.CurrentUserID, null);
            }

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.CurrentUserID));

            if (this.PageContext.CurrentForumPage.IsAdminPage)
            {
                this.BindData();
            }
            else
            {
                this.DoRedirect();
            }
        }

        /// <summary>
        /// Update the Signature Preview.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Preview_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            var body = this._sig.Text;

            // find forbidden BBcodes in signature
            var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this._allowedBbcodes, ',');

            if (this._allowedBbcodes.IndexOf("ALL") < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                      this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode));
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"));
                    return;
                }
            }

            // find forbidden HTMLTags in signature
            if (!this.PageContext.IsAdmin && this._allowedHtml.IndexOf("ALL") < 0)
            {
                var detectedHtmlTag = this.Get<IFormatMessage>().CheckHtmlTags(body, this._allowedHtml, ',');

                if (detectedHtmlTag.IsSet() && detectedHtmlTag != "ALL")
                {
                    this.PageContext.AddLoadMessage(detectedHtmlTag);
                    return;
                }

                if (detectedHtmlTag.IsSet() && detectedHtmlTag == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("HTML_FORBIDDEN"));
                    return;
                }
            }

            if (this._sig.Text.Length <= this._allowedNumberOfCharacters)
            {
                this.signaturePreview.Signature = this.Get<IBadWordReplace>().Replace(body);
                this.signaturePreview.DisplayUserID = this.CurrentUserID;
            }
            else
            {
                this.PageContext.AddLoadMessage(
                  this.GetTextFormatted("SIGNATURE_MAX", this._allowedNumberOfCharacters));
            }
        }

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.DoRedirect();
        }

        #endregion
    }
}