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
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// Polls, albums and attachments.
/// </summary>
[TestFixture]
public class ContentFeatureTests : ForumTestBase
{
    /// <summary>
    /// A png image of 1 x 1 pixel.
    /// </summary>
    private static readonly byte[] Png = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

    [Test]
    [Description("The author adds a poll to a topic, the poll is shown in the topic")]
    public async Task Create_Poll_Test()
    {
        var question = $"Poll question {Guid.NewGuid():N}";

        await this.LoginAsAdminAsync();

        var topicHref = await this.CreateTopicAsync($"Poll topic {Guid.NewGuid():N}", "A topic with a poll.");
        var topicId = TopicIdOf(topicHref);

        await this.GotoAsync($"PollEdit?t={topicId}");

        await this.Control("Question").FillAsync(question);
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl00_PollChoice']").FillAsync("First choice");
        await this.Page.Locator("[id$='_ChoiceRepeater_ctl01_PollChoice']").FillAsync("Second choice");

        await this.ClickAndWaitAsync(this.Control("SavePoll"));

        await this.GotoAsync(topicHref);

        var text = await this.Page.InnerTextAsync("body");

        text.Should().Contain(question).And.Contain("First choice").And.Contain("Second choice");
    }

    [Test]
    [Description("A member creates an album with an image, the album and the image are shown")]
    public async Task Album_Test()
    {
        var title = $"Album {Guid.NewGuid():N}".Substring(0, 30);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("EditAlbumImages?a=new");

        await this.Control("txtTitle").FillAsync(title);
        await this.Control("File").SetInputFilesAsync(
            new FilePayload { Name = "image.png", MimeType = "image/png", Buffer = Png });

        await this.ClickAndWaitAsync(this.Control("Upload"));

        // the new album is opened for editing (EditAlbumImages?a={id})
        var albumId = Regex.Match(this.Page.Url, @"[?&]a=(\d+)").Groups[1].Value;
        albumId.Should().NotBeEmpty($"the new album is shown, but the page was {this.Page.Url}");

        (await this.Page.InnerTextAsync("body")).Should().Contain("image.png");

        // the image of the album can be loaded
        var image = this.Page.Locator("img[src*='resource.ashx']").First;
        var source = await image.GetAttributeAsync("src");

        var response = await this.Page.APIRequest.GetAsync(source);

        response.Status.Should().Be(200);
        response.Headers["content-type"].Should().StartWith("image/");
    }

    [Test]
    [Description("An attachment of a post is downloaded by another member with the signed link of the post, without the signature it is refused")]
    public async Task Attachment_Signed_Link_Test()
    {
        // the admin uploads (members have no upload access by default)
        await this.LoginAsAdminAsync();

        await this.GotoAsync("PostTopic?f=1");

        var subject = $"Attachment topic {Guid.NewGuid():N}";

        await this.Control("TopicSubjectTextBox").FillAsync(subject);

        // the upload dialog of the editor (its upload inserts the attachment into the text)
        await this.Page.Locator("[data-bs-target='#UploadDialog']").First.ClickAsync();
        await this.Page.Locator("#UploadDialog").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Page.Locator("#fileElem").SetInputFilesAsync(
            new FilePayload { Name = "attachment.png", MimeType = "image/png", Buffer = Png });

        var editor = this.Control("YafTextEditor");

        await this.Page.WaitForFunctionAsync(
            "() => document.querySelector(\"[id$='_YafTextEditor']\").value.toLowerCase().includes('[attach]')");

        var attachmentId = Regex.Match(await editor.InputValueAsync(), @"\[attach\](\d+)\[/attach\]", RegexOptions.IgnoreCase)
            .Groups[1].Value;

        attachmentId.Should().NotBeEmpty();

        await this.ClickAndWaitAsync(this.Control("PostReply"));

        // another member opens the post
        await this.LoginAsUserAsync(TestConfig.TestUserName2);

        await this.GotoAsync("Topics/1-Test Forum");

        var topicHref = await this.Page.Locator("a[href*='/Posts/t']", new () { HasTextString = subject }).First
                            .GetAttributeAsync("href");

        await this.GotoAsync(topicHref.TrimStart('/'));

        var links = await this.Page.Locator($"a[href*='resource.ashx'][href*='={attachmentId}&'], img[src*='resource.ashx'][src*='={attachmentId}&']")
                        .EvaluateAllAsync<string[]>("es => es.map(e => e.getAttribute('href') || e.getAttribute('src'))");

        links.Should().NotBeEmpty("the post shows the attachment");

        var signedLink = links.First(l => l.Contains("&s="));

        var download = await this.Page.APIRequest.GetAsync(signedLink);

        download.Status.Should().Be(200);
        (await download.BodyAsync()).Should().Equal(Png);

        // the same link with a wrong signature
        var unsignedLink = Regex.Replace(signedLink, "&s=[^&]*", "&s=invalid");

        var refused = await this.Page.APIRequest.GetAsync(unsignedLink);

        (await refused.BodyAsync()).Should().NotEqual(Png);
    }

