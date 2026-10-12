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

using Microsoft.Extensions.Logging.Abstractions;

using YAF.Core.Extensions;
using YAF.Core.Handlers;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Core.Services.Cache;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The search index task, the message model built from a post row and the localization provider.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SearchTaskAndModelTests
{
    private TestBoardContext board;

    private TestDatabase db;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<BoardSettingsService>().AsSelf();
                b.RegisterType<CurrentBoardSettings>().AsSelf();
                b.RegisterType<TreatCacheKeyWithBoard>().As<ITreatCacheKey>();
            },
            useDatabase: true);

        this.db = this.board.Database;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A forced update indexes the posts of all forums and resets the flag")]
    public async Task Forced_Search_Index_Update_Test()
    {
        var alice = this.db.AddUser("Alice");
        var forum = this.db.AddForum();
        var topic = this.db.AddTopic(forum, alice, "Searchable");
        var first = this.db.AddMessage(topic, alice, "first post");
        var second = this.db.AddMessage(topic, alice, "second post");
        this.board.Current.GetRepository<Forum>().UpdateOnly(() => new Forum { LastPosted = DateTime.UtcNow }, f => f.ID == forum);

        this.board.BoardSettings.ForceUpdateSearchIndex = true;

        await this.NewTask().RunOnceAsync();

        this.board.Search.Indexed.Keys.Should().BeEquivalentTo([first, second]);
        this.board.BoardSettings.ForceUpdateSearchIndex.Should().BeFalse();
        this.board.BoardSettings.LastSearchIndexUpdated.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Description("Without a forced update nothing is indexed outside of the nightly window")]
    public async Task Not_Due_Test()
    {
        this.board.BoardSettings.LastSearchIndexUpdated = DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture);
        this.board.BoardSettings.UpdateSearchIndexEveryXHours = 24;

        await this.NewTask().RunOnceAsync();

        this.board.Search.Indexed.Should().BeEmpty();
    }

    [Test]
    [Description("A message built from a post row has the values of the row")]
    public void Message_From_Row_Test()
    {
        var posted = new DateTime(2026, 3, 1);
        var row = new PagedMessage
        {
            MessageID = 5,
            UserID = 2,
            UserName = "alice",
            Message = "text",
            TopicID = 3,
            Topic = "Thread",
            Posted = posted,
            Edited = posted.AddHours(1),
            EditReason = "typo",
            Flags = 16,
            Position = 4,
            IsModeratorChanged = true,
            DeleteReason = "spam",
            IP = "127.0.0.1",
            Signature = "sig",
            AnswerMessageId = 7
        };

        var message = new Message(row);

        message.ID.Should().Be(5);
        message.UserID.Should().Be(2);
        message.MessageText.Should().Be("text");
        message.TopicName.Should().Be("Thread");
        message.Edited.Should().Be(posted.AddHours(1));
        message.Position.Should().Be(4);
        message.IsModeratorChanged.Should().BeTrue();
        message.DeleteReason.Should().Be("spam");
        message.Signature.Should().Be("sig");
        message.AnswerMessageId.Should().Be(7);
        message.MessageFlags.IsApproved.Should().BeTrue();
    }

    [Test]
    [Description("The localization provider creates the localization once, a new page re-creates it")]
    public void Localization_Provider_Test()
    {
        var provider = new LocalizationProvider { TranslationPage = "TOPICS" };
        var before = 0;
        var after = 0;
        provider.BeforeInit += (_, _) => before++;
        provider.AfterInit += (_, _) => after++;

        var first = provider.Localization;
        provider.Localization.Should().BeSameAs(first);
        first.TransPage.Should().Be("TOPICS");

        provider.TranslationPage = "POSTS";
        provider.Localization.TransPage.Should().Be("POSTS");

        before.Should().Be(2);
        after.Should().Be(2);
    }

    private UpdateSearchIndexTask NewTask()
    {
        return new UpdateSearchIndexTask
        {
            ServiceLocator = this.board.Resolve<IServiceLocator>(),
            Logger = NullLogger<BaseBackgroundTask>.Instance
        };
    }
}
