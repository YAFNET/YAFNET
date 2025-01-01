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

using System;

namespace YAF.Core.Events.Cache;

using YAF.Types.Attributes;
using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The clear cache on events.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerScope)]
public class ClearCacheOnEvents : IHaveServiceLocator,
                                  IHandleEvent<SuccessfulUserLoginEvent>,
                                  IHandleEvent<UpdateUserEvent>,
                                  IHandleEvent<UserLogoutEvent>,
                                  IHandleEvent<RepositoryEvent<User>>,
                                  IHandleEvent<RepositoryEvent<BannedIP>>,
                                  IHandleEvent<RepositoryEvent<BannedUserAgent>>,
                                  IHandleEvent<RepositoryEvent<ReplaceWords>>,
                                  IHandleEvent<RepositoryEvent<SpamWords>>,
                                  IHandleEvent<RepositoryEvent<AccessMask>>,
                                  IHandleEvent<RepositoryEvent<BBCode>>,
                                  IHandleEvent<RepositoryEvent<Group>>,
                                  IHandleEvent<RepositoryEvent<Rank>>,
                                  IHandleEvent<RepositoryEvent<UserForum>>,
                                  IHandleEvent<RepositoryEvent<Category>>,
                                  IHandleEvent<RepositoryEvent<Forum>>,
                                  IHandleEvent<RepositoryEvent<Message>>,
                                  IHandleEvent<RepositoryEvent<Topic>>,
                                  IHandleEvent<RepositoryEvent<UserGroup>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCacheOnEvents"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    /// <param name="dataCache">
    /// The data Cache.
    /// </param>
    public ClearCacheOnEvents(IServiceLocator serviceLocator, IDataCache dataCache)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);
        ArgumentNullException.ThrowIfNull(dataCache);

        this.ServiceLocator = serviceLocator;
        this.DataCache = dataCache;
    }

    /// <summary>
    /// Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 10000;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<SuccessfulUserLoginEvent>.Handle(SuccessfulUserLoginEvent @event)
    {
        // to clear the cache to show user in the list at once
        this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
        this.DataCache.Remove(Constants.Cache.ActiveDiscussions);
        this.DataCache.Remove(Constants.Cache.BoardUserStats);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UpdateUserEvent>.Handle(UpdateUserEvent @event)
    {
        // clear the cache for this user...
        var userId = @event.UserId;

        // update forum moderators cache just in case something was changed...
        this.ClearModeratorsCache();

        // Clearing cache with old Active User Lazy Data ...
        this.DataCache.Remove(string.Format(Constants.Cache.ActiveUserLazyData, userId));
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    void IHandleEvent<UserLogoutEvent>.Handle(UserLogoutEvent @event)
    {
        // Clearing user cache with permissions data and active users cache
        this.DataCache.Remove(string.Format(Constants.Cache.ActiveUserLazyData, @event.UserId));
        this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<User> @event)
    {
        if (!@event.RepositoryEventType.IsIn(RepositoryEventType.Delete))
        {
            return;
        }

        // clear the cache
        this.DataCache.Clear();
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<BannedIP> @event)
    {
        this.DataCache.Remove(Constants.Cache.BannedIP);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<BannedUserAgent> @event)
    {
        this.DataCache.Remove(Constants.Cache.BannedUserAgent);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<ReplaceWords> @event)
    {
        this.DataCache.Remove(Constants.Cache.ReplaceWords);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<SpamWords> @event)
    {
        this.DataCache.Remove(Constants.Cache.SpamWords);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<AccessMask> @event)
    {
        this.ClearModeratorsCache();
        this.ClearAccess();
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<BBCode> @event)
    {
        this.DataCache.Remove(Constants.Cache.CustomBBCode);
        this.Get<IObjectStore>().RemoveOf<IProcessReplaceRules>();
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Group> @event)
    {
        // remove caching in case something got updated...
        this.ClearModeratorsCache();

        // Clearing cache with old permissions data...
        this.DataCache.Remove(
            k => k.StartsWith(string.Format(Constants.Cache.ActiveUserLazyData, string.Empty)));
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Rank> @event)
    {
        // Clearing cache with old permissions data...
        this.DataCache.RemoveOf<object>(
            k => k.Key.StartsWith(string.Format(Constants.Cache.ActiveUserLazyData, string.Empty)));
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<UserForum> @event)
    {
        this.ClearModeratorsCache();

        this.DataCache.Remove(Constants.Cache.BoardModerators);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Category> @event)
    {
        /*if (!@event.RepositoryEventType.IsIn(RepositoryEventType.Delete, RepositoryEventType.Update))
        {
            return;
        }*/

        // clear active discussions cache..
        this.DataCache.Remove(Constants.Cache.ActiveDiscussions);

        this.ClearModeratorsCache();
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Forum> @event)
    {
        // clear active discussions cache..
        this.DataCache.Remove(Constants.Cache.ActiveDiscussions);

        this.DataCache.Remove(
            k => k.StartsWith(string.Format(Constants.Cache.ForumJump, string.Empty)));

        this.ClearModeratorsCache();
        this.ClearAccess();
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Message> @event)
    {
        this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
        this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
        this.Get<IDataCache>().Remove(Constants.Cache.MostActiveUsers);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<Topic> @event)
    {
        this.Get<IDataCache>().Remove(Constants.Cache.BoardStats);
        this.Get<IDataCache>().Remove(Constants.Cache.BoardUserStats);
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(RepositoryEvent<UserGroup> @event)
    {
        if (!@event.RepositoryEventType.IsIn(RepositoryEventType.New))
        {
            return;
        }

        this.ClearAccess();
    }

    /// <summary>
    /// clear moderators cache
    /// </summary>
    private void ClearModeratorsCache()
    {
        this.DataCache.Remove(Constants.Cache.ForumModerators);
    }

    /// <summary>
    /// Empty out access table(s)
    /// </summary>
    private void ClearAccess()
    {
        this.GetRepository<Active>().DeleteAll();
        this.GetRepository<ActiveAccess>().DeleteAll();
    }
}