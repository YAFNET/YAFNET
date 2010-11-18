/* Yet Another Forum.NET
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

namespace YAF.DotNetNuke.Controller
{
  #region Using

  using System.Data;
  using System.Data.SqlClient;

  using YAF.Classes.Data;

  #endregion

  /// <summary>
  /// Module Data Controller to Handle SQL Stuff
  /// </summary>
  public class DataController
  {
    #region Public Methods

    /// <summary>
    /// Get The Latest Post from SQL
    /// </summary>
    /// <param name="boardId">
    /// The Board Id of the Board
    /// </param>
    /// <param name="numOfPostsToRetrieve">
    /// How many post should been retrieved
    /// </param>
    /// <param name="userId">
    /// Current Users Id
    /// </param>
    /// <param name="useStyledNicks">
    /// </param>
    /// <param name="showNoCountPosts">
    /// </param>
    /// <returns>
    /// Data Table of Latest Posts
    /// </returns>
    public static DataTable TopicLatest(
      object boardId, object numOfPostsToRetrieve, object userId, bool useStyledNicks, bool showNoCountPosts)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_latest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
        cmd.Parameters.AddWithValue("UserID", userId);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion
  }
}