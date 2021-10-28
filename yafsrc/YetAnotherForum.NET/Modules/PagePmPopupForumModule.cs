/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

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

    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Page PM Popup Module
    /// </summary>
    [Module("Page PopUp Module", "Tiny Gecko", 1)]
    public class PagePmPopupForumModule : SimpleBaseForumModule
    {
        #region Public Methods

        /// <summary>
        /// The init after page.
        /// </summary>
        public override void InitAfterPage()
        {
            this.CurrentForumPage.Load += this.ForumPageLoad;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the PM popup.
        /// </summary>
        /// <returns>
        /// The display pm popup.
        /// </returns>
        protected bool DisplayPmPopup()
        {
            return this.PageContext.UnreadPrivate > 0
                   && this.PageContext.LastUnreadPm > this.Get<ISession>().LastPm;
        }

        /// <summary>
        /// The last pending buddies.
        /// </summary>
        /// <returns>
        /// whether we should display the pending buddies notification or not
        /// </returns>
        protected bool DisplayPendingBuddies()
        {
            return this.PageContext.PendingBuddies > 0
                   && this.PageContext.LastPendingBuddies > this.Get<ISession>().LastPendingBuddies;
        }

        /// <summary>
        /// Handles the Load event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ForumPageLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.GeneratePopUp();
        }

        /// <summary>
        /// Creates this pages title and fires a PageTitleSet event if one is set
        /// </summary>
        private void GeneratePopUp()
        {
            // This happens when user logs in
            if (this.DisplayPmPopup() && this.PageContext.ForumPageType != ForumPages.MyMessages)
            {
                this.PageContext.PageElements.RegisterJsBlockStartup(
                    "ModalConfirmJs",
                    JavaScriptBlocks.BootBoxConfirmJs(
                        this.GetText("COMMON", "UNREAD_MSG_TITLE"),
                        this.GetTextFormatted("UNREAD_MSG2", this.PageContext.UnreadPrivate),
                        this.GetText("COMMON", "YES"),
                        this.GetText("COMMON", "NO"),
                        this.Get<LinkBuilder>().GetLink(ForumPages.MyMessages)));

                this.Get<ISession>().LastPm = this.PageContext.LastUnreadPm;

                // Avoid Showing Both Popups
                return;
            }

            if (!this.DisplayPendingBuddies() || this.PageContext.ForumPageType == ForumPages.MyMessages)
            {
                return;
            }

            this.PageContext.PageElements.RegisterJsBlockStartup(
                "ModalConfirmJs",
                JavaScriptBlocks.BootBoxConfirmJs(
                    this.GetText("BUDDY", "PENDINGBUDDIES_TITLE"),
                    this.GetTextFormatted("PENDINGBUDDIES2", this.PageContext.PendingBuddies),
                    this.GetText("COMMON", "YES"),
                    this.GetText("COMMON", "NO"),
                    this.Get<LinkBuilder>().GetLink(ForumPages.Friends)));

            this.Get<ISession>().LastPendingBuddies = this.PageContext.LastPendingBuddies;
        }
    }

    #endregion
}