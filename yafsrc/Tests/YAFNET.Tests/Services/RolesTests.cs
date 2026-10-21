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

using System.Threading;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Identity;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// Roles: the OrmLite <see cref="RoleStore"/> and <see cref="AspNetRolesHelper"/>, which keeps
/// identity roles and forum groups in sync.
/// </summary>
[TestFixture]
[NonParallelizable]
public class RolesTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetRolesHelper roles;

    private AspNetUsersManager users;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.Register(
                    c => new AspNetUsersManager(
                        new UserStore(c.Resolve<IServiceLocator>()),
                        Options.Create(new IdentityOptions()),
                        null,
                        [],
                        [],
                        new UpperInvariantLookupNormalizer(),
                        new IdentityErrorDescriber(),
                        null,
                        NullLogger<AspNetUsersManager>.Instance)).AsSelf().SingleInstance();
                b.Register(
                    c => new AspNetRoleManager(
                        new RoleStore(c.Resolve<IServiceLocator>()),
                        [],
                        new UpperInvariantLookupNormalizer(),
                        new IdentityErrorDescriber(),
                        NullLogger<AspNetRoleManager>.Instance)).AsSelf().As<IAspNetRoleManager>().SingleInstance();
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.RegisterType<AspNetRolesHelper>().As<IAspNetRolesHelper>().AsSelf();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.board.PageLoad.GuestUserID = 9999;
        this.roles = this.board.Resolve<IAspNetRolesHelper>();
        this.users = this.board.Resolve<AspNetUsersManager>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("RoleStore: create, find (case insensitive), update and delete")]
    public async Task RoleStore_Crud_Test()
    {
        using var store = new RoleStore(this.board.Resolve<IServiceLocator>());
        var none = CancellationToken.None;
        var role = new AspNetRoles { Name = "Moderators" };

        (await store.CreateAsync(role, none)).Succeeded.Should().BeTrue();

        (await store.FindByIdAsync(role.Id, none)).Name.Should().Be("Moderators");
        (await store.FindByNameAsync("MODERATORS", none)).Id.Should().Be(role.Id);
        store.Roles.Should().ContainSingle();
        (await store.GetRoleIdAsync(role, none)).Should().Be(role.Id);

        await store.SetRoleNameAsync(role, "Mods", none);
        (await store.UpdateAsync(role, none)).Succeeded.Should().BeTrue();
        (await store.GetRoleNameAsync(await store.FindByIdAsync(role.Id, none), none)).Should().Be("Mods");

        (await store.DeleteAsync(role, none)).Succeeded.Should().BeTrue();
        (await store.FindByIdAsync(role.Id, none)).Should().BeNull();
        (await store.DeleteAsync(role, none)).Succeeded.Should().BeFalse("it is already gone");
    }

    [Test]
    [Description("RoleStore rejects null roles and cancelled tokens")]
    public async Task RoleStore_Arguments_Test()
    {
        using var store = new RoleStore(this.board.Resolve<IServiceLocator>());
        using var cancelled = new CancellationTokenSource();
        await cancelled.CancelAsync();

        await store.Invoking(s => s.CreateAsync(null, CancellationToken.None)).Should().ThrowAsync<ArgumentNullException>();
        await store.Invoking(s => s.CreateAsync(new AspNetRoles { Name = "x" }, cancelled.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    [Description("Create, list, check and delete roles")]
    public async Task Create_Delete_Role_Test()
    {
        await this.roles.CreateRoleAsync("Editors");
        await this.roles.CreateRoleAsync("Readers");

        this.roles.GetAllRoles().Should().BeEquivalentTo("Editors", "Readers");
        (await this.roles.RoleNameExistsAsync("Editors")).Should().BeTrue();

        await this.roles.DeleteRoleAsync("Editors");

        (await this.roles.RoleNameExistsAsync("Editors")).Should().BeFalse();
        this.roles.GetAllRoles().Should().Equal("Readers");
    }

    [Test]
    [Description("Deleting a role that does not exist does nothing")]
    public async Task Delete_Unknown_Role_Test()
    {
        await this.roles.Invoking(r => r.DeleteRoleAsync("Nope")).Should().NotThrowAsync();
    }

    [Test]
    [Description("Users in a role, an unknown role has no users")]
    public async Task GetUsersInRole_Test()
    {
        await this.roles.CreateRoleAsync("Editors");
        var alice = await this.CreateIdentityAsync("alice");
        await this.CreateIdentityAsync("bob");

        this.roles.AddUserToRole(alice, "Editors");

        (await this.roles.GetUsersInRoleAsync("Editors")).Select(u => u.UserName).Should().Equal("alice");
        (await this.roles.GetRolesForUserAsync(alice)).Should().Equal("Editors");
        (await this.roles.GetUsersInRoleAsync("Nope")).Should().BeEmpty();
    }

    [Test]
    [Description("New users get the roles of the start groups (not the guest group)")]
    public async Task SetupUserRoles_Test()
    {
        this.AddGroup("Registered", GroupFlags.Flags.IsStart);
        this.AddGroup("Guests", GroupFlags.Flags.IsGuest | GroupFlags.Flags.IsStart);
        this.AddGroup("Admins", GroupFlags.Flags.IsAdmin);
        await this.roles.CreateRoleAsync("Registered");
        await this.roles.CreateRoleAsync("Guests");
        await this.roles.CreateRoleAsync("Admins");

        var alice = await this.CreateIdentityAsync("alice");

        await this.roles.SetupUserRolesAsync(TestDatabase.BoardId, alice);
        await this.roles.SetupUserRolesAsync(TestDatabase.BoardId, alice);

        (await this.roles.GetRolesForUserAsync(alice)).Should().Equal("Registered");
    }

    [Test]
    [Description("SyncRoles creates a role for every group except the guest group")]
    public async Task SyncRoles_Test()
    {
        this.AddGroup("Registered", GroupFlags.Flags.IsStart);
        this.AddGroup("Guests", GroupFlags.Flags.IsGuest);
        await this.roles.CreateRoleAsync("Registered");

        await this.roles.SyncRolesAsync(TestDatabase.BoardId);

        this.roles.GetAllRoles().Should().Equal("Registered");

        this.AddGroup("Editors", GroupFlags.Flags.None);
        await this.roles.SyncRolesAsync(TestDatabase.BoardId);

        this.roles.GetAllRoles().Should().BeEquivalentTo("Registered", "Editors");
    }

    [Test]
    [Description("Creating the forum user of an identity puts it into the groups of its roles")]
    public async Task CreateForumUser_Test()
    {
        var group = this.AddGroup("Editors", GroupFlags.Flags.None);
        await this.roles.CreateRoleAsync("Editors");
        var alice = await this.CreateIdentityAsync("alice");
        this.roles.AddUserToRole(alice, "Editors");

        var userId = await this.roles.CreateForumUserAsync(alice, "Alice W.", TestDatabase.BoardId);

        userId.Should().NotBeNull();
        var user = await this.Repository<User>().GetByIdAsync(userId.Value);
        user.ProviderUserKey.Should().Be(alice.Id);
        user.DisplayName.Should().Be("Alice W.");
        (await this.Repository<UserGroup>().GetAsync(x => x.UserID == userId.Value)).Select(x => x.GroupID).Should().Contain(group);
    }

    [Test]
    [Description("DidCreateForumUser reports success only when the forum user was created")]
    public async Task DidCreateForumUser_Test()
    {
        var alice = await this.CreateIdentityAsync("alice");

        this.roles.DidCreateForumUser(alice, TestDatabase.BoardId).Should().BeTrue();
        (await this.Repository<User>().CountAsync(u => u.ProviderUserKey == alice.Id)).Should().Be(1);

        var broken = new AspNetUsers { Id = "broken", UserName = null, Email = null };
        this.roles.DidCreateForumUser(broken, TestDatabase.BoardId).Should().BeFalse("the forum user could not be created");
    }

    [Test]
    [Description("IsMemberOfGroup only counts groups the user is a member of")]
    public void IsMemberOfGroup_Test()
    {
        List<GroupMember> groups =
        [
            new() { Name = "Editors", UserID = 1 },
            new() { Name = "Readers", UserID = null }
        ];

        this.roles.IsMemberOfGroup("Editors", groups).Should().BeTrue();
        this.roles.IsMemberOfGroup("Readers", groups).Should().BeFalse();
        this.roles.IsMemberOfGroup("Nope", groups).Should().BeFalse();
    }

    private async Task<AspNetUsers> CreateIdentityAsync(string name)
    {
        var user = new AspNetUsers
        {
            Id = $"{name}-key",
            UserName = name,
            LoweredUserName = name,
            Email = $"{name}@example.com",
            LoweredEmail = $"{name}@example.com",
            ApplicationId = Guid.NewGuid(),
            IsApproved = true
        };

        (await this.users.CreateUserAsync(user, "S3cret!pass")).Succeeded.Should().BeTrue();

        return user;
    }

    private int AddGroup(string name, GroupFlags.Flags flags)
    {
        return this.Repository<Group>().Insert(
            new Group { BoardID = TestDatabase.BoardId, Name = name, Flags = (int)flags, SortOrder = 1 });
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
