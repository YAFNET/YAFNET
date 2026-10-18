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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The edit page of an album: the title, deleting images and the album, and the album of another user.
/// </summary>
[TestFixture]
public class AlbumEditTests : ForumTestBase
{
    /// <summary>
    /// A png image of 1 x 1 pixel.
    /// </summary>
    private static readonly byte[] Png = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

    [Test]
    [Description("The title is changed on the edit page, an image and the album are deleted")]
    public async Task Title_Image_And_Album_Delete_Test()
    {
        var member = $"Alb{DateTime.UtcNow:HHmmssfff}";
        var title = $"Album {Guid.NewGuid():N}".Substring(0, 20);
        var renamed = $"Renamed {Guid.NewGuid():N}".Substring(0, 20);

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        var albumId = await this.CreateAlbumAsync(title, images: 2);

        // the title
        await this.Control("txtTitle").FillAsync(renamed);
        await this.ClickAndWaitAsync(this.Control("UpdateTitle"));
        (await this.Control("txtTitle").InputValueAsync()).Should().Be(renamed);

        // an image
        (await this.Control("ImageDelete").CountAsync()).Should().Be(2);
        await this.Control("ImageDelete").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        await this.GotoAsync($"EditAlbumImages?a={albumId}");
        (await this.Control("ImageDelete").CountAsync()).Should().Be(1);

        // the album
        await this.Control("Delete").ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        this.Page.Url.Should().Contain("Albums");
        (await this.Page.InnerTextAsync("body")).Should().NotContain(renamed);
    }

    [Test]
    [Description("Another member can not edit the album")]
    public async Task Album_Of_Other_User_Test()
    {
        var owner = $"Own{DateTime.UtcNow:HHmmssfff}";
        var other = $"Oth{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(owner, TestConfig.TestUserPassword);
        await this.RegisterAsync(other, TestConfig.TestUserPassword);

        await this.LoginAsync(owner, TestConfig.TestUserPassword);
        var albumId = await this.CreateAlbumAsync($"Private {Guid.NewGuid():N}".Substring(0, 20), images: 1);

        await this.LoginAsync(other, TestConfig.TestUserPassword);
        await this.GotoAsync($"EditAlbumImages?a={albumId}");

        (await this.Control("ImageDelete").CountAsync()).Should().Be(0);
        (await this.Control("UpdateTitle").CountAsync()).Should().Be(0);
    }

    [Test]
    [Description("A file that is not an image is refused, back goes to the album (or to the albums for a new one)")]
    public async Task Refused_File_And_Back_Test()
    {
        var member = $"Alr{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        // back of a new album
        await this.GotoAsync("EditAlbumImages?a=new");
        await this.ClickAndWaitAsync(this.Control("Back"));
        this.Page.Url.Should().Contain("Albums");

        var albumId = await this.CreateAlbumAsync($"Refused {Guid.NewGuid():N}".Substring(0, 20), images: 1);

        // a text file and a file with another extension
        foreach (var file in new[]
                     {
                         new FilePayload { Name = "notes.txt", MimeType = "text/plain", Buffer = [65, 66] },
                         new FilePayload { Name = "program.exe", MimeType = "application/octet-stream", Buffer = [77, 90] }
                     })
        {
            await this.GotoAsync($"EditAlbumImages?a={albumId}");
            await this.Control("File").SetInputFilesAsync(file);
            await this.ClickAndWaitAsync(this.Control("Upload"));

            (await this.Control("ImageDelete").CountAsync()).Should().Be(1, file.Name);
        }

        await this.GotoAsync($"EditAlbumImages?a={albumId}");
        await this.ClickAndWaitAsync(this.Control("Back"));
        this.Page.Url.Should().Contain("Album");
    }

    [Test]
    [Description("The cover of an album without images and an unknown cover are not found (no error)")]
    public async Task Cover_Of_Empty_Album_Test()
    {
        var member = $"Cov{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        var albumId = await this.CreateAlbumAsync($"Empty {Guid.NewGuid():N}".Substring(0, 20), images: 1);

        await this.Control("ImageDelete").First.ClickAsync();
        await this.ClickAndWaitAsync(this.Page.Locator(".bootbox .btn-success"));

        foreach (var cover in new[] { "0", "999999" })
        {
            var response = await this.Page.APIRequest.GetAsync(
                $"{TestConfig.BaseUrl}resource.ashx?album={albumId}&cover={cover}");

            response.Status.Should().Be(404, cover);
        }
    }

    [Test]
    [Description("The caption of an image and the title of the album are changed inline, they are shown as typed (encoded once)")]
    public async Task Inline_Caption_And_Title_Test()
    {
        var member = $"Alc{DateTime.UtcNow:HHmmssfff}";
        var title = $"Cats & <b>Dogs</b> {Guid.NewGuid():N}".Substring(0, 30);
        var caption = $"Mice & <i>Birds</i> {Guid.NewGuid():N}".Substring(0, 30);

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);

        await this.CreateAlbumAsync($"Inline {Guid.NewGuid():N}".Substring(0, 20), images: 1);

        // the album page
        await this.ClickAndWaitAsync(this.Control("Back"));
        var albumUrl = this.Page.Url;

        await this.EditInlineAsync(this.Page.Locator("a.album-image-caption").First, caption, "ChangeImageCaption");

        await this.Page.GotoAsync(albumUrl);
        (await this.Page.Locator("a.album-image-caption").First.InnerTextAsync()).Should().Be(caption);
        (await this.Page.InnerTextAsync("body")).Should().NotContain("&amp;");

        // the albums page
        await this.GotoAsync($"Albums?u={Regex.Match(albumUrl, @"[?&]u=(\d+)").Groups[1].Value}");
        var albumsUrl = this.Page.Url;

