﻿/* Yet Another Forum.NET
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
public class SearchInputModel
{
    /// <summary>
    /// Gets or sets the search input.
    /// </summary>
    /// <value>The search input.</value>
    public string SearchInput { get; set; }

    /// <summary>
    /// Gets or sets the search string from who.
    /// </summary>
    /// <value>The search string from who.</value>
    public string SearchStringFromWho { get; set; }

    /// <summary>
    /// Gets or sets the search string tag.
    /// </summary>
    /// <value>The search string tag.</value>
    public string SearchStringTag { get; set; }

    /// <summary>
    /// Gets or sets the search what.
    /// </summary>
    /// <value>The search what.</value>
    public string SearchWhat { get; set; }

    /// <summary>
    /// Gets or sets the title only.
    /// </summary>
    /// <value>The title only.</value>
    public string TitleOnly { get; set; }

    /// <summary>
    /// Gets or sets the results per page.
    /// </summary>
    /// <value>The results per page.</value>
    public string ResultsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the forum list selected.
    /// </summary>
    /// <value>The forum list selected.</value>
    public string ForumListSelected { get; set; } = "0";
}