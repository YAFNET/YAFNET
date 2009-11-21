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
  using System.Drawing;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for forums.
  /// </summary>
  public partial class accessmasks : AdminPage
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
        this.PageLinks.AddLink("Access Masks", string.Empty);
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
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this access mask?')";
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.List.DataSource = DB.accessmask_list(PageContext.PageBoardID, null);     
      DataBind();
    }

    /// <summary>
    /// The get item color.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// </returns>
    protected Color GetItemColor(bool enabled)
    {
      if (enabled)
      {
        return Color.Red;
      }

      return Color.Black;
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void List_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_editaccessmask, "i={0}", e.CommandArgument);
          break;
        case "delete":
          if (DB.accessmask_delete(e.CommandArgument))
          {
            PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));
            BindData();
          }
          else
          {
            PageContext.AddLoadMessage("You cannot delete this access mask because it is in use.");
          }

          break;
      }
    }

    /// <summary>
    /// The new_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void New_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_editaccessmask);
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