/* Yet Another Forum.net
 * Copyright (C) 2003 Bjørnar Henden
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
using yaf.classes;

namespace yaf.pages
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
		private string m_adminMessage = string.Empty;

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
			TransPage = transPage;
			stopWatch.Start();

			this.Load += new System.EventHandler( this.ForumPage_Load );
			this.Error += new System.EventHandler( this.ForumPage_Error );
			this.PreRender += new EventHandler( ForumPage_PreRender );
		}

		private void ForumPage_Error( object sender, System.EventArgs e )
		{
			// This doesn't seem to work...
			Exception x = Server.GetLastError();
			DB.eventlog_create( PageUserID, this, x );
			if ( !Data.IsLocal )
				Utils.LogToMail( Server.GetLastError() );
		}

		static public object ValidInt( object o )
		{
			try
			{
				if ( o == null )
					return null;

				return int.Parse( o.ToString() );
			}
			catch ( Exception )
			{
				return null;
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

			try
			{
				string key = string.Format( "BannedIP.{0}", PageBoardID );
				DataTable banip = ( DataTable ) HttpContext.Current.Cache [key];
				if ( banip == null )
				{
					banip = DB.bannedip_list( PageBoardID, null );
					HttpContext.Current.Cache [key] = banip;
				}
				foreach ( DataRow row in banip.Rows )
					if ( Utils.IsBanned( ( string ) row ["Mask"], HttpContext.Current.Request.ServerVariables ["REMOTE_ADDR"] ) )
						HttpContext.Current.Response.End();
			}
			catch ( Exception )
			{
				// If the above fails chances are that this is a new install
				Response.Redirect( Data.ForumRoot + "install/" );
			}

			// Find user name
			MembershipUser user = Membership.GetUser();
			if ( user != null && Session ["UserUpdated"] == null )
			{
				Security.UpdateForumUser( PageBoardID, user );
				Session ["UserUpdated"] = true;
			}

			string browser = String.Format( "{0} {1}", HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version );
			string platform = HttpContext.Current.Request.Browser.Platform;

			if ( HttpContext.Current.Request.UserAgent != null )
			{
				if ( HttpContext.Current.Request.UserAgent.IndexOf( "Windows NT 5.2" ) >= 0 )
					platform = "Win2003";
			}

			object categoryID = ValidInt( HttpContext.Current.Request.QueryString ["c"] );
			object forumID = ValidInt( HttpContext.Current.Request.QueryString ["f"] );
			object topicID = ValidInt( HttpContext.Current.Request.QueryString ["t"] );
			object messageID = ValidInt( HttpContext.Current.Request.QueryString ["m"] );

			if ( ForumControl.CategoryID != null )
				categoryID = ForumControl.CategoryID;

			object userKey = DBNull.Value;
			if ( user != null )
				userKey = user.ProviderUserKey;

			do
			{
				m_pageinfo = DB.pageload(
						HttpContext.Current.Session.SessionID,
						PageBoardID,
						userKey,
						HttpContext.Current.Request.UserHostAddress,
						HttpContext.Current.Request.FilePath,
						browser,
						platform,
						categoryID,
						forumID,
						topicID,
						messageID );

				if ( user != null && m_pageinfo == null )
				{
					if ( !Security.CreateForumUser( user, this ) )
						throw new ApplicationException( "Failed to use new user." );
				}
			} while ( m_pageinfo == null && user != null );


			if ( m_pageinfo == null )
			{
				if ( user != null )
					throw new ApplicationException( string.Format( "User '{0}' isn't registered.", user.UserName ) );
				else
					throw new ApplicationException( "Failed to find guest user." );
			}

			//if (user!=null && m_pageinfo["ProviderUserKey"] == DBNull.Value)
			//    throw new ApplicationException("User not migrated to ASP.NET 2.0");

			if ( m_checkSuspended && IsSuspended )
			{
				if ( SuspendedTo < DateTime.Now )
				{
					DB.user_suspend( PageUserID, null );
					HttpContext.Current.Response.Redirect( Utils.GetSafeRawUrl() );
				}
				Forum.Redirect( Pages.info, "i=2" );
			}

			// This happens when user logs in
			if ( Mession.LastVisit == DateTime.MinValue )
			{
				if ( ( int ) m_pageinfo ["Incoming"] > 0 )
					AddLoadMessage( String.Format( GetText( "UNREAD_MSG" ), m_pageinfo ["Incoming"] ) );
			}

			if ( !IsGuest && m_pageinfo ["PreviousVisit"] != DBNull.Value && !Mession.HasLastVisit )
			{
				//if(Mession.LastVisit == DateTime.MinValue || (DateTime)m_pageinfo["PreviousVisit"]<Mession.LastVisit)
				Mession.LastVisit = ( DateTime ) m_pageinfo ["PreviousVisit"];
				Mession.HasLastVisit = true;
			}
			else if ( Mession.LastVisit == DateTime.MinValue )
			{
				Mession.LastVisit = DateTime.Now;
			}

			// Check if pending mails, and send 10 of them if possible
			if ( ( int ) m_pageinfo ["MailsPending"] > 0 )
			{
				try
				{
					using ( DataTable dt = DB.mail_list() )
					{
						for ( int i = 0; i < dt.Rows.Count; i++ )
						{
							// Build a MailMessage
							if ( dt.Rows [i] ["ToUser"].ToString().Trim() != String.Empty )
							{
								Utils.SendMail( this, BoardSettings.ForumEmail, ( string ) dt.Rows [i] ["ToUser"], ( string ) dt.Rows [i] ["Subject"], ( string ) dt.Rows [i] ["Body"] );
							}
							DB.mail_delete( dt.Rows [i] ["MailID"] );
						}
						if ( IsAdmin ) this.AddAdminMessage( "Sent Mail", String.Format( "Sent {0} mails.", dt.Rows.Count ) );
					}
				}
				catch ( Exception x )
				{
					DB.eventlog_create( PageUserID, this, x );
					if ( IsAdmin )
					{
						this.AddAdminMessage( "Error sending emails to users", x.ToString() );
					}
				}
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
				DB.eventlog_create( PageUserID, this, ex );
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
		/// Gets the last time the forum was read.
		/// </summary>
		/// <param name="forumID">This is the ID of the forum you wish to get the last read date from.</param>
		/// <returns>A DateTime object of when the forum was last read.</returns>
		public DateTime GetForumRead( int forumID )
		{
			System.Collections.Hashtable t = Mession.ForumRead;
			if ( t == null || !t.ContainsKey( forumID ) )
				return ( DateTime ) Mession.LastVisit;
			else
				return ( DateTime ) t [forumID];
		}

		/// <summary>
		/// Sets the time that the forum was read.
		/// </summary>
		/// <param name="forumID">The forum ID that was read.</param>
		/// <param name="date">The DateTime you wish to set the read to.</param>
		public void SetForumRead( int forumID, DateTime date )
		{
			System.Collections.Hashtable t = Mession.ForumRead;
			if ( t == null )
			{
				t = new System.Collections.Hashtable();
			}
			t [forumID] = date;
			Mession.ForumRead = t;
		}

		/// <summary>
		/// Returns the last time that the topicID was read.
		/// </summary>
		/// <param name="topicID">The topicID you wish to find the DateTime object for.</param>
		/// <returns>The DateTime object from the topicID.</returns>
		public DateTime GetTopicRead( int topicID )
		{
			System.Collections.Hashtable t = Mession.TopicRead;
			if ( t == null || !t.ContainsKey( topicID ) )
				return ( DateTime ) Mession.LastVisit;
			else
				return ( DateTime ) t [topicID];
		}

		/// <summary>
		/// Sets the time that the topicID was read.
		/// </summary>
		/// <param name="topicID">The topic ID that was read.</param>
		/// <param name="date">The DateTime you wish to set the read to.</param>
		public void SetTopicRead( int topicID, DateTime date )
		{
			System.Collections.Hashtable t = Mession.TopicRead;
			if ( t == null )
			{
				t = new System.Collections.Hashtable();
			}
			t [topicID] = date;
			Mession.TopicRead = t;
		}
		#endregion
		#region Render Functions
		private Forum m_forumControl = null;
		public yaf.Forum ForumControl
		{
			get
			{
				if ( m_forumControl != null )
					return m_forumControl;

				System.Web.UI.Control ctl = Parent;
				while ( ctl.GetType() != typeof( yaf.Forum ) )
					ctl = ctl.Parent;

				m_forumControl = ( yaf.Forum ) ctl;
				return m_forumControl;
			}
			set
			{
				m_forumControl = value;
			}
		}

		private string m_headerInfo = null;

		private void ForumPage_PreRender( object sender, EventArgs e )
		{
			System.Web.UI.HtmlControls.HtmlImage graphctl;
			if ( BoardSettings.AllowThemedLogo & !Config.IsDotNetNuke & !Config.IsPortal & !Config.IsRainbow )
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
				if ( this.PageTopicID != 0 )
					title.AppendFormat( "{0} - ", Utils.BadWordReplace( this.PageTopicName ) ); // Tack on the topic we're viewing
				if ( this.PageForumName != string.Empty )
					title.AppendFormat( "{0} - ", Server.HtmlEncode( this.PageForumName ) ); // Tack on the forum we're viewing
				title.Append( Server.HtmlEncode( BoardSettings.Name ) ); // and lastly, tack on the board's name
				ctl.Text = title.ToString();
			}

			// BEGIN HEADER
			StringBuilder header = new StringBuilder();

			// get the theme header -- usually used for javascript
			string themeHeader = GetThemeContents( "THEME", "HEADER", null, true );

			if ( themeHeader != null && themeHeader.Length > 0 )
			{
				header.Append(themeHeader);
			} 

			header.AppendFormat( "<table width=100% cellspacing=0 class=content cellpadding=0><tr>" );

			MembershipUser user = Membership.GetUser();

			if ( user != null )
			{
				header.AppendFormat( "<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>", String.Format( GetText( "TOOLBAR", "LOGGED_IN_AS" ) + " ", Server.HtmlEncode( PageUserName ) ) );
				header.AppendFormat( "<td style=\"padding:5px\" align=right valign=middle class=post>" );
				if ( !IsGuest )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.cp_inbox ), GetText( "CP_INBOX", "TITLE" ) ) );

				/* TODO: help is currently useless...
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.help_index ), GetText( "TOOLBAR", "HELP" ) ) );
				*/

				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.search ), GetText( "TOOLBAR", "SEARCH" ) ) );
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.admin_admin ), GetText( "TOOLBAR", "ADMIN" ) ) );
				if ( IsModerator || IsForumModerator )
					header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.moderate_index ), GetText( "TOOLBAR", "MODERATE" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.active ), GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				if ( !IsGuest )
					header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.cp_profile ), GetText( "TOOLBAR", "MYPROFILE" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", Forum.GetLink( Pages.members ), GetText( "TOOLBAR", "MEMBERS" ) ) );
				if ( CanLogin )
					header.AppendFormat( String.Format( " | <a href=\"{0}\" onclick=\"return confirm('Are you sure you want to logout?');\">{1}</a>", Forum.GetLink( Pages.logout ), GetText( "TOOLBAR", "LOGOUT" ) ) );
			}
			else
			{
				header.AppendFormat( String.Format( "<td style=\"padding:5px\" class=post align=left><b>{0}</b></td>", GetText( "TOOLBAR", "WELCOME_GUEST" ) ) );

				header.AppendFormat( "<td style=\"padding:5px\" align=right valign=middle class=post>" );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.search ), GetText( "TOOLBAR", "SEARCH" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", Forum.GetLink( Pages.active ), GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				header.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", Forum.GetLink( Pages.members ), GetText( "TOOLBAR", "MEMBERS" ) ) );
				if ( CanLogin )
				{
					header.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", Forum.GetLink( Pages.login, "ReturnUrl={0}", Server.UrlEncode( Utils.GetSafeRawUrl() ) ), GetText( "TOOLBAR", "LOGIN" ) ) );
					if ( !BoardSettings.DisableRegistrations )
						header.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", Forum.GetLink( Pages.rules ), GetText( "TOOLBAR", "REGISTER" ) ) );
				}
			}
			header.AppendFormat( "</td></tr></table>" );
			header.AppendFormat( "<br />" );
			if ( ForumControl.Header != null )
				ForumControl.Header.Info = header.ToString();
			else
				m_headerInfo = header.ToString();
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
				writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}forum.css"" />", Data.ForumRoot );
				writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}"" />", ThemeFile( "theme.css" ) );
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

				// BEGIN HEADER
				if ( m_headerInfo != null && ForumControl.LockedForum == 0 )
					writer.Write( m_headerInfo );
				// END HEADER				

				RenderBody( writer );

				// BEGIN FOOTER
				StringBuilder footer = new StringBuilder();
				footer.AppendFormat( "<p style=\"text-align:center;font-size:7pt\">" );

				if ( BoardSettings.ShowRSSLink )
				{
					footer.AppendFormat( "{2} : <a href=\"{0}\"><img valign=\"absmiddle\" src=\"{1}images/rss.gif\" alt=\"RSS\" /></a><br /><br />", Forum.GetLink( Pages.rsstopic, "pg=forum" ), Data.ForumRoot, GetText( "DEFAULT", "MAIN_FORUM_RSS" ) );
					// footer.AppendFormat("Main Forum Rss Feed : <a href=\"{0}rsstopic.aspx?pg=forum\"><img valign=\"absmiddle\" src=\"{1}images/rss.gif\" alt=\"RSS\" /></a><br /><br />", Data.ForumRoot, Data.ForumRoot);
				}

				// get the theme credit info from the theme file
				// it's not really an error if it doesn't exist
				string themeCredit = GetThemeContents( "THEME", "CREDIT", null, true );

				if ( themeCredit != null && themeCredit.Length > 0 )
				{
					themeCredit = @"<span id=""themecredit"" style=""color:#999999"">" + themeCredit + @"</span><br />";
				}

				stopWatch.Stop();
				double duration = ( double ) stopWatch.ElapsedMilliseconds / 1000.0;

				if ( Config.IsDotNetNuke )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under DotNetNuke.", Data.AppVersionName );
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
				}
				else if ( Config.IsRainbow )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a> version {0} running under Rainbow.", Data.AppVersionName );
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
				}
				else if ( ForumControl.LockedForum == 0 )
				{
					if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
					footer.AppendFormat( GetText( "COMMON", "POWERED_BY" ),
						String.Format( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.net</a>" ),
						String.Format( "{0} (NET v{2}.{3}) - {1}", Data.AppVersionName, FormatDateShort( Data.AppVersionDate ), System.Environment.Version.Major.ToString(), System.Environment.Version.Minor.ToString() )
						);
					footer.AppendFormat( "<br />Copyright &copy; 2003-2006 Yet Another Forum.net. All rights reserved." );
					footer.AppendFormat( "<br/>" );
					footer.AppendFormat( this.m_adminMessage ); // Append a error message for an admin to see (but not nag)

					if ( BoardSettings.ShowPageGenerationTime )
						footer.AppendFormat( GetText( "COMMON", "GENERATED" ), duration );
				}

#if DEBUG
				footer.AppendFormat( "<br/>{0} queries ({1:N3} seconds, {2:N2}%).<br/>{3}", QueryCounter.Count, QueryCounter.Duration, (100 * QueryCounter.Duration) / duration, QueryCounter.Commands );
#endif
				footer.AppendFormat( "</p>" );
				if ( ForumControl.LockedForum == 0 )
				{
					if ( ForumControl.Footer != null )
						ForumControl.Footer.Info = footer.ToString();
					else
						writer.Write( footer.ToString() );
				}
				// END FOOTER

				writer.WriteLine( script );
			}
			else
			{
				writer.WriteLine( "<html>" );
				writer.WriteLine( "<head>" );
				writer.WriteLine( String.Format( @"<link rel=""stylesheet"" type=""text/css"" href=""{0}forum.css"">", Data.ForumRoot ) );
				writer.WriteLine( String.Format( @"<link rel=""stylesheet"" type=""text/css"" href=""{0}"">", ThemeFile( "theme.css" ) ) );
				writer.WriteLine( String.Format( @"<title>{0}</title>", BoardSettings.Name ) );
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
		/// Find the path of a smiley icon
		/// </summary>
		/// <param name="icon">The file name of the icon you want</param>
		/// <returns>The path to the image file</returns>
		public string Smiley( string icon )
		{
			return String.Format( "{0}images/emoticons/{1}", Data.ForumRoot, icon );
		}

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
		private DataRow m_pageinfo = null;
		private MembershipUser mUser = null;
		private string m_strLoadMessage = "";

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
				if ( mUser == null )
					mUser = Membership.GetUser();
				return mUser;
			}
		}
