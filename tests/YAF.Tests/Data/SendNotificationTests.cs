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

using System.IO;
using System.Net.Mail;

using Autofac;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// SendNotification: watch mails, moderator mails and the account related mails.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SendNotificationTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private RecordingMailService mail;

    private IAspNetUsersHelper aspNetUsers;

    private int alice;

    private int bob;

    private int carol;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        var theme = Substitute.For<ITheme>();
        theme.BuildThemePath(Arg.Any<string>()).Returns(c => c.Arg<string>());

        // the html mails are formatted with the bb code of the posts, here the text is kept as it is
        var bbCode = Substitute.For<IBBCode>();
        bbCode.MakeHtml(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>()).Returns(c => c.Arg<string>());

        var displayName = Substitute.For<IUserDisplayName>();
        displayName.GetNameById(Arg.Any<int>()).Returns(c => $"User {c.Arg<int>()}");

        this.mail = new RecordingMailService();
        this.aspNetUsers = Substitute.For<IAspNetUsersHelper>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<SendNotification>().As<ISendNotification>();
                b.RegisterType<ActivityStream>().As<IActivityStream>();
                b.RegisterType<BadWordReplace>().As<IBadWordReplace>();
                b.RegisterType<FormatMessage>().As<IFormatMessage>();
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => Substitute.For<IProcessReplaceRules>());
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterInstance(theme).As<ITheme>();
                b.RegisterInstance(bbCode).As<IBBCode>();
                b.RegisterInstance(displayName).As<IUserDisplayName>();
                b.RegisterInstance(this.mail).As<IMailService>();
                b.RegisterInstance(this.aspNetUsers).As<IAspNetUsersHelper>();
                b.RegisterInstance(Substitute.For<IAspNetRolesHelper>()).As<IAspNetRolesHelper>();
            },
            useDatabase: true);

        File.WriteAllText(Path.Combine(this.board.WebRoot, "EmailTemplate.html"), "<html>{CONTENT}</html>");
        File.WriteAllText(Path.Combine(this.board.WebRoot, "bootstrap-forum.min.css"), "body{}");

        this.db = this.board.Database;

        this.board.BoardSettings.Name = "Test Forum";
        this.board.BoardSettings.ForumEmail = "forum@example.com";

        this.board.Localization
            .Add("COMMON", "TOPIC_NOTIFICATION_SUBJECT", "New post on {0}")
            .Add("TEMPLATES", "TOPICPOST_TEXT", "{topic} by {postedby}: {body}")
            .Add("COMMON", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL", "Approval needed on {0}")
            .Add("TEMPLATES", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL_TEXT", "Hi {user}, see {adminlink}");

        this.alice = this.db.AddUser("Alice", u => u.NotificationType = 0);
        this.bob = this.db.AddUser("Bob", u =>
        {
            u.NotificationType = 0;
            u.Activity = false;
        });
        this.carol = this.db.AddUser("Carol", u =>
        {
            u.NotificationType = 0;
            u.Activity = true;
        });

        this.forum = this.db.AddForum("Forum");
        this.topic = this.db.AddTopic(this.forum, this.alice, "Watched topic");

        this.board.PageUser.ID = this.alice;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Watchers of the topic and the forum get one mail each, the poster gets none")]
    public void ToWatchingUsers_Test()
    {
        this.GrantRead(this.alice, this.bob, this.carol);
        this.Watch(this.bob, this.carol);

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchForum { UserID = this.carol, ForumID = this.forum, Created = DateTime.UtcNow });
            connection.Insert(new WatchTopic { UserID = this.alice, TopicID = this.topic, Created = DateTime.UtcNow });
        }

        this.Notifications.ToWatchingUsers(this.Post(this.alice, "Hello watchers"));

        this.mail.Recipients.Should().BeEquivalentTo("bob@example.com", "carol@example.com");
        this.mail.Sent.Should().OnlyContain(m => m.Subject == "New post on Test Forum");

        // only Carol has the activity stream turned on
        this.Repository<Activity>().Get(_ => true).Select(a => a.UserID).Should().Equal(this.carol);
    }

    [Test]
    [Description("Watchers who can not read the forum (any more) get no mail and no activity with the post")]
    public void ToWatchingUsers_Without_Read_Access_Test()
    {
        this.GrantRead(this.alice, this.bob);
        this.Watch(this.bob, this.carol);

        this.Notifications.ToWatchingUsers(this.Post(this.alice, "Secret"));

        this.mail.Recipients.Should().Equal("bob@example.com");
        this.Repository<Activity>().Count(a => a.UserID == this.carol).Should().Be(0);
    }

    [Test]
    [Description("The hidden content of a post (only shown in the forum) is not in the mail")]
    public void ToWatchingUsers_Hidden_Content_Test()
    {
        this.GrantRead(this.alice, this.bob);
        this.Watch(this.bob);

        this.Notifications.ToWatchingUsers(this.Post(this.alice, "visible [hide-reply]secret[/hide-reply] text"));

        var message = this.mail.Sent.Should().ContainSingle().Subject;
        RecordingMailService.TextOf(message).Should().Contain("visible").And.NotContain("secret");
        RecordingMailService.HtmlOf(message).Should().Contain("visible").And.NotContain("secret");
    }

    [Test]
    [Description("Without watchers no mail is sent")]
    public void ToWatchingUsers_No_Watchers_Test()
    {
        this.Notifications.ToWatchingUsers(this.Post(this.alice, "Nobody watches"));

        this.mail.Sent.Should().BeEmpty();
    }

    [Test]
    [Description("A mail error (e.g. the mail server is down) does not fail the post the notification is sent for")]
    public void ToWatchingUsers_Mail_Error_Test()
    {
        this.GrantRead(this.alice, this.bob);
        this.Watch(this.bob);

        this.mail.Error = new SmtpException("the server is down");

        FluentActions.Invoking(() => this.Notifications.ToWatchingUsers(this.Post(this.alice, "Hello")))
            .Should().NotThrow();

        this.board.Logger.ReceivedWithAnyArgs().Log(default);
    }

    private ISendNotification Notifications => this.board.Resolve<ISendNotification>();

    private Message Post(int userId, string text)
    {
        var id = this.db.AddMessage(this.topic, userId, text);

        return this.Repository<Message>().GetMessage(id);
    }

    private void Watch(params int[] userIds)
    {
        using var connection = this.db.Open();

        foreach (var userId in userIds)
        {
            connection.Insert(new WatchTopic { UserID = userId, TopicID = this.topic, Created = DateTime.UtcNow });
        }
    }

    private void GrantRead(params int[] userIds)
    {
        foreach (var userId in userIds)
        {
            this.board.GrantAccess(userId, this.forum);
        }
    }

    private IRepository<T> Repository<T>()
        where T : class, IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
