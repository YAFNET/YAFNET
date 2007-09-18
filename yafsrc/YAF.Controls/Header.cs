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
using System.Text;
using System.Web;
using System.Web.Security;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Header.
	/// </summary>
	public class Header : BaseControl
	{
		private bool _simpleRender = false;
		private string _refreshURL = null;
		private int _refreshTime = 10;

		/// <summary>
		/// SimpleRender is used for for admin pages
		/// </summary>
		public bool SimpleRender
		{
			get
			{
				return _simpleRender;
			}
			set
			{
				_simpleRender = value;
			}
		}

		public string RefreshURL
		{
			get
			{
				return _refreshURL;
			}
			set
			{
				_refreshURL = value;
			}
		}

		public int RefreshTime
		{
			get
			{
				return _refreshTime;
			}
			set
			{
				_refreshTime = value;
			}
		}

		public void Reset()
		{
			SimpleRender = false;
			RefreshURL = null;
			RefreshTime = 10;
		}

		/// <summary>
		/// Renders the header.
		/// </summary>
		/// <param name="writer">The HtmlTextWriter that we are using.</param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			if ( !SimpleRender ) RenderRegular( ref writer );
			else RenderSimple( ref writer );
		}

		protected void WriteCSS( ref System.Web.UI.HtmlTextWriter writer )
		{
			writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}forum.css"" />", YafForumInfo.ForumRoot );
			writer.WriteLine( @"<link type=""text/css"" rel=""stylesheet"" href=""{0}"" />", YafBuildLink.ThemeFile( "theme.css" ) );
		}

		protected void WriteRefresh( ref System.Web.UI.HtmlTextWriter writer )
		{
			if ( _refreshURL != null && _refreshTime >= 0 )
				writer.WriteLine( String.Format( "<meta http-equiv=\"Refresh\" content=\"{1};url={0}\">\n", _refreshURL, _refreshTime ) );
		}

		protected void RenderSimple( ref System.Web.UI.HtmlTextWriter writer )
		{
			writer.WriteLine( @"<html><head>" );

			WriteCSS( ref writer );

			writer.WriteLine( String.Format( @"<title>{0}</title>", PageContext.BoardSettings.Name ) );

			WriteRefresh( ref writer );

			writer.WriteLine( @"</head><body>" );
		}

		protected void RenderRegular( ref System.Web.UI.HtmlTextWriter writer )
		{
			// BEGIN HEADER
			StringBuilder buildHeader = new StringBuilder();

			// get the theme header -- usually used for javascript
			string themeHeader = PageContext.Theme.GetItem( "THEME", "HEADER", "" );

			if ( themeHeader != null && themeHeader.Length > 0 )
			{
				buildHeader.Append( themeHeader );
			}

			buildHeader.AppendFormat( @"<table width=""100%"" cellspacing=""0"" class=""content"" cellpadding=""0""><tr>" );

			MembershipUser user = Membership.GetUser();

			if ( user != null )
			{
				buildHeader.AppendFormat( @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", String.Format( PageContext.Localization.GetText( "TOOLBAR", "LOGGED_IN_AS" ) + " ", HttpContext.Current.Server.HtmlEncode( PageContext.PageUserName ) ) );
				buildHeader.AppendFormat( @"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">" );

        if ( !PageContext.IsGuest && PageContext.BoardSettings.AllowPrivateMessages )
          buildHeader.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.cp_pm ), PageContext.Localization.GetText( "CP_PM", "INBOX" ) ) );

				/* TODO: help is currently useless...
				if ( IsAdmin )
					header.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.help_index ), GetText( "TOOLBAR", "HELP" ) ) );
				*/

				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.search ), PageContext.Localization.GetText( "TOOLBAR", "SEARCH" ) ) );
				if ( PageContext.IsAdmin )
					buildHeader.AppendFormat( String.Format( "	<a target='_top' href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.admin_admin ), PageContext.Localization.GetText( "TOOLBAR", "ADMIN" ) ) );
				if ( PageContext.IsModerator || PageContext.IsForumModerator )
					buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.moderate_index ), PageContext.Localization.GetText( "TOOLBAR", "MODERATE" ) ) );
				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.active ), PageContext.Localization.GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				if ( !PageContext.IsGuest )
					buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.cp_profile ), PageContext.Localization.GetText( "TOOLBAR", "MYPROFILE" ) ) );
				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.members ), PageContext.Localization.GetText( "TOOLBAR", "MEMBERS" ) ) );
				buildHeader.AppendFormat( String.Format( " | <a href=\"{0}\" onclick=\"return confirm('{2}');\">{1}</a>", YafBuildLink.GetLink( ForumPages.logout ), PageContext.Localization.GetText( "TOOLBAR", "LOGOUT" ), PageContext.Localization.GetText( "TOOLBAR", "LOGOUT_QUESTION" ) ) );
			}
			else
			{
				buildHeader.AppendFormat( String.Format( @"<td style=""padding:5px"" class=""post"" align=""left""><b>{0}</b></td>", PageContext.Localization.GetText( "TOOLBAR", "WELCOME_GUEST" ) ) );

				buildHeader.AppendFormat( @"<td style=""padding:5px"" align=""right"" valign=""middle"" class=""post"">" );
				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.search ), PageContext.Localization.GetText( "TOOLBAR", "SEARCH" ) ) );
				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a> | ", YafBuildLink.GetLink( ForumPages.active ), PageContext.Localization.GetText( "TOOLBAR", "ACTIVETOPICS" ) ) );
				buildHeader.AppendFormat( String.Format( "	<a href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.members ), PageContext.Localization.GetText( "TOOLBAR", "MEMBERS" ) ) );
				buildHeader.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.login, "ReturnUrl={0}", HttpContext.Current.Server.UrlEncode( General.GetSafeRawUrl() ) ), PageContext.Localization.GetText( "TOOLBAR", "LOGIN" ) ) );
				if ( !PageContext.BoardSettings.DisableRegistrations )
					buildHeader.AppendFormat( String.Format( " | <a href=\"{0}\">{1}</a>", YafBuildLink.GetLink( ForumPages.rules ), PageContext.Localization.GetText( "TOOLBAR", "REGISTER" ) ) );
			}
			buildHeader.AppendFormat( "</td></tr></table>" );
			buildHeader.AppendFormat( "<br />" );

			// END HEADER

			// write CSS, Refresh, then header...
			WriteCSS( ref writer );
			WriteRefresh( ref writer );
			writer.Write( buildHeader );
		}
	}

	/// <summary>
	/// Class test.
	/// </summary>
	public class Test : BaseControl
	{
		/// <summary>
		/// The default constructor for Test.
		/// </summary>
		public Test()
		{
		}

		/// <summary>
		/// Renders the Test class.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			string act_rank = "";

			act_rank += "<table width=\"90%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">";
			act_rank += "<tr class=\"header2\"><td>Most active users</td></tr>";
			//act_rank += "<tr class=header2><td colspan=\"2\">User</td>";
			//act_rank += "<td align=\"center\">Posts</td></tr>";

			System.Data.DataTable rank = YAF.Classes.Data.DB.user_activity_rank( null );
			int i = 1;

			act_rank += "<tr><td class=post><table cellspacing=0 cellpadding=0 align=center>";

			foreach ( System.Data.DataRow r in rank.Rows )
			{
				string img = "<img src='/yetanotherforum.net/themes/standard/user_rank1.gif'/>";
				// string.Format( "<img src=\"{0}\"/>", MyPage.ThemeFile( string.Format( "user_rank{0}.gif", i ) ) );

				i++;
				act_rank += "<tr class=\"post\">";

				// Immagine
				act_rank += string.Format( "<td align=\"center\">{0}</td>", img );

				// Nome autore
				act_rank += string.Format( "<td width=\"75%\">&nbsp;<a href='{1}'>{0}</a></td>", r ["Name"], YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", r ["ID"] ) );

				// Numero post
				act_rank += string.Format( "<td align=\"center\">{0}</td></tr>", r ["NumOfPosts"] );

				act_rank += "</tr>";
			}

			act_rank += "</table></td></tr>";

			act_rank += "</table>";
			writer.Write( act_rank );
		}
	}
}
