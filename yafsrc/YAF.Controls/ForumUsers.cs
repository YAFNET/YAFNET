/* Yet Another Forum.NET
 * Copyright (C) 2006-2008 Jaben Cargman
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
		public ForumUsers()
		{
			this.Load += new EventHandler( ForumUsers_Load );
		}

		void ForumUsers_Load( object sender, EventArgs e )
		{
			DataTable dt = ( DataTable ) ViewState ["data"];
			bool bTopic = PageContext.PageTopicID > 0;

			if ( dt == null )
			{
				if ( bTopic )
					dt = YAF.Classes.Data.DB.active_listtopic( PageContext.PageTopicID );
				else
					dt = YAF.Classes.Data.DB.active_listforum( PageContext.PageForumID );
				ViewState ["data"] = dt;
			}

			bool bFirst = true;

			foreach (DataRow row in dt.Rows)
			{
				UserLink userLink = new UserLink();
				userLink.UserID = Convert.ToInt32( row ["UserID"] );
				userLink.UserName = row ["UserName"].ToString();
				userLink.ID = "UserLink" + userLink.UserID.ToString();

				this.Controls.Add( userLink );

				if ( bFirst ) bFirst = false;
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			// Ederon : 07/14/2007
			if (!PageContext.BoardSettings.ShowBrowsingUsers) return;

			bool bTopic = PageContext.PageTopicID > 0;

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
			foreach ( System.Web.UI.Control control in this.Controls )
			{
				if ( !bFirst ) writer.WriteLine( ", " );
				control.RenderControl( writer );
				bFirst = false;
			}

			//base.Render( writer );
			
			writer.WriteLine( "</td>" );
			writer.WriteLine( "</tr>" );

			
		}
	}
}
