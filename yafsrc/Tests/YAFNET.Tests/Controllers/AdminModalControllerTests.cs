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

namespace YAF.Tests.Controllers;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Controllers.Modals;
using YAF.Core.Events;
using YAF.Core.Events.Cache;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Core.Services.CheckForSpam;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Services;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The admin dialogs (replace words, spam words, banned e-mails / names, medals, user forum access)
/// and the ignore list.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AdminModalControllerTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private ISessionService session;

    private int alice;

    private int bob;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.session = Substitute.For<ISessionService>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ServiceLocatorEventRaiser>().As<IRaiseEvent>();
                b.RegisterType<ClearCacheOnEvents>().AsImplementedInterfaces();
                b.RegisterInstance(this.session).As<ISessionService>();
                b.RegisterType<UserIgnored>().As<IUserIgnored>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum();

        this.board.LoginAs(this.alice);
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Replace words: a valid pattern is saved, an invalid regex is rejected with a message")]
    public async Task ReplaceWords_Edit_Test()
    {
        var controller = this.board.CreateController<ReplaceWordsController>();

        (await controller.Edit(new ReplaceWordsEditModal { BadWord = "darn", GoodWord = "d**n" })).Should().BeOfType<OkResult>();
        (await controller.Edit(new ReplaceWordsEditModal { BadWord = "([", GoodWord = "x" })).Should().BeOfType<OkObjectResult>();
        (await controller.Edit(new ReplaceWordsEditModal { GoodWord = "x" })).Should().BeOfType<OkObjectResult>("an empty word is left out by the form");
        (await this.board.CreateController<SpamWordsController>().Edit(new SpamWordsEditModal())).Should().BeOfType<OkObjectResult>();

        this.Repository<ReplaceWords>().Get(_ => true).Select(w => w.BadWord).Should().Equal("darn");
    }

    [Test]
    [Description("Replace words import adds new words from the xml export and skips existing ones")]
    public async Task ReplaceWords_Import_Test()
    {
        await this.board.CreateController<ReplaceWordsController>()
            .Edit(new ReplaceWordsEditModal { BadWord = "darn", GoodWord = "d**n" });

        const string Xml = """
            <YafReplaceWordsList>
              <YafReplaceWords><badword>darn</badword><goodword>d**n</goodword></YafReplaceWords>
              <YafReplaceWords><badword>heck</badword><goodword>h**k</goodword></YafReplaceWords>
            </YafReplaceWordsList>
            """;

        var result = await this.board.CreateController<ReplaceWordsController>().Import(File(Xml, "text/xml"));

        Notification(result).MessageType.Should().Be(MessageTypes.success.ToString());
        this.Repository<ReplaceWords>().Get(_ => true).Select(w => w.BadWord).Should().BeEquivalentTo(["darn", "heck"]);
    }

    [Test]
    [Description("Replace words import rejects binary files and files without the expected columns")]
    public async Task ReplaceWords_Import_Invalid_Test()
    {
        var controller = this.board.CreateController<ReplaceWordsController>();

        Notification(await controller.Import(File("x", "application/octet-stream"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await controller.Import(File("<root><other>1</other></root>", "text/xml"))).MessageType.Should().Be(MessageTypes.warning.ToString());
        Notification(await controller.Import(File("not xml", "text/plain"))).MessageType.Should().Be(MessageTypes.danger.ToString());

        this.Repository<ReplaceWords>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Spam words: a valid pattern is saved")]
    public async Task SpamWords_Edit_Test()
    {
        (await this.board.CreateController<SpamWordsController>().Edit(new SpamWordsEditModal { SpamWord = "casino" }))
            .Should().BeOfType<OkResult>();

        this.Repository<SpamWords>().Get(_ => true).Select(w => w.SpamWord).Should().Equal("casino");
    }

    [Test]
    [Description("Spam words: an invalid regex is rejected with a message (not a redirect)")]
    public async Task SpamWords_Invalid_Regex_Test()
    {
        var result = await this.board.CreateController<SpamWordsController>().Edit(new SpamWordsEditModal { SpamWord = "([" });

        Notification(result).Should().NotBeNull();
        this.Repository<SpamWords>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Banned e-mails and names: saved, duplicates rejected")]
    public void Banned_Email_Name_Test()
    {
        this.board.CreateController<BannedEmailController>().Edit(new BannedEmailEditModal { Mask = @"spam@example\.com" })
            .Should().BeOfType<OkResult>();
        this.board.CreateController<BannedEmailController>().Edit(new BannedEmailEditModal { Mask = @"spam@example\.com" })
            .Should().BeOfType<OkObjectResult>("a duplicate returns a warning");

        this.board.CreateController<BannedNameController>().Edit(new BannedEmailEditModal { Mask = "^casino", BanReason = " spam " })
            .Should().BeOfType<OkResult>();

        this.Repository<BannedEmail>().Count(_ => true).Should().Be(1);
        this.Repository<BannedName>().GetSingle(_ => true).Reason.Should().Be("spam");
    }

    [Test]
    [Description("Banned e-mails and names are regular expressions, invalid ones are rejected")]
    public void Banned_Invalid_Regex_Test()
    {
        this.board.CreateController<BannedEmailController>().Edit(new BannedEmailEditModal { Mask = "([" });
        this.board.CreateController<BannedNameController>().Edit(new BannedEmailEditModal { Mask = "([" });

        this.Repository<BannedEmail>().Count(_ => true).Should().Be(0);
        this.Repository<BannedName>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("One invalid banned e-mail mask does not disable the other e-mail bans")]
    public async Task Invalid_Email_Mask_Does_Not_Disable_Bans_Test()
    {
        using (var connection = this.db.Open())
        {
            connection.Insert(new BannedEmail { BoardID = TestDatabase.BoardId, Mask = "([", Since = DateTime.UtcNow });
            connection.Insert(new BannedEmail { BoardID = TestDatabase.BoardId, Mask = @"@spam\.example$", Since = DateTime.UtcNow });
        }

        (await new InternalCheck().IsBotAsync("198.51.100.1", "x@spam.example", "someone")).IsBot.Should().BeTrue();
    }

    [Test]
    [Description("Medals: a user id is required, a new award is saved once and the user is notified")]
    public async Task UserMedal_Award_Test()
    {
        this.board.BoardSettings.EmailUserOnMedalAward = true;
        var medal = this.AddMedal();
        var controller = this.board.CreateController<MedalController>();

        Notification(await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal })).MessageType
            .Should().Be(MessageTypes.warning.ToString());

        await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal, UserID = this.bob, MedalName = "Gold", UserMessage = "well done" });

        this.Repository<UserMedal>().GetSingle(m => m.UserID == this.bob).Message.Should().Be("well done");
        await this.board.Notifications.Received(1).ToUserWithNewMedalAsync(this.bob, "Gold");

        await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal, UserID = this.bob, UserName = "Bob", UserMessage = "updated", UserSortOrder = 3 });

        var award = this.Repository<UserMedal>().GetSingle(m => m.UserID == this.bob);
        award.Message.Should().Be("updated");
        award.SortOrder.Should().Be(3);
    }

    [Test]
    [Description("Awarding the same medal to a user twice does not fail or duplicate it")]
    public async Task UserMedal_Award_Twice_Test()
    {
        var medal = this.AddMedal();
        var controller = this.board.CreateController<MedalController>();

        await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal, UserID = this.bob, MedalName = "Gold" });

        await controller.Invoking(c => c.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal, UserID = this.bob, MedalName = "Gold" }))
            .Should().NotThrowAsync();

        this.Repository<UserMedal>().Count(m => m.UserID == this.bob).Should().Be(1);
    }

    [Test]
    [Description("Medals, users and groups of another board can not be awarded")]
    public async Task Medal_Other_Board_Test()
    {
        var medal = this.AddMedal();
        var group = this.db.AddForumAccess(this.bob, this.forum);
        var otherUser = this.db.AddUser("Other", u => u.BoardID = 2);

        int otherMedal;

        using (var connection = this.db.Open())
        {
            otherMedal = (int)connection.Insert(
                new Medal { BoardID = 2, Name = "Other", Message = "m", Category = "c", MedalURL = "other.png", Flags = 0 },
                selectIdentity: true);
        }

        var controller = this.board.CreateController<MedalController>();

        (await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = otherMedal, UserID = this.bob, MedalName = "Other" }))
            .Should().BeOfType<BadRequestResult>();
        (await controller.EditUserMedalAsync(new UserMedalEditModal { MedalId = medal, UserID = otherUser, MedalName = "Gold" }))
            .Should().BeOfType<BadRequestResult>();
        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, MedalId = otherMedal })
            .Should().BeOfType<BadRequestResult>();
        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = 99999, MedalId = medal })
            .Should().BeOfType<BadRequestResult>();

        this.Repository<UserMedal>().Count(_ => true).Should().Be(0);
        this.Repository<GroupMedal>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Group medals: award and update")]
    public void GroupMedal_Test()
    {
        var medal = this.AddMedal();
        var group = this.db.AddForumAccess(this.bob, this.forum);
        var controller = this.board.CreateController<MedalController>();

        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, MedalId = medal, GroupMessage = "team" });
        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, GroupName = "Team", MedalId = medal, GroupMessage = "team 2", GroupHide = true });

        var award = this.Repository<GroupMedal>().GetSingle(m => m.GroupID == group);
        award.Message.Should().Be("team 2");
        award.Hide.Should().BeTrue();
    }

    [Test]
    [Description("Awarding a medal to a group drops the cached medals of the users (the members of the group)")]
    public void GroupMedal_Clears_User_Medals_Cache_Test()
    {
        var medal = this.AddMedal();
        var group = this.db.AddForumAccess(this.bob, this.forum);
        var controller = this.board.CreateController<MedalController>();

        var key = string.Format(YAF.Types.Constants.Constants.Cache.UserMedals, this.bob);
        this.board.Cache.Set(key, "cached");

        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, MedalId = medal, GroupMessage = "team" });

        this.board.Cache.GetAll<object>().Select(e => e.Key).Should().NotContain(key);
    }

    [Test]
    [Description("Awarding a medal to a group again updates the award, sort orders out of range are limited to 0..255")]
    public void GroupMedal_Award_Again_Test()
    {
        var medal = this.AddMedal();
        var group = this.db.AddForumAccess(this.bob, this.forum);
        var controller = this.board.CreateController<MedalController>();

        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, MedalId = medal, GroupMessage = "team", GroupSortOrder = 300 })
            .Should().BeOfType<OkResult>();
        controller.EditGroupMedal(new GroupMedalEditModal { GroupId = group, MedalId = medal, GroupMessage = "team 2", GroupSortOrder = -1 })
            .Should().BeOfType<OkResult>();

        var award = this.Repository<GroupMedal>().GetSingle(m => m.GroupID == group);
        award.Message.Should().Be("team 2");
        award.SortOrder.Should().Be(0);
        this.Repository<GroupMedal>().Count(_ => true).Should().Be(1);
    }

    [Test]
    [Description("User forum access: a user id is required, the access mask is saved and changed")]
    public void ModForumUser_Test()
    {
        var first = this.AddMask(TestDatabase.BoardId);
        var second = this.AddMask(TestDatabase.BoardId);
        var controller = this.board.CreateController<ModForumUserController>();

        Notification(controller.EditModForumUser(new ModForumUserModal { ForumId = this.forum, AccessMaskID = 1 })).MessageType
            .Should().Be(MessageTypes.warning.ToString());

        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = this.forum, AccessMaskID = first });
        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = this.forum, AccessMaskID = second });

        this.Repository<UserForum>().GetSingle(u => u.UserID == this.bob).AccessMaskID.Should().Be(second);
    }

    [Test]
    [Description("User forum access can not use users, forums or access masks of another board")]
    public void ModForumUser_Other_Board_Test()
    {
        var mask = this.AddMask(TestDatabase.BoardId);
        var otherMask = this.AddMask(2);
        var otherUser = this.db.AddUser("Other", u => u.BoardID = 2);
        var controller = this.board.CreateController<ModForumUserController>();

        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = this.forum, AccessMaskID = otherMask })
            .Should().BeOfType<BadRequestResult>();
        controller.EditModForumUser(new ModForumUserModal { UserID = otherUser, ForumId = this.forum, AccessMaskID = mask })
            .Should().BeOfType<BadRequestResult>();
        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = 99999, AccessMaskID = mask })
            .Should().BeOfType<BadRequestResult>();

        this.Repository<UserForum>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Changing a user's access in a forum drops the cached access rights, so it applies at once")]
    public void ModForumUser_Clears_Access_Test()
    {
        this.board.GrantAccess(this.bob, this.forum);
        var first = this.AddMask(TestDatabase.BoardId);
        var second = this.AddMask(TestDatabase.BoardId);
        var controller = this.board.CreateController<ModForumUserController>();

        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = this.forum, AccessMaskID = first });
        this.ActiveAccessCount(this.bob).Should().Be(0, "a new user access clears the cache");

        this.board.RefreshActiveAccess(this.bob);
        controller.EditModForumUser(new ModForumUserModal { UserID = this.bob, ForumId = this.forum, AccessMaskID = second });
        this.ActiveAccessCount(this.bob).Should().Be(0, "a changed user access clears the cache");
    }

    [Test]
    [Description("Ignoring and un-ignoring the author of a shown post")]
    public async Task IgnoredUser_Test()
    {
        var post = this.ShowPost(this.bob);
        var controller = this.board.CreateController<IgnoredUserController>();

        await controller.AddIgnoredUser(post);
        this.Repository<IgnoreUser>().Count(i => i.UserID == this.alice && i.IgnoredUserID == this.bob).Should().Be(1);

        await controller.RemoveIgnoredUser(post);
        this.Repository<IgnoreUser>().Count(_ => true).Should().Be(0);

        await controller.AddIgnoredUser(99999);
        this.Repository<IgnoreUser>().Count(_ => true).Should().Be(0, "only shown posts");
    }

    [Test]
    [Description("Users can not ignore themselves")]
    public async Task IgnoredUser_Self_Test()
    {
        var post = this.ShowPost(this.alice);

        await this.board.CreateController<IgnoredUserController>().AddIgnoredUser(post);

        this.Repository<IgnoreUser>().Count(_ => true).Should().Be(0);
    }

    private static MessageModalNotification Notification(IActionResult result)
    {
        return (result as OkObjectResult)?.Value as MessageModalNotification;
    }

    private static FormFile File(string content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content);

        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "import.xml")
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };
    }

    private int ShowPost(int authorId)
    {
        var post = this.db.AddMessage(this.db.AddTopic(this.forum, authorId), authorId);

        this.session.GetPageData<List<PagedMessage>>().Returns(
            [new PagedMessage { MessageID = post, UserID = authorId, Topic = "Topic" }]);

        return post;
    }

    private int AddMask(int boardId)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new AccessMask { BoardID = boardId, Name = "Mask", Flags = 1, SortOrder = 1 },
            selectIdentity: true);
    }

    private int AddMedal()
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new Medal { BoardID = TestDatabase.BoardId, Name = "Gold", Message = "m", Category = "c", MedalURL = "gold.png", Flags = 0 },
            selectIdentity: true);
    }

    private long ActiveAccessCount(int userId)
    {
        return this.Repository<ActiveAccess>().Count(a => a.UserID == userId);
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
