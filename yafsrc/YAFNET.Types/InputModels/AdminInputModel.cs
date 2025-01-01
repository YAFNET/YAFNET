/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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
/// The input model.
/// </summary>
public class AdminInputModel
{
    /// <summary>
    /// Gets or sets the size of the database.
    /// </summary>
    /// <value>The size of the database.</value>
    public string DBSize { get; set; }

    /// <summary>
    /// Gets or sets the days old.
    /// </summary>
    /// <value>The days old.</value>
    public int DaysOld { get; set; } = 14;

    /// <summary>
    /// Gets or sets the selected board identifier.
    /// </summary>
    /// <value>The selected board identifier.</value>
    public int SelectedBoardId { get; set; }

    /// <summary>
    /// Gets or sets the number categories.
    /// </summary>
    /// <value>The number categories.</value>
    public int NumCategories { get; set; }

    /// <summary>
    /// Gets or sets the number forums.
    /// </summary>
    /// <value>The number forums.</value>
    public int NumForums { get; set; }

    /// <summary>
    /// Gets or sets the number topics.
    /// </summary>
    /// <value>The number topics.</value>
    public string NumTopics { get; set; }

    /// <summary>
    /// Gets or sets the number posts.
    /// </summary>
    /// <value>The number posts.</value>
    public string NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the number users.
    /// </summary>
    /// <value>The number users.</value>
    public string NumUsers { get; set; }

    /// <summary>
    /// Gets or sets the board start.
    /// </summary>
    /// <value>The board start.</value>
    public string BoardStart { get; set; }

    /// <summary>
    /// Gets or sets the board start ago.
    /// </summary>
    /// <value>The board start ago.</value>
    public DateTime BoardStartAgo { get; set; }

    /// <summary>
    /// Gets or sets the day posts.
    /// </summary>
    /// <value>The day posts.</value>
    public string DayPosts { get; set; }

    /// <summary>
    /// Gets or sets the day topics.
    /// </summary>
    /// <value>The day topics.</value>
    public string DayTopics { get; set; }

    /// <summary>
    /// Gets or sets the day users.
    /// </summary>
    /// <value>The day users.</value>
    public string DayUsers { get; set; }

    /// <summary>
    /// Gets or sets the size of the unverified page.
    /// </summary>
    /// <value>The size of the unverified page.</value>
    public int UnverifiedPageSize { get; set; }

    /// <summary>
    /// Gets or sets the unverified count.
    /// </summary>
    /// <value>The unverified count.</value>
    public int UnverifiedCount { get; set; }
}