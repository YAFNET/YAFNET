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

using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using ServiceStack.OrmLite;

using YAF.Core.Identity;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models.Identity;

/// <summary>
/// The OrmLite backed ASP.NET Identity <see cref="UserStore"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserStoreTests
{
    private TestBoardContext board;

    private UserStore store;

    private CancellationToken none;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.store = new UserStore(this.board.Resolve<IServiceLocator>());
        this.none = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        this.store?.Dispose();
        this.board?.Dispose();
    }

    [Test]
    [Description("Create, find (case insensitive), update and delete a user")]
    public async Task Crud_Test()
    {
        var user = NewUser("u1", "Alice", "Alice@Example.com");

        (await this.store.CreateAsync(user, this.none)).Succeeded.Should().BeTrue();

        (await this.store.FindByIdAsync("u1", this.none)).UserName.Should().Be("Alice");
        (await this.store.FindByNameAsync("ALICE", this.none)).Id.Should().Be("u1");
        (await this.store.FindByEmailAsync("alice@example.COM", this.none)).Id.Should().Be("u1");
        this.store.Users.Should().ContainSingle();

        user.PhoneNumber = "123";
        await this.store.UpdateAsync(user, this.none);
        (await this.store.FindByIdAsync("u1", this.none)).PhoneNumber.Should().Be("123");

        await this.store.DeleteAsync(user, this.none);
        (await this.store.FindByIdAsync("u1", this.none)).Should().BeNull();
    }

    [Test]
    [Description("Finders return null for unknown users")]
    public async Task Find_Unknown_Test()
    {
        (await this.store.FindByIdAsync("nobody", this.none)).Should().BeNull();
        (await this.store.FindByNameAsync("nobody", this.none)).Should().BeNull();
        (await this.store.FindByEmailAsync("nobody@example.com", this.none)).Should().BeNull();
        (await this.store.FindAsync(new UserLoginInfo("Google", "x", "Google"))).Should().BeNull();
    }

    [Test]
    [Description("A cancelled token stops the store before it touches the database")]
    public async Task Cancelled_Test()
    {
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();

        await this.store.Invoking(s => s.CreateAsync(NewUser("u1", "Alice", "a@example.com"), cancelled.Token))
            .Should().ThrowAsync<OperationCanceledException>();

        this.store.Users.Should().BeEmpty();
    }

    [Test]
    [Description("Roles: add (case insensitive), list, check and remove; unknown roles are ignored")]
    public async Task Roles_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");
        this.InsertRole("r1", "Administrators");
        this.InsertRole("r2", "Registered");

        await this.store.AddToRoleAsync(user, "administrators", this.none);
        await this.store.AddToRoleAsync(user, "Registered", this.none);
        await this.store.AddToRoleAsync(user, "Does Not Exist", this.none);

        (await this.store.GetRolesAsync(user, this.none)).Should().BeEquivalentTo(["Administrators", "Registered"]);
        (await this.store.IsInRoleAsync(user, "ADMINISTRATORS", this.none)).Should().BeTrue();
        (await this.store.IsInRoleAsync(user, "Does Not Exist", this.none)).Should().BeFalse();

        await this.store.RemoveFromRoleAsync(user, "Administrators", this.none);
        await this.store.RemoveFromRoleAsync(user, "Does Not Exist", this.none);

        (await this.store.GetRolesAsync(user, this.none)).Should().Equal("Registered");
        (await this.store.IsInRoleAsync(user, "Administrators", this.none)).Should().BeFalse();
    }

    [Test]
    [Description("External logins: add, find the user by login, list and remove")]
    public async Task Logins_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");
        var google = new UserLoginInfo("Google", "g-123", "Google");

        await this.store.AddLoginAsync(user, google);

        (await this.store.FindAsync(google)).Id.Should().Be("u1");
        (await this.store.GetLoginsAsync(user)).Select(l => (l.LoginProvider, l.ProviderKey))
            .Should().Equal(("Google", "g-123"));

        await this.store.RemoveLoginAsync(user, google);

        (await this.store.GetLoginsAsync(user)).Should().BeEmpty();
        (await this.store.FindAsync(google)).Should().BeNull();
    }

    [Test]
    [Description("Claims keep their type and can be removed again")]
    public async Task Claims_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");
        var claim = new Claim("forum:role", "moderator");

        await this.store.AddClaimAsync(user, claim);

        var stored = (await this.store.GetClaimsAsync(user)).Should().ContainSingle().Subject;
        stored.Type.Should().Be("forum:role");
        stored.Value.Should().Be("moderator");

        await this.store.RemoveClaimAsync(user, claim);

        (await this.store.GetClaimsAsync(user)).Should().BeEmpty();
    }

    [Test]
    [Description("Password, e-mail, phone and two factor setters are persisted")]
    public async Task Setters_Persist_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");

        (await this.store.HasPasswordAsync(user, this.none)).Should().BeFalse();

        await this.store.SetPasswordHashAsync(user, "hash", this.none);
        await this.store.SetEmailAsync(user, "new@example.com", this.none);
        await this.store.SetEmailConfirmedAsync(user, true, this.none);
        await this.store.SetPhoneNumberAsync(user, "555", this.none);
        await this.store.SetPhoneNumberConfirmedAsync(user, true, this.none);
        await this.store.SetTwoFactorEnabledAsync(user, true, this.none);
        await this.store.SetSecurityStampAsync(user, "stamp");

        var stored = await this.store.FindByIdAsync("u1", this.none);

        (await this.store.GetPasswordHashAsync(stored, this.none)).Should().Be("hash");
        (await this.store.HasPasswordAsync(stored, this.none)).Should().BeTrue();
        (await this.store.GetEmailAsync(stored, this.none)).Should().Be("new@example.com");
        (await this.store.GetEmailConfirmedAsync(stored, this.none)).Should().BeTrue();
        (await this.store.GetPhoneNumberAsync(stored, this.none)).Should().Be("555");
        (await this.store.GetPhoneNumberConfirmedAsync(stored, this.none)).Should().BeTrue();
        (await this.store.GetTwoFactorEnabledAsync(stored, this.none)).Should().BeTrue();
        (await this.store.GetSecurityStampAsync(stored)).Should().Be("stamp");
    }

    [Test]
    [Description("The store supports security stamps, users without a stamp get one that is saved")]
    public async Task Security_Stamp_Test()
    {
        this.store.Should().BeAssignableTo<IUserSecurityStampStore<AspNetUsers>>(
            "the stamp ends the other logins after a password, e-mail or 2FA change");

        var user = await this.CreateUserAsync("u1", "Alice");
        user.SecurityStamp.Should().BeNull();

        var stamp = await this.store.GetSecurityStampAsync(user, this.none);

        stamp.Should().NotBeNullOrEmpty();
        (await this.store.GetSecurityStampAsync(user, this.none)).Should().Be(stamp, "the stamp is created only once");
        (await this.store.FindByIdAsync("u1", this.none)).SecurityStamp.Should().Be(stamp, "the new stamp is saved");

        // the user manager saves the stamp that is set
        await this.store.SetSecurityStampAsync(user, "changed", this.none);

        user.SecurityStamp.Should().Be("changed");
        (await this.store.FindByIdAsync("u1", this.none)).SecurityStamp.Should().Be(stamp);

        UserStore.NewSecurityStamp().Should().NotBe(UserStore.NewSecurityStamp());
    }

    [Test]
    [Description("Failed access counter and lockout values are persisted, the lockout end is UTC")]
    public async Task Lockout_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");

        (await this.store.GetLockoutEndDateAsync(user)).Should().Be(default(DateTimeOffset), "no lockout yet");

        (await this.store.IncrementAccessFailedCountAsync(user)).Should().Be(1);
        (await this.store.IncrementAccessFailedCountAsync(user)).Should().Be(2);

        var end = new DateTimeOffset(2030, 1, 2, 3, 4, 5, TimeSpan.FromHours(2));
        await this.store.SetLockoutEndDateAsync(user, end);
        await this.store.SetLockoutEnabledAsync(user, true);

        var stored = await this.store.FindByIdAsync("u1", this.none);
        (await this.store.GetAccessFailedCountAsync(stored)).Should().Be(2);
        (await this.store.GetLockoutEnabledAsync(stored)).Should().BeTrue();
        (await this.store.GetLockoutEndDateAsync(stored)).Should().Be(end);

        await this.store.ResetAccessFailedCountAsync(stored);
        (await this.store.FindByIdAsync("u1", this.none)).AccessFailedCount.Should().Be(0);
    }

    [Test]
    [Description("Name and e-mail getters return the stored values")]
    public async Task Names_Test()
    {
        var user = await this.CreateUserAsync("u1", "Alice");

        (await this.store.GetUserIdAsync(user, this.none)).Should().Be("u1");
        (await this.store.GetUserNameAsync(user, this.none)).Should().Be("Alice");
        (await this.store.GetNormalizedUserNameAsync(user, this.none)).Should().Be("alice");
        (await this.store.GetNormalizedEmailAsync(user, this.none)).Should().Be("alice@example.com");

        await this.store.SetUserNameAsync(user, "Alicia", this.none);
        await this.store.SetNormalizedUserNameAsync(user, "ALICIA", this.none);
        await this.store.SetNormalizedEmailAsync(user, "ALICE@EXAMPLE.COM", this.none);

        user.UserName.Should().Be("Alicia");
        user.NormalizedUserName.Should().Be("ALICIA");
        user.NormalizedEmail.Should().Be("ALICE@EXAMPLE.COM");
    }

    private static AspNetUsers NewUser(string id, string name, string email)
    {
        return new AspNetUsers
        {
            Id = id,
            UserName = name,
            LoweredUserName = name.ToLowerInvariant(),
            Email = email,
            LoweredEmail = email.ToLowerInvariant(),
            ApplicationId = Guid.NewGuid(),
            IsApproved = true
        };
    }

    private async Task<AspNetUsers> CreateUserAsync(string id, string name)
    {
        var user = NewUser(id, name, $"{name.ToLowerInvariant()}@example.com");
        await this.store.CreateAsync(user, this.none);
        return user;
    }

    private void InsertRole(string id, string name)
    {
        using var connection = this.board.Database.Open();
        connection.Insert(new AspNetRoles { Id = id, Name = name });
    }
}
