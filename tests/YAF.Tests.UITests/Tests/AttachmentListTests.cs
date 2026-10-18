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
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The attachment lists of the profile and of the admin edit user page.
/// </summary>
[TestFixture]
public class AttachmentListTests : ForumTestBase
{
    [Test]
    [Description("The own attachments are deleted (selected) on the profile page, the admin deletes them (one and selected) on the edit user page")]
    public async Task Delete_Attachments_Test()
    {
        var id = Guid.NewGuid().ToString("N").Substring(0, 10);

        await this.LoginAsAdminAsync();

        foreach (var name in new[] { "profile1", "admin1", "admin2" })
        {
            await this.UploadAsync($"{name}-{id}.txt");
        }

        // the profile page: another page size and the selected attachment
        await this.GotoAsync("Profile/Attachments");
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("PageSize").SelectOptionAsync("50"),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.Row($"profile1-{id}").Locator("input[type='checkbox']").CheckAsync();
        await this.Control("DeleteAttachment2").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));
        (await this.Row($"profile1-{id}").CountAsync()).Should().Be(0);

        // the edit user page: one attachment and the selected ones
        var adminId = await this.OpenAdminEditUserAsync(TestConfig.AdminUserName);
        await this.OpenAttachmentsTabAsync(adminId);
        await this.Page.RunAndWaitForResponseAsync(
            () => this.Control("UserAttachments_PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }),
            response => response.Request.Method == "POST");
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        await this.OpenAttachmentsTabAsync(adminId);
        await this.Row($"admin1-{id}").Locator("[id$='_ThemeButtonDelete']").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.OpenAttachmentsTabAsync(adminId);
        (await this.Row($"admin1-{id}").CountAsync()).Should().Be(0);

        await this.Row($"admin2-{id}").Locator("input[type='checkbox']").CheckAsync();
        await this.Control("UserAttachments_DeleteAttachment2").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.OpenAttachmentsTabAsync(adminId);
        (await this.Row($"admin2-{id}").CountAsync()).Should().Be(0);

        await this.ClickAndWaitAsync(this.Control("UserAttachments_Back"));
        this.Page.Url.Should().Contain("Admin/Users");
    }

    [Test]
    [Description("The admin list of all attachments: another page size and an attachment is deleted")]
    public async Task Admin_Attachments_Test()
    {
        var name = $"adminlist-{Guid.NewGuid().ToString("N").Substring(0, 10)}.txt";

        await this.LoginAsAdminAsync();
        await this.UploadAsync(name);

        await this.GotoAsync("Admin/Attachments");
        await this.Page.RunAndWaitForNavigationAsync(
            () => this.Control("PageSize").SelectOptionAsync("50", new LocatorSelectOptionOptions { Force = true }));

        (await this.Row(name).CountAsync()).Should().Be(1);

        await this.Row(name).Locator(".btn-group a:has(i.fa-trash)").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        (await this.Row(name).CountAsync()).Should().Be(0);
    }

    private ILocator Row(string name)
    {
        return this.Page.Locator("li", new PageLocatorOptions { HasTextString = name });
    }

    private async Task OpenAttachmentsTabAsync(int userId)
    {
        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a.nav-link[href='#View11']").ClickAsync();
    }

    /// <summary>
    /// Uploads a file like the upload script of the forum.
    /// </summary>
    private async Task UploadAsync(string name)
    {
        await this.GotoAsync(string.Empty);

        var form = this.Page.APIRequest.CreateFormData();
        form.Append("files[]", new FilePayload { Name = name, MimeType = "text/plain", Buffer = [65, 66, 67] });

        var response = await this.Page.APIRequest.PostAsync(
                           $"{TestConfig.BaseUrl}FileUploader.ashx",
                           new APIRequestContextOptions
                               {
                                   Multipart = form,
                                   Headers = new Dictionary<string, string> { ["X-Requested-With"] = "XMLHttpRequest" }
                               });

        response.Status.Should().Be(200);
    }
}
