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

using System.Threading.Tasks;

using Autofac;

using ServiceStack.OrmLite;

using YAF.Core.Events;
using YAF.Core.Services;
using YAF.Core.Services.CheckForSpam;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// Spam checks: spam words and links in posts, and the internal bot check (banned e-mails, IPs and names)
/// used when users register.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SpamCheckTests
{
    private TestBoardContext board;

    private ISpamCheck spamCheck;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<SpamCheck>().As<ISpamCheck>();
                b.RegisterType<SpamWordCheck>().As<ISpamWordCheck>();
            },
            useDatabase: true);

        this.spamCheck = this.board.Resolve<ISpamCheck>();

        this.board.BoardSettings.IgnoreSpamWordCheckPostCount = 10;
        this.board.BoardSettings.AllowedNumberOfUrls = 1;
        this.board.BoardSettings.BotSpamServiceType = (int)BotSpamService.NoService;
        this.board.PageUser.NumPosts = 0;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A post with a spam word is spam when a spam service is enabled")]
    public void Post_SpamWord_Test()
    {
        this.Insert(new SpamWords { BoardID = TestDatabase.BoardId, SpamWord = "cheap pills" });
        this.board.BoardSettings.SpamServiceType = (int)SpamService.Internal;

        this.spamCheck.CheckPostForSpam("bot", "1.2.3.4", "buy CHEAP PILLS now", "bot@example.com", out var result)
            .Should().BeTrue();
        result.Should().BeEquivalentTo("cheap pills", "the matched text is returned");

        this.spamCheck.CheckPostForSpam("bot", "1.2.3.4", "a normal post", "bot@example.com", out _).Should().BeFalse();
    }

    [Test]
    [Description("Spam words are not checked without a spam service")]
    public void Post_SpamWord_NoService_Test()
    {
        this.Insert(new SpamWords { BoardID = TestDatabase.BoardId, SpamWord = "cheap pills" });
        this.board.BoardSettings.SpamServiceType = (int)SpamService.NoService;

        this.spamCheck.CheckPostForSpam("bot", "1.2.3.4", "cheap pills", "bot@example.com", out _).Should().BeFalse();
    }

    [Test]
    [Description("New users may only post the allowed number of links")]
    public void Post_Too_Many_Urls_Test()
    {
        const string twoLinks = "see https://a.example.com and https://b.example.com";

        this.spamCheck.CheckPostForSpam("u", "1.2.3.4", "see https://a.example.com", "u@example.com", out _).Should().BeFalse();

        this.spamCheck.CheckPostForSpam("u", "1.2.3.4", twoLinks, "u@example.com", out var result).Should().BeTrue();
        result.Should().Be("The user posted 2 urls but allowed only 1");
    }

    [Test]
    [Description("Experienced users, admins and moderators are not checked")]
    public void Post_Trusted_Users_Test()
    {
        const string twoLinks = "see https://a.example.com and https://b.example.com";

        this.board.PageUser.NumPosts = 10;
        this.spamCheck.CheckPostForSpam("u", "1.2.3.4", twoLinks, "u@example.com", out _).Should().BeFalse();

        this.board.PageUser.NumPosts = 0;
        this.board.PageLoad.IsAdmin = true;
        this.spamCheck.ContainsSpamUrls(twoLinks, out _).Should().BeFalse();

        this.board.PageLoad.IsAdmin = false;
        this.board.PageLoad.ModeratorAccess = true;
        this.spamCheck.ContainsSpamUrls(twoLinks, out _).Should().BeFalse();
    }

    [Test]
    [Description("A banned e-mail (regular expression) is a bot")]
    public async Task Bot_Banned_Email_Test()
    {
        this.Insert(new BannedEmail { BoardID = TestDatabase.BoardId, Mask = @".*@spam\.example$", Since = DateTime.UtcNow });

        var (result, isBot) = await this.spamCheck.CheckUserForSpamBotAsync("someone", "x@spam.example", "1.2.3.4");
        isBot.Should().BeTrue();
        result.Should().Contain("x@spam.example");

        (await this.spamCheck.CheckUserForSpamBotAsync("someone", "x@good.example", "1.2.3.4")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("A banned IP is a bot, also when banned with a wildcard mask")]
    [TestCase("203.0.113.10")]
    [TestCase("203.0.113.*")]
    public async Task Bot_Banned_IP_Test(string mask)
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = mask, Since = DateTime.UtcNow });

        (await this.spamCheck.CheckUserForSpamBotAsync("someone", "a@example.com", "203.0.113.10")).IsBot.Should().BeTrue();
        (await this.spamCheck.CheckUserForSpamBotAsync("someone", "a@example.com", "198.51.100.1")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("A banned name (regular expression) is a bot")]
    public async Task Bot_Banned_Name_Test()
    {
        this.Insert(new BannedName { BoardID = TestDatabase.BoardId, Mask = "^casino", Since = DateTime.UtcNow });

        (await this.spamCheck.CheckUserForSpamBotAsync("casino-king", "a@example.com", "1.2.3.4")).IsBot.Should().BeTrue();
        (await this.spamCheck.CheckUserForSpamBotAsync("i-like-casino", "a@example.com", "1.2.3.4")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("An invalid name mask does not cancel a match on a banned IP")]
    public async Task Bot_Invalid_Name_Mask_Keeps_IP_Match_Test()
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = "203.0.113.10", Since = DateTime.UtcNow });
        this.Insert(new BannedName { BoardID = TestDatabase.BoardId, Mask = "([", Since = DateTime.UtcNow });

        (await this.spamCheck.CheckUserForSpamBotAsync("someone", "a@example.com", "203.0.113.10")).IsBot.Should().BeTrue();
    }

    [Test]
    [Description("Without bans nobody is a bot (no external service configured)")]
    public async Task Bot_Clean_Test()
    {
        (await new InternalCheck().IsBotAsync("1.2.3.4", "a@example.com", "alice")).Should().Be((string.Empty, false));
        (await this.spamCheck.CheckUserForSpamBotAsync("alice", "a@example.com", "1.2.3.4")).IsBot.Should().BeFalse();
    }

    [Test]
    [Description("Auto-banning a bot only bans that exact name and e-mail, not every name containing it")]
    public async Task AutoBan_Is_Exact_Test()
    {
        new BanUser(this.board.Resolve<IServiceLocator>()).Handle(
            new BanUserEvent(1, "ann", "a.b@spam.example", "203.0.113.10"));

        (await new InternalCheck().IsBotAsync("198.51.100.1", "x@example.com", "ann")).IsBot.Should().BeTrue();
        (await new InternalCheck().IsBotAsync("198.51.100.1", "a.b@spam.example", "someone")).IsBot.Should().BeTrue();

        (await new InternalCheck().IsBotAsync("198.51.100.1", "x@example.com", "Joanna")).IsBot
            .Should().BeFalse("banning 'ann' must not ban every name that contains it");
        (await new InternalCheck().IsBotAsync("198.51.100.1", "a1b@spam.example", "someone")).IsBot
            .Should().BeFalse("the dot of an e-mail is not a wildcard");
    }

    [Test]
    [Description("Auto-banning a bot with regex characters in its name still bans it")]
    public async Task AutoBan_Special_Characters_Test()
    {
        new BanUser(this.board.Resolve<IServiceLocator>()).Handle(
            new BanUserEvent(1, "bot(1)+", "bot+1@spam.example", "203.0.113.10"));

        (await new InternalCheck().IsBotAsync("198.51.100.1", "x@example.com", "bot(1)+")).IsBot.Should().BeTrue();
        (await new InternalCheck().IsBotAsync("198.51.100.1", "bot+1@spam.example", "someone")).IsBot.Should().BeTrue();
    }

    private void Insert<T>(T entity)
    {
        using var connection = this.board.Database.Open();
        connection.Insert(entity);
    }
}
