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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using System.Linq;
using System.Threading.Tasks;

namespace YAF.Pages;

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The Team Page
/// </summary>
public class TeamModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "TeamModel" /> class.
    /// </summary>
    public TeamModel()
        : base("TEAM", ForumPages.Team)
    {
    }

    /// <summary>
    /// Gets or sets the admins.
    /// </summary>
    [BindProperty]
    public List<User> Admins { get; set; }

    /// <summary>
    ///   Gets or sets the Moderators List
    /// </summary>
    [BindProperty]
    public List<SimpleModerator> CompleteMods { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("TEAM", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public async Task<IActionResult> OnGetAsync()
    {
        if (!this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowTeamTo))
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// Go to Selected Forum, if one is Selected
    /// </summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var item = this.CompleteMods.Find(x => x.SelectedForumId.IsSet());

        if (item != null && item.SelectedForumId != "intro" && item.SelectedForumId != "break")
        {
            var forumId = item.SelectedForumId.ToType<int>();
            var forum = await this.GetRepository<Forum>().GetByIdAsync(forumId);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Topics, new { f = forumId, name = forum.Name });
        }

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        this.Admins = await this.GetAdminsAsync();

        this.CompleteMods = await this.GetModeratorsAsync();
    }

    /// <summary>
    /// Get all Admins.
    /// </summary>
    /// <returns>
    /// Moderators List
    /// </returns>
    private async Task<List<User>> GetAdminsAsync()
    {
        // get a row with user lazy data...
        var adminList = await this.Get<IDataCache>().GetOrSetAsync(
            Constants.Cache.BoardAdmins,
            () => this.GetRepository<User>().ListAdminsAsync(),
            TimeSpan.FromMinutes(this.PageBoardContext.BoardSettings.BoardModeratorsCacheTimeout));

        return adminList;
    }

    /// <summary>
    /// Get all Moderators Without Groups.
    /// </summary>
    /// <returns>
    /// Moderators List
    /// </returns>
    private async Task<List<SimpleModerator>> GetModeratorsAsync()
    {
        var moderators = await this.Get<DataBroker>().GetModeratorsAsync();

        var modsSorted = new List<SimpleModerator>();

        moderators.Where(m => !m.IsGroup).ForEach(
            mod =>
            {
                var sortedMod = mod;

                // Check if Mod is already in modsSorted
                if (modsSorted.Find(
                        s => s.Name.Equals(sortedMod.Name) && s.ModeratorID.Equals(sortedMod.ModeratorID)) != null)
                {
                    return;
                }

                // Get All Items from that MOD
                var modList = moderators.Where(m => m.Name.Equals(sortedMod.Name)).ToList();
                var forumsCount = modList.Count;

                sortedMod.ForumIDs = new ModeratorsForums[forumsCount];

                for (var i = 0; i < forumsCount; i++)
                {
                    var forumsId = new ModeratorsForums
                                   {
                                       CategoryID = modList[i].CategoryID,
                                       CategoryName = modList[i].CategoryName,
                                       ParentID = modList[i].ParentID,
                                       ForumID = modList[i].ForumID,
                                       ForumName = modList[i].ForumName
                                   };

                    sortedMod.ForumIDs[i] = forumsId;
                }

                modsSorted.Add(sortedMod);
            });

        return modsSorted;
    }
}