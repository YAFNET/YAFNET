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

namespace YAF.Pages.Moderate;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// Base root control for moderating, linking to other moderating controls/pages.
/// </summary>
public class ModerateModel : ModerateForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModerateModel"/> class.
    /// </summary>
    public ModerateModel()
        : base("MODERATE_DEFAULT", ForumPages.Moderate_Moderate)
    {
    }

    /// <summary>
    /// Gets or sets the categories.
    /// </summary>
    public List<Category> Categories { get; set; }

    /// <summary>
    /// Gets or sets the forums.
    /// </summary>
    public List<ModerateForum> Forums { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        // moderation index
        this.PageBoardContext.PageLinks.AddLink(this.GetText("MODERATE_DEFAULT", "MODERATEINDEX_TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        // bind data
        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// The on post unapproved.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<RedirectToPageResult> OnPostUnapprovedAsync(int forumId)
    {
        return Task.FromResult(this.RedirectToPage(
            ForumPages.Moderate_UnapprovedPosts.GetPageName(),
            new { f = forumId }));
    }

    /// <summary>
    /// The on post reported.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public Task<RedirectToPageResult> OnPostReportedAsync(int forumId)
    {
        return Task.FromResult(this.RedirectToPage(
            ForumPages.Moderate_ReportedPosts.GetPageName(),
            new { f = forumId }));
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private void BindData()
    {
        this.Forums = this.GetRepository<Forum>()
            .ModerateList(this.PageBoardContext.PageUserID, this.PageBoardContext.PageBoardID);

        this.Categories = [.. this.GetRepository<Category>().GetByBoardId().OrderBy(c => c.SortOrder)];
    }
}