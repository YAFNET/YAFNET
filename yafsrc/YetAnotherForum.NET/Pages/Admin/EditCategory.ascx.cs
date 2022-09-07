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

namespace YAF.Pages.Admin;

using System.IO;
using YAF.Types.Objects;
using YAF.Types.Models;

/// <summary>
/// Class for the Edit Category Page
/// </summary>
public partial class EditCategory : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditCategory"/> class. 
    /// </summary>
    public EditCategory()
        : base("ADMIN_EDITCATEGORY", ForumPages.Admin_EditCategory)
    {
    }

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
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITCATEGORY", "TITLE"), string.Empty);
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
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITCATEGORY", "MSG_POSITIVE_VALUE"),
                MessageTypes.danger);
            return;
        }

        var category = this.GetRepository<Category>().GetSingle(c => c.Name == this.Name.Text);

        // Check Name duplicate only if new Category
        if (category != null && this.PageBoardContext.PageCategoryID == 0)
        {
            this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITCATEGORY", "MSG_CATEGORY_EXISTS"),
                MessageTypes.warning);
            return;
        }

        var categoryFlags = new CategoryFlags {IsActive = this.Active.Checked};

        // save category
        this.GetRepository<Category>().Save(
            this.PageBoardContext.PageCategoryID,
            this.Name.Text,
            categoryImage,
            this.SortOrder.Text.ToType<short>(),
            categoryFlags);

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

        this.Active.Checked = category.CategoryFlags.IsActive;

        var item = this.CategoryImages.Items.FindByText(category.CategoryImage);

        if (item == null)
        {
            return;
        }

        item.Selected = true;
    }
}