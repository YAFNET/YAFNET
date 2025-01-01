
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

    public List<SelectListItem> ForumImages { get; set; }

    public SelectList Categories { get; set; }

    public List<SelectListItem> ParentForums { get; set; }

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
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums));
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITFORUM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Create images list.
    /// </summary>
    protected void CreateImagesList()
    {
        var list = new List<SelectListItem> {new(this.GetText("COMMON", "NONE"), "") };

        var dir = new DirectoryInfo(
            Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Forums));

        if (dir.Exists)
        {
            var files = dir.GetFiles("*.*").ToList();

            list.AddImageFiles(files, this.Get<BoardFolders>().Categories);
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

    public IActionResult OnPost(int? fa = null, bool copy = false)
    {
        return this.BindData(fa, copy);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    /// <param name="forumId"></param>
    /// <param name="copy"></param>
    private IActionResult BindData(int? forumId, bool copy = false)
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

        if (forumId.HasValue)
        {
            this.AccessList = this.GetRepository<ForumAccess>().GetForumAccessList(forumId.Value).Select(
                i => new GroupAccessMask
                     {GroupID = i.Item2.ID, GroupName = i.Item2.Name, AccessMaskID = i.Item1.AccessMaskID}).ToList();
        }
        else
        {
            this.AccessList = this.PageBoardContext.GetRepository<Group>().GetByBoardId().Select(
                i => new GroupAccessMask {GroupID = i.ID, GroupName = i.Name, AccessMaskID = this.PageBoardContext.BoardSettings.ForumDefaultAccessMask }).ToList();
        }

        // Load forum's themes
        var themes = StaticDataHelper.Themes().ToList();

        themes.Insert(0, new SelectListItem(this.GetText("ADMIN_EDITFORUM", "CHOOSE_THEME"), string.Empty));

        this.Themes = themes;

        if (!forumId.HasValue)
        {
            var sortOrder = 1;

            try
            {
                // Currently creating a New Forum, and auto fill the Forum Sort Order + 1
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
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
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
            return this.PageBoardContext.Notify(this.GetText("ADMIN_EDITFORUM", "MSG_CATEGORY"), MessageTypes.warning);
        }

        // Forum
        int? forumId = this.Input.Id == 0 ? null : this.Input.Id;

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
                return this.PageBoardContext.Notify(this.GetText("ADMIN_EDITFORUM", "MSG_PARENT_SELF"), MessageTypes.warning);
            }

            if (this.GetRepository<Forum>().IsParentsChecker(forumId.Value))
            {
                return this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITFORUM", "MSG_CHILD_PARENT"),
                    MessageTypes.warning);
            }
        }

        // duplicate name checking...
        if (!forumId.HasValue && !this.Input.Copy)
        {
            var forumList = await this.GetRepository<Forum>().GetAsync(f => f.Name == this.Input.Name.Trim());

            if (forumList.Count != 0 && !this.PageBoardContext.BoardSettings.AllowForumsWithSameName)
            {
                return this.PageBoardContext.Notify(
                    this.GetText("ADMIN_EDITFORUM", "MSG_FORUMNAME_EXISTS"),
                    MessageTypes.warning);
            }
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

        if (this.Input.ImageURL.IsSet() && this.Input.ImageURL.Equals(this.GetText("COMMON", "NONE")))
        {
            this.Input.ImageURL = null;
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
            this.Input.ImageURL,
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

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Forums);
    }
}