/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

  using YAF.Classes;
  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Utils;
  

  #endregion

  /// <summary>
  /// The page requires secure connection module.
  /// </summary>
  [YafModule("Page Requires Secure Connection Module", "Tiny Gecko", 1)]
  public class PageRequiresSecureConnectionForumModule : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.ForumControl.Load += this.ForumControl_Load;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum control_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool accessDenied = false;

      switch (this.ForumPageType)
      {
        case ForumPages.login:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToLogIn)
          {
            accessDenied = true;
          }

          break;

        case ForumPages.register:
          if (!HttpContext.Current.Request.IsSecureConnection & this.PageContext.BoardSettings.UseSSLToRegister)
          {
            accessDenied = true;
          }

          break;
      }

      if (accessDenied)
      {
        YafBuildLink.RedirectInfoPage(InfoMessage.AccessDenied);
      }
    }

    #endregion
  }
}