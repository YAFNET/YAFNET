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
  using System.Web.UI;

  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// Summary description for PagePopupModule
  /// </summary>
  [YafModule("Page Popup Module", "Tiny Gecko", 1)]
  public class PagePopupForumModule : SimpleBaseForumModule
  {
    #region Constants and Fields

    /// <summary>
    ///   The _error popup.
    /// </summary>
    protected PopupDialogNotification _errorPopup;

    #endregion

    #region Public Methods

    /// <summary>
    /// The init after page.
    /// </summary>
    public override void InitAfterPage()
    {
      this._errorPopup.Title = this.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
      this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
    }

    /// <summary>
    /// The init forum.
    /// </summary>
    public override void InitForum()
    {
      this.ForumControl.Init += this.ForumControl_Init;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The register load string.
    /// </summary>
    protected void RegisterLoadString()
    {
      this.PageContext.PageElements.RegisterJQuery();

        if (this.PageContext.LoadMessage.LoadString.Length <= 0)
        {
            return;
        }

        if (ScriptManager.GetCurrent(this.ForumControl.Page) == null)
        {
            return;
        }

        string displayMessage = this.PageContext.LoadMessage.LoadStringDelimited("<br />");

        // Get the clean JS string.
        displayMessage = LoadMessage.CleanJsString(displayMessage);
        this.PageContext.PageElements.RegisterJsBlockStartup(
            this.ForumControl.Page, 
            "modalNotification",
            "var fpModal = function() {{ {1}('{0}'); Sys.Application.remove_load(fpModal); }}; Sys.Application.add_load(fpModal);".FormatWith(displayMessage, this._errorPopup.ShowModalFunction));
    }

    /// <summary>
    /// Sets up the Modal Error Popup Dialog
    /// </summary>
    private void AddErrorPopup()
    {
        if (this.ForumControl.FindControl("YafForumPageErrorPopup1") == null)
        {
            // add error control...
            this._errorPopup = new PopupDialogNotification { ID = "YafForumPageErrorPopup1" };

            this.ForumControl.Controls.Add(this._errorPopup);
        }
        else
        {
            // reference existing control...
            this._errorPopup = (PopupDialogNotification)this.ForumControl.FindControl("YafForumPageErrorPopup1");
        }
    }

      /// <summary>
    /// The current forum page_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.RegisterLoadString();
    }

    /// <summary>
    /// The forum control_ init.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumControl_Init([NotNull] object sender, [NotNull] EventArgs e)
    {
      // at this point, init has already been called...
      this.AddErrorPopup();
    }

    #endregion
  }
}