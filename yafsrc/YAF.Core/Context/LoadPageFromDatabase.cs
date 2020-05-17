/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;
    using System.Web;

    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
            [NotNull] IServiceLocator serviceLocator, ILogger logger, [NotNull] IDataCache dataCache)
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
        public ILogger Logger { get; set; }

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
        /// <exception cref="ApplicationException">Failed to find guest user.</exception>
        /// <exception cref="ApplicationException">Failed to create new user.</exception>
        /// <exception cref="ApplicationException">Unable to find the Guest User!</exception>
        public void Handle([NotNull] InitPageLoadEvent @event)
        {
            try
            {
                object userKey = null;

                if (BoardContext.Current.User != null)
                {
                    userKey = BoardContext.Current.User.Id;
                }

                var tries = 0;
                DataRow pageRow;
                var forumPage = this.Get<HttpRequestBase>().QueryString.ToString();
                var location = this.Get<HttpRequestBase>().FilePath;

                // resources are not handled by ActiveLocation control so far.
                if (location.Contains("resource.ashx"))
                {
                    forumPage = string.Empty;
                    location = string.Empty;
                }

                do
                {
                    pageRow = this.GetRepository<ActiveAccess>().PageLoadAsDataRow(
                        this.Get<HttpSessionStateBase>().SessionID,
                        BoardContext.Current.PageBoardID,
                        userKey,
                        this.Get<HttpRequestBase>().GetUserRealIPAddress(),
                        location,
                        forumPage,
                        (string)@event.Data.Browser,
                        (string)@event.Data.Platform,
                        (int?)@event.Data.CategoryID,
                        (int?)@event.Data.ForumID,
                        (int?)@event.Data.TopicID,
                        (int?)@event.Data.MessageID,
                        (bool)@event.Data.IsSearchEngine,
                        (bool)@event.Data.IsMobileDevice,
                        (bool)@event.Data.DontTrack);
                    
                    // if the user doesn't exist...
                    if (userKey != null && pageRow == null)
                    {
                        // create the user...
                        if (
                            !AspNetRolesHelper.DidCreateForumUser(
                                BoardContext.Current.User, BoardContext.Current.PageBoardID))
                        {
                            throw new ApplicationException("Failed to create new user.");
                        }
                    }

                    if (tries++ < 2)
                    {
                        continue;
                    }

                    if (userKey != null && pageRow == null)
                    {
                        // probably no permissions, use guest user instead...
                        userKey = null;
                        continue;
                    }

                    // fail...
                    break;
                }
                while (pageRow == null && userKey != null);

                if (pageRow == null)
                {
                    throw new ApplicationException("Unable to find the Guest User!");
                }

                // add all loaded page data into our data dictionary...
                @event.DataDictionary.AddRange(pageRow.ToDictionary());

                // clear active users list
                if (@event.DataDictionary["ActiveUpdate"].ToType<bool>())
                {
                    // purge the cache if something has changed...
                    this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
                }
            }
            catch (Exception x)
            {
#if !DEBUG

                // log the exception...
                this.Logger.Fatal(
                    x,
                    "Failure Initializing User/Page (URL: {0}).",
                    this.Get<HttpRequestBase>().Url.ToString());

                // log the user out...
                // FormsAuthentication.SignOut();
                if (BoardContext.Current.ForumPageType != ForumPages.Info)
                {
                    // show a failure notice since something is probably up with membership...
                    BuildLink.RedirectInfoPage(InfoMessage.Failure);
                }
                else
                {
                    // totally failing... just re-throw the exception...
                    throw;
                }

#else
                // re-throw exception...
                throw;
#endif
            }
        }

        #endregion

        #endregion
    }
}