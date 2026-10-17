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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Services;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

using YAF.Core.Identity;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models.Identity;

/// <summary>
/// The OrmLite backed ASP.NET Identity <see cref="UserStore"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class UserStoreTests
{
    private TestBoardContext board;

    private UserStore store;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(useDatabase: true);
        this.store = new UserStore(this.board.Resolve<IServiceLocator>());
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Claims keep their type and can be removed again")]
    public async Task Claims_Test()
    {
        var user = new AspNetUsers
                       {
                           Id = "u1",
                           UserName = "Alice",
                           LoweredUserName = "alice",
                           Email = "alice@example.com",
                           LoweredEmail = "alice@example.com",
                           ApplicationId = Guid.NewGuid(),
                           IsApproved = true
                       };

        await this.store.CreateAsync(user);

        var claim = new Claim("forum:role", "moderator");

        await this.store.AddClaimAsync(user, claim);

        var stored = (await this.store.GetClaimsAsync(user)).Should().ContainSingle().Subject;
        stored.Type.Should().Be("forum:role");
        stored.Value.Should().Be("moderator");

        await this.store.RemoveClaimAsync(user, claim);

        (await this.store.GetClaimsAsync(user)).Should().BeEmpty();
    }
}
