/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Security;

namespace YAF.Classes.Data
{
  using System.Web;

  using YAF.Classes.Extensions;
  using YAF.Classes.Utils;

  /// <summary>
  /// All the Database functions for YAF
  /// </summary>
  public static class DB
  {
    #region ConnectionStringOptions

    /// <summary>
    /// Gets ProviderAssemblyName.
    /// </summary>
    public static string ProviderAssemblyName
    {
      get
      {
        return "System.Data.SqlClient";
      }
    }

    /// <summary>
    /// Gets a value indicating whether PasswordPlaceholderVisible.
    /// </summary>
    public static bool PasswordPlaceholderVisible
    {
      get
      {
        return false;
      }
    }

    // Text boxes : Parameters 1-10 
    // Parameter 1
    /// <summary>
    /// Gets Parameter1_Name.
    /// </summary>
    public static string Parameter1_Name
    {
      get
      {
        return "Data Source";
      }
    }

    /// <summary>
    /// Gets Parameter1_Value.
    /// </summary>
    public static string Parameter1_Value
    {
      get
      {
        return "(local)";
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter1_Visible.
    /// </summary>
    public static bool Parameter1_Visible
    {
      get
      {
        return true;
      }
    }

    // Parameter 2
    /// <summary>
    /// Gets Parameter2_Name.
    /// </summary>
    public static string Parameter2_Name
    {
      get
      {
        return "Initial Catalog";
      }
    }

    /// <summary>
    /// Gets Parameter2_Value.
    /// </summary>
    public static string Parameter2_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter2_Visible.
    /// </summary>
    public static bool Parameter2_Visible
    {
      get
      {
        return true;
      }
    }

    // Parameter 3
    /// <summary>
    /// Gets Parameter3_Name.
    /// </summary>
    public static string Parameter3_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter3_Value.
    /// </summary>
    public static string Parameter3_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter3_Visible.
    /// </summary>
    public static bool Parameter3_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 4
    /// <summary>
    /// Gets Parameter4_Name.
    /// </summary>
    public static string Parameter4_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter4_Value.
    /// </summary>
    public static string Parameter4_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter4_Visible.
    /// </summary>
    public static bool Parameter4_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 5
    /// <summary>
    /// Gets Parameter5_Name.
    /// </summary>
    public static string Parameter5_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter5_Value.
    /// </summary>
    public static string Parameter5_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter5_Visible.
    /// </summary>
    public static bool Parameter5_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 6
    /// <summary>
    /// Gets Parameter6_Name.
    /// </summary>
    public static string Parameter6_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter6_Value.
    /// </summary>
    public static string Parameter6_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter6_Visible.
    /// </summary>
    public static bool Parameter6_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 7
    /// <summary>
    /// Gets Parameter7_Name.
    /// </summary>
    public static string Parameter7_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter7_Value.
    /// </summary>
    public static string Parameter7_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter7_Visible.
    /// </summary>
    public static bool Parameter7_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 8
    /// <summary>
    /// Gets Parameter8_Name.
    /// </summary>
    public static string Parameter8_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter8_Value.
    /// </summary>
    public static string Parameter8_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter8_Visible.
    /// </summary>
    public static bool Parameter8_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 9
    /// <summary>
    /// Gets Parameter9_Name.
    /// </summary>
    public static string Parameter9_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter9_Value.
    /// </summary>
    public static string Parameter9_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter9_Visible.
    /// </summary>
    public static bool Parameter9_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 10
    /// <summary>
    /// Gets Parameter10_Name.
    /// </summary>
    public static string Parameter10_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets Parameter10_Value.
    /// </summary>
    public static string Parameter10_Value
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter10_Visible.
    /// </summary>
    public static bool Parameter10_Visible
    {
      get
      {
        return false;
      }
    }

    // Check boxes: Parameters 11-19 

    // Parameter 11 hides user password placeholder! 

    /// <summary>
    /// Gets Parameter11_Name.
    /// </summary>
    public static string Parameter11_Name
    {
      get
      {
        return "Use Integrated Security";
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter11_Value.
    /// </summary>
    public static bool Parameter11_Value
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter11_Visible.
    /// </summary>
    public static bool Parameter11_Visible
    {
      get
      {
        return true;
      }
    }

    // Parameter 12 (reserved for 'User Instance' in MS SQL SERVER)
    /// <summary>
    /// Gets Parameter12_Name.
    /// </summary>
    public static string Parameter12_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter12_Value.
    /// </summary>
    public static bool Parameter12_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter12_Visible.
    /// </summary>
    public static bool Parameter12_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 13
    /// <summary>
    /// Gets Parameter13_Name.
    /// </summary>
    public static string Parameter13_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter13_Value.
    /// </summary>
    public static bool Parameter13_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter13_Visible.
    /// </summary>
    public static bool Parameter13_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 14
    /// <summary>
    /// Gets Parameter14_Name.
    /// </summary>
    public static string Parameter14_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter14_Value.
    /// </summary>
    public static bool Parameter14_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter14_Visible.
    /// </summary>
    public static bool Parameter14_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 15
    /// <summary>
    /// Gets Parameter15_Name.
    /// </summary>
    public static string Parameter15_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter15_Value.
    /// </summary>
    public static bool Parameter15_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter15_Visible.
    /// </summary>
    public static bool Parameter15_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 16
    /// <summary>
    /// Gets Parameter16_Name.
    /// </summary>
    public static string Parameter16_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter16_Value.
    /// </summary>
    public static bool Parameter16_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter16_Visible.
    /// </summary>
    public static bool Parameter16_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 17
    /// <summary>
    /// Gets Parameter17_Name.
    /// </summary>
    public static string Parameter17_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter17_Value.
    /// </summary>
    public static bool Parameter17_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter17_Visible.
    /// </summary>
    public static bool Parameter17_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 18
    /// <summary>
    /// Gets Parameter18_Name.
    /// </summary>
    public static string Parameter18_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter18_Value.
    /// </summary>
    public static bool Parameter18_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter18_Visible.
    /// </summary>
    public static bool Parameter18_Visible
    {
      get
      {
        return false;
      }
    }

    // Parameter 19
    /// <summary>
    /// Gets Parameter19_Name.
    /// </summary>
    public static string Parameter19_Name
    {
      get
      {
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter19_Value.
    /// </summary>
    public static bool Parameter19_Value
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Gets a value indicating whether Parameter19_Visible.
    /// </summary>
    public static bool Parameter19_Visible
    {
      get
      {
        return false;
      }
    }

    #endregion

    #region Basic Forum Properties

    /// <summary>
    /// The _script list.
    /// </summary>
    private static readonly string[] _scriptList = {
                                                     "mssql/tables.sql", "mssql/indexes.sql", "mssql/views.sql", "mssql/constraints.sql", "mssql/triggers.sql", 
                                                     "mssql/functions.sql", "mssql/procedures.sql",  "mssql/providers/procedures.sql", 
                                                     "mssql/providers/tables.sql", "mssql/providers/indexes.sql"
                                                   };

    /// <summary>
    /// The _full text script.
    /// </summary>
    private static string _fullTextScript = "mssql/fulltext.sql";

    /// <summary>
    /// The _full text supported.
    /// </summary>
    private static bool _fullTextSupported = true;

    /// <summary>
    /// Gets the database size
    /// </summary>
    /// <returns>intager value for database size</returns>
    public static int DBSize
    {
      get
      {
        using (var cmd = new SqlCommand("select sum(cast(size as integer))/128 from sysfiles"))
        {
          cmd.CommandType = CommandType.Text;
          return (int)YafDBAccess.Current.ExecuteScalar(cmd);
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether IsForumInstalled.
    /// </summary>
    public static bool IsForumInstalled
    {
      get
      {
        try
        {
          using (DataTable dt = board_list(DBNull.Value))
          {
            return dt.Rows.Count > 0;
          }
        }
        catch
        {
        }

        return false;
      }
    }

    /// <summary>
    /// Gets DBVersion.
    /// </summary>
    public static int DBVersion
    {
      get
      {
        try
        {
          using (DataTable dt = registry_list("version"))
          {
            if (dt.Rows.Count > 0)
            {
              // get the version...
              return Convert.ToInt32(dt.Rows[0]["Value"]);
            }
          }
        }
        catch
        {
          // not installed...
        }

        return -1;
      }
    }

    // MS SQL Support fulltext....

    /// <summary>
    /// Gets or sets a value indicating whether FullTextSupported.
    /// </summary>
    public static bool FullTextSupported
    {
      get
      {
        return _fullTextSupported;
      }

      set
      {
        _fullTextSupported = value;
      }
    }

    /// <summary>
    /// Gets or sets FullTextScript.
    /// </summary>
    public static string FullTextScript
    {
      get
      {
        return _fullTextScript;
      }

      set
      {
        _fullTextScript = value;
      }
    }

    /// <summary>
    /// Gets ScriptList.
    /// </summary>
    public static string[] ScriptList
    {
      get
      {
        return _scriptList;
      }
    }

    #endregion

    /// <summary>
    /// The get boolean registry value.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// The get boolean registry value.
    /// </returns>
    private static bool GetBooleanRegistryValue(string name)
    {
      using (DataTable dt = registry_list(name))
      {
        foreach (DataRow dr in dt.Rows)
        {
          int i;
          return int.TryParse(dr["Value"].ToString(), out i) ? SqlDataLayerConverter.VerifyBool(i) : SqlDataLayerConverter.VerifyBool(dr["Value"]);
        }
      }

      return false;
    }

    #region Forum

    /// <summary>
    /// The pageload.
    /// </summary>
    /// <param name="sessionID">
    /// The session id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userKey">
    /// The user key.
    /// </param>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <param name="location">
    /// The location.
    /// </param>
    /// <param name="forumPage">
    /// The forum page name.   
    /// </param>
    /// <param name="browser">
    /// The browser.
    /// </param>
    /// <param name="platform">
    /// The platform.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="donttrack">
    /// The donttrack.
    /// </param>
    /// <returns>
    /// Common User Info DataRow
    /// </returns>
    /// <exception cref="ApplicationException">
    /// </exception>
    public static DataRow pageload(
      object sessionID,
      object boardID,
      object userKey,
      object ip,
      object location,
      object forumPage,
      object browser,
      object platform,
      object categoryID,
      object forumID,
      object topicID,
      object messageID,
      object isCrawler,
      object donttrack)
    {
      int nTries = 0;
      while (true)
      {
        try
        {
          using (SqlCommand cmd = YafDBAccess.GetCommand("pageload"))
          {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("SessionID", sessionID);
            cmd.Parameters.AddWithValue("BoardID", boardID);
            cmd.Parameters.AddWithValue("UserKey", userKey);
            cmd.Parameters.AddWithValue("IP", ip);
            cmd.Parameters.AddWithValue("Location", location);
            cmd.Parameters.AddWithValue("ForumPage", forumPage);
            cmd.Parameters.AddWithValue("Browser", browser);
            cmd.Parameters.AddWithValue("Platform", platform);
            cmd.Parameters.AddWithValue("CategoryID", categoryID);
            cmd.Parameters.AddWithValue("ForumID", forumID);
            cmd.Parameters.AddWithValue("TopicID", topicID);
            cmd.Parameters.AddWithValue("MessageID", messageID);
            cmd.Parameters.AddWithValue("IsCrawler", isCrawler);
            cmd.Parameters.AddWithValue("DontTrack", donttrack);

            using (DataTable dt = YafDBAccess.Current.GetData(cmd))
            {
              if (dt.Rows.Count > 0)
              {
                return dt.Rows[0];
              }
              else
              {
                return null;
              }
            }
          }
        }
        catch (SqlException x)
        {
          if (x.Number == 1205 && nTries < 3)
          {
            // Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
          }
          else
          {
            throw new ApplicationException(string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
          }
        }

        ++nTries;
      }
    }

    /// <summary>
    /// Returns Search results
    /// </summary>
    /// <param name="toSearchWhat">
    /// The to Search What.
    /// </param>
    /// <param name="toSearchFromWho">
    /// The to Search From Who.
    /// </param>
    /// <param name="searchFromWhoMethod">
    /// The search From Who Method.
    /// </param>
    /// <param name="searchWhatMethod">
    /// The search What Method.
    /// </param>
    /// <param name="forumIDToStartAt">
    /// The forum ID To Start At.
    /// </param>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="maxResults">
    /// The max Results.
    /// </param>
    /// <param name="useFullText">
    /// The use Full Text.
    /// </param>
    /// <returns>
    /// Results
    /// </returns>
    public static DataTable GetSearchResult(
      string toSearchWhat,
      string toSearchFromWho,
      SearchWhatFlags searchFromWhoMethod,
      SearchWhatFlags searchWhatMethod,
      int forumIDToStartAt,
      int userID,
      int boardId,
      int maxResults,
      bool useFullText,
     bool searchDisplayName)
    {
      if (toSearchWhat == "*")
      {
        toSearchWhat = string.Empty;
      }

      IEnumerable<int> forumIds = new List<int>();

      if (forumIDToStartAt != 0)
      {
        forumIds = ForumListAll(boardId, userID, forumIDToStartAt).Select(f => f.ForumID ?? 0).Distinct();
      }

      string searchSql = new SearchBuilder().BuildSearchSql(toSearchWhat, toSearchFromWho, searchFromWhoMethod, searchWhatMethod, userID, searchDisplayName, boardId, maxResults, useFullText, forumIds);

      using (SqlCommand cmd = YafDBAccess.GetCommand(searchSql, true))
      {
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region DataSets

    /// <summary>
    /// Gets a list of categories????
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <returns>
    /// DataSet with categories
    /// </returns>
    public static DataSet ds_forumadmin(object boardID)
    {
      using (var connMan = new YafDBConnManager())
      {
        using (var ds = new DataSet())
        {
          using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(YafDBAccess.IsolationLevel))
          {
            using (var da = new SqlDataAdapter(YafDBAccess.GetObjectName("category_list"), connMan.DBConnection))
            {
              da.SelectCommand.Transaction = trans;
              da.SelectCommand.Parameters.AddWithValue("BoardID", boardID);
              da.SelectCommand.CommandType = CommandType.StoredProcedure;
              da.Fill(ds, YafDBAccess.GetObjectName("Category"));
              da.SelectCommand.CommandText = YafDBAccess.GetObjectName("forum_list");
              da.Fill(ds, YafDBAccess.GetObjectName("ForumUnsorted"));

              DataTable dtForumListSorted = ds.Tables[YafDBAccess.GetObjectName("ForumUnsorted")].Clone();
              dtForumListSorted.TableName = YafDBAccess.GetObjectName("Forum");
              ds.Tables.Add(dtForumListSorted);
              dtForumListSorted.Dispose();
              forum_list_sort_basic(ds.Tables[YafDBAccess.GetObjectName("ForumUnsorted")], ds.Tables[YafDBAccess.GetObjectName("Forum")], 0, 0);
              ds.Tables.Remove(YafDBAccess.GetObjectName("ForumUnsorted"));
              ds.Relations.Add(
                "FK_Forum_Category",
                ds.Tables[YafDBAccess.GetObjectName("Category")].Columns["CategoryID"],
                ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["CategoryID"]);
              trans.Commit();
            }

            return ds;
          }
        }
      }
    }

    #endregion

    #region AccessMask

    /// <summary>
    /// Gets a list of access mask properities
    /// </summary>
    /// <param name="boardID">
    /// ID of Board
    /// </param>
    /// <param name="accessMaskID">
    /// ID of access mask
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable accessmask_list(object boardID, object accessMaskID)
    {
      return accessmask_list(boardID, accessMaskID, 0);
    }

    /// <summary>
    /// Gets a list of access mask properities
    /// </summary>
    /// <param name="boardID">
    /// ID of Board
    /// </param>
    /// <param name="accessMaskID">
    /// ID of access mask
    /// </param>
    /// <param name="excludeFlags">
    /// Ommit access masks with this flags set.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable accessmask_list(object boardID, object accessMaskID, object excludeFlags)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("accessmask_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        cmd.Parameters.AddWithValue("ExcludeFlags", excludeFlags);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Deletes an access mask
    /// </summary>
    /// <param name="accessMaskID">
    /// ID of access mask
    /// </param>
    /// <returns>
    /// The accessmask_delete.
    /// </returns>
    public static bool accessmask_delete(object accessMaskID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("accessmask_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd) != 0;
      }
    }

    /// <summary>
    /// Saves changes to a access mask 
    /// </summary>
    /// <param name="accessMaskID">
    /// ID of access mask
    /// </param>
    /// <param name="boardID">
    /// ID of board
    /// </param>
    /// <param name="name">
    /// Name of access mask
    /// </param>
    /// <param name="readAccess">
    /// Read Access?
    /// </param>
    /// <param name="postAccess">
    /// Post Access?
    /// </param>
    /// <param name="replyAccess">
    /// Reply Access?
    /// </param>
    /// <param name="priorityAccess">
    /// Priority Access?
    /// </param>
    /// <param name="pollAccess">
    /// Poll Access?
    /// </param>
    /// <param name="voteAccess">
    /// Vote Access?
    /// </param>
    /// <param name="moderatorAccess">
    /// Moderator Access?
    /// </param>
    /// <param name="editAccess">
    /// Edit Access?
    /// </param>
    /// <param name="deleteAccess">
    /// Delete Access?
    /// </param>
    /// <param name="uploadAccess">
    /// Upload Access?
    /// </param>
    /// <param name="downloadAccess">
    /// Download Access?
    /// </param> 
    /// <param name="sortOrder">
    /// Sort Order?
    /// </param> 
    public static void accessmask_save(
      object accessMaskID,
      object boardID,
      object name,
      object readAccess,
      object postAccess,
      object replyAccess,
      object priorityAccess,
      object pollAccess,
      object voteAccess,
      object moderatorAccess,
      object editAccess,
      object deleteAccess,
      object uploadAccess,
      object downloadAccess,
      object sortOrder)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("accessmask_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("ReadAccess", readAccess);
        cmd.Parameters.AddWithValue("PostAccess", postAccess);
        cmd.Parameters.AddWithValue("ReplyAccess", replyAccess);
        cmd.Parameters.AddWithValue("PriorityAccess", priorityAccess);
        cmd.Parameters.AddWithValue("PollAccess", pollAccess);
        cmd.Parameters.AddWithValue("VoteAccess", voteAccess);
        cmd.Parameters.AddWithValue("ModeratorAccess", moderatorAccess);
        cmd.Parameters.AddWithValue("EditAccess", editAccess);
        cmd.Parameters.AddWithValue("DeleteAccess", deleteAccess);
        cmd.Parameters.AddWithValue("UploadAccess", uploadAccess);
        cmd.Parameters.AddWithValue("DownloadAccess", downloadAccess);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Active

    /// <summary>
    /// Gets list of active users
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="Guests">
    /// </param>
    /// <param name="activeTime">
    /// The active Time.
    /// </param>
    /// <param name="styledNicks">
    /// The styled Nicks.
    /// </param>
    /// <returns>
    /// Returns a DataTable of active users
    /// </returns>
    public static DataTable active_list(object boardID, object Guests, object showCrawlers, int activeTime, object styledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("active_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Guests", Guests);
        cmd.Parameters.AddWithValue("ShowCrawlers", showCrawlers);
        cmd.Parameters.AddWithValue("ActiveTime", activeTime);
        cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets list of active users for a specific user with access fixes to not display him forbidden locations. 
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// the UserID
    /// </param>
    /// <param name="Guests">
    /// </param>
    /// <param name="activeTime">
    /// The active Time.
    /// </param>
    /// <param name="styledNicks">
    /// The styled Nicks.
    /// </param>
    /// <returns>
    /// Returns a DataTable of active users
    /// </returns>
    public static DataTable active_list_user(object boardID, object userID, object Guests, object showCrawlers, int activeTime, object styledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("active_list_user"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Guests", Guests);
        cmd.Parameters.AddWithValue("ShowCrawlers", showCrawlers);
        cmd.Parameters.AddWithValue("ActiveTime", activeTime);
        cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets the list of active users within a certain forum
    /// </summary>
    /// <param name="forumID">
    /// forumID
    /// </param>
    /// <param name="styledNicks">
    /// The styled Nicks.
    /// </param>
    /// <returns>
    /// DataTable of all ative users in a forum
    /// </returns>
    public static DataTable active_listforum(object forumID, object styledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("active_listforum"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets the list of active users in a topic
    /// </summary>
    /// <param name="topicID">
    /// ID of topic 
    /// </param>
    /// <param name="styledNicks">
    /// The styled Nicks.
    /// </param>
    /// <returns>
    /// DataTable of all users that are in a topic
    /// </returns>
    public static DataTable active_listtopic(object topicID, object styledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("active_listtopic"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets the activity statistics for a board
    /// </summary>
    /// <param name="boardID">
    /// boardID
    /// </param>
    /// <returns>
    /// DataRow of activity stata
    /// </returns>
    public static DataRow active_stats(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("active_stats"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          return dt.Rows[0];
        }
      }
    }

    #endregion

    #region Attachment

    /// <summary>
    /// Gets a list of attachments
    /// </summary>
    /// <param name="messageID">
    /// messageID
    /// </param>
    /// <param name="attachmentID">
    /// attachementID
    /// </param>
    /// <param name="boardID">
    /// boardID
    /// </param>
    /// <returns>
    /// DataTable with attachement list
    /// </returns>
    public static DataTable attachment_list(object messageID, object attachmentID, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// saves attachment
    /// </summary>
    /// <param name="messageID">
    /// messageID
    /// </param>
    /// <param name="fileName">
    /// File Name
    /// </param>
    /// <param name="bytes">
    /// number of bytes
    /// </param>
    /// <param name="contentType">
    /// type of attchment
    /// </param>
    /// <param name="stream">
    /// stream of bytes
    /// </param>
    public static void attachment_save(object messageID, object fileName, object bytes, object contentType, Stream stream)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_save"))
      {
        byte[] fileData = null;
        if (stream != null)
        {
          fileData = new byte[stream.Length];
          stream.Seek(0, SeekOrigin.Begin);
          stream.Read(fileData, 0, (int)stream.Length);
        }

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("FileName", fileName);
        cmd.Parameters.AddWithValue("Bytes", bytes);
        cmd.Parameters.AddWithValue("ContentType", contentType);
        cmd.Parameters.AddWithValue("FileData", fileData);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Delete attachment
    /// </summary>
    /// <param name="attachmentID">
    /// ID of attachment to delete
    /// </param>
    public static void attachment_delete(object attachmentID)
    {
      bool useFileTable = GetBooleanRegistryValue("UseFileTable");

      // If the files are actually saved in the Hard Drive
      if (!useFileTable)
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_list"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
          DataTable tbAttachments = YafDBAccess.Current.GetData(cmd);

          string uploadDir = HostingEnvironment.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

          foreach (DataRow row in tbAttachments.Rows)
          {
            try
            {
              string fileName = String.Format("{0}/{1}.{2}.yafupload", uploadDir, row["MessageID"], row["FileName"]);
              if (File.Exists(fileName))
              {
                File.Delete(fileName);
              }
            }
            catch
            {
              // error deleting that file... 
            }
          }
        }
      }

      using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }

      // End ABOT CHANGE 16.04.04
    }

    /// <summary>
    /// Attachement dowload
    /// </summary>
    /// <param name="attachmentID">
    /// ID of attachemnt to download
    /// </param>
    public static void attachment_download(object attachmentID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_download"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region BannedIP

    /// <summary>
    /// List of Baned IP's
    /// </summary>
    /// <param name="boardID">
    /// ID of board
    /// </param>
    /// <param name="ID">
    /// ID
    /// </param>
    /// <returns>
    /// DataTable of banned IPs
    /// </returns>
    public static DataTable bannedip_list(object boardID, object ID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bannedip_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ID", ID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Saves baned ip in database
    /// </summary>
    /// <param name="ID">
    /// ID
    /// </param>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="Mask">
    /// Mask
    /// </param>
    public static void bannedip_save(object ID, object boardID, object Mask, string reason, int userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bannedip_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ID", ID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Mask", Mask);
        cmd.Parameters.AddWithValue("Reason", reason);
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Deletes Banned IP
    /// </summary>
    /// <param name="ID">
    /// ID of banned ip to delete
    /// </param>
    public static void bannedip_delete(object ID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bannedip_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ID", ID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Board

    /// <summary>
    /// Gets a list of information about a board
    /// </summary>
    /// <param name="boardID">
    /// board id
    /// </param>
    /// <returns>
    /// DataTable
    /// </returns>
    public static DataTable board_list(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets posting statistics
    /// </summary>
    /// <param name="boardId">
    /// BoardID
    /// </param>
    /// <param name="useStyledNick">
    /// UseStyledNick
    /// </param>
    /// <param name="showNoCountPosts">
    /// ShowNoCountPosts
    /// </param> 
    /// <returns>
    /// DataRow of Poststats
    /// </returns>
    public static DataRow board_poststats(int? boardId, bool useStyledNick, bool showNoCountPosts)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_poststats"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNick);
        cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
         
        cmd.Parameters.AddWithValue("GetDefaults", 0);
          DataTable dt = YafDBAccess.Current.GetData(cmd);
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0];
            }
       
      }
        // vzrus - this happens at new install only when we don't have posts or when they are not visible to a user 
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_poststats"))
      {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("BoardID", boardId);
          cmd.Parameters.AddWithValue("StyledNicks", useStyledNick);
          cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
          cmd.Parameters.AddWithValue("GetDefaults", 1);
          DataTable dt = YafDBAccess.Current.GetData(cmd);
          if (dt.Rows.Count > 0)
          {
              return dt.Rows[0];
          }

      }
        return null;
    }

    /// <summary>
    /// Gets users statistics
    /// </summary>
    /// <param name="boardId">
    /// BoardID
    /// </param>
    /// <returns>
    /// DataRow of Poststats
    /// </returns>
    public static DataRow board_userstats(int? boardId)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("board_userstats"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("BoardID", boardId);
            using (DataTable dt = YafDBAccess.Current.GetData(cmd))
            {
                return dt.Rows[0];
            }
        }
    }

    /// <summary>
    /// Recalculates topic and post numbers and updates last post for all forums in all boards
    /// </summary>
    public static void board_resync()
    {
      board_resync(null);
    }

    /// <summary>
    /// Recalculates topic and post numbers and updates last post for specified board
    /// </summary>
    /// <param name="boardID">
    /// BoardID of board to do re-sync for, if null, all boards are re-synced
    /// </param>
    public static void board_resync(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_resync"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Gets statistica about number of posts etc.
    /// </summary>
    /// <returns>
    /// DataRow
    /// </returns>
    public static DataRow board_stats()
    {
      return board_stats(null);
    }

    /// <summary>
    /// The board_stats.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataRow board_stats(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_stats"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);

        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          return dt.Rows[0];
        }
      }
    }

    /// <summary>
    /// Saves board information
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="name">
    /// Name of Board
    /// </param>
    /// <param name="allowThreaded">
    /// Boolen value, allowThreaded
    /// </param>
    /// <returns>
    /// The board_save.
    /// </returns>
    public static int board_save(object boardID, object languageFile, object culture, object name, object allowThreaded)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("LanguageFile", languageFile);
        cmd.Parameters.AddWithValue("Culture", culture);
        cmd.Parameters.AddWithValue("AllowThreaded", allowThreaded);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// Creates a new board
    /// </summary>
    /// <param name="adminUsername">
    /// Membership Provider User Name
    /// </param>
    /// <param name="adminUserKey">
    /// Membership Provider User Key
    /// </param>
    /// <param name="boardName">
    /// Name of new board
    /// </param>
    /// <param name="boardMembershipName">
    /// Membership Provider Application Name for new board
    /// </param>
    /// <param name="boardRolesName">
    /// Roles Provider Application Name for new board
    /// </param>
    /// <returns>
    /// The board_create.
    /// </returns>
    public static int board_create(object adminUsername, object adminUserEmail, object adminUserKey, object boardName, object culture, object languageFile, object boardMembershipName, object boardRolesName)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_create"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardName", boardName);
        cmd.Parameters.AddWithValue("Culture", culture);
        cmd.Parameters.AddWithValue("LanguageFile", languageFile);
        cmd.Parameters.AddWithValue("MembershipAppName", boardMembershipName);
        cmd.Parameters.AddWithValue("RolesAppName", boardRolesName);
        cmd.Parameters.AddWithValue("UserName", adminUsername);
        cmd.Parameters.AddWithValue("UserEmail", adminUserEmail);
        cmd.Parameters.AddWithValue("UserKey", adminUserKey);
        cmd.Parameters.AddWithValue("IsHostAdmin", 0);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// Deletes a board
    /// </summary>
    /// <param name="boardID">
    /// ID of board to delete
    /// </param>
    public static void board_delete(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("board_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Category

    /// <summary>
    /// Deletes a category
    /// </summary>
    /// <param name="CategoryID">
    /// ID of category to delete
    /// </param>
    /// <returns>
    /// Bool value indicationg if category was deleted
    /// </returns>
    public static bool category_delete(object CategoryID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("category_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("CategoryID", CategoryID);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd) != 0;
      }
    }

    /// <summary>
    /// Gets a list of forums in a category
    /// </summary>
    /// <param name="boardID">
    /// boardID
    /// </param>
    /// <param name="categoryID">
    /// categotyID
    /// </param>
    /// <returns>
    /// DataTable with a list of forums in a category
    /// </returns>
    public static DataTable category_list(object boardID, object categoryID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("category_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets a list of forum categories
    /// </summary>
    /// <param name="boardID">
    /// </param>
    /// <param name="userID">
    /// </param>
    /// <param name="categoryID">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable category_listread(object boardID, object userID, object categoryID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("category_listread"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Lists categories very simply (for URL rewriting)
    /// </summary>
    /// <param name="startID">
    /// The start ID.
    /// </param>
    /// <param name="limit">
    /// The limit.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable category_simplelist(int startID, int limit)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("category_simplelist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("StartID", startID);
        cmd.Parameters.AddWithValue("Limit", limit);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Saves changes to a category
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="categoryId">
    /// The category Id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="categoryImage">
    /// The category Image.
    /// </param>
    /// <param name="sortOrder">
    /// The sort Order.
    /// </param>
    public static void category_save(object boardID, object categoryId, object name, object categoryImage, object sortOrder)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("category_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("CategoryID", categoryId);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("CategoryImage", categoryImage);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region CheckEmail

    /// <summary>
    /// Saves a new email into the table for verification
    /// </summary>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <param name="hash">
    /// The hash.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    public static void checkemail_save(object userID, object hash, object email)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("checkemail_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Hash", hash);
        cmd.Parameters.AddWithValue("Email", email);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Updates a hash
    /// </summary>
    /// <param name="hash">
    /// New hash
    /// </param>
    /// <returns>
    /// DataTable with user information
    /// </returns>
    public static DataTable checkemail_update(object hash)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("checkemail_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Hash", hash);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Gets a check email entry based on email or all if no email supplied
    /// </summary>
    /// <param name="email">
    /// Associated email
    /// </param>
    /// <returns>
    /// DataTable with check email information
    /// </returns>
    public static DataTable checkemail_list(object email)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("checkemail_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Email", email);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region Choice

    /// <summary>
    /// Saves a vote in the database
    /// </summary>
    /// <param name="choiceID">
    /// Choice of the vote
    /// </param>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <param name="remoteIP">
    /// The remote IP.
    /// </param>
    public static void choice_vote(object choiceID, object userID, object remoteIP)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("choice_vote"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ChoiceID", choiceID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("RemoteIP", remoteIP);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region EventLog

    /// <summary>
    /// The eventlog_create.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="type">
    /// The type.
    /// </param>
    public static void eventlog_create(object userID, object source, object description, object type)
    {
      try
      {
        if (userID == null)
        {
          userID = DBNull.Value;
        }

        using (SqlCommand cmd = YafDBAccess.GetCommand("eventlog_create"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("Type", type);
          cmd.Parameters.AddWithValue("UserID", userID);
          cmd.Parameters.AddWithValue("Source", source.ToString());
          cmd.Parameters.AddWithValue("Description", description.ToString());
          YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
      }
      catch
      {
        // Ignore any errors in this method
      }
    }


    /// <summary>
    /// The eventlog_create.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    public static void eventlog_create(object userID, object source, object description)
    {
      eventlog_create(userID, source.GetType().ToString(), description, 0);
    }

    /// <summary>
    /// Deletes all event log entries for given board.
    /// </summary>
    /// <param name="boardID">
    /// ID of board.
    /// </param>
    public static void eventlog_delete(int boardID)
    {
      eventlog_delete(null, boardID);
    }

    /// <summary>
    /// Deletes event log entry of given ID.
    /// </summary>
    /// <param name="eventLogID">
    /// ID of event log entry.
    /// </param>
    public static void eventlog_delete(object eventLogID)
    {
      eventlog_delete(eventLogID, null);
    }

    /// <summary>
    /// Calls underlying stroed procedure for deletion of event log entry(ies).
    /// </summary>
    /// <param name="eventLogID">
    /// When not null, only given event log entry is deleted.
    /// </param>
    /// <param name="boardID">
    /// Specifies board. It is ignored if eventLogID parameter is not null.
    /// </param>
    private static void eventlog_delete(object eventLogID, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("eventlog_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("EventLogID", eventLogID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The eventlog_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable eventlog_list(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("eventlog_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion EventLog

    #region Extensions

    /// <summary>
    /// The extension_delete.
    /// </summary>
    /// <param name="extensionId">
    /// The extension id.
    /// </param>
    public static void extension_delete(object extensionId)
    {
      try
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("extension_delete"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("ExtensionId", extensionId);
          YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
      }
      catch
      {
        // Ignore any errors in this method
      }
    }

    // Get Extension record by extensionId
    /// <summary>
    /// The extension_edit.
    /// </summary>
    /// <param name="extensionId">
    /// The extension id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable extension_edit(object extensionId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("extension_edit"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("extensionId", extensionId);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // Used to validate a file before uploading
    /// <summary>
    /// The extension_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="extension">
    /// The extension.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable extension_list(object boardID, object extension)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("extension_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Extension", extension);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // Returns an extension list for a given Board
    /// <summary>
    /// The extension_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable extension_list(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("extension_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Extension", string.Empty);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // Saves / creates extension
    /// <summary>
    /// The extension_save.
    /// </summary>
    /// <param name="extensionId">
    /// The extension id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="Extension">
    /// The extension.
    /// </param>
    public static void extension_save(object extensionId, object boardID, object Extension)
    {
      try
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("extension_save"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("extensionId", extensionId);
          cmd.Parameters.AddWithValue("BoardId", boardID);
          cmd.Parameters.AddWithValue("Extension", Extension);
          YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
      }
      catch
      {
        // Ignore any errors in this method
      }
    }

    #endregion EventLog

    #region PollVote

    /// <summary>
    /// Checks for a vote in the database
    /// </summary>
    /// <param name="pollid">
    /// The pollid.
    /// </param>
    /// <param name="userid">
    /// The userid.
    /// </param>
    /// <param name="remoteip">
    /// The remoteip.
    /// </param>
    public static DataTable pollvote_check(object pollid, object userid, object remoteip)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pollvote_check"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("PollID", pollid);
        cmd.Parameters.AddWithValue("UserID", userid);
        cmd.Parameters.AddWithValue("RemoteIP", remoteip);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Checks for a vote in the database 
    /// </summary>
    /// <param name="pollGroupId">
    /// The pollGroupid.
    /// </param>
    /// <param name="userId">
    /// The userid.
    /// </param>
    /// <param name="remoteIp">
    /// The remoteip.
    /// </param>
    public static DataTable pollgroup_votecheck(object pollGroupId, object userId, object remoteIp)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("pollgroup_votecheck"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
            cmd.Parameters.AddWithValue("UserID", userId);
            cmd.Parameters.AddWithValue("RemoteIP", remoteIp);
            return YafDBAccess.Current.GetData(cmd);
        }
    }

    #endregion

    #region Forum

    // ABOT NEW 16.04.04
    /// <summary>
    /// Deletes attachments out of a entire forum
    /// </summary>
    /// <param name="forumID">
    /// The forum ID.
    /// </param>
    private static void forum_deleteAttachments(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listtopics"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          foreach (DataRow row in dt.Rows)
          {
            if (row != null && row["TopicID"] != DBNull.Value)
            {
              topic_delete(row["TopicID"], true);
            }
          }
        }
      }
    }

    // END ABOT NEW 16.04.04
    // ABOT CHANGE 16.04.04
    /// <summary>
    /// Deletes a forum
    /// </summary>
    /// <param name="forumID">
    /// The forum ID.
    /// </param>
    /// <returns>
    /// bool to indicate that forum has been deleted
    /// </returns>
    public static bool forum_delete(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listSubForums"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        if (YafDBAccess.Current.ExecuteScalar(cmd) is DBNull)
        {
          forum_deleteAttachments(forumID);
          using (SqlCommand cmd_new = YafDBAccess.GetCommand("forum_delete"))
          {
            cmd_new.CommandType = CommandType.StoredProcedure;
            cmd_new.CommandTimeout = 99999;
            cmd_new.Parameters.AddWithValue("ForumID", forumID);
            YafDBAccess.Current.ExecuteNonQuery(cmd_new);
          }

          return true;
        }
        else
        {
          return false;
        }
      }
    }

    // END ABOT CHANGE 16.04.04
    // ABOT NEW 16.04.04: This new function lists all moderated topic by the specified user
    /// <summary>
    /// Lists all moderated forums for a user
    /// </summary>
    /// <param name="boardID">
    /// board if of moderators
    /// </param>
    /// <param name="userID">
    /// user id
    /// </param>
    /// <returns>
    /// DataTable of moderated forums
    /// </returns>
    public static DataTable forum_listallMyModerated(object boardID, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listallmymoderated"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // END ABOT NEW 16.04.04
    /// <summary>
    /// Gets a list of topics in a forum
    /// </summary>
    /// <param name="boardID">
    /// boardID
    /// </param>
    /// <param name="forumID">
    /// The forum ID.
    /// </param>
    /// <returns>
    /// DataTable with list of topics from a forum
    /// </returns>
    public static DataTable forum_list(object boardID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Listes all forums accessible to a user
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// ID of user
    /// </param>
    /// <returns>
    /// DataTable of all accessible forums
    /// </returns>
    public static DataTable forum_listall(object boardID, object userID)
    {
      return forum_listall(boardID, userID, 0);
    }

    public static IEnumerable<TypedForumListAll> ForumListAll(int boardId, int userId)
    {
      return forum_listall(boardId, userId, 0).AsEnumerable().Select(r => new TypedForumListAll(r));
    }

    public static IEnumerable<TypedForumListAll> ForumListAll(int boardId, int userId, int startForumId)
    {
      var allForums = ForumListAll(boardId, userId);

      var forumIds = new List<int>();
      var tempForumIds = new List<int>();

      forumIds.Add(startForumId);
      tempForumIds.Add(startForumId);

      while (true)
      {
        var temp = allForums.Where(f => tempForumIds.Contains(f.ParentID ?? 0));

        if (!temp.Any())
        {
          break;
        }

        // replace temp forum ids with these...
        tempForumIds = temp.Select(f => f.ForumID ?? 0).Distinct().ToList();

        // add them...
        forumIds.AddRange(tempForumIds);
      }

      // return filtered forums...
      return allForums.Where(f => forumIds.Contains(f.ForumID ?? 0)).Distinct();
    }

    /// <summary>
    /// Lists all forums accessible to a user
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// ID of user
    /// </param>
    /// <param name="startAt">
    /// startAt ID
    /// </param>
    /// <returns>
    /// DataTable of all accessible forums
    /// </returns>
    public static DataTable forum_listall(object boardID, object userID, object startAt)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listall"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Root", startAt);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Lists all forums within a given subcategory
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="categoryID">
    /// The category ID.
    /// </param>
    /// <returns>
    /// DataTable with list
    /// </returns>
    public static DataTable forum_listall_fromCat(object boardID, object categoryID)
    {
      return forum_listall_fromCat(boardID, categoryID, true);
    }

    /// <summary>
    /// Lists forums very simply (for URL rewriting)
    /// </summary>
    /// <param name="StartID">
    /// </param>
    /// <param name="Limit">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forum_simplelist(int StartID, int Limit)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_simplelist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("StartID", StartID);
        cmd.Parameters.AddWithValue("Limit", Limit);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Lists all forums within a given subcategory
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="categoryID">
    /// The category ID.
    /// </param>
    /// <param name="emptyFirstRow">
    /// The empty First Row.
    /// </param>
    /// <returns>
    /// DataTable with list
    /// </returns>
    public static DataTable forum_listall_fromCat(object boardID, object categoryID, bool emptyFirstRow)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listall_fromCat"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);

        int intCategoryID = Convert.ToInt32(categoryID.ToString());

        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          return forum_sort_list(dt, 0, intCategoryID, 0, null, emptyFirstRow);
        }
      }
    }

    /// <summary>
    /// Sorry no idea what this does
    /// </summary>
    /// <param name="forumID">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forum_listpath(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listpath"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Lists read topics
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="userID">
    /// The user ID.
    /// </param>
    /// <param name="categoryID">
    /// The category ID.
    /// </param>
    /// <param name="parentID">
    /// ParentID
    /// </param>
    /// <returns>
    /// DataTable with list
    /// </returns>
    public static DataTable forum_listread(object boardID, object userID, object categoryID, object parentID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_listread"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        cmd.Parameters.AddWithValue("ParentID", parentID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Return admin view of Categories with Forums/Subforums ordered accordingly.
    /// </summary>
    /// <param name="userID">
    /// UserID
    /// </param>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <returns>
    /// DataSet with categories
    /// </returns>
    public static DataSet forum_moderatelist(object userID, object boardID)
    {
      using (var connMan = new YafDBConnManager())
      {
        using (var ds = new DataSet())
        {
          using (var da = new SqlDataAdapter(YafDBAccess.GetObjectName("category_list"), connMan.OpenDBConnection))
          {
            using (SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction(YafDBAccess.IsolationLevel))
            {
              da.SelectCommand.Transaction = trans;
              da.SelectCommand.Parameters.AddWithValue("BoardID", boardID);
              da.SelectCommand.CommandType = CommandType.StoredProcedure;
              da.Fill(ds, YafDBAccess.GetObjectName("Category"));
              da.SelectCommand.CommandText = YafDBAccess.GetObjectName("forum_moderatelist");
              da.SelectCommand.Parameters.AddWithValue("UserID", userID);
              da.Fill(ds, YafDBAccess.GetObjectName("ForumUnsorted"));
              DataTable dtForumListSorted = ds.Tables[YafDBAccess.GetObjectName("ForumUnsorted")].Clone();
              dtForumListSorted.TableName = YafDBAccess.GetObjectName("Forum");
              ds.Tables.Add(dtForumListSorted);
              dtForumListSorted.Dispose();
              forum_list_sort_basic(ds.Tables[YafDBAccess.GetObjectName("ForumUnsorted")], ds.Tables[YafDBAccess.GetObjectName("Forum")], 0, 0);
              ds.Tables.Remove(YafDBAccess.GetObjectName("ForumUnsorted"));
              // vzrus: Remove here all forums with no reports. Would be better to do it in query...
              // Array to write categories numbers
              int[] categories = new int[ds.Tables[YafDBAccess.GetObjectName("Forum")].Rows.Count];
              int cntr = 0;
              //We should make it before too as the colection was changed
              ds.Tables[YafDBAccess.GetObjectName("Forum")].AcceptChanges();
              foreach (DataRow dr in ds.Tables[YafDBAccess.GetObjectName("Forum")].Rows)
              {
                categories[cntr] = Convert.ToInt32(dr["CategoryID"]);
                if (Convert.ToInt32(dr["ReportedCount"]) == 0 && Convert.ToInt32(dr["MessageCount"]) == 0)
                {
                  dr.Delete();
                  categories[cntr] = 0;
                }
                cntr++;
              }
              ds.Tables[YafDBAccess.GetObjectName("Forum")].AcceptChanges();

              foreach (DataRow dr in ds.Tables[YafDBAccess.GetObjectName("Category")].Rows)
              {
                bool deleteMe = true;
                for (int i = 0; i < categories.Length; i++)
                {
                  // We check here if the Category is missing in the array where 
                  // we've written categories number for each forum
                  if (categories[i] == Convert.ToInt32(dr["CategoryID"]))
                  {
                    deleteMe = false;
                  }
                }
                if (deleteMe) dr.Delete();
              }
              ds.Tables[YafDBAccess.GetObjectName("Category")].AcceptChanges();

              ds.Relations.Add(
                "FK_Forum_Category",
                ds.Tables[YafDBAccess.GetObjectName("Category")].Columns["CategoryID"],
                ds.Tables[YafDBAccess.GetObjectName("Forum")].Columns["CategoryID"]);

              trans.Commit();
            }

            return ds;
          }
        }
      }
    }

    /// <summary>
    /// The forum_moderators.
    /// </summary>
    /// <returns>
    /// </returns>
    public static DataTable forum_moderators()
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_moderators"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Updates topic and post count and last topic for all forums in specified board
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    public static void forum_resync(object boardID)
    {
      forum_resync(boardID, null);
    }

    /// <summary>
    /// Updates topic and post count and last topic for specified forum
    /// </summary>
    /// <param name="boardID">
    /// BoardID
    /// </param>
    /// <param name="forumID">
    /// If null, all forums in board are updated
    /// </param>
    public static void forum_resync(object boardID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_resync"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The forum_save.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="locked">
    /// The locked.
    /// </param>
    /// <param name="hidden">
    /// The hidden.
    /// </param>
    /// <param name="isTest">
    /// The is test.
    /// </param>
    /// <param name="moderated">
    /// The moderated.
    /// </param>
    /// <param name="accessMaskID">
    /// The access mask id.
    /// </param>
    /// <param name="remoteURL">
    /// The remote url.
    /// </param>
    /// <param name="themeURL">
    /// The theme url.
    /// </param>
    /// <param name="imageURL">
    /// The imageURL.
    /// </param>
    /// <param name="styles">
    /// The styles.
    /// </param>
    /// <param name="dummy">
    /// The dummy.
    /// </param>
    /// <returns>
    /// The forum_save.
    /// </returns>
    public static long forum_save(
      object forumID,
      object categoryID,
      object parentID,
      object name,
      object description,
      object sortOrder,
      object locked,
      object hidden,
      object isTest,
      object moderated,
      object accessMaskID,
      object remoteURL,
      object themeURL,
      object imageURL,
      object styles,
       bool dummy)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forum_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        cmd.Parameters.AddWithValue("ParentID", parentID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Description", description);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("Locked", locked);
        cmd.Parameters.AddWithValue("Hidden", hidden);
        cmd.Parameters.AddWithValue("IsTest", isTest);
        cmd.Parameters.AddWithValue("Moderated", moderated);
        cmd.Parameters.AddWithValue("RemoteURL", remoteURL);
        cmd.Parameters.AddWithValue("ThemeURL", themeURL);
        cmd.Parameters.AddWithValue("ImageURL", imageURL);
        cmd.Parameters.AddWithValue("Styles", styles);
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        return long.Parse(YafDBAccess.Current.ExecuteScalar(cmd).ToString());
      }
    }

    /// <summary>
    /// The method returns an integer value for a  found parent forum 
    /// if a forum is a parent of an existing child to avoid circular dependency
    /// while creating a new forum
    /// </summary>
    /// <param name="forumID"></param>
    /// <param name="parentID"></param>
    /// <returns>Integer value for a found dependency</returns>
    public static int forum_save_parentschecker(object forumID, object parentID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("SELECT " + YafDBAccess.GetObjectName("forum_save_parentschecker") + "(@ForumID,@ParentID)", true))
      {
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("@ForumID", forumID);
        cmd.Parameters.AddWithValue("@ParentID", parentID);
        return Convert.ToInt32(YafDBAccess.Current.ExecuteScalar(cmd));
      }

    }

    /// <summary>
    /// The forum_sort_list.
    /// </summary>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="startingIndent">
    /// The starting indent.
    /// </param>
    /// <param name="forumidExclusions">
    /// The forumid exclusions.
    /// </param>
    /// <returns>
    /// </returns>
    private static DataTable forum_sort_list(DataTable listSource, int parentID, int categoryID, int startingIndent, int[] forumidExclusions)
    {
      return forum_sort_list(listSource, parentID, categoryID, startingIndent, forumidExclusions, true);
    }

    /// <summary>
    /// The forum_sort_list.
    /// </summary>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="startingIndent">
    /// The starting indent.
    /// </param>
    /// <param name="forumidExclusions">
    /// The forumid exclusions.
    /// </param>
    /// <param name="emptyFirstRow">
    /// The empty first row.
    /// </param>
    /// <returns>
    /// </returns>
    private static DataTable forum_sort_list(
      DataTable listSource, int parentID, int categoryID, int startingIndent, int[] forumidExclusions, bool emptyFirstRow)
    {
      var listDestination = new DataTable();

      listDestination.Columns.Add("ForumID", typeof(int));
      listDestination.Columns.Add("Title", typeof(string));

      if (emptyFirstRow)
      {
        DataRow blankRow = listDestination.NewRow();
        blankRow["ForumID"] = 0;
        blankRow["Title"] = string.Empty;
        listDestination.Rows.Add(blankRow);
      }

      // filter the forum list
      DataView dv = listSource.DefaultView;

      if (forumidExclusions != null && forumidExclusions.Length > 0)
      {
        dv.RowFilter = string.Format("ForumID NOT IN ({0})", forumidExclusions.ToDelimitedString(","));
        dv.ApplyDefaultSort = true;
      }

      forum_sort_list_recursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);

      return listDestination;
    }

    /// <summary>
    /// The forum_listall_sorted.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forum_listall_sorted(object boardID, object userID)
    {
      return forum_listall_sorted(boardID, userID, null, false, 0);
    }

    /// <summary>
    /// The forum_listall_sorted.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumidExclusions">
    /// The forumid exclusions.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forum_listall_sorted(object boardID, object userID, int[] forumidExclusions)
    {
      return forum_listall_sorted(boardID, userID, null, false, 0);
    }

    /// <summary>
    /// The forum_listall_sorted.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumidExclusions">
    /// The forumid exclusions.
    /// </param>
    /// <param name="emptyFirstRow">
    /// The empty first row.
    /// </param>
    /// <param name="startAt">
    /// The start at.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forum_listall_sorted(object boardID, object userID, int[] forumidExclusions, bool emptyFirstRow, int startAt)
    {
      using (DataTable dataTable = forum_listall(boardID, userID, startAt))
      {
        int baseForumId = 0;
        int baseCategoryId = 0;

        if (startAt != 0)
        {
          // find the base ids...
          foreach (DataRow dataRow in dataTable.Rows)
          {
            if (Convert.ToInt32(dataRow["ForumID"]) == startAt && dataRow["ParentID"] != DBNull.Value && dataRow["CategoryID"] != DBNull.Value)
            {
              baseForumId = Convert.ToInt32(dataRow["ParentID"]);
              baseCategoryId = Convert.ToInt32(dataRow["CategoryID"]);
              break;
            }
          }
        }

        return forum_sort_list(dataTable, baseForumId, baseCategoryId, 0, forumidExclusions, emptyFirstRow);
      }
    }

    /// <summary>
    /// The forum_list_sort_basic.
    /// </summary>
    /// <param name="listsource">
    /// The listsource.
    /// </param>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <param name="parentid">
    /// The parentid.
    /// </param>
    /// <param name="currentLvl">
    /// The current lvl.
    /// </param>
    private static void forum_list_sort_basic(DataTable listsource, DataTable list, int parentid, int currentLvl)
    {
      for (int i = 0; i < listsource.Rows.Count; i++)
      {
        DataRow row = listsource.Rows[i];
        if (row["ParentID"] == DBNull.Value)
        {
          row["ParentID"] = 0;
        }

        if ((int)row["ParentID"] == parentid)
        {
          string sIndent = string.Empty;
          int iIndent = Convert.ToInt32(currentLvl);
          for (int j = 0; j < iIndent; j++)
          {
            sIndent += "--";
          }

          row["Name"] = string.Format(" -{0} {1}", sIndent, row["Name"]);
          list.Rows.Add(row.ItemArray);
          forum_list_sort_basic(listsource, list, (int)row["ForumID"], currentLvl + 1);
        }
      }
    }

    /// <summary>
    /// The forum_sort_list_recursive.
    /// </summary>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <param name="listDestination">
    /// The list destination.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="currentIndent">
    /// The current indent.
    /// </param>
    private static void forum_sort_list_recursive(DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent)
    {
      DataRow newRow;

      foreach (DataRow row in listSource.Rows)
      {
        // see if this is a root-forum
        if (row["ParentID"] == DBNull.Value)
        {
          row["ParentID"] = 0;
        }

        if ((int)row["ParentID"] == parentID)
        {
          if ((int)row["CategoryID"] != categoryID)
          {
            categoryID = (int)row["CategoryID"];

            newRow = listDestination.NewRow();
            newRow["ForumID"] = -categoryID; // Ederon : 9/4/2007
            newRow["Title"] = string.Format("{0}", row["Category"]);
            listDestination.Rows.Add(newRow);
          }

          string sIndent = string.Empty;

          for (int j = 0; j < currentIndent; j++)
          {
            sIndent += "--";
          }

          // import the row into the destination
          newRow = listDestination.NewRow();

          newRow["ForumID"] = row["ForumID"];
          newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["Forum"]);

          listDestination.Rows.Add(newRow);

          // recurse through the list...
          forum_sort_list_recursive(listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
        }
      }
    }

    #endregion

    #region ForumAccess

    /// <summary>
    /// The forumaccess_list.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forumaccess_list(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forumaccess_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The forumaccess_save.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <param name="accessMaskID">
    /// The access mask id.
    /// </param>
    public static void forumaccess_save(object forumID, object groupID, object accessMaskID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forumaccess_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The forumaccess_group.
    /// </summary>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable forumaccess_group(object groupID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("forumaccess_group"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("GroupID", groupID);
        return userforumaccess_sort_list(YafDBAccess.Current.GetData(cmd), 0, 0, 0);
      }
    }

    #endregion

    #region Group

    /// <summary>
    /// The group_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable group_list(object boardID, object groupID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("GroupID", groupID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The group_delete.
    /// </summary>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    public static void group_delete(object groupID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("GroupID", groupID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The group_member.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable group_member(object boardID, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_member"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The group_save.
    /// </summary>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="isAdmin">
    /// The is admin.
    /// </param>
    /// <param name="isGuest">
    /// The is guest.
    /// </param>
    /// <param name="isStart">
    /// The is start.
    /// </param>
    /// <param name="isModerator">
    /// The is moderator.
    /// </param>
    /// <param name="accessMaskID">
    /// The access mask id.
    /// </param>
    /// <param name="pmLimit">
    /// The pm limit.
    /// </param>
    /// <param name="style">
    /// The style.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="usrSigChars">
    /// The usrSigChars defines number of allowed characters in user signature.
    /// </param>    
    /// <param name="usrSigBBCodes">
    /// The UsrSigBBCodes.defines comma separated bbcodes allowed for a rank, i.e in a user signature 
    /// </param>
    /// <param name="usrSigHTMLTags">
    /// The UsrSigHTMLTags defines comma separated tags allowed for a rank, i.e in a user signature 
    /// </param>
    /// <param name="usrAlbums">
    /// The UsrAlbums defines allowed number of albums.
    /// </param>
    /// <param name="usrAlbumImages">
    /// The UsrAlbumImages defines number of images allowed for an album.
    /// </param>
    /// <returns>
    /// The group_save.
    /// </returns>
    public static long group_save(
      object groupID,
      object boardID,
      object name,
      object isAdmin,
      object isGuest,
      object isStart,
      object isModerator,
      object accessMaskID,
      object pmLimit,
      object style,
      object sortOrder,
      object description,
      object usrSigChars,
      object usrSigBBCodes,
      object usrSigHTMLTags,
      object usrAlbums,
      object usrAlbumImages)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("IsAdmin", isAdmin);
        cmd.Parameters.AddWithValue("IsGuest", isGuest);
        cmd.Parameters.AddWithValue("IsStart", isStart);
        cmd.Parameters.AddWithValue("IsModerator", isModerator);
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        cmd.Parameters.AddWithValue("PMLimit", pmLimit);
        cmd.Parameters.AddWithValue("Style", style);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("Description", description);
        cmd.Parameters.AddWithValue("UsrSigChars", usrSigChars);
        cmd.Parameters.AddWithValue("UsrSigBBCodes", usrSigBBCodes);
        cmd.Parameters.AddWithValue("UsrSigHTMLTags", usrSigHTMLTags);
        cmd.Parameters.AddWithValue("UsrAlbums", usrAlbums);
        cmd.Parameters.AddWithValue("UsrAlbumImages", usrAlbumImages);

        return long.Parse(YafDBAccess.Current.ExecuteScalar(cmd).ToString());
      }
    }

    #endregion

    #region Mail

    /// <summary>
    /// The mail_delete.
    /// </summary>
    /// <param name="mailID">
    /// The mail id.
    /// </param>
    public static void mail_delete(object mailID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("mail_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MailID", mailID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The mail_list.
    /// </summary>
    /// <param name="processId">
    /// The process id.
    /// </param>
    /// <returns>
    /// </returns>
    public static IEnumerable<TypedMailList> MailList(long processId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("mail_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ProcessID", processId);

        return YafDBAccess.Current.GetData(cmd).SelectTypedList(x => new TypedMailList(x));
      }
    }

    /// <summary>
    /// The mail_createwatch.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="from">
    /// The from.
    /// </param>
    /// <param name="fromName">
    /// The from name.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="bodyHtml">
    /// The body html.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void mail_createwatch(object topicID, object from, object fromName, object subject, object body, object bodyHtml, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("mail_createwatch"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("From", from);
        cmd.Parameters.AddWithValue("FromName", fromName);
        cmd.Parameters.AddWithValue("Subject", subject);
        cmd.Parameters.AddWithValue("Body", body);
        cmd.Parameters.AddWithValue("BodyHtml", bodyHtml);
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The mail_create.
    /// </summary>
    /// <param name="from">
    /// The from.
    /// </param>
    /// <param name="fromName">
    /// The from name.
    /// </param>
    /// <param name="to">
    /// The to.
    /// </param>
    /// <param name="toName">
    /// The to name.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="bodyHtml">
    /// The body html.
    /// </param>
    public static void mail_create(object from, object fromName, object to, object toName, object subject, object body, object bodyHtml)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("mail_create"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("From", from);
        cmd.Parameters.AddWithValue("FromName", fromName);
        cmd.Parameters.AddWithValue("To", to);
        cmd.Parameters.AddWithValue("ToName", toName);
        cmd.Parameters.AddWithValue("Subject", subject);
        cmd.Parameters.AddWithValue("Body", body);
        cmd.Parameters.AddWithValue("BodyHtml", bodyHtml);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Message

      /// <summary>
      /// The post_list.
      /// </summary>
      /// <param name="topicID">
      /// The topic id.
      /// </param>
      /// <param name="updateViewCount">
      /// The update view count.
      /// </param>
      /// <param name="showDeleted">
      /// The show deleted.
      /// </param>
      /// <param name="styledNicks">
      /// The styled nicks.
      /// </param>
      /// <returns>
      /// </returns>
      public static DataTable post_list(object topicID, object updateViewCount, bool showDeleted, bool styledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("post_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("UpdateViewCount", updateViewCount);
        cmd.Parameters.AddWithValue("ShowDeleted", showDeleted);
        cmd.Parameters.AddWithValue("StyledNicks", styledNicks);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The post_list_reverse 10.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable post_list_reverse10(object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("post_list_reverse10"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The post_last 10 user.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="pageUserID">
    /// The page user id.
    /// </param>
    /// <returns>
    /// </returns>
    [Obsolete("Use post_alluser() instead.")]
    public static DataTable post_last10user(object boardID, object userID, object pageUserID)
    {
      // use all posts procedure to return top ten
      return post_alluser(boardID, userID, pageUserID, 10);
    }

    /// <summary>
    /// Gets all the post by a user.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="pageUserID">
    /// The page user id.
    /// </param>
    /// <param name="topCount">
    /// Top count to return. Null is all.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable post_alluser(object boardID, object userID, object pageUserID, object topCount)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("post_alluser"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("PageUserID", pageUserID);
        cmd.Parameters.AddWithValue("topCount", topCount);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// gets list of replies to message
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_getRepliesList(object messageID)
    {
      var list = new DataTable();
      list.Columns.Add("Posted", typeof(DateTime));
      list.Columns.Add("Subject", typeof(string));
      list.Columns.Add("Message", typeof(string));
      list.Columns.Add("UserID", typeof(int));
      list.Columns.Add("Flags", typeof(int));
      list.Columns.Add("UserName", typeof(string));
      list.Columns.Add("Signature", typeof(string));

      using (SqlCommand cmd = YafDBAccess.GetCommand("message_reply_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        DataTable dtr = YafDBAccess.Current.GetData(cmd);

        for (int i = 0; i < dtr.Rows.Count; i++)
        {
          DataRow newRow = list.NewRow();
          DataRow row = dtr.Rows[i];
          newRow = list.NewRow();
          newRow["Posted"] = row["Posted"];
          newRow["Subject"] = row["Subject"];
          newRow["Message"] = row["Message"];
          newRow["UserID"] = row["UserID"];
          newRow["Flags"] = row["Flags"];
          newRow["UserName"] = row["UserName"];
          newRow["Signature"] = row["Signature"];
          list.Rows.Add(newRow);
          message_getRepliesList_populate(dtr, list, (int)row["MessageId"]);
        }

        return list;
      }
    }

    // gets list of nested replies to message
    /// <summary>
    /// The message_get replies list_populate.
    /// </summary>
    /// <param name="listsource">
    /// The listsource.
    /// </param>
    /// <param name="list">
    /// The list.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    private static void message_getRepliesList_populate(DataTable listsource, DataTable list, int messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_reply_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        DataTable dtr = YafDBAccess.Current.GetData(cmd);

        for (int i = 0; i < dtr.Rows.Count; i++)
        {
          DataRow newRow = list.NewRow();
          DataRow row = dtr.Rows[i];
          newRow = list.NewRow();
          newRow["Posted"] = row["Posted"];
          newRow["Subject"] = row["Subject"];
          newRow["Message"] = row["Message"];
          newRow["UserID"] = row["UserID"];
          newRow["Flags"] = row["Flags"];
          newRow["UserName"] = row["UserName"];
          newRow["Signature"] = row["Signature"];
          list.Rows.Add(newRow);
          message_getRepliesList_populate(dtr, list, (int)row["MessageId"]);
        }
      }
    }

    // creates new topic, using some parameters from message itself
    /// <summary>
    /// The topic_create_by_message.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="newTopicSubj">
    /// The new topic subj.
    /// </param>
    /// <returns>
    /// The topic_create_by_message.
    /// </returns>
    public static long topic_create_by_message(object messageID, object forumId, object newTopicSubj)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_create_by_message"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("ForumID", forumId);
        cmd.Parameters.AddWithValue("Subject", newTopicSubj);
        DataTable dt = YafDBAccess.Current.GetData(cmd);
        return long.Parse(dt.Rows[0]["TopicID"].ToString());
      }
    }

    /// <summary>
    /// The message_list.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    [Obsolete("Use MessageList(int messageId) instead")]
    public static DataTable message_list(object messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The message_list.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    public static IEnumerable<TypedMessageList> MessageList(int messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);

        return YafDBAccess.Current.GetData(cmd).AsEnumerable().Select(t => new TypedMessageList(t));
      }
    }

    /// <summary>
    /// The message_delete.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="isDeleteAction">
    /// The is delete action.
    /// </param>
    /// <param name="DeleteLinked">
    /// The delete linked.
    /// </param>
    public static void message_delete(object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked)
    {
      message_delete(messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false);
    }

    /// <summary>
    /// The message_delete.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="isDeleteAction">
    /// The is delete action.
    /// </param>
    /// <param name="DeleteLinked">
    /// The delete linked.
    /// </param>
    /// <param name="eraseMessage">
    /// The erase message.
    /// </param>
    public static void message_delete(object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked, bool eraseMessage)
    {
      message_deleteRecursively(messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false, eraseMessage);
    }

      /// <summary>
      /// Retrieve all reported messages with the correct forumID argument.
      /// </summary>
      /// <param name="forumID">
      /// The forum id.
      /// </param>
      /// <returns>
      /// </returns>
      public static DataTable message_listreported(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_listreported"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Here we get reporters list for a reported message
    /// </summary>
    /// <param name="messageID">
    /// The message ID.
    /// </param>
    /// <returns>
    /// Returns reporters DataTable for a reported message.
    /// </returns>
    public static DataTable message_listreporters(int messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_listreporters"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("UserID", 0);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The message_listreporters.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_listreporters(int messageID, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_listreporters"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // <summary> Save reported message back to the database. </summary>
      /// <summary>
      /// The message_report.
      /// </summary>
      /// <param name="messageID">
      /// The message id.
      /// </param>
      /// <param name="userID">
      /// The user id.
      /// </param>
      /// <param name="reportedDateTime">
      /// The reported date time.
      /// </param>
      /// <param name="reportText">
      /// The report text.
      /// </param>
      public static void message_report(object messageID, object userID, object reportedDateTime, object reportText)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_report"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("ReporterID", userID);
        cmd.Parameters.AddWithValue("ReportedDate", reportedDateTime);
        cmd.Parameters.AddWithValue("ReportText", reportText);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    // <summary> Copy current Message text over reported Message text. </summary>
    /// <summary>
    /// The message_reportcopyover.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    public static void message_reportcopyover(object messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_reportcopyover"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    // <summary> Copy current Message text over reported Message text. </summary>
    /// <summary>
    /// The message_reportresolve.
    /// </summary>
    /// <param name="messageFlag">
    /// The message flag.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void message_reportresolve(object messageFlag, object messageID, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_reportresolve"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageFlag", messageFlag);
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Delete message and all subsequent releated messages to that ID
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="isDeleteAction">
    /// The is delete action.
    /// </param>
    /// <param name="DeleteLinked">
    /// The delete linked.
    /// </param>
    /// <param name="isLinked">
    /// The is linked.
    /// </param>
    private static void message_deleteRecursively(
      object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked, bool isLinked)
    {
      message_deleteRecursively(messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, isLinked, false);
    }

    /// <summary>
    /// The message_delete recursively.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="deleteReason">
    /// The delete reason.
    /// </param>
    /// <param name="isDeleteAction">
    /// The is delete action.
    /// </param>
    /// <param name="deleteLinked">
    /// The delete linked.
    /// </param>
    /// <param name="isLinked">
    /// The is linked.
    /// </param>
    /// <param name="eraseMessages">
    /// The erase messages.
    /// </param>
    private static void message_deleteRecursively(
      object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool deleteLinked, bool isLinked, bool eraseMessages)
    {
      bool useFileTable = GetBooleanRegistryValue("UseFileTable");

      if (deleteLinked)
      {
        // Delete replies
        using (SqlCommand cmd = YafDBAccess.GetCommand("message_getReplies"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("MessageID", messageID);
          DataTable tbReplies = YafDBAccess.Current.GetData(cmd);

          foreach (DataRow row in tbReplies.Rows)
          {
            message_deleteRecursively(row["MessageID"], isModeratorChanged, deleteReason, isDeleteAction, deleteLinked, true, eraseMessages);
          }
        }
      }

      // If the files are actually saved in the Hard Drive
      if (!useFileTable)
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("attachment_list"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("MessageID", messageID);
          DataTable tbAttachments = YafDBAccess.Current.GetData(cmd);

          string uploadDir = HostingEnvironment.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

          foreach (DataRow row in tbAttachments.Rows)
          {
            try
            {
              string fileName = String.Format("{0}/{1}.{2}.yafupload", uploadDir, messageID, row["FileName"]);
              if (File.Exists(fileName))
              {
                File.Delete(fileName);
              }
            }
            catch
            {
              // error deleting that file... 
            }
          }
        }
      }

      // Ederon : erase message for good
      if (eraseMessages)
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("message_delete"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("MessageID", messageID);
          cmd.Parameters.AddWithValue("EraseMessage", eraseMessages);
          YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
      }
      else
      {
        // Delete Message
        // undelete function added
        using (SqlCommand cmd = YafDBAccess.GetCommand("message_deleteundelete"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("MessageID", messageID);
          cmd.Parameters.AddWithValue("isModeratorChanged", isModeratorChanged);
          cmd.Parameters.AddWithValue("DeleteReason", deleteReason);
          cmd.Parameters.AddWithValue("isDeleteAction", isDeleteAction);
          YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
      }
    }

    /// <summary>
    /// Set flag on message to approved and store in DB
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    public static void message_approve(object messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_approve"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Get message topic IDs (for URL rewriting)
    /// </summary>
    /// <param name="StartID">
    /// </param>
    /// <param name="Limit">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_simplelist(int StartID, int Limit)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_simplelist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("StartID", StartID);
        cmd.Parameters.AddWithValue("Limit", Limit);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // <summary> Update message to DB. </summary>
    /// <summary>
    /// The message_update.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="priority">
    /// The priority.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="reasonOfEdit">
    /// The reason of edit.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="editedBy">
    /// UserId of who edited the message.
    /// </param>
    public static void message_update(
      object messageID, object priority, object message, object subject, object flags, object reasonOfEdit, object isModeratorChanged, object origMessage, object editedBy)
    {
      message_update(messageID, priority, message, subject, flags, reasonOfEdit, isModeratorChanged, null, origMessage, editedBy);
    }

    /// <summary>
    /// The message_update.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="priority">
    /// The priority.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="reasonOfEdit">
    /// The reason of edit.
    /// </param>
    /// <param name="isModeratorChanged">
    /// The is moderator changed.
    /// </param>
    /// <param name="overrideApproval">
    /// The override approval.
    /// </param>
    /// <param name="originalMessage">
    /// The original Message.
    /// </param> 
    /// <param name="editedBy">
    /// UserId of who edited the message.
    /// </param>
    public static void message_update(
      object messageID, object priority,
        object message, object subject,
        object flags, object reasonOfEdit,
        object isModeratorChanged,
        object overrideApproval,
        object originalMessage,
        object editedBy)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("Priority", priority);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("Subject", subject);
        cmd.Parameters.AddWithValue("Flags", flags);
        cmd.Parameters.AddWithValue("Reason", reasonOfEdit);
        cmd.Parameters.AddWithValue("EditedBy", editedBy);
        cmd.Parameters.AddWithValue("IsModeratorChanged", isModeratorChanged);
        cmd.Parameters.AddWithValue("OverrideApproval", overrideApproval);
        cmd.Parameters.AddWithValue("OriginalMessage", originalMessage);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    // <summary> Save message to DB. </summary>
    /// <summary>
    /// The message_save.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="replyTo">
    /// The reply to.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The message_save.
    /// </returns>
    public static bool message_save(
      object topicID, object userID, object message, object userName, object ip, object posted, object replyTo, object flags, ref long messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_save"))
      {
        var paramMessageID = new SqlParameter("MessageID", messageID);
        paramMessageID.Direction = ParameterDirection.Output;

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("IP", ip);
        cmd.Parameters.AddWithValue("Posted", posted);
        cmd.Parameters.AddWithValue("ReplyTo", replyTo);
        cmd.Parameters.AddWithValue("BlogPostID", null); // Ederon : 6/16/2007
        cmd.Parameters.AddWithValue("Flags", flags);
        cmd.Parameters.Add(paramMessageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        messageID = (long)paramMessageID.Value;
        return true;
      }
    }

    /// <summary>
    /// The message_unapproved.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_unapproved(object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_unapproved"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The message_findunread.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="lastRead">
    /// The last read.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_findunread(object topicID, object lastRead)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_findunread"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("LastRead", lastRead);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// message movind function
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="moveToTopic">
    /// The move to topic.
    /// </param>
    /// <param name="moveAll">
    /// The move all.
    /// </param>
    public static void message_move(object messageID, object moveToTopic, bool moveAll)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_move"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("MoveToTopic", moveToTopic);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }

      // moveAll=true anyway
      // it's in charge of moving answers of moved post
      if (moveAll)
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("message_getReplies"))
        {
          cmd.CommandType = CommandType.StoredProcedure;
          cmd.Parameters.AddWithValue("MessageID", messageID);
          DataTable tbReplies = YafDBAccess.Current.GetData(cmd);
          foreach (DataRow row in tbReplies.Rows)
          {
            message_moveRecursively(row["MessageID"], moveToTopic);
          }
        }
      }
    }

    /// <summary>
    /// moves answers of moved post
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="moveToTopic">
    /// The move to topic.
    /// </param>
    private static void message_moveRecursively(object messageID, object moveToTopic)
    {
      bool UseFileTable = GetBooleanRegistryValue("UseFileTable");

      // Delete replies
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_getReplies"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        DataTable tbReplies = YafDBAccess.Current.GetData(cmd);
        foreach (DataRow row in tbReplies.Rows)
        {
          message_moveRecursively(row["messageID"], moveToTopic);
        }

        using (SqlCommand innercmd = YafDBAccess.GetCommand("message_move"))
        {
          innercmd.CommandType = CommandType.StoredProcedure;
          innercmd.Parameters.AddWithValue("MessageID", messageID);
          innercmd.Parameters.AddWithValue("MoveToTopic", moveToTopic);
          YafDBAccess.Current.ExecuteNonQuery(innercmd);
        }
      }
    }

    // functions for Thanks feature

    // <summary> Return the number of times the message with the provided messageID
    // has been thanked. </summary>
    /// <summary>
    /// The message_ thanks number.
    /// </summary>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The message_ thanks number.
    /// </returns>
    public static int message_ThanksNumber(object messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_thanksnumber"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter();
        paramOutput.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return Convert.ToInt32(paramOutput.Value);
      }
    }

    /// <summary>
    /// Returns the UserIDs and UserNames who have thanked the message
    /// with the provided messageID.
    /// </summary>
    /// <param name="MessageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_GetThanks(object MessageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_getthanks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", MessageID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Retuns All the message text for the Message IDs which are in the 
    /// delimited string variable MessageIDs
    /// </summary>
    /// <param name="messageIDs">
    /// The message i ds.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable message_GetTextByIds(string messageIDs)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_gettextbyids"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageIDs", messageIDs);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Retuns All the Thanks for the Message IDs which are in the 
    /// delimited string variable MessageIDs
    /// </summary>
    /// <param name="messageIdsSeparatedWithColon">
    /// The message i ds.
    /// </param>
    /// <returns>
    /// </returns>
    public static IEnumerable<TypedAllThanks> MessageGetAllThanks(string messageIdsSeparatedWithColon)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_getallthanks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageIDs", messageIdsSeparatedWithColon);

        return YafDBAccess.Current.GetData(cmd).AsEnumerable().Select(t => new TypedAllThanks(t));
      }
    }

    /// <summary>
    /// The message_ add thanks.
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="MessageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The message_ add thanks.
    /// </returns>
    public static string message_AddThanks(object FromUserID, object MessageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_Addthanks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("MessageID", MessageID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

    /// <summary>
    /// The message_ remove thanks.
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="MessageID">
    /// The message id.
    /// </param>
    /// <returns>
    /// The message_ remove thanks.
    /// </returns>
    public static string message_RemoveThanks(object FromUserID, object MessageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_Removethanks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("MessageID", MessageID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

    /// <summary>
    /// The messagehistory_list.
    /// </summary>
    /// <param name="messageID">
    /// The Message ID.
    /// </param>
    /// <param name="daysToClean">
    /// Days to clean.
    /// </param>
    /// <param name="showAll">
    /// The Show All.
    /// </param>
    /// <returns>
    /// List of all message changes. 
    /// </returns>
    public static DataTable messagehistory_list(int messageID, int daysToClean, object showAll)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("messagehistory_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("DaysToClean", daysToClean);
        cmd.Parameters.AddWithValue("ShowAll", showAll);
        return YafDBAccess.Current.GetData(cmd);
      }
    }


    /// <summary>
    /// Returns message data based on user access rights
    /// </summary>
    /// <param name="MessageID">The Message Id.</param>
    /// <param name="UserID">The UserId.</param>
    /// <returns></returns>
    public static DataTable message_secdata(int MessageID, object UserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("message_secdata"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", UserID);
        cmd.Parameters.AddWithValue("MessageID", MessageID);

        return YafDBAccess.Current.GetData(cmd);

      }
    }



    #endregion

    #region Medal

    /// <summary>
    /// Lists given medal.
    /// </summary>
    /// <param name="medalID">
    /// ID of medal to list.
    /// </param>
    public static DataTable medal_list(object medalID)
    {
      return medal_list(null, medalID, null);
    }

    /// <summary>
    /// Lists given medals.
    /// </summary>
    /// <param name="boardID">
    /// ID of board of which medals to list. Required.
    /// </param>
    /// <param name="category">
    /// Cateogry of medals to list. Can be null. In such case this parameter is ignored.
    /// </param>
    public static DataTable medal_list(object boardID, object category)
    {
      return medal_list(boardID, null, category);
    }

    /// <summary>
    /// Lists medals.
    /// </summary>
    /// <param name="boardID">
    /// ID of board of which medals to list. Can be null if medalID parameter is specified.
    /// </param>
    /// <param name="medalID">
    /// ID of medal to list. When specified, boardID and category parameters are ignored.
    /// </param>
    /// <param name="category">
    /// Cateogry of medals to list. Must be complemented with not-null boardID parameter.
    /// </param>
    private static DataTable medal_list(object boardID, object medalID, object category)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("medal_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Category", category);

        return YafDBAccess.Current.GetData(cmd);
      }
    }


    /// <summary>
    /// List users who own this medal.
    /// </summary>
    /// <param name="medalID">
    /// Medal of which owners to get.
    /// </param>
    /// <returns>
    /// List of users with their user id and usernames, who own this medal.
    /// </returns>
    public static DataTable medal_listusers(object medalID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("medal_listusers"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("MedalID", medalID);

        return YafDBAccess.Current.GetData(cmd);
      }
    }


    /// <summary>
    /// Deletes given medal.
    /// </summary>
    /// <param name="medalID">
    /// ID of medal to delete.
    /// </param>
    public static void medal_delete(object medalID)
    {
      medal_delete(null, medalID, null);
    }

    /// <summary>
    /// Deletes given medals.
    /// </summary>
    /// <param name="boardID">
    /// ID of board of which medals to delete. Required.
    /// </param>
    /// <param name="category">
    /// Cateogry of medals to delete. Can be null. In such case this parameter is ignored.
    /// </param>
    public static void medal_delete(object boardID, object category)
    {
      medal_delete(boardID, null, category);
    }

    /// <summary>
    /// Deletes medals.
    /// </summary>
    /// <param name="boardID">
    /// ID of board of which medals to delete. Can be null if medalID parameter is specified.
    /// </param>
    /// <param name="medalID">
    /// ID of medal to delete. When specified, boardID and category parameters are ignored.
    /// </param>
    /// <param name="category">
    /// Cateogry of medals to delete. Must be complemented with not-null boardID parameter.
    /// </param>
    private static void medal_delete(object boardID, object medalID, object category)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("medal_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Category", category);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// The medal_save.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="medalID">
    /// The medal id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="category">
    /// The category.
    /// </param>
    /// <param name="medalURL">
    /// The medal url.
    /// </param>
    /// <param name="ribbonURL">
    /// The ribbon url.
    /// </param>
    /// <param name="smallMedalURL">
    /// The small medal url.
    /// </param>
    /// <param name="smallRibbonURL">
    /// The small ribbon url.
    /// </param>
    /// <param name="smallMedalWidth">
    /// The small medal width.
    /// </param>
    /// <param name="smallMedalHeight">
    /// The small medal height.
    /// </param>
    /// <param name="smallRibbonWidth">
    /// The small ribbon width.
    /// </param>
    /// <param name="smallRibbonHeight">
    /// The small ribbon height.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <returns>
    /// The medal_save.
    /// </returns>
    public static bool medal_save(
      object boardID,
      object medalID,
      object name,
      object description,
      object message,
      object category,
      object medalURL,
      object ribbonURL,
      object smallMedalURL,
      object smallRibbonURL,
      object smallMedalWidth,
      object smallMedalHeight,
      object smallRibbonWidth,
      object smallRibbonHeight,
      object sortOrder,
      object flags)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("medal_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Description", description);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("Category", category);
        cmd.Parameters.AddWithValue("MedalURL", medalURL);
        cmd.Parameters.AddWithValue("RibbonURL", ribbonURL);
        cmd.Parameters.AddWithValue("SmallMedalURL", smallMedalURL);
        cmd.Parameters.AddWithValue("SmallRibbonURL", smallRibbonURL);
        cmd.Parameters.AddWithValue("SmallMedalWidth", smallMedalWidth);
        cmd.Parameters.AddWithValue("SmallMedalHeight", smallMedalHeight);
        cmd.Parameters.AddWithValue("SmallRibbonWidth", smallRibbonWidth);
        cmd.Parameters.AddWithValue("SmallRibbonHeight", smallRibbonHeight);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("Flags", flags);

        // command succeeded if returned value is greater than zero (number of affected rows)
        return (int)YafDBAccess.Current.ExecuteScalar(cmd) > 0;
      }
    }


    /// <summary>
    /// Changes medal's sort order.
    /// </summary>
    /// <param name="boardID">
    /// ID of board.
    /// </param>
    /// <param name="medalID">
    /// ID of medal to re-sort.
    /// </param>
    /// <param name="move">
    /// Change of sort.
    /// </param>
    public static void medal_resort(object boardID, object medalID, int move)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("medal_resort"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Move", move);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// Deletes medal allocation to a group.
    /// </summary>
    /// <param name="groupID">
    /// ID of group owning medal.
    /// </param>
    /// <param name="medalID">
    /// ID of medal.
    /// </param>
    public static void group_medal_delete(object groupID, object medalID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_medal_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("MedalID", medalID);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// Lists medal(s) assigned to the group
    /// </summary>
    /// <param name="groupID">
    /// ID of group of which to list medals.
    /// </param>
    /// <param name="medalID">
    /// ID of medal to list.
    /// </param>
    public static DataTable group_medal_list(object groupID, object medalID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_medal_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("MedalID", medalID);

        return YafDBAccess.Current.GetData(cmd);
      }
    }


    /// <summary>
    /// Saves new or update existing group-medal allocation.
    /// </summary>
    /// <param name="groupID">
    /// ID of user group.
    /// </param>
    /// <param name="medalID">
    /// ID of medal.
    /// </param>
    /// <param name="message">
    /// Medal message, to override medal's default one. Can be null.
    /// </param>
    /// <param name="hide">
    /// Hide medal in user box.
    /// </param>
    /// <param name="onlyRibbon">
    /// Show only ribbon bar in user box.
    /// </param>
    /// <param name="sortOrder">
    /// Sort order in user box. Overrides medal's default sort order.
    /// </param>
    public static void group_medal_save(object groupID, object medalID, object message, object hide, object onlyRibbon, object sortOrder)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_medal_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("Hide", hide);
        cmd.Parameters.AddWithValue("OnlyRibbon", onlyRibbon);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// Deletes medal allocation to a user.
    /// </summary>
    /// <param name="userID">
    /// ID of user owning medal.
    /// </param>
    /// <param name="medalID">
    /// ID of medal.
    /// </param>
    public static void user_medal_delete(object userID, object medalID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_medal_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("MedalID", medalID);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// Lists medal(s) assigned to the group
    /// </summary>
    /// <param name="userID">
    /// ID of user who was given medal.
    /// </param>
    /// <param name="medalID">
    /// ID of medal to list.
    /// </param>
    public static DataTable user_medal_list(object userID, object medalID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_medal_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("MedalID", medalID);

        return YafDBAccess.Current.GetData(cmd);
      }
    }


    /// <summary>
    /// Saves new or update existing user-medal allocation.
    /// </summary>
    /// <param name="userID">
    /// ID of user.
    /// </param>
    /// <param name="medalID">
    /// ID of medal.
    /// </param>
    /// <param name="message">
    /// Medal message, to override medal's default one. Can be null.
    /// </param>
    /// <param name="hide">
    /// Hide medal in user box.
    /// </param>
    /// <param name="onlyRibbon">
    /// Show only ribbon bar in user box.
    /// </param>
    /// <param name="sortOrder">
    /// Sort order in user box. Overrides medal's default sort order.
    /// </param>
    /// <param name="dateAwarded">
    /// Date when medal was awarded to a user. Is ignored when existing user-medal allocation is edited.
    /// </param>
    public static void user_medal_save(object userID, object medalID, object message, object hide, object onlyRibbon, object sortOrder, object dateAwarded)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_medal_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("MedalID", medalID);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("Hide", hide);
        cmd.Parameters.AddWithValue("OnlyRibbon", onlyRibbon);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("DateAwarded", dateAwarded);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }


    /// <summary>
    /// Lists all medals held by user as they are to be shown in user box.
    /// </summary>
    /// <param name="userID">
    /// ID of user.
    /// </param>
    /// <returns>
    /// List of medals, ribbon bar only first.
    /// </returns>
    public static DataTable user_listmedals(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_listmedals"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", userID);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region NntpForum

    /// <summary>
    /// The nntpforum_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="minutes">
    /// The minutes.
    /// </param>
    /// <param name="nntpForumID">
    /// The nntp forum id.
    /// </param>
    /// <param name="active">
    /// The active.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable nntpforum_list(object boardID, object minutes, object nntpForumID, object active)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpforum_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Minutes", minutes);
        cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
        cmd.Parameters.AddWithValue("Active", active);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The nntpforum_update.
    /// </summary>
    /// <param name="nntpForumID">
    /// The nntp forum id.
    /// </param>
    /// <param name="lastMessageNo">
    /// The last message no.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void nntpforum_update(object nntpForumID, object lastMessageNo, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpforum_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
        cmd.Parameters.AddWithValue("LastMessageNo", lastMessageNo);
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The nntpforum_save.
    /// </summary>
    /// <param name="nntpForumID">
    /// The nntp forum id.
    /// </param>
    /// <param name="nntpServerID">
    /// The nntp server id.
    /// </param>
    /// <param name="groupName">
    /// The group name.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="active">
    /// The active.
    /// </param>
    public static void nntpforum_save(object nntpForumID, object nntpServerID, object groupName, object forumID, object active)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpforum_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
        cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
        cmd.Parameters.AddWithValue("GroupName", groupName);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("Active", active);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The nntpforum_delete.
    /// </summary>
    /// <param name="nntpForumID">
    /// The nntp forum id.
    /// </param>
    public static void nntpforum_delete(object nntpForumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpforum_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region NntpServer

    /// <summary>
    /// The nntpserver_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="nntpServerID">
    /// The nntp server id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable nntpserver_list(object boardID, object nntpServerID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpserver_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The nntpserver_save.
    /// </summary>
    /// <param name="nntpServerID">
    /// The nntp server id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="address">
    /// The address.
    /// </param>
    /// <param name="port">
    /// The port.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userPass">
    /// The user pass.
    /// </param>
    public static void nntpserver_save(object nntpServerID, object boardID, object name, object address, object port, object userName, object userPass)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpserver_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Address", address);
        cmd.Parameters.AddWithValue("Port", port);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("UserPass", userPass);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The nntpserver_delete.
    /// </summary>
    /// <param name="nntpServerID">
    /// The nntp server id.
    /// </param>
    public static void nntpserver_delete(object nntpServerID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntpserver_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region NntpTopic

    /// <summary>
    /// The nntptopic_list.
    /// </summary>
    /// <param name="thread">
    /// The thread.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable nntptopic_list(object thread)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntptopic_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Thread", thread);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The nntptopic_savemessage.
    /// </summary>
    /// <param name="nntpForumID">
    /// The nntp forum id.
    /// </param>
    /// <param name="topic">
    /// The topic.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <param name="posted">
    /// The posted.
    /// </param>
    /// <param name="thread">
    /// The thread.
    /// </param>
    public static void nntptopic_savemessage(
      object nntpForumID, object topic, object body, object userID, object userName, object ip, object posted, object thread)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("nntptopic_savemessage"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
        cmd.Parameters.AddWithValue("Topic", topic);
        cmd.Parameters.AddWithValue("Body", body);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("IP", ip);
        cmd.Parameters.AddWithValue("Posted", posted);
        cmd.Parameters.AddWithValue("Thread", thread);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region PMessage

    /// <summary>
    /// Returns a list of private messages based on the arguments specified.
    /// If pMessageID != null, returns the PM of id pMessageId.
    /// If toUserID != null, returns the list of PMs sent to the user with the given ID.
    /// If fromUserID != null, returns the list of PMs sent by the user of the given ID.
    /// </summary>
    /// <param name="toUserID">
    /// </param>
    /// <param name="fromUserID">
    /// </param>
    /// <param name="userPMessageID">
    /// The user P Message ID.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable pmessage_list(object toUserID, object fromUserID, object userPMessageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ToUserID", toUserID);
        cmd.Parameters.AddWithValue("FromUserID", fromUserID);
        cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Returns a list of private messages based on the arguments specified.
    /// If pMessageID != null, returns the PM of id pMessageId.
    /// If toUserID != null, returns the list of PMs sent to the user with the given ID.
    /// If fromUserID != null, returns the list of PMs sent by the user of the given ID.
    /// </summary>
    /// <param name="userPMessageID">
    /// The user P Message ID.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable pmessage_list(object userPMessageID)
    {
      return pmessage_list(null, null, userPMessageID);
    }

    /// <summary>
    /// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
    /// the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
    /// </summary>
    /// <param name="userPMessageID">
    /// The user P Message ID.
    /// </param>
    /// <param name="fromOutbox">
    /// If true, removes the message from the outbox.  Otherwise deletes the message completely.
    /// </param>
    public static void pmessage_delete(object userPMessageID, bool fromOutbox)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
        cmd.Parameters.AddWithValue("FromOutbox", fromOutbox);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Deletes the private message from the database as per the given parameter.  If fromOutbox is true,
    /// the message is only deleted from the user's outbox.  Otherwise, it is completely delete from the database.
    /// </summary>
    /// <param name="userPMessageID">
    /// </param>
    public static void pmessage_delete(object userPMessageID)
    {
      pmessage_delete(userPMessageID, false);
    }

    /// <summary>
    /// Archives the private message of the given id.  Archiving moves the message from the user's inbox to his message archive.
    /// </summary>
    /// <param name="userPMessageID">
    /// The user P Message ID.
    /// </param>
    public static void pmessage_archive(object userPMessageID)
    {
      using (SqlCommand sqlCommand = YafDBAccess.GetCommand("pmessage_archive"))
      {
        sqlCommand.CommandType = CommandType.StoredProcedure;
        sqlCommand.Parameters.AddWithValue("UserPMessageID", userPMessageID);
        YafDBAccess.Current.ExecuteNonQuery(sqlCommand);
      }
    }

    /// <summary>
    /// The pmessage_save.
    /// </summary>
    /// <param name="fromUserID">
    /// The from user id.
    /// </param>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <param name="subject">
    /// The subject.
    /// </param>
    /// <param name="body">
    /// The body.
    /// </param>
    /// <param name="Flags">
    /// The flags.
    /// </param>
    public static void pmessage_save(object fromUserID, object toUserID, object subject, object body, object Flags)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("FromUserID", fromUserID);
        cmd.Parameters.AddWithValue("ToUserID", toUserID);
        cmd.Parameters.AddWithValue("Subject", subject);
        cmd.Parameters.AddWithValue("Body", body);
        cmd.Parameters.AddWithValue("Flags", Flags);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The pmessage_markread.
    /// </summary>
    /// <param name="userPMessageID">
    /// The user p message id.
    /// </param>
    public static void pmessage_markread(object userPMessageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_markread"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The pmessage_info.
    /// </summary>
    /// <returns>
    /// </returns>
    public static DataTable pmessage_info()
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_info"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The pmessage_prune.
    /// </summary>
    /// <param name="daysRead">
    /// The days read.
    /// </param>
    /// <param name="daysUnread">
    /// The days unread.
    /// </param>
    public static void pmessage_prune(object daysRead, object daysUnread)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pmessage_prune"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("DaysRead", daysRead);
        cmd.Parameters.AddWithValue("DaysUnread", daysUnread);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Poll

    /// <summary>
    /// The pollgroup_stats.
    /// </summary>
    /// <param name="pollGroupId">
    /// The poll group id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable pollgroup_stats(int? pollGroupId)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("pollgroup_stats"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
            return YafDBAccess.Current.GetData(cmd);
        }
    }

    /// <summary>
    /// The pollgroup_attach.
    /// </summary>
    /// <param name="pollGroupId">
    /// The poll group id.
    /// </param>
    /// <returns>
    /// </returns>
    public static int pollgroup_attach(int? pollGroupId, int? topicId, int? forumId, int? categoryId, int? boardId)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("pollgroup_attach"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
            cmd.Parameters.AddWithValue("TopicID", topicId);
            cmd.Parameters.AddWithValue("ForumID", forumId);
            cmd.Parameters.AddWithValue("CategoryID", categoryId);
            cmd.Parameters.AddWithValue("BoardID", boardId);
            return (int)YafDBAccess.Current.ExecuteScalar(cmd);
        }
    }


    /// <summary>
    /// The poll_stats.
    /// </summary>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable poll_stats(int? pollId)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("poll_stats"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("PollID", pollId);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

      /// <summary>
    /// The method saves many questions and answers to them in a single transaction 
    /// </summary>
    /// <param name="pollList">List to hold all polls data</param>
    /// <returns>Last saved poll id.</returns>
    public static int? poll_save(List<PollSaveList> pollList)
    {

      foreach (PollSaveList question in pollList)
      {
          StringBuilder sb = new StringBuilder();

            // Check if the group already exists
           if (question.TopicId > 0)
           {
               sb.Append("select @PollGroupID = PollID  from ");
               sb.Append(YafDBAccess.GetObjectName("Topic"));
               sb.Append("WHERE TopicID = @TopicID; ");
           }
           else if (question.ForumId > 0)
           {

               sb.Append("select @PollGroupID = PollGroupID  from ");
               sb.Append(YafDBAccess.GetObjectName("Forum"));
               sb.Append("WHERE ForumID = @ForumID");
           }
           else if (question.CategoryId > 0)
           {

               sb.Append("select @PollGroupID = PollGroupID  from ");
               sb.Append(YafDBAccess.GetObjectName("Category"));
               sb.Append("WHERE CategoryID = @CategoryID");
           }

          // the group doesn't exists, create a new one

           sb.Append("IF @PollGroupID IS NULL BEGIN INSERT INTO ");
           sb.Append(YafDBAccess.GetObjectName("PollGroupCluster"));
           sb.Append("(UserID,Flags ) VALUES(@UserID, @Flags) SET @NewPollGroupID = SCOPE_IDENTITY(); END; ");

        
          sb.Append("INSERT INTO ");
        sb.Append(YafDBAccess.GetObjectName("Poll"));

        if (question.Closes > DateTime.MinValue)
        {
            sb.Append("(Question,Closes, UserID,PollGroupID,ObjectPath,MimeType) ");
        }
        else
        {
            sb.Append("(Question,UserID, PollGroupID, ObjectPath, MimeType) ");
        }

        sb.Append(" VALUES(");
        sb.Append("@Question");

        if (question.Closes > DateTime.MinValue)
        {
          sb.Append(",@Closes");
        }
        sb.Append(",@UserID, (CASE WHEN  @NewPollGroupID IS NULL THEN @PollGroupID ELSE @NewPollGroupID END), @QuestionObjectPath,@QuestionMimeType");
        sb.Append("); ");
        sb.Append("SET @PollID = SCOPE_IDENTITY(); ");
     
            
      

        // The cycle through question reply choices

        for (uint choiceCount = 0; choiceCount < question.Choice.GetUpperBound(1); choiceCount++)
        {
            if (!string.IsNullOrEmpty(question.Choice[0,choiceCount]))
            {
                sb.Append("INSERT INTO ");
                sb.Append(YafDBAccess.GetObjectName("Choice"));
                sb.Append("(PollID,Choice,Votes,ObjectPath,MimeType) VALUES (");
                sb.AppendFormat("@PollID,@Choice{0},@Votes{0},@ChoiceObjectPath{0}, @ChoiceMimeType{0}", choiceCount);
                sb.Append("); ");

            }
        }
     
       
        // we don't update if no new group is created 
        sb.Append("IF  @PollGroupID IS NULL BEGIN  ");
         // fill a pollgroup field - double work if a poll exists 
        if (question.TopicId > 0)
        {
          
            sb.Append("UPDATE ");
            sb.Append(YafDBAccess.GetObjectName("Topic"));
            sb.Append(" SET PollID = @NewPollGroupID WHERE TopicID = @TopicID; ");
           
        }

        // fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
        if (question.ForumId > 0)
        {
            sb.Append("UPDATE ");
            sb.Append(YafDBAccess.GetObjectName("Forum"));
            sb.Append(" SET PollGroupID= @NewPollGroupID WHERE ForumID= @ForumID; ");
        }

        // fill a pollgroup field in Category Table if the call comes from a category's topic list 
        if (question.CategoryId > 0)
        {
            sb.Append("UPDATE ");
            sb.Append(YafDBAccess.GetObjectName("Category"));
            sb.Append(" SET PollGroupID= @NewPollGroupID WHERE CategoryID= @CategoryID; ");
        }

        // fill a pollgroup field in Board Table if the call comes from the main page poll 
  
        sb.Append("END;  ");

        using (SqlCommand cmd = YafDBAccess.GetCommand(sb.ToString(), true))
        {
          SqlParameter ret = new SqlParameter();
          ret.ParameterName = "@PollID";
          ret.SqlDbType = SqlDbType.Int;
          ret.Direction = ParameterDirection.Output;
          cmd.Parameters.Add(ret);

          SqlParameter ret2 = new SqlParameter();
          ret2.ParameterName = "@PollGroupID";
          ret2.SqlDbType = SqlDbType.Int;
          ret2.Direction = ParameterDirection.Output;
          cmd.Parameters.Add(ret2);

          SqlParameter ret3 = new SqlParameter();
          ret3.ParameterName = "@NewPollGroupID";
          ret3.SqlDbType = SqlDbType.Int;
          ret3.Direction = ParameterDirection.Output;
          cmd.Parameters.Add(ret3);

          cmd.Parameters.AddWithValue("@Question", question.Question);

          if (question.Closes > DateTime.MinValue)
          {
            cmd.Parameters.AddWithValue("@Closes", question.Closes);
          }
            // set poll group flags
            int groupFlags = 0;
            if (question.IsBound)
            {
                groupFlags = groupFlags | 2;
            }
                 if (question.IsClosedBound)
            {
                groupFlags = groupFlags | 4;
            }
          cmd.Parameters.AddWithValue("@UserID", question.UserId);
          cmd.Parameters.AddWithValue("@Flags", groupFlags);
          cmd.Parameters.AddWithValue("@QuestionObjectPath",
                                          string.IsNullOrEmpty(question.QuestionObjectPath)
                                              ? String.Empty
                                              : question.QuestionObjectPath);
          cmd.Parameters.AddWithValue("@QuestionMimeType",
                                          string.IsNullOrEmpty(question.QuestionMimeType)
                                              ? String.Empty
                                              : question.QuestionMimeType);
         
            for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetUpperBound(1); choiceCount1++)
          {
            if (!string.IsNullOrEmpty(question.Choice[0,choiceCount1]))
            {
              cmd.Parameters.AddWithValue(String.Format("@Choice{0}", choiceCount1), question.Choice[0,choiceCount1]);
              cmd.Parameters.AddWithValue(String.Format("@Votes{0}", choiceCount1), 0);
           
                  cmd.Parameters.AddWithValue(String.Format("@ChoiceObjectPath{0}", choiceCount1),
                      question.Choice[1, choiceCount1].IsNotSet() ? String.Empty : question.Choice[1, choiceCount1]);
                  cmd.Parameters.AddWithValue(String.Format("@ChoiceMimeType{0}", choiceCount1),
                                               question.Choice[2, choiceCount1].IsNotSet() ? String.Empty : question.Choice[2, choiceCount1]);
              

            }
          }

          


              if (question.TopicId > 0)
              {
                  cmd.Parameters.AddWithValue("@TopicID", question.TopicId);
              }
              if (question.ForumId > 0)
              {
                  cmd.Parameters.AddWithValue("@ForumID", question.ForumId);
              }
              if (question.CategoryId > 0)
              {
                  cmd.Parameters.AddWithValue("@CategoryID", question.CategoryId);
              }

              YafDBAccess.Current.ExecuteNonQuery(cmd, true);
              if (ret.Value != DBNull.Value)
              {
                  return (int?) ret.Value;
              }
         
      }
      }
      return null;
    }

    /// <summary>
    /// The poll_update.
    /// </summary>
    /// <param name="pollID">
    /// The poll id.
    /// </param>
    /// <param name="question">
    /// The question.
    /// </param>
    /// <param name="closes">
    /// The closes.
    /// </param>
    public static void poll_update(object pollID, object question, object closes, object isBounded, bool isClosedBounded, object questionPath, object questionMime)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("poll_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("PollID", pollID);
        cmd.Parameters.AddWithValue("Question", question);
        cmd.Parameters.AddWithValue("Closes", closes);
        cmd.Parameters.AddWithValue("QuestionObjectPath", questionPath);
        cmd.Parameters.AddWithValue("QuestionMimeType", questionMime);
        cmd.Parameters.AddWithValue("IsBounded", isBounded);
        cmd.Parameters.AddWithValue("IsClosedBounded", isClosedBounded);
       

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The poll_remove.
    /// </summary>
    /// <param name="pollGroupID">
    /// The poll group id. The parameter should always be present. 
    /// </param>
    /// <param name="pollID">
    /// The poll id. If null all polls in a group a deleted. 
    /// </param>
    /// <param name="boardId">
    /// The BoardID id. 
    /// </param>
    /// <param name="removeCompletely">
    /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely, 
    /// else only one poll is deleted completely. 
    /// </param>
    public static void poll_remove(object pollGroupID, object pollID, object boardId, bool removeCompletely, bool removeEverywhere)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("poll_remove"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("PollGroupID", pollGroupID);
        cmd.Parameters.AddWithValue("PollID", pollID);
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("RemoveCompletely", removeCompletely);
        cmd.Parameters.AddWithValue("RemoveEverywhere", removeEverywhere);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Gets a typed poll group list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// </returns>
    public static IEnumerable<TypedPollGroup> PollGroupList(int userID, int? forumId, int boardId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("pollgroup_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumId);
        cmd.Parameters.AddWithValue("BoardID", boardId);

        return YafDBAccess.Current.GetData(cmd).AsEnumerable().Select(r => new TypedPollGroup(r));
      }
    }

      /// <summary>
      /// The poll_remove.
      /// </summary>
      /// <param name="pollGroupID">
      /// The poll group id. The parameter should always be present. 
      /// </param>
      /// <param name="topicId">
      /// The poll id. If null all polls in a group a deleted. 
      /// </param>
      /// <param name="boardId">
      /// The BoardID id. 
      /// </param>
      /// <param name="removeCompletely">
      /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely, 
      /// else only one poll is deleted completely. 
      /// </param>
      /// <param name="forumId"></param>
      /// <param name="removeEverywhere"></param>
      public static void pollgroup_remove(object pollGroupID, object topicId, object forumId, object categoryId, object boardId, bool removeCompletely, bool removeEverywhere)
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("pollgroup_remove"))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("PollGroupID", pollGroupID);
            cmd.Parameters.AddWithValue("TopicID", topicId);
            cmd.Parameters.AddWithValue("ForumID", forumId);
            cmd.Parameters.AddWithValue("CategoryID", categoryId);
            cmd.Parameters.AddWithValue("BoardID", boardId);
            cmd.Parameters.AddWithValue("RemoveCompletely", removeCompletely);
            cmd.Parameters.AddWithValue("RemoveEverywhere", removeEverywhere);
            YafDBAccess.Current.ExecuteNonQuery(cmd);
        }
    }



    /// <summary>
    /// The choice_delete.
    /// </summary>
    /// <param name="choiceID">
    /// The choice id.
    /// </param>
    public static void choice_delete(object choiceID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("choice_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ChoiceID", choiceID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The choice_update.
    /// </summary>
    /// <param name="choiceID">
    /// The choice id.
    /// </param>
    /// <param name="choice">
    /// The choice.
    /// </param>
    public static void choice_update(object choiceID, object choice, object path, object mime)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("choice_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ChoiceID", choiceID);
        cmd.Parameters.AddWithValue("Choice", choice);
        cmd.Parameters.AddWithValue("ObjectPath", path);
        cmd.Parameters.AddWithValue("MimeType", mime);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The choice_add.
    /// </summary>
    /// <param name="pollID">
    /// The poll id.
    /// </param>
    /// <param name="choice">
    /// The choice.
    /// </param>
    public static void choice_add(object pollID, object choice, object path, object mime)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("choice_add"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("PollID", pollID);
        cmd.Parameters.AddWithValue("Choice", choice);
        cmd.Parameters.AddWithValue("ObjectPath", path);
        cmd.Parameters.AddWithValue("MimeType", mime);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Rank

    /// <summary>
    /// The rank_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable rank_list(object boardID, object rankID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("rank_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("RankID", rankID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The rank_save.
    /// </summary>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="isStart">
    /// The is start.
    /// </param>
    /// <param name="isLadder">
    /// The is ladder.
    /// </param>
    /// <param name="minPosts">
    /// The min posts.
    /// </param>
    /// <param name="rankImage">
    /// The rank image.
    /// </param>
    /// <param name="pmLimit">
    /// The pm limit.
    /// </param>
    /// <param name="style">
    /// The style.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="usrSigChars">
    /// The usrSigChars defines number of allowed characters in user signature.
    /// </param>    
    /// <param name="usrSigBBCodes">
    /// The UsrSigBBCodes.defines comma separated bbcodes allowed for a rank, i.e in a user signature 
    /// </param>
    /// <param name="usrSigHTMLTags">
    /// The UsrSigHTMLTags defines comma separated tags allowed for a rank, i.e in a user signature 
    /// </param>
    /// <param name="usrAlbums">
    /// The UsrAlbums defines allowed number of albums.
    /// </param>
    /// <param name="usrAlbumImages">
    /// The UsrAlbumImages defines number of images allowed for an album.
    /// </param>
    /// <returns>
    /// The group_save.
    /// </returns>   
    public static void rank_save(
      object rankID,
      object boardID,
      object name,
      object isStart,
      object isLadder,
      object minPosts,
      object rankImage,
      object pmLimit,
      object style,
      object sortOrder,
      object description,
      object usrSigChars,
      object usrSigBBCodes,
      object usrSigHTMLTags,
      object usrAlbums,
      object usrAlbumImages)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("rank_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("RankID", rankID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("IsStart", isStart);
        cmd.Parameters.AddWithValue("IsLadder", isLadder);
        cmd.Parameters.AddWithValue("MinPosts", minPosts);
        cmd.Parameters.AddWithValue("RankImage", rankImage);
        cmd.Parameters.AddWithValue("PMLimit", pmLimit);
        cmd.Parameters.AddWithValue("Style", style);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("Description", description);
        cmd.Parameters.AddWithValue("UsrSigChars", usrSigChars);
        cmd.Parameters.AddWithValue("UsrSigBBCodes", usrSigBBCodes);
        cmd.Parameters.AddWithValue("UsrSigHTMLTags", usrSigHTMLTags);
        cmd.Parameters.AddWithValue("UsrAlbums", usrAlbums);
        cmd.Parameters.AddWithValue("UsrAlbumImages", usrAlbumImages);

        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The rank_delete.
    /// </summary>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    public static void rank_delete(object rankID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("rank_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("RankID", rankID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Smiley

    /// <summary>
    /// The smiley_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="smileyID">
    /// The smiley id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable smiley_list(object boardID, object smileyID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("smiley_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("SmileyID", smileyID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The smiley_listunique.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable smiley_listunique(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("smiley_listunique"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The smiley_delete.
    /// </summary>
    /// <param name="smileyID">
    /// The smiley id.
    /// </param>
    public static void smiley_delete(object smileyID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("smiley_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("SmileyID", smileyID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The smiley_save.
    /// </summary>
    /// <param name="smileyID">
    /// The smiley id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    /// <param name="emoticon">
    /// The emoticon.
    /// </param>
    /// <param name="sortOrder">
    /// The sort order.
    /// </param>
    /// <param name="replace">
    /// The replace.
    /// </param>
    public static void smiley_save(object smileyID, object boardID, object code, object icon, object emoticon, object sortOrder, object replace)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("smiley_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("SmileyID", smileyID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Code", code);
        cmd.Parameters.AddWithValue("Icon", icon);
        cmd.Parameters.AddWithValue("Emoticon", emoticon);
        cmd.Parameters.AddWithValue("SortOrder", sortOrder);
        cmd.Parameters.AddWithValue("Replace", replace);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The smiley_resort.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="smileyID">
    /// The smiley id.
    /// </param>
    /// <param name="move">
    /// The move.
    /// </param>
    public static void smiley_resort(object boardID, object smileyID, int move)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("smiley_resort"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("SmileyID", smileyID);
        cmd.Parameters.AddWithValue("Move", move);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region BBCode

    /// <summary>
    /// The bbcode_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="bbcodeID">
    /// The bbcode id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable bbcode_list(object boardID, object bbcodeID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bbcode_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("BBCodeID", bbcodeID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The bbcode_delete.
    /// </summary>
    /// <param name="bbcodeID">
    /// The bbcode id.
    /// </param>
    public static void bbcode_delete(object bbcodeID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bbcode_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BBCodeID", bbcodeID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The bbcode_save.
    /// </summary>
    /// <param name="bbcodeID">
    /// The bbcode id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="description">
    /// The description.
    /// </param>
    /// <param name="onclickjs">
    /// The onclickjs.
    /// </param>
    /// <param name="displayjs">
    /// The displayjs.
    /// </param>
    /// <param name="editjs">
    /// The editjs.
    /// </param>
    /// <param name="displaycss">
    /// The displaycss.
    /// </param>
    /// <param name="searchregex">
    /// The searchregex.
    /// </param>
    /// <param name="replaceregex">
    /// The replaceregex.
    /// </param>
    /// <param name="variables">
    /// The variables.
    /// </param>
    /// <param name="usemodule">
    /// The usemodule.
    /// </param>
    /// <param name="moduleclass">
    /// The moduleclass.
    /// </param>
    /// <param name="execorder">
    /// The execorder.
    /// </param>
    public static void bbcode_save(
      object bbcodeID,
      object boardID,
      object name,
      object description,
      object onclickjs,
      object displayjs,
      object editjs,
      object displaycss,
      object searchregex,
      object replaceregex,
      object variables,
      object usemodule,
      object moduleclass,
      object execorder)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("bbcode_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BBCodeID", bbcodeID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Description", description);
        cmd.Parameters.AddWithValue("OnClickJS", onclickjs);
        cmd.Parameters.AddWithValue("DisplayJS", displayjs);
        cmd.Parameters.AddWithValue("EditJS", editjs);
        cmd.Parameters.AddWithValue("DisplayCSS", displaycss);
        cmd.Parameters.AddWithValue("SearchRegEx", searchregex);
        cmd.Parameters.AddWithValue("ReplaceRegEx", replaceregex);
        cmd.Parameters.AddWithValue("Variables", variables);
        cmd.Parameters.AddWithValue("UseModule", usemodule);
        cmd.Parameters.AddWithValue("ModuleClass", moduleclass);
        cmd.Parameters.AddWithValue("ExecOrder", execorder);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region Registry

    /// <summary>
    /// Retrieves entries in the board settings registry
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="boardID">
    /// The board ID.
    /// </param>
    /// <returns>
    /// DataTable filled will registry entries
    /// </returns>
    public static DataTable registry_list(object name, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("registry_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Retrieves entries in the board settings registry
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <returns>
    /// DataTable filled will registry entries
    /// </returns>
    public static DataTable registry_list(object name)
    {
      return registry_list(name, null);
    }

    /// <summary>
    /// Retrieves all the entries in the board settings registry
    /// </summary>
    /// <returns>
    /// DataTable filled will all registry entries
    /// </returns>
    public static DataTable registry_list()
    {
      return registry_list(null, null);
    }

    /// <summary>
    /// Saves a single registry entry pair to the database.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public static void registry_save(object name, object value)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("registry_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Value", value);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Saves a single registry entry pair to the database.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="boardID">
    /// The board ID.
    /// </param>
    public static void registry_save(object name, object value, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("registry_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Value", value);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region System

    /// <summary>
    /// Not in use anymore. Only required for old database versions.
    /// </summary>
    /// <returns>
    /// </returns>
    public static DataTable system_list()
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("system_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region Topic

    public static void topic_updatetopic(int topicId, string topic)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_updatetopic"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicId);
        cmd.Parameters.AddWithValue("Topic", topic);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }      
    }

    /// <summary>
    /// The topic_poll_update.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="messageID">
    /// The message id.
    /// </param>
    /// <param name="pollID">
    /// The poll id.
    /// </param>
    public static void topic_poll_update(object topicID, object messageID, object pollID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_poll_update"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("MessageID", messageID);
        cmd.Parameters.AddWithValue("PollID", pollID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The topic_prune.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="days">
    /// The days.
    /// </param>
    /// <param name="permDelete">
    /// The perm delete.
    /// </param>
    /// <returns>
    /// The topic_prune.
    /// </returns>
    public static int topic_prune(object boardID, object forumID, object days, object permDelete)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_prune"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("Days", days);
        cmd.Parameters.AddWithValue("PermDelete", permDelete);

        cmd.CommandTimeout = 99999;
        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// The topic_list.
    /// </summary>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="announcement">
    /// The announcement.
    /// </param>
    /// <param name="date">
    /// The date.
    /// </param>
    /// <param name="offset">
    /// The offset.
    /// </param>
    /// <param name="count">
    /// The count.
    /// </param>
    /// <param name="useStyledNicks">
    /// To return style for user nicks in topic_list.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_list(object forumID, object userId, object announcement, object date, object offset, object count, object useStyledNicks, object showMoved)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("UserID", userId);
        cmd.Parameters.AddWithValue("Announcement", announcement);
        cmd.Parameters.AddWithValue("Date", date);
        cmd.Parameters.AddWithValue("Offset", offset);
        cmd.Parameters.AddWithValue("Count", count);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        cmd.Parameters.AddWithValue("ShowMoved", showMoved);
        return YafDBAccess.Current.GetData(cmd, true);
      }
    }

    /// <summary>
    /// Lists topics very simply (for URL rewriting)
    /// </summary>
    /// <param name="StartID">
    /// </param>
    /// <param name="Limit">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_simplelist(int StartID, int Limit)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_simplelist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("StartID", StartID);
        cmd.Parameters.AddWithValue("Limit", Limit);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_move.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="showMoved">
    /// The show moved.
    /// </param>
    public static void topic_move(object topicID, object forumID, object showMoved)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_move"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("ShowMoved", showMoved);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The topic_announcements.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="numOfPostsToRetrieve">
    /// The num of posts to retrieve.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_announcements(object boardID, object numOfPostsToRetrieve, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_announcements"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_latest.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="numOfPostsToRetrieve">
    /// The num of posts to retrieve.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="useStyledNicks">
    /// If true returns string for userID style.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_latest(object boardID, object numOfPostsToRetrieve, object userID, bool useStyledNicks, bool showNoCountPosts)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_latest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_active.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="since">
    /// The since.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="useStyledNicks">
    /// Set to true to get color nicks for last user and topic starter.
    /// </param>    
    /// <returns>
    /// </returns>
    public static DataTable topic_active(object boardID, object userID, object since, object categoryID, object useStyledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_active"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Since", since);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    // ABOT NEW 16.04.04:Delete all topic's messages
    /// <summary>
    /// The topic_delete attachments.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    private static void topic_deleteAttachments(object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_listmessages"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          foreach (DataRow row in dt.Rows)
          {
            message_deleteRecursively(row["MessageID"], true, string.Empty, 0, true, false);
          }
        }
      }
    }

    // Ederon : 12/9/2007
    /// <summary>
    /// The topic_delete.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    public static void topic_delete(object topicID)
    {
      topic_delete(topicID, false);
    }

    /// <summary>
    /// The topic_delete.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="eraseTopic">
    /// The erase topic.
    /// </param>
    public static void topic_delete(object topicID, object eraseTopic)
    {
      // ABOT CHANGE 16.04.04
      topic_deleteAttachments(topicID);

      // END ABOT CHANGE 16.04.04
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("EraseTopic", eraseTopic);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The topic_findprev.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_findprev(object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_findprev"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_findnext.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_findnext(object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_findnext"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_lock.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="locked">
    /// The locked.
    /// </param>
    public static void topic_lock(object topicID, object locked)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_lock"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("Locked", locked);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

      /// <summary>
      /// The topic_save.
      /// </summary>
      /// <param name="forumID">
      /// The forum id.
      /// </param>
      /// <param name="subject">
      /// The subject.
      /// </param>
      /// <param name="message">
      /// The message.
      /// </param>
      /// <param name="userID">
      /// The user id.
      /// </param>
      /// <param name="priority">
      /// The priority.
      /// </param>
      /// <param name="userName">
      /// The user name.
      /// </param>
      /// <param name="ip">
      /// The ip.
      /// </param>
      /// <param name="posted">
      /// The posted.
      /// </param>
      /// <param name="blogPostID">
      /// The blog post id.
      /// </param>
      /// <param name="flags">
      /// The flags.
      /// </param>
      /// <param name="messageID">
      /// The message id.
      /// </param>
      /// <returns>
      /// The topic_save.
      /// </returns>
      public static long topic_save(
      object forumID,
      object subject,
      object message,
      object userID,
      object priority,
      object userName,
      object ip,
      object posted,
      object blogPostID,
      object flags,
      ref long messageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("Subject", subject);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("Priority", priority);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("IP", ip);
       // cmd.Parameters.AddWithValue("PollID", pollID);
        cmd.Parameters.AddWithValue("Posted", posted);
        cmd.Parameters.AddWithValue("BlogPostID", blogPostID);
        cmd.Parameters.AddWithValue("Flags", flags);

        DataTable dt = YafDBAccess.Current.GetData(cmd);
        messageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
        return long.Parse(dt.Rows[0]["TopicID"].ToString());
      }
    }

    /// <summary>
    /// The topic_info.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataRow topic_info(object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_info"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        using (DataTable dt = YafDBAccess.Current.GetData(cmd))
        {
          if (dt.Rows.Count > 0)
          {
            return dt.Rows[0];
          }
          else
          {
            return null;
          }
        }
      }
    }

    /// <summary>
    /// The topic_ favorite_ details.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="since">
    /// The since.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="useStyledNicks">
    /// Set to true to get color nicks for last user and topic starter.
    /// </param>    
    /// <returns>
    /// a Data Table containing the current user's favorite topics with details.
    /// </returns>
    public static DataTable topic_favorite_details(object boardID, object userID, object since, object categoryID, object useStyledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_favorite_details"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Since", since);
        cmd.Parameters.AddWithValue("CategoryID", categoryID);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_favorite_list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable topic_favorite_list(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_favorite_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The topic_favorite_remove.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    public static void topic_favorite_remove(object userID, object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_favorite_remove"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("TopicID", topicID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The topic_favorite_add.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    public static void topic_favorite_add(object userID, object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("topic_favorite_add"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("TopicID", topicID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region ReplaceWords

    // rico : replace words / begin
    /// <summary>
    /// Gets a list of replace words
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <returns>
    /// DataTable with replace words
    /// </returns>
    public static DataTable replace_words_list(object boardId, object id)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("replace_words_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("ID", id);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Saves changs to a words
    /// </summary>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="id">
    /// ID of bad/good word
    /// </param>
    /// <param name="badword">
    /// bad word
    /// </param>
    /// <param name="goodword">
    /// good word
    /// </param>
    public static void replace_words_save(object boardId, object id, object badword, object goodword)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("replace_words_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ID", id);
        cmd.Parameters.AddWithValue("BoardID", boardId);
        cmd.Parameters.AddWithValue("badword", badword);
        cmd.Parameters.AddWithValue("goodword", goodword);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Deletes a bad/good word
    /// </summary>
    /// <param name="ID">
    /// ID of bad/good word to delete
    /// </param>
    public static void replace_words_delete(object ID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("replace_words_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ID", ID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region IgnoreUser

    /// <summary>
    /// The user_addignoreduser.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public static void user_addignoreduser(object userId, object ignoredUserId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_addignoreduser"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserId", userId);
        cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_removeignoreduser.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    public static void user_removeignoreduser(object userId, object ignoredUserId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_removeignoreduser"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserId", userId);
        cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_isuserignored.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="ignoredUserId">
    /// The ignored user id.
    /// </param>
    /// <returns>
    /// The user_isuserignored.
    /// </returns>
    public static bool user_isuserignored(object userId, object ignoredUserId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_isuserignored"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserId", userId);
        cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
        cmd.Parameters.Add("result", SqlDbType.Bit);
        cmd.Parameters["result"].Direction = ParameterDirection.ReturnValue;

        YafDBAccess.Current.ExecuteNonQuery(cmd);

        return Convert.ToBoolean(cmd.Parameters["result"].Value);
      }
    }

    /// <summary>
    /// The user_ignoredlist.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_ignoredlist(object userId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_ignoredlist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserId", userId);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region User

    /// <summary>
    /// To return a rather rarely updated active user data
    /// </summary>
    /// <param name="userID">The UserID. It is always should have a positive > 0 value.</param>
    /// <param name="styledNicks">If styles should be returned.</param>
    /// <returns>A DataRow, it should never return a null value.</returns>
    public static DataRow user_lazydata(object userID, object boardID, bool showPendingMails, bool showPendingBuddies, bool showUnreadPMs, bool showUserAlbums, bool styledNicks)
    {

      int nTries = 0;
      while (true)
      {
        try
        {
          using (SqlCommand cmd = YafDBAccess.GetCommand("user_lazydata"))
          {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("UserID", userID);
            cmd.Parameters.AddWithValue("BoardID", boardID);
            cmd.Parameters.AddWithValue("ShowPendingMails", showPendingMails);
            cmd.Parameters.AddWithValue("ShowPendingBuddies", showPendingBuddies);
            cmd.Parameters.AddWithValue("ShowUnreadPMs", showUnreadPMs);
            cmd.Parameters.AddWithValue("ShowUserAlbums", showUserAlbums);
            cmd.Parameters.AddWithValue("ShowUserStyle", styledNicks);
            return YafDBAccess.Current.GetData(cmd).Rows[0];
          }
        }
        catch (SqlException x)
        {
          if (x.Number == 1205 && nTries < 3)
          {
            // Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.

          }
          else
          {
            throw new ApplicationException(string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
          }
        }

        ++nTries;
      }
    }


    /// <summary>
    /// The user_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>    
    /// <returns>
    /// </returns>
    public static DataTable user_list(object boardID, object userID, object approved)
    {
      return user_list(boardID, userID, approved, null, null, false);
    }
    /// <summary>
    /// The user_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>
    /// <param name="useStyledNicks">
    /// Return style info.
    /// </param> 
    /// <returns>
    /// </returns>
    public static DataTable user_list(object boardID, object userID, object approved, object useStyledNicks)
    {
      return user_list(boardID, userID, approved, null, null, useStyledNicks);
    }

    /// <summary>
    /// The user_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_list(object boardID, object userID, object approved, object groupID, object rankID)
    {
      return user_list(boardID, userID, approved, null, null, false);
    }

    /// <summary>
    /// The user_list.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    /// <param name="useStyledNicks">
    /// Return style info.
    /// </param> 
    /// <returns>
    /// </returns>
    public static DataTable user_list(object boardID, object userID, object approved, object groupID, object rankID, object useStyledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Approved", approved);
        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("RankID", rankID);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// For URL Rewriting
    /// </summary>
    /// <param name="StartID">
    /// </param>
    /// <param name="Limit">
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_simplelist(int StartID, int Limit)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_simplelist"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("StartID", StartID);
        cmd.Parameters.AddWithValue("Limit", Limit);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_delete.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void user_delete(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_setrole.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="role">
    /// The role.
    /// </param>
    public static void user_setrole(int boardID, object providerUserKey, object role)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_setrole"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
        cmd.Parameters.AddWithValue("Role", role);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    // TODO: The method is not in use
    /// <summary>
    /// The user_setinfo.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="user">
    /// The user.
    /// </param>
    public static void user_setinfo(int boardID, MembershipUser user)
    {
      using (
        SqlCommand cmd =
          YafDBAccess.GetCommand(
            "update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true))
      {
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("UserName", user.UserName);
        cmd.Parameters.AddWithValue("Email", user.Email);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ProviderUserKey", user.ProviderUserKey);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_migrate.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="updateProvider">
    /// The update provider.
    /// </param>
    public static void user_migrate(object userID, object providerUserKey, object updateProvider)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_migrate"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("UpdateProvider", updateProvider);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_deleteold.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    public static void user_deleteold(object boardID, object days)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_deleteold"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Days", days);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_approve.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void user_approve(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_approve"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_approveall.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    public static void user_approveall(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_approveall"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_suspend.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="suspend">
    /// The suspend.
    /// </param>
    public static void user_suspend(object userID, object suspend)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_suspend"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Suspend", suspend);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Returns data about allowed signature tags and character limits
    /// </summary>
    /// <param name="userID">The userID</param>
    /// <param name="boardID">The boardID</param>
    /// <returns>Data Table</returns>
    public static DataTable user_getsignaturedata(object userID, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getsignaturedata"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Returns data about albums: allowed number of images and albums
    /// </summary>
    /// <param name="userID">The userID</param>
    /// <param name="boardID">The boardID</param>  
    public static DataTable user_getalbumsdata(object userID, object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getalbumsdata"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_changepassword.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="oldPassword">
    /// The old password.
    /// </param>
    /// <param name="newPassword">
    /// The new password.
    /// </param>
    /// <returns>
    /// The user_changepassword.
    /// </returns>
    public static bool user_changepassword(object userID, object oldPassword, object newPassword)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_changepassword"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("OldPassword", oldPassword);
        cmd.Parameters.AddWithValue("NewPassword", newPassword);
        return (bool)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// The user_pmcount.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_pmcount(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_pmcount"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_save.
    /// </summary>
    /// <param name="displayName"></param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="timeZone">
    /// The time zone.
    /// </param>
    /// <param name="languageFile">
    /// The language file.
    /// </param>
    /// <param name="themeFile">
    /// The theme file.
    /// </param>
    /// <param name="overrideDefaultThemes">
    /// The override default themes.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>
    /// <param name="pmNotification">
    /// The pm notification.
    /// </param>
    public static void user_save(
      object userID,
      object boardID,
      object userName,
      object displayName,
      object email,
      object timeZone,
      object languageFile,
      object culture,
      object themeFile,
      object overrideDefaultThemes,
      object approved,
      object pmNotification,
      object autoWatchTopics,
      object dSTUser,
      object hideUser,
      object notificationType)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("DisplayName", displayName);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("TimeZone", timeZone);
        cmd.Parameters.AddWithValue("LanguageFile", languageFile);
        cmd.Parameters.AddWithValue("Culture", culture);
        cmd.Parameters.AddWithValue("ThemeFile", themeFile);
        cmd.Parameters.AddWithValue("OverrideDefaultTheme", overrideDefaultThemes);
        cmd.Parameters.AddWithValue("Approved", approved);
        cmd.Parameters.AddWithValue("PMNotification", pmNotification);
        cmd.Parameters.AddWithValue("AutoWatchTopics", autoWatchTopics);
        cmd.Parameters.AddWithValue("DSTUser", dSTUser);
        cmd.Parameters.AddWithValue("HideUser", hideUser);
        cmd.Parameters.AddWithValue("NotificationType", notificationType);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Saves the notification type for a user
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="notificationType">
    /// The notification type.
    /// </param>
    public static void user_savenotification(
          object userID,
          object pmNotification,
          object autoWatchTopics,
          object notificationType,
          object dailyDigest)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_savenotification"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("PMNotification", pmNotification);
        cmd.Parameters.AddWithValue("AutoWatchTopics", autoWatchTopics);
        cmd.Parameters.AddWithValue("NotificationType", notificationType);
        cmd.Parameters.AddWithValue("DailyDigest", dailyDigest);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_adminsave.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="rankID">
    /// The rank id.
    /// </param>
    public static void user_adminsave(object boardID, object userID, object name, object displayName, object email, object flags, object rankID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_adminsave"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("DisplayName", displayName);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("Flags", flags);
        cmd.Parameters.AddWithValue("RankID", rankID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_emails.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_emails(object boardID, object groupID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_emails"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("GroupID", groupID);

        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_accessmasks.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_accessmasks(object boardID, object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_accessmasks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserID", userID);

        return userforumaccess_sort_list(YafDBAccess.Current.GetData(cmd), 0, 0, 0);
      }
    }

    // adds some convenience while editing group's access rights (indent forums)
    /// <summary>
    /// The userforumaccess_sort_list.
    /// </summary>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="startingIndent">
    /// The starting indent.
    /// </param>
    /// <returns>
    /// </returns>
    private static DataTable userforumaccess_sort_list(DataTable listSource, int parentID, int categoryID, int startingIndent)
    {
      var listDestination = new DataTable();

      listDestination.Columns.Add("ForumID", typeof(String));
      listDestination.Columns.Add("ForumName", typeof(String));

      // it is uset in two different procedures with different tables, 
      // so, we must add correct columns
      if (listSource.Columns.IndexOf("AccessMaskName") >= 0)
      {
        listDestination.Columns.Add("AccessMaskName", typeof(String));
      }
      else
      {
        listDestination.Columns.Add("BoardName", typeof(String));
        listDestination.Columns.Add("CategoryName", typeof(String));
        listDestination.Columns.Add("AccessMaskId", typeof(Int32));
      }

      DataView dv = listSource.DefaultView;
      userforumaccess_sort_list_recursive(dv.ToTable(), listDestination, parentID, categoryID, startingIndent);
      return listDestination;
    }

    /// <summary>
    /// The userforumaccess_sort_list_recursive.
    /// </summary>
    /// <param name="listSource">
    /// The list source.
    /// </param>
    /// <param name="listDestination">
    /// The list destination.
    /// </param>
    /// <param name="parentID">
    /// The parent id.
    /// </param>
    /// <param name="categoryID">
    /// The category id.
    /// </param>
    /// <param name="currentIndent">
    /// The current indent.
    /// </param>
    private static void userforumaccess_sort_list_recursive(DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent)
    {
      DataRow newRow;

      foreach (DataRow row in listSource.Rows)
      {
        // see if this is a root-forum
        if (row["ParentID"] == DBNull.Value)
        {
          row["ParentID"] = 0;
        }

        if ((int)row["ParentID"] == parentID)
        {
          string sIndent = string.Empty;

          for (int j = 0; j < currentIndent; j++)
          {
            sIndent += "--";
          }

          // import the row into the destination
          newRow = listDestination.NewRow();

          newRow["ForumID"] = row["ForumID"];
          newRow["ForumName"] = string.Format("{0} {1}", sIndent, row["ForumName"]);
          if (listDestination.Columns.IndexOf("AccessMaskName") >= 0)
          {
            newRow["AccessMaskName"] = row["AccessMaskName"];
          }
          else
          {
            newRow["BoardName"] = row["BoardName"];
            newRow["CategoryName"] = row["CategoryName"];
            newRow["AccessMaskId"] = row["AccessMaskId"];
          }


          listDestination.Rows.Add(newRow);

          // recurse through the list...
          userforumaccess_sort_list_recursive(listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
        }
      }
    }

    /// <summary>
    /// The user_recoverpassword.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// The user_recoverpassword.
    /// </returns>
    public static object user_recoverpassword(object boardID, object userName, object email)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_recoverpassword"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("Email", email);
        return YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// The user_savepassword.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    public static void user_savepassword(object userID, object password)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_savepassword"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_login.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <returns>
    /// The user_login.
    /// </returns>
    public static object user_login(object boardID, object name, object password)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_login"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Name", name);
        cmd.Parameters.AddWithValue("Password", password);
        return YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// The user_avatarimage.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_avatarimage(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_avatarimage"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_get.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <returns>
    /// The user_get.
    /// </returns>
    public static int user_get(int boardID, object providerUserKey)
    {
      using (
        SqlCommand cmd =
          YafDBAccess.GetCommand("select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true))
      {
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
        return (int)(YafDBAccess.Current.ExecuteScalar(cmd) ?? 0);
      }
    }

    /// <summary>
    /// The UserFind.
    /// </summary>
    /// <param name="boardID">
    ///   The board id.
    /// </param>
    /// <param name="filter">
    ///   The filter.
    /// </param>
    /// <param name="userName">
    ///   The user name.
    /// </param>
    /// <param name="email">
    ///   The email.
    /// </param>
    /// <param name="displayName"></param>
    /// <param name="notificationType"></param>
    /// <param name="dailyDigest"></param>
    /// <returns>
    /// </returns>
    public static IEnumerable<TypedUserFind> UserFind(int boardID, bool filter, string userName, string email, string displayName, object notificationType, object dailyDigest)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_find"))
      {
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("Filter", filter);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("DisplayName", displayName);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("NotificationType", notificationType);
        cmd.Parameters.AddWithValue("DailyDigest", dailyDigest);

        return YafDBAccess.Current.GetData(cmd).AsEnumerable().Select(u => new TypedUserFind(u));
      }
    }

    /// <summary>
    /// The user_getsignature.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// The user_getsignature.
    /// </returns>
    public static string user_getsignature(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getsignature"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.ExecuteScalar(cmd).ToString();
      }
    }

    /// <summary>
    /// The user_savesignature.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="signature">
    /// The signature.
    /// </param>
    public static void user_savesignature(object userID, object signature)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_savesignature"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Signature", signature);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_saveavatar.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="avatar">
    /// The avatar.
    /// </param>
    /// <param name="stream">
    /// The stream.
    /// </param>
    /// <param name="avatarImageType">
    /// The avatar image type.
    /// </param>
    public static void user_saveavatar(object userID, object avatar, Stream stream, object avatarImageType)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_saveavatar"))
      {
        byte[] data = null;

        if (stream != null)
        {
          data = new byte[stream.Length];
          stream.Seek(0, SeekOrigin.Begin);
          stream.Read(data, 0, (int)stream.Length);
        }

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Avatar", avatar);
        cmd.Parameters.AddWithValue("AvatarImage", data);
        cmd.Parameters.AddWithValue("AvatarImageType", avatarImageType);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_deleteavatar.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    public static void user_deleteavatar(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_deleteavatar"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    // TODO: The method is not in use
    /// <summary>
    /// The user_register.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="password">
    /// The password.
    /// </param>
    /// <param name="hash">
    /// The hash.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="location">
    /// The location.
    /// </param>
    /// <param name="homePage">
    /// The home page.
    /// </param>
    /// <param name="timeZone">
    /// The time zone.
    /// </param>
    /// <param name="approved">
    /// The approved.
    /// </param>
    /// <returns>
    /// The user_register.
    /// </returns>
    public static bool user_register(
      object boardID, object userName, object password, object hash, object email, object location, object homePage, object timeZone, bool approved)
    {
      using (var connMan = new YafDBConnManager())
      {
        using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(YafDBAccess.IsolationLevel))
        {
          try
          {
            using (SqlCommand cmd = YafDBAccess.GetCommand("user_save", connMan.DBConnection))
            {
              cmd.Transaction = trans;
              cmd.CommandType = CommandType.StoredProcedure;
              int UserID = 0;
              cmd.Parameters.AddWithValue("UserID", UserID);
              cmd.Parameters.AddWithValue("BoardID", boardID);
              cmd.Parameters.AddWithValue("UserName", userName);
              cmd.Parameters.AddWithValue("Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
              cmd.Parameters.AddWithValue("Email", email);
              cmd.Parameters.AddWithValue("Hash", hash);
              cmd.Parameters.AddWithValue("Location", location);
              cmd.Parameters.AddWithValue("HomePage", homePage);
              cmd.Parameters.AddWithValue("TimeZone", timeZone);
              cmd.Parameters.AddWithValue("Approved", approved);
              cmd.Parameters.AddWithValue("PMNotification", 1);
              cmd.Parameters.AddWithValue("AutoWatchTopics", 0);
              cmd.ExecuteNonQuery();
            }

            trans.Commit();
          }
          catch (Exception x)
          {
            trans.Rollback();
            eventlog_create(null, "user_register in YAF.Classes.Data.DB.cs", x, EventLogTypes.Error);
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// The user_aspnet.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    /// <param name="isApproved">
    /// The is approved.
    /// </param>
    /// <returns>
    /// The user_aspnet.
    /// </returns>
    public static int user_aspnet(int boardID, string userName, string displayName, string email, object providerUserKey, object isApproved)
    {
      try
      {
        using (SqlCommand cmd = YafDBAccess.GetCommand("user_aspnet"))
        {
          cmd.CommandType = CommandType.StoredProcedure;

          cmd.Parameters.AddWithValue("BoardID", boardID);
          cmd.Parameters.AddWithValue("UserName", userName);
          cmd.Parameters.AddWithValue("DisplayName", displayName);
          cmd.Parameters.AddWithValue("Email", email);
          cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
          cmd.Parameters.AddWithValue("IsApproved", isApproved);
          return (int)YafDBAccess.Current.ExecuteScalar(cmd);
        }
      }
      catch (Exception x)
      {
        eventlog_create(null, "user_aspnet in YAF.Classes.Data.DB.cs", x, EventLogTypes.Error);
        return 0;
      }
    }

    /// <summary>
    /// The user_guest.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <returns>
    /// The user_guest.
    /// </returns>
    public static int? user_guest(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_guest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        return YafDBAccess.Current.ExecuteScalar(cmd).ToType<int?>();
      }
    }

    /// <summary>
    /// The user_activity_rank.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="startDate">
    /// The start date.
    /// </param>
    /// <param name="displayNumber">
    /// The display number.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_activity_rank(object boardID, object startDate, object displayNumber)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_activity_rank"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("StartDate", startDate);
        cmd.Parameters.AddWithValue("DisplayNumber", displayNumber);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The user_nntp.
    /// </summary>
    /// <param name="boardID">
    /// The board id.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="email">
    /// The email.
    /// </param>
    /// <returns>
    /// The user_nntp.
    /// </returns>
    public static int user_nntp(object boardID, object userName, object email, int? timeZone)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_nntp"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardID", boardID);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("Email", email);
        cmd.Parameters.AddWithValue("TimeZone", timeZone);

        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    /// <summary>
    /// The user_addpoints.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="points">
    /// The points.
    /// </param>
    public static void user_addpoints(object userID, object points)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_addpoints"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Points", points);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_removepoints by topic id.
    /// </summary>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <param name="points">
    /// The points.
    /// </param>
    public static void user_removepointsByTopicID(object topicID, object points)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_removepointsbytopicid"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("TopicID", topicID);
        cmd.Parameters.AddWithValue("Points", points);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_removepoints.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="points">
    /// The points.
    /// </param>
    public static void user_removepoints(object userID, object points)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_removepoints"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Points", points);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_setpoints.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="points">
    /// The points.
    /// </param>
    public static void user_setpoints(object userID, object points)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_setpoints"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Points", points);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The user_getpoints.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// The user_getpoints.
    /// </returns>
    public static int user_getpoints(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getpoints"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    // <summary> Returns the number of times a specific user with the provided UserID 
    // has thanked others.
    /// <summary>
    /// The user_getthanks_from.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// The user_getthanks_from.
    /// </returns>
    public static int user_getthanks_from(object userID, object pageUserId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getthanks_from"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("PageUserID", pageUserId);
        return (int)YafDBAccess.Current.ExecuteScalar(cmd);
      }
    }

    // <summary> Returns the number of times and posts that other users have thanked the 
    // user with the provided userID.
    /// <summary>
    /// The user_getthanks_to.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static int[] user_getthanks_to(object userID, object pageUserId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_getthanks_to"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramThanksToNumber = new SqlParameter("ThanksToNumber", 0);
        paramThanksToNumber.Direction = ParameterDirection.Output;
        var paramThanksToPostsNumber = new SqlParameter("ThanksToPostsNumber", 0);
        paramThanksToPostsNumber.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("PageUserID", pageUserId);
        cmd.Parameters.Add(paramThanksToNumber);
        cmd.Parameters.Add(paramThanksToPostsNumber);
        YafDBAccess.Current.ExecuteNonQuery(cmd);

        int ThanksToPostsNumber, ThanksToNumber;
        if (paramThanksToNumber.Value == DBNull.Value)
        {
          ThanksToNumber = 0;
          ThanksToPostsNumber = 0;
        }
        else
        {
          ThanksToPostsNumber = Convert.ToInt32(paramThanksToPostsNumber.Value);
          ThanksToNumber = Convert.ToInt32(paramThanksToNumber.Value);
        }

        return new[]
          {
            ThanksToNumber, ThanksToPostsNumber
          };
      }
    }

    /// <summary>
    /// Returns the posts which is thanked by the user + the posts which are posted by the user and 
    /// are thanked by other users.
    /// </summary>
    /// <param name="UserID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable user_viewallthanks(object UserID, object pageUserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("user_viewallthanks"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", UserID);
        cmd.Parameters.AddWithValue("PageUserID", pageUserID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region UserForum

    /// <summary>
    /// The userforum_list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable userforum_list(object userID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("userforum_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The userforum_delete.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    public static void userforum_delete(object userID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("userforum_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The userforum_save.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <param name="accessMaskID">
    /// The access mask id.
    /// </param>
    public static void userforum_save(object userID, object forumID, object accessMaskID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("userforum_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region UserGroup

    /// <summary>
    /// The usergroup_list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable usergroup_list(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("usergroup_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The usergroup_save.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="groupID">
    /// The group id.
    /// </param>
    /// <param name="member">
    /// The member.
    /// </param>
    public static void usergroup_save(object userID, object groupID, object member)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("usergroup_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("GroupID", groupID);
        cmd.Parameters.AddWithValue("Member", member);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region WatchForum

    /// <summary>
    /// The watchforum_add.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    public static void watchforum_add(object userID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchforum_add"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The watchforum_list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable watchforum_list(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchforum_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The watchforum_check.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="forumID">
    /// The forum id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable watchforum_check(object userID, object forumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchforum_check"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("ForumID", forumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The watchforum_delete.
    /// </summary>
    /// <param name="watchForumID">
    /// The watch forum id.
    /// </param>
    public static void watchforum_delete(object watchForumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchforum_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("WatchForumID", watchForumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region WatchTopic

    /// <summary>
    /// The watchtopic_list.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable watchtopic_list(object userID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchtopic_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The watchtopic_check.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable watchtopic_check(object userID, object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchtopic_check"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("TopicID", topicID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The watchtopic_delete.
    /// </summary>
    /// <param name="watchTopicID">
    /// The watch topic id.
    /// </param>
    public static void watchtopic_delete(object watchTopicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchtopic_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("WatchTopicID", watchTopicID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The watchtopic_add.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="topicID">
    /// The topic id.
    /// </param>
    public static void watchtopic_add(object userID, object topicID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("watchtopic_add"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("TopicID", topicID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    #endregion

    #region vzrus addons

    #region reindex page controls

    // DB Maintenance page buttons name
    /// <summary>
    /// Gets btnGetStatsName.
    /// </summary>
    public static string btnGetStatsName
    {
      get
      {
        return "Table Index Statistics";
      }
    }

    /// <summary>
    /// Gets btnShrinkName.
    /// </summary>
    public static string btnShrinkName
    {
      get
      {
        return "Shrink Database";
      }
    }

    /// <summary>
    /// Gets btnRecoveryModeName.
    /// </summary>
    public static string btnRecoveryModeName
    {
      get
      {
        return "Set Recovery Mode";
      }
    }

    /// <summary>
    /// Gets btnReindexName.
    /// </summary>
    public static string btnReindexName
    {
      get
      {
        return "Reindex Tables";
      }
    }

    // DB Maintenance page panels visibility
    /// <summary>
    /// Gets a value indicating whether PanelGetStats.
    /// </summary>
    public static bool PanelGetStats
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PanelRecoveryMode.
    /// </summary>
    public static bool PanelRecoveryMode
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PanelReindex.
    /// </summary>
    public static bool PanelReindex
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Gets a value indicating whether PanelShrink.
    /// </summary>
    public static bool PanelShrink
    {
      get
      {
        return true;
      }
    }

    #endregion

    /// <summary>
    /// The rsstopic_list.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable rsstopic_list(int forumId)
    {
      // TODO: vzrus: possible move to an sp and registry settings for rsstopiclimit
      int topicLimit = 1000;

      StringBuilder sb = new StringBuilder();

      sb.AppendFormat("select top {0} Topic = a.Topic,TopicID = a.TopicID, Name = b.Name, LastPosted = IsNull(a.LastPosted,a.Posted), LastUserID = IsNull(a.LastUserID, a.UserID), LastMessageID= IsNull(a.LastMessageID,(select top 1 m.MessageID ", topicLimit);
      sb.Append("from {databaseOwner}.{objectQualifier}Message m where m.TopicID = a.TopicID order by m.Posted desc)), LastMessageFlags = IsNull(a.LastMessageFlags,22) ");
        //sb.Append(", message = (SELECT TOP 1 CAST([Message] as nvarchar(1000)) FROM [{databaseOwner}].[{objectQualifier}Message] mes2 where mes2.TopicID = IsNull(a.TopicMovedID,a.TopicID) AND mes2.IsApproved = 1 AND mes2.IsDeleted = 0 ORDER BY mes2.Posted DESC) ");
      sb.Append("from {databaseOwner}.{objectQualifier}Topic a, {databaseOwner}.{objectQualifier}Forum b where a.ForumID = @ForumID and b.ForumID = a.ForumID and a.TopicMovedID is null and a.IsDeleted = 0");
      sb.Append(" order by a.Posted desc");

      using (SqlCommand cmd = YafDBAccess.GetCommand(sb.ToString(), true))
      {
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("ForumID", forumId);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The db_getstats_warning.
    /// </summary>
    /// <param name="connMan">
    /// The conn man.
    /// </param>
    /// <returns>
    /// The db_getstats_warning.
    /// </returns>
    public static string db_getstats_warning(YafDBConnManager connMan)
    {
      return string.Empty;
    }

    /// <summary>
    /// The db_getstats.
    /// </summary>
    /// <param name="connMan">
    /// The conn man.
    /// </param>
    public static void db_getstats(YafDBConnManager connMan)
    {
      // create statistic getting SQL...
      var sb = new StringBuilder();

      sb.AppendLine("DECLARE @TableName sysname");
      sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
      sb.AppendFormat("SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", Config.DatabaseObjectQualifier);
      sb.AppendLine("OPEN cur_showfragmentation");
      sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
      sb.AppendLine("WHILE @@FETCH_STATUS = 0");
      sb.AppendLine("BEGIN");
      sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
      sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
      sb.AppendLine("END");
      sb.AppendLine("CLOSE cur_showfragmentation");
      sb.AppendLine("DEALLOCATE cur_showfragmentation");

      using (var cmd = new SqlCommand(sb.ToString(), connMan.OpenDBConnection))
      {
        cmd.Connection = connMan.DBConnection;

        // up the command timeout...
        cmd.CommandTimeout = 9999;

        // run it...
        cmd.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// The db_reindex_warning.
    /// </summary>
    /// <param name="connMan">
    /// The conn man.
    /// </param>
    /// <returns>
    /// The db_reindex_warning.
    /// </returns>
    public static string db_reindex_warning(YafDBConnManager connMan)
    {
      return string.Empty;
    }

    /// <summary>
    /// The db_reindex.
    /// </summary>
    /// <param name="connMan">
    /// The conn man.
    /// </param>
    public static void db_reindex(YafDBConnManager connMan)
    {
      // create statistic getting SQL...
      var sb = new StringBuilder();

      sb.AppendLine("DECLARE @MyTable VARCHAR(255)");
      sb.AppendLine("DECLARE myCursor");
      sb.AppendLine("CURSOR FOR");
      sb.AppendFormat(
        "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", Config.DatabaseObjectQualifier);
      sb.AppendLine("OPEN myCursor");
      sb.AppendLine("FETCH NEXT");
      sb.AppendLine("FROM myCursor INTO @MyTable");
      sb.AppendLine("WHILE @@FETCH_STATUS = 0");
      sb.AppendLine("BEGIN");
      sb.AppendLine("PRINT 'Reindexing Table:  ' + @MyTable");
      sb.AppendLine("DBCC DBREINDEX(@MyTable, '', 80)");
      sb.AppendLine("FETCH NEXT");
      sb.AppendLine("FROM myCursor INTO @MyTable");
      sb.AppendLine("END");
      sb.AppendLine("CLOSE myCursor");
      sb.AppendLine("DEALLOCATE myCursor");

      using (var cmd = new SqlCommand(sb.ToString(), connMan.OpenDBConnection))
      {
        cmd.Connection = connMan.DBConnection;

        // up the command timeout...
        cmd.CommandTimeout = 9999;

        // run it...
        cmd.ExecuteNonQuery();
      }
    }

    /// <summary>
    /// The db_runsql.
    /// </summary>
    /// <param name="sql">
    /// The sql.
    /// </param>
    /// <param name="connMan">
    /// The conn man.
    /// </param>
    /// <returns>
    /// The db_runsql.
    /// </returns>
    public static string db_runsql(string sql, YafDBConnManager connMan, bool useTransaction)
    {
      using (var command = new SqlCommand(sql, connMan.OpenDBConnection))
      {
        command.CommandTimeout = 9999;
        command.Connection = connMan.OpenDBConnection;

        return InnerRunSqlExecuteReader(command, useTransaction);
      }
    }

    /// <summary>
    /// Called from db_runsql -- just runs a sql command according to specificiations.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="useTransaction"></param>
    /// <returns></returns>
    private static string InnerRunSqlExecuteReader(SqlCommand command, bool useTransaction)
    {
      SqlDataReader reader = null;
      var results = new StringBuilder();

      try
      {
        try
        {
          command.Transaction = useTransaction ? command.Connection.BeginTransaction(YafDBAccess.IsolationLevel) : null;
          reader = command.ExecuteReader();

          if (reader != null)
          {
            if (reader.HasRows)
            {
              int rowIndex = 1;
              var columnNames =
                reader.GetSchemaTable().Rows.Cast<DataRow>().Select(r => r["ColumnName"].ToString()).ToList();

              results.Append("RowNumber");

              columnNames.ForEach(
                n =>
                {
                  results.Append(",");
                  results.Append(n);
                });

              results.AppendLine();

              while (reader.Read())
              {
                results.AppendFormat(@"""{0}""", rowIndex++);

                // dump all columns...
                foreach (var col in columnNames)
                {
                  results.AppendFormat(@",""{0}""", reader[col].ToString().Replace("\"", "\"\""));
                }

                results.AppendLine();
              }
            }
            else if (reader.RecordsAffected > 0)
            {
              results.AppendFormat("{0} Record(s) Affected", reader.RecordsAffected);
              results.AppendLine();
            }
            else
            {
              results.AppendLine("No Results Returned.");
            }

            reader.Close();

            if (command.Transaction != null)
            {
              command.Transaction.Commit();
            }
          }
        }
        finally
        {
          if (command.Transaction != null)
          {
            command.Transaction.Rollback();
          }          
        }
      }
      catch (Exception x)
      {
        if (reader != null)
        {
          reader.Close();
        }
        
        results.AppendLine();
        results.AppendFormat("SQL ERROR: {0}", x);
      }

      return results.ToString();
    }

    /// <summary>
    /// The forumpage_initdb.
    /// </summary>
    /// <param name="errorStr">
    /// The error str.
    /// </param>
    /// <param name="debugging">
    /// The debugging.
    /// </param>
    /// <returns>
    /// The forumpage_initdb.
    /// </returns>
    public static bool forumpage_initdb(out string errorStr, bool debugging)
    {
      errorStr = string.Empty;

      try
      {
        using (var connMan = new YafDBConnManager())
        {
          // just attempt to open the connection to test if a DB is available.
          SqlConnection getConn = connMan.OpenDBConnection;
        }
      }
      catch (SqlException ex)
      {
        // unable to connect to the DB...
        if (!debugging)
        {
          errorStr = "Unable to connect to the Database. Exception Message: " + ex.Message + " (" + ex.Number + ")";
          return false;
        }

        // re-throw since we are debugging...
        throw;
      }

      return true;
    }

    /// <summary>
    /// The forumpage_validateversion.
    /// </summary>
    /// <param name="appVersion">
    /// The app version.
    /// </param>
    /// <returns>
    /// The forumpage_validateversion.
    /// </returns>
    public static string forumpage_validateversion(int appVersion)
    {
      string redirect = string.Empty;
      try
      {
        DataTable registry = registry_list("Version");

        if ((registry.Rows.Count == 0) || (Convert.ToInt32(registry.Rows[0]["Value"]) < appVersion))
        {
          // needs upgrading...
          redirect = "install/default.aspx?upgrade=" + Convert.ToInt32(registry.Rows[0]["Value"]);
        }
      }
      catch (SqlException)
      {
        // needs to be setup...
        redirect = "install/";
      }

      return redirect;
    }

    /// <summary>
    /// The system_deleteinstallobjects.
    /// </summary>
    public static void system_deleteinstallobjects()
    {
      string tSQL = "DROP PROCEDURE" + YafDBAccess.GetObjectName("system_initialize");
      using (SqlCommand cmd = YafDBAccess.GetCommand(tSQL, true))
      {
        cmd.CommandType = CommandType.Text;
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The system_initialize_executescripts.
    /// </summary>
    /// <param name="script">
    /// The script.
    /// </param>
    /// <param name="scriptFile">
    /// The script file.
    /// </param>
    /// <param name="useTransactions">
    /// The use transactions.
    /// </param>
    /// <exception cref="Exception">
    /// </exception>
    public static void system_initialize_executescripts(string script, string scriptFile, bool useTransactions)
    {
      script = YafDBAccess.GetCommandTextReplaced(script);

      List<string> statements = Regex.Split(script, "\\sGO\\s", RegexOptions.IgnoreCase).ToList();
      ushort sqlMajVersion = SqlServerMajorVersionAsShort();
      using (var connMan = new YafDBConnManager())
      {
        // use transactions...
        if (useTransactions)
        {
          using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(YafDBAccess.IsolationLevel))
          {
            foreach (string sql0 in statements)
            {
              string sql = sql0.Trim();

              sql = YafDBAccess.CleanForSQLServerVersion(sql, sqlMajVersion);

              try
              {
                if (sql.ToLower().IndexOf("setuser") >= 0)
                {
                  continue;
                }

                if (sql.Length > 0)
                {
                  using (var cmd = new SqlCommand())
                  {
                    // added so command won't timeout anymore...
                    cmd.CommandTimeout = 99999;
                    cmd.Transaction = trans;
                    cmd.Connection = connMan.DBConnection;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql.Trim();
                    cmd.ExecuteNonQuery();
                  }
                }
              }
              catch (Exception x)
              {
                trans.Rollback();
                throw new Exception(String.Format("FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
              }
            }

            trans.Commit();
          }
        }
        else
        {
          // don't use transactions
          foreach (string sql0 in statements)
          {
            string sql = sql0.Trim();

            // add ARITHABORT option
            // sql = "SET ARITHABORT ON\r\nGO\r\n" + sql;

            try
            {
              if (sql.ToLower().IndexOf("setuser") >= 0)
              {
                continue;
              }

              if (sql.Length > 0)
              {
                using (var cmd = new SqlCommand())
                {
                  cmd.Connection = connMan.OpenDBConnection;
                  cmd.CommandType = CommandType.Text;
                  cmd.CommandText = sql.Trim();
                  cmd.ExecuteNonQuery();
                }
              }
            }
            catch (Exception x)
            {
              throw new Exception(String.Format("FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
            }
          }
        }
      }
    }

    /// <summary>
    /// The system_initialize_fixaccess.
    /// </summary>
    /// <param name="grant">
    /// The grant.
    /// </param>
    public static void system_initialize_fixaccess(bool grant)
    {
      using (var connMan = new YafDBConnManager())
      {
        using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(YafDBAccess.IsolationLevel))
        {
          // REVIEW : Ederon - would "{databaseOwner}.{objectQualifier}" work, might need only "{objectQualifier}"
          using (
            var da =
              new SqlDataAdapter(
                "select Name,IsUserTable = OBJECTPROPERTY(id, N'IsUserTable'),IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like '{databaseOwner}.{objectQualifier}%'",
                connMan.OpenDBConnection))
          {
            da.SelectCommand.Transaction = trans;
            using (var dt = new DataTable("sysobjects"))
            {
              da.Fill(dt);
              using (SqlCommand cmd = connMan.DBConnection.CreateCommand())
              {
                cmd.Transaction = trans;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select current_user";
                var userName = (string)cmd.ExecuteScalar();

                if (grant)
                {
                  cmd.CommandType = CommandType.Text;
                  foreach (DataRow row in dt.Select("IsProcedure=1 or IsScalarFunction=1"))
                  {
                    cmd.CommandText = string.Format("grant execute on \"{0}\" to \"{1}\"", row["Name"], userName);
                    cmd.ExecuteNonQuery();
                  }

                  foreach (DataRow row in dt.Select("IsUserTable=1 or IsView=1"))
                  {
                    cmd.CommandText = string.Format("grant select,update on \"{0}\" to \"{1}\"", row["Name"], userName);
                    cmd.ExecuteNonQuery();
                  }
                }
                else
                {
                  cmd.CommandText = "sp_changeobjectowner";
                  cmd.CommandType = CommandType.StoredProcedure;
                  foreach (DataRow row in dt.Select("IsUserTable=1"))
                  {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@objname", row["Name"]);
                    cmd.Parameters.AddWithValue("@newowner", "dbo");
                    try
                    {
                      cmd.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                    }
                  }

                  foreach (DataRow row in dt.Select("IsView=1"))
                  {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@objname", row["Name"]);
                    cmd.Parameters.AddWithValue("@newowner", "dbo");
                    try
                    {
                      cmd.ExecuteNonQuery();
                    }
                    catch (SqlException)
                    {
                    }
                  }
                }
              }
            }
          }

          trans.Commit();
        }
      }
    }

    /// <summary>
    /// The system_initialize.
    /// </summary>
    /// <param name="forumName">
    /// The forum name.
    /// </param>
    /// <param name="timeZone">
    /// The time zone.
    /// </param>
    /// <param name="forumEmail">
    /// The forum email.
    /// </param>
    /// <param name="smtpServer">
    /// The smtp server.
    /// </param>
    /// <param name="userName">
    /// The user name.
    /// </param>
    /// <param name="userEmail">
    /// The user email.
    /// </param>
    /// <param name="providerUserKey">
    /// The provider user key.
    /// </param>
    public static void system_initialize(
      string forumName, string timeZone, string culture, string languageFile, string forumEmail, string smtpServer, string userName, string userEmail, object providerUserKey)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("system_initialize"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Name", forumName);
        cmd.Parameters.AddWithValue("@TimeZone", timeZone);
        cmd.Parameters.AddWithValue("@Culture", culture);
        cmd.Parameters.AddWithValue("@LanguageFile", languageFile);
        cmd.Parameters.AddWithValue("@ForumEmail", forumEmail);
        cmd.Parameters.AddWithValue("@SmtpServer", string.Empty);
        cmd.Parameters.AddWithValue("@User", userName);
        cmd.Parameters.AddWithValue("@UserEmail", userEmail);
        cmd.Parameters.AddWithValue("@UserKey", providerUserKey);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// The system_updateversion.
    /// </summary>
    /// <param name="version">
    /// The version.
    /// </param>
    /// <param name="versionname">
    /// The versionname.
    /// </param>
    public static void system_updateversion(int version, string versionname)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("system_updateversion"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Version", version);
        cmd.Parameters.AddWithValue("@VersionName", versionname);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    public static ushort SqlServerMajorVersionAsShort()
    {
        using (SqlCommand cmd = YafDBAccess.GetCommand("SELECT SUBSTRING(CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')), 1, PATINDEX('%.%', CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')))-1)", true))
      {
        return Convert.ToUInt16(YafDBAccess.Current.ExecuteScalar(cmd));
      }
    }

    /// <summary>
    /// Returns info about all Groups and Rank styles. 
    /// Used in GroupRankStyles cache.
    /// Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name 
    /// </summary>
    /// <param name="boardID">
    /// The board ID.
    /// </param>
    public static DataTable group_rank_style(object boardID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("group_rank_style"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@BoardID", boardID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    #endregion

    #region DLESKTECH_ShoutBox

    /// <summary>
    /// The shoutbox_getmessages.
    /// </summary>
    /// <param name="boardId"></param>
    /// <param name="numberOfMessages">
    /// The number of messages.
    /// </param>
    /// <param name="useStyledNicks">
    /// Use style for user nicks in ShoutBox.
    /// </param>
    /// <returns>
    /// </returns>
    public static DataTable shoutbox_getmessages(int boardId, int numberOfMessages, object useStyledNicks)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("shoutbox_getmessages"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("NumberOfMessages", numberOfMessages);
        cmd.Parameters.AddWithValue("BoardId", boardId);
        cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// The shoutbox_savemessage.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="userName">
    /// The usern name.
    /// </param>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The shoutbox_savemessage.
    /// </returns>
    public static bool shoutbox_savemessage(int boardId, string message, string userName, int userID, object ip)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("shoutbox_savemessage"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardId", boardId);
        cmd.Parameters.AddWithValue("Message", message);
        cmd.Parameters.AddWithValue("UserName", userName);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("IP", ip);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return true;
      }
    }

    /// <summary>
    /// The shoutbox_clearmessages.
    /// </summary>
    /// <returns>
    /// The shoutbox_clearmessages.
    /// </returns>
    public static bool shoutbox_clearmessages(int boardId)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("shoutbox_clearmessages"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("BoardId", boardId);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return true;
      }
    }

    #endregion

    #region Touradg Mods

    // Shinking Operation
    /// <summary>
    /// The db_shrink_warning.
    /// </summary>
    /// <param name="DBName">
    /// The db name.
    /// </param>
    /// <returns>
    /// The db_shrink_warning.
    /// </returns>
    public static string db_shrink_warning(YafDBConnManager DBName)
    {
      return string.Empty;
    }

    /// <summary>
    /// The db_shrink.
    /// </summary>
    /// <param name="DBName">
    /// The db name.
    /// </param>
    public static void db_shrink(YafDBConnManager DBName)
    {
      string ShrinkSql = "DBCC SHRINKDATABASE(N'" + DBName.DBConnection.Database + "')";
      var ShrinkConn = new SqlConnection(Config.ConnectionString);
      var ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
      ShrinkConn.Open();
      ShrinkCmd.ExecuteNonQuery();
      ShrinkConn.Close();
      using (var cmd = new SqlCommand(ShrinkSql, DBName.OpenDBConnection))
      {
        cmd.Connection = DBName.DBConnection;
        cmd.CommandTimeout = 9999;
        cmd.ExecuteNonQuery();
      }
    }

    // Set Recovery
    /// <summary>
    /// The db_recovery_mode_warning.
    /// </summary>
    /// <param name="DBName">
    /// The db name.
    /// </param>
    /// <returns>
    /// The db_recovery_mode_warning.
    /// </returns>
    public static string db_recovery_mode_warning(YafDBConnManager DBName)
    {
      return string.Empty;
    }

    /// <summary>
    /// The db_recovery_mode.
    /// </summary>
    /// <param name="DBName">
    /// The db name.
    /// </param>
    /// <param name="dbRecoveryMode">
    /// The db recovery mode.
    /// </param>
    public static void db_recovery_mode(YafDBConnManager DBName, string dbRecoveryMode)
    {
      string RecoveryMode = "ALTER DATABASE " + DBName.DBConnection.Database + " SET RECOVERY " + dbRecoveryMode;
      var RecoveryModeConn = new SqlConnection(Config.ConnectionString);
      var RecoveryModeCmd = new SqlCommand(RecoveryMode, RecoveryModeConn);
      RecoveryModeConn.Open();
      RecoveryModeCmd.ExecuteNonQuery();
      RecoveryModeConn.Close();
      using (var cmd = new SqlCommand(RecoveryMode, DBName.OpenDBConnection))
      {
        cmd.Connection = DBName.DBConnection;
        cmd.CommandTimeout = 9999;
        cmd.ExecuteNonQuery();
      }
    }

    #endregion
    #region Buddy
    /// <summary>
    /// Adds a buddy request. (Should be approved later by "ToUserID")
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user + Whether this request is approved or not.
    /// </returns>
    public static string[] buddy_addrequest(object FromUserID, object ToUserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("buddy_addrequest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        var approved = new SqlParameter("approved", SqlDbType.Bit);
        paramOutput.Direction = ParameterDirection.Output;
        approved.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("ToUserID", ToUserID);
        cmd.Parameters.Add(paramOutput);
        cmd.Parameters.Add(approved);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return new string[] { paramOutput.Value.ToString(), approved.Value.ToString() };
      }
    }

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <param name="Mutual">
    /// Should the requesting user (ToUserID) be added to FromUserID's buddy list too?
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    public static string buddy_approveRequest(object FromUserID, object ToUserID, object Mutual)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("buddy_approverequest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("ToUserID", ToUserID);
        cmd.Parameters.AddWithValue("mutual", Mutual);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    public static string buddy_denyRequest(object FromUserID, object ToUserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("buddy_denyrequest"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("ToUserID", ToUserID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

    /// <summary>
    /// Removes the "ToUserID" from "FromUserID"'s buddy list.
    /// </summary>
    /// <param name="FromUserID">
    /// The from user id.
    /// </param>
    /// <param name="ToUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    public static string buddy_remove(object FromUserID, object ToUserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("buddy_remove"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        cmd.Parameters.AddWithValue("ToUserID", ToUserID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

      /// <summary>
      /// Gets all the buddies of a certain user.
      /// </summary>
      /// <param name="FromUserID">
      /// The from user id.
      /// </param>
      /// <returns>
      /// a Datatable containing the buddy list.
      /// </returns>
      public static DataTable buddy_list(object FromUserID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("buddy_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("FromUserID", FromUserID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }
    #endregion

    #region Album

    /// <summary>
    /// Inserts/Saves a user album.
    /// </summary>
    /// <param name="albumID">
    /// AlbumID of an existing Album.
    /// </param>
    /// <param name="userID">
    /// UserID of the user who wants to create a new album.
    /// </param>
    /// <param name="title">
    /// New Album title.
    /// </param>
    /// <param name="coverImageID">
    /// New Cover image id.
    /// </param>
    public static int album_save(object albumID, object userID, object title, object coverImageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter();
        paramOutput.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("Title", title);
        cmd.Parameters.AddWithValue("CoverImageID", coverImageID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return Convert.ToInt32(paramOutput.Value);
      }
    }

    /// <summary>
    /// Lists all the albums associated with the UserID or gets all the
    /// specifications for the specified album id.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <param name="albumID">
    /// the album id.
    /// </param>
    /// <returns>
    /// a Datatable containing the albums.
    /// </returns>
    public static DataTable album_list(object userID, object albumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Deletes an album and all Images in that album.
    /// </summary>
    /// <param name="albumID">
    /// the album id.
    /// </param>
    public static void album_delete(object albumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Deletes an album and all Images in that album.
    /// </summary>
    /// <param name="albumID">
    /// the album id.
    /// </param>
    public static string album_gettitle(object albumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_gettitle"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
        paramOutput.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        cmd.Parameters.Add(paramOutput);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
        return paramOutput.Value.ToString();
      }
    }

    /// <summary>
    /// The album_ get stats method.
    /// </summary>
    /// <param name="userID">
    /// the User ID.
    /// </param>
    /// <param name="albumID">
    /// the album id.
    /// </param>
    /// <returns>
    /// The number of albums + number of current uploaded files by the user if UserID is not null,
    /// Otherwise, it gets the number of images in the album with AlbumID.
    /// </returns>
    public static int[] album_getstats(object userID, object albumID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_getstats"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        var paramAlbumNumber = new SqlParameter("AlbumNumber", 0);
        paramAlbumNumber.Direction = ParameterDirection.Output;
        var paramImageNumber = new SqlParameter("ImageNumber", 0);
        paramImageNumber.Direction = ParameterDirection.Output;
        cmd.Parameters.AddWithValue("UserID", userID);
        cmd.Parameters.AddWithValue("AlbumID", albumID);

        cmd.Parameters.Add(paramAlbumNumber);
        cmd.Parameters.Add(paramImageNumber);
        YafDBAccess.Current.ExecuteNonQuery(cmd);

        int albumNumber = paramAlbumNumber.Value == DBNull.Value ?
            0 : Convert.ToInt32(paramAlbumNumber.Value);
        int imageNumber = paramImageNumber.Value == DBNull.Value ?
            0 : Convert.ToInt32(paramImageNumber.Value);
        return new[] { albumNumber, imageNumber };
      }
    }

    /// <summary>
    /// Inserts/Saves a user image.
    /// </summary>
    /// <param name="imageID">
    /// the image id of an existing image.
    /// </param>
    /// <param name="albumID">
    /// the album id for adding a new image.
    /// </param>
    /// <param name="caption">
    /// the caption of the existing/new image.
    /// </param>
    /// <param name="fileName">
    /// the file name of the new image.
    /// </param>
    /// <param name="bytes">
    /// the size of the new image.
    /// </param>
    /// <param name="contentType">
    /// the content type.
    /// </param>
    public static void album_image_save(object imageID, object albumID, object caption, object fileName, object bytes, object contentType)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_image_save"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ImageID", imageID);
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        cmd.Parameters.AddWithValue("Caption", caption);
        cmd.Parameters.AddWithValue("FileName", fileName);
        cmd.Parameters.AddWithValue("Bytes", bytes);
        cmd.Parameters.AddWithValue("ContentType", contentType);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Lists all the images associated with the AlbumID or
    /// the image with the ImageID.
    /// </summary>
    /// <param name="albumID">
    /// the Album id.
    /// </param>
    /// <param name="imageID">
    /// The image id.
    /// </param>
    /// <returns>
    /// a Datatable containing the image(s).
    /// </returns>
    public static DataTable album_image_list(object albumID, object imageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_image_list"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("AlbumID", albumID);
        cmd.Parameters.AddWithValue("ImageID", imageID);
        return YafDBAccess.Current.GetData(cmd);
      }
    }

    /// <summary>
    /// Deletes the image which has the specified image id.
    /// </summary>
    /// <param name="imageID">
    /// the image id.
    /// </param>
    public static void album_image_delete(object imageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_image_delete"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ImageID", imageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }

    /// <summary>
    /// Increments the image's download times.
    /// </summary>
    /// <param name="imageID">
    /// the image id.
    /// </param>
    public static void album_image_download(object imageID)
    {
      using (SqlCommand cmd = YafDBAccess.GetCommand("album_image_download"))
      {
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("ImageID", imageID);
        YafDBAccess.Current.ExecuteNonQuery(cmd);
      }
    }
    #endregion

    public static void unencode_all_topics_subjects(Func<string,string> decodeTopicFunc)
    {
      var topics = DB.topic_simplelist(0, 99999999).SelectTypedList(r => new TypedTopicSimpleList(r)).ToList();

      foreach (var topic in topics.Where(t => t.TopicID.HasValue && t.Topic.IsSet()))
      {
        try
        {
          var decodedTopic = decodeTopicFunc(topic.Topic);

          if (!decodedTopic.Equals(topic.Topic))
          {
            // unencode it and update.
            DB.topic_updatetopic(topic.TopicID.Value, decodedTopic);
          }

        }
        catch
        {
          // soft-fail...
        }
      }
    }
  }
}