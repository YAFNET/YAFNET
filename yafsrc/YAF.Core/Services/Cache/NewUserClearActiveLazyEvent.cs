/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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

    /// <summary>
    ///     The attachment event handle file delete.
    /// </summary>
    [ExportService(ServiceLifetimeScope.OwnedByContainer)]
    public class NewUserClearActiveLazyEvent : IHandleEvent<NewUserRegisteredEvent>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewUserClearActiveLazyEvent"/> class.
        /// </summary>
        /// <param name="dataCache">
        /// The data cache.
        /// </param>
        public NewUserClearActiveLazyEvent(IDataCache dataCache)
        {
            this.DataCache = dataCache;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the data cache.
        /// </summary>
        public IDataCache DataCache { get; set; }

        /// <summary>
        ///     Gets the order.
        /// </summary>
        public int Order
        {
            get
            {
                return 10000;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle.
        /// </summary>
        /// <param name="event">
        /// The event.
        /// </param>
        public void Handle(NewUserRegisteredEvent @event)
        {
            this.DataCache.Remove(Constants.Cache.ActiveUserLazyData.FormatWith(@event.UserId));
            this.DataCache.Remove(Constants.Cache.ForumActiveDiscussions);
        }

        #endregion
    }
}