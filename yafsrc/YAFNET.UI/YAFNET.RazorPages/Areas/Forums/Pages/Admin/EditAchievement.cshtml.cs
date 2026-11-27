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

using Microsoft.AspNetCore.Mvc.Filters;

using YAF.Core.Extensions;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;

/// <summary>
/// The Admin Edit Achievement page.
/// </summary>
public class EditAchievementModel : AdminPage
{
    /// <summary>
    /// The highest goal and the most points.
    /// </summary>
    public const int MaxValue = 1000000;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditAchievementModel"/> class.
    /// </summary>
    public EditAchievementModel()
        : base("ADMIN_EDITACHIEVEMENT", ForumPages.Admin_EditAchievement)
    {
    }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public EditAchievementInputModel Input { get; set; }

    /// <summary>
    /// Gets or sets the number of the users that have unlocked the achievement.
    /// </summary>
    public int UserCount { get; set; }

    /// <summary>
    /// Gets the types (value, name).
    /// </summary>
    public List<(int Value, string Name)> Types =>
    [
        .. Enum.GetValues<AchievementType>().Select(
            t => ((int)t, this.Get<IAchievementService>().GetTypeName(t)))
    ];

    /// <summary>
    /// Gets the rarities (value, name, class).
    /// </summary>
    public List<(int Value, string Name, string Class)> Rarities =>
    [
        .. Enum.GetValues<AchievementRarity>().Select(
            r => ((int)r, this.Get<IAchievementService>().GetRarityName(r), $"rarity-{r.ToString().ToLowerInvariant()}"))
    ];

    /// <summary>
    /// Creates the page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_ACHIEVEMENTS", "TITLE"),
            this.Get<ILinkBuilder>().GetLink(ForumPages.Admin_Achievements));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITACHIEVEMENT", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Only the achievements of the board can be edited.
    /// </summary>
    /// <param name="context">The context.</param>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        var id = context.HandlerArguments.TryGetValue("achievementId", out var argument) && argument is int argumentId
                     ? argumentId
                     : this.Input?.Id;

        if (id is not (null or 0) && !this.GetRepository<Achievement>()
                .Exists(x => x.ID == id && x.BoardID == this.PageBoardContext.PageBoardID))
        {
            context.Result = this.Get<ILinkBuilder>().AccessDenied();
        }
    }

    /// <summary>
    /// Shows a new or an existing achievement.
    /// </summary>
    /// <param name="achievementId">The achievement id (none for a new achievement).</param>
    public void OnGet(int? achievementId = null)
    {
        this.Input = new EditAchievementInputModel();

        if (!achievementId.HasValue)
        {
            return;
        }

        var achievement = this.GetRepository<Achievement>().GetById(achievementId.Value);

        if (achievement is null)
        {
            return;
        }

        var service = this.Get<IAchievementService>();

        this.Input.Id = achievement.ID;
        this.Input.Name = service.GetName(achievement);
        this.Input.Description = service.GetDescription(achievement);
        this.Input.Icon = achievement.Icon;
        this.Input.Type = achievement.Type;
        this.Input.Threshold = achievement.Threshold;
        this.Input.HasExtraGoal = achievement.HasExtraGoal;
        this.Input.ExtraType = achievement.ExtraType ?? (int)AchievementType.ReactionsReceived;
        this.Input.ExtraThreshold = achievement.ExtraThreshold ?? 1;
        this.Input.Points = achievement.Points;
        this.Input.Rarity = achievement.Rarity;
        this.Input.IsActive = achievement.IsActive;

        this.UserCount = (int)this.GetRepository<UserAchievement>().Count(u => u.AchievementID == achievement.ID);
    }

    /// <summary>
    /// Saves the achievement.
    /// </summary>
    /// <returns>The achievements page, or the page again with a message.</returns>
    public async Task<IActionResult> OnPostSaveAsync()
    {
        // a name of spaces is no name
        this.Input.Name = this.Input.Name?.Trim();
        this.Input.Description = this.Input.Description?.Trim();

        var message = this.Validate();

        if (message != null)
        {
            if (this.Input.Id > 0)
            {
                this.UserCount = (int)await this.GetRepository<UserAchievement>().CountAsync(u => u.AchievementID == this.Input.Id);
            }

            return this.PageBoardContext.Notify(this.GetText("ADMIN_EDITACHIEVEMENT", message), MessageTypes.warning);
        }

        var achievement = this.Input.Id > 0
                              ? await this.GetRepository<Achievement>().GetByIdAsync(this.Input.Id)
                              : new Achievement { BoardID = this.PageBoardContext.PageBoardID };

        var service = this.Get<IAchievementService>();

        // a default achievement keeps the texts of the language files (shown in the language of the user) until
        // the administrator changes them
        var defaultName = achievement.TextKey.IsSet()
                              ? this.GetText("ACHIEVEMENT_DEFAULTS", $"{achievement.TextKey}_NAME")
                              : null;
        var defaultDescription = achievement.TextKey.IsSet()
                                     ? this.GetText("ACHIEVEMENT_DEFAULTS", $"{achievement.TextKey}_DESC")
                                     : null;

        achievement.Name = this.Input.Name == defaultName ? null : this.Input.Name;
        achievement.Description = this.Input.Description.IsNotSet() || this.Input.Description == defaultDescription
                                      ? null
                                      : this.Input.Description;

        achievement.Icon = this.Input.Icon;
        achievement.Type = this.Input.Type;
        achievement.Threshold = this.Input.Threshold;
        achievement.ExtraType = this.Input.HasExtraGoal ? this.Input.ExtraType : null;
        achievement.ExtraThreshold = this.Input.HasExtraGoal ? this.Input.ExtraThreshold : null;
        achievement.Points = this.Input.Points;
        achievement.Rarity = this.Input.Rarity;
        achievement.IsActive = this.Input.IsActive;

        if (achievement.ID > 0)
        {
            await this.GetRepository<Achievement>().UpdateAsync(achievement);
        }
        else
        {
            await this.GetRepository<Achievement>().InsertAsync(achievement);
        }

        service.ClearBoardCache(this.PageBoardContext.PageBoardID);

        this.PageBoardContext.SessionNotify(
            this.GetTextFormatted("MSG_SAVED", this.HtmlEncode(service.GetName(achievement))),
            MessageTypes.success);

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_Achievements);
    }

    /// <summary>
    /// Checks the input (the values come from the client).
    /// </summary>
    /// <returns>The tag of the message, or null if the input is valid.</returns>
    private string Validate()
    {
        if (this.Input.Name.IsNotSet())
        {
            return "MSG_NAME";
        }

        if (!AchievementIcons.IsValid(this.Input.Icon))
        {
            this.Input.Icon = AchievementIcons.Default;
            return "MSG_ICON";
        }

        if (!Enum.IsDefined(typeof(AchievementType), this.Input.Type)
            || this.Input.HasExtraGoal && !Enum.IsDefined(typeof(AchievementType), this.Input.ExtraType))
        {
            return "MSG_TYPE";
        }

        if (this.Input.HasExtraGoal && this.Input.ExtraType == this.Input.Type)
        {
            return "MSG_SAME_TYPE";
        }

        if (this.Input.Threshold is < 1 or > MaxValue
            || this.Input.HasExtraGoal && this.Input.ExtraThreshold is < 1 or > MaxValue)
        {
            return "MSG_GOAL";
        }

        if (this.Input.Points is < 0 or > MaxValue)
        {
            return "MSG_POINTS";
        }

        return !Enum.IsDefined(typeof(AchievementRarity), this.Input.Rarity) ? "MSG_RARITY" : null;
    }
}
