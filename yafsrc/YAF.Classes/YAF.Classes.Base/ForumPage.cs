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
using System.IO;
using System.Text;
using System.Collections;
using System.Data;
using System.Xml;
using System.Web;
using System.Web.Security;
using System.Threading;
using System.Globalization;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Classes.Base
{
	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class ForumPage : System.Web.UI.UserControl
	{
		#region Variables

		private System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
		private string m_strRefreshURL = null;
		private int m_nRefreshTime = 2;
		private bool m_bNoDataBase = false;
		private bool m_bShowToolBar = true;
		private bool m_checkSuspended = true;
		private string m_transPage = string.Empty;

		#endregion

		#region Constructor and events
		/// <summary>
		/// Constructor
		/// </summary>
		public ForumPage()
			: this( "" )
		{

		}

		public ForumPage( string transPage )
		{
			m_transPage = transPage;
			stopWatch.Start();

			this.Load += new System.EventHandler( this.ForumPage_Load );
			this.Error += new System.EventHandler( this.ForumPage_Error );
			this.PreRender += new EventHandler( ForumPage_PreRender );
		}

		private void ForumPage_Error( object sender, System.EventArgs e )
		{
			// This doesn't seem to work...
			Exception x = Server.GetLastError();
			YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
			if ( !yaf_ForumInfo.IsLocal )
				General.LogToMail( Server.GetLastError() );
		}

		static public int ValidInt( object o )
		{
			try
			{
				if ( o == null )
					return 0;

				return int.Parse( o.ToString() );
			}
			catch ( Exception )
			{
				return 0;
			}
		}

		/// <summary>
		/// Called when page is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ForumPage_Load( object sender, System.EventArgs e )
		{
			if ( m_bNoDataBase )
				return;

#if DEBUG
			QueryCounter.Reset();
#endif

			// setup the culture based on the browser...
			InitCulture();

			// checks the DB exists and the version is current, otherwise redirects to in the install page...
			InitDB();

			// deal with banned users...
			CheckBannedIPs();

			// initalize the user and current page data...
			InitUserAndPage();

			// initalize theme
			InitTheme();

			// initalize localization
			InitLocalization();

			//if (user!=null && m_pageinfo["ProviderUserKey"] == DBNull.Value)
			//    throw new ApplicationException("User not migrated to ASP.NET 2.0");

			if ( m_checkSuspended && PageContext.IsSuspended )
			{
				if ( PageContext.SuspendedUntil < DateTime.Now )
				{
					YAF.Classes.Data.DB.user_suspend( PageContext.PageUserID, null );
					HttpContext.Current.Response.Redirect( General.GetSafeRawUrl() );
				}
				yaf_BuildLink.Redirect( ForumPages.info, "i=2" );
			}

			// This happens when user logs in
			if ( Mession.LastVisit == DateTime.MinValue )
			{
				if ( PageContext.Page.Incoming > 0 )
					PageContext.AddLoadMessage( String.Format( GetText( "UNREAD_MSG" ), PageContext.Page.Incoming ) );
			}

			if ( !PageContext.IsGuest && !PageContext.Page.IsPreviousVisitNull() && !Mession.HasLastVisit )
			{
				Mession.LastVisit = ( DateTime ) PageContext.Page.PreviousVisit;
				Mession.HasLastVisit = true;
			}
			else if ( Mession.LastVisit == DateTime.MinValue )
			{
				Mession.LastVisit = DateTime.Now;
			}

			// Check if pending mails, and send 10 of them if possible
			if ( PageContext.Page.MailsPending > 0 )
			{
				try
				{
					using ( DataTable dt = YAF.Classes.Data.DB.mail_list() )
					{
						for ( int i = 0; i < dt.Rows.Count; i++ )
						{
							// Build a MailMessage
							if ( dt.Rows [i] ["ToUser"].ToString().Trim() != String.Empty )
							{
								General.SendMail( PageContext.BoardSettings.ForumEmail, ( string ) dt.Rows [i] ["ToUser"], ( string ) dt.Rows [i] ["Subject"], ( string ) dt.Rows [i] ["Body"] );
							}
							YAF.Classes.Data.DB.mail_delete( dt.Rows [i] ["MailID"] );
						}
						if ( PageContext.IsAdmin ) PageContext.AddAdminMessage( "Sent Mail", String.Format( "Sent {0} mails.", dt.Rows.Count ) );
					}
				}
				catch ( Exception x )
				{
					YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, x );
					if ( PageContext.IsAdmin )
					{
						PageContext.AddAdminMessage( "Error sending emails to users", x.ToString() );
					}
				}
			}
		}

		/// <summary>
		/// Checks for DB connectivity and DB version
		/// </summary>
		private void InitDB()
		{
			try
			{
				// validate the version of the database (also check for connectivity)...
				YAF.Classes.Data.YAFDBTableAdapters.yaf_RegistryTableAdapter adapter = new YAF.Classes.Data.YAFDBTableAdapters.yaf_RegistryTableAdapter();
				YAFDB.yaf_RegistryDataTable registry = adapter.GetData( "Version", null );

				if ( (registry.Count == 0) || (Convert.ToInt32( registry [0].Value ) < yaf_ForumInfo.AppVersion) )
				{
					// needs upgrading...
					Response.Redirect( yaf_ForumInfo.ForumRoot + "install/" );
				}
			}
			catch ( Exception ex )
			{
				// If the above fails chances are that this is a new install
				Response.Redirect( yaf_ForumInfo.ForumRoot + "install/" );
			}
		}

		/// <summary>
		/// Look for banned IPs and handle
		/// </summary>
		private void CheckBannedIPs()
		{
			string key = string.Format( "BannedIP.{0}", PageContext.PageBoardID );			
			
			// load the banned IP table...
			YAFDB.yaf_BannedIPDataTable bannedIPs = ( YAFDB.yaf_BannedIPDataTable ) HttpContext.Current.Cache [key];
			if ( bannedIPs == null )
			{
				// load the table and cache it...
				YAF.Classes.Data.YAFDBTableAdapters.yaf_BannedIPTableAdapter adapter = new YAF.Classes.Data.YAFDBTableAdapters.yaf_BannedIPTableAdapter();
				bannedIPs = adapter.GetData( PageContext.PageBoardID, null );
				HttpContext.Current.Cache [key] = bannedIPs;
			}

			// check for this user in the list...
			foreach ( YAFDB.yaf_BannedIPRow row in bannedIPs )
			{
				if ( General.IsBanned( ( string ) row.Mask, HttpContext.Current.Request.ServerVariables ["REMOTE_ADDR"] ) )
					HttpContext.Current.Response.End();
			}
		}

		/// <summary>
		/// Set the culture and UI culture to the browser's accept language
		/// </summary>
		private void InitCulture()
		{			
			try
			{
				string sCulture = "";
				string [] sTmp = HttpContext.Current.Request.UserLanguages;
				if ( sTmp != null )
				{
					sCulture = sTmp [0];
					if ( sCulture.IndexOf( ';' ) >= 0 )
					{
						sCulture = sCulture.Substring( 0, sCulture.IndexOf( ';' ) ).Replace( '_', '-' );
					}
				}
				else
				{
					sCulture = "en-US";
				}

				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture( sCulture );
				Thread.CurrentThread.CurrentUICulture = new CultureInfo( sCulture );

			}
#if DEBUG
			catch ( Exception ex )
			{
				YAF.Classes.Data.DB.eventlog_create( PageContext.PageUserID, this, ex );
				throw new ApplicationException( "Error getting User Language." + Environment.NewLine + ex.ToString() );
			}
#else
			catch(Exception)
			{
				// set to default...
				Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture( "en-US" );
				Thread.CurrentThread.CurrentUICulture = new CultureInfo( "en-US" );
			}
#endif
		}

		/// <summary>
		/// Initalize the user data and page data...
		/// </summary>
		private void InitUserAndPage()
		{
			YAFDB.yaf_PageLoadDataTable pageLoad;
			YAF.Classes.Data.YAFDBTableAdapters.yaf_PageLoadTableAdapter taPageLoad = new YAF.Classes.Data.YAFDBTableAdapters.yaf_PageLoadTableAdapter();

			// Find user name
			MembershipUser user = Membership.GetUser();
			if ( user != null && Session ["UserUpdated"] == null )
			{
				Security.UpdateForumUser( PageContext.PageBoardID, user );
				Session ["UserUpdated"] = true;
			}

			string browser = String.Format( "{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version );
			string platform = HttpContext.Current.Request.Browser.Platform;

			if ( HttpContext.Current.Request.UserAgent != null )
			{
				if ( HttpContext.Current.Request.UserAgent.IndexOf( "Windows NT 5.2" ) >= 0 )
					platform = "Win2003";
			}

			int? categoryID = ValidInt( HttpContext.Current.Request.QueryString ["c"] );
			int? forumID = ValidInt( HttpContext.Current.Request.QueryString ["f"] );
			int? topicID = ValidInt( HttpContext.Current.Request.QueryString ["t"] );
			int? messageID = ValidInt( HttpContext.Current.Request.QueryString ["m"] );

			if ( PageContext.Settings.CategoryID != 0 )
				categoryID = PageContext.Settings.CategoryID;

			object userKey = null;
			if ( user != null )
				userKey = user.ProviderUserKey;

			do
			{
				pageLoad = taPageLoad.GetData(
						HttpContext.Current.Session.SessionID,
						PageContext.PageBoardID,
						(System.Guid?)userKey,
						HttpContext.Current.Request.UserHostAddress,
						HttpContext.Current.Request.FilePath,
						browser,
						platform,
						categoryID,
						forumID,
						topicID,
						messageID );

				// if the user doesn't exist...
				if ( user != null && pageLoad.Count == 0 )
				{
					// create the user...
					if ( !Security.CreateForumUser( user, PageContext.PageBoardID ) )
						throw new ApplicationException( "Failed to use new user." );
				}

				// only continue if either the page has been loaded or the user has been found...
			} while ( pageLoad.Count == 0 && user != null );

			// page still hasn't been loaded...
			if ( pageLoad.Count == 0 )
			{
				if ( user != null )
					throw new ApplicationException( string.Format( "User '{0}' isn't registered.", user.UserName ) );
				else
					throw new ApplicationException( "Failed to find guest user." );
			}

			// save this page data to the context...
			PageContext.Page = pageLoad [0];
		}

		/// <summary>
		/// Sets the theme class up for usage
		/// </summary>
		private void InitTheme()
		{
			string themeFile = null;

			if ( PageContext.Page != null && !PageContext.Page.IsThemeFileNull() && PageContext.BoardSettings.AllowUserTheme )
			{
				// use user-selected themem
				themeFile = PageContext.Page.ThemeFile;
			}
			else if ( PageContext.Page != null && !PageContext.Page.IsForumThemeNull() )
			{
				themeFile = PageContext.Page.ForumTheme;
			}
			else
			{
				themeFile = PageContext.BoardSettings.Theme;
			}

			if ( themeFile == null )
			{
				themeFile = "standard.xml";
			}		

			// create the theme class
			PageContext.Theme = new YAF.Classes.Utils.yaf_Theme( themeFile );
		}

		/// <summary>
		/// Sets up the localization class for usage
		/// </summary>
		private void InitLocalization()
		{
			PageContext.Localization = new YAF.Classes.Utils.yaf_Localization(m_transPage);
		}
		#endregion

		#region Render Functions

		private string m_headerInfo = null;

		private void ForumPage_PreRender( object sender, EventArgs e )
		{
			System.Web.UI.HtmlControls.HtmlImage graphctl;
			if ( PageContext.BoardSettings.AllowThemedLogo & !YAF.Classes.Config.IsDotNetNuke & !YAF.Classes.Config.IsPortal & !YAF.Classes.Config.IsRainbow )
			{
				graphctl = ( System.Web.UI.HtmlControls.HtmlImage ) Page.FindControl( "imgBanner" );
				if ( graphctl != null )
				{
					graphctl.Src = GetThemeContents( "FORUM", "BANNER" );
				}
			}

			System.Web.UI.HtmlControls.HtmlTitle ctl;
			ctl = ( System.Web.UI.HtmlControls.HtmlTitle ) Page.FindControl( "ForumTitle" );

			if ( ctl != null )
			{
				System.Text.StringBuilder title = new StringBuilder();
				if ( PageContext.PageTopicID != 0 )
					title.AppendFormat( "{0} - ", General.BadWordReplace( PageContext.PageTopicName ) ); // Tack on the topic we're viewing
				if ( PageContext.PageForumName != string.Empty )
					title.AppendFormat( "{0} - ", Server.HtmlEncode( PageContext.PageForumName ) ); // Tack on the forum we're viewing
				title.Append( Server.HtmlEncode( PageContext.BoardSettings.Name ) ); // and lastly, tack on the board's name
				ctl.Text = title.ToString();
			}

			// BEGIN HEADER
			StringBuilder header = new StringBuilder();

			// get the theme header -- usually used for javascript
			string themeHeader = GetThemeContents( "THEME", "HEADER", null );

			if ( themeHeader != null && themeHeader.Length > 0 )
			{
				header.Append(themeHeader);
			} 

			header.AppendFormat( "<table width=100% cellspacing=0 class=content cellpadding=0><tr>" );

			MembershipUser user = Membership.GetUser();

			if ( user != null )
			{
				header.AppendFormat( "<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>", String.Format( GetText( "TOOLBAR", "LOGGED_IN_AS" ) + " ", Server.HtmlEncode( PageContext.PageUserName ) ) );
				header.AppendFormat( "<td style=\"padding:5px\" align=right valign=middle class=post>" );
				if ( !PageContext.IsGuest )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.cp_inbox ), GetText( "CP_INBOX", "TITLE" ) ) );

				/* TODO: help is currently useless...
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.help_index ), GetText( "TOOLBAR", "HELP" ) ) );
				*/

				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.search ), GetText( "TOOLBAR", "SEARCH" ) ) );
				if ( PageContext.IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.admin_admin ), GetText( "TOOLBAR", "ADMIN" ) ) );
				if ( PageContext.IsModerator || PageContext.IsForumModerator )
					header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.moderate_index ), GetText( "TOOLBAR", "MODERATE" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.active ), GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				if ( !PageContext.IsGuest )
					header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.cp_profile ), GetText( "TOOLBAR", "MYPROFILE" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", yaf_BuildLink.GetLink( ForumPages.members ), GetText( "TOOLBAR", "MEMBERS" ) ) );
				if ( CanLogin )
					header.AppendFormat( String.Format( " | <a href=\"{0}\" onclick=\"return confirm('Are you sure you want to logout?');\">{1}</a>", yaf_BuildLink.GetLink( ForumPages.logout ), GetText( "TOOLBAR", "LOGOUT" ) ) );
			}
			else
			{
				header.AppendFormat( String.Format( "<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>", GetText( "TOOLBAR", "WELCOME_GUEST" ) ) );

				header.AppendFormat( "<td style=\"padding:5px\" align=right valign=middle class=post>" );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.search ), GetText( "TOOLBAR", "SEARCH" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", yaf_BuildLink.GetLink( ForumPages.active ), GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", yaf_BuildLink.GetLink( ForumPages.members ), GetText( "TOOLBAR", "MEMBERS" ) ) );
				if ( CanLogin )
				{
					header.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", yaf_BuildLink.GetLink( ForumPages.login, "ReturnUrl={0}", Server.UrlEncode( General.GetSafeRawUrl() ) ), GetText( "TOOLBAR", "LOGIN" ) ) );
					if ( !PageContext.BoardSettings.DisableRegistrations )
						header.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", yaf_BuildLink.GetLink( ForumPages.rules ), GetText( "TOOLBAR", "REGISTER" ) ) );
				}
			}
			header.AppendFormat( "</td></tr></table>" );
			header.AppendFormat( "<br />" );

			/*
			if ( ForumControl.Header != null )
				ForumControl.Header.Info = header.ToString();
			else
				m_headerInfo = header.ToString();
			*/

			// END HEADER
		}

		/// <summary>
		/// Writes the document
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			if ( m_bShowToolBar )
			{
				writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}forum.css"" />", yaf_ForumInfo.ForumRoot );
				writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}"" />", yaf_BuildLink.ThemeFile( "theme.css" ));
				string script = "";
				if ( LoadMessage.Length > 0 )
					script = String.Format( "<script language='javascript'>\nonload=function(){1}\nalert(\"{0}\")\n{2}\n</script>\n", LoadMessage, '{', '}' );

