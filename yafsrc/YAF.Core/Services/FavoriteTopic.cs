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

    using System.Linq;
    using System.Web;

    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     Favorite Topic Service for the current user.
    /// </summary>
    public class FavoriteTopic : IFavoriteTopic, IHaveServiceLocator
    {
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
        public FavoriteTopic(
            HttpSessionStateBase sessionState,
            IServiceLocator serviceLocator,
            ITreatCacheKey treatCacheKey)
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
        /// Adds Topic to the Favorite Topics 
        /// </summary>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int AddFavoriteTopic(int topicId)
        {
           var newTopicId = this.GetRepository<YAF.Types.Models.FavoriteTopic>().Insert(
                new YAF.Types.Models.FavoriteTopic { UserID = BoardContext.Current.PageUserID, TopicID = topicId });
            this.ClearFavoriteTopicCache();

            return newTopicId;
        }

        /// <summary>
        /// Checks if Topic is Favorite Topic
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsFavoriteTopic(int topicID)
        {
            var list = this.Get<DataBroker>().FavoriteTopicList(BoardContext.Current.PageUserID);

            return list.Any() && list.Contains(topicID);
        }

        /// <summary>
        /// Removes the Favorite Topic
        /// </summary>
        /// <param name="topicId">
        /// The favorite topic id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int RemoveFavoriteTopic(int topicId)
        {
            this.GetRepository<YAF.Types.Models.FavoriteTopic>()
                .DeleteByUserAndTopic(BoardContext.Current.PageUserID, topicId);
            this.ClearFavoriteTopicCache();

            return topicId;
        }

        /// <summary>
        ///     Clears the Favorite Topic Cache
        /// </summary>
        private void ClearFavoriteTopicCache()
        {
            // clear for the session
            this.SessionState.Remove(
                this.TreatCacheKey.Treat(
                    string.Format(Constants.Cache.FavoriteTopicList, BoardContext.Current.PageUserID)));
        }

        #endregion
    }
}