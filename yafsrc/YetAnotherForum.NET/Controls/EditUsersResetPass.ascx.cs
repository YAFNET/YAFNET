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

namespace YAF.Controls;

using System.Net.Mail;

using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The edit users reset pass.
/// </summary>
public partial class EditUsersResetPass : BaseUserControl
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    
    public Tuple<User, AspNetUsers, Rank, VAccess> User { get; set; }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ChangePassword.Text =
            $"<i class=\"fa fa-key fa-fw\"></i>&nbsp;{this.GetText("ADMIN_EDITUSER", "CHANGE_PASS")}";

        if (this.IsPostBack)
        {
            return;
        }

        this.lblPassRequirements.Text = this.Get<ILocalization>().GetTextFormatted(
            "PASS_REQUIREMENT",
            this.PageBoardContext.BoardSettings.MinRequiredPasswordLength,
            1);

        this.PasswordValidator.ErrorMessage =
            this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_NEW_PASS");
        this.RequiredFieldValidator1.ErrorMessage =
            this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_CONFIRM_PASS");
        this.CompareValidator1.ErrorMessage =
            this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_PASS_NOTMATCH");
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
    protected void ChangePassword_Click(object sender, EventArgs e)
    {
        this.Page.Validate();

        if (!this.Page.IsValid)
        {
            return;
        }

        // change password...
        try
        {
            var newPass = this.txtNewPassword.Text.Trim();

            var token = this.Get<IAspNetUsersHelper>().GeneratePasswordResetToken(this.User.Item2.Id);

            var result = this.Get<IAspNetUsersHelper>().ResetPassword(this.User.Item2.Id, token, newPass);

            if (result.Succeeded)
            {
                if (this.chkEmailNotify.Checked)
                {
                    var subject = this.GetTextFormatted(
                        "PASSWORDRETRIEVAL_EMAIL_SUBJECT",
                        this.Get<BoardSettings>().Name);

                    // email a notification...
                    var passwordRetrieval = new TemplateEmail("PASSWORDRETRIEVAL_ADMIN")
                                            {
                                                TemplateParams =
                                                {
                                                    ["{username}"] = this.User.Item1.DisplayOrUserName(),
                                                    ["{password}"] = newPass
                                                }
                                            };

                    passwordRetrieval.SendEmail(new MailAddress(this.User.Item1.Email, this.User.Item1.DisplayOrUserName()), subject);

                    this.PageBoardContext.Notify(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED_NOTI"),
                        MessageTypes.success);
                }
                else
                {
                    this.PageBoardContext.Notify(
                        this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED"),
                        MessageTypes.success);
                }

                return;
            }

            this.PageBoardContext.Notify(result.Errors.FirstOrDefault(), MessageTypes.danger);
        }
        catch (Exception x)
        {
            this.PageBoardContext.Notify($"Exception: {x.Message}", MessageTypes.danger);
        }
    }
}