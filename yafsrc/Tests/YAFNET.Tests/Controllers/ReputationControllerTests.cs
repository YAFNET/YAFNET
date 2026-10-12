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

using Autofac;

using NSubstitute;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Reputation votes: the rules the post page uses to show the vote buttons must also hold for direct requests.
/// </summary>
[TestFixture]
[NonParallelizable]
public class ReputationControllerTests
{
    private TestBoardContext board;

    private ISessionService session;

    private int voter;

    private int author;

    private int post;

    [SetUp]
    public void SetUp()
    {
        this.session = Substitute.For<ISessionService>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.session).As<ISessionService>();
                b.RegisterType<Reputation>().As<IReputation>();
            },
            useDatabase: true);

        var db = this.board.Database;
        this.voter = db.AddUser("Voter", u => u.Points = 200);
        this.author = db.AddUser("Author", u => u.Points = 10);

        var forum = db.AddForum();
        this.post = db.AddMessage(db.AddTopic(forum, this.author), this.author);

        this.board.BoardSettings.EnableUserReputation = true;
        this.board.BoardSettings.ReputationAllowNegative = true;
        this.board.BoardSettings.ReputationMinDownVoting = 100;

        this.board.LoginAs(this.voter);
        this.ShowPost(this.post, this.author);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Up and down votes change the author's points and are recorded")]
    public async Task Vote_Test()
    {
        await this.Controller().AddReputation(this.post);

        this.Points(this.author).Should().Be(11);
        this.board.Current.GetRepository<ReputationVote>().Count(v => v.ReputationFromUserID == this.voter).Should().Be(1);
    }

    [Test]
    [Description("Down votes reduce the points")]
    public async Task Vote_Down_Test()
    {
        await this.Controller().RemoveReputation(this.post);

        this.Points(this.author).Should().Be(9);
    }

    [Test]
    [Description("A user can vote for another user only once in 24 hours, also without reloading the page")]
    public async Task Vote_Once_Per_Day_Test()
    {
        await this.Controller().AddReputation(this.post);
        await this.Controller().AddReputation(this.post);

        this.Points(this.author).Should().Be(11, "the second vote within 24 hours is ignored");

        await this.Controller().RemoveReputation(this.post);

        this.Points(this.author).Should().Be(11, "a down vote within 24 hours is ignored too");
    }

    [Test]
    [Description("Users can not vote for their own posts")]
    public async Task Own_Post_Test()
    {
        var own = this.board.Database.AddMessage(this.board.Database.AddTopic(this.board.Database.AddForum(), this.voter), this.voter);
        this.ShowPost(own, this.voter);

        await this.Controller().AddReputation(own);

        this.Points(this.voter).Should().Be(200);
    }

    [Test]
    [Description("Nobody can vote when reputation is disabled")]
    public async Task Reputation_Disabled_Test()
    {
        this.board.BoardSettings.EnableUserReputation = false;

        await this.Controller().AddReputation(this.post);

        this.Points(this.author).Should().Be(10);
    }

    [Test]
    [Description("Down votes need enough own points and negative reputation to be allowed")]
    public async Task Vote_Down_Rules_Test()
    {
        this.board.BoardSettings.ReputationMinDownVoting = 500;
        await this.Controller().RemoveReputation(this.post);
        this.Points(this.author).Should().Be(10, "the voter has not enough points to vote down");

        this.board.BoardSettings.ReputationMinDownVoting = 100;
        this.board.BoardSettings.ReputationAllowNegative = false;
        await this.Controller().RemoveReputation(this.post);
        this.Points(this.author).Should().Be(10, "negative reputation is disabled");
    }

    [Test]
    [Description("A post that was not shown to the user can not be voted for")]
    public async Task Unknown_Post_Test()
    {
        await this.Controller().AddReputation(99999);

        this.Points(this.author).Should().Be(10);
    }

    [Test]
    [Description("Guests can not vote")]
    public async Task Guest_Test()
    {
        this.board.User.IsGuest = true;

        await this.Controller().AddReputation(this.post);

        this.Points(this.author).Should().Be(10);
    }

    private ReputationController Controller()
    {
        return this.board.CreateController<ReputationController>();
    }

    /// <summary>
    /// The post page stores the shown posts in the session, the vote date is the one at render time.
    /// </summary>
    private void ShowPost(int messageId, int userId)
    {
        this.session.GetPageData<List<PagedMessage>>().Returns(
            [
                new PagedMessage
                {
                    MessageID = messageId,
                    UserID = userId,
                    Topic = "Topic",
                    UserName = "Author",
                    DisplayName = "Author",
                    Points = this.Points(userId),
                    ReputationVoteDate = DateTime.MinValue
                }
            ]);
    }

    private int Points(int userId)
    {
        return this.board.Current.GetRepository<User>().GetById(userId).Points;
    }
}
