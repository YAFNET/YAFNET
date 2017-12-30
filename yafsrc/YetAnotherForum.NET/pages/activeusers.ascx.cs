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

namespace YAF.Pages
{
    #region Using

    using System;
    using System.Data;
    using System.Linq;
    using System.Web;

    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Active Users Page.
    /// </summary>
    public partial class activeusers : ForumPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "activeusers" /> class.
        /// </summary>
        public activeusers()
            : base("ACTIVEUSERS")
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

            if (this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("v").IsSet()
                && this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ActiveUsersViewPermissions))
            {
                this.BindData();
            }
            else
            {
                YafBuildLink.AccessDenied();
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
        /// Handles the Click event of the Return control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Return_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.forum);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            DataTable activeUsers = null;

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
                    activeUsers = this.GetActiveUsersData(true, this.PageContext.BoardSettings.ShowCrawlersInActiveList);
                    if (activeUsers != null)
                    {
                        this.RemoveAllButGuests(ref activeUsers);
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
                        YafBuildLink.AccessDenied();
                    }

                    break;
                default:
                    YafBuildLink.AccessDenied();
                    break;
            }

            if (activeUsers == null || !activeUsers.HasRows())
            {
                return;
            }

            YafContext.Current.PageElements.RegisterJsBlock(
                "UnverifiedUserstablesorterLoadJs",
                JavaScriptBlocks.LoadTableSorter(
                    "#ActiveUsers",
                    "sortList: [[0,0]]",
                    "#ActiveUsersPager"));

            this.UserList.DataSource = activeUsers;
            this.DataBind();
        }

        /// <summary>
        /// Gets active user data Table data for a page user
        /// </summary>
        /// <param name="showGuests">
        /// The show guests.
        /// </param>
        /// <param name="showCrawlers">
        /// The show crawlers.
        /// </param>
        /// <returns>
        /// A DataTable
        /// </returns>
        private DataTable GetActiveUsersData(bool showGuests, bool showCrawlers)
        {
            // vzrus: Here should not be a common cache as it's should be individual for each user because of ActiveLocationcontrol to hide unavailable places.        
            var activeUsers = this.GetRepository<Active>()
                .ListUser(
                    userID: this.PageContext.PageUserID,
                    guests: showGuests,
                    showCrawlers: showCrawlers,
                    activeTime: this.PageContext.BoardSettings.ActiveListTime,
                    styledNicks: this.PageContext.BoardSettings.UseStyledNicks);

            return activeUsers;
        }

        /// <summary>
        /// Removes from the DataView all users but guests.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private void RemoveAllButGuests([NotNull] ref DataTable activeUsers)
        {
            if (!activeUsers.HasRows())
            {
                return;
            }

            // remove non-guest users...
            foreach (var row in activeUsers.Rows.Cast<DataRow>().Where(row => !Convert.ToBoolean(row["IsGuest"])))
            {
                // remove this active user...
                row.Delete();
            }
        }

        /// <summary>
        /// Removes from the DataView all users but hidden.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private void RemoveAllButHiddenUsers([NotNull] ref DataTable activeUsers)
        {
            if (!activeUsers.HasRows())
            {
                return;
            }

            // remove hidden users...
            foreach (
                var row in
                    activeUsers.Rows.Cast<DataRow>()
                        .Where(
                            row =>
                            !row["IsHidden"].ToType<bool>()
                            && this.PageContext.PageUserID != row["UserID"].ToType<int>()))
            {
                // remove this active user...
                row.Delete();
            }
        }

        /// <summary>
        /// Removes hidden users.
        /// </summary>
        /// <param name="activeUsers">
        /// The active users.
        /// </param>
        private void RemoveHiddenUsers([NotNull] ref DataTable activeUsers)
        {
            if (!activeUsers.HasRows())
            {
                return;
            }

            // remove hidden users...
            foreach (
                var row in
                    activeUsers.Rows.Cast<DataRow>()
                        .Where(
                            row =>
                            row["IsHidden"].ToType<bool>() && !this.PageContext.IsAdmin
                            && this.PageContext.PageUserID != row["UserID"].ToType<int>()))
            {
                // remove this active user...
                row.Delete();
            }
        }

        #endregion
    }
}