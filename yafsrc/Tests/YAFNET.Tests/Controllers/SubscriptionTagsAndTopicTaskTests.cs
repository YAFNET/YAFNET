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

namespace YAF.Tests.Controllers;

using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Tasks;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Push device subscriptions, the tag select list and the prune / linked topic clean up tasks.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SubscriptionTagsAndTopicTaskTests
{
    private const string Chrome = "Mozilla/5.0 (Windows NT 10.0) Chrome/120.0.6099.71";

    private TestBoardContext board;

    private TestDatabase db;

    private VapidConfiguration vapid;

    private ITaskModuleManager tasks;

    private int alice;

    private int forum;

    [SetUp]
    public void SetUp()
    {
        this.vapid = new VapidConfiguration { PublicKey = "public", PrivateKey = "private" };
        this.tasks = Substitute.For<ITaskModuleManager>();

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(this.vapid);
                b.RegisterInstance(this.tasks).As<ITaskModuleManager>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.forum = this.db.AddForum("General");
        this.board.LoginAs(this.alice);
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A device is stored once per user with the version numbers masked, and can be unsubscribed again")]
    public async Task Subscribe_And_Unsubscribe_Device_Test()
    {
        this.board.HttpContext.Request.Headers.UserAgent = Chrome;
        var controller = this.board.CreateController<SubscriptionController>();

        (await controller.SubscribeDevice(this.Subscription(userId: 999))).Should().BeOfType<OkResult>();
        (await controller.SubscribeDevice(this.Subscription())).Should().BeOfType<OkResult>();

        var stored = this.Subscriptions.Get(_ => true).Single();
        stored.UserID.Should().Be(this.alice, "the user is taken from the request, not the body");
        stored.BoardID.Should().Be(TestDatabase.BoardId);
        stored.Device.Should().Be("Mozilla/* (Windows NT *) Chrome/*");
        stored.UserAgent.Should().Be(Chrome);

        (await this.Subscriptions.GetSubscriptionsByUserAsync(this.alice)).Should().ContainSingle();

        (await controller.UnSubscribeDevice(this.Subscription())).Should().BeOfType<OkResult>();
        this.Subscriptions.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Only endpoints of the browser push services are stored (the forum sends requests to them), and not for guests")]
    public async Task Subscribe_Refuses_Other_Endpoints_And_Guests_Test()
    {
        this.board.HttpContext.Request.Headers.UserAgent = Chrome;
        var controller = this.board.CreateController<SubscriptionController>();

        var internalAddress = this.Subscription();
        internalAddress.EndPoint = "http://169.254.169.254/latest/meta-data";
        (await controller.SubscribeDevice(internalAddress)).Should().BeOfType<BadRequestResult>();

        var withoutKeys = this.Subscription();
        withoutKeys.Auth = null;
        (await controller.SubscribeDevice(withoutKeys)).Should().BeOfType<BadRequestResult>();

        this.board.User.IsGuest = true;
        (await controller.SubscribeDevice(this.Subscription())).Should().BeOfType<BadRequestResult>();

        this.Subscriptions.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Devices whose user agent is changed by the html encoding can be unsubscribed too")]
    public async Task Unsubscribe_Encoded_User_Agent_Test()
    {
        this.board.HttpContext.Request.Headers.UserAgent = "Bot <b>&amp; 1.0";
        var controller = this.board.CreateController<SubscriptionController>();

        await controller.SubscribeDevice(this.Subscription());
        this.Subscriptions.Count(_ => true).Should().Be(1);

        await controller.UnSubscribeDevice(this.Subscription());
        this.Subscriptions.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("A long user agent (e.g. of an in-app browser) fits the columns (MySQL creates them with 255 characters, the device is part of the key) and can be unsubscribed")]
    public async Task Subscribe_Long_User_Agent_Test()
    {
        this.board.HttpContext.Request.Headers.UserAgent = $"{Chrome} [FBAN/FBIOS;{new string('x', 400)}&]";
        var controller = this.board.CreateController<SubscriptionController>();

        (await controller.SubscribeDevice(this.Subscription())).Should().BeOfType<OkResult>();

        var stored = this.Subscriptions.Get(_ => true).Single();
        stored.Device.Length.Should().BeLessThanOrEqualTo(255);
        stored.UserAgent.Length.Should().BeLessThanOrEqualTo(255);

        (await controller.UnSubscribeDevice(this.Subscription())).Should().BeOfType<OkResult>();
        this.Subscriptions.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Without push notifications configured the requests are rejected and nothing is stored")]
    public async Task Subscribe_Pwa_Disabled_Test()
    {
        this.vapid.PrivateKey = null;
        this.board.HttpContext.Request.Headers.UserAgent = Chrome;
        var controller = this.board.CreateController<SubscriptionController>();

        (await controller.SubscribeDevice(this.Subscription())).Should().BeOfType<BadRequestResult>();
        (await controller.UnSubscribeDevice(this.Subscription())).Should().BeOfType<BadRequestResult>();

        this.Subscriptions.Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("Tags are searched case-insensitively or paged by 20")]
    public async Task Board_Tags_Test()
    {
        var tags = this.board.Current.GetRepository<Tag>();

        for (var i = 1; i <= 25; i++)
        {
            tags.Insert(new Tag { BoardID = TestDatabase.BoardId, TagName = $"tag{i:00}" });
        }

        tags.Insert(new Tag { BoardID = TestDatabase.BoardId, TagName = "CSharp" });
        tags.Insert(new Tag { BoardID = 2, TagName = "csharp-other-board" });

        var controller = this.board.CreateController<TagsController>();

        var searched = Value(await controller.GetBoardTags(new SearchTopic { SearchTerm = "sharp" }));
        searched.Results.Select(r => r.text).Should().Equal("CSharp");

        var first = Value(await controller.GetBoardTags(new SearchTopic { Page = 0 }));
        first.Results.Should().HaveCount(20);
        first.Total.Should().Be(26);

        var second = Value(await controller.GetBoardTags(new SearchTopic { Page = 1 }));
        second.Results.Should().HaveCount(6);

        var beyond = Value(await controller.GetBoardTags(new SearchTopic { Page = 5 }));
        beyond.Results.Should().BeEmpty();
        beyond.Total.Should().Be(0);
    }

    [Test]
    [Description("The prune task deletes the old topics of the forum, starting it queues the task")]
    public async Task Prune_Topic_Task_Test()
    {
        var old = this.db.AddTopic(this.forum, this.alice);
        this.db.AddMessage(old, this.alice, posted: DateTime.UtcNow.AddDays(-60));
        var recent = this.db.AddTopic(this.forum, this.alice);
        this.db.AddMessage(recent, this.alice, posted: DateTime.UtcNow.AddDays(-1));

        var task = new PruneTopicTask
        {
            ServiceLocator = this.board.Resolve<IServiceLocator>(),
            Logger = NullLogger<BaseBackgroundTask>.Instance,
            Data = TestDatabase.BoardId,
            ForumId = this.forum,
            Days = 30,
            PermDelete = true
        };

        task.TaskName.Should().Be(nameof(PruneTopicTask));
        task.TaskDescription.Should().NotBeNullOrEmpty();

        await task.RunOnceAsync();

        this.Topics.GetById(old).Should().BeNull();
        this.Topics.GetById(recent).Should().NotBeNull();

        PruneTopicTask.Start(TestDatabase.BoardId, this.forum, 30, false).Should().BeTrue();
        this.tasks.Received(1).StartTask(nameof(PruneTopicTask), Arg.Any<Func<IBackgroundTask>>());
    }

    [Test]
    [Description("Expired links of moved topics are removed, active links and normal topics stay")]
    public async Task Linked_Topic_Clean_Up_Task_Test()
    {
        var target = this.db.AddTopic(this.forum, this.alice, "Moved");
        var expired = this.db.AddTopic(this.forum, this.alice, "Expired link", t =>
        {
            t.TopicMovedID = target;
            t.LinkDate = DateTime.UtcNow.AddDays(-1);
        });
        var active = this.db.AddTopic(this.forum, this.alice, "Active link", t =>
        {
            t.TopicMovedID = target;
            t.LinkDate = DateTime.UtcNow.AddDays(3);
        });

        var task = new LinkedTopicCleanUpTask
        {
            ServiceLocator = this.board.Resolve<IServiceLocator>(), Logger = NullLogger<BaseBackgroundTask>.Instance
        };

        task.TaskName.Should().Be(nameof(LinkedTopicCleanUpTask));
        task.TaskDescription.Should().NotBeNullOrEmpty();

        await task.RunOnceAsync();

        this.Topics.GetById(expired).Should().BeNull();
        this.Topics.GetById(active).Should().NotBeNull();
        this.Topics.GetById(target).Should().NotBeNull();
    }

    private IRepository<DeviceSubscription> Subscriptions => this.board.Current.GetRepository<DeviceSubscription>();

    private IRepository<Topic> Topics => this.board.Current.GetRepository<Topic>();

    private DeviceSubscription Subscription(int userId = 0)
    {
        return new DeviceSubscription { UserID = userId, EndPoint = "https://fcm.googleapis.com/fcm/send/1", P256dh = "key", Auth = "auth" };
    }

    private static SelectPagedOptions Value(ActionResult<SearchGridDataSet> result)
    {
        return (SelectPagedOptions)((OkObjectResult)result.Result).Value;
    }
}
