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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The admin forum access mask selection of the edit group page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AccessMaskControllerTests
{
    private TestBoardContext board;

    private int forum;

    private int group;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        var admin = this.board.Database.AddUser("Admin");
        this.board.LoginAs(admin);
        this.board.PageLoad.IsAdmin = true;

        this.forum = this.board.Database.AddForum("Forum");
        this.group = this.board.Database.AddForumAccess(admin, this.forum);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("An admin changes the access mask of a group of the board")]
    public async Task Set_Group_Mask_Test()
    {
        var mask = this.AddMask(TestDatabase.BoardId);

        (await this.SetAsync(mask)).Should().BeOfType<OkResult>();

        this.Access.AccessMaskID.Should().Be(mask);
    }

    [Test]
    [Description("Groups or access masks of another board can not be used")]
    public async Task Other_Board_Test()
    {
        var before = this.Access.AccessMaskID;

        (await this.SetAsync(this.AddMask(2))).Should().BeOfType<BadRequestResult>();
        (await this.SetAsync(this.AddMask(TestDatabase.BoardId), 99999)).Should().BeOfType<BadRequestResult>();

        // a forum of another board
        var otherForum = this.board.Database.AddForum("Other", this.board.Database.AddCategory("Other", c => c.BoardID = 2));
        (await this.SetAsync(this.AddMask(TestDatabase.BoardId), forum: otherForum)).Should().BeOfType<BadRequestResult>();
        this.board.Current.GetRepository<ForumAccess>().Exists(a => a.ForumID == otherForum).Should().BeFalse();

        this.Access.AccessMaskID.Should().Be(before);
    }

    private ForumAccess Access =>
        this.board.Current.GetRepository<ForumAccess>().GetSingle(a => a.ForumID == this.forum && a.GroupID == this.group);

    private int AddMask(int boardId)
    {
        return (int)this.board.Current.GetRepository<AccessMask>().Insert(
            new AccessMask { BoardID = boardId, Name = $"Mask {boardId}", Flags = 1, SortOrder = 1 },
            true);
    }

    private Task<IActionResult> SetAsync(int mask, int? group = null, int? forum = null)
    {
        return this.board.CreateController<AccessMaskController>().SetGroupMask(
            new PagedResults { UserId = forum ?? this.forum, PageSize = mask, PageNumber = group ?? this.group });
    }
}
