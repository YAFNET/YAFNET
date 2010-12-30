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
  using System.Text.RegularExpressions;

  using YAF.Classes;
  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.BBCode;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for smilies_edit.
  /// </summary>
  public partial class smilies_edit : AdminPage
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
      save.Click += this.save_Click;
      cancel.Click += this.cancel_Click;

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
        this.PageLinks.AddLink("Smilies", string.Empty);

        this.BindData();
      }
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
        dr["Description"] = "Select Smiley Image";
        dt.Rows.Add(dr);

        var dir =
          new DirectoryInfo(
            this.Request.MapPath(
              "{0}{1}".FormatWith(YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Emoticons)));
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

        this.Icon.DataSource = dt;
        this.Icon.DataValueField = "FileName";
        this.Icon.DataTextField = "Description";
      }

      this.DataBind();

      if (this.Request["s"] != null)
      {
        using (
          DataTable dt = LegacyDb.smiley_list(this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("s")))
        {
          if (dt.Rows.Count > 0)
          {
            this.Code.Text = dt.Rows[0]["Code"].ToString();
            this.Emotion.Text = dt.Rows[0]["Emoticon"].ToString();
            if (this.Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()) != null)
            {
              this.Icon.Items.FindByText(dt.Rows[0]["Icon"].ToString()).Selected = true;
            }

            this.Preview.Src = "{0}{1}/{2}".FormatWith(
              YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, dt.Rows[0]["Icon"]);
            this.SortOrder.Text = dt.Rows[0]["SortOrder"].ToString(); // Ederon : 9/4/2007
          }
        }
      }
      else
      {
        this.Preview.Src = "{0}images/spacer.gif".FormatWith(YafForumInfo.ForumClientFileRoot);
      }

      this.Icon.Attributes["onchange"] =
        "getElementById('{2}').src='{0}{1}/' + this.value".FormatWith(
          YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, this.Preview.ClientID);
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    ///   the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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
    private void cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_smilies);
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
    private void save_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      string code = this.Code.Text.Trim();
      string emotion = this.Emotion.Text.Trim();
      string icon = this.Icon.SelectedItem.Text.Trim();
      int sortOrder;

      if (emotion.Length > 50)
      {
        this.PageContext.AddLoadMessage("An emotion description is too long.");
        return;
      }

      if (code.Length == 0)
      {
        this.PageContext.AddLoadMessage("Please enter the code to use for this emotion.");
        return;
      }

      if (code.Length > 10)
      {
        this.PageContext.AddLoadMessage("The code to use for this emotion should not be more then 10 symbols.");
        return;
      }

      if (!new Regex(@"\[.+\]").IsMatch(code))
      {
        this.PageContext.AddLoadMessage("Please enter the code to use for this emotion in square brackets.");
        return;
      }

      if (emotion.Length == 0)
      {
        this.PageContext.AddLoadMessage("Please enter an emotion for this icon.");
        return;
      }

      if (this.Icon.SelectedIndex < 1)
      {
        this.PageContext.AddLoadMessage("Please select an icon to use for this emotion.");
        return;
      }

      // Ederon 9/4/2007
      if (!int.TryParse(this.SortOrder.Text, out sortOrder) || sortOrder < 0 || sortOrder > 255)
      {
        this.PageContext.AddLoadMessage("Sort order must be number between 0 and 255.");
        return;
      }

      LegacyDb.smiley_save(
        this.Request.QueryString.GetFirstOrDefault("s"), this.PageContext.PageBoardID, code, icon, emotion, sortOrder, 0);

      // invalidate the cache...
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
      ReplaceRulesCreator.ClearCache();

      YafBuildLink.Redirect(ForumPages.admin_smilies);
    }

    #endregion
  }
}