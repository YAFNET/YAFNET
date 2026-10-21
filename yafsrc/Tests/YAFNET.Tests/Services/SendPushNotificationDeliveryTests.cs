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

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using Microsoft.Extensions.Logging;

using NSubstitute;

using ServiceStack.OrmLite;

using WebEssentials.AspNetCore.Pwa.WebPush;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Sending the push notifications of new topics and posts to the push services of the subscribed devices.
/// </summary>
[TestFixture]
[NonParallelizable]
public class SendPushNotificationDeliveryTests
{
    private const string Endpoint = "https://fcm.googleapis.com/fcm/send/device-1";

    private TestBoardContext board;

    private RecordingHandler pushService;

    private ILogger<SendPushNotification> logger;

    private int alice;

    private int topic;

    private int message;

    [SetUp]
    public void SetUp()
    {
        this.pushService = new RecordingHandler();
        this.logger = Substitute.For<ILogger<SendPushNotification>>();

        var clients = Substitute.For<IHttpClientFactory>();
        clients.CreateClient(Arg.Any<string>()).Returns(_ => new HttpClient(this.pushService, false));

        var vapid = VapidHelper.GenerateVapidKeys();

        var displayName = Substitute.For<IUserDisplayName>();
        displayName.GetNameById(Arg.Any<int>()).Returns("Bob");

        var badWords = Substitute.For<IBadWordReplace>();
        badWords.Replace(Arg.Any<string>()).Returns(c => c.Arg<string>());

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(clients).As<IHttpClientFactory>();
                b.RegisterInstance(new VapidConfiguration { PublicKey = vapid.PublicKey, PrivateKey = vapid.PrivateKey });
                b.RegisterInstance(this.logger).As<ILogger<SendPushNotification>>();
                b.RegisterInstance(displayName).As<IUserDisplayName>();
                b.RegisterInstance(badWords).As<IBadWordReplace>();
            },
            useDatabase: true);

        this.board.BoardSettings.ForumEmail = "forum@example.com";
        this.board.Localization.Add("ACCOUNT", "WATCH_TOPIC_MSG", "{0} replied in {1}");

        var db = this.board.Database;
        this.alice = db.AddUser("Alice");
        var bob = db.AddUser("Bob");
        this.topic = db.AddTopic(db.AddForum(), bob, "Push topic");
        this.message = db.AddMessage(this.topic, bob, "the reply");
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Every device of the user gets the notification, encrypted and signed with the VAPID keys")]
    public async Task Send_To_All_Devices_Test()
    {
        this.AddSubscription(this.alice, "Chrome", Endpoint);
        this.AddSubscription(this.alice, "Firefox", "https://updates.push.services.mozilla.com/wpush/v2/device-2");

        await this.SendAsync();


        this.pushService.Requests.Select(r => r.Uri).Should().BeEquivalentTo(Endpoint, "https://updates.push.services.mozilla.com/wpush/v2/device-2");

        var request = this.pushService.Requests[0];
        request.Authorization.Should().StartWith("WebPush ");
        request.Body.Should().NotBeEmpty();
        request.BodyText.Should().NotContain("Push topic", "the payload is encrypted");

        this.Subscriptions().Should().Be(2);
    }

    [Test]
    [Description("The keys are sent by the forum script as standard base64 with padding, they work too")]
    public async Task Send_With_Padded_Keys_Test()
    {
        this.AddSubscription(this.alice, "Chrome", Endpoint, padded: true);

        await this.SendAsync();

        this.pushService.Requests.Should().ContainSingle();
        this.logger.ReceivedCalls().Should().BeEmpty();
    }

    [Test]
    [Description("Nothing is sent to users without subscription or to devices of other users")]
    public async Task No_Subscription_Test()
    {
        var other = this.board.Database.AddUser("Other");
        this.AddSubscription(other, "Chrome", Endpoint);

        await this.SendAsync();

        this.pushService.Requests.Should().BeEmpty();
    }

    [TestCase(HttpStatusCode.Gone)]
    [TestCase(HttpStatusCode.NotFound)]
    [Description("An expired or revoked subscription is deleted, the other devices keep theirs")]
    public async Task Expired_Subscription_Is_Deleted_Test(HttpStatusCode status)
    {
        this.AddSubscription(this.alice, "Chrome", Endpoint);
        this.AddSubscription(this.alice, "Firefox", "https://updates.push.services.mozilla.com/wpush/v2/device-2");

        this.pushService.Status = uri => uri == Endpoint ? status : HttpStatusCode.Created;

        await this.SendAsync();

        this.board.Current.GetRepository<DeviceSubscription>().Get(d => d.UserID == this.alice).Select(d => d.Device)
            .Should().Equal("Firefox");
        this.logger.ReceivedCalls().Should().BeEmpty();
    }

    [Test]
    [Description("Other errors of the push service are logged, the subscription is kept")]
    public async Task Push_Service_Error_Is_Logged_Test()
    {
        this.AddSubscription(this.alice, "Chrome", Endpoint);

        this.pushService.Status = _ => HttpStatusCode.InternalServerError;

        await this.SendAsync();

        this.Subscriptions().Should().Be(1);
        this.logger.ReceivedCalls().Should().NotBeEmpty();
    }

    [Test]
    [Description("Stored subscriptions with an address that is not a push service are deleted without a request")]
    public async Task Not_Allowed_Endpoint_Test()
    {
        this.AddSubscription(this.alice, "Chrome", "https://intranet.example.com/push");
        this.AddSubscription(this.alice, "Firefox", "http://fcm.googleapis.com/fcm/send/device-2");

        await this.SendAsync();

        this.pushService.Requests.Should().BeEmpty();
        this.Subscriptions().Should().Be(0);
    }

    [Test]
    [Description("The notification of a new topic has another icon than the one of a reply")]
    public void Topic_Notification_Icon_Test()
    {
        var push = new SendPushNotification(this.board.Resolve<IServiceLocator>());
        var post = new Message { UserID = this.alice, Topic = new Topic { TopicName = "Push topic" } };

        var reply = push.GetTopicNotification(post, false, "Subject", "https://forum/Post/1", null);
        var newTopic = push.GetTopicNotification(post, true, "Subject", "https://forum/Post/1", null);

        reply.Icon.Should().StartWith("data:image/svg+xml,").And.Be(reply.Badge);
        newTopic.Icon.Should().StartWith("data:image/svg+xml,").And.NotBe(reply.Icon);
    }

    private Task SendAsync()
    {
        var push = new SendPushNotification(this.board.Resolve<IServiceLocator>());

        var user = this.board.Current.GetRepository<User>().GetById(this.alice);
        var post = this.board.Current.GetRepository<Message>().GetById(this.message);
        post.Topic = this.board.Current.GetRepository<Topic>().GetById(this.topic);

        return push.SendTopicPushNotificationAsync(user, post, false, "Push topic", "https://forum/Post/1", null);
    }

    private void AddSubscription(int userId, string device, string endpoint, bool padded = false)
    {
        var keys = VapidHelper.GenerateVapidKeys();
        var secret = Guid.NewGuid().ToByteArray();

        this.board.Current.GetRepository<DeviceSubscription>().Insert(
            new DeviceSubscription
            {
                BoardID = TestDatabase.BoardId,
                UserID = userId,
                Device = device,
                UserAgent = device,
                EndPoint = endpoint,

                // a public key of the P-256 curve, like the browser sends (url safe base64 without padding)
                P256dh = padded ? ToBase64(keys.PublicKey) : keys.PublicKey,
                Auth = padded
                           ? Convert.ToBase64String(secret)
                           : Convert.ToBase64String(secret).TrimEnd('=').Replace('+', '-').Replace('/', '_')
            });
    }

    private static string ToBase64(string urlBase64)
    {
        var base64 = urlBase64.Replace('-', '+').Replace('_', '/');

        return base64.PadRight(base64.Length + ((4 - (base64.Length % 4)) % 4), '=');
    }

    private long Subscriptions()
    {
        return this.board.Current.GetRepository<DeviceSubscription>().Count(d => d.UserID == this.alice);
    }

    /// <summary>
    /// The push service: records the requests and answers with the status.
    /// </summary>
    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<(string Uri, string Authorization, byte[] Body, string BodyText)> Requests { get; } = [];

        public Func<string, HttpStatusCode> Status { get; set; } = _ => HttpStatusCode.Created;

        protected async override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uri = request.RequestUri!.ToString();
            var body = request.Content is null ? [] : await request.Content.ReadAsByteArrayAsync(cancellationToken);

            this.Requests.Add(
                (uri, request.Headers.Authorization?.ToString(), body, System.Text.Encoding.UTF8.GetString(body)));

            return new HttpResponseMessage(this.Status(uri)) { RequestMessage = request };
        }
    }
}
