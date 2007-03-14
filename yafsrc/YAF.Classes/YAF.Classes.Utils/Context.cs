/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
		private string loadString = "";
		private string adminLoadString = "";

		public string LoadString
		{
			get
			{
				return loadString;
			}
		}

		public string AdminLoadString
		{
			get
			{
				return adminLoadString;
			}
		}

		/// <summary>
		/// AddLoadMessage creates a message that will be returned on the next page load.
		/// </summary>
		/// <param name="msg">The message you wish to display.</param>
		public void AddLoadMessage( string msg )
		{
			msg = msg.Replace( "\\", "\\\\" );
			msg = msg.Replace( "'", "\\'" );
			msg = msg.Replace( "\r\n", "\\r\\n" );
			msg = msg.Replace( "\n", "\\n" );
			msg = msg.Replace( "\"", "\\\"" );
			loadString += msg + "\\n\\n";
		}

		/// <summary>
		/// Instead of showing error messages in a pop-up javascript window every time
		/// the page loads (in some cases) provide a error message towards the bottom 
		/// of the page.
		/// </summary>
		/// <param name="msg"></param>
		public void AddAdminMessage( string errorType, string errorMessage )
		{
			adminLoadString = string.Format( "<div style=\"margin: 2%; padding: 7px; border: 3px Solid Red; background-color: #ccc;\"><h1>{0}</h1>{1}</div>", errorType, errorMessage );
		}		

		public void ResetLoadStrings( )
		{
			loadString = "";
			adminLoadString = "";
		}

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

		/// <summary>
		/// Helper function to see if the Page variable is populated
		/// </summary>
		public bool PageIsNull()
		{
			if ( Page == null ) return true;

			return false;
		}

		#region Forum and Page Helper Properties
		/// <summary>
		/// True if current user has post access in the current forum
		/// </summary>
		public bool ForumPostAccess
		{
			get
			{
				if ( Page.IsPostAccessNull() )
					return false;
				else
					return ( Page.PostAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user has reply access in the current forum
		/// </summary>
		public bool ForumReplyAccess
		{
			get
			{
				if ( Page.IsReplyAccessNull() )
					return false;
				else
					return ( Page.ReplyAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user has read access in the current forum
		/// </summary>
		public bool ForumReadAccess
		{
			get
			{
				if ( Page.IsReadAccessNull() )
					return false;
				else
					return ( Page.ReadAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user has access to create priority topics in the current forum
		/// </summary>
		public bool ForumPriorityAccess
		{
			get
			{
				if ( Page.IsPriorityAccessNull() )
					return false;
				else
					return ( Page.PriorityAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user has access to create polls in the current forum.
		/// </summary>
		public bool ForumPollAccess
		{
			get
			{
				if ( Page.IsPollAccessNull() )
					return false;
				else
					return ( Page.PollAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user has access to vote on polls in the current forum
		/// </summary>
		public bool ForumVoteAccess
		{
			get
			{
				if ( Page.IsVoteAccessNull() )
					return false;
				else
					return Page.VoteAccess > 0;
			}
		}
		/// <summary>
		/// True if the current user is a moderator of the current forum
		/// </summary>
		public bool ForumModeratorAccess
		{
			get
			{
				if ( Page.IsModeratorAccessNull() )
					return false;
				else
					return ( Page.ModeratorAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user can delete own messages in the current forum
		/// </summary>
		public bool ForumDeleteAccess
		{
			get
			{
				if ( Page.IsDeleteAccessNull() )
					return false;
				else
					return ( Page.DeleteAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user can edit own messages in the current forum
		/// </summary>
		public bool ForumEditAccess
		{
			get
			{
				if ( Page.IsEditAccessNull() )
					return false;
				else
					return ( Page.EditAccess > 0 );
			}
		}
		/// <summary>
		/// True if the current user can upload attachments
		/// </summary>
		public bool ForumUploadAccess
		{
			get
			{
				if ( Page.IsUploadAccessNull() )
					return false;
				else
					return ( Page.UploadAccess > 0 );
			}
		}

		public int PageBoardID
		{
			get
			{
				try
				{
					return Settings.BoardID;
				}
				catch ( Exception )
				{
					return 1;
				}
			}
		}
		/// <summary>
		/// The UserID of the current user.
		/// </summary>
		public int PageUserID
		{
			get
			{
				return ( Page == null ) ? 0 : Page.UserID;
			}
		}
		public string PageUserName
		{
			get
			{
				return ( Page == null ) ? "" : Page.UserName;
			}
		}
		/// <summary>
		/// ForumID for the current page, or 0 if not in any forum
		/// </summary>
		public int PageForumID
		{
			get
			{
				int nLockedForum = Settings.LockedForum;
				if ( nLockedForum != 0 )
					return nLockedForum;

				if ( Page != null && !Page.IsForumIDNull() )
					return Page.ForumID;

				return 0;
			}
		}
		/// <summary>
		/// Name of forum for the current page, or an empty string if not in any forum
		/// </summary>
		public string PageForumName
		{
			get
			{
				if ( Page != null && !Page.IsForumNameNull() )
					return ( string ) Page.ForumName;

				return "";
			}
		}
		/// <summary>
		/// CategoryID for the current page, or 0 if not in any category
		/// </summary>
		public int PageCategoryID
		{
			get
			{
				if ( Settings.CategoryID != 0 )
					return Settings.CategoryID;
				else if ( Page != null && !Page.IsCategoryIDNull() )
					return Page.CategoryID;

				return 0;
			}
		}
		/// <summary>
		/// Name of category for the current page, or an empty string if not in any category
		/// </summary>
		public string PageCategoryName
		{
			get
			{
				if ( Page != null && !Page.IsCategoryNameNull() )
					return Page.CategoryName;

				return "";
			}
		}
		/// <summary>
		/// The TopicID of the current page, or 0 if not in any topic
		/// </summary>
		public int PageTopicID
		{
			get
			{
				if ( Page != null && !Page.IsTopicIDNull() )
					return Page.TopicID;

				return 0;
			}
		}
		/// <summary>
		/// Name of topic for the current page, or an empty string if not in any topic
		/// </summary>
		public string PageTopicName
		{
			get
			{
				if ( Page != null && !Page.IsTopicNameNull() )
					return Page.TopicName;

				return "";
			}
		}

		/// <summary>
		/// Is the current user host admin?
		/// </summary>
		public bool IsHostAdmin
		{
			get
			{
				bool isHostAdmin = false;

				if ( Page != null )
				{
					if ( ( Page.UserFlags & ( int ) UserFlags.IsHostAdmin ) == ( int ) UserFlags.IsHostAdmin )
						isHostAdmin = true;
				}

				return isHostAdmin;
			}
		}

		/// <summary>
		/// True if current user is an administrator
		/// </summary>
		public bool IsAdmin
		{
			get
			{
				if ( IsHostAdmin )
					return true;

				if ( Page != null && !Page.IsIsAdminNull() )
					return Page.IsAdmin != 0;

				return false;
			}
		}
		/// <summary>
		/// True if the current user is a guest
		/// </summary>
		public bool IsGuest
		{
			get
			{
				if ( Page != null && !Page.IsIsGuestNull() )
					return Page.IsGuest != 0;

				return false;
			}
		}
		/// <summary>
		/// True if the current user is a forum moderator (mini-admin)
		/// </summary>
		public bool IsForumModerator
		{
			get
			{
				if ( Page != null && !Page.IsIsForumModeratorNull() )
					return Page.IsForumModerator != 0;

				return false;
			}
		}
		/// <summary>
		/// True if current user is a modeator for at least one forum
		/// </summary>
		public bool IsModerator
		{
			get
			{
				if ( Page != null && !Page.IsIsModeratorNull() )
					return Page.IsModerator != 0;

				return false;
			}
		}

		/// <summary>
		/// True if the current user is suspended
		/// </summary>
		public bool IsSuspended
		{
			get
			{
				if ( Page != null && !Page.IsSuspendedNull() )
					return true;

				return false;
			}
		}

		/// <summary>
		/// When the user is suspended until
		/// </summary>
		public DateTime SuspendedUntil
		{
			get
			{
				if ( Page == null || Page.IsSuspendedNull() )
					return DateTime.Now;
				else
					return Page.Suspended;
			}
		}

		/// <summary>
		/// The number of private messages that are unread
		/// </summary>
		public int UnreadPrivate
		{
			get
			{
				return Page.Incoming;
			}
		}

		/// <summary>
		/// True if board is private (20050909 CHP)
		/// </summary>
		public bool IsPrivate
		{
			get
			{
#if TODO
				try
				{
					return
						int.Parse(Utils.UtilsSection[string.Format("isprivate{0}", PageBoardID)])!=0;
				}
				catch
				{
					return false;
				}
#else
				return false;
#endif
			}
		}
		#endregion
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
		static public string ForumRoot
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

		static public string ForumURL
		{
			get
			{
				return string.Format( "{0}{1}", yaf_ForumInfo.ServerURL, yaf_BuildLink.GetLink( ForumPages.forum ) );
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

		static public void AccessDenied()
		{
			Redirect( ForumPages.info, "i=4" );
		}

		static public string ThemeFile( string filename )
		{
			return yaf_Context.Current.Theme.ThemeDir + filename;
		}

		static public string Smiley( string icon )
		{
			return String.Format( "{0}images/emoticons/{1}", yaf_ForumInfo.ForumRoot, icon );
		}
	}
}
