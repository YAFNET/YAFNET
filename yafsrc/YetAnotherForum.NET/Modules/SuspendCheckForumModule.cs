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
  using System.Web;

  using YAF.Classes.Data;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for SuspendCheckModule
  /// </summary>
  [YafModule("Suspend Check Module", "Tiny Gecko", 1)]
  public class SuspendCheckForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    /// The _pre load page.
    /// </summary>
    private readonly IFireEvent<ForumPagePreLoadEvent> _preLoadPage;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SuspendCheckForumModule"/> class.
    /// </summary>
    /// <param name="preLoadPage">
    /// The pre load page.
    /// </param>
    public SuspendCheckForumModule([NotNull] IFireEvent<ForumPagePreLoadEvent> preLoadPage)
    {
      this._preLoadPage = preLoadPage;
      this._preLoadPage.HandleEvent += this._preLoadPage_HandleEvent;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The _pre load page_ handle event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void _preLoadPage_HandleEvent([NotNull] object sender, [NotNull] EventConverterArgs<ForumPagePreLoadEvent> e)
    {
      // check for suspension if enabled...
      if (this.PageContext.Globals.IsSuspendCheckEnabled && this.PageContext.IsSuspended)
      {
        if (this.PageContext.SuspendedUntil < DateTime.UtcNow)
        {
          LegacyDb.user_suspend(this.PageContext.PageUserID, null);
          HttpContext.Current.Response.Redirect(General.GetSafeRawUrl());
        }
        else
        {
          YafBuildLink.RedirectInfoPage(InfoMessage.Suspended);
        }
      }
    }

    #endregion
  }
}