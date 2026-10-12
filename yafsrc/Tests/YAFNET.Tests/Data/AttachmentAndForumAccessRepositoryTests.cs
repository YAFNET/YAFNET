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

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The attachment repository (admin list, delete with the upload file, download counter) and the
/// forum access lists of the admin forum / group pages.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AttachmentAndForumAccessRepositoryTests
{
    private string webRoot;

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-attach-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "uploads"));

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterInstance(new BoardConfiguration { StaticFilesRoot = string.Empty });
                b.RegisterType<BoardFolders>().AsSelf();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.webRoot, true);
    }

    [Test]
    [Description("The admin list pages the attachments of the board, newest first, with their user")]
    public void Get_By_Board_Paged_Test()
    {
        var other = this.db.AddUser("Other board", u => u.BoardID = 2);

        var ids = Enumerable.Range(1, 5).Select(i => this.AddAttachment(i % 2 == 0 ? this.alice : this.bob, $"f{i}.txt")).ToList();
        this.AddAttachment(other, "other.txt");

        var first = this.Attachments.GetByBoardPaged(out var count, TestDatabase.BoardId, 0, 2);
        var last = this.Attachments.GetByBoardPaged(out _, TestDatabase.BoardId, 2, 2);

        count.Should().Be(5);
        first.Select(t => t.Item2.ID).Should().Equal(ids[4], ids[3]);
        first[1].Item1.Name.Should().Be("Alice");
        last.Select(t => t.Item2.ID).Should().Equal(ids[0]);
    }

    [Test]
    [Description("Deleting an attachment removes the upload file (old and new file names) and the row")]
    public void Delete_Test()
    {
        var topic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var message = this.db.AddMessage(topic, this.alice);

        var posted = this.AddAttachment(this.alice, "posted.txt", message);
        var unattached = this.AddAttachment(this.bob, "draft.txt");
        var missing = this.AddAttachment(this.bob, "missing.txt");

        var postedFile = this.UploadFile($"{message}.posted.txt.yafupload");
        var unattachedFile = this.UploadFile($"u{this.bob}-{unattached}.draft.txt.yafupload");

        this.Attachments.Delete(posted);
        this.Attachments.Delete(unattached);
        this.Attachments.Delete(missing);
        this.Attachments.Delete(9999);

        File.Exists(postedFile).Should().BeFalse();
        File.Exists(unattachedFile).Should().BeFalse();
        this.Attachments.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Downloads are counted per attachment, messages with attachments are listed")]
    public async Task Download_Counter_And_Message_List_Test()
    {
        var topic = this.db.AddTopic(this.db.AddForum(), this.alice);
        var withFiles = this.db.AddMessage(topic, this.alice);
        this.db.AddMessage(topic, this.alice);

        var first = this.AddAttachment(this.alice, "a.txt", withFiles);
        this.AddAttachment(this.alice, "b.txt", withFiles);

        await this.Attachments.IncrementDownloadCounterAsync(first);
        await this.Attachments.IncrementDownloadCounterAsync(first);

        this.Attachments.GetById(first).Downloads.Should().Be(2);
        this.Attachments.GetMessageAttachments().Select(m => m.ID).Distinct().Should().Equal(withFiles);
    }

    [Test]
    [Description("The forum access list of a forum contains every group with its group name and access mask")]
    public void Forum_Access_List_Test()
    {
        var forum = this.db.AddForum("General");
        var members = this.db.AddForumAccess(this.alice, forum);
        var moderators = this.db.AddForumAccess(this.bob, forum);
        this.db.AddForumAccess(this.bob, this.db.AddForum("Other"));

        var list = this.board.Current.GetRepository<ForumAccess>().GetForumAccessList(forum);

        list.Select(t => t.Item2.ID).Should().BeEquivalentTo([members, moderators]);
        list.Select(t => t.Item2.Name).Should().OnlyContain(n => n.StartsWith("Group "));
        list.Should().OnlyContain(t => t.Item1.ForumID == forum && t.Item1.AccessMaskID > 0);
    }

    [Test]
    [Description("The forums of a group are listed with their access mask, the mask can be changed")]
    public async Task List_By_Groups_And_Save_Test()
    {
        var category = this.db.AddCategory();
        var first = this.db.AddForum("First", category, f => f.SortOrder = 1);
        var second = this.db.AddForum("Second", category, f => f.SortOrder = 2);

        var group = this.db.AddForumAccess(this.alice, first);
        var forumAccess = this.board.Current.GetRepository<ForumAccess>();
        var mask = forumAccess.GetSingle(a => a.GroupID == group).AccessMaskID;
        forumAccess.Create(second, group, mask);

        var list = forumAccess.ListByGroups(group);

        list.Select(l => l.ForumName).Should().Equal("First", "Second");
        list.Select(l => l.ForumID).Should().Equal(first, second);
        list.Should().OnlyContain(l => l.AccessMaskID == mask);

        var otherMask = this.board.Current.GetRepository<AccessMask>()
            .Insert(new AccessMask { BoardID = TestDatabase.BoardId, Name = "Read only", Flags = 1 }, true);

        await forumAccess.SaveAsync(second, group, (int)otherMask);

        forumAccess.ListByGroups(group).Single(l => l.ForumID == second).AccessMaskID.Should().Be((int)otherMask);
    }

    private IRepository<Attachment> Attachments => this.board.Current.GetRepository<Attachment>();

    private int AddAttachment(int userId, string fileName, int messageId = 0)
    {
        return (int)this.Attachments.Insert(
            new Attachment { UserID = userId, MessageID = messageId, FileName = fileName, ContentType = "text/plain", Bytes = 1 },
            true);
    }

    private string UploadFile(string name)
    {
        var path = Path.Combine(this.webRoot, "uploads", name);
        File.WriteAllText(path, "x");

        return path;
    }
}
