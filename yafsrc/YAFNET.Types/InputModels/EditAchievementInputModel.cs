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

namespace YAF.Types.InputModels;

/// <summary>
/// The input of the Admin > Edit Achievement page.
/// </summary>
public class EditAchievementInputModel
{
    /// <summary>
    /// Gets or sets the id (0 for a new achievement).
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
    public string Icon { get; set; } = "trophy";

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    public int Type { get; set; } = 1;

    /// <summary>
    /// Gets or sets the goal.
    /// </summary>
    public int Threshold { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the achievement has a second goal.
    /// </summary>
    public bool HasExtraGoal { get; set; }

    /// <summary>
    /// Gets or sets the type of the second goal.
    /// </summary>
    public int ExtraType { get; set; } = 3;

    /// <summary>
    /// Gets or sets the second goal.
    /// </summary>
    public int ExtraThreshold { get; set; } = 1;

    /// <summary>
    /// Gets or sets the points.
    /// </summary>
    public int Points { get; set; } = 1;

    /// <summary>
    /// Gets or sets the rarity.
    /// </summary>
    public int Rarity { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the achievement can be unlocked.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
