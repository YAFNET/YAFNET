/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjшrnar Henden
 * Copyright (C) 2006-2007 Jaben Cargman
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
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace YAF.Classes.Data
{
	public static class DB
	{
		/// <summary>
		/// Gets the database size
		/// </summary>
		/// <returns>intager value for database size</returns>
		static public int DBSize()
		{
			using ( SqlCommand cmd = new SqlCommand( "select sum(cast(size as integer))/128 from sysfiles" ) )
			{
				cmd.CommandType = CommandType.Text;
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}

		#region Forum

		static public DataRow pageload( object sessionID, object boardID, object userKey, object ip, object location, object browser,
			object platform, object categoryID, object forumID, object topicID, object messageID, object donttrack )
		{
			int nTries = 0;
			while ( true )
			{
				try
				{
					using ( SqlCommand cmd = DBAccess.GetCommand( "pageload" ) )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue( "SessionID", sessionID );
						cmd.Parameters.AddWithValue( "BoardID", boardID );
						cmd.Parameters.AddWithValue( "UserKey", userKey );
						cmd.Parameters.AddWithValue( "IP", ip );
						cmd.Parameters.AddWithValue( "Location", location );
						cmd.Parameters.AddWithValue( "Browser", browser );
						cmd.Parameters.AddWithValue( "Platform", platform );
						cmd.Parameters.AddWithValue( "CategoryID", categoryID );
						cmd.Parameters.AddWithValue( "ForumID", forumID );
						cmd.Parameters.AddWithValue( "TopicID", topicID );
						cmd.Parameters.AddWithValue( "MessageID", messageID );
						cmd.Parameters.AddWithValue( "DontTrack", donttrack );
						using ( DataTable dt = DBAccess.GetData( cmd ) )
						{
							if ( dt.Rows.Count > 0 )
								return dt.Rows [0];
							else
								return null;
						}
					}
				}
				catch ( SqlException x )
				{
					if ( x.Number == 1205 && nTries < 3 )
					{
						/// Transaction (Process ID XXX) was deadlocked on lock resources with another process and has been chosen as the deadlock victim. Rerun the transaction.
					}
					else
						throw new ApplicationException( string.Format( "Sql Exception with error number {0} (Tries={1})", x.Number, nTries ), x );
				}
				++nTries;
			}
		}
		/// <summary>
		/// Returns Search results
		/// </summary>
		/// <param name="ToSearch"></param>
		/// <param name="sf">Field to search</param>
		/// <param name="sw">Search what</param>
		/// <param name="fid"></param>
		/// <param name="UserID">ID of user</param>
		/// <returns>Results</returns>
		static public DataTable GetSearchResult( string toSearchWhat, string toSearchFromWho, SearchWhatFlags searchFromWhoMethod, SearchWhatFlags searchWhatMethod, int forumIDToStartAt, int userID, int boardId, int maxResults, bool useFullText )
		{
			// if ( ToSearch.Length == 0 )
			//	return new DataTable();

			if ( toSearchWhat == "*" )
				toSearchWhat = "";
			string forumIDs = "";

			if ( forumIDToStartAt != 0 )
			{
				DataTable dt = forum_listall_sorted( boardId, userID, null, false, forumIDToStartAt );
				foreach ( DataRow dr in dt.Rows )
					forumIDs = forumIDs + Convert.ToString( Convert.ToInt32( dr ["ForumID"] ) ) + ",";
				forumIDs = forumIDs.Substring( 0, forumIDs.Length - 1 );
			}

			// fix quotes for SQL insertion...
			toSearchWhat = toSearchWhat.Replace( "'", "''" );
			toSearchFromWho = toSearchFromWho.Replace( "'", "''" );

			string searchSql = ( maxResults == 0 ) ? "SELECT" : ( "SELECT TOP " + maxResults.ToString() );

			searchSql += " a.ForumID, a.TopicID, a.Topic, b.UserID, b.Name, c.MessageID, c.Posted, c.Message, c.Flags ";
			searchSql += "from {databaseOwner}.{objectQualifier}topic a left join {databaseOwner}.{objectQualifier}message c on a.TopicID = c.TopicID left join {databaseOwner}.{objectQualifier}user b on c.UserID = b.UserID join {databaseOwner}.{objectQualifier}vaccess x on x.ForumID=a.ForumID ";
			searchSql += String.Format( "where x.ReadAccess<>0 and x.UserID={0} and c.IsApproved = 1 AND c.IsDeleted = 0", userID );

			string [] words;
			searchSql += " and ( ";

			// generate user search sql...
			switch ( searchFromWhoMethod )
			{
				case SearchWhatFlags.AllWords:
					words = toSearchFromWho.Split( ' ' );
					foreach ( string word in words )
					{
						searchSql += string.Format( " b.Name like N'%{0}%' and ", word );

					}
					// remove last OR in sql query
					searchSql = searchSql.Substring( 0, searchSql.Length - 5 );
					break;
				case SearchWhatFlags.AnyWords:
					words = toSearchFromWho.Split( ' ' );
					foreach ( string word in words )
					{
						searchSql += string.Format( " b.Name like N'%{0}%' or ", word );

					}
					// remove last OR in sql query
					searchSql = searchSql.Substring( 0, searchSql.Length - 4 );
					break;
				case SearchWhatFlags.ExactMatch:
					searchSql += string.Format( "b.Name like N'%{0}%' or ", toSearchFromWho );

					break;
			}

			searchSql += " ) and (";
			bool bFirst = true;

			// generate message and topic search sql...
			switch ( searchWhatMethod )
			{
				case SearchWhatFlags.AllWords:
					words = toSearchWhat.Split( ' ' );
					if ( useFullText )
					{
						string ftInner = "";

						// make the inner FULLTEXT search
						foreach ( string word in words )
						{
							if ( !bFirst ) ftInner += " AND ";
							else bFirst = false;
							ftInner += String.Format( @"""{0}""", word );
						}
						// make final string...
						searchSql += string.Format( "( CONTAINS (c.Message, ' {0} ') OR CONTAINS (a.Topic, ' {0} ') )", ftInner );
					}
					else
					{
						foreach ( string word in words )
						{
							if ( !bFirst ) searchSql += " AND ";
							else bFirst = false;
							searchSql += String.Format( "(c.Message like '%{0}%' OR a.Topic LIKE '%{0}%')", word );
						}
					}
					break;
				case SearchWhatFlags.AnyWords:
					words = toSearchWhat.Split( ' ' );

					if ( useFullText )
					{
						string ftInner = "";

						// make the inner FULLTEXT search
						foreach ( string word in words )
						{
							if ( !bFirst ) ftInner += " OR ";
							else bFirst = false;
							ftInner += String.Format( @"""{0}""", word );
						}
						// make final string...
						searchSql += string.Format( "( CONTAINS (c.Message, ' {0} ') OR CONTAINS (a.Topic, ' {0} ') )", ftInner );
					}
					else
					{
						foreach ( string word in words )
						{
							if ( !bFirst ) searchSql += " OR ";
							else bFirst = false;
							searchSql += String.Format( "c.Message LIKE '%{0}%' OR a.Topic LIKE '%{0}%'", word );
						}
					}
					break;
				case SearchWhatFlags.ExactMatch:
					if ( useFullText )
					{
						searchSql += string.Format( "( CONTAINS (c.Message, ' \"{0}\" ') OR CONTAINS (a.Topic, ' \"{0}\" ') )", toSearchWhat );
					}
					else
					{
						searchSql += string.Format( "c.Message LIKE '%{0}%' OR a.Topic LIKE '%{0}%' ", toSearchWhat );
					}
					break;
			}
			searchSql += " ) ";

			// Ederon : 6/16/2007 - forum IDs start above 0, if forum id is 0, there is no forum filtering
			if ( forumIDToStartAt > 0 )
			{
				searchSql += string.Format( "and a.ForumID in {0}", forumIDs );
			}

			searchSql += " order by c.Posted desc";

			using ( SqlCommand cmd = DBAccess.GetCommand( searchSql, true ) )
			{
				return DBAccess.GetData( cmd );
			}
		}

		#endregion

		#region DataSets
		/// <summary>
		/// Gets a list of categories????
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <returns>DataSet with categories</returns>
		static public DataSet ds_forumadmin( object boardID )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				using ( DataSet ds = new DataSet() )
				{
					using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( DBAccess.IsolationLevel ) )
					{
						using ( SqlDataAdapter da = new SqlDataAdapter( DBAccess.GetObjectName( "category_list" ), connMan.DBConnection ) )
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "BoardID", boardID );
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill( ds, DBAccess.GetObjectName( "Category" ) );
							da.SelectCommand.CommandText = DBAccess.GetObjectName( "forum_list" );
							da.Fill( ds, DBAccess.GetObjectName( "ForumUnsorted" ) );

							DataTable dtForumListSorted = ds.Tables [DBAccess.GetObjectName( "ForumUnsorted" )].Clone();
							dtForumListSorted.TableName = DBAccess.GetObjectName( "Forum" );
							ds.Tables.Add( dtForumListSorted );
							dtForumListSorted.Dispose();
							forum_list_sort_basic( ds.Tables [DBAccess.GetObjectName( "ForumUnsorted" )], ds.Tables [DBAccess.GetObjectName( "Forum" )], 0, 0 );
							ds.Tables.Remove( DBAccess.GetObjectName( "ForumUnsorted" ) );
							ds.Relations.Add( "FK_Forum_Category", ds.Tables [DBAccess.GetObjectName( "Category" )].Columns ["CategoryID"], ds.Tables [DBAccess.GetObjectName( "Forum" )].Columns ["CategoryID"] );
							trans.Commit();
						}

						return ds;
					}
				}
			}
		}
		#endregion

		#region yaf_AccessMask
		/// <summary>
		/// Gets a list of access mask properities
		/// </summary>
		/// <param name="boardID">ID of Board</param>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <returns></returns>
		static public DataTable accessmask_list( object boardID, object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "accessmask_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Deletes an access mask
		/// </summary>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <returns></returns>
		static public bool accessmask_delete( object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "accessmask_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return ( int )DBAccess.ExecuteScalar( cmd ) != 0;
			}
		}
		/// <summary>
		/// Saves changes to a access mask 
		/// </summary>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <param name="boardID">ID of board</param>
		/// <param name="name">Name of access mask</param>
		/// <param name="readAccess">Read Access?</param>
		/// <param name="postAccess">Post Access?</param>
		/// <param name="replyAccess">Reply Access?</param>
		/// <param name="priorityAccess">Priority Access?</param>
		/// <param name="pollAccess">Poll Access?</param>
		/// <param name="voteAccess">Vote Access?</param>
		/// <param name="moderatorAccess">Moderator Access?</param>
		/// <param name="editAccess">Edit Access?</param>
		/// <param name="deleteAccess">Delete Access?</param>
		/// <param name="uploadAccess">Upload Access?</param>
		static public void accessmask_save( object accessMaskID, object boardID, object name, object readAccess, object postAccess, object replyAccess, object priorityAccess, object pollAccess, object voteAccess, object moderatorAccess, object editAccess, object deleteAccess, object uploadAccess )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "accessmask_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "ReadAccess", readAccess );
				cmd.Parameters.AddWithValue( "PostAccess", postAccess );
				cmd.Parameters.AddWithValue( "ReplyAccess", replyAccess );
				cmd.Parameters.AddWithValue( "PriorityAccess", priorityAccess );
				cmd.Parameters.AddWithValue( "PollAccess", pollAccess );
				cmd.Parameters.AddWithValue( "VoteAccess", voteAccess );
				cmd.Parameters.AddWithValue( "ModeratorAccess", moderatorAccess );
				cmd.Parameters.AddWithValue( "EditAccess", editAccess );
				cmd.Parameters.AddWithValue( "DeleteAccess", deleteAccess );
				cmd.Parameters.AddWithValue( "UploadAccess", uploadAccess );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Active
		/// <summary>
		/// Gets list of active users
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="Guests"></param>
		/// <returns>Returns a DataTable of active users</returns>
		static public DataTable active_list( object boardID, object Guests )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "active_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Guests", Guests );
				return DBAccess.GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the list of active users within a certain forum
		/// </summary>
		/// <param name="forumID">forumID</param>
		/// <returns>DataTable of all ative users in a forum</returns>
		static public DataTable active_listforum( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "active_listforum" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Gets the list of active users in a topic
		/// </summary>
		/// <param name="topicID">ID of topic </param>
		/// <returns>DataTable of all users that are in a topic</returns>
		static public DataTable active_listtopic( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "active_listtopic" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return DBAccess.GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the activity statistics for a board
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <returns>DataRow of activity stata</returns>
		static public DataRow active_stats( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "active_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					return dt.Rows [0];
				}
			}
		}
		#endregion

		#region yaf_Attachment
		/// <summary>
		/// Gets a list of attachments
		/// </summary>
		/// <param name="messageID">messageID</param>
		/// <param name="attachmentID">attachementID</param>
		/// <param name="boardID">boardID</param>
		/// <returns>DataTable with attachement list</returns>
		static public DataTable attachment_list( object messageID, object attachmentID, object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// saves attachment
		/// </summary>
		/// <param name="messageID">messageID</param>
		/// <param name="fileName">File Name</param>
		/// <param name="bytes">number of bytes</param>
		/// <param name="contentType">type of attchment</param>
		/// <param name="stream">stream of bytes</param>
		static public void attachment_save( object messageID, object fileName, object bytes, object contentType, System.IO.Stream stream )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_save" ) )
			{
				byte [] fileData = null;
				if ( stream != null )
				{
					fileData = new byte [stream.Length];
					stream.Seek( 0, System.IO.SeekOrigin.Begin );
					stream.Read( fileData, 0, ( int )stream.Length );
				}
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "FileName", fileName );
				cmd.Parameters.AddWithValue( "Bytes", bytes );
				cmd.Parameters.AddWithValue( "ContentType", contentType );
				cmd.Parameters.AddWithValue( "FileData", fileData );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		//ABOT CHANGE 16.04.04
		/// <summary>
		/// Delete attachment
		/// </summary>
		/// <param name="attachmentID">ID of attachment to delete</param>
		static public void attachment_delete( object attachmentID )
		{
			bool UseFileTable = false;

			using ( DataTable dt = YAF.Classes.Data.DB.registry_list( "UseFileTable" ) )
				foreach ( DataRow dr in dt.Rows )
					UseFileTable = Convert.ToBoolean( Convert.ToInt32( dr ["Value"] ) );

			//If the files are actually saved in the Hard Drive
			if ( !UseFileTable )
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
					DataTable tbAttachments = DBAccess.GetData( cmd );
					string sUpDir = HttpContext.Current.Server.MapPath( YAF.Classes.Config.UploadDir );
					foreach ( DataRow row in tbAttachments.Rows )
						System.IO.File.Delete( String.Format( "{0}{1}.{2}", sUpDir, row ["MessageID"], row ["FileName"] ) );
				}

			}
			using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				DBAccess.ExecuteNonQuery( cmd );
			}
			//End ABOT CHANGE 16.04.04
		}


		/// <summary>
		/// Attachement dowload
		/// </summary>
		/// <param name="attachmentID">ID of attachemnt to download</param>
		static public void attachment_download( object attachmentID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_download" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_BannedIP
		/// <summary>
		/// List of Baned IP's
		/// </summary>
		/// <param name="boardID">ID of board</param>
		/// <param name="ID">ID</param>
		/// <returns>DataTable of banned IPs</returns>
		static public DataTable bannedip_list( object boardID, object ID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bannedip_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ID", ID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Saves baned ip in database
		/// </summary>
		/// <param name="ID">ID</param>
		/// <param name="boardID">BoardID</param>
		/// <param name="Mask">Mask</param>
		static public void bannedip_save( object ID, object boardID, object Mask )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bannedip_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Mask", Mask );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes Banned IP
		/// </summary>
		/// <param name="ID">ID of banned ip to delete</param>
		static public void bannedip_delete( object ID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bannedip_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Board
		/// <summary>
		/// Gets a list of information about a board
		/// </summary>
		/// <param name="boardID">board id</param>
		/// <returns>DataTable</returns>
		static public DataTable board_list( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Gets posting statistics
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <returns>DataRow of Poststats</returns>
		static public DataRow board_poststats( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_poststats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					return dt.Rows [0];
				}
			}
		}
		/// <summary>
		/// Recalculates topic and post numbers and updates last post for all forums in all boards
		/// </summary>
		static public void board_resync()
		{
			board_resync( null );
		}
		/// <summary>
		/// Recalculates topic and post numbers and updates last post for specified board
		/// </summary>
		/// <param name="boardID">BoardID of board to do re-sync for, if null, all boards are re-synced</param>
		static public void board_resync( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_resync" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Gets statistica about number of posts etc.
		/// </summary>
		/// <returns>DataRow</returns>
		static public DataRow board_stats()
		{
			return board_stats( null );
		}
		static public DataRow board_stats( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );

				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					return dt.Rows [0];
				}
			}
		}
		/// <summary>
		/// Saves board information
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="name">Name of Board</param>
		/// <param name="allowThreaded">Boolen value, allowThreaded</param>
		static public void board_save( object boardID, object name, object allowThreaded )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "AllowThreaded", allowThreaded );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Creates a new board
		/// </summary>
		/// <param name="name">Name of new board</param>
		/// <param name="allowThreaded">Bool, allow threaded?</param>
		/// <param name="userName">User name of admin</param>
		/// <param name="userEmail">Email of admin</param>
		/// <param name="userPass">Admins password</param>
		static public void board_create( object name, object allowThreaded, object userName, object userEmail, object userPass )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_create" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardName", name );
				cmd.Parameters.AddWithValue( "AllowThreaded", allowThreaded );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "UserEmail", userEmail );
				cmd.Parameters.AddWithValue( "UserPass", userPass );
				cmd.Parameters.AddWithValue( "IsHostAdmin", false );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes a board
		/// </summary>
		/// <param name="boardID">ID of board to delete</param>
		static public void board_delete( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "board_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Category
		/// <summary>
		/// Deletes a category
		/// </summary>
		/// <param name="CategoryID">ID of category to delete</param>
		/// <returns>Bool value indicationg if category was deleted</returns>
		static public bool category_delete( object CategoryID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "category_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "CategoryID", CategoryID );
				return ( int )DBAccess.ExecuteScalar( cmd ) != 0;
			}
		}
		/// <summary>
		/// Gets a list of forums in a category
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <param name="categoryID">categotyID</param>
		/// <returns>DataTable with a list of forums in a category</returns>
		static public DataTable category_list( object boardID, object categoryID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "category_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Gets a list of forum categories
		/// </summary>
		/// <param name="boardID"></param>
		/// <param name="userID"></param>
		/// <param name="categoryID"></param>
		/// <returns></returns>
		static public DataTable category_listread( object boardID, object userID, object categoryID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "category_listread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists categories very simply (for URL rewriting)
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable category_simplelist( int StartID, int Limit )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "category_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Saves changes to a category
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="CategoryID">CategoryID so save changes to</param>
		/// <param name="Name">Name of the category</param>
		/// <param name="SortOrder">Sort Order</param>
		static public void category_save( object boardID, object CategoryID, object Name, object SortOrder )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "category_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", CategoryID );
				cmd.Parameters.AddWithValue( "Name", Name );
				cmd.Parameters.AddWithValue( "SortOrder", SortOrder );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_CheckEmail
		/// <summary>
		/// Saves a new email into the table for verification
		/// </summary>
		/// <param name="UserID">ID of user to verify</param>
		/// <param name="Hash">Hash of user</param>
		/// <param name="Email">email of user</param>
		static public void checkemail_save( object userID, object hash, object email )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "checkemail_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Hash", hash );
				cmd.Parameters.AddWithValue( "Email", email );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Updates a hash
		/// </summary>
		/// <param name="hash">New hash</param>
		/// <returns>DataTable with user information</returns>
		static public DataTable checkemail_update( object hash )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "checkemail_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Hash", hash );
				return DBAccess.GetData( cmd );
			}
		}
		#endregion

		#region yaf_Choice
		/// <summary>
		/// Saves a vote in the database
		/// </summary>
		/// <param name="choiceID">Choice of the vote</param>
		static public void choice_vote( object choiceID, object userID, object remoteIP )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "choice_vote" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ChoiceID", choiceID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "RemoteIP", remoteIP );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_EventLog
		static public void eventlog_create( object userID, object source, object description, object type )
		{
			try
			{
				if ( userID == null ) userID = DBNull.Value;
				using ( SqlCommand cmd = DBAccess.GetCommand( "eventlog_create" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "Type", ( object )type );
					cmd.Parameters.AddWithValue( "UserID", ( object )userID );
					cmd.Parameters.AddWithValue( "Source", ( object )source.ToString() );
					cmd.Parameters.AddWithValue( "Description", ( object )description.ToString() );
					DBAccess.ExecuteNonQuery( cmd );
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}

		static public void eventlog_create( object userID, object source, object description )
		{
			eventlog_create( userID, ( object )source.GetType().ToString(), description, ( object )0 );
		}

		static public void eventlog_delete( object eventLogID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "eventlog_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "EventLogID", eventLogID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public DataTable eventlog_list( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "eventlog_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return DBAccess.GetData( cmd );
			}
		}
		#endregion yaf_EventLog

		// Admin control of file extensions - MJ Hufford
		#region yaf_Extensions

		static public void extension_delete( object extensionId )
		{
			try
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "extension_delete" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "ExtensionId", extensionId );
					DBAccess.ExecuteNonQuery( cmd );
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}

		// Get Extension record by extensionId
		static public DataTable extension_edit( object extensionId )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "extension_edit" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "extensionId", extensionId );
				return DBAccess.GetData( cmd );
			}

		}

		// Used to validate a file before uploading
		static public DataTable extension_list( object boardID, object extension )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "extension_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Extension", extension );
				return DBAccess.GetData( cmd );
			}

		}

		// Returns an extension list for a given Board
		static public DataTable extension_list( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "extension_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Extension", "" );
				return DBAccess.GetData( cmd );
			}

		}

		// Saves / creates extension
		static public void extension_save( object extensionId, object boardID, object Extension )
		{
			try
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "extension_save" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "extensionId", extensionId );
					cmd.Parameters.AddWithValue( "BoardId", boardID );
					cmd.Parameters.AddWithValue( "Extension", Extension );
					DBAccess.ExecuteNonQuery( cmd );
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}
		#endregion yaf_EventLog

		#region yaf_PollVote
		/// <summary>
		/// Checks for a vote in the database
		/// </summary>
		/// <param name="choiceID">Choice of the vote</param>
		static public DataTable pollvote_check( object pollid, object userid, object remoteip )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pollvote_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollid );
				cmd.Parameters.AddWithValue( "UserID", userid );
				cmd.Parameters.AddWithValue( "RemoteIP", remoteip );
				return DBAccess.GetData( cmd );
			}
		}
		#endregion

		#region yaf_Forum
		//ABOT NEW 16.04.04
		/// <summary>
		/// Deletes attachments out of a entire forum
		/// </summary>
		/// <param name="ForumID">ID of forum to delete all attachemnts out of</param>
		static private void forum_deleteAttachments( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listtopics" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						topic_delete( row ["TopicID"] );
					}
				}
			}
		}
		//END ABOT NEW 16.04.04
		//ABOT CHANGE 16.04.04
		/// <summary>
		/// Deletes a forum
		/// </summary>
		/// <param name="ForumID">forum to delete</param>
		/// <returns>bool to indicate that forum has been deleted</returns>
		static public bool forum_delete( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listSubForums" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				if ( DBAccess.ExecuteScalar( cmd ) is DBNull )
				{
					forum_deleteAttachments( forumID );
					using ( SqlCommand cmd_new = DBAccess.GetCommand( "forum_delete" ) )
					{
						cmd_new.CommandType = CommandType.StoredProcedure;
						cmd_new.Parameters.AddWithValue( "ForumID", forumID );
						DBAccess.ExecuteNonQuery( cmd_new );
					}
					return true;
				}
				else
					return false;
			}
		}
		//END ABOT CHANGE 16.04.04
		//ABOT NEW 16.04.04: This new function lists all moderated topic by the specified user
		/// <summary>
		/// Lists all moderated forums for a user
		/// </summary>
		/// <param name="boardID">board if of moderators</param>
		/// <param name="userID">user id</param>
		/// <returns>DataTable of moderated forums</returns>
		static public DataTable forum_listallMyModerated( object boardID, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listallmymoderated" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		//END ABOT NEW 16.04.04
		/// <summary>
		/// Gets a list of topics in a forum
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <param name="ForumID">forumID</param>
		/// <returns>DataTable with list of topics from a forum</returns>
		static public DataTable forum_list( object boardID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}

		/// <summary>
		/// Listes all forums accessible to a user
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="userID">ID of user</param>
		/// <returns>DataTable of all accessible forums</returns>
		static public DataTable forum_listall( object boardID, object userID )
		{
			return forum_listall( boardID, userID, 0 );
		}

		/// <summary>
		/// Lists all forums accessible to a user
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="userID">ID of user</param>
		/// <param name="startAt">startAt ID</param>
		/// <returns>DataTable of all accessible forums</returns>
		static public DataTable forum_listall( object boardID, object userID, object startAt )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listall" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Root", startAt );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists all forums within a given subcategory
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <returns>DataTable with list</returns>
		static public DataTable forum_listall_fromCat( object boardID, object categoryID )
		{
			return forum_listall_fromCat( boardID, categoryID, true );
		}
		/// <summary>
		/// Lists forums very simply (for URL rewriting)
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable forum_simplelist( int StartID, int Limit )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists all forums within a given subcategory
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <param name="EmptyFirstRow">EmptyFirstRow</param>
		/// <returns>DataTable with list</returns>
		static public DataTable forum_listall_fromCat( object boardID, object categoryID, bool emptyFirstRow )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listall_fromCat" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );

				int intCategoryID = Convert.ToInt32( categoryID.ToString() );

				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					return forum_sort_list( dt, 0, intCategoryID, 0, null, emptyFirstRow );
				}
			}
		}
		/// <summary>
		/// Sorry no idea what this does
		/// </summary>
		/// <param name="forumID"></param>
		/// <returns></returns>
		static public DataTable forum_listpath( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listpath" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists read topics
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="UserID">UserID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <param name="parentID">ParentID</param>
		/// <returns>DataTable with list</returns>
		static public DataTable forum_listread( object boardID, object userID, object categoryID, object parentID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_listread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				cmd.Parameters.AddWithValue( "ParentID", parentID );
				return DBAccess.GetData( cmd );
			}
		}

		/// <summary>
		/// Return admin view of Categories with Forums/Subforums ordered accordingly.
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="userID">UserID</param>
		/// <returns>DataSet with categories</returns>
		static public DataSet forum_moderatelist( object userID, object boardID )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				using ( DataSet ds = new DataSet() )
				{
					using ( SqlDataAdapter da = new SqlDataAdapter( DBAccess.GetObjectName( "category_list" ), connMan.OpenDBConnection ) )
					{
						using ( SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction( DBAccess.IsolationLevel ) )
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "BoardID", boardID );
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill( ds, DBAccess.GetObjectName( "Category" ) );
							da.SelectCommand.CommandText = DBAccess.GetObjectName( "forum_moderatelist" );
							da.SelectCommand.Parameters.AddWithValue( "UserID", userID );
							da.Fill( ds, DBAccess.GetObjectName( "ForumUnsorted" ) );
							DataTable dtForumListSorted = ds.Tables [DBAccess.GetObjectName( "ForumUnsorted" )].Clone();
							dtForumListSorted.TableName = DBAccess.GetObjectName( "Forum" );
							ds.Tables.Add( dtForumListSorted );
							dtForumListSorted.Dispose();
							forum_list_sort_basic( ds.Tables [DBAccess.GetObjectName( "ForumUnsorted" )], ds.Tables [DBAccess.GetObjectName( "Forum" )], 0, 0 );
							ds.Tables.Remove( DBAccess.GetObjectName( "ForumUnsorted" ) );
							ds.Relations.Add( "FK_Forum_Category", ds.Tables [DBAccess.GetObjectName( "Category" )].Columns ["CategoryID"], ds.Tables [DBAccess.GetObjectName( "Forum" )].Columns ["CategoryID"] );
							trans.Commit();
						}
						return ds;
					}
				}
			}
		}

		static public DataTable forum_moderators()
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_moderators" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return DBAccess.GetData( cmd );
			}
		}

		/// <summary>
		/// Updates topic and post count and last topic for all forums in specified board
		/// </summary>
		/// <param name="boardID">BoardID</param>
		static public void forum_resync( object boardID )
		{
			forum_resync( boardID, null );
		}
		/// <summary>
		/// Updates topic and post count and last topic for specified forum
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="forumID">If null, all forums in board are updated</param>
		static public void forum_resync( object boardID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_resync" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public long forum_save( object forumID, object categoryID, object parentID, object name, object description, object sortOrder, object locked, object hidden, object isTest, object moderated, object accessMaskID, object remoteURL, object themeURL, bool dummy )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				cmd.Parameters.AddWithValue( "ParentID", parentID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Description", description );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				cmd.Parameters.AddWithValue( "Locked", locked );
				cmd.Parameters.AddWithValue( "Hidden", hidden );
				cmd.Parameters.AddWithValue( "IsTest", isTest );
				cmd.Parameters.AddWithValue( "Moderated", moderated );
				cmd.Parameters.AddWithValue( "RemoteURL", remoteURL );
				cmd.Parameters.AddWithValue( "ThemeURL", themeURL );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return long.Parse( DBAccess.ExecuteScalar( cmd ).ToString() );
			}
		}

		static private void forum_list_sort_basic( DataTable listsource, DataTable list, int parentid, int currentLvl )
		{
			for ( int i = 0; i < listsource.Rows.Count; i++ )
			{
				DataRow row = listsource.Rows [i];
				if ( ( row ["ParentID"] ) == DBNull.Value )
					row ["ParentID"] = 0;

				if ( ( int )row ["ParentID"] == parentid )
				{
					string sIndent = "";
					int iIndent = Convert.ToInt32( currentLvl );
					for ( int j = 0; j < iIndent; j++ ) sIndent += "--";
					row ["Name"] = string.Format( " -{0} {1}", sIndent, row ["Name"] );
					list.Rows.Add( row.ItemArray );
					forum_list_sort_basic( listsource, list, ( int )row ["ForumID"], currentLvl + 1 );
				}
			}
		}

		static private void forum_sort_list_recursive( DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent )
		{
			DataRow newRow;

			foreach ( DataRow row in listSource.Rows )
			{
				// see if this is a root-forum
				if ( row ["ParentID"] == DBNull.Value )
					row ["ParentID"] = 0;

				if ( ( int )row ["ParentID"] == parentID )
				{
					if ( ( int )row ["CategoryID"] != categoryID )
					{
						categoryID = ( int )row ["CategoryID"];

						newRow = listDestination.NewRow();
						newRow ["ForumID"] = -categoryID;		// Ederon : 9/4/2007
						newRow ["Title"] = string.Format( "{0}", row ["Category"].ToString() );
						listDestination.Rows.Add( newRow );
					}

					string sIndent = "";

					for ( int j = 0; j < currentIndent; j++ )
						sIndent += "--";

					// import the row into the destination
					newRow = listDestination.NewRow();

					newRow ["ForumID"] = row ["ForumID"];
					newRow ["Title"] = string.Format( " -{0} {1}", sIndent, row ["Forum"] );

					listDestination.Rows.Add( newRow );

					// recurse through the list...
					forum_sort_list_recursive( listSource, listDestination, ( int )row ["ForumID"], categoryID, currentIndent + 1 );
				}
			}
		}

		static private DataTable forum_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent, int [] forumidExclusions )
		{
			return forum_sort_list( listSource, parentID, categoryID, startingIndent, forumidExclusions, true );
		}

		static private DataTable forum_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent, int [] forumidExclusions, bool emptyFirstRow )
		{
			DataTable listDestination = new DataTable();

			listDestination.Columns.Add( "ForumID", typeof( String ) );
			listDestination.Columns.Add( "Title", typeof( String ) );

			if ( emptyFirstRow )
			{
				DataRow blankRow = listDestination.NewRow();
				blankRow ["ForumID"] = string.Empty;
				blankRow ["Title"] = string.Empty;
				listDestination.Rows.Add( blankRow );
			}
			// filter the forum list -- not sure if this code actually works
			DataView dv = listSource.DefaultView;

			if ( forumidExclusions != null && forumidExclusions.Length > 0 )
			{
				string strExclusions = "";
				bool bFirst = true;

				foreach ( int forumID in forumidExclusions )
				{
					if ( bFirst ) bFirst = false;
					else strExclusions += ",";

					strExclusions += forumID.ToString();
				}

				dv.RowFilter = string.Format( "ForumID NOT IN ({0})", strExclusions );
				dv.ApplyDefaultSort = true;
			}

			forum_sort_list_recursive( dv.ToTable(), listDestination, parentID, categoryID, startingIndent );

			return listDestination;
		}

		static public DataTable forum_listall_sorted( object boardID, object userID )
		{
			return forum_listall_sorted( boardID, userID, null, false, 0 );
		}

		static public DataTable forum_listall_sorted( object boardID, object userID, int [] forumidExclusions )
		{
			return forum_listall_sorted( boardID, userID, null, false, 0 );
		}

		static public DataTable forum_listall_sorted( object boardID, object userID, int [] forumidExclusions, bool emptyFirstRow, int startAt )
		{
			using ( DataTable dt = forum_listall( boardID, userID, startAt ) )
			{
				return forum_sort_list( dt, 0, 0, 0, forumidExclusions, emptyFirstRow );
			}
		}

		#endregion

		#region yaf_ForumAccess
		static public DataTable forumaccess_list( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forumaccess_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void forumaccess_save( object forumID, object groupID, object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forumaccess_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable forumaccess_group( object groupID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "forumaccess_group" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				return userforumaccess_sort_list( DBAccess.GetData( cmd ), 0, 0, 0 );
			}
		}
		#endregion

		#region yaf_Group
		static public DataTable group_list( object boardID, object groupID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void group_delete( object groupID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable group_member( object boardID, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_member" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public long group_save( object groupID, object boardID, object name, object isAdmin, object isGuest, object isStart, object isModerator, object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "group_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "IsAdmin", isAdmin );
				cmd.Parameters.AddWithValue( "IsGuest", isGuest );
				cmd.Parameters.AddWithValue( "IsStart", isStart );
				cmd.Parameters.AddWithValue( "IsModerator", isModerator );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return long.Parse( DBAccess.ExecuteScalar( cmd ).ToString() );
			}
		}
		#endregion

		#region yaf_Mail
		static public void mail_delete( object mailID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "mail_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MailID", mailID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable mail_list()
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "mail_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return DBAccess.GetData( cmd );
			}
		}
		static public void mail_createwatch( object topicID, object from, object subject, object body, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "mail_createwatch" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "From", from );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void mail_create( object from, object to, object subject, object body )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "mail_create" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "From", from );
				cmd.Parameters.AddWithValue( "To", to );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Message
		static public DataTable post_list( object topicID, object updateViewCount, bool showDeleted )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "post_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "UpdateViewCount", updateViewCount );
				cmd.Parameters.AddWithValue( "ShowDeleted", showDeleted );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable post_list_reverse10( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "post_list_reverse10" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable post_last10user( object boardID, object userID, object pageUserID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "post_last10user" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "PageUserID", pageUserID );
				return DBAccess.GetData( cmd );
			}
		}

		// gets list of replies to message
		static public DataTable message_getRepliesList( object messageID )
		{
			DataTable list = new DataTable();
			list.Columns.Add( "Posted", typeof( DateTime ) );
			list.Columns.Add( "Subject", typeof( string ) );
			list.Columns.Add( "Message", typeof( string ) );
			list.Columns.Add( "UserID", typeof( int ) );
			list.Columns.Add( "Flags", typeof( int ) );
			list.Columns.Add( "UserName", typeof( string ) );
			list.Columns.Add( "Signature", typeof( string ) );

			using ( SqlCommand cmd = DBAccess.GetCommand( "message_reply_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable dtr = DBAccess.GetData( cmd );

				for ( int i = 0; i < dtr.Rows.Count; i++ )
				{
					DataRow newRow = list.NewRow();
					DataRow row = dtr.Rows [i];
					newRow = list.NewRow();
					newRow ["Posted"] = row ["Posted"];
					newRow ["Subject"] = row ["Subject"];
					newRow ["Message"] = row ["Message"];
					newRow ["UserID"] = row ["UserID"];
					newRow ["Flags"] = row ["Flags"];
					newRow ["UserName"] = row ["UserName"];
					newRow ["Signature"] = row ["Signature"];
					list.Rows.Add( newRow );
					message_getRepliesList_populate( dtr, list, ( int )row ["MessageId"] );
				}
				return list;
			}
		}

		// gets list of nested replies to message
		static private void message_getRepliesList_populate( DataTable listsource, DataTable list, int messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_reply_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable dtr = DBAccess.GetData( cmd );

				for ( int i = 0; i < dtr.Rows.Count; i++ )
				{
					DataRow newRow = list.NewRow();
					DataRow row = dtr.Rows [i];
					newRow = list.NewRow();
					newRow ["Posted"] = row ["Posted"];
					newRow ["Subject"] = row ["Subject"];
					newRow ["Message"] = row ["Message"];
					newRow ["UserID"] = row ["UserID"];
					newRow ["Flags"] = row ["Flags"];
					newRow ["UserName"] = row ["UserName"];
					newRow ["Signature"] = row ["Signature"];
					list.Rows.Add( newRow );
					message_getRepliesList_populate( dtr, list, ( int )row ["MessageId"] );
				}
			}

		}

		//creates new topic, using some parameters from message itself
		static public long topic_create_by_message( object messageID, object forumId, object newTopicSubj )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_create_by_message" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "ForumID", forumId );
				cmd.Parameters.AddWithValue( "Subject", newTopicSubj );
				DataTable dt = DBAccess.GetData( cmd );
				return long.Parse( dt.Rows [0] ["TopicID"].ToString() );
			}
		}

		static public DataTable message_list( object messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				return DBAccess.GetData( cmd );
			}
		}

		static public void message_delete( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked )
		{
			message_deleteRecursively( messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false );
		}

		// <summary> Retrieve all reported messages with the correct forumID argument. </summary>
		static public DataTable message_listreported( object messageFlag, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_listreported" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "MessageFlag", messageFlag );
				return DBAccess.GetData( cmd );
			}
		}

		// <summary> Save reported message back to the database. </summary>
		static public void message_report( object reportFlag, object messageID, object userID, object reportedDateTime )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_report" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ReportFlag", reportFlag );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "ReporterID", userID );
				cmd.Parameters.AddWithValue( "ReportedDate", reportedDateTime );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		// <summary> Copy current Message text over reported Message text. </summary>
		static public void message_reportcopyover( object messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_reportcopyover" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		// <summary> Copy current Message text over reported Message text. </summary>
		static public void message_reportresolve( object messageFlag, object messageID, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_reportresolve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageFlag", messageFlag );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		//BAI ADDED 30.01.2004
		// <summary> Delete message and all subsequent releated messages to that ID </summary>
		static private void message_deleteRecursively( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked, bool isLinked )
		{
			bool UseFileTable = false;

			using ( DataTable dt = DB.registry_list( "UseFileTable" ) )
				foreach ( DataRow dr in dt.Rows )
					UseFileTable = Convert.ToBoolean( Convert.ToInt32( dr ["Value"] ) );


			if ( DeleteLinked )
				//Delete replies
				using ( SqlCommand cmd = DBAccess.GetCommand( "message_getReplies" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbReplies = DBAccess.GetData( cmd );

					foreach ( DataRow row in tbReplies.Rows )
						message_deleteRecursively( row ["MessageID"], isModeratorChanged, isLinked ? deleteReason : deleteReason + " + удалено, т.к. является ответом на удаленное сообщение", isDeleteAction, DeleteLinked, true );
				}

			//ABOT CHANGED 16.01.04: Delete files from hard disk
			//If the files are actually saved in the Hard Drive
			if ( !UseFileTable )
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbAttachments = DBAccess.GetData( cmd );
					string sUpDir = HttpContext.Current.Server.MapPath( Config.UploadDir );
					foreach ( DataRow row in tbAttachments.Rows )
						System.IO.File.Delete( String.Format( "{0}{1}.{2}", sUpDir, messageID, row ["FileName"] ) );
				}

			}
			//END ABOT CHANGE 16.04.04

			//Delete Message
			// undelete function added
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_deleteundelete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "isModeratorChanged", isModeratorChanged );
				cmd.Parameters.AddWithValue( "DeleteReason", deleteReason );
				cmd.Parameters.AddWithValue( "isDeleteAction", isDeleteAction );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		// <summary> Set flag on message to approved and store in DB </summary>
		static public void message_approve( object messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Get message topic IDs (for URL rewriting)
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable message_simplelist( int StartID, int Limit )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return DBAccess.GetData( cmd );
			}
		}

		// <summary> Update message to DB. </summary>
		static public void message_update( object messageID, object priority, object message, object subject, object flags, object reasonOfEdit, object isModeratorChanged )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "Priority", priority );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Flags", flags );
				cmd.Parameters.AddWithValue( "Reason", reasonOfEdit );
				cmd.Parameters.AddWithValue( "IsModeratorChanged", isModeratorChanged );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		// <summary> Save message to DB. </summary>
		static public bool message_save( object topicID, object userID, object message, object userName, object ip, object posted, object replyTo, object flags, ref long messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_save" ) )
			{
				SqlParameter paramMessageID = new SqlParameter( "MessageID", messageID );
				paramMessageID.Direction = ParameterDirection.Output;

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "IP", ip );
				cmd.Parameters.AddWithValue( "Posted", posted );
				cmd.Parameters.AddWithValue( "ReplyTo", replyTo );
				cmd.Parameters.AddWithValue( "BlogPostID", null );		// Ederon : 6/16/2007
				cmd.Parameters.AddWithValue( "Flags", flags );
				cmd.Parameters.Add( paramMessageID );
				DBAccess.ExecuteNonQuery( cmd );
				messageID = ( long )paramMessageID.Value;
				return true;
			}
		}
		static public DataTable message_unapproved( object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_unapproved" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable message_findunread( object topicID, object lastRead )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_findunread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "LastRead", lastRead );
				return DBAccess.GetData( cmd );
			}
		}

		// message movind function
		static public void message_move( object messageID, object moveToTopic, bool moveAll )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_move" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "MoveToTopic", moveToTopic );
				DBAccess.ExecuteNonQuery( cmd );
			}
			//moveAll=true anyway
			// it's in charge of moving answers of moved post
			if ( moveAll )
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "message_getReplies" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbReplies = DBAccess.GetData( cmd );
					foreach ( DataRow row in tbReplies.Rows )
					{
						message_moveRecursively( row ["MessageID"], moveToTopic );
					}

				}
			}
		}

		//moves answers of moved post
		static private void message_moveRecursively( object messageID, object moveToTopic )
		{
			bool UseFileTable = false;

			using ( DataTable dt = DB.registry_list( "UseFileTable" ) )
				foreach ( DataRow dr in dt.Rows )
					UseFileTable = Convert.ToBoolean( Convert.ToInt32( dr ["Value"] ) );

			//Delete replies
			using ( SqlCommand cmd = DBAccess.GetCommand( "message_getReplies" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable tbReplies = DBAccess.GetData( cmd );
				foreach ( DataRow row in tbReplies.Rows )
				{
					message_moveRecursively( row ["messageID"], moveToTopic );
				}
				using ( SqlCommand innercmd = DBAccess.GetCommand( "message_move" ) )
				{
					innercmd.CommandType = CommandType.StoredProcedure;
					innercmd.Parameters.AddWithValue( "MessageID", messageID );
					innercmd.Parameters.AddWithValue( "MoveToTopic", moveToTopic );
					DBAccess.ExecuteNonQuery( innercmd );
				}
			}
		}


		#endregion

		#region yaf_NntpForum
		static public DataTable nntpforum_list( object boardID, object minutes, object nntpForumID, object active )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Minutes", minutes );
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "Active", active );
				return DBAccess.GetData( cmd );
			}
		}
		static public void nntpforum_update( object nntpForumID, object lastMessageNo, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpforum_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "LastMessageNo", lastMessageNo );
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_save( object nntpForumID, object nntpServerID, object groupName, object forumID, object active )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "GroupName", groupName );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Active", active );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_delete( object nntpForumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_NntpServer
		static public DataTable nntpserver_list( object boardID, object nntpServerID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpserver_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void nntpserver_save( object nntpServerID, object boardID, object name, object address, object port, object userName, object userPass )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpserver_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Address", address );
				cmd.Parameters.AddWithValue( "Port", port );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "UserPass", userPass );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpserver_delete( object nntpServerID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntpserver_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_NntpTopic
		static public DataTable nntptopic_list( object thread )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntptopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Thread", thread );
				return DBAccess.GetData( cmd );
			}
		}
		static public void nntptopic_savemessage( object nntpForumID, object topic, object body, object userID, object userName, object ip, object posted, object thread )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "nntptopic_savemessage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "Topic", topic );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "IP", ip );
				cmd.Parameters.AddWithValue( "Posted", posted );
				cmd.Parameters.AddWithValue( "Thread", thread );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_PMessage
		/// <summary>
		/// Returns a list of private messages based on the arguments specified.
		/// If pMessageID != null, returns the PM of id pMessageId.
		/// If toUserID != null, returns the list of PMs sent to the user with the given ID.
		/// If fromUserID != null, returns the list of PMs sent by the user of the given ID.
		/// </summary>
		/// <param name="toUserID"></param>
		/// <param name="fromUserID"></param>
		/// <param name="pMessageID">The id of the private message</param>
		/// <returns></returns>
		static public DataTable pmessage_list( object toUserID, object fromUserID, object pMessageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "PMessageID", pMessageID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Returns a list of private messages based on the arguments specified.
		/// If pMessageID != null, returns the PM of id pMessageId.
		/// If toUserID != null, returns the list of PMs sent to the user with the given ID.
		/// If fromUserID != null, returns the list of PMs sent by the user of the given ID.
		/// </summary>
		/// <param name="toUserID"></param>
		/// <param name="fromUserID"></param>
		/// <param name="pMessageID">The id of the private message</param>
		/// <returns></returns>
		static public DataTable pmessage_list( object pMessageID )
		{
			return pmessage_list( null, null, pMessageID );
		}
		/// <summary>
		/// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
		/// the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
		/// </summary>
		/// <param name="pMessageID"></param>
		/// <param name="fromOutbox">If true, removes the message from the outbox.  Otherwise deletes the message completely.</param>
		static public void pmessage_delete( object pMessageID, bool fromOutbox )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PMessageID", pMessageID );
				cmd.Parameters.AddWithValue( "FromOutbox", fromOutbox );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes the private message from the database as per the given parameter.  If fromOutbox is true,
		/// the message is only deleted from the user's outbox.  Otherwise, it is completely delete from the database.
		/// </summary>
		/// <param name="userPMessageID"></param>
		static public void pmessage_delete( object userPMessageID )
		{
			pmessage_delete( userPMessageID, false );
		}

		/// <summary>
		/// Archives the private message of the given id.  Archiving moves the message from the user's inbox to his message archive.
		/// </summary>
		/// <param name="pMessageID">The ID of the private message</param>
		public static void pmessage_archive( object pMessageID )
		{
			using ( SqlCommand sqlCommand = DBAccess.GetCommand( "pmessage_archive" ) )
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue( "PMessageID", pMessageID );
				DBAccess.ExecuteNonQuery( sqlCommand );
			}
		}

		static public void pmessage_save( object fromUserID, object toUserID, object subject, object body, object Flags )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "Flags", Flags );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void pmessage_markread( object userPMessageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_markread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserPMessageID", userPMessageID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable pmessage_info()
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return DBAccess.GetData( cmd );
			}
		}
		static public void pmessage_prune( object daysRead, object daysUnread )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "pmessage_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "DaysRead", daysRead );
				cmd.Parameters.AddWithValue( "DaysUnread", daysUnread );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Poll
		static public DataTable poll_stats( object pollID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "poll_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollID );
				return DBAccess.GetData( cmd );
			}
		}
		static public int poll_save( object question, object c1, object c2, object c3, object c4, object c5, object c6, object c7, object c8, object c9, object c10 )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "poll_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Question", question );
				cmd.Parameters.AddWithValue( "Choice1", c1 );
				cmd.Parameters.AddWithValue( "Choice2", c2 );
				cmd.Parameters.AddWithValue( "Choice3", c3 );
				cmd.Parameters.AddWithValue( "Choice4", c4 );
				cmd.Parameters.AddWithValue( "Choice5", c5 );
				cmd.Parameters.AddWithValue( "Choice6", c6 );
				cmd.Parameters.AddWithValue( "Choice7", c7 );
				cmd.Parameters.AddWithValue( "Choice8", c8 );
				cmd.Parameters.AddWithValue( "Choice9", c9 );
				cmd.Parameters.AddWithValue( "Closes", c10 );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}
		#endregion

		#region yaf_Rank
		static public DataTable rank_list( object boardID, object rankID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "rank_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void rank_save( object rankID, object boardID, object name, object isStart, object isLadder, object minPosts, object rankImage )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "rank_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "RankID", rankID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "IsStart", isStart );
				cmd.Parameters.AddWithValue( "IsLadder", isLadder );
				cmd.Parameters.AddWithValue( "MinPosts", minPosts );
				cmd.Parameters.AddWithValue( "RankImage", rankImage );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void rank_delete( object rankID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "rank_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "RankID", rankID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Smiley
		static public DataTable smiley_list( object boardID, object smileyID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "smiley_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable smiley_listunique( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "smiley_listunique" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void smiley_delete( object smileyID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "smiley_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				DBAccess.ExecuteNonQuery( cmd );
				// todo : move this away to non-static code
				System.Web.HttpContext.Current.Cache.Remove( "Smilies" );
			}
		}
		static public void smiley_save( object smileyID, object boardID, object code, object icon, object emoticon, object sortOrder, object replace )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "smiley_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Code", code );
				cmd.Parameters.AddWithValue( "Icon", icon );
				cmd.Parameters.AddWithValue( "Emoticon", emoticon );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				cmd.Parameters.AddWithValue( "Replace", replace );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void smiley_resort( object boardID, object smileyID, int move )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "smiley_resort" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				cmd.Parameters.AddWithValue( "Move", move );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_BBCode
		static public DataTable bbcode_list( object boardID, object bbcodeID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bbcode_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "BBCodeID", bbcodeID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void bbcode_delete( object bbcodeID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bbcode_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BBCodeID", bbcodeID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void bbcode_save( object bbcodeID, object boardID, object name, object description, object onclickjs, object displayjs, object editjs, object displaycss, object searchregex, object replaceregex, object variables, object execorder )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "bbcode_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BBCodeID", bbcodeID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Description", description );
				cmd.Parameters.AddWithValue( "OnClickJS", onclickjs );
				cmd.Parameters.AddWithValue( "DisplayJS", displayjs );
				cmd.Parameters.AddWithValue( "EditJS", editjs );
				cmd.Parameters.AddWithValue( "DisplayCSS", displaycss );
				cmd.Parameters.AddWithValue( "SearchRegEx", searchregex );
				cmd.Parameters.AddWithValue( "ReplaceRegEx", replaceregex );
				cmd.Parameters.AddWithValue( "Variables", variables );
				cmd.Parameters.AddWithValue( "ExecOrder", execorder );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Registry
		/// <summary>
		/// Retrieves entries in the board settings registry
		/// </summary>
		/// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
		/// <returns>DataTable filled will registry entries</returns>
		static public DataTable registry_list( object name, object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "registry_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Retrieves entries in the board settings registry
		/// </summary>
		/// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
		/// <returns>DataTable filled will registry entries</returns>
		static public DataTable registry_list( object name )
		{
			return registry_list( name, null );
		}
		/// <summary>
		/// Retrieves all the entries in the board settings registry
		/// </summary>
		/// <returns>DataTable filled will all registry entries</returns>
		static public DataTable registry_list()
		{
			return registry_list( null, null );
		}
		/// <summary>
		/// Saves a single registry entry pair to the database.
		/// </summary>
		/// <param name="Name">Unique name associated with this entry</param>
		/// <param name="Value">Value associated with this entry which can be null</param>
		static public void registry_save( object name, object value )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Value", value );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Saves a single registry entry pair to the database.
		/// </summary>
		/// <param name="Name">Unique name associated with this entry</param>
		/// <param name="Value">Value associated with this entry which can be null</param>
		/// <param name="BoardID">The BoardID for this entry</param>
		static public void registry_save( object name, object value, object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Value", value );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_System
		/// <summary>
		/// Not in use anymore. Only required for old database versions.
		/// </summary>
		/// <returns></returns>
		static public DataTable system_list()
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "system_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return DBAccess.GetData( cmd );
			}
		}
		#endregion

		#region yaf_Topic
		static public int topic_prune( object forumID, object days )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Days", days );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}

		static public DataTable topic_list( object forumID, object announcement, object date, object offset, object count )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Announcement", announcement );
				cmd.Parameters.AddWithValue( "Date", date );
				cmd.Parameters.AddWithValue( "Offset", offset );
				cmd.Parameters.AddWithValue( "Count", count );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists topics very simply (for URL rewriting)
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable topic_simplelist( int StartID, int Limit )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return DBAccess.GetData( cmd );
			}
		}
		static public void topic_move( object topicID, object forumID, object showMoved )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_move" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "ShowMoved", showMoved );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public DataTable topic_announcements( object boardID, object numOfPostsToRetrieve, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_announcements" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NumPosts", numOfPostsToRetrieve );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}

		static public DataTable topic_latest( object boardID, object numOfPostsToRetrieve, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_latest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NumPosts", numOfPostsToRetrieve );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable topic_active( object boardID, object userID, object since, object categoryID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_active" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Since", since );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return DBAccess.GetData( cmd );
			}
		}
		//ABOT NEW 16.04.04:Delete all topic's messages
		static private void topic_deleteAttachments( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_listmessages" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						message_deleteRecursively( row ["MessageID"], true, "", 0, true, false );
					}
				}
			}
		}
		//END ABOT NEW
		static public void topic_delete( object topicID )
		{
			//ABOT CHANGE 16.04.04
			topic_deleteAttachments( topicID );
			//END ABOT CHANGE 16.04.04
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable topic_findprev( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_findprev" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable topic_findnext( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_findnext" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void topic_lock( object topicID, object locked )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_lock" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "Locked", locked );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public long topic_save( object forumID, object subject, object message, object userID, object priority, object pollID, object userName, object ip, object posted, object blogPostID, object flags, ref long messageID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Priority", priority );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "IP", ip );
				cmd.Parameters.AddWithValue( "PollID", pollID );
				cmd.Parameters.AddWithValue( "Posted", posted );
				cmd.Parameters.AddWithValue( "BlogPostID", blogPostID );
				cmd.Parameters.AddWithValue( "Flags", flags );

				DataTable dt = DBAccess.GetData( cmd );
				messageID = long.Parse( dt.Rows [0] ["MessageID"].ToString() );
				return long.Parse( dt.Rows [0] ["TopicID"].ToString() );
			}
		}
		static public DataRow topic_info( object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "topic_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				using ( DataTable dt = DBAccess.GetData( cmd ) )
				{
					if ( dt.Rows.Count > 0 )
						return dt.Rows [0];
					else
						return null;
				}
			}
		}
		#endregion

		#region yaf_ReplaceWords
		// rico : replace words / begin
		/// <summary>
		/// Gets a list of replace words
		/// </summary>
		/// <returns>DataTable with replace words</returns>
		static public DataTable replace_words_list()
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "replace_words_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// Saves changs to a words
		/// </summary>
		/// <param name="ID">ID of bad/good word</param>
		/// <param name="badword">bad word</param>
		/// <param name="goodword">good word</param>
		static public void replace_words_save( object ID, object badword, object goodword )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "replace_words_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				cmd.Parameters.AddWithValue( "badword", badword );
				cmd.Parameters.AddWithValue( "goodword", goodword );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes a bad/good word
		/// </summary>
		/// <param name="ID">ID of bad/good word to delete</param>
		static public void replace_words_delete( object ID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "replace_words_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Edits bad words?
		/// </summary>
		/// <param name="ID">ID of badword</param>
		/// <returns>DataTable</returns>
		static public DataTable replace_words_edit( object ID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "replace_words_edit" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				return DBAccess.GetData( cmd );
			}
		}
		// rico : replace words / end
		#endregion

		#region yaf_User
		static public DataTable user_list( object boardID, object userID, object approved )
		{
			return user_list( boardID, userID, approved, null, null );
		}
		static public DataTable user_list( object boardID, object userID, object approved, object groupID, object rankID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Approved", approved );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				return DBAccess.GetData( cmd );
			}
		}
		/// <summary>
		/// For URL Rewriting
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable user_simplelist( int StartID, int Limit )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return DBAccess.GetData( cmd );
			}
		}
		static public void user_delete( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_setrole( int boardID, object providerUserKey, object role )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_setrole" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "Role", role );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_setinfo( int boardID, System.Web.Security.MembershipUser user )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true ) )
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue( "UserName", user.UserName );
				cmd.Parameters.AddWithValue( "Email", user.Email );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", user.ProviderUserKey );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_migrate( object userID, object providerUserKey, object updateProvider )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_migrate" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "UpdateProvider", updateProvider );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_deleteold( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_deleteold" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_approve( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_approveall( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_approveall" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_suspend( object userID, object suspend )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_suspend" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Suspend", suspend );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public bool user_changepassword( object userID, object oldPassword, object newPassword )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_changepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "OldPassword", oldPassword );
				cmd.Parameters.AddWithValue( "NewPassword", newPassword );
				return ( bool )DBAccess.ExecuteScalar( cmd );
			}
		}

		static public void user_save( object userID, object boardID, object userName, object email,
			object timeZone, object languageFile, object themeFile, object overrideDefaultThemes, object approved,
			object pmNotification )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				cmd.Parameters.AddWithValue( "TimeZone", timeZone );
				cmd.Parameters.AddWithValue( "LanguageFile", languageFile );
				cmd.Parameters.AddWithValue( "ThemeFile", themeFile );
				cmd.Parameters.AddWithValue( "OverrideDefaultTheme", overrideDefaultThemes );
				cmd.Parameters.AddWithValue( "Approved", approved );
				cmd.Parameters.AddWithValue( "PMNotification", pmNotification );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_adminsave( object boardID, object userID, object name, object email, object isHostAdmin, object isGuest, object rankID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_adminsave" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "IsGuest", isGuest );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Email", email );
				cmd.Parameters.AddWithValue( "IsHostAdmin", isHostAdmin );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable user_emails( object boardID, object groupID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_emails" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );

				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable user_accessmasks( object boardID, object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_accessmasks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );

				return userforumaccess_sort_list( DBAccess.GetData( cmd ), 0, 0, 0 );
			}
		}

		//adds some convenience while editing group's access rights (indent forums)
		static private DataTable userforumaccess_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent )
		{

			DataTable listDestination = new DataTable();

			listDestination.Columns.Add( "ForumID", typeof( String ) );
			listDestination.Columns.Add( "ForumName", typeof( String ) );
			//it is uset in two different procedures with different tables, 
			//so, we must add correct columns
			if ( listSource.Columns.IndexOf( "AccessMaskName" ) >= 0 )
				listDestination.Columns.Add( "AccessMaskName", typeof( String ) );
			else
			{
				listDestination.Columns.Add( "CategoryName", typeof( String ) );
				listDestination.Columns.Add( "AccessMaskId", typeof( Int32 ) );
			}
			DataView dv = listSource.DefaultView;
			userforumaccess_sort_list_recursive( dv.ToTable(), listDestination, parentID, categoryID, startingIndent );
			return listDestination;
		}

		static private void userforumaccess_sort_list_recursive( DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent )
		{
			DataRow newRow;

			foreach ( DataRow row in listSource.Rows )
			{
				// see if this is a root-forum
				if ( row ["ParentID"] == DBNull.Value )
					row ["ParentID"] = 0;

				if ( ( int )row ["ParentID"] == parentID )
				{
					string sIndent = "";

					for ( int j = 0; j < currentIndent; j++ )
						sIndent += "--";

					// import the row into the destination
					newRow = listDestination.NewRow();

					newRow ["ForumID"] = row ["ForumID"];
					newRow ["ForumName"] = string.Format( "{0} {1}", sIndent, row ["ForumName"] );
					if ( listDestination.Columns.IndexOf( "AccessMaskName" ) >= 0 )
						newRow ["AccessMaskName"] = row ["AccessMaskName"];
					else
					{
						newRow ["CategoryName"] = row ["CategoryName"];
						newRow ["AccessMaskId"] = row ["AccessMaskId"];
					}


					listDestination.Rows.Add( newRow );

					// recurse through the list...
					userforumaccess_sort_list_recursive( listSource, listDestination, ( int )row ["ForumID"], categoryID, currentIndent + 1 );
				}
			}
		}

		static public object user_recoverpassword( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_recoverpassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return DBAccess.ExecuteScalar( cmd );
			}
		}
		static public void user_savepassword( object userID, object password )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_savepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Password", FormsAuthentication.HashPasswordForStoringInConfigFile( password.ToString(), "md5" ) );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public object user_login( object boardID, object name, object password )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_login" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Password", password );
				return DBAccess.ExecuteScalar( cmd );
			}
		}
		static public DataTable user_avatarimage( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_avatarimage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public int user_get( int boardID, object providerUserKey )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true ) )
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}
		static public DataTable user_find( object boardID, bool filter, object userName, object email )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_find" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Filter", filter );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return DBAccess.GetData( cmd );
			}
		}
		static public string user_getsignature( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_getsignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.ExecuteScalar( cmd ).ToString();
			}
		}
		static public void user_savesignature( object userID, object signature )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_savesignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Signature", signature );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_saveavatar( object userID, object avatar, System.IO.Stream stream )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_saveavatar" ) )
			{
				byte [] data = null;

				if ( stream != null )
				{
					data = new byte [stream.Length];
					stream.Seek( 0, System.IO.SeekOrigin.Begin );
					stream.Read( data, 0, ( int )stream.Length );
				}

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Avatar", avatar );
				cmd.Parameters.AddWithValue( "AvatarImage", data );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void user_deleteavatar( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_deleteavatar" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public bool user_register( object boardID, object userName, object password, object hash, object email, object location, object homePage, object timeZone, bool approved )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( DBAccess.IsolationLevel ) )
				{
					try
					{
						using ( SqlCommand cmd = DBAccess.GetCommand( "user_save", connMan.DBConnection ) )
						{
							cmd.Transaction = trans;
							cmd.CommandType = CommandType.StoredProcedure;
							int UserID = 0;
							cmd.Parameters.AddWithValue( "UserID", UserID );
							cmd.Parameters.AddWithValue( "BoardID", boardID );
							cmd.Parameters.AddWithValue( "UserName", userName );
							cmd.Parameters.AddWithValue( "Password", FormsAuthentication.HashPasswordForStoringInConfigFile( password.ToString(), "md5" ) );
							cmd.Parameters.AddWithValue( "Email", email );
							cmd.Parameters.AddWithValue( "Hash", hash );
							cmd.Parameters.AddWithValue( "Location", location );
							cmd.Parameters.AddWithValue( "HomePage", homePage );
							cmd.Parameters.AddWithValue( "TimeZone", timeZone );
							cmd.Parameters.AddWithValue( "Approved", approved );
							cmd.Parameters.AddWithValue( "PMNotification", 1 );
							cmd.ExecuteNonQuery();
						}

						trans.Commit();
					}
					catch ( Exception x )
					{
						trans.Rollback();
						YAF.Classes.Data.DB.eventlog_create( null, "user_register in YAF.Classes.Data.DB.cs", x, EventLogTypes.Error );
						return false;
					}
				}
			}

			return true;
		}
		static public int user_aspnet( int boardID, string userName, string email, object providerUserKey, object isApproved )
		{
			try
			{
				using ( SqlCommand cmd = DBAccess.GetCommand( "user_aspnet" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue( "BoardID", boardID );
					cmd.Parameters.AddWithValue( "UserName", userName );
					cmd.Parameters.AddWithValue( "Email", email );
					cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
					cmd.Parameters.AddWithValue( "IsApproved", isApproved );
					return ( int )DBAccess.ExecuteScalar( cmd );
				}
			}
			catch ( Exception x )
			{
				YAF.Classes.Data.DB.eventlog_create( null, "user_aspnet in YAF.Classes.Data.DB.cs", x, EventLogTypes.Error );
				return 0;
			}
		}
		static public int user_guest( object boardID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_guest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}
		static public DataTable user_activity_rank( object boardID, object startDate, object displayNumber )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_activity_rank" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "StartDate", startDate );
				cmd.Parameters.AddWithValue( "DisplayNumber", displayNumber );
				return DBAccess.GetData( cmd );
			}
		}
		static public int user_nntp( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_nntp" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}

		static public void user_addpoints( object userID, object points )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_addpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public void user_removepointsByTopicID( object topicID, object points )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_removepointsbytopicid" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "Points", points );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public void user_removepoints( object userID, object points )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_removepoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public void user_setpoints( object userID, object points )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_setpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}

		static public int user_getpoints( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "user_getpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return ( int )DBAccess.ExecuteScalar( cmd );
			}
		}
		#endregion

		#region yaf_UserForum
		static public DataTable userforum_list( object userID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "userforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void userforum_delete( object userID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "userforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void userforum_save( object userID, object forumID, object accessMaskID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "userforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_UserGroup
		static public DataTable usergroup_list( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "usergroup_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void usergroup_save( object userID, object groupID, object member )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "usergroup_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "Member", member );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_WatchForum
		static public void watchforum_add( object userID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchforum_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable watchforum_list( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable watchforum_check( object userID, object forumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchforum_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void watchforum_delete( object watchForumID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "WatchForumID", watchForumID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_WatchTopic
		static public DataTable watchtopic_list( object userID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchtopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return DBAccess.GetData( cmd );
			}
		}
		static public DataTable watchtopic_check( object userID, object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchtopic_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return DBAccess.GetData( cmd );
			}
		}
		static public void watchtopic_delete( object watchTopicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchtopic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "WatchTopicID", watchTopicID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		static public void watchtopic_add( object userID, object topicID )
		{
			using ( SqlCommand cmd = DBAccess.GetCommand( "watchtopic_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				DBAccess.ExecuteNonQuery( cmd );
			}
		}
		#endregion
	}

}
