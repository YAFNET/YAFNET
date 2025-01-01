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

namespace YAF.Core.BasePages;

/// <summary>
/// Admin page with extra security. All admin pages need to be derived from this base class.
/// </summary>
public class AdminPage : ForumPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    /// <param name="page">
    /// The page.
    /// </param>
    public AdminPage(string transPage, ForumPages page)
        : base(transPage, page)
    {
        this.IsAdminPage = true;
    }

    /// <summary>
    /// Gets the item status color
    /// </summary>
    /// <param name="value">
    /// The bool value to check
    /// </param>
    /// <returns>
    /// Sets the css class for the badge.
    /// </returns>
#pragma warning disable CA1822 // Mark members as static
    public string GetItemColor(bool value)

    {
        return value ? "badge text-bg-success mb-2" : "badge text-bg-danger mb-2";
    }
#pragma warning restore CA1822 // Mark members as static

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="value">
    /// The bool value to check
    /// </param>
    /// <returns>
    /// the Localized text.
    /// </returns>
    public string GetItemName(bool value)
    {
        return value ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }
}