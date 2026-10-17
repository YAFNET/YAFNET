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
