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
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Net.Mail;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Services;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The edit users reset pass.
    /// </summary>
    public partial class EditUsersResetPass : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets CurrentUserID.
        /// </summary>
        public long? CurrentUserID => this.PageContext.QueryIDs["u"];

        #endregion

        #region Methods

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            if (!this.PageContext.IsAdmin)
            {
                YafBuildLink.AccessDenied();
            }

            if (!this.IsPostBack)
            {
                this.rblPasswordResetFunction.Items.Add(
                    new ListItem(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "PASS_OPTION_RESET"),
                        "reset",
                        true));
                this.rblPasswordResetFunction.Items.Add(
                    new ListItem(this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "PASS_OPTION_CHANGE"), "change"));

                this.rblPasswordResetFunction.SelectedIndex = 0;

                this.btnResetPassword.Text =
                    $"<i class=\"fa fa-sync fa-fw\"></i>&nbsp;{this.GetText("ADMIN_EDITUSER", "RESET_PASS")}";
                this.btnChangePassword.Text =
                    $"<i class=\"fa fa-key fa-fw\"></i>&nbsp;{this.GetText("ADMIN_EDITUSER", "CHANGE_PASS")}";

                this.lblPassRequirements.Text = this.Get<ILocalization>().GetTextFormatted(
                    "PASS_REQUIREMENT",
                    this.Get<MembershipProvider>().MinRequiredPasswordLength,
                    this.Get<MembershipProvider>().MinRequiredNonAlphanumericCharacters);

                this.PasswordValidator.ErrorMessage =
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_NEW_PASS");
                this.RequiredFieldValidator1.ErrorMessage =
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_CONFIRM_PASS");
                this.CompareValidator1.ErrorMessage =
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_PASS_NOTMATCH");

                if (this.Get<MembershipProvider>().EnablePasswordReset)
                {
                    return;
                }

                this.PasswordResetErrorHolder.Visible = true;
                this.btnResetPassword.Enabled = false;
                this.rblPasswordResetFunction.Enabled = false;
            }
            else
            {
                this.btnResetPassword.Text =
                    $"<i class=\"fa fa-sync fa-fw\"></i>&nbsp;{this.GetText("ADMIN_EDITUSER", "RESET_PASS")}";
                this.btnChangePassword.Text =
                    $"<i class=\"fa fa-exchange fa-fw\"></i>&nbsp;{this.GetText("ADMIN_EDITUSER", "CHANGE_PASS")}";
            }
        }

        /// <summary>
        /// The toggle change pass UI enabled.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        protected void ToggleChangePassUIEnabled(bool status)
        {
            this.ChangePasswordHolder.Visible = status;
            this.btnChangePassword.Visible = status;
            this.btnResetPassword.Visible = !status;
        }

        /// <summary>
        /// Change the Users Password
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnChangePassword_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Page.Validate();

            if (!this.Page.IsValid)
            {
                return;
            }

            // change password...
            try
            {
                var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID.Value);

                if (user == null)
                {
                    return;
                }

                // new password...
                var newPass = this.txtNewPassword.Text.Trim();

                // reset the password...
                user.UnlockUser();
                var tempPass = user.ResetPassword();

                // change to new password...
                user.ChangePassword(tempPass, newPass);

                if (this.chkEmailNotify.Checked)
                {
                    var subject = this.GetTextFormatted(
                        "PASSWORDRETRIEVAL_EMAIL_SUBJECT",
                        this.Get<YafBoardSettings>().Name);

                    // email a notification...
                    var passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL_ADMIN")
                                                {
                                                    TemplateParams =
                                                        {
                                                            ["{username}"] = user.UserName,
                                                            ["{password}"] = newPass
                                                        }
                                                };

                    passwordRetrieval.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

                    this.PageContext.AddLoadMessage(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED_NOTI"),
                        MessageTypes.success);
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED"),
                        MessageTypes.success);
                }
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage($"Exception: {x.Message}", MessageTypes.danger);
            }
        }

        /// <summary>
        /// Reset the User Password
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void btnResetPassword_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            // reset password...
            try
            {
                var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserID.Value);

                if (user == null)
                {
                    return;
                }

                // reset the password...
                user.UnlockUser();
                var newPassword = user.ResetPassword();

                var subject = this.GetTextFormatted(
                    "PASSWORDRETRIEVAL_EMAIL_SUBJECT",
                    this.Get<YafBoardSettings>().Name);
                var logoUrl =
                    $"{YafForumInfo.ForumClientFileRoot}{YafBoardFolders.Current.Logos}/{this.PageContext.BoardSettings.ForumLogo}";
                var themeCss =
                    $"{this.Get<YafBoardSettings>().BaseUrlMask}{this.Get<ITheme>().BuildThemePath("bootstrap-forum.min.css")}";

                // email a notification...
                var passwordRetrieval = new YafTemplateEmail("PASSWORDRETRIEVAL_ADMIN")
                                            {
                                                TemplateParams =
                                                    {
                                                        ["{username}"] = user.UserName,
                                                        ["{password}"] = newPassword,
                                                        ["{forumname}"] = this.Get<YafBoardSettings>().Name,
                                                        ["{forumlink}"] = YafForumInfo.ForumURL,
                                                        ["{themecss}"] = themeCss,
                                                        ["{logo}"] =
                                                            $"{this.Get<YafBoardSettings>().BaseUrlMask}{logoUrl}"
                                                    }
                                            };

                passwordRetrieval.SendEmail(new MailAddress(user.Email, user.UserName), subject, true);

                this.PageContext.AddLoadMessage(
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_RESET"),
                    MessageTypes.success);
            }
            catch (Exception x)
            {
                this.PageContext.AddLoadMessage($"Exception: {x.Message}", MessageTypes.danger);
            }
        }

        /// <summary>
        /// The rbl password reset function_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void rblPasswordResetFunction_SelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.ToggleChangePassUIEnabled(this.rblPasswordResetFunction.SelectedValue == "change");
        }

        #endregion
    }
}