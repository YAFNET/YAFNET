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

namespace YAF.Tests.Controllers;

using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

using NSubstitute;

using WebEssentials.AspNetCore.Pwa;

using YAF.Core.Controllers;
using YAF.Tests.TestInfrastructure;

/// <summary>
/// The service worker, the offline page and the web manifest of the progressive web app.
/// </summary>
[TestFixture]
[NonParallelizable]
public class PwaControllerTests
{
    private string webRoot;

    private TestBoardContext board;

    private PwaOptions options;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-pwa-{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.webRoot);

        File.WriteAllText(
            Path.Combine(this.webRoot, "customserviceworker.js"),
            "const version = '{version}'; const routes = [{routes}]; const offline = '{offlineRoute}'; "
            + "const html = '{offlineHtml}'; const ignore = [{ignoreRoutes}];");

        this.options = new PwaOptions
        {
            CacheId = "v2",
            RoutesToPreCache = "/, /forum/topics ,",
            BaseRoute = "/forum/",
            RoutesToIgnore = " /admin,/install",
            ServiceWorkerCacheControlMaxAge = 120,
            WebManifestCacheControlMaxAge = 60
        };

        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();

        try
        {
            Directory.Delete(this.webRoot, true);
        }
        catch (IOException)
        {
            // ignore
        }
    }

    [Test]
    [Description("The service worker is the custom script with the cache id, the routes and the offline page filled in")]
    public async Task Service_Worker_Test()
    {
        var controller = this.CreateController();

        var result = (ContentResult)await controller.ServiceWorkerAsync();

        result.Content.Should().Be(
            "const version = 'v2::CustomStrategy'; const routes = ['/','/forum/topics']; "
            + "const offline = '/forum/offline.html'; const html = '/forum/offline.html'; "
            + "const ignore = ['/admin','/install'];");

        controller.Response.ContentType.Should().Be("application/javascript; charset=utf-8");
        controller.Response.Headers.CacheControl.ToString().Should().Be("max-age=120");
    }

    [Test]
    [Description("The offline page is the embedded html page")]
    public async Task Offline_Page_Test()
    {
        var controller = this.CreateController();

        var result = (ContentResult)await controller.OfflineAsync();

        result.Content.Should().Contain("<html");
        controller.Response.ContentType.Should().Be("text/html");
    }

    [Test]
    [Description("The web manifest has the name and description of the board, empty values are left out")]
    public void Web_Manifest_Test()
    {
        this.board.BoardSettings.Name = "My \"Forum\"";
        this.board.BoardSettings.Description = "About <everything>";

        var controller = this.CreateController();

        var result = (ContentResult)controller.WebManifest(
            new WebManifest { Name = "old", ShortName = "Forum", StartUrl = "/" });

        using var json = JsonDocument.Parse(result.Content);
        json.RootElement.GetProperty("name").GetString().Should().Be("My \"Forum\"");
        json.RootElement.GetProperty("description").GetString().Should().Be("About <everything>");
        json.RootElement.GetProperty("short_name").GetString().Should().Be("Forum");
        json.RootElement.TryGetProperty("lang", out _).Should().BeFalse("empty values are not written");

        controller.Response.ContentType.Should().Be("application/manifest+json; charset=utf-8");
        controller.Response.Headers.CacheControl.ToString().Should().Be("max-age=60");
    }

    [Test]
    [Description("Without a web manifest there is none")]
    public void No_Web_Manifest_Test()
    {
        this.CreateController().WebManifest(null).Should().BeOfType<NotFoundResult>();
    }

    private PwaController CreateController()
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootFileProvider.Returns(new PhysicalFileProvider(this.webRoot));

        return new PwaController(this.options, new RetrieveCustomServiceworker(environment))
        {
            ControllerContext = new ControllerContext { HttpContext = this.board.HttpContext }
        };
    }
}
