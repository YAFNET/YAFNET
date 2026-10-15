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

using MimeKit;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Cache;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// <see cref="DigestSendTask"/>: sends the digest of every board that is due to the users who want it.
/// </summary>
[TestFixture]
[NonParallelizable]
public class DigestSendTaskTests
{
    private const int SecondBoard = TestDatabase.BoardId + 1;

    private TestBoardContext board;

    private TestDatabase db;

    private RecordingMailService mail;

    private IDigestService digest;

    [SetUp]
    public void SetUp()
    {
        this.mail = new RecordingMailService();
        this.digest = Substitute.For<IDigestService>();
        this.digest.CreateDigest(Arg.Any<User>(), Arg.Any<MailboxAddress>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(c => new MimeMessage { Subject = $"digest for {c.ArgAt<string>(3)}" });

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.mail).As<IMailService>();
                b.RegisterInstance(this.digest).As<IDigestService>();
                b.RegisterType<BoardSettingsService>().AsSelf();
                b.RegisterType<CurrentBoardSettings>().AsSelf();
                b.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.board.BoardSettings.ForumEmail = "forum@example.com";
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A forced digest is sent to approved members who want it, and the send time is stored")]
    public async Task Send_Test()
    {
        this.EnableDigest();

        this.AddDigestUser("Alice");
        this.db.AddUser("NoDigest", u => u.ProviderUserKey = "k-nodigest");
        this.AddDigestUser("Unapproved", u => u.Flags = 0);

        await this.RunAsync();

        this.mail.Sent.Select(m => m.Subject).Should().Equal("digest for Alice");
        this.board.BoardSettings.ForceDigestSend.Should().BeFalse("a forced send is done once");
        this.board.BoardSettings.LastDigestSend.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Description("Nothing is sent when digests are disabled or not due")]
    public async Task Not_Due_Test()
    {
        this.AddDigestUser("Alice");

        await this.RunAsync();
        this.mail.Sent.Should().BeEmpty("digests are disabled");

        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.LastDigestSend = DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture);

        await this.RunAsync();
        this.mail.Sent.Should().BeEmpty("the last digest was just sent");
    }

    [Test]
    [Description("A user without a provider key is skipped, the other users still get their digest")]
    public async Task User_Without_Key_Test()
    {
        this.EnableDigest();

        this.AddDigestUser("Broken", u => u.ProviderUserKey = null);
        this.AddDigestUser("Alice");

        await this.RunAsync();

        this.mail.Sent.Select(m => m.Subject).Should().Equal("digest for Alice");
    }

    [Test]
    [Description("Only the digest of the own board is sent: an application serves one board (the board id of the configuration), the digest of another board would have the topics, name and links of this board")]
    public async Task Only_Own_Board_Test()
    {
        using (var connection = this.db.Open())
        {
            connection.Insert(new Board { ID = SecondBoard, Name = "Second" });
            connection.Insert(new Registry { BoardID = SecondBoard, Name = "allowdigestemail", Value = "true" });
            connection.Insert(new Registry { BoardID = SecondBoard, Name = "forcedigestsend", Value = "true" });
        }

        this.EnableDigest();

        this.AddDigestUser("Alice");
        this.AddDigestUser("Bob", u => u.BoardID = SecondBoard);

        await this.RunAsync();

        this.mail.Sent.Select(m => m.Subject).Should().Equal("digest for Alice");

        // the digest of the second board stays due for its own application
        (await this.board.Current.GetRepository<Registry>()
            .CountAsync(r => r.BoardID == SecondBoard && r.Name == "lastdigestsend")).Should().Be(0);
    }

    [Test]
    [Description("An unreadable time of the last digest counts as never sent")]
    public async Task Invalid_Last_Send_Test()
    {
        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.LastDigestSend = "not a date";

        this.AddDigestUser("Alice");

        await this.RunAsync();

        this.mail.Sent.Select(m => m.Subject).Should().Equal("digest for Alice");
        DateTime.Parse(this.board.BoardSettings.LastDigestSend, System.Globalization.CultureInfo.InvariantCulture)
            .Should().BeCloseTo(DateTime.Now, TimeSpan.FromMinutes(1));
    }

    [Test]
    [Description("An error in the digest of one user does not stop the digests of the others")]
    public async Task Digest_Error_Of_One_User_Test()
    {
        this.EnableDigest();

        this.digest.CreateDigest(Arg.Is<User>(u => u.Name == "Broken"), Arg.Any<MailboxAddress>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(_ => throw new InvalidOperationException("broken"));

        this.AddDigestUser("Broken");
        this.AddDigestUser("Alice");

        await this.RunAsync();

        this.mail.Sent.Select(m => m.Subject).Should().Equal("digest for Alice");
    }

    [Test]
    [Description("Without users who want a digest nothing is sent, the task has a name and description")]
    public async Task No_Users_Test()
    {
        this.EnableDigest();

        await this.RunAsync();

        this.mail.Sent.Should().BeEmpty();

        var task = new DigestSendTask();
        task.TaskName.Should().Be(nameof(DigestSendTask));
        task.TaskDescription.Should().NotBeNullOrEmpty();
    }

    private void EnableDigest()
    {
        this.board.BoardSettings.AllowDigestEmail = true;
        this.board.BoardSettings.ForceDigestSend = true;
    }

    private void AddDigestUser(string name, Action<User> configure = null)
    {
        this.db.AddUser(
            name,
            u =>
            {
                u.DailyDigest = true;
                u.ProviderUserKey = $"k-{name}";
                configure?.Invoke(u);
            });
    }

    private Task RunAsync()
    {
        var task = new DigestSendTask { ServiceLocator = this.board.Resolve<IServiceLocator>() };

        return task.RunOnceAsync();
    }
}
