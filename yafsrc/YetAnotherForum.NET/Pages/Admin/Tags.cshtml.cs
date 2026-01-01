
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

using YAF.Types.Extensions;

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Models;

/// <summary>
/// The Admin Manage Tags Page.
/// </summary>
public class TagsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<Tag> List { get; set; }

    /// <summary>
    /// Gets or sets the count.
    /// </summary>
    /// <value>The count.</value>
    [BindProperty]
    public int Count { get; set; }

    /// <summary>
    ///   Initializes a new instance of the <see cref = "TagsModel" /> class.
    /// </summary>
    public TagsModel()
        : base("ADMIN_TAGS", ForumPages.Admin_Tags)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index second
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_TAGS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        // bind data to controls
        return this.BindDataAsync();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public Task OnPostAsync()
    {
        return this.BindDataAsync();
    }

    /// <summary>
    /// Handles single record commands in a repeater.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await this.GetRepository<TopicTag>().DeleteAsync(x => x.TagID == id);

        await this.GetRepository<Tag>().DeleteByIdAsync(id);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Tags);
    }

    /// <summary>
    /// Populates data source and binds data to controls.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        var currentPageIndex = this.PageBoardContext.PageIndex;

        // list event for this board
        this.List = await this.GetRepository<Tag>().GetPagedAsync(
            x => x.BoardID == this.PageBoardContext.PageBoardID,
            currentPageIndex,
            this.Size);

        this.Count = 0;

        if (!this.List.NullOrEmpty())
        {
            var tags =  await this.GetRepository<Tag>().GetByBoardIdAsync();

            this.Count = tags.Count;
        }
    }
}