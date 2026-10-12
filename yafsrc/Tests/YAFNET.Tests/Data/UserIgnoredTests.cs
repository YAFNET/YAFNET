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

using Microsoft.AspNetCore.Http;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;

/// <summary>
/// UserIgnored service and IgnoreUser repository tests against the in-memory database.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserIgnoredTests
{
    private TestBoardContext board;

    private int alice;

    private int bob;

    private int carol;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);

        this.alice = this.board.Database.AddUser("Alice");
        this.bob = this.board.Database.AddUser("Bob");
        this.carol = this.board.Database.AddUser("Carol");

        // Alice is the current user
        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Ignoring a user is persisted and reported by IsIgnored")]
    public async Task AddIgnored_Test()
    {
        await this.CreateService().AddIgnoredAsync(this.bob);

        // a fresh service instance has no in-memory list
        var service = this.CreateService();

        (await service.IsIgnoredAsync(this.bob)).Should().BeTrue();
        (await service.IsIgnoredAsync(this.carol)).Should().BeFalse();
    }

    [Test]
    [Description("Ignoring the same user twice stores a single row")]
    public async Task AddIgnored_Twice_Test()
    {
        var service = this.CreateService();

        await service.AddIgnoredAsync(this.bob);
        await service.AddIgnoredAsync(this.bob);

        this.board.Current.GetRepository<IgnoreUser>().Count(x => x.UserID == this.alice).Should().Be(1);
    }

    [Test]
    [Description("RemoveIgnored deletes the row and clears the session cache")]
    public async Task RemoveIgnored_Test()
    {
        var service = this.CreateService();

        await service.AddIgnoredAsync(this.bob);
        (await service.UserIgnoredListAsync(this.alice)).Should().Equal(this.bob);

        await service.RemoveIgnoredAsync(this.bob);

        (await service.UserIgnoredListAsync(this.alice)).Should().BeEmpty();
        this.board.Current.GetRepository<IgnoreUser>().Count(x => x.UserID == this.alice).Should().Be(0);
    }

    [Test]
    [Description("The ignore list is cached in the session until it is cleared")]
    public async Task UserIgnoredList_Session_Cache_Test()
    {
        var service = this.CreateService();

        (await service.UserIgnoredListAsync(this.alice)).Should().BeEmpty();

        // bypass the service, the cached (empty) list is still returned
        await this.board.Current.GetRepository<IgnoreUser>().AddIgnoredUserAsync(this.alice, this.carol);

        (await service.UserIgnoredListAsync(this.alice)).Should().BeEmpty();

        service.ClearIgnoreCache();

        (await service.UserIgnoredListAsync(this.alice)).Should().Equal(this.carol);
    }

    [Test]
    [Description("Ignore lists are per user")]
    public async Task Ignore_Lists_Are_Per_User_Test()
    {
        var repository = this.board.Current.GetRepository<IgnoreUser>();

        await repository.AddIgnoredUserAsync(this.alice, this.bob);
        await repository.AddIgnoredUserAsync(this.carol, this.alice);

        (await this.CreateService().UserIgnoredListAsync(this.carol)).Should().Equal(this.alice);
        (await this.CreateService().UserIgnoredListAsync(this.bob)).Should().BeEmpty();
    }

    [Test]
    [Description("IgnoredUsersAsync joins the ignored users")]
    public async Task IgnoredUsers_Join_Test()
    {
        var repository = this.board.Current.GetRepository<IgnoreUser>();

        await repository.AddIgnoredUserAsync(this.alice, this.bob);
        await repository.AddIgnoredUserAsync(this.alice, this.carol);

        var users = await repository.IgnoredUsersAsync(this.alice);

        users.Select(u => u.Name).Should().BeEquivalentTo("Bob", "Carol");
    }

    [Test]
    [Description("DeleteAsync reports whether a row was removed")]
    public async Task Repository_DeleteAsync_Test()
    {
        var repository = this.board.Current.GetRepository<IgnoreUser>();

        await repository.AddIgnoredUserAsync(this.alice, this.bob);

        (await repository.DeleteAsync(this.alice, this.bob)).Should().BeTrue();
        (await repository.DeleteAsync(this.alice, this.bob)).Should().BeFalse();
    }

    private UserIgnored CreateService()
    {
        return new UserIgnored(this.board.Resolve<IHttpContextAccessor>(), this.board.Resolve<IServiceLocator>());
    }
}
