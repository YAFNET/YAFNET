/* Yet Another Forum.net
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
using System.Linq;
using System.Web;
using YAF.Classes;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
{
	/// <summary>
	/// Class used for multi-step DB operations so they can be cached, etc.
	/// </summary>
	public class YafDBBroker
	{
		public List<int> UserIgnoredList( int userId )
		{
			string key = YafCache.GetBoardCacheKey( String.Format( Constants.Cache.UserIgnoreList, userId ) );
			// stored in the user session...
			List<int> userList = HttpContext.Current.Session[key] as List<int>;

			// was it in the cache?
			if ( userList == null )
			{
				// get fresh values
				DataTable userListDt = DB.user_ignoredlist( userId );

				// convert to list...
				userList = userListDt.GetColumnAsList<int>( "IgnoredUserID" );

				// store it in the user session...
				HttpContext.Current.Session.Add( key, userList );
			}

			return userList;
		}

		public DataTable UserMedals( int userId )
		{
			string key = YafCache.GetBoardCacheKey( String.Format( Constants.Cache.UserMedals, userId ) );

			// get the medals cached...
			DataTable dt = YafContext.Current.Cache.GetItem<DataTable>( key, 10, () => DB.user_listmedals( userId ) );

			return dt;
		}

		public DataTable GetModerators()
		{
			DataTable moderator = DB.forum_moderators();
			moderator.TableName = YafDBAccess.GetObjectName( "Moderator" );

			return moderator;
		}

		public List<Moderator> GetAllModerators()
		{
			// get the cached version of forum moderators if it's valid
			string key = YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators );

			DataTable moderator = YafContext.Current.Cache.GetItem<DataTable>( key,
			                                                                   YafContext.Current.BoardSettings.
																																					BoardModeratorsCacheTimeout, GetModerators );

			return
				moderator.ToListObject(
					( row ) =>
					new Moderator( Convert.ToInt64( row["ForumID"] ), Convert.ToInt64( row["ModeratorID"] ),
					               row["ModeratorName"].ToString(), SqlDataLayerConverter.VerifyBool( row["IsGroup"] ) ) );
		}

		/// <summary>
		/// Returns the layout of the board
		/// </summary>
		/// <param name="boardID">BoardID</param>
		/// <param name="userID">UserID</param>
		/// <param name="categoryID">CategoryID</param>
		/// <param name="parentID">ParentID</param>
		/// <returns>Returns board layout</returns>
		public DataSet BoardLayout( object boardID, object userID, object categoryID, object parentID )
		{
			if ( categoryID != null && long.Parse( categoryID.ToString() ) == 0 )
				categoryID = null;

			using ( DataSet ds = new DataSet() )
			{
				// get the cached version of forum moderators if it's valid
				string key = YafCache.GetBoardCacheKey( Constants.Cache.ForumModerators );

				DataTable moderator = YafContext.Current.Cache.GetItem<DataTable>( key,
																																					 YafContext.Current.BoardSettings.
																																						BoardModeratorsCacheTimeout, GetModerators );
				// insert it into this DataSet
				ds.Tables.Add( moderator.Copy() );

				// get the Category Table
				key = YafCache.GetBoardCacheKey( Constants.Cache.ForumCategory );
				DataTable category = YafContext.Current.Cache.GetItem<DataTable>( key,
																																					 YafContext.Current.BoardSettings.
																																						BoardCategoriesCacheTimeout, () =>
																																						{
																																							var catDt = DB.category_list( boardID, null );
																																							catDt.TableName = YafDBAccess.GetObjectName( "Category" );
																																							return catDt;
																																						} );
				// add it to this dataset				
				ds.Tables.Add( category.Copy() );

				if ( categoryID != null )
				{
					// make sure this only has the category desired in the dataset
					foreach ( DataRow row in ds.Tables [YafDBAccess.GetObjectName("Category")].Rows )
					{
						if ( Convert.ToInt32( row ["CategoryID"] ) != Convert.ToInt32(categoryID) )
						{
							// delete it...
							row.Delete();
						}
					}
					ds.Tables [YafDBAccess.GetObjectName( "Category" )].AcceptChanges();
				}

				DataTable forum = DB.forum_listread( boardID, userID, categoryID, parentID );
				forum.TableName = YafDBAccess.GetObjectName( "Forum" );
				ds.Tables.Add( forum.Copy() );

				ds.Relations.Add( "FK_Forum_Category", ds.Tables [YafDBAccess.GetObjectName( "Category" )].Columns ["CategoryID"], ds.Tables [YafDBAccess.GetObjectName( "Forum" )].Columns ["CategoryID"], false );
				ds.Relations.Add( "FK_Moderator_Forum", ds.Tables [YafDBAccess.GetObjectName( "Forum" )].Columns ["ForumID"], ds.Tables [YafDBAccess.GetObjectName( "Moderator" )].Columns ["ForumID"], false );

				bool deletedCategory = false;

				// remove empty categories...
				foreach ( DataRow row in ds.Tables[YafDBAccess.GetObjectName( "Category" )].Rows )
				{
					DataRow[] childRows = row.GetChildRows( "FK_Forum_Category" );

					if ( childRows.Length == 0 )
					{
						// remove this category...
						row.Delete();
						deletedCategory = true;
					}
				}

				if ( deletedCategory ) ds.Tables[YafDBAccess.GetObjectName( "Category" )].AcceptChanges();

				return ds;
			}
		}
	}
}