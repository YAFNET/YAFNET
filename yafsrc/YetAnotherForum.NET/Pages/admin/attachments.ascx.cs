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
namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// The Admin Manage User Attachments Page.
    /// </summary>
    public partial class attachments : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "attachments" /> class.
        /// </summary>
        public attachments()
            : base("ADMIN_ATTACHMENTS")
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            // do it only once, not on post-backs
            if (this.IsPostBack)
            {
                return;
            }

            // bind data to controls
            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();

            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_ATTACHMENTS", "TITLE"), string.Empty);
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
        /// Handles single record commands in a repeater.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void ListItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            // what command are we serving?
            switch (e.CommandName)
            {
                // delete log entry
                case "delete":

                    this.GetRepository<Attachment>().DeleteById(e.CommandArgument.ToType<int>());

                    // re-bind controls
                    this.BindData();
                    break;
            }
        }

        /// <summary>
        /// Gets the preview image.
        /// </summary>
        /// <param name="o">The Data Row object.</param>
        /// <returns>Returns the Preview Image</returns>
        protected string GetPreviewImage([NotNull] object o)
        {
            var attach = o.ToType<Attachment>();

            var fileName = attach.FileName;
            var isImage = fileName.IsImageName();
            var url =
                $"{BoardInfo.ForumClientFileRoot}resource.ashx?i={attach.ID}&b={this.PageContext.PageBoardID}&editor=true";

            return isImage
                ? $"<img src=\"{url}\" alt=\"{fileName}\" title=\"{fileName}\" data-url=\"{url}\" style=\"max-width:30px\" class=\"me-2\" />"
                : "<i class=\"far fa-file-alt attachment-icon me-2\"></i>";
        }

        /// <summary>
        /// Renders the UserLink
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string UserLink([NotNull] User item)
        {
            var userLink = new UserLink
            {
                UserID = item.ID,
                Suspended = item.Suspended,
                Style = item.UserStyle,
                ReplaceName = this.PageContext.BoardSettings.EnableDisplayName ? item.DisplayName : item.Name
            };

            return userLink.RenderToString();
        }

        /// <summary>
        /// Populates data source and binds data to controls.
        /// </summary>
        private void BindData()
        {
            var currentPageIndex = this.PagerTop.CurrentPageIndex;
            this.PagerTop.PageSize = 10;

            // list event for this board
            var list = this.GetRepository<Attachment>().GetByBoardPaged(
                out var count,
                this.PageContext.PageBoardID,
                currentPageIndex,
                10);

            this.List.DataSource = list;

            this.PagerTop.Count = list != null && list.Any() ? count : 0;

            // bind data to controls
            this.DataBind();

            if (this.List.Items.Count == 0)
            {
                this.NoInfo.Visible = true;
            }
        }

        #endregion
    }
}