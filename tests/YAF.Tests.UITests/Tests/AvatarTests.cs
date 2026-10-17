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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The upload of avatars (admin edit user and the profile of a member).
/// </summary>
[TestFixture]
public class AvatarTests : ForumTestBase
{
    [Test]
    [Description("The admin uploads an avatar for a member, a too big image is resized, the avatar is deleted again")]
    public async Task Admin_Upload_Test()
    {
        var member = $"Ava{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        var userId = await this.OpenEditUserAvatarAsync(member);

        (await this.Control("AvatarEditControl_DeleteAvatar").IsVisibleAsync()).Should().BeFalse("the member has no avatar");

        await this.UploadAsync(CreatePng(80, 40, noise: false));

        (await this.Control("AvatarEditControl_DeleteAvatar").CountAsync()).Should().Be(1, "the avatar is saved");

        // the avatar of the member
        var avatar = await this.Control("AvatarEditControl_AvatarImg").GetAttributeAsync("src");
        var response = await this.Page.APIRequest.GetAsync(avatar);
        response.Status.Should().Be(200);
        response.Headers["content-type"].Should().StartWith("image/");

        // a too big image (width and height) is resized
        await this.OpenEditUserAvatarAsync(member);
        await this.UploadAsync(CreatePng(400, 400, noise: false));
        (await this.Page.ContentAsync()).Should().Contain("resized");

        // deleted
        await this.OpenEditUserAvatarAsync(member);
        await this.Control("AvatarEditControl_DeleteAvatar").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoAsync($"Admin/EditUser?u={userId}");
        await this.Page.Locator("a.nav-link[href='#View4']").ClickAsync();
        (await this.Control("AvatarEditControl_DeleteAvatar").CountAsync()).Should().Be(0, "the avatar is deleted");
    }

    [Test]
    [Description("An image file bigger than the avatar size (bytes) that is not resized is not saved")]
    public async Task Too_Big_File_Is_Not_Saved_Test()
    {
        var member = $"Big{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsAdminAsync();

        await this.OpenEditUserAvatarAsync(member);

        // within 200 x 100 pixels, more than 50000 bytes
        var png = CreatePng(199, 99, noise: true);
        png.Length.Should().BeGreaterThan(50000);

        await this.UploadAsync(png);

        (await this.Page.ContentAsync()).Should().Contain("50000");
        (await this.Control("AvatarEditControl_DeleteAvatar").CountAsync()).Should().Be(0, "the avatar is not saved");
    }

    [Test]
    [Description("With the avatar upload turned off a member can not upload an avatar with a made up post of the hidden upload button")]
    public async Task Member_Upload_Turned_Off_Test()
    {
        var member = $"Off{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.GotoAsync("Profile/EditAvatar");
        (await this.Control("UpdateUpload").CountAsync()).Should().Be(0, "the upload is turned off");

        // the upload button and the file of the (hidden) upload form
        var back = await this.Control("Back").GetAttributeAsync("name");
        var prefix = back.Substring(0, back.Length - "Back".Length);

        await this.Page.EvaluateAsync(
            @"prefix => {
                const form = document.forms[0];
                form.enctype = 'multipart/form-data';
                const file = document.createElement('input');
                file.type = 'file';
                file.id = 'madeUpFile';
                file.name = prefix + 'File';
                form.appendChild(file);
                const link = document.createElement('a');
                link.id = 'madeUpPost';
                link.href = `javascript:__doPostBack('${prefix}UpdateUpload','')`;
                link.textContent = 'upload';
                form.appendChild(link);
            }",
            prefix);

        await this.Page.Locator("#madeUpFile").SetInputFilesAsync(
            new FilePayload { Name = "avatar.png", MimeType = "image/png", Buffer = CreatePng(40, 40, noise: false) });

        var response = await this.Page.RunAndWaitForResponseAsync(
            () => this.Page.Locator("#madeUpPost").ClickAsync(),
            r => r.Request.Method == "POST");

        // the event of a button that is not on the page is refused
        response.Status.Should().Be(500);
        await this.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        this.ClearServerErrors();

        await this.GotoAsync("Profile/EditAvatar");
        (await this.Control("DeleteAvatar").CountAsync()).Should().Be(0, "no avatar was uploaded");
    }

    private static byte[] CreatePng(int width, int height, bool noise)
    {
        using var bitmap = new Bitmap(width, height);

        var random = new Random(1);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                bitmap.SetPixel(
                    x,
                    y,
                    noise ? Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)) : Color.SteelBlue);
            }
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);

        return stream.ToArray();
    }

    /// <summary>
    /// Opens the avatar tab of the admin edit user page of the user.
    /// </summary>
    private async Task<int> OpenEditUserAvatarAsync(string userName)
    {
        var userId = await this.OpenAdminEditUserAsync(userName);

        await this.Page.Locator("a.nav-link[href='#View4']").ClickAsync();

        return userId;
    }

    private async Task UploadAsync(byte[] png)
    {
        await this.Control("AvatarEditControl_File").SetInputFilesAsync(
            new FilePayload { Name = "avatar.png", MimeType = "image/png", Buffer = png });

        await this.ClickAndWaitAsync(this.Control("AvatarEditControl_UpdateUpload"));
    }
}
