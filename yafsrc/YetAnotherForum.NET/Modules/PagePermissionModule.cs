/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Pattern;

  #endregion

  /// <summary>
  /// Module that handles page permission feature
  /// </summary>
  [YafModule("Page Permission Module", "Tiny Gecko", 1)]
  public class PagePermissionModule : SimpleBaseModule
  {
    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this.CurrentForumPage.Load += this.CurrentPage_Load;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The current page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CurrentPage_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // check access permissions for specific pages...
      switch (this.ForumPageType)
      {
        case ForumPages.activeusers:
          PageContext.Get<IPermissions>().HandleRequest(this.PageContext.BoardSettings.ActiveUsersViewPermissions);
          break;
        case ForumPages.members:
          PageContext.Get<IPermissions>().HandleRequest(this.PageContext.BoardSettings.MembersListViewPermissions);
          break;
        case ForumPages.profile:
        case ForumPages.albums:
        case ForumPages.album:
          PageContext.Get<IPermissions>().HandleRequest(this.PageContext.BoardSettings.ProfileViewPermissions);
          break;
        case ForumPages.search:
          PageContext.Get<IPermissions>().HandleRequest(
            PageContext.Get<IPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions)
              ? this.PageContext.BoardSettings.SearchPermissions
              : this.PageContext.BoardSettings.ExternalSearchPermissions);
          break;
        default:
          break;
      }
    }

    #endregion
  }
}