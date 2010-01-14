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
  /// Summary description for WebForm1.
  /// </summary>
  public partial class editaccessmask : AdminPage
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
        if (Request.QueryString["i"] != null)
        {
          using (DataTable dt = DB.accessmask_list(PageContext.PageBoardID, Request.QueryString["i"]))
          {
            DataRow row = dt.Rows[0];
            var flags = new AccessFlags(row["Flags"]);
            this.Name.Text = (string) row["Name"];
            this.ReadAccess.Checked = flags.ReadAccess;
            this.PostAccess.Checked = flags.PostAccess;
            this.ReplyAccess.Checked = flags.ReplyAccess;
            this.PriorityAccess.Checked = flags.PriorityAccess;
            this.PollAccess.Checked = flags.PollAccess;
            this.VoteAccess.Checked = flags.VoteAccess;
            this.ModeratorAccess.Checked = flags.ModeratorAccess;
            this.EditAccess.Checked = flags.EditAccess;
            this.DeleteAccess.Checked = flags.DeleteAccess;
            this.UploadAccess.Checked = flags.UploadAccess;
            this.DownloadAccess.Checked = flags.DownloadAccess;
          }
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
      // Forum
      object accessMaskID = null;
      if (Request.QueryString["i"] != null)
      {
        accessMaskID = Request.QueryString["i"];
      }

      DB.accessmask_save(
        accessMaskID, 
        PageContext.PageBoardID, 
        this.Name.Text, 
        this.ReadAccess.Checked, 
        this.PostAccess.Checked, 
        this.ReplyAccess.Checked, 
        this.PriorityAccess.Checked, 
        this.PollAccess.Checked, 
        this.VoteAccess.Checked, 
        this.ModeratorAccess.Checked, 
        this.EditAccess.Checked, 
        this.DeleteAccess.Checked, 
        this.UploadAccess.Checked, 
        this.DownloadAccess.Checked,
        this.SortOrder.Text);

      PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumModerators));

      YafBuildLink.Redirect(ForumPages.admin_accessmasks);
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
      YafBuildLink.Redirect(ForumPages.admin_accessmasks);
    }
  }
}