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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Data;
    using System.Web.Security;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The Admin edit user page.
    /// </summary>
    public partial class edituser : AdminPage
    {
        #region Properties

        /// <summary>
        ///   Gets user ID of edited user.
        /// </summary>
        protected int CurrentUserId
        {
            get
            {
                return this.PageContext.QueryIDs["u"].ToType<int>();
            }
        }

        /// <summary>
        ///   Gets a value indicating whether Is Guest User.
        /// </summary>
        protected bool IsGuestUser
        {
            get
            {
                return UserMembershipHelper.IsGuestUser(this.CurrentUserId);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether [is user host admin]
        /// </summary>
        /// <param name="userRow">The user row.</param>
        /// <returns>
        /// The is user host admin.
        /// </returns>
        protected bool IsUserHostAdmin([NotNull] DataRow userRow)
        {
            var userFlags = new UserFlags(userRow["Flags"]);
            return userFlags.IsHostAdmin;
        }

        /// <summary>
        /// Registers the java scripts
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJsBlock(
                "EditUserTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(this.EditUserTabs.ClientID, this.hidLastTab.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PageContext.QueryIDs = new QueryStringIDHelper("u", true);

            var dt = LegacyDb.user_list(this.PageContext.PageBoardID, this.CurrentUserId, null);

            if (dt.Rows.Count != 1)
            {
                return;
            }

            var userRow = dt.Rows[0];

            // do admin permission check...
            if (!this.PageContext.IsHostAdmin && this.IsUserHostAdmin(userRow))
            {
                // user is not host admin and is attempted to edit host admin account...
                YafBuildLink.AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            var userName = this.Get<YafBoardSettings>().EnableDisplayName
                               ? userRow["DisplayName"].ToString()
                               : userRow["Name"].ToString();

            var header = this.GetText("ADMIN_EDITUSER", "TITLE").FormatWith(userName);

            this.Header.Text = this.Header2.Text = header;

            // current page label (no link)
            this.PageLinks.AddLink(
                header,
                string.Empty);

            this.Page.Header.Title = "{0} - {1} - {2}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"),
                this.GetText("ADMIN_USERS", "TITLE"),
                this.GetText("ADMIN_EDITUSER", "TITLE").FormatWith(userName));

            // do a quick user membership sync...
            var user = UserMembershipHelper.GetMembershipUserById(this.CurrentUserId);

            // update if the user is not Guest
            if (!this.IsGuestUser)
            {
                RoleMembershipHelper.UpdateForumUser(user, this.PageContext.PageBoardID);
            }

            this.EditUserTabs.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_USERS", "TITLE"), YafBuildLink.GetLink(ForumPages.admin_users));
        }

        #endregion
    }
}