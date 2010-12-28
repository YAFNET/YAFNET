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

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
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
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Banned IP Addresses", YafBuildLink.GetLink(ForumPages.admin_bannedip));

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
      string[] ipParts = this.mask.Text.Trim().Split('.');

      // do some validation...
      string ipError = string.Empty;

      if (ipParts.Length != 4)
      {
        ipError += "Invalid IP address.";
      }

      // see if they are numbers...
      ulong number;

      foreach (string ip in ipParts)
      {
        if (!ulong.TryParse(ip, out number))
        {
          if (ip.Trim() != "*")
          {
            if (ip.Trim().Length == 0)
            {
              ipError +=
                "\r\nOne of the IP section does not have a value. Valid values are 0-255 or \"*\" for a wildcard.";
            }
            else
            {
              ipError += "\r\n\"{0}\" is not a valid IP section value.".FormatWith(ip);
            }

            break;
          }
        }
        else
        {
          // try parse succeeded... verify number amount...
          if (number > 255)
          {
            ipError += "\r\n\"{0}\" is not a valid IP section value (must be less then 255).".FormatWith(ip);
          }
        }
      }

      // show error(s) if not valid...
      if (ipError.IsSet())
      {
        this.PageContext.AddLoadMessage(ipError);
        return;
      }

      DB.bannedip_save(
        this.Request.QueryString.GetFirstOrDefault("i"), 
        this.PageContext.PageBoardID, 
        this.mask.Text.Trim(), 
        this.BanReason.Text.Trim(), 
        this.PageContext.PageUserID);

      // clear cache of banned IPs for this board
      this.PageContext.Cache.Remove(YafCache.GetBoardCacheKey(Constants.Cache.BannedIP));

      // go back to banned IP's administration page
      YafBuildLink.Redirect(ForumPages.admin_bannedip);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      if (this.Request.QueryString.GetFirstOrDefault("i") != null)
      {
        DataRow row =
          DB.bannedip_list(this.PageContext.PageBoardID, this.Request.QueryString.GetFirstOrDefault("i")).Rows[0];
        this.mask.Text = (string)row["Mask"];
      }
    }

    #endregion
  }
}