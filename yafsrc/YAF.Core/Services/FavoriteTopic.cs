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
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;

    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    ///     Favorite Topic Service for the current user.
    /// </summary>
    public class FavoriteTopic : IFavoriteTopic, IHaveServiceLocator
    {
        #region Fields

        /// <summary>
        ///     The _favorite Topic list.
        /// </summary>
        private List<int> _favoriteTopicList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteTopic"/> class.
        /// </summary>
        /// <param name="sessionState">
        /// The session state.
        /// </param>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        /// <param name="treatCacheKey">
        /// The treat cache key.
        /// </param>
        public FavoriteTopic(HttpSessionStateBase sessionState, IServiceLocator serviceLocator, ITreatCacheKey treatCacheKey)
        {
            this.SessionState = sessionState;
            this.ServiceLocator = serviceLocator;
            this.TreatCacheKey = treatCacheKey;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the service locator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        /// <summary>
        /// Gets or sets the session state.
        /// </summary>
        public HttpSessionStateBase SessionState { get; set; }

        /// <summary>
        /// Gets or sets the treat cache key.
        /// </summary>
        public ITreatCacheKey TreatCacheKey { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// The add favorite topic.
        /// </returns>
        public int AddFavoriteTopic(int topicId)
        {
            this.GetRepository<YAF.Types.Models.FavoriteTopic>().Insert(new YAF.Types.Models.FavoriteTopic { UserID = BoardContext.Current.PageUserID, TopicID = topicId });
            this.ClearFavoriteTopicCache();

            return topicId;
        }

        /// <summary>
        ///     The clear favorite topic cache.
        /// </summary>
        public void ClearFavoriteTopicCache()
        {
            // clear for the session
            this.SessionState.Remove(
                this.TreatCacheKey.Treat(string.Format(Constants.Cache.FavoriteTopicList, BoardContext.Current.PageUserID)));
        }

        /// <summary>
        /// The clear favorite topic cache.
        /// </summary>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// The favorite topic count.
        /// </returns>
        public int FavoriteTopicCount(int topicId)
        {
            return
                this.Get<IDataCache>().GetOrSet(
                    string.Format(Constants.Cache.FavoriteTopicCount, topicId),
                    () => this.GetRepository<YAF.Types.Models.FavoriteTopic>().Count(topicId),
                    TimeSpan.FromMilliseconds(90000)).ToType<int>();
        }

        /// <summary>
        /// the favorite topic details.
        /// </summary>
        /// <param name="sinceDate">
        /// the since date.
        /// </param>
        /// <returns>
        /// a Data table containing all the current user's favorite topics in details.
        /// </returns>
        public DataTable FavoriteTopicDetails(System.DateTime sinceDate)
        {
            return this.GetRepository<YAF.Types.Models.FavoriteTopic>().Details(
                BoardContext.Current.Settings.CategoryID == 0 ? null : (int?)BoardContext.Current.Settings.CategoryID,
                BoardContext.Current.PageUserID, 
                sinceDate,
                System.DateTime.UtcNow, 
                // page index in db is 1 based!
                0, 
                // set the page size here
                1000,
                BoardContext.Current.BoardSettings.UseStyledNicks, 
                false);
        }

        /// <summary>
        /// The is favorite topic.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The is favorite topic.
        /// </returns>
        public bool IsFavoriteTopic(int topicID)
        {
            this.InitializeFavoriteTopicList();

            return this._favoriteTopicList.Count > 0 && this._favoriteTopicList.Contains(topicID);
        }

        /// <summary>
        /// The remove favorite topic.
        /// </summary>
        /// <param name="topicId">
        /// The favorite topic id.
        /// </param>
        /// <returns>
        /// The remove favorite topic.
        /// </returns>
        public int RemoveFavoriteTopic(int topicId)
        {
            this.GetRepository<YAF.Types.Models.FavoriteTopic>().DeleteByUserAndTopic(BoardContext.Current.PageUserID, topicId);
            this.ClearFavoriteTopicCache();

            return topicId;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The initialize favorite topic list.
        /// </summary>
        private void InitializeFavoriteTopicList()
        {
            if (this._favoriteTopicList == null)
            {
                this._favoriteTopicList = BoardContext.Current.Get<DataBroker>().FavoriteTopicList(BoardContext.Current.PageUserID);
            }
        }

        #endregion
    }
}