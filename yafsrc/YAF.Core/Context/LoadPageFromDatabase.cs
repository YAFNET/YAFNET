/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Core.Context
{
    using System;
    using System.Web;

    using YAF.Core.Helpers;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Interfaces.Services;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    /// <summary>
    /// The load page from database.
    /// </summary>
    [ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
    public class LoadPageFromDatabase : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPageFromDatabase"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="dataCache">The data cache.</param>
        public LoadPageFromDatabase(
            [NotNull] IServiceLocator serviceLocator, ILoggerService logger, [NotNull] IDataCache dataCache)
        {
            this.ServiceLocator = serviceLocator;
            this.Logger = logger;
            this.DataCache = dataCache;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILoggerService Logger { get; set; }

        /// <summary>
        /// Gets or sets DataCache.
        /// </summary>
        public IDataCache DataCache { get; set; }

        /// <summary>
        ///   Gets Order.
        /// </summary>
        public int Order => 1000;

        /// <summary>
        ///   Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        #region Implemented Interfaces

        #region IHandleEvent<InitPageLoadEvent>

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            string userKey = null;

            if (BoardContext.Current.MembershipUser != null)
            {
                userKey = BoardContext.Current.MembershipUser.Id;
            }

            var tries = 0;
            Tuple<PageLoad, User, Category, Forum, Topic> pageRow;

            var location = this.Get<HttpRequestBase>().QueryString.ToString();

            if (location.Length > 250)
            {
                location = location.Truncate(250);
            }

            var forumPage = BoardContext.Current.CurrentForumPage != null
                                ? BoardContext.Current.CurrentForumPage.PageType.ToString()
                                : string.Empty;

            do
            {
                pageRow = this.Get<DataBroker>().GetPageLoad(
                    this.Get<HttpSessionStateBase>().SessionID,
                    BoardContext.Current.PageBoardID,
                    userKey,
                    this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                    location,
                    forumPage,
                    @event.UserRequestData.Browser,
                    @event.UserRequestData.Platform,
                    @event.PageQueryData.CategoryID,
                    @event.PageQueryData.ForumID,
                    @event.PageQueryData.TopicID,
                    @event.PageQueryData.MessageID,
                    @event.UserRequestData.IsSearchEngine,
                    @event.UserRequestData.DontTrack);

                // if the user doesn't exist create the user...
                if (userKey != null && pageRow is null && !this.Get<IAspNetRolesHelper>().DidCreateForumUser(
                    BoardContext.Current.MembershipUser,
                    BoardContext.Current.PageBoardID))
                {
                    throw new ApplicationException("Failed to create new user.");
                }

                if (tries++ < 2)
                {
                    continue;
                }

                if (userKey != null && pageRow is null)
                {
                    // probably no permissions, use guest user instead...
                    userKey = null;
                    continue;
                }

                // fail...
                break;
            }
            while (pageRow is null && userKey != null);

            // add all loaded page data into our data dictionary...
            @event.PageLoadData = pageRow ?? throw new ApplicationException("Unable to find the Guest User!");

            // update Query Data
            @event.PageQueryData.CategoryID = pageRow.Item3?.ID ?? 0;

            if (pageRow.Item4 != null)
            {
                @event.PageQueryData.ForumID = pageRow.Item4.ID;
            }

            if (pageRow.Item5 != null)
            {
                @event.PageQueryData.TopicID = pageRow.Item5.ID;
            }

            // clear active users list
            if (@event.PageLoadData.Item1.ActiveUpdate)
            {
                // purge the cache if something has changed...
                this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
            }
        }

        #endregion

        #endregion
    }
}