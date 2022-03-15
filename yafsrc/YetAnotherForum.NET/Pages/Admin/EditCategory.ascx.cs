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
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core.BasePages;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Core.Services;
    using YAF.Core.Utilities;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Class for the Edit Category Page
    /// </summary>
    public partial class EditCategory : AdminPage
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCategory"/> class. 
        /// </summary>
        public EditCategory()
            : base("ADMIN_EDITCATEGORY", ForumPages.Admin_EditCategory)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// create images List.
        /// </summary>
        protected void CreateImagesList()
        {
            var list = new List<NamedParameter>
            {
                new(this.GetText("COMMON", "NONE"), "")
            };

            var dir = new DirectoryInfo(
                this.Get<HttpRequestBase>()
                    .MapPath($"{BoardInfo.ForumServerFileRoot}{this.Get<BoardFolders>().Categories}"));
            if (dir.Exists)
            {
                var files = dir.GetFiles("*.*").ToList();

                list.AddImageFiles(files, this.Get<BoardFolders>().Categories);
            }

            this.CategoryImages.PlaceHolder = this.GetText("COMMON", "NONE");
            this.CategoryImages.AllowClear = true;

            this.CategoryImages.DataSource = list;
            this.CategoryImages.DataValueField = "Value";
            this.CategoryImages.DataTextField = "Name";
            this.CategoryImages.DataBind();
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

            this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                nameof(JavaScriptBlocks.FormValidatorJs),
                JavaScriptBlocks.FormValidatorJs(this.Save.ClientID));

            // Populate Categories
            this.CreateImagesList();

            this.BindData();
        }

        /// <summary>
        /// Creates page links for this page.
        /// </summary>
        protected override void CreatePageLinks()
        {
            this.PageLinks.AddRoot();
            this.PageLinks.AddAdminIndex();

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITCATEGORY", "TITLE"), string.Empty);
        }

        /// <summary>
        /// Saves the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            string categoryImage = null;

            if (this.CategoryImages.SelectedIndex > 0)
            {
                categoryImage = this.CategoryImages.SelectedItem.Text;
            }

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageBoardContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITCATEGORY", "MSG_POSITIVE_VALUE"),
                    MessageTypes.danger);
                return;
            }

            var category = this.GetRepository<Category>().GetSingle(c => c.Name == this.Name.Text);

            // Check Name duplicate only if new Category
            if (category != null && this.PageBoardContext.PageCategoryID == 0)
            {
                this.PageBoardContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITCATEGORY", "MSG_CATEGORY_EXISTS"),
                    MessageTypes.warning);
                return;
            }

            // save category
            this.GetRepository<Category>().Save(this.PageBoardContext.PageCategoryID, this.Name.Text, categoryImage, this.SortOrder.Text.ToType<short>());

            // redirect
            this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (this.Get<HttpRequestBase>().QueryString.Exists("c"))
            {
                this.BindExisting(); 
            }
            else
            {
                this.BindNew();
            }
        }

        private void BindNew()
        {
            this.IconHeader.Text = this.GetText("NEW_CATEGORY");

            // Currently creating a New Category, and auto fill the Category Sort Order + 1
            var sortOrder = 1;

            try
            {
                sortOrder = this.GetRepository<Category>().GetHighestSortOrder() + sortOrder;
            }
            catch
            {
                sortOrder = 1;
            }

            this.SortOrder.Text = sortOrder.ToString();
        }

        private void BindExisting()
        {
            var category = this.PageBoardContext.PageCategory;

            if (category == null)
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }

            this.Name.Text = category.Name;
            this.SortOrder.Text = category.SortOrder.ToString();

            this.IconHeader.Text = $"{this.GetText("ADMIN_EDITCATEGORY", "HEADER")} <strong>{this.Name.Text}</strong>";

            var item = this.CategoryImages.Items.FindByText(category.CategoryImage);

            if (item == null)
            {
                return;
            }

            item.Selected = true;
        }

        #endregion
    }
}