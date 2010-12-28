/* Yet Another Forum.NET
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

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The replacewords_import.
  /// </summary>
  public partial class replacewords_import : AdminPage
  {
    #region Methods

    /// <summary>
    /// The cancel_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_OnClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_replacewords);
    }

    /// <summary>
    /// The import_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Import_OnClick([NotNull] object sender, [NotNull] EventArgs e)
    {
      // import selected file (if it's the proper format)...
      if (this.importFile.PostedFile.ContentType == "text/xml")
      {
        try
        {
          // import replace words...
          var dsReplaceWords = new DataSet();
          dsReplaceWords.ReadXml(this.importFile.PostedFile.InputStream);

          if (dsReplaceWords.Tables["YafReplaceWords"] != null &&
              dsReplaceWords.Tables["YafReplaceWords"].Columns["badword"] != null &&
              dsReplaceWords.Tables["YafReplaceWords"].Columns["goodword"] != null)
          {
            int importedCount = 0;

            DataTable replaceWordsList = DB.replace_words_list(this.PageContext.PageBoardID, null);

            // import any extensions that don't exist...
            foreach (DataRow row in dsReplaceWords.Tables["YafReplaceWords"].Rows)
            {
              if (
                replaceWordsList.Select(
                  "badword = '{0}' AND goodword = '{1}'".FormatWith(row["badword"], row["goodword"])).Length == 0)
              {
                // add this...
                DB.replace_words_save(this.PageContext.PageBoardID, null, row["badword"], row["goodword"]);
                importedCount++;
              }
            }

            if (importedCount > 0)
            {
              this.PageContext.LoadMessage.AddSession(
                "{0} new replacement word(s) were imported successfully.".FormatWith(importedCount));
            }
            else
            {
              this.PageContext.LoadMessage.AddSession(
                "Nothing imported: no new replacement words were found in the upload.".FormatWith(importedCount));
            }

            YafBuildLink.Redirect(ForumPages.admin_replacewords);
          }
          else
          {
            this.PageContext.AddLoadMessage("Failed to import: Import file format is different than expected.");
          }
        }
        catch (Exception x)
        {
          this.PageContext.AddLoadMessage("Failed to import: " + x.Message);
        }
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
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Import Replace Words", string.Empty);
      }
    }

    #endregion
  }
}