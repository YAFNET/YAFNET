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

using System.IO;
using System.Net.Mail;

using Autofac;

using NSubstitute;

using YAF.Core.BBCode;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The digest e-mail of a user: new and active topics of the readable forums, rendered into the theme templates.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DigestTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        var theme = Substitute.For<ITheme>();
        theme.BuildThemePath(Arg.Any<string>()).Returns(c => $"~/theme/{c.Arg<string>()}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<Digest>().As<IDigest>();
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterInstance(theme).As<ITheme>();
                b.RegisterInstance(new RecordingMailService()).As<IMailService>();
            },
            useDatabase: true);

        Directory.CreateDirectory(Path.Combine(this.board.WebRoot, "theme"));
        File.WriteAllText(
            Path.Combine(this.board.WebRoot, "theme", "EmailTemplate.html"),
            "<html>{forumname}|{CONTENT}|{manageLinkText}</html>");
        File.WriteAllText(
            Path.Combine(this.board.WebRoot, "theme", "DigestTopicTemplate.html"),
            "<div>{topicName}|{forumName}|{starter}|{message}|<a href=\"%%topicLink%%\">{link}</a></div>");

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.forum = this.db.AddForum("General");
        this.board.GrantAccess(this.alice, this.forum);

        this.board.Localization.Add("DIGEST", "STARTEDBY", "Started by {0}");
        this.board.Localization.Add("DIGEST", "COMMENTS", "{0} comments");
        this.board.Localization.Add("DIGEST", "SUBJECT", "Digest of {0}");
        this.board.BoardSettings.Name = "My Board";
        this.board.BoardSettings.DigestSendEveryXHours = 24;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The digest lists new and active topics of the forums the user can read")]
    public void Digest_Test()
    {
        this.AddTopic(this.forum, "New topic", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));
        this.AddTopic(this.forum, "Active topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddHours(-3));
        this.AddTopic(this.forum, "Old topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-4));
        this.AddTopic(this.db.AddForum("Hidden"), "Hidden topic", DateTime.UtcNow.AddHours(-2), DateTime.UtcNow.AddHours(-1));

        var html = this.CreateDigest();

        html.Should().Contain("My Board").And.Contain("New topic").And.Contain("Active topic");
        html.Should().NotContain("Old topic").And.NotContain("Hidden topic");
    }

    [Test]
    [Description("No digest is created without new or active topics")]
    public void No_Topics_Test()
    {
        this.AddTopic(this.forum, "Old topic", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-4));

        this.CreateDigest().Should().BeNull();
    }

    [Test]
    [Description("A topic posted at the start of the digest period is included whatever the time zone of the server")]
    public void Digest_Period_Is_Utc_Test()
    {
        this.AddTopic(this.forum, "Edge topic", DateTime.UtcNow.AddHours(-23.5), DateTime.UtcNow.AddHours(-23.5));

        this.CreateDigest().Should().NotBeNull("the topic was posted 23.5 hours ago, inside the 24 hours")
            .And.Contain("Edge topic");
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

        var html = this.CreateDigest();

        html.Should().NotContain("evil.example\">").And.NotContain("<b>bold</b>");
        html.Should().Contain("&lt;a href=");
    }

    [Test]
    [Description("The message excerpt is plain text, entities of older html posts are not encoded twice, hidden content is left out")]
    public void Digest_Message_Excerpt_Test()
    {
        var topic = this.db.AddTopic(this.forum, this.bob, "Cartoons", t => t.Posted = DateTime.UtcNow.AddHours(-2));
        this.db.AddMessage(
            topic,
            this.bob,
            "<p>Tom &amp; Jerry [b]rock[/b][hide-reply]only for repliers[/hide-reply]</p>",
            DateTime.UtcNow.AddHours(-1));

        var html = this.CreateDigest();

        html.Should().Contain("Tom &amp; Jerry rock").And.NotContain("&amp;amp;").And.NotContain("[b]");
        html.Should().NotContain("only for repliers", "hidden content is only shown in the forum");
    }

    private string CreateDigest()
    {
        var user = this.board.Current.GetRepository<User>().GetById(this.alice);

        var mail = this.board.Resolve<IDigest>().CreateDigest(
            user,
            new MailAddress("board@example.com", "Board"),
            "alice@example.com",
            "Alice");

        return mail == null ? null : RecordingMailService.HtmlOf(mail);
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
                t.LastPosted = lastPosted;

                if (starter != null)
                {
                    t.UserName = starter;
                    t.UserDisplayName = starter;
                }
            });

        this.db.AddMessage(topic, this.bob, "Some text", lastPosted);
    }
}