#if TODO
				if(m_strRefreshURL!=null) 
					script = script.Insert(0,String.Format("<meta http-equiv=\"Refresh\" content=\"10;{0}\">\n",m_strRefreshURL));
#else
				if ( m_strRefreshURL != null && m_nRefreshTime >= 0 )
					script = script.Insert( 0, String.Format( "<meta http-equiv=\"Refresh\" content=\"{1};url={0}\">\n", m_strRefreshURL, m_nRefreshTime ) );
#endif

				/* BEGIN HEADER
				if ( m_headerInfo != null && ForumControl.LockedForum == 0 )
					writer.Write( m_headerInfo );
				// END HEADER				
				 */

				RenderBody( writer );

				// BEGIN FOOTER
				StringBuilder footer = new StringBuilder();
				footer.AppendFormat( "<p style=\"text-align:center;font-size:7pt\">" );

				if ( PageContext.BoardSettings.ShowRSSLink )
				{
					footer.AppendFormat( "{2} : <a href=\"{0}\"><img valign=\"absmiddle\" src=\"{1}images/rss.gif\" alt=\"RSS\" /></a><br /><br />", yaf_BuildLink.GetLink( ForumPages.rsstopic, "pg=forum" ), yaf_ForumInfo.ForumRoot, GetText( "DEFAULT", "MAIN_FORUM_RSS" ) );
					// footer.AppendFormat("Main Forum Rss Feed : <a href=\"{0}rsstopic.aspx?pg=forum\"><img valign=\"absmiddle\" src=\"{1}images/rss.gif\" alt=\"RSS\" /></a><br /><br />", Data.ForumRoot, Data.ForumRoot);
				}

				// get the theme credit info from the theme file
				// it's not really an error if it doesn't exist
				string themeCredit = GetThemeContents( "THEME", "CREDIT", null );

				if ( themeCredit != null && themeCredit.Length > 0 )
				{
					themeCredit = @"<span id=""themecredit"" style=""color:#999999"">" + themeCredit + @"</span><br />";
				}

				stopWatch.Stop();
				double duration = ( double ) stopWatch.ElapsedMilliseconds / 1000.0;

				if ( YAF.Classes.Config.IsDotNetNuke )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under DotNetNuke.", yaf_ForumInfo.AppVersionName );
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
				}
				else if ( YAF.Classes.Config.IsRainbow )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under Rainbow.", yaf_ForumInfo.AppVersionName );
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
				}
				else if ( PageContext.Settings.LockedForum == 0 )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( GetText( "COMMON", "POWERED_BY" ),
						String.Format( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a>" ),
						String.Format( "{0} (NET v{2}.{3}) - {1}", yaf_ForumInfo.AppVersionName, yaf_DateTime.FormatDateShort( yaf_ForumInfo.AppVersionDate ), System.Environment.Version.Major.ToString(), System.Environment.Version.Minor.ToString() )
						);
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
					footer.AppendFormat( "<br/>" );
					footer.AppendFormat( PageContext.AdminLoadString ); // Append a error message for an admin to see (but not nag)

					if ( PageContext.BoardSettings.ShowPageGenerationTime )
						footer.AppendFormat( GetText( "COMMON", "GENERATED" ), duration );
				}

