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

namespace YAF.Core.Services.Cache
{
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.EventProxies;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;

    /// <summary>
    /// The category event handle cache invalidate.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class CategoryEventHandleCacheInvalidate : IHandleEvent<RepositoryEvent<Category>>
    {
        #region Fields

        /// <summary>
        /// The _data cache.
        /// </summary>
        private readonly IDataCache _dataCache;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryEventHandleCacheInvalidate"/> class.
        /// </summary>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        public CategoryEventHandleCacheInvalidate(IDataCache dataCache)
        {
            this._dataCache = dataCache;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the order.
        /// </summary>
        public int Order => 10000;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(RepositoryEvent<Category> @event)
        {
            if (!@event.RepositoryEventType.IsIn(RepositoryEventType.Delete, RepositoryEventType.Update))
            {
                return;
            }

            // clear moderatorss cache
            this._dataCache.Remove(Constants.Cache.ForumModerators);

            // clear category cache...
            this._dataCache.Remove(Constants.Cache.ForumCategory);

            // clear active discussions cache..
            this._dataCache.Remove(Constants.Cache.ForumActiveDiscussions);
        }

        #endregion
    }
}