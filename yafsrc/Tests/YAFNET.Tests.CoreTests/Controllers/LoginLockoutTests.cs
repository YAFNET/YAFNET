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

namespace YAF.Tests.CoreTests.Controllers;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using YAF.Core.Controllers;
using YAF.Core.Membership;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Flags;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Brute force protection of the login box: failed attempts lock the account, and a locked account
/// cannot sign in, not even with the right password.
/// </summary>
[TestFixture]
[NonParallelizable]
public class LoginLockoutTests
{
    private const string Password = "S3cret!pass";

    private TestBoardContext board;

    private IAspNetUsersHelper users;

    private AspNetUsers alice;

    private IActionResult signedIn;

    [SetUp]
    public void SetUp()
    {
        var hasher = new SQLPasswordHasher();

        this.alice = new AspNetUsers { Id = "alice-key", UserName = "alice", Email = "alice@example.com", IsApproved = true };
        this.alice.PasswordHash = hasher.HashPassword(this.alice, Password);

        this.signedIn = new OkResult();

        this.users = Substitute.For<IAspNetUsersHelper>();
        this.users.PasswordHasher.Returns(hasher);
        this.users.ValidateUserAsync("alice").Returns(this.alice);
        this.users.SignInAsync(this.alice, Arg.Any<bool>()).Returns(this.signedIn);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.users).As<IAspNetUsersHelper>();
                b.RegisterInstance(Substitute.For<ISessionService>()).As<ISessionService>();
            });
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The right password signs in and a wrong one counts as a failed attempt")]
    public async Task SignIn_Test()
    {
        (await this.SignInAsync("wrong")).Should().NotBeSameAs(this.signedIn);
        this.alice.AccessFailedCount.Should().Be(1);
        await this.users.Received(1).UpdateUserAsync(this.alice);

        (await this.SignInAsync(Password)).Should().BeSameAs(this.signedIn);
    }

    [Test]
    [Description("The 10th failed attempt locks the account for 15 minutes")]
    public async Task Tenth_Failure_Locks_Test()
    {
        this.alice.AccessFailedCount = 9;

        await this.SignInAsync("wrong");

        this.alice.LockoutEndDateUtc.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromMinutes(1));
        await this.users.Received().UpdateUserAsync(Arg.Is<AspNetUsers>(u => u.LockoutEndDateUtc != null));
    }

    [Test]
    [Description("A locked account cannot sign in, even with the right password")]
    public async Task Locked_Account_Cannot_SignIn_Test()
    {
        this.alice.AccessFailedCount = 10;
        this.alice.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(10);

        (await this.SignInAsync(Password)).Should().NotBeSameAs(this.signedIn);

        await this.users.DidNotReceive().SignInAsync(Arg.Any<AspNetUsers>(), Arg.Any<bool>());
    }

    [Test]
    [Description("Once the lockout has expired the right password signs in again")]
    public async Task Expired_Lockout_Test()
    {
        this.alice.AccessFailedCount = 10;
        this.alice.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(-1);

        (await this.SignInAsync(Password)).Should().BeSameAs(this.signedIn);
    }

    [Test]
    [Description("Unknown users and not approved accounts are not signed in, deleted accounts get no notice")]
    public async Task Unknown_And_Not_Approved_Test()
    {
        var controller = this.board.CreateController<LoginBox>();

        (await controller.SignInAsync(new LoginModal { UserName = "nobody", Password = Password })).Should()
            .NotBeSameAs(this.signedIn);

        var pending = new AspNetUsers { Id = "pending-key", UserName = "pending", IsApproved = false };
        pending.PasswordHash = new SQLPasswordHasher().HashPassword(pending, Password);
        this.users.ValidateUserAsync("pending").Returns(pending);
        this.users.GetUserFromProviderUserKeyAsync("pending-key").Returns(new User { Name = "pending" });

        (await controller.SignInAsync(new LoginModal { UserName = "pending", Password = Password })).Should()
            .NotBeSameAs(this.signedIn);

        this.users.GetUserFromProviderUserKeyAsync("pending-key")
            .Returns(new User { Name = "pending", Flags = (int)UserFlags.Flags.IsDeleted });

        (await controller.SignInAsync(new LoginModal { UserName = "pending", Password = Password })).Should()
            .NotBeSameAs(this.signedIn);

        await this.users.DidNotReceive().SignInAsync(Arg.Any<AspNetUsers>(), Arg.Any<bool>());
    }

    [Test]
    [Description("The state of a not approved account is only checked (and shown) with the right password")]
    public async Task Not_Approved_Wrong_Password_Test()
    {
        var pending = new AspNetUsers { Id = "pending-key", UserName = "pending", IsApproved = false };
        pending.PasswordHash = new SQLPasswordHasher().HashPassword(pending, Password);
        this.users.ValidateUserAsync("pending").Returns(pending);

        var controller = this.board.CreateController<LoginBox>();

        await controller.SignInAsync(new LoginModal { UserName = "pending", Password = "wrong" });

        await this.users.DidNotReceive().GetUserFromProviderUserKeyAsync("pending-key");

        await controller.SignInAsync(new LoginModal { UserName = "pending", Password = Password });

        await this.users.Received(1).GetUserFromProviderUserKeyAsync("pending-key");
        await this.users.DidNotReceive().SignInAsync(Arg.Any<AspNetUsers>(), Arg.Any<bool>());
    }

    [Test]
    [Description("A not approved identity account without a forum user does not crash the login box")]
    public async Task Not_Approved_Without_Forum_User_Test()
    {
        var pending = new AspNetUsers { Id = "orphan-key", UserName = "orphan", IsApproved = false };
        pending.PasswordHash = new SQLPasswordHasher().HashPassword(pending, Password);
        this.users.ValidateUserAsync("orphan").Returns(pending);
        this.users.GetUserFromProviderUserKeyAsync("orphan-key").Returns((User)null);

        var result = await this.board.CreateController<LoginBox>()
            .SignInAsync(new LoginModal { UserName = "orphan", Password = Password });

        result.Should().NotBeSameAs(this.signedIn);
    }

    [Test]
    [Description("A password that needs a new hash signs in with the rehashed password")]
    public async Task Rehash_Needed_Test()
    {
        var hasher = Substitute.For<IPasswordHasher<AspNetUsers>>();
        hasher.VerifyHashedPassword(this.alice, Arg.Any<string>(), Password).Returns(PasswordVerificationResult.SuccessRehashNeeded);
        hasher.HashPassword(this.alice, Password).Returns("new-hash");
        this.users.PasswordHasher.Returns(hasher);

        (await this.SignInAsync(Password)).Should().BeSameAs(this.signedIn);

        this.alice.PasswordHash.Should().Be("new-hash");
    }

    [Test]
    [Description("With two factor authentication the user is sent to the code page instead of being signed in")]
    public async Task Two_Factor_Test()
    {
        this.alice.TwoFactorEnabled = true;
        var session = this.board.Resolve<ISessionService>();

        var result = await this.SignInAsync(Password);

        result.Should().NotBeSameAs(this.signedIn);
        session.Received(1).SetPageData(this.alice);
        await this.users.DidNotReceive().SignInAsync(Arg.Any<AspNetUsers>(), Arg.Any<bool>());
    }

    [Test]
    [Description("The login box dialog is rendered as a partial view")]
    public void Show_Modal_Test()
    {
        var result = this.board.CreateController<LoginBox>().ShowModal().Should().BeOfType<PartialViewResult>().Subject;

        result.ViewName.Should().Be("_LoginBox");
        result.ViewData.Model.Should().BeOfType<LoginModal>();
    }

    private Task<IActionResult> SignInAsync(string password)
    {
        return this.board.CreateController<LoginBox>()
            .SignInAsync(new LoginModal { UserName = "alice", Password = password });
    }
}
