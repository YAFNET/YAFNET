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
using System.Collections;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Core
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

		private bool _allowAsPopup = false;
		public bool AllowAsPopup
		{
			get
			{
				return _allowAsPopup;
			}
			protected set
			{
				_allowAsPopup = value;
			}
		}

		private IYafHeader _header = null;
		public IYafHeader ForumHeader
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

		private IYafFooter _footer = null;
		public IYafFooter ForumFooter
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

			// set the current translation page...
			YafContext.Current.InstanceFactory.GetInstance<LocalizationHandler>().TranslationPage = _transPage;

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
			// release cache
			if (_pageCache != null) _pageCache.Clear();
		}

		private void InitProviderSettings()
		{
			PageContext.CurrentMembership.ApplicationName = PageContext.BoardSettings.MembershipAppName;
			PageContext.CurrentRoles.ApplicationName = PageContext.BoardSettings.RolesAppName;
		}
		#endregion

		#region Render Functions

		protected void InsertCssRefresh( System.Web.UI.Control addTo )
		{
			// make the style sheet link controls.
			addTo.Controls.Add( ControlHelper.MakeCssIncludeControl( YafForumInfo.GetURLToResource( "css/forum.css" ) ) );
			addTo.Controls.Add( ControlHelper.MakeCssIncludeControl( YafContext.Current.Theme.BuildThemePath( "theme.css" )) );

			if ( ForumHeader.RefreshURL != null && ForumHeader.RefreshTime >= 0 )
			{
				HtmlMeta refresh = new HtmlMeta();
				refresh.HttpEquiv = "Refresh";
				refresh.Content = String.Format( "{1};url={0}", ForumHeader.RefreshURL, ForumHeader.RefreshTime );

				addTo.Controls.Add( refresh );
			}
		}

		protected Control _topPageControl = null;
		public Control TopPageControl
		{
			get
			{
				if ( _topPageControl == null )
				{
					if ( Page != null && Page.Header != null )
					{
						_topPageControl = Page.Header;
					}
					else
					{
						_topPageControl = ControlHelper.FindControlRecursiveBoth( this, "YafHead" ) ?? ForumHeader.ThisControl;
					}
				}

				return _topPageControl; 
			}
		}

		protected void SetupHeaderElements()
		{
			InsertCssRefresh(TopPageControl);
		}

		private void ForumPage_PreRender( object sender, EventArgs e )
		{
			// sets up the head elements in addition to the Css and image elements
			SetupHeaderElements();
			// setup the forum control header & footer properties
			ForumHeader.SimpleRender = !ShowToolBar;
			ForumFooter.SimpleRender = !ShowFooter;
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
			YafServices.Permissions.HandleRequest( ViewPermissions.RegisteredUsers );
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
		public bool ShowToolBar
		{
			get
			{
				return _showToolBar;
			}
			protected set
			{
				_showToolBar = value;
			}
		}

		public bool ShowFooter
		{
			get
			{
				return _showFooter;
			}
			protected set
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
				return PageContext.LoadMessage.LoadString;
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
			if ( data == null || !(data is string)) return null;
			return Server.HtmlEncode( data.ToString() );
		}

		protected virtual void CreatePageLinks()
		{
			// Page link creation goes to this method (overloads in descendant classes)
		}
	}
}