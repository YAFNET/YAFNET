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
using System.Data;
using YAF.Classes.UI;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class ForumUsers : BaseControl
	{
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			// Ederon : 07/14/2007
			if (!PageContext.BoardSettings.ShowBrowsingUsers) return;

			DataTable dt = ( DataTable ) ViewState ["data"];
			bool bTopic = PageContext.PageTopicID > 0;
			try
			{
				if ( dt == null )
				{
					if ( bTopic )
						dt = YAF.Classes.Data.DB.active_listtopic( PageContext.PageTopicID );
					else
						dt = YAF.Classes.Data.DB.active_listforum( PageContext.PageForumID );
					ViewState ["data"] = dt;
				}

				if ( bTopic )
				{
					writer.WriteLine( "<tr class=\"header2\">" );
					writer.WriteLine( String.Format( "<td colspan=\"3\">{0}</td>", PageContext.Localization.GetText( "TOPICBROWSERS" ) ) );
					writer.WriteLine( "</tr>" );
					writer.WriteLine( "<tr class=\"post\">" );
					writer.WriteLine( "<td colspan=\"3\">" );
				}
				else
				{
					writer.WriteLine( "<tr class=\"header2\">" );
					writer.WriteLine( String.Format( "<td colspan=\"6\">{0}</td>", PageContext.Localization.GetText( "FORUMUSERS" ) ) );
					writer.WriteLine( "</tr>" );
					writer.WriteLine( "<tr class=\"post\">" );
					writer.WriteLine( "<td colspan=\"6\">" );
				}

				bool bFirst = true;
				foreach ( DataRow dr in dt.Rows )
				{
					if ( !bFirst )
					{
						writer.WriteLine( "," );
					}
					else
					{
						bFirst = false;
					}
					writer.Write( String.Format( "<a href=\"{0}\">{1}</a>", YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.profile, "u={0}", dr ["UserID"] ), BBCode.EncodeHTML( dr ["UserName"].ToString() ) ) );
				}
				writer.WriteLine( "</td>" );
				writer.WriteLine( "</tr>" );
			}
			finally
			{
				if ( dt != null ) dt.Dispose();
			}
		}
	}
}
