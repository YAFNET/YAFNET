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

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Services;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

using Autofac;

using NSubstitute;

using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The task that sends the digest: only when the digest is allowed and due (or forced), to the members with the
/// digest.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DigestSendTaskTests
{
    private TestBoardContext board;

    private IDigest digest;

    private IMailService mail;

    private List<MailMessage> sent;

    [SetUp]
    public void SetUp()
    {
        this.digest = Substitute.For<IDigest>();
        this.mail = Substitute.For<IMailService>();
        this.sent = [];

        this.mail.When(m => m.SendAll(Arg.Any<IEnumerable<MailMessage>>()))
            .Do(c => this.sent.AddRange(c.Arg<IEnumerable<MailMessage>>()));

        this.digest.CreateDigest(Arg.Any<User>(), Arg.Any<MailAddress>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(c => new MailMessage("forum@example.com", c.ArgAt<string>(2)));

        this.board = new TestBoardContext(
            builder =>
                {
                    builder.RegisterInstance(this.digest).As<IDigest>();
                    builder.RegisterInstance(this.mail).As<IMailService>();
                    builder.RegisterType<BoardSettingsService>().AsSelf();
                    builder.RegisterType<CurrentBoardSettings>().AsSelf();
                },
            useDatabase: true);

        this.board.BoardSettings.BoardId = TestDatabase.BoardId;
        this.board.BoardSettings.ForumEmail = "forum@example.com";
        this.board.BoardSettings.Name = "Test Board";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("The digest of the members with the digest is sent when it is due, the last send time is saved")]
    public void Digest_Is_Sent_Test()
    {
        this.board.Database.AddUser("WithDigest", this.WithDigest);
        this.board.Database.AddUser("WithoutDigest");

        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.LastDigestSend = DateTime.Now.AddDays(-2).ToString(CultureInfo.InvariantCulture);

        this.Run();

        this.sent.Select(m => m.To.First().Address).Should().Equal("withdigest@example.com");
        this.board.BoardSettings.LastDigestSend.Should().NotBe(DateTime.Now.AddDays(-2).ToString(CultureInfo.InvariantCulture));
    }

    [Test]
    [Description("Nothing is sent when the digest is not allowed, or sent recently (unless forced)")]
    public void Not_Allowed_Or_Not_Due_Test()
    {
        this.board.Database.AddUser("WithDigest", this.WithDigest);

        this.board.BoardSettings.AllowDigestEmail = false;
        this.Run();
        this.sent.Should().BeEmpty("the digest is not allowed");

        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.LastDigestSend = DateTime.Now.ToString(CultureInfo.InvariantCulture);
        this.Run();
        this.sent.Should().BeEmpty("the digest was sent now");

        this.board.BoardSettings.ForceDigestSend = true;
        this.Run();
        this.sent.Should().HaveCount(1, "the digest is forced");
        this.board.BoardSettings.ForceDigestSend.Should().BeFalse();
    }

    [Test]
    [Description("No member with the digest is logged, an invalid last send time is sent again, an error of a digest is logged")]
    public void No_Members_Invalid_Date_And_Error_Test()
    {
        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.LastDigestSend = "not a date";

        this.Run();
        this.sent.Should().BeEmpty();
        this.board.Logger.Received().Info("no user found");

        // the digest of a member fails
        this.board.Database.AddUser("WithDigest", this.WithDigest);
        this.digest.CreateDigest(Arg.Any<User>(), Arg.Any<MailAddress>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(_ => throw new InvalidOperationException("digest failed"));

        this.board.BoardSettings.ForceDigestSend = true;
        this.Run();

        this.sent.Should().BeEmpty();
        this.board.Logger.Received().Log(
            Arg.Is<string>(s => s.StartsWith("Error In Creating Digest")),
            Arg.Any<EventLogTypes>(),
            Arg.Any<int?>(),
            Arg.Any<string>(),
            Arg.Any<Exception>());
    }

    /// <summary>
    /// A member with the digest (and a login account: members without one are skipped).
    /// </summary>
    private void WithDigest(User user)
    {
        user.DailyDigest = true;
        user.ProviderUserKey = Guid.NewGuid().ToString();
    }

    private void Run()
    {
        new DigestSendTask { ServiceLocator = this.board.Resolve<IServiceLocator>() }.RunOnce();
    }
}
