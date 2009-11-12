/* YetAnotherForum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
namespace YAF.Modules
{
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;

  /// <summary>
  /// Module that handles page permission feature
  /// </summary>
  [YafModule("Page Permission Module", "Tiny Gecko", 1)]
  public class PagePermissionModule : SimpleBaseModule
  {
    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      CurrentForumPage.Load += CurrentPage_Load;
    }

    /// <summary>
    /// The current page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CurrentPage_Load(object sender, EventArgs e)
    {
      // check access permissions for specific pages...
      switch (ForumPageType)
      {
        case ForumPages.activeusers:
          YafServices.Permissions.HandleRequest(PageContext.BoardSettings.ActiveUsersViewPermissions);
          break;
        case ForumPages.members:
          YafServices.Permissions.HandleRequest(PageContext.BoardSettings.MembersListViewPermissions);
          break;
        case ForumPages.profile:
          YafServices.Permissions.HandleRequest(PageContext.BoardSettings.ProfileViewPermissions);
          break;
        case ForumPages.search:
          YafServices.Permissions.HandleRequest(PageContext.BoardSettings.SearchPermissions);
          break;
        default:
          break;
      }
    }
  }
}