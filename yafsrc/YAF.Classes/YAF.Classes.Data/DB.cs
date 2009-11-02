/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System.Web;
using System.Linq;
using System.Web.Hosting;
using System.Web.Security;

namespace YAF.Classes.Data
{
	/// <summary>
	/// All the Database functions for YAF
	/// </summary>
	public static partial class DB
	{
		#region ConnectionStringOptions
		public static string ProviderAssemblyName
		{
			get
			{
				return "System.Data.SqlClient";
			}
		}
		public static bool PasswordPlaceholderVisible
		{
			get
			{
				return false;
			}
		}

		// Text boxes : Parameters 1-10 
		//Parameter 1
		public static string Parameter1_Name
		{
			get
			{
				return "Data Source";
			}
		}
		public static string Parameter1_Value
		{
			get
			{
				return "(local)";
			}
		}

		public static bool Parameter1_Visible
		{
			get
			{
				return true;
			}
		}
		//Parameter 2
		public static string Parameter2_Name
		{
			get
			{
				return "Initial Catalog";
			}
		}
		public static string Parameter2_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter2_Visible
		{
			get
			{
				return true;
			}
		}
		//Parameter 3
		public static string Parameter3_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter3_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter3_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 4
		public static string Parameter4_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter4_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter4_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 5
		public static string Parameter5_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter5_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter5_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 6
		public static string Parameter6_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter6_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter6_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 7
		public static string Parameter7_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter7_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter7_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 8
		public static string Parameter8_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter8_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter8_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 9
		public static string Parameter9_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter9_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter9_Visible
		{
			get
			{
				return false;
			}
		}

		//Parameter 10
		public static string Parameter10_Name
		{
			get
			{
				return "";
			}
		}
		public static string Parameter10_Value
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter10_Visible
		{
			get
			{
				return false;
			}
		}
		//Check boxes: Parameters 11-19 

		//Parameter 11 hides user password placeholder! 

		public static string Parameter11_Name
		{
			get
			{
				return "Use Integrated Security";
			}
		}

		public static bool Parameter11_Value
		{
			get
			{
				return true;
			}
		}

		public static bool Parameter11_Visible
		{
			get
			{
				return true;
			}
		}
		//Parameter 12 (reserved for 'User Instance' in MS SQL SERVER)
		public static string Parameter12_Name
		{
			get
			{

				return "";
			}
		}

		public static bool Parameter12_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter12_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 13
		public static string Parameter13_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter13_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter13_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 14
		public static string Parameter14_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter14_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter14_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 15
		public static string Parameter15_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter15_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter15_Visible
		{
			get
			{
				return false;
			}
		}

		//Parameter 16
		public static string Parameter16_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter16_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter16_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 17
		public static string Parameter17_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter17_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter17_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 18
		public static string Parameter18_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter18_Value
		{
			get
			{
				return false;
			}
		}

		public static bool Parameter18_Visible
		{
			get
			{
				return false;
			}
		}
		//Parameter 19
		public static string Parameter19_Name
		{
			get
			{
				return "";
			}
		}

		public static bool Parameter19_Value
		{
			get
			{
				return false;
			}
		}

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
		/// Gets the database size
		/// </summary>
		/// <returns>intager value for database size</returns>
		public static int DBSize
		{
			get
			{
				using ( SqlCommand cmd = new SqlCommand( "select sum(cast(size as integer))/128 from sysfiles" ) )
				{
					cmd.CommandType = CommandType.Text;
					return (int)YafDBAccess.Current.ExecuteScalar( cmd );
				}
			}
		}

