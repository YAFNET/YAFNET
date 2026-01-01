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
/// The input model.
/// </summary>
public class MembersInputModel
{
    /// <summary>
    /// Gets or sets the sort name field.
    /// </summary>
    /// <value>The sort name field.</value>
    public int SortNameField { get; set; }

    /// <summary>
    /// Gets or sets the sort rank name field.
    /// </summary>
    /// <value>The sort rank name field.</value>
    public int SortRankNameField { get; set; }

    /// <summary>
    /// Gets or sets the sort joined field.
    /// </summary>
    /// <value>The sort joined field.</value>
    public int SortJoinedField { get; set; }

    /// <summary>
    /// Gets or sets the sort number posts field.
    /// </summary>
    /// <value>The sort number posts field.</value>
    public int SortNumPostsField { get; set; }

    /// <summary>
    /// Gets or sets the sort last visit field.
    /// </summary>
    /// <value>The sort last visit field.</value>
    public int SortLastVisitField { get; set; }

    /// <summary>
    /// Gets or sets the number posts.
    /// </summary>
    /// <value>The number posts.</value>
    public int NumPosts { get; set; }

    /// <summary>
    /// Gets or sets the number post value.
    /// </summary>
    /// <value>The number post value.</value>
    public int NumPostValue { get; set; }

    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    /// <value>The group.</value>
    public int Group { get; set; }

    /// <summary>
    /// Gets or sets the rank.
    /// </summary>
    /// <value>The rank.</value>
    public int Rank { get; set; }

    /// <summary>
    /// Gets or sets the name of the user search.
    /// </summary>
    /// <value>The name of the user search.</value>
    public string UserSearchName { get; set; }
}