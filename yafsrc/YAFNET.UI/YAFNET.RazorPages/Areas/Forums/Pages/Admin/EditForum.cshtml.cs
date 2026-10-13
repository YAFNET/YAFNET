
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Objects;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

using Microsoft.AspNetCore.Mvc.Rendering;

/// <summary>
/// Administrative Page for the editing of forum properties.
/// </summary>
public class EditForumModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditForumInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public List<GroupAccessMask> AccessList { get; set; }

    /// <summary>
    /// The access mask list.
    /// </summary>
    public SelectList AccessMaskList { get; set; }

    /// <summary>
    /// Gets or sets the forum images.
    /// </summary>
    /// <value>The forum images.</value>
    public List<SelectListItem> ForumImages { get; set; }

    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    /// <value>The categories.</value>
    public SelectList Categories { get; set; }

    /// <summary>
    /// Gets or sets the parent forums.
    /// </summary>
    /// <value>The parent forums.</value>
    public List<SelectListItem> ParentForums { get; set; }

    /// <summary>
    /// Gets or sets the themes.
    /// </summary>
    /// <value>The themes.</value>
    public IReadOnlyCollection<SelectListItem> Themes { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditForumModel"/> class.
    /// </summary>
    public EditForumModel()
        : base("ADMIN_EDITFORUM", ForumPages.Admin_EditForum)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMINMENU", "ADMIN_FORUMS"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Create images list.
    /// </summary>
    protected void CreateImagesList()
    {
        var list = new List<SelectListItem> { new(this.GetText("COMMON", "NONE"), "") };

        var dir = new DirectoryInfo(
            Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Forums));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Forums);
        }

        this.ForumImages = list;
    }

    /// <summary>
    /// Called when [get].
    /// </summary>
    /// <param name="fa">The edit forum id.</param>
    /// <param name="copy">The copy forum id.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int? fa = null, bool copy = false)
    {
        this.Input = new EditForumInputModel();

        return this.BindData(fa, copy);
    }

    /// <summary>
    /// The category was changed (the select submits the form): the entered values are kept,
    /// the parent forums are the ones of the new category.
    /// </summary>
    /// <param name="fa">The forum identifier.</param>
    /// <param name="copy">The copy forum id.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPost(int? fa = null, bool copy = false)
    {
        this.BindLists(this.Input.Id == 0 ? null : this.Input.Id);

        return this.Page();
    }

    /// <summary>
    /// Binds the lists of the page (also when the page is shown again after a post).
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    private void BindLists(int? forumId)
    {
        this.AccessMaskList = new SelectList(
            this.GetRepository<AccessMask>().GetByBoardId(),
            nameof(AccessMask.ID),
            nameof(AccessMask.Name));

        // Populate Forum Images
        this.CreateImagesList();

        this.BindCategoryList();

        // populate parent forums list with forums according to selected category
        this.BindParentList();

        var groups = this.PageBoardContext.GetRepository<Group>().GetByBoardId();

        if (!this.AccessList.NullOrEmpty())
        {
            // the posted access masks, the group names are not posted
            this.AccessList.ForEach(a => a.GroupName = groups.FirstOrDefault(g => g.ID == a.GroupID)?.Name);
        }
        else if (forumId.HasValue)
        {
            this.AccessList = [.. this.GetRepository<ForumAccess>().GetForumAccessList(forumId.Value).Select(
                i => new GroupAccessMask
                     {GroupID = i.Item2.ID, GroupName = i.Item2.Name, AccessMaskID = i.Item1.AccessMaskID})];
        }
        else
        {
            this.AccessList = [.. groups.Select(
                i => new GroupAccessMask {GroupID = i.ID, GroupName = i.Name, AccessMaskID = this.PageBoardContext.BoardSettings.ForumDefaultAccessMask })];
        }

        // Load forum's themes
        var themes = StaticDataHelper.Themes().ToList();

        themes.Insert(0, new SelectListItem(this.GetText("ADMIN_EDITFORUM", "CHOOSE_THEME"), string.Empty));

        this.Themes = themes;
    }

    /// <summary>
    /// Shows a warning on the page (with the lists, the page is shown again with the entered values).
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>IActionResult.</returns>
    private IActionResult Warning(string message)
    {
        this.BindLists(this.Input.Id == 0 ? null : this.Input.Id);

        return this.PageBoardContext.Notify(message, MessageTypes.warning);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="forumId"></param>
    /// <param name="copy"></param>
    private IActionResult BindData(int? forumId, bool copy = false)
    {
        this.BindLists(forumId);

        if (!forumId.HasValue)
        {
            var sortOrder = 1;

            try
            {
                // Currently creating a New Forum, and autofill the Forum Sort Order + 1
                var forumCheck = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID)
                    .MaxBy(a => a.Item2.SortOrder);

                sortOrder = forumCheck.Item2.SortOrder + sortOrder;
            }
            catch
            {
                sortOrder = 1;
            }

            this.Input.SortOrder = sortOrder;

            return this.Page();
        }

        var forum = this.GetRepository<Forum>().GetById(forumId.Value);

        if (forum is null)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Input.Id = forum.ID;
        this.Input.Copy = copy;

        this.Input.Name = forum.Name;
        this.Input.Description = forum.Description;
        this.Input.SortOrder = forum.SortOrder;
        this.Input.HideNoAccess = forum.ForumFlags.IsHidden;
        this.Input.Locked = forum.ForumFlags.IsLocked;
        this.Input.IsTest = forum.ForumFlags.IsTest;
        this.Input.Moderated = forum.ForumFlags.IsModerated;

        if (!forum.ModeratedPostCount.HasValue)
        {
            this.Input.ModerateAllPosts = true;
        }
        else
        {
            this.Input.ModerateAllPosts = false;
            this.Input.ModeratedPostCount = forum.ModeratedPostCount.Value;
        }

        this.Input.ModerateNewTopicOnly = forum.IsModeratedNewTopicOnly;

        this.Input.Styles = forum.Styles;

        this.Input.CategoryID = forum.CategoryID;

        this.Input.ParentID = forum.ParentID;

        this.Input.ThemeURL = forum.ThemeURL;

        this.Input.RemoteURL = forum.RemoteURL;

        if (forum.ImageURL.IsSet())
        {
            this.Input.ImageURL = !forum.ImageURL.Contains('/')
                ? $"/{this.Get<BoardFolders>().Forums}/{forum.ImageURL}"
                : forum.ImageURL;
        }

        return this.Page();
    }

    /// <summary>
    /// Binds the parent list.
    /// </summary>
    private void BindParentList()
    {
        this.ParentForums = [];

        var forums = this.GetRepository<Forum>().ListAllFromCategory(this.Input.CategoryID);

        forums.Insert(0, new ForumSorted {ForumID = 0, Forum = this.GetText("NONE")});

        forums.ForEach(
            forum =>
            {
                this.ParentForums.Add(new SelectListItem(forum.Forum, forum.ForumID.ToString()));
            });
    }

    private void BindCategoryList()
    {
        var categories = this.GetRepository<Category>().List();

        if (this.Input.CategoryID == 0)
        {
            this.Input.CategoryID = categories.FirstOrDefault()!.ID;
        }

        this.Categories = new SelectList(
            categories,
            nameof(Category.ID),
            nameof(Category.Name));
    }

    /// <summary>
    /// Save the Forum
    /// </summary>
    public async Task<IActionResult> OnPostSaveAsync(int? fa = null, bool copy = false)
    {
        if (this.Input.CategoryID == 0)
        {
            return this.Warning(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"));
        }

        // Forum (a copy is a new forum, the id is the one of the copied forum)
        int? forumId = this.Input.Id == 0 || this.Input.Copy ? null : this.Input.Id;

        int? parentId = null;

        if (this.Input.ParentID > 0)
        {
            parentId = this.Input.ParentID;
        }

        // The picked forum cannot be a child forum as it's a parent
        // If we update a forum ForumID > 0
        if (forumId.HasValue && parentId.HasValue && !this.Input.Copy)
        {
            // check if parent and forum is the same
            if (parentId.Value == forumId.Value)
            {
                return this.Warning(this.GetText("ADMIN_EDITFORUM", "MSG_PARENT_SELF"));
            }

            // the parent can't be one of the sub forums (a forum with sub forums can still have a parent)
            if (this.GetRepository<Forum>().IsOwnSubForum(forumId.Value, parentId.Value))
            {
                return this.Warning(this.GetText("ADMIN_EDITFORUM", "MSG_CHILD_PARENT"));
            }
        }

        // duplicate name checking...
        var forumName = this.Input.Name.Trim();

        // the name must be unique in the category (also when a forum is renamed or copied)
        var nameExists = await this.GetRepository<Forum>().ExistsAsync(
            f => f.CategoryID == this.Input.CategoryID && f.Name == forumName && f.ID != (forumId ?? 0));

        // other forums of the board with the same name (if not allowed)
        if (!nameExists && !forumId.HasValue && !this.Input.Copy && !this.PageBoardContext.BoardSettings.AllowForumsWithSameName)
        {
            nameExists = this.GetRepository<Forum>().ListAll(this.PageBoardContext.PageBoardID)
                .Exists(f => f.Item2 != null && f.Item2.Name == forumName);
        }

        if (nameExists)
        {
            return this.Warning(this.GetText("ADMIN_EDITFORUM", "MSG_FORUMNAME_EXISTS"));
        }

        var themeUrl = string.Empty;

        if (this.Input.ThemeURL.IsSet())
        {
            themeUrl = this.Input.ThemeURL;
        }

        int? moderatedPostCount = null;

        if (!this.Input.ModerateAllPosts)
        {
            moderatedPostCount = this.Input.ModeratedPostCount;
        }

        string imageUrl = null;

        if (this.Input.ImageURL.IsSet() && this.Input.ImageURL != this.GetText("COMMON", "NONE"))
        {
            imageUrl = this.Input.ImageURL[(this.Input.ImageURL.LastIndexOf('/') + 1)..];
        }

        var newForumId = this.GetRepository<Forum>().Save(
            forumId,
            this.Input.CategoryID,
            parentId,
            this.Input.Name.Trim(),
            this.Input.Description.Trim(),
            this.Input.SortOrder,
            this.Input.Locked,
            this.Input.HideNoAccess,
            this.Input.IsTest,
            this.Input.Moderated,
            moderatedPostCount,
            this.Input.ModerateNewTopicOnly,
            this.Input.RemoteURL,
            themeUrl,
            imageUrl,
            this.Input.Styles);

        // Access
        if (forumId.HasValue)
        {
            foreach (var item in this.AccessList)
            {
                var groupId = item.GroupID;

                await this.GetRepository<ForumAccess>().SaveAsync(
                    newForumId,
                    groupId,
                    item.AccessMaskID);
            }
        }
        else
        {
            this.AccessList.ForEach(
                item =>
                {
                    var groupId = item.GroupID;

                    this.GetRepository<ForumAccess>().Create(
                        newForumId,
                        groupId,
                        item.AccessMaskID);
                });
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }

    /// <summary>
    /// Only forums of the current board can be edited, also the category and parent forum must be of the board (all handlers, the id is sent by the client).
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(Microsoft.AspNetCore.Mvc.Filters.PageHandlerExecutingContext context)
    {
        // the forum of the route and the forum of the form (the save uses the form)
        var id = context.HandlerArguments.TryGetValue("fa", out var argument) && argument is int argumentId ? argumentId : 0;

        if (id > 0 && !this.IsBoardForum(id)
            || this.Input?.Id is > 0 && !this.IsBoardForum(this.Input.Id)
            || this.Input is not null && (this.Input.CategoryID > 0 && !this.GetRepository<Category>().Exists(
                                              c => c.ID == this.Input.CategoryID && c.BoardID == this.PageBoardContext.PageBoardID)
                                          || this.Input.ParentID is > 0 && !this.IsBoardForum(this.Input.ParentID.Value)))
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Checks if the forum belongs to the current board.
    /// </summary>
    /// <param name="forumId">The forum identifier.</param>
    /// <returns>Returns if the forum belongs to the current board.</returns>
    private bool IsBoardForum(int forumId)
    {
        var forum = this.GetRepository<Forum>().GetById(forumId);

        return forum != null && this.GetRepository<Category>().Exists(
                   c => c.ID == forum.CategoryID && c.BoardID == this.PageBoardContext.PageBoardID);
    }
}