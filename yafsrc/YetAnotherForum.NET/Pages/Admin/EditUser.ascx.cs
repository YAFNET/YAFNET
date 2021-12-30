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

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Web;

    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Types.Models.Identity;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin edit user page.
    /// </summary>
    public partial class EditUser : AdminPage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the current edit user.
        /// </summary>
        /// <value>The user.</value>
        public Tuple<User, AspNetUsers, Rank, vaccess> EditBoardUser
        {
            get => this.ViewState["EditBoardUser"].ToType<Tuple<User, AspNetUsers, Rank, vaccess>>();

            set => this.ViewState["EditBoardUser"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the java scripts
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            this.PageContext.PageElements.RegisterJsBlock(
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
            var currentUserId = this.Get<LinkBuilder>()
                .StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("u"));

            this.EditBoardUser = this.Get<IAspNetUsersHelper>().GetBoardUser(currentUserId, includeNonApproved: true);

            if (this.EditBoardUser == null)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            // do admin permission check...
            if (!this.PageContext.User.UserFlags.IsHostAdmin && this.EditBoardUser.Item1.UserFlags.IsHostAdmin)
            {
                // user is not host admin and is attempted to edit host admin account...
                this.Get<LinkBuilder>().AccessDenied();
            }

            if (this.IsPostBack)
            {
                return;
            }

            var userName = this.HtmlEncode(this.EditBoardUser.Item1.DisplayOrUserName());

            var header = string.Format(this.GetText("ADMIN_EDITUSER", "TITLE"), userName);

            this.IconHeader.Text = header;

            // current page label (no link)
            this.PageLinks.AddLink(header, string.Empty);

            // update if the user is not Guest
            if (!this.EditBoardUser.Item1.UserFlags.IsGuest)
            {
                this.Get<IAspNetRolesHelper>().UpdateForumUser(this.EditBoardUser.Item2, this.PageContext.PageBoardID);
            }

            this.EditUserTabs.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddAdminIndex();

            this.PageLinks.AddLink(
                this.GetText("ADMIN_USERS", "TITLE"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users));
        }

        #endregion
    }
}