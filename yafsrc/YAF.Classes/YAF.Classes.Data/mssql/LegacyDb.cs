/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Classes.Data
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Hosting;
    using System.Web.Security;

    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Handlers;
    using YAF.Types.Interfaces;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// All the Database functions for YAF
    /// </summary>
    public static class LegacyDb
    {
        // Parameter 10
        #region Constants and Fields

        /// <summary>
        ///   The _script list.
        /// </summary>
        private static readonly string[] _scriptList = 
        {
            "mssql/tables.sql", 
            "mssql/indexes.sql", 
            "mssql/views.sql",
            "mssql/constraints.sql", 
            "mssql/triggers.sql",
            "mssql/functions.sql", 
            "mssql/procedures.sql",
            "mssql/forum_ns.sql",
            "mssql/providers/tables.sql",
            "mssql/providers/indexes.sql",
            "mssql/providers/procedures.sql" 
        };

        /// <summary>
        ///   The _full text script.
        /// </summary>
        private static string _fullTextScript = "mssql/fulltext.sql";

        /// <summary>
        ///   The _full text supported.
        /// </summary>
        private static bool _fullTextSupported = true;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether IsForumInstalled.
        /// </summary>
        public static bool GetIsForumInstalled()
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

        /// <summary>
        ///   Gets the database size
        /// </summary>
        /// <returns>intager value for database size</returns>
        public static int GetDBSize()
        {
            using (var cmd = new SqlCommand("select sum(cast(size as integer))/128 from sysfiles"))
            {
                cmd.CommandType = CommandType.Text;
                return (int) MsSqlDbAccess.Current.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        ///   Gets DBVersion.
        /// </summary>
        public static int GetDBVersion()
        {
            try
            {
                using (DataTable dt = registry_list("version"))
                {
                    if (dt.Rows.Count > 0)
                    {
                        // get the version...
                        return dt.Rows[0]["Value"].ToType<int>();
                    }
                }
            }
            catch
            {
                // not installed...
            }

            return -1;
        }

        /// <summary>
        ///   Gets or sets FullTextScript.
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
        ///   Gets or sets a value indicating whether FullTextSupported.
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
        ///   Gets a value indicating whether PanelGetStats.
        /// </summary>
        public static bool PanelGetStats
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether PanelRecoveryMode.
        /// </summary>
        public static bool PanelRecoveryMode
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether PanelReindex.
        /// </summary>
        public static bool PanelReindex
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether PanelShrink.
        /// </summary>
        public static bool PanelShrink
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets Parameter10_Name.
        /// </summary>
        [NotNull]
        public static string Parameter10_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter10_Value.
        /// </summary>
        [NotNull]
        public static string Parameter10_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter10_Visible.
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
        ///   Gets Parameter11_Name.
        /// </summary>
        [NotNull]
        public static string Parameter11_Name
        {
            get
            {
                return "Use Integrated Security";
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter11_Value.
        /// </summary>
        public static bool Parameter11_Value
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter11_Visible.
        /// </summary>
        public static bool Parameter11_Visible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets Parameter12_Name.
        ///   (reserved for 'User Instance' in MS SQL SERVER)
        /// </summary>
        [NotNull]
        public static string Parameter12_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter12_Value.
        /// </summary>
        public static bool Parameter12_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter12_Visible.
        /// </summary>
        public static bool Parameter12_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter13_Name.
        /// </summary>
        [NotNull]
        public static string Parameter13_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter13_Value.
        /// </summary>
        public static bool Parameter13_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter13_Visible.
        /// </summary>
        public static bool Parameter13_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter14_Name.
        /// </summary>
        [NotNull]
        public static string Parameter14_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter14_Value.
        /// </summary>
        public static bool Parameter14_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter14_Visible.
        /// </summary>
        public static bool Parameter14_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter15_Name.
        /// </summary>
        [NotNull]
        public static string Parameter15_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter15_Value.
        /// </summary>
        public static bool Parameter15_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter15_Visible.
        /// </summary>
        public static bool Parameter15_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter16_Name.
        /// </summary>
        [NotNull]
        public static string Parameter16_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter16_Value.
        /// </summary>
        public static bool Parameter16_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter16_Visible.
        /// </summary>
        public static bool Parameter16_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter17_Name.
        /// </summary>
        [NotNull]
        public static string Parameter17_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter17_Value.
        /// </summary>
        public static bool Parameter17_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter17_Visible.
        /// </summary>
        public static bool Parameter17_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter18_Name.
        /// </summary>
        [NotNull]
        public static string Parameter18_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter18_Value.
        /// </summary>
        public static bool Parameter18_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter18_Visible.
        /// </summary>
        public static bool Parameter18_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter19_Name.
        /// </summary>
        [NotNull]
        public static string Parameter19_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter19_Value.
        /// </summary>
        public static bool Parameter19_Value
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter19_Visible.
        /// </summary>
        public static bool Parameter19_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter1_Name.
        /// </summary>
        [NotNull]
        public static string Parameter1_Name
        {
            get
            {
                return "Data Source";
            }
        }

        /// <summary>
        ///   Gets Parameter1_Value.
        /// </summary>
        [NotNull]
        public static string Parameter1_Value
        {
            get
            {
                return "(local)";
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter1_Visible.
        /// </summary>
        public static bool Parameter1_Visible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets Parameter2_Name.
        /// </summary>
        [NotNull]
        public static string Parameter2_Name
        {
            get
            {
                return "Initial Catalog";
            }
        }

        /// <summary>
        ///   Gets Parameter2_Value.
        /// </summary>
        [NotNull]
        public static string Parameter2_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter2_Visible.
        /// </summary>
        public static bool Parameter2_Visible
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///   Gets Parameter3_Name.
        /// </summary>
        [NotNull]
        public static string Parameter3_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter3_Value.
        /// </summary>
        [NotNull]
        public static string Parameter3_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter3_Visible.
        /// </summary>
        public static bool Parameter3_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter4_Name.
        /// </summary>
        [NotNull]
        public static string Parameter4_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter4_Value.
        /// </summary>
        [NotNull]
        public static string Parameter4_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter4_Visible.
        /// </summary>
        public static bool Parameter4_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter5_Name.
        /// </summary>
        [NotNull]
        public static string Parameter5_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter5_Value.
        /// </summary>
        [NotNull]
        public static string Parameter5_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter5_Visible.
        /// </summary>
        public static bool Parameter5_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter6_Name.
        /// </summary>
        [NotNull]
        public static string Parameter6_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter6_Value.
        /// </summary>
        [NotNull]
        public static string Parameter6_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter6_Visible.
        /// </summary>
        public static bool Parameter6_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter7_Name.
        /// </summary>
        [NotNull]
        public static string Parameter7_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter7_Value.
        /// </summary>
        [NotNull]
        public static string Parameter7_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter7_Visible.
        /// </summary>
        public static bool Parameter7_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter8_Name.
        /// </summary>
        [NotNull]
        public static string Parameter8_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter8_Value.
        /// </summary>
        [NotNull]
        public static string Parameter8_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter8_Visible.
        /// </summary>
        public static bool Parameter8_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets Parameter9_Name.
        /// </summary>
        [NotNull]
        public static string Parameter9_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets Parameter9_Value.
        /// </summary>
        [NotNull]
        public static string Parameter9_Value
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Parameter9_Visible.
        /// </summary>
        public static bool Parameter9_Visible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets a value indicating whether PasswordPlaceholderVisible.
        /// </summary>
        public static bool PasswordPlaceholderVisible
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   Gets ProviderAssemblyName.
        /// </summary>
        [NotNull]
        public static string ProviderAssemblyName
        {
            get
            {
                return "System.Data.SqlClient";
            }
        }

        /// <summary>
        ///   Gets ScriptList.
        /// </summary>
        public static string[] ScriptList
        {
            get
            {
                return _scriptList;
            }
        }

        #endregion

        #region Public Methods

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
        /// Returns the BB Code List
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedBBCode> BBCodeList(int boardID, int? bbcodeID)
        {
            return bbcode_list(boardID, bbcodeID).AsEnumerable().Select(o => new TypedBBCode(o));
        }

        /// <summary>
        /// The forum list all.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns The forum list all.
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedForumListAll> ForumListAll(int boardId, int userId)
        {
            return forum_listall(boardId, userId, 0).AsEnumerable().Select(r => new TypedForumListAll(r));
        }

        /// <summary>
        /// The forum list all.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="startForumId">
        /// The start forum id.
        /// </param>
        /// <returns>
        /// The forum list all.
        /// </returns>
        [NotNull]
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
        /// <param name="searchDisplayName">
        /// The search Display Name.
        /// </param>
        /// <returns>
        /// Results
        /// </returns>
        public static DataTable GetSearchResult(
            [NotNull] string toSearchWhat,
            [NotNull] string toSearchFromWho,
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

            string searchSql = new SearchBuilder().BuildSearchSql(
              toSearchWhat,
              toSearchFromWho,
              searchFromWhoMethod,
              searchWhatMethod,
              userID,
              searchDisplayName,
              boardId,
              maxResults,
              useFullText,
              forumIds);

            using (var cmd = MsSqlDbAccess.GetCommand(searchSql, true))
            {
                return MsSqlDbAccess.Current.GetData(cmd);
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
        [NotNull]
        public static IEnumerable<TypedMailList> MailList(long processId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("mail_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ProcessID", processId);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd).SelectTypedList(x => new TypedMailList(x));
            }
        }

        /// <summary>
        /// Retuns All the Thanks for the Message IDs which are in the 
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIdsSeparatedWithColon">
        /// The message i ds.
        /// </param>
        [NotNull]
        public static IEnumerable<TypedAllThanks> MessageGetAllThanks([NotNull] string messageIdsSeparatedWithColon)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_getallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageIDs", messageIdsSeparatedWithColon);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(t => new TypedAllThanks(t));
            }
        }

        /// <summary>
        /// The message_list.
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        [NotNull]
        public static IEnumerable<TypedMessageList> MessageList(int messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(t => new TypedMessageList(t));
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
        [NotNull]
        public static IEnumerable<TypedPollGroup> PollGroupList(int userID, int? forumId, int boardId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pollgroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumId);
                cmd.Parameters.AddWithValue("BoardID", boardId);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(r => new TypedPollGroup(r));
            }
        }

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
        [NotNull]
        public static IEnumerable<TypedSmileyList> SmileyList(int boardID, int? smileyID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("SmileyID", smileyID);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(r => new TypedSmileyList(r));
            }
        }

        /// <summary>
        /// The sql server major version as short.
        /// </summary>
        /// <returns>
        /// The sql server major version as short.
        /// </returns>
        public static ushort SqlServerMajorVersionAsShort()
        {
            using (
              var cmd =
                MsSqlDbAccess.GetCommand(
                  "SELECT SUBSTRING(CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')), 1, PATINDEX('%.%', CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')))-1)",
                  true))
            {
                return Convert.ToUInt16(MsSqlDbAccess.Current.ExecuteScalar(cmd));
            }
        }

        /// <summary>
        /// Get the favorite count for a topic...
        /// </summary>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// The topic favorite count.
        /// </returns>
        public static int TopicFavoriteCount(int topicId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_favorite_count"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicId);

                return MsSqlDbAccess.Current.GetData(cmd).GetFirstRowColumnAsValue("FavoriteCount", 0);
            }
        }

        /// <summary>
        /// The UserFind.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <param name="displayName">
        /// </param>
        /// <param name="notificationType">
        /// </param>
        /// <param name="dailyDigest">
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedUserFind> UserFind(
          int boardID,
          bool filter, [NotNull] string userName, [NotNull] string email, [NotNull] string displayName, [NotNull] object notificationType, [NotNull] object dailyDigest)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_find"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Filter", filter);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("DisplayName", displayName);
                cmd.Parameters.AddWithValue("NotificationType", notificationType);
                cmd.Parameters.AddWithValue("DailyDigest", dailyDigest);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(u => new TypedUserFind(u));
            }
        }

        /// <summary>
        /// Get the user list as a typed list.
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
        /// The use styled nicks.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static IEnumerable<TypedUserList> UserList(
          int boardID, int? userID, bool? approved, int? groupID, int? rankID, bool? useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Approved", approved);
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("RankID", rankID);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(x => new TypedUserList(x));
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
        public static bool accessmask_delete([NotNull] object accessMaskID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("accessmask_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd) != 0;
            }
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
        /// <returns>
        /// </returns>
        public static DataTable accessmask_list([NotNull] object boardID, [NotNull] object accessMaskID)
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
        public static DataTable accessmask_list([NotNull] object boardID, [NotNull] object accessMaskID, [NotNull] object excludeFlags)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("accessmask_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
                cmd.Parameters.AddWithValue("ExcludeFlags", excludeFlags);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void accessmask_save([NotNull] object accessMaskID, [NotNull] object boardID, [NotNull] object name, [NotNull] object readAccess, [NotNull] object postAccess, [NotNull] object replyAccess, [NotNull] object priorityAccess, [NotNull] object pollAccess, [NotNull] object voteAccess, [NotNull] object moderatorAccess, [NotNull] object editAccess, [NotNull] object deleteAccess, [NotNull] object uploadAccess, [NotNull] object downloadAccess, [NotNull] object sortOrder)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("accessmask_save"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets list of active users
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="Guests">
        /// </param>
        /// <param name="showCrawlers">
        /// The show Crawlers.
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
        public static DataTable active_list([NotNull] object boardID, [NotNull] object Guests, [NotNull] object showCrawlers, int activeTime, [NotNull] object styledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("active_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Guests", Guests);
                cmd.Parameters.AddWithValue("ShowCrawlers", showCrawlers);
                cmd.Parameters.AddWithValue("ActiveTime", activeTime);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="showCrawlers">
        /// The show Crawlers.
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
        public static DataTable active_list_user([NotNull] object boardID, [NotNull] object userID, [NotNull] object Guests, [NotNull] object showCrawlers, int activeTime, [NotNull] object styledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("active_list_user"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Guests", Guests);
                cmd.Parameters.AddWithValue("ShowCrawlers", showCrawlers);
                cmd.Parameters.AddWithValue("ActiveTime", activeTime);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable active_listforum([NotNull] object forumID, [NotNull] object styledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("active_listforum"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable active_listtopic([NotNull] object topicID, [NotNull] object styledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("active_listtopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataRow active_stats([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("active_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    return dt.Rows[0];
                }
            }
        }

        /// <summary>
        /// The activeaccess_reset.
        /// </summary>
        public static void activeaccess_reset()
        {
            using (var cmd = MsSqlDbAccess.GetCommand("activeaccess_reset"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The admin_list.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable admin_list([CanBeNull] object boardId, [NotNull] object useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("admin_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The admin_pageaccesslist.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use styled nicks.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable admin_pageaccesslist([CanBeNull] object boardId, [NotNull] object useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("admin_pageaccesslist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Saves access for an admin user for an admin page.  
        /// </summary>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="pageName">
        /// The page name/
        /// </param>
        /// <param name="readAccess">
        /// The read access. 
        /// </param>
        public static void adminpageaccess_save([NotNull] object userId, [NotNull] object pageName)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("adminpageaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("PageName", pageName);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        ///  Deletes access for an admin user for an admin page.
        /// </summary>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="pageName">
        /// The page name/
        /// </param>
        public static void adminpageaccess_delete([NotNull] object userId, [CanBeNull] object pageName)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("adminpageaccess_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("PageName", pageName);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Returns access lists for admin pages.
        /// </summary>
        /// <param name="userId">
        /// The user ID.
        /// </param>
        /// <param name="pageName">
        /// The page name/
        /// </param>
        /// <returns>A DataTable with access lists for admin pages.</returns>
        public static DataTable adminpageaccess_list([CanBeNull] object userId, [CanBeNull] object pageName)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("adminpageaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("PageName", pageName);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Deletes an album and all Images in that album.
        /// </summary>
        /// <param name="albumID">
        /// the album id.
        /// </param>
        public static void album_delete([NotNull] object albumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        ///   Otherwise, it gets the number of images in the album with AlbumID.
        /// </returns>
        [NotNull]
        public static int[] album_getstats([NotNull] object userID, [NotNull] object albumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_getstats"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);

                int albumNumber = paramAlbumNumber.Value == DBNull.Value ? 0 : Convert.ToInt32(paramAlbumNumber.Value);
                int imageNumber = paramImageNumber.Value == DBNull.Value ? 0 : Convert.ToInt32(paramImageNumber.Value);
                return new[] { albumNumber, imageNumber };
            }
        }

        /// <summary>
        /// Deletes an album and all Images in that album.
        /// </summary>
        /// <param name="albumID">
        /// the album id.
        /// </param>
        /// <returns>
        /// The album_gettitle.
        /// </returns>
        [NotNull]
        public static string album_gettitle([NotNull] object albumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_gettitle"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return paramOutput.Value.ToString();
            }
        }

        /// <summary>
        /// Deletes the image which has the specified image id.
        /// </summary>
        /// <param name="imageID">
        /// the image id.
        /// </param>
        public static void album_image_delete([NotNull] object imageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_image_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ImageID", imageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Increments the image's download times.
        /// </summary>
        /// <param name="imageID">
        /// the image id.
        /// </param>
        public static void album_image_download([NotNull] object imageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_image_download"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ImageID", imageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Album images by users the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>All Albbum Images of the User</returns>
        public static DataTable album_images_by_user([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_images_by_user"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Lists all the images associated with the AlbumID or
        ///   the image with the ImageID.
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
        public static DataTable album_image_list([NotNull] object albumID, [NotNull] object imageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_image_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                cmd.Parameters.AddWithValue("ImageID", imageID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void album_image_save([NotNull] object imageID, [NotNull] object albumID, [NotNull] object caption, [NotNull] object fileName, [NotNull] object bytes, [NotNull] object contentType)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_image_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ImageID", imageID);
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                cmd.Parameters.AddWithValue("Caption", caption);
                cmd.Parameters.AddWithValue("FileName", fileName);
                cmd.Parameters.AddWithValue("Bytes", bytes);
                cmd.Parameters.AddWithValue("ContentType", contentType);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Lists all the albums associated with the UserID or gets all the
        ///   specifications for the specified album id.
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
        public static DataTable album_list([NotNull] object userID, [NotNull] object albumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        /// <returns>
        /// The album_save.
        /// </returns>
        public static int album_save([NotNull] object albumID, [NotNull] object userID, [NotNull] object title, [NotNull] object coverImageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("album_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter();
                paramOutput.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.AddWithValue("AlbumID", albumID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Title", title);
                cmd.Parameters.AddWithValue("CoverImageID", coverImageID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return Convert.ToInt32(paramOutput.Value);
            }
        }

        /// <summary>
        /// Delete attachment
        /// </summary>
        /// <param name="attachmentID">
        /// ID of attachment to delete
        /// </param>
        public static void attachment_delete([NotNull] object attachmentID)
        {
            bool useFileTable = GetBooleanRegistryValue("UseFileTable");

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                using (var cmd = MsSqlDbAccess.GetCommand("attachment_list"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
                    DataTable tbAttachments = MsSqlDbAccess.Current.GetData(cmd);

                    string uploadDir =
                      HostingEnvironment.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

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

            using (var cmd = MsSqlDbAccess.GetCommand("attachment_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }

            // End ABOT CHANGE 16.04.04
        }

        /// <summary>
        /// Attachement dowload
        /// </summary>
        /// <param name="attachmentID">
        /// ID of attachemnt to download
        /// </param>
        public static void attachment_download([NotNull] object attachmentID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("attachment_download"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable attachment_list([NotNull] object messageID, [NotNull] object attachmentID, [NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("attachment_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("AttachmentID", attachmentID);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void attachment_save([NotNull] object messageID, [NotNull] object fileName, [NotNull] object bytes, [NotNull] object contentType, [NotNull] Stream stream)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("attachment_save"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes Banned IP
        /// </summary>
        /// <param name="ID">
        /// ID of banned ip to delete
        /// </param>
        public static void bannedip_delete([NotNull] object ID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bannedip_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ID", ID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable bannedip_list([NotNull] object boardID, [NotNull] object ID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bannedip_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ID", ID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        public static void bannedip_save([NotNull] object ID, [NotNull] object boardID, [NotNull] object Mask, [NotNull] string reason, int userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bannedip_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ID", ID);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Mask", Mask);
                cmd.Parameters.AddWithValue("Reason", reason);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The bbcode_delete.
        /// </summary>
        /// <param name="bbcodeID">
        /// The bbcode id.
        /// </param>
        public static void bbcode_delete([NotNull] object bbcodeID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bbcode_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BBCodeID", bbcodeID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        [NotNull]
        public static DataTable bbcode_list([NotNull] object boardID, [NotNull] object bbcodeID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bbcode_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("BBCodeID", bbcodeID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void bbcode_save([NotNull] object bbcodeID, [NotNull] object boardID, [NotNull] object name, [NotNull] object description, [NotNull] object onclickjs, [NotNull] object displayjs, [NotNull] object editjs, [NotNull] object displaycss, [NotNull] object searchregex, [NotNull] object replaceregex, [NotNull] object variables, [NotNull] object usemodule, [NotNull] object moduleclass, [NotNull] object execorder)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("bbcode_save"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Creates a new board
        /// </summary>
        /// <param name="adminUsername">Membership Provider User Name</param>
        /// <param name="adminUserEmail">The admin User Email.</param>
        /// <param name="adminUserKey">Membership Provider User Key</param>
        /// <param name="boardName">Name of new board</param>
        /// <param name="culture">The culture.</param>
        /// <param name="languageFile">The language File.</param>
        /// <param name="boardMembershipName">Membership Provider Application Name for new board</param>
        /// <param name="boardRolesName">Roles Provider Application Name for new board</param>
        /// <param name="rolePrefix">The role Prefix.</param>
        /// <param name="isHostUser">The is host user.</param>
        /// <returns>
        /// The board_create.
        /// </returns>
        public static int board_create([NotNull] object adminUsername, [NotNull] object adminUserEmail, [NotNull] object adminUserKey, [NotNull] object boardName, [NotNull] object culture, [NotNull] object languageFile, [NotNull] object boardMembershipName, [NotNull] object boardRolesName, [NotNull] object rolePrefix, [NotNull] object isHostUser)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_create"))
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
                cmd.Parameters.AddWithValue("IsHostAdmin", isHostUser);
                cmd.Parameters.AddWithValue("RolePrefix", rolePrefix);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// Deletes a board
        /// </summary>
        /// <param name="boardID">
        /// ID of board to delete
        /// </param>
        public static void board_delete([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Gets a list of information about a board
        /// </summary>
        /// <param name="boardID">
        /// board id
        /// </param>
        /// <returns>
        /// DataTable
        /// </returns>
        public static DataTable board_list([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
            using (var cmd = MsSqlDbAccess.GetCommand("board_poststats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNick);
                cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);

                cmd.Parameters.AddWithValue("GetDefaults", 0);
                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }

            // vzrus - this happens at new install only when we don't have posts or when they are not visible to a user 
            using (var cmd = MsSqlDbAccess.GetCommand("board_poststats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNick);
                cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
                cmd.Parameters.AddWithValue("GetDefaults", 1);
                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
            }

            return null;
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
        public static void board_resync([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_resync"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Saves board information
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <param name="languageFile">
        /// The language File.
        /// </param>
        /// <param name="culture">
        /// The culture.
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
        public static int board_save([NotNull] object boardID, [NotNull] object languageFile, [NotNull] object culture, [NotNull] object name, [NotNull] object allowThreaded)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("LanguageFile", languageFile);
                cmd.Parameters.AddWithValue("Culture", culture);
                cmd.Parameters.AddWithValue("AllowThreaded", allowThreaded);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        public static DataRow board_stats([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("board_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);

                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    return dt.Rows[0];
                }
            }
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
            using (var cmd = MsSqlDbAccess.GetCommand("board_userstats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    return dt.Rows[0];
                }
            }
        }

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
        [NotNull]
        public static string[] buddy_addrequest([NotNull] object FromUserID, [NotNull] object ToUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("buddy_addrequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                var approved = new SqlParameter("approved", SqlDbType.Bit);
                paramOutput.Direction = ParameterDirection.Output;
                approved.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("ToUserID", ToUserID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                cmd.Parameters.Add(approved);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return new[] { paramOutput.Value.ToString(), approved.Value.ToString() };
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
        [NotNull]
        public static string buddy_approveRequest([NotNull] object FromUserID, [NotNull] object ToUserID, [NotNull] object Mutual)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("buddy_approverequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("ToUserID", ToUserID);
                cmd.Parameters.AddWithValue("mutual", Mutual);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        [NotNull]
        public static string buddy_denyRequest([NotNull] object FromUserID, [NotNull] object ToUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("buddy_denyrequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("ToUserID", ToUserID);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable buddy_list([NotNull] object FromUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("buddy_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        [NotNull]
        public static string buddy_remove([NotNull] object FromUserID, [NotNull] object ToUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("buddy_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("ToUserID", ToUserID);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return paramOutput.Value.ToString();
            }
        }

        /// <summary>
        /// Deletes a category
        /// </summary>
        /// <param name="CategoryID">
        /// ID of category to delete
        /// </param>
        /// <returns>
        /// Bool value indicationg if category was deleted
        /// </returns>
        public static bool category_delete([NotNull] object CategoryID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("category_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("CategoryID", CategoryID);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd) != 0;
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
        public static DataTable category_list([NotNull] object boardID, [CanBeNull] object categoryID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("category_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("CategoryID", categoryID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable category_listread([NotNull] object boardID, [NotNull] object userID, [NotNull] object categoryID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("category_listread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("CategoryID", categoryID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void category_save([NotNull] object boardID, [NotNull] object categoryId, [NotNull] object name, [NotNull] object categoryImage, [NotNull] object sortOrder)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("category_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("CategoryImage", categoryImage);
                cmd.Parameters.AddWithValue("SortOrder", sortOrder);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
            using (var cmd = MsSqlDbAccess.GetCommand("category_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StartID", startID);
                cmd.Parameters.AddWithValue("Limit", limit);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable checkemail_list([NotNull] object email)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("checkemail_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Email", email);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        public static void checkemail_save([NotNull] object userID, [NotNull] object hash, [NotNull] object email)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("checkemail_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Hash", hash);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable checkemail_update([NotNull] object hash)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("checkemail_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Hash", hash);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="mime">
        /// The mime.
        /// </param>
        public static void choice_add([NotNull] object pollID, [NotNull] object choice, [NotNull] object path, [NotNull] object mime)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("choice_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollID", pollID);
                cmd.Parameters.AddWithValue("Choice", choice);
                cmd.Parameters.AddWithValue("ObjectPath", path);
                cmd.Parameters.AddWithValue("MimeType", mime);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The choice_delete.
        /// </summary>
        /// <param name="choiceID">
        /// The choice id.
        /// </param>
        public static void choice_delete([NotNull] object choiceID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("choice_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ChoiceID", choiceID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="mime">
        /// The mime.
        /// </param>
        public static void choice_update([NotNull] object choiceID, [NotNull] object choice, [NotNull] object path, [NotNull] object mime)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("choice_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ChoiceID", choiceID);
                cmd.Parameters.AddWithValue("Choice", choice);
                cmd.Parameters.AddWithValue("ObjectPath", path);
                cmd.Parameters.AddWithValue("MimeType", mime);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static void choice_vote([NotNull] object choiceID, [NotNull] object userID, [NotNull] object remoteIP)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("choice_vote"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ChoiceID", choiceID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("RemoteIP", remoteIP);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The db_getstats.
        /// </summary>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        public static void db_getstats([NotNull] MsSqlDbConnectionManager connectionManager)
        {
            // create statistic getting SQL...
            var sb = new StringBuilder();

            sb.AppendLine("DECLARE @TableName sysname");
            sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
            sb.AppendFormat(
              "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
              Config.DatabaseObjectQualifier);
            sb.AppendLine("OPEN cur_showfragmentation");
            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
            sb.AppendLine("WHILE @@FETCH_STATUS = 0");
            sb.AppendLine("BEGIN");
            sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
            sb.AppendLine("END");
            sb.AppendLine("CLOSE cur_showfragmentation");
            sb.AppendLine("DEALLOCATE cur_showfragmentation");

            using (var cmd = new SqlCommand(sb.ToString(), connectionManager.OpenDBConnection))
            {
                cmd.Connection = connectionManager.DBConnection;

                // up the command timeout...
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                // run it...
                cmd.ExecuteNonQuery();
            }
        }

        private static string getStatsMessage;
        /// <summary>
        /// The db_getstats_new.
        /// </summary>
        public static string db_getstats_new()
        {
            try
            {
                using (var connMan = new MsSqlDbConnectionManager())
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(getStats_InfoMessage);

                    connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;

                    // create statistic getting SQL...
                    var sb = new StringBuilder();

                    sb.AppendLine("DECLARE @TableName sysname");
                    sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
                    sb.AppendFormat(
                        "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
                        Config.DatabaseObjectQualifier);
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
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it...
                        cmd.ExecuteNonQuery();
                        return getStatsMessage;
                    }

                }
            }
            finally
            {
                getStatsMessage = string.Empty;
            }
        }

        /// <summary>
        /// The reindexDb_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void getStats_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            getStatsMessage += "\r\n{0}".FormatWith(e.Message);
        }

        /// <summary>
        /// The db_getstats_warning.
        /// </summary>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        /// <returns>
        /// The db_getstats_warning.
        /// </returns>
        [NotNull]
        public static string db_getstats_warning()
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
        public static void db_recovery_mode([NotNull] MsSqlDbConnectionManager DBName, [NotNull] string dbRecoveryMode)
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
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                cmd.ExecuteNonQuery();
            }
        }

        private static string recoveryDbModeMessage;

        /// <summary>
        /// The db_recovery_mode.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        /// <param name="dbRecoveryMode">
        /// The db recovery mode.
        /// </param>
        public static string db_recovery_mode_new([NotNull] string dbRecoveryMode)
        {
            try
            {
                using (var connMan = new MsSqlDbConnectionManager())
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(recoveryDbMode_InfoMessage);
                    var RecoveryModeConn = new SqlConnection(Config.ConnectionString);
                    RecoveryModeConn.Open();
                   
                    string RecoveryMode = "ALTER DATABASE " + connMan.DBConnection.Database + " SET RECOVERY " + dbRecoveryMode;
                    var RecoveryModeCmd = new SqlCommand(RecoveryMode, RecoveryModeConn);
                   
                    RecoveryModeCmd.ExecuteNonQuery();
                    RecoveryModeConn.Close();
                    using (var cmd = new SqlCommand(RecoveryMode, connMan.OpenDBConnection))
                    {
                        cmd.Connection = connMan.DBConnection;
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                        cmd.ExecuteNonQuery();
                        return recoveryDbModeMessage;
                    }

                }
            }
            catch (Exception error)
            {
                string expressDb = string.Empty;
                if (error.Message.ToUpperInvariant().Contains("'SET'"))
                {
                    expressDb = "MS SQL Server Express Editions are not supported by the application.";
                }
                recoveryDbModeMessage += "\r\n{0}\r\n{1}".FormatWith(error.Message, expressDb);
                return recoveryDbModeMessage;
            }
           
            finally
            {
                recoveryDbModeMessage = string.Empty;
            }

            
            
        }
        /// <summary>
        /// The recoveryDbMode_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void recoveryDbMode_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            recoveryDbModeMessage += "\r\n{0}".FormatWith(e.Message);
        }

        /// <summary>
        /// The db_recovery_mode_warning.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        /// <returns>
        /// The db_recovery_mode_warning.
        /// </returns>
        [NotNull]
        public static string db_recovery_mode_warning([NotNull] MsSqlDbConnectionManager DBName)
        {
            return string.Empty;
        }

        /// <summary>
        /// The db_reindex.
        /// </summary>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        public static void db_reindex([NotNull] MsSqlDbConnectionManager connectionManager)
        {
           
            // create statistic getting SQL...
            var sb = new StringBuilder();

            sb.AppendLine("DECLARE @MyTable VARCHAR(255)");
            sb.AppendLine("DECLARE myCursor");
            sb.AppendLine("CURSOR FOR");
            sb.AppendFormat(
              "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
              Config.DatabaseObjectQualifier);
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

            using (var cmd = new SqlCommand(sb.ToString(), connectionManager.OpenDBConnection))
            {
                cmd.Connection = connectionManager.DBConnection;

                // up the command timeout...
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                // run it...
                cmd.ExecuteNonQuery();
            }
        }
        
        /// <summary>
        /// The db_recovery_mode_warning.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        /// <returns>
        /// The db_recovery_mode_warning.
        /// </returns>
        [NotNull]
        public static string db_recovery_mode_warning()
        {
            return string.Empty;
        }

        private static string reindexDbMessage;

        /// <summary>
        /// The db_reindex_new.
        /// </summary>
        public static string db_reindex_new()
        {
            try
            {
                using (var connMan = new MsSqlDbConnectionManager())
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(reindexDb_InfoMessage);
                    connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
                    // create statistic getting SQL...
                    var sb = new StringBuilder();

                    sb.AppendLine("DECLARE @MyTable VARCHAR(255)");
                    sb.AppendLine("DECLARE myCursor");
                    sb.AppendLine("CURSOR FOR");
                    sb.AppendFormat(
                      "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
                      Config.DatabaseObjectQualifier);
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
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                        // run it...
                        cmd.ExecuteNonQuery();
                    }
                    return reindexDbMessage;
                }
            }
            finally
            {
                reindexDbMessage = string.Empty;
            }
        }

        /// <summary>
        /// The reindexDb_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void reindexDb_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            reindexDbMessage += "\r\n{0}".FormatWith(e.Message);
        }

        /// <summary>
        /// The db_reindex_warning.
        /// </summary>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        /// <returns>
        /// The db_reindex_warning.
        /// </returns>
        [NotNull]
        public static string db_reindex_warning()
        {
            return string.Empty;
        }

        /// <summary>
        /// The db_runsql.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        /// <param name="useTransaction">
        /// The use Transaction.
        /// </param>
        /// <returns>
        /// The db_runsql.
        /// </returns>
        public static string db_runsql([NotNull] string sql, [NotNull] MsSqlDbConnectionManager connectionManager, bool useTransaction)
        {
            using (var command = new SqlCommand(sql, connectionManager.OpenDBConnection))
            {
                command.CommandTimeout = 9999;
                command.Connection = connectionManager.OpenDBConnection;

                return InnerRunSqlExecuteReader(command, useTransaction);
            }
        }

        private static string messageRunSql;
        /// <summary>
        /// The db_runsql.
        /// </summary>
        /// <param name="sql">
        /// The sql.
        /// </param>
        /// <param name="connectionManager">
        /// The conn man.
        /// </param>
        /// <param name="useTransaction">
        /// The use Transaction.
        /// </param>
        /// <returns>
        /// The db_runsql.
        /// </returns>
        public static string db_runsql_new([NotNull] string sql, bool useTransaction)
        {

            try
            {
                using (var connMan = new MsSqlDbConnectionManager())
                {
                    connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(runSql_InfoMessage);
                    connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;
                    sql = MsSqlDbAccess.GetCommandTextReplaced(sql.Trim());

                    using (var command = new SqlCommand(sql, connMan.OpenDBConnection))
                    {
                        command.CommandTimeout = 9999;
                        command.Connection = connMan.OpenDBConnection;

                        return InnerRunSqlExecuteReader(command, useTransaction);
                    }
                }
            }
            finally
            {
                messageRunSql = string.Empty;
            }
  
           
        }

        /// <summary>
        /// The runSql_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void runSql_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            messageRunSql = "\r\n" + e.Message;
        }
        /// <summary>
        /// The db_shrink.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        public static void db_shrink([NotNull] MsSqlDbConnectionManager DBName)
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
                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                cmd.ExecuteNonQuery();
            }
        }

        private static string dbShinkMessage;
        /// <summary>
        /// The db_shrink.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        public static string db_shrink_new()
        {
            try
            {
                using (var conn = new MsSqlDbConnectionManager())
                {
                    conn.InfoMessage += new YafDBConnInfoMessageEventHandler(dbShink_InfoMessage);
                    conn.DBConnection.FireInfoMessageEventOnUserErrors = true;
                    string ShrinkSql = "DBCC SHRINKDATABASE(N'" + conn.DBConnection.Database + "')";
                    var ShrinkConn = new SqlConnection(Config.ConnectionString);
                    var ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
                    ShrinkConn.Open();
                    ShrinkCmd.ExecuteNonQuery();
                    ShrinkConn.Close();
                    using (var cmd = new SqlCommand(ShrinkSql, conn.OpenDBConnection))
                    {
                        cmd.Connection = conn.DBConnection;
                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                        cmd.ExecuteNonQuery();
                    }
                }
                return dbShinkMessage;
            }
            finally
            {
                dbShinkMessage = string.Empty;
            }

        }

        /// <summary>
        /// The runSql_InfoMessage.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void dbShink_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        {
            dbShinkMessage = "\r\n" + e.Message;
        }

        /// <summary>
        /// The db_shrink_warning.
        /// </summary>
        /// <param name="DBName">
        /// The db name.
        /// </param>
        /// <returns>
        /// The db_shrink_warning.
        /// </returns>
        [NotNull]
        public static string db_shrink_warning()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets a list of categories????
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        /// <returns>
        /// DataSet with categories
        /// </returns>
        [NotNull]
        public static DataSet ds_forumadmin([NotNull] object boardID)
        {
            // TODO: this function is TERRIBLE. Recode or remove completely.
            using (var connMan = new MsSqlDbConnectionManager())
            {
                using (var ds = new DataSet())
                {
                    using (var trans = connMan.OpenDBConnection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
                    {
                        using (var da = new SqlDataAdapter(MsSqlDbAccess.GetObjectName("category_list"), connMan.DBConnection))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.Parameters.AddWithValue("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, MsSqlDbAccess.GetObjectName("Category"));
                            da.SelectCommand.CommandText = MsSqlDbAccess.GetObjectName("forum_list");
                            da.Fill(ds, MsSqlDbAccess.GetObjectName("ForumUnsorted"));

                            DataTable dtForumListSorted = ds.Tables[MsSqlDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = MsSqlDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            forum_list_sort_basic(
                              ds.Tables[MsSqlDbAccess.GetObjectName("ForumUnsorted")],
                              ds.Tables[MsSqlDbAccess.GetObjectName("Forum")],
                              0,
                              0);
                            ds.Tables.Remove(MsSqlDbAccess.GetObjectName("ForumUnsorted"));
                            ds.Relations.Add(
                              "FK_Forum_Category",
                              ds.Tables[MsSqlDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                              ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);
                            trans.Commit();
                        }

                        return ds;
                    }
                }
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
        /// <param name="type">
        /// The type.
        /// </param>
        public static void eventlog_create([NotNull] object userID, [NotNull] object source, [NotNull] object description, [NotNull] object type)
        {
            try
            {
                if (userID == null)
                {
                    userID = DBNull.Value;
                }

                using (var cmd = MsSqlDbAccess.GetCommand("eventlog_create"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("Type", type);
                    cmd.Parameters.AddWithValue("UserID", userID);
                    cmd.Parameters.AddWithValue("Source", source.ToString());
                    cmd.Parameters.AddWithValue("Description", description.ToString());
                    cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void eventlog_create([NotNull] object userID, [NotNull] object source, [NotNull] object description)
        {
            eventlog_create(userID, source.GetType().ToString(), description, 0);
        }

        /// <summary>
        /// Deletes all event log entries for given board.
        /// </summary>
        /// <param name="boardID">
        /// ID of board.
        /// </param>
        public static void eventlog_delete(int boardID, int pageUserId)
        {
            eventlog_delete(null, boardID, pageUserId);
        }

        /// <summary>
        /// Deletes event log entry of given ID.
        /// </summary>
        /// <param name="eventLogID">
        /// ID of event log entry.
        /// </param>
        public static void eventlog_delete([NotNull] object eventLogID, int pageUserId)
        {
            eventlog_delete(eventLogID, null,pageUserId);
        }

        /// <summary>
        /// The eventlog_list.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="pageUserID"> 
        /// The page user ID.
        /// </param>
        /// <param name="maxRows"> 
        /// The max Rows.
        /// </param>
        /// <param name="maxDays"> 
        /// The max Days. 
        /// </param>
        /// <param name="pageIndex"> 
        /// The page index. 
        /// </param>
        /// <param name="pageSize"> 
        /// The page size. 
        /// </param>
        /// <param name="sinceDate"> 
        /// The since date. 
        /// </param>
        /// <param name="toDate"> 
        /// The to date. 
        /// </param>
        /// <param name="eventIDs">
        /// Comma delimited list event types.
        /// </param>
        /// <returns>
        /// A list of events for the pageUserID access level. 
        /// </returns>
        public static DataTable eventlog_list([NotNull] object boardID, [NotNull] object pageUserID, [NotNull] object maxRows, [NotNull] object maxDays, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object eventIDs)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventlog_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("PageUserID", pageUserID);
                cmd.Parameters.AddWithValue("MaxRows", maxRows);
                cmd.Parameters.AddWithValue("MaxDays", maxDays);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("EventIDs", eventIDs);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Saves access entry for a log type for a user.
        /// </summary>
        /// <param name="groupID">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <param name="eventTypeName">
        /// The event Type Name.
        /// </param>
        public static void eventloggroupaccess_save([NotNull] object groupID, [NotNull] object eventTypeId, [NotNull] object eventTypeName, [NotNull] object deleteAccess)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventloggroupaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("EventTypeID", eventTypeId);
                cmd.Parameters.AddWithValue("EventTypeName", eventTypeName);
                cmd.Parameters.AddWithValue("DeleteAccess", deleteAccess);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }
       
        /// <summary>
        /// Deletes event log access entries from table.
        /// </summary>
        /// <param name="groupID">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <param name="eventTypeName">
        /// The event Type Name.
        /// </param>
        public static void eventloggroupaccess_delete([NotNull] object groupID, [NotNull] object eventTypeId, [NotNull] object eventTypeName)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventloggroupaccess_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("EventTypeID", eventTypeId);
                cmd.Parameters.AddWithValue("EventTypeName", eventTypeName);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Returns a list of access entries for a group.
        /// </summary>
        /// <param name="groupID">
        /// The group Id.
        /// </param>
        /// <param name="eventTypeId">
        /// The event Type Id.
        /// </param>
        /// <returns>Returns a list of access entries for a group.</returns>
        public static DataTable eventloggroupaccess_list([NotNull] object groupID, [NotNull] object eventTypeId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventloggroupaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("EventTypeID", eventTypeId);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Lists group for the board Id handy to display on the calling admin page. 
        /// </summary>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>Lists group for the board Id handy to display on the calling admin page.
        /// </returns>
        public static DataTable group_eventlogaccesslist([CanBeNull] object boardId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_eventlogaccesslist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BoardID", boardId);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The extension_delete.
        /// </summary>
        /// <param name="extensionId">
        /// The extension id.
        /// </param>
        public static void extension_delete([NotNull] object extensionId)
        {
            try
            {
                using (var cmd = MsSqlDbAccess.GetCommand("extension_delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ExtensionId", extensionId);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable extension_edit([NotNull] object extensionId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("extension_edit"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("extensionId", extensionId);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable extension_list([NotNull] object boardID, [NotNull] object extension)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("extension_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Extension", extension);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The extension list for a given Board
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <returns>
        /// Returns an extension list for a given Board
        /// </returns>
        public static DataTable extension_list([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("extension_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Extension", string.Empty);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Saves / creates extension
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
        public static void extension_save([NotNull] object extensionId, [NotNull] object boardID, [NotNull] object Extension)
        {
            try
            {
                using (var cmd = MsSqlDbAccess.GetCommand("extension_save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("extensionId", extensionId);
                    cmd.Parameters.AddWithValue("BoardId", boardID);
                    cmd.Parameters.AddWithValue("Extension", Extension);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        /// <summary>
        /// Delete a topic status.
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        public static void TopicStatus_Delete([NotNull] object topicStatusID)
        {
            try
            {
                using (var cmd = MsSqlDbAccess.GetCommand("TopicStatus_Delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("TopicStatusID", topicStatusID);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        /// <summary>
        /// Get a Topic Status by topicStatusID
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_Edit([NotNull] object topicStatusID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("TopicStatus_Edit"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicStatusID", topicStatusID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// List all Topics of the Current Board
        /// </summary>
        /// <param name="boardID">The board ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_List([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("TopicStatus_List"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Saves a topic status
        /// </summary>
        /// <param name="topicStatusID">The topic status ID.</param>
        /// <param name="boardID">The board ID.</param>
        /// <param name="topicStatusName">Name of the topic status.</param>
        /// <param name="defaultDescription">The default description.</param>
        public static void TopicStatus_Save([NotNull] object topicStatusID, [NotNull] object boardID, [NotNull] object topicStatusName, [NotNull] object defaultDescription)
        {
            try
            {
                using (var cmd = MsSqlDbAccess.GetCommand("TopicStatus_Save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("TopicStatusID", topicStatusID);
                    cmd.Parameters.AddWithValue("BoardId", boardID);
                    cmd.Parameters.AddWithValue("TopicStatusName", topicStatusName);
                    cmd.Parameters.AddWithValue("DefaultDescription", defaultDescription);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                }
            }
            catch
            {
                // Ignore any errors in this method
            }
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// bool to indicate that forum has been deleted
        /// </returns>
        public static bool forum_delete([NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);

                if (!(MsSqlDbAccess.Current.ExecuteScalar(cmd) is DBNull))
                {
                    return false;
                }

                forum_deleteAttachments(forumID);
                
                using (SqlCommand cmd_new = MsSqlDbAccess.GetCommand("forum_delete"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd_new.Parameters.AddWithValue("ForumID", forumID);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd_new);
                }

                return true;
            }
        }

        /// <summary>
        /// Deletes a forum
        /// </summary>
        /// <param name="forumOldID">
        /// The forum Old ID.
        /// </param>
        /// <param name="forumNewID">
        /// The forum New ID.
        /// </param>
        /// <returns>
        /// bool to indicate that forum has been deleted
        /// </returns>
        public static bool forum_move([NotNull] object forumOldID, [NotNull] object forumNewID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumOldID);

                if (!(MsSqlDbAccess.Current.ExecuteScalar(cmd) is DBNull))
                {
                    return false;
                }

                using (SqlCommand cmd_new = MsSqlDbAccess.GetCommand("forum_move"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd_new.Parameters.AddWithValue("ForumOldID", forumOldID);
                    cmd_new.Parameters.AddWithValue("ForumNewID", forumNewID);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd_new);
                }

                return true;
            }
        }

        // END ABOT CHANGE 16.04.04
        // ABOT NEW 16.04.04: This new function lists all moderated topic by the specified user

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
        public static DataTable forum_list([NotNull] object boardID, [CanBeNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        // END ABOT CHANGE 16.04.04
        // ABOT NEW 16.04.04: This new function lists all moderated topic by the specified user

       
        /// <summary>
        /// Gets a max id of forums.
        /// </summary>
        /// <param name="boardID">
        /// boardID
        /// </param>
        /// <returns>
        /// DataTable with list of topics from a forum
        /// </returns>
        public static int forum_maxid([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_maxid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return Convert.ToInt32(MsSqlDbAccess.Current.ExecuteScalar(cmd));
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
        public static DataTable forum_listall([NotNull] object boardID, [NotNull] object userID)
        {
            return forum_listall(boardID, userID, 0);
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
        public static DataTable forum_listall([NotNull] object boardID, [NotNull] object userID, [NotNull] object startAt)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Root", startAt);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        public static DataTable forum_listallMyModerated([NotNull] object boardID, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listallmymoderated"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable forum_listall_fromCat([NotNull] object boardID, [NotNull] object categoryID)
        {
            return forum_listall_fromCat(boardID, categoryID, true);
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
        public static DataTable forum_listall_fromCat([NotNull] object boardID, [NotNull] object categoryID, bool emptyFirstRow)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listall_fromCat"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("CategoryID", categoryID);

                int intCategoryID = Convert.ToInt32(categoryID.ToString());

                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    return forum_sort_list(dt, 0, intCategoryID, 0, null, emptyFirstRow);
                }
            }
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
        public static DataTable forum_listall_sorted([NotNull] object boardID, [NotNull] object userID)
        {
            return !MsSqlDbAccess.LargeForumTree ? forum_listall_sorted(boardID, userID, null, false, 0) : forum_ns_getchildren_activeuser((int)boardID, 0, 0, (int)userID, false, false, "-");
        }

        static public DataTable forum_ns_getchildren_anyuser(int boardid, int categoryid, int forumid, int userid, bool notincluded, bool immediateonly, string indentchars)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_ns_getchildren_anyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardid);
                cmd.Parameters.AddWithValue("CategoryID", categoryid);
                cmd.Parameters.AddWithValue("ForumID", forumid);
                cmd.Parameters.AddWithValue("UserID", userid);
                cmd.Parameters.AddWithValue("NotIncluded", notincluded);
                cmd.Parameters.AddWithValue("ImmediateOnly", immediateonly);

                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                DataTable sorted = dt.Clone();
                bool forumRow = false;
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = sorted.NewRow();
                    newRow.ItemArray = row.ItemArray;
                    newRow = row;

                    int currentIndent = (int)row["Level"];
                    string sIndent = "";

                    for (int j = 0; j < currentIndent; j++)
                        sIndent += "--";
                    if (currentIndent == 1 && !forumRow)
                    {
                        newRow["ForumID"] = currentIndent;
                        newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["CategoryName"]);
                        forumRow = true;
                    }
                    else
                    {
                        newRow["ForumID"] = currentIndent;
                        newRow["Title"] = string.Format(" -{0} {1}", sIndent, row["Title"]);
                        forumRow = false;
                    }

                    // import the row into the destination
                    sorted.Rows.Add(newRow);
                }
                return sorted;
            }
        }

        static public DataTable forum_ns_getchildren_activeuser(int boardid, int categoryid, int forumid, int userid, bool notincluded, bool immediateonly, string indentchars)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_ns_getchildren_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardid);
                cmd.Parameters.AddWithValue("CategoryID",categoryid);
                cmd.Parameters.AddWithValue("ForumID",forumid);
                cmd.Parameters.AddWithValue("UserID", userid);
                cmd.Parameters.AddWithValue("NotIncluded", notincluded);
                cmd.Parameters.AddWithValue("ImmediateOnly", immediateonly);

                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                DataTable sorted = dt.Clone();
                int categoryId = 0;
                foreach (DataRow row in dt.Rows)
                {
                    DataRow newRow = sorted.NewRow();
                    newRow.ItemArray = row.ItemArray;

                    int currentIndent = (int)row["Level"];
                    string sIndent = "";


                    if (currentIndent >= 2)
                    {
                        for (int j = 0; j < currentIndent - 1; j++)
                        {
                            sIndent += "-";
                            if (currentIndent > 2)
                            {
                                sIndent += "-";
                            }
                        }
                    }

                    if ((int)row["CategoryID"] != categoryId)
                    {
                        DataRow cRow = sorted.NewRow();
                        // we add a category
                        cRow["ForumID"] = -(int)row["CategoryID"];
                        cRow["Title"] = string.Format(" {0}", row["CategoryName"]);
                        categoryId = (int)row["CategoryID"];
                        sorted.Rows.Add(cRow);

                    }

                    newRow["ForumID"] = row["ForumID"];
                    newRow["Title"] = string.Format(" {0} {1}", sIndent, row["Title"]);
                    sorted.Rows.Add(newRow);

                }
                return sorted;

            }
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
        public static DataTable forum_listall_sorted([NotNull] object boardID, [NotNull] object userID, [NotNull] int[] forumidExclusions)
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
        [NotNull]
        public static DataTable forum_listall_sorted([NotNull] object boardID, [NotNull] object userID, [NotNull] int[] forumidExclusions, bool emptyFirstRow, int startAt)
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
                        if (Convert.ToInt32(dataRow["ForumID"]) == startAt && dataRow["ParentID"] != DBNull.Value &&
                            dataRow["CategoryID"] != DBNull.Value)
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
        /// Sorry no idea what this does
        /// </summary>
        /// <param name="forumID">
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable forum_listpath([NotNull] object forumID)
        {
           

            if (!MsSqlDbAccess.LargeForumTree)
            {

                using (var cmd = MsSqlDbAccess.GetCommand("forum_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ForumID", forumID);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
            }
            else
            {
                using (var cmd = MsSqlDbAccess.GetCommand("forum_ns_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("ForumID", forumID);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
            }
        }

        /// <summary>
        /// Lists read topics
        /// </summary>
        /// <param name="boardID">
        ///  The Board ID
        /// </param>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="categoryID">
        /// The category ID.
        /// </param>
        /// <param name="parentID">
        /// The Parent ID.
        /// </param>
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// DataTable with list
        /// </returns>
        public static DataTable forum_listread([NotNull] object boardID, [NotNull] object userID, [NotNull] object categoryID, [NotNull] object parentID, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
           
            if (!MsSqlDbAccess.LargeForumTree)
            {


                using (var cmd = MsSqlDbAccess.GetCommand("forum_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("BoardID", boardID);
                    cmd.Parameters.AddWithValue("UserID", userID);
                    cmd.Parameters.AddWithValue("CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("ParentID", parentID);
                    cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                    cmd.Parameters.AddWithValue("FindLastRead", findLastRead);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
            }
            else
            {
                using (var cmd = MsSqlDbAccess.GetCommand("forum_ns_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("BoardID", boardID);
                    cmd.Parameters.AddWithValue("UserID", userID);
                    cmd.Parameters.AddWithValue("CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("ParentID", parentID);
                    cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                    cmd.Parameters.AddWithValue("FindLastRead", findLastRead);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
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
        [NotNull]
        public static DataSet forum_moderatelist([NotNull] object userID, [NotNull] object boardID)
        {
            using (var connMan = new MsSqlDbConnectionManager())
            {
                using (var ds = new DataSet())
                {
                    using (var da = new SqlDataAdapter(MsSqlDbAccess.GetObjectName("category_list"), connMan.OpenDBConnection))
                    {
                        using (SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.Parameters.AddWithValue("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, MsSqlDbAccess.GetObjectName("Category"));
                            da.SelectCommand.CommandText = MsSqlDbAccess.GetObjectName("forum_moderatelist");
                            da.SelectCommand.Parameters.AddWithValue("UserID", userID);
                            da.Fill(ds, MsSqlDbAccess.GetObjectName("ForumUnsorted"));
                            DataTable dtForumListSorted = ds.Tables[MsSqlDbAccess.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = MsSqlDbAccess.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            forum_list_sort_basic(
                              ds.Tables[MsSqlDbAccess.GetObjectName("ForumUnsorted")],
                              ds.Tables[MsSqlDbAccess.GetObjectName("Forum")],
                              0,
                              0);
                            ds.Tables.Remove(MsSqlDbAccess.GetObjectName("ForumUnsorted"));

                            // vzrus: Remove here all forums with no reports. Would be better to do it in query...
                            // Array to write categories numbers
                            var categories = new int[ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].Rows.Count];
                            int cntr = 0;

                            // We should make it before too as the colection was changed
                            ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].Rows)
                            {
                                categories[cntr] = Convert.ToInt32(dr["CategoryID"]);
                                if (Convert.ToInt32(dr["ReportedCount"]) == 0 && Convert.ToInt32(dr["MessageCount"]) == 0)
                                {
                                    dr.Delete();
                                    categories[cntr] = 0;
                                }

                                cntr++;
                            }

                            ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].AcceptChanges();

                            foreach (DataRow dr in ds.Tables[MsSqlDbAccess.GetObjectName("Category")].Rows)
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

                                if (deleteMe)
                                {
                                    dr.Delete();
                                }
                            }

                            ds.Tables[MsSqlDbAccess.GetObjectName("Category")].AcceptChanges();

                            ds.Relations.Add(
                              "FK_Forum_Category",
                              ds.Tables[MsSqlDbAccess.GetObjectName("Category")].Columns["CategoryID"],
                              ds.Tables[MsSqlDbAccess.GetObjectName("Forum")].Columns["CategoryID"]);

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
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <returns>
        ///  Returns Data Table with all Mods
        /// </returns>
        public static DataTable forum_moderators(bool useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_moderators"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The moderators_team_list
        /// </summary>
        /// <param name="useStyledNicks">
        /// The use Styled Nicks.
        /// </param>
        /// <returns>
        ///  Returns Data Table with all Mods
        /// </returns>
        public static DataTable moderators_team_list(bool useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("moderators_team_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Updates topic and post count and last topic for all forums in specified board
        /// </summary>
        /// <param name="boardID">
        /// BoardID
        /// </param>
        public static void forum_resync([NotNull] object boardID)
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
        public static void forum_resync([NotNull] object boardID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_resync"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static long forum_save([NotNull] object forumID, [NotNull] object categoryID, [NotNull] object parentID, [NotNull] object name, [NotNull] object description, [NotNull] object sortOrder, [NotNull] object locked, [NotNull] object hidden, [NotNull] object isTest, [NotNull] object moderated, [NotNull] object accessMaskID, [NotNull] object remoteURL, [NotNull] object themeURL, [NotNull] object imageURL, [NotNull] object styles,
          bool dummy)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_save"))
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
                return long.Parse(MsSqlDbAccess.Current.ExecuteScalar(cmd).ToString());
            }
        }

        /// <summary>
        /// The method returns an integer value for a  found parent forum 
        ///   if a forum is a parent of an existing child to avoid circular dependency
        ///   while creating a new forum
        /// </summary>
        /// <param name="forumID">
        /// </param>
        /// <param name="parentID">
        /// </param>
        /// <returns>
        /// Integer value for a found dependency
        /// </returns>
        public static int forum_save_parentschecker([NotNull] object forumID, [NotNull] object parentID)
        {
            using (
              var cmd =
                MsSqlDbAccess.GetCommand(
                  "SELECT " + MsSqlDbAccess.GetObjectName("forum_save_parentschecker") + "(@ForumID,@ParentID)", true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ForumID", forumID);
                cmd.Parameters.AddWithValue("@ParentID", parentID);
                return Convert.ToInt32(MsSqlDbAccess.Current.ExecuteScalar(cmd));
            }
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
            using (var cmd = MsSqlDbAccess.GetCommand("forum_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StartID", StartID);
                cmd.Parameters.AddWithValue("Limit", Limit);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable forumaccess_group([NotNull] object groupID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forumaccess_group"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GroupID", groupID);
                return userforumaccess_sort_list(MsSqlDbAccess.Current.GetData(cmd), 0, 0, 0);
            }
        }

        /// <summary>
        /// The forumaccess_list.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable forumaccess_list([NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forumaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void forumaccess_save([NotNull] object forumID, [NotNull] object groupID, [NotNull] object accessMaskID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forumaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
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
        public static bool forumpage_initdb([NotNull] out string errorStr, bool debugging)
        {
            errorStr = string.Empty;

            try
            {
                using (var connMan = new MsSqlDbConnectionManager())
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

                if ((registry.Rows.Count == 0) || (registry.Rows[0]["Value"].ToType<int>() < appVersion))
                {
                    // needs upgrading...
                    redirect = "install/default.aspx?upgrade=" + registry.Rows[0]["Value"].ToType<int>();
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
        /// The group_delete.
        /// </summary>
        /// <param name="groupID">
        /// The group id.
        /// </param>
        public static void group_delete([NotNull] object groupID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GroupID", groupID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable group_list([NotNull] object boardID, [NotNull] object groupID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("GroupID", groupID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void group_medal_delete([NotNull] object groupID, [NotNull] object medalID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_medal_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("MedalID", medalID);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable group_medal_list([NotNull] object groupID, [NotNull] object medalID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_medal_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("MedalID", medalID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void group_medal_save([NotNull] object groupID, [NotNull] object medalID, [NotNull] object message, [NotNull] object hide, [NotNull] object onlyRibbon, [NotNull] object sortOrder)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_medal_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("MedalID", medalID);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("Hide", hide);
                cmd.Parameters.AddWithValue("OnlyRibbon", onlyRibbon);
                cmd.Parameters.AddWithValue("SortOrder", sortOrder);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable group_member([NotNull] object boardID, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_member"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Returns info about all Groups and Rank styles. 
        ///   Used in GroupRankStyles cache.
        ///   Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name
        /// </summary>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        public static DataTable group_rank_style([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_rank_style"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="description">
        /// The description.
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
        public static long group_save([NotNull] object groupID, [NotNull] object boardID, [NotNull] object name, [NotNull] object isAdmin, [NotNull] object isGuest, [NotNull] object isStart, [NotNull] object isModerator, [NotNull] object accessMaskID, [NotNull] object pmLimit, [NotNull] object style, [NotNull] object sortOrder, [NotNull] object description, [NotNull] object usrSigChars, [NotNull] object usrSigBBCodes, [NotNull] object usrSigHTMLTags, [NotNull] object usrAlbums, [NotNull] object usrAlbumImages)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("group_save"))
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

                return long.Parse(MsSqlDbAccess.Current.ExecuteScalar(cmd).ToString());
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
        public static void mail_create([NotNull] object from, [NotNull] object fromName, [NotNull] object to, [NotNull] object toName, [NotNull] object subject, [NotNull] object body, [NotNull] object bodyHtml)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("mail_create"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("From", from);
                cmd.Parameters.AddWithValue("FromName", fromName);
                cmd.Parameters.AddWithValue("To", to);
                cmd.Parameters.AddWithValue("ToName", toName);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Body", body);
                cmd.Parameters.AddWithValue("BodyHtml", bodyHtml);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void mail_createwatch([NotNull] object topicID, [NotNull] object from, [NotNull] object fromName, [NotNull] object subject, [NotNull] object body, [NotNull] object bodyHtml, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("mail_createwatch"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("From", from);
                cmd.Parameters.AddWithValue("FromName", fromName);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Body", body);
                cmd.Parameters.AddWithValue("BodyHtml", bodyHtml);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The mail_delete.
        /// </summary>
        /// <param name="mailID">
        /// The mail id.
        /// </param>
        public static void mail_delete([NotNull] object mailID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("mail_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MailID", mailID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes given medal.
        /// </summary>
        /// <param name="medalID">
        /// ID of medal to delete.
        /// </param>
        public static void medal_delete([NotNull] object medalID)
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
        public static void medal_delete([NotNull] object boardID, [NotNull] object category)
        {
            medal_delete(boardID, null, category);
        }

        /// <summary>
        /// Lists given medal.
        /// </summary>
        /// <param name="medalID">
        /// ID of medal to list.
        /// </param>
        public static DataTable medal_list([NotNull] object medalID)
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
        public static DataTable medal_list([NotNull] object boardID, [NotNull] object category)
        {
            return medal_list(boardID, null, category);
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
        public static DataTable medal_listusers([NotNull] object medalID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("medal_listusers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("MedalID", medalID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void medal_resort([NotNull] object boardID, [NotNull] object medalID, int move)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("medal_resort"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("MedalID", medalID);
                cmd.Parameters.AddWithValue("Move", move);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static bool medal_save([NotNull] object boardID, [NotNull] object medalID, [NotNull] object name, [NotNull] object description, [NotNull] object message, [NotNull] object category, [NotNull] object medalURL, [NotNull] object ribbonURL, [NotNull] object smallMedalURL, [NotNull] object smallRibbonURL, [NotNull] object smallMedalWidth, [NotNull] object smallMedalHeight, [NotNull] object smallRibbonWidth, [NotNull] object smallRibbonHeight, [NotNull] object sortOrder, [NotNull] object flags)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("medal_save"))
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
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd) > 0;
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
        [NotNull]
        public static string message_AddThanks([NotNull] object FromUserID, [NotNull] object MessageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_addthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("MessageID", MessageID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return paramOutput.Value.ToString();
            }
        }

        /// <summary>
        /// Retuns All the message text for the Message IDs which are in the 
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIDs">
        /// The message i ds.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_GetTextByIds([NotNull] string messageIDs)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_gettextbyids"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageIDs", messageIDs);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Returns the UserIDs and UserNames who have thanked the message
        ///   with the provided messageID.
        /// </summary>
        /// <param name="MessageID">
        /// The message id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_GetThanks([NotNull] object MessageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_getthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", MessageID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        [NotNull]
        public static string message_RemoveThanks([NotNull] object FromUserID, [NotNull] object MessageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_Removethanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("FromUserID", FromUserID);
                cmd.Parameters.AddWithValue("MessageID", MessageID);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return paramOutput.Value.ToString();
            }
        }

        /// <summary>
        /// The message_ thanks number.
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <returns>
        /// The message_ thanks number.
        /// </returns>
        public static int message_ThanksNumber([NotNull] object messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_thanksnumber"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter();
                paramOutput.Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.Add(paramOutput);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return Convert.ToInt32(paramOutput.Value);
            }
        }

        /// <summary>
        /// Set flag on message to approved and store in DB
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        public static void message_approve([NotNull] object messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void message_delete([NotNull] object messageID, bool isModeratorChanged, [NotNull] string deleteReason, int isDeleteAction, bool DeleteLinked)
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
        public static void message_delete([NotNull] object messageID,
          bool isModeratorChanged, [NotNull] string deleteReason,
          int isDeleteAction,
          bool DeleteLinked,
          bool eraseMessage)
        {
            message_deleteRecursively(
              messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false, eraseMessage);
        }

        /// <summary>
        /// The message_findunread.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="lastRead">
        /// The last read.
        /// </param>
        /// <param name="showDeleted">
        /// The show Deleted.
        /// </param>
        /// <param name="authorUserID">
        /// The author User ID.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_findunread([NotNull] object topicID, [NotNull] object messageId, [NotNull] object lastRead, [NotNull] object showDeleted, [NotNull] object authorUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_findunread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("MessageID", messageId);
                cmd.Parameters.AddWithValue("LastRead", lastRead);
                cmd.Parameters.AddWithValue("ShowDeleted", showDeleted);
                cmd.Parameters.AddWithValue("AuthorUserID", authorUserID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        [NotNull]
        public static DataTable message_getRepliesList([NotNull] object messageID)
        {
            var list = new DataTable();
            list.Columns.Add("MessageID", typeof(int));
            list.Columns.Add("Posted", typeof(DateTime));
            list.Columns.Add("Subject", typeof(string));
            list.Columns.Add("Message", typeof(string));
            list.Columns.Add("UserID", typeof(int));
            list.Columns.Add("Flags", typeof(int));
            list.Columns.Add("UserName", typeof(string));
            list.Columns.Add("Signature", typeof(string));

            using (var cmd = MsSqlDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                DataTable dtr = MsSqlDbAccess.Current.GetData(cmd);

                for (int i = 0; i < dtr.Rows.Count; i++)
                {
                    DataRow row = dtr.Rows[i];
                    DataRow newRow = list.NewRow();
                    newRow["MessageID"] = row["MessageID"];
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

        /// <summary>
        /// The message_list.
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <returns>
        /// </returns>
        [Obsolete("Use MessageList(int messageId) instead")]
        public static DataTable message_list([NotNull] object messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Retrieve all reported messages with the correct forumID argument.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_listreported([NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_listreported"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
            using (var cmd = MsSqlDbAccess.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("UserID", 0);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable message_listreporters(int messageID, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void message_move([NotNull] object messageID, [NotNull] object moveToTopic, bool moveAll)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("MoveToTopic", moveToTopic);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }

            // moveAll=true anyway
            // it's in charge of moving answers of moved post
            if (moveAll)
            {
                using (var cmd = MsSqlDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MessageID", messageID);
                    DataTable tbReplies = MsSqlDbAccess.Current.GetData(cmd);
                    foreach (DataRow row in tbReplies.Rows)
                    {
                        message_moveRecursively(row["MessageID"], moveToTopic);
                    }
                }
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
        public static void message_report([NotNull] object messageID, [NotNull] object userID, [NotNull] object reportedDateTime, [NotNull] object reportText)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_report"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("ReporterID", userID);
                cmd.Parameters.AddWithValue("ReportedDate", reportedDateTime);
                cmd.Parameters.AddWithValue("ReportText", reportText);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        // <summary> Copy current Message text over reported Message text. </summary>
        /// <summary>
        /// The message_reportcopyover.
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        public static void message_reportcopyover([NotNull] object messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_reportcopyover"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void message_reportresolve([NotNull] object messageFlag, [NotNull] object messageID, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_reportresolve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageFlag", messageFlag);
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static bool message_save([NotNull] object topicID, [NotNull] object userID, [NotNull] object message, [NotNull] object userName, [NotNull] object ip, [NotNull] object posted, [NotNull] object replyTo, [NotNull] object flags,
                                        ref long messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_save"))
            {
                var paramMessageID = new SqlParameter("MessageID", messageID) { Direction = ParameterDirection.Output };

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
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramMessageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                messageID = (long)paramMessageID.Value;
                return true;
            }
        }

        /// <summary>
        /// Returns message data based on user access rights
        /// </summary>
        /// <param name="MessageID">
        /// The Message Id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable message_secdata(int MessageID, [NotNull] object pageUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_secdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("MessageID", MessageID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
            using (var cmd = MsSqlDbAccess.GetCommand("message_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StartID", StartID);
                cmd.Parameters.AddWithValue("Limit", Limit);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable message_unapproved([NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_unapproved"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
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
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="status">
        /// The status.
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
        public static void message_update([NotNull] object messageID, [NotNull] object priority, [NotNull] object message, [NotNull] object description, [CanBeNull] object status, [CanBeNull] object styles, [NotNull] object subject, [NotNull] object flags, [NotNull] object reasonOfEdit, [NotNull] object isModeratorChanged, [NotNull] object overrideApproval, [NotNull] object originalMessage, [NotNull] object editedBy)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("Priority", priority);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.Parameters.AddWithValue("Status", status);
                cmd.Parameters.AddWithValue("Styles", styles);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Flags", flags);
                cmd.Parameters.AddWithValue("Reason", reasonOfEdit);
                cmd.Parameters.AddWithValue("EditedBy", editedBy);
                cmd.Parameters.AddWithValue("IsModeratorChanged", isModeratorChanged);
                cmd.Parameters.AddWithValue("OverrideApproval", overrideApproval);
                cmd.Parameters.AddWithValue("OriginalMessage", originalMessage);
                cmd.Parameters.AddWithValue("CurrentUtcTimestamp", DateTime.UtcNow);
                
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
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
        /// <param name="description">
        /// The description.
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
        public static void message_update([NotNull] object messageID, [NotNull] object priority, [NotNull] object message, [NotNull] object description, [NotNull] object subject, [NotNull] object flags, [NotNull] object reasonOfEdit, [NotNull] object isModeratorChanged, [NotNull] object overrideApproval, [NotNull] object originalMessage, [NotNull] object editedBy)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("Priority", priority);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Flags", flags);
                cmd.Parameters.AddWithValue("Reason", reasonOfEdit);
                cmd.Parameters.AddWithValue("EditedBy", editedBy);
                cmd.Parameters.AddWithValue("IsModeratorChanged", isModeratorChanged);
                cmd.Parameters.AddWithValue("OverrideApproval", overrideApproval);
                cmd.Parameters.AddWithValue("OriginalMessage", originalMessage);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        // <summary> Save message to DB. </summary>

        /// <summary>
        /// The messagehistory_list.
        /// </summary>
        /// <param name="messageId">
        /// The Message ID.
        /// </param>
        /// <param name="daysToClean">
        /// Days to clean.
        /// </param>
        /// <returns>
        /// List of all message changes. 
        /// </returns>
        public static DataTable messagehistory_list(int messageId, int daysToClean)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("messagehistory_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("MessageID", messageId);
                cmd.Parameters.AddWithValue("DaysToClean", daysToClean);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The nntpforum_delete.
        /// </summary>
        /// <param name="nntpForumID">
        /// The nntp forum id.
        /// </param>
        public static void nntpforum_delete([NotNull] object nntpForumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable nntpforum_list([NotNull] object boardID, [NotNull] object minutes, [NotNull] object nntpForumID, [NotNull] object active)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Minutes", minutes);
                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                cmd.Parameters.AddWithValue("Active", active);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        public static IEnumerable<TypedNntpForum> NntpForumList(int boardID, int? minutes, int? nntpForumID, bool? active)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Minutes", minutes);
                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                cmd.Parameters.AddWithValue("Active", active);

                return MsSqlDbAccess.Current.GetData(cmd).AsEnumerable().Select(r => new TypedNntpForum(r));
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
        /// <param name="datecutoff">
        /// The datecutoff.
        /// </param>
        public static void nntpforum_save([NotNull] object nntpForumID, [NotNull] object nntpServerID, [NotNull] object groupName, [NotNull] object forumID, [NotNull] object active, [NotNull] object datecutoff)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpforum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
                cmd.Parameters.AddWithValue("GroupName", groupName);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("Active", active);
                cmd.Parameters.AddWithValue("DateCutOff", datecutoff);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void nntpforum_update([NotNull] object nntpForumID, [NotNull] object lastMessageNo, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpforum_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                cmd.Parameters.AddWithValue("LastMessageNo", lastMessageNo);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The nntpserver_delete.
        /// </summary>
        /// <param name="nntpServerID">
        /// The nntp server id.
        /// </param>
        public static void nntpserver_delete([NotNull] object nntpServerID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpserver_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable nntpserver_list([NotNull] object boardID, [NotNull] object nntpServerID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpserver_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void nntpserver_save([NotNull] object nntpServerID, [NotNull] object boardID, [NotNull] object name, [NotNull] object address, [NotNull] object port, [NotNull] object userName, [NotNull] object userPass)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntpserver_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("NntpServerID", nntpServerID);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Address", address);
                cmd.Parameters.AddWithValue("Port", port);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("UserPass", userPass);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The nntptopic_list.
        /// </summary>
        /// <param name="thread">
        /// The thread.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable nntptopic_list([NotNull] object thread)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntptopic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Thread", thread);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="externalMessageId">
        /// The external Message Id.
        /// </param>
        /// <param name="referenceMessageId">
        /// The reference Message Id.
        /// </param>
        public static void nntptopic_savemessage([NotNull] object nntpForumID, [NotNull] object topic, [NotNull] object body, [NotNull] object userID, [NotNull] object userName, [NotNull] object ip, [NotNull] object posted, [NotNull] object externalMessageId, [NotNull] object referenceMessageId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("nntptopic_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("NntpForumID", nntpForumID);
                cmd.Parameters.AddWithValue("Topic", topic);
                cmd.Parameters.AddWithValue("Body", body);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("IP", ip);
                cmd.Parameters.AddWithValue("Posted", posted);
                cmd.Parameters.AddWithValue("ExternalMessageId", externalMessageId);
                cmd.Parameters.AddWithValue("ReferenceMessageId", referenceMessageId);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The pageload.
        /// </summary>
        /// <param name="sessionID">
        /// The session id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="boardUid">
        /// The board Uid.
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
        /// <param name="isCrawler">
        /// The is Crawler.
        /// </param>
        /// <param name="isMobileDevice">
        /// The browser is a mobile device.
        /// </param>
        /// <param name="donttrack">
        /// The donttrack.
        /// </param>
        /// <returns>
        /// Common User Info DataRow
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public static DataRow pageload([NotNull] object sessionID, [NotNull] object boardID,
                                       [NotNull] object userKey, [NotNull] object ip, [NotNull] object location, [NotNull] object forumPage, [NotNull] object browser, [NotNull] object platform, [NotNull] object categoryID, [NotNull] object forumID, [NotNull] object topicID, [NotNull] object messageID, [NotNull] object isCrawler, [NotNull] object isMobileDevice, [NotNull] object donttrack)
        {
            int nTries = 0;
            while (true)
            {
                try
                {
                    using (var cmd = MsSqlDbAccess.GetCommand("pageload"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("SessionID", sessionID);
                        cmd.Parameters.AddWithValue("BoardID", boardID);
                        cmd.Parameters.AddWithValue("UserKey", userKey ?? DBNull.Value);
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
                        cmd.Parameters.AddWithValue("IsMobileDevice", isMobileDevice);
                        cmd.Parameters.AddWithValue("DontTrack", donttrack);
                        cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                        using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                        {
                            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
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
                        throw new ApplicationException(
                          string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
                    }
                }

                ++nTries;
            }
        }

        /// <summary>
        /// Archives the private message of the given id.  Archiving moves the message from the user's inbox to his message archive.
        /// </summary>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        public static void pmessage_archive([NotNull] object userPMessageID)
        {
            using (SqlCommand sqlCommand = MsSqlDbAccess.GetCommand("pmessage_archive"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("UserPMessageID", userPMessageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(sqlCommand);
            }
        }

        /// <summary>
        /// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
        ///   the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
        /// </summary>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        /// <param name="fromOutbox">
        /// If true, removes the message from the outbox.  Otherwise deletes the message completely.
        /// </param>
        public static void pmessage_delete([NotNull] object userPMessageID, bool fromOutbox)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
                cmd.Parameters.AddWithValue("FromOutbox", fromOutbox);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes the private message from the database as per the given parameter.  If fromOutbox is true,
        ///   the message is only deleted from the user's outbox.  Otherwise, it is completely delete from the database.
        /// </summary>
        /// <param name="userPMessageID">
        /// </param>
        public static void pmessage_delete([NotNull] object userPMessageID)
        {
            pmessage_delete(userPMessageID, false);
        }

        /// <summary>
        /// The pmessage_info.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable pmessage_info()
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
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
        public static DataTable pmessage_list([NotNull] object toUserID, [NotNull] object fromUserID, [NotNull] object userPMessageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ToUserID", toUserID);
                cmd.Parameters.AddWithValue("FromUserID", fromUserID);
                cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Returns a list of private messages based on the arguments specified.
        ///   If pMessageID != null, returns the PM of id pMessageId.
        ///   If toUserID != null, returns the list of PMs sent to the user with the given ID.
        ///   If fromUserID != null, returns the list of PMs sent by the user of the given ID.
        /// </summary>
        /// <param name="userPMessageID">
        /// The user P Message ID.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable pmessage_list([NotNull] object userPMessageID)
        {
            return pmessage_list(null, null, userPMessageID);
        }

        /// <summary>
        /// The pmessage_markread.
        /// </summary>
        /// <param name="userPMessageID">
        /// The user p message id.
        /// </param>
        public static void pmessage_markread([NotNull] object userPMessageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_markread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserPMessageID", userPMessageID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void pmessage_prune([NotNull] object daysRead, [NotNull] object daysUnread)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("DaysRead", daysRead);
                cmd.Parameters.AddWithValue("DaysUnread", daysUnread);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The pmessage_save.
        /// </summary>
        /// <param name="fromUserID">The from user id.</param>
        /// <param name="toUserID">The to user id.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="replyTo">The reply to.</param>
        public static void pmessage_save([NotNull] object fromUserID, [NotNull] object toUserID, [NotNull] object subject, [NotNull] object body, [NotNull] object flags, [CanBeNull] object replyTo)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pmessage_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromUserID", fromUserID);
                cmd.Parameters.AddWithValue("ToUserID", toUserID);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Body", body);
                cmd.Parameters.AddWithValue("Flags", flags);
                cmd.Parameters.AddWithValue("ReplyTo", replyTo);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        ///   else only one poll is deleted completely. 
        /// </param>
        /// <param name="removeEverywhere">
        /// The remove Everywhere.
        /// </param>
        public static void poll_remove([NotNull] object pollGroupID, [NotNull] object pollID, [NotNull] object boardId, bool removeCompletely, bool removeEverywhere)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("poll_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollGroupID", pollGroupID);
                cmd.Parameters.AddWithValue("PollID", pollID);
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("RemoveCompletely", removeCompletely);
                cmd.Parameters.AddWithValue("RemoveEverywhere", removeEverywhere);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The method saves many questions and answers to them in a single transaction
        /// </summary>
        /// <param name="pollList">
        /// List to hold all polls data
        /// </param>
        /// <returns>
        /// Last saved poll id.
        /// </returns>
        public static int? poll_save([NotNull] List<PollSaveList> pollList)
        {
            foreach (PollSaveList question in pollList)
            {
                var sb = new StringBuilder();

                // Check if the group already exists
                if (question.TopicId > 0)
                {
                    sb.Append("select @PollGroupID = PollID  from ");
                    sb.Append(MsSqlDbAccess.GetObjectName("Topic"));
                    sb.Append(" WHERE TopicID = @TopicID; ");
                }
                else if (question.ForumId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(MsSqlDbAccess.GetObjectName("Forum"));
                    sb.Append(" WHERE ForumID = @ForumID");
                }
                else if (question.CategoryId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(MsSqlDbAccess.GetObjectName("Category"));
                    sb.Append(" WHERE CategoryID = @CategoryID");
                }

                // the group doesn't exists, create a new one
                sb.Append("IF @PollGroupID IS NULL BEGIN INSERT INTO ");
                sb.Append(MsSqlDbAccess.GetObjectName("PollGroupCluster"));
                sb.Append("(UserID,Flags ) VALUES(@UserID, @Flags) SET @NewPollGroupID = SCOPE_IDENTITY(); END; ");

                sb.Append("INSERT INTO ");
                sb.Append(MsSqlDbAccess.GetObjectName("Poll"));

                if (question.Closes > DateTime.MinValue)
                {
                    sb.Append("(Question,Closes, UserID,PollGroupID,ObjectPath,MimeType,Flags) ");
                }
                else
                {
                    sb.Append("(Question,UserID, PollGroupID, ObjectPath, MimeType,Flags) ");
                }

                sb.Append(" VALUES(");
                sb.Append("@Question");

                if (question.Closes > DateTime.MinValue)
                {
                    sb.Append(",@Closes");
                }

                sb.Append(
                  ",@UserID, (CASE WHEN  @NewPollGroupID IS NULL THEN @PollGroupID ELSE @NewPollGroupID END), @QuestionObjectPath,@QuestionMimeType,@PollFlags");
                sb.Append("); ");
                sb.Append("SET @PollID = SCOPE_IDENTITY(); ");

                // The cycle through question reply choices
                for (uint choiceCount = 0; choiceCount < question.Choice.GetUpperBound(1) + 1; choiceCount++)
                {
                    if (!string.IsNullOrEmpty(question.Choice[0, choiceCount]))
                    {
                        sb.Append("INSERT INTO ");
                        sb.Append(MsSqlDbAccess.GetObjectName("Choice"));
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
                    sb.Append(MsSqlDbAccess.GetObjectName("Topic"));
                    sb.Append(" SET PollID = @NewPollGroupID WHERE TopicID = @TopicID; ");
                }

                // fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
                if (question.ForumId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(MsSqlDbAccess.GetObjectName("Forum"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE ForumID= @ForumID; ");
                }

                // fill a pollgroup field in Category Table if the call comes from a category's topic list 
                if (question.CategoryId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(MsSqlDbAccess.GetObjectName("Category"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE CategoryID= @CategoryID; ");
                }

                // fill a pollgroup field in Board Table if the call comes from the main page poll 
                sb.Append("END;  ");

                using (var cmd = MsSqlDbAccess.GetCommand(sb.ToString(), true))
                {
                    var ret = new SqlParameter();
                    ret.ParameterName = "@PollID";
                    ret.SqlDbType = SqlDbType.Int;
                    ret.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(ret);

                    var ret2 = new SqlParameter();
                    ret2.ParameterName = "@PollGroupID";
                    ret2.SqlDbType = SqlDbType.Int;
                    ret2.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(ret2);

                    var ret3 = new SqlParameter();
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

                    cmd.Parameters.AddWithValue("@UserID", question.UserId);
                    cmd.Parameters.AddWithValue("@Flags", groupFlags);
                    cmd.Parameters.AddWithValue(
                      "@QuestionObjectPath",
                      string.IsNullOrEmpty(question.QuestionObjectPath) ? String.Empty : question.QuestionObjectPath);
                    cmd.Parameters.AddWithValue(
                      "@QuestionMimeType",
                      string.IsNullOrEmpty(question.QuestionMimeType) ? String.Empty : question.QuestionMimeType);

                    int pollFlags = question.IsClosedBound ? 0 | 4 : 0;
                    pollFlags = question.AllowMultipleChoices ? pollFlags | 8 : pollFlags;
                    pollFlags = question.ShowVoters ? pollFlags | 16 : pollFlags;
                    pollFlags = question.AllowSkipVote ? pollFlags | 32 : pollFlags;

                    cmd.Parameters.AddWithValue("@PollFlags", pollFlags);

                    for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetUpperBound(1) + 1; choiceCount1++)
                    {
                        if (!string.IsNullOrEmpty(question.Choice[0, choiceCount1]))
                        {
                            cmd.Parameters.AddWithValue(String.Format("@Choice{0}", choiceCount1), question.Choice[0, choiceCount1]);
                            cmd.Parameters.AddWithValue(String.Format("@Votes{0}", choiceCount1), 0);

                            cmd.Parameters.AddWithValue(
                              String.Format("@ChoiceObjectPath{0}", choiceCount1),
                              question.Choice[1, choiceCount1].IsNotSet() ? String.Empty : question.Choice[1, choiceCount1]);
                            cmd.Parameters.AddWithValue(
                              String.Format("@ChoiceMimeType{0}", choiceCount1),
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

                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd, true);
                    if (ret.Value != DBNull.Value)
                    {
                        return (int?)ret.Value;
                    }
                }
            }

            return null;
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
            using (var cmd = MsSqlDbAccess.GetCommand("poll_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollID", pollId);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
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
        /// <param name="isBounded">
        /// The is bounded.
        /// </param>
        /// <param name="isClosedBounded">
        /// The is closed bounded.
        /// </param>
        /// <param name="allowMultipleChoices">
        /// The allow Multiple Choices option.
        /// </param>
        /// <param name="showVoters">
        /// The show Voters.
        /// </param>
        /// <param name="allowSkipVote">
        /// The allow Skip Vote.
        /// </param>
        /// <param name="questionPath">
        /// The question file path.
        /// </param>
        /// <param name="questionMime">
        /// The question file mime type.
        /// </param>
        public static void poll_update(
            [NotNull] object pollID,
            [NotNull] object question,
            [NotNull] object closes,
            [NotNull] object isBounded,
            bool isClosedBounded,
            bool allowMultipleChoices,
            bool showVoters,
            bool allowSkipVote,
            [NotNull] object questionPath,
            [NotNull] object questionMime)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("poll_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollID", pollID);
                cmd.Parameters.AddWithValue("Question", question);
                cmd.Parameters.AddWithValue("Closes", closes);
                cmd.Parameters.AddWithValue("QuestionObjectPath", questionPath);
                cmd.Parameters.AddWithValue("QuestionMimeType", questionMime);
                cmd.Parameters.AddWithValue("IsBounded", isBounded);
                cmd.Parameters.AddWithValue("IsClosedBounded", isClosedBounded);
                cmd.Parameters.AddWithValue("AllowMultipleChoices", allowMultipleChoices);
                cmd.Parameters.AddWithValue("ShowVoters", showVoters);
                cmd.Parameters.AddWithValue("AllowSkipVote", allowSkipVote);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The pollgroup_attach.
        /// </summary>
        /// <param name="pollGroupId">
        /// The poll group id.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The pollgroup_attach.
        /// </returns>
        public static int pollgroup_attach(int? pollGroupId, int? topicId, int? forumId, int? categoryId, int? boardId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pollgroup_attach"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
                cmd.Parameters.AddWithValue("TopicID", topicId);
                cmd.Parameters.AddWithValue("ForumID", forumId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("BoardID", boardId);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        /// <param name="forumId">
        /// </param>
        /// <param name="categoryId">
        /// The category Id.
        /// </param>
        /// <param name="boardId">
        /// The BoardID id. 
        /// </param>
        /// <param name="removeCompletely">
        /// The RemoveCompletely. If true and pollID is null , all polls in a group are deleted completely, 
        ///   else only one poll is deleted completely. 
        /// </param>
        /// <param name="removeEverywhere">
        /// </param>
        public static void pollgroup_remove([NotNull] object pollGroupID, [NotNull] object topicId, [NotNull] object forumId, [NotNull] object categoryId, [NotNull] object boardId,
          bool removeCompletely,
          bool removeEverywhere)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pollgroup_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollGroupID", pollGroupID);
                cmd.Parameters.AddWithValue("TopicID", topicId);
                cmd.Parameters.AddWithValue("ForumID", forumId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("RemoveCompletely", removeCompletely);
                cmd.Parameters.AddWithValue("RemoveEverywhere", removeEverywhere);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
            using (var cmd = MsSqlDbAccess.GetCommand("pollgroup_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable pollgroup_votecheck([NotNull] object pollGroupId, [NotNull] object userId, [NotNull] object remoteIp)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pollgroup_votecheck"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollGroupID", pollGroupId);
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("RemoteIP", remoteIp);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        public static DataTable pollvote_check([NotNull] object pollid, [NotNull] object userid, [NotNull] object remoteip)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("pollvote_check"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("PollID", pollid);
                cmd.Parameters.AddWithValue("UserID", userid);
                cmd.Parameters.AddWithValue("RemoteIP", remoteip);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
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
        public static DataTable post_alluser([NotNull] object boardID, [NotNull] object userID, [NotNull] object pageUserID, [NotNull] object topCount)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("post_alluser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("PageUserID", pageUserID);
                cmd.Parameters.AddWithValue("topCount", topCount);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable post_last10user([NotNull] object boardID, [NotNull] object userID, [NotNull] object pageUserID)
        {
            // use all posts procedure to return top ten
            return post_alluser(boardID, userID, pageUserID, 10);
        }

        /// <summary>
        /// The post_list.
        /// </summary>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="currentUserID"> </param>
        /// <param name="authorUserID">
        /// The author User ID.
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
        /// <param name="sincePostedDate">
        /// The posted date.
        /// </param>
        /// <param name="toPostedDate">
        /// The to Posted Date.
        /// </param>
        /// <param name="sinceEditedDate">
        /// The edited date.
        /// </param>
        /// <param name="toEditedDate">
        /// The to Edited Date.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The Page size.
        /// </param>
        /// <param name="sortPosted">
        /// The sort by posted date.
        ///   0 - no sort, 1 - ASC, 2 - DESC
        /// </param>
        /// <param name="sortEdited">
        /// The sort by edited date.
        ///   0 - no sort, 1 - ASC, 2 - DESC.
        /// </param>
        /// <param name="sortPosition">
        /// The sort Position.
        /// </param>
        /// <param name="showThanks">
        /// The show thanks. Returnes thanked posts. Not implemented.
        /// </param>
        /// <param name="messagePosition">
        /// The message Position.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable post_list(
            [NotNull] object topicId, 
            object currentUserID,
            [NotNull] object authorUserID, 
            [NotNull] object updateViewCount,
                                          bool showDeleted,
                                          bool styledNicks,
                                          bool showReputation,
                                          DateTime sincePostedDate,
                                          DateTime toPostedDate,
                                          DateTime sinceEditedDate,
                                          DateTime toEditedDate,
                                          int pageIndex,
                                          int pageSize,
                                          int sortPosted,
                                          int sortEdited,
                                          int sortPosition,
                                          bool showThanks,
                                          int messagePosition)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("post_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicId);
                cmd.Parameters.AddWithValue("PageUserID", currentUserID);
                cmd.Parameters.AddWithValue("AuthorUserID", authorUserID);
                cmd.Parameters.AddWithValue("UpdateViewCount", updateViewCount);
                cmd.Parameters.AddWithValue("ShowDeleted", showDeleted);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                cmd.Parameters.AddWithValue("ShowReputation", showReputation);
                cmd.Parameters.AddWithValue("SincePostedDate", sincePostedDate);
                cmd.Parameters.AddWithValue("ToPostedDate", toPostedDate);
                cmd.Parameters.AddWithValue("SinceEditedDate", sinceEditedDate);
                cmd.Parameters.AddWithValue("ToEditedDate", toEditedDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("SortPosted", sortPosted);
                cmd.Parameters.AddWithValue("SortEdited", sortEdited);
                cmd.Parameters.AddWithValue("SortPosition", sortPosition);
                cmd.Parameters.AddWithValue("ShowThanks", showThanks);
                cmd.Parameters.AddWithValue("MessagePosition", messagePosition);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable post_list_reverse10([NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("post_list_reverse10"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The rank_delete.
        /// </summary>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        public static void rank_delete([NotNull] object rankID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("rank_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("RankID", rankID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable rank_list([NotNull] object boardID, [NotNull] object rankID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("rank_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("RankID", rankID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="description">
        /// The description.
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
        public static void rank_save([NotNull] object rankID, [NotNull] object boardID, [NotNull] object name, [NotNull] object isStart, [NotNull] object isLadder, [NotNull] object minPosts, [NotNull] object rankImage, [NotNull] object pmLimit, [NotNull] object style, [NotNull] object sortOrder, [NotNull] object description, [NotNull] object usrSigChars, [NotNull] object usrSigBBCodes, [NotNull] object usrSigHTMLTags, [NotNull] object usrAlbums, [NotNull] object usrAlbumImages)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("rank_save"))
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

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Get the list of recently logged in users.
        /// </summary>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        /// <param name="timeSinceLastLogin">
        /// The time since last login in minutes.
        /// </param>
        /// <param name="styledNicks">
        /// The styled Nicks.
        /// </param>
        /// <returns>
        /// The list of users in Datatable format.
        /// </returns>
        public static DataTable recent_users([NotNull] object boardID, int timeSinceLastLogin, [NotNull] object styledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("recent_users"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("TimeSinceLastLogin", timeSinceLastLogin);
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        public static DataTable registry_list([NotNull] object name, [NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("registry_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable registry_list([NotNull] object name)
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
        public static void registry_save([NotNull] object name, [NotNull] object value)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("registry_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Value", value);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void registry_save([NotNull] object name, [NotNull] object value, [NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("registry_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Value", value);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes a bad/good word
        /// </summary>
        /// <param name="ID">
        /// ID of bad/good word to delete
        /// </param>
        public static void replace_words_delete([NotNull] object ID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("replace_words_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ID", ID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable replace_words_list([NotNull] object boardId, [NotNull] object id)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("replace_words_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("ID", id);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void replace_words_save([NotNull] object boardId, [NotNull] object id, [NotNull] object badword, [NotNull] object goodword)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("replace_words_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ID", id);
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("badword", badword);
                cmd.Parameters.AddWithValue("goodword", goodword);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The rss_topic_latest.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page UserId id.
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <param name="showNoCountPosts">
        /// The show No Count Posts.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable rss_topic_latest([NotNull] object boardID, [NotNull] object numOfPostsToRetrieve, [NotNull] object pageUserId, bool useStyledNicks, bool showNoCountPosts)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("rss_topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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

            var sb = new StringBuilder();

            sb.AppendFormat(
              "select top {0} Topic = a.Topic,TopicID = a.TopicID, Name = b.Name, LastPosted = IsNull(a.LastPosted,a.Posted), LastUserID = IsNull(a.LastUserID, a.UserID), LastMessageID= IsNull(a.LastMessageID,(select top 1 m.MessageID ",
              topicLimit);
            sb.Append(
              "from {databaseOwner}.{objectQualifier}Message m where m.TopicID = a.TopicID order by m.Posted desc)), LastMessageFlags = IsNull(a.LastMessageFlags,22) ");

            // sb.Append(", message = (SELECT TOP 1 CAST([Message] as nvarchar(1000)) FROM [{databaseOwner}].[{objectQualifier}Message] mes2 where mes2.TopicID = IsNull(a.TopicMovedID,a.TopicID) AND mes2.IsApproved = 1 AND mes2.IsDeleted = 0 ORDER BY mes2.Posted DESC) ");
            sb.Append(
              "from {databaseOwner}.{objectQualifier}Topic a, {databaseOwner}.{objectQualifier}Forum b where a.ForumID = @ForumID and b.ForumID = a.ForumID and a.TopicMovedID is null and a.IsDeleted = 0");
            sb.Append(" order by a.Posted desc");

            using (var cmd = MsSqlDbAccess.GetCommand(sb.ToString(), true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("ForumID", forumId);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The shoutbox_clearmessages.
        /// </summary>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <returns>
        /// The shoutbox_clearmessages.
        /// </returns>
        public static bool shoutbox_clearmessages(int boardId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("shoutbox_clearmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardId", boardId);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return true;
            }
        }

        /// <summary>
        /// The shoutbox_getmessages.
        /// </summary>
        /// <param name="boardId">
        /// </param>
        /// <param name="numberOfMessages">
        /// The number of messages.
        /// </param>
        /// <param name="useStyledNicks">
        /// Use style for user nicks in ShoutBox.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable shoutbox_getmessages(int boardId, int numberOfMessages, [NotNull] object useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("shoutbox_getmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("NumberOfMessages", numberOfMessages);
                cmd.Parameters.AddWithValue("BoardId", boardId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The shoutbox_savemessage.
        /// </summary>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
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
        public static bool shoutbox_savemessage(int boardId, [NotNull] string message, [NotNull] string userName, int userID, [NotNull] object ip)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("shoutbox_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardId", boardId);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("IP", ip);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                return true;
            }
        }

        /// <summary>
        /// The smiley_delete.
        /// </summary>
        /// <param name="smileyID">
        /// The smiley id.
        /// </param>
        public static void smiley_delete([NotNull] object smileyID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("SmileyID", smileyID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable smiley_list([NotNull] object boardID, [NotNull] object smileyID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("SmileyID", smileyID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable smiley_listunique([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_listunique"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void smiley_resort([NotNull] object boardID, [NotNull] object smileyID, int move)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_resort"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("SmileyID", smileyID);
                cmd.Parameters.AddWithValue("Move", move);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void smiley_save([NotNull] object smileyID, [NotNull] object boardID, [NotNull] object code, [NotNull] object icon, [NotNull] object emoticon, [NotNull] object sortOrder, [NotNull] object replace)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("smiley_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("SmileyID", smileyID);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Code", code);
                cmd.Parameters.AddWithValue("Icon", icon);
                cmd.Parameters.AddWithValue("Emoticon", emoticon);
                cmd.Parameters.AddWithValue("SortOrder", sortOrder);
                cmd.Parameters.AddWithValue("Replace", replace);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The system_deleteinstallobjects.
        /// </summary>
        public static void system_deleteinstallobjects()
        {
            string tSQL = "DROP PROCEDURE" + MsSqlDbAccess.GetObjectName("system_initialize");
            using (var cmd = MsSqlDbAccess.GetCommand(tSQL, true))
            {
                cmd.CommandType = CommandType.Text;
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <param name="languageFile">
        /// The language File.
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
        /// <param name="rolePrefix">
        /// The role Prefix.
        /// </param>
        public static void system_initialize([NotNull] string forumName, [NotNull] string timeZone, [NotNull] string culture, [NotNull] string languageFile, [NotNull] string forumEmail, [NotNull] string smtpServer, [NotNull] string userName, [NotNull] string userEmail, [NotNull] object providerUserKey, [NotNull] string rolePrefix)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("system_initialize"))
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
                cmd.Parameters.AddWithValue("@RolePrefix", rolePrefix);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void system_initialize_executescripts([NotNull] string script, [NotNull] string scriptFile, bool useTransactions)
        {
            script = MsSqlDbAccess.GetCommandTextReplaced(script);

            List<string> statements = Regex.Split(script, "\\sGO\\s", RegexOptions.IgnoreCase).ToList();
            ushort sqlMajVersion = SqlServerMajorVersionAsShort();
            using (var connMan = new MsSqlDbConnectionManager())
            {
                // use transactions...
                if (useTransactions)
                {
                    using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
                    {
                        foreach (string sql0 in statements)
                        {
                            string sql = sql0.Trim();

                            sql = MsSqlDbAccess.CleanForSQLServerVersion(sql, sqlMajVersion);

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
                                        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
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
                                throw new Exception(
                                  String.Format("FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
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
                            throw new Exception(
                              String.Format("FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message));
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
            using (var connMan = new MsSqlDbConnectionManager())
            {
                using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
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
                            using (var cmd = connMan.DBConnection.CreateCommand())
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
        /// Not in use anymore. Only required for old database versions.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable system_list()
        {
            using (var cmd = MsSqlDbAccess.GetCommand("system_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void system_updateversion(int version, [NotNull] string versionname)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("system_updateversion"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Version", version);
                cmd.Parameters.AddWithValue("@VersionName", versionname);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Returns Topics Unread by a user
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param> 
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable topic_active([NotNull] object boardId, [CanBeNull] object categoryId, [NotNull] object pageUserId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_active"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The topic_unanswered
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param> 
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unanswered
        /// </returns>
        public static DataTable topic_unanswered([NotNull] object boardId, [CanBeNull] object categoryId, [NotNull] object pageUserId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_unanswered"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }
        /// <summary>
        /// Returns Topics Unread by a user
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param> 
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the Topics Unread be a PageUserId
        /// </returns>
        public static DataTable topic_unread([NotNull] object boardId, [CanBeNull] object categoryId, [NotNull] object pageUserId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_unread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }
        /// <summary>
        /// Gets all topics where the pageUserid has posted
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="pageUserId">
        /// The page user id.
        /// </param>
        /// <param name="sinceDate">
        /// The since Date.
        /// </param>
        /// <param name="toDate">
        /// The to Date.
        /// </param> 
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <param name="useStyledNicks">
        /// Set to true to get color nicks for last user and topic starter.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returns the List with the User Topics
        /// </returns>
        public static DataTable Topics_ByUser([NotNull] object boardId, [NotNull] object categoryId, [NotNull] object pageUserId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topics_byuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable topic_announcements([NotNull] object boardID, [NotNull] object numOfPostsToRetrieve, [NotNull] object pageUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_announcements"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
        public static long topic_create_by_message([NotNull] object messageID, [NotNull] object forumId, [NotNull] object newTopicSubj)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_create_by_message"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                cmd.Parameters.AddWithValue("ForumID", forumId);
                cmd.Parameters.AddWithValue("Subject", newTopicSubj);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                return long.Parse(dt.Rows[0]["TopicID"].ToString());
            }
        }

        // ABOT NEW 16.04.04:Delete all topic's messages

        // Ederon : 12/9/2007
        /// <summary>
        /// The topic_delete.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void topic_delete([NotNull] object topicID)
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
        public static void topic_delete([NotNull] object topicID, [NotNull] object eraseTopic)
        {
            // ABOT CHANGE 16.04.04
            topic_deleteAttachments(topicID);

            // END ABOT CHANGE 16.04.04
            using (var cmd = MsSqlDbAccess.GetCommand("topic_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("EraseTopic", eraseTopic);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void topic_favorite_add([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_favorite_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The topic_ favorite_ details.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="pageUserId">
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
        /// a Data Table containing the current page user's favorite topics with details.
        /// </returns>
        public static DataTable topic_favorite_details([NotNull] object boardId, [CanBeNull] object categoryId, [NotNull] object pageUserId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_favorite_details"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("CategoryID", categoryId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("SinceDate", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable topic_favorite_list([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_favorite_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void topic_favorite_remove([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_favorite_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The topic_findduplicate.
        /// </summary>
        /// <param name="topicName">
        /// The topic name.
        /// </param>
        /// <returns>
        /// The topic_findduplicate.
        /// </returns>
        public static int topic_findduplicate([NotNull] object topicName)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_findduplicate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicName", topicName);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        public static DataTable topic_findnext([NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_findnext"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataTable topic_findprev([NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_findprev"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static DataRow topic_info([NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
        }

        /// <summary>
        /// Get the Latest Topics
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="numOfPostsToRetrieve">
        /// The num of posts to retrieve.
        /// </param>
        /// <param name="pageUserId">
        /// The page UserId id. 
        /// </param>
        /// <param name="useStyledNicks">
        /// If true returns string for userID style.
        /// </param>
        /// <param name="showNoCountPosts">
        /// The show No Count Posts.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// Returnst the DataTable with the Latest Topics
        /// </returns>
        public static DataTable topic_latest([NotNull] object boardID, [NotNull] object numOfPostsToRetrieve, [NotNull] object pageUserId, bool useStyledNicks, bool showNoCountPosts, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("NumPosts", numOfPostsToRetrieve);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("ShowNoCountPosts", showNoCountPosts);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable topic_list([NotNull] object forumID, [NotNull] object userId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [NotNull] object showMoved, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("Date", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("ShowMoved", showMoved);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);
                return MsSqlDbAccess.Current.GetData(cmd, true);
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
        /// <param name="showMoved">
        /// The show Moved.
        /// </param>
        /// <param name="findLastRead">
        /// Indicates if the Table should Countain the last Access Date
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable announcements_list([NotNull] object forumID, [NotNull] object userId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [NotNull] object showMoved, [CanBeNull]bool findLastRead)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("announcements_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("Date", sinceDate);
                cmd.Parameters.AddWithValue("ToDate", toDate);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("ShowMoved", showMoved);
                cmd.Parameters.AddWithValue("FindLastRead", findLastRead);
                return MsSqlDbAccess.Current.GetData(cmd, true);
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
        public static void topic_lock([NotNull] object topicID, [NotNull] object locked)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_lock"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("Locked", locked);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void topic_move([NotNull] object topicID, [NotNull] object forumID, [NotNull] object showMoved, [NotNull] object linkDays)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("ShowMoved", showMoved);
                cmd.Parameters.AddWithValue("LinkDays", linkDays);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static int topic_prune([NotNull] object boardID, [NotNull] object forumID, [NotNull] object days, [NotNull] object permDelete)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("Days", days);
                cmd.Parameters.AddWithValue("PermDelete", permDelete);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="description">
        /// The description.
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
        /// Returns the Topic ID
        /// </returns>
        public static long topic_save(
            [NotNull] object forumID,
            [NotNull] object subject,
            [CanBeNull] object status,
            [CanBeNull] object styles,
            [CanBeNull] object description,
            [NotNull] object message,
            [NotNull] object userID,
            [NotNull] object priority,
            [NotNull] object userName,
            [NotNull] object ip,
            [NotNull] object posted,
            [NotNull] object blogPostID,
            [NotNull] object flags,
            ref long messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("Subject", subject);
                cmd.Parameters.AddWithValue("Description", description);
                cmd.Parameters.AddWithValue("Status", status);
                cmd.Parameters.AddWithValue("Styles", styles);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("Priority", priority);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("IP", ip);

                // cmd.Parameters.AddWithValue("PollID", pollID);
                cmd.Parameters.AddWithValue("Posted", posted);
                cmd.Parameters.AddWithValue("BlogPostID", blogPostID);
                cmd.Parameters.AddWithValue("Flags", flags);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                DataTable dt = MsSqlDbAccess.Current.GetData(cmd);
                messageID = long.Parse(dt.Rows[0]["MessageID"].ToString());
                return long.Parse(dt.Rows[0]["TopicID"].ToString());
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
            using (var cmd = MsSqlDbAccess.GetCommand("topic_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StartID", StartID);
                cmd.Parameters.AddWithValue("Limit", Limit);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The topic_updatetopic.
        /// </summary>
        /// <param name="topicId">
        /// The topic id.
        /// </param>
        /// <param name="topic">
        /// The topic.
        /// </param>
        public static void topic_updatetopic(int topicId, [NotNull] string topic)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_updatetopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicId);
                cmd.Parameters.AddWithValue("Topic", topic);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The unencode_all_topics_subjects.
        /// </summary>
        /// <param name="decodeTopicFunc">
        /// The decode topic func.
        /// </param>
        public static void unencode_all_topics_subjects([NotNull] Func<string, string> decodeTopicFunc)
        {
            var topics = topic_simplelist(0, 99999999).SelectTypedList(r => new TypedTopicSimpleList(r)).ToList();

            foreach (var topic in topics.Where(t => t.TopicID.HasValue && t.Topic.IsSet()))
            {
                try
                {
                    var decodedTopic = decodeTopicFunc(topic.Topic);

                    if (!decodedTopic.Equals(topic.Topic))
                    {
                        // unencode it and update.
                        topic_updatetopic(topic.TopicID.Value, decodedTopic);
                    }
                }
                catch
                {
                    // soft-fail...
                }
            }
        }

        /// <summary>
        /// Get the Thanks From Count for the user.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns the Thank Count.
        /// </returns>
        public static int user_ThankFromCount([NotNull] object userId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_thankfromcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var thankCount = (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);

                return thankCount;
            }
        }

        /// <summary>
        /// Checks if the User has replied tho the specifc topic.
        /// </summary>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns if true or not
        /// </returns>
        public static bool user_RepliedTopic([NotNull] object messageId, [NotNull] object userId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_repliedtopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("MessageID", messageId);
                cmd.Parameters.AddWithValue("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var messageCount = (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);

                return messageCount > 0;
            }
        }

        /// <summary>
        /// Is User Thanked the current Message
        /// </summary>
        /// <param name="messageId">
        /// The message Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// If the User Thanked the the Current Message
        /// </returns>
        public static bool user_ThankedMessage([NotNull] object messageId, [NotNull] object userId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_thankedmessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("MessageID", messageId);
                cmd.Parameters.AddWithValue("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var thankCount = (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);

                return thankCount > 0;
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
        public static DataTable user_accessmasks([NotNull] object boardID, [NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_accessmasks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);

                ///TODO: Recursion doesn't work here correctly at all because of UNION in underlying sql script. Possibly the only acceptable solution will be splitting the UNIONed queries and displaying 2 "trees". Maybe another solution exists.  
               return userforumaccess_sort_list(MsSqlDbAccess.Current.GetData(cmd), 0, 0, 0);
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
        public static DataTable user_activity_rank([NotNull] object boardID, [NotNull] object startDate, [NotNull] object displayNumber)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_activity_rank"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("StartDate", startDate);
                cmd.Parameters.AddWithValue("DisplayNumber", displayNumber);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The user_addignoreduser.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="ignoredUserId">
        /// The ignored user id.
        /// </param>
        public static void user_addignoreduser([NotNull] object userId, [NotNull] object ignoredUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_addignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserId", userId);
                cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Add Reputation Points to the specified user id.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="fromUserID">From user ID.</param>
        /// <param name="points">The points.</param>
        public static void user_addpoints([NotNull] object userID, [CanBeNull] object fromUserID, [NotNull] object points)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_addpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("FromUserID", fromUserID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("Points", points);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Remove Repuatation Points from the specified user id.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="fromUserID">From user ID.</param>
        /// <param name="points">The points.</param>
        public static void user_removepoints([NotNull] object userID, [CanBeNull] object fromUserID, [NotNull] object points)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_removepoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("FromUserID", fromUserID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.AddWithValue("Points", points);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        /// <param name="displayName">
        /// The display Name.
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
        public static void user_adminsave([NotNull] object boardID, [NotNull] object userID, [NotNull] object name, [NotNull] object displayName, [NotNull] object email, [NotNull] object flags, [NotNull] object rankID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_adminsave"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("DisplayName", displayName);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("Flags", flags);
                cmd.Parameters.AddWithValue("RankID", rankID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_approve.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static void user_approve([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_approveall.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        public static void user_approveall([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_approveall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
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
        /// <param name="displayName">
        /// The display Name.
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
        public static int user_aspnet(
          int boardID, [NotNull] string userName, [NotNull] string displayName, [NotNull] string email, [NotNull] object providerUserKey, [NotNull] object isApproved)
        {
            try
            {
                using (var cmd = MsSqlDbAccess.GetCommand("user_aspnet"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("BoardID", boardID);
                    cmd.Parameters.AddWithValue("UserName", userName);
                    cmd.Parameters.AddWithValue("DisplayName", displayName);
                    cmd.Parameters.AddWithValue("Email", email);
                    cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
                    cmd.Parameters.AddWithValue("IsApproved", isApproved);
                    cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                    return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
                }
            }
            catch (Exception x)
            {
                eventlog_create(null, "user_aspnet in YAF.Classes.Data.DB.cs", x, EventLogTypes.Error);
                return 0;
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
        public static DataTable user_avatarimage([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_avatarimage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static bool user_changepassword([NotNull] object userID, [NotNull] object oldPassword, [NotNull] object newPassword)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_changepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("OldPassword", oldPassword);
                cmd.Parameters.AddWithValue("NewPassword", newPassword);
                return (bool)MsSqlDbAccess.Current.ExecuteScalar(cmd);
            }
        }

        /// <summary>
        /// The user_delete.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static void user_delete([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_deleteavatar.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static void user_deleteavatar([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_deleteavatar"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_deleteold.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="days">
        /// The days.
        /// </param>
        public static void user_deleteold([NotNull] object boardID, [NotNull] object days)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_deleteold"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Days", days);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable user_emails([NotNull] object boardID, [NotNull] object groupID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_emails"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("GroupID", groupID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static int user_get(int boardID, [NotNull] object providerUserKey)
        {
            using (
              var cmd =
                MsSqlDbAccess.GetCommand(
                 MsSqlDbAccess.GetCommandTextReplaced("select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey"),
                  true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
                return (int)(MsSqlDbAccess.Current.ExecuteScalar(cmd) ?? 0);
            }
        }

        /// <summary>
        /// Returns data about albums: allowed number of images and albums
        /// </summary>
        /// <param name="userID">
        /// The userID
        /// </param>
        /// <param name="boardID">
        /// The boardID
        /// </param>
        public static DataTable user_getalbumsdata([NotNull] object userID, [NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getalbumsdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static int user_getpoints([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        [NotNull]
        public static string user_getsignature([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getsignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.ExecuteScalar(cmd).ToString();
            }
        }

        /// <summary>
        /// Returns data about allowed signature tags and character limits
        /// </summary>
        /// <param name="userID">
        /// The userID
        /// </param>
        /// <param name="boardID">
        /// The boardID
        /// </param>
        /// <returns>
        /// Data Table
        /// </returns>
        public static DataTable user_getsignaturedata([NotNull] object userID, [NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getsignaturedata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The user_getthanks_from.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// The user_getthanks_from.
        /// </returns>
        public static int user_getthanks_from([NotNull] object userID, [NotNull] object pageUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getthanks_from"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static int[] user_getthanks_to([NotNull] object userID, [NotNull] object pageUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_getthanks_to"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);

                int ThanksToPostsNumber, ThanksToNumber;
                if (paramThanksToNumber.Value == DBNull.Value)
                {
                    ThanksToNumber = 0;
                    ThanksToPostsNumber = 0;
                }
                else
                {
                    ThanksToPostsNumber = paramThanksToPostsNumber.Value.ToType<int>();
                    ThanksToNumber = paramThanksToNumber.Value.ToType<int>();
                }

                return new[] { ThanksToNumber, ThanksToPostsNumber };
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
        public static int? user_guest([NotNull] object boardID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_guest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                return MsSqlDbAccess.Current.ExecuteScalar(cmd).ToType<int?>();
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
        public static DataTable user_ignoredlist([NotNull] object userId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_ignoredlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserId", userId);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static bool user_isuserignored([NotNull] object userId, [NotNull] object ignoredUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_isuserignored"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserId", userId);
                cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
                cmd.Parameters.Add("result", SqlDbType.Bit);
                cmd.Parameters["result"].Direction = ParameterDirection.ReturnValue;

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);

                return Convert.ToBoolean(cmd.Parameters["result"].Value);
            }
        }

        /// <summary>
        /// To return a rather rarely updated active user data
        /// </summary>
        /// <param name="userID">
        /// The UserID. It is always should have a positive &gt; 0 value.
        /// </param>
        /// <param name="boardID">
        /// The board ID.
        /// </param>
        /// <param name="showPendingMails">
        /// The show Pending Mails.
        /// </param>
        /// <param name="showPendingBuddies">
        /// The show Pending Buddies.
        /// </param>
        /// <param name="showUnreadPMs">
        /// The show Unread P Ms.
        /// </param>
        /// <param name="showUserAlbums">
        /// The show User Albums.
        /// </param>
        /// <param name="styledNicks">
        /// If styles should be returned.
        /// </param>
        /// <exception cref="ApplicationException"></exception>
        /// <returns>
        /// A DataRow, it should never return a null value.
        /// </returns>
        public static DataRow user_lazydata(
            [NotNull] object userID,
            [NotNull] object boardID,
            bool showPendingMails,
            bool showPendingBuddies,
          bool showUnreadPMs,
          bool showUserAlbums,
          bool styledNicks)
        {
            int nTries = 0;
            while (true)
            {
                try
                {
                    using (var cmd = MsSqlDbAccess.GetCommand("user_lazydata"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("UserID", userID);
                        cmd.Parameters.AddWithValue("BoardID", boardID);
                        cmd.Parameters.AddWithValue("ShowPendingMails", showPendingMails);
                        cmd.Parameters.AddWithValue("ShowPendingBuddies", showPendingBuddies);
                        cmd.Parameters.AddWithValue("ShowUnreadPMs", showUnreadPMs);
                        cmd.Parameters.AddWithValue("ShowUserAlbums", showUserAlbums);
                        cmd.Parameters.AddWithValue("ShowUserStyle", styledNicks);
                        return MsSqlDbAccess.Current.GetData(cmd).Rows[0];
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
                        throw new ApplicationException(
                          string.Format("Sql Exception with error number {0} (Tries={1})", x.Number, nTries), x);
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
        public static DataTable user_list([NotNull] object boardID, [NotNull] object userID, [NotNull] object approved)
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
        public static DataTable user_list([NotNull] object boardID, [NotNull] object userID, [NotNull] object approved, [NotNull] object useStyledNicks)
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
        public static DataTable user_list([NotNull] object boardID, [NotNull] object userID, [NotNull] object approved, [NotNull] object groupID, [NotNull] object rankID)
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
        public static DataTable user_list([NotNull] object boardID, [NotNull] object userID, [NotNull] object approved, [NotNull] object groupID, [NotNull] object rankID, [CanBeNull] object useStyledNicks)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Approved", approved);
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("RankID", rankID);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("@UTCTIMESTAMP", DateTime.UtcNow);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The user_ list with todays birthdays.
        /// </summary>
        /// <param name="userIdsList">
        /// The Int array of userIds.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return or not style info.
        /// </param>
        /// <returns>
        /// The user_ list profiles.
        /// </returns>
        public static DataTable User_ListProfilesByIdsList([NotNull] int boardID, [NotNull] int[] userIdsList, [CanBeNull] object useStyledNicks)
        {
            string stIds = userIdsList.Aggregate(string.Empty, (current, userId) => current + (',' + userId)).Trim(',');
            // Profile columns cannot yet exist when we first are gettinng data.
            try
            {
                var sqlBuilder = new StringBuilder("SELECT up.*, u.Name as UserName,u.DisplayName as UserDisplayName, (case(@StyledNicks) when 1 then u.UserStyle ");
                sqlBuilder.Append("  else '' end) AS Style ");
                sqlBuilder.Append(" FROM ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("UserProfile"));
                sqlBuilder.Append(" up JOIN ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("User"));
                sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("Rank"));
                sqlBuilder.AppendFormat(" r ON r.RankID = u.RankID where UserID IN ({0})  ", stIds);
                using (var cmd = MsSqlDbAccess.GetCommand(sqlBuilder.ToString(), true))
                {
                    cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                    cmd.Parameters.AddWithValue("BoardID", boardID);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
            }
            catch (Exception e)
            {
                LegacyDb.eventlog_create(null, e.Source, e.Message, EventLogTypes.Error);
            }

            return null;
        }

        /// <summary>
        /// The user_ list with todays birthdays.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param>
        /// <returns>
        /// The user_ list with todays birthdays.
        /// </returns>
        public static DataTable User_ListTodaysBirthdays([NotNull] object boardID, [CanBeNull] object useStyledNicks)
        {
            // Profile columns cannot yet exist when we first are gettinng data.
            try
            {
                var sqlBuilder = new StringBuilder("SELECT up.Birthday, up.UserID, u.Name as UserName,u.DisplayName AS UserDisplayName, u.TimeZone, (case(@StyledNicks) when 1 then  u.UserStyle ");
                sqlBuilder.Append(" else '' end) AS Style ");
                sqlBuilder.Append(" FROM ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("UserProfile"));
                sqlBuilder.Append(" up JOIN ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("User"));
                sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
                sqlBuilder.Append(MsSqlDbAccess.GetObjectName("Rank"));
                sqlBuilder.Append(" r ON r.RankID = u.RankID where u.BoardID = @BoardID AND DAY(up.Birthday) = DAY(@CurrentDate) AND MONTH(up.Birthday) = MONTH(@CurrentDate) ");
                using (var cmd = MsSqlDbAccess.GetCommand(sqlBuilder.ToString(), true))
                {
                    cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                    cmd.Parameters.AddWithValue("BoardID", boardID);
                    cmd.Parameters.AddWithValue("CurrentDate", DateTime.UtcNow.Date);
                    return MsSqlDbAccess.Current.GetData(cmd);
                }
            }
            catch (Exception e)
            {
                LegacyDb.eventlog_create(null, e.Source, e.Message,EventLogTypes.Error);
            } 

            return null;
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
        public static DataTable user_listmedals([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_listmedals"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userID);

                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The user_list20members.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="groupId">
        /// The group id.
        /// </param>
        /// <param name="rankId">
        /// The rank id.
        /// </param>
        /// <param name="useStyledNicks">
        /// Return style info.
        /// </param>
        /// <param name="lastUserId">
        /// The last user Id.
        /// </param>
        /// <param name="literals">
        /// The literals.
        /// </param>
        /// <param name="exclude">
        /// The exclude.
        /// </param>
        /// <param name="beginsWith">
        /// The begins with.
        /// </param>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="sortName">
        /// The sort Name.
        /// </param>
        /// <param name="sortRank">
        /// The sort Rank.
        /// </param>
        /// <param name="sortJoined">
        /// The sort Joined.
        /// </param>
        /// <param name="sortPosts">
        /// The sort Posts.
        /// </param>
        /// <param name="sortLastVisit">
        /// The sort Last Visit.
        /// </param>
        /// <param name="numPosts">
        /// The num Posts.
        /// </param>
        /// <param name="numPostCompare">
        /// The num Post Compare.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable user_listmembers([NotNull] object boardId, [NotNull] object userId, [NotNull] object approved, [NotNull] object groupId, [NotNull] object rankId, [NotNull] object useStyledNicks, [NotNull] object lastUserId, [NotNull] object literals, [NotNull] object exclude, [NotNull] object beginsWith, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object sortName, [NotNull] object sortRank, [NotNull] object sortJoined, [NotNull] object sortPosts, [NotNull] object sortLastVisit, [NotNull] object numPosts, [NotNull] object numPostCompare)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_listmembers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("UserID", userId);
                cmd.Parameters.AddWithValue("Approved", approved);
                cmd.Parameters.AddWithValue("GroupID", groupId);
                cmd.Parameters.AddWithValue("RankID", rankId);
                cmd.Parameters.AddWithValue("StyledNicks", useStyledNicks);
                cmd.Parameters.AddWithValue("Literals", literals);
                cmd.Parameters.AddWithValue("Exclude", exclude);
                cmd.Parameters.AddWithValue("BeginsWith", beginsWith);
                cmd.Parameters.AddWithValue("PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("PageSize", pageSize);
                cmd.Parameters.AddWithValue("SortName", sortName);
                cmd.Parameters.AddWithValue("SortRank", sortRank);
                cmd.Parameters.AddWithValue("SortJoined", sortJoined);
                cmd.Parameters.AddWithValue("SortPosts", sortPosts);
                cmd.Parameters.AddWithValue("SortLastVisit", sortLastVisit);
                cmd.Parameters.AddWithValue("NumPosts", numPosts);
                cmd.Parameters.AddWithValue("NumPostsCompare", numPostCompare);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static object user_login([NotNull] object boardID, [NotNull] object name, [NotNull] object password)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_login"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("Name", name);
                cmd.Parameters.AddWithValue("Password", password);
                return MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        public static void user_medal_delete([NotNull] object userID, [NotNull] object medalID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_medal_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("MedalID", medalID);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable user_medal_list([NotNull] object userID, [NotNull] object medalID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_medal_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("MedalID", medalID);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void user_medal_save([NotNull] object userID, [NotNull] object medalID, [NotNull] object message, [NotNull] object hide, [NotNull] object onlyRibbon, [NotNull] object sortOrder, [NotNull] object dateAwarded)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_medal_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("MedalID", medalID);
                cmd.Parameters.AddWithValue("Message", message);
                cmd.Parameters.AddWithValue("Hide", hide);
                cmd.Parameters.AddWithValue("OnlyRibbon", onlyRibbon);
                cmd.Parameters.AddWithValue("SortOrder", sortOrder);
                cmd.Parameters.AddWithValue("DateAwarded", dateAwarded);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void user_migrate([NotNull] object userID, [NotNull] object providerUserKey, [NotNull] object updateProvider)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_migrate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("UpdateProvider", updateProvider);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        /// <param name="timeZone">
        /// The time Zone.
        /// </param>
        /// <returns>
        /// The user_nntp.
        /// </returns>
        public static int user_nntp([NotNull] object boardID, [NotNull] object userName, [NotNull] object email, int? timeZone)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_nntp"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Email", email);
                cmd.Parameters.AddWithValue("TimeZone", timeZone);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);

                return (int)MsSqlDbAccess.Current.ExecuteScalar(cmd);
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
        public static DataTable user_pmcount([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_pmcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static object user_recoverpassword([NotNull] object boardID, [NotNull] object userName, [NotNull] object email)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_recoverpassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("UserName", userName);
                cmd.Parameters.AddWithValue("Email", email);
                return MsSqlDbAccess.Current.ExecuteScalar(cmd);
            }
        }

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
        public static bool user_register([NotNull] object boardID, [NotNull] object userName, [NotNull] object password, [NotNull] object hash, [NotNull] object email, [NotNull] object location, [NotNull] object homePage, [NotNull] object timeZone,
                                         bool approved)
        {
            using (var connMan = new MsSqlDbConnectionManager())
            {
                using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
                {
                    try
                    {
                        using (var cmd = MsSqlDbAccess.GetCommand("user_save", connMan.DBConnection))
                        {
                            cmd.Transaction = trans;
                            cmd.CommandType = CommandType.StoredProcedure;
                            int UserID = 0;
                            cmd.Parameters.AddWithValue("UserID", UserID);
                            cmd.Parameters.AddWithValue("BoardID", boardID);
                            cmd.Parameters.AddWithValue("UserName", userName);
                            cmd.Parameters.AddWithValue(
                              "Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
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
        /// The user_removeignoreduser.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="ignoredUserId">
        /// The ignored user id.
        /// </param>
        public static void user_removeignoreduser([NotNull] object userId, [NotNull] object ignoredUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_removeignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserId", userId);
                cmd.Parameters.AddWithValue("IgnoredUserId", ignoredUserId);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_save.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="displayName">
        /// the display name.
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
        /// <param name="culture">
        /// the user culture
        /// </param>
        /// <param name="themeFile">
        /// The theme file.
        /// </param>
        /// <param name="useSingleSignOn">
        /// The use Single Sign On.
        /// </param>
        /// <param name="textEditor">
        /// The text Editor.
        /// </param>
        /// <param name="useMobileTheme">
        /// The override Mobile Theme.
        /// </param>
        /// <param name="approved">
        /// The approved.
        /// </param>
        /// <param name="pmNotification">
        /// The pm notification.
        /// </param>
        /// <param name="autoWatchTopics">
        /// The auto Watch Topics.
        /// </param>
        /// <param name="dSTUser">
        /// The d ST User.
        /// </param>
        /// <param name="hideUser">
        /// The hide User.
        /// </param>
        /// <param name="notificationType">
        /// The notification Type.
        /// </param>
        public static void user_save(
            [NotNull] object userID, 
            [NotNull] object boardID, 
            [NotNull] object userName, 
            [NotNull] object displayName, 
            [NotNull] object email, 
            [NotNull] object timeZone, 
            [NotNull] object languageFile, 
            [NotNull] object culture, 
            [NotNull] object themeFile, 
            [NotNull] object useSingleSignOn, 
            [NotNull] object textEditor, 
            [NotNull] object useMobileTheme, 
            [NotNull] object approved, 
            [NotNull] object pmNotification, 
            [NotNull] object autoWatchTopics, 
            [NotNull] object dSTUser, 
            [NotNull] object hideUser, 
            [NotNull] object notificationType)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_save"))
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
                cmd.Parameters.AddWithValue("UseSingleSignOn", useSingleSignOn);
                cmd.Parameters.AddWithValue("TextEditor", textEditor);
                cmd.Parameters.AddWithValue("OverrideDefaultTheme", useMobileTheme);
                cmd.Parameters.AddWithValue("Approved", approved);
                cmd.Parameters.AddWithValue("PMNotification", pmNotification);
                cmd.Parameters.AddWithValue("AutoWatchTopics", autoWatchTopics);
                cmd.Parameters.AddWithValue("DSTUser", dSTUser);
                cmd.Parameters.AddWithValue("HideUser", hideUser);
                cmd.Parameters.AddWithValue("NotificationType", notificationType);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void user_saveavatar([NotNull] object userID, [NotNull] object avatar, [NotNull] Stream stream, [NotNull] object avatarImageType)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_saveavatar"))
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
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Saves the notification type for a user
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="pmNotification">
        /// The pm Notification.
        /// </param>
        /// <param name="autoWatchTopics">
        /// The auto Watch Topics.
        /// </param>
        /// <param name="notificationType">
        /// The notification type.
        /// </param>
        /// <param name="dailyDigest">
        /// The daily Digest.
        /// </param>
        public static void user_savenotification([NotNull] object userID, [NotNull] object pmNotification, [NotNull] object autoWatchTopics, [NotNull] object notificationType, [NotNull] object dailyDigest)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_savenotification"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("PMNotification", pmNotification);
                cmd.Parameters.AddWithValue("AutoWatchTopics", autoWatchTopics);
                cmd.Parameters.AddWithValue("NotificationType", notificationType);
                cmd.Parameters.AddWithValue("DailyDigest", dailyDigest);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void user_savepassword([NotNull] object userID, [NotNull] object password)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_savepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue(
                  "Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Update the single Sign on Status
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="isFacebookUser">
        /// The is Facebook User
        /// </param>
        /// <param name="isTwitterUser">
        /// The is Twitter User.
        /// </param>
        public static void user_update_single_sign_on_status([NotNull] object userID, [NotNull] object isFacebookUser, [NotNull] object isTwitterUser)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_update_single_sign_on_status"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("IsFacebookUser", isFacebookUser);
                cmd.Parameters.AddWithValue("IsTwitterUser", isTwitterUser);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void user_savesignature([NotNull] object userID, [NotNull] object signature)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_savesignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Signature", signature);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_setinfo.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        public static void user_setinfo(int boardID, [NotNull] MembershipUser user)
        {
            using (
              var cmd =
                MsSqlDbAccess.GetCommand(
                  "update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey",
                  true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("UserName", user.UserName);
                cmd.Parameters.AddWithValue("Email", user.Email);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ProviderUserKey", user.ProviderUserKey);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Set the User Reputation Points to a specific value
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="points">
        /// The points.
        /// </param>
        public static void user_setpoints([NotNull] object userID, [NotNull] object points)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_setpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Points", points);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static void user_setrole(int boardID, [NotNull] object providerUserKey, [NotNull] object role)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_setrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("ProviderUserKey", providerUserKey);
                cmd.Parameters.AddWithValue("Role", role);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_setnotdirty.
        /// </summary>
        /// <param name="boardId">
        /// The board id.
        /// </param>
        /// <param name="userId">
        /// The userId key.
        /// </param>
        public static void user_setnotdirty(int boardId, int userId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_setnotdirty"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userId);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
            using (var cmd = MsSqlDbAccess.GetCommand("user_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("StartID", StartID);
                cmd.Parameters.AddWithValue("Limit", Limit);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void user_suspend([NotNull] object userID, [NotNull] object suspend)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_suspend"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("Suspend", suspend);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Returns the posts which is thanked by the user + the posts which are posted by the user and 
        ///   are thanked by other users.
        /// </summary>
        /// <param name="UserID">
        /// The user id.
        /// </param>
        /// <param name="pageUserID">
        /// The page User ID.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable user_viewallthanks([NotNull] object UserID, [NotNull] object pageUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("user_viewallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", UserID);
                cmd.Parameters.AddWithValue("PageUserID", pageUserID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void userforum_delete([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("userforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

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
        public static DataTable userforum_list([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("userforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void userforum_save([NotNull] object userID, [NotNull] object forumID, [NotNull] object accessMaskID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("userforum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("AccessMaskID", accessMaskID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The usergroup_list.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable usergroup_list([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("usergroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void usergroup_save([NotNull] object userID, [NotNull] object groupID, [NotNull] object member)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("usergroup_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("GroupID", groupID);
                cmd.Parameters.AddWithValue("Member", member);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The watchforum_add.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        public static void watchforum_add([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchforum_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable watchforum_check([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchforum_check"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The watchforum_delete.
        /// </summary>
        /// <param name="watchForumID">
        /// The watch forum id.
        /// </param>
        public static void watchforum_delete([NotNull] object watchForumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("WatchForumID", watchForumID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable watchforum_list([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
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
        public static void watchtopic_add([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchtopic_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
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
        public static DataTable watchtopic_check([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchtopic_check"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The watchtopic_delete.
        /// </summary>
        /// <param name="watchTopicID">
        /// The watch topic id.
        /// </param>
        public static void watchtopic_delete([NotNull] object watchTopicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchtopic_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("WatchTopicID", watchTopicID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// List the Watch Topics per User
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <returns>
        /// Returns the List of Topics
        /// </returns>
        public static DataTable watchtopic_list([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("watchtopic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// Add Or Update Read Tracking for the Current User and Topic
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        public static void Readtopic_AddOrUpdate([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readtopic_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Delete the Read Tracking
        /// </summary>
        /// <param name="userID">
        /// The user id
        /// </param>
        /*public static void Readtopic_delete([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readtopic_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }*/

        /// <summary>
        /// Get the Global Last Read DateTime User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="lastVisitDate">
        /// The last Visit Date of the User
        /// </param>
        /// <returns>
        /// Returns the Global Last Read DateTime
        /// </returns>
        public static DateTime? User_LastRead([NotNull] object userID)
        {
					using (var cmd = MsSqlDbAccess.GetCommand("user_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);

                var tableLastRead = MsSqlDbAccess.Current.ExecuteScalar(cmd);

                return tableLastRead.ToType<DateTime?>();
            }
        }

        /// <summary>
        /// Get the Last Read DateTime for the Current Topic and User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="topicID">
        /// The topic ID.
        /// </param>
        /// <returns>
        /// Returns the Last Read DateTime
        /// </returns>
        public static DateTime? Readtopic_lastread([NotNull] object userID, [NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readtopic_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("TopicID", topicID);

                var tableLastRead = MsSqlDbAccess.Current.ExecuteScalar(cmd);

            	return tableLastRead.ToType<DateTime?>();
            }
        }

        /// <summary>
        /// Add Or Update Read Tracking for the forum and Topic
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        public static void ReadForum_AddOrUpdate([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readforum_addorupdate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);
                cmd.Parameters.AddWithValue("UTCTIMESTAMP", DateTime.UtcNow);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Delete the Read Tracking
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /*public static void ReadForum_delete([NotNull] object userID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }*/

        /// <summary>
        /// Get the Last Read DateTime for the Forum and User
        /// </summary>
        /// <param name="userID">
        /// The user ID.
        /// </param>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        /// <returns>
        /// Returns the Last Read DateTime
        /// </returns>
        public static DateTime? ReadForum_lastread([NotNull] object userID, [NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("readforum_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("UserID", userID);
                cmd.Parameters.AddWithValue("ForumID", forumID);

                var tableLastRead = MsSqlDbAccess.Current.ExecuteScalar(cmd);

            	return tableLastRead.ToType<DateTime?>();
            }
        }

        #region ProfileMirror

        /// <summary>
        /// The set property values.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public static void SetPropertyValues(int boardId, string appname, int userId, SettingsPropertyValueCollection collection, bool dirtyOnly = true)
        {
            if (userId == 0 || collection.Count < 1)
            {
                return;
            }
            bool itemsToSave = true;
            if (dirtyOnly)
            {
                itemsToSave = collection.Cast<SettingsPropertyValue>().Any(pp => pp.IsDirty);
            }

            // First make sure we have at least one item to save

            if (!itemsToSave)
            {
                return;
            }
            
            // load the data for the configuration
            List<SettingsPropertyColumn> spc = LoadFromPropertyValueCollection(collection);
            
            if (spc != null && spc.Count > 0)
            {
                // start saving...
                LegacyDb.SetProfileProperties(boardId, appname, userId, collection, spc, dirtyOnly);
            }
        }
        /// <summary>
        /// The set profile properties.
        /// </summary>
        /// <param name="appName">
        /// The app name.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="settingsColumnsList">
        /// The settings columns list.
        /// </param>
        public static void SetProfileProperties([NotNull] int boardId, [NotNull] object appName, [NotNull] int userID, [NotNull] SettingsPropertyValueCollection values, [NotNull] List<SettingsPropertyColumn> settingsColumnsList, bool dirtyOnly)
        {
            string userName = string.Empty;
            var dtu =  LegacyDb.UserList(boardId, userID, true, null, null, true);
            foreach (var typedUserList in dtu)
            {
                userName = typedUserList.Name;
                break;

            }
            if (userName.IsNotSet())
            {
                return;
            }
            using ( var conn = new MsSqlDbConnectionManager().OpenDBConnection)
            {
                var cmd = new SqlCommand();

                cmd.Connection = conn;
                
                string table = MsSqlDbAccess.GetObjectName("UserProfile");
                StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                sqlCommand.Append(" WHERE UserId = @UserID AND ApplicationName = @ApplicationName) ");
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@ApplicationName", appName);

                // Build up strings used in the query
                var columnStr = new StringBuilder();
                var valueStr = new StringBuilder();
                var setStr = new StringBuilder();
                int count = 0;

                foreach (SettingsPropertyColumn column in settingsColumnsList)
                {
                    // only write if it's dirty
                    if (!dirtyOnly || values[column.Settings.Name].IsDirty)
                    {
                        columnStr.Append(", ");
                        valueStr.Append(", ");
                        columnStr.Append(column.Settings.Name);
                        string valueParam = "@Value" + count;
                        valueStr.Append(valueParam);
                        cmd.Parameters.AddWithValue(valueParam, values[column.Settings.Name].PropertyValue);

                        if ((column.DataType != SqlDbType.Timestamp) || column.Settings.Name != "LastUpdatedDate" || column.Settings.Name != "LastActivity")
                        {
                            if (count > 0)
                            {
                                setStr.Append(",");
                            }

                            setStr.Append(column.Settings.Name);
                            setStr.Append("=");
                            setStr.Append(valueParam);
                        }

                        count++;
                    }
                }

                columnStr.Append(",LastUpdatedDate ");
                valueStr.Append(",@LastUpdatedDate");
                setStr.Append(",LastUpdatedDate=@LastUpdatedDate");
                cmd.Parameters.AddWithValue("@LastUpdatedDate", DateTime.UtcNow);

                // MembershipUser mu = System.Web.Security.Membership.GetUser(userID);

                columnStr.Append(",LastActivity ");
                valueStr.Append(",@LastActivity");
                setStr.Append(",LastActivity=@LastActivity");
                cmd.Parameters.AddWithValue("@LastActivity", DateTime.UtcNow);

                columnStr.Append(",ApplicationName ");
                valueStr.Append(",@ApplicationName");
                setStr.Append(",ApplicationName=@ApplicationName");
                // cmd.Parameters.AddWithValue("@ApplicationID", appId);

                columnStr.Append(",IsAnonymous ");
                valueStr.Append(",@IsAnonymous");
                setStr.Append(",IsAnonymous=@IsAnonymous");
                cmd.Parameters.AddWithValue("@IsAnonymous", 0);

                columnStr.Append(",UserName ");
                valueStr.Append(",@UserName");
                setStr.Append(",UserName=@UserName");
                cmd.Parameters.AddWithValue("@UserName", userName);

                sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString());
                sqlCommand.Append(" WHERE UserId = ").Append(userID.ToString()).Append("");

                sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserId").Append(columnStr.ToString());
                sqlCommand.Append(") VALUES (").Append(userID.ToString()).Append("").Append(valueStr.ToString()).Append(
                  ") END");

                cmd.CommandText = sqlCommand.ToString();
                cmd.CommandType = CommandType.Text;

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// The get profile structure.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(MsSqlDbAccess.GetObjectName("UserProfile"));

            using (var cmd = MsSqlDbAccess.GetCommand(sql,true))
            {
                cmd.CommandType = CommandType.Text;
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

        /// <summary>
        /// The add profile column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="columnType">
        /// The column type.
        /// </param>
        /// <param name="size">
        /// The size.
        /// </param>
        public static void AddProfileColumn([NotNull] string name, SqlDbType columnType, int size)
        {
            // get column type...
            string type = columnType.ToString();

            if (size > 0)
            {
                type += "(" + size + ")";
            }

            string sql = "ALTER TABLE {0} ADD [{1}] {2} NULL".FormatWith(
              MsSqlDbAccess.GetObjectName("UserProfile"), name, type);

            using (var cmd = MsSqlDbAccess.GetCommand(sql, true))
            {
                cmd.CommandType = CommandType.Text;
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }
        /// <summary>
        /// The get db type and size from string.
        /// </summary>
        /// <param name="providerData">
        ///  The provider data.
        /// </param>
        /// <param name="dbType">
    /// The db type.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    /// <returns>
    /// The get db type and size from string.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// </exception>
        public static bool GetDbTypeAndSizeFromString(string providerData, out SqlDbType dbType, out int size)
        {
            size = -1;
            dbType = SqlDbType.NVarChar;

            if (providerData.IsNotSet())
            {
                return false;
            }

            // split the data
            string[] chunk = providerData.Split(new[] { ';' });

            // first item is the column name...
            string columnName = chunk[0];

            // get the datatype and ignore case...
            dbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), chunk[1], true);

            if (chunk.Length > 2)
            {
                // handle size...
                if (!Int32.TryParse(chunk[2], out size))
                {
                    throw new ArgumentException("Unable to parse as integer: " + chunk[2]);
                }
            }

            return true;
        }

        static List<SettingsPropertyColumn> LoadFromPropertyValueCollection(SettingsPropertyValueCollection collection)
        {
            List<SettingsPropertyColumn> settingsColumnsList = new List<SettingsPropertyColumn>();
                // clear it out just in case something is still in there...
              

                // validiate all the properties and populate the internal settings collection
                foreach (SettingsPropertyValue value in collection)
                {
                    SqlDbType dbType;
                    int size;

                    // parse custom provider data...
                   GetDbTypeAndSizeFromString(
                      value.Property.Attributes["CustomProviderData"].ToString(), out dbType, out size);

                    // default the size to 256 if no size is specified
                    if (dbType == SqlDbType.NVarChar && size == -1)
                    {
                        size = 256;
                    }

                    settingsColumnsList.Add(new SettingsPropertyColumn(value.Property, dbType, size));
                }

                // sync profile table structure with the db...
                DataTable structure = LegacyDb.GetProfileStructure();

                // verify all the columns are there...
                foreach (SettingsPropertyColumn column in settingsColumnsList)
                {
                    // see if this column exists
                    if (!structure.Columns.Contains(column.Settings.Name))
                    {
                        // if not, create it...
                        LegacyDb.AddProfileColumn(column.Settings.Name, column.DataType, column.Size);
                    }
                }
            return settingsColumnsList;
        }
  
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The get boolean registry value.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The get boolean registry value.
        /// </returns>
        private static bool GetBooleanRegistryValue([NotNull] string name)
        {
            using (DataTable dt = registry_list(name))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int i;
                    return int.TryParse(dr["Value"].ToString(), out i)
                             ? Convert.ToBoolean(i)
                             : Convert.ToBoolean(dr["Value"]);
                }
            }

            return false;
        }

        /// <summary>
        /// Called from db_runsql -- just runs a sql command according to specificiations.
        /// </summary>
        /// <param name="command">
        /// </param>
        /// <param name="useTransaction">
        /// </param>
        /// <returns>
        /// The inner run sql execute reader.
        /// </returns>
        [NotNull]
        private static string InnerRunSqlExecuteReader([NotNull] SqlCommand command, bool useTransaction)
        {
            SqlDataReader reader = null;
            var results = new StringBuilder();

            try
            {
                try
                {
                    command.Transaction = useTransaction
                                            ? command.Connection.BeginTransaction(MsSqlDbAccess.IsolationLevel)
                                            : null;
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
                            if (messageRunSql.IsSet())
                            {
                                results.AppendLine(messageRunSql);
                                results.AppendLine();
                            }
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
        /// Calls underlying stroed procedure for deletion of event log entry(ies).
        /// </summary>
        /// <param name="eventLogID">
        /// When not null, only given event log entry is deleted.
        /// </param>
        /// <param name="boardID">
        /// Specifies board. It is ignored if eventLogID parameter is not null.
        /// </param>
        private static void eventlog_delete([NotNull] object eventLogID, [NotNull] object boardID, [NotNull] object pageUserID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventlog_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("EventLogID", eventLogID);
                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("PageUserID", pageUserID);
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes events of a type.
        /// </summary>
        /// <param name="boardId">
        /// The board Id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        public static void eventlog_deletebyuser([NotNull] object boardId, [NotNull] object pageUserId)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("eventlog_deletebyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("BoardID", boardId);
                cmd.Parameters.AddWithValue("PageUserID", pageUserId);
               
                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Deletes attachments out of a entire forum
        /// </summary>
        /// <param name="forumID">
        /// The forum ID.
        /// </param>
        private static void forum_deleteAttachments([NotNull] object forumID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("forum_listtopics"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("ForumID", forumID);
                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    foreach (var row in dt.AsEnumerable().AsParallel())
                    {
                        if (row != null && row["TopicID"] != DBNull.Value)
                        {
                            topic_delete(row["TopicID"], true);
                        }
                    }
                }
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
        private static void forum_list_sort_basic([NotNull] DataTable listsource, [NotNull] DataTable list, int parentid, int currentLvl)
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
        private static DataTable forum_sort_list([NotNull] DataTable listSource, int parentID, int categoryID, int startingIndent, [NotNull] int[] forumidExclusions)
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
        [NotNull]
        private static DataTable forum_sort_list([NotNull] DataTable listSource,
                                                 int parentID,
                                                 int categoryID,
                                                 int startingIndent, [NotNull] int[] forumidExclusions,
                                                 bool emptyFirstRow)
        {
            var listDestination = new DataTable();

            listDestination.TableName = "forum_sort_list";

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
        private static void forum_sort_list_recursive([NotNull] DataTable listSource, [NotNull] DataTable listDestination, int parentID, int categoryID, int currentIndent)
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
        private static void medal_delete([NotNull] object boardID, [NotNull] object medalID, [NotNull] object category)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("medal_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("MedalID", medalID);
                cmd.Parameters.AddWithValue("Category", category);

                MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
            }
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
        /// <returns>
        /// Returns the Lists medals.
        /// </returns>
        private static DataTable medal_list([NotNull] object boardID, [NotNull] object medalID, [NotNull] object category)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("medal_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("BoardID", boardID);
                cmd.Parameters.AddWithValue("MedalID", medalID);
                cmd.Parameters.AddWithValue("Category", category);

                return MsSqlDbAccess.Current.GetData(cmd);
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
        private static void message_deleteRecursively([NotNull] object messageID,
                                                      bool isModeratorChanged, [NotNull] string deleteReason,
                                                      int isDeleteAction,
                                                      bool DeleteLinked,
                                                      bool isLinked)
        {
            message_deleteRecursively(
              messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, isLinked, false);
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
        private static void message_deleteRecursively([NotNull] object messageID,
                                                      bool isModeratorChanged, [NotNull] string deleteReason,
                                                      int isDeleteAction,
                                                      bool deleteLinked,
                                                      bool isLinked,
                                                      bool eraseMessages)
        {
            bool useFileTable = GetBooleanRegistryValue("UseFileTable");

            if (deleteLinked)
            {
                // Delete replies
                using (var cmd = MsSqlDbAccess.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MessageID", messageID);
                    DataTable tbReplies = MsSqlDbAccess.Current.GetData(cmd);

                    foreach (DataRow row in tbReplies.Rows)
                    {
                        message_deleteRecursively(
                          row["MessageID"], isModeratorChanged, deleteReason, isDeleteAction, deleteLinked, true, eraseMessages);
                    }
                }
            }

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                using (var cmd = MsSqlDbAccess.GetCommand("attachment_list"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MessageID", messageID);
                    DataTable tbAttachments = MsSqlDbAccess.Current.GetData(cmd);

                    string uploadDir =
                      HostingEnvironment.MapPath(String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

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
                using (var cmd = MsSqlDbAccess.GetCommand("message_delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MessageID", messageID);
                    cmd.Parameters.AddWithValue("EraseMessage", eraseMessages);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                }
            }
            else
            {
                // Delete Message
                // undelete function added
                using (var cmd = MsSqlDbAccess.GetCommand("message_deleteundelete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("MessageID", messageID);
                    cmd.Parameters.AddWithValue("isModeratorChanged", isModeratorChanged);
                    cmd.Parameters.AddWithValue("DeleteReason", deleteReason);
                    cmd.Parameters.AddWithValue("isDeleteAction", isDeleteAction);
                    MsSqlDbAccess.Current.ExecuteNonQuery(cmd);
                }
            }
        }

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
        private static void message_getRepliesList_populate([NotNull] DataTable listsource, [NotNull] DataTable list, int messageID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                DataTable dtr = MsSqlDbAccess.Current.GetData(cmd);

                for (int i = 0; i < dtr.Rows.Count; i++)
                {
                    DataRow row = dtr.Rows[i];
                    DataRow newRow = list.NewRow();
                    newRow["MessageID"] = row["MessageID"];
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

        /// <summary>
        /// moves answers of moved post
        /// </summary>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="moveToTopic">
        /// The move to topic.
        /// </param>
        private static void message_moveRecursively([NotNull] object messageID, [NotNull] object moveToTopic)
        {
            bool UseFileTable = GetBooleanRegistryValue("UseFileTable");

            // Delete replies
            using (var cmd = MsSqlDbAccess.GetCommand("message_getReplies"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("MessageID", messageID);
                DataTable tbReplies = MsSqlDbAccess.Current.GetData(cmd);
                foreach (DataRow row in tbReplies.Rows)
                {
                    message_moveRecursively(row["messageID"], moveToTopic);
                }

                using (SqlCommand innercmd = MsSqlDbAccess.GetCommand("message_move"))
                {
                    innercmd.CommandType = CommandType.StoredProcedure;
                    innercmd.Parameters.AddWithValue("MessageID", messageID);
                    innercmd.Parameters.AddWithValue("MoveToTopic", moveToTopic);
                    MsSqlDbAccess.Current.ExecuteNonQuery(innercmd);
                }
            }
        }

        /// <summary>
        /// The topic_delete attachments.
        /// </summary>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        private static void topic_deleteAttachments([NotNull] object topicID)
        {
            using (var cmd = MsSqlDbAccess.GetCommand("topic_listmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("TopicID", topicID);
                using (DataTable dt = MsSqlDbAccess.Current.GetData(cmd))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        message_deleteRecursively(row["MessageID"], true, string.Empty, 0, true, false);
                    }
                }
            }
        }

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
        [NotNull]
        private static DataTable userforumaccess_sort_list([NotNull] DataTable listSource, int parentID, int categoryID, int startingIndent)
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
        private static void userforumaccess_sort_list_recursive([NotNull] DataTable listSource, [NotNull] DataTable listDestination, int parentID, int categoryID, int currentIndent)
        {
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
                    DataRow newRow = listDestination.NewRow();

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
                    userforumaccess_sort_list_recursive(
                      listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1);
                }
            }
        }

        #endregion
    }
}