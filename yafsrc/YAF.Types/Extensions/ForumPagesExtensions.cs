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
namespace YAF.Types.Extensions;

using YAF.Types.Constants;

/// <summary>
/// ForumPages Extensions
/// </summary>
public static class ForumPagesExtensions
{
    /// <summary>
    /// The pages only the host admin can use (they can't be granted to the admins of a board):
    /// the boards, the host settings, the page access, and the pages that work on the whole database or application.
    /// </summary>
    public static readonly string[] HostAdminPages =
    [
        nameof(ForumPages.Admin_Boards), nameof(ForumPages.Admin_EditBoard), nameof(ForumPages.Admin_HostSettings),
        nameof(ForumPages.Admin_PageAccessList), nameof(ForumPages.Admin_PageAccessEdit), nameof(ForumPages.Admin_RunSql),
        nameof(ForumPages.Admin_ReIndex), nameof(ForumPages.Admin_Languages), nameof(ForumPages.Admin_EditLanguage),
        nameof(ForumPages.Admin_RestartApp), nameof(ForumPages.Admin_TestData)
    ];

    /// <summary>
    /// Checks if only the host admin can use the page.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>Returns if it is a host admin page.</returns>
    public static bool IsHostAdminPage(this ForumPages page)
    {
        return Array.IndexOf(HostAdminPages, page.ToString()) >= 0;
    }
}
