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
  using System.Text.RegularExpressions;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Core.BBCode;
  using YAF.Classes.Data;
  using YAF.Classes.UI;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for smilies_import.
  /// </summary>
  public partial class smilies_import : AdminPage
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
        this.PageLinks.AddLink("Smilies Import", string.Empty);

        BindData();
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
        DataRow dr = dt.NewRow();
        dr["FileID"] = 0;
        dr["FileName"] = "Select File (*.pak)";
        dt.Rows.Add(dr);

        var dir = new DirectoryInfo(Request.MapPath(String.Format("{0}{1}", YafForumInfo.ForumServerFileRoot, YafBoardFolders.Current.Emoticons)));
        FileInfo[] files = dir.GetFiles("*.pak");
        long nFileID = 1;
        foreach (FileInfo file in files)
        {
          dr = dt.NewRow();
          dr["FileID"] = nFileID++;
          dr["FileName"] = file.Name;
          dt.Rows.Add(dr);
        }

        this.File.DataSource = dt;
        this.File.DataValueField = "FileID";
        this.File.DataTextField = "FileName";
      }

      DataBind();
    }

    /// <summary>
    /// The import_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void import_Click(object sender, EventArgs e)
    {
      if (long.Parse(this.File.SelectedValue) < 1)
      {
        PageContext.AddLoadMessage("You must select a .pak file to import.");
        return;
      }

      string fileName = Request.MapPath(String.Format("{0}{1}/{2}", YafForumInfo.ForumClientFileRoot, YafBoardFolders.Current.Emoticons, this.File.SelectedItem.Text));
      string split = Regex.Escape("=+:");

      using (var file = new StreamReader(fileName))
      {
        int sortOrder = 1;

        // Delete existing smilies?
        if (this.DeleteExisting.Checked)
        {
          DB.smiley_delete(null);
        }
        else
        {
          // Get max value of SortOrder
          using (DataView dv = DB.smiley_listunique(PageContext.PageBoardID).DefaultView)
          {
            dv.Sort = "SortOrder desc";
            if (dv.Count > 0)
            {
              DataRowView dr = dv[0];
              if (dr != null)
              {
                object o = dr["SortOrder"];
                if (int.TryParse(o.ToString(), out sortOrder))
                {
                  sortOrder++;
                }
              }
            }
          }
        }

        do
        {
          string line = file.ReadLine();
          if (line == null)
          {
            break;
          }

          string[] lineSplit = Regex.Split(line, split, RegexOptions.None);

          if (lineSplit.Length == 3)
          {
            DB.smiley_save(null, PageContext.PageBoardID, lineSplit[2], lineSplit[0], lineSplit[1], sortOrder, 0);
            sortOrder++;
          }
        }
 while (true);

        file.Close();

        // invalidate the cache...
        PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.Smilies));
        ReplaceRulesCreator.ClearCache();
      }

      YafBuildLink.Redirect(ForumPages.admin_smilies);
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
    private void cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_smilies);
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
      import.Click += new EventHandler(import_Click);
      cancel.Click += new EventHandler(cancel_Click);

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