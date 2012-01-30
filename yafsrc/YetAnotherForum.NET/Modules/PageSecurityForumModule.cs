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

  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Module that handles individual page security features -- needs to be expanded.
  /// </summary>
  [YafModule("Page Security Module", "Tiny Gecko", 1)]
  public class PageSecurityForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _page pre load.
    /// </summary>
    private readonly IFireEvent<ForumPagePreLoadEvent> _pagePreLoad;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PageSecurityForumModule"/> class.
    /// </summary>
    /// <param name="pagePreLoad">
    /// The page pre load.
    /// </param>
    public PageSecurityForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> pagePreLoad)
    {
      this._pagePreLoad = pagePreLoad;
      this._pagePreLoad.HandleEvent += this.PagePreLoad_HandleEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page pre load_ handle event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PagePreLoad_HandleEvent([NotNull] object sender, [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
    {
      // no security features for login/logout pages
      if (this.ForumPageType == ForumPages.login || this.ForumPageType == ForumPages.approve ||
          this.ForumPageType == ForumPages.logout || this.ForumPageType == ForumPages.recoverpassword)
      {
        return;
      }

      // check if login is required
      if (this.PageContext.BoardSettings.RequireLogin && this.PageContext.IsGuest && this.CurrentForumPage.IsProtected)
      {
        // redirect to login page if login is required
        this.CurrentForumPage.RedirectNoAccess();
      }

      // check if it's a "registered user only page" and check permissions.
      if (this.CurrentForumPage.IsRegisteredPage && this.CurrentForumPage.User == null)
      {
        this.CurrentForumPage.RedirectNoAccess();
      }

      // not totally necessary... but provides another layer of protection...
      if (this.CurrentForumPage.IsAdminPage && !this.PageContext.IsAdmin)
      {
        YafBuildLink.AccessDenied();
        return;
      }

      // handle security features...
    	if (this.ForumPageType == ForumPages.register && this.PageContext.BoardSettings.DisableRegistrations)
    	{
    		YafBuildLink.AccessDenied();
    	}
    }

    #endregion
  }
}