/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
 * Copyright (C) 2006-2012 Jaben Cargman
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
  using System.Text;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for bannedip_edit.
  /// </summary>
  public partial class bannedip_edit : AdminPage
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
      YafBuildLink.Redirect(ForumPages.admin_bannedip);
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
        if (this.IsPostBack)
        {
            return;
        }

        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

        this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_bannedip));

        // current page label (no link)
        this.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"), string.Empty);

        this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
           this.GetText("ADMIN_ADMIN", "Administration"),
           this.GetText("ADMIN_BANNEDIP", "TITLE"),
           this.GetText("ADMIN_BANNEDIP_EDIT", "TITLE"));

        this.save.Text = this.GetText("COMMON", "SAVE");
        this.cancel.Text = this.GetText("COMMON", "CANCEL");

        this.BindData();
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
      string[] ipParts = this.mask.Text.Trim().Split('.');

      // do some validation...
      var ipError = new StringBuilder(); 

      if (ipParts.Length != 4)
      {
        ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_ADRESS"));
      }

      foreach (string ip in ipParts)
      {
          // see if they are numbers...
          ulong number;
          if (!ulong.TryParse(ip, out number))
        {
          if (ip.Trim() != "*")
          {
            if (ip.Trim().Length == 0)
            {
              ipError.AppendLine(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_VALUE"));
            }
            else
            {
              ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_SECTION"), ip);
            }

            break;
          }
        }
        else
        {
          // try parse succeeded... verify number amount...
          if (number > 255)
          {
              ipError.AppendFormat(this.GetText("ADMIN_BANNEDIP_EDIT", "INVALID_LESS"), ip);
          }
        }
      }

        // show error(s) if not valid...
      if (ipError.Length > 0)
      {
        this.PageContext.AddLoadMessage(ipError.ToString());
        return;
      }

      LegacyDb.bannedip_save(
        this.Request.QueryString.GetFirstOrDefault("i"), 
        this.PageContext.PageBoardID, 
        this.mask.Text.Trim(), 
        this.BanReason.Text.Trim(), 
        this.PageContext.PageUserID);

      // clear cache of banned IPs for this board
      this.Get<IDataCache>().Remove(Constants.Cache.BannedIP);

      // go back to banned IP's administration page
      YafBuildLink.Redirect(ForumPages.admin_bannedip);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        if (this.Request.QueryString.GetFirstOrDefault("i") == null)
        {
            return;
        }

        DataRow row =
            LegacyDb.bannedip_list(this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("i")).Rows[0];
        this.mask.Text = (string)row["Mask"];
    }

    #endregion
  }
}