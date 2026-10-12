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

using Autofac;

using Microsoft.AspNetCore.Hosting;

using NSubstitute;

using YAF.Configuration;
using YAF.Core.Events.Cache;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.EventProxies;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

using Constants = YAF.Types.Constants.Constants;

/// <summary>
/// Each cache handler of <see cref="ClearCacheOnEvents"/> drops the cached data it depends on, and <see cref="BoardInfo"/>.
/// </summary>
[TestFixture]
[NonParallelizable]
public class CacheHandlersAndBoardInfoTests
{
    private TestBoardContext board;

    private ClearCacheOnEvents handlers;

    private IWebHostEnvironment environment;

    [SetUp]
    public void SetUp()
    {
        this.environment = Substitute.For<IWebHostEnvironment>();
        this.environment.WebRootPath.Returns("/var/www/forum");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<ClearCacheOnEvents>().AsSelf();
                b.RegisterType<BoardInfo>().AsSelf();
                b.RegisterInstance(this.environment).As<IWebHostEnvironment>();
            },
            useDatabase: true);

        this.handlers = this.board.Resolve<ClearCacheOnEvents>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Login, logout and user updates drop the online list, the statistics and the user's cached data")]
    public void User_Events_Test()
    {
        var lazy = string.Format(Constants.Cache.ActiveUserLazyData, 5);
        var other = string.Format(Constants.Cache.ActiveUserLazyData, 6);

        this.Fill(Constants.Cache.UsersOnlineStatus, Constants.Cache.ActiveDiscussions, Constants.Cache.BoardUserStats, lazy, other);
        ((IHandleEvent<SuccessfulUserLoginEvent>)this.handlers).Handle(new SuccessfulUserLoginEvent(5));
        this.Cached().Should().BeEquivalentTo([lazy, other]);

        this.Fill(Constants.Cache.ForumModerators, Constants.Cache.UsersOnlineStatus);
        ((IHandleEvent<UpdateUserEvent>)this.handlers).Handle(new UpdateUserEvent(5));
        this.Cached().Should().BeEquivalentTo([Constants.Cache.UsersOnlineStatus, other]);

        this.Fill(lazy);
        ((IHandleEvent<UserLogoutEvent>)this.handlers).Handle(new UserLogoutEvent(5));
        this.Cached().Should().BeEquivalentTo([other]);
    }

    [Test]
    [Description("Deleting a user clears the whole cache, other user changes do not")]
    public void User_Repository_Test()
    {
        this.Fill("a", "b");

        this.handlers.Handle(new RepositoryEvent<User>(RepositoryEventType.Update));
        this.Cached().Should().HaveCount(2);

        this.handlers.Handle(new RepositoryEvent<User>(RepositoryEventType.Delete));
        this.Cached().Should().BeEmpty();
    }

    [Test]
    [Description("Changes of the ban lists, replace and spam words and custom BBCodes drop their cached lists")]
    public void Lists_Test()
    {
        this.Fill(
            Constants.Cache.BannedIP,
            Constants.Cache.BannedUserAgent,
            Constants.Cache.BannedCountry,
            Constants.Cache.ReplaceWords,
            Constants.Cache.SpamWords,
            Constants.Cache.CustomBBCode,
            Constants.Cache.CustomBBCodeRegExDictionary);

        this.handlers.Handle(new RepositoryEvent<BannedIP>(RepositoryEventType.New));
        this.handlers.Handle(new RepositoryEvent<BannedUserAgent>(RepositoryEventType.New));
        this.handlers.Handle(new RepositoryEvent<BannedCountry>(RepositoryEventType.New));
        this.handlers.Handle(new RepositoryEvent<ReplaceWords>(RepositoryEventType.Update));
        this.handlers.Handle(new RepositoryEvent<SpamWords>(RepositoryEventType.Delete));
        this.handlers.Handle(new RepositoryEvent<BBCode>(RepositoryEventType.Update));

        this.Cached().Should().BeEmpty();
    }

    [Test]
    [Description("Changes of the custom profile definitions drop the cached custom profile data of all users")]
    public void Profile_Definition_Test()
    {
        var other = Constants.Cache.SpamWords;

        this.Fill(
            string.Format(Constants.Cache.UserCustomProfileData, 1),
            string.Format(Constants.Cache.UserCustomProfileData, 25),
            other);

        this.handlers.Handle(new RepositoryEvent<ProfileDefinition>(RepositoryEventType.Delete));

        this.Cached().Should().BeEquivalentTo([other]);
    }

    [Test]
    [Description("Forum structure and statistics: categories, forums, topics, messages and ranks drop what depends on them")]
    public void Structure_And_Stats_Test()
    {
        var jump = string.Format(Constants.Cache.ForumJump, 3);
        var lazy = string.Format(Constants.Cache.ActiveUserLazyData, 3);

        this.Fill(Constants.Cache.ActiveDiscussions, Constants.Cache.ForumModerators);
        this.handlers.Handle(new RepositoryEvent<Category>(RepositoryEventType.Update));
        this.Cached().Should().BeEmpty();

        this.Fill(Constants.Cache.ActiveDiscussions, jump, Constants.Cache.ForumModerators);
        this.handlers.Handle(new RepositoryEvent<Forum>(RepositoryEventType.Update));
        this.Cached().Should().BeEmpty();

        this.Fill(Constants.Cache.BoardStats, Constants.Cache.BoardUserStats, Constants.Cache.MostActiveUsers);
        this.handlers.Handle(new RepositoryEvent<Message>(RepositoryEventType.New));
        this.Cached().Should().BeEmpty();

        this.Fill(Constants.Cache.BoardStats, Constants.Cache.BoardUserStats);
        this.handlers.Handle(new RepositoryEvent<Topic>(RepositoryEventType.New));
        this.Cached().Should().BeEmpty();

        this.Fill(lazy, Constants.Cache.BoardStats);
        this.handlers.Handle(new RepositoryEvent<Rank>(RepositoryEventType.Update));
        this.Cached().Should().Equal(Constants.Cache.BoardStats);
    }

    [Test]
    [Description("Access changes (masks, groups, memberships, forum and user access) drop the cached access rights")]
    public void Access_Test()
    {
        var user = this.board.Database.AddUser("Alice");
        var forum = this.board.Database.AddForum();

        void Check(Action raise)
        {
            this.board.GrantAccess(user, forum);
            this.board.RefreshActiveAccess(user);
            this.Fill(Constants.Cache.ForumModerators, string.Format(Constants.Cache.ActiveUserLazyData, user));

            raise();

            this.board.Current.GetRepository<ActiveAccess>().Count(a => a.UserID == user).Should().Be(0);
        }

        Check(() => this.handlers.Handle(new RepositoryEvent<AccessMask>(RepositoryEventType.Update)));
        Check(() => this.handlers.Handle(new RepositoryEvent<Group>(RepositoryEventType.Update)));
        this.Cached().Should().BeEmpty("the group flags are part of the user data and the moderators");

        Check(() => this.handlers.Handle(new RepositoryEvent<UserForum>(RepositoryEventType.New)));
        Check(() => this.handlers.Handle(new RepositoryEvent<UserGroup>(RepositoryEventType.Delete)));
        Check(() => this.handlers.Handle(new RepositoryEvent<ForumAccess>(RepositoryEventType.Update)));
        this.Cached().Should().NotContain(
            Constants.Cache.ForumModerators,
            "the access mask of a group in a forum decides who moderates the forum");
    }

    [Test]
    [Description("The version name contains the version and, for pre-releases, the build date")]
    public void Version_Test()
    {
        var info = this.board.Resolve<BoardInfo>();

        info.AppVersionName.Should().StartWith("4.0.10 BETA ").And.EndWith(info.AppVersionDate.ToString("yyyyMMddHHmm"));
        info.AppVersion.Should().BeGreaterThan(0);
    }

    [Test]
    [Description("Theme and css urls, the base url without a trailing slash and the web root")]
    public void Urls_Test()
    {
        var info = this.board.Resolve<BoardInfo>();

        info.GetUrlToContentThemes("yaf/bootstrap.css").Should().Be("/css/themes/yaf/bootstrap.css");
        info.GetUrlToCss("forum.css").Should().Be("~/css/forum.css");
        info.Invoking(i => i.GetUrlToCss(null)).Should().Throw<ArgumentNullException>();
        info.Invoking(i => i.GetUrlToContentThemes(null)).Should().Throw<ArgumentNullException>();

        info.ForumBaseUrl.Should().Be("https://forum.example.com");

        var previous = Config.WebRootPath;

        try
        {
            Config.WebRootPath = null;
            info.WebRootPath.Should().Be("/var/www/forum");

            Config.WebRootPath = "/custom";
            info.WebRootPath.Should().Be("/custom");
        }
        finally
        {
            Config.WebRootPath = previous;
        }
    }

    private void Fill(params string[] keys)
    {
        foreach (var key in keys)
        {
            this.board.Cache.Set(key, new object());
        }
    }

    private List<string> Cached()
    {
        return [.. this.board.Cache.GetAll<object>().Select(e => e.Key)];
    }
}
