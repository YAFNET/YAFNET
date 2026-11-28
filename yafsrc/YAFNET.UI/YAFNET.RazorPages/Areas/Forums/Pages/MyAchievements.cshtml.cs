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

namespace YAF.Pages;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// The My Achievements page: the achievements the user has unlocked and the progress of the others.
/// </summary>
public class MyAchievementsModel : ForumPageRegistered
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MyAchievementsModel"/> class.
    /// </summary>
    public MyAchievementsModel()
        : base("ACHIEVEMENTS", ForumPages.MyAchievements)
    {
    }

    /// <summary>
    /// Gets or sets the achievements (unlocked or not, with the progress).
    /// </summary>
    public List<AchievementInfo> Achievements { get; set; } = [];

    /// <summary>
    /// Gets the unlocked achievements.
    /// </summary>
    public List<AchievementInfo> Unlocked => [.. this.Achievements.Where(a => a.Unlocked)];

    /// <summary>
    /// Gets the achievement points of the user.
    /// </summary>
    public int Points => this.Unlocked.Sum(a => a.Points);

    /// <summary>
    /// Gets the most points the user can get.
    /// </summary>
    public int MaxPoints => this.Achievements.Sum(a => a.Points);

    /// <summary>
    /// Gets the rarest unlocked achievement (or null).
    /// </summary>
    public AchievementInfo Rarest =>
        this.Unlocked.OrderByDescending(a => a.Rarity).ThenByDescending(a => a.Points).FirstOrDefault();

    /// <summary>
    /// Gets the locked achievement the user is closest to (or null).
    /// </summary>
    public AchievementInfo Next =>
        this.Achievements.Where(a => !a.Unlocked).OrderByDescending(a => a.Progress).ThenBy(a => a.Points)
            .FirstOrDefault();

    /// <summary>
    /// Creates the page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageUser.DisplayOrUserName(),
            this.Get<ILinkBuilder>().GetLink(ForumPages.MyAccount));

        this.PageBoardContext.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// Shows the achievements of the user.
    /// </summary>
    /// <returns>The page.</returns>
    public async Task<IActionResult> OnGetAsync()
    {
        if (!this.PageBoardContext.BoardSettings.EnableAchievements)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        var service = this.Get<IAchievementService>();

        // the goals the user has reached since the last check are awarded before they are shown as almost done
        await service.CheckUserAsync(this.PageBoardContext.PageUserID);

        this.Achievements = await service.GetAchievementInfosAsync(
                                this.PageBoardContext.PageBoardID,
                                this.PageBoardContext.PageUserID,
                                true);

        return this.Page();
    }
}
