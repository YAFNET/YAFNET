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
namespace YAF.Pages
{
  // YAF.Pages

  #region Using

  using System;
  using System.Data;
  using System.Linq;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  ///   Summary description for activeusers.
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
    ///   The page_ load.
    /// </summary>
    /// <param name = "sender">
    ///   The sender.
    /// </param>
    /// <param name = "e">
    ///   The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!this.IsPostBack)
      {
        this.PageLinks.AddLink(this.PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);

        if (this.Request.QueryString.GetFirstOrDefault("v").IsSet() &&
            YafServices.Permissions.Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions))
        {
          int mode;
          if (Int32.TryParse(this.Request.QueryString.GetFirstOrDefault("v"), out mode))
          {
            DataTable activeUsers;
            switch (mode)
            {
              case 0:
                // Show all users
                activeUsers = this.GetActiveUsersData(true,this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList);
                this.RemoveHiddenUsers(ref activeUsers);
                this.UserList.DataSource = activeUsers;
                this.DataBind();
                break;
              case 1:
                // Show members
                activeUsers = this.GetActiveUsersData(false,false);
                this.RemoveHiddenUsers(ref activeUsers);
                this.UserList.DataSource = activeUsers;
                this.DataBind();
                break;
              case 2:
                // Show guests
                activeUsers = this.GetActiveUsersData(true, this.PageContext.BoardSettings.ShowCrawlersInActiveList);
                this.RemoveAllButGusts(ref activeUsers);
                this.UserList.DataSource = activeUsers;
                this.DataBind();
                break;
              case 3:
                // Show hidden                         
                if (this.PageContext.IsAdmin)
                {
                  activeUsers = this.GetActiveUsersData(false,false);
                  this.RemoveAllButHiddenUsers(ref activeUsers);
                  this.UserList.DataSource = activeUsers;
                  this.DataBind();
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
          }
        }
        else
        {
          YafBuildLink.AccessDenied();
        }
      }
    }

    protected void btnReturn_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.forum);
    }

    private DataTable GetActiveUsersData(bool showGuests, bool showCrawlers)
    {
      // vzrus: Here should not be a common cache as it's should be individual for each user because of ActiveLocationcontrol to hide unavailable places.        
      DataTable activeUsers = DB.active_list_user(
        this.PageContext.PageBoardID,
        this.PageContext.PageUserID,
        showGuests,
        showCrawlers,
        this.PageContext.BoardSettings.ActiveListTime,
        this.PageContext.BoardSettings.UseStyledNicks);
      // Set colorOnly parameter to false, as we get active users style from database        
      if (this.PageContext.BoardSettings.UseStyledNicks)
      {
        new StyleTransform(this.PageContext.Theme).DecodeStyleByTable(ref activeUsers, false);
      }
      return activeUsers;
    }

    private void RemoveAllButGusts(ref DataTable activeUsers)
    {
      if (activeUsers.Rows.Count <= 0)
      {
        return;
      }
      // remove non-guest users...
      foreach (DataRow row in activeUsers.Rows.Cast<DataRow>().Where(row => !Convert.ToBoolean(row["IsGuest"])))
      {
        // remove this active user...
        row.Delete();
      }
      activeUsers.AcceptChanges();
    }

    private void RemoveAllButHiddenUsers(ref DataTable activeUsers)
    {
      if (activeUsers.Rows.Count <= 0)
      {
        return;
      }
      // remove hidden users...
      foreach (DataRow row in activeUsers.Rows)
      {
        if (!Convert.ToBoolean(row["IsHidden"]) && this.PageContext.PageUserID != Convert.ToInt32(row["UserID"]))
        {
          // remove this active user...
          row.Delete();
        }
      }
      activeUsers.AcceptChanges();
    }

    private void RemoveHiddenUsers(ref DataTable activeUsers)
    {
      if (activeUsers.Rows.Count <= 0)
      {
        return;
      }
      // remove hidden users...
      foreach (DataRow row in activeUsers.Rows)
      {
        if (Convert.ToBoolean(row["IsHidden"]) && !this.PageContext.IsAdmin &&
            this.PageContext.PageUserID != Convert.ToInt32(row["UserID"]))
        {
          // remove this active user...
          row.Delete();
        }
      }
      activeUsers.AcceptChanges();
    }

    #endregion
  }
}