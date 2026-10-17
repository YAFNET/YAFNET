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
/// Every page opens without a server or JavaScript error, for guests, members and the admin; the admin and
/// moderator pages are refused to guests and members.
/// </summary>
[TestFixture]
public class PageSmokeTests : ForumTestBase
{
    /// <summary>
    /// The pages that guests can open.
    /// </summary>
    private static readonly string[] GuestPages =
        [
            string.Empty, "category/1-Test-Category", "Topics/1-Test Forum", "Posts/t1--Hello-from-YAF-NET",
            "Posts/m1--Hello-from-YAF-NET", "Search", "Members", "Team", "Help", "Cookies", "RulesAndPrivacy",
            "ActiveUsers?v=0", "UserProfile/2-Watcher", "Account/Login", "Account/Register", "Account/ForgotPassword"
        ];

    /// <summary>
    /// The pages of a logged in member.
    /// </summary>
    private static readonly string[] MemberPages =
        [
            "MyAccount", "MyMessages", "MyTopics", "Notification", "PostPrivateMessage", "PostTopic?f=1",
            "PostMessage?t=1&f=1", "PostMessage?t=1&f=1&q=1", "ReportPost?m=1", "Profile/Attachments",
            "Profile/BlockOptions", "Profile/ChangePassword", "Profile/EditAvatar", "Profile/EditProfile",
            "Profile/EditSettings", "Profile/EditSignature", "Profile/Subscriptions", "Search?postedby=Watcher",
            "Members?letter=W", "MessageHistory?m=1", "Albums?u=2", "EditAlbumImages?a=new"
        ];

    /// <summary>
    /// The admin and moderator pages.
    /// </summary>
    private static readonly string[] AdminPages =
        [
            "Admin/Admin", "Admin/AccessMasks", "Admin/Attachments", "Admin/BBCodes", "Admin/BannedEmails",
            "Admin/BannedIps", "Admin/BannedNames", "Admin/BannedUserAgents", "Admin/BoardAnnouncement",
            "Admin/Boards", "Admin/Digest", "Admin/EditUser?u=2", "Admin/EventLog", "Admin/Forums", "Admin/Groups",
            "Admin/HostSettings", "Admin/Languages", "Admin/Mail", "Admin/Medals", "Admin/PageAccessList",
            "Admin/Pm", "Admin/ProfileDefinitions", "Admin/Prune", "Admin/Ranks", "Admin/ReIndex",
            "Admin/ReplaceWords", "Admin/Restore", "Admin/RunSql", "Admin/Settings", "Admin/SpamLog",
            "Admin/SpamWords", "Admin/Tags", "Admin/TaskManager", "Admin/Users", "Admin/Version",
            "Admin/EditForum?fa=1", "Admin/EditCategory?c=1", "Admin/EditGroup?i=1", "Admin/EditRank?r=1",
            "Admin/EditAccessMask?i=1", "Moderate/Index", "Moderate/Forums?f=1", "EditMessage?m=1",
            "MoveMessage?m=1", "PollEdit?t=1"
        ];

    [TestCaseSource(nameof(GuestPages))]
    [Description("The pages for guests open without error")]
    public async Task Guest_Page_Test(string url)
    {
        await this.LogoutAsync();

        await this.GotoAsync(url);
    }

    [TestCaseSource(nameof(MemberPages))]
    [Description("The pages of a member open without error")]
    public async Task Member_Page_Test(string url)
    {
        await this.EnsureLoggedInAsync(TestConfig.TestUserName);

        await this.GotoAsync(url);
    }

    [TestCaseSource(nameof(AdminPages))]
    [Description("The admin and moderator pages open without error for the admin")]
    public async Task Admin_Page_Test(string url)
    {
        await this.EnsureLoggedInAsync(TestConfig.AdminUserName);

        await this.GotoAsync(url);

        this.Page.Url.Should().NotContain("Info?i=");
    }

    [TestCaseSource(nameof(AdminPages))]
    [Description("The admin and moderator pages are refused to a member")]
    public async Task Admin_Page_Refused_To_Member_Test(string url)
    {
        await this.EnsureLoggedInAsync(TestConfig.TestUserName);

        await this.GotoAsync(url);

        // the access denied info page (or the login page)
        this.Page.Url.Should().NotEndWith(url);
    }

    /// <summary>
    /// Logs in once for the test cases of the same user.
    /// </summary>
    private async Task EnsureLoggedInAsync(string userName)
    {
        await this.GotoAsync(string.Empty);

        if (await this.IsLoggedInAsAsync(userName))
        {
            return;
        }

        if (userName == TestConfig.AdminUserName)
        {
            await this.LoginAsAdminAsync();
        }
        else
        {
            await this.LoginAsUserAsync(userName);
        }
    }
}
