/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2011 Jaben Cargman
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
	using System.Data.Common;
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
	using YAF.Types.Interfaces.Extensions;
	using YAF.Types.Objects;
	using YAF.Utils;
	using YAF.Utils.Extensions;
	using YAF.Utils.Helpers;

	#endregion

	/// <summary>
	/// All the Database functions for YAF
	/// </summary>
	public class LegacyDb
	{
		/// <summary>
		///   Gets Current IDbAccess -- needs to be switched to direct injection into all DB classes.
		/// </summary>
		public static Types.Interfaces.IDbAccess Current
		{
			get
			{
				return ServiceLocatorAccess.CurrentServiceProvider.Get<IDbAccess>();
			}
		}

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

			using (var cmd = Current.GetCommand(searchSql, false))
			{
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("mail_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ProcessID", processId);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd).SelectTypedList(x => new TypedMailList(x));
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
			using (var cmd = Current.GetCommand("message_getallthanks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageIDs", messageIdsSeparatedWithColon);

				return Current.GetData(cmd).AsEnumerable().Select(t => new TypedAllThanks(t));
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
			using (var cmd = Current.GetCommand("message_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);

				return Current.GetData(cmd).AsEnumerable().Select(t => new TypedMessageList(t));
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
			using (var cmd = Current.GetCommand("pollgroup_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumId);
				cmd.AddParam("BoardID", boardId);

				return Current.GetData(cmd).AsEnumerable().Select(r => new TypedPollGroup(r));
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
			using (var cmd = Current.GetCommand("smiley_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("SmileyID", smileyID);

				return Current.GetData(cmd).AsEnumerable().Select(r => new TypedSmileyList(r));
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
					Current.GetCommand(
						"SELECT SUBSTRING(CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')), 1, PATINDEX('%.%', CONVERT(VARCHAR(20), SERVERPROPERTY('productversion')))-1)",
						false))
			{
				return Convert.ToUInt16(Current.ExecuteScalar(cmd));
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
			using (var cmd = Current.GetCommand("topic_favorite_count"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicId);

				return Current.GetData(cmd).GetFirstRowColumnAsValue("FavoriteCount", 0);
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
			using (var cmd = Current.GetCommand("user_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Approved", approved);
				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("RankID", rankID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd).AsEnumerable().Select(x => new TypedUserList(x));
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
			using (var cmd = Current.GetCommand("accessmask_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AccessMaskID", accessMaskID);
				return (int)Current.ExecuteScalar(cmd) != 0;
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
			using (var cmd = Current.GetCommand("accessmask_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("AccessMaskID", accessMaskID);
				cmd.AddParam("ExcludeFlags", excludeFlags);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("accessmask_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AccessMaskID", accessMaskID);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Name", name);
				cmd.AddParam("ReadAccess", readAccess);
				cmd.AddParam("PostAccess", postAccess);
				cmd.AddParam("ReplyAccess", replyAccess);
				cmd.AddParam("PriorityAccess", priorityAccess);
				cmd.AddParam("PollAccess", pollAccess);
				cmd.AddParam("VoteAccess", voteAccess);
				cmd.AddParam("ModeratorAccess", moderatorAccess);
				cmd.AddParam("EditAccess", editAccess);
				cmd.AddParam("DeleteAccess", deleteAccess);
				cmd.AddParam("UploadAccess", uploadAccess);
				cmd.AddParam("DownloadAccess", downloadAccess);
				cmd.AddParam("SortOrder", sortOrder);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("active_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Guests", Guests);
				cmd.AddParam("ShowCrawlers", showCrawlers);
				cmd.AddParam("ActiveTime", activeTime);
				cmd.AddParam("StyledNicks", styledNicks);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("active_list_user"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Guests", Guests);
				cmd.AddParam("ShowCrawlers", showCrawlers);
				cmd.AddParam("ActiveTime", activeTime);
				cmd.AddParam("StyledNicks", styledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("active_listforum"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("StyledNicks", styledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("active_listtopic"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("StyledNicks", styledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("active_stats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("activeaccess_reset"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				Current.ExecuteNonQuery(cmd);
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
		public static DataTable admin_list([NotNull] object boardId, [NotNull] object useStyledNicks)
		{
			using (var cmd = Current.GetCommand("admin_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("album_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AlbumID", albumID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("album_getstats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramAlbumNumber = new SqlParameter("AlbumNumber", 0);
				paramAlbumNumber.Direction = ParameterDirection.Output;
				var paramImageNumber = new SqlParameter("ImageNumber", 0);
				paramImageNumber.Direction = ParameterDirection.Output;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("AlbumID", albumID);

				cmd.Parameters.Add(paramAlbumNumber);
				cmd.Parameters.Add(paramImageNumber);
				Current.ExecuteNonQuery(cmd);

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
			using (var cmd = Current.GetCommand("album_gettitle"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("AlbumID", albumID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("album_image_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ImageID", imageID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("album_image_download"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ImageID", imageID);
				Current.ExecuteNonQuery(cmd);
			}
		}

		public static DataTable album_images_by_user([NotNull] object userID)
		{
			using (var cmd = Current.GetCommand("album_images_by_user"))
			{
				cmd.AddParam("UserID", userID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("album_image_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AlbumID", albumID);
				cmd.AddParam("ImageID", imageID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("album_image_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ImageID", imageID);
				cmd.AddParam("AlbumID", albumID);
				cmd.AddParam("Caption", caption);
				cmd.AddParam("FileName", fileName);
				cmd.AddParam("Bytes", bytes);
				cmd.AddParam("ContentType", contentType);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("album_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("AlbumID", albumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("album_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter();
				paramOutput.Direction = ParameterDirection.ReturnValue;
				cmd.AddParam("AlbumID", albumID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Title", title);
				cmd.AddParam("CoverImageID", coverImageID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
				using (var cmd = Current.GetCommand("attachment_list"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("AttachmentID", attachmentID);
					DataTable tbAttachments = Current.GetData(cmd);

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

			using (var cmd = Current.GetCommand("attachment_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AttachmentID", attachmentID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("attachment_download"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("AttachmentID", attachmentID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("attachment_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("AttachmentID", attachmentID);
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("attachment_save"))
			{
				byte[] fileData = null;
				if (stream != null)
				{
					fileData = new byte[stream.Length];
					stream.Seek(0, SeekOrigin.Begin);
					stream.Read(fileData, 0, (int)stream.Length);
				}

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("FileName", fileName);
				cmd.AddParam("Bytes", bytes);
				cmd.AddParam("ContentType", contentType);
				cmd.AddParam("FileData", fileData);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("bannedip_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ID", ID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("bannedip_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ID", ID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("bannedip_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ID", ID);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Mask", Mask);
				cmd.AddParam("Reason", reason);
				cmd.AddParam("UserID", userID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("bbcode_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BBCodeID", bbcodeID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("bbcode_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("BBCodeID", bbcodeID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("bbcode_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BBCodeID", bbcodeID);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Name", name);
				cmd.AddParam("Description", description);
				cmd.AddParam("OnClickJS", onclickjs);
				cmd.AddParam("DisplayJS", displayjs);
				cmd.AddParam("EditJS", editjs);
				cmd.AddParam("DisplayCSS", displaycss);
				cmd.AddParam("SearchRegEx", searchregex);
				cmd.AddParam("ReplaceRegEx", replaceregex);
				cmd.AddParam("Variables", variables);
				cmd.AddParam("UseModule", usemodule);
				cmd.AddParam("ModuleClass", moduleclass);
				cmd.AddParam("ExecOrder", execorder);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Creates a new board
		/// </summary>
		/// <param name="adminUsername">
		/// Membership Provider User Name
		/// </param>
		/// <param name="adminUserEmail">
		/// The admin User Email.
		/// </param>
		/// <param name="adminUserKey">
		/// Membership Provider User Key
		/// </param>
		/// <param name="boardName">
		/// Name of new board
		/// </param>
		/// <param name="culture">
		/// The culture.
		/// </param>
		/// <param name="languageFile">
		/// The language File.
		/// </param>
		/// <param name="boardMembershipName">
		/// Membership Provider Application Name for new board
		/// </param>
		/// <param name="boardRolesName">
		/// Roles Provider Application Name for new board
		/// </param>
		/// <param name="rolePrefix">
		/// The role Prefix.
		/// </param>
		/// <returns>
		/// The board_create.
		/// </returns>
		public static int board_create([NotNull] object adminUsername, [NotNull] object adminUserEmail, [NotNull] object adminUserKey, [NotNull] object boardName, [NotNull] object culture, [NotNull] object languageFile, [NotNull] object boardMembershipName, [NotNull] object boardRolesName, [NotNull] object rolePrefix)
		{
			using (var cmd = Current.GetCommand("board_create"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardName", boardName);
				cmd.AddParam("Culture", culture);
				cmd.AddParam("LanguageFile", languageFile);
				cmd.AddParam("MembershipAppName", boardMembershipName);
				cmd.AddParam("RolesAppName", boardRolesName);
				cmd.AddParam("UserName", adminUsername);
				cmd.AddParam("UserEmail", adminUserEmail);
				cmd.AddParam("UserKey", adminUserKey);
				cmd.AddParam("IsHostAdmin", 0);
				cmd.AddParam("RolePrefix", rolePrefix);

				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("board_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("board_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("board_poststats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("StyledNicks", useStyledNick);
				cmd.AddParam("ShowNoCountPosts", showNoCountPosts);

				cmd.AddParam("GetDefaults", 0);
				DataTable dt = Current.GetData(cmd);
				if (dt.Rows.Count > 0)
				{
					return dt.Rows[0];
				}
			}

			// vzrus - this happens at new install only when we don't have posts or when they are not visible to a user 
			using (var cmd = Current.GetCommand("board_poststats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("StyledNicks", useStyledNick);
				cmd.AddParam("ShowNoCountPosts", showNoCountPosts);
				cmd.AddParam("GetDefaults", 1);
				DataTable dt = Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("board_resync"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("board_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Name", name);
				cmd.AddParam("LanguageFile", languageFile);
				cmd.AddParam("Culture", culture);
				cmd.AddParam("AllowThreaded", allowThreaded);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("board_stats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);

				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("board_userstats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardId);
				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("buddy_addrequest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				var approved = new SqlParameter("approved", SqlDbType.Bit);
				paramOutput.Direction = ParameterDirection.Output;
				approved.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("ToUserID", ToUserID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				cmd.Parameters.Add(paramOutput);
				cmd.Parameters.Add(approved);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("buddy_approverequest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("ToUserID", ToUserID);
				cmd.AddParam("mutual", Mutual);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("buddy_denyrequest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("ToUserID", ToUserID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("buddy_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("FromUserID", FromUserID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("buddy_remove"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("ToUserID", ToUserID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("category_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("CategoryID", CategoryID);
				return (int)Current.ExecuteScalar(cmd) != 0;
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
			using (var cmd = Current.GetCommand("category_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("CategoryID", categoryID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("category_listread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("CategoryID", categoryID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("category_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("CategoryID", categoryId);
				cmd.AddParam("Name", name);
				cmd.AddParam("CategoryImage", categoryImage);
				cmd.AddParam("SortOrder", sortOrder);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("category_simplelist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StartID", startID);
				cmd.AddParam("Limit", limit);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("checkemail_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Email", email);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("checkemail_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Hash", hash);
				cmd.AddParam("Email", email);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("checkemail_update"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Hash", hash);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("choice_add"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollID", pollID);
				cmd.AddParam("Choice", choice);
				cmd.AddParam("ObjectPath", path);
				cmd.AddParam("MimeType", mime);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("choice_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ChoiceID", choiceID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("choice_update"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ChoiceID", choiceID);
				cmd.AddParam("Choice", choice);
				cmd.AddParam("ObjectPath", path);
				cmd.AddParam("MimeType", mime);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("choice_vote"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ChoiceID", choiceID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("RemoteIP", remoteIP);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// The db_getstats.
		/// </summary>
		public static void db_getstats()
		{

		}

		private static string getStatsMessage;
		/// <summary>
		/// The db_getstats_new.
		/// </summary>
		public static string db_getstats_new()
		{
			try
			{
				using (var connMan = new MsSqlDbConnectionProvider())
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

					using (var cmd = new SqlCommand(sb.ToString(), connMan.GetOpenDbConnection()))
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
		public static void db_recovery_mode([NotNull] string dbRecoveryMode)
		{
			using (var unitOfWork = Current.BeginTransaction())
			{
				string recoveryModeSql = string.Format("ALTER DATABASE {0} SET RECOVERY {1}", unitOfWork.Transaction.Connection.Database, dbRecoveryMode);

				using (var cmd = Current.GetCommand(recoveryModeSql, false))
				{
					Current.ExecuteNonQuery(cmd, unitOfWork);
				}
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
				using (var connMan = new MsSqlDbConnectionProvider())
				{
					connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(recoveryDbMode_InfoMessage);
					var RecoveryModeConn = new SqlConnection(Config.ConnectionString);
					RecoveryModeConn.Open();

					string RecoveryMode = "ALTER DATABASE " + connMan.DBConnection.Database + " SET RECOVERY " + dbRecoveryMode;
					var RecoveryModeCmd = new SqlCommand(RecoveryMode, RecoveryModeConn);

					RecoveryModeCmd.ExecuteNonQuery();
					RecoveryModeConn.Close();
					using (var cmd = new SqlCommand(RecoveryMode, connMan.GetOpenDbConnection()))
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
		public static string db_recovery_mode_warning()
		{
			return string.Empty;
		}

		/// <summary>
		/// The db_reindex.
		/// </summary>
		/// <param name="connectionProvider">
		/// The conn man.
		/// </param>
		public static void db_reindex()
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

			using (var cmd = Current.GetCommand(sb.ToString(), false))
			{
				Current.ExecuteNonQuery(cmd);
			}
		}

		private static string reindexDbMessage;

		/// <summary>
		/// The db_reindex_new.
		/// </summary>
		public static string db_reindex_new()
		{
			try
			{
				using (var connMan = new MsSqlDbConnectionProvider())
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

					using (var cmd = Current.GetCommand(sb.ToString(), false))
					{
						Current.ExecuteNonQuery(cmd);
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
		/// <param name="connectionProvider">
		/// The conn man.
		/// </param>
		/// <param name="useTransaction">
		/// The use Transaction.
		/// </param>
		/// <returns>
		/// The db_runsql.
		/// </returns>
		public static string db_runsql([NotNull] string sql, bool useTransaction)
		{
			using (var command = new SqlCommand(sql, connectionProvider.GetOpenDbConnection()))
			{
				command.CommandTimeout = 9999;
				command.Connection = connectionProvider.GetOpenDbConnection();

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
				using (var connMan = new MsSqlDbConnectionProvider())
				{
					connMan.InfoMessage += new YafDBConnInfoMessageEventHandler(runSql_InfoMessage);
					connMan.DBConnection.FireInfoMessageEventOnUserErrors = true;

					using (var command = new SqlCommand(sql.GetCommandTextReplaced(), connMan.GetOpenDbConnection()))
					{
						command.CommandTimeout = 9999;
						command.Connection = connMan.GetOpenDbConnection();

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
		public static void db_shrink([NotNull] MsSqlDbConnectionProvider DBName)
		{
			string ShrinkSql = "DBCC SHRINKDATABASE(N'" + DBName.DBConnection.Database + "')";
			var ShrinkConn = new SqlConnection(Config.ConnectionString);
			var ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
			ShrinkConn.Open();
			ShrinkCmd.ExecuteNonQuery();
			ShrinkConn.Close();
			using (var cmd = new SqlCommand(ShrinkSql, DBName.GetOpenDbConnection()))
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
				using (var conn = new MsSqlDbConnectionProvider())
				{
					conn.InfoMessage += new YafDBConnInfoMessageEventHandler(dbShink_InfoMessage);
					conn.DBConnection.FireInfoMessageEventOnUserErrors = true;
					string ShrinkSql = "DBCC SHRINKDATABASE(N'" + conn.DBConnection.Database + "')";
					var ShrinkConn = new SqlConnection(Config.ConnectionString);
					var ShrinkCmd = new SqlCommand(ShrinkSql, ShrinkConn);
					ShrinkConn.Open();
					ShrinkCmd.ExecuteNonQuery();
					ShrinkConn.Close();
					using (var cmd = new SqlCommand(ShrinkSql, conn.GetOpenDbConnection()))
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
			using (var connection = new MsSqlDbConnectionProvider().GetOpenDbConnection())
			{
				using (var ds = new DataSet())
				{
					using (var trans = connection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
					{
						using (var da = new SqlDataAdapter(DataExtensions.GetObjectName("category_list"), connection))
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.AddParam("BoardID", boardID);
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill(ds, DataExtensions.GetObjectName("Category"));
							da.SelectCommand.CommandText = DataExtensions.GetObjectName("forum_list");
							da.Fill(ds, DataExtensions.GetObjectName("ForumUnsorted"));

							DataTable dtForumListSorted = ds.GetTable("ForumUnsorted").Clone();
							dtForumListSorted.TableName = DataExtensions.GetObjectName("Forum");
							ds.Tables.Add(dtForumListSorted);
							dtForumListSorted.Dispose();
							forum_list_sort_basic(ds.GetTable("ForumUnsorted"), ds.GetTable("Forum"), 0, 0);
							ds.Tables.Remove(DataExtensions.GetObjectName("ForumUnsorted"));
							ds.Relations.Add(
								"FK_Forum_Category",
								ds.GetTable("Category").Columns["CategoryID"],
								ds.GetTable("Forum").Columns["CategoryID"]);
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

				using (var cmd = Current.GetCommand("eventlog_create"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("Type", type);
					cmd.AddParam("UserID", userID);
					cmd.AddParam("Source", source.ToString());
					cmd.AddParam("Description", description.ToString());
					cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
					Current.ExecuteNonQuery(cmd);
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
		public static void eventlog_delete([NotNull] object eventLogID)
		{
			eventlog_delete(eventLogID, null);
		}

		/// <summary>
		/// The eventlog_list.
		/// </summary>
		/// <param name="boardID">
		/// The board id.
		/// </param>
		/// <returns>
		/// </returns>
		public static DataTable eventlog_list([NotNull] object boardID)
		{
			using (var cmd = Current.GetCommand("eventlog_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
				using (var cmd = Current.GetCommand("extension_delete"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("ExtensionId", extensionId);
					Current.ExecuteNonQuery(cmd);
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}

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
			using (var cmd = Current.GetCommand("extension_edit"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("extensionId", extensionId);
				return Current.GetData(cmd);
			}
		}

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
			using (var cmd = Current.GetCommand("extension_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Extension", extension);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("extension_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Extension", string.Empty);
				return Current.GetData(cmd);
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
				using (var cmd = Current.GetCommand("extension_save"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("extensionId", extensionId);
					cmd.AddParam("BoardId", boardID);
					cmd.AddParam("Extension", Extension);
					Current.ExecuteNonQuery(cmd);
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
				using (var cmd = Current.GetCommand("TopicStatus_Delete"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("TopicStatusID", topicStatusID);
					Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("TopicStatus_Edit"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicStatusID", topicStatusID);
				return Current.GetData(cmd);
			}
		}

		/// <summary>
		/// List all Topics of the Current Board
		/// </summary>
		/// <param name="boardID">The board ID.</param>
		/// <returns></returns>
		public static DataTable TopicStatus_List([NotNull] object boardID)
		{
			using (var cmd = Current.GetCommand("TopicStatus_List"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
				using (var cmd = Current.GetCommand("TopicStatus_Save"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("TopicStatusID", topicStatusID);
					cmd.AddParam("BoardId", boardID);
					cmd.AddParam("TopicStatusName", topicStatusName);
					cmd.AddParam("DefaultDescription", defaultDescription);
					Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("forum_listSubForums"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);

				if (Current.ExecuteScalar(cmd) is DBNull)
				{
					forum_deleteAttachments(forumID);

					using (var cmd_new = Current.GetCommand("forum_delete"))
					{
						cmd_new.CommandType = CommandType.StoredProcedure;
						cmd_new.CommandTimeout = 99999;
						cmd_new.AddParam("ForumID", forumID);
						Current.ExecuteNonQuery(cmd_new);
					}

					return true;
				}

				return false;
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
			using (var cmd = Current.GetCommand("forum_listSubForums"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumOldID);

				if (!(Current.ExecuteScalar(cmd) is DBNull))
				{
					return false;
				}

				using (var cmd_new = Current.GetCommand("forum_move"))
				{
					cmd_new.CommandType = CommandType.StoredProcedure;
					cmd_new.CommandTimeout = 99999;
					cmd_new.AddParam("ForumOldID", forumOldID);
					cmd_new.AddParam("ForumNewID", forumNewID);
					Current.ExecuteNonQuery(cmd_new);
				}

				return true;
			}
		}

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
			using (var cmd = Current.GetCommand("forum_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forum_listall"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Root", startAt);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forum_listallmymoderated"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forum_listall_fromCat"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("CategoryID", categoryID);

				int intCategoryID = Convert.ToInt32(categoryID.ToString());

				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("forum_listpath"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forum_listread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("CategoryID", categoryID);
				cmd.AddParam("ParentID", parentID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("FindLastRead", findLastRead);
				return Current.GetData(cmd);
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
			using (var connMan = new MsSqlDbConnectionProvider())
			{
				using (var ds = new DataSet())
				{
					using (var da = new SqlDataAdapter(DataExtensions.GetObjectName("category_list"), connMan.GetOpenDbConnection()))
					{
						using (SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction(MsSqlDbAccess.IsolationLevel))
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.AddParam("BoardID", boardID);
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill(ds, DataExtensions.GetObjectName("Category"));
							da.SelectCommand.CommandText = DataExtensions.GetObjectName("forum_moderatelist");
							da.SelectCommand.AddParam("UserID", userID);
							da.Fill(ds, DataExtensions.GetObjectName("ForumUnsorted"));
							DataTable dtForumListSorted = ds.GetTable("ForumUnsorted").Clone();
							dtForumListSorted.TableName = DataExtensions.GetObjectName("Forum");
							ds.Tables.Add(dtForumListSorted);
							dtForumListSorted.Dispose();
							forum_list_sort_basic(
								ds.GetTable("ForumUnsorted"),
								ds.GetTable("Forum"),
								0,
								0);
							ds.Tables.Remove(DataExtensions.GetObjectName("ForumUnsorted"));

							// vzrus: Remove here all forums with no reports. Would be better to do it in query...
							// Array to write categories numbers
							var categories = new int[ds.GetTable("Forum").Rows.Count];
							int cntr = 0;

							// We should make it before too as the colection was changed
							ds.GetTable("Forum").AcceptChanges();
							foreach (DataRow dr in ds.GetTable("Forum").Rows)
							{
								categories[cntr] = Convert.ToInt32(dr["CategoryID"]);
								if (Convert.ToInt32(dr["ReportedCount"]) == 0 && Convert.ToInt32(dr["MessageCount"]) == 0)
								{
									dr.Delete();
									categories[cntr] = 0;
								}

								cntr++;
							}

							ds.GetTable("Forum").AcceptChanges();

							foreach (DataRow dr in ds.GetTable("Category").Rows)
							{
								bool deleteMe = true;
								foreach (int t in categories)
								{
									// We check here if the Category is missing in the array where 
									// we've written categories number for each forum
									if (t == Convert.ToInt32(dr["CategoryID"]))
									{
										deleteMe = false;
									}
								}

								if (deleteMe)
								{
									dr.Delete();
								}
							}

							ds.GetTable("Category").AcceptChanges();

							ds.Relations.Add(
								"FK_Forum_Category",
								ds.GetTable("Category").Columns["CategoryID"],
								ds.GetTable("Forum").Columns["CategoryID"]);

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
			using (var cmd = Current.GetCommand("forum_moderators"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StyledNicks", useStyledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("moderators_team_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StyledNicks", useStyledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forum_resync"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ForumID", forumID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("forum_save"))
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
				return long.Parse(Current.ExecuteScalar(cmd).ToString());
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
					Current.GetCommand(
						string.Format("SELECT {0}(@ForumID,@ParentID)", DataExtensions.GetObjectName("forum_save_parentschecker")), false))
			{
				cmd.CommandType = CommandType.Text;
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("ParentID", parentID);
				return Convert.ToInt32(Current.ExecuteScalar(cmd));
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
			using (var cmd = Current.GetCommand("forum_simplelist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StartID", StartID);
				cmd.AddParam("Limit", Limit);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forumaccess_group"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("GroupID", groupID);
				return userforumaccess_sort_list(Current.GetData(cmd), 0, 0, 0);
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
			using (var cmd = Current.GetCommand("forumaccess_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("forumaccess_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("AccessMaskID", accessMaskID);
				Current.ExecuteNonQuery(cmd);
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
				using (var connection = Current.CreateConnection())
				{
					// just attempt to open the connection to test if a DB is available.
					connection.Open();
				}
			}
			catch (DbException ex)
			{
				// unable to connect to the DB...
				if (!debugging)
				{
					errorStr = string.Format("Unable to connect to the Database. Exception Message: {0} ({1})", ex.Message, ex.ErrorCode);
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
			using (var cmd = Current.GetCommand("group_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("GroupID", groupID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("group_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("GroupID", groupID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("group_medal_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("MedalID", medalID);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("group_medal_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("MedalID", medalID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("group_medal_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("MedalID", medalID);
				cmd.AddParam("Message", message);
				cmd.AddParam("Hide", hide);
				cmd.AddParam("OnlyRibbon", onlyRibbon);
				cmd.AddParam("SortOrder", sortOrder);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("group_member"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
		public static DataTable group_rank_style([NotNull] int boardID)
		{
			using (var cmd = Current.GetCommand("group_rank_style"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("group_save"))
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

				return long.Parse(Current.ExecuteScalar(cmd).ToString());
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
			using (var cmd = Current.GetCommand("mail_create"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("From", from);
				cmd.AddParam("FromName", fromName);
				cmd.AddParam("To", to);
				cmd.AddParam("ToName", toName);
				cmd.AddParam("Subject", subject);
				cmd.AddParam("Body", body);
				cmd.AddParam("BodyHtml", bodyHtml);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("mail_createwatch"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("From", from);
				cmd.AddParam("FromName", fromName);
				cmd.AddParam("Subject", subject);
				cmd.AddParam("Body", body);
				cmd.AddParam("BodyHtml", bodyHtml);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("mail_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MailID", mailID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("medal_listusers"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("MedalID", medalID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("medal_resort"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("MedalID", medalID);
				cmd.AddParam("Move", move);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_Addthanks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("MessageID", MessageID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_gettextbyids"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageIDs", messageIDs);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_getthanks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", MessageID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_Removethanks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter("paramOutput", SqlDbType.NVarChar, 255);
				paramOutput.Direction = ParameterDirection.Output;
				cmd.AddParam("FromUserID", FromUserID);
				cmd.AddParam("MessageID", MessageID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_thanksnumber"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				var paramOutput = new SqlParameter();
				paramOutput.Direction = ParameterDirection.ReturnValue;
				cmd.AddParam("MessageID", messageID);
				cmd.Parameters.Add(paramOutput);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_approve"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_findunread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("MessageID", messageId);
				cmd.AddParam("LastRead", lastRead);
				cmd.AddParam("ShowDeleted", showDeleted);
				cmd.AddParam("AuthorUserID", authorUserID);
				return Current.GetData(cmd);
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

			using (var cmd = Current.GetCommand("message_reply_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				DataTable dtr = Current.GetData(cmd);

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
			using (var cmd = Current.GetCommand("message_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_listreported"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_listreporters"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("UserID", 0);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_listreporters"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_move"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("MoveToTopic", moveToTopic);
				Current.ExecuteNonQuery(cmd);
			}

			// moveAll=true anyway
			// it's in charge of moving answers of moved post
			if (moveAll)
			{
				using (var cmd = Current.GetCommand("message_getReplies"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("MessageID", messageID);
					DataTable tbReplies = Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_report"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("ReporterID", userID);
				cmd.AddParam("ReportedDate", reportedDateTime);
				cmd.AddParam("ReportText", reportText);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_reportcopyover"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_reportresolve"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageFlag", messageFlag);
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_save"))
			{
				var paramMessageID = new SqlParameter("MessageID", messageID);
				paramMessageID.Direction = ParameterDirection.Output;

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
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_secdata"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("MessageID", MessageID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_simplelist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StartID", StartID);
				cmd.AddParam("Limit", Limit);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("message_unapproved"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
		public static void message_update([NotNull] object messageID, [NotNull] object priority, [NotNull] object message, [NotNull] object description, [NotNull] object subject, [NotNull] object flags, [NotNull] object reasonOfEdit, [NotNull] object isModeratorChanged, [NotNull] object overrideApproval, [NotNull] object originalMessage, [NotNull] object editedBy, [CanBeNull] object status = null, [CanBeNull] object styles = null)
		{
			using (var cmd = Current.GetCommand("message_update"))
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

				Current.ExecuteNonQuery(cmd);
			}
		}

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
			using (var cmd = Current.GetCommand("messagehistory_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("MessageID", messageId);
				cmd.AddParam("DaysToClean", daysToClean);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("nntpforum_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("NntpForumID", nntpForumID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("nntpforum_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Minutes", minutes);
				cmd.AddParam("NntpForumID", nntpForumID);
				cmd.AddParam("Active", active);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd);
			}
		}

		public static IEnumerable<TypedNntpForum> NntpForumList(int boardID, int? minutes, int? nntpForumID, bool? active)
		{
			using (var cmd = Current.GetCommand("nntpforum_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Minutes", minutes);
				cmd.AddParam("NntpForumID", nntpForumID);
				cmd.AddParam("Active", active);

				return Current.GetData(cmd).AsEnumerable().Select(r => new TypedNntpForum(r));
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
			using (var cmd = Current.GetCommand("nntpforum_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("NntpForumID", nntpForumID);
				cmd.AddParam("NntpServerID", nntpServerID);
				cmd.AddParam("GroupName", groupName);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("Active", active);
				cmd.AddParam("DateCutOff", datecutoff);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("nntpforum_update"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("NntpForumID", nntpForumID);
				cmd.AddParam("LastMessageNo", lastMessageNo);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("nntpserver_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("NntpServerID", nntpServerID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("nntpserver_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("NntpServerID", nntpServerID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("nntpserver_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("NntpServerID", nntpServerID);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Name", name);
				cmd.AddParam("Address", address);
				cmd.AddParam("Port", port);
				cmd.AddParam("UserName", userName);
				cmd.AddParam("UserPass", userPass);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("nntptopic_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Thread", thread);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("nntptopic_savemessage"))
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
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
					using (var cmd = Current.GetCommand("pageload"))
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

						using (DataTable dt = Current.GetData(cmd))
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
			using (var sqlCommand = Current.GetCommand("pmessage_archive"))
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.AddParam("UserPMessageID", userPMessageID);

				Current.ExecuteNonQuery(sqlCommand);
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
			using (var cmd = Current.GetCommand("pmessage_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserPMessageID", userPMessageID);
				cmd.AddParam("FromOutbox", fromOutbox);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("pmessage_info"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("pmessage_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ToUserID", toUserID);
				cmd.AddParam("FromUserID", fromUserID);
				cmd.AddParam("UserPMessageID", userPMessageID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("pmessage_markread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserPMessageID", userPMessageID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("pmessage_prune"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("DaysRead", daysRead);
				cmd.AddParam("DaysUnread", daysUnread);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
		public static void pmessage_save([NotNull] object fromUserID, [NotNull] object toUserID, [NotNull] object subject, [NotNull] object body, [NotNull] object Flags)
		{
			using (var cmd = Current.GetCommand("pmessage_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("FromUserID", fromUserID);
				cmd.AddParam("ToUserID", toUserID);
				cmd.AddParam("Subject", subject);
				cmd.AddParam("Body", body);
				cmd.AddParam("Flags", Flags);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("poll_remove"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollGroupID", pollGroupID);
				cmd.AddParam("PollID", pollID);
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("RemoveCompletely", removeCompletely);
				cmd.AddParam("RemoveEverywhere", removeEverywhere);
				Current.ExecuteNonQuery(cmd);
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
					sb.Append(DataExtensions.GetObjectName("Topic"));
					sb.Append(" WHERE TopicID = @TopicID; ");
				}
				else if (question.ForumId > 0)
				{
					sb.Append("select @PollGroupID = PollGroupID  from ");
					sb.Append(DataExtensions.GetObjectName("Forum"));
					sb.Append(" WHERE ForumID = @ForumID");
				}
				else if (question.CategoryId > 0)
				{
					sb.Append("select @PollGroupID = PollGroupID  from ");
					sb.Append(DataExtensions.GetObjectName("Category"));
					sb.Append(" WHERE CategoryID = @CategoryID");
				}

				// the group doesn't exists, create a new one
				sb.Append("IF @PollGroupID IS NULL BEGIN INSERT INTO ");
				sb.Append(DataExtensions.GetObjectName("PollGroupCluster"));
				sb.Append("(UserID,Flags ) VALUES(@UserID, @Flags) SET @NewPollGroupID = SCOPE_IDENTITY(); END; ");

				sb.Append("INSERT INTO ");
				sb.Append(DataExtensions.GetObjectName("Poll"));

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
						sb.Append(DataExtensions.GetObjectName("Choice"));
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
					sb.Append(DataExtensions.GetObjectName("Topic"));
					sb.Append(" SET PollID = @NewPollGroupID WHERE TopicID = @TopicID; ");
				}

				// fill a pollgroup field in Forum Table if the call comes from a forum's topic list 
				if (question.ForumId > 0)
				{
					sb.Append("UPDATE ");
					sb.Append(DataExtensions.GetObjectName("Forum"));
					sb.Append(" SET PollGroupID= @NewPollGroupID WHERE ForumID= @ForumID; ");
				}

				// fill a pollgroup field in Category Table if the call comes from a category's topic list 
				if (question.CategoryId > 0)
				{
					sb.Append("UPDATE ");
					sb.Append(DataExtensions.GetObjectName("Category"));
					sb.Append(" SET PollGroupID= @NewPollGroupID WHERE CategoryID= @CategoryID; ");
				}

				// fill a pollgroup field in Board Table if the call comes from the main page poll 
				sb.Append("END;  ");

				using (var cmd = Current.GetCommand(sb.ToString(), true))
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

					cmd.AddParam("Question", question.Question);

					if (question.Closes > DateTime.MinValue)
					{
						cmd.AddParam("Closes", question.Closes);
					}

					// set poll group flags
					int groupFlags = 0;
					if (question.IsBound)
					{
						groupFlags = groupFlags | 2;
					}

					cmd.AddParam("UserID", question.UserId);
					cmd.AddParam("Flags", groupFlags);
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

					cmd.AddParam("PollFlags", pollFlags);

					for (uint choiceCount1 = 0; choiceCount1 < question.Choice.GetUpperBound(1) + 1; choiceCount1++)
					{
						if (!string.IsNullOrEmpty(question.Choice[0, choiceCount1]))
						{
							cmd.AddParam(String.Format("Choice{0}", choiceCount1), question.Choice[0, choiceCount1]);
							cmd.AddParam(String.Format("Votes{0}", choiceCount1), 0);

							cmd.AddParam(
								String.Format("ChoiceObjectPath{0}", choiceCount1),
								question.Choice[1, choiceCount1].IsNotSet() ? String.Empty : question.Choice[1, choiceCount1]);
							cmd.AddParam(
								String.Format("ChoiceMimeType{0}", choiceCount1),
								question.Choice[2, choiceCount1].IsNotSet() ? String.Empty : question.Choice[2, choiceCount1]);
						}
					}

					if (question.TopicId > 0)
					{
						cmd.AddParam("TopicID", question.TopicId);
					}

					if (question.ForumId > 0)
					{
						cmd.AddParam("ForumID", question.ForumId);
					}

					if (question.CategoryId > 0)
					{
						cmd.AddParam("CategoryID", question.CategoryId);
					}

					using (var unitOfWork = Current.BeginTransaction())
					{
						Current.ExecuteNonQuery(cmd, unitOfWork);

						unitOfWork.Commit();
					}

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
			using (var cmd = Current.GetCommand("poll_stats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollID", pollId);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("poll_update"))
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

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("pollgroup_attach"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollGroupID", pollGroupId);
				cmd.AddParam("TopicID", topicId);
				cmd.AddParam("ForumID", forumId);
				cmd.AddParam("CategoryID", categoryId);
				cmd.AddParam("BoardID", boardId);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("pollgroup_remove"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollGroupID", pollGroupID);
				cmd.AddParam("TopicID", topicId);
				cmd.AddParam("ForumID", forumId);
				cmd.AddParam("CategoryID", categoryId);
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("RemoveCompletely", removeCompletely);
				cmd.AddParam("RemoveEverywhere", removeEverywhere);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("pollgroup_stats"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollGroupID", pollGroupId);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("pollgroup_votecheck"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollGroupID", pollGroupId);
				cmd.AddParam("UserID", userId);
				cmd.AddParam("RemoteIP", remoteIp);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("pollvote_check"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("PollID", pollid);
				cmd.AddParam("UserID", userid);
				cmd.AddParam("RemoteIP", remoteip);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("post_alluser"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("PageUserID", pageUserID);
				cmd.AddParam("topCount", topCount);
				return Current.GetData(cmd);
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
		public static DataTable post_list([NotNull] object topicId, [NotNull] object authorUserID, [NotNull] object updateViewCount,
																			bool showDeleted,
																			bool styledNicks,
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
			using (var cmd = Current.GetCommand("post_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicId);
				cmd.AddParam("AuthorUserID", authorUserID);
				cmd.AddParam("UpdateViewCount", updateViewCount);
				cmd.AddParam("ShowDeleted", showDeleted);
				cmd.AddParam("StyledNicks", styledNicks);
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

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("post_list_reverse10"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("rank_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("RankID", rankID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("rank_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("RankID", rankID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("rank_save"))
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

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("recent_users"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("TimeSinceLastLogin", timeSinceLastLogin);
				cmd.AddParam("StyledNicks", styledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("registry_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Name", name);
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("registry_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Name", name);
				cmd.AddParam("Value", value);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("registry_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Name", name);
				cmd.AddParam("Value", value);
				cmd.AddParam("BoardID", boardID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("replace_words_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ID", ID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("replace_words_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("ID", id);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("replace_words_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ID", id);
				cmd.AddParam("BoardID", boardId);
				cmd.AddParam("badword", badword);
				cmd.AddParam("goodword", goodword);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("rss_topic_latest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("NumPosts", numOfPostsToRetrieve);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("ShowNoCountPosts", showNoCountPosts);
				return Current.GetData(cmd);
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

			using (var cmd = Current.GetCommand(sb.ToString(), true))
			{
				cmd.CommandType = CommandType.Text;
				cmd.AddParam("ForumID", forumId);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("shoutbox_clearmessages"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardId", boardId);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("shoutbox_getmessages"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("NumberOfMessages", numberOfMessages);
				cmd.AddParam("BoardId", boardId);
				cmd.AddParam("StyledNicks", useStyledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("shoutbox_savemessage"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardId", boardId);
				cmd.AddParam("Message", message);
				cmd.AddParam("UserName", userName);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("IP", ip);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("smiley_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("SmileyID", smileyID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("smiley_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("SmileyID", smileyID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("smiley_listunique"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("smiley_resort"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("SmileyID", smileyID);
				cmd.AddParam("Move", move);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("smiley_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("SmileyID", smileyID);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Code", code);
				cmd.AddParam("Icon", icon);
				cmd.AddParam("Emoticon", emoticon);
				cmd.AddParam("SortOrder", sortOrder);
				cmd.AddParam("Replace", replace);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// The system_deleteinstallobjects.
		/// </summary>
		public static void system_deleteinstallobjects()
		{
			string sql = string.Format("DROP PROCEDURE{0}", DataExtensions.GetObjectName("system_initialize"));
			using (var cmd = Current.GetCommand(sql, false))
			{
				cmd.CommandType = CommandType.Text;
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("system_initialize"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Name", forumName);
				cmd.AddParam("TimeZone", timeZone);
				cmd.AddParam("Culture", culture);
				cmd.AddParam("LanguageFile", languageFile);
				cmd.AddParam("ForumEmail", forumEmail);
				cmd.AddParam("SmtpServer", string.Empty);
				cmd.AddParam("User", userName);
				cmd.AddParam("UserEmail", userEmail);
				cmd.AddParam("UserKey", providerUserKey);
				cmd.AddParam("RolePrefix", rolePrefix);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			script = script.GetCommandTextReplaced();

			List<string> statements = Regex.Split(script, "\\sGO\\s", RegexOptions.IgnoreCase).ToList();
			ushort sqlMajVersion = SqlServerMajorVersionAsShort();
			using (var connMan = new MsSqlDbConnectionProvider())
			{
				// use transactions...
				if (useTransactions)
				{
					using (SqlTransaction trans = connMan.GetOpenDbConnection().BeginTransaction(MsSqlDbAccess.IsolationLevel))
					{
						foreach (string sql0 in statements)
						{
							string sql = sql0.Trim();

							sql = sql.CleanForSQLServerVersion(sqlMajVersion);

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
									cmd.Connection = connMan.GetOpenDbConnection();
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
			var databaseObjects =
				Current.GetData(
					Current.GetCommand(
						"select Name,IsUserTable = OBJECTPROPERTY(id, N'IsUserTable'),IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like '{databaseOwner}.{objectQualifier}%'"));

			var userName = Current.ExecuteScalar(Current.GetCommand("select current_user", false));

			using (var unitOfWork = Current.BeginTransaction())
			{
				if (grant)
				{
					foreach (DataRow row in databaseObjects.Select("IsProcedure=1 or IsScalarFunction=1"))
					{
						Current.ExecuteNonQuery(
							Current.GetCommand(string.Format(@"grant execute on ""{0}"" to ""{1}""", row["Name"], userName), false),
							unitOfWork);
					}

					foreach (DataRow row in databaseObjects.Select("IsUserTable=1 or IsView=1"))
					{
						Current.ExecuteNonQuery(
							Current.GetCommand(string.Format(@"grant select,update on ""{0}"" to ""{1}""", row["Name"], userName), false),
							unitOfWork);
					}
				}
				else
				{
					var cmd = Current.GetCommand("sp_changeobjectowner");

					foreach (DataRow row in databaseObjects.Select("IsUserTable=1"))
					{
						cmd.Parameters.Clear();

						cmd.AddParam("objname", row["Name"]);
						cmd.AddParam("newowner", "dbo");

						try
						{
							Current.ExecuteNonQuery(cmd, unitOfWork);
						}
						catch (DbException)
						{
						}
					}

					foreach (DataRow row in databaseObjects.Select("IsView=1"))
					{
						cmd.Parameters.Clear();
						cmd.AddParam("objname", row["Name"]);
						cmd.AddParam("newowner", "dbo");
						try
						{
							Current.ExecuteNonQuery(cmd, unitOfWork);
						}
						catch (DbException)
						{
						}
					}
				}

				unitOfWork.Commit();
			}
		}

		/// <summary>
		/// Not in use anymore. Only required for old database versions.
		/// </summary>
		/// <returns>
		/// </returns>
		public static DataTable system_list()
		{
			using (var cmd = Current.GetCommand("system_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("system_updateversion"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("Version", version);
				cmd.AddParam("VersionName", versionname);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// The topic_active.
		/// </summary>
		/// <param name="boardID">
		/// The board id.
		/// </param>
		/// <param name="pageUserId">
		/// The page user id.
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
		/// <param name="findLastRead">
		/// Indicates if the Table should Countain the last Access Date
		/// </param>
		/// <returns>
		/// Returns the List with the Active Topics
		/// </returns>
		public static DataTable topic_active([NotNull] object boardID, [NotNull] object pageUserId, [NotNull] object since, [NotNull] object categoryID, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
		{
			using (var cmd = Current.GetCommand("topic_active"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("Since", since);
				cmd.AddParam("CategoryID", categoryID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("FindLastRead", findLastRead);

				return Current.GetData(cmd);
			}
		}

		/// The topic_unanswered
		/// </summary>
		/// <param name="boardID">
		/// The board id.
		/// </param>
		/// <param name="pageUserId">
		/// The page user id.
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
		/// <param name="findLastRead">
		/// Indicates if the Table should Countain the last Access Date
		/// </param>
		/// <returns>
		/// Returns the List with the Active Topics
		/// </returns>
		public static DataTable topic_unanswered([NotNull] object boardID, [NotNull] object pageUserId, [NotNull] object since, [NotNull] object categoryID, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
		{
			using (var cmd = Current.GetCommand("topic_unanswered"))
			{
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("Since", since);
				cmd.AddParam("CategoryID", categoryID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("FindLastRead", findLastRead);

				return Current.GetData(cmd);
			}
		}


		/// <summary>
		/// Gets all topics where the pageUserid has posted
		/// </summary>
		/// <param name="boardID">
		/// The board id.
		/// </param>
		/// <param name="pageUserId">
		/// The page user id.
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
		/// <param name="findLastRead">
		/// Indicates if the Table should Countain the last Access Date
		/// </param>
		/// <returns>
		/// Returns the List with the User Topics
		/// </returns>
		public static DataTable Topics_ByUser([NotNull] object boardID, [NotNull] object pageUserId, [NotNull] object since, [NotNull] object categoryID, [NotNull] object useStyledNicks, [CanBeNull]bool findLastRead)
		{
			using (var cmd = Current.GetCommand("topics_byuser"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("Since", since);
				cmd.AddParam("CategoryID", categoryID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("FindLastRead", findLastRead);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_announcements"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("NumPosts", numOfPostsToRetrieve);
				cmd.AddParam("PageUserID", pageUserId);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_create_by_message"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				cmd.AddParam("ForumID", forumId);
				cmd.AddParam("Subject", newTopicSubj);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				DataTable dt = Current.GetData(cmd);
				return long.Parse(dt.Rows[0]["TopicID"].ToString());
			}
		}

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
			using (var cmd = Current.GetCommand("topic_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("EraseTopic", eraseTopic);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("topic_favorite_add"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);
				Current.ExecuteNonQuery(cmd);
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
		public static DataTable topic_favorite_details([NotNull] object boardID, [NotNull] object pageUserId, [NotNull] object since, [NotNull] object categoryID, [NotNull] object useStyledNicks)
		{
			using (var cmd = Current.GetCommand("topic_favorite_details"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("Since", since);
				cmd.AddParam("CategoryID", categoryID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_favorite_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_favorite_remove"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("topic_findduplicate"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicName", topicName);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("topic_findnext"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_findprev"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_info"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("topic_latest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("NumPosts", numOfPostsToRetrieve);
				cmd.AddParam("PageUserID", pageUserId);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("ShowNoCountPosts", showNoCountPosts);
				cmd.AddParam("FindLastRead", findLastRead);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_list"))
			{
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("UserID", userId);
				cmd.AddParam("Date", sinceDate);
				cmd.AddParam("ToDate", toDate);
				cmd.AddParam("PageIndex", pageIndex);
				cmd.AddParam("PageSize", pageSize);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("ShowMoved", showMoved);
				cmd.AddParam("FindLastRead", findLastRead);

				return Current.GetData(cmd);
			}
		}

		public static DataTable announcements_list([NotNull] object forumID, [NotNull] object userId, [NotNull] object sinceDate, [NotNull] object toDate, [NotNull] object pageIndex, [NotNull] object pageSize, [NotNull] object useStyledNicks, [NotNull] object showMoved, [CanBeNull]bool findLastRead)
		{
			using (var cmd = Current.GetCommand("announcements_list"))
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

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_lock"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("Locked", locked);
				Current.ExecuteNonQuery(cmd);
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
		public static void topic_move([NotNull] object topicID, [NotNull] object forumID, [NotNull] object showMoved)
		{
			using (var cmd = Current.GetCommand("topic_move"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("ShowMoved", showMoved);
				cmd.AddParam("LinkDays", linkDays);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("topic_prune"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("Days", days);
				cmd.AddParam("PermDelete", permDelete);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("topic_save"))
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

				DataTable dt = Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_simplelist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StartID", StartID);
				cmd.AddParam("Limit", Limit);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("topic_updatetopic"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicId);
				cmd.AddParam("Topic", topic);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_thankfromcount"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("UserID", userId);

				cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

				var thankCount = (int)Current.ExecuteScalar(cmd);

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
			using (var cmd = Current.GetCommand("user_repliedtopic"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("MessageID", messageId);
				cmd.AddParam("UserID", userId);

				cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

				var messageCount = (int)Current.ExecuteScalar(cmd);

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
			using (var cmd = Current.GetCommand("user_thankedmessage"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("MessageID", messageId);
				cmd.AddParam("UserID", userId);

				cmd.CommandTimeout = int.Parse(Config.SqlCommandTimeout);

				var thankCount = (int)Current.ExecuteScalar(cmd);

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
			using (var cmd = Current.GetCommand("user_accessmasks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);

				return userforumaccess_sort_list(Current.GetData(cmd), 0, 0, 0);
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
			using (var cmd = Current.GetCommand("user_activity_rank"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("StartDate", startDate);
				cmd.AddParam("DisplayNumber", displayNumber);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_addignoreduser"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserId", userId);
				cmd.AddParam("IgnoredUserId", ignoredUserId);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Add Reputation Points to the specified user id.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="points">The points.</param>
		public static void user_addpoints([NotNull] object userID, [NotNull] object points)
		{
			using (var cmd = Current.GetCommand("user_addpoints"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Points", points);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_adminsave"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Name", name);
				cmd.AddParam("DisplayName", displayName);
				cmd.AddParam("Email", email);
				cmd.AddParam("Flags", flags);
				cmd.AddParam("RankID", rankID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_approve"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_approveall"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				Current.ExecuteNonQuery(cmd);
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
				using (var cmd = Current.GetCommand("user_aspnet"))
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.AddParam("BoardID", boardID);
					cmd.AddParam("UserName", userName);
					cmd.AddParam("DisplayName", displayName);
					cmd.AddParam("Email", email);
					cmd.AddParam("ProviderUserKey", providerUserKey);
					cmd.AddParam("IsApproved", isApproved);
					cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

					return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_avatarimage"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_changepassword"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("OldPassword", oldPassword);
				cmd.AddParam("NewPassword", newPassword);
				return (bool)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_deleteavatar"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_deleteold"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Days", days);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_emails"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("GroupID", groupID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", false))
			{
				cmd.CommandType = CommandType.Text;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ProviderUserKey", providerUserKey);

				return (int)(Current.ExecuteScalar(cmd) ?? 0);
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
			using (var cmd = Current.GetCommand("user_getalbumsdata"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_getpoints"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_getsignature"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.ExecuteScalar(cmd).ToString();
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
			using (var cmd = Current.GetCommand("user_getsignaturedata"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_getthanks_from"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("PageUserID", pageUserId);
				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_getthanks_to"))
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
				Current.ExecuteNonQuery(cmd);

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
			using (var cmd = Current.GetCommand("user_guest"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				return Current.ExecuteScalar(cmd).ToType<int?>();
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
			using (var cmd = Current.GetCommand("user_ignoredlist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserId", userId);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_isuserignored"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserId", userId);
				cmd.AddParam("IgnoredUserId", ignoredUserId);

				var param = cmd.CreateParameter();

				param.ParameterName = "result";
				param.DbType = DbType.Boolean;
				param.Direction = ParameterDirection.ReturnValue;

				cmd.Parameters.Add(param);

				Current.ExecuteNonQuery(cmd);

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
					using (var cmd = Current.GetCommand("user_lazydata"))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.AddParam("UserID", userID);
						cmd.AddParam("BoardID", boardID);
						cmd.AddParam("ShowPendingMails", showPendingMails);
						cmd.AddParam("ShowPendingBuddies", showPendingBuddies);
						cmd.AddParam("ShowUnreadPMs", showUnreadPMs);
						cmd.AddParam("ShowUserAlbums", showUserAlbums);
						cmd.AddParam("ShowUserStyle", styledNicks);
						return Current.GetData(cmd).Rows[0];
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
			using (var cmd = Current.GetCommand("user_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Approved", approved);
				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("RankID", rankID);
				cmd.AddParam("StyledNicks", useStyledNicks);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return Current.GetData(cmd);
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
		public static DataTable User_ListProfilesByIdsList([NotNull] int[] userIdsList, [CanBeNull] object useStyledNicks)
		{
			string stIds = userIdsList.Aggregate(string.Empty, (current, userId) => current + (',' + userId)).Trim(',');
			// Profile columns cannot yet exist when we first are gettinng data.
			try
			{
				var sqlBuilder = new StringBuilder("SELECT up.*, u.Name as UserName,u.DisplayName,Style = case(@StyledNicks) when 1 then  ISNULL(( SELECT TOP 1 f.Style FROM ");
				sqlBuilder.Append(DataExtensions.GetObjectName("UserGroup"));
				sqlBuilder.Append(" e join ");
				sqlBuilder.Append(DataExtensions.GetObjectName("Group"));
				sqlBuilder.Append(" f on f.GroupID=e.GroupID WHERE e.UserID=u.UserID AND LEN(f.Style) > 2 ORDER BY f.SortOrder), r.Style) else '' end ");
				sqlBuilder.Append(" FROM ");
				sqlBuilder.Append(DataExtensions.GetObjectName("UserProfile"));
				sqlBuilder.Append(" up JOIN ");
				sqlBuilder.Append(DataExtensions.GetObjectName("User"));
				sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
				sqlBuilder.Append(DataExtensions.GetObjectName("Rank"));
				sqlBuilder.AppendFormat(" r ON r.RankID = u.RankID where UserID IN ({0})  ", stIds);
				using (var cmd = Current.GetCommand(sqlBuilder.ToString(), false))
				{
					cmd.AddParam("StyledNicks", useStyledNicks);
					return Current.GetData(cmd);
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
				var sqlBuilder = new StringBuilder("SELECT up.Birthday, up.UserID, u.Name as UserName,u.DisplayName, u.TimeZone, Style = case(@StyledNicks) when 1 then  ISNULL(( SELECT TOP 1 f.Style FROM ");
				sqlBuilder.Append(DataExtensions.GetObjectName("UserGroup"));
				sqlBuilder.Append(" e join ");
				sqlBuilder.Append(DataExtensions.GetObjectName("Group"));
				sqlBuilder.Append(" f on f.GroupID=e.GroupID WHERE e.UserID=u.UserID AND LEN(f.Style) > 2 ORDER BY f.SortOrder), r.Style) else '' end ");
				sqlBuilder.Append(" FROM ");
				sqlBuilder.Append(DataExtensions.GetObjectName("UserProfile"));
				sqlBuilder.Append(" up JOIN ");
				sqlBuilder.Append(DataExtensions.GetObjectName("User"));
				sqlBuilder.Append(" u ON u.UserID = up.UserID JOIN ");
				sqlBuilder.Append(DataExtensions.GetObjectName("Rank"));
				sqlBuilder.Append(" r ON r.RankID = u.RankID where up.Birthday = @CurrentDate ");
				using (var cmd = Current.GetCommand(sqlBuilder.ToString(), false))
				{
					cmd.AddParam("StyledNicks", useStyledNicks);
					cmd.AddParam("CurrentDate", DateTime.UtcNow.Date);
					return Current.GetData(cmd);
				}
			}
			catch (Exception e)
			{
				LegacyDb.eventlog_create(null, e.Source, e.Message, EventLogTypes.Error);
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
			using (var cmd = Current.GetCommand("user_listmedals"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("UserID", userID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_listmembers"))
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
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_login"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("Name", name);
				cmd.AddParam("Password", password);
				return Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_medal_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("UserID", userID);
				cmd.AddParam("MedalID", medalID);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_medal_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("UserID", userID);
				cmd.AddParam("MedalID", medalID);

				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_medal_save"))
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

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_migrate"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ProviderUserKey", providerUserKey);
				cmd.AddParam("UserID", userID);
				cmd.AddParam("UpdateProvider", updateProvider);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_nntp"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserName", userName);
				cmd.AddParam("Email", email);
				cmd.AddParam("TimeZone", timeZone);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);

				return (int)Current.ExecuteScalar(cmd);
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
			using (var cmd = Current.GetCommand("user_pmcount"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_recoverpassword"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("UserName", userName);
				cmd.AddParam("Email", email);
				return Current.ExecuteScalar(cmd);
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
		public static void user_removeignoreduser([NotNull] object userId, [NotNull] object ignoredUserId)
		{
			using (var cmd = Current.GetCommand("user_removeignoreduser"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserId", userId);
				cmd.AddParam("IgnoredUserId", ignoredUserId);
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Remove Repuatation Points from the specified user id.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <param name="points">The points.</param>
		public static void user_removepoints([NotNull] object userID, [NotNull] object points)
		{
			using (var cmd = Current.GetCommand("user_removepoints"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Points", points);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_save"))
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
				cmd.AddParam("UseSingleSignOn", useSingleSignOn);
				cmd.AddParam("TextEditor", textEditor);
				cmd.AddParam("OverrideDefaultTheme", useMobileTheme);
				cmd.AddParam("Approved", approved);
				cmd.AddParam("PMNotification", pmNotification);
				cmd.AddParam("AutoWatchTopics", autoWatchTopics);
				cmd.AddParam("DSTUser", dSTUser);
				cmd.AddParam("HideUser", hideUser);
				cmd.AddParam("NotificationType", notificationType);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
		public static void user_saveavatar([NotNull] object userID, [NotNull] object avatar, [NotNull] byte[] avatarData, [NotNull] string avatarImageType)
		{
			using (var cmd = Current.GetCommand("user_saveavatar"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Avatar", avatar);
				cmd.AddParam("AvatarImage", avatarData);
				cmd.AddParam("AvatarImageType", avatarImageType);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_savenotification"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("PMNotification", pmNotification);
				cmd.AddParam("AutoWatchTopics", autoWatchTopics);
				cmd.AddParam("NotificationType", notificationType);
				cmd.AddParam("DailyDigest", dailyDigest);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_savepassword"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam(
					"Password", FormsAuthentication.HashPasswordForStoringInConfigFile(password.ToString(), "md5"));
				Current.ExecuteNonQuery(cmd);
			}
		}

		/// <summary>
		/// Save the IsFacebook Status
		/// </summary>
		/// <param name="userID">
		/// The user id.
		/// </param>
		/// <param name="isFacebookUser">
		/// The is Facebook User.
		/// </param>
		public static void user_update_single_sign_on_status([NotNull] object userID, [NotNull] object isFacebookUser, [NotNull] object isTwitterUser)
		{
			using (var cmd = Current.GetCommand("user_update_single_sign_on_status"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("IsFacebookUser", isFacebookUser);
				cmd.AddParam("IsTwitterUser", isTwitterUser);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_savesignature"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Signature", signature);
				Current.ExecuteNonQuery(cmd);
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
					Current.GetCommand(
						"update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey",
						true))
			{
				cmd.CommandType = CommandType.Text;
				cmd.AddParam("UserName", user.UserName);
				cmd.AddParam("Email", user.Email);
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ProviderUserKey", user.ProviderUserKey);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_setpoints"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Points", points);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_setrole"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("ProviderUserKey", providerUserKey);
				cmd.AddParam("Role", role);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_setnotdirty"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userId);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_simplelist"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("StartID", StartID);
				cmd.AddParam("Limit", Limit);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("user_suspend"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("Suspend", suspend);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("user_viewallthanks"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", UserID);
				cmd.AddParam("PageUserID", pageUserID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("userforum_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("userforum_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("userforum_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("AccessMaskID", accessMaskID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("usergroup_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("usergroup_save"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("GroupID", groupID);
				cmd.AddParam("Member", member);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("watchforum_add"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("watchforum_check"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("watchforum_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("WatchForumID", watchForumID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("watchforum_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("watchtopic_add"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("watchtopic_check"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("watchtopic_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("WatchTopicID", watchTopicID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("watchtopic_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				return Current.GetData(cmd);
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
			using (var cmd = Current.GetCommand("readtopic_addorupdate"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
				using (var cmd = Current.GetCommand("readtopic_delete"))
				{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.AddParam("UserID", userID);
						Current.ExecuteNonQuery(cmd);
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
		public static DateTime User_LastRead([NotNull] object userID, [NotNull] DateTime lastVisitDate)
		{
			using (var cmd = Current.GetCommand("user_lastread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);

				var tableLastRead = Current.ExecuteScalar(cmd);

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
		public static DateTime Readtopic_lastread([NotNull] object userID, [NotNull] object topicID)
		{
			using (var cmd = Current.GetCommand("readtopic_lastread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("TopicID", topicID);

				var tableLastRead = Current.ExecuteScalar(cmd);

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
			using (var cmd = Current.GetCommand("readforum_addorupdate"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);
				cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
				Current.ExecuteNonQuery(cmd);
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
				using (var cmd = Current.GetCommand("readforum_delete"))
				{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.AddParam("UserID", userID);
						Current.ExecuteNonQuery(cmd);
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
		public static DateTime ReadForum_lastread([NotNull] object userID, [NotNull] object forumID)
		{
			using (var cmd = Current.GetCommand("readforum_lastread"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("UserID", userID);
				cmd.AddParam("ForumID", forumID);

				var tableLastRead = Current.ExecuteScalar(cmd);

            	return tableLastRead.ToType<DateTime?>();
			}
		}

		#endregion

		#region Methods

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

        public static DataTable GetProfileStructure()
        {
            string sql = @"SELECT TOP 1 * FROM {0}".FormatWith(MsSqlDbAccess.GetObjectName("UserProfile"));

            using (var cmd = MsSqlDbAccess.GetCommand(sql,true))
            {
                cmd.CommandType = CommandType.Text;
                return MsSqlDbAccess.Current.GetData(cmd);
            }
        }

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
				using (var unitOfWork = Current.BeginTransaction())
				{
					unitOfWork.Setup(command);

					using (reader = command.ExecuteReader())
					{
						{
							if (reader.HasRows)
							{
								int rowIndex = 1;
								var columnNames = reader.GetSchemaTable().Rows.Cast<DataRow>().Select(r => r["ColumnName"].ToString()).ToList();

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
						}

						unitOfWork.Commit();
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
		private static void eventlog_delete([NotNull] object eventLogID, [NotNull] object boardID)
		{
			using (var cmd = Current.GetCommand("eventlog_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("EventLogID", eventLogID);
				cmd.AddParam("BoardID", boardID);
				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("forum_listtopics"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("ForumID", forumID);
				using (DataTable dt = Current.GetData(cmd))
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
			using (var cmd = Current.GetCommand("medal_delete"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("MedalID", medalID);
				cmd.AddParam("Category", category);

				Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("medal_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.AddParam("BoardID", boardID);
				cmd.AddParam("MedalID", medalID);
				cmd.AddParam("Category", category);

				return Current.GetData(cmd);
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
				using (var cmd = Current.GetCommand("message_getReplies"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("MessageID", messageID);
					DataTable tbReplies = Current.GetData(cmd);

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
				using (var cmd = Current.GetCommand("attachment_list"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("MessageID", messageID);
					DataTable tbAttachments = Current.GetData(cmd);

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
				using (var cmd = Current.GetCommand("message_delete"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("MessageID", messageID);
					cmd.AddParam("EraseMessage", eraseMessages);
					Current.ExecuteNonQuery(cmd);
				}
			}
			else
			{
				// Delete Message
				// undelete function added
				using (var cmd = Current.GetCommand("message_deleteundelete"))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.AddParam("MessageID", messageID);
					cmd.AddParam("isModeratorChanged", isModeratorChanged);
					cmd.AddParam("DeleteReason", deleteReason);
					cmd.AddParam("isDeleteAction", isDeleteAction);
					Current.ExecuteNonQuery(cmd);
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
			using (var cmd = Current.GetCommand("message_reply_list"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				DataTable dtr = Current.GetData(cmd);

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
			using (var cmd = Current.GetCommand("message_getReplies"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("MessageID", messageID);
				DataTable tbReplies = Current.GetData(cmd);

				foreach (DataRow row in tbReplies.Rows)
				{
					message_moveRecursively(row["messageID"], moveToTopic);
				}

				using (var innercmd = Current.GetCommand("message_move"))
				{
					innercmd.CommandType = CommandType.StoredProcedure;
					innercmd.AddParam("MessageID", messageID);
					innercmd.AddParam("MoveToTopic", moveToTopic);
					Current.ExecuteNonQuery(innercmd);
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
			using (var cmd = Current.GetCommand("topic_listmessages"))
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.AddParam("TopicID", topicID);
				using (DataTable dt = Current.GetData(cmd))
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