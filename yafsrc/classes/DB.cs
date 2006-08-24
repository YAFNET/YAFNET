/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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

namespace yaf
{
	#region QueryCounter class
	public sealed class QueryCounter : System.IDisposable
	{
#if DEBUG
		private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		private string m_cmd;
#endif

		public QueryCounter( string sql )
		{
#if DEBUG
			m_cmd = sql;

			if ( HttpContext.Current.Items ["NumQueries"] == null )
				HttpContext.Current.Items ["NumQueries"] = ( int ) 1;
			else
				HttpContext.Current.Items ["NumQueries"] = 1 + ( int ) HttpContext.Current.Items ["NumQueries"];

			stopWatch.Start();
#endif
		}

		public void Dispose()
		{
#if DEBUG
			stopWatch.Stop();

			double duration = ( double ) stopWatch.ElapsedMilliseconds / 1000.0;

			m_cmd = String.Format( "{0}: {1:N3}", m_cmd, duration );

			if ( HttpContext.Current.Items ["TimeQueries"] == null )
				HttpContext.Current.Items ["TimeQueries"] = duration;
			else
				HttpContext.Current.Items ["TimeQueries"] = duration + ( double ) HttpContext.Current.Items ["TimeQueries"];

			if ( HttpContext.Current.Items ["CmdQueries"] == null )
				HttpContext.Current.Items ["CmdQueries"] = m_cmd;
			else
				HttpContext.Current.Items ["CmdQueries"] += "<br/>" + m_cmd;
#endif
		}

#if DEBUG
		static public void Reset()
		{
			HttpContext.Current.Items ["NumQueries"] = 0;
			HttpContext.Current.Items ["TimeQueries"] = ( double ) 0;
			HttpContext.Current.Items ["CmdQueries"] = "";
		}

		static public int Count
		{
			get
			{
				return ( int ) HttpContext.Current.Items ["NumQueries"];
			}
		}
		static public double Duration
		{
			get
			{
				return ( double ) HttpContext.Current.Items ["TimeQueries"];
			}
		}
		static public string Commands
		{
			get
			{
				return ( string ) HttpContext.Current.Items ["CmdQueries"];
			}
		}
#endif
	}
	#endregion

	public class DB
	{
		private static IsolationLevel m_isoLevel = IsolationLevel.ReadUncommitted;

		private DB()
		{
		}
		#region DB Access Functions
		static public IsolationLevel IsolationLevel
		{
			get
			{
				return m_isoLevel;
			}
		}

