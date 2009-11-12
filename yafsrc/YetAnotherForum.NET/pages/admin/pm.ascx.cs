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
  /// Summary description for prune.
  /// </summary>
  public partial class pm : AdminPage
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
        this.PageLinks.AddLink("PM Maintenance", string.Empty);

        this.Days1.Text = "60";
        this.Days2.Text = "180";
        BindData();
      }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      using (DataTable dt = DB.pmessage_info())
      {
        this.Count.Text = dt.Rows[0]["NumTotal"].ToString();
      }
    }

    /// <summary>
    /// The commit_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void commit_Click(object sender, EventArgs e)
    {
      DB.pmessage_prune(this.Days1.Text, this.Days2.Text);
      BindData();
    }

    /// <summary>
    /// The delete button_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void DeleteButton_Load(object sender, EventArgs e)
    {
      ((Button) sender).Attributes["onclick"] = String.Format(
        "return confirm('{0}')", "Do you really want to delete private messages? This process is irreversible.");
    }

    #region Web Form Designer generated code

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit(EventArgs e)
    {
      commit.Click += new EventHandler(commit_Click);

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
    }

    #endregion
  }
}