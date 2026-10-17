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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.Extensions;

using YAF.Types.Constants;
using YAF.Types.Extensions;

/// <summary>
/// The pages only the host admin can use (<see cref="ForumPagesExtensions"/>).
/// </summary>
[TestFixture]
public class ForumPagesExtensionsTests
{
    [TestCase(ForumPages.Admin_Boards)]
    [TestCase(ForumPages.Admin_EditBoard)]
    [TestCase(ForumPages.Admin_HostSettings)]
    [TestCase(ForumPages.Admin_PageAccessList)]
    [TestCase(ForumPages.Admin_PageAccessEdit)]
    [TestCase(ForumPages.Admin_RunSql)]
    [TestCase(ForumPages.Admin_ReIndex)]
    [TestCase(ForumPages.Admin_Languages)]
    [TestCase(ForumPages.Admin_EditLanguage)]
    [TestCase(ForumPages.Admin_RestartApp)]
    [TestCase(ForumPages.Admin_TestData)]
    [Description("The pages that work on all boards, the database or the application can't be granted to board admins")]
    public void Host_Admin_Page_Test(ForumPages page)
    {
        page.IsHostAdminPage().Should().BeTrue();
    }

    [TestCase(ForumPages.Admin_Admin)]
    [TestCase(ForumPages.Admin_Forums)]
    [TestCase(ForumPages.Admin_Users)]
    [TestCase(ForumPages.Admin_Settings)]
    [TestCase(ForumPages.Board)]
    [Description("The pages of the board can be granted")]
    public void Board_Page_Test(ForumPages page)
    {
        page.IsHostAdminPage().Should().BeFalse();
    }
}
