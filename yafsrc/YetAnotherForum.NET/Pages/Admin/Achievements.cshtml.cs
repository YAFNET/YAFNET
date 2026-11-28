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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The Admin Achievements page: the achievements (trophies) the users unlock automatically.
/// </summary>
public class AchievementsModel : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AchievementsModel"/> class.
    /// </summary>
    public AchievementsModel()
        : base("ADMIN_ACHIEVEMENTS", ForumPages.Admin_Achievements)
    {
    }

    /// <summary>
    /// Gets or sets the achievements of the board by type.
    /// </summary>
    public List<IGrouping<AchievementType, Achievement>> AchievementGroups { get; set; } = [];

    /// <summary>
    /// Gets or sets the number of the users that have unlocked the achievements.
    /// </summary>
    public Dictionary<int, int> UserCounts { get; set; } = [];

    /// <summary>
    /// Gets the number of the achievements.
    /// </summary>
    public int Count => this.AchievementGroups.Sum(g => g.Count());

    /// <summary>
    /// Creates the page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ACHIEVEMENTS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Shows the achievements.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }

    /// <summary>
    /// Deletes an achievement (the users lose it).
    /// </summary>
    /// <param name="achievementId">The achievement id.</param>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int achievementId)
    {
        // only achievements of the board
        var achievement = await this.GetRepository<Achievement>().GetSingleAsync(
                              a => a.ID == achievementId && a.BoardID == this.PageBoardContext.PageBoardID);

        if (achievement != null)
        {
            var users = (await this.GetRepository<UserAchievement>().GetAsync(u => u.AchievementID == achievementId))
                .Select(u => u.UserID).ToList();

            await this.GetRepository<UserAchievement>().DeleteAsync(u => u.AchievementID == achievementId);
            await this.GetRepository<Activity>().DeleteAsync(a => a.AchievementID == achievementId);
            await this.GetRepository<Achievement>().DeleteAsync(a => a.ID == achievementId);

            this.Get<IAchievementService>().ClearBoardCache(this.PageBoardContext.PageBoardID);
            users.ForEach(this.Get<IAchievementService>().ClearUserCache);

            this.PageBoardContext.SessionNotify(
                this.GetTextFormatted("MSG_DELETED", this.HtmlEncode(this.Get<IAchievementService>().GetName(achievement))),
                MessageTypes.success);
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Achievements);
    }

    /// <summary>
    /// Awards the achievements to all members that have reached them (e.g. after a new achievement was added).
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnPostCheckAllAsync()
    {
        if (!this.PageBoardContext.BoardSettings.EnableAchievements)
        {
            this.PageBoardContext.SessionNotify(this.GetText("MSG_DISABLED"), MessageTypes.warning);

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Achievements);
        }

        var count = await this.Get<IAchievementService>().CheckBoardAsync(this.PageBoardContext.PageBoardID);

        this.PageBoardContext.SessionNotify(this.GetTextFormatted("MSG_CHECKED", count), MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Achievements);
    }

    /// <summary>
    /// Gets the question of the delete dialog (names the users that lose the achievement).
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <returns>The question.</returns>
    public string GetDeleteText(Achievement achievement)
    {
        return this.GetTextFormatted(
            "CONFIRM_DELETE",
            this.HtmlEncode(this.Get<IAchievementService>().GetName(achievement)),
            this.UserCounts.GetValueOrDefault(achievement.ID));
    }

    /// <summary>
    /// Loads the achievements.
    /// </summary>
    private void BindData()
    {
        var boardId = this.PageBoardContext.PageBoardID;

        // not the cached list, the page shows the current state
        this.Get<IAchievementService>().ClearBoardCache(boardId);

        var achievements = this.Get<IAchievementService>().GetBoardAchievements(boardId);

        this.AchievementGroups = [.. achievements.GroupBy(a => a.AchievementType).OrderBy(g => g.Key)];

        this.UserCounts = this.GetRepository<UserAchievement>().CountUsers([.. achievements.Select(a => a.ID)]);
    }
}
