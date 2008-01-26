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
		protected DataTable _dtSmileys;
		private string _onclick;

		//public int pagenum = 0;
		private int _pagesize = 18;
		private int _perrow = 6;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				MoreSmilies.Text = PageContext.Localization.GetText("SMILIES_MORE");
				MoreSmilies.NavigateUrl = YafBuildLink.GetLink(ForumPages.showsmilies);
				MoreSmilies.Target = "yafShowSmilies";
				MoreSmilies.Attributes.Add("onclick",
					String.Format( "var smiliesWin = window.open('{0}', '{1}', 'height={2},width={3},scrollbars=yes,resizable=yes');smiliesWin.focus();return false;",
						MoreSmilies.NavigateUrl,
						MoreSmilies.Target,
						550,
						300));
			}

			YafBoardSettings bs = PageContext.BoardSettings;
			_pagesize = bs.SmiliesColumns * bs.SmiliesPerRow;
			_perrow = bs.SmiliesPerRow;

			// setup the header
			AddSmiley.Attributes.Add("colspan", _perrow.ToString());
			AddSmiley.InnerHtml = PageContext.Localization.GetText("SMILIES_HEADER");
			// setup footer
			MoreSmiliesCell.Attributes.Add("colspan", _perrow.ToString());

			_dtSmileys = YAF.Classes.Data.DB.smiley_listunique(base.PageContext.PageBoardID);

			if (_dtSmileys.Rows.Count == 0)
			{
				SmiliesPlaceholder.Visible = false;
			}
			else
			{
				MoreSmiliesHolder.Visible = ( _dtSmileys.Rows.Count > _pagesize );
				CreateSmileys();
			}
		}

		private void CreateSmileys()
		{
			System.Text.StringBuilder html = new System.Text.StringBuilder();
			html.AppendFormat("<tr class='post'>");
			int rowcells = 0;

			for (int i = 0; i < _pagesize; i++)
			{
				if (i < _dtSmileys.Rows.Count)
				{
					DataRow row = _dtSmileys.Rows[i];
					if (i % _perrow == 0 && i > 0 && i < _dtSmileys.Rows.Count)
					{
						html.Append("</tr><tr class='post'>\n");
						rowcells = 0;
					}
					string evt = "";
					if (_onclick.Length > 0)
					{
						string strCode = Convert.ToString(row["Code"]).ToLower();
						strCode = strCode.Replace("&", "&amp;");
						strCode = strCode.Replace("\"", "&quot;");
						strCode = strCode.Replace("'", "\\'");
						evt = String.Format("javascript:{0}('{1} ','{3}images/emoticons/{2}')", _onclick, strCode, row["Icon"], YafForumInfo.ForumRoot);
					}
					else
					{
						evt = "javascript:void()";
					}
					html.AppendFormat( "<td><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" /></a></td>\n", YafBuildLink.Smiley( ( string )row ["Icon"] ), row ["Emoticon"], evt );
					rowcells++;
				}
			}
			while (rowcells++ < _perrow) html.AppendFormat("<td>&nbsp;</td>");
			html.AppendFormat("</tr>");

			SmileyResults.Text = html.ToString();
		}

		public string OnClick
		{
			set
			{
				_onclick = value;
			}
		}
	}
}
