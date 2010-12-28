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
  using System;
  using System.Web;

  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;

  /// <summary>
  /// Summary description for SuspendCheckModule
  /// </summary>
  [YafModule("Suspend Check Module", "Tiny Gecko", 1)]
  public class SuspendCheckForumModule : SimpleBaseForumModule
  {
    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
      PageContext.PagePreLoad += PageContext_PagePreLoad;
    }

    /// <summary>
    /// The page context_ page pre load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void PageContext_PagePreLoad(object sender, EventArgs e)
    {
      // check for suspension if enabled...
      if (PageContext.Globals.IsSuspendCheckEnabled && PageContext.IsSuspended)
      {
        if (PageContext.SuspendedUntil < DateTime.UtcNow)
        {
          DB.user_suspend(PageContext.PageUserID, null);
          HttpContext.Current.Response.Redirect(General.GetSafeRawUrl());
        }
        else
        {
          YafBuildLink.RedirectInfoPage(InfoMessage.Suspended);
        }
      }
    }
  }
}