		public static bool IsForumInstalled
		{
			get
			{
				try
				{
					using ( DataTable dt = board_list( DBNull.Value ) )
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

		public static int DBVersion
		{
			get
			{
				try
				{
					using ( DataTable dt = registry_list( "version" ) )
					{
						if ( dt.Rows.Count > 0 )
						{
							// get the version...
							return Convert.ToInt32( dt.Rows[0]["Value"] );
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
		private static bool _fullTextSupported = true;

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

		private static string _fullTextScript = "mssql/fulltext.sql";

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

		private static readonly string[] _scriptList = {
		                                               	"mssql/tables.sql",
		                                               	"mssql/indexes.sql",
		                                               	"mssql/views.sql",
		                                               	"mssql/constraints.sql",
		                                               	"mssql/triggers.sql",
		                                               	"mssql/procedures.sql",
		                                               	"mssql/functions.sql",
		                                               	"mssql/providers/procedures.sql",
		                                               	"mssql/providers/tables.sql",
		                                               	"mssql/providers/indexes.sql"
		                                               };

		static public string[] ScriptList
		{
			get
			{
				return _scriptList;
			}
		}

		#endregion

		static private bool GetBooleanRegistryValue( string name )
		{
			using ( DataTable dt = YAF.Classes.Data.DB.registry_list( name ) )
			{
				foreach ( DataRow dr in dt.Rows )
				{
					int i;
					return int.TryParse( dr["Value"].ToString(), out i )
					       	? SqlDataLayerConverter.VerifyBool( i )
					       	: SqlDataLayerConverter.VerifyBool( dr["Value"] );
				}
			}
			return false;
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
					using ( SqlCommand cmd = YafDBAccess.GetCommand( "pageload" ) )
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
						using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
						{
							if ( dt.Rows.Count > 0 )
								return dt.Rows[0];
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
			// TODO: Break up this function to be more managable.

			bool bFirst = true;
			System.Text.StringBuilder forumIds = new System.Text.StringBuilder();

			if ( toSearchWhat == "*" )
			{
				toSearchWhat = "";
			}

			if ( forumIDToStartAt != 0 )
			{
				DataTable dt = forum_listall_sorted( boardId, userID, null, false, forumIDToStartAt );

				bFirst = true;
				foreach ( DataRow dr in dt.Rows )
				{
					if ( bFirst ) bFirst = false;
					else forumIds.Append( "," );

					forumIds.Append( Convert.ToString( Convert.ToInt32( dr["ForumID"] ) ) );
				}
			}

			// fix quotes for SQL insertion...
			toSearchWhat = toSearchWhat.Replace( "'", "''" ).Trim();
			toSearchFromWho = toSearchFromWho.Replace( "'", "''" ).Trim();

			string searchSql = ( maxResults == 0 ) ? "SELECT" : ( "SELECT TOP " + maxResults.ToString() );

			searchSql += " a.ForumID, a.TopicID, a.Topic, b.UserID, IsNull(c.Username, b.Name) as Name, c.MessageID, c.Posted, c.Message, c.Flags ";
			searchSql += "from {databaseOwner}.{objectQualifier}topic a left join {databaseOwner}.{objectQualifier}message c on a.TopicID = c.TopicID left join {databaseOwner}.{objectQualifier}user b on c.UserID = b.UserID join {databaseOwner}.{objectQualifier}vaccess x on x.ForumID=a.ForumID ";
			searchSql += String.Format( "where x.ReadAccess<>0 AND x.UserID={0} AND c.IsApproved = 1 AND a.TopicMovedID IS NULL AND a.IsDeleted = 0 AND c.IsDeleted = 0 ", userID );

			string[] words;

			if ( !String.IsNullOrEmpty( toSearchFromWho ) )
			{
				searchSql += "AND (";
				bFirst = true;

				// generate user search sql...
				switch ( searchFromWhoMethod )
				{
					case SearchWhatFlags.AllWords:
						words = toSearchFromWho.Replace( "\"", "" ).Split( ' ' );
						foreach ( string word in words )
						{
							if ( !bFirst ) searchSql += " AND "; else bFirst = false;
							searchSql += string.Format( " ((c.Username IS NULL AND b.Name LIKE N'%{0}%') OR (c.Username LIKE N'%{0}%'))", word );
						}
						break;
					case SearchWhatFlags.AnyWords:
						words = toSearchFromWho.Split( ' ' );
						foreach ( string word in words )
						{
							if ( !bFirst ) searchSql += " OR "; else bFirst = false;
							searchSql += string.Format( " ((c.Username IS NULL AND b.Name LIKE N'%{0}%') OR (c.Username LIKE N'%{0}%'))", word );
						}
						break;
					case SearchWhatFlags.ExactMatch:
						searchSql += string.Format( " ((c.Username IS NULL AND b.Name = N'{0}') OR (c.Username = N'{0}'))", toSearchFromWho );
						break;
				}
				searchSql += ") ";
			}


			if ( !String.IsNullOrEmpty( toSearchWhat ) )
			{
				searchSql += "AND (";
				bFirst = true;

				// generate message and topic search sql...
				switch ( searchWhatMethod )
				{
					case SearchWhatFlags.AllWords:
						words = toSearchWhat.Replace( "\"", "" ).Split( ' ' );
						if ( useFullText )
						{
							string ftInner = "";

							// make the inner FULLTEXT search
							foreach ( string word in words )
							{
								if ( !bFirst ) ftInner += " AND "; else bFirst = false;
								ftInner += String.Format( @"""{0}""", word );
							}
							// make final string...
							searchSql += string.Format( "( CONTAINS (c.Message, N' {0} ') OR CONTAINS (a.Topic, N' {0} ') )", ftInner );
						}
						else
						{
							foreach ( string word in words )
							{
								if ( !bFirst ) searchSql += " AND "; else bFirst = false;
								searchSql += String.Format( "(c.Message like N'%{0}%' OR a.Topic LIKE N'%{0}%')", word );
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
								if ( !bFirst ) ftInner += " OR "; else bFirst = false;
								ftInner += String.Format( @"""{0}""", word );
							}
							// make final string...
							searchSql += string.Format( "( CONTAINS (c.Message, N' {0} ') OR CONTAINS (a.Topic, N' {0} ') )", ftInner );
						}
						else
						{
							foreach ( string word in words )
							{
								if ( !bFirst ) searchSql += " OR "; else bFirst = false;
								searchSql += String.Format( "c.Message LIKE N'%{0}%' OR a.Topic LIKE N'%{0}%'", word );
							}
						}
						break;
					case SearchWhatFlags.ExactMatch:
						if ( useFullText )
						{
							searchSql += string.Format( "( CONTAINS (c.Message, N' \"{0}\" ') OR CONTAINS (a.Topic, N' \"{0}\" ') )", toSearchWhat );
						}
						else
						{
							searchSql += string.Format( "c.Message LIKE N'%{0}%' OR a.Topic LIKE N'%{0}%' ", toSearchWhat );
						}
						break;
				}
				searchSql += ") ";
			}

			// Ederon : 6/16/2007 - forum IDs start above 0, if forum id is 0, there is no forum filtering
			if ( forumIDToStartAt > 0 && forumIds.Length > 0 )
			{
				searchSql += string.Format( "AND a.ForumID IN ({0})", forumIds.ToString() );
			}

			searchSql += " ORDER BY c.Posted DESC";

			using ( SqlCommand cmd = YafDBAccess.GetCommand( searchSql, true ) )
			{
				return YafDBAccess.Current.GetData( cmd );
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
					using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( YafDBAccess.IsolationLevel ) )
					{
						using ( SqlDataAdapter da = new SqlDataAdapter( YafDBAccess.GetObjectName( "category_list" ), connMan.DBConnection ) )
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "BoardID", boardID );
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill( ds, YafDBAccess.GetObjectName( "Category" ) );
							da.SelectCommand.CommandText = YafDBAccess.GetObjectName( "forum_list" );
							da.Fill( ds, YafDBAccess.GetObjectName( "ForumUnsorted" ) );

							DataTable dtForumListSorted = ds.Tables[YafDBAccess.GetObjectName( "ForumUnsorted" )].Clone();
							dtForumListSorted.TableName = YafDBAccess.GetObjectName( "Forum" );
							ds.Tables.Add( dtForumListSorted );
							dtForumListSorted.Dispose();
							forum_list_sort_basic( ds.Tables[YafDBAccess.GetObjectName( "ForumUnsorted" )], ds.Tables[YafDBAccess.GetObjectName( "Forum" )], 0, 0 );
							ds.Tables.Remove( YafDBAccess.GetObjectName( "ForumUnsorted" ) );
							ds.Relations.Add( "FK_Forum_Category", ds.Tables[YafDBAccess.GetObjectName( "Category" )].Columns["CategoryID"], ds.Tables[YafDBAccess.GetObjectName( "Forum" )].Columns["CategoryID"] );
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
		/// <param name="boardID">ID of Board</param>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <returns></returns>
		static public DataTable accessmask_list( object boardID, object accessMaskID )
		{
			return accessmask_list( boardID, accessMaskID, 0 );
		}
		/// <summary>
		/// Gets a list of access mask properities
		/// </summary>
		/// <param name="boardID">ID of Board</param>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <param name="excludeFlags">Ommit access masks with this flags set.</param>
		/// <returns></returns>
		static public DataTable accessmask_list( object boardID, object accessMaskID, object excludeFlags )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "accessmask_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				cmd.Parameters.AddWithValue( "ExcludeFlags", excludeFlags );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Deletes an access mask
		/// </summary>
		/// <param name="accessMaskID">ID of access mask</param>
		/// <returns></returns>
		static public bool accessmask_delete( object accessMaskID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "accessmask_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd ) != 0;
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
		/// <param name="downloadAccess">Download Access?</param>
		static public void accessmask_save( object accessMaskID, object boardID, object name, object readAccess, object postAccess, object replyAccess, object priorityAccess, object pollAccess, object voteAccess, object moderatorAccess, object editAccess, object deleteAccess, object uploadAccess, object downloadAccess )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "accessmask_save" ) )
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
				cmd.Parameters.AddWithValue( "DownloadAccess", downloadAccess );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Active
		/// <summary>
		/// Gets list of active users
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="Guests"></param>
		/// <returns>Returns a DataTable of active users</returns>
		static public DataTable active_list( object boardID, object Guests, int activeTime, object styledNicks )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "active_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Guests", Guests );
				cmd.Parameters.AddWithValue( "ActiveTime", activeTime );
				cmd.Parameters.AddWithValue( "StyledNicks", styledNicks );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the list of active users within a certain forum
		/// </summary>
		/// <param name="forumID">forumID</param>
		/// <returns>DataTable of all ative users in a forum</returns>
		static public DataTable active_listforum( object forumID, object styledNicks )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "active_listforum" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
                cmd.Parameters.AddWithValue("StyledNicks", styledNicks);
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the list of active users in a topic
		/// </summary>
		/// <param name="topicID">ID of topic </param>
		/// <returns>DataTable of all users that are in a topic</returns>
        static public DataTable active_listtopic(object topicID, object styledNicks)
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "active_listtopic" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
                cmd.Parameters.AddWithValue( "StyledNicks", styledNicks );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		/// <summary>
		/// Gets the activity statistics for a board
		/// </summary>
		/// <param name="boardID">boardID</param>
		/// <returns>DataRow of activity stata</returns>
		static public DataRow active_stats( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "active_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
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
		/// <param name="messageID">messageID</param>
		/// <param name="attachmentID">attachementID</param>
		/// <param name="boardID">boardID</param>
		/// <returns>DataTable with attachement list</returns>
		static public DataTable attachment_list( object messageID, object attachmentID, object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_save" ) )
			{
				byte[] fileData = null;
				if ( stream != null )
				{
					fileData = new byte[stream.Length];
					stream.Seek( 0, System.IO.SeekOrigin.Begin );
					stream.Read( fileData, 0, (int)stream.Length );
				}
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "FileName", fileName );
				cmd.Parameters.AddWithValue( "Bytes", bytes );
				cmd.Parameters.AddWithValue( "ContentType", contentType );
				cmd.Parameters.AddWithValue( "FileData", fileData );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		//ABOT CHANGE 16.04.04
		/// <summary>
		/// Delete attachment
		/// </summary>
		/// <param name="attachmentID">ID of attachment to delete</param>
		static public void attachment_delete( object attachmentID )
		{
			bool useFileTable = GetBooleanRegistryValue( "UseFileTable" );

			//If the files are actually saved in the Hard Drive
			if ( !useFileTable )
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
					DataTable tbAttachments = YafDBAccess.Current.GetData( cmd );

					string uploadDir = HostingEnvironment.MapPath( String.Concat( UrlBuilder.FileRoot, YafBoardFolders.Current.Uploads ) );

					foreach ( DataRow row in tbAttachments.Rows )
					{
						try
						{
							string fileName = String.Format( "{0}/{1}.{2}", uploadDir, row["MessageID"], row["FileName"] );
							if ( File.Exists( fileName ) )
							{
								File.Delete( fileName );
							}
						}
						catch
						{
							// error deleting that file... 
						}
					}
				}
			}

			using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
			//End ABOT CHANGE 16.04.04
		}

		/// <summary>
		/// Attachement dowload
		/// </summary>
		/// <param name="attachmentID">ID of attachemnt to download</param>
		static public void attachment_download( object attachmentID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_download" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "AttachmentID", attachmentID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region BannedIP
		/// <summary>
		/// List of Baned IP's
		/// </summary>
		/// <param name="boardID">ID of board</param>
		/// <param name="ID">ID</param>
		/// <returns>DataTable of banned IPs</returns>
		static public DataTable bannedip_list( object boardID, object ID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bannedip_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ID", ID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bannedip_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Mask", Mask );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes Banned IP
		/// </summary>
		/// <param name="ID">ID of banned ip to delete</param>
		static public void bannedip_delete( object ID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bannedip_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Board
		/// <summary>
		/// Gets a list of information about a board
		/// </summary>
		/// <param name="boardID">board id</param>
		/// <returns>DataTable</returns>
		static public DataTable board_list( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Gets posting statistics
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <returns>DataRow of Poststats</returns>
		static public DataRow board_poststats( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_poststats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					return dt.Rows[0];
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_resync" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );

				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					return dt.Rows[0];
				}
			}
		}
		/// <summary>
		/// Saves board information
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="name">Name of Board</param>
		/// <param name="allowThreaded">Boolen value, allowThreaded</param>
		static public int board_save( object boardID, object name, object allowThreaded )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "AllowThreaded", allowThreaded );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}

		/// <summary>
		/// Creates a new board
		/// </summary>
		/// <param name="adminUsername">Membership Provider User Name</param>
		/// <param name="adminUserKey">Membership Provider User Key</param>
		/// <param name="boardName">Name of new board</param>
		/// <param name="boardMembershipName">Membership Provider Application Name for new board</param>
		/// <param name="boardRolesName">Roles Provider Application Name for new board</param>
		static public int board_create( object adminUsername, object adminUserKey, object boardName, object boardMembershipName, object boardRolesName )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_create" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardName", boardName );
				cmd.Parameters.AddWithValue( "MembershipAppName", boardMembershipName );
				cmd.Parameters.AddWithValue( "RolesAppName", boardRolesName );
				cmd.Parameters.AddWithValue( "UserName", adminUsername );
				cmd.Parameters.AddWithValue( "UserKey", adminUserKey );
				cmd.Parameters.AddWithValue( "IsHostAdmin", 0 );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		/// <summary>
		/// Deletes a board
		/// </summary>
		/// <param name="boardID">ID of board to delete</param>
		static public void board_delete( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "board_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Category
		/// <summary>
		/// Deletes a category
		/// </summary>
		/// <param name="CategoryID">ID of category to delete</param>
		/// <returns>Bool value indicationg if category was deleted</returns>
		static public bool category_delete( object CategoryID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "category_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "CategoryID", CategoryID );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd ) != 0;
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "category_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "category_listread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Lists categories very simply (for URL rewriting)
		/// </summary>
		/// <param name="StartID"></param>
		/// <param name="Limit"></param>
		/// <returns></returns>
		static public DataTable category_simplelist( int startID, int limit )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "category_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", startID );
				cmd.Parameters.AddWithValue( "Limit", limit );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Saves changes to a category
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="CategoryID">CategoryID so save changes to</param>
		/// <param name="Name">Name of the category</param>
		/// <param name="SortOrder">Sort Order</param>
		static public void category_save( object boardID, object categoryId, object name, object categoryImage, object sortOrder )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "category_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryId );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "CategoryImage", categoryImage );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region CheckEmail
		/// <summary>
		/// Saves a new email into the table for verification
		/// </summary>
		/// <param name="UserID">ID of user to verify</param>
		/// <param name="Hash">Hash of user</param>
		/// <param name="Email">email of user</param>
		static public void checkemail_save( object userID, object hash, object email )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "checkemail_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Hash", hash );
				cmd.Parameters.AddWithValue( "Email", email );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Updates a hash
		/// </summary>
		/// <param name="hash">New hash</param>
		/// <returns>DataTable with user information</returns>
		static public DataTable checkemail_update( object hash )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "checkemail_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Hash", hash );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Gets a check email entry based on email or all if no email supplied
		/// </summary>
		/// <param name="email">Associated email</param>
		/// <returns>DataTable with check email information</returns>
		static public DataTable checkemail_list( object email )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "checkemail_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Email", email );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		#endregion

		#region Choice
		/// <summary>
		/// Saves a vote in the database
		/// </summary>
		/// <param name="choiceID">Choice of the vote</param>
		static public void choice_vote( object choiceID, object userID, object remoteIP )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "choice_vote" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ChoiceID", choiceID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "RemoteIP", remoteIP );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region EventLog
		static public void eventlog_create( object userID, object source, object description, object type )
		{
			try
			{
				if ( userID == null ) userID = DBNull.Value;
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "eventlog_create" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "Type", (object)type );
					cmd.Parameters.AddWithValue( "UserID", (object)userID );
					cmd.Parameters.AddWithValue( "Source", (object)source.ToString() );
					cmd.Parameters.AddWithValue( "Description", (object)description.ToString() );
					YafDBAccess.Current.ExecuteNonQuery( cmd );
				}
			}
			catch
			{
				// Ignore any errors in this method
			}
		}


		static public void eventlog_create( object userID, object source, object description )
		{
			eventlog_create( userID, (object)source.GetType().ToString(), description, (object)0 );
		}

		/// <summary>
		/// Deletes all event log entries for given board.
		/// </summary>
		/// <param name="boardID">ID of board.</param>
		static public void eventlog_delete( int boardID )
		{
			eventlog_delete( null, boardID );
		}
		/// <summary>
		/// Deletes event log entry of given ID.
		/// </summary>
		/// <param name="eventLogID">ID of event log entry.</param>
		static public void eventlog_delete( object eventLogID )
		{
			eventlog_delete( eventLogID, null );
		}
		/// <summary>
		/// Calls underlying stroed procedure for deletion of event log entry(ies).
		/// </summary>
		/// <param name="eventLogID">When not null, only given event log entry is deleted.</param>
		/// <param name="boardID">Specifies board. It is ignored if eventLogID parameter is not null.</param>
		static private void eventlog_delete( object eventLogID, object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "eventlog_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "EventLogID", eventLogID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public DataTable eventlog_list( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "eventlog_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		#endregion EventLog

		#region Extensions

		static public void extension_delete( object extensionId )
		{
			try
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "extension_delete" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "ExtensionId", extensionId );
					YafDBAccess.Current.ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "extension_edit" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "extensionId", extensionId );
				return YafDBAccess.Current.GetData( cmd );
			}

		}

