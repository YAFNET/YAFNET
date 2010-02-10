/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System.Text;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for smileys.
  /// </summary>
  public partial class smileys : BaseUserControl
  {
    /// <summary>
    /// The _dt smileys.
    /// </summary>
    protected DataTable _dtSmileys;

    /// <summary>
    /// The _onclick.
    /// </summary>
    private string _onclick;

    // public int pagenum = 0;

    /// <summary>
    /// The _pagesize.
    /// </summary>
    private int _pagesize = 18;

    /// <summary>
    /// The _perrow.
    /// </summary>
    private int _perrow = 6;

    /// <summary>
    /// Sets OnClick.
    /// </summary>
    public string OnClick
    {
      set
      {
        this._onclick = value;
      }
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.MoreSmilies.Text = PageContext.Localization.GetText("SMILIES_MORE");
        this.MoreSmilies.NavigateUrl = YafBuildLink.GetLink(ForumPages.showsmilies);
        this.MoreSmilies.Target = "yafShowSmilies";
        this.MoreSmilies.Attributes.Add(
          "onclick", 
          String.Format(
            "var smiliesWin = window.open('{0}', '{1}', 'height={2},width={3},scrollbars=yes,resizable=yes');smiliesWin.focus();return false;", 
            this.MoreSmilies.NavigateUrl, 
            this.MoreSmilies.Target, 
            550, 
            300));
      }

      YafBoardSettings bs = PageContext.BoardSettings;
      this._pagesize = bs.SmiliesColumns * bs.SmiliesPerRow;
      this._perrow = bs.SmiliesPerRow;

      // setup the header
      this.AddSmiley.Attributes.Add("colspan", this._perrow.ToString());
      this.AddSmiley.InnerHtml = PageContext.Localization.GetText("SMILIES_HEADER");

      // setup footer
      this.MoreSmiliesCell.Attributes.Add("colspan", this._perrow.ToString());

      this._dtSmileys = DB.smiley_listunique(PageContext.PageBoardID);

      if (this._dtSmileys.Rows.Count == 0)
      {
        this.SmiliesPlaceholder.Visible = false;
      }
      else
      {
        this.MoreSmiliesHolder.Visible = this._dtSmileys.Rows.Count > this._pagesize;
        CreateSmileys();
      }
    }

    /// <summary>
    /// The create smileys.
    /// </summary>
    private void CreateSmileys()
    {
      var html = new StringBuilder();
      html.AppendFormat("<tr class='post'>");
      int rowcells = 0;

      for (int i = 0; i < this._pagesize; i++)
      {
        if (i < this._dtSmileys.Rows.Count)
        {
          DataRow row = this._dtSmileys.Rows[i];
          if (i % this._perrow == 0 && i > 0 && i < this._dtSmileys.Rows.Count)
          {
            html.Append("</tr><tr class='post'>\n");
            rowcells = 0;
          }

          string evt = string.Empty;
          if (this._onclick.Length > 0)
          {
            string strCode = Convert.ToString(row["Code"]).ToLower();
            strCode = strCode.Replace("&", "&amp;");
            strCode = strCode.Replace(">", "&gt;");
            strCode = strCode.Replace("<", "&lt;");
            strCode = strCode.Replace("\"", "&quot;");
            strCode = strCode.Replace("\\", "\\\\");
            strCode = strCode.Replace("'", "\\'");
            evt = String.Format(
              "javascript:{0}('{1} ','{3}{4}/{2}')", this._onclick, strCode, row["Icon"], YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons);
          }
          else
          {
            evt = "javascript:void()";
          }

          html.AppendFormat(
            "<td><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" /></a></td>\n", 
            YafBuildLink.Smiley((string) row["Icon"]), 
            row["Emoticon"], 
            evt);
          rowcells++;
        }
      }

      while (rowcells++ < this._perrow)
      {
        html.AppendFormat("<td>&nbsp;</td>");
      }

      html.AppendFormat("</tr>");

      this.SmileyResults.Text = html.ToString();
    }
  }
}