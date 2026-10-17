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

using System.Threading.Tasks;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// The watched forums of the users (e-mail notification of new topics).
/// </summary>
[TestFixture]
[NonParallelizable]
public class WatchForumRepositoryTests
{
    private TestBoardContext board;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
        this.bob = this.board.Database.AddUser("Bob");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A watched forum is found for its user only")]
    public void Add_And_Check_Test()
    {
        var forum = this.board.Database.AddForum();

        (this.Repository.Check(this.alice, forum)).Should().BeNull();

        this.Repository.Add(this.alice, forum);

        var watch = this.Repository.Check(this.alice, forum);
        watch.Should().NotBeNull();
        (this.Repository.GetById(watch.Value)).Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

        (this.Repository.Check(this.bob, forum)).Should().BeNull();
    }

    [Test]
    [Description("The list has the watched forums of the user with the forum, newest first, paged with a zero based page index")]
    public void List_Test()
    {
        var forums = Enumerable.Range(0, 3).Select(_ => this.board.Database.AddForum()).ToList();

        foreach (var forum in forums)
        {
            this.Repository.Add(this.alice, forum);
        }

        this.Repository.Add(this.bob, forums[0]);

        var list = this.Repository.List(this.alice);

        list.Select(w => w.ForumID).Should().Equal(forums[2], forums[1], forums[0]);
        list.Should().OnlyContain(w => w.Forum != null && w.Forum.ID == w.ForumID);

        (this.Repository.List(this.alice, 0, 2)).Select(w => w.ForumID).Should().Equal(forums[2], forums[1]);
        (this.Repository.List(this.alice, 1, 2)).Select(w => w.ForumID).Should().Equal(forums[0]);
    }

    [Test]
    [Description("Watched forums that do not exist anymore are not listed")]
    public void List_Without_Deleted_Forum_Test()
    {
        var kept = this.board.Database.AddForum();
        var deleted = this.board.Database.AddForum();

        this.Repository.Add(this.alice, kept);
        this.Repository.Add(this.alice, deleted);

        using (var db = this.board.Database.Open())
        {
            db.DeleteById<Forum>(deleted);
        }

        (this.Repository.List(this.alice)).Select(w => w.ForumID).Should().Equal(kept);
    }

    private IRepository<WatchForum> Repository => this.board.Current.GetRepository<WatchForum>();
}
