/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Modules
{
    #region Using

    using System;

    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Extensions;

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
        private PopupDialogNotification errorPopup;

        #endregion

        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            if (this.errorPopup == null)
            {
                this.AddErrorPopup();
            }

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
            message.Message = message.Message.ToJsString();

            if (string.IsNullOrEmpty(message.Message))
            {
                return;
            }

            this.PageContext.PageElements.RegisterJsBlockStartup(
                this.ForumControl.Page,
                "modalNotification",
                "var fpModal = function() {{ {2}('{0}', '{1}'); Sys.Application.remove_load(fpModal); }}; Sys.Application.add_load(fpModal);"
                    .FormatWith(message.Message, message.MessageType.ToString().ToLower(), this.errorPopup.ShowModalFunction));
        }

        /// <summary>
        /// Sets up the Modal Error Popup Dialog
        /// </summary>
        private void AddErrorPopup()
        {
            if (this.ForumControl.FindControl("YafForumPageErrorPopup1") == null)
            {
                // add error control...
                this.errorPopup = new PopupDialogNotification
                    {
                        ID = "YafForumPageErrorPopup1"
                    };

                this.ForumControl.Controls.Add(this.errorPopup);
            }
            else
            {
                // reference existing control...
                this.errorPopup = (PopupDialogNotification)this.ForumControl.FindControl("YafForumPageErrorPopup1");
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