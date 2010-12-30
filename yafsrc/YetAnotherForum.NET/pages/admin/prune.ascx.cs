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
  #region Using

  using System;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Tasks;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for prune.
  /// </summary>
  public partial class prune : AdminPage
  {
    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      commit.Click += this.commit_Click;

      // CODEGEN: This call is required by the ASP.NET Web Form Designer.
      InitializeComponent();
      base.OnInit(e);
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), string.Empty);
        this.PageLinks.AddLink("Prune", string.Empty);

        this.days.Text = "60";
        this.BindData();
      }

      this.lblPruneInfo.Text = string.Empty;

      if (YafTaskModule.Current.IsTaskRunning(PruneTopicTask.TaskName))
      {
        this.lblPruneInfo.Text =
          "NOTE: Prune Task is currently RUNNING. Cannot start a new prune task until it's finished.";
        this.commit.Enabled = false;
      }
    }

    /// <summary>
    /// The prune button_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PruneButton_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      ((Button)sender).Attributes["onclick"] =
        "return confirm('{0}')".FormatWith("Do you really want to prune topics? This process is irreversible.");
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.forumlist.DataSource = LegacyDb.forum_listread(
        this.PageContext.PageBoardID, this.PageContext.PageUserID, null, null, false);
      this.forumlist.DataValueField = "ForumID";
      this.forumlist.DataTextField = "Forum";
      this.DataBind();
      this.forumlist.Items.Insert(0, new ListItem("All Forums", "0"));
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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
    private void commit_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      PruneTopicTask.Start(
        this.PageContext.PageBoardID, 
        Convert.ToInt32(this.forumlist.SelectedValue), 
        Convert.ToInt32(this.days.Text), 
        this.permDeleteChkBox.Checked);
      this.PageContext.AddLoadMessage("Prune Task Scheduled");
    }

    #endregion
  }
}