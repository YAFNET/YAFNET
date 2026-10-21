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

using System.ServiceModel.Syndication;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using NSubstitute;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;

/// <summary>
/// The Atom feeds: latest posts of the board, latest topics of a forum and the posts of a topic.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SyndicationFeedsTests
{
    private readonly static TimeSpan Offset = TimeSpan.FromHours(2);

    private TestBoardContext board;

    private TestDatabase db;

    private SyndicationFeeds feeds;

    private int alice;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        var format = Substitute.For<IFormatMessage>();
        format.FormatSyndicationMessageAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
            .Returns(c => Task.FromResult(c.ArgAt<string>(0)));

        var badWords = Substitute.For<IBadWordReplace>();
        badWords.Replace(Arg.Any<string>()).Returns(c => c.Arg<string>());
        badWords.ReplaceHtml(Arg.Any<string>()).Returns(c => c.Arg<string>());

        var dateTime = Substitute.For<IDateTimeService>();
        dateTime.TimeOffset.Returns(Offset);

        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<UrlActionContext>()).Returns("/api/Feed/GetLatestPosts");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<SyndicationFeeds>().AsSelf();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new YAF.Types.Objects.BoardConfiguration());
                b.RegisterInstance(format).As<IFormatMessage>();
                b.RegisterInstance(badWords).As<IBadWordReplace>();
                b.RegisterInstance(dateTime).As<IDateTimeService>();
                b.RegisterInstance(urlHelper).As<IUrlHelper>();
                b.RegisterInstance(Substitute.For<IUserDisplayName>()).As<IUserDisplayName>();
            },
            useDatabase: true);

        this.board.LinkBuilder.GetAbsoluteLink(Arg.Any<ForumPages>(), Arg.Any<object>())
            .Returns("https://forum.example.com/post");
        this.board.LinkBuilder.GetUserProfileLink(Arg.Any<int>(), Arg.Any<string>()).Returns("https://forum.example.com/user");

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.forum = this.db.AddForum("General");
        this.board.GrantAccess(this.alice, this.forum);
        this.board.LoginAs(this.alice);

        this.feeds = this.board.Resolve<SyndicationFeeds>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The latest posts feed lists the newest topics with their last post, the times are utc (the feed reader shows them in its time zone)")]
    public async Task Latest_Posts_Test()
    {
        var posted = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        var topic = this.db.AddTopic(this.forum, this.alice, "Hello");
        this.db.AddMessage(topic, this.alice, "latest text", posted);

        var feed = await this.feeds.GetPostLatestFeedAsync();

        var item = feed.Items.Should().ContainSingle().Subject;
        item.Title.Text.Should().Be("Hello");
        ((TextSyndicationContent)item.Content).Text.Should().Be("latest text");
        // the moment of the post (the time of the user was marked with the time zone of the server)
        item.PublishDate.Should().Be(new DateTimeOffset(posted));
        item.PublishDate.Offset.Should().Be(TimeSpan.Zero);
        feed.LastUpdatedTime.Should().Be(new DateTimeOffset(posted));
    }

    [Test]
    [Description("A board without posts has an empty feed")]
    public async Task Latest_Posts_Empty_Test()
    {
        var feed = await this.feeds.GetPostLatestFeedAsync();

        feed.Items.Should().BeEmpty();
    }

    [Test]
    [Description("The topics feed lists the latest topics of the forum")]
    public async Task Topics_Feed_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.alice, "Forum topic");
        this.db.AddMessage(topic, this.alice, "topic text");
        this.board.SetPage(this.forum);

        var feed = await this.feeds.GetTopicsFeedAsync(this.forum);

        feed.Items.Should().ContainSingle().Which.Title.Text.Should().Be("Forum topic");
        feed.Title.Text.Should().Contain("General");
    }

    [Test]
    [Description("The posts feed lists the posts of the topic")]
    public async Task Posts_Feed_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.alice, "Thread");
        this.db.AddMessage(topic, this.alice, "first");
        this.db.AddMessage(topic, this.alice, "second");
        this.board.SetPage(this.forum, topic);

        var feed = await this.feeds.GetPostsFeedAsync(topic);

        feed.Items.Select(i => ((TextSyndicationContent)i.Content).Text).Should().BeEquivalentTo("first", "second");
    }

    [Test]
    [Description("Deleted posts never show their text in the posts feed, also when deleted posts are shown to all")]
    public async Task Posts_Feed_Deleted_Test()
    {
        this.board.BoardSettings.ShowDeletedMessagesToAll = true;

        var topic = this.db.AddTopic(this.forum, this.alice, "Thread");
        this.db.AddMessage(topic, this.alice, "visible");
        this.db.AddMessage(
            topic,
            this.alice,
            "secret deleted text",
            configure: m => m.Flags |= (int)MessageFlags.Flags.IsDeleted);
        this.board.SetPage(this.forum, topic);

        var feed = await this.feeds.GetPostsFeedAsync(topic);

        feed.Items.Select(i => ((TextSyndicationContent)i.Content).Text).Should().NotContain("secret deleted text");
    }
}
