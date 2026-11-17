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

using System;

namespace YAF.Core.Events;

using Microsoft.Extensions.Logging;

using YAF.Core.Hubs;
using YAF.Types.Attributes;
using YAF.Types.Objects;

/// <summary>
/// Sends the real-time update of a published post to the clients that can read the forum.
/// </summary>
[ExportService(ServiceLifetimeScope.OwnedByContainer)]
public class PostPublishedNotification : IHaveServiceLocator, IHandleEvent<PostPublishedEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostPublishedNotification"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public PostPublishedNotification(IServiceLocator serviceLocator)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);

        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    ///     Gets the order.
    /// </summary>
    public int Order => 10000;

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    ///     The event.
    /// </param>
    public void Handle(PostPublishedEvent @event)
    {
        if (!this.Get<BoardSettings>().EnableLiveUpdates)
        {
            return;
        }

        // the guests load the active discussions from the cache, it has to contain the new post
        this.Get<IDataCache>().Remove(Constants.Cache.ActiveDiscussions);

        var logger = this.Get<ILogger<PostPublishedNotification>>();

        try
        {
            // not awaited: the post is saved, a client that is not reachable must not delay (or fail) it
            _ = this.Get<NotificationClient>().SendPostPublishedAsync(
                new PostPublishedInfo
                {
                    ForumId = @event.ForumId,
                    TopicId = @event.TopicId,
                    MessageId = @event.MessageId,
                    IsNewTopic = @event.IsNewTopic
                }).ContinueWith(
                t => logger.Error(t.Exception, "Real-time update failed"),
                System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Real-time update failed");
        }
    }
}
