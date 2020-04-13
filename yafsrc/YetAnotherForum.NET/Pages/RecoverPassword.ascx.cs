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
    using System.Linq;
    using System.Net.Mail;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The recover Password Page.
    /// </summary>
    public partial class RecoverPassword : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "RecoverPassword" /> class.
        /// </summary>
        public RecoverPassword()
            : base("RECOVER_PASSWORD")
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsProtected.
        /// </summary>
        public override bool IsProtected => false;

        #endregion

        #region Methods

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

            this.PasswordRecovery1.MembershipProvider = Config.MembershipProvider;

            // handle localization
            var usernameRequired =
                this.PasswordRecovery1.UserNameTemplateContainer.FindControlAs<RequiredFieldValidator>(
                    "UserNameRequired");
            var answerRequired =
                this.PasswordRecovery1.QuestionTemplateContainer
                    .FindControlAs<RequiredFieldValidator>("AnswerRequired");

            usernameRequired.ToolTip = usernameRequired.ErrorMessage = this.GetText("REGISTER", "NEED_USERNAME");
            answerRequired.ToolTip = answerRequired.ErrorMessage = this.GetText("REGISTER", "NEED_ANSWER");

            this.PasswordRecovery1.UserNameTemplateContainer.FindControlAs<Button>("SubmitButton").Text =
                this.GetText("SUBMIT");
            this.PasswordRecovery1.QuestionTemplateContainer.FindControlAs<Button>("SubmitButton").Text =
                this.GetText("SUBMIT");

            this.PasswordRecovery1.UserNameFailureText = this.GetText("USERNAME_FAILURE");
            this.PasswordRecovery1.GeneralFailureText = this.GetText("GENERAL_FAILURE");
            this.PasswordRecovery1.QuestionFailureText = this.GetText("QUESTION_FAILURE");

            this.DataBind();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        /// <summary>
        /// Handles the AnswerLookupError event of the PasswordRecovery1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void PasswordRecovery1_AnswerLookupError([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.LoadMessage.AddSession(this.GetText("QUESTION_FAILURE"), MessageTypes.danger);
        }

        /// <summary>
        /// Handles the SendMailError event of the PasswordRecovery1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SendMailErrorEventArgs"/> instance containing the event data.</param>
        protected void PasswordRecovery1_SendMailError([NotNull] object sender, [NotNull] SendMailErrorEventArgs e)
        {
            // it will fail to send the message...
            e.Handled = true;
        }

        /// <summary>
        /// Handles the SendingMail event of the PasswordRecovery1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MailMessageEventArgs"/> instance containing the event data.</param>
        protected void PasswordRecovery1_SendingMail([NotNull] object sender, [NotNull] MailMessageEventArgs e)
        {
            // get the username and password from the body
            var body = e.Message.Body;

            // remove first line...
            body = body.Remove(0, body.IndexOf('\n') + 1);

            // remove "Username: "
            body = body.Remove(0, body.IndexOf(": ", StringComparison.Ordinal) + 2);

            // get first line which is the username
            var userName = body.Substring(0, body.IndexOf('\n'));

            // delete that same line...
            body = body.Remove(0, body.IndexOf('\n') + 1);

            // remove the "Password: " part
            body = body.Remove(0, body.IndexOf(": ", StringComparison.Ordinal) + 2);

            // the rest is the password...
            var password = body.Substring(0, body.IndexOf('\n'));

            var subject = this.GetTextFormatted("PASSWORDRETRIEVAL_EMAIL_SUBJECT", this.Get<BoardSettings>().Name);

            var userIpAddress = this.Get<HttpRequestBase>().GetUserRealIPAddress();

            // get the e-mail ready from the real template.
            var passwordRetrieval = new TemplateEmail("PASSWORDRETRIEVAL")
                                        {
                                            TemplateParams =
                                                {
                                                    ["{username}"] = userName,
                                                    ["{password}"] = password,
                                                    ["{ipaddress}"] = userIpAddress
                                                }
                                        };

            passwordRetrieval.SendEmail(e.Message.To[0], subject);

            // log password reset attempt
            this.Logger.Log(
                userName,
                $"{userName} Requested a Password Reset",
                $"The user {userName} with the IP address: '{userIpAddress}' requested a password reset.",
                EventLogTypes.Information);

            // manually set to success...
            e.Cancel = true;
            this.PasswordRecovery1.TabIndex = 3;
        }

        /// <summary>
        /// The password recovery 1_ verifying answer.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_VerifyingAnswer([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
        {
            // needed to handle event
        }

        /// <summary>
        /// The password recovery 1_ verifying user.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PasswordRecovery1_VerifyingUser([NotNull] object sender, [NotNull] LoginCancelEventArgs e)
        {
            MembershipUser user = null;

            if (this.PasswordRecovery1.UserName.Contains("@") && this.Get<MembershipProvider>().RequiresUniqueEmail)
            {
                // Email Login
                var username = this.Get<MembershipProvider>().GetUserNameByEmail(this.PasswordRecovery1.UserName);
                if (username != null)
                {
                    user = this.Get<MembershipProvider>().GetUser(username, false);

                    // update the username
                    this.PasswordRecovery1.UserName = username;
                }
            }
            else
            {
                // Standard user name login
                if (this.Get<BoardSettings>().EnableDisplayName)
                {
                    // Display name login
                    var id = this.Get<IUserDisplayName>().GetId(this.PasswordRecovery1.UserName);

                    if (id.HasValue)
                    {
                        // get the username associated with this id...
                        var username = UserMembershipHelper.GetUserNameFromID(id.Value);

                        // update the username
                        this.PasswordRecovery1.UserName = username;
                    }

                    user = this.Get<MembershipProvider>().GetUser(this.PasswordRecovery1.UserName, false);
                }
            }

            if (user == null)
            {
                return;
            }

            // verify the user is approved, etc...
            if (user.IsApproved)
            {
                return;
            }

            if (this.Get<BoardSettings>().EmailVerification)
            {
                // get the hash from the db associated with this user...
                var checkTyped = this.GetRepository<CheckEmail>().ListTyped(user.Email).FirstOrDefault();

                if (checkTyped != null)
                {
                    // re-send verification email instead of lost password...
                    var verifyEmail = new TemplateEmail("VERIFYEMAIL");

                    var subject = this.GetTextFormatted(
                        "VERIFICATION_EMAIL_SUBJECT",
                        this.Get<BoardSettings>().Name);

                    verifyEmail.TemplateParams["{link}"] = BuildLink.GetLinkNotEscaped(
                        ForumPages.Approve,
                        true,
                        "k={0}",
                        checkTyped.Hash);
                    verifyEmail.TemplateParams["{key}"] = checkTyped.Hash;
                    verifyEmail.TemplateParams["{forumname}"] = this.Get<BoardSettings>().Name;
                    verifyEmail.TemplateParams["{forumlink}"] = $"{BoardInfo.ForumURL}";

                    verifyEmail.SendEmail(new MailAddress(user.Email, user.UserName), subject);

                    this.PageContext.LoadMessage.AddSession(
                        this.GetTextFormatted("ACCOUNT_NOT_APPROVED_VERIFICATION", user.Email),
                        MessageTypes.warning);
                }
            }
            else
            {
                // explain they are not approved yet...
                this.PageContext.LoadMessage.AddSession(this.GetText("ACCOUNT_NOT_APPROVED"), MessageTypes.warning);
            }

            // just in case cancel the verification...
            e.Cancel = true;

            // nothing they can do here... redirect to login...
            BuildLink.Redirect(ForumPages.Login);
        }

        /// <summary>
        /// The submit button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void SubmitButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Login);
        }

        #endregion
    }
}