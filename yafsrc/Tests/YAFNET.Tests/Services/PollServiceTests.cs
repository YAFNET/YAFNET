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

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Models;

/// <summary>
/// The poll helpers: remaining days, closed polls and the width of the vote bars.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PollServiceTests
{
    private TestBoardContext board;

    private PollService polls;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(b => b.RegisterType<PollService>().AsSelf());
        this.polls = this.board.Resolve<PollService>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Polls without an end run forever, the days are counted, the last day closes soon")]
    public void Days_To_Run_Test()
    {
        this.polls.DaysToRun(new Poll(), out var soon).Should().BeNull();
        soon.Should().BeFalse();

        this.polls.DaysToRun(new Poll { Closes = DateTime.UtcNow.AddDays(3).AddHours(1) }, out soon).Should().Be(3);
        soon.Should().BeFalse();

        this.polls.DaysToRun(new Poll { Closes = DateTime.UtcNow.AddHours(5) }, out soon).Should().Be(1);
        soon.Should().BeTrue("less than a day is left");

        this.polls.DaysToRun(new Poll { Closes = DateTime.UtcNow.AddMinutes(-1) }, out soon).Should().Be(0);
        soon.Should().BeFalse();
    }

    [Test]
    [Description("A poll is closed once its end has passed")]
    public void Is_Poll_Closed_Test()
    {
        this.polls.IsPollClosed(new Poll()).Should().BeFalse();
        this.polls.IsPollClosed(new Poll { Closes = DateTime.UtcNow.AddHours(1) }).Should().BeFalse();
        this.polls.IsPollClosed(new Poll { Closes = DateTime.UtcNow.AddSeconds(-1) }).Should().BeTrue();
    }

    [Test]
    [Description("The vote bar is the share of all votes in percent, without votes it is empty")]
    public void Vote_Width_Test()
    {
        var poll = new Poll();
        var choices = new List<Tuple<Poll, Choice>>
        {
            Tuple.Create(poll, new Choice { Votes = 1 }),
            Tuple.Create(poll, new Choice { Votes = 3 })
        };

        this.polls.VoteWidth(choices, choices[0]).Should().Be(25);
        this.polls.VoteWidth(choices, choices[1]).Should().Be(75);

        var noVotes = new List<Tuple<Poll, Choice>> { Tuple.Create(poll, new Choice { Votes = 0 }) };
        this.polls.VoteWidth(noVotes, noVotes[0]).Should().Be(0);
    }
}
