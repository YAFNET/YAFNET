/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

    using YAF.Core.BasePages;
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

    using Forum = YAF.Types.Models.Forum;

    #endregion

    /// <summary>
    /// The Admin Manage Forums and Categories Page.
    /// </summary>
    public partial class Forums : AdminPage
    {
        #region Methods

        /// <summary>
        /// Handle Commands for Edit/Copy/Delete Forum
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void ForumList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    BuildLink.Redirect(ForumPages.Admin_EditForum, "fa={0}", e.CommandArgument);
                    break;
                case "copy":
                    BuildLink.Redirect(ForumPages.Admin_EditForum, "copy={0}", e.CommandArgument);
                    break;
                case "delete":
                    BuildLink.Redirect(ForumPages.Admin_DeleteForum, "fa={0}", e.CommandArgument);
                    break;
            }
        }

        /// <summary>
        /// Handle Commands for EditDelete a Category
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
        protected void CategoryList_ItemCommand([NotNull] object source, [NotNull] RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "edit":
                    BuildLink.Redirect(ForumPages.Admin_EditCategory, "c={0}", e.CommandArgument);
                    break;
                case "delete":
                    if (this.GetRepository<Category>().DeleteById(e.CommandArgument.ToType<int>()))
                    {
                        this.BindData();
                    }
                    else
                    {
                        this.PageContext.AddLoadMessage(
                            this.GetText("ADMIN_FORUMS", "MSG_NOT_DELETE"),
                            MessageTypes.warning);
                    }

                    break;
            }
        }

        /// <summary>
        /// The new category_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void NewCategory_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_EditCategory);
        }

        /// <summary>
        /// The new forum_ click.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void NewForum_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_EditForum);
        }

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

            this.BindData();
        }

        /// <summary>
        /// The category list_ on item data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CategoryList_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            var category = (Category)e.Item.DataItem;

            var themeButtonDelete = e.Item.FindControlAs<ThemeButton>("ThemeButtonDelete");
            var themeButton2 = e.Item.FindControlAs<ThemeButton>("ThemeButton2");

            var forums = this.GetRepository<Forum>().GetByCategorySorted(category.ID);

            themeButtonDelete.Visible = themeButton2.Visible = !forums.Any();

            if (!forums.Any())
            {
                return;
            }

            var forumRepeater = e.Item.FindControlAs<Repeater>("ForumList");
            forumRepeater.DataSource = forums;
            forumRepeater.DataBind();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"),
                BuildLink.GetLink(ForumPages.Admin_Admin));
            this.PageLinks.AddLink(this.GetText("ADMINMENU", "admin_forums"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("ADMINMENU", "admin_forums")}";
        }

        /// <summary>
        /// Binds the data.
        /// </summary>
        private void BindData()
        {
            this.CategoryList.DataSource = this.GetRepository<Category>().GetByBoardId().OrderBy(c => c.SortOrder);

            // Hide the New Forum Button if there are no Categories.
            this.NewForum.Visible = this.CategoryList.Items.Count < 1;

            this.DataBind();
        }

        #endregion
    }
}