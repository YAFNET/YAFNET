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

using YAF.Types.Attributes;
using YAF.Types.Models;

/// <summary>
/// Checks the achievements of the author of a published post (the posts and topics are counted).
/// </summary>
[ExportService(ServiceLifetimeScope.OwnedByContainer)]
public class PostPublishedAchievements : IHaveServiceLocator, IHandleEvent<PostPublishedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostPublishedAchievements"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    public PostPublishedAchievements(IServiceLocator serviceLocator)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);

        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets the order (after the post is saved and counted).
    /// </summary>
    public int Order => 10000;

    /// <summary>
    /// Handles the event.
    /// </summary>
    /// <param name="event">The event.</param>
    public void Handle(PostPublishedEvent @event)
    {
        if (!this.Get<BoardSettings>().EnableAchievements)
        {
            return;
        }

        try
        {
            var message = this.GetRepository<Message>().GetById(@event.MessageId);

            if (message is null)
            {
                return;
            }

            this.Get<IAchievementService>().CheckUserAsync(message.UserID).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            // the post is saved, the achievements are checked again on a later page load
            this.Get<ILogger<PostPublishedAchievements>>().Error(ex, "Checking the achievements failed");
        }
    }
}
