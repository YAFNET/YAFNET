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

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Helpers;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

using Forum = YAF.Types.Models.Forum;

/// <summary>
/// The Admin Manage Forums and Categories Page.
/// </summary>
public class ForumsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the category list.
    /// </summary>
    /// <value>The category list.</value>
    [BindProperty]
    public List<Category> CategoryList { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumsModel"/> class.
    /// </summary>
    public ForumsModel()
        : base("ADMIN_FORUMS", ForumPages.Admin_Forums)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMINMENU", "admin_forums"), string.Empty);

        this.PageTitle = this.GetText("ADMINMENU", "admin_forums");
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
       this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

        this.BindData();
    }

    /// <summary>
    /// Called when [post delete category].
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
    {
        if (!await this.GetRepository<Category>().DeleteByIdAsync(id))
        {
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_FORUMS", "MSG_NOT_DELETE"),
                MessageTypes.warning);
        }

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// The sort categories ascending.
    /// </summary>
    protected IActionResult OnPostSortCategoriesAscending()
    {
        var listAll = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>();

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        this.GetRepository<Category>().ReOrderAllAscending(categories);

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort categories descending.
    /// </summary>
    protected IActionResult OnPostSortCategoriesDescending()
    {
        var listAll = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>();

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        this.GetRepository<Category>().ReOrderAllDescending(categories);

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort forums ascending.
    /// </summary>
    protected IActionResult OnPostSortForumsAscending()
    {
        var listAll = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>();

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        if (categories.NullOrEmpty())
        {
            return this.Page();
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllAscending(forums);
            });

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort forums descending.
    /// </summary>
    protected IActionResult OnPostSortForumsDescending()
    {
        var listAll = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>();

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        if (categories.NullOrEmpty())
        {
            return this.Page();
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllDescending(forums);
            });

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var listAll = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>();

        var pager = new Paging
                    {
                        CurrentPageIndex = this.PageBoardContext.PageIndex,
                        PageSize = this.Size, Count = listAll.Count
                    };

        this.CategoryList = [.. listAll.GetPaged(pager).Select(x => x.Item1).DistinctBy(x => x.Name)];
    }
}