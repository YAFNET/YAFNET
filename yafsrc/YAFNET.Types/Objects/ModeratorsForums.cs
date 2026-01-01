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

namespace YAF.Types.Objects;

/// <summary>
/// Moderator Forums
/// </summary>
[Serializable]
public class ModeratorsForums
{
    /// <summary>
    ///   Gets or sets The Forum ID.
    /// </summary>
    public int ForumID { get; set; }

    /// <summary>
    /// Gets or sets the parent identifier.
    /// </summary>
    /// <value>The parent identifier.</value>
    public int? ParentID { get; set; }

    /// <summary>
    /// Gets or sets the name of the forum.
    /// </summary>
    /// <value>
    /// The name of the forum.
    /// </value>
    public string ForumName { get; set; }

    /// <summary>
    /// Gets or sets the category identifier.
    /// </summary>
    /// <value>The category identifier.</value>
    public int CategoryID { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    /// <value>The name of the category.</value>
    public string CategoryName { get; set; }
}