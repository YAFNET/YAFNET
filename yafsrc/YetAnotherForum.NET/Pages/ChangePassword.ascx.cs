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
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Change Password Page.
    /// </summary>
    public partial class ChangePassword : ForumPageRegistered
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePassword"/> class.
        /// </summary>
        public ChangePassword()
            : base("CHANGE_PASSWORD")
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
            BuildLink.Redirect(ForumPages.Account);
        }

        /// <summary>
        /// The continue push button_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void ContinuePushButton_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Account);
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
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (Config.IsDotNetNuke)
            {
                // Not accessible...
                BuildLink.AccessDenied();
            }

            if (!this.Get<BoardSettings>().AllowPasswordChange
                && !(this.PageContext.IsAdmin || this.PageContext.IsForumModerator))
            {
                // Not accessible...
                BuildLink.AccessDenied();
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
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<Button>("ChangePasswordPushButton");

            changeButton.Text = this.GetText("CHANGE_BUTTON");
            this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<Button>("CancelPushButton").Text =
                this.GetText("CANCEL");

            // make failure text...
            // 1. Password incorrect or New Password invalid.
            // 2. New Password length minimum: {0}.t
            // 3. Non-alphanumeric characters required: {1}.
            var failureText = "<div class=\"alert alert-danger col-12\" role=\"alert\">";
            failureText += "<ul>";
            failureText += $"<li>{this.GetText("PASSWORD_INCORRECT")}</li>";
            failureText += $"<li>{this.GetText("PASSWORD_BAD_LENGTH")}</li>";
            failureText += $"<li>{this.GetText("PASSWORD_NOT_COMPLEX")}</li>";
            failureText += "</ul>";
            failureText += "</div>";

            this.ChangePassword1.ChangePasswordFailureText = failureText;

            var currentPassword =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("CurrentPassword");
            var newPassword = this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("NewPassword");
            var confirmNewPassword =
                this.ChangePassword1.ChangePasswordTemplateContainer.FindControlAs<TextBox>("ConfirmNewPassword");

            currentPassword.Attributes.Add("onKeyPress", $"doClick('{changeButton.ClientID}',event)");

            newPassword.Attributes.Add("onKeyPress", $"doClick('{changeButton.ClientID}',event)");

            confirmNewPassword.Attributes.Add("onKeyPress", $"doClick('{changeButton.ClientID}',event)");

            if (this.Get<MembershipProvider>().RequiresQuestionAndAnswer)
            {
                this.QuestionTab.Visible = true;
                this.QuestionLink.Visible = true;

                this.QuestionOld.Text = this.PageContext.CurrentUserData.Membership.PasswordQuestion;
            }

            this.DataBind();
        }

        /// <summary>
        /// Create the Page links.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.Get<BoardSettings>().EnableDisplayName
                    ? this.PageContext.CurrentUserData.DisplayName
                    : this.PageContext.PageUserName,
                BuildLink.GetLink(ForumPages.Account));
            this.PageLinks.AddLink(this.GetText("TITLE"));
        }

        #endregion
    }
}