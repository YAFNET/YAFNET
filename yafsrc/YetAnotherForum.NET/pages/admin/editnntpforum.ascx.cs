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
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for editgroup.
  /// </summary>
  public partial class editnntpforum : AdminPage
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
        this.PageLinks.AddLink("NNTP Forums", string.Empty);

        BindData();
        if (Request.QueryString["s"] != null)
        {
          using (DataTable dt = DB.nntpforum_list(PageContext.PageBoardID, null, Request.QueryString["s"], DBNull.Value))
          {
            DataRow row = dt.Rows[0];
            this.NntpServerID.Items.FindByValue(row["NntpServerID"].ToString()).Selected = true;
            this.GroupName.Text = row["GroupName"].ToString();
            this.ForumID.Items.FindByValue(row["ForumID"].ToString()).Selected = true;
            this.Active.Checked = (bool) row["Active"];
          }
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.NntpServerID.DataSource = DB.nntpserver_list(PageContext.PageBoardID, null);
      this.NntpServerID.DataValueField = "NntpServerID";
      this.NntpServerID.DataTextField = "Name";
      this.ForumID.DataSource = DB.forum_listall_sorted(PageContext.PageBoardID, PageContext.PageUserID);
      this.ForumID.DataValueField = "ForumID";
      this.ForumID.DataTextField = "Title";
      DataBind();
    }

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_nntpforums);
    }

    /// <summary>
    /// The save_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Save_Click(object sender, EventArgs e)
    {
      object nntpForumID = null;
      if (Request.QueryString["s"] != null)
      {
        nntpForumID = Request.QueryString["s"];
      }
      if (Convert.ToInt32(this.ForumID.SelectedValue) <= 0)
      {
          PageContext.AddLoadMessage("You must select a forum to save NNTP messages.");
          return;
      }
      DB.nntpforum_save(nntpForumID, this.NntpServerID.SelectedValue, this.GroupName.Text, this.ForumID.SelectedValue, this.Active.Checked);
      YafBuildLink.Redirect(ForumPages.admin_nntpforums);
    }
  }
}