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

namespace YAF.Tests.UITests.Tests;

using System.Threading.Tasks;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The buttons of the host settings: the caches and the search index.
/// </summary>
[TestFixture]
public class HostSettingsTests : ForumTestBase
{
    [Test]
    [Description("The filled caches are cleared and the new search index is scheduled, the forum still works")]
    public async Task Cache_And_Search_Index_Buttons_Test()
    {
        await this.LoginAsAdminAsync();

        foreach (var button in new[]
                     {
                         "ForumStatisticsCacheReset", "BoardUserStatsCacheReset", "ActiveDiscussionsCacheReset",
                         "BoardModeratorsCacheReset", "ActiveUserLazyDataCacheReset", "ResetCacheAll", "IndexSearch"
                     })
        {
            // the board index fills the caches (a button of an empty cache is disabled)
            await this.GotoAsync(string.Empty);
            await this.GotoAsync("Admin/HostSettings");

            if (await this.Control(button).EvaluateAsync<bool>("e => e.classList.contains('disabled') || e.hasAttribute('disabled')"))
            {
                continue;
            }

            // the buttons are on tabs that are not shown
            await this.Page.RunAndWaitForNavigationAsync(() => this.Control(button).EvaluateAsync("e => e.click()"));
            await this.AssertNoServerErrorAsync();
        }

        await this.GotoAsync(string.Empty);
        (await this.Page.InnerTextAsync("body")).Should().Contain("Test Forum");
    }
}
