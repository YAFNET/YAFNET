/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BaseControls;
    using YAF.Core.BaseModules;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Web.Controls;
    using YAF.Web.Editors;

    #endregion

    /// <summary>
    /// The edit users signature.
    /// </summary>
    public partial class EditUsersSignature : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _sig.
        /// </summary>
        private ForumEditor signatureEditor;

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
            get => this.ViewState["ShowHeader"] == null || Convert.ToBoolean(this.ViewState["ShowHeader"]);

            set => this.ViewState["ShowHeader"] = value;
        }

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        public int CurrentUserID
        {
            get
            {
                if (this.PageContext.CurrentForumPage.IsAdminPage && this.PageContext.IsAdmin
                                                                  && this.Get<HttpRequestBase>().QueryString.Exists("u"))
                {
                    return this.Get<LinkBuilder>().StringToIntOrRedirect(
                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
                }

                if (this.InModeratorMode && (this.PageContext.IsAdmin || this.PageContext.IsForumModerator)
                                         && this.Get<HttpRequestBase>().QueryString.Exists("u"))
                {
                    return this.Get<LinkBuilder>().StringToIntOrRedirect(
                        this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));
                }

                return this.PageContext.PageUserID;
            }
        }

        /// <summary>
        /// Gets or sets the string with allowed BBCodes info.
        /// </summary>
        /// <value>
        /// The allowed BBCodes.
        /// </value>
        private string AllowedBBCodes
        {
            get => this.ViewState["AllowedBBCodes"].ToString();

            set => this.ViewState["AllowedBBCodes"] = value;
        }

        /// <summary>
        ///   Gets or sets the number of characters which is allowed in user signature.
        /// </summary>
        private int AllowedNumberOfCharacters
        {
            get => this.ViewState["AllowedNumberOfCharacters"].ToType<int>();

            set => this.ViewState["AllowedNumberOfCharacters"] = value;
        }

        /// <summary>
        /// Gets the User Data.
        /// </summary>
        [NotNull]
        private User user
        {
            get => this.ViewState["User"].ToType<User>();

            set => this.ViewState["User"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        protected void BindData()
        {
            if (!this.Visible)
            {
                return;
            }

            this.user =
                this.PageContext.CurrentForumPage.IsAdminPage ||
                this.PageContext.ForumPageType == ForumPages.UserProfile
                    ? this.GetRepository<User>().GetById(this.CurrentUserID)
                    : this.PageContext.User;

            this.signatureEditor.Text = this.user.Signature;

            this.signaturePreview.Signature = this.signatureEditor.Text;
            this.signaturePreview.DisplayUserID = this.CurrentUserID;

            var data = this.GetRepository<User>().SignatureData(this.CurrentUserID, this.PageContext.PageBoardID);

            if (data == null)
            {
                return;
            }

            this.AllowedBBCodes = (string)data.UsrSigBBCodes.Trim().Trim(',').Trim();

            this.AllowedNumberOfCharacters = (int)data.UsrSigChars;

            this.signatureEditor.MaxCharacters = this.AllowedNumberOfCharacters;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.signatureEditor = new CKEditorBBCodeEditorBasic
            {
                UserCanUpload = false,
                MaxCharacters = this.AllowedNumberOfCharacters
            };

            this.EditorLine.Controls.Add(this.signatureEditor);

            this.signaturePreview = new SignaturePreview();
            this.PreviewLine.Controls.Add(this.signaturePreview);

            this.save.Click += this.Save_Click;
            this.preview.Click += this.Preview_Click;
            this.cancel.Click += this.Cancel_Click;

            base.OnInit(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

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
        /// The do redirect.
        /// </summary>
        private void DoRedirect()
        {
            if (this.InModeratorMode)
            {
                this.Get<LinkBuilder>().Redirect(
                    ForumPages.UserProfile,
                    "u={0}&name={1}",
                    this.CurrentUserID,
                    this.user.DisplayOrUserName());
            }
            else
            {
                this.Get<LinkBuilder>().Redirect(ForumPages.MyAccount);
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
            var body = this.signatureEditor.Text;

            // find forbidden BBCodes in signature
            var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this.AllowedBBCodes, ',');

            if (this.AllowedBBCodes.IndexOf("ALL", StringComparison.Ordinal) < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode),
                        MessageTypes.warning);
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"), MessageTypes.warning);
                    return;
                }
            }

            if (this.signatureEditor.Text.Length > 0)
            {
                if (this.signatureEditor.Text.Length <= this.AllowedNumberOfCharacters)
                {
                    if (this.user.NumPosts < this.PageContext.BoardSettings.IgnoreSpamWordCheckPostCount)
                    {
                        // Check for spam
                        if (this.Get<ISpamWordCheck>().CheckForSpamWord(body, out var result))
                        {
                            switch (this.PageContext.BoardSettings.BotHandlingOnRegister)
                            {
                                // Log and Send Message to Admins
                                case 1:
                                    this.Logger.SpamBotDetected(
                                        this.user.ID,
                                        $@"Internal Spam Word Check detected a SPAM BOT: (
                                                      user name : '{this.user.Name}', 
                                                      user id : '{this.CurrentUserID}') 
                                                 after the user included a spam word in his/her signature: {result}");
                                    break;
                                case 2:
                                    {
                                        this.Logger.SpamBotDetected(
                                            this.user.ID,
                                            $@"Internal Spam Word Check detected a SPAM BOT: (
                                                       user name : '{this.user.Name}', 
                                                       user id : '{this.CurrentUserID}') 
                                                 after the user included a spam word in his/her signature: {result}, user was deleted and the name, email and IP Address are banned.");

                                        // Kill user
                                        if (!this.PageContext.CurrentForumPage.IsAdminPage)
                                        {
                                            var membershipUser = this.Get<IAspNetUsersHelper>()
                                                .GetMembershipUserById(this.user.ID);
                                            this.Get<IAspNetUsersHelper>().DeleteAndBanUser(
                                                this.CurrentUserID,
                                                membershipUser,
                                                this.user.IP);
                                        }

                                        break;
                                    }
                            }
                        }
                    }

                    this.GetRepository<User>().SaveSignature(
                        this.CurrentUserID,
                        this.Get<IBadWordReplace>().Replace(body));
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
                        MessageTypes.warning);

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
            var body = this.signatureEditor.Text;

            // find forbidden BBCodes in signature
            var detectedBbCode = this.Get<IFormatMessage>().BBCodeForbiddenDetector(body, this.AllowedBBCodes, ',');

            if (this.AllowedBBCodes.IndexOf("ALL", StringComparison.Ordinal) < 0)
            {
                if (detectedBbCode.IsSet() && detectedBbCode != "ALL")
                {
                    this.PageContext.AddLoadMessage(
                        this.GetTextFormatted("SIGNATURE_BBCODE_WRONG", detectedBbCode),
                        MessageTypes.warning);
                    return;
                }

                if (detectedBbCode.IsSet() && detectedBbCode == "ALL")
                {
                    this.PageContext.AddLoadMessage(this.GetText("BBCODE_FORBIDDEN"), MessageTypes.warning);
                    return;
                }
            }

            if (this.signatureEditor.Text.Length <= this.AllowedNumberOfCharacters)
            {
                this.signaturePreview.Signature = this.Get<IBadWordReplace>().Replace(body);
                this.signaturePreview.DisplayUserID = this.CurrentUserID;
            }
            else
            {
                this.PageContext.AddLoadMessage(
                    this.GetTextFormatted("SIGNATURE_MAX", this.AllowedNumberOfCharacters),
                    MessageTypes.warning);
            }

            this.signatureEditor.MaxCharacters = this.AllowedNumberOfCharacters;
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