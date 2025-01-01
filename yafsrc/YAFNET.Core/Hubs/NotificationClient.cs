/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using Microsoft.AspNetCore.SignalR;

using System.Threading.Tasks;

namespace YAF.Core.Hubs;

/// <summary>
/// Class NotificationClient.
/// </summary>
public class NotificationClient
{
    /// <summary>
    /// The notification hub
    /// </summary>
    private readonly IHubContext<NotificationHub, INotificationClient> notificationHub;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationClient"/> class.
    /// </summary>
    /// <param name="hub">The hub.</param>
    public NotificationClient(IHubContext<NotificationHub, INotificationClient> hub)
    {
        this.notificationHub = hub;
    }

    /// <summary>
    /// Sends the activity asynchronous.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="alerts">The alerts.</param>
    /// <returns>Task.</returns>
    public Task SendActivityAsync(string userName, int alerts)
    {
        return this.notificationHub
            .Clients
            .User(userName)
            .NewActivityAsync(alerts);
    }
}