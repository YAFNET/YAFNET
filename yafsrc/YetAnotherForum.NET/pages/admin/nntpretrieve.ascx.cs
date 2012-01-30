/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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

namespace YAF.Pages.Admin
{
  #region Using

  using System;
  using System.Data;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for ranks.
  /// </summary>
  public partial class nntpretrieve : AdminPage
  {
    #region Methods

    /// <summary>
    /// The last message no.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The last message no.
    /// </returns>
    protected string LastMessageNo([NotNull] object _o)
    {
      var row = (DataRowView)_o;
      return "{0:N0}".FormatWith(row["LastMessageNo"]);
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
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (this.IsPostBack)
      {
        return;
      }

      this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
      this.PageLinks.AddLink(
        this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));
      this.PageLinks.AddLink(this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"), string.Empty);

      this.Page.Header.Title = "{0} - {1}".FormatWith(
        this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_NNTPRETRIEVE", "TITLE"));

      this.Retrieve.Text = this.GetText("ADMIN_NNTPRETRIEVE", "RETRIEVE");

      this.BindData();
    }

    /// <summary>
    /// The retrieve_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Retrieve_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      int nSeconds = int.Parse(this.Seconds.Text);
      if (nSeconds < 1)
      {
        nSeconds = 1;
      }

      int nArticleCount = this.Get<INewsreader>().ReadArticles(
        this.PageContext.PageBoardID, 10, nSeconds, this.PageContext.BoardSettings.CreateNntpUsers);

      this.PageContext.AddLoadMessage(
        this.GetText("ADMIN_NNTPRETRIEVE", "Retrieved").FormatWith(nArticleCount, (double)nArticleCount / nSeconds));
      this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = LegacyDb.nntpforum_list(this.PageContext.PageBoardID, 10, null, true);
      this.DataBind();
    }

    #endregion
  }
}