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

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for PageLinks.
	/// </summary>
	public class PageLinks : BaseControl
	{
		public void AddLink( string title )
		{
			AddLink( title, "" );
		}

		public void AddLink( string title, string url )
		{
			DataTable dt = ( DataTable ) ViewState ["data"];
			if ( dt == null )
			{
				dt = new DataTable();
				dt.Columns.Add( "Title", typeof( string ) );
				dt.Columns.Add( "URL", typeof( string ) );
				ViewState ["data"] = dt;
			}
			DataRow dr = dt.NewRow();
			dr ["Title"] = title;
			dr ["URL"] = url;
			dt.Rows.Add( dr );
		}

		/// <summary>
		/// Clear all Links
		/// </summary>
		public void Clear()
		{
			DataTable dt = ( DataTable ) ViewState ["data"];
			if ( dt != null )
			{
				ViewState ["data"] = null;
			}
		}

		public void AddForumLinks( int forumID )
		{
			this.AddForumLinks( forumID, false );
		}

		public void AddForumLinks( int forumID, bool noForumLink )
		{
			using ( DataTable dtLinks = YAF.Classes.Data.DB.forum_listpath( forumID ) )
			{
				foreach ( DataRow row in dtLinks.Rows )
				{
					if ( noForumLink && Convert.ToInt32( row ["ForumID"] ) == forumID )
						AddLink( row ["Name"].ToString(), "" );
					else
						AddLink( row ["Name"].ToString(), YAF.Classes.Utils.YafBuildLink.GetLink( YAF.Classes.Utils.ForumPages.topics, "f={0}", row ["ForumID"] ) );
				}
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			DataTable m_links = ( DataTable ) ViewState ["data"];

			if ( m_links == null || m_links.Rows.Count == 0 ) return;

			writer.WriteLine( "<p class=\"navlinks\">" );

			bool bFirst = true;
			foreach ( DataRow row in m_links.Rows )
			{
				if ( !bFirst )
					writer.WriteLine( "&#187;" );
				else
					bFirst = false;

				string title = HtmlEncode( row ["Title"].ToString().Trim() );
				string url = row ["URL"].ToString().Trim();

				if ( String.IsNullOrEmpty( url ) )
				{
					writer.WriteLine( String.Format( "<span id=\"current\">{0}</span>", title ) );
				}
				else
				{
					writer.WriteLine( String.Format( "<a href=\"{0}\">{1}</a>", url, title ) );
				}
			}

			writer.WriteLine( "</p>" );
		}
	}
}
