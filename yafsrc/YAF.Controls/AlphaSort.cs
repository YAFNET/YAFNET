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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using YAF.Classes.Data;
using YAF.Classes.Utils;

namespace YAF.Controls
{
	/// <summary>
	/// Summary description for Footer.
	/// </summary>
	public class AlphaSort : BaseControl
	{
		public AlphaSort()
		{
			this.Load += new EventHandler( AlphaSort_Load );
		}

		void AlphaSort_Load( object sender, EventArgs e )
		{
			HtmlTable table = new HtmlTable();

			table.Attributes.Add( "class", "content" );
			table.Width = "100%";

			this.Controls.Add( table );

			// create row
			HtmlTableRow tr = new HtmlTableRow();
			table.Controls.Add( tr );

			// get the localized character set
			string charSet = PageContext.Localization.GetText( "LANGUAGE", "CHARSET" );
			// get the current selected character (if there is one)
			char selectedLetter = CurrentLetter;

			foreach ( char letter in charSet )
			{
				HtmlTableCell cell = new HtmlTableCell();
				cell.Align = "center";

				if ( selectedLetter != char.MinValue && selectedLetter == letter )
				{
					cell.Attributes ["class"] = "postheader";
				}
				else
				{
					cell.Attributes ["class"] = "post";
				}

				// create a link to this letter
				HyperLink link = new HyperLink();
				link.Text = letter.ToString();
				link.NavigateUrl = YAF.Classes.Utils.YafBuildLink.GetLinkNotEscaped( YAF.Classes.Utils.ForumPages.members, "letter={0}", letter == '#' ? '_' : letter );
				// add it to this td
				cell.Controls.Add( link );

				// add this cell to the row
				tr.Cells.Add( cell );
			}
		}

		protected override void Render( System.Web.UI.HtmlTextWriter writer )
		{
			base.Render( writer );
		}

		public char CurrentLetter
		{
			get
			{
				char currentLetter = char.MinValue;

				if ( HttpContext.Current.Request.QueryString ["letter"] != null )
				{
					try
					{
						currentLetter = char.Parse( HttpContext.Current.Request.QueryString ["letter"] );
						if ( currentLetter == '_' ) currentLetter = '#';
					}
					catch
					{

					}
				}
				return currentLetter;
			}
		}
	}
}
