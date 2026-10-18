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
/// The activity stream of the account page: the filters and the page size.
/// </summary>
[TestFixture]
public class ActivityTests : ForumTestBase
{
    [Test]
    [Description("The created topic is in the activity stream, the filter hides it, reset shows it again, another page size")]
    public async Task Activity_Filter_Test()
    {
        var member = $"Act{DateTime.UtcNow:HHmmssfff}";
        var subject = $"Activity {Guid.NewGuid():N}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("PostTopic?f=1");
        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync("A topic for the activity stream.");
        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("MyAccount");
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);

        // without the created topics
        await this.Page.Locator("[aria-label='Filters'] .dropdown-toggle").ClickAsync();
        await this.Control("CreatedTopic").UncheckAsync();
        await this.ClickAndWaitAsync(this.Control("Update"));
        (await this.Page.InnerTextAsync("body")).Should().NotContain(subject);

        // reset
        await this.Page.Locator("[aria-label='Filters'] .dropdown-toggle").ClickAsync();
        await this.ClickAndWaitAsync(this.Control("Reset"));
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);

        // another page size (posts back)
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("10", new LocatorSelectOptionOptions { Force = true }));
        await this.AssertNoServerErrorAsync();
        (await this.Page.InnerTextAsync("body")).Should().Contain(subject);
    }
}
