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
  /// Summary description for mail.
  /// </summary>
  public partial class mail : AdminPage
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
        this.PageLinks.AddLink("Mail", string.Empty);

        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.ToList.DataSource = DB.group_list(PageContext.PageBoardID, null);
      DataBind();

      var item = new ListItem("All Users", "0");
      this.ToList.Items.Insert(0, item);
    }

    /// <summary>
    /// The send_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Send_Click(object sender, EventArgs e)
    {
      object GroupID = null;
      if (this.ToList.SelectedItem.Value != "0")
      {
        GroupID = this.ToList.SelectedValue;
      }

      string subject = this.Subject.Text.Trim();

      if (String.IsNullOrEmpty(subject))
      {
        PageContext.AddLoadMessage("Subject is Required");
      }
      else
      {
        using (DataTable dt = DB.user_emails(PageContext.PageBoardID, GroupID))
        {
          foreach (DataRow row in dt.Rows)
          {
            // Wes - Changed to use queue to improve scalability
            YafServices.SendMail.Queue(PageContext.BoardSettings.ForumEmail, (string) row["Email"], this.Subject.Text.Trim(), this.Body.Text.Trim());
          }
        }

        this.Subject.Text = string.Empty;
        this.Body.Text = string.Empty;
        PageContext.AddLoadMessage("Mails queued.");
      }
    }
  }
}