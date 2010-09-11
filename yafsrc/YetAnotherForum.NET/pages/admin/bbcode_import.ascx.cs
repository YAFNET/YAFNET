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
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data.Import;
  using YAF.Classes.Utils;

  /// <summary>
  /// The bbcode_import.
  /// </summary>
  public partial class bbcode_import : AdminPage
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
        this.PageLinks.AddLink("Import Custom YafBBCode", string.Empty);
      }
    }

    /// <summary>
    /// The cancel_ on click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_OnClick(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_bbcode);
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
    protected void Import_OnClick(object sender, EventArgs e)
    {
      // import selected file (if it's the proper format)...
      if (this.importFile.PostedFile.ContentType == "text/xml")
      {
        try
        {
          int importedCount = DataImport.BBCodeExtensionImport(PageContext.PageBoardID, this.importFile.PostedFile.InputStream);

          if (importedCount > 0)
          {
            PageContext.LoadMessage.AddSession("{0} new custom bbcode(s) imported successfully.".FormatWith(importedCount));
          }
          else
          {
            PageContext.LoadMessage.AddSession("Nothing imported: no new custom bbcode was found in the upload.".FormatWith(importedCount));
          }

          YafBuildLink.Redirect(ForumPages.admin_bbcode);
        }
        catch (Exception x)
        {
          PageContext.AddLoadMessage("Failed to import: " + x.Message);
        }
      }
    }
  }
}