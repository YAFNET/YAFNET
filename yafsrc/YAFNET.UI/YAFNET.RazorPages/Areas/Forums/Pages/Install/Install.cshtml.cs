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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Install;

using Microsoft.AspNetCore.Mvc;

using YAF.Types.Interfaces.Data;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Configuration;
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Core.Context;

using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;

using Microsoft.Extensions.Configuration;

/// <summary>
/// Class InstallModel.
/// Implements the <see cref="YAF.Core.BasePages.InstallPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.InstallPage" />
public class InstallModel : InstallPage
{
    /// <summary>
    /// Gets the database access.
    /// </summary>
    /// <value>
    /// The database access.
    /// </value>
    public IDbAccess DbAccess => this.Get<IDbAccess>();

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet()
    {
        // set the connection string provider...
        var previousProvider = this.Get<IDbAccess>().Information.ConnectionString;

        this.DbAccess.Information.ConnectionString = DynamicConnectionString;

        var cachePage = this.Get<IDataCache>().Get("Install");

        if (this.IsForumInstalled && cachePage is null)
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
        }

        this.Get<ILocalization>().TransPage = "INSTALL";

        // fake the board settings
        BoardContext.Current.BoardSettings = new BoardSettings();

        this.Get<IDataCache>().Set("Install", "InstallWelcome");

        return new PartialViewResult
               {
                   ViewName = "Install/InstallWelcome",
                   ViewData = new ViewDataDictionary<InstallModal>(
                       this.ViewData,
                       new InstallModal())
               };