        await this.EditInlineAsync(this.Page.Locator("a.album-caption").First, title, "ChangeAlbumTitle");

        await this.Page.GotoAsync(albumsUrl);
        (await this.Page.Locator("a.album-caption").First.InnerTextAsync()).Should().Be(title);
        (await this.Page.InnerTextAsync("body")).Should().NotContain("&amp;");

        await this.Page.GotoAsync(albumUrl);
        (await this.Page.InnerTextAsync("body")).Should().Contain(title).And.NotContain("&amp;");
    }

    /// <summary>
    /// Changes the text of an inline editor (a popover with a form).
    /// </summary>
    private async Task EditInlineAsync(ILocator link, string text, string action)
    {
        await link.ClickAsync();

        var input = this.Page.Locator(".popover input[type='text']").First;
        await input.FillAsync(text);

        var response = await this.Page.RunAndWaitForResponseAsync(
            () => input.PressAsync("Enter"),
            r => r.Url.Contains(action));

        response.Status.Should().Be(200, action);
    }

    [Test]
    [Description("An image bigger than the maximum size of the album images is refused")]
    public async Task Too_Big_Image_Test()
    {
        var member = $"Alg{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);
        await this.LoginAsync(member, TestConfig.TestUserPassword);
        var albumId = await this.CreateAlbumAsync($"Big {Guid.NewGuid():N}".Substring(0, 20), images: 1);

        await this.LoginAsAdminAsync();
        await this.SetAlbumImagesSizeMaxAsync("10");

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            await this.GotoAsync($"EditAlbumImages?a={albumId}");
            await this.Control("File").SetInputFilesAsync(
                new FilePayload { Name = "image2.png", MimeType = "image/png", Buffer = Png });
            await this.ClickAndWaitAsync(this.Control("Upload"));

            (await this.Page.ContentAsync()).Should().Contain("You cannot upload images that big.");
            (await this.Control("ImageDelete").CountAsync()).Should().Be(1);
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetAlbumImagesSizeMaxAsync("1048576");
        }
    }

    private async Task SetAlbumImagesSizeMaxAsync(string value)
    {
        await this.GotoAsync("Admin/HostSettings");
        await this.Control("AlbumImagesSizeMax").EvaluateAsync("(e, v) => e.value = v", value);
        await this.ClickAndWaitAsync(this.Control("Save"));
    }

    [Test]
    [Description("The limits of the role: the second image with the same name gets a new name, then the album is full and no second album can be created")]
    public async Task Album_Limits_And_Same_Name_Test()
    {
        var member = $"All{DateTime.UtcNow:HHmmssfff}";

        await this.RegisterAsync(member, TestConfig.TestUserPassword);

        await this.LoginAsAdminAsync();
        var old = await this.SetAlbumLimitsAsync("1", "2");

        try
        {
            await this.LoginAsync(member, TestConfig.TestUserPassword);
            var albumId = await this.CreateAlbumAsync($"Limits {Guid.NewGuid():N}".Substring(0, 20), images: 1);

            // the same name again
            await this.GotoAsync($"EditAlbumImages?a={albumId}");
            await this.Control("File").SetInputFilesAsync(
                new FilePayload { Name = "image1.png", MimeType = "image/png", Buffer = Png });
            await this.ClickAndWaitAsync(this.Control("Upload"));

            (await this.Page.InnerTextAsync("body")).Should().Contain("image1 (2).png");

            // the album is full
            (await this.Control("Upload").CountAsync()).Should().Be(0, "the album has the allowed number of images");

            // no second album
            await this.GotoAsync("EditAlbumImages?a=new");
            this.Page.Url.Should().Contain("Info", "the member has the allowed number of albums");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetAlbumLimitsAsync(old.Albums, old.Images);
        }
    }

    /// <summary>
    /// Sets the album limits of the registered users, returns the old limits.
    /// </summary>
    private async Task<(string Albums, string Images)> SetAlbumLimitsAsync(string albums, string images)
    {
        await this.GotoAsync("Admin/Groups");
        await this.ClickAndWaitAsync(
            this.Page.Locator("li", new PageLocatorOptions { HasTextString = "Registered Users" })
                .Filter(new LocatorFilterOptions { Has = this.Page.Locator("[id*='_RoleListYaf_']") })
                .Locator("[id$='_ThemeButtonEdit']").First);

        var old = (await this.Control("UsrAlbums").InputValueAsync(), await this.Control("UsrAlbumImages").InputValueAsync());

        await this.Control("UsrAlbums").EvaluateAsync("(e, v) => { e.type = 'text'; e.value = v; }", albums);
        await this.Control("UsrAlbumImages").EvaluateAsync("(e, v) => { e.type = 'text'; e.value = v; }", images);
        await this.ClickAndWaitAsync(this.Control("Save"));

        return old;
    }

    private async Task<string> CreateAlbumAsync(string title, int images)
    {
        await this.GotoAsync("EditAlbumImages?a=new");

        await this.Control("txtTitle").FillAsync(title);
        await this.Control("File").SetInputFilesAsync(
            new FilePayload { Name = "image1.png", MimeType = "image/png", Buffer = Png });
        await this.ClickAndWaitAsync(this.Control("Upload"));

        var albumId = Regex.Match(this.Page.Url, @"[?&]a=(\d+)").Groups[1].Value;

        for (var i = 2; i <= images; i++)
        {
            await this.Control("File").SetInputFilesAsync(
                new FilePayload { Name = $"image{i}.png", MimeType = "image/png", Buffer = Png });
            await this.ClickAndWaitAsync(this.Control("Upload"));
        }

        return albumId;
    }
}
