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
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for bannedip.
  /// </summary>
  public partial class bannedip : AdminPage
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
        this.PageLinks.AddLink("Banned IP Addresses", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.list.DataSource = DB.bannedip_list(PageContext.PageBoardID, null);
      DataBind();
    }

    /// <summary>
    /// The list_ item command.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void list_ItemCommand(object sender, RepeaterCommandEventArgs e)
    {
      if (e.CommandName == "add")
      {
        YafBuildLink.Redirect(ForumPages.admin_bannedip_edit);
      }
      else if (e.CommandName == "edit")
      {
        YafBuildLink.Redirect(ForumPages.admin_bannedip_edit, "i={0}", e.CommandArgument);
      }
      else if (e.CommandName == "delete")
      {
        // vzrus: Logging is disabled here as the log entries are not protected anyway from simply admins in the standard YAF edition  
        // string maskStr = DB.bannedip_list(this.PageContext.PageBoardID, e.CommandArgument).Rows[0]["Mask"].ToString();
        DB.bannedip_delete(e.CommandArgument);
        // DB.eventlog_create(this.PageContext.PageUserID, this, string.Format("Banned IP entry '{0}' was deleted.", maskStr), EventLogTypes.Information);
        // clear cache of banned IPs for this board
        PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BannedIP));

        BindData();
        PageContext.AddLoadMessage("Removed IP address ban.");
      }
    }
  }
}