        string DynamicConnectionString() => this.Get<IConfiguration>().GetConnectionString("DefaultConnection").IsSet()
            ? this.Get<IConfiguration>().GetConnectionString("DefaultConnection")
            : previousProvider();
    }

    /// <summary>
    /// Load the test settings page.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostInstallTestSettings()
    {
        var connectionSuccess = this.InstallService.TestDatabaseConnection(out var message);

        this.Get<IDataCache>().Set("Install", "InstallTestSettings");

        return new PartialViewResult {
                                         ViewName = "Install/InstallTestSettings",
                                         ViewData = new ViewDataDictionary<InstallModal>(
                                             this.ViewData,
                                             new InstallModal {
                                                                  ConnectionInfo = message,
                                                                  ConnectionSuccess = connectionSuccess
                                                              })
                                     };
    }

    /// <summary>
    /// Send a test email
    /// </summary>
    /// <param name="fromEmail"></param>
    /// <param name="toEmail"></param>
    public async Task<IActionResult> OnPostTestSmtpAsync(string fromEmail, string toEmail)
    {
        bool testEmailSuccess;
        string testEmailInfo;

        try
        {
            await this.Get<IMailService>().SendAsync(
                fromEmail.Trim(),
                toEmail.Trim(),
                fromEmail.Trim(),
                "Test Email From Yet Another Forum.NET",
                "The email sending appears to be working from your YAF installation.");

            testEmailSuccess = true;
            testEmailInfo = "success";
        }
        catch (Exception x)
        {
            testEmailSuccess = false;
            testEmailInfo = x.Message;
        }

        var connectionSuccess = this.InstallService.TestDatabaseConnection(out var message);

        return new PartialViewResult
               {
                   ViewName = "Install/InstallTestSettings",
                   ViewData = new ViewDataDictionary<InstallModal>(
                       this.ViewData,
                       new InstallModal
                       {
                           ConnectionInfo = message,
                           ConnectionSuccess = connectionSuccess,
                           TestEmailSuccess = testEmailSuccess,
                           TestEmailInfo = testEmailInfo
                       })
               };
    }

    /// <summary>
    /// Loads the initial db page.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostInstallInitDatabase()
    {
        this.Get<IDataCache>().Set("Install", "InstallInitDatabase");

        return new PartialViewResult {
                                         ViewName = "Install/InstallInitDatabase",
                                         ViewData = new ViewDataDictionary<InstallModal>(
                                             this.ViewData,
                                             new InstallModal())
                                     };
    }

    /// <summary>
    /// Loads the create forum page.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostInstallCreateForum()
    {
        this.InstallService.InitializeDatabase();

        this.Get<IDataCache>().Set("Install", "InstallCreateForum");

        return new PartialViewResult {
                                         ViewName = "Install/InstallCreateForum",
                                         ViewData = new ViewDataDictionary<InstallModal>(
                                             this.ViewData,
                                             new InstallModal())
                                     };
    }

    /// <summary>
    /// Loads the installation finish page.
    /// </summary>
    /// <param name="forumName">Name of the forum.</param>
    /// <param name="cultures">The cultures.</param>
    /// <param name="forumEmailAddress">The forum email address.</param>
    /// <param name="forumBaseUrlMask">The forum base URL mask.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="adminEmail">The admin email.</param>
    /// <param name="password1">The password1.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostInstallFinishedAsync(
        string forumName,
        string cultures,
        string forumEmailAddress,
        string forumBaseUrlMask,
        string userName,
        string adminEmail,
        string password1)
    {
        var result =
            await
                this.CreateForumAsync(forumName, cultures, forumEmailAddress, forumBaseUrlMask, userName, adminEmail, password1);

        if (!result.Result)
        {
            return new PartialViewResult {
                                             ViewName = "Install/InstallCreateForum",
                                             ViewData = new ViewDataDictionary<InstallModal>(
                                                 this.ViewData,
                                                 new InstallModal {
                                                                      Message = result.Message
                                                 })
                                         };
        }

        this.Get<IDataCache>().Set("Install", "InstallFinished");

        return new PartialViewResult {
                                         ViewName = "Install/InstallFinished",
                                         ViewData = new ViewDataDictionary<InstallModal>(
                                             this.ViewData,
                                             new InstallModal())
                                     };
    }

    /// <summary>
    /// Go to new Forum.
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostComplete()
    {
        this.Get<IDataCache>().Remove("Install");

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }

    /// <summary>
    /// Create forum as an asynchronous operation.
    /// </summary>
    /// <param name="forumName">Name of the forum.</param>
    /// <param name="cultures">The cultures.</param>
    /// <param name="forumEmailAddress">The forum email address.</param>
    /// <param name="forumBaseUrlMask">The forum base URL mask.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="adminEmail">The admin email.</param>
    /// <param name="password">The password.</param>
    /// <returns>A Task&lt;Tuple`2&gt; representing the asynchronous operation.</returns>
    private async Task<(bool Result, string Message)> CreateForumAsync(
        string forumName,
        string cultures,
        string forumEmailAddress,
        string forumBaseUrlMask,
        string userName,
        string adminEmail,
        string password)
    {
        var applicationId = Guid.NewGuid();

        // fake the board settings
        BoardContext.Current.BoardSettings ??= new BoardSettings();

        // create the admin user...
        AspNetUsers user = new() {
            Id = Guid.NewGuid().ToString(),
            ApplicationId = applicationId,
            UserName = userName,
            LoweredUserName = userName.ToLower(),
            Email = adminEmail,
            LoweredEmail = adminEmail.ToLower(),
            IsApproved = true
        };

        var result = await this.Get<IAspNetUsersHelper>().CreateUserAsync(user, password);

        if (!result.Succeeded)
        {
            return (false, result.Errors.FirstOrDefault()?.Description);
        }

        try
        {
            var prefix = string.Empty;

            // add administrators and registered if they don't already exist...
            if (!await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync($"{prefix}Administrators"))
            {
                await this.Get<IAspNetRolesHelper>().CreateRoleAsync($"{prefix}Administrators");
            }

            if (!await this.Get<IAspNetRolesHelper>().RoleNameExistsAsync($"{prefix}Registered Users"))
            {
                await this.Get<IAspNetRolesHelper>().CreateRoleAsync($"{prefix}Registered Users");
            }

            if (!await this.Get<IAspNetUsersHelper>().IsUserInRoleAsync(user, $"{prefix}Administrators"))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(user, $"{prefix}Administrators");
            }

            // logout administrator...
            await this.Get<IAspNetUsersHelper>().SignOutAsync();

            // init forum...
            this.InstallService.InitializeForum(
                applicationId,
                forumName,
                cultures,
                forumEmailAddress,
                "YAFLogo.svg",
                forumBaseUrlMask,
                user.UserName,
                user.Email,
                user.Id);
        }
        catch (Exception x)
        {
            return (false, x.Message);
        }

        return (true, string.Empty);
    }
}