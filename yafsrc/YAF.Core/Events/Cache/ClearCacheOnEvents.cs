/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
namespace YAF.Core.Events.Cache
{
    #region Using

    using System;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Types.Structures;

    #endregion

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
        IHandleEvent<RepositoryEvent<Replace_Words>>,
        IHandleEvent<RepositoryEvent<Spam_Words>>,
        IHandleEvent<RepositoryEvent<AccessMask>>,
        IHandleEvent<RepositoryEvent<BBCode>>,
        IHandleEvent<RepositoryEvent<Group>>,
        IHandleEvent<RepositoryEvent<Rank>>,
        IHandleEvent<RepositoryEvent<UserForum>>,
        IHandleEvent<RepositoryEvent<Category>>,
        IHandleEvent<RepositoryEvent<Forum>>,
        IHandleEvent<RepositoryEvent<Message>>,
        IHandleEvent<RepositoryEvent<Topic>>,
        IHandleEvent<RepositoryEvent<UserGroup>>,
        IHandleEvent<UpdateUserPrivateMessageEvent>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClearCacheOnEvents"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="dataCache">
        /// The data Cache.
        /// </param>
        public ClearCacheOnEvents([NotNull] IServiceLocator serviceLocator, [NotNull] IDataCache dataCache)
        {
            CodeContracts.VerifyNotNull(serviceLocator, "serviceLocator");
            CodeContracts.VerifyNotNull(dataCache, "dataCache");

            this.ServiceLocator = serviceLocator;
            this.DataCache = dataCache;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<SuccessfulUserLoginEvent>

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        void IHandleEvent<SuccessfulUserLoginEvent>.Handle([NotNull] SuccessfulUserLoginEvent @event)
        {
            // to clear the cache to show user in the list at once
            this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
            this.DataCache.Remove(Constants.Cache.ActiveDiscussions);
            this.DataCache.Remove(Constants.Cache.BoardUserStats);
        }

        #endregion

        #region IHandleEvent<UpdateUserEvent>

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        void IHandleEvent<UpdateUserEvent>.Handle([NotNull] UpdateUserEvent @event)
        {
            // clear the cache for this user...
            var userId = @event.UserId;
            this.DataCache.Remove(string.Format(Constants.Cache.UserBuddies, userId));

            // update forum moderators cache just in case something was changed...
            this.ClearModeratorsCache();

            var cache = this.DataCache.GetOrSet(
                Constants.Cache.UserSignatureCache,
                () => new MostRecentlyUsed(250),
                TimeSpan.FromMinutes(10));

            // remove from the the signature cache...
            cache.Remove(userId);

            // Clearing cache with old Active User Lazy Data ...
            this.DataCache.Remove(string.Format(Constants.Cache.ActiveUserLazyData, userId));
        }

        #endregion

        #region IHandleEvent<UserLogoutEvent>

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        void IHandleEvent<UserLogoutEvent>.Handle([NotNull] UserLogoutEvent @event)
        {
            // Clearing user cache with permissions data and active users cache...));
            this.DataCache.Remove(string.Format(Constants.Cache.ActiveUserLazyData, @event.UserId));
            this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
        }

        #endregion

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
        public void Handle(RepositoryEvent<Replace_Words> @event)
        {
            this.DataCache.Remove(Constants.Cache.ReplaceWords);
        }

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(RepositoryEvent<Spam_Words> @event)
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
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(UpdateUserPrivateMessageEvent @event)
        {
            this.DataCache.Remove(string.Format(Constants.Cache.ActiveUserLazyData, @event.UserId));
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

        #endregion
    }
}