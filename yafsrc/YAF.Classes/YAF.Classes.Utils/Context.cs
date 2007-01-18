/* YetAnotherForum.NET
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
using System.Collections.Generic;
using System.Text;
using System.Web;
using YAF.Classes.Data;

namespace YAF.Classes.Utils
{
	/// <summary>
	/// Context class that accessable with the same instance from all locations
	/// </summary>
	public class yaf_Context
	{
		private static yaf_Context currentInstance = new yaf_Context();
		private YAF.Classes.Data.YAFDB.yaf_PageLoadRow page = null;
		private YAF.Classes.Utils.yaf_ControlSettings settings = null;
		private YAF.Classes.Utils.yaf_Theme theme = null;
		private YAF.Classes.Utils.yaf_Localization localization = null;
		private System.Web.Security.MembershipUser user = null;

		public static yaf_Context Current
		{
			get
			{
				return currentInstance;
			}
		}

		public YAF.Classes.Data.YAFDB.yaf_PageLoadRow Page
		{
			get
			{
				return page;
			}
			set
			{
				page = value;
			}
		}

		public YAF.Classes.Utils.yaf_ControlSettings Settings
		{
			get
			{
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		public YAF.Classes.Utils.yaf_Theme Theme
		{
			get
			{
				return theme;
			}
			set
			{
				theme = value;
			}
		}

		public YAF.Classes.Utils.yaf_Localization Localization
		{
			get
			{
				return localization;
			}
			set
			{
				localization = value;
			}
		}

		public System.Web.Security.MembershipUser User
		{
			get
			{
				if ( user == null )
					user = System.Web.Security.Membership.GetUser();
				return user;
			}
			set
			{
				user = value;
			}
		}

		public yaf_BoardSettings BoardSettings
		{
			get
			{
				int PageBoardID = ( Settings == null ) ? 1 : Settings.BoardID;
				string key = string.Format( "yaf_BoardSettings.{0}", PageBoardID );

				if ( HttpContext.Current.Application [key] == null )
					HttpContext.Current.Application [key] = new yaf_BoardSettings( PageBoardID );

				return ( yaf_BoardSettings ) HttpContext.Current.Application [key];

			}
			set
			{
				int PageBoardID = ( Settings == null ) ? 1 : Settings.BoardID;
				string key = string.Format( "yaf_BoardSettings.{0}", PageBoardID );

				HttpContext.Current.Application.Remove( key );
			}
		}
	}

	/// <summary>
	/// Class provides glue/settings transfer between YAF forum control and base classes
	/// </summary>
	public class yaf_ControlSettings
	{
		private int m_boardID;
		private int m_categoryID;
		private int mLockedForum = 0;

		public yaf_ControlSettings()
		{
			try
			{
				m_categoryID = int.Parse( Config.CategoryID );
			}
			catch
			{
				m_categoryID = 1;
			}
			try
			{
				m_boardID = int.Parse( Config.BoardID );
			}
			catch
			{
				m_boardID = 1;
			}
		}

		public int BoardID
		{
			get
			{
				return m_boardID;
			}
			set
			{
				m_boardID = value;
			}
		}

		public int CategoryID
		{
			get
			{
				return m_categoryID;
			}
			set
			{
				m_categoryID = value;
			}
		}

		public int LockedForum
		{
			set
			{
				mLockedForum = value;
			}
			get
			{
				return mLockedForum;
			}
		}
	}

	/// <summary>
	/// Class provides misc helper functions and forum version information
	/// </summary>
	public static class yaf_ForumInfo
	{
		public static string ForumRoot
		{
			get
			{
				try
				{
					string path = HttpContext.Current.Request.ApplicationPath;
					if ( !path.EndsWith( "/" ) ) path += "/";

					if ( YAF.Classes.Config.Root != null )
					{
						// use specified root
						path = YAF.Classes.Config.Root;
						if ( path [0] != '/' ) path = path.Insert( 0, "/" );

					}
					else if ( YAF.Classes.Config.IsDotNetNuke )
					{
						path += "DesktopModules/YetAnotherForumDotNet/";
					}
					else if ( YAF.Classes.Config.IsRainbow )
					{
						path += "DesktopModules/Forum/";
					}
					else if ( YAF.Classes.Config.IsPortal )
					{
						path += "Modules/Forum/";
					}

					if ( !path.EndsWith( "/" ) ) path += "/";

					return path;
				}
				catch ( Exception )
				{
					return "/";
				}
			}
		}

		static public string ServerURL
		{
			get
			{
				long serverPort = long.Parse( HttpContext.Current.Request.ServerVariables ["SERVER_PORT"] );
				bool isSecure = ( HttpContext.Current.Request.ServerVariables ["HTTPS"] == "ON" );

				StringBuilder url = new StringBuilder( "http" );

				if ( isSecure )
				{
					url.Append( "s" );
				}

				url.AppendFormat( "://{0}", HttpContext.Current.Request.ServerVariables ["SERVER_NAME"] );

				if ( ( !isSecure && serverPort != 80 ) || ( isSecure && serverPort != 443 ) )
				{
					url.AppendFormat( ":{0}", serverPort.ToString() );
				}

				return url.ToString();
			}
		}

		static public bool IsLocal
		{
			get
			{
				string s = HttpContext.Current.Request.ServerVariables ["SERVER_NAME"];
				return s != null && s.ToLower() == "localhost";
			}
		}

		#region Version Information
		static public string AppVersionNameFromCode( long code )
		{
			if ( ( code & 0xFF ) > 0 )
			{
				return String.Format( "{0}.{1}.{2} RC{3}", ( code >> 24 ) & 0xFF, ( code >> 16 ) & 0xFF, ( code >> 8 ) & 0xFF, code & 0xFF );
			}
			else
			{
				return String.Format( "{0}.{1}.{2}", ( code >> 24 ) & 0xFF, ( code >> 16 ) & 0xFF, ( code >> 8 ) & 0xFF );
			}
		}
		static public string AppVersionName
		{
			get
			{
				return AppVersionNameFromCode( AppVersionCode );
			}
		}
		static public int AppVersion
		{
			get
			{
				return 23;
			}
		}
		static public long AppVersionCode
		{
			get
			{
				return 0x01090300;
			}
		}
		static public DateTime AppVersionDate
		{
			get
			{
				return new DateTime( 2006, 09, 19 );
			}
		}
		#endregion
	}

	/// <summary>
	/// Static class with link building functions
	/// </summary>
	public static class yaf_BuildLink
	{
		static public string GetLink( ForumPages Page )
		{
			return Config.UrlBuilder.BuildUrl( string.Format( "g={0}", Page ) );
		}

		static public string GetLink( ForumPages Page, string format, params object [] args )
		{
			return Config.UrlBuilder.BuildUrl( string.Format( "g={0}&{1}", Page, string.Format( format, args ) ) );
		}

		static public void Redirect( ForumPages Page )
		{
			HttpContext.Current.Response.Redirect( GetLink( Page ) );
		}

		static public void Redirect( ForumPages Page, string format, params object [] args )
		{
			HttpContext.Current.Response.Redirect( GetLink( Page, format, args ) );
		}

		public static void AccessDenied()
		{
			Redirect( ForumPages.info, "i=4" );
		}

	}
}
