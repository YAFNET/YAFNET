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

using System.IO;

using Autofac;

using Microsoft.Owin.Security.DataProtection;

using NSubstitute;

using YAF.Core.Context.Start;
using YAF.Core.Extensions;
using YAF.Core.Identity;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Deleting users (admin) and deleting and banning spammers: posts, albums, avatar files, the identity and the log.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserDeleteTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IAspNetUsersHelper helper;

    private IDataProtectionProvider previousProvider;

    private string uploads;

    private int guest;

    private int forum;

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
                b.RegisterType<AspNetUsersHelper>().As<IAspNetUsersHelper>();
                b.RegisterType<Album>().As<IAlbum>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.helper = this.board.Resolve<IAspNetUsersHelper>();

        this.uploads = this.board.MapPath(this.board.Resolve<BoardFolders>().Uploads);
        Directory.CreateDirectory(this.uploads);

        this.guest = this.db.AddUser("Guest", u => u.Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsGuest));
        this.board.PageLoad.GuestUserID = this.guest;
        this.board.BoardSettings.BanBotIpOnDetection = false;

        this.forum = this.db.AddForum("General");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Startup.DataProtectionProvider = this.previousProvider;
    }

    [Test]
    [Description("Delete and ban never deletes the guest account and the posts of all guests")]
    public void Delete_And_Ban_Guest_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.guest, "Guest topic");
        this.db.AddMessage(topic, this.guest, "first guest post");
        this.db.AddMessage(topic, this.guest, "second guest post");

        var guestUser = this.Repository<User>().GetById(this.guest);

        this.helper.DeleteAndBanUser(guestUser, null, "1.2.3.4").Should().BeFalse();
        this.helper.DeleteAndBanUser(guestUser, new AspNetUsers { Id = "x", UserName = "Guest" }, "1.2.3.4")
            .Should().BeFalse();

        this.Repository<Message>().Count(m => m.UserID == this.guest).Should().Be(2);
        this.Repository<User>().GetById(this.guest).Should().NotBeNull();
    }

    [Test]
    [Description("Delete and ban removes the spammer with posts, albums and album files")]
    public void Delete_And_Ban_Spammer_Test()
    {
        var (spammer, aspNetUser) = this.AddIdentityUser("spammer");
        var topic = this.db.AddTopic(this.forum, spammer, "Spam");
        this.db.AddMessage(topic, spammer, "buy now");
        var (album, file) = this.AddAlbumWithImage(spammer);

        this.board.LoginAs(spammer);

        this.helper.DeleteAndBanUser(this.Repository<User>().GetById(spammer), aspNetUser, "1.2.3.4").Should().BeTrue();

        this.Repository<User>().GetById(spammer).Should().BeNull();
        this.Repository<Message>().Count(m => m.UserID == spammer).Should().Be(0);
        this.Repository<UserAlbum>().GetById(album).Should().BeNull();
        this.Repository<UserAlbumImage>().Count(i => i.AlbumID == album).Should().Be(0);
        this.Repository<AspNetUsers>().Count(u => u.Id == aspNetUser.Id).Should().Be(0);
        File.Exists(file).Should().BeFalse();
    }

    [Test]
    [Description("Deleting a user removes the albums and album files of the user, not of others")]
    public void Delete_User_Albums_Test()
    {
        var (alice, _) = this.AddIdentityUser("alice");
        var (bob, _) = this.AddIdentityUser("bob");

        var (aliceAlbum, aliceFile) = this.AddAlbumWithImage(alice);
        var (bobAlbum, bobFile) = this.AddAlbumWithImage(bob);

        this.helper.DeleteUser(alice).Should().BeTrue();

        this.Repository<UserAlbum>().GetById(aliceAlbum).Should().BeNull();
        File.Exists(aliceFile).Should().BeFalse();

        this.Repository<UserAlbum>().GetById(bobAlbum).Should().NotBeNull();
        File.Exists(bobFile).Should().BeTrue();

        this.helper.DeleteUser(9999).Should().BeFalse();
    }

    [Test]
    [Description("Deleting a user removes the uploaded avatar files of the user (in every image format), not of others")]
    public void Delete_User_Avatar_Files_Test()
    {
        var (alice, _) = this.AddIdentityUser("alice");
        var (bob, _) = this.AddIdentityUser("bob");

        this.board.BoardSettings.AvatarUpload = true;
        this.board.BoardSettings.UseFileTable = false;

        string[] aliceFiles = [Path.Combine(this.uploads, $"{alice}.png"), Path.Combine(this.uploads, $"{alice}.jpg")];
        var bobFile = Path.Combine(this.uploads, $"{bob}.png");

        foreach (var file in aliceFiles.Append(bobFile))
        {
            File.WriteAllBytes(file, [1]);
        }

        this.helper.DeleteUser(alice).Should().BeTrue();

        aliceFiles.Should().AllSatisfy(file => File.Exists(file).Should().BeFalse());
        File.Exists(bobFile).Should().BeTrue();
    }

    [Test]
    [Description("Avatar files are only deleted when the avatars are uploaded to the folder")]
    public void Delete_User_Keeps_Files_Without_Avatar_Upload_Test()
    {
        var (alice, _) = this.AddIdentityUser("alice");
        var (bob, _) = this.AddIdentityUser("bob");

        var aliceFile = Path.Combine(this.uploads, $"{alice}.png");
        var bobFile = Path.Combine(this.uploads, $"{bob}.png");
        File.WriteAllBytes(aliceFile, [1]);
        File.WriteAllBytes(bobFile, [1]);

        this.board.BoardSettings.AvatarUpload = false;
        this.helper.DeleteUser(alice).Should().BeTrue();
        File.Exists(aliceFile).Should().BeTrue();

        this.board.BoardSettings.AvatarUpload = true;
        this.board.BoardSettings.UseFileTable = true;
        this.helper.DeleteUser(bob).Should().BeTrue();
        File.Exists(bobFile).Should().BeTrue();
    }

    [Test]
    [Description("Deleting a user removes the identity and is logged with the name of the admin or the spam check")]
    public void Delete_User_Log_Test()
    {
        var (admin, _) = this.AddIdentityUser("admin");
        var (alice, _) = this.AddIdentityUser("alice");
        var (bob, _) = this.AddIdentityUser("bob");

        this.board.LoginAs(admin);
        this.board.BoardSettings.LogUserDeleted = true;

        this.helper.DeleteUser(alice).Should().BeTrue();
        this.helper.DeleteUser(bob, isBotAutoDelete: true).Should().BeTrue();

        this.Repository<AspNetUsers>().Count(u => u.Id == "alice-key").Should().Be(0);

        this.LoggedDeletions().Should().Equal(
            (admin, "User alice was deleted by admin."),
            (admin, "User bob was deleted by the automatic spam check system."));

        this.board.BoardSettings.LogUserDeleted = false;
        this.board.Logger.ClearReceivedCalls();

        var (carol, _) = this.AddIdentityUser("carol");
        this.helper.DeleteUser(carol).Should().BeTrue();

        this.LoggedDeletions().Should().BeEmpty();
    }

    [Test]
    [Description("Delete and ban counts the banned user, bans the IP when enabled and logs the deletion")]
    public void Delete_And_Ban_Event_And_Log_Test()
    {
        var (admin, _) = this.AddIdentityUser("admin");
        var (spammer, aspNetUser) = this.AddIdentityUser("spammer");
        var (other, otherAspNetUser) = this.AddIdentityUser("other");

        this.board.LoginAs(admin);
        this.board.BoardSettings.LogUserDeleted = true;
        this.board.BoardSettings.BanBotIpOnDetection = true;
        this.board.BoardSettings.BannedUsers = 0;

        this.helper.DeleteAndBanUser(this.Repository<User>().GetById(spammer), aspNetUser, "1.2.3.4").Should().BeTrue();

        var ban = this.board.Events.OfType<BanUserEvent>().Should().ContainSingle().Subject;
        ban.UserId.Should().Be(spammer);
        ban.Name.Should().Be("spammer");
        ban.IpAddress.Should().Be("1.2.3.4");

        this.board.BoardSettings.BannedUsers.Should().Be(1);
        this.LoggedDeletions().Should().Equal((admin, "User spammer was deleted by the automatic spam check system."));

        // without banning the ip and logging
        this.board.BoardSettings.LogUserDeleted = false;
        this.board.BoardSettings.BanBotIpOnDetection = false;
        this.board.Logger.ClearReceivedCalls();

        this.helper.DeleteAndBanUser(this.Repository<User>().GetById(other), otherAspNetUser, "5.6.7.8").Should().BeTrue();

        this.board.Events.OfType<BanUserEvent>().Should().ContainSingle();
        this.board.BoardSettings.BannedUsers.Should().Be(2);
        this.LoggedDeletions().Should().BeEmpty();
    }

    /// <summary>
    /// The user deleted log entries: the user id and the message.
    /// </summary>
    private List<(int?, string)> LoggedDeletions()
    {
        return this.board.Logger.ReceivedCalls().Where(c => c.GetMethodInfo().Name == nameof(ILoggerService.Log))
            .Select(c => c.GetArguments())
            .Where(a => (EventLogTypes)a[1] == EventLogTypes.UserDeleted)
            .Select(a => ((int?)a[2], (string)a[0])).ToList();
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

        this.Repository<AspNetUsers>().Insert(aspNetUser, false);

        var userId = this.db.AddUser(name, u => u.ProviderUserKey = aspNetUser.Id);

        return (userId, aspNetUser);
    }

    private (int AlbumId, string File) AddAlbumWithImage(int userId)
    {
        var albumId = this.Repository<UserAlbum>().Insert(
            new UserAlbum { UserID = userId, Title = "Album", Updated = DateTime.UtcNow });
        this.Repository<UserAlbumImage>().Insert(
            new UserAlbumImage { AlbumID = albumId, FileName = "pic.png", ContentType = "image/png", Bytes = 3, Uploaded = DateTime.UtcNow });

        var file = Path.Combine(this.uploads, $"{userId}.{albumId}.pic.png.yafalbum");
        File.WriteAllBytes(file, [1, 2, 3]);

        return (albumId, file);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
