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

namespace YAF.Tests.Data;

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// User avatars (deleting uploaded files), album / signature limits and the reporters of a post.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserAvatarSignatureTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private string contentRoot;

    private string webRoot;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        this.contentRoot = Path.Combine(Path.GetTempPath(), $"yaf-avatar-{Guid.NewGuid():N}");
        this.webRoot = Path.Combine(this.contentRoot, "wwwroot");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "uploads"));
        Directory.CreateDirectory(Path.Combine(this.webRoot, "css"));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.contentRoot, true);
    }

    [Test]
    [Description("Deleting an uploaded avatar removes the file and clears the avatar")]
    public async Task Delete_Uploaded_Avatar_Test()
    {
        var file = Path.Combine(this.webRoot, "uploads", "alice.webp");
        await File.WriteAllTextAsync(file, "image");
        this.SetAvatar("/uploads/alice.webp");

        await this.Users.DeleteAvatarAsync(this.alice);

        File.Exists(file).Should().BeFalse();
        var user = await this.Users.GetByIdAsync(this.alice);
        user.Avatar.Should().BeNull();
        user.AvatarImage.Should().BeNull();
    }

    [Test]
    [Description("Deleting an avatar never deletes files outside the uploads folder")]
    [TestCase("/../appsettings.json", "appsettings.json")]
    [TestCase("/css/site.css", "wwwroot/css/site.css")]
    [TestCase("/uploads/../css/site.css", "wwwroot/css/site.css")]
    public async Task Delete_Avatar_Outside_Uploads_Test(string avatar, string protectedFile)
    {
        var file = Path.Combine(this.contentRoot, protectedFile);
        await File.WriteAllTextAsync(file, "important");
        this.SetAvatar(avatar);

        await this.Users.DeleteAvatarAsync(this.alice);

        File.Exists(file).Should().BeTrue($"the avatar {avatar} must not delete {protectedFile}");
        (await this.Users.GetByIdAsync(this.alice)).Avatar.Should().BeNull("the avatar is cleared anyway");
    }

    [Test]
    [Description("Album and signature limits are the highest of the user's groups and rank")]
    public async Task Album_And_Signature_Limits_Test()
    {
        var rank = this.db.AddRank("Limited", configure: r =>
        {
            r.UsrAlbums = 2;
            r.UsrAlbumImages = 10;
            r.UsrSigChars = 100;
            r.UsrSigBBCodes = "B";
        });
        await this.Users.UpdateOnlyAsync(() => new User { RankID = rank }, u => u.ID == this.alice);

        var group = this.db.AddForumAccess(this.alice, this.db.AddForum());
        await this.board.Current.GetRepository<Group>().UpdateOnlyAsync(
            () => new Group { UsrAlbums = 5, UsrAlbumImages = 3, UsrSigChars = 50, UsrSigBBCodes = "B,I,URL" },
            g => g.ID == group);

        var albums = await this.Users.MaxAlbumDataAsync(this.alice, TestDatabase.BoardId);
        ((int)albums.UserAlbum).Should().Be(5);
        ((int)albums.UserAlbumImages).Should().Be(10);

        var signature = await this.Users.SignatureDataAsync(this.alice, TestDatabase.BoardId);
        ((int)signature.UsrSigChars).Should().Be(100);
        ((string)signature.UsrSigBBCodes).Should().Be("B,I,URL");

        // the groups allow no bb codes, the codes of the rank are used
        await this.board.Current.GetRepository<Group>().UpdateOnlyAsync(
            () => new Group { UsrSigBBCodes = null },
            g => g.ID == group);

        signature = await this.Users.SignatureDataAsync(this.alice, TestDatabase.BoardId);
        ((string)signature.UsrSigBBCodes).Should().Be("B");
    }

    [Test]
    [Description("The reporters of a post and the report of one user")]
    public async Task Message_Reporters_Test()
    {
        var bob = this.db.AddUser("Bob");
        var message = this.db.AddMessage(this.db.AddTopic(this.db.AddForum(), this.alice), this.alice);

        using (var connection = this.db.Open())
        {
            connection.Insert(new MessageReportedAudit { MessageID = message, UserID = bob, ReportedNumber = 1, ReportText = "spam" });
        }

        (await this.Users.MessageReportersAsync(message)).Should().ContainSingle().Which.Item2.ID.Should().Be(bob);
        (await this.Users.MessageReporterAsync(message, bob)).Should().ContainSingle().Which.Item2.ReportText.Should().Be("spam");
        (await this.Users.MessageReporterAsync(message, this.alice)).Should().BeEmpty();
    }

    private IRepository<User> Users => this.board.Current.GetRepository<User>();

    private void SetAvatar(string avatar)
    {
        this.Users.UpdateOnly(() => new User { Avatar = avatar }, u => u.ID == this.alice);
    }
}
