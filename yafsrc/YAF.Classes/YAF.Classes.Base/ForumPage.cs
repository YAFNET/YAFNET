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
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Threading;
using System.Globalization;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// EventArgs class for the PageTitleSet event
	/// </summary>
	public class ForumPageRenderedArgs : EventArgs
	{
		private HtmlTextWriter _writer;

		public ForumPageRenderedArgs(System.Web.UI.HtmlTextWriter writer)
		{
			_writer = writer;
		}

		public HtmlTextWriter Writer
		{
			get { return _writer; }
		}
	}

	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class ForumPage : System.Web.UI.UserControl
	{
		/* Ederon : 6/16/2007 - conventions */

		#region Variables

		// cache for this page
		private Hashtable _pageCache;

		private bool _noDataBase = false;
    private bool _showToolBar = Config.ShowToolBar;
    private bool _showFooter = Config.ShowFooter;
    private bool _checkSuspended = true;
		private string _transPage = string.Empty;

		public event EventHandler<ForumPageRenderedArgs> ForumPageRendered;

		protected bool _isAdminPage = false;
		public bool IsAdminPage
		{
			get { return _isAdminPage; }
		}

		protected bool _isRegisteredPage = false;
		public bool IsRegisteredPage
		{
			get
			{
				return _isRegisteredPage;
			}
		}

		private YAF.Controls.Header _header = null;
		private YAF.Controls.Footer _footer = null;

		public YAF.Controls.Header ForumHeader
		{
			get
			{
				return _header;
			}
			set
			{
				_header = value;
			}
		}
		public YAF.Controls.Footer ForumFooter
		{
			get
			{
				return _footer;
			}
			set
			{
				_footer = value;
			}
		}

		#endregion

		#region Constructor and events

		/// <summary>
		/// Constructor
		/// </summary>
		public ForumPage() : this( "" ) { }
		public ForumPage( string transPage )
		{
			// create empty hashtable for cache entries
			_pageCache = new Hashtable();

			_transPage = transPage;
			this.Init += new EventHandler( ForumPage_Init );
			this.Load += new System.EventHandler( this.ForumPage_Load );
			this.Unload += new System.EventHandler( this.ForumPage_Unload );
			this.Error += new System.EventHandler( this.ForumPage_Error );
			this.PreRender += new EventHandler( ForumPage_PreRender );
		}

		private void ForumPage_Error( object sender, System.EventArgs e )
		{
			// This doesn't seem to work...
			Exception x = Server.GetLastError();
			YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
			if ( !YafForumInfo.IsLocal )
				CreateMail.CreateLogEmail( Server.GetLastError() );
		}

		/// <summary>
		/// Called first to initialize the context
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void ForumPage_Init( object sender, EventArgs e )
		{
			if ( _noDataBase )
				return;

#if DEBUG
			QueryCounter.Reset();
#endif

			if ( ForumFooter != null ) ForumFooter.StopWatch.Start();

			// set the current translation page...
			PageContext.TranslationPage = _transPage;

			// checks the DB exists and the version is current, otherwise redirects to in the install page...
			InitDB();

			// deal with banned users...
			CheckBannedIPs();

			// initialize the providers...
			InitProviderSettings();

			// check if login is required
			if ( PageContext.BoardSettings.RequireLogin && PageContext.IsGuest && IsProtected )
			{
				// redirect to login page if login is required
				YafBuildLink.Redirect( ForumPages.login, "ReturnUrl={0}", General.GetSafeRawUrl() );
			}

			if ( _checkSuspended && PageContext.IsSuspended )
			{
				if ( PageContext.SuspendedUntil < DateTime.Now )
				{
					YAF.Classes.Data.DB.user_suspend( PageContext.PageUserID, null );
					HttpContext.Current.Response.Redirect( General.GetSafeRawUrl() );
				}
				YafBuildLink.RedirectInfoPage( InfoMessage.Suspended );
			}

			// This happens when user logs in
			if ( Mession.LastVisit == DateTime.MinValue )
			{
				if ( PageContext.UnreadPrivate > 0 )
					PageContext.AddLoadMessage( GetTextFormatted( "UNREAD_MSG" , PageContext.UnreadPrivate ) );
			}

			if ( !PageContext.IsGuest && PageContext.Page ["PreviousVisit"] != DBNull.Value && !Mession.HasLastVisit )
			{
				Mession.LastVisit = Convert.ToDateTime( PageContext.Page ["PreviousVisit"] );
				Mession.HasLastVisit = true;
			}
			else if ( Mession.LastVisit == DateTime.MinValue )
			{
				Mession.LastVisit = DateTime.Now;
			}
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForumPage_Load( object sender, System.EventArgs e )
		{
			if ( PageContext.BoardSettings.DoUrlReferrerSecurityCheck ) Security.CheckRequestValidity( Request );
		}

		/// <summary>
		/// Called when the page is unloaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForumPage_Unload( object sender, System.EventArgs e )
		{
			// reset stuff...
			PageContext.ResetLoadStrings();
			if ( ForumHeader != null ) ForumHeader.Reset();
			if ( ForumFooter != null ) ForumFooter.Reset();
			
			// release cache
			if (_pageCache != null) _pageCache.Clear();
		}

		/// <summary>
		/// Checks for DB connectivity and DB version
		/// </summary>
		private void InitDB()
		{
			// 1st test if for DB connectivity...
			string errorStr = "";
			bool debugging = false;

#if DEBUG
        debugging = true;
#endif

			// attempt to init the db...
			if ( !DB.forumpage_initdb( out errorStr, debugging ) )
			{
				// unable to connect to the DB...
				Session["StartupException"] = errorStr;
				Response.Redirect( YafForumInfo.ForumRoot + "error.aspx" );				
			}

			// step 2: validate the database version...
			string redirectStr = DB.forumpage_validateversion( YafForumInfo.AppVersion );
			if ( !String.IsNullOrEmpty( redirectStr ) ) Response.Redirect( YafForumInfo.ForumRoot + redirectStr );
		}

		/// <summary>
		/// Look for banned IPs and handle
		/// </summary>
		private void CheckBannedIPs()
		{
			string key = YafCache.GetBoardCacheKey( Constants.Cache.BannedIP );

			// load the banned IP table...
			DataTable bannedIPs = ( DataTable ) YafCache.Current [key];

			if ( bannedIPs == null )
			{
				// load the table and cache it...
				bannedIPs = DB.bannedip_list( PageContext.PageBoardID, null );
				YafCache.Current [key] = bannedIPs;
			}

			// check for this user in the list...
			foreach ( DataRow row in bannedIPs.Rows )
			{
				if ( General.IsBanned( ( string ) row ["Mask"], HttpContext.Current.Request.ServerVariables ["REMOTE_ADDR"] ) )
					HttpContext.Current.Response.End();
			}
		}

		private void InitProviderSettings()
		{
			System.Web.Security.Membership.ApplicationName = PageContext.BoardSettings.MembershipAppName;
			System.Web.Security.Roles.ApplicationName = PageContext.BoardSettings.RolesAppName;
		}
		#endregion

		#region Render Functions

		protected void InsertCssRefresh( System.Web.UI.Control addTo )
		{
			// make the style sheet link controls.
			addTo.Controls.Add( ControlHelper.MakeCssIncludeControl( YafForumInfo.GetURLToResource( "css/forum.css" ) ) );
			addTo.Controls.Add( ControlHelper.MakeCssIncludeControl( YafBuildLink.ThemeFile( "theme.css" ) ) );

			if ( ForumHeader.RefreshURL != null && ForumHeader.RefreshTime >= 0 )
			{
				HtmlMeta refresh = new HtmlMeta();
				refresh.HttpEquiv = "Refresh";
				refresh.Content = String.Format( "{1};url={0}", ForumHeader.RefreshURL, ForumHeader.RefreshTime );

				addTo.Controls.Add( refresh );
			}
		}

		public Control GetTopPageElement()
		{
			return ControlHelper.FindControlRecursiveBoth(this, "YafHead") ?? ForumHeader;
		}

		protected void SetupHeaderElements()
		{
			HtmlImage graphctl;
			if ( PageContext.BoardSettings.AllowThemedLogo & !Config.IsDotNetNuke & !Config.IsPortal & !Config.IsRainbow )
			{
				graphctl = ( HtmlImage )Page.FindControl( "imgBanner" );
				if ( graphctl != null )
				{
					graphctl.Src = GetThemeContents( "FORUM", "BANNER" );
				}
			}

			InsertCssRefresh( GetTopPageElement() );
		}

		private void ForumPage_PreRender( object sender, EventArgs e )
		{
			// sets up the head elements in addition to the Css and image elements
			SetupHeaderElements();
			// setup the forum control header & footer properties
			ForumHeader.SimpleRender = !_showToolBar;
			ForumFooter.SimpleRender = !_showFooter;
		}

		/// <summary>
		/// Writes the document
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
			// handle additional rendering if desired...
			if (ForumPageRendered != null) ForumPageRendered( this, new ForumPageRenderedArgs( writer ) );
		}

		#endregion

		#region Page/User properties
		/// <summary>
		/// Set to true if this is the start page. Should only be set by the page that initialized the database.
		/// </summary>
		protected bool NoDataBase
		{
			set
			{
				_noDataBase = value;
			}
		}

		public void RedirectNoAccess()
		{
			General.HandleRequest( PageContext, ViewPermissions.RegisteredUsers );
		}
		#endregion

		#region Page Cache
		
		/// <summary>
		/// Gets cache associated with this page.
		/// </summary>
		public Hashtable PageCache
		{
			get { return _pageCache; }
		}

		#endregion

		#region Other
		/// <summary>
		/// Adds a message that is displayed to the user when the page is loaded.
		/// </summary>
		/// <param name="msg">The message to display</param>
		public string RefreshURL
		{
			set
			{
				if ( ForumHeader != null ) ForumHeader.RefreshURL = value;
			}
			get
			{
				if ( ForumHeader != null ) return ForumHeader.RefreshURL;
				return null;
			}
		}
		public int RefreshTime
		{
			set
			{
				if ( ForumHeader != null ) ForumHeader.RefreshTime = value;
			}
			get
			{
				if ( ForumHeader != null ) return ForumHeader.RefreshTime;
				return 0;
			}
		}

		/// <summary>
		/// Gets info whether page should be hidden to guest users when forum admin requires login.
		/// </summary>
		public virtual bool IsProtected
		{
			get { return true; }
		}

		#endregion

		#region Layout functions
		/// <summary>
		/// Set to false if you don't want the menus at top and bottom. Only admin pages will set this to false
		/// </summary>
		protected bool ShowToolBar
		{
			set
			{
				_showToolBar = value;
			}
		}

		protected bool ShowFooter
		{
			set
			{
				_showFooter = value;
			}
		}
		#endregion

		public bool CheckSuspended
		{
			set
			{
				_checkSuspended = value;
			}
		}

		static public object IsNull( string value )
		{
			if ( value == null || value.ToLower() == string.Empty )
				return DBNull.Value;
			else
				return value;
		}

		#region PageInfo class

		[Obsolete( "Useless property that always returns true. Do not use anymore." )]
		public bool CanLogin
		{
			get
			{
				return true;
			}
		}

		public MembershipUser User
		{
			get
			{
				return PageContext.User;
			}
		}

		public string LoadMessage
		{
			get
			{
				return PageContext.LoadString;
			}
		}

		#region Theme Helper Functions
		/// <summary>
		/// Get a value from the currently configured forum theme.
		/// </summary>
		/// <param name="page">Page to look under</param>
		/// <param name="tag">Theme item</param>
		/// <returns>Converted Theme information</returns>
		public string GetThemeContents( string page, string tag )
		{
			return PageContext.Theme.GetItem( page, tag );
		}

		/// <summary>
		/// Get a value from the currently configured forum theme.
		/// </summary>
		/// <param name="page">Page to look under</param>
		/// <param name="tag">Theme item</param>
		/// <param name="defaultValue">Value to return if the theme item doesn't exist</param>
		/// <returns>Converted Theme information or Default Value if it doesn't exist</returns>
		public string GetThemeContents( string page, string tag, string defaultValue )
		{
			return PageContext.Theme.GetItem( page, tag, defaultValue );
		}

		/// <summary>
		/// Gets the collapsible panel image url (expanded or collapsed). 
		/// 
		/// <param name="panelID">ID of collapsible panel</param>
		/// <param name="defaultState">Default Panel State</param>
		/// </summary>
		/// <returns>Image URL</returns>
		public string GetCollapsiblePanelImageURL( string panelID, PanelSessionState.CollapsiblePanelState defaultState )
		{
			return PageContext.Theme.GetCollapsiblePanelImageURL( panelID, defaultState );
		}
		#endregion

		#region Localization Helper Functions

		public string GetTextFormatted(string text, params object [] args)
		{
			return PageContext.Localization.GetTextFormatted( text, args );
		}

		public string GetText( string text )
		{
			return PageContext.Localization.GetText( text );
		}

		public string GetText( string page, string text )
		{
			return PageContext.Localization.GetText( page, text );
		}

		#endregion

		/// <summary>
		/// Gets the current forum Context (helper reference)
		/// </summary>
		public YafContext PageContext
		{
			get
			{
				return YafContext.Current;
			}
		}

		public string ForumURL
		{
			get
			{
				return string.Format( "{0}{1}", YafForumInfo.ServerURL, YafBuildLink.GetLink( ForumPages.forum ) );
			}
		}

		#endregion

		public string HtmlEncode( object data )
		{
			return Server.HtmlEncode( data.ToString() );
		}

		protected virtual void CreatePageLinks()
		{
			// Page link creation goes to this method (overloads in descendant classes)
		}
	}
}
