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
  using System;
  using System.Data;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for activeusers.
  /// </summary>
  public partial class activeusers : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="activeusers"/> class.
    /// </summary>
    public activeusers()
      : base("ACTIVEUSERS")
    {
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
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink(GetText("TITLE"), string.Empty);
        DataTable activeUsers; 
        int mode;
        if (!String.IsNullOrEmpty(Request.QueryString.GetFirstOrDefault("v")) && YafServices.Permissions.Check(PageContext.BoardSettings.ActiveUsersViewPermissions))
        {
            if (Int32.TryParse(this.Request.QueryString.GetFirstOrDefault("v"), out mode))
            {
                
                switch (mode)
                {
                    case 0:
                        // Show all users
                        activeUsers = GetActiveUsersData(this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList);
                        RemoveHiddenUsers(ref activeUsers);
                        this.UserList.DataSource = activeUsers;
                        DataBind();
                        break;
                    case 1:
                        // Show members
                        activeUsers = GetActiveUsersData(false);
                        RemoveHiddenUsers(ref activeUsers);
                        this.UserList.DataSource = activeUsers;
                        DataBind();
                        break;
                    case 2:
                        // Show guests
                        activeUsers = GetActiveUsersData(true);                      
                        RemoveAllButGusts(ref activeUsers);
                        this.UserList.DataSource = activeUsers;
                        DataBind();
                        break;
                    case 3:
                        // Show hidden                         
                        if (this.PageContext.IsAdmin)                        {
                            activeUsers = GetActiveUsersData(false);
                            RemoveAllButHiddenUsers(ref activeUsers);
                            this.UserList.DataSource = activeUsers;
                            DataBind();
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

    private void RemoveAllButGusts(ref DataTable activeUsers)
    {
        if (activeUsers.Rows.Count <= 0)
        {
            return;
        }
        // remove non-guest users...
        foreach (DataRow row in activeUsers.Rows)
        {
            if (!Convert.ToBoolean(row["IsGuest"]))
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
            if (Convert.ToBoolean(row["IsHidden"]) && !PageContext.IsAdmin && !(PageContext.PageUserID == Convert.ToInt32(row["UserID"])))
            {
                // remove this active user...
                row.Delete();
            }            
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
            if (!Convert.ToBoolean(row["IsHidden"]) && !(PageContext.PageUserID == Convert.ToInt32(row["UserID"])))
            {
                // remove this active user...
                row.Delete();
            }           
        }
        activeUsers.AcceptChanges();
    }

    private DataTable GetActiveUsersData(bool showGuests)
    {
        // vzrus: Here should not be a common cache as it's should be individual for each user because of ActiveLocationcontrol to hide unavailable places.        
        DataTable activeUsers = DB.active_list_user(this.PageContext.PageBoardID, this.PageContext.PageUserID, showGuests, this.PageContext.BoardSettings.ActiveListTime, this.PageContext.BoardSettings.UseStyledNicks);
        // Set colorOnly parameter to false, as we get active users style from database        
        if (PageContext.BoardSettings.UseStyledNicks)
        {
            new StyleTransform(PageContext.Theme).DecodeStyleByTable(ref activeUsers, false);
        }
        return activeUsers;
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        YafBuildLink.Redirect(ForumPages.forum);
    }
  }
}