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
    using System.Web;

    using YAF.Classes;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Dialogs;
    using YAF.Types;
    using YAF.Types.Attributes;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The Page PM Popup Module
    /// </summary>
    [YafModule("Page PopUp Module", "Tiny Gecko", 1)]
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

        /// <summary>
        /// The init before page.
        /// </summary>
        public override void InitBeforePage()
        {
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
            return (this.PageContext.UnreadPrivate > 0)
                   && (this.PageContext.LastUnreadPm > this.Get<IYafSession>().LastPm);
        }

        /// <summary>
        /// The last pending buddies.
        /// </summary>
        /// <returns>
        /// whether we should display the pending buddies notification or not
        /// </returns>
        protected bool DisplayPendingBuddies()
        {
            return (this.PageContext.PendingBuddies > 0)
                   && (this.PageContext.LastPendingBuddies > this.Get<IYafSession>().LastPendingBuddies);
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
            var notification = (DialogBox)this.PageContext.CurrentForumPage.Notification;

            // This happens when user logs in
            if (this.DisplayPmPopup() && (!this.PageContext.ForumPageType.Equals(ForumPages.cp_pm)
                                          || !this.PageContext.ForumPageType.Equals(ForumPages.cp_editbuddies)))
            {
                if (!(this.Get<YafBoardSettings>().NotifcationNativeOnMobile
                      && this.Get<HttpRequestBase>().Browser.IsMobileDevice))
                {
                    notification.Show(
                        this.GetText("COMMON", "UNREAD_MSG2").FormatWith(this.PageContext.UnreadPrivate),
                        this.GetText("COMMON", "UNREAD_MSG_TITLE"),
                        new DialogBox.DialogButton
                            {
                                Text = this.GetText("COMMON", "YES"),
                                CssClass = "btn btn-success btn-sm",
                                ForumPageLink =
                                    new DialogBox.ForumLink { ForumPage = ForumPages.cp_pm }
                            },
                        new DialogBox.DialogButton
                            {
                                Text = this.GetText("COMMON", "NO"),
                                CssClass = "btn btn-danger btn-sm",
                                ForumPageLink =
                                    new DialogBox.ForumLink
                                        {
                                            ForumPage = YafContext.Current
                                                .ForumPageType
                                        }
                            });
                }
                else
                {
                    this.PageContext.AddLoadMessage(
                        this.GetText("COMMON", "UNREAD_MSG").FormatWith(this.PageContext.UnreadPrivate));
                }

                this.Get<IYafSession>().LastPm = this.PageContext.LastUnreadPm;

                // Avoid Showing Both Popups
                return;
            }

            if (!this.DisplayPendingBuddies() || (this.PageContext.ForumPageType.Equals(ForumPages.cp_editbuddies)
                                                  || this.PageContext.ForumPageType.Equals(ForumPages.cp_pm)))
            {
                return;
            }

            if (!(this.Get<YafBoardSettings>().NotifcationNativeOnMobile
                  && this.Get<HttpRequestBase>().Browser.IsMobileDevice))
            {
                notification.Show(
                    this.GetText("BUDDY", "PENDINGBUDDIES2").FormatWith(this.PageContext.PendingBuddies),
                    this.GetText("BUDDY", "PENDINGBUDDIES_TITLE"),
                    new DialogBox.DialogButton
                        {
                            Text = this.GetText("COMMON", "YES"),
                            CssClass = "btn btn-success btn-sm",
                            ForumPageLink =
                                new DialogBox.ForumLink { ForumPage = ForumPages.cp_editbuddies }
                        },
                    new DialogBox.DialogButton
                        {
                            Text = this.GetText("COMMON", "NO"),
                            CssClass = "btn btn-danger btn-sm",
                            ForumPageLink =
                                new DialogBox.ForumLink
                                    {
                                        ForumPage = YafContext.Current
                                            .ForumPageType
                                    }
                        });
            }
            else
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("BUDDY", "PENDINGBUDDIES2").FormatWith(this.PageContext.PendingBuddies));
            }

            this.Get<IYafSession>().LastPendingBuddies = this.PageContext.LastPendingBuddies;
        }
    }

    #endregion
}