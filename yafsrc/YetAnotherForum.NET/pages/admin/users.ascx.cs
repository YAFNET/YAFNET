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
  using YAF.Utilities;

  /// <summary>
  /// Summary description for members.
  /// </summary>
  public partial class users : AdminPage
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
      PageContext.PageElements.RegisterJQuery();
      PageContext.PageElements.RegisterJsResourceInclude("blockUIJs", "js/jquery.blockUI.js");

      if (!IsPostBack)
      {
        this.LoadingImage.ImageUrl = YafForumInfo.GetURLToResource("images/loading-white.gif");

        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Users", string.Empty);

        using (DataTable dt = DB.group_list(PageContext.PageBoardID, null))
        {
          DataRow newRow = dt.NewRow();
          newRow["Name"] = string.Empty;
          newRow["GroupID"] = DBNull.Value;
          dt.Rows.InsertAt(newRow, 0);

          this.group.DataSource = dt;
          this.group.DataTextField = "Name";
          this.group.DataValueField = "GroupID";
          this.group.DataBind();
        }

        using (DataTable dt = DB.rank_list(PageContext.PageBoardID, null))
        {
          DataRow newRow = dt.NewRow();
          newRow["Name"] = string.Empty;
          newRow["RankID"] = DBNull.Value;
          dt.Rows.InsertAt(newRow, 0);

          this.rank.DataSource = dt;
          this.rank.DataTextField = "Name";
          this.rank.DataValueField = "RankID";
          this.rank.DataBind();
        }

        this.PagerTop.PageSize = 25;
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      var pds = new PagedDataSource();
      pds.AllowPaging = true;
      pds.PageSize = this.PagerTop.PageSize;

      using (
        DataTable dt = DB.user_list(
          PageContext.PageBoardID, 
          null, 
          null, 
          this.group.SelectedIndex <= 0 ? null : this.group.SelectedValue, 
          this.rank.SelectedIndex <= 0 ? null : this.rank.SelectedValue))
      {
        using (DataView dv = dt.DefaultView)
        {
          if (this.name.Text.Trim().Length > 0 || (this.Email.Text.Trim().Length > 0))
          {
            dv.RowFilter = string.Format("Name like '%{0}%' and Email like '%{1}%'", this.name.Text.Trim(), this.Email.Text.Trim());
          }

          this.PagerTop.Count = dv.Count;
          pds.DataSource = dv;

          pds.CurrentPageIndex = this.PagerTop.CurrentPageIndex;
          if (pds.CurrentPageIndex >= pds.PageCount)
          {
            pds.CurrentPageIndex = pds.PageCount - 1;
          }

          this.UserList.DataSource = pds;
          this.UserList.DataBind();
        }
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
    public void Delete_Load(object sender, EventArgs e)
    {
      ((LinkButton) sender).Attributes["onclick"] = "return confirm('Delete this user?')";
    }

    /// <summary>
    /// The user list_ item command.
    /// </summary>
    /// <param name="source">
    /// The source.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void UserList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
      switch (e.CommandName)
      {
        case "edit":
          YafBuildLink.Redirect(ForumPages.admin_edituser, "u={0}", e.CommandArgument);
          break;
        case "delete":
          if (PageContext.PageUserID == int.Parse(e.CommandArgument.ToString()))
          {
            PageContext.AddLoadMessage("You can't delete yourself.");
            return;
          }

          string userName = string.Empty;
          using (DataTable dt = DB.user_list(PageContext.PageBoardID, e.CommandArgument, DBNull.Value))
          {
            foreach (DataRow row in dt.Rows)
            {
              userName = (string) row["Name"];
              if (SqlDataLayerConverter.VerifyInt32(row["IsGuest"]) > 0)
              {
                PageContext.AddLoadMessage("You can't delete the Guest.");
                return;
              }

              if ((row["IsAdmin"] != DBNull.Value && SqlDataLayerConverter.VerifyInt32(row["IsAdmin"]) > 0) ||
                  (row["IsHostAdmin"] != DBNull.Value && Convert.ToInt32(row["IsHostAdmin"]) > 0))
              {
                PageContext.AddLoadMessage("You can't delete the Admin.");
                return;
              }
            }
          }

          UserMembershipHelper.DeleteUser(Convert.ToInt32(e.CommandArgument));
          BindData();
          break;
      }
    }

    /// <summary>
    /// The new user_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void NewUser_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_reguser);
    }


    /// <summary>
    /// The search_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public void search_Click(object sender, EventArgs e)
    {
      BindData();
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

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PagerTop_PageChange(object sender, EventArgs e)
    {
      // rebind
      BindData();
    }

    /// <summary>
    /// The sync users_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SyncUsers_Click(object sender, EventArgs e)
    {
      // start...
      SyncMembershipUsersTask.Start(PageContext.PageBoardID);

      // enable timer...
      this.UpdateStatusTimer.Enabled = true;

      // show blocking ui...
      PageContext.PageElements.RegisterJsBlockStartup("BlockUIExecuteJs", JavaScriptBlocks.BlockUIExecuteJs("SyncUsersMessage"));
    }

    /// <summary>
    /// The update status timer_ tick.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void UpdateStatusTimer_Tick(object sender, EventArgs e)
    {
      // see if the migration is done....
      if (YafTaskModule.Current.IsTaskRunning(SyncMembershipUsersTask.TaskName))
      {
        // continue...
        return;
      }

      this.UpdateStatusTimer.Enabled = false;

      // done here...
      YafBuildLink.Redirect(ForumPages.admin_users);
    }
  }
}