		// Used to validate a file before uploading
		static public DataTable extension_list( object boardID, object extension )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "extension_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Extension", extension );
				return YafDBAccess.Current.GetData( cmd );
			}

		}

		// Returns an extension list for a given Board
		static public DataTable extension_list( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "extension_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Extension", "" );
				return YafDBAccess.Current.GetData( cmd );
			}

		}

		// Saves / creates extension
		static public void extension_save( object extensionId, object boardID, object Extension )
		{
			try
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "extension_save" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "extensionId", extensionId );
					cmd.Parameters.AddWithValue( "BoardId", boardID );
					cmd.Parameters.AddWithValue( "Extension", Extension );
					YafDBAccess.Current.ExecuteNonQuery( cmd );
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
		/// <param name="choiceID">Choice of the vote</param>
		static public DataTable pollvote_check( object pollid, object userid, object remoteip )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pollvote_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollid );
				cmd.Parameters.AddWithValue( "UserID", userid );
				cmd.Parameters.AddWithValue( "RemoteIP", remoteip );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		#endregion

		#region Forum
		//ABOT NEW 16.04.04
		/// <summary>
		/// Deletes attachments out of a entire forum
		/// </summary>
		/// <param name="ForumID">ID of forum to delete all attachemnts out of</param>
		static private void forum_deleteAttachments( object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listtopics" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						if ( row != null && row["TopicID"] != DBNull.Value )
						{
							topic_delete( row["TopicID"], true );
						}
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listSubForums" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				if ( YafDBAccess.Current.ExecuteScalar( cmd ) is DBNull )
				{
					forum_deleteAttachments( forumID );
					using ( SqlCommand cmd_new = YafDBAccess.GetCommand( "forum_delete" ) )
					{
						cmd_new.CommandType = CommandType.StoredProcedure;
						cmd_new.CommandTimeout = 99999;
						cmd_new.Parameters.AddWithValue( "ForumID", forumID );
						YafDBAccess.Current.ExecuteNonQuery( cmd_new );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listallmymoderated" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listall" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Root", startAt );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listall_fromCat" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );

				int intCategoryID = Convert.ToInt32( categoryID.ToString() );

				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listpath" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_listread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				cmd.Parameters.AddWithValue( "ParentID", parentID );
				return YafDBAccess.Current.GetData( cmd );
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
					using ( SqlDataAdapter da = new SqlDataAdapter( YafDBAccess.GetObjectName( "category_list" ), connMan.OpenDBConnection ) )
					{
						using ( SqlTransaction trans = da.SelectCommand.Connection.BeginTransaction( YafDBAccess.IsolationLevel ) )
						{
							da.SelectCommand.Transaction = trans;
							da.SelectCommand.Parameters.AddWithValue( "BoardID", boardID );
							da.SelectCommand.CommandType = CommandType.StoredProcedure;
							da.Fill( ds, YafDBAccess.GetObjectName( "Category" ) );
							da.SelectCommand.CommandText = YafDBAccess.GetObjectName( "forum_moderatelist" );
							da.SelectCommand.Parameters.AddWithValue( "UserID", userID );
							da.Fill( ds, YafDBAccess.GetObjectName( "ForumUnsorted" ) );
							DataTable dtForumListSorted = ds.Tables[YafDBAccess.GetObjectName( "ForumUnsorted" )].Clone();
							dtForumListSorted.TableName = YafDBAccess.GetObjectName( "Forum" );
							ds.Tables.Add( dtForumListSorted );
							dtForumListSorted.Dispose();
							forum_list_sort_basic( ds.Tables[YafDBAccess.GetObjectName( "ForumUnsorted" )], ds.Tables[YafDBAccess.GetObjectName( "Forum" )], 0, 0 );
							ds.Tables.Remove( YafDBAccess.GetObjectName( "ForumUnsorted" ) );
							ds.Relations.Add( "FK_Forum_Category", ds.Tables[YafDBAccess.GetObjectName( "Category" )].Columns["CategoryID"], ds.Tables[YafDBAccess.GetObjectName( "Forum" )].Columns["CategoryID"] );
							trans.Commit();
						}
						return ds;
					}
				}
			}
		}

		static public DataTable forum_moderators()
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_moderators" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_resync" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public long forum_save( object forumID, object categoryID, object parentID, object name, object description, object sortOrder, object locked, object hidden, object isTest, object moderated, object accessMaskID, object remoteURL, object themeURL, bool dummy )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forum_save" ) )
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
				return long.Parse( YafDBAccess.Current.ExecuteScalar( cmd ).ToString() );
			}
		}

		static private DataTable forum_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent, int[] forumidExclusions )
		{
			return forum_sort_list( listSource, parentID, categoryID, startingIndent, forumidExclusions, true );
		}

		static private DataTable forum_sort_list( DataTable listSource, int parentID, int categoryID, int startingIndent, int[] forumidExclusions, bool emptyFirstRow )
		{
			DataTable listDestination = new DataTable();

			listDestination.Columns.Add( "ForumID", typeof( String ) );
			listDestination.Columns.Add( "Title", typeof( String ) );

			if ( emptyFirstRow )
			{
				DataRow blankRow = listDestination.NewRow();
				blankRow["ForumID"] = string.Empty;
				blankRow["Title"] = string.Empty;
				listDestination.Rows.Add( blankRow );
			}
			// filter the forum list
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

		static public DataTable forum_listall_sorted( object boardID, object userID, int[] forumidExclusions )
		{
			return forum_listall_sorted( boardID, userID, null, false, 0 );
		}

		static public DataTable forum_listall_sorted( object boardID, object userID, int[] forumidExclusions, bool emptyFirstRow, int startAt )
		{
			using ( DataTable dataTable = forum_listall( boardID, userID, startAt ) )
			{
				int baseForumId = 0;
				int baseCategoryId = 0;

				if ( startAt != 0 )
				{
					// find the base ids...
					foreach ( DataRow dataRow in dataTable.Rows )
					{
						if ( Convert.ToInt32( dataRow["ForumID"] ) == startAt && dataRow["ParentID"] != DBNull.Value && dataRow["CategoryID"] != DBNull.Value )
						{
							baseForumId = Convert.ToInt32( dataRow["ParentID"] );
							baseCategoryId = Convert.ToInt32( dataRow["CategoryID"] );
							break;
						}
					}
				}

				return forum_sort_list( dataTable, baseForumId, baseCategoryId, 0, forumidExclusions, emptyFirstRow );
			}
		}

		static private void forum_list_sort_basic( DataTable listsource, DataTable list, int parentid, int currentLvl )
		{
			for ( int i = 0; i < listsource.Rows.Count; i++ )
			{
				DataRow row = listsource.Rows[i];
				if ( ( row["ParentID"] ) == DBNull.Value )
					row["ParentID"] = 0;

				if ( (int)row["ParentID"] == parentid )
				{
					string sIndent = "";
					int iIndent = Convert.ToInt32( currentLvl );
					for ( int j = 0; j < iIndent; j++ ) sIndent += "--";
					row["Name"] = string.Format( " -{0} {1}", sIndent, row["Name"] );
					list.Rows.Add( row.ItemArray );
					forum_list_sort_basic( listsource, list, (int)row["ForumID"], currentLvl + 1 );
				}
			}
		}

		static private void forum_sort_list_recursive( DataTable listSource, DataTable listDestination, int parentID, int categoryID, int currentIndent )
		{
			DataRow newRow;

			foreach ( DataRow row in listSource.Rows )
			{
				// see if this is a root-forum
				if ( row["ParentID"] == DBNull.Value )
					row["ParentID"] = 0;

				if ( (int)row["ParentID"] == parentID )
				{
					if ( (int)row["CategoryID"] != categoryID )
					{
						categoryID = (int)row["CategoryID"];

						newRow = listDestination.NewRow();
						newRow["ForumID"] = -categoryID;		// Ederon : 9/4/2007
						newRow["Title"] = string.Format( "{0}", row["Category"].ToString() );
						listDestination.Rows.Add( newRow );
					}

					string sIndent = "";

					for ( int j = 0; j < currentIndent; j++ )
						sIndent += "--";

					// import the row into the destination
					newRow = listDestination.NewRow();

					newRow["ForumID"] = row["ForumID"];
					newRow["Title"] = string.Format( " -{0} {1}", sIndent, row["Forum"] );

					listDestination.Rows.Add( newRow );

					// recurse through the list...
					forum_sort_list_recursive( listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1 );
				}
			}
		}

		#endregion

		#region ForumAccess
		static public DataTable forumaccess_list( object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forumaccess_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void forumaccess_save( object forumID, object groupID, object accessMaskID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forumaccess_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable forumaccess_group( object groupID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "forumaccess_group" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				return userforumaccess_sort_list( YafDBAccess.Current.GetData( cmd ), 0, 0, 0 );
			}
		}
		#endregion

		#region Group
		static public DataTable group_list( object boardID, object groupID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void group_delete( object groupID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable group_member( object boardID, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_member" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public long group_save( object groupID, object boardID, object name, object isAdmin, object isGuest, object isStart, object isModerator, object accessMaskID, object pmLimit, object style, object sortOrder )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_save" ) )
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
				cmd.Parameters.AddWithValue( "PMLimit", pmLimit );
				cmd.Parameters.AddWithValue( "Style", style );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				return long.Parse( YafDBAccess.Current.ExecuteScalar( cmd ).ToString() );
			}
		}
		#endregion

		#region Mail
		static public void mail_delete( object mailID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "mail_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MailID", mailID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable mail_list( object processId )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "mail_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ProcessID", processId );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void mail_createwatch( object topicID, object from, object fromName, object subject, object body, object bodyHtml, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "mail_createwatch" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "From", from );
				cmd.Parameters.AddWithValue( "FromName", fromName );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "BodyHtml", bodyHtml );
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void mail_create( object from, object fromName, object to, object toName, object subject, object body, object bodyHtml )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "mail_create" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "From", from );
				cmd.Parameters.AddWithValue( "FromName", fromName );
				cmd.Parameters.AddWithValue( "To", to );
				cmd.Parameters.AddWithValue( "ToName", toName );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "BodyHtml", bodyHtml );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Message

		static public DataTable post_list( object topicID, object updateViewCount, bool showDeleted, bool styledNicks )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "post_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "UpdateViewCount", updateViewCount );
				cmd.Parameters.AddWithValue( "ShowDeleted", showDeleted );
                cmd.Parameters.AddWithValue( "StyledNicks", styledNicks );                
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable post_list_reverse10( object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "post_list_reverse10" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable post_last10user( object boardID, object userID, object pageUserID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "post_last10user" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "PageUserID", pageUserID );
				return YafDBAccess.Current.GetData( cmd );
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

			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_reply_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable dtr = YafDBAccess.Current.GetData( cmd );

				for ( int i = 0; i < dtr.Rows.Count; i++ )
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
					list.Rows.Add( newRow );
					message_getRepliesList_populate( dtr, list, (int)row["MessageId"] );
				}
				return list;
			}
		}

		// gets list of nested replies to message
		static private void message_getRepliesList_populate( DataTable listsource, DataTable list, int messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_reply_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable dtr = YafDBAccess.Current.GetData( cmd );

				for ( int i = 0; i < dtr.Rows.Count; i++ )
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
					list.Rows.Add( newRow );
					message_getRepliesList_populate( dtr, list, (int)row["MessageId"] );
				}
			}

		}

		//creates new topic, using some parameters from message itself
		static public long topic_create_by_message( object messageID, object forumId, object newTopicSubj )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_create_by_message" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "ForumID", forumId );
				cmd.Parameters.AddWithValue( "Subject", newTopicSubj );
				DataTable dt = YafDBAccess.Current.GetData( cmd );
				return long.Parse( dt.Rows[0]["TopicID"].ToString() );
			}
		}

		static public DataTable message_list( object messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public void message_delete( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked )
		{
			message_delete( messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false );
		}
		static public void message_delete( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked, bool eraseMessage )
		{
			message_deleteRecursively( messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, false, eraseMessage );
		}

		// <summary> Retrieve all reported messages with the correct forumID argument. </summary>
		static public DataTable message_listreported( object messageFlag, object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_listreported" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "MessageFlag", messageFlag );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		// <summary> Save reported message back to the database. </summary>
		static public void message_report( object reportFlag, object messageID, object userID, object reportedDateTime )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_report" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ReportFlag", reportFlag );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "ReporterID", userID );
				cmd.Parameters.AddWithValue( "ReportedDate", reportedDateTime );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		// <summary> Copy current Message text over reported Message text. </summary>
		static public void message_reportcopyover( object messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_reportcopyover" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		// <summary> Copy current Message text over reported Message text. </summary>
		static public void message_reportresolve( object messageFlag, object messageID, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_reportresolve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageFlag", messageFlag );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		//BAI ADDED 30.01.2004
		// <summary> Delete message and all subsequent releated messages to that ID </summary>
		static private void message_deleteRecursively( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool DeleteLinked, bool isLinked )
		{
			message_deleteRecursively( messageID, isModeratorChanged, deleteReason, isDeleteAction, DeleteLinked, isLinked, false );
		}
		static private void message_deleteRecursively( object messageID, bool isModeratorChanged, string deleteReason, int isDeleteAction, bool deleteLinked, bool isLinked, bool eraseMessages )
		{
			bool useFileTable = GetBooleanRegistryValue( "UseFileTable" );

			if ( deleteLinked )
			{
				//Delete replies
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_getReplies" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbReplies = YafDBAccess.Current.GetData( cmd );

					foreach ( DataRow row in tbReplies.Rows )
						message_deleteRecursively( row["MessageID"], isModeratorChanged, deleteReason, isDeleteAction, deleteLinked, true, eraseMessages );
				}
			}

			//If the files are actually saved in the Hard Drive
			if ( !useFileTable )
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "attachment_list" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbAttachments = YafDBAccess.Current.GetData( cmd );

					string uploadDir = HostingEnvironment.MapPath( String.Concat( UrlBuilder.FileRoot, YafBoardFolders.Current.Uploads ) );

					foreach ( DataRow row in tbAttachments.Rows )
					{
						try
						{
							string fileName = String.Format( "{0}/{1}.{2}", uploadDir, messageID, row["FileName"] );
							if ( File.Exists( fileName ) )
							{
								File.Delete( fileName );
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
			if ( eraseMessages )
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_delete" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					cmd.Parameters.AddWithValue( "EraseMessage", eraseMessages );
					YafDBAccess.Current.ExecuteNonQuery( cmd );
				}
			}
			else
			{
				//Delete Message
				// undelete function added
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_deleteundelete" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					cmd.Parameters.AddWithValue( "isModeratorChanged", isModeratorChanged );
					cmd.Parameters.AddWithValue( "DeleteReason", deleteReason );
					cmd.Parameters.AddWithValue( "isDeleteAction", isDeleteAction );
					YafDBAccess.Current.ExecuteNonQuery( cmd );
				}
			}
		}

		// <summary> Set flag on message to approved and store in DB </summary>
		static public void message_approve( object messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		// <summary> Update message to DB. </summary>
		static public void message_update( object messageID, object priority, object message, object subject, object flags, object reasonOfEdit, object isModeratorChanged )
		{
			message_update( messageID, priority, message, subject, flags, reasonOfEdit, isModeratorChanged, null );
		}
		static public void message_update( object messageID, object priority, object message, object subject, object flags, object reasonOfEdit, object isModeratorChanged, object overrideApproval )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "Priority", priority );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Flags", flags );
				cmd.Parameters.AddWithValue( "Reason", reasonOfEdit );
				cmd.Parameters.AddWithValue( "IsModeratorChanged", isModeratorChanged );
				cmd.Parameters.AddWithValue( "OverrideApproval", overrideApproval );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		// <summary> Save message to DB. </summary>
		static public bool message_save( object topicID, object userID, object message, object userName, object ip, object posted, object replyTo, object flags, ref long messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_save" ) )
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
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				messageID = (long)paramMessageID.Value;
				return true;
			}
		}
		static public DataTable message_unapproved( object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_unapproved" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable message_findunread( object topicID, object lastRead )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_findunread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "LastRead", lastRead );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		// message movind function
		static public void message_move( object messageID, object moveToTopic, bool moveAll )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_move" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "MoveToTopic", moveToTopic );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
			//moveAll=true anyway
			// it's in charge of moving answers of moved post
			if ( moveAll )
			{
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_getReplies" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue( "MessageID", messageID );
					DataTable tbReplies = YafDBAccess.Current.GetData( cmd );
					foreach ( DataRow row in tbReplies.Rows )
					{
						message_moveRecursively( row["MessageID"], moveToTopic );
					}

				}
			}
		}

		//moves answers of moved post
		static private void message_moveRecursively( object messageID, object moveToTopic )
		{
			bool UseFileTable = GetBooleanRegistryValue( "UseFileTable" );

			//Delete replies
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_getReplies" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				DataTable tbReplies = YafDBAccess.Current.GetData( cmd );
				foreach ( DataRow row in tbReplies.Rows )
				{
					message_moveRecursively( row["messageID"], moveToTopic );
				}
				using ( SqlCommand innercmd = YafDBAccess.GetCommand( "message_move" ) )
				{
					innercmd.CommandType = CommandType.StoredProcedure;
					innercmd.Parameters.AddWithValue( "MessageID", messageID );
					innercmd.Parameters.AddWithValue( "MoveToTopic", moveToTopic );
					YafDBAccess.Current.ExecuteNonQuery( innercmd );
				}
			}
		}
		// functions for Thanks feature

		// <summary> Checks if the message with the provided messageID is thanked 
		//           by the user with the provided UserID. if so, returns true,
		//           otherwise returns false. </summary>
		static public bool message_isThankedByUser( object userID, object messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_isthankedbyuser" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter paramOutput = new SqlParameter();
				paramOutput.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.Add( paramOutput );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return Convert.ToBoolean( paramOutput.Value );
			}
		}

		// <summary> Return the number of times the message with the provided messageID
		//           has been thanked. </summary>
		static public int message_ThanksNumber( object messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_thanksnumber" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter paramOutput = new SqlParameter();
				paramOutput.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.Add( paramOutput );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return Convert.ToInt32( paramOutput.Value );
			}
		}

		// <summary> Returns the UserIDs and UserNames who have thanked the message
		//           with the provided messageID. </summary>
		static public DataTable message_GetThanks( object MessageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_getthanks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "MessageID", MessageID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public string message_AddThanks( object FromUserID, object MessageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_Addthanks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter paramOutput = new SqlParameter( "paramOutput", SqlDbType.NVarChar, 50 );
				paramOutput.Direction = ParameterDirection.Output;
				cmd.Parameters.AddWithValue( "FromUserID", FromUserID );
				cmd.Parameters.AddWithValue( "MessageID", MessageID );
				cmd.Parameters.Add( paramOutput );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return ( paramOutput.Value.ToString() );
			}
		}

		static public string message_RemoveThanks( object FromUserID, object MessageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "message_Removethanks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter paramOutput = new SqlParameter( "paramOutput", SqlDbType.NVarChar, 50 );
				paramOutput.Direction = ParameterDirection.Output;
				cmd.Parameters.AddWithValue( "FromUserID", FromUserID );
				cmd.Parameters.AddWithValue( "MessageID", MessageID );
				cmd.Parameters.Add( paramOutput );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return ( paramOutput.Value.ToString() );
			}
		}

		#endregion

		#region Medal

		/// <summary>
		/// Lists given medal.
		/// </summary>
		/// <param name="medalID">ID of medal to list.</param>
		static public DataTable medal_list( object medalID )
		{
			return medal_list( null, medalID, null );
		}
		/// <summary>
		/// Lists given medals.
		/// </summary>
		/// <param name="boardID">ID of board of which medals to list. Required.</param>
		/// <param name="category">Cateogry of medals to list. Can be null. In such case this parameter is ignored.</param>
		static public DataTable medal_list( object boardID, object category )
		{
			return medal_list( boardID, null, category );
		}
		/// <summary>
		/// Lists medals.
		/// </summary>
		/// <param name="boardID">ID of board of which medals to list. Can be null if medalID parameter is specified.</param>
		/// <param name="medalID">ID of medal to list. When specified, boardID and category parameters are ignored.</param>
		/// <param name="category">Cateogry of medals to list. Must be complemented with not-null boardID parameter.</param>
		static private DataTable medal_list( object boardID, object medalID, object category )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "medal_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Category", category );

				return YafDBAccess.Current.GetData( cmd );
			}
		}


		/// <summary>
		/// List users who own this medal.
		/// </summary>
		/// <param name="medalID">Medal of which owners to get.</param>
		/// <returns>List of users with their user id and usernames, who own this medal.</returns>
		static public DataTable medal_listusers( object medalID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "medal_listusers" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "MedalID", medalID );

				return YafDBAccess.Current.GetData( cmd );
			}
		}


		/// <summary>
		/// Deletes given medal.
		/// </summary>
		/// <param name="medalID">ID of medal to delete.</param>
		static public void medal_delete( object medalID )
		{
			medal_delete( null, medalID, null );
		}
		/// <summary>
		/// Deletes given medals.
		/// </summary>
		/// <param name="boardID">ID of board of which medals to delete. Required.</param>
		/// <param name="category">Cateogry of medals to delete. Can be null. In such case this parameter is ignored.</param>
		static public void medal_delete( object boardID, object category )
		{
			medal_delete( boardID, null, category );
		}
		/// <summary>
		/// Deletes medals.
		/// </summary>
		/// <param name="boardID">ID of board of which medals to delete. Can be null if medalID parameter is specified.</param>
		/// <param name="medalID">ID of medal to delete. When specified, boardID and category parameters are ignored.</param>
		/// <param name="category">Cateogry of medals to delete. Must be complemented with not-null boardID parameter.</param>
		static private void medal_delete( object boardID, object medalID, object category )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "medal_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Category", category );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}


		/// <summary>
		/// Saves new medal or updates existing one. 
		/// </summary>
		/// <param name="boardID">ID of a board.</param>
		/// <param name="medalID">ID of medal to update. Null if new medal is being created.</param>
		/// <param name="name">Name of medal.</param>
		/// <param name="description">Description of medal.</param>
		/// <param name="message">Defaukt message to display. Should briefly describe why was medal awarded to user.</param>
		/// <param name="category">Category of medal.</param>
		/// <param name="medalURL">URL of medal's image.</param>
		/// <param name="ribbonURL">URL of medal's ribbon bar. Can be null.</param>
		/// <param name="smallMedalURL">URL of medal's small image. This one is displayed in user box.</param>
		/// <param name="smallRibbonURL">URL of medal's small ribbon bar. This one is eventually displayed in user box. Can be null.</param>
		/// <param name="smallMedalWidth">Width of small medal's image, in pixels.</param>
		/// <param name="smallMedalHeight">Height of small medal's image, in pixels.</param>
		/// <param name="smallRibbonWidth">Width of small medal's ribbon bar image, in pixels.</param>
		/// <param name="smallRibbonHeight">Width of small medal's ribbon bar image, in pixels.</param>
		/// <param name="sortOrder">Default order of medal as it will be displayed in user box.</paramHeight
		/// <param name="flags">Medal's flags.</param>
		/// <returns>True if medal was successfully created or updated. False otherwise.</returns>
		static public bool medal_save(
			object boardID, object medalID, object name, object description, object message, object category,
			object medalURL, object ribbonURL, object smallMedalURL, object smallRibbonURL, object smallMedalWidth,
			object smallMedalHeight, object smallRibbonWidth, object smallRibbonHeight, object sortOrder, object flags )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "medal_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Description", description );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Category", category );
				cmd.Parameters.AddWithValue( "MedalURL", medalURL );
				cmd.Parameters.AddWithValue( "RibbonURL", ribbonURL );
				cmd.Parameters.AddWithValue( "SmallMedalURL", smallMedalURL );
				cmd.Parameters.AddWithValue( "SmallRibbonURL", smallRibbonURL );
				cmd.Parameters.AddWithValue( "SmallMedalWidth", smallMedalWidth );
				cmd.Parameters.AddWithValue( "SmallMedalHeight", smallMedalHeight );
				cmd.Parameters.AddWithValue( "SmallRibbonWidth", smallRibbonWidth );
				cmd.Parameters.AddWithValue( "SmallRibbonHeight", smallRibbonHeight );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				cmd.Parameters.AddWithValue( "Flags", flags );

				// command succeeded if returned value is greater than zero (number of affected rows)
				return (int)YafDBAccess.Current.ExecuteScalar( cmd ) > 0;
			}
		}


		/// <summary>
		/// Changes medal's sort order.
		/// </summary>
		/// <param name="boardID">ID of board.</param>
		/// <param name="medalID">ID of medal to re-sort.</param>
		/// <param name="move">Change of sort.</param>
		static public void medal_resort( object boardID, object medalID, int move )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "medal_resort" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Move", move );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}


		/// <summary>
		/// Deletes medal allocation to a group.
		/// </summary>
		/// <param name="groupID">ID of group owning medal.</param>
		/// <param name="medalID">ID of medal.</param>
		static public void group_medal_delete( object groupID, object medalID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_medal_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}


		/// <summary>
		/// Lists medal(s) assigned to the group
		/// </summary>
		/// <param name="groupID">ID of group of which to list medals.</param>
		/// <param name="medalID">ID of medal to list.</param>
		static public DataTable group_medal_list( object groupID, object medalID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_medal_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );

				return YafDBAccess.Current.GetData( cmd );
			}
		}


		/// <summary>
		/// Saves new or update existing group-medal allocation.
		/// </summary>
		/// <param name="groupID">ID of user group.</param>
		/// <param name="medalID">ID of medal.</param>
		/// <param name="message">Medal message, to override medal's default one. Can be null.</param>
		/// <param name="hide">Hide medal in user box.</param>
		/// <param name="onlyRibbon">Show only ribbon bar in user box.</param>
		/// <param name="sortOrder">Sort order in user box. Overrides medal's default sort order.</param>
		static public void group_medal_save(
			object groupID, object medalID, object message,
			object hide, object onlyRibbon, object sortOrder )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "group_medal_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Hide", hide );
				cmd.Parameters.AddWithValue( "OnlyRibbon", onlyRibbon );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}



		/// <summary>
		/// Deletes medal allocation to a user.
		/// </summary>
		/// <param name="userID">ID of user owning medal.</param>
		/// <param name="medalID">ID of medal.</param>
		static public void user_medal_delete( object userID, object medalID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_medal_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}


		/// <summary>
		/// Lists medal(s) assigned to the group
		/// </summary>
		/// <param name="userID">ID of user who was given medal.</param>
		/// <param name="medalID">ID of medal to list.</param>
		static public DataTable user_medal_list( object userID, object medalID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_medal_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );

				return YafDBAccess.Current.GetData( cmd );
			}
		}


		/// <summary>
		/// Saves new or update existing user-medal allocation.
		/// </summary>
		/// <param name="userID">ID of user.</param>
		/// <param name="medalID">ID of medal.</param>
		/// <param name="message">Medal message, to override medal's default one. Can be null.</param>
		/// <param name="hide">Hide medal in user box.</param>
		/// <param name="onlyRibbon">Show only ribbon bar in user box.</param>
		/// <param name="sortOrder">Sort order in user box. Overrides medal's default sort order.</param>
		/// <param name="dateAwarded">Date when medal was awarded to a user. Is ignored when existing user-medal allocation is edited.</param>
		static public void user_medal_save(
			object userID, object medalID, object message,
			object hide, object onlyRibbon, object sortOrder, object dateAwarded )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_medal_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "MedalID", medalID );
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "Hide", hide );
				cmd.Parameters.AddWithValue( "OnlyRibbon", onlyRibbon );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				cmd.Parameters.AddWithValue( "DateAwarded", dateAwarded );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}


		/// <summary>
		/// Lists all medals held by user as they are to be shown in user box.
		/// </summary>
		/// <param name="userID">ID of user.</param>
		/// <returns>List of medals, ribbon bar only first.</returns>
		static public DataTable user_listmedals( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_listmedals" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue( "UserID", userID );

				return YafDBAccess.Current.GetData( cmd );
			}
		}

		#endregion

		#region NntpForum
		static public DataTable nntpforum_list( object boardID, object minutes, object nntpForumID, object active )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Minutes", minutes );
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "Active", active );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void nntpforum_update( object nntpForumID, object lastMessageNo, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpforum_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "LastMessageNo", lastMessageNo );
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_save( object nntpForumID, object nntpServerID, object groupName, object forumID, object active )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "GroupName", groupName );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Active", active );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpforum_delete( object nntpForumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpForumID", nntpForumID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region NntpServer
		static public DataTable nntpserver_list( object boardID, object nntpServerID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpserver_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void nntpserver_save( object nntpServerID, object boardID, object name, object address, object port, object userName, object userPass )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpserver_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Address", address );
				cmd.Parameters.AddWithValue( "Port", port );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "UserPass", userPass );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void nntpserver_delete( object nntpServerID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntpserver_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NntpServerID", nntpServerID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region NntpTopic
		static public DataTable nntptopic_list( object thread )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntptopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Thread", thread );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void nntptopic_savemessage( object nntpForumID, object topic, object body, object userID, object userName, object ip, object posted, object thread )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "nntptopic_savemessage" ) )
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
				YafDBAccess.Current.ExecuteNonQuery( cmd );
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
		/// <param name="toUserID"></param>
		/// <param name="fromUserID"></param>
		/// <param name="pMessageID">The id of the private message</param>
		/// <returns></returns>
		static public DataTable pmessage_list( object toUserID, object fromUserID, object userPMessageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "UserPMessageID", userPMessageID );
				return YafDBAccess.Current.GetData( cmd );
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
		static public DataTable pmessage_list( object userPMessageID )
		{
			return pmessage_list( null, null, userPMessageID );
		}
		/// <summary>
		/// Deletes the private message from the database as per the given parameter.  If <paramref name="fromOutbox"/> is true,
		/// the message is only removed from the user's outbox.  Otherwise, it is completely delete from the database.
		/// </summary>
		/// <param name="pMessageID"></param>
		/// <param name="fromOutbox">If true, removes the message from the outbox.  Otherwise deletes the message completely.</param>
		static public void pmessage_delete( object userPMessageID, bool fromOutbox )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserPMessageID", userPMessageID );
				cmd.Parameters.AddWithValue( "FromOutbox", fromOutbox );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
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
		public static void pmessage_archive( object userPMessageID )
		{
			using ( SqlCommand sqlCommand = YafDBAccess.GetCommand( "pmessage_archive" ) )
			{
				sqlCommand.CommandType = CommandType.StoredProcedure;
				sqlCommand.Parameters.AddWithValue( "UserPMessageID", userPMessageID );
				YafDBAccess.Current.ExecuteNonQuery( sqlCommand );
			}
		}

		static public void pmessage_save( object fromUserID, object toUserID, object subject, object body, object Flags )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "FromUserID", fromUserID );
				cmd.Parameters.AddWithValue( "ToUserID", toUserID );
				cmd.Parameters.AddWithValue( "Subject", subject );
				cmd.Parameters.AddWithValue( "Body", body );
				cmd.Parameters.AddWithValue( "Flags", Flags );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void pmessage_markread( object userPMessageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_markread" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserPMessageID", userPMessageID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable pmessage_info()
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void pmessage_prune( object daysRead, object daysUnread )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "pmessage_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "DaysRead", daysRead );
				cmd.Parameters.AddWithValue( "DaysUnread", daysUnread );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Poll
		static public DataTable poll_stats( object pollID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "poll_stats" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public int poll_save( object question, object c1, object c2, object c3, object c4, object c5, object c6, object c7, object c8, object c9, object closes )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "poll_save" ) )
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
				cmd.Parameters.AddWithValue( "Closes", closes );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		static public void poll_update( object pollID, object question, object closes )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "poll_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollID );
				cmd.Parameters.AddWithValue( "Question", question );
				cmd.Parameters.AddWithValue( "Closes", closes );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void poll_remove( object pollID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "poll_remove" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public void choice_delete( object choiceID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "choice_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ChoiceID", choiceID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void choice_update( object choiceID, object choice )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "choice_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ChoiceID", choiceID );
				cmd.Parameters.AddWithValue( "Choice", choice );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void choice_add( object pollID, object choice )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "choice_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "PollID", pollID );
				cmd.Parameters.AddWithValue( "Choice", choice );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		#endregion

		#region Rank
		static public DataTable rank_list( object boardID, object rankID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "rank_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void rank_save( object rankID, object boardID, object name, object isStart, object isLadder, object minPosts, object rankImage, object pmLimit, object style, object sortOrder )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "rank_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "RankID", rankID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "IsStart", isStart );
				cmd.Parameters.AddWithValue( "IsLadder", isLadder );
				cmd.Parameters.AddWithValue( "MinPosts", minPosts );
				cmd.Parameters.AddWithValue( "RankImage", rankImage );
				cmd.Parameters.AddWithValue( "PMLimit", pmLimit );
				cmd.Parameters.AddWithValue( "Style", style );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );

				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void rank_delete( object rankID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "rank_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "RankID", rankID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Smiley
		static public DataTable smiley_list( object boardID, object smileyID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "smiley_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable smiley_listunique( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "smiley_listunique" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void smiley_delete( object smileyID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "smiley_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void smiley_save( object smileyID, object boardID, object code, object icon, object emoticon, object sortOrder, object replace )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "smiley_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Code", code );
				cmd.Parameters.AddWithValue( "Icon", icon );
				cmd.Parameters.AddWithValue( "Emoticon", emoticon );
				cmd.Parameters.AddWithValue( "SortOrder", sortOrder );
				cmd.Parameters.AddWithValue( "Replace", replace );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void smiley_resort( object boardID, object smileyID, int move )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "smiley_resort" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "SmileyID", smileyID );
				cmd.Parameters.AddWithValue( "Move", move );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region BBCode
		static public DataTable bbcode_list( object boardID, object bbcodeID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bbcode_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "BBCodeID", bbcodeID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void bbcode_delete( object bbcodeID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bbcode_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BBCodeID", bbcodeID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void bbcode_save( object bbcodeID, object boardID, object name, object description, object onclickjs, object displayjs, object editjs, object displaycss, object searchregex, object replaceregex, object variables, object usemodule, object moduleclass, object execorder )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "bbcode_save" ) )
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
				cmd.Parameters.AddWithValue( "UseModule", usemodule );
				cmd.Parameters.AddWithValue( "ModuleClass", moduleclass );
				cmd.Parameters.AddWithValue( "ExecOrder", execorder );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region Registry
		/// <summary>
		/// Retrieves entries in the board settings registry
		/// </summary>
		/// <param name="Name">Use to specify return of specific entry only. Setting this to null returns all entries.</param>
		/// <returns>DataTable filled will registry entries</returns>
		static public DataTable registry_list( object name, object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "registry_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Value", value );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "registry_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Value", value );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region System
		/// <summary>
		/// Not in use anymore. Only required for old database versions.
		/// </summary>
		/// <returns></returns>
		static public DataTable system_list()
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "system_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		#endregion

		#region Topic
		static public void topic_poll_update( object topicID, object messageID, object pollID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_poll_update" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "MessageID", messageID );
				cmd.Parameters.AddWithValue( "PollID", pollID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public int topic_prune( object boardID, object forumID, object days, object permDelete )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_prune" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "Days", days );
				cmd.Parameters.AddWithValue( "PermDelete", permDelete );

				cmd.CommandTimeout = 99999;
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}

		static public DataTable topic_list( object forumID, object userId, object announcement, object date, object offset, object count )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "UserID", userId );
				cmd.Parameters.AddWithValue( "Announcement", announcement );
				cmd.Parameters.AddWithValue( "Date", date );
				cmd.Parameters.AddWithValue( "Offset", offset );
				cmd.Parameters.AddWithValue( "Count", count );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void topic_move( object topicID, object forumID, object showMoved )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_move" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "ShowMoved", showMoved );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public DataTable topic_announcements( object boardID, object numOfPostsToRetrieve, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_announcements" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NumPosts", numOfPostsToRetrieve );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public DataTable topic_latest( object boardID, object numOfPostsToRetrieve, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_latest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "NumPosts", numOfPostsToRetrieve );
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable topic_active( object boardID, object userID, object since, object categoryID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_active" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Since", since );
				cmd.Parameters.AddWithValue( "CategoryID", categoryID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		//ABOT NEW 16.04.04:Delete all topic's messages
		static private void topic_deleteAttachments( object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_listmessages" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					foreach ( DataRow row in dt.Rows )
					{
						message_deleteRecursively( row["MessageID"], true, "", 0, true, false );
					}
				}
			}
		}
		// Ederon : 12/9/2007
		static public void topic_delete( object topicID )
		{
			topic_delete( topicID, false );
		}
		static public void topic_delete( object topicID, object eraseTopic )
		{
			//ABOT CHANGE 16.04.04
			topic_deleteAttachments( topicID );
			//END ABOT CHANGE 16.04.04
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "EraseTopic", eraseTopic );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable topic_findprev( object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_findprev" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable topic_findnext( object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_findnext" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void topic_lock( object topicID, object locked )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_lock" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "Locked", locked );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public long topic_save( object forumID, object subject, object message, object userID, object priority, object pollID, object userName, object ip, object posted, object blogPostID, object flags, ref long messageID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_save" ) )
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

				DataTable dt = YafDBAccess.Current.GetData( cmd );
				messageID = long.Parse( dt.Rows[0]["MessageID"].ToString() );
				return long.Parse( dt.Rows[0]["TopicID"].ToString() );
			}
		}
		static public DataRow topic_info( object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "topic_info" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				using ( DataTable dt = YafDBAccess.Current.GetData( cmd ) )
				{
					if ( dt.Rows.Count > 0 )
						return dt.Rows[0];
					else
						return null;
				}
			}
		}
		#endregion

		#region ReplaceWords
		// rico : replace words / begin
		/// <summary>
		/// Gets a list of replace words
		/// </summary>
		/// <returns>DataTable with replace words</returns>
		static public DataTable replace_words_list( object boardId, object id )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "replace_words_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardId );
				cmd.Parameters.AddWithValue( "ID", id );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		/// <summary>
		/// Saves changs to a words
		/// </summary>
		/// <param name="id">ID of bad/good word</param>
		/// <param name="badword">bad word</param>
		/// <param name="goodword">good word</param>
		static public void replace_words_save( object boardId, object id, object badword, object goodword )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "replace_words_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", id );
				cmd.Parameters.AddWithValue( "BoardID", boardId );
				cmd.Parameters.AddWithValue( "badword", badword );
				cmd.Parameters.AddWithValue( "goodword", goodword );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		/// <summary>
		/// Deletes a bad/good word
		/// </summary>
		/// <param name="ID">ID of bad/good word to delete</param>
		static public void replace_words_delete( object ID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "replace_words_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ID", ID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region IgnoreUser

		static public void user_addignoreduser( object userId, object ignoredUserId )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_addignoreduser" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserId", userId );
				cmd.Parameters.AddWithValue( "IgnoredUserId", ignoredUserId );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public void user_removeignoreduser( object userId, object ignoredUserId )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_removeignoreduser" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserId", userId );
				cmd.Parameters.AddWithValue( "IgnoredUserId", ignoredUserId );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public bool user_isuserignored( object userId, object ignoredUserId )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_isuserignored" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserId", userId );
				cmd.Parameters.AddWithValue( "IgnoredUserId", ignoredUserId );
				cmd.Parameters.Add( "result", SqlDbType.Bit );
				cmd.Parameters["result"].Direction = ParameterDirection.ReturnValue;

				YafDBAccess.Current.ExecuteNonQuery( cmd );

				return Convert.ToBoolean( cmd.Parameters["result"].Value );
			}
		}

		static public DataTable user_ignoredlist( object userId )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_ignoredlist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserId", userId );

				return YafDBAccess.Current.GetData( cmd );
			}
		}

		#endregion

		#region User
		static public DataTable user_list( object boardID, object userID, object approved )
		{
			return user_list( boardID, userID, approved, null, null );
		}
		static public DataTable user_list( object boardID, object userID, object approved, object groupID, object rankID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Approved", approved );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				return YafDBAccess.Current.GetData( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_simplelist" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "StartID", StartID );
				cmd.Parameters.AddWithValue( "Limit", Limit );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void user_delete( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_setrole( int boardID, object providerUserKey, object role )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_setrole" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "Role", role );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		// TODO: The method is not in use
		static public void user_setinfo( int boardID, System.Web.Security.MembershipUser user )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "update {databaseOwner}.{objectQualifier}User set Name=@UserName,Email=@Email where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true ) )
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue( "UserName", user.UserName );
				cmd.Parameters.AddWithValue( "Email", user.Email );
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", user.ProviderUserKey );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_migrate( object userID, object providerUserKey, object updateProvider )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_migrate" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "UpdateProvider", updateProvider );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_deleteold( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_deleteold" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_approve( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_approve" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_approveall( object boardID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_approveall" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_suspend( object userID, object suspend )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_suspend" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Suspend", suspend );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public bool user_changepassword( object userID, object oldPassword, object newPassword )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_changepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "OldPassword", oldPassword );
				cmd.Parameters.AddWithValue( "NewPassword", newPassword );
				return (bool)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}

		static public DataTable user_pmcount( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_pmcount" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public void user_save( object userID, object boardID, object userName, object email,
				object timeZone, object languageFile, object themeFile, object overrideDefaultThemes, object approved,
				object pmNotification )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_save" ) )
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
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_adminsave( object boardID, object userID, object name, object email, object flags, object rankID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_adminsave" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Email", email );
				cmd.Parameters.AddWithValue( "Flags", flags );
				cmd.Parameters.AddWithValue( "RankID", rankID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable user_emails( object boardID, object groupID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_emails" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );

				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable user_accessmasks( object boardID, object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_accessmasks" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserID", userID );

				return userforumaccess_sort_list( YafDBAccess.Current.GetData( cmd ), 0, 0, 0 );
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
				if ( row["ParentID"] == DBNull.Value )
					row["ParentID"] = 0;

				if ( (int)row["ParentID"] == parentID )
				{
					string sIndent = "";

					for ( int j = 0; j < currentIndent; j++ )
						sIndent += "--";

					// import the row into the destination
					newRow = listDestination.NewRow();

					newRow["ForumID"] = row["ForumID"];
					newRow["ForumName"] = string.Format( "{0} {1}", sIndent, row["ForumName"] );
					if ( listDestination.Columns.IndexOf( "AccessMaskName" ) >= 0 )
						newRow["AccessMaskName"] = row["AccessMaskName"];
					else
					{
						newRow["CategoryName"] = row["CategoryName"];
						newRow["AccessMaskId"] = row["AccessMaskId"];
					}


					listDestination.Rows.Add( newRow );

					// recurse through the list...
					userforumaccess_sort_list_recursive( listSource, listDestination, (int)row["ForumID"], categoryID, currentIndent + 1 );
				}
			}
		}

		static public object user_recoverpassword( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_recoverpassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		static public void user_savepassword( object userID, object password )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_savepassword" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Password", FormsAuthentication.HashPasswordForStoringInConfigFile( password.ToString(), "md5" ) );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public object user_login( object boardID, object name, object password )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_login" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Name", name );
				cmd.Parameters.AddWithValue( "Password", password );
				return YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		static public DataTable user_avatarimage( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_avatarimage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public int user_get( int boardID, object providerUserKey )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "select UserID from {databaseOwner}.{objectQualifier}User where BoardID=@BoardID and ProviderUserKey=@ProviderUserKey", true ) )
			{
				cmd.CommandType = CommandType.Text;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
				return (int)( YafDBAccess.Current.ExecuteScalar( cmd ) ?? 0 );
			}
		}
		static public DataTable user_find( object boardID, bool filter, object userName, object email )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_find" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "Filter", filter );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public string user_getsignature( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_getsignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.ExecuteScalar( cmd ).ToString();
			}
		}
		static public void user_savesignature( object userID, object signature )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_savesignature" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Signature", signature );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_saveavatar( object userID, object avatar, System.IO.Stream stream, object avatarImageType )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_saveavatar" ) )
			{
				byte[] data = null;

				if ( stream != null )
				{
					data = new byte[stream.Length];
					stream.Seek( 0, System.IO.SeekOrigin.Begin );
					stream.Read( data, 0, (int)stream.Length );
				}

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Avatar", avatar );
				cmd.Parameters.AddWithValue( "AvatarImage", data );
				cmd.Parameters.AddWithValue( "AvatarImageType", avatarImageType );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void user_deleteavatar( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_deleteavatar" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		// TODO: The method is not in use
		static public bool user_register( object boardID, object userName, object password, object hash, object email, object location, object homePage, object timeZone, bool approved )
		{
			using ( YafDBConnManager connMan = new YafDBConnManager() )
			{
				using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( YafDBAccess.IsolationLevel ) )
				{
					try
					{
						using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_save", connMan.DBConnection ) )
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
				using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_aspnet" ) )
				{
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue( "BoardID", boardID );
					cmd.Parameters.AddWithValue( "UserName", userName );
					cmd.Parameters.AddWithValue( "Email", email );
					cmd.Parameters.AddWithValue( "ProviderUserKey", providerUserKey );
					cmd.Parameters.AddWithValue( "IsApproved", isApproved );
					return (int)YafDBAccess.Current.ExecuteScalar( cmd );
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
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_guest" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		static public DataTable user_activity_rank( object boardID, object startDate, object displayNumber )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_activity_rank" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "StartDate", startDate );
				cmd.Parameters.AddWithValue( "DisplayNumber", displayNumber );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public int user_nntp( object boardID, object userName, object email )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_nntp" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "BoardID", boardID );
				cmd.Parameters.AddWithValue( "UserName", userName );
				cmd.Parameters.AddWithValue( "Email", email );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}

		static public void user_addpoints( object userID, object points )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_addpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public void user_removepointsByTopicID( object topicID, object points )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_removepointsbytopicid" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				cmd.Parameters.AddWithValue( "Points", points );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public void user_removepoints( object userID, object points )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_removepoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public void user_setpoints( object userID, object points )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_setpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "Points", points );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		static public int user_getpoints( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_getpoints" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}
		//<summary> Returns the number of times a specific user with the provided UserID 
		// has thanked others.
		static public int user_getthanks_from( object userID )
		{

			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_getthanks_from" ) )
			{

				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return (int)YafDBAccess.Current.ExecuteScalar( cmd );
			}
		}

		//<summary> Returns the number of times and posts that other users have thanked the 
		// user with the provided userID.
		static public int[] user_getthanks_to( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "user_getthanks_to" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				SqlParameter paramThanksToNumber = new SqlParameter( "ThanksToNumber", 0 );
				paramThanksToNumber.Direction = ParameterDirection.Output;
				SqlParameter paramThanksToPostsNumber = new SqlParameter( "ThanksToPostsNumber", 0 );
				paramThanksToPostsNumber.Direction = ParameterDirection.Output;
				cmd.Parameters.AddWithValue( "UserID", userID );

				cmd.Parameters.Add( paramThanksToNumber );
				cmd.Parameters.Add( paramThanksToPostsNumber );
				YafDBAccess.Current.ExecuteNonQuery( cmd );

				int ThanksToPostsNumber, ThanksToNumber;
				if ( paramThanksToNumber.Value == DBNull.Value )
				{
					ThanksToNumber = 0;
					ThanksToPostsNumber = 0;
				}
				else
				{
					ThanksToPostsNumber = Convert.ToInt32( paramThanksToPostsNumber.Value );
					ThanksToNumber = Convert.ToInt32( paramThanksToNumber.Value );
				}
				return new int[] { ThanksToNumber, ThanksToPostsNumber };
			}
		}

		#endregion

		#region UserForum
		static public DataTable userforum_list( object userID, object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "userforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void userforum_delete( object userID, object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "userforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void userforum_save( object userID, object forumID, object accessMaskID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "userforum_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				cmd.Parameters.AddWithValue( "AccessMaskID", accessMaskID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region UserGroup
		static public DataTable usergroup_list( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "usergroup_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void usergroup_save( object userID, object groupID, object member )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "usergroup_save" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "GroupID", groupID );
				cmd.Parameters.AddWithValue( "Member", member );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region WatchForum
		static public void watchforum_add( object userID, object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchforum_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public DataTable watchforum_list( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchforum_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable watchforum_check( object userID, object forumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchforum_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "ForumID", forumID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void watchforum_delete( object watchForumID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchforum_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "WatchForumID", watchForumID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region WatchTopic
		static public DataTable watchtopic_list( object userID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchtopic_list" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public DataTable watchtopic_check( object userID, object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchtopic_check" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				return YafDBAccess.Current.GetData( cmd );
			}
		}
		static public void watchtopic_delete( object watchTopicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchtopic_delete" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "WatchTopicID", watchTopicID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		static public void watchtopic_add( object userID, object topicID )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "watchtopic_add" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "TopicID", topicID );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
		#endregion

		#region vzrus addons
		#region reindex page controls
		//DB Maintenance page buttons name
		public static string btnGetStatsName
		{
			get
			{
				return "Table Index Statistics";
			}
		}
		public static string btnShrinkName
		{
			get
			{
				return "Shrink Database";
			}
		}
		public static string btnRecoveryModeName
		{
			get
			{
				return "Set Recovery Mode";
			}
		}
		public static string btnReindexName
		{
			get
			{
				return "Reindex Tables";
			}
		}
		//DB Maintenance page panels visibility
		public static bool PanelGetStats
		{
			get
			{
				return true;
			}
		}
		public static bool PanelRecoveryMode
		{
			get
			{
				return true;
			}
		}
		public static bool PanelReindex
		{
			get
			{
				return true;
			}
		}
		public static bool PanelShrink
		{
			get
			{
				return true;
			}
		}


		#endregion
		static public DataTable rsstopic_list( int forumId )
		{
			// TODO: vzrus: possible move to an sp and registry settings for rsstopiclimit
			int topicLimit = 1000;
			string tSQL = "select top " + topicLimit + " Topic = a.Topic,TopicID = a.TopicID, Name = b.Name, Posted = a.Posted from {databaseOwner}.{objectQualifier}Topic a, {databaseOwner}.{objectQualifier}Forum b where a.ForumID=" +
                                                        forumId + " and b.ForumID = a.ForumID and a.IsDeleted = 0;";
			using ( SqlCommand cmd = YafDBAccess.GetCommand( tSQL, true ) )
			{
				cmd.CommandType = CommandType.Text;
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		static public string db_getstats_warning( YafDBConnManager connMan )
		{
			return "";
		}

		public static void db_getstats( YafDBConnManager connMan )
		{
			// create statistic getting SQL...
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.AppendLine( "DECLARE @TableName sysname" );
			sb.AppendLine( "DECLARE cur_showfragmentation CURSOR FOR" );
			sb.AppendFormat( "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", YafDBAccess.ObjectQualifier );
			sb.AppendLine( "OPEN cur_showfragmentation" );
			sb.AppendLine( "FETCH NEXT FROM cur_showfragmentation INTO @TableName" );
			sb.AppendLine( "WHILE @@FETCH_STATUS = 0" );
			sb.AppendLine( "BEGIN" );
			sb.AppendLine( "DBCC SHOWCONTIG (@TableName)" );
			sb.AppendLine( "FETCH NEXT FROM cur_showfragmentation INTO @TableName" );
			sb.AppendLine( "END" );
			sb.AppendLine( "CLOSE cur_showfragmentation" );
			sb.AppendLine( "DEALLOCATE cur_showfragmentation" );

			using ( SqlCommand cmd = new SqlCommand( sb.ToString(), connMan.OpenDBConnection ) )
			{
				cmd.Connection = connMan.DBConnection;
				// up the command timeout...
				cmd.CommandTimeout = 9999;
				// run it...
				cmd.ExecuteNonQuery();
			}
		}

		static public string db_reindex_warning( YafDBConnManager connMan )
		{
			return "";
		}

		public static void db_reindex( YafDBConnManager connMan )
		{
			// create statistic getting SQL...
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.AppendLine( "DECLARE @MyTable VARCHAR(255)" );
			sb.AppendLine( "DECLARE myCursor" );
			sb.AppendLine( "CURSOR FOR" );
			sb.AppendFormat( "SELECT table_name FROM information_schema.tables WHERE table_type = 'base table' AND table_name LIKE '{0}%'", YafDBAccess.ObjectQualifier );
			sb.AppendLine( "OPEN myCursor" );
			sb.AppendLine( "FETCH NEXT" );
			sb.AppendLine( "FROM myCursor INTO @MyTable" );
			sb.AppendLine( "WHILE @@FETCH_STATUS = 0" );
			sb.AppendLine( "BEGIN" );
			sb.AppendLine( "PRINT 'Reindexing Table:  ' + @MyTable" );
			sb.AppendLine( "DBCC DBREINDEX(@MyTable, '', 80)" );
			sb.AppendLine( "FETCH NEXT" );
			sb.AppendLine( "FROM myCursor INTO @MyTable" );
			sb.AppendLine( "END" );
			sb.AppendLine( "CLOSE myCursor" );
			sb.AppendLine( "DEALLOCATE myCursor" );

			using ( SqlCommand cmd = new SqlCommand( sb.ToString(), connMan.OpenDBConnection ) )
			{
				cmd.Connection = connMan.DBConnection;
				// up the command timeout...
				cmd.CommandTimeout = 9999;
				// run it...
				cmd.ExecuteNonQuery();
			}
		}

		public static string db_runsql( string sql, YafDBConnManager connMan )
		{
			string txtResult = "";

			using ( SqlCommand cmd = new SqlCommand( sql, connMan.OpenDBConnection ) )
			{
				cmd.CommandTimeout = 9999;
				SqlDataReader reader = null;

				using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( YafDBAccess.IsolationLevel ) )
				{
					try
					{
						cmd.Connection = connMan.DBConnection;
						cmd.Transaction = trans;
						reader = cmd.ExecuteReader();

						if ( reader != null )
						{
							if ( reader.HasRows )
							{
								txtResult = "\r\n" + String.Format( "Result set has {0} fields.", reader.FieldCount );
							}
							else if ( reader.RecordsAffected > 0 )
							{
								txtResult += "\r\n" + String.Format( "{0} Record(s) Affected", reader.RecordsAffected );
							}

							reader.Close();
						}

						trans.Commit();
					}
					catch ( Exception x )
					{
						if ( reader != null )
						{
							reader.Close();
						}

						// rollback...
						trans.Rollback();
						txtResult = "\r\n" + "SQL ERROR: " + x.Message;
					}
					return txtResult;
				}
			}
		}

		public static bool forumpage_initdb( out string errorStr, bool debugging )
		{
			errorStr = "";

			try
			{
				using ( YafDBConnManager connMan = new YafDBConnManager() )
				{
					// just attempt to open the connection to test if a DB is available.
					SqlConnection getConn = connMan.OpenDBConnection;
				}
			}
			catch ( SqlException ex )
			{
				// unable to connect to the DB...
				if ( !debugging )
				{
					errorStr = "Unable to connect to the Database. Exception Message: " + ex.Message + " (" + ex.Number + ")";
					return false;
				}

				// re-throw since we are debugging...
				throw;
			}

			return true;
		}

		public static string forumpage_validateversion( int appVersion )
		{
			string redirect = "";
			try
			{
				DataTable registry = YAF.Classes.Data.DB.registry_list( "Version" );

				if ( ( registry.Rows.Count == 0 ) || ( Convert.ToInt32( registry.Rows[0]["Value"] ) < appVersion ) )
				{
					// needs upgrading...
					redirect = "install/default.aspx?upgrade=" + Convert.ToInt32( registry.Rows[0]["Value"] );
				}
			}
			catch ( System.Data.SqlClient.SqlException )
			{
				// needs to be setup...
				redirect = "install/";
			}
			return redirect;
		}

		public static void system_deleteinstallobjects()
		{
			string tSQL = "DROP PROCEDURE" + YafDBAccess.GetObjectName( "system_initialize" );
			using ( SqlCommand cmd = YafDBAccess.GetCommand( tSQL, true ) )
			{
				cmd.CommandType = CommandType.Text;
				YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}

		public static void system_initialize_executescripts( string script, string scriptFile, bool useTransactions )
		{
			// apply database owner
			script = script.Replace( "{databaseOwner}", YafDBAccess.DatabaseOwner );
			// apply object qualifier
			script = script.Replace( "{objectQualifier}", YafDBAccess.ObjectQualifier );

			List<string> statements = System.Text.RegularExpressions.Regex.Split( script, "\\sGO\\s", System.Text.RegularExpressions.RegexOptions.IgnoreCase ).ToList();

			using ( YAF.Classes.Data.YafDBConnManager connMan = new YafDBConnManager() )
			{
				// use transactions...
				if ( useTransactions )
				{
					using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( YAF.Classes.Data.YafDBAccess.IsolationLevel ) )
					{
						foreach ( string sql0 in statements )
						{
							string sql = sql0.Trim();

							try
							{
								if ( sql.ToLower().IndexOf( "setuser" ) >= 0 )
									continue;

								if ( sql.Length > 0 )
								{
									using ( SqlCommand cmd = new SqlCommand() )
									{
										cmd.Transaction = trans;
										cmd.Connection = connMan.DBConnection;
										cmd.CommandType = CommandType.Text;
										cmd.CommandText = sql.Trim();
										cmd.ExecuteNonQuery();
									}
								}
							}
							catch ( Exception x )
							{
								trans.Rollback();
								throw new Exception( String.Format( "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message ) );
							}
						}
						trans.Commit();
					}
				}
				else
				{
					// don't use transactions
					foreach ( string sql0 in statements )
					{
						string sql = sql0.Trim();
						// add ARITHABORT option
						//sql = "SET ARITHABORT ON\r\nGO\r\n" + sql;

						try
						{
							if ( sql.ToLower().IndexOf( "setuser" ) >= 0 )
								continue;

							if ( sql.Length > 0 )
							{
								using ( SqlCommand cmd = new SqlCommand() )
								{
									cmd.Connection = connMan.OpenDBConnection;
									cmd.CommandType = CommandType.Text;
									cmd.CommandText = sql.Trim();
									cmd.ExecuteNonQuery();
								}
							}
						}
						catch ( Exception x )
						{
							throw new Exception( String.Format( "FILE:\n{0}\n\nERROR:\n{2}\n\nSTATEMENT:\n{1}", scriptFile, sql, x.Message ) );
						}
					}
				}
			}


		}
		public static void system_initialize_fixaccess( bool grant )
		{
			using ( YAF.Classes.Data.YafDBConnManager connMan = new YafDBConnManager() )
			{
				using ( SqlTransaction trans = connMan.OpenDBConnection.BeginTransaction( YAF.Classes.Data.YafDBAccess.IsolationLevel ) )
				{
					// REVIEW : Ederon - would "{databaseOwner}.{objectQualifier}" work, might need only "{objectQualifier}"
					using ( SqlDataAdapter da = new SqlDataAdapter( "select Name,IsUserTable = OBJECTPROPERTY(id, N'IsUserTable'),IsScalarFunction = OBJECTPROPERTY(id, N'IsScalarFunction'),IsProcedure = OBJECTPROPERTY(id, N'IsProcedure'),IsView = OBJECTPROPERTY(id, N'IsView') from dbo.sysobjects where Name like '{databaseOwner}.{objectQualifier}%'", connMan.OpenDBConnection ) )
					{
						da.SelectCommand.Transaction = trans;
						using ( DataTable dt = new DataTable( "sysobjects" ) )
						{
							da.Fill( dt );
							using ( SqlCommand cmd = connMan.DBConnection.CreateCommand() )
							{
								cmd.Transaction = trans;
								cmd.CommandType = CommandType.Text;
								cmd.CommandText = "select current_user";
								string userName = (string)cmd.ExecuteScalar();

								if ( grant )
								{
									cmd.CommandType = CommandType.Text;
									foreach ( DataRow row in dt.Select( "IsProcedure=1 or IsScalarFunction=1" ) )
									{
										cmd.CommandText = string.Format( "grant execute on \"{0}\" to \"{1}\"", row["Name"], userName );
										cmd.ExecuteNonQuery();
									}
									foreach ( DataRow row in dt.Select( "IsUserTable=1 or IsView=1" ) )
									{
										cmd.CommandText = string.Format( "grant select,update on \"{0}\" to \"{1}\"", row["Name"], userName );
										cmd.ExecuteNonQuery();
									}
								}
								else
								{
									cmd.CommandText = "sp_changeobjectowner";
									cmd.CommandType = CommandType.StoredProcedure;
									foreach ( DataRow row in dt.Select( "IsUserTable=1" ) )
									{
										cmd.Parameters.Clear();
										cmd.Parameters.AddWithValue( "@objname", row["Name"] );
										cmd.Parameters.AddWithValue( "@newowner", "dbo" );
										try
										{
											cmd.ExecuteNonQuery();
										}
										catch ( SqlException )
										{
										}
									}
									foreach ( DataRow row in dt.Select( "IsView=1" ) )
									{
										cmd.Parameters.Clear();
										cmd.Parameters.AddWithValue( "@objname", row["Name"] );
										cmd.Parameters.AddWithValue( "@newowner", "dbo" );
										try
										{
											cmd.ExecuteNonQuery();
										}
										catch ( SqlException )
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
		public static void system_initialize( string forumName, string timeZone, string forumEmail, string smtpServer, string userName, string userEmail, object providerUserKey )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "system_initialize" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Name", forumName );
				cmd.Parameters.AddWithValue( "@TimeZone", timeZone );
				cmd.Parameters.AddWithValue( "@ForumEmail", forumEmail );
				cmd.Parameters.AddWithValue( "@SmtpServer", "" );
				cmd.Parameters.AddWithValue( "@User", userName );
				// vzrus:The input parameter should be implemented in the system initialize and board_create procedures, else there will be an error in create watch because the user email is missing
				//cmd.Parameters.AddWithValue("@UserEmail", userEmail);
				cmd.Parameters.AddWithValue( "@UserKey", providerUserKey );
				YAF.Classes.Data.YafDBAccess.Current.ExecuteNonQuery( cmd );
			}

		}

		public static void system_updateversion( int version, string versionname )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "system_updateversion" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "@Version", version );
				cmd.Parameters.AddWithValue( "@VersionName", versionname );
				YAF.Classes.Data.YafDBAccess.Current.ExecuteNonQuery( cmd );
			}
		}
        /// <summary>
        /// Returns info about all Groups and Rank styles. 
        /// Used in GroupRankStyles cache.
        /// Usage: LegendID = 1 - Select Groups, LegendID = 2 - select Ranks by Name 
        /// </summary>
        public static DataTable group_rank_style(object boardID)
        {
            using (SqlCommand cmd = YafDBAccess.GetCommand("group_rank_style"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BoardID", boardID);
               return YAF.Classes.Data.YafDBAccess.Current.GetData(cmd);
            }
        }

		#endregion

		#region DLESKTECH_ShoutBox

		public static DataTable shoutbox_getmessages( int numberOfMessages )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "shoutbox_getmessages" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "NumberOfMessages", numberOfMessages );
				return YafDBAccess.Current.GetData( cmd );
			}
		}

		public static bool shoutbox_savemessage( string message, string usernName, int userID, object ip )
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "shoutbox_savemessage" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue( "Message", message );
				cmd.Parameters.AddWithValue( "UserName", usernName );
				cmd.Parameters.AddWithValue( "UserID", userID );
				cmd.Parameters.AddWithValue( "IP", ip );
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return true;
			}
		}

		public static Boolean shoutbox_clearmessages()
		{
			using ( SqlCommand cmd = YafDBAccess.GetCommand( "shoutbox_clearmessages" ) )
			{
				cmd.CommandType = CommandType.StoredProcedure;
				YafDBAccess.Current.ExecuteNonQuery( cmd );
				return true;
			}
		}

		#endregion

		#region Touradg Mods
		//Shinking Operation
		static public string db_shrink_warning( YafDBConnManager DBName )
		{
			return "";
		}

		public static void db_shrink( YafDBConnManager DBName )
		{
			String ShrinkSql = "DBCC SHRINKDATABASE(N'" + DBName.DBConnection.Database + "')";
			SqlConnection ShrinkConn = new SqlConnection( YAF.Classes.Config.ConnectionString );
			SqlCommand ShrinkCmd = new SqlCommand( ShrinkSql, ShrinkConn );
			ShrinkConn.Open();
			ShrinkCmd.ExecuteNonQuery();
			ShrinkConn.Close();
			using ( SqlCommand cmd = new SqlCommand( ShrinkSql.ToString(), DBName.OpenDBConnection ) )
			{
				cmd.Connection = DBName.DBConnection;
				cmd.CommandTimeout = 9999;
				cmd.ExecuteNonQuery();
			}
		}
		//Set Recovery
		static public string db_recovery_mode_warning( YafDBConnManager DBName )
		{
			return "";
		}

		public static void db_recovery_mode( YafDBConnManager DBName, string dbRecoveryMode )
		{
			String RecoveryMode = "ALTER DATABASE " + DBName.DBConnection.Database + " SET RECOVERY " + dbRecoveryMode;
			SqlConnection RecoveryModeConn = new SqlConnection( YAF.Classes.Config.ConnectionString );
			SqlCommand RecoveryModeCmd = new SqlCommand( RecoveryMode, RecoveryModeConn );
			RecoveryModeConn.Open();
			RecoveryModeCmd.ExecuteNonQuery();
			RecoveryModeConn.Close();
			using ( SqlCommand cmd = new SqlCommand( RecoveryMode.ToString(), DBName.OpenDBConnection ) )
			{
				cmd.Connection = DBName.DBConnection;
				cmd.CommandTimeout = 9999;
				cmd.ExecuteNonQuery();
			}
		}
		#endregion
	}

}
