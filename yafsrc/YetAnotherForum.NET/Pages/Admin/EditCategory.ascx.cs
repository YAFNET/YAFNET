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
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
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
    using YAF.Web.Extensions;

    #endregion

    /// <summary>
    /// Class for the Edit Category Page
    /// </summary>
    public partial class EditCategory : AdminPage
    {
        /// <summary>
        /// The category id.
        /// </summary>
        public int CategoryId =>
            this.Get<HttpRequestBase>().QueryString.Exists("c")
                ? Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("c"))
                : 0;

        #region Methods

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void CancelClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            BuildLink.Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// The create images data table.
        /// </summary>
        protected void CreateImagesDataTable()
        {
            using (var dt = new DataTable("Files"))
            {
                dt.Columns.Add("FileName", typeof(string));
                dt.Columns.Add("Description", typeof(string));
                
                var dr = dt.NewRow();
                dr["FileName"] =
                    $"{BoardInfo.ForumClientFileRoot}Content/images/spacer.gif"; // use spacer.gif for Description Entry
                dr["Description"] = "None";
                dt.Rows.Add(dr);

                var dir = new DirectoryInfo(
                    this.Get<HttpRequestBase>().MapPath($"{BoardInfo.ForumServerFileRoot}{BoardFolders.Current.Categories}"));
                if (dir.Exists)
                {
                    var files = dir.GetFiles("*.*");

                    dt.AddImageFiles(files, BoardFolders.Current.Categories);
                }

                this.CategoryImages.DataSource = dt;
                this.CategoryImages.DataValueField = "FileName";
                this.CategoryImages.DataTextField = "Description";
                this.CategoryImages.DataBind();
            }
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

            // Populate Category Table
            this.CreateImagesDataTable();

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
                BuildLink.GetLink(ForumPages.Admin_Admin));

            this.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), BuildLink.GetLink(ForumPages.Admin_Forums));
            this.PageLinks.AddLink(this.GetText("ADMIN_EDITCATEGORY", "TITLE"), string.Empty);

            this.Page.Header.Title =
                $"{this.GetText("ADMIN_ADMIN", "Administration")} - {this.GetText("TEAM", "FORUMS")} - {this.GetText("ADMIN_EDITCATEGORY", "TITLE")}";
        }

        /// <summary>
        /// Saves the click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void SaveClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var name = this.Name.Text.Trim();
            string categoryImage = null;

            if (this.CategoryImages.SelectedIndex > 0)
            {
                categoryImage = this.CategoryImages.SelectedItem.Text;
            }

            if (!ValidationHelper.IsValidPosShort(this.SortOrder.Text.Trim()))
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITCATEGORY", "MSG_POSITIVE_VALUE"),
                    MessageTypes.danger);
                return;
            }

            if (!short.TryParse(this.SortOrder.Text.Trim(), out var sortOrder))
            {
                // error...
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITCATEGORY", "MSG_NUMBER"), MessageTypes.danger);
                return;
            }

            if (name.IsNotSet())
            {
                // error...
                this.PageContext.AddLoadMessage(this.GetText("ADMIN_EDITCATEGORY", "MSG_VALUE"), MessageTypes.danger);
                return;
            }

            var category = this.GetRepository<Category>().GetSingle(c => c.Name == name);

            // Check Name duplicate only if new Category
            if (category != null && this.CategoryId == 0)
            {
                this.PageContext.AddLoadMessage(
                    this.GetText("ADMIN_EDITCATEGORY", "MSG_CATEGORY_EXISTS"),
                    MessageTypes.warning);
                return;
            }

            // save category
            this.GetRepository<Category>().Save(this.CategoryId, name, categoryImage, sortOrder);

            // remove category cache...
            this.Get<IDataCache>().Remove(Constants.Cache.ForumCategory);

            // redirect
            BuildLink.Redirect(ForumPages.Admin_Forums);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        private void BindData()
        {
            if (!this.Get<HttpRequestBase>().QueryString.Exists("c"))
            {
                this.LocalizedLabel2.LocalizedTag = "NEW_CATEGORY";

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

                return;
            }

            var category = this.GetRepository<Category>().List(this.CategoryId)
                .FirstOrDefault();

            if (category == null)
            {
                return;
            }

            this.Name.Text = category.Name;
            this.SortOrder.Text = category.SortOrder.ToString();

            this.IconHeader.Text = $"{this.GetText("ADMIN_EDITCATEGORY", "HEADER")} <strong>{this.Name.Text}</strong>";

            this.Label1.Text = this.Name.Text;

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