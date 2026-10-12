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

using YAF.Tests.UITests.Infrastructure;

namespace YAF.Tests.UITests.AdminTests.Users;

/// <summary>
/// The Role (Group) Tests
/// </summary>
public class RoleTests(ComposeScenario scenario) : DatabaseTestBase(scenario)
{
    /// <summary>
    /// Create a new Test Group (Role)
    /// </summary>
    [Test]
    [Description("Create a new Test Group (Role)")]
    public async Task CreateNewGroupTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    // Log user in first!
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    // Do actual test

                    var random = new Random();

                    var roleName = $"TestRole{random.Next()}";

                    var pageSource = await page.CreateNewRole(this.TestSettings, roleName);

                    Assert.That(pageSource, Does.Contain(roleName), "Test Role creating failed");
                });
    }

    /// <summary>
    /// The guest group can't be deleted, also not with a manipulated form (the list has no delete button for it).
    /// </summary>
    [Test]
    [Description("The guest group can't be deleted")]
    public async Task GuestGroupCanNotBeDeletedTest()
    {
        await this.Fixture.Context.GotoPageAsync(
            this.TestSettings.TestForumUrl,
            async page =>
                {
                    Assert.That(
                        await page.LoginUserAsync(
                            this.TestSettings,
                            this.TestSettings.AdminUserName,
                            this.TestSettings.AdminPassword), Is.True,
                        "Login failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    // the group without delete button
                    const string findGuestGroupId = """
                                                    () => {
                                                        for (const group of document.querySelectorAll('.btn-group')) {
                                                            const edit = group.querySelector('a[href*="EditGroup"]');
                                                            if (edit && !group.querySelector('button[formaction*="Delete"]')) {
                                                                return edit.getAttribute('href').match(/(\d+)\D*$/)[1];
                                                            }
                                                        }
                                                        return null;
                                                    }
                                                    """;

                    var guestGroupId = await page.EvaluateAsync<string>(findGuestGroupId);

                    Assert.That(guestGroupId, Is.Not.Null, "The guest group is not listed");

                    // the delete handler with the id of the guest group
                    var status = await page.EvaluateAsync<int>(
                        """
                        async url => {
                            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                            const body = new URLSearchParams({ '__RequestVerificationToken': token });
                            const response = await fetch(url, { method: 'POST', body: body });
                            return response.status;
                        }
                        """,
                        $"{this.TestSettings.TestForumUrl}Admin/Groups?handler=Delete&id={guestGroupId}");

                    Assert.That(status, Is.EqualTo(200), "The delete handler failed");

                    await page.GotoAsync($"{this.TestSettings.TestForumUrl}Admin/Groups");

                    Assert.That(
                        await page.EvaluateAsync<string>(findGuestGroupId),
                        Is.EqualTo(guestGroupId),
                        "The guest group was deleted");
                });
    }
}
