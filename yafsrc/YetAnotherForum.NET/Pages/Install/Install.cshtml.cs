/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using YAF.Core.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;

public class InstallModel : InstallPage
{
    /// <summary>
    /// Gets the database access.
    /// </summary>
    /// <value>
    /// The database access.
    /// </value>
    public IDbAccess DbAccess => this.Get<IDbAccess>();

    public IActionResult OnGet()
    {
        // set the connection string provider...
        var previousProvider = this.Get<IDbAccess>().Information.ConnectionString;

        string DynamicConnectionString() => Config.ConnectionString.IsSet() ? Config.ConnectionString : previousProvider();

        this.DbAccess.Information.ConnectionString = DynamicConnectionString;

        var cachePage = this.Get<IDataCache>().Get("Install");

        if (this.IsForumInstalled && cachePage == null)
        {
            return this.RedirectToPage(ForumPages.Index.GetPageName());
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
    }

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
    public IActionResult OnPostTestSmtp(string fromEmail, string toEmail)
    {
        bool testEmailSuccess;
        string testEmailInfo;

        try
        {
            this.Get<IMailService>().Send(
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

    public IActionResult OnPostInstallFinished(
        string forumName,
        string cultures,
        string forumEmailAddress,
        string forumBaseUrlMask,
        string userName,
        string adminEmail,
        string password1,
        string password2)
    {
        if (!this.CreateForum(
                forumName,
                cultures,
                forumEmailAddress,
                forumBaseUrlMask,
                userName,
                adminEmail,
                password1,
                out var message))
        {
            return new PartialViewResult {
                                             ViewName = "Install/InstallCreateForum",
                                             ViewData = new ViewDataDictionary<InstallModal>(
                                                 this.ViewData,
                                                 new InstallModal {
                                                                      Message = message
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

    public IActionResult OnPostComplete()
    {
        this.Get<IDataCache>().Remove("Install");

        return this.RedirectToPage(ForumPages.Index.GetPageName());
    }

    /// <summary>
    ///     Creates the forum.
    /// </summary>
    /// <returns>
    ///     The create forum.
    /// </returns>
    private bool CreateForum(
        string forumName,
        string cultures,
        string forumEmailAddress,
        string forumBaseUrlMask,
        string userName,
        string adminEmail,
        string password1,
        out string message)
    {
        var applicationId = Guid.NewGuid();

        // fake the board settings
        BoardContext.Current.BoardSettings ??= new BoardSettings();

        // create the admin user...
        AspNetUsers user = new() {
                                     Id = Guid.NewGuid().ToString(),
                                     ApplicationId = applicationId,
                                     UserName = userName,
                                     LoweredUserName = userName,
                                     Email = adminEmail,
                                     IsApproved = true
                                 };

        var result = this.Get<IAspNetUsersHelper>().Create(user, password1);

        if (!result.Succeeded)
        {
            message = result.Errors.FirstOrDefault()?.Description;

            return false;
        }

        try
        {
            var prefix = string.Empty;

            // add administrators and registered if they don't already exist...
            if (!this.Get<IAspNetRolesHelper>().RoleExists($"{prefix}Administrators"))
            {
                this.Get<IAspNetRolesHelper>().CreateRole($"{prefix}Administrators");
            }

            if (!this.Get<IAspNetRolesHelper>().RoleExists($"{prefix}Registered Users"))
            {
                this.Get<IAspNetRolesHelper>().CreateRole($"{prefix}Registered Users");
            }

            if (!this.Get<IAspNetRolesHelper>().IsUserInRole(user, $"{prefix}Administrators"))
            {
                this.Get<IAspNetRolesHelper>().AddUserToRole(user, $"{prefix}Administrators");
            }

            // logout administrator...
            this.Get<IAspNetUsersHelper>().SignOut();

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
            message = x.Message;

            return false;
        }

        message = string.Empty;

        return true;
    }
}