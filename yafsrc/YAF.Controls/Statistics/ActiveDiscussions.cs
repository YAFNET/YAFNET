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
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using YAF.Classes.Utils;

namespace YAF.Controls.Statistics
{
	[ToolboxData( "<{0}:ActiveDiscussions runat=\"server\"></{0}:ActiveDiscussions>" )]
	public class ActiveDiscussions : BaseControl
	{
		private int _displayNumber = 10;

		/// <summary>
		/// The default constructor for ActiveDiscussions.
		/// </summary>
		public ActiveDiscussions()
		{

		}

		public int DisplayNumber
		{
			get
			{
				return _displayNumber;
			}
			set
			{
				_displayNumber = value;
			}
		}

		/// <summary>
		/// Renders the ActiveDiscussions class.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder();

			string cacheKey = YafCache.GetBoardCacheKey( Constants.Cache.ActiveDiscussions );
			DataTable dt = YafCache.Current [cacheKey] as DataTable;

			if ( dt == null )
			{
				dt = YAF.Classes.Data.DB.topic_latest( PageContext.PageBoardID, _displayNumber, PageContext.PageUserID );
				YafCache.Current.Insert(cacheKey, dt, null, DateTime.Now.AddMinutes(PageContext.BoardSettings.ActiveDiscussionsCacheTimeout), TimeSpan.Zero);
			}		

			html.Append( "<table width=\"100%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">" );
			html.AppendFormat( "<tr><td class=\"header1\">{0}</td></tr>", PageContext.Localization.GetText( "LATEST_POSTS" ) );

			int currentPost = 1;

			html.Append( "<tr><td class=\"post\">" );

			foreach ( System.Data.DataRow r in dt.Rows )
			{
				//Output Topic Link
				html.AppendFormat( "{2}.&nbsp;<a href=\"{1}\">{0}</a> ({3})",
					General.BadWordReplace( Convert.ToString( r ["Topic"] ) ),
					YafBuildLink.GetLink( ForumPages.posts, "m={0}#{0}", r ["LastMessageID"] ),
					currentPost,
					r["NumPosts"]
					);
				html.Append( "<br/>" );

				currentPost++;
			}

			html.Append( "</td></tr></table>" );

			writer.Write( html.ToString() );
		}
	}
}
