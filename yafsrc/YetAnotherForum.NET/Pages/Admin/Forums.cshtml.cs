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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;

using Forum = YAF.Types.Models.Forum;

/// <summary>
/// The Admin Manage Forums and Categories Page.
/// </summary>
public class ForumsModel : AdminPage
{
    /// <summary>
    /// The forums of the board with their categories (a category without forums has a forum with the default values).
    /// </summary>
    private List<Tuple<Category, Forum>> listAll = [];

    /// <summary>
    /// Gets or sets the categories of the board (in the sort order).
    /// </summary>
    public List<Category> CategoryList { get; set; } = [];

    /// <summary>
    /// Gets the number of forums of the board.
    /// </summary>
    public int ForumCount => this.listAll.Count(x => IsForum(x.Item2));

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
        this.BindData();
    }

    /// <summary>
    /// Deletes a category (only a category without forums).
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteCategoryAsync(int id)
    {
        // only categories of the board
        var category = await this.GetRepository<Category>().GetSingleAsync(
                           x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID);

        if (category is null)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        if (!await this.GetRepository<Category>().DeleteByIdAsync(id))
        {
            return this.PageBoardContext.SessionNotify(
                this.GetText("ADMIN_FORUMS", "MSG_NOT_DELETE"),
                MessageTypes.warning,
                ForumPages.Admin_Forums);
        }

        // the notification shows the message as html
        return this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("CATEGORY_DELETED", WebUtility.HtmlEncode(category.Name)),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Saves the order of the categories or of the forums of one level (drag and drop).
    /// </summary>
    /// <param name="order">The categories or the forums in their new order.</param>
    /// <returns>The result as json (ok or bad request).</returns>
    public async Task<IActionResult> OnPostSaveOrderAsync([FromBody] SortOrderInput order)
    {
        if (order?.Ids is null || order.Ids.Count == 0 || order.Ids.Count != order.Ids.Distinct().Count())
        {
            return this.BadRequest();
        }

        var boardId = this.PageBoardContext.PageBoardID;

        switch (order.Type)
        {
            case "categories":
            {
                var categories = await this.GetRepository<Category>().GetAsync(c => c.BoardID == boardId);

                // only categories of the board, the order of all categories
                if (order.Ids.Count != categories.Count || order.Ids.Exists(id => categories.TrueForAll(c => c.ID != id)))
                {
                    return this.BadRequest();
                }

                for (var i = 0; i < order.Ids.Count; i++)
                {
                    var categoryId = order.Ids[i];
                    var sortOrder = (short)i;

                    await this.GetRepository<Category>().UpdateOnlyAsync(
                        () => new Category { SortOrder = sortOrder },
                        c => c.ID == categoryId);
                }

                break;
            }

            case "forums":
            {
                var forums = this.GetRepository<Forum>().ListAll(boardId).Select(x => x.Item2).Where(IsForum).ToList();
                var sorted = order.Ids.Select(id => forums.Find(f => f.ID == id)).ToList();

                // only forums of the board and of one level (the same category and the same parent),
                // a forum is moved to another category or parent on its edit page
                if (sorted.Exists(f => f is null)
                    || sorted.Exists(f => f.CategoryID != sorted[0].CategoryID || f.ParentID != sorted[0].ParentID))
                {
                    return this.BadRequest();
                }

                for (var i = 0; i < sorted.Count; i++)
                {
                    var forumId = sorted[i].ID;
                    var sortOrder = i;

                    await this.GetRepository<Forum>().UpdateOnlyAsync(
                        () => new Forum { SortOrder = sortOrder },
                        f => f.ID == forumId);
                }

                break;
            }

            default:
                return this.BadRequest();
        }

        return new JsonResult(new { saved = true });
    }

    /// <summary>
    /// The sort categories ascending.
    /// </summary>
    public IActionResult OnPostSortCategoriesAscending()
    {
        this.GetRepository<Category>().ReOrderAllAscending(this.LoadCategories());

        return this.PageBoardContext.SessionNotify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// The sort categories descending.
    /// </summary>
    public IActionResult OnPostSortCategoriesDescending()
    {
        this.GetRepository<Category>().ReOrderAllDescending(this.LoadCategories());

        return this.PageBoardContext.SessionNotify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// The sort forums ascending.
    /// </summary>
    public IActionResult OnPostSortForumsAscending()
    {
        this.LoadForumsByCategory().ForEach(forums => this.GetRepository<Forum>().ReOrderAllAscending(forums));

        return this.PageBoardContext.SessionNotify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// The sort forums descending.
    /// </summary>
    public IActionResult OnPostSortForumsDescending()
    {
        this.LoadForumsByCategory().ForEach(forums => this.GetRepository<Forum>().ReOrderAllDescending(forums));

        return this.PageBoardContext.SessionNotify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.success,
            ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Gets the number of forums of a category.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <returns>The number of forums.</returns>
    public int ForumCountOf(int categoryId) =>
        this.listAll.Count(x => IsForum(x.Item2) && x.Item2.CategoryID == categoryId);

    /// <summary>
    /// Gets the forums of a category as a tree: each forum with its sub forums (at any depth),
    /// the forums of a level are ordered by the sort order.
    /// </summary>
    /// <param name="categoryId">The category identifier.</param>
    /// <returns>The forums without parent in the category, with their sub forums.</returns>
    public List<ForumNode> GetForumTree(int categoryId)
    {
        var forums = this.listAll
            .Select(x => x.Item2)
            .Where(f => IsForum(f) && f.CategoryID == categoryId)
            .OrderBy(f => f.SortOrder).ThenBy(f => f.ID)
            .ToList();

        var ids = forums.Select(f => f.ID).ToHashSet();
        var children = forums.Where(f => f.ParentID.HasValue).ToLookup(f => f.ParentID.Value);
        var added = new HashSet<int>();

        ForumNode Node(Forum forum, int depth)
        {
            var node = new ForumNode(forum, depth, []);

            // a broken hierarchy (a forum below its own sub forum) is shown only once
            foreach (var child in children[forum.ID].Where(child => added.Add(child.ID)).ToList())
            {
                node.Children.Add(Node(child, depth + 1));
            }

            return node;
        }

        // the parent of a forum can be in another category, then the forum is a top forum of this category
        var roots = forums.Where(f => !f.ParentID.HasValue || !ids.Contains(f.ParentID.Value)).ToList();

        roots.ForEach(f => added.Add(f.ID));

        var tree = roots.Select(f => Node(f, 0)).ToList();

        // forums of a broken hierarchy that were not reached
        // (not a list: a forum reached by the node of a forum before is skipped)
        foreach (var forum in forums.Where(f => added.Add(f.ID)))
        {
            tree.Add(Node(forum, 0));
        }

        return tree;
    }

    /// <summary>
    /// A forum row of the list (a category without forums has a forum with the default values).
    /// </summary>
    private static bool IsForum(Forum forum) => forum is not null && forum.ID > 0;

    /// <summary>
    /// Loads the categories of the board (for the sort tools).
    /// </summary>
    private List<Category> LoadCategories() =>
        [.. this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID).Select(x => x.Item1).DistinctBy(x => x.ID)];

    /// <summary>
    /// Loads the forums of the board grouped by category (for the sort tools).
    /// </summary>
    private List<List<Forum>> LoadForumsByCategory() =>
        [.. this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID)
            .Select(x => x.Item2)
            .Where(IsForum)
            .GroupBy(f => f.CategoryID)
            .Select(g => g.ToList())];

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        this.CategoryList = [.. this.listAll.Select(x => x.Item1).DistinctBy(x => x.ID)];
    }

    /// <summary>
    /// A forum of the tree with its sub forums.
    /// </summary>
    /// <param name="Forum">The forum.</param>
    /// <param name="Depth">The depth (0 = forum without parent in the category).</param>
    /// <param name="Children">The sub forums.</param>
    public record ForumNode(Forum Forum, int Depth, List<ForumNode> Children);

    /// <summary>
    /// The new order of the categories or of the forums of one level.
    /// </summary>
    public class SortOrderInput
    {
        /// <summary>
        /// Gets or sets the type ("categories" or "forums").
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the ids in the new order.
        /// </summary>
        public List<int> Ids { get; set; }
    }
}
