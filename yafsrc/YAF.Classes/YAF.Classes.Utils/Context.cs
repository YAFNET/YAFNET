/* Yet Another Forum.NET
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
	public class YafContext
	{
		/* Ederon : 6/16/2007 - conventions */

		private static YafContext _currentInstance = new YafContext();
		
		private System.Data.DataRow _page = null;
		private YAF.Classes.Utils.YafControlSettings _settings = null;
		private YAF.Classes.Utils.YafTheme _theme = null;
		private YAF.Classes.Utils.YafLocalization _localization = null;
		private System.Web.Security.MembershipUser _user = null;
		private string _loadString = "";
		private string _adminLoadString = "";
		private UserFlags _userFlags = null;

		public string LoadString
		{
			get
			{
				if ( HttpContext.Current.Session ["LoadMessage"] != null )
				{
					// get this as the current "loadstring"
					_loadString = HttpContext.Current.Session ["LoadMessage"].ToString();
					// session load string no longer needed
					HttpContext.Current.Session ["LoadMessage"] = null;
				}
				return _loadString;
			}
		}

		public string AdminLoadString
		{
			get
			{
				return _adminLoadString;
			}
		}

		/// <summary>
		/// AddLoadMessage creates a message that will be returned on the next page load.
		/// </summary>
		/// <param name="message">The message you wish to display.</param>
		public void AddLoadMessage( string message )
		{
			message = message.Replace("\\", "\\\\");
			message = message.Replace( "'", "\\'" );
			message = message.Replace( "\r\n", "\\r\\n" );
			message = message.Replace( "\n", "\\n" );
			message = message.Replace( "\"", "\\\"" );
			_loadString += message + "\\n\\n";
		}

		/// <summary>
		/// AddLoadMessageSession creates a message that will be returned on the next page.
		/// </summary>
		/// <param name="message">The message you wish to display.</param>
		public void AddLoadMessageSession( string message )
		{
			message = message.Replace( "\\", "\\\\" );
			message = message.Replace( "'", "\\'" );
			message = message.Replace( "\r\n", "\\r\\n" );
			message = message.Replace( "\n", "\\n" );
			message = message.Replace( "\"", "\\\"" );

			HttpContext.Current.Session["LoadMessage"] = message + "\\n\\n";
		}

		/// <summary>
		/// Instead of showing error messages in a pop-up javascript window every time
		/// the page loads (in some cases) provide a error message towards the bottom 
		/// of the page.
		/// </summary>
		public void AddAdminMessage( string errorType, string errorMessage )
		{
			_adminLoadString = string.Format( "<div style=\"margin: 2%; padding: 7px; border: 3px Solid Red; background-color: #ccc;\"><h1>{0}</h1>{1}</div>", errorType, errorMessage );
		}

		public void ResetLoadStrings()
		{
			_loadString = "";
			_adminLoadString = "";
		}

		public static YafContext Current
		{
			get
			{
				return _currentInstance;
			}
		}

		public System.Data.DataRow Page
		{
			get
			{
				return _page;
			}
			set
			{
				_page = value;

				// get user flags
				if (_page != null) _userFlags = new UserFlags(_page["UserFlags"]);
				else _userFlags = null;
			}
		}

		public YAF.Classes.Utils.YafUserProfile Profile
		{
			get
			{
				return ( YafUserProfile ) HttpContext.Current.Profile;
			}
		}

		public YAF.Classes.Utils.YafControlSettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				_settings = value;
			}
		}

		public YAF.Classes.Utils.YafTheme Theme
		{
			get
			{
				return _theme;
			}
			set
			{
				_theme = value;
			}
		}

		public YAF.Classes.Utils.YafLocalization Localization
		{
			get
			{
				return _localization;
			}
			set
			{
				_localization = value;
			}
		}

		public System.Web.Security.MembershipUser User
		{
			get
			{
				if ( _user == null )
					_user = System.Web.Security.Membership.GetUser();
				return _user;
			}
			set
			{
				_user = value;
			}
		}

		public YafBoardSettings BoardSettings
		{
			get
			{
				int PageBoardID = ( Settings == null ) ? 1 : Settings.BoardID;
				string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

				if ( HttpContext.Current.Application [key] == null )
					HttpContext.Current.Application [key] = new YafBoardSettings( PageBoardID );

				return ( YafBoardSettings ) HttpContext.Current.Application [key];

			}
			set
			{
				int PageBoardID = ( Settings == null ) ? 1 : Settings.BoardID;
				string key = YafCache.GetBoardCacheKey(Constants.Cache.BoardSettings);

				HttpContext.Current.Application.Remove( key );
			}
		}

		/// <summary>
		/// Helper function to get a profile from the system
		/// </summary>
		/// <param name="userName"></param>
		/// <returns></returns>
		public YafUserProfile GetProfile( string userName )
		{
      //if ( profile == null )
      //{
      //  // doesn't exist...
      //  profile = ( YafUserProfile ) System.Web.Profile.ProfileBase.Create( userName );
      //  HttpContext.Current.Cache.Add(key,profile,null,DateTime.Now.AddMinutes(30),TimeSpan.Zero,System.Web.Caching.CacheItemPriority.Default,null);
      //}


      return ( YafUserProfile ) System.Web.Profile.ProfileBase.Create( userName );
		}

		/// <summary>
		/// Get the current page as the forumPage Enum (for comparison)
		/// </summary>
		public YAF.Classes.Utils.ForumPages ForumPageType
		{
			get
			{
				ForumPages currentPage = ForumPages.forum;
				try
				{
					currentPage = ( ForumPages ) System.Enum.Parse( typeof( ForumPages ), HttpContext.Current.Request.QueryString ["g"], true );
				}
				catch ( Exception )
				{
					currentPage = ForumPages.forum;
				}
				return currentPage;
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

		/// <summary>
		/// Helper function used for redundant "access" fields internally
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool AccessNotNull( string field )
		{
			if ( Page [field] == DBNull.Value ) return false;
			return ( Convert.ToInt32( Page [field] ) > 0 );
		}

		/// <summary>
		/// Internal helper function used for redundant page variable access (bool)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private bool PageValueAsBool( string field )
		{
			if ( Page != null && Page [field] != DBNull.Value )
				return Convert.ToInt32( Page [field] ) != 0;

			return false;
		}

		/// <summary>
		/// Internal helper function used for redundant page variable access (int)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private int PageValueAsInt( string field )
		{
			if ( Page != null && Page [field] != DBNull.Value )
				return Convert.ToInt32( Page [field] );

			return 0;
		}

		/// <summary>
		/// Internal helper function used for redudant page variable access (string)
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		private string PageValueAsString( string field )
		{
			if ( Page != null && Page [field] != DBNull.Value )
				return Page [field].ToString();

			return "";
		}


		#region Forum and Page Helper Properties
		/// <summary>
		/// True if current user has post access in the current forum
		/// </summary>
		public bool ForumPostAccess
		{
			get
			{
				return AccessNotNull( "PostAccess" );
			}
		}
		/// <summary>
		/// True if the current user has reply access in the current forum
		/// </summary>
		public bool ForumReplyAccess
		{
			get
			{
				return AccessNotNull( "ReplyAccess" );
			}
		}
		/// <summary>
		/// True if the current user has read access in the current forum
		/// </summary>
		public bool ForumReadAccess
		{
			get
			{
				return AccessNotNull( "ReadAccess" );
			}
		}
		/// <summary>
		/// True if the current user has access to create priority topics in the current forum
		/// </summary>
		public bool ForumPriorityAccess
		{
			get
			{
				return AccessNotNull( "PriorityAccess" );
			}
		}
		/// <summary>
		/// True if the current user has access to create polls in the current forum.
		/// </summary>
		public bool ForumPollAccess
		{
			get
			{
				return AccessNotNull( "PollAccess" );
			}
		}
		/// <summary>
		/// True if the current user has access to vote on polls in the current forum
		/// </summary>
		public bool ForumVoteAccess
		{
			get
			{
				return AccessNotNull( "VoteAccess" );
			}
		}
		/// <summary>
		/// True if the current user is a moderator of the current forum
		/// </summary>
		public bool ForumModeratorAccess
		{
			get
			{
				return AccessNotNull( "ModeratorAccess" );
			}
		}
		/// <summary>
		/// True if the current user can delete own messages in the current forum
		/// </summary>
		public bool ForumDeleteAccess
		{
			get
			{
				return AccessNotNull( "DeleteAccess" );
			}
		}
		/// <summary>
		/// True if the current user can edit own messages in the current forum
		/// </summary>
		public bool ForumEditAccess
		{
			get
			{
				return AccessNotNull( "EditAccess" );
			}
		}
		/// <summary>
		/// True if the current user can upload attachments
		/// </summary>
		public bool ForumUploadAccess
		{
			get
			{
				return AccessNotNull( "UploadAccess" );
			}
		}
		/// <summary>
		/// True if the current user can download attachments
		/// </summary>
		public bool ForumDownloadAccess
		{
			get
			{
				return AccessNotNull("DownloadAccess");
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
				return PageValueAsInt( "UserID" );
			}
		}
		public string PageUserName
		{
			get
			{
				return PageValueAsString( "UserName" );
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

				return PageValueAsInt( "ForumID" );
			}
		}
		/// <summary>
		/// Name of forum for the current page, or an empty string if not in any forum
		/// </summary>
		public string PageForumName
		{
			get
			{
				return PageValueAsString( "ForumName" );
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
				{
					return Settings.CategoryID;
				}

				return PageValueAsInt( "CategoryID" );
			}
		}
		/// <summary>
		/// Name of category for the current page, or an empty string if not in any category
		/// </summary>
		public string PageCategoryName
		{
			get
			{
				return PageValueAsString( "CategoryName" );
			}
		}
		/// <summary>
		/// The TopicID of the current page, or 0 if not in any topic
		/// </summary>
		public int PageTopicID
		{
			get
			{
				return PageValueAsInt( "TopicID" );
			}
		}
		/// <summary>
		/// Name of topic for the current page, or an empty string if not in any topic
		/// </summary>
		public string PageTopicName
		{
			get
			{
				return PageValueAsString( "TopicName" );
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

				if (_userFlags != null)
				{
					isHostAdmin = _userFlags.IsHostAdmin;
					// Obsolette : Ederon
					// if (General.BinaryAnd(Page["UserFlags"], UserFlags.IsHostAdmin))
					//	isHostAdmin = true;
				}

				return isHostAdmin;
			}
		}

		/// <summary>
		/// True if user is excluded from CAPTCHA check.
		/// </summary>
		public bool IsCaptchaExcluded
		{
			get
			{
				bool isCaptchaExcluded = false;

				if (_userFlags != null)
				{
					isCaptchaExcluded = _userFlags.IsCaptchaExcluded;
				}

				return isCaptchaExcluded;
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

				return PageValueAsBool( "IsAdmin" );
			}
		}
		/// <summary>
		/// True if the current user is a guest
		/// </summary>
		public bool IsGuest
		{
			get
			{
				return PageValueAsBool( "IsGuest" );
			}
		}
		/// <summary>
		/// True if the current user is a forum moderator (mini-admin)
		/// </summary>
		public bool IsForumModerator
		{
			get
			{
				return PageValueAsBool( "IsForumModerator" );
			}
		}
		/// <summary>
		/// True if current user is a modeator for at least one forum
		/// </summary>
		public bool IsModerator
		{
			get
			{
				return PageValueAsBool( "IsModerator" );
			}
		}

		/// <summary>
		/// True if the current user is suspended
		/// </summary>
		public bool IsSuspended
		{
			get
			{
				if ( Page != null && Page ["Suspended"] != DBNull.Value )
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
				if ( Page == null || Page ["Suspended"] == DBNull.Value )
					return DateTime.Now;
				else
					return Convert.ToDateTime( Page ["Suspended"] );
			}
		}

		/// <summary>
		/// The number of private messages that are unread
		/// </summary>
		public int UnreadPrivate
		{
			get
			{
				return Convert.ToInt32( Page ["Incoming"] );
			}
		}

		/// <summary>
		/// The time zone offset for the user
		/// </summary>
		public int TimeZoneUser
		{
			get
			{
				return Convert.ToInt32( Page ["TimeZoneUser"] );
			}
		}

		/// <summary>
		/// The language file for the user
		/// </summary>
		public string LanguageFile
		{
			get
			{
				return PageValueAsString( "LanguageFile" );
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
	public class YafControlSettings
	{
		/* Ederon : 6/16/2007 - conventions */

		private int _boardID;
		private int _categoryID;
		private int _lockedForum = 0;

		public YafControlSettings()
		{
			try
			{
				_categoryID = int.Parse( Config.CategoryID );
			}
			catch
			{
				// Ederon : 6/16/2007 - changed from 1 to 0
				_categoryID = 0;
			}

			try
			{
				_boardID = int.Parse( Config.BoardID );
			}
			catch
			{
				_boardID = 1;
			}
		}

		public int BoardID
		{
			get
			{
				return _boardID;
			}
			set
			{
				_boardID = value;
			}
		}

		public int CategoryID
		{
			get
			{
				return _categoryID;
			}
			set
			{
				_categoryID = value;
			}
		}

		public int LockedForum
		{
			set
			{
				_lockedForum = value;
			}
			get
			{
				return _lockedForum;
			}
		}
	}

	/// <summary>
	/// Class provides misc helper functions and forum version information
	/// </summary>
	public static class YafForumInfo
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
				return string.Format( "{0}{1}", YafForumInfo.ServerURL, YafBuildLink.GetLink( ForumPages.forum ) );
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
			string version;

			if ( ( code & 0xF0 ) > 0 )
			{
				version = String.Format( "{0}.{1}.{2}.{3}", ( code >> 24 ) & 0xFF, ( code >> 16 ) & 0xFF, ( code >> 8 ) & 0xFF, ( code >> 4 ) & 0x0F );
			}
			else
			{
				version = String.Format( "{0}.{1}.{2}", ( code >> 24 ) & 0xFF, ( code >> 16 ) & 0xFF, ( code >> 8 ) & 0xFF );
			}

			if ( ( code & 0x0F ) > 0 )
			{				
				if ( ( code & 0x0F ) == 1 )
				{
					// alpha release...
					version += " alpha";
				}
				else if ( ( code & 0x0F ) == 2 )
				{
					version += " beta";
				}
				else
				{
					// Add Release Candidate
					version += string.Format( " RC{0}", (code & 0x0F) - 2 );
				}
			}

			return version;
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
				return 30;
			}
		}
		static public long AppVersionCode
		{
			get
			{
				return 0x01090301;
			}
		}
		static public DateTime AppVersionDate
		{
			get
			{
				return new DateTime( 2007, 12, 18 );
			}
		}
		#endregion
	}
}
