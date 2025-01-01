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

namespace YAF.Pages.Admin;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Flags;
using YAF.Types.Models;

/// <summary>
/// Add or Edit User Rank Page.
/// </summary>
public class EditRankModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditRankInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditRankModel"/> class.
    /// </summary>
    public EditRankModel()
        : base("ADMIN_EDITRANK", ForumPages.Admin_EditRank)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_RANKS", "TITLE"), this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Ranks));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITRANK", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? r = null)
    {
        this.Input = new EditRankInputModel();

        if (!r.HasValue)
        {
            return this.Page();
        }

        var rankId = r.Value;
        var rank = this.GetRepository<Rank>().GetById(rankId);

        if (rank is null)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.Input.Id = rankId;
        this.Input.Name = rank.Name;
        this.Input.IsStart = rank.RankFlags.IsStart;
        this.Input.IsLadder = rank.RankFlags.IsLadder;
        this.Input.MinPosts = rank.MinPosts ?? 0;
        this.Input.Style = rank.Style;
        this.Input.RankPriority = rank.SortOrder;
        this.Input.UsrAlbums = rank.UsrAlbums;
        this.Input.UsrAlbumImages = rank.UsrAlbumImages;
        this.Input.UsrSigChars = rank.UsrSigChars;
        this.Input.UsrSigBBCodes = rank.UsrSigBBCodes;
        this.Input.Description = rank.Description;

        return this.Page();
    }

    /// <summary>
    /// Save (New) Rank
    /// </summary>
    public IActionResult OnPostSave()
    {
        // Rank
        int? rankId = null;

        if (this.Input.Id > 0)
        {
            rankId = this.Input.Id;
        }

        this.GetRepository<Rank>().Save(
            rankId,
            this.PageBoardContext.PageBoardID,
            this.Input.Name,
            new RankFlags { IsStart = this.Input.IsStart, IsLadder = this.Input.IsLadder },
            this.Input.MinPosts,
            this.Input.Style,
            this.Input.RankPriority,
            this.Input.Description,
            this.Input.UsrSigChars,
            this.Input.UsrSigBBCodes,
            this.Input.UsrAlbums,
            this.Input.UsrAlbumImages);

        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_Ranks);
    }
}