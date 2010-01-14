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
  using System.IO;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for editgroup.
  /// </summary>
  public partial class editrank : AdminPage
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
        if (Request.QueryString["r"] != null)
        {
          using (DataTable dt = DB.rank_list(PageContext.PageBoardID, Request.QueryString["r"]))
          {
            DataRow row = dt.Rows[0];
            var flags = new RankFlags(row["Flags"]);
            this.Name.Text = (string) row["Name"];
            this.IsStart.Checked = flags.IsStart;
            this.IsLadder.Checked = flags.IsLadder;
            this.MinPosts.Text = row["MinPosts"].ToString();
            this.PMLimit.Text = row["PMLimit"].ToString();
            this.Style.Text = row["Style"].ToString();
            this.RankPriority.Text = row["SortOrder"].ToString();

            ListItem item = this.RankImage.Items.FindByText(row["RankImage"].ToString());
            if (item != null)
            {
              item.Selected = true;
              this.Preview.Src = String.Format("{0}{1}/{2}", YafForumInfo.ForumRoot, YafBoardFolders.Current.Ranks, row["RankImage"]); // path corrected
            }
            else
            {
              this.Preview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
            }
          }
        }
        else
        {
          this.Preview.Src = String.Format("{0}images/spacer.gif", YafForumInfo.ForumRoot);
        }
      }

      this.RankImage.Attributes["onchange"] = String.Format(
        "getElementById('{2}_ctl01_Preview').src='{0}{1}/' + this.value", YafForumInfo.ForumRoot, YafBoardFolders.Current.Ranks, Parent.ID);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      using (var dt = new DataTable("Files"))
      {
        dt.Columns.Add("FileID", typeof(long));
        dt.Columns.Add("FileName", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        DataRow dr = dt.NewRow();
        dr["FileID"] = 0;
        dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
        dr["Description"] = "Select Rank Image";
        dt.Rows.Add(dr);

        var dir = new DirectoryInfo(Request.MapPath(String.Format("{0}{1}", YafForumInfo.ForumFileRoot, YafBoardFolders.Current.Ranks)));
        FileInfo[] files = dir.GetFiles("*.*");
        long nFileID = 1;
        foreach (FileInfo file in files)
        {
          string sExt = file.Extension.ToLower();
          if (sExt != ".png" && sExt != ".gif" && sExt != ".jpg")
          {
            continue;
          }

          dr = dt.NewRow();
          dr["FileID"] = nFileID++;
          dr["FileName"] = file.Name;
          dr["Description"] = file.Name;
          dt.Rows.Add(dr);
        }

        this.RankImage.DataSource = dt;
        this.RankImage.DataValueField = "FileName";
        this.RankImage.DataTextField = "Description";
      }

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
      YafBuildLink.Redirect(ForumPages.admin_ranks);
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
      if (!ValidationHelper.IsValidInt(this.PMLimit.Text.Trim()))
      {
        PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
        return;
      }

      if (!ValidationHelper.IsValidInt(this.RankPriority.Text.Trim()))
      {
        PageContext.AddLoadMessage("Rank Priority should be small integer.");
        return;
      }

      // Group
      int RankID = 0;
      if (Request.QueryString["r"] != null)
      {
        RankID = int.Parse(Request.QueryString["r"]);
      }

      object rankImage = null;
      if (this.RankImage.SelectedIndex > 0)
      {
        rankImage = this.RankImage.SelectedValue;
      }

      DB.rank_save(
        RankID, 
        PageContext.PageBoardID, 
        this.Name.Text, 
        this.IsStart.Checked, 
        this.IsLadder.Checked, 
        this.MinPosts.Text, 
        rankImage, 
        Convert.ToInt32(this.PMLimit.Text), 
        this.Style.Text.Trim(), 
        this.RankPriority.Text.Trim());

      YafBuildLink.Redirect(ForumPages.admin_ranks);
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