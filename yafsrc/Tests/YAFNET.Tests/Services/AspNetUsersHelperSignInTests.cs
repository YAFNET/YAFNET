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

namespace YAF.Tests.Services;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NSubstitute;

using YAF.Core.Helpers;
using YAF.Core.Identity;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models.Identity;

/// <summary>
/// <see cref="AspNetUsersHelper"/>: signing in (also with an external login) and the tokens for the password
/// reset and the e-mail confirmation.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AspNetUsersHelperSignInTests
{
    private const string Password = "S3cret!pass";

    private TestBoardContext board;

    private IAspNetUsersHelper helper;

    private SignInManager<AspNetUsers> signInManager;

    [SetUp]
    public void SetUp()
    {
        // NUnit keeps the fixture instance between the tests
        this.signInManager = null;

        this.board = new TestBoardContext(
            b =>
            {
                b.Register(
                    c =>
                    {
                        var manager = new AspNetUsersManager(
                            new UserStore(c.Resolve<IServiceLocator>()),
                            Options.Create(new IdentityOptions()),
                            null,
                            [],
                            [],
                            new UpperInvariantLookupNormalizer(),
                            new IdentityErrorDescriber(),
                            null,
                            NullLogger<AspNetUsersManager>.Instance);

                        // what AddDefaultTokenProviders registers
                        manager.RegisterTokenProvider(
                            TokenOptions.DefaultProvider,
                            new DataProtectorTokenProvider<AspNetUsers>(
                                new EphemeralDataProtectionProvider(),
                                Options.Create(new DataProtectionTokenProviderOptions()),
                                NullLogger<DataProtectorTokenProvider<AspNetUsers>>.Instance));

                        return manager;
                    }).AsSelf().SingleInstance();
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.Register(
                    c => this.signInManager ??= Substitute.For<SignInManager<AspNetUsers>>(
                             c.Resolve<AspNetUsersManager>(),
                             Substitute.For<IHttpContextAccessor>(),
                             Substitute.For<IUserClaimsPrincipalFactory<AspNetUsers>>(),
                             Options.Create(new IdentityOptions()),
                             NullLogger<SignInManager<AspNetUsers>>.Instance,
                             Substitute.For<IAuthenticationSchemeProvider>(),
                             Substitute.For<IUserConfirmation<AspNetUsers>>())).AsSelf();
            },
            useDatabase: true);

        this.helper = this.board.Resolve<IAspNetUsersHelper>();
        this.signInManager = this.board.Resolve<SignInManager<AspNetUsers>>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Signing in replaces the old login, resets the failed logins and the lockout and goes to the index")]
    public async Task SignIn_Test()
    {
        var user = await this.AddUserAsync("alice");
        await this.LockOutAsync(user);

        var redirect = Substitute.For<IActionResult>();
        this.board.LinkBuilder.Redirect(ForumPages.Index).Returns(redirect);

        (await this.helper.SignInAsync(user)).Should().BeSameAs(redirect);

        Received.InOrder(
            () =>
            {
                this.signInManager.SignOutAsync();
                this.signInManager.SignInAsync(
                    user,
                    Arg.Is<AuthenticationProperties>(
                        p => p.IsPersistent && p.ExpiresUtc > DateTimeOffset.UtcNow.AddDays(27)),
                    null);
            });

        var stored = await this.helper.GetUserAsync(user.Id);
        stored.AccessFailedCount.Should().Be(0);
        stored.LockoutEndDateUtc.Should().BeNull();

        this.board.Events.OfType<SuccessfulUserLoginEvent>().Should().ContainSingle();
    }

    [Test]
    [Description("A login without 'remember me' ends with the browser session")]
    public async Task SignIn_Not_Persistent_Test()
    {
        var user = await this.AddUserAsync("alice");

        await this.helper.SignInAsync(user, isPersistent: false);

        await this.signInManager.Received(1).SignInAsync(
            user,
            Arg.Is<AuthenticationProperties>(p => !p.IsPersistent && p.ExpiresUtc == null),
            null);
    }

    [Test]
    [Description("The external login signs in when the provider sent the login info, else it goes back to the login page")]
    public async Task SignIn_External_Test()
    {
        var user = await this.AddUserAsync("alice");
        await this.LockOutAsync(user);

        var login = Substitute.For<IActionResult>();
        var index = Substitute.For<IActionResult>();
        this.board.LinkBuilder.Redirect(ForumPages.Account_Login).Returns(login);
        this.board.LinkBuilder.Redirect(ForumPages.Index).Returns(index);

        this.signInManager.GetExternalLoginInfoAsync().Returns((ExternalLoginInfo)null);

        (await this.helper.SignInExternalAsync(user)).Should().BeSameAs(login);
        await this.signInManager.DidNotReceive().SignInAsync(Arg.Any<AspNetUsers>(), Arg.Any<bool>(), Arg.Any<string>());
        this.board.Events.OfType<SuccessfulUserLoginEvent>().Should().BeEmpty();

        this.signInManager.GetExternalLoginInfoAsync().Returns(
            new ExternalLoginInfo(new System.Security.Claims.ClaimsPrincipal(), "Google", "google-key", "Google"));

        (await this.helper.SignInExternalAsync(user)).Should().BeSameAs(index);
        await this.signInManager.Received(1).SignInAsync(user, true, null);
        (await this.helper.GetUserAsync(user.Id)).AccessFailedCount.Should().Be(0);
        this.board.Events.OfType<SuccessfulUserLoginEvent>().Should().ContainSingle();
    }

    [Test]
    [Description("A password reset token resets the password once, for its user only")]
    public async Task Reset_Password_Test()
    {
        var alice = await this.AddUserAsync("alice");
        var bob = await this.AddUserAsync("bob");

        var token = await this.helper.GeneratePasswordResetTokenAsync(alice);

        (await this.helper.ResetPasswordAsync(bob, token, "N3w!password")).Succeeded.Should().BeFalse("the token of alice");
        (await this.helper.ResetPasswordAsync(alice, "invalid", "N3w!password")).Succeeded.Should().BeFalse();

        alice = await this.helper.GetUserAsync(alice.Id);
        (await this.helper.ResetPasswordAsync(alice, token, "N3w!password")).Succeeded.Should().BeTrue();
        this.Verify(await this.helper.GetUserAsync(alice.Id), "N3w!password").Should().BeTrue();

        // the new security stamp ends the token
        alice = await this.helper.GetUserAsync(alice.Id);
        (await this.helper.ResetPasswordAsync(alice, token, "Other!password1")).Succeeded.Should().BeFalse();
    }

    [Test]
    [Description("Changing the password needs the current password")]
    public async Task Change_Password_Test()
    {
        var alice = await this.AddUserAsync("alice");

        (await this.helper.ChangePasswordAsync(alice, "wrong", "N3w!password")).Succeeded.Should().BeFalse();
        this.Verify(await this.helper.GetUserAsync(alice.Id), Password).Should().BeTrue();

        alice = await this.helper.GetUserAsync(alice.Id);
        (await this.helper.ChangePasswordAsync(alice, Password, "N3w!password")).Succeeded.Should().BeTrue();
        this.Verify(await this.helper.GetUserAsync(alice.Id), "N3w!password").Should().BeTrue();
    }

    [Test]
    [Description("The e-mail confirmation token confirms the e-mail of its user")]
    public async Task Confirm_Email_Test()
    {
        var alice = await this.AddUserAsync("alice");
        var bob = await this.AddUserAsync("bob");

        var token = await this.helper.GenerateEmailConfirmationResetTokenAsync(alice);

        (await this.helper.ConfirmUserEmailAsync(bob, token)).Succeeded.Should().BeFalse("the token of alice");
        (await this.helper.GetUserAsync(bob.Id)).EmailConfirmed.Should().BeFalse();

        (await this.helper.ConfirmUserEmailAsync(alice, token)).Succeeded.Should().BeTrue();
        (await this.helper.GetUserAsync(alice.Id)).EmailConfirmed.Should().BeTrue();
    }

    private bool Verify(AspNetUsers user, string password)
    {
        return this.helper.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password)
               != PasswordVerificationResult.Failed;
    }

    private async Task LockOutAsync(AspNetUsers user)
    {
        user.AccessFailedCount = 3;
        user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(10);

        (await this.helper.UpdateUserAsync(user)).Succeeded.Should().BeTrue();

        (await this.helper.GetUserAsync(user.Id)).AccessFailedCount.Should().Be(3);
    }

    private async Task<AspNetUsers> AddUserAsync(string name)
    {
        var user = new AspNetUsers
        {
            Id = $"{name}-key",
            UserName = name,
            LoweredUserName = name,
            Email = $"{name}@example.com",
            LoweredEmail = $"{name}@example.com",
            ApplicationId = Guid.NewGuid(),
            IsApproved = true,
            CreateDate = DateTime.UtcNow
        };

        (await this.helper.CreateUserAsync(user, Password)).Succeeded.Should().BeTrue();

        return await this.helper.GetUserAsync(user.Id);
    }
}
