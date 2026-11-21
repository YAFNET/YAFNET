/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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
    /// Gets or sets the number of all categories (of all pages).
    /// </summary>
    public int CategoryCount { get; set; }

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
        // only categories of the board
        if (!await this.GetRepository<Category>().ExistsAsync(x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID))
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (!await this.GetRepository<Category>().DeleteByIdAsync(id))
        {
            // the page is shown again with the list
            this.BindData();

            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_FORUMS", "MSG_NOT_DELETE"),
                MessageTypes.warning);
        }

        // the list of the page is kept in the session, it still contains the deleted category
        this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// The sort categories ascending.
    /// </summary>
    public IActionResult OnPostSortCategoriesAscending()
    {
        // the current forums (the list of the page can be outdated)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        this.GetRepository<Category>().ReOrderAllAscending(categories);

        this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort categories descending.
    /// </summary>
    public IActionResult OnPostSortCategoriesDescending()
    {
        // the current forums (the list of the page can be outdated)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        this.GetRepository<Category>().ReOrderAllDescending(categories);

        this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort forums ascending.
    /// </summary>
    public IActionResult OnPostSortForumsAscending()
    {
        // the current forums (the list of the page can be outdated)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        if (categories.NullOrEmpty())
        {
            this.BindData();

            return this.Page();
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllAscending(forums);
            });

        this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.warning);
    }

    /// <summary>
    /// The sort forums descending.
    /// </summary>
    public IActionResult OnPostSortForumsDescending()
    {
        // the current forums (the list of the page can be outdated)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.Name).ToList();

        if (categories.NullOrEmpty())
        {
            this.BindData();

            return this.Page();
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllDescending(forums);
            });

        this.Get<ISessionService>().SetPageData(this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID));

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

        // the categories are paged (each with all its forums): paging the rows of categories and forums
        // showed a category on more than one page with all its forums on each of them
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList();

        this.CategoryCount = categories.Count;

        var pager = new Paging
                    {
                        CurrentPageIndex = this.PageBoardContext.PageIndex,
                        PageSize = this.Size, Count = categories.Count
                    };

        this.CategoryList = [.. categories.GetPaged(pager)];
    }

    /// <summary>
    /// Gets the forums of a category as a tree: each forum is followed by its sub forums (at any depth),
    /// the forums of a level are ordered by the sort order.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <returns>The forums with their depth (0 = forum without parent in the category).</returns>
    public List<(Forum Forum, int Depth)> GetForumTree(int categoryId)
    {
        // a category without forums has a forum row with the default values (left join)
        var forums = this.Get<ISessionService>().GetPageData<IList<Tuple<Category, Forum>>>()
            .Select(x => x.Item2)
            .Where(f => f is not null && f.ID > 0 && f.CategoryID == categoryId)
            .OrderBy(f => f.SortOrder).ThenBy(f => f.ID)
            .ToList();

        var ids = forums.Select(f => f.ID).ToHashSet();
        var children = forums.Where(f => f.ParentID.HasValue).ToLookup(f => f.ParentID.Value);

        var tree = new List<(Forum Forum, int Depth)>();
        var added = new HashSet<int>();

        void Add(Forum forum, int depth)
        {
            // a broken hierarchy (a forum below its own sub forum) is shown only once
            if (!added.Add(forum.ID))
            {
                return;
            }

            tree.Add((forum, depth));

            foreach (var child in children[forum.ID])
            {
                Add(child, depth + 1);
            }
        }

        // the parent of a forum can be in another category, then the forum is a top forum of this category
        foreach (var forum in forums.Where(f => !f.ParentID.HasValue || !ids.Contains(f.ParentID.Value)))
        {
            Add(forum, 0);
        }

        // forums of a broken hierarchy that were not reached
        foreach (var forum in forums.Where(f => !added.Contains(f.ID)))
        {
            Add(forum, 0);
        }

        return tree;
    }
}