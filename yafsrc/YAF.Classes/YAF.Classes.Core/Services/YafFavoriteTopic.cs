/* YetAnotherForum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
namespace YAF.Classes.Core
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Web;

  using AjaxPro;

  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// The i favorite topic.
  /// </summary>
  public interface IFavoriteTopic
  {
    #region Public Methods

    /// <summary>
    /// The add favorite topic.
    /// </summary>
    /// <param name="topicId">
    /// The topic ID.
    /// </param>
    /// <returns>
    /// The add favorite topic.
    /// </returns>
    [AjaxMethod]
    int AddFavoriteTopic(int topicId);

    /// <summary>
    /// The clear favorite topic cache.
    /// </summary>
    void ClearFavoriteTopicCache();

    /// <summary>
    /// The clear favorite topic cache.
    /// </summary>
    /// <param name="topicId">
    /// The topic Id.
    /// </param>
    /// <returns>
    /// The favorite topic count.
    /// </returns>
    int FavoriteTopicCount(int topicId);

    /// <summary>
    /// the favorite topic details.
    /// </summary>
    /// <param name="sinceDate">
    /// the since date.
    /// </param>
    /// <returns>
    /// a Data table containing all the current user's favorite topics in details.
    /// </returns>
    DataTable FavoriteTopicDetails(DateTime sinceDate);

    /// <summary>
    /// The is favorite topic.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// The is favorite topic.
    /// </returns>
    bool IsFavoriteTopic(int topicID);

    /// <summary>
    /// The remove favorite topic.
    /// </summary>
    /// <param name="topicId">
    /// The favorite topic id.
    /// </param>
    /// <returns>
    /// The remove favorite topic.
    /// </returns>
    [AjaxMethod]
    int RemoveFavoriteTopic(int topicId);

    #endregion
  }

  /// <summary>
  /// Favorite Topic Service for the current user.
  /// </summary>
  public class YafFavoriteTopic : IFavoriteTopic
  {
    #region Constants and Fields

    /// <summary>
    ///   The _favorite Topic list.
    /// </summary>
    private List<int> _favoriteTopicList;

    #endregion

    #region Implemented Interfaces

    #region IFavoriteTopic

    /// <summary>
    /// The add favorite topic.
    /// </summary>
    /// <param name="topicId">
    /// The topic ID.
    /// </param>
    /// <returns>
    /// The add favorite topic.
    /// </returns>
    [AjaxMethod]
    public int AddFavoriteTopic(int topicId)
    {
      DB.topic_favorite_add(YafContext.Current.PageUserID, topicId);
      this.ClearFavoriteTopicCache();

      if (YafContext.Current.CurrentUserData.NotificationSetting == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
      {
        // add to watches...
        this.WatchTopic(YafContext.Current.PageUserID, topicId);
      }

      return topicId;
    }

    /// <summary>
    /// The clear favorite topic cache.
    /// </summary>
    public void ClearFavoriteTopicCache()
    {
      // clear for the session
      string key = YafCache.GetBoardCacheKey(
        Constants.Cache.FavoriteTopicList.FormatWith(YafContext.Current.PageUserID));
      YafContext.Current.Get<HttpSessionStateBase>().Remove(key);
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
      string key = YafCache.GetBoardCacheKey(Constants.Cache.FavoriteTopicCount.FormatWith(topicId));

      return
        YafContext.Current.Cache.GetItem(key, (double)90000, () => DB.TopicFavoriteCount(topicId) as object).ToType<int>
          ();
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
      return DB.topic_favorite_details(
        YafContext.Current.PageBoardID, 
        YafContext.Current.PageUserID, 
        sinceDate, 
        (YafContext.Current.Settings.CategoryID == 0) ? null : (object)YafContext.Current.Settings.CategoryID, 
        YafContext.Current.BoardSettings.UseStyledNicks);
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

      if (this._favoriteTopicList.Count > 0)
      {
        return this._favoriteTopicList.Contains(topicID);
      }

      return false;
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
    [AjaxMethod]
    public int RemoveFavoriteTopic(int topicId)
    {
      DB.topic_favorite_remove(YafContext.Current.PageUserID, topicId);
      this.ClearFavoriteTopicCache();

      if (YafContext.Current.CurrentUserData.NotificationSetting == UserNotificationSetting.TopicsIPostToOrSubscribeTo)
      {
        // no longer watching this topic...
        this.UnwatchTopic(YafContext.Current.PageUserID, topicId);
      }

      return topicId;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// The initialize favorite topic list.
    /// </summary>
    private void InitializeFavoriteTopicList()
    {
      if (this._favoriteTopicList == null)
      {
        this._favoriteTopicList = YafContext.Current.Get<IDBBroker>().FavoriteTopicList(YafContext.Current.PageUserID);
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
    /// </returns>
    private int? TopicWatchedId(int userId, int topicId)
    {
      return DB.watchtopic_check(userId, topicId).GetFirstRowColumnAsValue<int?>("WatchTopicID", null);
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
        DB.watchtopic_delete(watchedId);
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
        DB.watchtopic_add(userId, topicId);
      }
    }

    #endregion
  }
}