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

using YAF.Types.Constants;

namespace YAF.Types.Objects;

/// <summary>
/// An achievement as it is shown to a user (localized texts, the share of the members that have it and the
/// progress of the user).
/// </summary>
public class AchievementInfo
{
    /// <summary>
    /// Gets or sets the id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets or sets the type (the category).
    /// </summary>
    public AchievementType Type { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// Gets or sets the achievement points.
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Gets or sets the rarity.
    /// </summary>
    public AchievementRarity Rarity { get; set; }

    /// <summary>
    /// Gets or sets the name of the rarity.
    /// </summary>
    public string RarityName { get; set; }

    /// <summary>
    /// Gets or sets the css class of the rarity (e.g. "rarity-epic").
    /// </summary>
    public string RarityClass { get; set; }

    /// <summary>
    /// Gets or sets the share of the members that have the achievement (e.g. "6.2%" or "&lt;0.1%").
    /// </summary>
    public string Percent { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has unlocked the achievement.
    /// </summary>
    public bool Unlocked { get; set; }

    /// <summary>
    /// Gets or sets the date the user has unlocked the achievement.
    /// </summary>
    public DateTime? DateAwarded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user shows the achievement in the profile.
    /// </summary>
    public bool Featured { get; set; }

    /// <summary>
    /// Gets or sets the progress of the user in percent (0-100).
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// Gets or sets the progress as text (e.g. "12 / 50").
    /// </summary>
    public string ProgressText { get; set; }
}
