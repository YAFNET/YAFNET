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
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Footer.
	/// </summary>
	public class Footer : BaseControl
	{
		private bool _simpleRender = false;
		private System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();

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

		public System.Diagnostics.Stopwatch StopWatch
		{
			get
			{
				return _stopWatch;
			}
		}

		public void Reset()
		{
			StopWatch.Reset();
			SimpleRender = false;
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			if ( !SimpleRender ) RenderRegular( ref writer );
			else RenderSimple( ref writer );
		}

		protected void WriteOnLoadString( ref System.Web.UI.HtmlTextWriter writer )
		{
			if ( PageContext.LoadString.Length > 0 )
			{
				writer.WriteLine( String.Format( "<script language=\"javascript\" type=\"text/javascript\">\nonload=function(){1}\nalert(\"{0}\")\n{2}\n</script>\n", PageContext.LoadString, '{', '}' ) );
			}
		}

		protected void RenderSimple( ref System.Web.UI.HtmlTextWriter writer )
		{
			WriteOnLoadString( ref writer );

			writer.WriteLine( "</body>" );
			writer.WriteLine( "</html>" );
		}

		protected void RenderRegular( ref System.Web.UI.HtmlTextWriter writer )
		{
			// BEGIN FOOTER
			StringBuilder footer = new StringBuilder();
			footer.AppendFormat( "<p style=\"text-align:center;font-size:7pt\">" );

			/* Commented out by Jaben on 9/27/2007: No longer needed with the RSS feed on the main forum.	 
			if ( PageContext.BoardSettings.ShowRSSLink )
			{
				footer.AppendFormat( "{2} : <a href=\"{0}\"><img style=\"vertical-align: middle\" src=\"{1}images/RssFeed.png\" alt=\"RSS Feed\" /></a><br /><br />", YafBuildLink.GetLink( ForumPages.rsstopic, "pg=forum" ), YafForumInfo.ForumRoot, PageContext.Localization.GetText( "DEFAULT", "MAIN_FORUM_RSS" ) );
				// footer.AppendFormat("Main Forum Rss Feed : <a href=\"{0}rsstopic.aspx?pg=forum\"><img valign=\"absmiddle\" src=\"{1}images/rss.gif\" alt=\"RSS\" /></a><br /><br />", Data.ForumRoot, Data.ForumRoot);
			}*/

			// get the theme credit info from the theme file
			// it's not really an error if it doesn't exist
			string themeCredit = PageContext.Theme.GetItem( "THEME", "CREDIT", null );

			if ( themeCredit != null && themeCredit.Length > 0 )
			{
				themeCredit = @"<span id=""themecredit"" style=""color:#999999"">" + themeCredit + @"</span><br />";
			}

			StopWatch.Stop();
			double duration = ( double ) StopWatch.ElapsedMilliseconds / 1000.0;

			if ( YAF.Classes.Config.IsDotNetNuke )
			{
				if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
        footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.NET</a> version {0} running under DotNetNuke.", YafForumInfo.AppVersionName );
        footer.AppendFormat( "<br />Copyright &copy; 2003-2007 Yet Another Forum.NET. All rights reserved." );
			}
			else if ( YAF.Classes.Config.IsRainbow )
			{
				if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
        footer.AppendFormat( "<a target=\"_top\" title=\"Yet Another Forum.net Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.NET</a> version {0} running under Rainbow.", YafForumInfo.AppVersionName );
        footer.AppendFormat( "<br />Copyright &copy; 2003-2007 Yet Another Forum.NET. All rights reserved." );
			}
			else if ( PageContext.Settings.LockedForum == 0 )
			{
				if ( themeCredit != null && themeCredit.Length > 0 ) footer.Append( themeCredit );
				footer.AppendFormat( PageContext.Localization.GetText( "COMMON", "POWERED_BY" ),
					String.Format( "<a target=\"_top\" title=\"Yet Another Forum.NET Home Page\" href=\"http://www.yetanotherforum.net/\">Yet Another Forum.NET</a>" ),
					String.Format( "{0} (NET v{2}.{3}) - {1}", YafForumInfo.AppVersionName, YafDateTime.FormatDateShort( YafForumInfo.AppVersionDate ), System.Environment.Version.Major.ToString(), System.Environment.Version.Minor.ToString() )
					);
				footer.AppendFormat( "<br />Copyright &copy; 2003-2007 Yet Another Forum.NET. All rights reserved." );
				footer.AppendFormat( "<br/>" );
				footer.AppendFormat( PageContext.AdminLoadString ); // Append a error message for an admin to see (but not nag)

				if ( PageContext.BoardSettings.ShowPageGenerationTime )
					footer.AppendFormat( PageContext.Localization.GetText( "COMMON", "GENERATED" ), duration );
			}

#if DEBUG      
      footer.AppendFormat( @"<br/><br/><div style=""width:350px;margin:auto;padding:5px;text-align:center;font-size:7pt;""><span style=""color:#990000"">YAF Compiled in <b>DEBUG MODE</b></span>.<br/>Recompile in <b>RELEASE MODE</b> to remove this information:" );
			footer.AppendFormat( @"<br/>{0} sql queries ({1:N3} seconds, {2:N2}%).<br/>{3}", QueryCounter.Count, QueryCounter.Duration, ( 100 * QueryCounter.Duration ) / duration, QueryCounter.Commands );
      footer.Append( "</div>" );
#endif
      footer.AppendFormat( "</p>" );
			// END FOOTER

			// write CSS, Refresh, then header...
			writer.Write( footer );

			WriteOnLoadString( ref writer );
		}
	}
}
