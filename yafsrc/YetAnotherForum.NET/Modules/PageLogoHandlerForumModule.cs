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
  using System.Web.UI.HtmlControls;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Core;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Utils;
  using YAF.Utils.Helpers;
  using YAF.Types;

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page Logo Handler Module", "Tiny Gecko", 1)]
  public class PageLogoHandlerForumModule : SimpleBaseForumModule
  {
    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      CurrentForumPage.PreRender += ForumPage_PreRender;
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    /// <summary>
    /// The forum page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_PreRender(object sender, EventArgs e)
    {
      var htmlImgBanner = ControlHelper.FindControlRecursiveBothAs<HtmlImage>(CurrentForumPage, "imgBanner");
      var imgBanner = ControlHelper.FindControlRecursiveBothAs<Image>(CurrentForumPage, "imgBanner");

      if (!CurrentForumPage.ShowToolBar)
      {
        if (htmlImgBanner != null)
        {
          htmlImgBanner.Visible = false;
        }
        else if (imgBanner != null)
        {
          imgBanner.Visible = false;
        }
      }

      if (PageContext.BoardSettings.AllowThemedLogo && !Config.IsAnyPortal)
      {
        string graphicSrc = PageContext.Theme.GetItem("FORUM", "BANNER", null);

        if (graphicSrc.IsSet())
        {
          if (htmlImgBanner != null)
          {
            htmlImgBanner.Src = graphicSrc;
          }
          else if (imgBanner != null)
          {
            imgBanner.ImageUrl = graphicSrc;
          }
        }
      }
    }
  }
}