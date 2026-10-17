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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Services;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;

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

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.store = new UserStore(this.board.Resolve<IServiceLocator>());
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Claims keep their type and can be removed again")]
    public async Task Claims_Test()
    {
        var user = new AspNetUsers
                       {
                           Id = "u1",
                           UserName = "Alice",
                           LoweredUserName = "alice",
                           Email = "alice@example.com",
                           LoweredEmail = "alice@example.com",
                           ApplicationId = Guid.NewGuid(),
                           IsApproved = true
                       };

        await this.store.CreateAsync(user);

        var claim = new Claim("forum:role", "moderator");

        await this.store.AddClaimAsync(user, claim);

        var stored = (await this.store.GetClaimsAsync(user)).Should().ContainSingle().Subject;
        stored.Type.Should().Be("forum:role");
        stored.Value.Should().Be("moderator");

        await this.store.RemoveClaimAsync(user, claim);

        (await this.store.GetClaimsAsync(user)).Should().BeEmpty();
    }

    [Test]
    [Description("An external login finds its user, an unknown login finds nobody (it failed when it was awaited)")]
    public async Task Logins_Test()
    {
        var user = await this.CreateUserAsync("u2", "Bob");
        var login = new UserLoginInfo("Google", "google-key-1");

        (await this.store.FindAsync(login)).Should().BeNull();

        await this.store.AddLoginAsync(user, login);

        (await this.store.FindAsync(login)).Id.Should().Be("u2");
        (await this.store.GetLoginsAsync(user)).Should().ContainSingle(l => l.ProviderKey == "google-key-1");

        await this.store.RemoveLoginAsync(user, login);

        (await this.store.GetLoginsAsync(user)).Should().BeEmpty();
        (await this.store.FindAsync(login)).Should().BeNull();
    }

    [Test]
    [Description("The roles of a user: an unknown role is ignored")]
    public async Task Roles_Test()
    {
        var user = await this.CreateUserAsync("u3", "Carol");

        using (var roles = new RoleStore(this.board.Resolve<IServiceLocator>()))
        {
            await roles.CreateAsync(new AspNetRoles { Id = Guid.NewGuid().ToString(), Name = "Editors" });
        }

        await this.store.AddToRoleAsync(user, "Unknown");
        await this.store.AddToRoleAsync(user, "Editors");

        (await this.store.GetRolesAsync(user)).Should().Equal("Editors");
        (await this.store.IsInRoleAsync(user, "Editors")).Should().BeTrue();
        (await this.store.IsInRoleAsync(user, "Unknown")).Should().BeFalse();

        await this.store.RemoveFromRoleAsync(user, "Unknown");
        await this.store.RemoveFromRoleAsync(user, "Editors");

        (await this.store.GetRolesAsync(user)).Should().BeEmpty();
    }

    [Test]
    [Description("The values of the identity (e-mail, phone, two factor, lockout, password, security stamp) are stored")]
    public async Task Properties_Test()
    {
        var user = await this.CreateUserAsync("u4", "Dave");

        await this.store.SetEmailAsync(user, "dave@example.com");
        await this.store.SetEmailConfirmedAsync(user, true);
        await this.store.SetPhoneNumberAsync(user, "+49 123");
        await this.store.SetPhoneNumberConfirmedAsync(user, true);
        await this.store.SetTwoFactorEnabledAsync(user, true);
        await this.store.SetLockoutEnabledAsync(user, true);
        await this.store.SetLockoutEndDateAsync(user, new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
        await this.store.SetPasswordHashAsync(user, "hash");
        await this.store.SetSecurityStampAsync(user, "stamp");
        await this.store.UpdateAsync(user);

        (await this.store.IncrementAccessFailedCountAsync(user)).Should().Be(1);
        (await this.store.IncrementAccessFailedCountAsync(user)).Should().Be(2);

        var stored = await this.store.FindByEmailAsync("dave@example.com");
        stored.Id.Should().Be("u4");

        (await this.store.GetEmailAsync(stored)).Should().Be("dave@example.com");
        (await this.store.GetEmailConfirmedAsync(stored)).Should().BeTrue();
        (await this.store.GetPhoneNumberAsync(stored)).Should().Be("+49 123");
        (await this.store.GetPhoneNumberConfirmedAsync(stored)).Should().BeTrue();
        (await this.store.GetTwoFactorEnabledAsync(stored)).Should().BeTrue();
        (await this.store.GetLockoutEnabledAsync(stored)).Should().BeTrue();
        (await this.store.GetLockoutEndDateAsync(stored)).Year.Should().Be(2030);
        (await this.store.GetPasswordHashAsync(stored)).Should().Be("hash");
        (await this.store.HasPasswordAsync(stored)).Should().BeTrue();
        (await this.store.GetSecurityStampAsync(stored)).Should().Be("stamp");
        (await this.store.GetAccessFailedCountAsync(user)).Should().Be(2);

        await this.store.ResetAccessFailedCountAsync(user);
        (await this.store.GetAccessFailedCountAsync(user)).Should().Be(0);

        (await this.store.FindByNameAsync("Dave")).Id.Should().Be("u4");

        await this.store.DeleteAsync(user);
        (await this.store.FindByIdAsync("u4")).Should().BeNull();
    }

    private async Task<AspNetUsers> CreateUserAsync(string id, string name)
    {
        var user = new AspNetUsers
                       {
                           Id = id,
                           UserName = name,
                           LoweredUserName = name.ToLowerInvariant(),
                           Email = $"{name.ToLowerInvariant()}@example.com",
                           LoweredEmail = $"{name.ToLowerInvariant()}@example.com",
                           ApplicationId = Guid.NewGuid(),
                           IsApproved = true
                       };

        await this.store.CreateAsync(user);

        return user;
    }
}