    [Test]
    [Description("A member thanks the post of another member and removes the thanks again (api requests of the page)")]
    public async Task Thanks_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        var topicHref = await this.CreateTopicAsync($"Thanks topic {Guid.NewGuid():N}", "A post to thank.");

        await this.LoginAsUserAsync(TestConfig.TestUserName2);
        await this.GotoAsync(topicHref);

        await this.Page.Locator("a[href^='javascript:addThanks']").First.ClickAsync();
        await this.Page.Locator("a[href^='javascript:removeThanks']").First
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // stored: the page shows the remove button again
        await this.GotoAsync(topicHref);
        (await this.Page.Locator("a[href^='javascript:removeThanks']").CountAsync()).Should().Be(1);

        await this.Page.Locator("a[href^='javascript:removeThanks']").First.ClickAsync();
        await this.Page.Locator("a[href^='javascript:addThanks']").First
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.GotoAsync(topicHref);
        (await this.Page.Locator("a[href^='javascript:removeThanks']").CountAsync()).Should().Be(0);
    }

    [Test]
    [Description("The owner changes the album title in the album list (api request of the inline editor)")]
    public async Task Album_Title_Edit_Test()
    {
        var title = $"Renamed {Guid.NewGuid():N}".Substring(0, 20);

        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("EditAlbumImages?a=new");
        await this.Control("txtTitle").FillAsync("Album to rename");
        await this.Control("File").SetInputFilesAsync(
            new FilePayload { Name = "image.png", MimeType = "image/png", Buffer = Png });
        await this.ClickAndWaitAsync(this.Control("Upload"));

        var albumsHref = await this.Page.Locator("a[href*='Albums']").First.GetAttributeAsync("href");
        await this.GotoAsync(albumsHref.TrimStart('/'));

        await this.Page.Locator(".album-caption").First.ClickAsync();

        var input = this.Page.Locator(".popover form input");
        await input.FillAsync(title);
        await this.Page.Locator(".popover form button[type='submit']").ClickAsync();

        await this.Page.Locator(".popover form").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });

        await this.GotoAsync(albumsHref.TrimStart('/'));
        (await this.Page.InnerTextAsync("body")).Should().Contain(title);
    }

    [Test]
    [Description("Only images are shown in the page, other attachments (e.g. xml with scripts) are always downloaded")]
    public async Task Attachment_Xml_Is_Downloaded_Test()
    {
        await this.LoginAsAdminAsync();

        await this.GotoAsync("PostTopic?f=1");

        var subject = $"Xml attachment {Guid.NewGuid():N}";
        await this.Control("TopicSubjectTextBox").FillAsync(subject);

        await this.Page.Locator("[data-bs-target='#UploadDialog']").First.ClickAsync();
        await this.Page.Locator("#UploadDialog").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        await this.Page.Locator("#fileElem").SetInputFilesAsync(
            new FilePayload
            {
                Name = "evil.xml",
                MimeType = "text/xml",
                Buffer = System.Text.Encoding.UTF8.GetBytes(
                    "<html xmlns=\"http://www.w3.org/1999/xhtml\"><script>alert(1)</script></html>")
            });

        await this.Page.WaitForFunctionAsync(
            "() => document.querySelector(\"[id$='_YafTextEditor']\").value.toLowerCase().includes('[attach]')");

        var attachmentId = Regex.Match(
                await this.Control("YafTextEditor").InputValueAsync(),
                @"\[attach\](\d+)\[/attach\]",
                RegexOptions.IgnoreCase)
            .Groups[1].Value;

        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");
        var topicHref = await this.Page.Locator("a[href*='/Posts/t']", new () { HasTextString = subject }).First
                            .GetAttributeAsync("href");
        await this.GotoAsync(topicHref.TrimStart('/'));

        var link = await this.Page.Locator($"a[href*='resource.ashx'][href*='a={attachmentId}&']").First.GetAttributeAsync("href");

        // the same file with the address of an image
        var imageLink = link.Replace($"a={attachmentId}&", $"i={attachmentId}&");

        var response = await this.Page.APIRequest.GetAsync(imageLink);

        response.Headers.Should().ContainKey("content-disposition");
        response.Headers["content-disposition"].Should().StartWith("attachment");
        response.Headers["x-content-type-options"].Should().Be("nosniff");
    }

    [Test]
    [Description("An album image is only sent with an image type, a declared type with scripts (e.g. xhtml) is sent as png")]
    public async Task Album_Image_Content_Type_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        await this.GotoAsync("EditAlbumImages?a=new");
        await this.Control("txtTitle").FillAsync("Album with a script");
        await this.Control("File").SetInputFilesAsync(
            new FilePayload
            {
                Name = "image.png",
                MimeType = "application/xhtml+xml",
                Buffer = System.Text.Encoding.UTF8.GetBytes(
                    "<html xmlns=\"http://www.w3.org/1999/xhtml\"><script>alert(1)</script></html>")
            });
        await this.ClickAndWaitAsync(this.Control("Upload"));

        // the album page shows the previews of the images
        var albumId = Regex.Match(this.Page.Url, @"[?&]a=(\d+)").Groups[1].Value;
        var albumsHref = await this.Page.Locator("a[href*='Albums']").First.GetAttributeAsync("href");
        await this.GotoAsync(albumsHref.TrimStart('/'));

        var albumHref = await this.Page.Locator($"a[href*='a={albumId}']").First.GetAttributeAsync("href");
        await this.GotoAsync(albumHref.TrimStart('/'));

        var preview = await this.Page.Locator("img[src*='imgprv=']").First.GetAttributeAsync("src");
        var imageId = Regex.Match(preview, @"imgprv=(\d+)").Groups[1].Value;

        var response = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}resource.ashx?image={imageId}");

        response.Headers["content-type"].Should().StartWith("image/png");
        response.Headers["x-content-type-options"].Should().Be("nosniff");
    }

    private static int TopicIdOf(string topicHref)
    {
        return int.Parse(Regex.Match(topicHref, @"Posts/t(\d+)").Groups[1].Value);
    }

    /// <summary>
    /// Creates a topic in the test forum.
    /// </summary>
    /// <returns>Returns the address of the topic.</returns>
    private async Task<string> CreateTopicAsync(string subject, string text)
    {
        await this.GotoAsync("PostTopic?f=1");

        await this.Control("TopicSubjectTextBox").FillAsync(subject);
        await this.Control("YafTextEditor").FillAsync(text);

        await this.ClickAndWaitAsync(this.Control("PostReply"));

        await this.GotoAsync("Topics/1-Test Forum");

        var href = await this.Page.Locator("a[href*='/Posts/t']", new () { HasTextString = subject }).First
                       .GetAttributeAsync("href");

        return href.TrimStart('/');
    }
}
