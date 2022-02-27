/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Active Users Page.
    /// </summary>
    public partial class ActiveUsers : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ActiveUsers" /> class.
        /// </summary>
        public ActiveUsers()
            : base("ACTIVEUSERS", ForumPages.ActiveUsers)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageSize.DataSource = StaticDataHelper.PageEntries();
            this.PageSize.DataTextField = "Name";
            this.PageSize.DataValueField = "Value";
            this.PageSize.DataBind();

            try
            {
                this.PageSize.SelectedValue = this.PageContext.PageUser.PageSize.ToString();
            }
            catch (Exception)
            {
                this.PageSize.SelectedValue = "5";
            }

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v").IsSet() && this.Get<IPermissions>()
                .Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions))
            {
                this.BindData();
            }
            else
            {
                this.Get<LinkBuilder>().AccessDenied();
            }
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            // forum index
            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
        }

        /// <summary>
        /// The pager top_ page change.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void PagerTopPageChange([NotNull] object sender, [NotNull] EventArgs e)
        {
            // rebind
            this.BindData();
        }

        /// <summary>
        /// The page size on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// Removes from the DataView all users but guests.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private static void RemoveAllButGuests([NotNull] ref List<ActiveUser> activeUsers)
        {
            if (!activeUsers.Any())
            {
                return;
            }

            // remove non-guest users...
            activeUsers.RemoveAll(row => row.IsGuest == false);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            var baseSize = this.PageSize.SelectedValue.ToType<int>();
            this.PagerTop.PageSize = baseSize;

            List<ActiveUser> activeUsers = null;

            switch (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefaultAs<int>("v"))
            {
                case 0:

                    // Show all users
                    activeUsers = this.GetActiveUsersData(
                        true,
                        this.PageContext.BoardSettings.ShowGuestsInDetailedActiveList);

                    if (activeUsers != null)
                    {
                        this.RemoveHiddenUsers(ref activeUsers);
                    }

                    break;
                case 1:

                    // Show members
                    activeUsers = this.GetActiveUsersData(false, false);

                    if (activeUsers != null)
                    {
                        this.RemoveHiddenUsers(ref activeUsers);
                    }

                    break;
                case 2:

                    // Show guests
                    activeUsers = this.GetActiveUsersData(
                        true,
                        this.PageContext.BoardSettings.ShowCrawlersInActiveList);

                    if (activeUsers != null)
                    {
                        RemoveAllButGuests(ref activeUsers);
                    }

                    break;
                case 3:

                    // Show hidden
                    if (this.PageContext.IsAdmin)
                    {
                        activeUsers = this.GetActiveUsersData(false, false);
                        if (activeUsers != null)
                        {
                            this.RemoveAllButHiddenUsers(ref activeUsers);
                        }
                    }
                    else
                    {
                        this.Get<LinkBuilder>().AccessDenied();
                    }

                    break;
                default:
                    this.Get<LinkBuilder>().AccessDenied();
                    break;
            }

            if (activeUsers.NullOrEmpty())
            {
                return;
            }

            this.UserList.DataSource = activeUsers.GetPaged(this.PagerTop);
            this.DataBind();
        }

        /// <summary>
        /// Gets active user(s) data for a page user
        /// </summary>
        /// <param name="showGuests">
        /// The show guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers.
        /// </param>
        /// <returns>
        /// Returns the Active user list
        /// </returns>
        private List<ActiveUser> GetActiveUsersData(bool showGuests, bool showCrawlers)
        {
            var activeUsers = this.GetRepository<Active>().ListUsersPaged(
                this.PageContext.PageUserID,
                showGuests,
                showCrawlers,
                this.PageContext.BoardSettings.ActiveListTime,
                0,
                5000);

            return activeUsers;
        }

        /// <summary>
        /// Removes from the DataView all users but hidden.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private void RemoveAllButHiddenUsers([NotNull] ref List<ActiveUser> activeUsers)
        {
            if (!activeUsers.Any())
            {
                return;
            }

            // remove hidden users...
            activeUsers.RemoveAll(
                row => row.IsActiveExcluded == false && this.PageContext.PageUserID != row.UserID);
        }

        /// <summary>
        /// Removes hidden users.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private void RemoveHiddenUsers([NotNull] ref List<ActiveUser> activeUsers)
        {
            if (!activeUsers.Any())
            {
                return;
            }

            // remove hidden users...
            activeUsers.RemoveAll(
                row => row.IsActiveExcluded && !this.PageContext.IsAdmin &&
                       this.PageContext.PageUserID != row.UserID);
        }

        #endregion
    }
}