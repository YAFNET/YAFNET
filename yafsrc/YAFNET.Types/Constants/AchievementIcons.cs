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

namespace YAF.Types.Constants;

/// <summary>
/// The Font Awesome icons an achievement can have. The css files only have the icons that are named in the
/// sources (see fontawesome-icons.js), so the administrator picks one of these and not any icon name.
/// </summary>
public static class AchievementIcons
{
    /// <summary>
    /// The icon of a new achievement.
    /// </summary>
    public const string Default = "trophy";

    /// <summary>
    /// All icons an achievement can have.
    /// </summary>
    public readonly static string[] All =
    [
        "trophy", "medal", "award", "crown", "star", "gem", "certificate", "ranking-star",
        "comment", "comments", "comment-dots", "message", "bullhorn", "quote-left", "reply", "newspaper",
        "feather", "pen-nib", "scroll", "book-open", "landmark", "lightbulb", "file-circle-plus", "signs-post",
        "thumbs-up", "heart", "heart-pulse", "hand-holding-heart", "hands-clapping", "handshake", "face-smile",
        "face-grin-stars", "face-laugh-beam", "face-surprise", "fire", "fire-flame-curved", "bolt", "meteor",
        "rocket", "user-astronaut", "user-ninja", "user-graduate", "user-group", "users", "people-group",
        "dove", "calendar-check", "cake-candles", "hourglass-half", "clock", "shield-halved", "shield",
        "infinity", "seedling", "leaf", "tree", "mountain-sun", "sun", "moon", "compass", "anchor",
        "map-location-dot", "earth-europe", "route", "flag", "bullseye", "crosshairs", "gauge-high", "chart-line",
        "circle-check", "key", "lock-open", "puzzle-piece", "dice", "chess-king", "chess-queen", "chess-knight",
        "hat-wizard", "wand-magic-sparkles", "dragon", "ghost", "crow", "paw", "brain", "atom", "flask", "bug",
        "palette", "music", "guitar", "microphone", "camera", "masks-theater", "champagne-glasses", "gift",
        "mug-hot", "pizza-slice", "couch", "graduation-cap", "repeat"
    ];

    /// <summary>
    /// Checks if the icon is one of the achievement icons.
    /// </summary>
    /// <param name="icon">The icon name.</param>
    /// <returns><c>true</c> if an achievement can have the icon.</returns>
    public static bool IsValid(string icon)
    {
        return icon != null && Array.IndexOf(All, icon) >= 0;
    }
}
