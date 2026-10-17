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

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.DataProtection;

using NSubstitute;

using YAF.Core.Context.Start;
using YAF.Core.Extensions;
using YAF.Core.Identity;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// The identity roles of the users (<see cref="RoleStore"/>, <see cref="AspNetRolesHelper"/>), keeping the
/// identity roles and forum groups in sync.
/// </summary>
[TestFixture]
[NonParallelizable]
public class RolesTests
{
    private TestBoardContext board;

    private IAspNetRolesHelper roles;

    private AspNetUsersManager users;

    private IDataProtectionProvider previousProvider;

    [SetUp]
    public void SetUp()
    {
        // the user manager creates its token provider from the OWIN data protection
        this.previousProvider = Startup.DataProtectionProvider;
        Startup.DataProtectionProvider = Substitute.For<IDataProtectionProvider>();
        Startup.DataProtectionProvider.Create(Arg.Any<string[]>()).Returns(Substitute.For<IDataProtector>());

        this.board = new TestBoardContext(
            b =>
            {
                b.Register(c => new AspNetUsersManager(new UserStore(c.Resolve<IServiceLocator>()))).AsSelf()
                    .SingleInstance();
                b.Register(c => new AspNetRoleManager(new RoleStore(c.Resolve<IServiceLocator>())))
                    .As<IAspNetRoleManager>().SingleInstance();
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.RegisterType<AspNetRolesHelper>().As<IAspNetRolesHelper>().AsSelf();
            },
            useDatabase: true);

        this.board.PageLoad.GuestUserID = 9999;
        this.roles = this.board.Resolve<IAspNetRolesHelper>();
        this.users = this.board.Resolve<AspNetUsersManager>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Startup.DataProtectionProvider = this.previousProvider;
    }

    [Test]
    [Description("RoleStore: create, find, update and delete")]
    public async Task RoleStore_Crud_Test()
    {
        using var store = new RoleStore(this.board.Resolve<IServiceLocator>());
        var role = new AspNetRoles { Id = Guid.NewGuid().ToString(), Name = "Moderators" };

        await store.CreateAsync(role);

        (await store.FindByIdAsync(role.Id)).Name.Should().Be("Moderators");
        (await store.FindByNameAsync("Moderators")).Id.Should().Be(role.Id);
        store.Roles.Should().ContainSingle();

        role.Name = "Mods";
        await store.UpdateAsync(role);
        (await store.FindByIdAsync(role.Id)).Name.Should().Be("Mods");

        await store.DeleteAsync(role);
        (await store.FindByIdAsync(role.Id)).Should().BeNull();
    }

    [Test]
    [Description("Create, list, check and delete roles")]
    public void Create_Delete_Role_Test()
    {
        this.roles.CreateRole("Editors");
        this.roles.CreateRole("Readers");

        this.roles.GetAllRoles().Should().BeEquivalentTo(["Editors", "Readers"]);
        this.roles.RoleExists("Editors").Should().BeTrue();

        this.roles.DeleteRole("Editors");

        this.roles.RoleExists("Editors").Should().BeFalse();
        this.roles.GetAllRoles().Should().Equal("Readers");
    }

    [Test]
    [Description("Deleting a role that does not exist does nothing")]
    public void Delete_Unknown_Role_Test()
    {
        this.roles.Invoking(r => r.DeleteRole("Nope")).Should().NotThrow();
    }

    [Test]
    [Description("Users in a role, an unknown role has no users")]
    public void GetUsersInRole_Test()
    {
        this.roles.CreateRole("Editors");
        var alice = this.CreateIdentity("alice");
        this.CreateIdentity("bob");

        this.roles.AddUserToRole(alice, "Editors");

        this.roles.GetUsersInRole("Editors").Select(u => u.UserName).Should().Equal("alice");
        this.roles.GetRolesForUser(alice).Should().Equal("Editors");
        this.roles.IsUserInRole(alice, "Editors").Should().BeTrue();
        this.roles.GetUsersInRole("Nope").Should().BeEmpty();
    }

    [Test]
    [Description("New users get the roles of the start groups (not the guest group)")]
    public void SetupUserRoles_Test()
    {
        this.AddGroup("Registered", GroupFlags.Flags.IsStart);
        this.AddGroup("Guests", GroupFlags.Flags.IsGuest | GroupFlags.Flags.IsStart);
        this.AddGroup("Admins", GroupFlags.Flags.IsAdmin);
        this.roles.CreateRole("Registered");
        this.roles.CreateRole("Guests");
        this.roles.CreateRole("Admins");

        var alice = this.CreateIdentity("alice");

        this.roles.SetupUserRoles(TestDatabase.BoardId, alice);
        this.roles.SetupUserRoles(TestDatabase.BoardId, alice);

        this.roles.GetRolesForUser(alice).Should().Equal("Registered");
    }

    [Test]
    [Description("SyncRoles creates a role for every group except the guest group")]
    public void SyncRoles_Test()
    {
        this.AddGroup("Registered", GroupFlags.Flags.IsStart);
        this.AddGroup("Guests", GroupFlags.Flags.IsGuest);
        this.roles.CreateRole("Registered");

        this.roles.SyncRoles(TestDatabase.BoardId);

        this.roles.GetAllRoles().Should().Equal("Registered");

        this.AddGroup("Editors", GroupFlags.Flags.None);
        this.roles.SyncRoles(TestDatabase.BoardId);

        this.roles.GetAllRoles().Should().BeEquivalentTo(["Registered", "Editors"]);
    }

    [Test]
    [Description("Creating the forum user of an identity puts it into the groups of its roles")]
    public void CreateForumUser_Test()
    {
        var group = this.AddGroup("Editors", GroupFlags.Flags.None);
        this.roles.CreateRole("Editors");
        var alice = this.CreateIdentity("alice");
        this.roles.AddUserToRole(alice, "Editors");

        var userId = this.roles.CreateForumUser(alice, "Alice W.", TestDatabase.BoardId);

        userId.Should().NotBeNull();
        var user = this.Repository<User>().GetById(userId.Value);
        user.ProviderUserKey.Should().Be(alice.Id);
        user.DisplayName.Should().Be("Alice W.");
        this.Repository<UserGroup>().Get(x => x.UserID == userId.Value).Select(x => x.GroupID).Should().Contain(group);
    }

    [Test]
    [Description("DidCreateForumUser reports success only when the forum user was created")]
    public void DidCreateForumUser_Test()
    {
        var alice = this.CreateIdentity("alice");

        this.roles.DidCreateForumUser(alice, TestDatabase.BoardId).Should().BeTrue();
        this.Repository<User>().Count(u => u.ProviderUserKey == alice.Id).Should().Be(1);

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

    private AspNetUsers CreateIdentity(string name)
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

        var result = this.users.Create(user, "S3cret!pass");
        result.Succeeded.Should().BeTrue(string.Join(", ", result.Errors));

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
