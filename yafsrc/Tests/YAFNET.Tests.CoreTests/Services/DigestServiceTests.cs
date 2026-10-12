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

namespace YAF.Tests.CoreTests.Services;

using System.IO;

using Autofac;

using Microsoft.AspNetCore.Hosting;

using MimeKit;

using NSubstitute;

using YAF.Core.BBCode;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The digest e-mail of a user: new and active topics of the readable forums, rendered into the theme templates.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DigestServiceTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private string webRoot;

    private int alice;

    private int bob;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-digest-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(this.webRoot, "theme"));
        File.WriteAllText(Path.Combine(this.webRoot, "theme", "EmailTemplate.html"), "<html>{forumname}|{CONTENT}|{manageLinkText}</html>");
        File.WriteAllText(
            Path.Combine(this.webRoot, "theme", "DigestTopicTemplate.html"),
            "<div>{topicName}|{forumName}|{starter}|{message}|<a href=\"%%topicLink%%\">{link}</a></div>");

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        var theme = Substitute.For<ITheme>();
        theme.BuildThemePath(Arg.Any<string>()).Returns(c => $"/theme/{c.Arg<string>()}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<DigestService>().As<IDigestService>();
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterInstance(theme).As<ITheme>();
                b.RegisterInstance(new RecordingMailService()).As<IMailService>();
            },
            useDatabase: true);

        this.board.LinkBuilder.GetAbsoluteLink(Arg.Any<YAF.Types.Constants.ForumPages>(), Arg.Any<object>())
            .Returns("https://forum.example.com/post");

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum("General");
        this.board.GrantAccess(this.alice, this.forum);

        this.board.Localization.Add("DIGEST", "STARTEDBY", "Started by {0}");
        this.board.Localization.Add("DIGEST", "COMMENTS", "{0} comments");
        this.board.BoardSettings.Name = "My Board";
        this.board.BoardSettings.DigestSendEveryXHours = 24;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(this.webRoot, true);
    }

    [Test]
    [Description("The digest lists new and active topics of the forums the user can read")]
    public void Digest_Test()
    {
        this.AddTopic(this.forum, "New topic", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        this.AddTopic(this.forum, "Active topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddHours(-3));
        this.AddTopic(this.forum, "Old topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-4));
        this.AddTopic(this.db.AddForum("Hidden"), "Hidden topic", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        var html = this.CreateDigest().HtmlBody;

        html.Should().Contain("My Board").And.Contain("New topic").And.Contain("Active topic");
        html.Should().NotContain("Old topic").And.NotContain("Hidden topic");
        html.Should().Contain("https://forum.example.com/post");
    }

    [Test]
    [Description("No digest is created without new or active topics")]
    public void No_Topics_Test()
    {
        this.AddTopic(this.forum, "Old topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-4));

        this.CreateDigest().Should().BeNull();
    }

    [Test]
    [Description("A topic posted inside the digest period is included whatever the time zone of the server")]
    public void Digest_Period_Is_Utc_Test()
    {
        this.AddTopic(this.forum, "Edge topic", DateTime.UtcNow.AddHours(-23.5), DateTime.UtcNow.AddHours(-23.5));

        this.CreateDigest()?.HtmlBody.Should().Contain("Edge topic");
        this.CreateDigest().Should().NotBeNull("the topic was posted 23.5 hours ago, inside the 24 hours");
    }

    [Test]
    [Description("Names in the digest are html encoded (guests choose their own name)")]
    public void Digest_Encodes_Names_Test()
    {
        this.AddTopic(
            this.forum,
            "Topic <b>bold</b>",
            DateTime.UtcNow.AddHours(-2),
            DateTime.UtcNow.AddHours(-1),
            "<a href=\"https://evil.example\">click</a>");

        var html = this.CreateDigest().HtmlBody;

        html.Should().NotContain("evil.example\">").And.NotContain("<b>bold</b>");
        html.Should().Contain("&lt;a href=");
    }

    [Test]
    [Description("The message excerpt is plain text, entities of older html posts are not encoded twice")]
    public void Digest_Message_Excerpt_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.bob, "Cartoons", t => t.Posted = DateTime.UtcNow.AddHours(-2));
        this.db.AddMessage(
            topic,
            this.bob,
            "<p>Tom &amp; Jerry [b]rock[/b][hide-reply]only for repliers[/hide-reply]</p>",
            DateTime.UtcNow.AddHours(-1));

        var html = this.CreateDigest().HtmlBody;

        html.Should().Contain("Tom &amp; Jerry rock").And.NotContain("&amp;amp;").And.NotContain("[b]");
        html.Should().NotContain("only for repliers", "hidden content is only shown in the forum");
    }

    private MimeMessage CreateDigest()
    {
        var user = this.board.Current.GetRepository<User>().GetById(this.alice);

        return this.board.Resolve<IDigestService>()
            .CreateDigest(user, new MailboxAddress("Board", "board@example.com"), "alice@example.com", "Alice");
    }

    private void AddTopic(int forumId, string subject, DateTime posted, DateTime lastPosted, string starter = null)
    {
        var topic = this.db.AddTopic(
            forumId,
            this.bob,
            subject,
            t =>
            {
                t.Posted = posted;
                if (starter != null)
                {
                    t.UserName = starter;
                }
            });

        this.db.AddMessage(topic, this.bob, "the last message", lastPosted);
    }
}
