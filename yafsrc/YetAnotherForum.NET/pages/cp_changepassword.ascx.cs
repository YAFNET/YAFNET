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
    using System.Web.Security;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Change Password Page.
    /// </summary>
    public partial class cp_changepassword : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="cp_changepassword"/> class.
        /// </summary>
        public cp_changepassword()
            : base("CP_CHANGEPASSWORD")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The cancel push button_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void CancelPushButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_profile);
        }

        /// <summary>
        /// The continue push button_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ContinuePushButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.cp_profile);
        }

        /// <summary>
        /// Changes the security question and answer
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void ChangeSecurityAnswerClick(object sender, EventArgs e)
        {
            if (this.AnswerOld.Text.IsSet() && this.AnswerNew.Text.IsSet() && this.QuestionNew.Text.IsSet())
            {
                var securityAndAnswerChanged =
                    this.PageContext.CurrentUserData.Membership.ChangePasswordQuestionAndAnswer(
                        this.AnswerOld.Text,
                        this.QuestionNew.Text,
                        this.AnswerNew.Text);

                if (securityAndAnswerChanged)
                {
                    this.PageContext.AddLoadMessage(this.GetText("SECURITY_CHANGED"), MessageTypes.success);
                }
                else
                {
                    this.PageContext.AddLoadMessage(this.GetText("SECURITY_NOT_CHANGED"), MessageTypes.danger);
                }
            }
            else if (this.AnswerOld.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("EMPTY_PASSWORD"), MessageTypes.warning);
            }
            else if (this.AnswerNew.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("EMPTY_ANSWER"), MessageTypes.warning);
            }
            else if (this.QuestionNew.Text.IsNotSet())
            {
                this.PageContext.AddLoadMessage(this.GetText("EMPTY_QUESTION"), MessageTypes.warning);
            }
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (Config.IsDotNetNuke)
            {
                // Not accessbile...
                YafBuildLink.AccessDenied();
            }

            if (!this.Get<YafBoardSettings>().AllowPasswordChange
                && !(this.PageContext.IsAdmin || this.PageContext.IsForumModerator))
            {
                // Not accessbile...
                YafBuildLink.AccessDenied();
            }

            if (!this.IsPostBack)
            {
                this.PageLinks.AddRoot();
                this.PageLinks.AddLink(
                    this.Get<YafBoardSettings>().EnableDisplayName
                        ? this.PageContext.CurrentUserData.DisplayName
                        : this.PageContext.PageUserName,
                    YafBuildLink.GetLink(ForumPages.cp_profile));
                this.PageLinks.AddLink(this.GetText("TITLE"));
            }

            var oldPasswordRequired =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<RequiredFieldValidator>(
                    "CurrentPasswordRequired");
            var newPasswordRequired =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<RequiredFieldValidator>(
                    "NewPasswordRequired");
            var confirmNewPasswordRequired =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<RequiredFieldValidator>(
                    "ConfirmNewPasswordRequired");
            var passwordsEqual =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<CompareValidator>(
                    "NewPasswordCompare");
            var passwordsNotEqual =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<CompareValidator>(
                    "NewOldPasswordCompare");

            oldPasswordRequired.ToolTip = oldPasswordRequired.ErrorMessage = this.GetText("NEED_OLD_PASSWORD");
            newPasswordRequired.ToolTip = newPasswordRequired.ErrorMessage = this.GetText("NEED_NEW_PASSWORD");
            confirmNewPasswordRequired.ToolTip =
                confirmNewPasswordRequired.ErrorMessage = this.GetText("NEED_NEW_CONFIRM_PASSWORD");
            passwordsEqual.ToolTip = passwordsEqual.ErrorMessage = this.GetText("NO_PASSWORD_MATCH");
            passwordsNotEqual.ToolTip = passwordsNotEqual.ErrorMessage = this.GetText("PASSWORD_NOT_NEW");

            var changeButton =
                (this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<Button>("ChangePasswordPushButton"));

            changeButton.Text = this.GetText("CHANGE_BUTTON");
            (this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<Button>("CancelPushButton")).Text =
                this.GetText("CANCEL");
            (this.ChangePassword1.SuccessTemplateContainer.FindControlAs<Button>("ContinuePushButton")).Text =
                this.GetText("CONTINUE");

            // make failure text...
            // 1. Password incorrect or New Password invalid.
            // 2. New Password length minimum: {0}.t
            // 3. Non-alphanumeric characters required: {1}.
            var failureText = this.GetText("PASSWORD_INCORRECT");
            failureText += "<br />{0}".FormatWith(this.GetText("PASSWORD_BAD_LENGTH"));
            failureText += "<br />{0}".FormatWith(this.GetText("PASSWORD_NOT_COMPLEX"));

            this.ChangePassword1.ChangePasswordFailureText = failureText;

            var currentPassword =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("CurrentPassword");
            var newPassword = this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("NewPassword");
            var confirmNewPassword =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("ConfirmNewPassword");

            currentPassword.Attributes.Add("onKeyPress", "doClick('{0}',event)".FormatWith(changeButton.ClientID));

            newPassword.Attributes.Add("onKeyPress", "doClick('{0}',event)".FormatWith(changeButton.ClientID));

            confirmNewPassword.Attributes.Add("onKeyPress", "doClick('{0}',event)".FormatWith(changeButton.ClientID));

            if (this.Get<MembershipProvider>().RequiresQuestionAndAnswer)
            {
                this.SecurityQuestionAndAnswer.Visible = true;

                this.QuestionOld.Text = this.PageContext.CurrentUserData.Membership.PasswordQuestion;
            }

            this.DataBind();
        }


       #endregion
    }
}