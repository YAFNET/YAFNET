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

namespace YAF.Tests.CoreTests.TestInfrastructure;

using Autofac;

using YAF.Configuration;
using YAF.Core;
using YAF.Core.Context;
using YAF.Types;
using YAF.Types.Interfaces;

/// <summary>
/// Makes sure the test harness itself wires up and isolates the board context.
/// </summary>
[TestFixture]
[NonParallelizable]
public class TestBoardContextTests
{
    [Test]
    [Description("BoardContext.Current resolves services from the test container")]
    public void Current_Resolves_Test_Services_Test()
    {
        using var board = new TestBoardContext();

        BoardContext.Current.Should().BeSameAs(board.Current);
        board.Current.Get<ILocalization>().Should().BeSameAs(board.Localization);
        board.Current.Get<IDataCache>().Should().BeSameAs(board.Cache);
        board.Current.Get<BoardSettings>().Should().BeSameAs(board.BoardSettings);
        board.Current.PageUserID.Should().Be(2);
        board.Current.IsGuest.Should().BeFalse();
    }

    [Test]
    [Description("Custom registrations override the defaults")]
    public void Custom_Registrations_Override_Defaults_Test()
    {
        var localization = new FakeLocalization().Add("A", "B", "custom");

        using var board = new TestBoardContext(b => b.RegisterInstance(localization).As<ILocalization>());

        board.Current.Get<ILocalization>().GetText("A", "B").Should().Be("custom");
    }

    [Test]
    [Description("Consecutive contexts don't share state and the previous container is restored")]
    public void Contexts_Are_Isolated_And_Restored_Test()
    {
        var previousContainer = GlobalContainer.AutoFacContainer;
        var previousLocator = ServiceLocatorAccess.CurrentServiceProvider;

        BoardContext first;

        using (var board = new TestBoardContext())
        {
            first = board.Current;
            board.Cache["key"] = "value";
            board.User.IsGuest = true;
        }

        GlobalContainer.AutoFacContainer.Should().BeSameAs(previousContainer);
        ServiceLocatorAccess.CurrentServiceProvider.Should().BeSameAs(previousLocator);

        using (var board = new TestBoardContext())
        {
            board.Current.Should().NotBeSameAs(first);
            board.Current.Get<IDataCache>()["key"].Should().BeNull();
            board.Current.IsGuest.Should().BeFalse();
        }
    }
}
