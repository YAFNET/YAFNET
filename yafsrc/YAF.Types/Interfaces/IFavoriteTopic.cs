/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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
namespace YAF.Types.Interfaces
{
  using System;
  using System.Data;

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
    int RemoveFavoriteTopic(int topicId);

    #endregion
  }
}