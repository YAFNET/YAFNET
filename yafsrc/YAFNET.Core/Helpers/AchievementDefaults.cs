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

namespace YAF.Core.Helpers;

using System.Collections.Generic;

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The achievements a new board gets (and the boards of the time before the achievements). Their texts are in the
/// language files (page ACHIEVEMENT_DEFAULTS), so every user sees them in the own language.
/// </summary>
public static class AchievementDefaults
{
    /// <summary>
    /// The default achievements: text key, type, goal, second type, second goal, points, rarity and icon.
    /// </summary>
    private readonly static (string Key, AchievementType Type, int Threshold, AchievementType? ExtraType, int? ExtraThreshold, int Points, AchievementRarity Rarity, string Icon)[] Defaults =
    [
        ("POSTS_1", AchievementType.Posts, 1, null, null, 1, AchievementRarity.Common, "comment"),
        ("POSTS_10", AchievementType.Posts, 10, null, null, 2, AchievementRarity.Common, "comments"),
        ("POSTS_50", AchievementType.Posts, 50, null, null, 5, AchievementRarity.Common, "comment-dots"),
        ("POSTS_100", AchievementType.Posts, 100, null, null, 10, AchievementRarity.Uncommon, "bullhorn"),
        ("POSTS_500", AchievementType.Posts, 500, null, null, 20, AchievementRarity.Uncommon, "feather"),
        ("POSTS_1000", AchievementType.Posts, 1000, null, null, 30, AchievementRarity.Rare, "scroll"),
        ("POSTS_5000", AchievementType.Posts, 5000, null, null, 75, AchievementRarity.Epic, "book-open"),
        ("POSTS_10000", AchievementType.Posts, 10000, null, null, 150, AchievementRarity.Legendary, "crown"),
        ("ALLROUNDER", AchievementType.Posts, 1000, AchievementType.ReactionsReceived, 1000, 75, AchievementRarity.Epic, "trophy"),
        ("TOPICS_1", AchievementType.Topics, 1, null, null, 2, AchievementRarity.Common, "lightbulb"),
        ("TOPICS_10", AchievementType.Topics, 10, null, null, 5, AchievementRarity.Common, "file-circle-plus"),
        ("TOPICS_50", AchievementType.Topics, 50, null, null, 15, AchievementRarity.Uncommon, "signs-post"),
        ("TOPICS_250", AchievementType.Topics, 250, null, null, 40, AchievementRarity.Rare, "fire"),
        ("LIKES_1", AchievementType.ReactionsReceived, 1, null, null, 2, AchievementRarity.Common, "thumbs-up"),
        ("LIKES_25", AchievementType.ReactionsReceived, 25, null, null, 5, AchievementRarity.Common, "face-smile"),
        ("LIKES_100", AchievementType.ReactionsReceived, 100, null, null, 15, AchievementRarity.Uncommon, "heart"),
        ("LIKES_500", AchievementType.ReactionsReceived, 500, null, null, 30, AchievementRarity.Rare, "star"),
        ("LIKES_1000", AchievementType.ReactionsReceived, 1000, null, null, 50, AchievementRarity.Epic, "award"),
        ("LIKES_5000", AchievementType.ReactionsReceived, 5000, null, null, 100, AchievementRarity.Legendary, "gem"),
        ("GIVEN_1", AchievementType.ReactionsGiven, 1, null, null, 1, AchievementRarity.Common, "hand-holding-heart"),
        ("GIVEN_100", AchievementType.ReactionsGiven, 100, null, null, 10, AchievementRarity.Uncommon, "hands-clapping"),
        ("GIVEN_1000", AchievementType.ReactionsGiven, 1000, null, null, 25, AchievementRarity.Rare, "handshake"),
        ("DAYS_30", AchievementType.MembershipDays, 30, null, null, 2, AchievementRarity.Common, "seedling"),
        ("DAYS_365", AchievementType.MembershipDays, 365, null, null, 15, AchievementRarity.Uncommon, "cake-candles"),
        ("DAYS_1825", AchievementType.MembershipDays, 1825, null, null, 50, AchievementRarity.Epic, "shield-halved")
    ];

    /// <summary>
    /// Gets the text keys of the default achievements.
    /// </summary>
    public static IEnumerable<string> Keys => Defaults.Select(d => d.Key);

    /// <summary>
    /// Creates the default achievements of a board.
    /// </summary>
    /// <param name="boardId">The board id.</param>
    /// <returns>The achievements (not saved).</returns>
    public static List<Achievement> Create(int boardId)
    {
        return
        [
            .. Defaults.Select(
                d => new Achievement
                     {
                         BoardID = boardId,
                         TextKey = d.Key,
                         Type = (int)d.Type,
                         Threshold = d.Threshold,
                         ExtraType = (int?)d.ExtraType,
                         ExtraThreshold = d.ExtraThreshold,
                         Points = d.Points,
                         Rarity = (int)d.Rarity,
                         Icon = d.Icon,
                         IsActive = true
                     })
        ];
    }
}
