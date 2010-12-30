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
  using System.Data;
  using System.IO;
  using System.Web.UI.WebControls;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers;

  #endregion

  /// <summary>
  /// Summary description for editcategory.
  /// </summary>
  public partial class editcategory : AdminPage
  {
    #region Methods

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_forums);
    }

    /// <summary>
    /// The create images data table.
    /// </summary>
    protected void CreateImagesDataTable()
    {
      using (var dt = new DataTable("Files"))
      {
        dt.Columns.Add("FileID", typeof(long));
        dt.Columns.Add("FileName", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        DataRow dr = dt.NewRow();
        dr["FileID"] = 0;
        dr["FileName"] = "../spacer.gif"; // use blank.gif for Description Entry
        dr["Description"] = "None";
        dt.Rows.Add(dr);

        var dir =
          new DirectoryInfo(
            this.Request.MapPath(
              "{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Categories)));
        if (dir.Exists)
        {
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
        }

        this.CategoryImages.DataSource = dt;
        this.CategoryImages.DataValueField = "FileName";
        this.CategoryImages.DataTextField = "Description";
        this.CategoryImages.DataBind();
      }
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
        this.PageLinks.AddLink("Forums", YafBuildLink.GetLink(ForumPages.admin_forums));
        this.PageLinks.AddLink("Category");

        // Populate Category Table
        this.CreateImagesDataTable();

        this.CategoryImages.Attributes["onchange"] =
          "getElementById('{1}').src='{0}{2}/' + this.value".FormatWith(
            YafForumInfo.ForumClientFileRoot, this.Preview.ClientID, YafBoardFolders.Current.Categories);

        this.Name.Style.Add("width", "100%");

        this.BindData();
      }
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
    protected void Save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      int CategoryID = 0;
      if (this.Request.QueryString.GetFirstOrDefault("c") != null)
      {
        CategoryID = int.Parse(this.Request.QueryString.GetFirstOrDefault("c"));
      }

      short sortOrder;
      string name = this.Name.Text.Trim();
      object categoryImage = null;

      if (this.CategoryImages.SelectedIndex > 0)
      {
        categoryImage = this.CategoryImages.SelectedValue;
      }

      if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
      {
        this.PageContext.AddLoadMessage("The Sort Order value should be a positive integer from 0 to 32767.");
        return;
      }

      if (!short.TryParse(this.SortOrder.Text.Trim(), out sortOrder))
      {
        // error...
        this.PageContext.AddLoadMessage("Invalid value entered for sort order: must enter a number.");
        return;
      }

      if (string.IsNullOrEmpty(name))
      {
        // error...
        this.PageContext.AddLoadMessage("Must enter a value for the category name field.");
        return;
      }

      // save category
      DB.category_save(this.PageContext.PageBoardID, CategoryID, name, categoryImage, sortOrder);

      // remove category cache...
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.ForumCategory));

      // redirect
      YafBuildLink.Redirect(ForumPages.admin_forums);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      this.Preview.Src = "{0}images/spacer.gif".FormatWith(YafForumInfo.ForumClientFileRoot);

      if (this.Request.QueryString.GetFirstOrDefault("c") != null)
      {
        using (
          DataTable dt = DB.category_list(this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("c"))
          )
        {
          DataRow row = dt.Rows[0];
          this.Name.Text = (string)row["Name"];
          this.SortOrder.Text = row["SortOrder"].ToString();
          this.CategoryNameTitle.Text = this.Name.Text;

          ListItem item = this.CategoryImages.Items.FindByText(row["CategoryImage"].ToString());
          if (item != null)
          {
            item.Selected = true;
            this.Preview.Src = "{0}{2}/{1}".FormatWith(
              YafForumInfo.ForumClientFileRoot, row["CategoryImage"], YafBoardFolders.Current.Categories);
              
              // path corrected
          }
        }
      }
    }

    #endregion
  }
}