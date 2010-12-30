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
  #region Using

  using System;
  using System.Data;
  using System.Text;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for smileys.
  /// </summary>
  public partial class smileys : BaseUserControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _dt smileys.
    /// </summary>
    protected DataTable _dtSmileys;

    /// <summary>
    ///   The _onclick.
    /// </summary>
    private string _onclick;

    /// <summary>
    ///   The _perrow.
    /// </summary>
    private int _perrow = 6;

    #endregion

    #region Properties

    /// <summary>
    ///   Sets OnClick.
    /// </summary>
    public string OnClick
    {
      set
      {
        this._onclick = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBoardSettings bs = this.PageContext.BoardSettings;

      this._perrow = bs.SmiliesPerRow;

      this._dtSmileys = LegacyDb.smiley_listunique(this.PageContext.PageBoardID);

      if (this._dtSmileys.Rows.Count == 0)
      {
        this.SmiliesPlaceholder.Visible = false;
      }
      else
      {
        this.CreateSmileys();
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

      for (int i = 0; i < this._dtSmileys.Rows.Count; i++)
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
          evt = "javascript:{0}('{1} ','{3}{4}/{2}')".FormatWith(
            this._onclick, strCode, row["Icon"], YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons);
        }
        else
        {
          evt = "javascript:void()";
        }

        html.AppendFormat(
          "<td><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" /></a></td>\n", 
          YafBuildLink.Smiley((string)row["Icon"]), 
          row["Emoticon"], 
          evt);
        rowcells++;
      }

      while (rowcells++ < this._perrow)
      {
        html.AppendFormat("<td>&nbsp;</td>");
      }

      html.AppendFormat("</tr>");

      this.SmileyResults.Text = html.ToString();
    }

    #endregion
  }
}