#endif

		public string LoadMessage
		{
			get
			{
				return m_strLoadMessage;
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
			m_strLoadMessage += msg + "\\n\\n";
		}

		/// <summary>
		/// Instead of showing error messages in a pop-up javascript window every time
		/// the page loads (in some cases) provide a error message towards the bottom 
		/// of the page.
		/// </summary>
		/// <param name="msg"></param>
		public void AddAdminMessage( string errorType, string errorMessage )
		{
			this.m_adminMessage = string.Format( "<div style=\"margin: 2%; padding: 7px; border: 3px Solid Red; background-color: #ccc;\"><h1>{0}</h1>{1}</div>", errorType, errorMessage );
		}

		#region Forum Access
		/// <summary>
		/// True if current user has post access in the current forum
		/// </summary>
		public bool ForumPostAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "PostAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["PostAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user has reply access in the current forum
		/// </summary>
		public bool ForumReplyAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "ReplyAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["ReplyAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user has read access in the current forum
		/// </summary>
		public bool ForumReadAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "ReadAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["ReadAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user has access to create priority topics in the current forum
		/// </summary>
		public bool ForumPriorityAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "PriorityAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["PriorityAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user has access to create polls in the current forum.
		/// </summary>
		public bool ForumPollAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "PollAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["PollAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user has access to vote on polls in the current forum
		/// </summary>
		public bool ForumVoteAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "VoteAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["VoteAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user is a moderator of the current forum
		/// </summary>
		public bool ForumModeratorAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "ModeratorAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["ModeratorAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user can delete own messages in the current forum
		/// </summary>
		public bool ForumDeleteAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "DeleteAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["DeleteAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user can edit own messages in the current forum
		/// </summary>
		public bool ForumEditAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "EditAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["EditAccess"].ToString() ) > 0;
			}
		}
		/// <summary>
		/// True if the current user can upload attachments
		/// </summary>
		public bool ForumUploadAccess
		{
			get
			{
				if ( m_pageinfo.IsNull( "UploadAccess" ) )
					return false;
				else
					return long.Parse( m_pageinfo ["UploadAccess"].ToString() ) > 0;
			}
		}

		public int PageBoardID
		{
			get
			{
				try
				{
					return ForumControl.BoardID;
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
				if ( m_pageinfo != null )
					return ( int ) m_pageinfo ["UserID"];
				else
					return 0;
			}
		}
		public string PageUserName
		{
			get
			{
				if ( m_pageinfo != null )
					return ( string ) m_pageinfo ["UserName"];
				else
					return "";
			}
		}
		/// <summary>
		/// ForumID for the current page, or 0 if not in any forum
		/// </summary>
		public int PageForumID
		{
			get
			{
				int nLockedForum = ForumControl.LockedForum;
				if ( nLockedForum != 0 )
					return nLockedForum;

				if ( m_pageinfo != null && !m_pageinfo.IsNull( "ForumID" ) )
					return ( int ) m_pageinfo ["ForumID"];
				else
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
				if ( m_pageinfo != null && !m_pageinfo.IsNull( "ForumName" ) )
					return ( string ) m_pageinfo ["ForumName"];
				else
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
				if ( ForumControl.CategoryID != null )
					return int.Parse( ForumControl.CategoryID.ToString() );
				else if ( m_pageinfo != null && !m_pageinfo.IsNull( "CategoryID" ) )
					return ( int ) m_pageinfo ["CategoryID"];
				else
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
				if ( m_pageinfo != null && !m_pageinfo.IsNull( "CategoryName" ) )
					return ( string ) m_pageinfo ["CategoryName"];
				else
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
				if ( m_pageinfo != null && !m_pageinfo.IsNull( "TopicID" ) )
					return ( int ) m_pageinfo ["TopicID"];
				else
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
				if ( m_pageinfo != null && !m_pageinfo.IsNull( "TopicName" ) )
					return ( string ) m_pageinfo ["TopicName"];
				else
					return "";
			}
		}
		public bool IsHostAdmin
		{
			get
			{
				if ( m_pageinfo != null )
					return ( ( int ) m_pageinfo ["UserFlags"] & ( int ) UserFlags.IsHostAdmin ) == ( int ) UserFlags.IsHostAdmin;
				else
					return false;
			}
		}

		public bool IsUserHostAdmin( object UserID )
		{
			if ( Convert.ToInt32( UserID ) == 2 )
				return true;
			else
				return false;
		}

		public BoardSettings BoardSettings
		{
			get
			{
				string key = string.Format( "yaf_BoardSettings.{0}", PageBoardID );
				if ( HttpContext.Current.Application [key] == null )
					HttpContext.Current.Application [key] = new BoardSettings( PageBoardID );

				return ( BoardSettings ) HttpContext.Current.Application [key];
			}
			set
			{
				string key = string.Format( "yaf_BoardSettings.{0}", PageBoardID );
				HttpContext.Current.Application.Remove( key );
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

				if ( m_pageinfo != null && m_pageinfo ["IsAdmin"] != DBNull.Value )
					return long.Parse( m_pageinfo ["IsAdmin"].ToString() ) != 0;
				else
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
				if ( m_pageinfo != null && m_pageinfo ["IsGuest"] != DBNull.Value )
					return long.Parse( m_pageinfo ["IsGuest"].ToString() ) != 0;
				else
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
				if ( m_pageinfo != null && m_pageinfo ["IsForumModerator"] != DBNull.Value )
					return long.Parse( m_pageinfo ["IsForumModerator"].ToString() ) != 0;
				else
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
				if ( m_pageinfo != null && m_pageinfo ["IsModerator"] != DBNull.Value )
					return long.Parse( m_pageinfo ["IsModerator"].ToString() ) != 0;
				else
					return false;
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
						int.Parse(Config.ConfigSection[string.Format("isprivate{0}", PageBoardID)])!=0;
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
		#region Date and time functions
		/// <summary>
		/// Returns the user timezone offset from GMT
		/// </summary>
		public TimeSpan TimeZoneOffsetUser
		{
			get
			{
				if ( m_pageinfo != null )
				{
					int min = ( int ) m_pageinfo ["TimeZoneUser"];
					return new TimeSpan( min / 60, min % 60, 0 );
				}
				else
					return new TimeSpan( 0 );
			}
		}
		/// <summary>
		/// Returns the time zone offset for the current user compared to the forum time zone.
		/// </summary>
		public TimeSpan TimeOffset
		{
			get
			{
				return TimeZoneOffsetUser - BoardSettings.TimeZone;
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003 22:32:34
		/// </summary>
		/// <param name="o">The date to be formatted</param>
		/// <returns>Formatted string of the formatted DateTime Object.</returns>
		public string FormatDateTime( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			string strDateFormat;

			strDateFormat = String.Format( "{0:F}", dt );

			try
			{
				if ( BoardSettings.DateFormatFromLanguage )
					strDateFormat = dt.ToString( GetText( "FORMAT_DATE_TIME_LONG" ) );
			}
			catch ( Exception )
			{

			}

			return strDateFormat;
		}

		/// <summary>
		/// Formats a datatime value into 07.03.2003 00:00:00 except if 
		/// the date is yesterday or today -- in which case it says that.
		/// </summary>
		/// <param name="o">The datetime to be formatted</param>
		/// <returns>Formatted string of DateTime object</returns>
		public string FormatDateTimeTopic( object o )
		{
			string strDateFormat;
			DateTime dt = Convert.ToDateTime( o ) + TimeOffset;
			DateTime nt = DateTime.Now + TimeOffset;

			try
			{
				if ( dt.Date == nt.Date )
				{
					// today
					strDateFormat = String.Format( GetText( "TodayAt" ), dt );
				}
				else if ( dt.Date == nt.AddDays( -1 ).Date )
				{
					// yesterday
					strDateFormat = String.Format( GetText( "YesterdayAt" ), dt );
				}
				else if ( BoardSettings.DateFormatFromLanguage )
				{
					strDateFormat = dt.ToString( GetText( "FORMAT_DATE_TIME_SHORT" ) );
				}
				else
				{
					strDateFormat = String.Format( "{0:f}", dt );
				}
				return strDateFormat;
			}
			catch ( Exception )
			{
				return dt.ToString( "f" );
			}
		}
		/// <summary>
		/// This formats a DateTime into a short string
		/// </summary>
		/// <param name="o">The DateTime like object you wish to make a formatted string.</param>
		/// <returns>The formatted string created from the DateTime object.</returns>
		public string FormatDateTimeShort( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			try
			{
				if ( BoardSettings.DateFormatFromLanguage )
					return dt.ToString( GetText( "FORMAT_DATE_TIME_SHORT" ) );
				else
					return String.Format( "{0:f}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "f" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 7. february 2003
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatDateLong( DateTime dt )
		{
			dt += TimeOffset;
			try
			{
				if ( BoardSettings.DateFormatFromLanguage )
					return dt.ToString( GetText( "FORMAT_DATE_LONG" ) );
				else
					return String.Format( "{0:D}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "D" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 07.03.2003
		/// </summary>
		/// <param name="o">This formats the date.</param>
		/// <returns>Short formatted date.</returns>
		public string FormatDateShort( object o )
		{
			DateTime dt = ( DateTime ) o + TimeOffset;
			try
			{
				if ( BoardSettings.DateFormatFromLanguage )
					return dt.ToString( GetText( "FORMAT_DATE_SHORT" ) );
				else
					return String.Format( "{0:d}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "d" );
			}
		}
		/// <summary>
		/// Formats a datetime value into 22:32:34
		/// </summary>
		/// <param name="dt">The date to be formatted</param>
		/// <returns></returns>
		public string FormatTime( DateTime dt )
		{
			dt += TimeOffset;
			try
			{
				if ( BoardSettings.DateFormatFromLanguage )
					return dt.ToString( GetText( "FORMAT_TIME" ) );
				else
					return String.Format( "{0:T}", dt );
			}
			catch ( Exception )
			{
				return dt.ToString( "T" );
			}
		}
		#endregion
		#region Theme Functions
		// XML THEME FILE (TEST)
		private XmlDocument LoadTheme( string themefile )
		{
			if ( themefile == null )
			{
				if ( m_pageinfo != null && !m_pageinfo.IsNull( "ThemeFile" ) && BoardSettings.AllowUserTheme )
				{
					// use user-selected themem
					themefile = ( string ) m_pageinfo ["ThemeFile"];
				}
				else if ( m_pageinfo != null && !m_pageinfo.IsNull( "ForumTheme" ) )
				{
					themefile = ( string ) m_pageinfo ["ForumTheme"];
				}
				else
				{
					themefile = BoardSettings.Theme;
				}

				if ( themefile == null )
				{
					themefile = "standard.xml";
				}
			}

			XmlDocument doc = null;
#if !DEBUG
			doc = (XmlDocument)System.Web.HttpContext.Current.Cache[themefile];
#endif
			if ( doc == null )
			{
				doc = new XmlDocument();
				doc.Load( System.Web.HttpContext.Current.Server.MapPath( String.Format( "{0}themes/{1}", Data.ForumRoot, themefile ) ) );
#if !DEBUG
				System.Web.HttpContext.Current.Cache[themefile] = doc;
#endif
			}
			return doc;
		}

		/// <summary>
		/// Get a value from the currently configured forum theme.
		/// </summary>
		/// <param name="page">Page to look under</param>
		/// <param name="tag">Theme item</param>
		/// <returns>Converted Theme information</returns>
		public string GetThemeContents( string page, string tag )
		{
			return GetThemeContents( page, tag, String.Format( "[{0}.{1}]", page.ToUpper(), tag.ToUpper() ), false );
		}

		/// <summary>
		/// Get a value from the currently configured forum theme.
		/// </summary>
		/// <param name="page">Page to look under</param>
		/// <param name="tag">Theme item</param>
		/// <param name="defaultValue">Value to return if the theme item doesn't exist</param>
		/// <param name="dontLogMissing">True if you don't want a log created if it doesn't exist</param>
		/// <returns>Converted Theme information or Default Value if it doesn't exist</returns>
		public string GetThemeContents( string page, string tag, string defaultValue, bool dontLogMissing )
		{
			XmlDocument doc = LoadTheme( null );

			string themeDir = doc.DocumentElement.Attributes ["dir"].Value;
			string langCode = LoadTranslation().ToUpper();
			string select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}' and @language='{2}']", page.ToUpper(), tag.ToUpper(), langCode );
			XmlNode node = doc.SelectSingleNode( select );
			if ( node == null )
			{
				select = string.Format( "//page[@name='{0}']/Resource[@tag='{1}']", page.ToUpper(), tag.ToUpper() );
				node = doc.SelectSingleNode( select );
			}
			if ( node == null )
			{
				if ( !dontLogMissing ) DB.eventlog_create( PageUserID, page.ToLower() + ".ascx", String.Format( "Missing Theme Item: {0}.{1}", page.ToUpper(), tag.ToUpper() ), EventLogTypes.Error );
				return defaultValue;
			}

			string contents = node.InnerText;
			contents = contents.Replace( "~", String.Format( "{0}themes/{1}", Data.ForumRoot, themeDir ) );
			return contents;
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

		public string ThemeDir
		{
			get
			{
				XmlDocument doc = LoadTheme( null );
				return String.Format( "{0}themes/{1}/", Data.ForumRoot, doc.DocumentElement.Attributes ["dir"].Value );
			}
		}

		public string GetURLToResource( string resourceName )
		{
			return string.Format( "{1}resource.ashx?r={0}", resourceName, Data.ForumRoot );
		}

		#endregion
		#region Localizing
		private Localizer m_localizer = null;
		private Localizer m_defaultLocale = null;

		private string m_transPage = null;

		/// <summary>
		/// What section of the xml is used to translate this page
		/// </summary>
		public string TransPage
		{
			get
			{
				if ( m_transPage != null )
					return m_transPage;

				throw new ApplicationException( string.Format( "Missing TransPage property for {0}", GetType() ) );
			}
			set
			{
				m_transPage = value;
			}
		}

		public string GetText( string text )
		{
			return GetText( TransPage, text );
		}

		private string LoadTranslation()
		{
			if ( m_localizer != null )
				return m_localizer.LanguageCode;

			string filename = null;

			if ( m_pageinfo == null || m_pageinfo.IsNull( "LanguageFile" ) || !BoardSettings.AllowUserLanguage )
				filename = BoardSettings.Language;
			else
				filename = ( string ) m_pageinfo ["LanguageFile"];

			if ( filename == null )
				filename = "english.xml";

#if !DEBUG
			if(m_localizer==null && HttpContext.Current.Cache["Localizer." + filename]!=null)
				m_localizer = (Localizer)HttpContext.Current.Cache["Localizer." + filename];
#endif
			if ( m_localizer == null )
			{

				m_localizer = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/{1}", Data.ForumRoot, filename ) ) );
#if !DEBUG
				HttpContext.Current.Cache["Localizer." + filename] = m_localizer;
#endif
			}
			// If not using default language load that too
			if ( filename.ToLower() != "english.xml" )
			{
#if !DEBUG
				if(m_defaultLocale==null && HttpContext.Current.Cache["DefaultLocale"]!=null)
					m_defaultLocale = (Localizer)HttpContext.Current.Cache["DefaultLocale"];
#endif

				if ( m_defaultLocale == null )
				{
					m_defaultLocale = new Localizer( HttpContext.Current.Server.MapPath( String.Format( "{0}languages/english.xml", Data.ForumRoot ) ) );
#if !DEBUG
					HttpContext.Current.Cache["DefaultLocale"] = m_defaultLocale;
#endif
				}
			}
			return m_localizer.LanguageCode;
		}

		public string GetText( string page, string text )
		{
			LoadTranslation();
			string str = m_localizer.GetText( page, text );
			// If not default language, try to use that instead
			if ( str == null && m_defaultLocale != null )
			{
				str = m_defaultLocale.GetText( page, text );
				if ( str != null ) str = '[' + str + ']';
			}
			if ( str == null )
			{
#if !DEBUG
				string filename = null;

				if(m_pageinfo==null || m_pageinfo.IsNull("LanguageFile") || !BoardSettings.AllowUserLanguage)
					filename = BoardSettings.Language;
				else
					filename = (string)m_pageinfo["LanguageFile"];

				if(filename==null)
					filename = "english.xml";

				HttpContext.Current.Cache.Remove("Localizer." + filename);
#endif
				DB.eventlog_create( PageUserID, page.ToLower() + ".ascx", String.Format( "Missing Translation For {1}.{0}", text.ToUpper(), page.ToUpper() ), EventLogTypes.Error );
				return String.Format( "[{1}.{0}]", text.ToUpper(), page.ToUpper() ); ;
			}
			str = str.Replace( "[b]", "<b>" );
			str = str.Replace( "[/b]", "</b>" );
			return str;
		}
		#endregion
		public bool IsSuspended
		{
			get
			{
				if ( m_pageinfo == null )
					return false;
				else
					return !m_pageinfo.IsNull( "Suspended" );
			}
		}

		public DateTime SuspendedTo
		{
			get
			{
				if ( m_pageinfo == null || m_pageinfo.IsNull( "Suspended" ) )
					return DateTime.Now;
				else
					return DateTime.Parse( m_pageinfo ["Suspended"].ToString() );
			}
		}
		public int UnreadPrivate
		{
			get
			{
				return ( int ) m_pageinfo ["Incoming"];
			}
		}
		public string ForumURL
		{
			get
			{
				return string.Format( "{0}{1}", ServerURL, Forum.GetLink( Pages.forum ) );
			}
		}
		public string ServerURL
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

		public string ThemeFile( string filename )
		{
			return ThemeDir + filename;
		}
		#endregion

		protected string HtmlEncode( object data )
		{
			return Server.HtmlEncode( data.ToString() );
		}
	}
}
