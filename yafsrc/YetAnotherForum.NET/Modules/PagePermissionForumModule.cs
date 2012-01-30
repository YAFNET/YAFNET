/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Module that handles page permission feature
  /// </summary>
  [YafModule("Page Permission Module", "Tiny Gecko", 1)]
  public class PagePermissionForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _permissions.
    /// </summary>
    private readonly IPermissions _permissions;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PagePermissionForumModule"/> class.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    public PagePermissionForumModule([NotNull] IPermissions permissions)
    {
      this._permissions = permissions;
    }

    #endregion

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
          this._permissions.HandleRequest(this.PageContext.BoardSettings.ActiveUsersViewPermissions);
          break;
        case ForumPages.members:
          this._permissions.HandleRequest(this.PageContext.BoardSettings.MembersListViewPermissions);
          break;
        case ForumPages.profile:
        case ForumPages.albums:
        case ForumPages.album:
          this._permissions.HandleRequest(this.PageContext.BoardSettings.ProfileViewPermissions);
          break;
        case ForumPages.search:
          this._permissions.HandleRequest(
            this._permissions.Check(this.PageContext.BoardSettings.SearchPermissions)
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