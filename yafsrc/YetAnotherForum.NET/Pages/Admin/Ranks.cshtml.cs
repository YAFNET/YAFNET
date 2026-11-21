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
using System.Linq;

using YAF.Core.Extensions;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Services;
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
    /// Gets or sets the ranks of the ladder (by their minimum posts, the users are promoted from one to the next).
    /// </summary>
    public List<Rank> LadderRanks { get; set; } = [];

    /// <summary>
    /// Gets or sets the ranks outside the ladder (by their priority).
    /// </summary>
    public List<Rank> OtherRanks { get; set; } = [];

    /// <summary>
    /// Gets or sets the number of users of each rank.
    /// </summary>
    public Dictionary<int, int> UserCounts { get; set; } = [];

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
    /// Gets the style of the rank name (the style can be set for each theme).
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <returns>Returns the style</returns>
    public string GetStyle(Rank rank)
    {
        return rank.Style.IsSet() ? this.Get<IStyleTransform>().Decode(rank.Style) : string.Empty;
    }

    /// <summary>
    /// Gets the number of users of the rank.
    /// </summary>
    /// <param name="rank">The rank.</param>
    /// <returns>Returns the number of users</returns>
    public int GetUserCount(Rank rank)
    {
        return this.UserCounts.TryGetValue(rank.ID, out var count) ? count : 0;
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        return this.BindDataAsync();
    }

    /// <summary>
    /// Called when [post delete].
    /// </summary>
    /// <param name="id">The identifier.</param>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        // a rank that users have can't be deleted (the users reference it)
        if (await this.GetRepository<User>().ExistsAsync(u => u.RankID == id))
        {
            await this.BindDataAsync();

            return this.PageBoardContext.Notify(this.GetText("ADMIN_RANKS", "MSG_NOT_DELETE"), MessageTypes.warning);
        }

        // the start rank of the new users (without it nobody can register)
        if (await this.GetRepository<Rank>().ExistsAsync(r => r.ID == id && (r.Flags & 1) == 1))
        {
            await this.BindDataAsync();

            return this.PageBoardContext.Notify(this.GetText("ADMIN_RANKS", "MSG_START_RANK"), MessageTypes.warning);
        }

        await this.GetRepository<Rank>().DeleteAsync(x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID);
        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.List = await this.GetRepository<Rank>().GetByBoardIdAsync();

        this.LadderRanks = [.. this.List.Where(r => r.RankFlags.IsLadder).OrderBy(r => r.MinPosts ?? 0).ThenBy(r => r.SortOrder)];
        this.OtherRanks = [.. this.List.Where(r => !r.RankFlags.IsLadder).OrderBy(r => r.SortOrder).ThenBy(r => r.Name)];

        var users = this.GetRepository<User>();

        foreach (var rank in this.List)
        {
            this.UserCounts[rank.ID] = (int)await users.CountAsync(u => u.RankID == rank.ID);
        }
    }
}