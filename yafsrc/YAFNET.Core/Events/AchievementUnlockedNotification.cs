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

namespace YAF.Core.Events;

using System;

using Microsoft.Extensions.Logging;

using YAF.Core.Hubs;
using YAF.Types.Attributes;

/// <summary>
/// Tells a user who is online about the new achievements (the page shows the dialog).
/// </summary>
[ExportService(ServiceLifetimeScope.OwnedByContainer)]
public class AchievementUnlockedNotification : IHaveServiceLocator, IHandleEvent<AchievementUnlockedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AchievementUnlockedNotification"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public AchievementUnlockedNotification(IServiceLocator serviceLocator)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);

        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets the order.
    /// </summary>
    public int Order => 10000;

    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="event">The event.</param>
    public void Handle(AchievementUnlockedEvent @event)
    {
        var logger = this.Get<ILogger<AchievementUnlockedNotification>>();

        try
        {
            // not awaited: a client that is not reachable must not delay the request
            _ = this.Get<NotificationClient>().SendAchievementUnlockedAsync(@event.UserName).ContinueWith(
                t => logger.Error(t.Exception, "Achievement notification failed"),
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Achievement notification failed");
        }
    }
}
