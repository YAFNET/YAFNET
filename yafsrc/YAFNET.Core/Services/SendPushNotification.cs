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

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using WebPush;

using YAF.Core.Model;
using YAF.Types.Objects;

namespace YAF.Core.Services;

using System;
using System.Threading.Tasks;

using YAF.Types.Models;

/// <summary>
/// The Send Push Notification Service.
/// </summary>
public class SendPushNotification : IHaveServiceLocator, ISendPushNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SendPushNotification"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public SendPushNotification(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Sends the new topic or reply browser push notification.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="message">The message.</param>
    /// <param name="newTopic">if set to <c>true</c> [new topic].</param>
    /// <param name="subject">The subject.</param>
    public async Task SendTopicPushNotificationAsync(User user, Message message, bool newTopic, string subject)
    {
        // Get the user's push subscription(s)
        var subscriptions = await this.GetRepository<DeviceSubscription>()
            .GetAsync(d => d.BoardID == BoardContext.Current.PageBoardID && d.UserID == user.ID);

        var unreadWatchTopics = this.GetRepository<Activity>().GetUnreadWatchTopics(user.ID);

        if (subscriptions.NullOrEmpty())
        {
            return;
        }

        var icon = newTopic
            ? "data:image/svg+xml,<svg style=\"fill:white\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 640 640\"><!--!Font Awesome Free by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2025 Fonticons, Inc.--><path d=\"M416 208C416 305.2 330 384 224 384C197.3 384 171.9 379 148.8 370L67.2 413.2C57.9 418.1 46.5 416.4 39 409C31.5 401.6 29.8 390.1 34.8 380.8L70.4 313.6C46.3 284.2 32 247.6 32 208C32 110.8 118 32 224 32C330 32 416 110.8 416 208zM416 576C321.9 576 243.6 513.9 227.2 432C347.2 430.5 451.5 345.1 463 229.3C546.3 248.5 608 317.6 608 400C608 439.6 593.7 476.2 569.6 505.6L605.2 572.8C610.1 582.1 608.4 593.5 601 601C593.6 608.5 582.1 610.2 572.8 605.2L491.2 562C468.1 571 442.7 576 416 576z\"/></svg>"
            : "data:image/svg+xml,<svg style=\"fill:white\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 640 640\"><!--!Font Awesome Free by @fontawesome - https://fontawesome.com License - https://fontawesome.com/license/free Copyright 2025 Fonticons, Inc.--><path d=\"M576 304C576 436.5 461.4 544 320 544C282.9 544 247.7 536.6 215.9 523.3L97.5 574.1C88.1 578.1 77.3 575.8 70.4 568.3C63.5 560.8 62 549.8 66.8 540.8L115.6 448.6C83.2 408.3 64 358.3 64 304C64 171.5 178.6 64 320 64C461.4 64 576 171.5 576 304z\"/></svg>";

        var postedUserName = this.Get<IUserDisplayName>().GetNameById(message.UserID);

        var topicLink = this.Get<ILinkBuilder>().GetLink(
            ForumPages.Post,
            new { m = message.ID, name = message.Topic.TopicName });

        var body = this.Get<ILocalization>().GetTextFormatted(newTopic ? "WATCH_FORUM_MSG" : "WATCH_TOPIC_MSG",
            postedUserName, message.Topic.TopicName);

        // create message
        var notification = new PushNotification()
        {
            Title = subject,
            Body = body,
            Image = "/assets/apple-splash-1136-640.webp",
            UnreadCount = unreadWatchTopics,
            Icon = icon,
            Badge = icon,
            Actions = new PushNotificationAction()
            {
                Url = topicLink,
                Action = this.Get<ILocalization>().GetText("TOMESSAGE"),
                Close = this.Get<ILocalization>().GetText("CLOSE_TEXT")
            }
        };

        foreach (var deviceSubscription in subscriptions)
        {
            await this.SendPushNotificationAsync(deviceSubscription, notification);
        }
    }

    /// <summary>
    /// Sends the push notification to the device.
    /// </summary>
    /// <param name="deviceSubscription">The device subscription.</param>
    /// <param name="notification">The notification.</param>
    private async Task SendPushNotificationAsync(DeviceSubscription deviceSubscription, PushNotification notification)
    {
        var subject = $"mailto:{this.Get<BoardSettings>().ForumEmail}";
        var publicKey = this.Get<VapidConfiguration>().PublicKey;
        var privateKey = this.Get<VapidConfiguration>().PrivateKey;

        var vapidDetails = new VapidDetails(subject, publicKey, privateKey);

        var webPushClient = new WebPushClient();

        var subscription = new PushSubscription(
            deviceSubscription.EndPoint,
            deviceSubscription.P256dh,
            deviceSubscription.Auth);

        var payload = JsonConvert.SerializeObject(notification, Formatting.Indented);

        try
        {
            await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails);
        }
        catch (Exception x)
        {
            this.Get<ILogger<SendPushNotification>>().Log(0, this, x, EventLogTypes.Information);
        }
    }
}