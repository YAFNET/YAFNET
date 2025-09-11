/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Flags;
using YAF.Types.Models;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Types.Extensions;

/// <summary>
/// Class for the Edit Category Page
/// </summary>
public class EditCategoryModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditCategoryInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the category images.
    /// </summary>
    /// <value>The category images.</value>
    [BindProperty]
    public List<SelectListItem> CategoryImages { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditCategoryModel"/> class.
    /// </summary>
    public EditCategoryModel()
        : base("ADMIN_EDITCATEGORY", ForumPages.Admin_EditCategory)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TEAM", "FORUMS"), this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITCATEGORY", "TITLE"), string.Empty);
    }

    /// <summary>
    /// create images List.
    /// </summary>
    protected void CreateImagesList()
    {
       var list = new List<SelectListItem> {new(this.GetText("COMMON", "NONE"), "") };

       var dir = new DirectoryInfo(
           Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Categories));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Categories);
        }

        this.CategoryImages = list;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? c)
    {
        this.Input = new EditCategoryInputModel();

        // Populate Categories
        this.CreateImagesList();

        return this.BindData(c);
    }

    /// <summary>
    /// Saves the click.
    /// </summary>
    public async Task<IActionResult> OnPostSave()
    {
        int? c = this.Input.Id == 0 ? null : this.Input.Id;

        string categoryImage = null;

        if (this.Input.CategoryImage.IsSet() && this.Input.CategoryImage != this.GetText("COMMON", "NONE"))
        {
           categoryImage = this.Input.CategoryImage;
        }

        var category = await this.GetRepository<Category>().GetSingleAsync(check => check.Name == this.Input.Name);

        // Check Name duplicate only if new Category
        if (category != null && this.PageBoardContext.PageCategoryID == 0)
        {
            this.BindData(c);

            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_EDITCATEGORY", "MSG_CATEGORY_EXISTS"),
                MessageTypes.warning);
        }

        var categoryFlags = new CategoryFlags {IsActive = this.Input.Active};

        // save category
        await this.GetRepository<Category>().SaveAsync(
            this.PageBoardContext.PageCategoryID,
            this.Input.Name,
            categoryImage,
            (short)this.Input.SortOrder,
            categoryFlags);

        // redirect
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Binds the existing category data.
    /// </summary>
    private IActionResult BindData(int? c)
    {
        return c is > 0 ? this.BindExisting() : this.BindNew();
    }


    /// <summary>
    /// Binds the data as new category.
    /// </summary>
    /// <returns>IActionResult.</returns>
    private PageResult BindNew()
    {
        // Currently creating a New Category, and autofill the Category Sort Order + 1
        var sortOrder = 1;

        try
        {
            sortOrder = this.GetRepository<Category>().GetHighestSortOrder() + sortOrder;
        }
        catch
        {
            sortOrder = 1;
        }

        this.Input.SortOrder = sortOrder;
        this.Input.Active = true;

        return this.Page();
    }

    private IActionResult BindExisting()
    {
        var category = this.PageBoardContext.PageCategory;

        if (category is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Input.Name = category.Name;
        this.Input.SortOrder = category.SortOrder;

        this.Input.Active = category.CategoryFlags.IsActive;

        this.Input.CategoryImage = category.CategoryImage;

        return this.Page();
    }
}