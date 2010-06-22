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
  public partial class editnntpserver : AdminPage
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
        this.PageLinks.AddLink("NNTP Servers", string.Empty);

        BindData();
        if (Request.QueryString.GetFirstOrDefault("s") != null)
        {
          using (DataTable dt = DB.nntpserver_list(PageContext.PageBoardID, Request.QueryString.GetFirstOrDefault("s")))
          {
            DataRow row = dt.Rows[0];
            this.Name.Text = row["Name"].ToString();
            this.Address.Text = row["Address"].ToString();
            this.Port.Text = row["Port"].ToString();
            this.UserName.Text = row["UserName"].ToString();
            this.UserPass.Text = row["UserPass"].ToString();
          }
        }
        else
        {
          this.Port.Text = "119";
        }
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
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
      YafBuildLink.Redirect(ForumPages.admin_nntpservers);
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
      if (this.Name.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("Missing server name.");
        return;
      }

      if (this.Address.Text.Trim().Length == 0)
      {
        PageContext.AddLoadMessage("Missing server address.");
        return;
      }

      object nntpServerID = null;
      if (Request.QueryString.GetFirstOrDefault("s") != null)
      {
        nntpServerID = Request.QueryString.GetFirstOrDefault("s");
      }

      DB.nntpserver_save(
        nntpServerID, 
        PageContext.PageBoardID, 
        this.Name.Text, 
        this.Address.Text, 
        this.Port.Text.Length > 0 ? this.Port.Text : null, 
        this.UserName.Text.Length > 0 ? this.UserName.Text : null, 
        this.UserPass.Text.Length > 0 ? this.UserPass.Text : null);
      YafBuildLink.Redirect(ForumPages.admin_nntpservers);
    }
  }
}