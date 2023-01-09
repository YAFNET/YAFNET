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
using System.Net;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using YAF.Types.Modals;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using YAF.Core.BasePages;

/// <summary>
/// The Albums controller.
/// </summary>
[Route("api/[controller]")]
public class LoginBox : ForumBaseController
{
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

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartialViewResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("SignIn")]
    public async Task<IActionResult> SignInAsync(LoginModal model)
    {
        var user = this.Get<IAspNetUsersHelper>().ValidateUser(model.UserName);

        if (user == null)
        {
            this.PageBoardContext.SessionNotify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);
            return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
        }

        if (!user.IsApproved)
        {
            var yafUser = this.Get<IAspNetUsersHelper>().GetUserFromProviderUserKey(user.Id);

            // Ignore Deleted User
            if (yafUser.UserFlags.IsDeleted)
            {
                return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
            }

            this.PageBoardContext.SessionNotify(this.GetText("LOGIN", "ACCOUNT_NOT_APPROVED"), MessageTypes.warning);

            return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
        }

        // Valid user, verify password
        var result = this.Get<IAspNetUsersHelper>().IPasswordHasher
            .VerifyHashedPassword(user, user.PasswordHash, model.Password);

        switch (result)
        {
            case PasswordVerificationResult.Success:
                await this.Get<IAspNetUsersHelper>().SignInAsync(user, model.RememberMe);

                return this.Get<LinkBuilder>().Redirect(ForumPages.Index);

            case PasswordVerificationResult.SuccessRehashNeeded:
                user.PasswordHash = this.Get<IAspNetUsersHelper>().IPasswordHasher
                    .HashPassword(user, model.Password);

                await this.Get<IAspNetUsersHelper>().SignInAsync(user, model.RememberMe);

                return this.Get<LinkBuilder>().Redirect(ForumPages.Index);

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

                    this.Get<IAspNetUsersHelper>().Update(user);

                    this.Get<ILogger<LoginBox>>().Log(
                        null,
                        $"Login Failure: {model.UserName}",
                        $"Login Failure for User: {model.UserName} with the IP Address: {this.Request.HttpContext.Connection.LocalIpAddress}",
                        EventLogTypes.LoginFailure);

                    this.PageBoardContext.SessionNotify(this.GetText("PASSWORD_ERROR"), MessageTypes.danger);

                    return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
                }
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PartialViewResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("Auth")]
    public Task<IActionResult> AuthAsync(string auth)
    {
        var redirectUrl = this.Get<LinkBuilder>().GetLink(
            ForumPages.Account_Login,
            new { auth, handler = "Callback" });

        var properties = this.Get<SignInManager<AspNetUsers>>()
            .ConfigureExternalAuthenticationProperties(auth, redirectUrl);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault |
                                               SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        return Task.FromResult<IActionResult>(new ChallengeResult(auth, properties));
    }
}