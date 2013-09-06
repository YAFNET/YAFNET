/* YetAnotherForum.NET
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

    using YAF.Types.Attributes;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

    #endregion

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page BBCode Registration Module", "Tiny Gecko", 1)]
  public class PageBBCodeRegistration : SimpleBaseForumModule
  {
    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      switch (this.PageContext.ForumPageType)
      {
        case ForumPages.cp_message:
        case ForumPages.search:
        case ForumPages.lastposts:
        case ForumPages.posts:
        case ForumPages.profile:
          this.Get<IBBCode>().RegisterCustomBBCodePageElements(
            this.PageContext.CurrentForumPage.Page, this.PageContext.CurrentForumPage.GetType());
          break;
      }
    }

    #endregion
  }
}