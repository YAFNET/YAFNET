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

using ServiceStack.DataAnnotations;

namespace YAF.Types.Models;

/// <summary>
/// A class which represents the UserAchievement table: an achievement a user has unlocked.
/// </summary>
[Serializable]
[CompositePrimaryKey(nameof(AchievementID), nameof(UserID))]
public class UserAchievement : IEntity
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    [References(typeof(User))]
    [Required]
    [Index]
    public int UserID { get; set; }

    /// <summary>
    /// Gets or sets the achievement id.
    /// </summary>
    [References(typeof(Achievement))]
    [Required]
    public int AchievementID { get; set; }

    /// <summary>
    /// Gets or sets the date the achievement was unlocked.
    /// </summary>
    [Required]
    public DateTime DateAwarded { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user has seen the "achievement unlocked" dialog.
    /// </summary>
    [Required]
    [Default(typeof(bool), "0")]
    public bool Seen { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user shows the achievement in the profile and with the posts.
    /// </summary>
    [Required]
    [Default(typeof(bool), "0")]
    public bool Featured { get; set; }
}
