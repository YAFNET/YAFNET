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

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;

/// <summary>
/// Saving with the id of a record of another board does not change that record
/// (the ids of the admin pages are sent by the client).
/// </summary>
[TestFixture]
[NonParallelizable]
public class BoardIsolationTests
{
    private const int OtherBoard = 2;

    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Spam words and replace words of another board are not changed or moved")]
    public async Task Words_Test()
    {
        var spamWord = this.Insert(new SpamWords { BoardID = OtherBoard, SpamWord = "other" });
        await this.Repository<SpamWords>().SaveAsync(spamWord, "changed");
        this.Repository<SpamWords>().GetById(spamWord).Should().BeEquivalentTo(new { BoardID = OtherBoard, SpamWord = "other" });

        await this.Repository<SpamWords>().SaveAsync(null, "new");
        this.Repository<SpamWords>().GetSingle(x => x.SpamWord == "new").BoardID.Should().Be(TestDatabase.BoardId);

        var replaceWord = this.Insert(new ReplaceWords { BoardID = OtherBoard, BadWord = "bad", GoodWord = "good" });
        await this.Repository<ReplaceWords>().SaveAsync(replaceWord, "changed", "changed");
        this.Repository<ReplaceWords>().GetById(replaceWord).BadWord.Should().Be("bad");

        var own = this.Insert(new ReplaceWords { BoardID = TestDatabase.BoardId, BadWord = "bad", GoodWord = "good" });
        await this.Repository<ReplaceWords>().SaveAsync(own, "worse", "better");
        this.Repository<ReplaceWords>().GetById(own).BadWord.Should().Be("worse", "the own board can be changed");
    }

    [Test]
    [Description("Access masks, bb codes and categories of another board are not changed")]
    public async Task Masks_BBCodes_Categories_Test()
    {
        var mask = this.Insert(new AccessMask { BoardID = OtherBoard, Name = "Other", Flags = 1, SortOrder = 1 });
        await this.Repository<AccessMask>().SaveAsync(mask, "Changed", new AccessFlags(), 1);
        this.Repository<AccessMask>().GetById(mask).Should().BeEquivalentTo(new { BoardID = OtherBoard, Name = "Other" });

        var code = this.Insert(new BBCode { BoardID = OtherBoard, Name = "OTHER", ExecOrder = 1 });
        await this.Repository<BBCode>().SaveAsync(code, "CHANGED", null, null, null, null, null, null, null, null, false, false, null, 1);
        this.Repository<BBCode>().GetById(code).Name.Should().Be("OTHER");

        var category = this.Insert(new Category { BoardID = OtherBoard, Name = "Other", SortOrder = 1, Flags = 1 });
        (await this.Repository<Category>().SaveAsync(category, "Changed", null, 1, new CategoryFlags())).Should().Be(0);
        this.Repository<Category>().GetById(category).Should().BeEquivalentTo(new { BoardID = OtherBoard, Name = "Other" });
    }

    [Test]
    [Description("Ranks, groups and medals of another board are not changed")]
    public async Task Ranks_Groups_Medals_Test()
    {
        var rank = this.Insert(new Rank { BoardID = OtherBoard, Name = "Other", Flags = 0, SortOrder = 1 });
        await this.Repository<Rank>().SaveAsync(rank, TestDatabase.BoardId, "Changed", new RankFlags(), null, null, 1, null, 0, null, 0, 0);
        this.Repository<Rank>().GetById(rank).Name.Should().Be("Other");

        var group = this.Insert(new Group { BoardID = OtherBoard, Name = "Other", Flags = 0, SortOrder = 1 });
        await this.Repository<Group>().SaveAsync(group, TestDatabase.BoardId, "Changed", new GroupFlags(), 1, null, 1, null, 0, null, 0, 0);
        this.Repository<Group>().GetById(group).Name.Should().Be("Other");

        var medal = this.Insert(new Medal { BoardID = OtherBoard, Name = "Other", Message = "m", MedalURL = "other.png", Flags = 0 });
        this.Repository<Medal>().Save(medal, "Changed", null, "m", null, "other.png", 0);
        this.Repository<Medal>().GetById(medal).Should().BeEquivalentTo(new { BoardID = OtherBoard, Name = "Other" });
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }

    private int Insert<T>(T entity)
        where T : class, IEntity, new()
    {
        return this.Repository<T>().Insert(entity, true);
    }
}
