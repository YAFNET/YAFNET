namespace YAF.Classes.Core
{
    using AjaxPro;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web;
    using Data;

    /// <summary>
    /// Favorite Topic Service for the current user.
    /// </summary>
    public class YafFavoriteTopic
    {
        /// <summary>
        /// The _favorite Topic list.
        /// </summary>
        private List<int> _favoriteTopicList = null;

        /// <summary>
        /// The initialize favorite topic list.
        /// </summary>
        private void InitializeFavoriteTopicList()
        {
            if (this._favoriteTopicList == null)
            {
                this._favoriteTopicList = YafServices.DBBroker.FavoriteTopicList(YafContext.Current.PageUserID);
            }
        }

        /// <summary>
        /// The clear favorite topic cache.
        /// </summary>
        public void ClearFavoriteTopicCache()
        {
            // clear for the session
            string key = YafCache.GetBoardCacheKey(String.Format(Constants.Cache.FavoriteTopicList, YafContext.Current.PageUserID));
            HttpContext.Current.Session.Remove(key);
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
            InitializeFavoriteTopicList();

            if (this._favoriteTopicList.Count > 0)
            {
                return this._favoriteTopicList.Contains(topicID);
            }
            return false;
        }

        /// <summary>
        /// The add favorite topic.
        /// </summary>
        /// <param name="ignoredUserId">
        /// The favorite topic id.
        /// </param>
        [AjaxMethod]
        public int AddFavoriteTopic(int topicID)
        {
            DB.topic_favorite_add(YafContext.Current.PageUserID, topicID);
            ClearFavoriteTopicCache();
            return topicID;
        }

        /// <summary>
        /// The remove favorite topic.
        /// </summary>
        /// <param name="topicID">
        /// The favorite topic id.
        /// </param>
        [AjaxMethod]
        public int RemoveFavoriteTopic(int topicID)
        {
            DB.topic_favorite_remove(YafContext.Current.PageUserID, topicID);
            ClearFavoriteTopicCache();
            return topicID;
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
            return DB.topic_favorite_details(YafContext.Current.PageBoardID,
                YafContext.Current.PageUserID, sinceDate,
                (YafContext.Current.Settings.CategoryID == 0) ? null : (object)YafContext.Current.Settings.CategoryID,
                YafContext.Current.BoardSettings.UseStyledNicks);
        }
    }
}