		/// <summary>
		/// Gets Connection out of Web.config
		/// </summary>
		/// <returns>Returns SqlConnection</returns>
		public static SqlConnection GetConnection()
		{
			SqlConnection conn = new SqlConnection(Config.ConnectionString );
			conn.Open();
			return conn;
		}
		/// <summary>
		/// Gets data out of the database
		/// </summary>
		/// <param name="cmd">The SQL Command</param>
		/// <returns>DataTable with the results</returns>
		static private DataTable GetData( SqlCommand cmd )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				if ( cmd.Connection != null )
				{
					using ( DataSet ds = new DataSet() )
					{
						using ( SqlDataAdapter da = new SqlDataAdapter() )
						{
							da.SelectCommand = cmd;
							da.Fill( ds );
							return ds.Tables [0];
						}
					}
				}
				else
				{
					using ( SqlConnection conn = GetConnection() )
					{
						using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
						{
							try
							{
								cmd.Transaction = trans;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
										da.SelectCommand.Connection = conn;
										da.Fill( ds );
										return ds.Tables [0];
									}
								}
							}
							finally
							{
								trans.Commit();
							}
						}
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}
		/// <summary>
		/// Gets data out of database using a plain text string command
		/// </summary>
		/// <param name="sql">string command to be executed</param>
		/// <returns>DataTable with results</returns>
		static public DataTable GetData( string sql )
		{
			QueryCounter qc = new QueryCounter( sql );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						try
						{
							using ( SqlCommand cmd = conn.CreateCommand() )
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = sql;
								using ( DataSet ds = new DataSet() )
								{
									using ( SqlDataAdapter da = new SqlDataAdapter() )
									{
										da.SelectCommand = cmd;
										da.SelectCommand.Connection = conn;
										da.Fill( ds );
										return ds.Tables [0];
									}
								}
							}
						}
						finally
						{
							trans.Commit();
						}
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}
		/// <summary>
		/// Executes a NonQuery
		/// </summary>
		/// <param name="cmd">NonQuery to execute</param>
		static public void ExecuteNonQuery( SqlCommand cmd )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						cmd.Connection = conn;
						cmd.Transaction = trans;
						cmd.ExecuteNonQuery();
						trans.Commit();
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}


		static public object ExecuteScalar( SqlCommand cmd )
		{
			QueryCounter qc = new QueryCounter( cmd.CommandText );
			try
			{
				using ( SqlConnection conn = GetConnection() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						cmd.Connection = conn;
						cmd.Transaction = trans;
						object res = cmd.ExecuteScalar();
						trans.Commit();
						return res;
					}
				}
			}
			finally
			{
				qc.Dispose();
			}
		}
		/// <summary>
		/// Gets the database size
		/// </summary>
		/// <returns>intager value for database size</returns>
		static public int DBSize()
		{
			using ( SqlCommand cmd = new SqlCommand( "select sum(cast(size as integer))/128 from sysfiles" ) )
			{
				cmd.CommandType = CommandType.Text;
				return ( int ) ExecuteScalar( cmd );
			}
		}
		#endregion

		#region Forum

		static public DataRow pageload( object SessionID, object boardID, object User, object IP, object Location, object Browser,
			object Platform, object CategoryID, object ForumID, object TopicID, object MessageID )
		{
			int nTries = 0;
			while ( true )
			{
				try
				{
					using ( SqlCommand cmd = new SqlCommand( "yaf_pageload" ) )
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue( "@SessionID", SessionID );
						cmd.Parameters.AddWithValue( "@BoardID", boardID );
						cmd.Parameters.AddWithValue( "@User", User );
						cmd.Parameters.AddWithValue( "@IP", IP );
						cmd.Parameters.AddWithValue( "@Location", Location );
						cmd.Parameters.AddWithValue( "@Browser", Browser );
						cmd.Parameters.AddWithValue( "@Platform", Platform );
						cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );
						cmd.Parameters.AddWithValue( "@ForumID", ForumID );
						cmd.Parameters.AddWithValue( "@TopicID", TopicID );
						cmd.Parameters.AddWithValue( "@MessageID", MessageID );
						using ( DataTable dt = GetData( cmd ) )
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
		static public DataTable GetSearchResult( string ToSearch, SEARCH_FIELD sf, SEARCH_WHAT sw, int fid, int UserID )
		{
			if ( ToSearch.Length == 0 )
				return new DataTable();

			if ( ToSearch == "*" )
				ToSearch = "";

			ToSearch = ToSearch.Replace( "'", "''" );

			string sql = "select a.ForumID, a.TopicID, a.Topic, a.TopicMovedID, b.UserID, b.Name, c.MessageID, c.Posted, c.Message, c.Flags ";
			sql += "from yaf_topic a left join yaf_message c on a.TopicID = c.TopicID left join yaf_user b on c.UserID = b.UserID join yaf_vaccess x on x.ForumID=a.ForumID ";

			sql += String.Format( "where  (a.Flags & 8) = 0 and x.ReadAccess<>0 and a.TopicMovedID IS NULL and x.UserID={0} ", UserID );

			if ( sf == SEARCH_FIELD.sfUSER_NAME )
			{
				sql += string.Format( "and b.Name like '%{0}%' ", ToSearch );
			}
			else
			{
				string [] words;
				sql += "and ( ";
				switch ( sw )
				{
					case SEARCH_WHAT.sfALL_WORDS:
						words = ToSearch.Split( ' ' );
						foreach ( string word in words )
						{
							sql += string.Format( "(c.Message like '%{0}%' or a.Topic like '%{0}%' ) and ", word );
						}
						// remove last OR in sql query
						sql = sql.Substring( 0, sql.Length - 5 );
						break;
					case SEARCH_WHAT.sfANY_WORDS:
						words = ToSearch.Split( ' ' );
						foreach ( string word in words )
						{
							sql += string.Format( "c.Message like '%{0}%' or a.Topic like '%{0}%' or ", word );
						}
						// remove last OR in sql query
						sql = sql.Substring( 0, sql.Length - 4 );
						break;
					case SEARCH_WHAT.sfEXACT:
						sql += string.Format( "c.Message like '%{0}%' or a.Topic like '%{0}%' ", ToSearch );
						break;
				}
				sql += " ) ";
			}

			if ( fid >= 0 )
			{
				sql += string.Format( "and a.ForumID = {0}", fid );
			}

			sql += " order by c.Posted desc";

			using ( SqlCommand cmd = new SqlCommand() )
			{
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
				return GetData( cmd );
			}
		}

		#endregion

		#region DataSets
		/// <summary>
		/// Returns the layout of the board
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="UserID">UserID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <param name="parentID">ParentID</param>
		/// <returns>Returns board layout</returns>
		static public DataSet board_layout( object boardID, object UserID, object CategoryID, object parentID )
		{
			if ( CategoryID != null && long.Parse( CategoryID.ToString() ) == 0 )
				CategoryID = null;

			using ( SqlConnection conn = GetConnection() )
			{
				using ( DataSet ds = new DataSet() )
				{
					using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
					{
						using ( SqlDataAdapter da = new SqlDataAdapter( "yaf_forum_moderators", conn ) )
						{
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.SelectCommand.Transaction = trans;
							da.Fill( ds, "Moderator" );
						}
						using ( SqlDataAdapter da = new SqlDataAdapter( "yaf_category_listread", conn ) )
						{
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "@BoardID", boardID );
							da.SelectCommand.Parameters.AddWithValue( "@UserID", UserID );
							da.SelectCommand.Parameters.AddWithValue( "@CategoryID", CategoryID );
							da.Fill( ds, "yaf_Category" );
						}
						using ( SqlDataAdapter da = new SqlDataAdapter( "yaf_forum_listread", conn ) )
						{
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "@BoardID", boardID );
							da.SelectCommand.Parameters.AddWithValue( "@UserID", UserID );
							da.SelectCommand.Parameters.AddWithValue( "@CategoryID", CategoryID );
							da.SelectCommand.Parameters.AddWithValue( "@ParentID", parentID );
							da.Fill( ds, "yaf_Forum" );
						}
						ds.Relations.Add( "FK_Forum_Category", ds.Tables ["yaf_Category"].Columns ["CategoryID"], ds.Tables ["yaf_Forum"].Columns ["CategoryID"] );
						ds.Relations.Add( "FK_Moderator_Forum", ds.Tables ["yaf_Forum"].Columns ["ForumID"], ds.Tables ["Moderator"].Columns ["ForumID"], false );
						trans.Commit();
					}
					return ds;
				}
			}
		}

		/// <summary>
		/// Gets a list of categories????
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <returns>DataSet with categories</returns>
		static public DataSet ds_forumadmin( object boardID )
		{
			using ( DataSet ds = new DataSet() )
			{
				using ( SqlDataAdapter da = new SqlDataAdapter( "yaf_category_list", GetConnection() ) )
				{
					using ( SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction( m_isoLevel ) )
					{
						da.SelectCommand.Transaction = trans;
						da.SelectCommand.Parameters.AddWithValue( "@BoardID", boardID );
						da.SelectCommand.CommandType = CommandType.StoredProcedure;
						da.Fill( ds, "yaf_Category" );
						da.SelectCommand.CommandText = "yaf_forum_list";
						da.Fill( ds, "yaf_ForumUnsorted" );

						DataTable dtForumListSorted = ds.Tables ["yaf_ForumUnsorted"].Clone();
						dtForumListSorted.TableName = "yaf_Forum";
						ds.Tables.Add( dtForumListSorted );
						dtForumListSorted.Dispose();
						forum_list_sort_basic( ds.Tables ["yaf_ForumUnsorted"], ds.Tables ["yaf_Forum"], 0, 0 );
						ds.Tables.Remove( "yaf_ForumUnsorted" );
						ds.Relations.Add( "FK_Forum_Category", ds.Tables ["yaf_Category"].Columns ["CategoryID"], ds.Tables ["yaf_Forum"].Columns ["CategoryID"] );
						trans.Commit();
					}
					return ds;
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_accessmask_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// Deletes an access mask
		/// </summary>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <returns></returns>
		static public bool accessmask_delete( object accessMaskID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_accessmask_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				return ( int ) ExecuteScalar( cmd ) != 0;
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_accessmask_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", name );
				cmd.Parameters.AddWithValue( "@ReadAccess", readAccess );
				cmd.Parameters.AddWithValue( "@PostAccess", postAccess );
				cmd.Parameters.AddWithValue( "@ReplyAccess", replyAccess );
				cmd.Parameters.AddWithValue( "@PriorityAccess", priorityAccess );
				cmd.Parameters.AddWithValue( "@PollAccess", pollAccess );
				cmd.Parameters.AddWithValue( "@VoteAccess", voteAccess );
				cmd.Parameters.AddWithValue( "@ModeratorAccess", moderatorAccess );
				cmd.Parameters.AddWithValue( "@EditAccess", editAccess );
				cmd.Parameters.AddWithValue( "@DeleteAccess", deleteAccess );
				cmd.Parameters.AddWithValue( "@UploadAccess", uploadAccess );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_active_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Guests", Guests );
				return GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the list of active users within a certain forum
		/// </summary>
		/// <param name="forumID">forumID</param>
		/// <returns>DataTable of all ative users in a forum</returns>
		static public DataTable active_listforum( object forumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_active_listforum" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// Gets the list of active users in a topic
		/// </summary>
		/// <param name="topicID">ID of topic </param>
		/// <returns>DataTable of all users that are in a topic</returns>
		static public DataTable active_listtopic( object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_active_listtopic" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				return GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the activity statistics for a board
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <returns>DataRow of activity stata</returns>
		static public DataRow active_stats( object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_active_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				using ( DataTable dt = GetData( cmd ) )
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				cmd.Parameters.AddWithValue( "@AttachmentID", attachmentID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_save" ) )
			{
				byte [] fileData = null;
				if ( stream != null )
				{
					fileData = new byte [stream.Length];
					stream.Seek( 0, System.IO.SeekOrigin.Begin );
					stream.Read( fileData, 0, ( int ) stream.Length );
				}
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				cmd.Parameters.AddWithValue( "@FileName", fileName );
				cmd.Parameters.AddWithValue( "@Bytes", bytes );
				cmd.Parameters.AddWithValue( "@ContentType", contentType );
				cmd.Parameters.AddWithValue( "@FileData", fileData );
				ExecuteNonQuery( cmd );
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

			using ( DataTable dt = DB.registry_list( "UseFileTable" ) )
				foreach ( DataRow dr in dt.Rows )
					UseFileTable = Convert.ToBoolean( Convert.ToInt32( dr ["Value"] ) );

			//If the files are actually saved in the Hard Drive
			if ( !UseFileTable )
			{
				using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "@AttachmentID", attachmentID );
					DataTable tbAttachments = GetData( cmd );
					string sUpDir = HttpContext.Current.Server.MapPath( Config.ConfigSection ["uploaddir"] );
					foreach ( DataRow row in tbAttachments.Rows )
						System.IO.File.Delete( String.Format( "{0}{1}.{2}", sUpDir, row ["MessageID"], row ["FileName"] ) );
				}

			}
			using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@AttachmentID", attachmentID );
				ExecuteNonQuery( cmd );
			}
			//End ABOT CHANGE 16.04.04
		}


		/// <summary>
		/// Attachement dowload
		/// </summary>
		/// <param name="attachmentID">ID of attachemnt to download</param>
		static public void attachment_download( object attachmentID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_download" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@AttachmentID", attachmentID );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_bannedip_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@ID", ID );
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_bannedip_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ID", ID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Mask", Mask );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes Banned IP
		/// </summary>
		/// <param name="ID">ID of banned ip to delete</param>
		static public void bannedip_delete( object ID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_bannedip_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ID", ID );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// Gets posting statistics
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <returns>DataRow of Poststats</returns>
		static public DataRow board_poststats( object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_poststats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				using ( DataTable dt = GetData( cmd ) )
				{
					return dt.Rows [0];
				}
			}
		}
		/// <summary>
		/// Gets statistica about number of posts etc.
		/// </summary>
		/// <returns>DataRow</returns>
		static public DataRow board_stats()
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				using ( DataTable dt = GetData( cmd ) )
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", name );
				cmd.Parameters.AddWithValue( "@AllowThreaded", allowThreaded );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_create" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardName", name );
				cmd.Parameters.AddWithValue( "@AllowThreaded", allowThreaded );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@UserEmail", userEmail );
				cmd.Parameters.AddWithValue( "@UserPass", userPass );
				cmd.Parameters.AddWithValue( "@IsHostAdmin", false );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes a board
		/// </summary>
		/// <param name="boardID">ID of board to delete</param>
		static public void board_delete( object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_board_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_category_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );
				return ( int ) ExecuteScalar( cmd ) != 0;
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_category_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@CategoryID", categoryID );
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_category_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@SortOrder", SortOrder );
				ExecuteNonQuery( cmd );
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
		static public void checkemail_save( object UserID, object Hash, object Email )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_checkemail_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Hash", Hash );
				cmd.Parameters.AddWithValue( "@Email", Email );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Updates a hash
		/// </summary>
		/// <param name="hash">New hash</param>
		/// <returns>bool value to indicate a update</returns>
		static public bool checkemail_update( object hash )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_checkemail_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Hash", hash );
				return ( bool ) ExecuteScalar( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_choice_vote" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ChoiceID", choiceID );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@RemoteIP", remoteIP );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_EventLog
		static public void eventlog_create( object userID, object source, object description, object type )
		{
			try
			{
				if ( userID == null ) userID = DBNull.Value;
				using ( SqlCommand cmd = new SqlCommand( "yaf_eventlog_create" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "@Type", ( object ) type );
					cmd.Parameters.AddWithValue( "@UserID", ( object ) userID );
					cmd.Parameters.AddWithValue( "@Source", ( object ) source.ToString() );
					cmd.Parameters.AddWithValue( "@Description", ( object ) description.ToString() );
					ExecuteNonQuery( cmd );
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}

		static public void eventlog_create( object userID, object source, object description )
		{
			eventlog_create( userID, ( object ) source.GetType().ToString(), description, ( object ) 0 );
		}

		static public void eventlog_delete( object eventLogID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_eventlog_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@EventLogID", eventLogID );
				ExecuteNonQuery( cmd );
			}
		}

		static public DataTable eventlog_list( object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_eventlog_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_pollvote_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@PollID", pollid );
				cmd.Parameters.AddWithValue( "@UserID", userid );
				cmd.Parameters.AddWithValue( "@RemoteIP", remoteip );
				return GetData( cmd );
			}
		}
		#endregion

		#region yaf_Forum
		//ABOT NEW 16.04.04
		/// <summary>
		/// Deletes attachments out of a entire forum
		/// </summary>
		/// <param name="ForumID">ID of forum to delete all attachemnts out of</param>
		static private void forum_deleteAttachments( object ForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listtopics" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				using ( DataTable dt = GetData( cmd ) )
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
		static public bool forum_delete( object ForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listSubForums" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				if ( ExecuteScalar( cmd ) is DBNull )
				{
					forum_deleteAttachments( ForumID );
					using ( SqlCommand cmd_new = new SqlCommand( "yaf_forum_delete" ) )
					{
						cmd_new.CommandType = CommandType.StoredProcedure;
						cmd_new.Parameters.AddWithValue( "@ForumID", ForumID );
						ExecuteNonQuery( cmd_new );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listallmymoderated" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
		//END ABOT NEW 16.04.04
		/// <summary>
		/// Gets a list of topics in a forum
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <param name="ForumID">forumID</param>
		/// <returns>DataTable with list of topics from a forum</returns>
		static public DataTable forum_list( object boardID, object ForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listall" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// Lists all forums within a given subcategory
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <returns>DataTable with list</returns>
		static public DataTable forum_listall_fromCat( object boardID, object CategoryID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listall_fromCat" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );

				int intCategoryID = Convert.ToInt32( CategoryID.ToString() );

				using ( DataTable dt = GetData( cmd ) )
				{
					return forum_sort_list( dt, 0, intCategoryID, 0, null );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listpath" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				return GetData( cmd );
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
		static public DataTable forum_listread( object boardID, object UserID, object CategoryID, object parentID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_listread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );
				cmd.Parameters.AddWithValue( "@ParentID", parentID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		static public DataTable forum_moderatelist()
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_moderatelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				//cmd.Parameters.AddWithValue("@UserID",UserID);
				return GetData( cmd );
			}
		}
		static public DataTable forum_moderators()
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_moderators" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData( cmd );
			}
		}

		static public long forum_save( object ForumID, object CategoryID, object parentID, object Name, object Description, object SortOrder, object Locked, object Hidden, object IsTest, object moderated, object accessMaskID, object remoteURL, object themeURL, bool dummy )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@CategoryID", CategoryID );
				cmd.Parameters.AddWithValue( "@ParentID", parentID );
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@Description", Description );
				cmd.Parameters.AddWithValue( "@SortOrder", SortOrder );
				cmd.Parameters.AddWithValue( "@Locked", Locked );
				cmd.Parameters.AddWithValue( "@Hidden", Hidden );
				cmd.Parameters.AddWithValue( "@IsTest", IsTest );
				cmd.Parameters.AddWithValue( "@Moderated", moderated );
				cmd.Parameters.AddWithValue( "@RemoteURL", remoteURL );
				cmd.Parameters.AddWithValue( "@ThemeURL", themeURL );
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				return long.Parse( ExecuteScalar( cmd ).ToString() );
			}
		}

		static private void forum_list_sort_basic( DataTable listsource, DataTable list, int parentid, int currentLvl )
		{
			for ( int i = 0; i < listsource.Rows.Count; i++ )
			{
				DataRow row = listsource.Rows [i];
				if ( ( row ["ParentID"] ) == DBNull.Value )
					row ["ParentID"] = 0;

				if ( ( int ) row ["ParentID"] == parentid )
				{
					string sIndent = "";
					int iIndent = Convert.ToInt32( currentLvl );
					for ( int j = 0; j < iIndent; j++ ) sIndent += "--";
					row ["Name"] = string.Format( " -{0} {1}", sIndent, row ["Name"] );
					list.Rows.Add( row.ItemArray );
					forum_list_sort_basic( listsource, list, ( int ) row ["ForumID"], currentLvl + 1 );
				}
			}
		}

		static private void forum_sort_list_recursive( DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent )
		{
			DataRow newRow;

			foreach (DataRow row in listSource.Rows)
			{
				// see if this is a root-forum
				if ( row ["ParentID"] == DBNull.Value )
					row ["ParentID"] = 0;

				if ( ( int ) row ["ParentID"] == parentID )
				{
					if ( ( int ) row ["CategoryID"] != categoryID )
					{
						categoryID = ( int ) row ["CategoryID"];

						newRow = listDestination.NewRow();
						newRow ["ForumID"] = string.Empty;
						newRow ["Title"] = string.Format("{0}",row ["Category"].ToString());
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
					forum_sort_list_recursive( listSource, listDestination, ( int ) row ["ForumID"], categoryID, currentIndent + 1 );
				}
			}
		}

		static protected DataTable forum_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent, int [] forumidExclusions )
		{
			DataTable listDestination = new DataTable();

			listDestination.Columns.Add( "ForumID", typeof(String) );
			listDestination.Columns.Add( "Title", typeof(String) );

			DataRow blankRow = listDestination.NewRow();
			blankRow ["ForumID"] = string.Empty;
			blankRow ["Title"] = string.Empty;
			listDestination.Rows.Add( blankRow );

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
			return forum_listall_sorted( boardID, userID, null );
		}

		static public DataTable forum_listall_sorted( object boardID, object userID, int [] forumidExclusions )
		{
			using ( DataTable dt = forum_listall( boardID, userID ) )
      {
				return forum_sort_list( dt, 0, 0, 0, forumidExclusions );
      }
		}
		#endregion

		#region yaf_ForumAccess
		static public DataTable forumaccess_list( object ForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forumaccess_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				return GetData( cmd );
			}
		}
		static public void forumaccess_save( object ForumID, object GroupID, object accessMaskID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forumaccess_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable forumaccess_group( object GroupID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_forumaccess_group" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				return GetData( cmd );
			}
		}
		#endregion

		#region yaf_Group
		static public DataTable group_list( object boardID, object GroupID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_group_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				return GetData( cmd );
			}
		}
        static public DataTable group_list(object boardID, object GroupID)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_group_list"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BoardID", boardID);
                cmd.Parameters.AddWithValue("@GroupID", GroupID);
                return GetData(cmd);
            }
        }
        static public void group_delete(object GroupID) 
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_group_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable group_member( object boardID, object UserID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_group_member" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				return GetData( cmd );
			}
		}
		static public long group_save( object GroupID, object boardID, object Name, object IsAdmin, object IsGuest, object IsStart, object isModerator, object accessMaskID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_group_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@IsAdmin", IsAdmin );
				cmd.Parameters.AddWithValue( "@IsGuest", IsGuest );
				cmd.Parameters.AddWithValue( "@IsStart", IsStart );
				cmd.Parameters.AddWithValue( "@IsModerator", isModerator );
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				return long.Parse( ExecuteScalar( cmd ).ToString() );
			}
		}
		#endregion

		#region yaf_Mail
		static public void mail_delete( object MailID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_mail_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MailID", MailID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable mail_list()
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_mail_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData( cmd );
			}
		}
		static public void mail_createwatch( object topicID, object from, object subject, object body, object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_mail_createwatch" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				cmd.Parameters.AddWithValue( "@From", from );
				cmd.Parameters.AddWithValue( "@Subject", subject );
				cmd.Parameters.AddWithValue( "@Body", body );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				ExecuteNonQuery( cmd );
			}
		}
        static public void mail_create(object from, object to, object subject, object body)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_mail_create"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@From", from);
                cmd.Parameters.AddWithValue("@To", to);
                cmd.Parameters.AddWithValue("@Subject", subject);
                cmd.Parameters.AddWithValue("@Body", body);
                ExecuteNonQuery(cmd);
            }
        }
        #endregion

		#region yaf_Message
		static public DataTable post_list( object topicID, object updateViewCount )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_post_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				cmd.Parameters.AddWithValue( "@UpdateViewCount", updateViewCount );
				return GetData( cmd );
			}
		}
		static public DataTable post_list_reverse10( object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_post_list_reverse10" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				return GetData( cmd );
			}
		}
		static public DataTable post_last10user( object boardID, object userID, object pageUserID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_post_last10user" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@PageUserID", pageUserID );
				return GetData( cmd );
			}
		}
		//BAI ADDED 30.01.2004
		static private void message_deleteRecursively( object messageID )
		{
			bool UseFileTable = false;

			using ( DataTable dt = DB.registry_list( "UseFileTable" ) )
				foreach ( DataRow dr in dt.Rows )
					UseFileTable = Convert.ToBoolean( Convert.ToInt32( dr ["Value"] ) );

			//Delete replies
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_getReplies" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				DataTable tbReplies = GetData( cmd );
				foreach ( DataRow row in tbReplies.Rows )
					message_deleteRecursively( row ["MessageID"] );
			}

			//ABOT CHANGED 16.01.04: Delete files from hard disk
			//If the files are actually saved in the Hard Drive
			if ( !UseFileTable )
			{
				using ( SqlCommand cmd = new SqlCommand( "yaf_attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "@MessageID", messageID );
					DataTable tbAttachments = GetData( cmd );
					string sUpDir = HttpContext.Current.Server.MapPath( Config.ConfigSection ["uploaddir"] );
					foreach ( DataRow row in tbAttachments.Rows )
						System.IO.File.Delete( String.Format( "{0}{1}.{2}", sUpDir, messageID, row ["FileName"] ) );
				}

			}
			//END ABOT CHANGE 16.04.04

			//Delete Message
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable message_list( object messageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				return GetData( cmd );
			}
		}
		static public void message_delete( object messageID )
		{
			message_deleteRecursively( messageID );
			/*
			using(SqlCommand cmd = new SqlCommand("yaf_message_delete")) 
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@MessageID",messageID);
				ExecuteNonQuery(cmd);
			}
			*/
		}
		static public void message_approve( object messageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void message_update( object messageID, object priority, object message, object subject, object flags )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@MessageID", messageID );
				cmd.Parameters.AddWithValue( "@Priority", priority );
				cmd.Parameters.AddWithValue( "@Message", message );
				cmd.Parameters.AddWithValue( "@Subject", subject );
				cmd.Parameters.AddWithValue( "@Flags", flags );
				ExecuteNonQuery( cmd );
			}
		}
		static public bool message_save( object TopicID, object UserID, object Message, object UserName, object IP, object posted, object replyTo, object Flags, ref long nMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_save" ) )
			{
				SqlParameter paramMessageID = new SqlParameter( "@MessageID", nMessageID );
				paramMessageID.Direction = ParameterDirection.Output;

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", TopicID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Message", Message );
				cmd.Parameters.AddWithValue( "@UserName", UserName );
				cmd.Parameters.AddWithValue( "@IP", IP );
				cmd.Parameters.AddWithValue( "@Posted", posted );
				cmd.Parameters.AddWithValue( "@ReplyTo", replyTo );
				cmd.Parameters.AddWithValue( "@Flags", Flags );
				cmd.Parameters.Add( paramMessageID );
				ExecuteNonQuery( cmd );
				nMessageID = ( long ) paramMessageID.Value;
				return true;
			}
		}
		static public DataTable message_unapproved( object forumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_unapproved" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				return GetData( cmd );
			}
		}
		static public DataTable message_findunread( object topicID, object lastRead )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_message_findunread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				cmd.Parameters.AddWithValue( "@LastRead", lastRead );
				return GetData( cmd );
			}
		}
		#endregion

		#region yaf_NntpForum
		static public DataTable nntpforum_list( object boardID, object minutes, object nntpForumID, object active )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Minutes", minutes );
				cmd.Parameters.AddWithValue( "@NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "@Active", active );
				return GetData( cmd );
			}
		}
		static public void nntpforum_update( object nntpForumID, object lastMessageNo, object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpforum_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "@LastMessageNo", lastMessageNo );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_save( object nntpForumID, object nntpServerID, object groupName, object forumID, object active )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "@NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "@GroupName", groupName );
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				cmd.Parameters.AddWithValue( "@Active", active );
				ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_delete( object nntpForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpForumID", nntpForumID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_NntpServer
		static public DataTable nntpserver_list( object boardID, object nntpServerID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpserver_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@NntpServerID", nntpServerID );
				return GetData( cmd );
			}
		}
		static public void nntpserver_save( object nntpServerID, object boardID, object name, object address, object port, object userName, object userPass )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpserver_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", name );
				cmd.Parameters.AddWithValue( "@Address", address );
				cmd.Parameters.AddWithValue( "@Port", port );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@UserPass", userPass );
				ExecuteNonQuery( cmd );
			}
		}
		static public void nntpserver_delete( object nntpServerID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntpserver_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpServerID", nntpServerID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_NntpTopic
		static public DataTable nntptopic_list( object thread )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntptopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Thread", thread );
				return GetData( cmd );
			}
		}
		static public void nntptopic_savemessage( object nntpForumID, object topic, object body, object userID, object userName, object ip, object posted, object thread )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_nntptopic_savemessage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "@Topic", topic );
				cmd.Parameters.AddWithValue( "@Body", body );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@IP", ip );
				cmd.Parameters.AddWithValue( "@Posted", posted );
				cmd.Parameters.AddWithValue( "@Thread", thread );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_PMessage
		static public DataTable pmessage_list( object toUserID, object fromUserID, object pMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "@FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "@PMessageID", pMessageID );
				return GetData( cmd );
			}
		}
		static public void pmessage_delete( object pMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@PMessageID", pMessageID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void pmessage_save( object fromUserID, object toUserID, object subject, object body, object Flags )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "@ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "@Subject", subject );
				cmd.Parameters.AddWithValue( "@Body", body );
				cmd.Parameters.AddWithValue( "@Flags", Flags );
				ExecuteNonQuery( cmd );
			}
		}
		static public void pmessage_markread( object userPMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_markread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserPMessageID", userPMessageID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable pmessage_info()
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData( cmd );
			}
		}
		static public void pmessage_prune( object daysRead, object daysUnread )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_pmessage_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@DaysRead", daysRead );
				cmd.Parameters.AddWithValue( "@DaysUnread", daysUnread );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Poll
		static public DataTable poll_stats( object pollID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_poll_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@PollID", pollID );
				return GetData( cmd );
			}
		}
		static public int poll_save( object question, object c1, object c2, object c3, object c4, object c5, object c6, object c7, object c8, object c9, object c10 )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_poll_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Question", question );
				cmd.Parameters.AddWithValue( "@Choice1", c1 );
				cmd.Parameters.AddWithValue( "@Choice2", c2 );
				cmd.Parameters.AddWithValue( "@Choice3", c3 );
				cmd.Parameters.AddWithValue( "@Choice4", c4 );
				cmd.Parameters.AddWithValue( "@Choice5", c5 );
				cmd.Parameters.AddWithValue( "@Choice6", c6 );
				cmd.Parameters.AddWithValue( "@Choice7", c7 );
				cmd.Parameters.AddWithValue( "@Choice8", c8 );
				cmd.Parameters.AddWithValue( "@Choice9", c9 );
				cmd.Parameters.AddWithValue( "@Closes", c10 );
				return ( int ) ExecuteScalar( cmd );
			}
		}
		#endregion

		#region yaf_Rank
		static public DataTable rank_list( object boardID, object rankID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_rank_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@RankID", rankID );
				return GetData( cmd );
			}
		}
		static public void rank_save( object RankID, object boardID, object Name, object IsStart, object IsLadder, object MinPosts, object RankImage )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_rank_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@RankID", RankID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@IsStart", IsStart );
				cmd.Parameters.AddWithValue( "@IsLadder", IsLadder );
				cmd.Parameters.AddWithValue( "@MinPosts", MinPosts );
				cmd.Parameters.AddWithValue( "@RankImage", RankImage );
				ExecuteNonQuery( cmd );
			}
		}
		static public void rank_delete( object RankID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_rank_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@RankID", RankID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_Smiley
		static public DataTable smiley_list( object boardID, object SmileyID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_smiley_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@SmileyID", SmileyID );
				return GetData( cmd );
			}
		}
		static public DataTable smiley_listunique( object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_smiley_listunique" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				return GetData( cmd );
			}
		}
		static public void smiley_delete( object SmileyID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_smiley_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@SmileyID", SmileyID );
				ExecuteNonQuery( cmd );
				System.Web.HttpContext.Current.Cache.Remove( "Smiles" );
			}
		}
		static public void smiley_save( object SmileyID, object boardID, object Code, object Icon, object Emoticon, object Replace )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_smiley_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@SmileyID", SmileyID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Code", Code );
				cmd.Parameters.AddWithValue( "@Icon", Icon );
				cmd.Parameters.AddWithValue( "@Emoticon", Emoticon );
				cmd.Parameters.AddWithValue( "@Replace", Replace );
				ExecuteNonQuery( cmd );
				System.Web.HttpContext.Current.Cache.Remove( "Smiles" );
			}
		}
		#endregion

		#region yaf_Registry
		/// <summary>
		/// Retrieves entries in the board settings registry
		/// </summary>
		/// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
		/// <returns>DataTable filled will registry entries</returns>
		static public DataTable registry_list( object Name, object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_registry_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				return GetData( cmd );
			}
		}
		/// <summary>
		/// Retrieves entries in the board settings registry
		/// </summary>
		/// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
		/// <returns>DataTable filled will registry entries</returns>
		static public DataTable registry_list( object Name )
		{
			return registry_list( Name, null );
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
		static public void registry_save( object Name, object Value )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@Value", Value );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Saves a single registry entry pair to the database.
		/// </summary>
		/// <param name="Name">Unique name associated with this entry</param>
		/// <param name="Value">Value associated with this entry which can be null</param>
		/// <param name="BoardID">The BoardID for this entry</param>
		static public void registry_save( object Name, object Value, object boardID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@Value", Value );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_system_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData( cmd );
			}
		}
		#endregion

		#region yaf_Topic
		static public int topic_prune( object ForumID, object Days )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@Days", Days );
				return ( int ) ExecuteScalar( cmd );
			}
		}
		static public DataTable topic_list( object ForumID, object Announcement, object Date, object offset, object count )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@Announcement", Announcement );
				cmd.Parameters.AddWithValue( "@Date", Date );
				cmd.Parameters.AddWithValue( "@Offset", offset );
				cmd.Parameters.AddWithValue( "@Count", count );
				return GetData( cmd );
			}
		}
		static public void topic_move( object TopicID, object ForumID, object ShowMoved )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_move" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", TopicID );
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@ShowMoved", ShowMoved );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable topic_latest( object boardID, object numOfPostsToRetrieve, object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_latest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@NumPosts", numOfPostsToRetrieve );
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
		static public DataTable topic_active( object boardID, object UserID, object Since, object categoryID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_active" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Since", Since );
				cmd.Parameters.AddWithValue( "@CategoryID", categoryID );
				return GetData( cmd );
			}
		}
		//ABOT NEW 16.04.04:Delete all topic's messages
		static private void topic_deleteAttachments( object TopicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_listmessages" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", TopicID );
				using ( DataTable dt = GetData( cmd ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						message_deleteRecursively( row ["MessageID"] );
					}
				}
			}
		}
		//END ABOT NEW
		static public void topic_delete( object TopicID )
		{
			//ABOT CHANGE 16.04.04
			topic_deleteAttachments( TopicID );
			//END ABOT CHANGE 16.04.04
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", TopicID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable topic_findprev( object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_findprev" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				return GetData( cmd );
			}
		}
		static public DataTable topic_findnext( object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_findnext" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				return GetData( cmd );
			}
		}
		static public void topic_lock( object topicID, object locked )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_lock" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				cmd.Parameters.AddWithValue( "@Locked", locked );
				ExecuteNonQuery( cmd );
			}
		}
		static public long topic_save( object ForumID, object Subject, object Message, object UserID, object Priority, object PollID, object UserName, object IP, object posted, object flags, ref long nMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				cmd.Parameters.AddWithValue( "@Subject", Subject );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Message", Message );
				cmd.Parameters.AddWithValue( "@Priority", Priority );
				cmd.Parameters.AddWithValue( "@UserName", UserName );
				cmd.Parameters.AddWithValue( "@IP", IP );
				cmd.Parameters.AddWithValue( "@PollID", PollID );
				cmd.Parameters.AddWithValue( "@Posted", posted );
				cmd.Parameters.AddWithValue( "@Flags", flags );

				DataTable dt = GetData( cmd );
				nMessageID = long.Parse( dt.Rows [0] ["MessageID"].ToString() );
				return long.Parse( dt.Rows [0] ["TopicID"].ToString() );
			}
		}
		static public DataRow topic_info( object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_topic_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				using ( DataTable dt = GetData( cmd ) )
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_replace_words_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return GetData( cmd );
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
			using ( SqlCommand cmd = new SqlCommand( "yaf_replace_words_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ID", ID );
				cmd.Parameters.AddWithValue( "@badword", badword );
				cmd.Parameters.AddWithValue( "@goodword", goodword );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes a bad/good word
		/// </summary>
		/// <param name="ID">ID of bad/good word to delete</param>
		static public void replace_words_delete( object ID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_replace_words_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ID", ID );
				ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Edits bad words?
		/// </summary>
		/// <param name="ID">ID of badword</param>
		/// <returns>DataTable</returns>
		static public DataTable replace_words_edit( object ID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_replace_words_edit" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@ID", ID );
				return GetData( cmd );
			}
		}
		// rico : replace words / end
		#endregion

		#region yaf_User
		static public DataTable user_list( object boardID, object UserID, object Approved )
		{
			return user_list( boardID, UserID, Approved, null, null );
		}
		static public DataTable user_list( object boardID, object UserID, object Approved, object groupID, object rankID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Approved", Approved );
				cmd.Parameters.AddWithValue( "@GroupID", groupID );
				cmd.Parameters.AddWithValue( "@RankID", rankID );
				return GetData( cmd );
			}
		}
		static public void user_delete( object UserID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				ExecuteNonQuery( cmd );
			}
		}
        static public void user_setrole(int nBoardID,object providerUserKey,object role)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_setrole"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BoardID", nBoardID);
                cmd.Parameters.AddWithValue("@ProviderUserKey", providerUserKey);
                cmd.Parameters.AddWithValue("@Role", role);
                ExecuteNonQuery(cmd);
            }
        }
        static public void user_setinfo(int nBoardID, System.Web.Security.MembershipUser user)
        {
            using (SqlCommand cmd = new SqlCommand("update dbo.yaf_User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@BoardID", nBoardID);
                cmd.Parameters.AddWithValue("@ProviderUserKey", user.ProviderUserKey);
                ExecuteNonQuery(cmd);
            }
        }
        static public void user_migrate(object UserID, object providerUserKey)
        {
            using (SqlCommand cmd = new SqlCommand("update dbo.yaf_User set ProviderUserKey=@ProviderUserKey where UserID=@UserID"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ProviderUserKey", providerUserKey);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                ExecuteNonQuery(cmd);
            }
        }
        static public void user_deleteold(object boardID) 
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_deleteold" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void user_approve( object UserID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void user_suspend( object userID, object suspend )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_suspend" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@Suspend", suspend );
				ExecuteNonQuery( cmd );
			}
		}
		static public bool user_changepassword( object UserID, object oldpw, object newpw )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_changepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@OldPassword", oldpw );
				cmd.Parameters.AddWithValue( "@NewPassword", newpw );
				return ( bool ) ExecuteScalar( cmd );
			}
		}
		static public void user_save( object UserID, object boardID, object UserName, object Password, object Email, object Hash,
			object Location, object HomePage, object TimeZone, object Avatar,
			object languageFile, object themeFile, object Approved,
			object msn, object yim, object aim, object icq,
			object realName, object occupation, object interests, object gender, object weblog,
			object PMNotification )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserName", UserName );
				cmd.Parameters.AddWithValue( "@Password", Password );
				cmd.Parameters.AddWithValue( "@Email", Email );
				cmd.Parameters.AddWithValue( "@Hash", Hash );
				cmd.Parameters.AddWithValue( "@Location", Location );
				cmd.Parameters.AddWithValue( "@HomePage", HomePage );
				cmd.Parameters.AddWithValue( "@TimeZone", TimeZone );
				cmd.Parameters.AddWithValue( "@Avatar", Avatar );
				cmd.Parameters.AddWithValue( "@LanguageFile", languageFile );
				cmd.Parameters.AddWithValue( "@ThemeFile", themeFile );
				cmd.Parameters.AddWithValue( "@Approved", Approved );
				cmd.Parameters.AddWithValue( "@MSN", msn );
				cmd.Parameters.AddWithValue( "@YIM", yim );
				cmd.Parameters.AddWithValue( "@AIM", aim );
				cmd.Parameters.AddWithValue( "@ICQ", icq );
				cmd.Parameters.AddWithValue( "@RealName", realName );
				cmd.Parameters.AddWithValue( "@Occupation", occupation );
				cmd.Parameters.AddWithValue( "@Interests", interests );
				cmd.Parameters.AddWithValue( "@Gender", gender );
				cmd.Parameters.AddWithValue( "@Weblog", weblog );
				cmd.Parameters.AddWithValue( "@PMNotification", PMNotification );
				ExecuteNonQuery( cmd );
			}
		}
		static public void user_adminsave( object boardID, object UserID, object Name, object email, object isHostAdmin, object RankID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_adminsave" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@Name", Name );
				cmd.Parameters.AddWithValue( "@Email", email );
				cmd.Parameters.AddWithValue( "@IsHostAdmin", isHostAdmin );
				cmd.Parameters.AddWithValue( "@RankID", RankID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable user_emails( object boardID, object GroupID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_emails" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );

				return GetData( cmd );
			}
		}
		static public DataTable user_accessmasks( object boardID, object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_accessmasks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserID", userID );

				return GetData( cmd );
			}
		}
		static public object user_recoverpassword( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_recoverpassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@Email", email );
				return ExecuteScalar( cmd );
			}
		}
		static public void user_savepassword( object userID, object password )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_savepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@Password", FormsAuthentication.HashPasswordForStoringInConfigFile( password.ToString(), "md5" ) );
				ExecuteNonQuery( cmd );
			}
		}
		static public object user_login( object boardID, object name, object password )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_login" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Name", name );
				cmd.Parameters.AddWithValue( "@Password", password );
				return ExecuteScalar( cmd );
			}
		}
		static public DataTable user_avatarimage( object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_avatarimage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
        static public int user_get(int nBoardID,object providerUserKey)
        {
            using (SqlCommand cmd = new SqlCommand("select UserID from dbo.yaf_User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BoardID", nBoardID);
                cmd.Parameters.AddWithValue("@ProviderUserKey", providerUserKey);
                return (int)ExecuteScalar(cmd);
            }
        }
        static public DataTable user_find(object boardID, bool filter, object userName, object email) 
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_find" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@Filter", filter );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@Email", email );
				return GetData( cmd );
			}
		}
		static public string user_getsignature( object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_getsignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return ExecuteScalar( cmd ).ToString();
			}
		}
		static public void user_savesignature( object userID, object signature )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_savesignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@Signature", signature );
				ExecuteNonQuery( cmd );
			}
		}
		static public void user_saveavatar( object userID, System.IO.Stream stream )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_saveavatar" ) )
			{
				byte [] data = new byte [stream.Length];
				stream.Seek( 0, System.IO.SeekOrigin.Begin );
				stream.Read( data, 0, ( int ) stream.Length );

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@AvatarImage", data );
				ExecuteNonQuery( cmd );
			}
		}
		static public void user_deleteavatar( object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_deleteavatar" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				ExecuteNonQuery( cmd );
			}
		}

		static public bool user_register( pages.ForumPage page, object boardID, object userName, object password, object email, object location, object homePage, object timeZone, bool emailVerification )
		{
			string hashinput = DateTime.Now.ToString() + email.ToString() + pages.register.CreatePassword( 20 );
			string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

			using ( SqlConnection conn = GetConnection() )
			{
				using ( SqlTransaction trans = conn.BeginTransaction( m_isoLevel ) )
				{
					try
					{
						using ( SqlCommand cmd = new SqlCommand( "yaf_user_save", conn ) )
						{
							cmd.Transaction = trans;
							cmd.Connection = conn;
							cmd.CommandType = CommandType.StoredProcedure;
							int UserID = 0;
							cmd.Parameters.AddWithValue( "@UserID", UserID );
							cmd.Parameters.AddWithValue( "@BoardID", boardID );
							cmd.Parameters.AddWithValue( "@UserName", userName );
							cmd.Parameters.AddWithValue( "@Password", FormsAuthentication.HashPasswordForStoringInConfigFile( password.ToString(), "md5" ) );
							cmd.Parameters.AddWithValue( "@Email", email );
							cmd.Parameters.AddWithValue( "@Hash", hash );
							cmd.Parameters.AddWithValue( "@Location", location );
							cmd.Parameters.AddWithValue( "@HomePage", homePage );
							cmd.Parameters.AddWithValue( "@TimeZone", timeZone );
							cmd.Parameters.AddWithValue( "@Approved", !emailVerification );
                            cmd.Parameters.AddWithValue("@PMNotification", 1);
                            cmd.ExecuteNonQuery();
						}

						if ( emailVerification )
						{
							//  Build a MailMessage
							string body = Utils.ReadTemplate( "verifyemail.txt" );
							body = body.Replace( "{link}", String.Format( "{1}{0}", Forum.GetLink( Pages.approve, "k={0}", hash ), page.ServerURL ) );
							body = body.Replace( "{key}", hash );
							body = body.Replace( "{forumname}", page.BoardSettings.Name );
							body = body.Replace( "{forumlink}", String.Format( "{0}", page.ForumURL ) );

							Utils.SendMail( page, page.BoardSettings.ForumEmail, email.ToString(), String.Format( "{0} email verification", page.BoardSettings.Name ), body );
							//ABOT DELETED 16.04.04
							//page.AddLoadMessage(page.GetText("REGMAIL_SENT"));
							//END ABOT DELETED 16.04.04
							trans.Commit();
						}
						else
						{
							trans.Commit();
						}
					}
					catch ( Exception x )
					{
						trans.Rollback();
						DB.eventlog_create( null, "user_register in DB.cs", x, EventLogTypes.Error );
						//page.AddLoadMessage(x.Message);
						return false;
					}
				}
			}
			return true;
		}
        static public int user_aspnet(int nBoardID, string sUserName, string sEmail, object providerUserKey)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("yaf_user_aspnet"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@BoardID", nBoardID);
                    cmd.Parameters.AddWithValue("@UserName", sUserName);
                    cmd.Parameters.AddWithValue("@Email", sEmail);
                    cmd.Parameters.AddWithValue("@ProviderUserKey", providerUserKey);
                    return (int)ExecuteScalar(cmd);
                }
            }
            catch (Exception x)
            {
                DB.eventlog_create(null, "user_aspnet in DB.cs", x, EventLogTypes.Error);
                return 0;
            }
        }
        static public int user_guest() 
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_guest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return ( int ) ExecuteScalar( cmd );
			}
		}
		static public DataTable user_activity_rank()
		{
			// This define the date since the posts are counted (can pass as parameter??)
			TimeSpan tsRange = new TimeSpan( 30, 0, 0, 0 );
			DateTime StartDate = DateTime.Now.Subtract( tsRange );

			using ( SqlCommand cmd = new SqlCommand( "yaf_user_activity_rank" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@StartDate", StartDate );
				return GetData( cmd );
			}
		}
		static public int user_nntp( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_user_nntp" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@BoardID", boardID );
				cmd.Parameters.AddWithValue( "@UserName", userName );
				cmd.Parameters.AddWithValue( "@Email", email );
				return ( int ) ExecuteScalar( cmd );
			}
        }

        static public void user_addpoints(object userID, object points)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_addpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Points", points);
                ExecuteNonQuery(cmd);
            }
        }

        static public void user_removepointsByTopicID(object topicID, object points)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_removepointsbytopicid"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TopicID", topicID);
                cmd.Parameters.AddWithValue("@Points", points);
                ExecuteNonQuery(cmd);
            }
        }

        static public void user_removepoints(object userID, object points)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_removepoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Points", points);
                ExecuteNonQuery(cmd);
            }
        }

        static public void user_setpoints(object userID, object points)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_setpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@Points", points);
                ExecuteNonQuery(cmd);
            }
        }

        static public int user_getpoints(object userID)
        {
            using (SqlCommand cmd = new SqlCommand("yaf_user_getpoints"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userID);
                return (int)ExecuteScalar(cmd);
            }
        }
		#endregion

		#region yaf_UserForum
		static public DataTable userforum_list( object userID, object forumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_userforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				return GetData( cmd );
			}
		}
		static public void userforum_delete( object userID, object forumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_userforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void userforum_save( object userID, object forumID, object accessMaskID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_userforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				cmd.Parameters.AddWithValue( "@AccessMaskID", accessMaskID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_UserGroup
		static public DataTable usergroup_list( object UserID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_usergroup_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				return GetData( cmd );
			}
		}
		static public void usergroup_save( object UserID, object GroupID, object Member )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_usergroup_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@GroupID", GroupID );
				cmd.Parameters.AddWithValue( "@Member", Member );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_UserPMessage
		static public void userpmessage_delete( object userPMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_userpmessage_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserPMessageID", userPMessageID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable userpmessage_list( object userPMessageID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_userpmessage_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserPMessageID", userPMessageID );
				return GetData( cmd );
			}
		}
		#endregion

		#region yaf_WatchForum
		static public void watchforum_add( object UserID, object ForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchforum_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", UserID );
				cmd.Parameters.AddWithValue( "@ForumID", ForumID );
				ExecuteNonQuery( cmd );
			}
		}
		static public DataTable watchforum_list( object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
		static public DataTable watchforum_check( object userID, object forumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchforum_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@ForumID", forumID );
				return GetData( cmd );
			}
		}
		static public void watchforum_delete( object watchForumID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@WatchForumID", watchForumID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region yaf_WatchTopic
		static public DataTable watchtopic_list( object userID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchtopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				return GetData( cmd );
			}
		}
		static public DataTable watchtopic_check( object userID, object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchtopic_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				return GetData( cmd );
			}
		}
		static public void watchtopic_delete( object watchTopicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchtopic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@WatchTopicID", watchTopicID );
				ExecuteNonQuery( cmd );
			}
		}
		static public void watchtopic_add( object userID, object topicID )
		{
			using ( SqlCommand cmd = new SqlCommand( "yaf_watchtopic_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@UserID", userID );
				cmd.Parameters.AddWithValue( "@TopicID", topicID );
				ExecuteNonQuery( cmd );
			}
		}
		#endregion
	}

}
