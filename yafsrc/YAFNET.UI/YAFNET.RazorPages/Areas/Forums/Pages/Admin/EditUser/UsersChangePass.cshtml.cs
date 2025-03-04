/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using YAF.Types.Extensions;

namespace YAF.Pages.Admin.EditUser;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MimeKit;

using YAF.Configuration;
using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersChangePassModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersChangePassModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersChangePassInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersChangePassModel"/> class.
    /// </summary>
    public UsersChangePassModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersChangePassInputModel
        {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// Change Password
    /// </summary>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostChangePasswordAsync()
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, this.Input.UserId)] is not
            Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = this.Input.UserId
                });
        }

        this.EditUser = user;

        // change password...
        try
        {
            if(this.Input.NewPassword.IsNotSet())
            {
                return this.PageBoardContext.Notify(
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_PASS_NOTMATCH"),
                    MessageTypes.danger);
            }

            if (this.Input.NewPassword != this.Input.NewPasswordConfirm)
            {
                return this.PageBoardContext.Notify(
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "ERROR_PASS_NOTMATCH"),
                    MessageTypes.danger);
            }

            var newPass = this.Input.NewPassword.Trim();

            var token = await this.Get<IAspNetUsersHelper>().GeneratePasswordResetTokenAsync(this.EditUser.Item2);

            var result = await this.Get<IAspNetUsersHelper>().ResetPasswordAsync(this.EditUser.Item2, token, newPass);

            if (!result.Succeeded)
            {
                return this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.danger);
            }

            if (!this.Input.EmailNotify)
            {
                return this.PageBoardContext.Notify(
                    this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED"),
                    MessageTypes.success);
            }

            var subject = this.GetTextFormatted(
                "PASSWORDRETRIEVAL_EMAIL_SUBJECT",
                this.Get<BoardSettings>().Name);

            // email a notification...
            var passwordRetrieval = new TemplateEmail("PASSWORDRETRIEVAL_ADMIN")
                                    {
                                        TemplateParams =
                                        {
                                            ["{username}"] = this.EditUser.Item1.DisplayOrUserName(),
                                            ["{password}"] = newPass
                                        }
                                    };

            await passwordRetrieval.SendEmailAsync(
                new MailboxAddress(this.EditUser.Item1.DisplayOrUserName(), this.EditUser.Item1.Email),
                subject);

            return this.PageBoardContext.Notify(
                this.Get<ILocalization>().GetText("ADMIN_EDITUSER", "MSG_PASS_CHANGED_NOTI"),
                MessageTypes.success);
        }
        catch (Exception x)
        {
            return this.PageBoardContext.Notify($"Exception: {x.Message}", MessageTypes.danger);
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
       if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
       {
           return this.Get<ILinkBuilder>().Redirect(
               ForumPages.Admin_EditUser,
               new {
                   u = this.Input.UserId
               });
       }

       this.EditUser = user;

       return this.Page();
    }
}