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

namespace YAF.Core.Services;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The achievements (trophies) the users unlock automatically.
/// </summary>
/// <param name="serviceLocator">The service locator.</param>
public class AchievementService(IServiceLocator serviceLocator) : IAchievementService, IHaveServiceLocator
{
    /// <summary>
    /// The most achievements a user can show in the profile and with the posts.
    /// </summary>
    public const int MaxFeatured = 3;

    /// <summary>
    /// The time between two checks of the achievements of a user on a page load.
    /// </summary>
    private readonly static TimeSpan CheckInterval = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; } = serviceLocator;

    /// <inheritdoc />
    public List<Achievement> GetBoardAchievements(int boardId)
    {
        return this.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.Achievements, boardId),
            () => this.GetRepository<Achievement>().Get(a => a.BoardID == boardId).OrderBy(a => a.Type)
                .ThenBy(a => a.Threshold).ThenBy(a => a.ExtraThreshold ?? 0).ThenBy(a => a.ID).ToList(),
            TimeSpan.FromMinutes(30));
    }

    /// <inheritdoc />
    public string GetName(Achievement achievement)
    {
        if (achievement.Name.IsSet())
        {
            return achievement.Name;
        }

        return achievement.TextKey.IsSet()
                   ? this.Get<ILocalization>().GetText("ACHIEVEMENT_DEFAULTS", $"{achievement.TextKey}_NAME")
                   : string.Empty;
    }

    /// <inheritdoc />
    public string GetDescription(Achievement achievement)
    {
        if (achievement.Description.IsSet())
        {
            return achievement.Description;
        }

        // a default achievement keeps the localized description until the administrator writes one
        return achievement.TextKey.IsSet()
                   ? this.Get<ILocalization>().GetText("ACHIEVEMENT_DEFAULTS", $"{achievement.TextKey}_DESC")
                   : string.Empty;
    }

    /// <inheritdoc />
    public string GetTypeName(AchievementType type)
    {
        return this.Get<ILocalization>().GetText("ACHIEVEMENTS", $"TYPE_{type.ToString().ToUpperInvariant()}");
    }

    /// <inheritdoc />
    public string GetRarityName(AchievementRarity rarity)
    {
        return this.Get<ILocalization>().GetText("ACHIEVEMENTS", $"RARITY_{rarity.ToString().ToUpperInvariant()}");
    }

    /// <inheritdoc />
    public string GetGoalText(Achievement achievement)
    {
        var goal = this.FormatGoal(achievement.AchievementType, achievement.Threshold);

        if (!achievement.HasExtraGoal)
        {
            return goal;
        }

        return string.Format(
            this.Get<ILocalization>().GetText("ACHIEVEMENTS", "GOAL_AND"),
            goal,
            this.FormatGoal(achievement.AchievementExtraType!.Value, achievement.ExtraThreshold!.Value));
    }

    /// <inheritdoc />
    public async Task<Dictionary<AchievementType, int>> GetUserValuesAsync(int userId, IEnumerable<AchievementType> types)
    {
        var values = new Dictionary<AchievementType, int>();

        var user = await this.GetRepository<User>().GetByIdAsync(userId);

        if (user is null)
        {
            return values;
        }

        foreach (var type in types.Distinct())
        {
            values[type] = type switch
            {
                AchievementType.Posts => user.NumPosts,

                // the visible topics the user has started (not the deleted, moved or not approved ones)
                AchievementType.Topics => (int)await this.GetRepository<Topic>().CountAsync(
                                              t => t.UserID == userId && (t.Flags & 8) != 8 && t.TopicMovedID == null
                                                   && t.NumPosts > 0),
                AchievementType.ReactionsReceived => (int)await this.GetRepository<Thanks>()
                                                         .CountAsync(t => t.ThanksToUserID == userId),
                AchievementType.ReactionsGiven => (int)await this.GetRepository<Thanks>()
                                                      .CountAsync(t => t.ThanksFromUserID == userId),
                AchievementType.MembershipDays => Math.Max(0, (int)(DateTime.UtcNow - user.Joined).TotalDays),
                AchievementType.Reputation => user.Points,
                _ => 0
            };
        }

        return values;
    }

    /// <inheritdoc />
    public async Task<List<Achievement>> CheckUserAsync(int userId)
    {
        var user = await this.GetRepository<User>().GetByIdAsync(userId);

        // only the approved members (not the guest)
        if (user is null || user.UserFlags.IsGuest || !user.UserFlags.IsApproved
            || !this.Get<BoardSettings>().EnableAchievements)
        {
            return [];
        }

        var unlocked = this.GetUserAchievements(userId).Select(u => u.AchievementID).ToHashSet();

        var open = this.GetBoardAchievements(user.BoardID).Where(a => a.IsActive && !unlocked.Contains(a.ID)).ToList();

        if (open.Count == 0)
        {
            return [];
        }

        var values = await this.GetUserValuesAsync(
                         userId,
                         open.Select(a => a.AchievementType)
                             .Concat(open.Where(a => a.HasExtraGoal).Select(a => a.AchievementExtraType!.Value)));

        var reached = open.Where(a => IsReached(a, values)).ToList();

        if (reached.Count == 0)
        {
            return [];
        }

        var now = DateTime.UtcNow;

        foreach (var achievementId in reached.Select(a => a.ID))
        {
            // a second request of the user (e.g. a reaction while a post is saved) may have awarded it already
            if (await this.GetRepository<UserAchievement>()
                    .ExistsAsync(u => u.UserID == userId && u.AchievementID == achievementId))
            {
                continue;
            }

            await this.GetRepository<UserAchievement>().InsertAsync(
                new UserAchievement { UserID = userId, AchievementID = achievementId, DateAwarded = now },
                selectIdentity: false);

            // the notification (bell)
            this.Get<IActivityStream>().AddAchievementToStream(user, achievementId);
        }

        this.ClearUserCache(userId);
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.AchievementCounts, user.BoardID));

        this.Get<IRaiseEvent>().Raise(
            new AchievementUnlockedEvent(userId, user.Name, [.. reached.Select(a => a.ID)]));

        return reached;
    }

    /// <inheritdoc />
    public async Task CheckUserOccasionallyAsync(int userId)
    {
        var key = string.Format(Constants.Cache.UserAchievementsChecked, userId);

        if (this.Get<IDataCache>().Get(key) != null)
        {
            return;
        }

        this.Get<IDataCache>().Set(key, DateTime.UtcNow, CheckInterval);

        try
        {
            await this.CheckUserAsync(userId);
        }
        catch (Exception ex)
        {
            // the check runs on every page (layout): a failure must not break the page, it is logged
            this.Get<ILogger<AchievementService>>().Error(ex, "Checking the achievements failed");
        }
    }

    /// <inheritdoc />
    public async Task<int> CheckBoardAsync(int boardId)
    {
        var userIds = await this.GetRepository<User>().DbAccess.ExecuteAsync(
                          db => db.ColumnAsync<int>(
                              db.From<User>().Where(
                                      u => u.BoardID == boardId && (u.Flags & 4) != 4 && (u.Flags & 2) == 2)
                                  .Select(u => u.ID)));

        var count = 0;

        foreach (var userId in userIds)
        {
            count += (await this.CheckUserAsync(userId)).Count;
        }

        return count;
    }

    /// <inheritdoc />
    public List<UserAchievement> GetUserAchievements(int userId)
    {
        return this.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.UserAchievements, userId),
            () => this.GetRepository<UserAchievement>().Get(u => u.UserID == userId),
            TimeSpan.FromMinutes(10));
    }

    /// <inheritdoc />
    public async Task<List<AchievementInfo>> GetAchievementInfosAsync(int boardId, int? userId, bool withProgress)
    {
        var achievements = this.GetBoardAchievements(boardId);
        var unlocked = userId.HasValue
                           ? this.GetUserAchievements(userId.Value).ToDictionary(u => u.AchievementID)
                           : [];

        // the inactive achievements are only listed for the users that have them
        achievements = [.. achievements.Where(a => a.IsActive || unlocked.ContainsKey(a.ID))];

        Dictionary<AchievementType, int> values = null;

        if (withProgress && userId.HasValue)
        {
            values = await this.GetUserValuesAsync(
                         userId.Value,
                         achievements.Select(a => a.AchievementType).Concat(
                             achievements.Where(a => a.HasExtraGoal).Select(a => a.AchievementExtraType!.Value)));
        }

        return [.. achievements.Select(a => this.CreateInfo(a, unlocked.GetValueOrDefault(a.ID), values))];
    }

    /// <inheritdoc />
    public List<AchievementInfo> GetFeatured(int userId)
    {
        var featured = this.GetUserAchievements(userId).Where(u => u.Featured).ToList();

        if (featured.Count == 0)
        {
            return [];
        }

        return [.. this.GetInfos(featured).OrderByDescending(i => i.Rarity).ThenByDescending(i => i.Points)];
    }

    /// <inheritdoc />
    public List<AchievementInfo> GetUnseen(int userId)
    {
        var unseen = this.GetUserAchievements(userId).Where(u => !u.Seen).ToList();

        // the most valuable achievement first
        return unseen.Count == 0
                   ? []
                   : [.. this.GetInfos(unseen).OrderByDescending(i => i.Rarity).ThenByDescending(i => i.Points)];
    }

    /// <inheritdoc />
    public bool HasUnseen(int userId)
    {
        return this.GetUserAchievements(userId).Exists(u => !u.Seen);
    }

    /// <inheritdoc />
    public async Task MarkSeenAsync(int userId, int achievementId)
    {
        await this.GetRepository<UserAchievement>().UpdateOnlyAsync(
            () => new UserAchievement { Seen = true },
            u => u.UserID == userId && u.AchievementID == achievementId);

        // the user has seen the dialog: the notification is read (the user is not told twice)
        await this.GetRepository<Activity>().UpdateOnlyAsync(
            () => new Activity { Notification = false },
            a => a.UserID == userId && a.AchievementID == achievementId && a.Notification);

        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

        this.ClearUserCache(userId);
    }

    /// <inheritdoc />
    public async Task<bool> SetFeaturedAsync(int userId, int achievementId, bool featured)
    {
        var achievements = this.GetUserAchievements(userId);

        var achievement = achievements.Find(u => u.AchievementID == achievementId);

        if (achievement is null)
        {
            return false;
        }

        if (featured && !achievement.Featured && achievements.Count(u => u.Featured) >= MaxFeatured)
        {
            return false;
        }

        await this.GetRepository<UserAchievement>().UpdateOnlyAsync(
            () => new UserAchievement { Featured = featured },
            u => u.UserID == userId && u.AchievementID == achievementId);

        this.ClearUserCache(userId);

        return true;
    }

    /// <inheritdoc />
    public int GetUserPoints(int userId)
    {
        var ids = this.GetUserAchievements(userId).Select(u => u.AchievementID).ToHashSet();

        if (ids.Count == 0)
        {
            return 0;
        }

        return this.GetBoardAchievements(BoardContext.Current.PageBoardID).Where(a => ids.Contains(a.ID))
            .Sum(a => a.Points);
    }

    /// <inheritdoc />
    public void ClearBoardCache(int boardId)
    {
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.Achievements, boardId));
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.AchievementCounts, boardId));
    }

    /// <inheritdoc />
    public void ClearUserCache(int userId)
    {
        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.UserAchievements, userId));
    }

    /// <summary>
    /// Checks if the user has reached the goals of an achievement.
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <param name="values">The values of the user.</param>
    /// <returns><c>true</c> if all goals are reached.</returns>
    public static bool IsReached(Achievement achievement, IReadOnlyDictionary<AchievementType, int> values)
    {
        if (values.GetValueOrDefault(achievement.AchievementType) < achievement.Threshold)
        {
            return false;
        }

        return !achievement.HasExtraGoal
               || values.GetValueOrDefault(achievement.AchievementExtraType!.Value) >= achievement.ExtraThreshold;
    }

    /// <summary>
    /// Formats the share of the members (e.g. "6.2%", "&lt;0.1%").
    /// </summary>
    /// <param name="count">The number of the members that have the achievement.</param>
    /// <param name="members">The number of the members.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>The share.</returns>
    public static string FormatPercent(int count, int members, CultureInfo culture)
    {
        if (count <= 0 || members <= 0)
        {
            return 0.ToString("0", culture) + "%";
        }

        var percent = Math.Min(100d, count * 100d / members);

        return percent < 0.1 ? $"<{0.1.ToString("0.0", culture)}%" : $"{percent.ToString("0.#", culture)}%";
    }

    /// <summary>
    /// Gets the infos of unlocked achievements of the board.
    /// </summary>
    /// <param name="userAchievements">The unlocked achievements.</param>
    /// <returns>The infos.</returns>
    private IEnumerable<AchievementInfo> GetInfos(List<UserAchievement> userAchievements)
    {
        var achievements = this.GetBoardAchievements(BoardContext.Current.PageBoardID).ToDictionary(a => a.ID);

        return userAchievements.Where(u => achievements.ContainsKey(u.AchievementID))
            .Select(u => this.CreateInfo(achievements[u.AchievementID], u, null));
    }

    /// <summary>
    /// Creates the info of an achievement.
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <param name="userAchievement">The unlocked achievement of the user (or null).</param>
    /// <param name="values">The values of the user for the progress (or null).</param>
    /// <returns>The info.</returns>
    private AchievementInfo CreateInfo(
        Achievement achievement,
        UserAchievement userAchievement,
        IReadOnlyDictionary<AchievementType, int> values)
    {
        var culture = this.Get<ILocalization>().Culture;

        var (counts, members) = this.GetCounts(achievement.BoardID);

        var info = new AchievementInfo
                   {
                       Id = achievement.ID,
                       Name = this.GetName(achievement),
                       Description = this.GetDescription(achievement),
                       Icon = AchievementIcons.IsValid(achievement.Icon) ? achievement.Icon : AchievementIcons.Default,
                       Type = achievement.AchievementType,
                       Category = this.GetTypeName(achievement.AchievementType),
                       Points = achievement.Points,
                       Rarity = achievement.AchievementRarity,
                       RarityName = this.GetRarityName(achievement.AchievementRarity),
                       RarityClass = $"rarity-{achievement.AchievementRarity.ToString().ToLowerInvariant()}",
                       Percent = FormatPercent(counts.GetValueOrDefault(achievement.ID), members, culture),
                       Unlocked = userAchievement != null,
                       DateAwarded = userAchievement?.DateAwarded,
                       Featured = userAchievement?.Featured ?? false
                   };

        if (info.Unlocked)
        {
            info.Progress = 100;
        }
        else if (values != null)
        {
            var value = values.GetValueOrDefault(achievement.AchievementType);

            var progress = Math.Min(1d, (double)value / Math.Max(1, achievement.Threshold));

            info.ProgressText =
                $"{Math.Min(value, achievement.Threshold).ToString("N0", culture)} / {achievement.Threshold.ToString("N0", culture)}";

            if (achievement.HasExtraGoal)
            {
                var extraValue = values.GetValueOrDefault(achievement.AchievementExtraType!.Value);
                var extraThreshold = achievement.ExtraThreshold!.Value;

                progress = (progress + Math.Min(1d, (double)extraValue / Math.Max(1, extraThreshold))) / 2;

                info.ProgressText +=
                    $" · {Math.Min(extraValue, extraThreshold).ToString("N0", culture)} / {extraThreshold.ToString("N0", culture)}";
            }

            // a goal that is reached but not awarded yet (the next check awards it) is not shown as complete
            info.Progress = Math.Min(99, (int)Math.Floor(progress * 100));
        }

        return info;
    }

    /// <summary>
    /// Gets the number of the members that have the achievements of the board and the number of the members.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <returns>The counts per achievement and the number of the members.</returns>
    private (Dictionary<int, int> Counts, int Members) GetCounts(int boardId)
    {
        return this.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.AchievementCounts, boardId),
            () =>
            {
                var counts = this.GetRepository<UserAchievement>()
                    .CountUsers([.. this.GetBoardAchievements(boardId).Select(a => a.ID)]);

                var members = (int)this.GetRepository<User>().Count(
                    u => u.BoardID == boardId && (u.Flags & 4) != 4 && (u.Flags & 2) == 2);

                return (counts, members);
            },
            TimeSpan.FromMinutes(10));
    }

    /// <summary>
    /// Formats a goal (e.g. "100 posts").
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="threshold">The goal.</param>
    /// <returns>The goal as text.</returns>
    private string FormatGoal(AchievementType type, int threshold)
    {
        return string.Format(
            this.Get<ILocalization>().GetText("ACHIEVEMENTS", $"GOAL_{type.ToString().ToUpperInvariant()}"),
            threshold.ToString("N0", this.Get<ILocalization>().Culture));
    }
}
