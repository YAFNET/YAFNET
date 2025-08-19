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

namespace YAF.Core.Controllers;

using System;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using YAF.Types.Modals;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using YAF.Core.BasePages;

/// <summary>
/// The Albums controller.
/// </summary>
[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
public class LoginBox : ForumBaseController
{
    /// <summary>
    /// Shows the modal.
    /// </summary>
    /// <returns>IActionResult.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartialViewResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("ShowModal")]
    public IActionResult ShowModal()
    {
        return new PartialViewResult
                   {
                       ViewName = "_LoginBox",
                       ViewData = new ViewDataDictionary<LoginModal>(this.ViewData, new LoginModal())
                   };
    }

    /// <summary>
    /// Sign in as an asynchronous operation.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartialViewResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignInAsync(LoginModal model)
    {
        var user = await this.Get<IAspNetUsersHelper>().ValidateUserAsync(model.UserName);

        if (user == null)
        {
            this.PageBoardContext.SessionNotify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
        }

        if (!user.IsApproved)
        {
            var yafUser = await this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKeyAsync(user.Id);

            // Ignore Deleted User
            if (yafUser.UserFlags.IsDeleted)
            {
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
            }

            this.PageBoardContext.SessionNotify(this.GetText("LOGIN", "ACCOUNT_NOT_APPROVED"), MessageTypes.warning);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
        }

        // Valid user, verify password
        var result = this.Get<IAspNetUsersHelper>().PasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash, model.Password);

        switch (result)
        {
            case PasswordVerificationResult.Success:
                return await this.SignInAsync(user, model);

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = this.Get<IAspNetUsersHelper>().PasswordHasher
                    .HashPassword(user, model.Password);

                return await this.SignInAsync(user, model);

            default:
                {
                    // Lockout for 15 minutes if more than 10 failed attempts
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount >= 10)
                    {
                        user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(15);

                        this.Get<ILogger<LoginBox>>().Info(
                            $"User: {user.UserName} has reached the Limit of 10 failed login attempts and is locked out until {user.LockoutEndDateUtc}");

                        this.PageBoardContext.SessionNotify(this.GetText("LOGIN", "ERROR_LOCKEDOUT"), MessageTypes.danger);
                    }

                    await this.Get<IAspNetUsersHelper>().UpdateUserAsync(user);

                    this.Get<ILogger<LoginBox>>().Log(
                        null,
                        $"Login Failure: {model.UserName}",
                        $"Login Failure for User: {model.UserName} with the IP Address: {this.Request.GetUserRealIPAddress()}",
                        EventLogTypes.LoginFailure);

                    this.PageBoardContext.SessionNotify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);

                    return this.Get<ILinkBuilder>().Redirect(ForumPages.Index);
                }
        }
    }

    /// <summary>
    /// Sign in user
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="modal">The modal.</param>
    /// <returns>A Task&lt;IActionResult&gt; representing the asynchronous operation.</returns>
    private async Task<IActionResult> SignInAsync(AspNetUsers user, LoginModal modal)
    {
        if (!user.TwoFactorEnabled)
        {
            return await this.Get<IAspNetUsersHelper>().SignInAsync(user, modal.RememberMe);
        }

        this.Get<ISessionService>().SetPageData(user);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Authorize);
    }
}