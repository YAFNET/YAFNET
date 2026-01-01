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

using System.Linq;

namespace YAF.Pages.Moderate;

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
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
    public async Task<IActionResult> OnGetAsync()
    {
        // bind data
        await this.BindDataAsync();

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
    public IActionResult OnPostUnapproved(int forumId)
    {
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Moderate_UnapprovedPosts,
            new { f = forumId });
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
    public IActionResult OnPostReported(int forumId)
    {
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Moderate_ReportedPosts,
            new { f = forumId });
    }

    /// <summary>
    /// Bind data for this control.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.Forums = this.GetRepository<Forum>()
            .ModerateList(this.PageBoardContext.PageUserID, this.PageBoardContext.PageBoardID);

        var categories = await this.GetRepository<Category>().GetByBoardIdAsync();

        this.Categories = [.. categories.OrderBy(c => c.SortOrder)];
    }
}