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

using System.Collections.Generic;
using System.Threading.Tasks;

using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects;

namespace YAF.Types.Interfaces.Services;

/// <summary>
/// The achievements (trophies) the users unlock automatically.
/// </summary>
public interface IAchievementService
{
    /// <summary>
    /// Gets all achievements of a board (also the inactive ones), sorted by type and goal.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <returns>The achievements.</returns>
    List<Achievement> GetBoardAchievements(int boardId);

    /// <summary>
    /// Gets the name of an achievement (the localized default name if the administrator has not changed it).
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <returns>The name.</returns>
    string GetName(Achievement achievement);

    /// <summary>
    /// Gets the description of an achievement (the localized default description if the administrator has not
    /// changed it).
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <returns>The description.</returns>
    string GetDescription(Achievement achievement);

    /// <summary>
    /// Gets the localized name of a type (the category of the achievements).
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The name.</returns>
    string GetTypeName(AchievementType type);

    /// <summary>
    /// Gets the localized name of a rarity.
    /// </summary>
    /// <param name="rarity">The rarity.</param>
    /// <returns>The name.</returns>
    string GetRarityName(AchievementRarity rarity);

    /// <summary>
    /// Gets the goal of an achievement as text (e.g. "100 posts" or "1,000 posts and 1,000 reactions received").
    /// </summary>
    /// <param name="achievement">The achievement.</param>
    /// <returns>The goal.</returns>
    string GetGoalText(Achievement achievement);

    /// <summary>
    /// Gets the current values of a user for the types (e.g. the post count).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="types">The types that are needed.</param>
    /// <returns>The values of the types.</returns>
    Task<Dictionary<AchievementType, int>> GetUserValuesAsync(int userId, IEnumerable<AchievementType> types);

    /// <summary>
    /// Awards the active achievements the user has reached and not unlocked yet, the user is told about them
    /// (dialog, live if the user is online).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The new achievements.</returns>
    Task<List<Achievement>> CheckUserAsync(int userId);

    /// <summary>
    /// Checks the achievements of the user at most once in a while (the page load: for the goals that are reached
    /// without an action, e.g. the membership days, and for the users of the time before the achievements).
    /// </summary>
    /// <param name="userId">The user id.</param>
    Task CheckUserOccasionallyAsync(int userId);

    /// <summary>
    /// Checks the achievements of all users of a board.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <returns>The number of the achievements that were awarded.</returns>
    Task<int> CheckBoardAsync(int boardId);

    /// <summary>
    /// Gets the achievements a user has unlocked.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The unlocked achievements of the user.</returns>
    List<UserAchievement> GetUserAchievements(int userId);

    /// <summary>
    /// Gets the active achievements of a board as they are shown to a user (unlocked or not and the progress).
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <param name="userId">The user id (null for a guest).</param>
    /// <param name="withProgress">Load the values of the user for the progress.</param>
    /// <returns>The achievements.</returns>
    Task<List<AchievementInfo>> GetAchievementInfosAsync(int boardId, int? userId, bool withProgress);

    /// <summary>
    /// Gets the achievements a user shows (in the profile and with the posts).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The featured achievements.</returns>
    List<AchievementInfo> GetFeatured(int userId);

    /// <summary>
    /// Gets the unlocked achievements the user has not seen (the dialog).
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The achievements.</returns>
    List<AchievementInfo> GetUnseen(int userId);

    /// <summary>
    /// Gets a value indicating whether the user has unlocked achievements that the user has not seen.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns><c>true</c> if there are unseen achievements.</returns>
    bool HasUnseen(int userId);

    /// <summary>
    /// Marks an unlocked achievement as seen.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="achievementId">The achievement id.</param>
    Task MarkSeenAsync(int userId, int achievementId);

    /// <summary>
    /// Shows or hides an unlocked achievement in the profile and with the posts.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="achievementId">The achievement id.</param>
    /// <param name="featured">Show the achievement.</param>
    /// <returns><c>false</c> if the user has not unlocked the achievement or already shows the most achievements.</returns>
    Task<bool> SetFeaturedAsync(int userId, int achievementId, bool featured);

    /// <summary>
    /// Gets the achievement points of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <returns>The points.</returns>
    int GetUserPoints(int userId);

    /// <summary>
    /// Clears the cached achievements of the board and the counts.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    void ClearBoardCache(int boardId);

    /// <summary>
    /// Clears the cached achievements of a user.
    /// </summary>
    /// <param name="userId">The user id.</param>
    void ClearUserCache(int userId);
}
