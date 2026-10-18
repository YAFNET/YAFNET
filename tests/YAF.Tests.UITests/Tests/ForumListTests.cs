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
/// The forum list of the board index: locked forums, forum images and sub forums.
/// </summary>
[TestFixture]
public class ForumListTests : ForumTestBase
{
    [Test]
    [Description("A locked forum and a forum with an image and a sub forum are listed, the names are encoded")]
    public async Task Locked_Image_And_Sub_Forums_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var parent = $"Parent <b>{id}</b>";
        var child = $"Sub <i>{id}</i>";
        var other = $"Image <u>{id}</u>";

        await this.LoginAsAdminAsync();

        // a locked forum and a forum with an image
        foreach (var name in new[] { parent, other })
        {
            await this.EditForumAsync(name, null);

            if (name == parent)
            {
                await this.Control("Locked").CheckAsync();
                await this.Control("Description").FillAsync($"Described <u>{id}</u>");
            }

            await this.Control("ForumImages").SelectOptionAsync(
                new SelectOptionValue { Label = "forumImageSample.webp" },
                new LocatorSelectOptionOptions { Force = true });
            await this.ClickAndWaitAsync(this.Control("Save"));
        }

        // a sub forum
        await this.EditForumAsync(child, parent);
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(string.Empty);
        await this.AssertNoServerErrorAsync();

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain(parent, "the names are encoded").And.Contain(child).And.Contain(other)
            .And.Contain($"Described <u>{id}</u>", "the description is encoded");

        (await this.Page.Locator("img.forum_customimage_locked").CountAsync()).Should().BeGreaterThan(0);
        (await this.Page.Locator("img.forum_customimage_newposts, img.forum_customimage_nonewposts").CountAsync()).Should()
            .BeGreaterThan(0);

        foreach (var tag in new[] { "b", "i", "u" })
        {
            (await this.Page.Locator(tag, new PageLocatorOptions { HasTextString = id }).CountAsync()).Should().Be(0, tag);
        }
    }

    [Test]
    [Description("A forum the members can't read is listed with (No Access) when it is not hidden; a new forum is hidden by default")]
    public async Task No_Access_And_Hidden_Forums_Test()
    {
        var id = DateTime.UtcNow.ToString("HHmmssfff");
        var shown = $"Closed shown <b>{id}</b>";
        var hidden = $"Closed hidden {id}";

        await this.LoginAsAdminAsync();

        // the option is checked for a new forum
        await this.EditForumAsync(hidden, null);
        (await this.Control("HideNoAccess").IsCheckedAsync()).Should().BeTrue("a new forum is hidden by default");
        await this.SetNoAccessAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.EditForumAsync(shown, null);
        await this.Control("HideNoAccess").UncheckAsync();
        await this.SetNoAccessAsync();
        await this.ClickAndWaitAsync(this.Control("Save"));

        await this.LoginAsUserAsync(TestConfig.TestUserName);
        await this.GotoAsync(string.Empty);

        var text = await this.Page.InnerTextAsync("body");
        text.Should().Contain($"{shown} (No Access)", "the forum is listed without a link").And.NotContain(hidden);
        (await this.Page.Locator("a", new PageLocatorOptions { HasTextString = shown }).CountAsync()).Should().Be(0);
    }

    private async Task SetNoAccessAsync()
    {
        await this.Page.GetByLabel("Registered Users", new PageGetByLabelOptions { Exact = true }).SelectOptionAsync(
            new SelectOptionValue { Label = "No Access" },
            new LocatorSelectOptionOptions { Force = true });
    }

    /// <summary>
    /// Opens the page of a new forum in the test category and fills it in.
    /// </summary>
    private async Task EditForumAsync(string name, string parentName)
    {
        await this.GotoAsync("Admin/EditForum");
        await this.Control("Name").FillAsync(name);
        await this.Control("Description").FillAsync("A forum of the forum list test");
        await this.Control("CategoryList").SelectOptionAsync(new SelectOptionValue { Label = "Test Category" });
        await this.Control("SortOrder").FillAsync("80");

        if (parentName != null)
        {
            await this.Control("ParentList").EvaluateAsync(
                "(e, name) => { const o = [...e.options].find(o => o.text.includes(name)); e.value = o.value; }",
                parentName);
        }
    }
}
