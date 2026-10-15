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

using Autofac;

using NSubstitute;

using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;
using YAF.Types.Objects.Model;

/// <summary>
/// The board context loads the data of the page (user, forum, topic) once, on the first use.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BoardContextInitTests
{
    private TestBoardContext board;

    private IAspNetUsersHelper aspNetUsers;

    [SetUp]
    public void SetUp()
    {
        this.aspNetUsers = Substitute.For<IAspNetUsersHelper>();

        this.board = new TestBoardContext(b => b.RegisterInstance(this.aspNetUsers).As<IAspNetUsersHelper>());
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The page data is loaded by the page load event on the first use, the user of the request is set")]
    public void Page_Data_Is_Loaded_Once_Test()
    {
        var identity = new AspNetUsers { Id = "alice-key", UserName = "alice" };
        this.aspNetUsers.GetUser().Returns(identity);

        var pageUser = new User { ID = 7, Name = "alice" };
        var loads = 0;

        this.board.Events.OnRaise = e =>
        {
            if (e is not InitPageLoadEvent pageLoad)
            {
                return;
            }

            loads++;

            pageLoad.UserRequestData = new UserRequestData();
            pageLoad.PageLoadData = Tuple.Create(
                new PageLoad { UserID = 7 },
                pageUser,
                new Category(),
                new Forum(),
                new Topic(),
                new Message());
            pageLoad.UserLazyData = new UserLazyData();
            pageLoad.PageQueryData = new PageQueryData();
        };

        var context = this.board.Current;
        var before = 0;
        var after = 0;
        context.BeforeInit += (_, _) => before++;
        context.AfterInit += (_, _) => after++;

        context.PageData = null;
        context.PageIsNull().Should().BeFalse("the page data is loaded when it is used");

        context.PageUser.Should().BeSameAs(pageUser);
        context.PageUserID.Should().Be(7);
        context.MembershipUser.Should().BeSameAs(identity);
        context.PageLinks.Should().BeEmpty();

        loads.Should().Be(1, "the page data is loaded once");
        (before, after).Should().Be((1, 1));
    }

    [Test]
    [Description("Disposing the board context tells the listeners that the request ended")]
    public void Unload_Test()
    {
        var unloaded = 0;
        this.board.Current.Unload += (_, _) => unloaded++;

        this.board.Current.Dispose();

        unloaded.Should().Be(1);
    }
}
