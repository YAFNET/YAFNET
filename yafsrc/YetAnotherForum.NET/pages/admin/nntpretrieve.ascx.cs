/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Core.Nntp;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for ranks.
  /// </summary>
  public partial class nntpretrieve : AdminPage
  {
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
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("NNTP Retrieve", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = DB.nntpforum_list(PageContext.PageBoardID, 10, null, true);
      DataBind();
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
    protected void Retrieve_Click(object sender, EventArgs e)
    {
      int nSeconds = int.Parse(this.Seconds.Text);
      if (nSeconds < 1)
      {
        nSeconds = 1;
      }

      int nArticleCount = YafNntp.ReadArticles(PageContext.PageBoardID, 10, nSeconds, PageContext.BoardSettings.CreateNntpUsers);
      PageContext.AddLoadMessage(String.Format("Retrieved {0} articles. {1:N2} articles per second.", nArticleCount, (double) nArticleCount / nSeconds));
      BindData();
    }

    /// <summary>
    /// The last message no.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The last message no.
    /// </returns>
    protected string LastMessageNo(object _o)
    {
      var row = (DataRowView) _o;
      return string.Format("{0:N0}", row["LastMessageNo"]);
    }
  }
}