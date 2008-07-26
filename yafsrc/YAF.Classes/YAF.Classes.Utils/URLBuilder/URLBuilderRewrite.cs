/* Yet Another Forum.net
 * Copyright (C) 2006-2008 Jaben Cargman
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
using System.Web;
using System.Collections.Specialized;
using YAF.Classes.Utils;
using YAF.Classes.Data;

namespace YAF.Classes
{
	public class RewriteUrlBuilder : YAF.Classes.IUrlBuilder
	{
		private int cacheSize = 500;

		protected int CacheSize
		{
			get
			{
				return ( int ) ( cacheSize );
			}
			set
			{
				if (cacheSize > 0) cacheSize = value;
			}
		}

		protected int HighRange(int id)
		{
			return ( int ) ( Math.Ceiling( ( double ) ( id / cacheSize ) ) * cacheSize );
		}

		protected int LowRange(int id)
		{
			return ( int ) ( Math.Floor( ( double ) ( id / cacheSize ) ) * cacheSize );
		}

		public string BuildUrl( string url )
		{
			string newURL = string.Format( "{0}{1}?{2}", UrlBuilder.BaseUrl, UrlBuilder.ScriptName, url );

			// create scriptName
			string scriptName = string.Format( "{0}{1}", UrlBuilder.BaseUrl, UrlBuilder.ScriptName );

			// get the base script file from the config -- defaults to, well, default.aspx :)
			string scriptFile = Config.BaseScriptFile;

			if ( scriptName.EndsWith( scriptFile ) ) 
			{
				string before = scriptName.Remove( scriptName.LastIndexOf( scriptFile ) );

				SimpleURLParameterParser parser = new SimpleURLParameterParser( url );

				// create "rewritten" url...
				newURL = before + "yaf_";

				string useKey = "";
				string description = "";
				string pageName = parser ["g"];
				string pageNameExtension = string.Empty;
				bool showKey = false;
				bool handlePage = false;

				switch (parser["g"])
				{
					case "topics":
						useKey = "f";
						description = GetForumName( Convert.ToInt32( parser [useKey] ) );
						handlePage = true;
						break;
					case "posts":
						if ( !String.IsNullOrEmpty( parser ["t"] ) )
						{
							useKey = "t"; pageName += "t";
							description = GetTopicName( Convert.ToInt32( parser [useKey] ) );
						}
						else if ( !String.IsNullOrEmpty( parser ["m"] ) )
						{
							useKey = "m"; pageName += "m";
							description = GetTopicNameFromMessage( Convert.ToInt32( parser [useKey] ) );
						}
						handlePage = true;
						break;
					case "profile":
						useKey = "u";
						//description = GetProfileName( Convert.ToInt32( parser [useKey] ) );
						break;
					case "forum":
						if ( !String.IsNullOrEmpty( parser ["c"] ) )
						{
							useKey = "c";
							description = GetCategoryName( Convert.ToInt32( parser [useKey] ) );
						}
						break;
				}

				newURL += pageName;

				if ( useKey.Length > 0 )
				{
					if ( !showKey )
						newURL += parser [useKey];
					else
						newURL += useKey + parser [useKey];
				}

				if ( handlePage && parser ["p"] != null)
				{
					int page = Convert.ToInt32( parser ["p"] );
					if ( page != 1 ) newURL += "p" + page.ToString();
					parser.Parameters.Remove( "p" );
				}

				if ( description.Length > 0 )
				{
					newURL += "_" + description;
				}

				newURL += ".aspx";

				string restURL = parser.CreateQueryString( new string [] { "g", useKey } );

				// append to the url if there are additional (unsupported) parameters
				if ( restURL.Length > 0 )
				{
					newURL += "?" + restURL;
				}

				// see if we can just use the default (/)
				if ( newURL.EndsWith( "yaf_forum.aspx" ) )
				{
					// remove in favor of just slash...
					newURL = newURL.Remove( newURL.LastIndexOf( "yaf_forum.aspx" ), "yaf_forum.aspx".Length );
				}

				// add anchor
				if ( parser.HasAnchor ) newURL += "#" + parser.Anchor;				
			}

			return newURL;
		}

		private string GetCacheName( string type, int id )
		{
			return String.Format(@"urlRewritingDT-{0}-Range-{1}-to-{2}", type, HighRange(id), LowRange(id)); 
		}

		private string CleanStringForURL( string str )
		{
			string cleaned = "";

			// trim...
			str = str.Trim();

			// fix quotes and ampersand...
			str = str.Replace( "&quot;", "" );
			str = str.Replace( "&amp;", "and" );
			str = str.Replace( "&", "and" );

			for ( int i = 0; i < str.Length; i++ )
			{
				if ( char.IsLetterOrDigit( str [i] ) )
				{
					cleaned += str [i];
				}
				else if ( char.IsSeparator( str [i] ) )
				{
					cleaned += '-';
				}
			}

			return cleaned;
		}

		private DataRow GetDataRowFromCache( string type, int id )
		{
			// get the datatable and find the value
			DataTable list = HttpContext.Current.Cache [GetCacheName( type, id )] as DataTable;

			if ( list != null )
			{
				DataRow row = list.Rows.Find( id );

				// valid, return...
				if ( row != null )
				{
					return row;
				}
				else
				{
					// invalidate this cache section
					HttpContext.Current.Cache.Remove( GetCacheName( type, id ) );
				}
			}

			return null;
		}

		private DataRow SetupDataToCache( ref DataTable list, string type, int id, string primaryKey )
		{
			DataRow row = null;

			if ( list != null )
			{
				list.Columns [primaryKey].Unique = true;
				list.PrimaryKey = new DataColumn [] { list.Columns [primaryKey] };

				// store it for the future
				Random randomValue = new Random();
				HttpContext.Current.Cache.Insert( GetCacheName( type, id ), list, null, DateTime.Now.AddMinutes( randomValue.Next( 5, 15 ) ), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Low, null );
				// find and return profile..
				row = list.Rows.Find( id );

				if ( row == null )
				{
					// invalidate this cache section
					HttpContext.Current.Cache.Remove( GetCacheName( type, id ) );
				}
			}

			return row;
		}
		
		private string GetProfileName( int id )
		{
			string type = "Profile";
			string primaryKey = "UserID";
			string nameField = "Name";

			DataRow row = GetDataRowFromCache( type, id );

			if ( row == null )
			{
				// get the section desired...
				DataTable list = DB.user_simplelist( LowRange( id ), CacheSize );
				// set it up in the cache
				row = SetupDataToCache( ref list, type, id, primaryKey );

				if ( row == null ) return "";
			}

			return CleanStringForURL( row [nameField].ToString() );
		}

		private string GetForumName( int id )
		{
			string type = "Forum";
			string primaryKey = "ForumID";
			string nameField = "Name";

			DataRow row = GetDataRowFromCache( type, id );

			if ( row == null )
			{
				// get the section desired...
				DataTable list = DB.forum_simplelist( LowRange( id ), CacheSize );
				// set it up in the cache
				row = SetupDataToCache( ref list, type, id, primaryKey );

				if ( row == null ) return "";
			}

			return CleanStringForURL( row [nameField].ToString() );
		}

		private string GetCategoryName( int id )
		{
			string type = "Category";
			string primaryKey = "CategoryID";
			string nameField = "Name";

			DataRow row = GetDataRowFromCache( type, id );

			if ( row == null )
			{
				// get the section desired...
				DataTable list = DB.category_simplelist( LowRange( id ), CacheSize );
				// set it up in the cache
				row = SetupDataToCache( ref list, type, id, primaryKey );

				if ( row == null ) return "";
			}

			return CleanStringForURL( row [nameField].ToString() );
		}

		private string GetTopicName( int id )
		{
			string type = "Topic";
			string primaryKey = "TopicID";
			string nameField = "Topic";

			DataRow row = GetDataRowFromCache( type, id );

			if ( row == null )
			{
				// get the section desired...
				DataTable list = DB.topic_simplelist( LowRange( id ), CacheSize );
				// set it up in the cache
				row = SetupDataToCache( ref list, type, id, primaryKey );

				if ( row == null ) return "";
			}

			return CleanStringForURL( row [nameField].ToString() );
		}

		private string GetTopicNameFromMessage( int id )
		{
			string type = "Message";
			string primaryKey = "MessageID";

			DataRow row = GetDataRowFromCache( type, id );

			if ( row == null )
			{
				// get the section desired...
				DataTable list = DB.message_simplelist( LowRange( id ), CacheSize );
				// set it up in the cache
				row = SetupDataToCache( ref list, type, id, primaryKey );

				if ( row == null ) return "";
			}

			return GetTopicName( Convert.ToInt32( row ["TopicID"] ) );
		}

	}
}
