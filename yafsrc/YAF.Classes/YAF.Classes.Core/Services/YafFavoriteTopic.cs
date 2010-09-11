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
  /// Favorite Topic Service for the current user.
  /// </summary>
  public class YafFavoriteTopic
  {
    #region Constants and Fields

    /// <summary>
    /// The _favorite Topic list.
    /// </summary>
    private List<int> _favoriteTopicList = null;

    #endregion

    #region Public Methods

    /// <summary>
    /// The add favorite topic.
    /// </summary>
    /// <param name="topicID">
    /// The topic ID.
    /// </param>
    /// <returns>
    /// The add favorite topic.
    /// </returns>
    [AjaxMethod]
    public int AddFavoriteTopic(int topicID)
    {
      DB.topic_favorite_add(YafContext.Current.PageUserID, topicID);
      this.ClearFavoriteTopicCache();
      return topicID;
    }

    /// <summary>
    /// The clear favorite topic cache.
    /// </summary>
    public void ClearFavoriteTopicCache()
    {
      // clear for the session
      string key =
        YafCache.GetBoardCacheKey(Constants.Cache.FavoriteTopicList.FormatWith(YafContext.Current.PageUserID));
      HttpContext.Current.Session.Remove(key);
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
    /// <param name="topicID">
    /// The favorite topic id.
    /// </param>
    /// <returns>
    /// The remove favorite topic.
    /// </returns>
    [AjaxMethod]
    public int RemoveFavoriteTopic(int topicID)
    {
      DB.topic_favorite_remove(YafContext.Current.PageUserID, topicID);
      this.ClearFavoriteTopicCache();
      return topicID;
    }

    #endregion

    #region Methods

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

    #endregion
  }
}