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

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The lists of the select boxes (web api): topics and users only for moderators, forums, tags, search and
/// notifications.
/// </summary>
[TestFixture]
public class ApiListsTests : ForumTestBase
{
    [Test]
    [Description("The topics and the users of the moderator select boxes are only listed to moderators")]
    public async Task Moderator_Lists_Test()
    {
        await this.LoginAsAdminAsync();

        var topics = await this.PostAsync("Topic/GetTopics", "{\"ForumId\":1,\"Page\":0}");
        topics.Status.Should().Be(200);
        (await topics.TextAsync()).Should().Contain("Results");

        var search = await this.PostAsync("Topic/GetTopics", "{\"ForumId\":1,\"Page\":0,\"SearchTerm\":\"a\"}");
        search.Status.Should().Be(200);

        var users = await this.PostAsync("User/GetUsers", "{\"Page\":0,\"SearchTerm\":\"Watch\"}");
        users.Status.Should().Be(200);
        (await users.TextAsync()).Should().Contain(TestConfig.AdminUserName);

        // a member
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        (await this.PostAsync("Topic/GetTopics", "{\"ForumId\":1,\"Page\":0}")).Status.Should().Be(404);
        (await this.PostAsync("User/GetUsers", "{\"Page\":0}")).Status.Should().Be(404);
    }

    [Test]
    [Description("The forums, the tags, the similar topics and the notifications of a member")]
    public async Task Member_Lists_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        var forums = await this.PostAsync("Forum/GetForums", "{\"Page\":0,\"AllForumsOption\":true}");
        forums.Status.Should().Be(200);
        (await forums.TextAsync()).Should().Contain("Test Forum");

        (await this.PostAsync("Forum/GetForums", "{\"Page\":0,\"SearchTerm\":\"test\"}")).Status.Should().Be(200);
        (await this.PostAsync("Forum/GetForum/1", "{}")).Status.Should().Be(200);
        (await this.PostAsync("Forum/GetForum/0", "{}")).Status.Should().Be(200);

        (await this.PostAsync("Tags/GetBoardTags", "{\"Page\":0}")).Status.Should().Be(200);
        (await this.PostAsync("Tags/GetBoardTags", "{\"Page\":0,\"SearchTerm\":\"x\"}")).Status.Should().Be(200);

        (await this.PostAsync("Search/GetSimilarTitles", "{\"SearchTerm\":\"Welcome\"}")).Status.Should().Be(200);
        (await this.PostAsync("Search/GetSearchResults", "{\"SearchTerm\":\"Welcome\",\"Page\":0,\"PageSize\":5}")).Status
            .Should().Be(200);

        (await this.PostAsync("Notify/GetNotifications", "{\"PageNumber\":0}")).Status.Should().Be(200);
    }

    private async Task<IAPIResponse> PostAsync(string action, string body)
    {
        await this.GotoAsync(string.Empty);

        return await this.Page.APIRequest.PostAsync(
                   $"{TestConfig.BaseUrl}api/{action}",
                   new APIRequestContextOptions
                       {
                           Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json;charset=utf-8" },
                           Data = body
                       });
    }
}
