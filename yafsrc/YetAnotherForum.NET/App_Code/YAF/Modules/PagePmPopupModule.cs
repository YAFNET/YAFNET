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
  using YAF.Classes.Utils;

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page Title Module", "Tiny Gecko", 1)]
  public class PagePmPopupModule : SimpleBaseModule
  {
    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      CurrentForumPage.PreRender += ForumPage_PreRender;
      CurrentForumPage.Load += ForumPage_Load;
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    /// <summary>
    /// The forum page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumPage_Load(object sender, EventArgs e)
    {
      GeneratePopUp();
    }

    /// <summary>
    /// Creates this pages title and fires a PageTitleSet event if one is set
    /// </summary>
    private void GeneratePopUp()
    {
      // This happens when user logs in
      if (DisplayPMPopup())
      {
        PageContext.AddLoadMessage(String.Format(PageContext.Localization.GetText("COMMON", "UNREAD_MSG"), PageContext.UnreadPrivate));
        Mession.LastPm = PageContext.LastUnreadPm;
      }
    }

    /// <summary>
    /// The display pm popup.
    /// </summary>
    /// <returns>
    /// The display pm popup.
    /// </returns>
    protected bool DisplayPMPopup()
    {
      return (PageContext.UnreadPrivate > 0) && (PageContext.LastUnreadPm > Mession.LastPm);
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
    }
  }
}