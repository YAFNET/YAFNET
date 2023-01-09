/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Extensions;
using YAF.Types.Models;

/// <summary>
/// Admin Ranks Page
/// </summary>
public class RanksModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IList<Rank> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RanksModel"/> class. 
    /// </summary>
    public RanksModel()
        : base("ADMIN_RANKS", ForumPages.Admin_Ranks)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RANKS", "TITLE"));
    }

    /// <summary>
    /// Format string color.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    /// <returns>
    /// Set values  are rendered green if true, and if not red
    /// </returns>
    public string GetItemColorString(string item)
    {
        // show enabled flag red
        return item.IsSet() ? "badge bg-success" : "badge bg-danger";
    }

    /// <summary>
    /// Format access mask setting color formatting.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Set access mask flags  are rendered green if true, and if not red
    /// </returns>
    public string GetItemColor(bool enabled)
    {
        // show enabled flag red
        return enabled ? "badge bg-success" : "badge bg-danger";
    }

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Item Name.
    /// </returns>
    public string GetItemName(bool enabled)
    {
        return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }

    /// <summary>
    /// Is Rank Ladder?
    /// </summary>
    /// <param name="rank">
    /// The rank.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string LadderInfo([NotNull] Rank rank)
    {
        var isLadder = rank.RankFlags.IsLadder;

        return isLadder
                   ? $"{this.GetItemName(true)} ({rank.MinPosts} {this.GetText("ADMIN_RANKS", "POSTS")})"
                   : this.GetItemName(false);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }

    public void OnPostDelete(int id)
    {
        this.GetRepository<Rank>().DeleteById(id);
        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.List = this.GetRepository<Rank>().GetByBoardId();
    }
}