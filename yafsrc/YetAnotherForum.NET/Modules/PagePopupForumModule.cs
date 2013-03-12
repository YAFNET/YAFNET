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

    using System;

    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Page Popup Module
    /// </summary>
    [YafModule("Page Popup Module", "Tiny Gecko", 1)]
    public class PagePopupForumModule : SimpleBaseForumModule
    {
        #region Constants and Fields

        /// <summary>
        ///   The _error popup.
        /// </summary>
        private PopupDialogNotification _errorPopup;

        #endregion

        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            if (this._errorPopup == null)
            {
                this.AddErrorPopup();
            }

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

            var message = this.PageContext.LoadMessage.GetMessage();

            if (message == null)
            {
                return;
            }

            /*if (ScriptManager.GetCurrent(this.ForumControl.Page) == null)
            {
                return;
            }*/

            // Get the clean JS string.
            message.Message = LoadMessage.CleanJsString(message.Message);

            if (string.IsNullOrEmpty(message.Message))
            {
                return;
            }

            this.PageContext.PageElements.RegisterJsBlockStartup(
                this.ForumControl.Page,
                "modalNotification",
                "var fpModal = function() {{ {2}('{0}', '{1}'); Sys.Application.remove_load(fpModal); }}; Sys.Application.add_load(fpModal);"
                    .FormatWith(message.Message, message.MessageType.ToString().ToLower(), this._errorPopup.ShowModalFunction));
        }

        /// <summary>
        /// Sets up the Modal Error Popup Dialog
        /// </summary>
        private void AddErrorPopup()
        {
            if (this.ForumControl.FindControl("YafForumPageErrorPopup1") == null)
            {
                // add error control...
                this._errorPopup = new PopupDialogNotification
                    {
                        ID = "YafForumPageErrorPopup1", 
                        Title = this.GetText("COMMON", "MODAL_NOTIFICATION_HEADER")
                    };

                this.ForumControl.Controls.Add(this._errorPopup);
            }
            else
            {
                // reference existing control...
                this._errorPopup = (PopupDialogNotification)this.ForumControl.FindControl("YafForumPageErrorPopup1");
                this._errorPopup.Title = this.GetText("COMMON", "MODAL_NOTIFICATION_HEADER");
            }
        }

        /// <summary>
        /// Handles the PreRender event of the CurrentForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RegisterLoadString();
        }

        /// <summary>
        /// Handles the Init event of the ForumControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumControl_Init([NotNull] object sender, [NotNull] EventArgs e)
        {
            // at this point, init has already been called...
            this.AddErrorPopup();
        }

        #endregion
    }
}