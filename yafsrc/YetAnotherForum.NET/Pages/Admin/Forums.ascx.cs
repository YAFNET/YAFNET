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

namespace YAF.Pages.Admin;

using YAF.Web.Controls;
using YAF.Types.Models;

using Forum = YAF.Types.Models.Forum;

/// <summary>
/// The Admin Manage Forums and Categories Page.
/// </summary>
public partial class Forums : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Forums"/> class. 
    /// </summary>
    public Forums()
        : base("ADMIN_FORUMS", ForumPages.Admin_Forums)
    {
    }

    /// <summary>
    /// The depth of the listed forums in the tree.
    /// </summary>
    private readonly Dictionary<int, int> forumDepths = new();

    /// <summary>
    /// Gets or sets the user album.
    /// </summary>
    public IList<Tuple<Category, Forum>> ListAll
    {
        get => this.ViewState["ListAll"].ToType<List<Tuple<Category, Forum>>>();

        set => this.ViewState["ListAll"] = value;
    }

    /// <summary>
    /// Handle Commands for Edit/Copy/Delete Forum
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void ForumList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditForum, new { fa = e.CommandArgument });
                break;
            case "copy":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditForum, new { copy = e.CommandArgument });
                break;
            case "delete":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_DeleteForum, new { fa = e.CommandArgument });
                break;
        }
    }

    /// <summary>
    /// Handle Commands for EditDelete a Category
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void CategoryList_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditCategory, new { c = e.CommandArgument });
                break;
            case "delete":
                if (this.GetRepository<Category>().DeleteById(e.CommandArgument.ToType<int>()))
                {
                    this.BindData();
                }
                else
                {
                    this.PageBoardContext.Notify(
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
    protected void NewCategory_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditCategory);
    }

    /// <summary>
    /// The new forum_ click.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void NewForum_Click(object sender, EventArgs e)
    {
        this.Get<LinkBuilder>().Redirect(ForumPages.Admin_EditForum);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsPostBack)
        {
            return;
        }

        this.PageSize.DataSource = StaticDataHelper.PageEntries();
        this.PageSize.DataTextField = "Name";
        this.PageSize.DataValueField = "Value";
        this.PageSize.DataBind();

        try
        {
            this.PageSize.SelectedValue = this.PageBoardContext.PageUser.PageSize.ToString();
        }
        catch (Exception)
        {
            this.PageSize.SelectedValue = "5";
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

        // a forum is followed by its sub forums (a sub forum with a lower sort order was shown above its parent)
        var forums = this.OrderAsTree(this.ListAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList());

        var themeButtonDelete = e.Item.FindControlAs<ThemeButton>("ThemeButtonDelete");
        var themeButton2 = e.Item.FindControlAs<ThemeButton>("ThemeButton2");

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
    /// The indent of a forum in the list (a level deeper per sub forum level, the third level was indented like the
    /// second).
    /// </summary>
    /// <param name="forum">The forum.</param>
    /// <returns>The indent.</returns>
    protected string ForumIndent(Forum forum)
    {
        return this.forumDepths.TryGetValue(forum.ID, out var depth) && depth > 0
                   ? string.Concat(Enumerable.Repeat("--", depth)) + "-"
                   : "-";
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMINMENU", "admin_forums"), string.Empty);
    }

    /// <summary>
    /// The sort categories ascending.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SortCategoriesAscending(object sender, EventArgs e)
    {
        // all categories of the board (the list of the page has only the entries of the current page)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList();

        this.GetRepository<Category>().ReOrderAllAscending(categories);

        this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);

        this.BindData();
    }

    /// <summary>
    /// The sort categories descending.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SortCategoriesDescending(object sender, EventArgs e)
    {
        // all categories of the board (the list of the page has only the entries of the current page)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList();

        this.GetRepository<Category>().ReOrderAllDescending(categories);

        this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_CATEGORIES"),
            MessageTypes.warning);

        this.BindData();
    }

    /// <summary>
    /// The sort forums ascending.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SortForumsAscending(object sender, EventArgs e)
    {
        // all categories of the board (the list of the page has only the entries of the current page)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList();

        if (categories.NullOrEmpty())
        {
            return;
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllAscending(forums);
            });

        this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.warning);

        this.BindData();
    }

    /// <summary>
    /// The sort forums descending.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void SortForumsDescending(object sender, EventArgs e)
    {
        // all categories of the board (the list of the page has only the entries of the current page)
        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList();

        if (categories.NullOrEmpty())
        {
            return;
        }

        categories.ForEach(category =>
            {
                var forums = listAll.Select(x => x.Item2).Where(x => x.CategoryID == category.ID).ToList();

                this.GetRepository<Forum>().ReOrderAllDescending(forums);
            });

        this.PageBoardContext.Notify(
            this.GetText("ADMIN_FORUMS", "MSG_SORTING_FORUMS"),
            MessageTypes.warning);

        this.BindData();
    }

    /// <summary>
    /// The pager top_ page change.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void PagerTopPageChange(object sender, EventArgs e)
    {
        // rebind
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void PageSizeSelectedIndexChanged(object sender, EventArgs e)
    {
        this.BindData();
    }

    /// <summary>
    /// Orders the forums of a category as a tree (each forum is followed by its sub forums), the depth of each forum
    /// is kept for its indent.
    /// </summary>
    /// <param name="forums">The forums of the category (sorted by the sort order).</param>
    /// <returns>The forums in the order of the tree.</returns>
    private List<Forum> OrderAsTree(List<Forum> forums)
    {
        var ordered = new List<Forum>(forums.Count);
        var ids = new HashSet<int>(forums.Select(f => f.ID));
        var children = forums.Where(f => f.ParentID.HasValue).ToLookup(f => f.ParentID.Value);

        void Add(Forum forum, int depth)
        {
            // a broken hierarchy (a forum below its own sub forum) is listed only once
            if (this.forumDepths.ContainsKey(forum.ID))
            {
                return;
            }

            this.forumDepths[forum.ID] = depth;
            ordered.Add(forum);

            foreach (var child in children[forum.ID])
            {
                Add(child, depth + 1);
            }
        }

        foreach (var forum in forums.Where(f => !f.ParentID.HasValue || !ids.Contains(f.ParentID.Value)))
        {
            Add(forum, 0);
        }

        foreach (var forum in forums.Where(f => !this.forumDepths.ContainsKey(f.ID)))
        {
            Add(forum, 0);
        }

        return ordered;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var baseSize = this.PageSize.SelectedValue.ToType<int>();
        this.PagerTop.PageSize = baseSize;

        var listAll = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID);

        // the pages have whole categories with all their forums (the rows were paged, a category with more forums than
        // the page size was shown on several pages), two categories with the same name are both shown
        var categories = listAll.Select(x => x.Item1).DistinctBy(x => x.ID).ToList().GetPaged(this.PagerTop);
        var categoryIds = new HashSet<int>(categories.Select(c => c.ID));

        this.ListAll = listAll.Where(x => categoryIds.Contains(x.Item1.ID)).ToList();

        this.CategoryList.DataSource = categories;

        // Hide the New Forum Button if there are no Categories.
        this.NewForum.Visible = this.CategoryList.Items.Count < 1;

        this.DataBind();
    }
}