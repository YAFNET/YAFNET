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

namespace YAF.Controls
{
	using System;
	using System.Data;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using YAF.Classes.Utils;

	/// <summary>
	///		Summary description for smileys.
	/// </summary>
	public partial class smileys : YAF.Classes.Base.BaseUserControl
	{
		protected DataTable dtSmileys;
		private string _onclick;

		public int pagenum = 0;
		public int pagesize = 18;
		public int perrow = 6;

		protected void Page_Load( object sender, System.EventArgs e )
		{
			yaf_BoardSettings bs = PageContext.BoardSettings;
			pagesize = bs.SmiliesColumns * bs.SmiliesPerRow;
			perrow = bs.SmiliesPerRow;

			// setup the header
			AddSmiley.Attributes.Add( "colspan", perrow.ToString() );
			AddSmiley.InnerHtml = PageContext.Localization.GetText( "SMILIES_HEADER" );

			dtSmileys = YAF.Classes.Data.DB.smiley_listunique( base.PageContext.PageBoardID );

			pager.PageSize = pagesize;
			CreateSmileys();

		}

		private void pager_PageChange( object sender, EventArgs e )
		{
			CreateSmileys();
		}

		private void CreateSmileys()
		{
			int pgnum = pager.CurrentPageIndex;
			pager.Count = dtSmileys.Rows.Count;
			int intpg = pgnum * pagesize;

			System.Text.StringBuilder html = new System.Text.StringBuilder();
			html.AppendFormat( "<tr class='post'>" );
			int rowcells = 0;

			for ( int i = intpg; i < intpg + pagesize; i++ )
			{
				if ( i < dtSmileys.Rows.Count )
				{
					DataRow row = dtSmileys.Rows [i];
					if ( i % perrow == 0 && i > 0 && i < dtSmileys.Rows.Count )
					{
						html.Append( "</tr><tr class='post'>\n" );
						rowcells = 0;
					}
					string evt = "";
					if ( _onclick.Length > 0 )
					{
						string strCode = Convert.ToString( row ["Code"] ).ToLower();
						strCode = strCode.Replace( "&", "&amp;" );
						strCode = strCode.Replace( "\"", "&quot;" );
						strCode = strCode.Replace( "'", "\\'" );
						evt = String.Format( "javascript:{0}('{1} ','{3}images/emoticons/{2}')", _onclick, strCode, row ["Icon"], yaf_ForumInfo.ForumRoot );
					}
					else
					{
						evt = "javascript:void()";
					}
					html.AppendFormat( "<td><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" title=\"{1}\"/></a></td>\n", yaf_BuildLink.Smiley( ( string ) row ["Icon"] ), row ["Emoticon"], evt );
					rowcells++;
				}
			}
			while ( rowcells++ < perrow ) html.AppendFormat( "<td>&nbsp;</td>" );
			html.AppendFormat( "</tr>" );

			SmileyResults.Text = html.ToString();
		}

		#region Web Form Designer generated code
		override protected void OnInit( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit( e );
		}

		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			pager.PageChange += new EventHandler( pager_PageChange );
		}
		#endregion

		public string onclick
		{
			set
			{
				_onclick = value;
			}
		}
	}
}
