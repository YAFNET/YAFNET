/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
    using YAF.Types.Extensions;
    using YAF.Types.Handlers;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// All the Database functions for YAF
    /// </summary>
    public static class LegacyDb
    {
        #region Constants and Fields

        public static IDbAccess DbAccess
        {
            get
            {
                return ServiceLocatorAccess.CurrentServiceProvider.Get<IDbAccess>();
            }
        }

        #endregion

        #region Properties

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

        #endregion

        #region Public Methods

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
        /// Retuns All the Thanks for the Message IDs which are in the 
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIdsSeparatedWithColon">
        /// The message i ds.
        /// </param>
        [NotNull]
        public static IEnumerable<TypedAllThanks> MessageGetAllThanks([NotNull] string messageIdsSeparatedWithColon)
        {
            using (var cmd = DbAccess.GetCommand("message_getallthanks"))
            {
                cmd.AddParam("MessageIDs", messageIdsSeparatedWithColon);

                return DbAccess.GetData(cmd).AsEnumerable().Select(t => new TypedAllThanks(t));
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
            using (var cmd = DbAccess.GetCommand("message_list"))
            {
                cmd.AddParam("MessageID", messageID);

                return DbAccess.GetData(cmd).AsEnumerable().Select(t => new TypedMessageList(t));
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
            using (var cmd = DbAccess.GetCommand("pollgroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);
                cmd.AddParam("ForumID", forumId);
                cmd.AddParam("BoardID", boardId);

                return DbAccess.GetData(cmd).AsEnumerable().Select(r => new TypedPollGroup(r));
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
                DbHelpers.GetCommand(
                  "SELECT SUBSTRING(CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')), 1, PATINDEX('%.%', CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')))-1)",
                  true))
            {
                return Convert.ToUInt16(DbAccess.ExecuteScalar(cmd));
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
            using (var cmd = DbHelpers.GetCommand("user_find"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Filter", filter);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("Email", email);
                cmd.AddParam("DisplayName", displayName);
                cmd.AddParam("NotificationType", notificationType);
                cmd.AddParam("DailyDigest", dailyDigest);

                return DbAccess.GetData(cmd).AsEnumerable().Select(u => new TypedUserFind(u));
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
            using (var cmd = DbHelpers.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Approved", approved);
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("RankID", rankID);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd).AsEnumerable().Select(x => new TypedUserList(x));
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
            using (var cmd = DbHelpers.GetCommand("admin_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("admin_pageaccesslist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
            
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("adminpageaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userId);
                cmd.AddParam("PageName", pageName);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("adminpageaccess_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userId);
                cmd.AddParam("PageName", pageName);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("adminpageaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userId);
                cmd.AddParam("PageName", pageName);
                  
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("AlbumID", albumID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_getstats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramAlbumNumber = new SqlParameter("AlbumNumber", 0) { Direction = ParameterDirection.Output };
                var paramImageNumber = new SqlParameter("ImageNumber", 0) { Direction = ParameterDirection.Output };
                cmd.AddParam("UserID", userID);
                cmd.AddParam("AlbumID", albumID);

                cmd.Parameters.Add(paramAlbumNumber);
                cmd.Parameters.Add(paramImageNumber);
                DbAccess.ExecuteNonQuery(cmd);

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
            using (var cmd = DbHelpers.GetCommand("album_gettitle"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
                cmd.AddParam("AlbumID", albumID);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_image_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ImageID", imageID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_image_download"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ImageID", imageID);
                DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Album images by users the specified user ID.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>All Albbum Images of the User</returns>
        public static DataTable album_images_by_user([NotNull] object userID)
        {
            using (var cmd = DbHelpers.GetCommand("album_images_by_user"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_image_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("AlbumID", albumID);
                cmd.AddParam("ImageID", imageID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_image_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ImageID", imageID);
                cmd.AddParam("AlbumID", albumID);
                cmd.AddParam("Caption", caption);
                cmd.AddParam("FileName", fileName);
                cmd.AddParam("Bytes", bytes);
                cmd.AddParam("ContentType", contentType);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("AlbumID", albumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("album_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter { Direction = ParameterDirection.ReturnValue };
                cmd.AddParam("AlbumID", albumID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Title", title);
                cmd.AddParam("CoverImageID", coverImageID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
                return Convert.ToInt32(paramOutput.Value);
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
            using (var cmd = DbHelpers.GetCommand("buddy_addrequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255) { Direction = ParameterDirection.Output };
                var approved = new SqlParameter("approved", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                cmd.AddParam("FromUserID", FromUserID);
                cmd.AddParam("ToUserID", ToUserID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                cmd.Parameters.Add(approved);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("buddy_approverequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.AddParam("FromUserID", FromUserID);
                cmd.AddParam("ToUserID", ToUserID);
                cmd.AddParam("mutual", Mutual);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("buddy_denyrequest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.AddParam("FromUserID", FromUserID);
                cmd.AddParam("ToUserID", ToUserID);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("buddy_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("FromUserID", FromUserID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("buddy_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
                paramOutput.Direction = ParameterDirection.Output;
                cmd.AddParam("FromUserID", FromUserID);
                cmd.AddParam("ToUserID", ToUserID);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
                return paramOutput.Value.ToString();
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
            using (var cmd = DbHelpers.GetCommand("category_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StartID", startID);
                cmd.AddParam("Limit", limit);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("choice_add"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollID", pollID);
                cmd.AddParam("Choice", choice);
                cmd.AddParam("ObjectPath", path);
                cmd.AddParam("MimeType", mime);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("choice_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ChoiceID", choiceID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("choice_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ChoiceID", choiceID);
                cmd.AddParam("Choice", choice);
                cmd.AddParam("ObjectPath", path);
                cmd.AddParam("MimeType", mime);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("choice_vote"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ChoiceID", choiceID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("RemoteIP", remoteIP);
                DbAccess.ExecuteNonQuery(cmd);
            }
        }

        ///// <summary>
        ///// The db_getstats.
        ///// </summary>
        ///// <param name="connectionManager">
        ///// The conn man.
        ///// </param>
        //public static void db_getstats([NotNull] MsSqlDbConnectionManager connectionManager)
        //{
        //    // create statistic getting SQL...
        //    var sb = new StringBuilder();

        //    sb.AppendLine("DECLARE @TableName sysname");
        //    sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
        //    sb.AppendFormat(
        //      "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
        //      Config.DatabaseObjectQualifier);
        //    sb.AppendLine("OPEN cur_showfragmentation");
        //    sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
        //    sb.AppendLine("WHILE @@FETCH_STATUS = 0");
        //    sb.AppendLine("BEGIN");
        //    sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
        //    sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
        //    sb.AppendLine("END");
        //    sb.AppendLine("CLOSE cur_showfragmentation");
        //    sb.AppendLine("DEALLOCATE cur_showfragmentation");

        //    using (var cmd = new SqlCommand(sb.ToString(), connectionManager.OpenDBConnection))
        //    {
        //        cmd.Connection = connectionManager.DBConnection;

        //        // up the command timeout...
        //        cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

        //        // run it...
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        //private static string getStatsMessage;
        ///// <summary>
        ///// The db_getstats_new.
        ///// </summary>
        //public static string db_getstats_new()
        //{
        //    try
        //    {
        //        using (var connMan = new MsSqlDbConnectionManager())
        //        {
        //            connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(getStats_InfoMessage);

        //            connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;

        //            // create statistic getting SQL...
        //            var sb = new StringBuilder();

        //            sb.AppendLine("DECLARE @TableName sysname");
        //            sb.AppendLine("DECLARE cur_showfragmentation CURSOR FOR");
        //            sb.AppendFormat(
        //                "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'",
        //                Config.DatabaseObjectQualifier);
        //            sb.AppendLine("OPEN cur_showfragmentation");
        //            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
        //            sb.AppendLine("WHILE @@FETCH_STATUS = 0");
        //            sb.AppendLine("BEGIN");
        //            sb.AppendLine("DBCC SHOWCONTIG (@TableName)");
        //            sb.AppendLine("FETCH NEXT FROM cur_showfragmentation INTO @TableName");
        //            sb.AppendLine("END");
        //            sb.AppendLine("CLOSE cur_showfragmentation");
        //            sb.AppendLine("DEALLOCATE cur_showfragmentation");

        //            using (var cmd = new SqlCommand(sb.ToString(), connMan.OpenDBConnection))
        //            {
        //                cmd.Connection = connMan.DBConnection;

        //                // up the command timeout...
        //                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

        //                // run it...
        //                cmd.ExecuteNonQuery();
        //                return getStatsMessage;
        //            }

        //        }
        //    }
        //    finally
        //    {
        //        getStatsMessage = string.Empty;
        //    }
        //}

        ///// <summary>
        ///// The reindexDb_InfoMessage.
        ///// </summary>
        ///// <param name="sender">
        ///// The sender.
        ///// </param>
        ///// <param name="e">
        ///// The e.
        ///// </param>
        //private static void getStats_InfoMessage([NotNull] object sender, [NotNull] YafDBConnInfoMessageEventArgs e)
        //{
        //    getStatsMessage += "\r\n{0}".FormatWith(e.Message);
        //}

        ///// <summary>
        ///// The db_getstats_warning.
        ///// </summary>
        ///// <param name="connectionManager">
        ///// The conn man.
        ///// </param>
        ///// <returns>
        ///// The db_getstats_warning.
        ///// </returns>
        //[NotNull]
        //public static string db_getstats_warning()
        //{
        //    return string.Empty;
        //}

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
                    sql = DbHelpers.GetCommandTextReplaced(sql.Trim());

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
                    using (var trans = connMan.OpenDBConnection.BeginTransaction())
                    {
                        using (var da = new SqlDataAdapter(DbHelpers.GetObjectName("category_list"), connMan.DBConnection))
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.AddParam("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, DbHelpers.GetObjectName("Category"));
                            da.SelectCommand.CommandText = DbHelpers.GetObjectName("forum_list");
                            da.Fill(ds, DbHelpers.GetObjectName("ForumUnsorted"));

                            DataTable dtForumListSorted = ds.Tables[DbHelpers.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = DbHelpers.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            forum_list_sort_basic(
                              ds.Tables[DbHelpers.GetObjectName("ForumUnsorted")],
                              ds.Tables[DbHelpers.GetObjectName("Forum")],
                              0,
                              0);
                            ds.Tables.Remove(DbHelpers.GetObjectName("ForumUnsorted"));
                            ds.Relations.Add(
                              "FK_Forum_Category",
                              ds.Tables[DbHelpers.GetObjectName("Category")].Columns["CategoryID"],
                              ds.Tables[DbHelpers.GetObjectName("Forum")].Columns["CategoryID"]);
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
        private static void eventlog_create([NotNull] object userID, [NotNull] object source, [NotNull] object description, [NotNull] object type)
        {
            try
            {
                if (userID == null)
                {
                    userID = DBNull.Value;
                }

                using (var cmd = DbHelpers.GetCommand("eventlog_create"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("Type", type);
                    cmd.AddParam("UserID", userID);
                    cmd.AddParam("Source", source.ToString());
                    cmd.AddParam("Description", description.ToString());
                    cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                    DbAccess.ExecuteNonQuery(cmd);
                }
            }
            catch
            {
                // Ignore any errors in this method
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
            using (var cmd = DbHelpers.GetCommand("eventloggroupaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("EventTypeID", eventTypeId);
                cmd.AddParam("EventTypeName", eventTypeName);
                cmd.AddParam("DeleteAccess", deleteAccess);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("eventloggroupaccess_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("EventTypeID", eventTypeId);
                cmd.AddParam("EventTypeName", eventTypeName);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("eventloggroupaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("EventTypeID", eventTypeId);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_eventlogaccesslist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("@BoardID", boardId);
                return DbAccess.GetData(cmd);
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
                using (var cmd = DbHelpers.GetCommand("TopicStatus_Delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("TopicStatusID", topicStatusID);
                    DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("TopicStatus_Edit"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicStatusID", topicStatusID);
                return DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// List all Topics of the Current Board
        /// </summary>
        /// <param name="boardID">The board ID.</param>
        /// <returns></returns>
        public static DataTable TopicStatus_List([NotNull] object boardID)
        {
            using (var cmd = DbHelpers.GetCommand("TopicStatus_List"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                return DbAccess.GetData(cmd);
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
                using (var cmd = DbHelpers.GetCommand("TopicStatus_Save"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("TopicStatusID", topicStatusID);
                    cmd.AddParam("BoardId", boardID);
                    cmd.AddParam("TopicStatusName", topicStatusName);
                    cmd.AddParam("DefaultDescription", defaultDescription);
                    DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);

                if (!(DbAccess.ExecuteScalar(cmd) is DBNull))
                {
                    return false;
                }

                forum_deleteAttachments(forumID);
                
                using (SqlCommand cmd_new = DbHelpers.GetCommand("forum_delete"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd_new.AddParam("ForumID", forumID);
                    DbAccess.ExecuteNonQuery(cmd_new);
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
            using (var cmd = DbHelpers.GetCommand("forum_listSubForums"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumOldID);

                if (!(DbAccess.ExecuteScalar(cmd) is DBNull))
                {
                    return false;
                }

                using (SqlCommand cmd_new = DbHelpers.GetCommand("forum_move"))
                {
                    cmd_new.CommandType = CommandType.StoredProcedure;
                    cmd_new.CommandTimeout = 99999;
                    cmd_new.AddParam("ForumOldID", forumOldID);
                    cmd_new.AddParam("ForumNewID", forumNewID);
                    DbAccess.ExecuteNonQuery(cmd_new);
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
            using (var cmd = DbHelpers.GetCommand("forum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ForumID", forumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_maxid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                return Convert.ToInt32(DbAccess.ExecuteScalar(cmd));
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
            using (var cmd = DbHelpers.GetCommand("forum_listall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Root", startAt);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_listallmymoderated"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_listall_fromCat"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("CategoryID", categoryID);

                int intCategoryID = Convert.ToInt32(categoryID.ToString());

                using (DataTable dt = DbAccess.GetData(cmd))
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
            return !DbHelpers.LargeForumTree ? forum_listall_sorted(boardID, userID, null, false, 0) : forum_ns_getchildren_activeuser((int)boardID, 0, 0, (int)userID, false, false, "-");
        }

        static public DataTable forum_ns_getchildren_anyuser(int boardid, int categoryid, int forumid, int userid, bool notincluded, bool immediateonly, string indentchars)
        {
            using (var cmd = DbHelpers.GetCommand("forum_ns_getchildren_anyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("BoardID", boardid);
                cmd.AddParam("CategoryID", categoryid);
                cmd.AddParam("ForumID", forumid);
                cmd.AddParam("UserID", userid);
                cmd.AddParam("NotIncluded", notincluded);
                cmd.AddParam("ImmediateOnly", immediateonly);

                DataTable dt = DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_ns_getchildren_activeuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("BoardID", boardid);
                cmd.AddParam("CategoryID",categoryid);
                cmd.AddParam("ForumID",forumid);
                cmd.AddParam("UserID", userid);
                cmd.AddParam("NotIncluded", notincluded);
                cmd.AddParam("ImmediateOnly", immediateonly);

                DataTable dt = DbAccess.GetData(cmd);
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
            if (!DbHelpers.LargeForumTree)
            {

                using (var cmd = DbHelpers.GetCommand("forum_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("ForumID", forumID);
                    return DbAccess.GetData(cmd);
                }
            }
            else
            {
                using (var cmd = DbHelpers.GetCommand("forum_ns_listpath"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("ForumID", forumID);
                    return DbAccess.GetData(cmd);
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
            if (!DbHelpers.LargeForumTree)
            {
                using (var cmd = DbHelpers.GetCommand("forum_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("BoardID", boardID);
                    cmd.AddParam("UserID", userID);
                    cmd.AddParam("CategoryID", categoryID);
                    cmd.AddParam("ParentID", parentID);
                    cmd.AddParam("StyledNicks", useStyledNicks);
                    cmd.AddParam("FindLastRead", findLastRead);
                    return DbAccess.GetData(cmd);
                }
            }
            else
            {
                using (var cmd = DbHelpers.GetCommand("forum_ns_listread"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("BoardID", boardID);
                    cmd.AddParam("UserID", userID);
                    cmd.AddParam("CategoryID", categoryID);
                    cmd.AddParam("ParentID", parentID);
                    cmd.AddParam("StyledNicks", useStyledNicks);
                    cmd.AddParam("FindLastRead", findLastRead);
                    return DbAccess.GetData(cmd);
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
                    using (var da = new SqlDataAdapter(DbHelpers.GetObjectName("category_list"), connMan.OpenDBConnection))
                    {
                        using (SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction())
                        {
                            da.SelectCommand.Transaction = trans;
                            da.SelectCommand.AddParam("BoardID", boardID);
                            da.SelectCommand.CommandType = CommandType.StoredProcedure;
                            da.Fill(ds, DbHelpers.GetObjectName("Category"));
                            da.SelectCommand.CommandText = DbHelpers.GetObjectName("forum_moderatelist");
                            da.SelectCommand.AddParam("UserID", userID);
                            da.Fill(ds, DbHelpers.GetObjectName("ForumUnsorted"));
                            DataTable dtForumListSorted = ds.Tables[DbHelpers.GetObjectName("ForumUnsorted")].Clone();
                            dtForumListSorted.TableName = DbHelpers.GetObjectName("Forum");
                            ds.Tables.Add(dtForumListSorted);
                            dtForumListSorted.Dispose();
                            forum_list_sort_basic(
                              ds.Tables[DbHelpers.GetObjectName("ForumUnsorted")],
                              ds.Tables[DbHelpers.GetObjectName("Forum")],
                              0,
                              0);
                            ds.Tables.Remove(DbHelpers.GetObjectName("ForumUnsorted"));

                            // vzrus: Remove here all forums with no reports. Would be better to do it in query...
                            // Array to write categories numbers
                            var categories = new int[ds.Tables[DbHelpers.GetObjectName("Forum")].Rows.Count];
                            int cntr = 0;

                            // We should make it before too as the colection was changed
                            ds.Tables[DbHelpers.GetObjectName("Forum")].AcceptChanges();
                            foreach (DataRow dr in ds.Tables[DbHelpers.GetObjectName("Forum")].Rows)
                            {
                                categories[cntr] = dr["CategoryID"].ToType<int>();
                                if (dr["ReportedCount"].ToType<int>() == 0 && dr["MessageCount"].ToType<int>() == 0)
                                {
                                    dr.Delete();
                                    categories[cntr] = 0;
                                }

                                cntr++;
                            }

                            ds.Tables[DbHelpers.GetObjectName("Forum")].AcceptChanges();

                            foreach (
                                DataRow dr in from DataRow dr in ds.Tables[DbHelpers.GetObjectName("Category")].Rows
                                              let dr1 = dr
                                              where
                                                  !categories.Where(
                                                      category => category == dr1["CategoryID"].ToType<int>()).Any()
                                              select dr)
                            {
                                dr.Delete();
                            }

                            ds.Tables[DbHelpers.GetObjectName("Category")].AcceptChanges();

                            ds.Relations.Add(
                              "FK_Forum_Category",
                              ds.Tables[DbHelpers.GetObjectName("Category")].Columns["CategoryID"],
                              ds.Tables[DbHelpers.GetObjectName("Forum")].Columns["CategoryID"]);

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
            using (var cmd = DbHelpers.GetCommand("forum_moderators"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StyledNicks", useStyledNicks);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("moderators_team_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StyledNicks", useStyledNicks);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_resync"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ForumID", forumID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("CategoryID", categoryID);
                cmd.AddParam("ParentID", parentID);
                cmd.AddParam("Name", name);
                cmd.AddParam("Description", description);
                cmd.AddParam("SortOrder", sortOrder);
                cmd.AddParam("Locked", locked);
                cmd.AddParam("Hidden", hidden);
                cmd.AddParam("IsTest", isTest);
                cmd.AddParam("Moderated", moderated);
                cmd.AddParam("RemoteURL", remoteURL);
                cmd.AddParam("ThemeURL", themeURL);
                cmd.AddParam("ImageURL", imageURL);
                cmd.AddParam("Styles", styles);
                cmd.AddParam("AccessMaskID", accessMaskID);
                return long.Parse(DbAccess.ExecuteScalar(cmd).ToString());
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
                DbHelpers.GetCommand(
                  "SELECT " + DbHelpers.GetObjectName("forum_save_parentschecker") + "(@ForumID,@ParentID)", true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.AddParam("@ForumID", forumID);
                cmd.AddParam("@ParentID", parentID);
                return Convert.ToInt32(DbAccess.ExecuteScalar(cmd));
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
            using (var cmd = DbHelpers.GetCommand("forum_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StartID", StartID);
                cmd.AddParam("Limit", Limit);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forumaccess_group"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                return userforumaccess_sort_list(DbAccess.GetData(cmd), 0, 0, 0);
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
            using (var cmd = DbHelpers.GetCommand("forumaccess_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forumaccess_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("AccessMaskID", accessMaskID);
                DbAccess.ExecuteNonQuery(cmd);
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
                    errorStr = string.Format("Unable to connect to the Database. Exception Message: {0} ({1})", ex.Message, ex.Number);
                    return false;
                }

                // re-throw since we are debugging...
                throw;
            }
            catch (InvalidOperationException ex)
            {
                // unable to connect to the DB...
                if (!debugging)
                {
                    errorStr = string.Format("Unable to connect to the Database. Exception Message: {0}", ex.Message);
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
            var redirect = string.Empty;

            try
            {
                DataTable registry = registry_list("Version");

                if ((registry.Rows.Count == 0) || (registry.Rows[0]["Value"].ToType<int>() < appVersion))
                {
                    // needs upgrading...
                    redirect = "install/default.aspx?upgrade={0}".FormatWith(registry.Rows[0]["Value"].ToType<int>());
                }
            }
            catch (SqlException)
            {
                // needs to be setup...
                redirect = "install/default.aspx";
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
            using (var cmd = DbHelpers.GetCommand("group_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("GroupID", groupID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_medal_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("MedalID", medalID);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_medal_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("MedalID", medalID);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_medal_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("MedalID", medalID);
                cmd.AddParam("Message", message);
                cmd.AddParam("Hide", hide);
                cmd.AddParam("OnlyRibbon", onlyRibbon);
                cmd.AddParam("SortOrder", sortOrder);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_member"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_rank_style"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("@BoardID", boardID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("group_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Name", name);
                cmd.AddParam("IsAdmin", isAdmin);
                cmd.AddParam("IsGuest", isGuest);
                cmd.AddParam("IsStart", isStart);
                cmd.AddParam("IsModerator", isModerator);
                cmd.AddParam("AccessMaskID", accessMaskID);
                cmd.AddParam("PMLimit", pmLimit);
                cmd.AddParam("Style", style);
                cmd.AddParam("SortOrder", sortOrder);
                cmd.AddParam("Description", description);
                cmd.AddParam("UsrSigChars", usrSigChars);
                cmd.AddParam("UsrSigBBCodes", usrSigBBCodes);
                cmd.AddParam("UsrSigHTMLTags", usrSigHTMLTags);
                cmd.AddParam("UsrAlbums", usrAlbums);
                cmd.AddParam("UsrAlbumImages", usrAlbumImages);

                return long.Parse(DbAccess.ExecuteScalar(cmd).ToString());
            }
        }

        /// <summary>
        /// The message_ add thanks.
        /// </summary>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="useDisplayName">
        /// Use Display Name.
        /// </param>
        /// <returns>
        /// Returns the Name of the User
        /// </returns>
        [NotNull]
        public static string message_AddThanks([NotNull] object fromUserID, [NotNull] object messageID, [NotNull] object useDisplayName)
        {
            using (var cmd = DbHelpers.GetCommand("message_addthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255)
                    {
                        Direction = ParameterDirection.Output
                    };

                cmd.AddParam("FromUserID", fromUserID);
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.AddParam("UseDisplayName", useDisplayName);
                cmd.Parameters.Add(paramOutput);

                DbAccess.ExecuteNonQuery(cmd);

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
            using (var cmd = DbHelpers.GetCommand("message_gettextbyids"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageIDs", messageIDs);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_getthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", MessageID);
                return DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// The message_ remove thanks.
        /// </summary>
        /// <param name="FromUserID">The from user id.</param>
        /// <param name="MessageID">The message id.</param>
        /// <param name="useDisplayName">use the display name.</param>
        /// <returns>
        /// Returns the name of the user
        /// </returns>
        [NotNull]
        public static string message_RemoveThanks([NotNull] object FromUserID, [NotNull] object MessageID, [NotNull] object useDisplayName)
        {
            using (var cmd = DbHelpers.GetCommand("message_Removethanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.AddParam("FromUserID", FromUserID);
                cmd.AddParam("MessageID", MessageID);
                cmd.AddParam("UseDisplayName", useDisplayName);
                cmd.Parameters.Add(paramOutput);
                
                DbAccess.ExecuteNonQuery(cmd);
                
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
            using (var cmd = DbHelpers.GetCommand("message_thanksnumber"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramOutput = new SqlParameter();
                paramOutput.Direction = ParameterDirection.ReturnValue;
                cmd.AddParam("MessageID", messageID);
                cmd.Parameters.Add(paramOutput);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                DbAccess.ExecuteNonQuery(cmd);
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
        public static void message_delete(
            [NotNull] object messageID,
            bool isModeratorChanged,
            [NotNull] string deleteReason,
            int isDeleteAction,
            bool DeleteLinked,
            bool eraseMessage)
        {
            message_deleteRecursively(
                messageID,
                isModeratorChanged,
                deleteReason,
                isDeleteAction,
                DeleteLinked,
                false,
                eraseMessage);
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
            // Make sure there are no more DateTime.MinValues coming from db.
            if ((DateTime)lastRead == DateTime.MinValue)
            {
                lastRead = DateTimeHelper.SqlDbMinTime();
            }

            using (var cmd = DbHelpers.GetCommand("message_findunread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                cmd.AddParam("MessageID", messageId);
                cmd.AddParam("LastRead", lastRead);
                cmd.AddParam("ShowDeleted", showDeleted);
                cmd.AddParam("AuthorUserID", authorUserID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_reply_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_listreported"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("UserID", 0);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_listreporters"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("MoveToTopic", moveToTopic);
                DbAccess.ExecuteNonQuery(cmd);
            }

            // moveAll=true anyway
            // it's in charge of moving answers of moved post
            if (moveAll)
            {
                using (var cmd = DbHelpers.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("MessageID", messageID);
                    DataTable tbReplies = DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_report"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("ReporterID", userID);
                cmd.AddParam("ReportedDate", reportedDateTime);
                cmd.AddParam("ReportText", reportText);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_reportcopyover"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_reportresolve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageFlag", messageFlag);
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_save"))
            {
                var paramMessageID = new SqlParameter("MessageID", messageID) { Direction = ParameterDirection.Output };

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Message", message);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("IP", ip);
                cmd.AddParam("Posted", posted);
                cmd.AddParam("ReplyTo", replyTo);
                cmd.AddParam("BlogPostID", null); // Ederon : 6/16/2007
                cmd.AddParam("Flags", flags);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.Parameters.Add(paramMessageID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_secdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("MessageID", MessageID);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StartID", StartID);
                cmd.AddParam("Limit", Limit);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_unapproved"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("Priority", priority);
                cmd.AddParam("Message", message);
                cmd.AddParam("Description", description);
                cmd.AddParam("Status", status);
                cmd.AddParam("Styles", styles);
                cmd.AddParam("Subject", subject);
                cmd.AddParam("Flags", flags);
                cmd.AddParam("Reason", reasonOfEdit);
                cmd.AddParam("EditedBy", editedBy);
                cmd.AddParam("IsModeratorChanged", isModeratorChanged);
                cmd.AddParam("OverrideApproval", overrideApproval);
                cmd.AddParam("OriginalMessage", originalMessage);
                cmd.AddParam("CurrentUtcTimestamp", DateTime.UtcNow);
                
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("Priority", priority);
                cmd.AddParam("Message", message);
                cmd.AddParam("Description", description);
                cmd.AddParam("Subject", subject);
                cmd.AddParam("Flags", flags);
                cmd.AddParam("Reason", reasonOfEdit);
                cmd.AddParam("EditedBy", editedBy);
                cmd.AddParam("IsModeratorChanged", isModeratorChanged);
                cmd.AddParam("OverrideApproval", overrideApproval);
                cmd.AddParam("OriginalMessage", originalMessage);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("messagehistory_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("MessageID", messageId);
                cmd.AddParam("DaysToClean", daysToClean);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("NntpForumID", nntpForumID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Minutes", minutes);
                cmd.AddParam("NntpForumID", nntpForumID);
                cmd.AddParam("Active", active);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                return DbAccess.GetData(cmd);
            }
        }

        public static IEnumerable<TypedNntpForum> NntpForumList(int boardID, int? minutes, int? nntpForumID, bool? active)
        {
            using (var cmd = DbHelpers.GetCommand("nntpforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Minutes", minutes);
                cmd.AddParam("NntpForumID", nntpForumID);
                cmd.AddParam("Active", active);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd).AsEnumerable().Select(r => new TypedNntpForum(r));
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
            using (var cmd = DbHelpers.GetCommand("nntpforum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("NntpForumID", nntpForumID);
                cmd.AddParam("NntpServerID", nntpServerID);
                cmd.AddParam("GroupName", groupName);
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("Active", active);
                cmd.AddParam("DateCutOff", datecutoff);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpforum_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("NntpForumID", nntpForumID);
                cmd.AddParam("LastMessageNo", lastMessageNo);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpserver_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("NntpServerID", nntpServerID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpserver_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("NntpServerID", nntpServerID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntpserver_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("NntpServerID", nntpServerID);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Name", name);
                cmd.AddParam("Address", address);
                cmd.AddParam("Port", port);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("UserPass", userPass);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntptopic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("Thread", thread);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("nntptopic_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("NntpForumID", nntpForumID);
                cmd.AddParam("Topic", topic);
                cmd.AddParam("Body", body);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("IP", ip);
                cmd.AddParam("Posted", posted);
                cmd.AddParam("ExternalMessageId", externalMessageId);
                cmd.AddParam("ReferenceMessageId", referenceMessageId);
                cmd.AddParam("@UTCTIMESTAMP", DateTime.UtcNow);

                DbAccess.ExecuteNonQuery(cmd);
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
                    using (var cmd = DbHelpers.GetCommand("pageload"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParam("SessionID", sessionID);
                        cmd.AddParam("BoardID", boardID);
                        cmd.AddParam("UserKey", userKey ?? DBNull.Value);
                        cmd.AddParam("IP", ip);
                        cmd.AddParam("Location", location);
                        cmd.AddParam("ForumPage", forumPage);
                        cmd.AddParam("Browser", browser);
                        cmd.AddParam("Platform", platform);
                        cmd.AddParam("CategoryID", categoryID);
                        cmd.AddParam("ForumID", forumID);
                        cmd.AddParam("TopicID", topicID);
                        cmd.AddParam("MessageID", messageID);
                        cmd.AddParam("IsCrawler", isCrawler);
                        cmd.AddParam("IsMobileDevice", isMobileDevice);
                        cmd.AddParam("DontTrack", donttrack);
                        cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                        using (DataTable dt = DbAccess.GetData(cmd))
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
            using (SqlCommand sqlCommand = DbHelpers.GetCommand("pmessage_archive"))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.AddParam("UserPMessageID", userPMessageID);
                DbAccess.ExecuteNonQuery(sqlCommand);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserPMessageID", userPMessageID);
                cmd.AddParam("FromOutbox", fromOutbox);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ToUserID", toUserID);
                cmd.AddParam("FromUserID", fromUserID);
                cmd.AddParam("UserPMessageID", userPMessageID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_markread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserPMessageID", userPMessageID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("DaysRead", daysRead);
                cmd.AddParam("DaysUnread", daysUnread);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pmessage_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("FromUserID", fromUserID);
                cmd.AddParam("ToUserID", toUserID);
                cmd.AddParam("Subject", subject);
                cmd.AddParam("Body", body);
                cmd.AddParam("Flags", flags);
                cmd.AddParam("ReplyTo", replyTo);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("poll_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollGroupID", pollGroupID);
                cmd.AddParam("PollID", pollID);
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("RemoveCompletely", removeCompletely);
                cmd.AddParam("RemoveEverywhere", removeEverywhere);
                DbAccess.ExecuteNonQuery(cmd);
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
                    sb.Append(DbHelpers.GetObjectName("Topic"));
                    sb.Append(" WHERE TopicID = @TopicID; ");
                }
                else if (question.ForumId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(DbHelpers.GetObjectName("Forum"));
                    sb.Append(" WHERE ForumID = @ForumID");
                }
                else if (question.CategoryId > 0)
                {
                    sb.Append("select @PollGroupID = PollGroupID  from ");
                    sb.Append(DbHelpers.GetObjectName("Category"));
                    sb.Append(" WHERE CategoryID = @CategoryID");
                }

                // the group doesn't exists, create a new one
                sb.Append("IF @PollGroupID IS NULL BEGIN INSERT INTO ");
                sb.Append(DbHelpers.GetObjectName("PollGroupCluster"));
                sb.Append("(UserID,Flags ) VALUES(@UserID, @Flags) SET @NewPollGroupID = SCOPE_IDENTITY(); END; ");

                sb.Append("INSERT INTO ");
                sb.Append(DbHelpers.GetObjectName("Poll"));

                sb.Append(
                    question.Closes > DateTime.MinValue
                        ? "(Question,Closes, UserID,PollGroupID,ObjectPath,MimeType,Flags) "
                        : "(Question,UserID, PollGroupID, ObjectPath, MimeType,Flags) ");

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
                    if (string.IsNullOrEmpty(question.Choice[0, choiceCount]))
                    {
                        continue;
                    }

                    sb.Append("INSERT INTO ");
                    sb.Append(DbHelpers.GetObjectName("Choice"));
                    sb.Append("(PollID,Choice,Votes,ObjectPath,MimeType) VALUES (");
                    sb.AppendFormat("@PollID,@Choice{0},@Votes{0},@ChoiceObjectPath{0}, @ChoiceMimeType{0}", choiceCount);
                    sb.Append("); ");
                }

                // we don't update if no new group is created 
                sb.Append("IF  @PollGroupID IS NULL BEGIN  ");

                // fill a pollgroup field - double work if a poll exists 
                if (question.TopicId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(DbHelpers.GetObjectName("Topic"));
                    sb.Append(" SET PollID = @NewPollGroupID WHERE TopicID = @TopicID; ");
                }

                // fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
                if (question.ForumId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(DbHelpers.GetObjectName("Forum"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE ForumID= @ForumID; ");
                }

                // fill a pollgroup field in Category Table if the call comes from a category's topic list 
                if (question.CategoryId > 0)
                {
                    sb.Append("UPDATE ");
                    sb.Append(DbHelpers.GetObjectName("Category"));
                    sb.Append(" SET PollGroupID= @NewPollGroupID WHERE CategoryID= @CategoryID; ");
                }

                // fill a pollgroup field in Board Table if the call comes from the main page poll 
                sb.Append("END;  ");

                using (var cmd = DbHelpers.GetCommand(sb.ToString(), true))
                {
                    var ret = new SqlParameter
                                  {
                                      ParameterName = "@PollID",
                                      SqlDbType = SqlDbType.Int,
                                      Direction = ParameterDirection.Output
                                  };
                    cmd.Parameters.Add(ret);

                    var ret2 = new SqlParameter
                                   {
                                       ParameterName = "@PollGroupID",
                                       SqlDbType = SqlDbType.Int,
                                       Direction = ParameterDirection.Output
                                   };
                    cmd.Parameters.Add(ret2);

                    var ret3 = new SqlParameter
                                   {
                                       ParameterName = "@NewPollGroupID",
                                       SqlDbType = SqlDbType.Int,
                                       Direction = ParameterDirection.Output
                                   };
                    cmd.Parameters.Add(ret3);

                    cmd.AddParam("@Question", question.Question);

                    if (question.Closes > DateTime.MinValue)
                    {
                        cmd.AddParam("@Closes", question.Closes);
                    }

                    // set poll group flags
                    int groupFlags = 0;
                    if (question.IsBound)
                    {
                        groupFlags = groupFlags | 2;
                    }

                    cmd.AddParam("@UserID", question.UserId);
                    cmd.AddParam("@Flags", groupFlags);
                    cmd.AddParam(
                      "@QuestionObjectPath",
                      string.IsNullOrEmpty(question.QuestionObjectPath) ? String.Empty : question.QuestionObjectPath);
                    cmd.AddParam(
                      "@QuestionMimeType",
                      string.IsNullOrEmpty(question.QuestionMimeType) ? String.Empty : question.QuestionMimeType);

                    int pollFlags = question.IsClosedBound ? 0 | 4 : 0;
                    pollFlags = question.AllowMultipleChoices ? pollFlags | 8 : pollFlags;
                    pollFlags = question.ShowVoters ? pollFlags | 16 : pollFlags;
                    pollFlags = question.AllowSkipVote ? pollFlags | 32 : pollFlags;

                    cmd.AddParam("@PollFlags", pollFlags);

                    for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetUpperBound(1) + 1; choiceCount1++)
                    {
                        if (!string.IsNullOrEmpty(question.Choice[0, choiceCount1]))
                        {
                            cmd.AddParam(String.Format("@Choice{0}", choiceCount1), question.Choice[0, choiceCount1]);
                            cmd.AddParam(String.Format("@Votes{0}", choiceCount1), 0);

                            cmd.AddParam(
                              String.Format("@ChoiceObjectPath{0}", choiceCount1),
                              question.Choice[1, choiceCount1].IsNotSet() ? String.Empty : question.Choice[1, choiceCount1]);
                            cmd.AddParam(
                              String.Format("@ChoiceMimeType{0}", choiceCount1),
                              question.Choice[2, choiceCount1].IsNotSet() ? String.Empty : question.Choice[2, choiceCount1]);
                        }
                    }

                    if (question.TopicId > 0)
                    {
                        cmd.AddParam("@TopicID", question.TopicId);
                    }

                    if (question.ForumId > 0)
                    {
                        cmd.AddParam("@ForumID", question.ForumId);
                    }

                    if (question.CategoryId > 0)
                    {
                        cmd.AddParam("@CategoryID", question.CategoryId);
                    }

                    DbAccess.ExecuteNonQuery(cmd, true);
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
            using (var cmd = DbHelpers.GetCommand("poll_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollID", pollId);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("poll_update"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollID", pollID);
                cmd.AddParam("Question", question);
                cmd.AddParam("Closes", closes);
                cmd.AddParam("QuestionObjectPath", questionPath);
                cmd.AddParam("QuestionMimeType", questionMime);
                cmd.AddParam("IsBounded", isBounded);
                cmd.AddParam("IsClosedBounded", isClosedBounded);
                cmd.AddParam("AllowMultipleChoices", allowMultipleChoices);
                cmd.AddParam("ShowVoters", showVoters);
                cmd.AddParam("AllowSkipVote", allowSkipVote);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pollgroup_attach"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollGroupID", pollGroupId);
                cmd.AddParam("TopicID", topicId);
                cmd.AddParam("ForumID", forumId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("BoardID", boardId);
                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pollgroup_remove"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollGroupID", pollGroupID);
                cmd.AddParam("TopicID", topicId);
                cmd.AddParam("ForumID", forumId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("RemoveCompletely", removeCompletely);
                cmd.AddParam("RemoveEverywhere", removeEverywhere);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pollgroup_stats"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollGroupID", pollGroupId);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pollgroup_votecheck"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollGroupID", pollGroupId);
                cmd.AddParam("UserID", userId);
                cmd.AddParam("RemoteIP", remoteIp);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("pollvote_check"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("PollID", pollid);
                cmd.AddParam("UserID", userid);
                cmd.AddParam("RemoteIP", remoteip);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("post_alluser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("PageUserID", pageUserID);
                cmd.AddParam("topCount", topCount);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("post_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicId);
                cmd.AddParam("PageUserID", currentUserID);
                cmd.AddParam("AuthorUserID", authorUserID);
                cmd.AddParam("UpdateViewCount", updateViewCount);
                cmd.AddParam("ShowDeleted", showDeleted);
                cmd.AddParam("StyledNicks", styledNicks);
                cmd.AddParam("ShowReputation", showReputation);
                cmd.AddParam("SincePostedDate", sincePostedDate);
                cmd.AddParam("ToPostedDate", toPostedDate);
                cmd.AddParam("SinceEditedDate", sinceEditedDate);
                cmd.AddParam("ToEditedDate", toEditedDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("SortPosted", sortPosted);
                cmd.AddParam("SortEdited", sortEdited);
                cmd.AddParam("SortPosition", sortPosition);
                cmd.AddParam("ShowThanks", showThanks);
                cmd.AddParam("MessagePosition", messagePosition);
                cmd.AddParam("@UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("post_list_reverse10"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("rank_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("RankID", rankID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("rank_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("RankID", rankID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("rank_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("RankID", rankID);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Name", name);
                cmd.AddParam("IsStart", isStart);
                cmd.AddParam("IsLadder", isLadder);
                cmd.AddParam("MinPosts", minPosts);
                cmd.AddParam("RankImage", rankImage);
                cmd.AddParam("PMLimit", pmLimit);
                cmd.AddParam("Style", style);
                cmd.AddParam("SortOrder", sortOrder);
                cmd.AddParam("Description", description);
                cmd.AddParam("UsrSigChars", usrSigChars);
                cmd.AddParam("UsrSigBBCodes", usrSigBBCodes);
                cmd.AddParam("UsrSigHTMLTags", usrSigHTMLTags);
                cmd.AddParam("UsrAlbums", usrAlbums);
                cmd.AddParam("UsrAlbumImages", usrAlbumImages);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("recent_users"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("TimeSinceLastLogin", timeSinceLastLogin);
                cmd.AddParam("StyledNicks", styledNicks);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("registry_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("Name", name);
                cmd.AddParam("BoardID", boardID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("registry_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("Name", name);
                cmd.AddParam("Value", value);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("registry_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("Name", name);
                cmd.AddParam("Value", value);
                cmd.AddParam("BoardID", boardID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("replace_words_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ID", ID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("replace_words_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("ID", id);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("replace_words_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ID", id);
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("badword", badword);
                cmd.AddParam("goodword", goodword);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("rss_topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("NumPosts", numOfPostsToRetrieve);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("ShowNoCountPosts", showNoCountPosts);
                return DbAccess.GetData(cmd);
            }
        }

        /// <summary>
        /// Gets all Topics for an RSS Feed of specified forum id.
        /// </summary>
        /// <param name="forumId">The forum id.</param>
        /// <param name="topicLimit">The topic limit.</param>
        /// <returns>
        /// Returns a DataTable with the Topics of a Forum
        /// </returns>
        public static DataTable rsstopic_list(int forumId, int topicLimit)
        {
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

            using (var cmd = DbHelpers.GetCommand(sb.ToString(), true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.AddParam("ForumID", forumId);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("shoutbox_clearmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardId", boardId);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("shoutbox_getmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("NumberOfMessages", numberOfMessages);
                cmd.AddParam("BoardId", boardId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("shoutbox_savemessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardId", boardId);
                cmd.AddParam("Message", message);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("IP", ip);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
                return true;
            }
        }

        /// <summary>
        /// The system_deleteinstallobjects.
        /// </summary>
        public static void system_deleteinstallobjects()
        {
            string tSQL = "DROP PROCEDURE" + DbHelpers.GetObjectName("system_initialize");
            using (var cmd = DbHelpers.GetCommand(tSQL, true))
            {
                cmd.CommandType = CommandType.Text;
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("system_initialize"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("@Name", forumName);
                cmd.AddParam("@TimeZone", timeZone);
                cmd.AddParam("@Culture", culture);
                cmd.AddParam("@LanguageFile", languageFile);
                cmd.AddParam("@ForumEmail", forumEmail);
                cmd.AddParam("@SmtpServer", string.Empty);
                cmd.AddParam("@User", userName);
                cmd.AddParam("@UserEmail", userEmail);
                cmd.AddParam("@UserKey", providerUserKey);
                cmd.AddParam("@RolePrefix", rolePrefix);
                cmd.AddParam("@UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            script = DbHelpers.GetCommandTextReplaced(script);

            List<string> statements = Regex.Split(script, "\\sGO\\s", RegexOptions.IgnoreCase).ToList();
            ushort sqlMajVersion = SqlServerMajorVersionAsShort();
            using (var connMan = new MsSqlDbConnectionManager())
            {
                // use transactions...
                if (useTransactions)
                {
                    using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction())
                    {
                        foreach (string sql0 in statements)
                        {
                            string sql = sql0.Trim();

                            sql = DbHelpers.CleanForSQLServerVersion(sql, sqlMajVersion);

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
                using (SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction())
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
                                        cmd.CommandText = "grant execute on \"{0}\" to \"{1}\"".FormatWith(row["Name"], userName);
                                        cmd.ExecuteNonQuery();
                                    }

                                    foreach (DataRow row in dt.Select("IsUserTable=1 or IsView=1"))
                                    {
                                        cmd.CommandText = "grant select,update on \"{0}\" to \"{1}\"".FormatWith(row["Name"], userName);
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
                                        cmd.AddParam("@objname", row["Name"]);
                                        cmd.AddParam("@newowner", "dbo");
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
                                        cmd.AddParam("@objname", row["Name"]);
                                        cmd.AddParam("@newowner", "dbo");
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
            using (var cmd = DbHelpers.GetCommand("system_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("system_updateversion"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("@Version", version);
                cmd.AddParam("@VersionName", versionname);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_active"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("SinceDate", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_unanswered"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("SinceDate", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_unread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("SinceDate", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topics_byuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("CategoryID", categoryId);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("SinceDate", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_announcements"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("NumPosts", numOfPostsToRetrieve);
                cmd.AddParam("PageUserID", pageUserId);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_create_by_message"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                cmd.AddParam("ForumID", forumId);
                cmd.AddParam("Subject", newTopicSubj);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DataTable dt = DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                cmd.AddParam("EraseTopic", eraseTopic);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_findduplicate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicName", topicName);
                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_findnext"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_findprev"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_info"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                using (DataTable dt = DbAccess.GetData(cmd))
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
            using (var cmd = DbHelpers.GetCommand("topic_latest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("NumPosts", numOfPostsToRetrieve);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("ShowNoCountPosts", showNoCountPosts);
                cmd.AddParam("FindLastRead", findLastRead);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("UserID", userId);
                cmd.AddParam("Date", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("ShowMoved", showMoved);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("announcements_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("UserID", userId);
                cmd.AddParam("Date", sinceDate);
                cmd.AddParam("ToDate", toDate);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("ShowMoved", showMoved);
                cmd.AddParam("FindLastRead", findLastRead);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_lock"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                cmd.AddParam("Locked", locked);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_move"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("ShowMoved", showMoved);
                cmd.AddParam("LinkDays", linkDays);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_prune"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("Days", days);
                cmd.AddParam("PermDelete", permDelete);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("Subject", subject);
                cmd.AddParam("Description", description);
                cmd.AddParam("Status", status);
                cmd.AddParam("Styles", styles);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Message", message);
                cmd.AddParam("Priority", priority);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("IP", ip);

                // cmd.AddParam("PollID", pollID);
                cmd.AddParam("Posted", posted);
                cmd.AddParam("BlogPostID", blogPostID);
                cmd.AddParam("Flags", flags);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                DataTable dt = DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StartID", StartID);
                cmd.AddParam("Limit", Limit);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_updatetopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicId);
                cmd.AddParam("Topic", topic);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_thankfromcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var thankCount = (int)DbAccess.ExecuteScalar(cmd);

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
            using (var cmd = DbHelpers.GetCommand("user_repliedtopic"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("MessageID", messageId);
                cmd.AddParam("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var messageCount = (int)DbAccess.ExecuteScalar(cmd);

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
            using (var cmd = DbHelpers.GetCommand("user_thankedmessage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("MessageID", messageId);
                cmd.AddParam("UserID", userId);

                cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

                var thankCount = (int)DbAccess.ExecuteScalar(cmd);

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
            using (var cmd = DbHelpers.GetCommand("user_accessmasks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);

                ///TODO: Recursion doesn't work here correctly at all because of UNION in underlying sql script. Possibly the only acceptable solution will be splitting the UNIONed queries and displaying 2 "trees". Maybe another solution exists.  
               return userforumaccess_sort_list(DbAccess.GetData(cmd), 0, 0, 0);
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
            using (var cmd = DbHelpers.GetCommand("user_activity_rank"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("StartDate", startDate);
                cmd.AddParam("DisplayNumber", displayNumber);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_addignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserId", userId);
                cmd.AddParam("IgnoredUserId", ignoredUserId);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_addpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("FromUserID", fromUserID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.AddParam("Points", points);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_removepoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("FromUserID", fromUserID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                cmd.AddParam("Points", points);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_adminsave"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Name", name);
                cmd.AddParam("DisplayName", displayName);
                cmd.AddParam("Email", email);
                cmd.AddParam("Flags", flags);
                cmd.AddParam("RankID", rankID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_approve"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_approveall"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                DbAccess.ExecuteNonQuery(cmd);
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
                using (var cmd = DbHelpers.GetCommand("user_aspnet"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.AddParam("BoardID", boardID);
                    cmd.AddParam("UserName", userName);
                    cmd.AddParam("DisplayName", displayName);
                    cmd.AddParam("Email", email);
                    cmd.AddParam("ProviderUserKey", providerUserKey);
                    cmd.AddParam("IsApproved", isApproved);
                    cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                    return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_avatarimage"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_changepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("OldPassword", oldPassword);
                cmd.AddParam("NewPassword", newPassword);
                return (bool)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_deleteavatar"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_deleteold"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Days", days);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_emails"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("GroupID", groupID);

                return DbAccess.GetData(cmd);
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
                DbHelpers.GetCommand(
                 DbHelpers.GetCommandTextReplaced("select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey"),
                  true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ProviderUserKey", providerUserKey);
                return (int)(DbAccess.ExecuteScalar(cmd) ?? 0);
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
            using (var cmd = DbHelpers.GetCommand("user_getalbumsdata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_getpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_getsignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return DbAccess.ExecuteScalar(cmd).ToString();
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
            using (var cmd = DbHelpers.GetCommand("user_getsignaturedata"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_getthanks_from"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("PageUserID", pageUserId);
                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_getthanks_to"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var paramThanksToNumber = new SqlParameter("ThanksToNumber", 0);
                paramThanksToNumber.Direction = ParameterDirection.Output;
                var paramThanksToPostsNumber = new SqlParameter("ThanksToPostsNumber", 0);
                paramThanksToPostsNumber.Direction = ParameterDirection.Output;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("PageUserID", pageUserId);
                cmd.Parameters.Add(paramThanksToNumber);
                cmd.Parameters.Add(paramThanksToPostsNumber);
                DbAccess.ExecuteNonQuery(cmd);

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
            using (var cmd = DbHelpers.GetCommand("user_guest"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                return DbAccess.ExecuteScalar(cmd).ToType<int?>();
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
            using (var cmd = DbHelpers.GetCommand("user_ignoredlist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserId", userId);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_isuserignored"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserId", userId);
                cmd.AddParam("IgnoredUserId", ignoredUserId);
                cmd.Parameters.Add("result", SqlDbType.Bit);
                cmd.Parameters["result"].Direction = ParameterDirection.ReturnValue;

                DbAccess.ExecuteNonQuery(cmd);

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
                    using (var cmd = DbHelpers.GetCommand("user_lazydata"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.AddParam("UserID", userID);
                        cmd.AddParam("BoardID", boardID);
                        cmd.AddParam("ShowPendingMails", showPendingMails);
                        cmd.AddParam("ShowPendingBuddies", showPendingBuddies);
                        cmd.AddParam("ShowUnreadPMs", showUnreadPMs);
                        cmd.AddParam("ShowUserAlbums", showUserAlbums);
                        cmd.AddParam("ShowUserStyle", styledNicks);
                        return DbAccess.GetData(cmd).Rows[0];
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
            using (var cmd = DbHelpers.GetCommand("user_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Approved", approved);
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("RankID", rankID);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("@UTCTIMESTAMP", DateTime.UtcNow);

                return DbAccess.GetData(cmd);
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
                sqlBuilder.Append(DbHelpers.GetObjectName("UserProfile"));
                sqlBuilder.Append(" up JOIN ");
                sqlBuilder.Append(DbHelpers.GetObjectName("User"));
                sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
                sqlBuilder.Append(DbHelpers.GetObjectName("Rank"));
                sqlBuilder.AppendFormat(" r ON r.RankID = u.RankID where UserID IN ({0})  ", stIds);
                using (var cmd = DbHelpers.GetCommand(sqlBuilder.ToString(), true))
                {
                    cmd.AddParam("StyledNicks", useStyledNicks);
                    cmd.AddParam("BoardID", boardID);
                    return DbAccess.GetData(cmd);
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
                sqlBuilder.Append(DbHelpers.GetObjectName("UserProfile"));
                sqlBuilder.Append(" up JOIN ");
                sqlBuilder.Append(DbHelpers.GetObjectName("User"));
                sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
                sqlBuilder.Append(DbHelpers.GetObjectName("Rank"));
                sqlBuilder.Append(" r ON r.RankID = u.RankID where u.BoardID = @BoardID AND DATEADD(year, DATEDIFF(year,up.Birthday,@CurrentUtc1),up.Birthday) > @CurrentUtc1 and  DATEADD(year, DATEDIFF(year,up.Birthday,@CurrentUtc2),up.Birthday) < @CurrentUtc2");
                using (var cmd = DbHelpers.GetCommand(sqlBuilder.ToString(), true))
                {
                    cmd.AddParam("StyledNicks", useStyledNicks);
                    cmd.AddParam("BoardID", boardID);
                    cmd.AddParam("CurrentYear", DateTime.UtcNow.Year);
                    cmd.AddParam("CurrentUtc1", DateTime.UtcNow.Date.AddDays(-1));
                    cmd.AddParam("CurrentUtc2", DateTime.UtcNow.Date.AddDays(1));

                    return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_listmedals"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_listmembers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("UserID", userId);
                cmd.AddParam("Approved", approved);
                cmd.AddParam("GroupID", groupId);
                cmd.AddParam("RankID", rankId);
                cmd.AddParam("StyledNicks", useStyledNicks);
                cmd.AddParam("Literals", literals);
                cmd.AddParam("Exclude", exclude);
                cmd.AddParam("BeginsWith", beginsWith);
                cmd.AddParam("PageIndex", pageIndex);
                cmd.AddParam("PageSize", pageSize);
                cmd.AddParam("SortName", sortName);
                cmd.AddParam("SortRank", sortRank);
                cmd.AddParam("SortJoined", sortJoined);
                cmd.AddParam("SortPosts", sortPosts);
                cmd.AddParam("SortLastVisit", sortLastVisit);
                cmd.AddParam("NumPosts", numPosts);
                cmd.AddParam("NumPostsCompare", numPostCompare);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_login"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("Name", name);
                cmd.AddParam("Password", password);
                return DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_medal_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);
                cmd.AddParam("MedalID", medalID);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_medal_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);
                cmd.AddParam("MedalID", medalID);

                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_medal_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);
                cmd.AddParam("MedalID", medalID);
                cmd.AddParam("Message", message);
                cmd.AddParam("Hide", hide);
                cmd.AddParam("OnlyRibbon", onlyRibbon);
                cmd.AddParam("SortOrder", sortOrder);
                cmd.AddParam("DateAwarded", dateAwarded);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_migrate"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ProviderUserKey", providerUserKey);
                cmd.AddParam("UserID", userID);
                cmd.AddParam("UpdateProvider", updateProvider);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_nntp"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("Email", email);
                cmd.AddParam("TimeZone", timeZone);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

                return (int)DbAccess.ExecuteScalar(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_pmcount"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_recoverpassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("Email", email);
                return DbAccess.ExecuteScalar(cmd);
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
            using (var connection = DbAccess.CreateConnectionOpen())
            {
                using (var trans = connection.BeginTransaction())
                {
                    try
                    {
                        const int UserID = 0;

                        using (var cmd = DbAccess.GetCommand("user_save"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.AddParam("UserID", UserID);
                            cmd.AddParam("BoardID", boardID);
                            cmd.AddParam("UserName", userName);
                            cmd.AddParam("Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
                            cmd.AddParam("Email", email);
                            cmd.AddParam("Hash", hash);
                            cmd.AddParam("Location", location);
                            cmd.AddParam("HomePage", homePage);
                            cmd.AddParam("TimeZone", timeZone);
                            cmd.AddParam("Approved", approved);
                            cmd.AddParam("PMNotification", 1);
                            cmd.AddParam("AutoWatchTopics", 0);

                            DbAccess.ExecuteNonQuery(cmd, trans);
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
            using (var cmd = DbHelpers.GetCommand("user_removeignoreduser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserId", userId);
                cmd.AddParam("IgnoredUserId", ignoredUserId);
                DbAccess.ExecuteNonQuery(cmd);
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
            [NotNull] object textEditor, 
            [NotNull] object useMobileTheme, 
            [NotNull] object approved, 
            [NotNull] object pmNotification, 
            [NotNull] object autoWatchTopics, 
            [NotNull] object dSTUser, 
            [NotNull] object hideUser, 
            [NotNull] object notificationType)
        {
            using (var cmd = DbHelpers.GetCommand("user_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("UserName", userName);
                cmd.AddParam("DisplayName", displayName);
                cmd.AddParam("Email", email);
                cmd.AddParam("TimeZone", timeZone);
                cmd.AddParam("LanguageFile", languageFile);
                cmd.AddParam("Culture", culture);
                cmd.AddParam("ThemeFile", themeFile);
                cmd.AddParam("TextEditor", textEditor);
                cmd.AddParam("OverrideDefaultTheme", useMobileTheme);
                cmd.AddParam("Approved", approved);
                cmd.AddParam("PMNotification", pmNotification);
                cmd.AddParam("AutoWatchTopics", autoWatchTopics);
                cmd.AddParam("DSTUser", dSTUser);
                cmd.AddParam("HideUser", hideUser);
                cmd.AddParam("NotificationType", notificationType);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// The user_saveavatar.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="avatarUrl">
        /// The avatar url.
        /// </param>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="avatarImageType">
        /// The avatar image type.
        /// </param>
        public static void user_saveavatar([NotNull] object userID, [CanBeNull] object avatarUrl, [CanBeNull] Stream stream, [CanBeNull] object avatarImageType)
        {
            using (var cmd = DbHelpers.GetCommand("user_saveavatar"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Avatar", avatarUrl);

                if (avatarUrl == null)
                {
                    byte[] data = null;

                    if (stream != null)
                    {
                        data = new byte[stream.Length];
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.Read(data, 0, stream.Length.ToType<int>());
                    }


                    cmd.AddParam("AvatarImage", data);
                    cmd.AddParam("AvatarImageType", avatarImageType);
                }

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_savenotification"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("PMNotification", pmNotification);
                cmd.AddParam("AutoWatchTopics", autoWatchTopics);
                cmd.AddParam("NotificationType", notificationType);
                cmd.AddParam("DailyDigest", dailyDigest);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_savepassword"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam(
                  "Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
                DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Update the single Sign on Status
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <param name="authService">The AUTH service.</param>
        public static void user_update_single_sign_on_status([NotNull] int userID, [NotNull]AuthService authService)
        {
            bool isFacebookUser = false, isTwitterUser = false, isGoogleUser = false;

            switch (authService)
            {
                case AuthService.facebook:
                    isFacebookUser = true;
                    break;
                case AuthService.twitter:
                    isTwitterUser = true;
                    break;
                case AuthService.google:
                    isGoogleUser = true;
                    break;
            }

            using (var cmd = DbHelpers.GetCommand("user_update_single_sign_on_status"))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.AddParam("UserID", userID);
                cmd.AddParam("IsFacebookUser", isFacebookUser);
                cmd.AddParam("IsTwitterUser", isTwitterUser);
                cmd.AddParam("IsGoogleUser", isGoogleUser);

                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_savesignature"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Signature", signature);
                DbAccess.ExecuteNonQuery(cmd);
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
                DbHelpers.GetCommand(
                  "update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey",
                  true))
            {
                cmd.CommandType = CommandType.Text;
                cmd.AddParam("UserName", user.UserName);
                cmd.AddParam("Email", user.Email);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ProviderUserKey", user.ProviderUserKey);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_setpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Points", points);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_setrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("ProviderUserKey", providerUserKey);
                cmd.AddParam("Role", role);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_setnotdirty"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userId);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_simplelist"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("StartID", StartID);
                cmd.AddParam("Limit", Limit);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_suspend"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("Suspend", suspend);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("user_viewallthanks"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", UserID);
                cmd.AddParam("PageUserID", pageUserID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("userforum_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("ForumID", forumID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("userforum_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("ForumID", forumID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("userforum_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("ForumID", forumID);
                cmd.AddParam("AccessMaskID", accessMaskID);
                cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("usergroup_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                return DbAccess.GetData(cmd);
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
            using (var cmd = DbHelpers.GetCommand("usergroup_save"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);
                cmd.AddParam("GroupID", groupID);
                cmd.AddParam("Member", member);
                DbAccess.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Delete the Read Tracking
        /// </summary>
        /// <param name="userID">The user id</param>
        /// <returns>Returns the Global Last Read DateTime</returns>
        public static DateTime? User_LastRead([NotNull] object userID)
        {
            using (var cmd = DbHelpers.GetCommand("user_lastread"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("UserID", userID);

                var tableLastRead = DbAccess.ExecuteScalar(cmd);

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

            using (var conn = DbAccess.CreateConnectionOpen())
            {
                using (var cmd = conn.CreateCommand())
                {
                    string table = DbHelpers.GetObjectName("UserProfile");
                    StringBuilder sqlCommand = new StringBuilder("IF EXISTS (SELECT 1 FROM ").Append(table);
                    sqlCommand.Append(" WHERE UserId = @UserID AND ApplicationName = @ApplicationName) ");

                    cmd.AddParam("UserID", userID);
                    cmd.AddParam("ApplicationName", appName);

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
                            cmd.AddParam(valueParam, values[column.Settings.Name].PropertyValue);

                            if ((column.DataType != SqlDbType.Timestamp) || column.Settings.Name != "LastUpdatedDate"
                                || column.Settings.Name != "LastActivity")
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

                    cmd.AddParam("LastUpdatedDate", DateTime.UtcNow);

                    // MembershipUser mu = System.Web.Security.Membership.GetUser(userID);

                    columnStr.Append(",LastActivity ");
                    valueStr.Append(",@LastActivity");
                    setStr.Append(",LastActivity=@LastActivity");

                    cmd.AddParam("LastActivity", DateTime.UtcNow);

                    columnStr.Append(",ApplicationName ");
                    valueStr.Append(",@ApplicationName");
                    setStr.Append(",ApplicationName=@ApplicationName");
                    // cmd.AddParam("@ApplicationID", appId);

                    columnStr.Append(",IsAnonymous ");
                    valueStr.Append(",@IsAnonymous");
                    setStr.Append(",IsAnonymous=@IsAnonymous");

                    cmd.AddParam("IsAnonymous", 0);

                    columnStr.Append(",UserName ");
                    valueStr.Append(",@UserName");
                    setStr.Append(",UserName=@UserName");

                    cmd.AddParam("UserName", userName);

                    sqlCommand.Append("BEGIN UPDATE ").Append(table).Append(" SET ").Append(setStr.ToString());
                    sqlCommand.Append(" WHERE UserId = ").Append(userID.ToString()).Append("");

                    sqlCommand.Append(" END ELSE BEGIN INSERT ").Append(table).Append(" (UserId").Append(columnStr.ToString());
                    sqlCommand.Append(") VALUES (").Append(userID.ToString()).Append("").Append(valueStr.ToString()).Append(") END");

                    cmd.CommandText = sqlCommand.ToString();
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The get profile structure.
        /// </summary>
        /// <returns>
        /// </returns>
        public static DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(DbHelpers.GetObjectName("UserProfile"));

            using (var cmd = DbHelpers.GetCommand(sql,true))
            {
                cmd.CommandType = CommandType.Text;
                return DbAccess.GetData(cmd);
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
              DbHelpers.GetObjectName("UserProfile"), name, type);

            using (var cmd = DbHelpers.GetCommand(sql, true))
            {
                cmd.CommandType = CommandType.Text;
                DbAccess.ExecuteNonQuery(cmd);
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

        /// <summary>
        /// Loads from property value collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        static List<SettingsPropertyColumn> LoadFromPropertyValueCollection(SettingsPropertyValueCollection collection)
        {
            // clear it out just in case something is still in there...
            List<SettingsPropertyColumn> settingsColumnsList = new List<SettingsPropertyColumn>();

            // validiate all the properties and populate the internal settings collection
            foreach (SettingsPropertyValue value in collection)
            {
                SqlDbType dbType;
                int size;

                var tempProperty = value.Property.Attributes["CustomProviderData"];

                if (tempProperty == null)
                {
                    continue;
                }

                // parse custom provider data...
                GetDbTypeAndSizeFromString(tempProperty.ToString(), out dbType, out size);


                // default the size to 256 if no size is specified
                if (dbType == SqlDbType.NVarChar && size == -1)
                {
                    size = 256;
                }

                settingsColumnsList.Add(new SettingsPropertyColumn(value.Property, dbType, size));
            }

            // sync profile table structure with the db...
            DataTable structure = GetProfileStructure();

            // verify all the columns are there...
            foreach (
                SettingsPropertyColumn column in
                    settingsColumnsList.Where(column => !structure.Columns.Contains(column.Settings.Name)))
            {
                // if not, create it...
                AddProfileColumn(column.Settings.Name, column.DataType, column.Size);
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
                    command.Transaction = useTransaction ? command.Connection.BeginTransaction() : null;
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
            using (var cmd = DbHelpers.GetCommand("eventlog_delete"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("EventLogID", eventLogID);
                cmd.AddParam("BoardID", boardID);
                cmd.AddParam("PageUserID", pageUserID);
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("eventlog_deletebyuser"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("BoardID", boardId);
                cmd.AddParam("PageUserID", pageUserId);
               
                DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("forum_listtopics"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("ForumID", forumID);
                using (DataTable dt = DbAccess.GetData(cmd))
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
        private static void message_deleteRecursively(
            [NotNull] object messageID,
            bool isModeratorChanged,
            [NotNull] string deleteReason,
            int isDeleteAction,
            bool deleteLinked,
            bool isLinked,
            bool eraseMessages)
        {
            bool useFileTable = GetBooleanRegistryValue("UseFileTable");

            if (deleteLinked)
            {
                // Delete replies
                using (var cmd = DbHelpers.GetCommand("message_getReplies"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("MessageID", messageID);
                    DataTable tbReplies = DbAccess.GetData(cmd);

                    foreach (DataRow row in tbReplies.Rows)
                    {
                        message_deleteRecursively(
                            row["MessageID"],
                            isModeratorChanged,
                            deleteReason,
                            isDeleteAction,
                            true,
                            true,
                            eraseMessages);
                    }
                }
            }

            // If the files are actually saved in the Hard Drive
            if (!useFileTable)
            {
                using (var cmd = DbHelpers.GetCommand("attachment_list"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("MessageID", messageID);
                    DataTable tbAttachments = DbAccess.GetData(cmd);

                    string uploadDir =
                        HostingEnvironment.MapPath(
                            String.Concat(BaseUrlBuilder.ServerFileRoot, YafBoardFolders.Current.Uploads));

                    foreach (DataRow row in tbAttachments.Rows)
                    {
                        try
                        {
                            string fileName = String.Format(
                                "{0}/{1}.{2}.yafupload",
                                uploadDir,
                                messageID,
                                row["FileName"]);
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
                using (var cmd = DbHelpers.GetCommand("message_delete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("MessageID", messageID);
                    cmd.AddParam("EraseMessage", eraseMessages);
                    DbAccess.ExecuteNonQuery(cmd);
                }
            }
            else
            {
                // Delete Message
                // undelete function added
                using (var cmd = DbHelpers.GetCommand("message_deleteundelete"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.AddParam("MessageID", messageID);
                    cmd.AddParam("isModeratorChanged", isModeratorChanged);
                    cmd.AddParam("DeleteReason", deleteReason);
                    cmd.AddParam("isDeleteAction", isDeleteAction);
                    DbAccess.ExecuteNonQuery(cmd);
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
            using (var cmd = DbHelpers.GetCommand("message_getReplies"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("MessageID", messageID);
                DataTable tbReplies = DbAccess.GetData(cmd);
                foreach (DataRow row in tbReplies.Rows)
                {
                    message_moveRecursively(row["messageID"], moveToTopic);
                }

                using (SqlCommand innercmd = DbHelpers.GetCommand("message_move"))
                {
                    innercmd.CommandType = CommandType.StoredProcedure;
                    innercmd.AddParam("MessageID", messageID);
                    innercmd.AddParam("MoveToTopic", moveToTopic);
                    DbAccess.ExecuteNonQuery(innercmd);
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
            using (var cmd = DbHelpers.GetCommand("topic_listmessages"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParam("TopicID", topicID);
                using (DataTable dt = DbAccess.GetData(cmd))
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