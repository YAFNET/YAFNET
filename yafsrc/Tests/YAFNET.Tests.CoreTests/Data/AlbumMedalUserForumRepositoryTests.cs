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

namespace YAF.Tests.CoreTests.Data;

using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// User albums and album images, the moderator list of user forum access and the medal holder lists.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AlbumMedalUserForumRepositoryTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Albums are listed per user, newest first, and their title and cover can be changed")]
    public async Task Albums_Test()
    {
        var albums = this.Repository<UserAlbum>();

        var first = albums.Save(this.alice, "First", null);
        var second = albums.Save(this.alice, "Second", null);
        albums.Save(this.bob, "Bob's", null);
        albums.UpdateOnly(() => new UserAlbum { Updated = DateTime.UtcNow.AddDays(-1) }, a => a.ID == first);

        albums.ListByUser(this.alice).Select(a => a.Title).Should().Equal("Second", "First");
        albums.ListByUserPaged(this.alice, 0, 1).Should().ContainSingle();
        albums.CountUserAlbum(this.alice).Should().Be(2);
        albums.GetTitle(second).Should().Be("Second");

        albums.UpdateTitle(second, "Renamed");
        albums.GetTitle(second).Should().Be("Renamed");

        var images = this.Repository<UserAlbumImage>();
        images.Save(null, second, "caption", "cover.png", 3, "image/png");
        var cover = images.List(second).Single();

        albums.UpdateCover(second, cover.ID);
        albums.GetById(second).CoverImageID.Should().Be(cover.ID);

        await albums.DeleteCoverAsync(cover.ID);
        albums.GetById(second).CoverImageID.Should().BeNull();
    }

    [Test]
    [Description("Album images are listed newest first, counted and paged")]
    public void Album_Images_Test()
    {
        var album = this.Repository<UserAlbum>().Save(this.alice, "Album", null);
        var images = this.Repository<UserAlbumImage>();

        images.Save(null, album, null, "old.png", 1, "image/png");
        images.Save(null, album, "new", "new.png", 2, "image/png");
        images.UpdateOnly(() => new UserAlbumImage { Uploaded = DateTime.UtcNow.AddDays(-3) }, i => i.FileName == "old.png");

        images.List(album).Select(i => i.FileName).Should().Equal("new.png", "old.png");
        images.CountAlbumImages(album).Should().Be(2);
        images.ListPaged(album, 0, 1).Should().ContainSingle();
        images.ListPaged(album, 1, 1).Should().ContainSingle();

        var saved = images.GetSingle(i => i.FileName == "new.png");
        saved.Caption.Should().Be("new");
        saved.Bytes.Should().Be(2);
        saved.Downloads.Should().Be(0);
    }

    [Test]
    [Description("The users with own access to a forum are listed with their access mask and can be removed")]
    public void User_Forum_Test()
    {
        var forum = this.db.AddForum("Forum");
        var other = this.db.AddForum("Other");
        var mask = (int)this.Repository<AccessMask>().Insert(
            new AccessMask { BoardID = TestDatabase.BoardId, Name = "Moderator", Flags = 64, SortOrder = 1 },
            true);

        var userForums = this.Repository<UserForum>();
        userForums.Insert(new UserForum { UserID = this.alice, ForumID = forum, AccessMaskID = mask, Invited = DateTime.UtcNow, Accepted = true });
        userForums.Insert(new UserForum { UserID = this.bob, ForumID = forum, AccessMaskID = mask, Invited = DateTime.UtcNow, Accepted = true });
        userForums.Insert(new UserForum { UserID = this.bob, ForumID = other, AccessMaskID = mask, Invited = DateTime.UtcNow, Accepted = true });

        var all = userForums.List(null, forum);
        all.Select(t => t.Item1.Name).Should().BeEquivalentTo(["Alice", "Bob"]);
        all.Should().OnlyContain(t => t.Item2.ForumID == forum && t.Item3.Name == "Moderator");

        userForums.List(this.bob, forum).Single().Item1.ID.Should().Be(this.bob);

        userForums.Delete(this.bob, forum).Should().BeTrue();
        userForums.Delete(this.bob, forum).Should().BeFalse("it was removed already");
        userForums.List(null, forum).Select(t => t.Item1.Name).Should().Equal("Alice");
        userForums.List(null, other).Should().ContainSingle("other forums are not touched");
    }

    [Test]
    [Description("The holders of a medal (users and groups) are listed by name, or a single holder")]
    public void Medal_Holders_Test()
    {
        var medal = (int)this.Repository<Medal>().Insert(
            new Medal { BoardID = TestDatabase.BoardId, Name = "Hero", Message = "m", MedalURL = "hero.png", Flags = 0 },
            true);
        var otherMedal = (int)this.Repository<Medal>().Insert(
            new Medal { BoardID = TestDatabase.BoardId, Name = "Other", Message = "m", MedalURL = "other.png", Flags = 0 },
            true);

        var userMedals = this.Repository<UserMedal>();
        userMedals.Insert(new UserMedal { UserID = this.bob, MedalID = medal, DateAwarded = DateTime.UtcNow, SortOrder = 1 });
        userMedals.Insert(new UserMedal { UserID = this.alice, MedalID = medal, DateAwarded = DateTime.UtcNow, SortOrder = 2, Message = "well done" });
        userMedals.Insert(new UserMedal { UserID = this.alice, MedalID = otherMedal, DateAwarded = DateTime.UtcNow, SortOrder = 1 });

        userMedals.List(null, medal).Select(t => t.Item3.Name).Should().Equal("Alice", "Bob");
        var single = userMedals.List(this.alice, medal).Single();
        single.Item2.Message.Should().Be("well done");
        single.Item1.Name.Should().Be("Hero");

        var groupA = this.db.AddForumAccess(this.alice, this.db.AddForum());
        var groupB = this.db.AddForumAccess(this.bob, this.db.AddForum());
        this.Repository<Group>().UpdateOnly(() => new Group { Name = "Zeta" }, g => g.ID == groupA);
        this.Repository<Group>().UpdateOnly(() => new Group { Name = "Alpha" }, g => g.ID == groupB);

        var groupMedals = this.Repository<GroupMedal>();
        groupMedals.Insert(new GroupMedal { GroupID = groupA, MedalID = medal, SortOrder = 1 });
        groupMedals.Insert(new GroupMedal { GroupID = groupB, MedalID = medal, SortOrder = 1 });

        groupMedals.List(null, medal).Select(t => t.Item3.Name).Should().Equal("Alpha", "Zeta");
        groupMedals.List(groupA, medal).Single().Item3.ID.Should().Be(groupA);
        groupMedals.List(groupA, otherMedal).Should().BeEmpty();
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
