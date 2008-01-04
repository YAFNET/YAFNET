using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Class used for multi-step DB operations so they can be cached, etc.
	/// </summary>
	public static class DBBroker
	{
		/// <summary>
		/// Returns the layout of the board
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="UserID">UserID</param>
		/// <param name="CategoryID">CategoryID</param>
		/// <param name="parentID">ParentID</param>
		/// <returns>Returns board layout</returns>
		static public DataSet board_layout( object boardID, object userID, object categoryID, object parentID )
		{
			if ( categoryID != null && long.Parse( categoryID.ToString() ) == 0 )
				categoryID = null;

			using ( DataSet ds = new DataSet() )
			{
				// get the cached version of forum moderators if it's valid
				string key = YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators );
				DataTable moderator = YafCache.Current [key] as DataTable;

				// was it in the cache?
				if ( moderator == null )
				{
					// get fresh values
					moderator = DB.forum_moderators();
					moderator.TableName = DBAccess.GetObjectName( "Moderator" );

					// cache it for 30 minutes (or longer)
					YafCache.Current.Add(key, moderator, null, null, new TimeSpan(0, 30, 0));
				}
				// insert it into this DataSet
				ds.Tables.Add( moderator.Copy() );

				// get the Category Table
				key = YafCache.GetBoardCacheKey( Constants.Cache.ForumCategory );
				DataTable category = YafCache.Current [key] as DataTable;

				// was it in the cache?
				if ( category == null )
				{
					// just get all categories since the list is short
					category = DB.category_list( boardID, null );
					category.TableName = DBAccess.GetObjectName( "Category" );
					YafCache.Current [key] = category;
				}	

				// add it to this dataset				
				ds.Tables.Add( category.Copy() );

				if ( categoryID != null )
				{
					// make sure this only has the category desired in the dataset
					foreach ( DataRow row in ds.Tables [DBAccess.GetObjectName("Category")].Rows )
					{
						if ( Convert.ToInt32( row ["CategoryID"] ) != Convert.ToInt32(categoryID) )
						{
							// delete it...
							row.Delete();
						}
					}
					ds.Tables [DBAccess.GetObjectName( "Category" )].AcceptChanges();
				}

				DataTable forum = DB.forum_listread( boardID, userID, categoryID, parentID );
				forum.TableName = DBAccess.GetObjectName( "Forum" );
				ds.Tables.Add( forum.Copy() );

				ds.Relations.Add( "FK_Forum_Category", ds.Tables [DBAccess.GetObjectName( "Category" )].Columns ["CategoryID"], ds.Tables [DBAccess.GetObjectName( "Forum" )].Columns ["CategoryID"] );
				ds.Relations.Add( "FK_Moderator_Forum", ds.Tables [DBAccess.GetObjectName( "Forum" )].Columns ["ForumID"], ds.Tables [DBAccess.GetObjectName( "Moderator" )].Columns ["ForumID"], false );

				return ds;
			}
		}
	}
}
