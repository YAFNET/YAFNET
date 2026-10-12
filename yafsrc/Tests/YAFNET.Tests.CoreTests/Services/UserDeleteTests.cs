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

namespace YAF.Tests.CoreTests.Services;

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using NSubstitute;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Identity;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// Deleting users (admin delete and the spam "delete and ban"): albums with their files are removed,
/// and the guest account is never deleted.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserDeleteTests
{
    private string webRoot;

    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetUsersHelper helper;

    private int guest;

    private int forum;

    private SignInManager<AspNetUsers> signInManager;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-delete-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "uploads"));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

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
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.RegisterType<Album>().As<IAlbum>();
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterInstance(new BoardConfiguration { StaticFilesRoot = string.Empty });
                b.RegisterType<BoardFolders>().AsSelf();
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

        this.db = this.board.Database;
        this.helper = this.board.Resolve<IAspNetUsersHelper>();

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.board.PageLoad.GuestUserID = this.guest;
        this.board.BoardSettings.BanBotIpOnDetection = false;

        this.forum = this.db.AddForum("General");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();

        try
        {
            Directory.Delete(this.webRoot, true);
        }
        catch (IOException)
        {
            // ignore
        }
    }

    [Test]
    [Description("Delete and ban never deletes the guest account and the posts of all guests")]
    public async Task Delete_And_Ban_Guest_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.guest, "Guest topic");
        this.db.AddMessage(topic, this.guest, "first guest post");
        this.db.AddMessage(topic, this.guest, "second guest post");

        var guestUser = this.Repository<User>().GetById(this.guest);

        (await this.helper.DeleteAndBanUserAsync(guestUser, null, "1.2.3.4")).Should().BeFalse();
        (await this.helper.DeleteAndBanUserAsync(guestUser, new AspNetUsers { Id = "x", UserName = "Guest" }, "1.2.3.4"))
            .Should().BeFalse();

        this.Repository<Message>().Count(m => m.UserID == this.guest).Should().Be(2);
        this.Repository<User>().GetById(this.guest).Should().NotBeNull();
    }

    [Test]
    [Description("Delete and ban removes the spammer with posts, albums and album files")]
    public async Task Delete_And_Ban_Spammer_Test()
    {
        var (spammer, aspNetUser) = this.AddIdentityUser("spammer");
        var topic = this.db.AddTopic(this.forum, spammer, "Spam");
        this.db.AddMessage(topic, spammer, "buy now");
        var (album, file) = this.AddAlbumWithImage(spammer);

        // the spam check found it in a post of the spammer
        this.board.LoginAs(spammer);

        (await this.helper.DeleteAndBanUserAsync(this.Repository<User>().GetById(spammer), aspNetUser, "1.2.3.4"))
            .Should().BeTrue();

        this.Repository<User>().GetById(spammer).Should().BeNull();
        this.Repository<Message>().Count(m => m.UserID == spammer).Should().Be(0);
        this.Repository<UserAlbum>().GetById(album).Should().BeNull();
        this.Repository<UserAlbumImage>().Count(i => i.AlbumID == album).Should().Be(0);
        File.Exists(file).Should().BeFalse();

        await this.board.Resolve<SignInManager<AspNetUsers>>().Received(1).SignOutAsync();
    }

    [Test]
    [Description("Deleting and banning another user (an admin action) does not log out the current user")]
    public async Task Delete_And_Ban_Other_User_Keeps_Login_Test()
    {
        var (admin, _) = this.AddIdentityUser("admin");
        var (spammer, aspNetUser) = this.AddIdentityUser("spammer");

        this.board.LoginAs(admin);

        (await this.helper.DeleteAndBanUserAsync(this.Repository<User>().GetById(spammer), aspNetUser, "1.2.3.4"))
            .Should().BeTrue();

        await this.board.Resolve<SignInManager<AspNetUsers>>().DidNotReceive().SignOutAsync();
    }

    [Test]
    [Description("Deleting a user removes the albums and album files of the user, not of others")]
    public async Task Delete_User_Albums_Test()
    {
        var (alice, _) = this.AddIdentityUser("alice");
        var (bob, _) = this.AddIdentityUser("bob");

        var (aliceAlbum, aliceFile) = this.AddAlbumWithImage(alice);
        var (bobAlbum, bobFile) = this.AddAlbumWithImage(bob);

        (await this.helper.DeleteUserAsync(alice)).Should().BeTrue();

        this.Repository<UserAlbum>().GetById(aliceAlbum).Should().BeNull();
        File.Exists(aliceFile).Should().BeFalse();

        this.Repository<UserAlbum>().GetById(bobAlbum).Should().NotBeNull();
        File.Exists(bobFile).Should().BeTrue();

        (await this.helper.DeleteUserAsync(9999)).Should().BeFalse();
    }

    private (int UserId, AspNetUsers AspNetUser) AddIdentityUser(string name)
    {
        var aspNetUser = new AspNetUsers
        {
            Id = $"{name}-key",
            UserName = name,
            LoweredUserName = name,
            Email = $"{name}@example.com",
            LoweredEmail = $"{name}@example.com",
            IsApproved = true
        };

        this.Repository<AspNetUsers>().Insert(aspNetUser);

        var userId = this.db.AddUser(name, u => u.ProviderUserKey = aspNetUser.Id);

        return (userId, aspNetUser);
    }

    private (int AlbumId, string File) AddAlbumWithImage(int userId)
    {
        var albumId = (int)this.Repository<UserAlbum>().Insert(
            new UserAlbum { UserID = userId, Title = "Album", Updated = DateTime.UtcNow },
            true);
        this.Repository<UserAlbumImage>().Insert(
            new UserAlbumImage { AlbumID = albumId, FileName = "pic.png", ContentType = "image/png", Bytes = 3, Uploaded = DateTime.UtcNow });

        var file = Path.Combine(this.webRoot, "uploads", $"{userId}.{albumId}.pic.png.yafalbum");
        File.WriteAllBytes(file, [1, 2, 3]);

        return (albumId, file);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
