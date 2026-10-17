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

using System;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Admin > Languages > Edit (host admin): a translated text of a page is changed and saved (the test site is a copy).
/// </summary>
[TestFixture]
public class EditLanguageTests : ForumTestBase
{
    [Test]
    [Description("A text of the german language file is changed and kept, a file name with a path is refused")]
    public async Task Edit_Text_Test()
    {
        var text = $"Geaendert {Guid.NewGuid():N}".Substring(0, 18);

        await this.LoginAsAdminAsync();

        // only a file of the languages folder
        await this.GotoAsync("EditLanguage?x=..%5C..%5Cweb.config".Insert(0, "Admin/"));
        (await this.Control("Locals").CountAsync()).Should().Be(0);

        await this.GotoAsync("Admin/EditLanguage?x=german-du.json");

        await this.Control("Pages").SelectOptionAsync("ACTIVELOCATION", new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("LoadPageLocalization"));

        var first = this.Page.Locator("[id$='_txtLocalized']").First;
        await first.FillAsync(text);
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.GotoAsync("Admin/EditLanguage?x=german-du.json");
        await this.Control("Pages").SelectOptionAsync("ACTIVELOCATION", new LocatorSelectOptionOptions { Force = true });
        await this.ClickAndWaitAsync(this.Control("LoadPageLocalization"));

        (await this.Page.Locator("[id$='_txtLocalized']").First.InputValueAsync()).Should().Be(text);
    }
}
