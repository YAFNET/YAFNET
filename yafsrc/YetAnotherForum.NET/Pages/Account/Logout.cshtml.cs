/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

namespace YAF.Pages.Account;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;

/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class LogoutModel : AccountPage
{
    /// <summary>
    /// The _sign in manager.
    /// </summary>
    private readonly SignInManager<AspNetUsers> signInManager;

    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<LogoutModel> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutModel"/> class.
    /// </summary>
    /// <param name="signInManager">
    /// The sign in manager.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public LogoutModel(SignInManager<AspNetUsers> signInManager, ILogger<LogoutModel> logger)
        : base("LOGOUT", ForumPages.Account_Logout)
    {
        this.signInManager = signInManager;
        this.logger = logger;
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnGetAsync()
    {
        await this.signInManager.SignOutAsync();

        await this.Get<IAspNetUsersHelper>().SignOutAsync();

        this.Get<IRaiseEvent>().Raise(new UserLogoutEvent(this.PageBoardContext.PageUserID));

        this.logger.LogInformation("User logged out.");

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
    }
}