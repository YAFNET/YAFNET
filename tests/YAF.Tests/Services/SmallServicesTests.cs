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

using System.Threading;

using Autofac;

using YAF.Core.Context;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Small services: the style of user names, the page timer and the lazy user data of the page.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SmallServicesTests
{
    private TestBoardContext board;

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [TestCase(null, "")]
    [TestCase("", "")]
    [TestCase("color: red", "color: red")]
    [TestCase("Admins!color: red;font-weight: bold", "color: red")]
    [TestCase("Admins!font-weight: bold;color: red/Members!color: blue", "color: blue")]
    [TestCase(" Admins!COLOR: Red ", "COLOR: Red")]
    [Description("Only the color of the group or rank style is used, the last style wins")]
    public void Style_Transform_Test(string style, string expected)
    {
        new StyleTransform().Decode(style).Should().Be(expected);
    }

    [Test]
    [Description("The page timer measures the time in seconds and can be reset")]
    public void Stop_Watch_Test()
    {
        var watch = new StopWatch();

        watch.Start();
        Thread.Sleep(20);
        watch.Stop();

        watch.Duration.Should().BeGreaterThan(0).And.BeLessThan(10);
        watch.Watch.IsRunning.Should().BeFalse();

        watch.Reset();
        watch.Duration.Should().Be(0);
    }

    [Test]
    [Description("The page gets the lazy data of the user (e.g. the counts of the menu)")]
    public void Lazy_User_Data_Test()
    {
        this.board = new TestBoardContext(b => b.RegisterType<DataBroker>().AsSelf(), useDatabase: true);

        var alice = this.board.Database.AddUser("Alice");

        var handler = new LoadPageLazyUserData(this.board.Resolve<IServiceLocator>(), this.board.Resolve<DataBroker>());

        var @event = new InitPageLoadEvent
        {
            PageLoadData = Tuple.Create(
                new PageLoad { UserID = alice },
                new User(),
                new Category(),
                new Forum(),
                new Topic(),
                new Message())
        };

        handler.Handle(@event);

        @event.UserLazyData.Should().NotBeNull();
        handler.Order.Should().Be(3000);
    }
}
