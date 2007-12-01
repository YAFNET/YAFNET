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
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for ForumUsers.
	/// </summary>
	public class IconLegend : BaseControl
	{
		public IconLegend()
		{
			this.Load += new EventHandler( IconLegend_Load );
		}

		void IconLegend_Load( object sender, EventArgs e )
		{
			string [] themeImageTags = { "TOPIC_NEW", "TOPIC", "TOPIC_NEW_LOCKED", "TOPIC_LOCKED", "TOPIC_ANNOUNCEMENT", "TOPIC_STICKY", "TOPIC_MOVED", "TOPIC_POLL" };
			string [] localizedTags = { "NEW_POSTS", "NO_NEW_POSTS", "NEW_POSTS_LOCKED", "NO_NEW_POSTS_LOCKED", "ANNOUNCEMENT", "STICKY", "MOVED", "POLL" };

			HtmlTableRow tr = null;
			HtmlTableCell td = null;

			// add a table control
			HtmlTable table = new HtmlTable();
			table.Attributes.Add( "class", "iconlegend" );
			this.Controls.Add( table );

			for ( int i = 0; i < themeImageTags.Length; i++ )
			{
				if ( ( i % 2 ) == 0 || tr == null )
				{
					// add <tr>
					tr = new HtmlTableRow();
					table.Controls.Add( tr );
				}

				// add this to the tr...
				td = new HtmlTableCell();
				tr.Controls.Add( td );

				// add the themed icons
				ThemeImage themeImage = new ThemeImage();
				themeImage.ThemeTag = themeImageTags [i];
				td.Controls.Add( themeImage );

				// space
				Literal space = new Literal();
				space.Text = " ";
				td.Controls.Add( space );

				// localized text describing the image
				LocalizedLabel localLabel = new LocalizedLabel();
				localLabel.LocalizedTag = localizedTags [i];
				td.Controls.Add( localLabel );
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
		}
	}
}