#if DEBUG
				footer.AppendFormat( "<br/>{0} queries ({1:N3} seconds, {2:N2}%).<br/>{3}", QueryCounter.Count, QueryCounter.Duration, (100 * QueryCounter.Duration) / duration, QueryCounter.Commands );
#endif
				footer.AppendFormat( "</p>" );
				if ( PageContext.Settings.LockedForum == 0 )
				{
					/*
					if ( ForumControl.Footer != null )
						ForumControl.Footer.Info = footer.ToString();
					else
						writer.Write( footer.ToString() );
					 */
				}
				// END FOOTER

				writer.WriteLine( script );
			}
			else
			{
				writer.WriteLine( "<html>" );
				writer.WriteLine( "<head>" );
				writer.WriteLine( String.Format( @"<link rel=""stylesheet"" type=""text/css"" href=""{0}forum.css"">", yaf_ForumInfo.ForumRoot ) );
				writer.WriteLine( String.Format( @"<link rel=""stylesheet"" type=""text/css"" href=""{0}"">", yaf_BuildLink.ThemeFile("theme.css") ) );
				writer.WriteLine( String.Format( @"<title>{0}</title>", PageContext.BoardSettings.Name ) );
				if ( m_strRefreshURL != null )
					writer.WriteLine( String.Format( "<meta http-equiv=\"Refresh\" content=\"10;{0}\">", m_strRefreshURL ) );
				writer.WriteLine( "</head>" );
				writer.WriteLine( "<body onload='yaf_onload()'>" );

				RenderBody( writer );

				writer.WriteLine( @"<script type=""text/javascript"">" );
				writer.WriteLine( "function yaf_onload() {" );
				if ( LoadMessage.Length > 0 )
					writer.WriteLine( String.Format( "	alert(\"{0}\");", LoadMessage ) );
				writer.WriteLine( "}" );
				writer.WriteLine( "yaf_onload();" );
				writer.WriteLine( "</script>" );

				writer.WriteLine( "</body>" );
				writer.WriteLine( "</html>" );
			}
		}

		/// <summary>
		/// Renders the body
		/// </summary>
		/// <param name="writer"></param>
		protected virtual void RenderBody( System.Web.UI.HtmlTextWriter writer )
		{
			RenderBase( writer );
		}

		/// <summary>
		/// Calls the base class to render components
		/// </summary>
		/// <param name="writer"></param>
		protected void RenderBase( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
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
				m_bNoDataBase = value;
			}
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
				m_strRefreshURL = value;
			}
			get
			{
				return m_strRefreshURL;
			}
		}
		public int RefreshTime
		{
			set { m_nRefreshTime = value; }
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
				m_bShowToolBar = value;
			}
		}
		#endregion

		public bool CheckSuspended
		{
			set
			{
				m_checkSuspended = value;
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

#if false
        private object User
        {
            get
            {
                return null;
            }
        }
#else
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
#endif

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
		/// <param name="dontLogMissing">True if you don't want a log created if it doesn't exist</param>
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
			PanelSessionState.CollapsiblePanelState stateValue = Mession.PanelState [panelID];
			if ( stateValue == PanelSessionState.CollapsiblePanelState.None )
			{
				stateValue = defaultState;
				Mession.PanelState [panelID] = defaultState;
			}

			return GetThemeContents( "ICONS", ( stateValue == PanelSessionState.CollapsiblePanelState.Expanded ? "PANEL_COLLAPSE" : "PANEL_EXPAND" ) );
		}
		#endregion		

		#region Localization Helper Functions

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
		public YAF.Classes.Utils.yaf_Context PageContext
		{
			get
			{
				return YAF.Classes.Utils.yaf_Context.Current;
			}
		}

		public string ForumURL
		{
			get
			{
				return string.Format( "{0}{1}", yaf_ForumInfo.ServerURL, yaf_BuildLink.GetLink( ForumPages.forum ) );
			}
		}

		#endregion

		protected string HtmlEncode( object data )
		{
			return Server.HtmlEncode( data.ToString() );
		}
	}
}
