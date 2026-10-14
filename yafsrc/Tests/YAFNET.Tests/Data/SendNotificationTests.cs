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
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;

using MimeKit;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.BBCode;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Models.Identity;
using YAF.Types.Objects;

/// <summary>
/// SendNotification: watch mails, moderator mails and the account related mails.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SendNotificationTests
{
    private string webRoot;

    private TestBoardContext board;

    private TestDatabase db;

    private RecordingMailService mail;

    private IAspNetUsersHelper aspNetUsers;

    private IUserDisplayName displayName;

    private int alice;

    private int bob;

    private int carol;

    private int forum;

    private int topic;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-mail-{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.webRoot);
        File.WriteAllText(Path.Combine(this.webRoot, "EmailTemplate.html"), "<html>{CONTENT}</html>");
        File.WriteAllText(Path.Combine(this.webRoot, "theme.css"), "body{}");

        // the mail templates are read from the real language file (BoardSettings.Language defaults to english.json)
        Directory.CreateDirectory(Path.Combine(this.webRoot, "languages"));
        File.Copy(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "english.json"),
            Path.Combine(this.webRoot, "languages", "english.json"));

        var theme = Substitute.For<ITheme>();
        theme.BuildMappedThemePath(Arg.Any<string>()).Returns(c => Path.Combine(this.webRoot, c.Arg<string>()));
        theme.BuildThemePath(Arg.Any<string>()).Returns("theme.css");

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        var displayName = this.displayName = Substitute.For<IUserDisplayName>();
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
                b.RegisterType<BBCodeService>().As<IBBCodeService>();
                b.Register<Func<IEnumerable<bool>, IProcessReplaceRules>>(_ => _ => new ProcessReplaceRules());
                b.RegisterType<DataBroker>().AsSelf();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(new VapidConfiguration());
                b.RegisterInstance(theme).As<ITheme>();
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterInstance(displayName).As<IUserDisplayName>();
                b.RegisterInstance(this.mail).As<IMailService>();
                b.RegisterInstance(this.aspNetUsers).As<IAspNetUsersHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.board.BoardSettings.Name = "Test Forum";
        this.board.BoardSettings.ForumEmail = "forum@example.com";

        this.board.Localization
            .Add("COMMON", "TOPIC_NOTIFICATION_SUBJECT", "New post on {0}")
            .Add("COMMON", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL", "Approval needed on {0}")
            .Add("COMMON", "NOTIFICATION_ON_MODERATOR_SPAMMESSAGE_APPROVAL", "Spam approval needed on {0}")
            .Add("TEMPLATES", "NOTIFICATION_ON_MODERATOR_MESSAGE_APPROVAL_TEXT", "Hi {user}, see {adminlink}")
            .Add("TEMPLATES", "NOTIFICATION_ON_MODERATOR_SPAMMESSAGE_APPROVAL_TEXT", "Hi {user}, spam at {adminlink}");

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

        try
        {
            Directory.Delete(this.webRoot, true);
        }
        catch (IOException)
        {
            // ignore
        }
    }

    [Test]
    [Description("Watchers of the topic and the forum get one mail each, the poster gets none")]
    public async Task ToWatchingUsers_Test()
    {
        var message = this.db.AddMessage(
            this.topic,
            this.alice,
            "[b]Hello[/b] <i>watchers</i>[hide-reply]only for repliers[/hide-reply][hide-thanks]only for thankers[/hide-thanks]");
        this.GrantReadAccess(this.alice, this.bob, this.carol);

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchTopic { UserID = this.alice, TopicID = this.topic, Created = DateTime.UtcNow });
            connection.Insert(new WatchTopic { UserID = this.bob, TopicID = this.topic, Created = DateTime.UtcNow });
            connection.Insert(new WatchForum { UserID = this.carol, ForumID = this.forum, Created = DateTime.UtcNow });
            connection.Insert(new WatchForum { UserID = this.bob, ForumID = this.forum, Created = DateTime.UtcNow });
        }

        await this.Notifications.ToWatchingUsersAsync(message);

        this.mail.Recipients.Should().BeEquivalentTo("bob@example.com", "carol@example.com");

        var mailToBob = this.mail.Sent.Single(m => m.To.Mailboxes.Single().Address == "bob@example.com");
        mailToBob.Subject.Should().Be("New post on Test Forum");
        mailToBob.From.Mailboxes.Single().Address.Should().Be("forum@example.com");
        mailToBob.TextBody.Should().Contain("There's a new post in topic \"Watched topic\" at Test Forum")
            .And.Contain($"By User {this.alice}:").And.Contain("\"Hello watchers\"");

        // hidden content is only shown in the forum (to users who replied, thanked, ...)
        mailToBob.TextBody.Should().NotContain("only for repliers").And.NotContain("only for thankers");

        // only Carol has the activity stream enabled
        (await this.Repository<Activity>().GetAsync(a => a.UserID == this.carol)).Should().ContainSingle()
            .Which.Flags.Should().Be(new ActivityFlags { WatchTopicReply = true }.BitValue);
        (await this.Repository<Activity>().CountAsync(a => a.UserID == this.bob)).Should().Be(0);
    }

    [Test]
    [Description("A new topic creates 'new topic in watched forum' activities")]
    public async Task ToWatchingUsers_New_Topic_Test()
    {
        var message = this.db.AddMessage(this.topic, this.alice, "first post");
        this.GrantReadAccess(this.carol);

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchForum { UserID = this.carol, ForumID = this.forum, Created = DateTime.UtcNow });
        }

        await this.Notifications.ToWatchingUsersAsync(this.Repository<Message>().GetMessage(message), true);

        (await this.Repository<Activity>().GetSingleAsync(a => a.UserID == this.carol)).Flags
            .Should().Be(new ActivityFlags { WatchForumReply = true }.BitValue);
    }

    [Test]
    [Description("Watchers who can not read the forum (any more) get no mail and no activity with the post")]
    public async Task ToWatchingUsers_Without_Read_Access_Test()
    {
        var message = this.db.AddMessage(this.topic, this.alice, "secret staff post");
        this.GrantReadAccess(this.bob);
        this.db.AddForumAccess(this.carol, this.forum, AccessFlags.Flags.None);

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchTopic { UserID = this.bob, TopicID = this.topic, Created = DateTime.UtcNow });
            connection.Insert(new WatchTopic { UserID = this.carol, TopicID = this.topic, Created = DateTime.UtcNow });
            connection.Insert(new WatchForum { UserID = this.carol, ForumID = this.forum, Created = DateTime.UtcNow });
        }

        await this.Notifications.ToWatchingUsersAsync(message);

        this.mail.Recipients.Should().Equal("bob@example.com");
        (await this.Repository<Activity>().CountAsync(a => a.UserID == this.carol)).Should().Be(0);
    }

    [Test]
    [Description("Without watchers no mail is sent")]
    public async Task ToWatchingUsers_No_Watchers_Test()
    {
        var message = this.db.AddMessage(this.topic, this.alice);

        await this.Notifications.ToWatchingUsersAsync(message);

        this.mail.Sent.Should().BeEmpty();
    }

    [Test]
    [Description("Every moderator of the forum gets one approval mail")]
    public async Task ToModerators_Approval_Test()
    {
        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        this.board.Database.AddForumAccess(this.carol, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        this.board.Database.AddForumAccess(this.alice, this.forum);

        await this.Notifications.ToModeratorsThatMessageNeedsApprovalAsync(this.forum, 1, false);

        this.mail.Recipients.Should().BeEquivalentTo("bob@example.com", "carol@example.com");
        this.mail.Sent.Should().OnlyContain(m => m.Subject == "Approval needed on Test Forum");

        await this.Notifications.ToModeratorsThatMessageNeedsApprovalAsync(this.forum, 1, true);

        this.mail.Sent.Where(m => m.Subject == "Spam approval needed on Test Forum").Should().HaveCount(2);
    }

    [Test]
    [Description("The approval notification only completes once the mails are sent")]
    public async Task ToModerators_Approval_Waits_For_Mails_Test()
    {
        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        var mailServer = new TaskCompletionSource();
        this.mail.Delay = mailServer.Task;

        var notification = this.Notifications.ToModeratorsThatMessageNeedsApprovalAsync(this.forum, 1, false);

        await Task.Delay(100);
        notification.IsCompleted.Should().BeFalse("the mail hasn't been sent yet");

        mailServer.SetResult();
        await notification;

        this.mail.Recipients.Should().Equal("bob@example.com");
    }

    [Test]
    [Description("Topic names and names of users are text in the html mail, not html")]
    public async Task ToWatchingUsers_Html_In_Topic_Name_Test()
    {
        await this.Repository<Topic>().UpdateOnlyAsync(
            () => new Topic { TopicName = "&lt;a href=\"https://evil.example\"&gt;Click&lt;/a&gt;" },
            t => t.ID == this.topic);
        this.displayName.GetNameById(this.alice).Returns("<img src=x onerror=alert(1)>");

        var message = this.db.AddMessage(this.topic, this.alice, "Hello watchers");
        this.GrantReadAccess(this.alice, this.bob);

        using (var connection = this.db.Open())
        {
            connection.Insert(new WatchTopic { UserID = this.bob, TopicID = this.topic, Created = DateTime.UtcNow });
        }

        await this.Notifications.ToWatchingUsersAsync(message);

        var mailToBob = this.mail.Sent.Should().ContainSingle().Subject;

        mailToBob.TextBody.Should().Contain("<a href=\"https://evil.example\">Click</a>", "the text part is plain text");

        mailToBob.HtmlBody.Should().NotContain("<a href=\"https://evil.example\"").And.NotContain("<img src=x")
            .And.Contain("&lt;a href=").And.Contain("&lt;img src=x");
    }

    [Test]
    [Description("The reason of a report is text in the html mail of the moderators, not html")]
    public async Task ToModerators_Reported_Html_Reason_Test()
    {
        this.board.Localization
            .Add("COMMON", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE", "Report on {0}")
            .Add("TEMPLATES", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE_TEXT", "{reporter} reported: {reason}");

        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        await this.Notifications.ToModeratorsThatMessageWasReportedAsync(
            this.forum,
            1,
            this.carol,
            "<a href='https://evil.example'>log in again</a>");

        var report = this.mail.Sent.Should().ContainSingle().Subject;

        report.HtmlBody.Should().NotContain("<a href='https://evil.example'>")
            .And.Contain("&lt;a href=&#39;https://evil.example&#39;&gt;log in again&lt;/a&gt;");
    }

    [Test]
    [Description("A code block in a value of a user is no html in the mail (the highlighter decodes the code once)")]
    public async Task ToModerators_Reported_Html_In_Code_Block_Test()
    {
        this.board.Localization
            .Add("COMMON", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE", "Report on {0}")
            .Add("TEMPLATES", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE_TEXT", "{reporter} reported: {reason}");

        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        await this.Notifications.ToModeratorsThatMessageWasReportedAsync(
            this.forum,
            1,
            this.carol,
            "[code]<a href='https://evil.example'>log in again</a>[/code]");

        var report = this.mail.Sent.Should().ContainSingle().Subject;

        report.HtmlBody.Should().NotContain("<a href='https://evil.example'>");
    }

    [Test]
    [Description("The bb codes of a value of a user are text in the mail (no disguised links)")]
    public async Task ToModerators_Reported_BBCode_Reason_Test()
    {
        // the template of the board language (english.json) with the link to the reported posts
        this.board.Localization.Add("COMMON", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE", "Report on {0}");
        this.board.LinkBuilder.GetAbsoluteLink(ForumPages.Moderate_ReportedPosts, Arg.Any<object>())
            .Returns("https://forum.example/moderate");

        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        await this.Notifications.ToModeratorsThatMessageWasReportedAsync(
            this.forum,
            1,
            this.carol,
            "[url=https://evil.example]Your account[/url] [b]bold[/b]");

        var report = this.mail.Sent.Should().ContainSingle().Subject;

        report.TextBody.Should().Contain("[url=https://evil.example]Your account[/url]");

        report.HtmlBody.Should().NotContain("https://evil.example\"").And.NotContain("<b>bold</b>")
            .And.Contain("&#91;url=https://evil.example&#93;Your account");

        // the links of the template text are still formatted
        report.HtmlBody.Should().Contain("href=\"https://forum.example/moderate\"");
    }

    [Test]
    [Description("Reported posts notify the moderators with the reason and reporter")]
    public async Task ToModerators_Reported_Test()
    {
        // the template is the one of the board language (english.json)
        this.board.Localization.Add("COMMON", "NOTIFICATION_ON_MODERATOR_REPORTED_MESSAGE", "Report on {0}");

        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);

        this.aspNetUsers.GetUserByNameAsync("Bob").Returns(new AspNetUsers { Id = "bob-key", UserName = "Bob", Email = "bob@example.com" });
        this.aspNetUsers.GetUserFromProviderUserKeyAsync("bob-key").Returns(await this.Repository<User>().GetByIdAsync(this.bob));

        await this.Notifications.ToModeratorsThatMessageWasReportedAsync(this.forum, 1, this.carol, "spam!");

        var report = this.mail.Sent.Should().ContainSingle().Subject;
        report.Subject.Should().Be("Report on Test Forum");
        report.TextBody.Should().Contain($"A message was reported by User {this.carol} for the following reason: spam!")
            .And.Contain("The Message was posted in the Test Forum forum.");
    }

    [Test]
    [Description("Errors while notifying about reports are logged, not thrown")]
    public async Task ToModerators_Reported_Error_Test()
    {
        this.board.Database.AddForumAccess(this.bob, this.forum, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        this.displayName.GetNameById(Arg.Any<int>()).Returns<string>(_ => throw new InvalidOperationException("boom"));

        await FluentActions.Awaiting(() => this.Notifications.ToModeratorsThatMessageWasReportedAsync(this.forum, 1, this.carol, "x"))
            .Should().NotThrowAsync();

        this.mail.Sent.Should().BeEmpty();
    }

    [Test]
    [Description("Medal, role and suspension notifications go to the user with the template filled in (the user has no own language: the templates of the board language)")]
    public async Task User_Notifications_Test()
    {
        this.board.Localization
            .Add("COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", "Medal on {0}")
            .Add("DEFAULT", "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT", "Roles on {0}");

        var user = await this.board.Current.GetRepository<User>().GetByIdAsync(this.bob);

        await this.Notifications.ToUserWithNewMedalAsync(this.bob, "Gold");
        await this.Notifications.ToUserWithNewMedalAsync(-1, "None");
        await this.Notifications.SendRoleAssignmentNotificationAsync(user, ["Editors", "Testers"]);
        await this.Notifications.SendRoleUnAssignmentNotificationAsync(user, ["Guests"]);
        await this.Notifications.SendUserSuspensionNotificationAsync(DateTime.UtcNow.AddDays(1), "spam", user);
        await this.Notifications.SendUserSuspensionEndedNotificationAsync(user);

        var texts = this.mail.Sent.Select(m => m.TextBody).ToList();

        texts.Should().HaveCount(5);
        texts[0].Should().Contain("Hello Bob.").And.Contain("awarded with the Gold on the Test Forum forum");
        texts[1].Should().Contain("Hello Bob,").And.Contain("include access to the following Group(s)").And.Contain("Editors,Testers");
        texts[2].Should().Contain("restrict access to the following Groups(s)").And.Contain("Guests");
        texts[3].Should().Contain("Your forum account on the Test Forum forum has been temporarily suspended").And.Contain("spam");
        texts[4].Should().Contain("The suspension of your forum account has ended");

        this.mail.Sent[0].Subject.Should().Be("Medal on Test Forum");
        this.mail.Recipients.Should().OnlyContain(r => r == "bob@example.com");
        this.mail.Sent[0].HtmlBody.Should().StartWith("<html>").And.Contain("awarded with the Gold");
    }

    [Test]
    [Description("The role and suspension mails are sent by an admin, they are in the language of the user who gets them (not of the admin)")]
    public async Task User_Notifications_Language_Of_The_User_Test()
    {
        this.board.BoardSettings.AllowUserLanguage = true;

        // the templates are read from the language file of the user
        File.Copy(
            Path.Combine(TestContext.CurrentContext.TestDirectory, "languages", "german-du.json"),
            Path.Combine(this.webRoot, "languages", "german-du.json"));

        this.board.Localization
            .Add("COMMON", "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT", "Roles on {0}")
            .Add("COMMON", "NOTIFICATION_ROLE_ASSIGNMENT_SUBJECT", "Rollen in {0}", "german-du.json")
            .Add("TEMPLATES", "NOTIFICATION_ROLE_ASSIGNMENT_TEXT", "{user} added {roles}")
            .Add("TEMPLATES", "NOTIFICATION_ROLE_UNASSIGNMENT_TEXT", "{user} removed {roles}")
            .Add("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", "Suspension on {0}")
            .Add("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", "Sperrung in {0}", "german-du.json")
            .Add("TEMPLATES", "NOTIFICATION_ON_SUSPENDING_USER_TEXT", "{user} suspended: {suspendReason}")
            .Add("TEMPLATES", "NOTIFICATION_ON_SUSPENDING_ENDED_USER_TEXT", "{user} is back");

        await this.board.Current.GetRepository<User>().UpdateOnlyAsync(() => new User { LanguageFile = "german-du.json" }, u => u.ID == this.bob);

        var user = await this.board.Current.GetRepository<User>().GetByIdAsync(this.bob);

        await this.Notifications.SendRoleAssignmentNotificationAsync(user, ["Editors"]);
        await this.Notifications.SendRoleUnAssignmentNotificationAsync(user, ["Guests"]);
        await this.Notifications.SendUserSuspensionNotificationAsync(DateTime.UtcNow.AddDays(1), "spam", user);
        await this.Notifications.SendUserSuspensionEndedNotificationAsync(user);

        this.mail.Sent.Select(m => m.Subject).Should().Equal(
            "Rollen in Test Forum",
            "Rollen in Test Forum",
            "Sperrung in Test Forum",
            "Sperrung in Test Forum");

        // the texts of the language file of the user, not the ones of the current language
        this.mail.Sent.Should().OnlyContain(m => m.TextBody.TrimStart().StartsWith("Hello Bob"));
    }

    [Test]
    [Description("A user without an own language gets the mails in the language of the board, not in the language of the sender (the current user)")]
    public async Task User_Notifications_Board_Language_Test()
    {
        this.board.BoardSettings.Language = "english.json";

        // the current language (of the admin) is german, the board language english
        this.board.Localization
            .Add("COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", "Medaille in {0}")
            .Add("COMMON", "NOTIFICATION_ON_MEDAL_AWARDED_SUBJECT", "Medal on {0}", "english.json")
            .Add("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", "Sperrung in {0}")
            .Add("COMMON", "NOTIFICATION_ON_SUSPENDING_USER_SUBJECT", "Suspension on {0}", "english.json");

        await this.Notifications.ToUserWithNewMedalAsync(this.bob, "Gold");
        await this.Notifications.SendUserSuspensionEndedNotificationAsync(await this.board.Current.GetRepository<User>().GetByIdAsync(this.bob));

        this.mail.Sent.Select(m => m.Subject).Should().Equal("Medal on Test Forum", "Suspension on Test Forum");
    }

    [Test]
    [Description("Registration mails go to every address of the notification list")]
    public async Task Registration_Notification_Test()
    {
        var aspNetUser = new AspNetUsers { UserName = "Dave", Email = "dave@example.com" };

        await this.Notifications.SendRegistrationNotificationEmailAsync(aspNetUser, 5);
        this.mail.Sent.Should().BeEmpty("no notification list is configured");

        this.board.BoardSettings.NotificationOnUserRegisterEmailList = "admin1@example.com; admin2@example.com;;";

        await this.Notifications.SendRegistrationNotificationEmailAsync(aspNetUser, 5);

        this.mail.Recipients.Should().Equal("admin1@example.com", "admin2@example.com");
        this.mail.Sent.Should().OnlyContain(
            m => m.TextBody.Contains("User Name: Dave") && m.TextBody.Contains("User Email: dave@example.com"));
    }

    [Test]
    [Description("Spam bot registrations are reported to the admin group")]
    public async Task SpamBot_Notification_Test()
    {
        this.board.Localization.Add("TEMPLATES", "NOTIFICATION_ON_BOT_USER_REGISTER_TEXT", "Bot {user}");
        var aspNetUser = new AspNetUsers { UserName = "Bot", Email = "bot@example.com" };

        await this.Notifications.SendSpamBotNotificationToAdminsAsync(aspNetUser, 9);
        this.mail.Sent.Should().BeEmpty("there is no admin group");

        var adminGroup = this.db.AddForumAccess(this.carol, this.forum, groupFlags: GroupFlags.Flags.IsAdmin);
        await this.Repository<Group>().UpdateOnlyAsync(() => new Group { Name = "Team" }, g => g.ID == adminGroup);

        await this.Notifications.SendSpamBotNotificationToAdminsAsync(aspNetUser, 9);

        this.mail.Recipients.Should().Equal("carol@example.com");
        this.mail.Sent.Single().TextBody.Should().Be("Bot Bot");
    }

    [Test]
    [Description("The welcome notification is a mail, a private message from the host admin, or nothing")]
    public async Task Welcome_Notification_Test()
    {
        this.board.Localization.Add("TEMPLATES", "NOTIFICATION_ON_WELCOME_USER_TEXT", "Welcome {user}!");
        var bobUser = await this.Repository<User>().GetByIdAsync(this.bob);

        this.board.BoardSettings.SendWelcomeNotificationAfterRegister = 0;
        await this.Notifications.SendUserWelcomeNotificationAsync(bobUser);
        this.mail.Sent.Should().BeEmpty();

        this.board.BoardSettings.SendWelcomeNotificationAfterRegister = 1;
        await this.Notifications.SendUserWelcomeNotificationAsync(bobUser);
        this.mail.Sent.Single().TextBody.Should().Be("Welcome Bob!");

        await this.Repository<User>().UpdateOnlyAsync(() => new User { Flags = (int)(UserFlags.Flags.IsApproved | UserFlags.Flags.IsHostAdmin) }, u => u.ID == this.carol);
        this.board.BoardSettings.SendWelcomeNotificationAfterRegister = 2;
        this.board.BoardSettings.AllowPrivateMessages = true;
        await this.Notifications.SendUserWelcomeNotificationAsync(bobUser);

        var pm = await this.Repository<PrivateMessage>().GetSingleAsync(p => p.ToUserId == this.bob);
        pm.FromUserId.Should().Be(this.carol);
        pm.Body.Should().Be("Welcome Bob!");
        this.mail.Sent.Should().ContainSingle("the private message replaces the mail");
    }

    [Test]
    [Description("The verification mail stores a hash of the key and links to the approve page")]
    public async Task Verification_Mail_Test()
    {
        this.board.Localization.Add("TEMPLATES", "VERIFYEMAIL_TEXT", "Hi {username}, key {key}");
        var aspNetUser = new AspNetUsers { UserName = "Bob", Email = "bob@example.com" };
        this.aspNetUsers.GenerateEmailConfirmationResetTokenAsync(aspNetUser).Returns("a+b/c");

        await this.Notifications.SendVerificationEmailAsync(aspNetUser, "new@example.com", this.bob);

        (await this.Repository<CheckEmail>().GetSingleAsync(c => c.UserID == this.bob)).Hash
            .Should().Be(CheckEmailRepositoryExtensions.HashKey("a%2bb%2fc"));
        this.mail.Recipients.Should().Equal("new@example.com");
        this.mail.Sent.Single().TextBody.Should().Be("Hi Bob, key a%2bb%2fc");

        // the key of the link finds the record
        (await this.Repository<CheckEmail>().UpdateAsync("a%2bb%2fc")).UserID.Should().Be(this.bob);
    }

    [Test]
    [Description("A long key (identity tokens with security stamp) fits into the column, old records with the key itself are still found")]
    public async Task Verification_Key_Hash_Test()
    {
        var longKey = new string('x', 400);

        await this.Repository<CheckEmail>().SaveAsync(this.bob, longKey, "bob@example.com");

        var stored = await this.Repository<CheckEmail>().GetSingleAsync(c => c.UserID == this.bob);
        stored.Hash.Length.Should().Be(64);
        (await this.Repository<CheckEmail>().UpdateAsync(longKey)).ID.Should().Be(stored.ID);

        await this.Repository<CheckEmail>().InsertAsync(new CheckEmail { UserID = this.carol, Email = "carol@example.com", Hash = "old-key", Created = DateTime.UtcNow });
        (await this.Repository<CheckEmail>().UpdateAsync("old-key")).UserID.Should().Be(this.carol);

        (await this.Repository<CheckEmail>().UpdateAsync("unknown")).Should().BeNull();
        (await this.Repository<CheckEmail>().UpdateAsync(null)).Should().BeNull();
    }

    [Test]
    [Description("New (e.g. imported) users get their user name and generated password")]
    public async Task Registration_Mail_To_User_Test()
    {
        this.board.Localization.Add(
            "TEMPLATES",
            "NOTIFICATION_ON_REGISTER_TEXT",
            "User Name: {user} Auto Generated Password: {pass} User Email: {email}");
        var aspNetUser = new AspNetUsers { UserName = "Erin", Email = "erin@example.com" };

        await this.Notifications.SendRegistrationNotificationToUserAsync(aspNetUser, "S3cret!", "NOTIFICATION_ON_REGISTER");

        var message = this.mail.Sent.Single();
        this.mail.Recipients.Should().Equal("erin@example.com");
        message.TextBody.Should().Contain("User Name: Erin").And.Contain("Auto Generated Password: S3cret!")
            .And.Contain("User Email: erin@example.com");
    }

    [Test]
    [Description("The password reset mail links to the reset page with the token")]
    public async Task Password_Reset_Mail_Test()
    {
        this.board.Localization.Add("TEMPLATES", "RESET_PASS_TEXT", "Reset at {forumname}: {link}");
        this.board.LinkBuilder
            .GetAbsoluteLink(ForumPages.Account_ResetPassword, Arg.Any<object>())
            .Returns(c => $"https://forum.example/Account/ResetPassword?code={new RouteValueDictionary(c.ArgAt<object>(1))["code"]}");

        await this.Notifications.SendPasswordResetAsync(new AspNetUsers { UserName = "Bob", Email = "bob@example.com" }, "tok3n");

        this.mail.Recipients.Should().Equal("bob@example.com");
        this.mail.Sent.Single().TextBody.Should().Contain("https://forum.example/Account/ResetPassword?code=tok3n");
    }

    private ISendNotification Notifications => this.board.Resolve<ISendNotification>();

    private void GrantReadAccess(params int[] users)
    {
        foreach (var user in users)
        {
            this.db.AddForumAccess(user, this.forum);
        }
    }

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }
}
