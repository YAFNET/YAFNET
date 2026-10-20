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

namespace YAF.Tests.Services;

using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using YAF.Core.Middleware;

/// <summary>
/// <see cref="AntiXssMiddleware"/>: rejects requests with HTML / character references in the path,
/// query string or body (except for the api, profile and admin pages).
/// </summary>
[TestFixture]
public class AntiXssMiddlewareTests
{
    [Test]
    [Description("A harmless request reaches the next middleware with its body intact")]
    public async Task Harmless_Request_Passes_Test()
    {
        string seenBody = null;

        var (context, nextCalled) = await RunAsync(
            "/Posts/1",
            "?p=2&q=a<5",
            "text=hello [b]world[/b] 3 < 4",
            async c => seenBody = await new StreamReader(c.Request.Body).ReadToEndAsync());

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(200);
        seenBody.Should().Be("text=hello [b]world[/b] 3 < 4", "the body can still be read after the check");
    }

    [Test]
    [Description("Markup in the path, the (url encoded) query string or the body is rejected with 400")]
    [TestCase("/Posts/<script>", "", "")]
    [TestCase("/Search", "?q=%3Cscript%3Ealert(1)%3C%2Fscript%3E", "")]
    [TestCase("/Search", "?q=%26%2360%3B", "")]
    [TestCase("/Posts/1", "", "<img src=x onerror=alert(1)>")]
    [TestCase("/Posts/1", "", "<!-- comment -->")]
    public async Task Dangerous_Request_Rejected_Test(string path, string query, string body)
    {
        var (context, nextCalled) = await RunAsync(path, query, body);

        nextCalled.Should().BeFalse();
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().StartWith("application/json");
        ReadResponse(context).Should().Contain("AntiXssMiddleware").And.Contain("400", "the error code of the response matches the status code");
    }

    [Test]
    [Description("Url encoded form posts are checked as they are, not decoded (decoding would reject posts like 'a<b'); "
                 + "posted markup is handled by output encoding, not by this middleware")]
    public async Task UrlEncoded_Form_Not_Decoded_Test()
    {
        var (_, nextCalled) = await RunAsync(
            "/Posts/1",
            string.Empty,
            "text=%3Cscript%3Ealert(1)%3C%2Fscript%3E",
            contentType: "application/x-www-form-urlencoded");

        nextCalled.Should().BeTrue();
    }

    [Test]
    [Description("The api, profile and admin pages may post markup (e.g. editors, templates)")]
    [TestCase("/api/Attachments/Upload")]
    [TestCase("/Profile/EditSignature")]
    [TestCase("/Admin/EditBBCode")]
    [TestCase("/EditAlbumImages/1")]
    public async Task Exempt_Paths_Body_Not_Checked_Test(string path)
    {
        var (_, nextCalled) = await RunAsync(path, string.Empty, "<b>bold</b>");

        nextCalled.Should().BeTrue();
    }

    [Test]
    [Description("Exempt paths still reject markup in the url")]
    public async Task Exempt_Paths_Query_Checked_Test()
    {
        var (_, nextCalled) = await RunAsync("/Admin/Users", "?name=<script>", string.Empty);

        nextCalled.Should().BeFalse();
    }

    [Test]
    [Description("Bodies of exempt paths (file uploads) are passed through without being buffered")]
    public async Task Exempt_Paths_Body_Not_Buffered_Test()
    {
        Stream seen = null;
        var original = new MemoryStream(Encoding.UTF8.GetBytes("binary upload"));

        var context = CreateContext("/api/FileUpload/Upload", string.Empty, original);

        await new AntiXssMiddleware(c =>
        {
            seen = c.Request.Body;
            return Task.CompletedTask;
        }).InvokeAsync(context);

        seen.Should().BeSameAs(original, "the upload stream is handed on as it is");
        original.Position.Should().Be(0, "nothing was read from it");
    }

    [Test]
    [Description("The original body stream is restored after the request")]
    public async Task Body_Restored_Test()
    {
        var original = new MemoryStream(Encoding.UTF8.GetBytes("hello"));
        var context = CreateContext("/Posts/1", string.Empty, original);

        await new AntiXssMiddleware(_ => Task.CompletedTask).InvokeAsync(context);

        context.Request.Body.Should().BeSameAs(original);
    }

    [Test]
    [Description("A missing next delegate is rejected")]
    public void Null_Next_Test()
    {
        FluentActions.Invoking(() => new AntiXssMiddleware(null)).Should().Throw<ArgumentNullException>();
    }

    private async static Task<(HttpContext Context, bool NextCalled)> RunAsync(
        string path,
        string query,
        string body,
        Func<HttpContext, Task> next = null,
        string contentType = null)
    {
        var nextCalled = false;

        var context = CreateContext(path, query, new MemoryStream(Encoding.UTF8.GetBytes(body)));
        context.Request.ContentType = contentType;

        await new AntiXssMiddleware(
            async c =>
            {
                nextCalled = true;

                if (next != null)
                {
                    await next(c);
                }
            }).InvokeAsync(context);

        return (context, nextCalled);
    }

    private static DefaultHttpContext CreateContext(string path, string query, Stream body)
    {
        var context = new DefaultHttpContext { Request =
            {
                Method = "POST", Path = path, QueryString = new QueryString(query.IsNotSet() ? null : query), Body = body
            },
            Response = { Body = new MemoryStream() }
        };

        return context;
    }

    private static string ReadResponse(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return new StreamReader(context.Response.Body).ReadToEnd();
    }
}
