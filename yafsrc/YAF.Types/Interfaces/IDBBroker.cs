/* Yet Another Forum.NET
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
namespace YAF.Types.Interfaces
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Data;

  using YAF.Types.Objects;

  #endregion

  /// <summary>
  /// The idb broker.
  /// </summary>
  public interface IDBBroker
  {
    #region Public Methods

    IEnumerable<TypedBBCode> GetCustomBBCode();

    IEnumerable<DataRow> GetShoutBoxMessages(int boardId);

    /// <summary>
    /// Get the list of recently logged in users.
    /// </summary>
    /// <param name="timeSinceLastLogin">Time since last login in minutes.</param>
    /// <returns>The DataTable of the users.</returns>
    DataTable GetRecentUsers(int timeSinceLastLogin);

    /// <summary>
    /// The user lazy data.
    /// </summary>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// </returns>
    DataRow ActiveUserLazyData([NotNull] int userId);

    /// <summary>
    /// Adds the Thanks info to a dataTable
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    void AddThanksInfo([NotNull] IEnumerable<DataRow> dataRows);

    /// <summary>
    /// Returns the layout of the board
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// UserID
    /// </param>
    /// <param name="categoryID">
    /// CategoryID
    /// </param>
    /// <param name="parentID">
    /// ParentID
    /// </param>
    /// <returns>
    /// Returns board layout
    /// </returns>
    DataSet BoardLayout(int boardID, int userID, int? categoryID, int? parentID);

    /// <summary>
    /// The favorite topic list.
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <returns>
    /// </returns>
    List<int> FavoriteTopicList(int userID);

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <param name="crawlers">
    /// The crawlers.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetActiveList(bool guests, bool crawlers);

    /// <summary>
    /// The get active list.
    /// </summary>
    /// <param name="activeTime">
    /// The active time.
    /// </param>
    /// <param name="guests">
    /// The guests.
    /// </param>
    /// <param name="crawlers">
    /// The crawlers.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetActiveList(int activeTime, bool guests, bool crawlers);

    /// <summary>
    /// Get all moderators by Groups and User
    /// </summary>
    /// <returns>
    /// Returns the Moderator List
    /// </returns>
    List<SimpleModerator> GetAllModerators();

    /// <summary>
    /// Get all moderators without Groups
    /// </summary>
    /// <returns>
    /// Returns the Moderator List
    /// </returns>
    List<SimpleModerator> GetAllModeratorsTeam();

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts);

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts, int userId);

    /// <summary>
    /// The get latest topics.
    /// </summary>
    /// <param name="numberOfPosts">
    /// The number of posts.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="styleColumnNames">
    /// The style Column Names.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable GetLatestTopics(int numberOfPosts, int userId, [NotNull] params string[] styleColumnNames);

    /// <summary>
    /// The get moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    DataTable GetModerators();

    /// <summary>
    /// Get a simple forum/topic listing.
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <param name="timeFrame">
    /// The time Frame.
    /// </param>
    /// <param name="maxCount">
    /// The max Count.
    /// </param>
    /// <returns>
    /// </returns>
    List<SimpleForum> GetSimpleForumTopic(int boardId, int userId, DateTime timeFrame, int maxCount);

    /// <summary>
    /// The get smilies.
    /// </summary>
    /// <returns>
    /// Table with list of smiles
    /// </returns>
    IEnumerable<TypedSmileyList> GetSmilies();

    /// <summary>
    /// Loads the message text into the paged data if "Message" and "MessageID" exists.
    /// </summary>
    /// <param name="dataRows">
    /// </param>
    void LoadMessageText([NotNull] IEnumerable<DataRow> dataRows);

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    DataTable StyleTransformDataTable([NotNull] DataTable dt);

    /// <summary>
    /// The style transform func wrap.
    /// </summary>
    /// <param name="dt">
    /// The DateTable
    /// </param>
    /// <param name="styleColumns">
    /// Style columns names
    /// </param>
    /// <returns>
    /// The style transform wrap.
    /// </returns>
    DataTable StyleTransformDataTable([NotNull] DataTable dt, [NotNull] params string[] styleColumns);

    /// <summary>
    /// The Buddy list for the user with the specified UserID.
    /// </summary>
    /// <param name="UserID">
    /// </param>
    /// <returns>
    /// </returns>
    DataTable UserBuddyList(int UserID);

    /// <summary>
    /// The user ignored list.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    List<int> UserIgnoredList(int userId);

    /// <summary>
    /// The user medals.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    DataTable UserMedals(int userId);

    #endregion
  }
}