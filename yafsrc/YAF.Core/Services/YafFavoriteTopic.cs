/* YetAnotherForum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;

    using YAF.Classes.Data;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     Favorite Topic Service for the current user.
    /// </summary>
    public class YafFavoriteTopic : IFavoriteTopic, IHaveServiceLocator
    {
        #region Fields

        /// <summary>
        ///     The _favorite Topic list.
        /// </summary>
        private List<int> _favoriteTopicList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YafFavoriteTopic"/> class.
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
        public YafFavoriteTopic(HttpSessionStateBase sessionState, IServiceLocator serviceLocator, ITreatCacheKey treatCacheKey)
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
            this.GetRepository<FavoriteTopic>().Insert(new FavoriteTopic { UserID = YafContext.Current.PageUserID, TopicID = topicId });
            this.ClearFavoriteTopicCache();

            if (YafContext.Current.CurrentUserData.NotificationSetting == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
            {
                // add to watches...
                this.WatchTopic(YafContext.Current.PageUserID, topicId);
            }

            return topicId;
        }

        /// <summary>
        ///     The clear favorite topic cache.
        /// </summary>
        public void ClearFavoriteTopicCache()
        {
            // clear for the session
            this.SessionState.Remove(
                this.TreatCacheKey.Treat(Constants.Cache.FavoriteTopicList.FormatWith(YafContext.Current.PageUserID)));
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
                    Constants.Cache.FavoriteTopicCount.FormatWith(topicId),
                    () => this.GetRepository<FavoriteTopic>().Count(topicId),
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
        public DataTable FavoriteTopicDetails(DateTime sinceDate)
        {
            return this.GetRepository<FavoriteTopic>().Details(
                (YafContext.Current.Settings.CategoryID == 0) ? null : (int?)YafContext.Current.Settings.CategoryID, 
                YafContext.Current.PageUserID, 
                sinceDate, 
                DateTime.UtcNow, 
                // page index in db is 1 based!
                0, 
                // set the page size here
                1000, 
                YafContext.Current.BoardSettings.UseStyledNicks, 
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
            this.GetRepository<FavoriteTopic>().DeleteByUserAndTopic(YafContext.Current.PageUserID, topicId);
            this.ClearFavoriteTopicCache();

            if (YafContext.Current.CurrentUserData.NotificationSetting == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
            {
                // no longer watching this topic...
                this.UnwatchTopic(YafContext.Current.PageUserID, topicId);
            }

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
                this._favoriteTopicList = YafContext.Current.Get<YafDbBroker>().FavoriteTopicList(YafContext.Current.PageUserID);
            }
        }

        /// <summary>
        /// Returns true if the topic is set to watch for userId
        /// </summary>
        /// <param name="userId">
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// The <see cref="int?"/>.
        /// </returns>
        private int? TopicWatchedId(int userId, int topicId)
        {
            return LegacyDb.watchtopic_check(userId, topicId).GetFirstRowColumnAsValue<int?>("WatchTopicID", null);
        }

        /// <summary>
        /// Checks if this topic is watched, if not, adds it.
        /// </summary>
        /// <param name="userId">
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        private void UnwatchTopic(int userId, int topicId)
        {
            var watchedId = this.TopicWatchedId(userId, topicId);

            if (watchedId.HasValue)
            {
                // subscribe to this forum
                LegacyDb.watchtopic_delete(watchedId);
            }
        }

        /// <summary>
        /// Checks if this topic is watched, if not, adds it.
        /// </summary>
        /// <param name="userId">
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        private void WatchTopic(int userId, int topicId)
        {
            if (!this.TopicWatchedId(userId, topicId).HasValue)
            {
                // subscribe to this forum
                LegacyDb.watchtopic_add(userId, topicId);
            }
        }

        #endregion
    }
}