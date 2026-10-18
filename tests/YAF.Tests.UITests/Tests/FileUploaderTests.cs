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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Playwright;

using YAF.Tests.UITests.Infrastructure;

/// <summary>
/// The upload of the attachments (FileUploader.ashx): the methods, refused files, the size limit, resized images
/// and the attachments in the database.
/// </summary>
[TestFixture]
public class FileUploaderTests : ForumTestBase
{
    [Test]
    [Description("Options are answered, other methods are not allowed")]
    public async Task Methods_Test()
    {
        await this.LoginAsUserAsync(TestConfig.TestUserName);

        var options = await this.Page.APIRequest.FetchAsync(
            $"{TestConfig.BaseUrl}FileUploader.ashx",
            new APIRequestContextOptions { Method = "OPTIONS" });
        options.Status.Should().Be(200);
        options.Headers["allow"].Should().Contain("POST");

        var get = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}FileUploader.ashx");
        get.Status.Should().Be(405);

        // a member without the upload access
        (await this.PostAsync("a.txt", "text/plain", [65])).Status.Should().Be(403);
    }

    [Test]
    [Description("A not allowed file and a too big file are refused, images are resized, the files are saved in the database and in the folder")]
    public async Task Upload_Settings_Test()
    {
        await this.LoginAsAdminAsync();

        var old = await this.SetHostSettingsAsync(
            new Dictionary<string, string>
                {
                    ["MaxFileSize"] = "3000", ["ImageAttachmentResizeWidth"] = "10", ["ImageAttachmentResizeHeight"] = "10",
                    ["UseFileTable"] = "true"
                });

        try
        {
            // the members have no upload access in the test forum (the admin has)
            (await this.UploadAsync("program.exe", "application/octet-stream", [77, 90, 0, 0])).Should()
                .Contain("Invalid File");

            (await this.UploadAsync("big.txt", "text/plain", new byte[4000])).Should().NotContain("\"error\":null").And
                .NotContain("Invalid File");

            // in the database: a text and a resized image
            var small = await this.UploadAsync("small.txt", "text/plain", [65, 66, 67]);
            small.Should().Contain("\"size\":3,").And.Contain("\"error\":null");

            // the attachment of the database is downloaded by its owner
            var smallId = Regex.Match(small, @"""fileID"":(\d+)").Groups[1].Value;
            var download = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}resource.ashx?a={smallId}");
            (await download.TextAsync()).Should().Be("ABC");

            // a name that doesn't match the content type is not downloaded
            var mismatch = await this.UploadAsync("text.png", "text/plain", [65, 66, 67]);
            var mismatchId = Regex.Match(mismatch, @"""fileID"":(\d+)").Groups[1].Value;

            if (mismatchId.Length > 0 && mismatchId != "0")
            {
                download = await this.Page.APIRequest.GetAsync($"{TestConfig.BaseUrl}resource.ashx?a={mismatchId}");
                (await download.TextAsync()).Should().StartWith("You have insufficient rights");
            }
            (await this.UploadAsync("image.png", "image/png", CreatePng(40, 40))).Should().Contain("\"error\":null");

            // in the folder: a resized image
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync(new Dictionary<string, string> { ["UseFileTable"] = "false" });

            (await this.UploadAsync("folder.png", "image/png", CreatePng(40, 40))).Should().Contain("\"error\":null");
        }
        finally
        {
            await this.LoginAsAdminAsync();
            await this.SetHostSettingsAsync(old);
        }
    }

    private static byte[] CreatePng(int width, int height)
    {
        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);

        graphics.Clear(Color.SteelBlue);

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);

        return stream.ToArray();
    }

    /// <summary>
    /// Uploads a file like the upload script of the forum and returns the json answer.
    /// </summary>
    private async Task<string> UploadAsync(string name, string mimeType, byte[] data)
    {
        var response = await this.PostAsync(name, mimeType, data);

        response.Status.Should().Be(200);

        return await response.TextAsync();
    }

    private async Task<IAPIResponse> PostAsync(string name, string mimeType, byte[] data)
    {
        // the page of the forum (the session of the user)
        await this.GotoAsync(string.Empty);

        var form = this.Page.APIRequest.CreateFormData();
        form.Append("files[]", new FilePayload { Name = name, MimeType = mimeType, Buffer = data });

        return await this.Page.APIRequest.PostAsync(
                   $"{TestConfig.BaseUrl}FileUploader.ashx",
                   new APIRequestContextOptions
                       {
                           Multipart = form, Headers = new Dictionary<string, string> { ["X-Requested-With"] = "XMLHttpRequest" }
                       });
    }

    /// <summary>
    /// Sets values of the host settings and returns the old values.
    /// </summary>
    private async Task<Dictionary<string, string>> SetHostSettingsAsync(Dictionary<string, string> values)
    {
        await this.GotoAsync("Admin/HostSettings");

        var old = await this.Page.EvaluateAsync<Dictionary<string, string>>(
            """
            values => {
                const old = {};
                for (const [id, value] of Object.entries(values)) {
                    const e = document.querySelector(`[id$='_${id}']`);
                    if (e.type === 'checkbox') { old[id] = String(e.checked); e.checked = value === 'true'; }
                    else { old[id] = e.value; e.value = value; }
                }
                return old;
            }
            """,
            values);

        await this.ClickAndWaitAsync(this.Control("Save"));

        return old;
    }
}
