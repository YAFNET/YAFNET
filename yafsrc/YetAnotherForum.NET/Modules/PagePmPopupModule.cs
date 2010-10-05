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

  using YAF.Classes.Core;
  using YAF.Classes.Pattern;
  using YAF.Classes.Utils;

  #endregion

  /// <summary>
  /// Summary description for PageTitleModule
  /// </summary>
  [YafModule("Page Title Module", "Tiny Gecko", 1)]
  public class PagePmPopupModule : SimpleBaseModule
  {
    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this.CurrentForumPage.PreRender += this.ForumPage_PreRender;
      this.CurrentForumPage.Load += this.ForumPage_Load;
    }

    /// <summary>
    /// The init before page.
    /// </summary>
    public override void InitBeforePage()
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// The display pm popup.
    /// </summary>
    /// <returns>
    /// The display pm popup.
    /// </returns>
    protected bool DisplayPMPopup()
    {
      return (this.PageContext.UnreadPrivate > 0) &&
             (this.PageContext.LastUnreadPm > YafContext.Current.Get<YafSession>().LastPm);
    }

    /// <summary>
    /// The last pending buddies.
    /// </summary>
    /// <returns>
    /// whether we should display the pending buddies notification or not
    /// </returns>
    protected bool DisplayPendingBuddies()
    {
      return (this.PageContext.PendingBuddies > 0) &&
             (this.PageContext.LastPendingBuddies > YafContext.Current.Get<YafSession>().LastPendingBuddies);
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
    private void ForumPage_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.GeneratePopUp();
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
    private void ForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
    }

    /// <summary>
    /// Creates this pages title and fires a PageTitleSet event if one is set
    /// </summary>
    private void GeneratePopUp()
    {
      // This happens when user logs in
      if (this.DisplayPMPopup())
      {
        this.PageContext.AddLoadMessage(
          this.PageContext.Localization.GetText("COMMON", "UNREAD_MSG").FormatWith(this.PageContext.UnreadPrivate));
        this.PageContext.Get<YafSession>().LastPm = this.PageContext.LastUnreadPm;
      }

      if (this.DisplayPendingBuddies())
      {
        this.PageContext.AddLoadMessage(
          this.PageContext.Localization.GetText("BUDDY", "PENDINGBUDDIES").FormatWith(this.PageContext.PendingBuddies));
        this.PageContext.Get<YafSession>().LastPendingBuddies = this.PageContext.LastPendingBuddies;
      }
    }

    #endregion
  }
}