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
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Identity;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Exceptions;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// <see cref="AspNetUsersHelper"/> on top of the real <see cref="AspNetUsersManager"/> and <see cref="UserStore"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AspNetUsersHelperTests
{
    private const int NoGuest = 9999;

    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetUsersHelper helper;

    private IAspNetRolesHelper roles;

    [SetUp]
    public void SetUp()
    {
        this.roles = Substitute.For<IAspNetRolesHelper>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.roles).As<IAspNetRolesHelper>();
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
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();

                // deleting a user removes the uploaded files of the user
                var environment = Substitute.For<IWebHostEnvironment>();
                environment.WebRootPath.Returns(System.IO.Path.GetTempPath());
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new YAF.Types.Objects.BoardConfiguration());
                b.RegisterType<Album>().As<YAF.Types.Interfaces.Services.IAlbum>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.helper = this.board.Resolve<IAspNetUsersHelper>();

        // the harness default (1) would be the first user added here
        this.board.PageLoad.GuestUserID = NoGuest;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The guest user of the board is found and cached")]
    public void GuestUser_Test()
    {
        var guest = this.AddGuest();

        this.helper.GuestUser(TestDatabase.BoardId).ID.Should().Be(guest);

        this.Repository<User>().Delete(u => u.ID == guest);
        this.helper.GuestUser(TestDatabase.BoardId).ID.Should().Be(guest, "the guest user is cached");
    }

    [Test]
    [Description("A board without a guest user throws")]
    public async Task GuestUser_Missing_Test()
    {
        this.db.AddUser("Alice");

        this.helper.Invoking(h => h.GuestUser(TestDatabase.BoardId)).Should().Throw<NoValidGuestUserForBoardException>();
        await this.helper.Invoking(h => h.GuestUserAsync(TestDatabase.BoardId))
            .Should().ThrowAsync<NoValidGuestUserForBoardException>();
    }

    [Test]
    [Description("A board with more than one guest user throws")]
    public async Task GuestUser_Two_Guests_Test()
    {
        this.AddGuest();
        this.AddGuest("Guest2");

        await this.helper.Invoking(h => h.GuestUserAsync(TestDatabase.BoardId)).Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Description("Users log in with user name, e-mail or (when enabled) display name")]
    public async Task ValidateUser_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com", displayName: "Alice Wonder");

        (await this.helper.ValidateUserAsync("alice")).Id.Should().Be("alice-key");
        (await this.helper.ValidateUserAsync("ALICE")).Id.Should().Be("alice-key");
        (await this.helper.ValidateUserAsync("alice@example.com")).Id.Should().Be("alice-key");

        this.board.BoardSettings.EnableDisplayName = false;
        (await this.helper.ValidateUserAsync("Alice Wonder")).Should().BeNull();

        this.board.BoardSettings.EnableDisplayName = true;
        (await this.helper.ValidateUserAsync("Alice Wonder")).Id.Should().Be("alice-key");
    }

    [Test]
    [Description("Unknown names, unknown e-mails and the guest's display name do not validate")]
    public async Task ValidateUser_Unknown_Test()
    {
        this.board.BoardSettings.EnableDisplayName = true;
        this.AddGuest();

        (await this.helper.ValidateUserAsync("nobody")).Should().BeNull();
        (await this.helper.ValidateUserAsync("nobody@example.com")).Should().BeNull();
        (await this.helper.ValidateUserAsync("Guest")).Should().BeNull();
    }

    [Test]
    [Description("UserExists checks the name, or the e-mail when no name is given")]
    public async Task UserExists_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");

        this.helper.UserExists("alice", null).Should().BeTrue();
        this.helper.UserExists(null, "alice@example.com").Should().BeTrue();
        this.helper.UserExists("bob", "alice@example.com").Should().BeFalse("the e-mail is only checked without a name");
        this.helper.UserExists(null, null).Should().BeFalse();
    }

    [Test]
    [Description("Creating a user stores a hash that verifies the password")]
    public async Task CreateUser_Password_Test()
    {
        var user = NewAspNetUser("new-key", "newbie", "newbie@example.com");

        (await this.helper.CreateUserAsync(user, "S3cret!pass")).Succeeded.Should().BeTrue();

        var stored = await this.helper.GetUserAsync("new-key");
        stored.PasswordHash.Should().NotBeNullOrEmpty().And.NotContain("S3cret");
        this.helper.PasswordHasher.VerifyHashedPassword(stored, stored.PasswordHash, "S3cret!pass")
            .Should().NotBe(PasswordVerificationResult.Failed);
        this.helper.PasswordHasher.VerifyHashedPassword(stored, stored.PasswordHash, "wrong")
            .Should().Be(PasswordVerificationResult.Failed);
    }

    [Test]
    [Description("Approving a user approves both the identity and the forum user and removes the e-mail check")]
    public async Task ApproveUser_Test()
    {
        var userId = await this.AddMemberAsync("bob-key", "bob", "bob@example.com", approved: false);
        this.Insert(new CheckEmail { UserID = userId, Email = "bob@example.com", Hash = "h", Created = DateTime.UtcNow });

        (await this.helper.ApproveUserAsync(userId)).Should().BeTrue();

        (await this.helper.GetUserAsync("bob-key")).IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(userId)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<CheckEmail>().CountAsync(c => c.UserID == userId)).Should().Be(0);
    }

    [Test]
    [Description("Approving an unknown user fails")]
    public async Task ApproveUser_Unknown_Test()
    {
        (await this.helper.ApproveUserAsync(9999)).Should().BeFalse();
    }

    [Test]
    [Description("ApproveAll approves the unapproved users of the board, not the disabled (deleted) accounts and not the users of other boards")]
    public async Task ApproveAll_Test()
    {
        var bob = await this.AddMemberAsync("b-bob", "bob", "bob@example.com", approved: false);
        var carol = await this.AddMemberAsync("c-carol", "carol", "carol@example.com", approved: false);
        var disabled = await this.AddMemberAsync("d-disabled", "disabled", "disabled@example.com", approved: false);
        var otherBoard = await this.AddMemberAsync("e-other", "other", "other@example.com", approved: false);

        await this.Repository<User>().UpdateOnlyAsync(() => new User { Flags = (int)UserFlags.Flags.IsDeleted }, u => u.ID == disabled);
        await this.Repository<User>().UpdateOnlyAsync(() => new User { BoardID = TestDatabase.BoardId + 1 }, u => u.ID == otherBoard);

        // a forum user without identity is skipped
        var noIdentity = this.db.AddUser("NoIdentity", u => u.Flags = 0);

        await this.helper.ApproveAllAsync(TestDatabase.BoardId);

        (await this.Repository<User>().GetByIdAsync(bob)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(carol)).UserFlags.IsApproved.Should().BeTrue();

        (await this.Repository<User>().GetByIdAsync(disabled)).UserFlags.IsApproved.Should().BeFalse("disabled accounts stay disabled");
        (await this.helper.GetUserAsync("d-disabled")).IsApproved.Should().BeFalse();

        (await this.Repository<User>().GetByIdAsync(otherBoard)).UserFlags.IsApproved.Should().BeFalse("a user of another board");
        (await this.Repository<User>().GetByIdAsync(noIdentity)).UserFlags.IsApproved.Should().BeFalse();
    }

    [Test]
    [Description("DeleteAllUnapproved deletes unapproved users created before the cutoff")]
    public async Task DeleteAllUnapproved_Test()
    {
        var old = await this.AddMemberAsync("old-key", "old", "old@example.com", approved: false, created: DateTime.UtcNow.AddDays(-10));
        var fresh = await this.AddMemberAsync("fresh-key", "fresh", "fresh@example.com", approved: false, created: DateTime.UtcNow);
        var approved = await this.AddMemberAsync("ok-key", "ok", "ok@example.com", created: DateTime.UtcNow.AddDays(-10));

        var otherBoard = await this.AddMemberAsync("other-key", "other", "other@example.com", approved: false, created: DateTime.UtcNow.AddDays(-10));
        await this.Repository<User>().UpdateOnlyAsync(() => new User { BoardID = TestDatabase.BoardId + 1 }, u => u.ID == otherBoard);

        await this.helper.DeleteAllUnapprovedAsync(TestDatabase.BoardId, DateTime.UtcNow.AddDays(-5));

        (await this.Repository<User>().GetByIdAsync(otherBoard)).Should().NotBeNull("a user of another board");
        (await this.Repository<User>().GetByIdAsync(old)).Should().BeNull();
        (await this.helper.GetUserAsync("old-key")).Should().BeNull();
        (await this.Repository<User>().GetByIdAsync(fresh)).Should().NotBeNull();
        (await this.Repository<User>().GetByIdAsync(approved)).Should().NotBeNull();
    }

    [Test]
    [Description("LockInactiveAccounts locks approved members who have not visited since the cutoff")]
    public async Task LockInactiveAccounts_Test()
    {
        var guest = this.AddGuest(configure: u => u.LastVisit = DateTime.UtcNow.AddYears(-2));
        var inactive = await this.AddMemberAsync("inactive-key", "Inactive", "inactive@example.com");
        await this.Repository<User>().UpdateOnlyAsync(() => new User { LastVisit = DateTime.UtcNow.AddYears(-2) }, u => u.ID == inactive);
        var active = this.db.AddUser("Active", u => u.LastVisit = DateTime.UtcNow);
        var stamp = (await this.helper.GetUserAsync("inactive-key")).SecurityStamp;

        // the admin who locks the accounts (the current user is never locked)
        this.board.LoginAs(active);

        await this.helper.LockInactiveAccountsAsync(DateTime.UtcNow.AddYears(-1));

        var locked = (await this.Repository<User>().GetByIdAsync(inactive)).UserFlags;
        locked.IsApproved.Should().BeFalse();
        locked.IsDeleted.Should().BeTrue();

        // the identity is not approved either, so the user can not log in again
        var identity = await this.helper.GetUserAsync("inactive-key");
        identity.IsApproved.Should().BeFalse();
        identity.SecurityStamp.Should().NotBe(stamp, "the existing logins end");

        (await this.Repository<User>().GetByIdAsync(active)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(guest)).UserFlags.IsApproved.Should().BeTrue("the guest is never locked");
    }

    [Test]
    [Description("LockInactiveAccounts never locks host admins, members of admin groups or the current user")]
    public async Task LockInactiveAccounts_Keeps_Admins_Test()
    {
        var hostAdmin = this.db.AddUser(
            "Host",
            u =>
            {
                u.LastVisit = DateTime.UtcNow.AddYears(-3);
                u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsHostAdmin);
            });
        var admin = this.db.AddUser("Admin", u => u.LastVisit = DateTime.UtcNow.AddYears(-3));
        this.db.AddForumAccess(admin, this.db.AddForum(), groupFlags: GroupFlags.Flags.IsAdmin);
        var current = this.db.AddUser("Current", u => u.LastVisit = DateTime.UtcNow.AddYears(-3));
        var inactive = this.db.AddUser("Inactive", u => u.LastVisit = DateTime.UtcNow.AddYears(-3));

        this.board.LoginAs(current);

        await this.helper.LockInactiveAccountsAsync(DateTime.UtcNow.AddYears(-1));

        (await this.Repository<User>().GetByIdAsync(hostAdmin)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(admin)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(current)).UserFlags.IsApproved.Should().BeTrue();
        (await this.Repository<User>().GetByIdAsync(inactive)).UserFlags.IsApproved.Should().BeFalse();
    }

    [Test]
    [Description("Changing the e-mail updates the identity and the forum user")]
    public async Task UpdateEmail_Test()
    {
        var userId = await this.AddMemberAsync("alice-key", "alice", "alice@example.com");
        var user = await this.helper.GetUserAsync("alice-key");

        (await this.helper.UpdateEmailAsync(user, "new@example.com")).Should().BeTrue();

        (await this.helper.GetUserByEmailAsync("new@example.com")).Id.Should().Be("alice-key");
        (await this.Repository<User>().GetByIdAsync(userId)).Email.Should().Be("new@example.com");
    }

    [Test]
    [Description("Lookups between forum users and identities")]
    public async Task Lookups_Test()
    {
        var userId = await this.AddMemberAsync("alice-key", "alice", "alice@example.com");

        this.helper.GetUserProviderKeyFromUserId(userId).Should().Be("alice-key");
        (await this.helper.GetUserFromProviderUserKeyAsync("alice-key")).ID.Should().Be(userId);
        (await this.helper.GetMembershipUserByIdAsync(userId)).UserName.Should().Be("alice");
        (await this.helper.GetUserByNameAsync("alice")).Id.Should().Be("alice-key");
        this.helper.FindUsersByName("alice").Should().ContainSingle();
        this.helper.FindUsersByEmail("alice@example.com").Should().ContainSingle();
        this.helper.GetAllUsers().Should().ContainSingle();

        var boardUser = await this.helper.GetBoardUserAsync(userId);
        boardUser.Item1.ID.Should().Be(userId);
        boardUser.Item2.Id.Should().Be("alice-key");

        this.helper.IsGuestUser(NoGuest).Should().BeTrue();
        this.helper.IsGuestUser(userId).Should().BeFalse();
    }

    [Test]
    [Description("The current identity comes from the authenticated HTTP user")]
    public async Task GetUser_From_HttpContext_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");

        (await this.helper.GetUserAsync()).Should().BeNull("the request is anonymous");

        this.board.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(ClaimTypes.Name, "Alice")], "test"));

        (await this.helper.GetUserAsync()).Id.Should().Be("alice-key");
        (await this.helper.GetUserAsync()).Id.Should().Be("alice-key");
    }

    [Test]
    [Description("Two factor authentication stores the key when enabled and removes it when disabled")]
    public async Task Two_Factor_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");
        var user = await this.helper.GetUserByNameAsync("alice");

        await this.helper.SetTwoFactorEnabledAsync(user, "secret-key");

        var enabled = await this.helper.GetUserByNameAsync("alice");
        enabled.TwoFactorEnabled.Should().BeTrue();
        enabled.MobilePIN.Should().Be("secret-key");

        await this.helper.SetTwoFactorDisabledAsync(enabled, "secret-key");

        var disabled = await this.helper.GetUserByNameAsync("alice");
        disabled.TwoFactorEnabled.Should().BeFalse();
        disabled.MobilePIN.Should().BeEmpty();
    }

    [Test]
    [Description("Identities without a forum user get one, existing forum users are kept")]
    public async Task Import_Membership_Users_Test()
    {
        var created = new List<string>();
        this.roles.CreateForumUserAsync(Arg.Any<AspNetUsers>(), Arg.Any<int>()).Returns(
            c =>
            {
                created.Add(c.Arg<AspNetUsers>().UserName);
                return (int?)this.db.AddUser(c.Arg<AspNetUsers>().UserName, u => u.ProviderUserKey = c.Arg<AspNetUsers>().Id);
            });

        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");

        // an identity without forum user (e.g. created by another application)
        await this.board.Resolve<AspNetUsersManager>().CreateUserAsync(NewAspNetUser("bob-key", "Bob", "Bob@Example.com"), "S3cret!pass");

        this.board.BoardSettings.Registry["ApplicationId"] = Guid.NewGuid().ToString();
        this.board.BoardSettings.BoardId = TestDatabase.BoardId;

        await this.helper.ImportAllMembershipUsersAsync();

        created.Should().Equal("Bob");
        await this.roles.Received(1).SetupUserRolesAsync(TestDatabase.BoardId, Arg.Is<AspNetUsers>(u => u.LoweredEmail == "bob@example.com"));
        (await this.helper.GetUserFromProviderUserKeyAsync("bob-key")).Should().NotBeNull();
    }

    [Test]
    [Description("An external login is added once per user")]
    public async Task AddLogin_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");

        this.helper.AddLogin("alice-key", new UserLoginInfo("Google", "g-1", "Google"));
        this.helper.AddLogin("alice-key", new UserLoginInfo("Google", "g-1", "Google"));

        (await this.Repository<AspNetUserLogins>().CountAsync(l => l.UserId == "alice-key")).Should().Be(1);
        this.helper.Invoking(h => h.AddLogin(null, new UserLoginInfo("Google", "g", "Google"))).Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Description("Roles through the user manager")]
    public async Task Roles_Test()
    {
        await this.AddMemberAsync("alice-key", "alice", "alice@example.com");
        this.Insert(new AspNetRoles { Id = "r1", Name = "Moderators" });
        var user = await this.helper.GetUserAsync("alice-key");

        this.helper.AddToRole(user, "Moderators");

        (await this.helper.IsUserInRoleAsync(user, "Moderators")).Should().BeTrue();
        (await this.helper.GetUserRolesAsync(user)).Should().Equal("Moderators");

        (await this.helper.RemoveUserFromRoleAsync(user, "Moderators")).Succeeded.Should().BeTrue();
        (await this.helper.IsUserInRoleAsync(user, "Moderators")).Should().BeFalse();
    }

    [Test]
    [Description("Admin user list: filters by name, e-mail, suspension, rank and group, with paging and total count")]
    public async Task GetUsersPaged_Test()
    {
        this.AddGuest("zguest");
        var alice = await this.AddMemberAsync("a", "alice", "alice@example.com");
        var bob = await this.AddMemberAsync("b", "bob", "bob@other.org");
        await this.AddMemberAsync("c", "carol", "carol@example.com", approved: false);

        await this.Repository<User>().UpdateOnlyAsync(() => new User { Suspended = DateTime.UtcNow.AddDays(1) }, u => u.ID == bob);
        var groupId = this.db.AddForumAccess(alice, this.db.AddForum());

        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, null, false, null, null)).Should().Equal("alice", "bob", "zguest");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, null, false, null, null, includeGuests: false)).Should().Equal("alice", "bob");
        Names(this.helper.GetUsersPaged(null, 0, 10, "ali", null, null, false, null, null)).Should().Equal("alice");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, "other.org", null, false, null, null)).Should().Equal("bob");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, null, true, null, null)).Should().Equal("bob");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, null, false, groupId, null)).Should().Equal("alice");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, null, false, null, TestDatabase.RankId, includeGuests: false)).Should().Equal("alice", "bob");
        Names(this.helper.GetUsersPaged(null, 0, 10, null, null, DateTime.UtcNow.AddDays(1), false, null, null)).Should().BeEmpty();

        var page = this.helper.GetUsersPaged(null, 1, 2, null, null, null, false, null, null);
        Names(page).Should().Equal("zguest");
        page[0].TotalRows.Should().Be(3);
        page[0].IsGuest.Should().BeTrue();
    }

    [Test]
    [Description("Member list: hides guests and deleted users, filters and sorts")]
    public async Task ListMembersPaged_Test()
    {
        this.AddGuest();
        var alice = await this.AddMemberAsync("a", "alice", "alice@example.com", numPosts: 5);
        await this.AddMemberAsync("b", "bob", "bob@example.com", numPosts: 20);
        var deleted = await this.AddMemberAsync("d", "dave", "dave@example.com");
        await this.Repository<User>().UpdateOnlyAsync(
            () => new User { Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsDeleted) },
            u => u.ID == deleted);

        Names(this.Members(sortName: 1)).Should().Equal("alice", "bob");
        Names(this.Members(sortName: 2)).Should().Equal("bob", "alice");
        Names(this.Members(sortPosts: 2)).Should().Equal("bob", "alice");
        Names(this.Members(startLetter: 'b')).Should().Equal("bob");
        Names(this.Members(name: "ali")).Should().Equal("alice");
        Names(this.Members(numPosts: 10, numPostCompare: 3)).Should().Equal("bob");
        Names(this.Members(numPosts: 10, numPostCompare: 2)).Should().Equal("alice");
        Names(this.Members(numPosts: 5, numPostCompare: 1)).Should().Equal("alice");

        var groupId = this.db.AddForumAccess(alice, this.db.AddForum());
        Names(this.Members(groupId: groupId)).Should().Equal("alice");

        var page = this.Members(sortName: 1, pageIndex: 1, pageSize: 1);
        Names(page).Should().Equal("bob");
        page[0].TotalRows.Should().Be(2);
    }

    private static List<string> Names(List<YAF.Types.Objects.Model.PagedUser> users)
    {
        return [.. users.Select(u => u.Name)];
    }

    private List<YAF.Types.Objects.Model.PagedUser> Members(
        int? groupId = null,
        char startLetter = char.MinValue,
        string name = null,
        int pageIndex = 0,
        int pageSize = 10,
        int? sortName = null,
        int? sortPosts = null,
        int? numPosts = null,
        int numPostCompare = 0)
    {
        return this.helper.ListMembersPaged(
            null,
            groupId,
            null,
            startLetter,
            name,
            pageIndex,
            pageSize,
            sortName,
            null,
            null,
            sortPosts,
            null,
            numPosts,
            numPostCompare);
    }

    private static AspNetUsers NewAspNetUser(string id, string name, string email, bool approved = true, DateTime? created = null)
    {
        return new AspNetUsers
        {
            Id = id,
            UserName = name,
            LoweredUserName = name.ToLowerInvariant(),
            Email = email,
            LoweredEmail = email.ToLowerInvariant(),
            ApplicationId = Guid.NewGuid(),
            IsApproved = approved,
            CreateDate = created ?? DateTime.UtcNow
        };
    }

    /// <summary>
    /// Adds an identity and the matching forum user, linked by the provider key.
    /// </summary>
    private async Task<int> AddMemberAsync(
        string key,
        string name,
        string email,
        bool approved = true,
        string displayName = null,
        DateTime? created = null,
        int numPosts = 0)
    {
        var result = await this.board.Resolve<AspNetUsersManager>()
            .CreateUserAsync(NewAspNetUser(key, name, email, approved, created), "S3cret!pass");

        result.Succeeded.Should().BeTrue();

        return this.db.AddUser(
            name,
            u =>
            {
                u.ProviderUserKey = key;
                u.Email = email;
                u.DisplayName = displayName ?? name;
                u.NumPosts = numPosts;
                u.Flags = approved ? (int)UserFlags.Flags.IsApproved : 0;
            });
    }

    private int AddGuest(string name = "Guest", Action<User> configure = null)
    {
        return this.db.AddUser(
            name,
            u =>
            {
                u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest);
                configure?.Invoke(u);
            });
    }

    private void Insert<T>(T entity)
    {
        using var connection = this.db.Open();
        connection.Insert(entity);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
