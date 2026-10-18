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
/// The help pages: the index, a help page and the search.
/// </summary>
[TestFixture]
public class HelpTests : ForumTestBase
{
    [Test]
    [Description("The index, a help page and an unknown help page (shows the index) are shown")]
    public async Task Help_Pages_Test()
    {
        await this.GotoAsync("Help?faq=index");
        await this.AssertNoServerErrorAsync();
        (await this.Control("search").CountAsync()).Should().BeGreaterThan(0, "the index has the search");

        await this.GotoAsync("Help?faq=bbcodes");
        await this.AssertNoServerErrorAsync();
        (await this.Control("search").CountAsync()).Should().Be(0, "the help page is shown");

        await this.GotoAsync("Help?faq=unknown");
        await this.AssertNoServerErrorAsync();
        (await this.Control("search").CountAsync()).Should().BeGreaterThan(0, "the index is shown");
    }

    [Test]
    [Description("The search needs more than 3 letters, a word without results stays on the index, found words are marked")]
    public async Task Help_Search_Test()
    {
        foreach (var word in new[] { "bbc", "nothingtofindhere" })
        {
            await this.GotoAsync("Help");
            await this.Control("search").FillAsync(word);
            await this.ClickAndWaitAsync(this.Control("DoSearch"));

            await this.AssertNoServerErrorAsync();
            (await this.Control("search").CountAsync()).Should().BeGreaterThan(0, word);
        }

        await this.GotoAsync("Help");
        await this.Control("search").FillAsync("password");
        await this.ClickAndWaitAsync(this.Control("DoSearch"));

        (await this.Page.Locator("mark").CountAsync()).Should().BeGreaterThan(0, "the found words are marked");
    }
}
