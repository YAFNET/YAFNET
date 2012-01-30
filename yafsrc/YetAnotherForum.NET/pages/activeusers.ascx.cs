/* Yet Another Forum.NET
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

namespace YAF.Pages
{
  #region Using

  using System;
  using System.Data;
  using System.Linq;
  using System.Web;
  using System.Web.UI.WebControls;

  using YAF.Classes.Data;
  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for activeusers.
  /// </summary>
  public partial class activeusers : ForumPage
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "activeusers" /> class.
    /// </summary>
    public activeusers()
      : base("ACTIVEUSERS")
    {
    }

    #endregion

    #region Methods

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
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v").IsSet() &&
            this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions))
        {
          this.BindData();
        }
        else
        {
          YafBuildLink.AccessDenied();
        }
      }
    }

    /// <summary>
    /// The pager_ page change.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Pager_PageChange([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.BindData();
    }

    /// <summary>
    /// Button click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The Event args
    /// </param>
    protected void btnReturn_Click([NotNull] object sender, [NotNull] EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.forum);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
      int mode = 0;
      if (Int32.TryParse(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v"), out mode))
      {
        DataView activeUsers = null;
        switch (mode)
        {
          case 0:

            // Show all users
            activeUsers =
              this.GetActiveUsersData(true, this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList).DefaultView;
            if (activeUsers != null)
            {
              this.RemoveHiddenUsers(ref activeUsers);
            }

            break;
          case 1:

            // Show members
            activeUsers = this.GetActiveUsersData(false, false).DefaultView;
            if (activeUsers != null)
            {
              this.RemoveHiddenUsers(ref activeUsers);
            }

            break;
          case 2:

            // Show guests
            activeUsers =
              this.GetActiveUsersData(true, this.PageContext.BoardSettings.ShowCrawlersInActiveList).DefaultView;
            if (activeUsers != null)
            {
              this.RemoveAllButGusts(ref activeUsers);
            }

            break;
          case 3:

            // Show hidden                         
            if (this.PageContext.IsAdmin)
            {
              activeUsers = this.GetActiveUsersData(false, false).DefaultView;
              if (activeUsers != null)
              {
                this.RemoveAllButHiddenUsers(ref activeUsers);
              }
            }
            else
            {
              YafBuildLink.AccessDenied();
            }

            break;
          default:
            YafBuildLink.AccessDenied();
            break;
        }

        if (activeUsers != null && activeUsers.Count > 0)
        {
          this.Pager.PageSize = 20;

          var pds = new PagedDataSource { AllowPaging = true, PageSize = this.Pager.PageSize };
          this.Pager.Count = activeUsers.Count;
          pds.DataSource = activeUsers;
          pds.CurrentPageIndex = this.Pager.CurrentPageIndex;

          if (pds.CurrentPageIndex >= pds.PageCount)
          {
            pds.CurrentPageIndex = pds.PageCount - 1;
          }

          this.UserList.DataSource = pds;
          this.DataBind();
        }
      }
    }

    /// <summary>
    /// Gets active user data Table data for a page user
    /// </summary>
    /// <param name="showGuests">
    /// The show guests.
    /// </param>
    /// <param name="showCrawlers">
    /// The show crawlers.
    /// </param>
    /// <returns>
    /// A DataTable
    /// </returns>
    private DataTable GetActiveUsersData(bool showGuests, bool showCrawlers)
    {
      // vzrus: Here should not be a common cache as it's should be individual for each user because of ActiveLocationcontrol to hide unavailable places.        
      DataTable activeUsers = LegacyDb.active_list_user(
        this.PageContext.PageBoardID, 
        this.PageContext.PageUserID, 
        showGuests, 
        showCrawlers, 
        this.PageContext.BoardSettings.ActiveListTime, 
        this.PageContext.BoardSettings.UseStyledNicks);

      // Set colorOnly parameter to false, as we get active users style from database        
      if (this.PageContext.BoardSettings.UseStyledNicks)
      {
        this.Get<IStyleTransform>().DecodeStyleByTable(ref activeUsers, false);
      }

      return activeUsers;
    }

    /// <summary>
    /// Removes from the DataView all users but guests.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private void RemoveAllButGusts([NotNull] ref DataView activeUsers)
    {
      if (activeUsers.Count <= 0)
      {
        return;
      }

      // remove non-guest users...
      foreach (DataRowView row in activeUsers.Cast<DataRowView>().Where(row => !Convert.ToBoolean(row["IsGuest"])))
      {
        // remove this active user...
        row.Delete();
      }
    }

    /// <summary>
    /// Removes from the DataView all users but hidden.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private void RemoveAllButHiddenUsers([NotNull] ref DataView activeUsers)
    {
      if (activeUsers.Count <= 0)
      {
        return;
      }

      // remove hidden users...
      foreach (DataRowView row in activeUsers)
      {
        if (!Convert.ToBoolean(row["IsHidden"]) && this.PageContext.PageUserID != Convert.ToInt32(row["UserID"]))
        {
          // remove this active user...
          row.Delete();
        }
      }
    }

    /// <summary>
    /// Removes hidden users.
    /// </summary>
    /// <param name="activeUsers">
    /// The active users.
    /// </param>
    private void RemoveHiddenUsers([NotNull] ref DataView activeUsers)
    {
      if (activeUsers.Count <= 0)
      {
        return;
      }

      // remove hidden users...
      foreach (DataRowView row in activeUsers)
      {
        if (Convert.ToBoolean(row["IsHidden"]) && !this.PageContext.IsAdmin &&
            this.PageContext.PageUserID != Convert.ToInt32(row["UserID"]))
        {
          // remove this active user...
          row.Delete();
        }
      }
    }

    #endregion
  }
}