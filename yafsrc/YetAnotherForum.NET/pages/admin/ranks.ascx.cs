/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2009 Jaben Cargman
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
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for ranks.
  /// </summary>
  public partial class ranks : AdminPage
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
        this.PageLinks.AddLink("Ranks", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The delete_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Delete_Load(object sender, EventArgs e)
    {
      ControlHelper.AddOnClickConfirmDialog(sender, "Delete this rank?");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.RankList.DataSource = DB.rank_list(PageContext.PageBoardID, null);
      DataBind();
    }

    /// <summary>
    /// The rank list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void RankList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_editrank, "r={0}", e.CommandArgument);
          break;
        case "delete":
          DB.rank_delete(e.CommandArgument);
          BindData();
          break;
      }
    }

    /// <summary>
    /// The new rank_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void NewRank_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_editrank);
    }

    /// <summary>
    /// The ladder info.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <returns>
    /// The ladder info.
    /// </returns>
    protected string LadderInfo(object _o)
    {
      var dr = (DataRowView) _o;

      // object IsLadder,object MinPosts
      // Eval( "IsLadder"),Eval( "MinPosts")
      bool isLadder = General.BinaryAnd(dr["Flags"], RankFlags.Flags.IsLadder);

      string tmp = String.Format("{0}", isLadder);
      if (isLadder)
      {
        tmp += String.Format(" ({0} posts)", dr["MinPosts"]);
      }

      return tmp;
    }

    /// <summary>
    /// The bit set.
    /// </summary>
    /// <param name="_o">
    /// The _o.
    /// </param>
    /// <param name="bitmask">
    /// The bitmask.
    /// </param>
    /// <returns>
    /// The bit set.
    /// </returns>
    protected bool BitSet(object _o, int bitmask)
    {
      var i = (int) _o;
      return (i & bitmask) != 0;
    }
  }
}