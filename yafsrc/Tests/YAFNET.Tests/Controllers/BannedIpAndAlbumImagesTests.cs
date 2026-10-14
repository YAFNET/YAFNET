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
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Controllers.Modals;
using YAF.Core.Extensions;
using YAF.Core.Middleware;
using YAF.Core.Services.Import;
using YAF.Core.Tasks;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Modals;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Banned IPs (admin dialog, import and the request check), the album image list and the task clean up.
/// </summary>
[TestFixture]
[NonParallelizable]
public class BannedIpAndAlbumImagesTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    [SetUp]
    public void SetUp()
    {
        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<UrlActionContext>()).Returns(c => $"/preview/{c.Arg<UrlActionContext>().Values}");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<DataImporter>().As<IDataImporter>();
                b.RegisterInstance(urlHelper).As<IUrlHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.board.LoginAs(this.alice);
        this.board.PageLoad.IsAdmin = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("Banned IPs: addresses and wildcard masks are saved (trimmed), duplicates rejected, existing bans updated")]
    public void Edit_Test()
    {
        var controller = this.board.CreateController<BannedIpController>();

        controller.Edit(new BannedEmailEditModal { Mask = " 10.0.0.1 ", BanReason = " spam " }).Should().BeOfType<OkResult>();
        controller.Edit(new BannedEmailEditModal { Mask = "192.168.*.*" }).Should().BeOfType<OkResult>();
        Notification(controller.Edit(new BannedEmailEditModal { Mask = "10.0.0.1" })).MessageType
            .Should().Be(MessageTypes.warning.ToString(), "the address is already banned");

        var ban = this.Repository<BannedIP>().GetSingle(b => b.Mask == "10.0.0.1");
        ban.Reason.Should().Be("spam");
        ban.UserID.Should().Be(this.alice);

        controller.Edit(new BannedEmailEditModal { Id = ban.ID, Mask = "10.0.0.2", BanReason = "changed" }).Should().BeOfType<OkResult>();

        this.Repository<BannedIP>().Get(_ => true).Select(b => b.Mask).Should().BeEquivalentTo(["10.0.0.2", "192.168.*.*"]);
    }

    [TestCase("1.2.3")]
    [TestCase("1.2.3.4.5")]
    [TestCase("1.2.3.256")]
    [TestCase("1.2.x.4")]
    [TestCase("1..3.4")]
    [Description("Invalid addresses are not saved, the dialog gets a warning message (not a redirect)")]
    public void Edit_Invalid_Test(string mask)
    {
        var result = this.board.CreateController<BannedIpController>().Edit(new BannedEmailEditModal { Mask = mask });

        Notification(result).Should().NotBeNull("the dialog expects a json message");
        Notification(result).MessageType.Should().Be(MessageTypes.warning.ToString());
        this.Repository<BannedIP>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("IPv6 addresses can be banned in the dialog, like in the import")]
    public void Edit_IPv6_Test()
    {
        this.board.CreateController<BannedIpController>().Edit(new BannedEmailEditModal { Mask = "2001:db8::1" })
            .Should().BeOfType<OkResult>();

        this.Repository<BannedIP>().Get(_ => true).Select(b => b.Mask).Should().Equal("2001:db8::1");
    }

    [Test]
    [Description("An empty mask is rejected with a message instead of an error")]
    public void Edit_Empty_Test()
    {
        var controller = this.board.CreateController<BannedIpController>();

        controller.Invoking(c => c.Edit(new BannedEmailEditModal { Mask = null })).Should().NotThrow();
        this.Repository<BannedIP>().Count(_ => true).Should().Be(0);
    }

    [Test]
    [Description("The ban date is stored in UTC like all other dates (it is shown converted from UTC to the user's time zone)")]
    public void Ban_Date_Is_Utc_Test()
    {
        this.board.CreateController<BannedIpController>().Edit(new BannedEmailEditModal { Mask = "10.0.0.1" });
        this.board.CreateController<BannedNameController>().Edit(new BannedEmailEditModal { Mask = "^casino" });
        this.board.CreateController<BannedEmailController>().Edit(new BannedEmailEditModal { Mask = @"spam@example\.com" });

        this.Repository<BannedIP>().GetSingle(_ => true).Since.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.Repository<BannedName>().GetSingle(_ => true).Since.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        this.Repository<BannedEmail>().GetSingle(_ => true).Since.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Test]
    [Description("Banned IP and name import: only text files, the result is reported")]
    public async Task Import_Test()
    {
        var ips = this.board.CreateController<BannedIpController>();
        var names = this.board.CreateController<BannedNameController>();

        Notification(await ips.Import(File("x", "application/octet-stream"))).MessageType.Should().Be(MessageTypes.danger.ToString());
        Notification(await names.Import(File("x", "image/png"))).MessageType.Should().Be(MessageTypes.danger.ToString());

        Notification(await ips.Import(File("10.0.0.1\n10.0.0.2", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());
        Notification(await ips.Import(File("10.0.0.1", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString(), "nothing new is no error");
        Notification(await names.Import(File("^casino", "text/plain"))).MessageType.Should().Be(MessageTypes.success.ToString());

        (await this.Repository<BannedIP>().CountAsync(_ => true)).Should().Be(2);
        (await this.Repository<BannedName>().CountAsync(_ => true)).Should().Be(1);
    }

    [TestCase("203.0.113.10", false)]
    [TestCase("203.0.113.*", false)]
    [TestCase("203.0.113.11", true)]
    [TestCase("198.51.*.*", true)]
    [Description("Requests from banned IPs (or masks) are ended with 403, others pass")]
    public async Task Check_Banned_Ips_Test(string mask, bool allowed)
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = $" {mask} ", Since = DateTime.UtcNow });

        var (next, status) = await this.RunCheckAsync();

        next.Should().Be(allowed);
        status.Should().Be(allowed ? 200 : 403);
    }

    [Test]
    [Description("Behind a reverse proxy the client address of the forwarded headers is checked (like the country check), not the proxy address")]
    public async Task Check_Banned_Ips_Behind_Proxy_Test()
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = "198.51.100.7", Since = DateTime.UtcNow });
        this.board.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
        this.board.HttpContext.Request.Headers["X-Forwarded-For"] = "198.51.100.7";

        // the forwarded headers middleware runs first (trusted proxy on the same host)
        await this.RunForwardedHeadersAsync();

        var (next, status) = await this.RunCheckAsync();

        next.Should().BeFalse();
        status.Should().Be(403);
    }

    [Test]
    [Description("A client (not a trusted proxy) can not escape a ban with a forwarded header of another address")]
    public async Task Check_Banned_Ips_Spoofed_Forwarded_For_Test()
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = "203.0.113.7", Since = DateTime.UtcNow });
        this.board.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("203.0.113.7");
        this.board.HttpContext.Request.Headers["X-Forwarded-For"] = "198.51.100.99";

        await this.RunForwardedHeadersAsync();

        var (next, status) = await this.RunCheckAsync();

        next.Should().BeFalse();
        status.Should().Be(403);
    }

    private Task RunForwardedHeadersAsync()
    {
        var middleware = new Microsoft.AspNetCore.HttpOverrides.ForwardedHeadersMiddleware(
            _ => Task.CompletedTask,
            Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory.Instance,
            Microsoft.Extensions.Options.Options.Create(ForwardedHeadersHelper.CreateOptions([])));

        return middleware.Invoke(this.board.HttpContext);
    }

    [Test]
    [Description("Requests without a remote address (e.g. in-process requests) are not banned and do not fail")]
    public async Task Check_Banned_Ips_No_Address_Test()
    {
        this.Insert(new BannedIP { BoardID = TestDatabase.BoardId, Mask = "10.0.0.1", Since = DateTime.UtcNow });
        this.board.HttpContext.Connection.RemoteIpAddress = null;

        var (next, _) = await this.RunCheckAsync();

        next.Should().BeTrue();
    }

    [Test]
    [Description("The album image list for the editor: only the own images, newest first, paged, with an encoded caption or file name")]
    public void Album_Images_Test()
    {
        var own = this.AddAlbum(this.alice);
        var foreign = this.AddAlbum(this.db.AddUser("Bob"));
        this.AddImage(own, "a.png", "Beach");
        var second = this.AddImage(own, "b<x>.png", null);
        this.AddImage(foreign, "c.png", null);

        var controller = this.board.CreateController<AlbumController>();

        var page = Grid(controller.GetAlbumImages(new PagedResults { PageNumber = 0, PageSize = 1 }));

        page.TotalRecords.Should().Be(2);
        page.AttachmentList.Should().ContainSingle();
        page.AttachmentList[0].FileName.Should().Be("b<x>.png");
        page.AttachmentList[0].OnClick.Should().Contain($"'{second}'");
        page.AttachmentList[0].IconImage.Should().Contain("b&lt;x&gt;.png").And.NotContain("<x>");

        var all = Grid(controller.GetAlbumImages(new PagedResults { PageNumber = 0, PageSize = 10 }));
        all.AttachmentList.Select(a => a.FileName).Should().Equal("b<x>.png", "a.png");
        all.AttachmentList[1].IconImage.Should().Contain("alt=\"Beach\"");

        this.board.LoginAs(this.db.AddUser("Carol"));
        var empty = Grid(this.board.CreateController<AlbumController>().GetAlbumImages(new PagedResults { PageNumber = 0, PageSize = 10 }));
        empty.TotalRecords.Should().Be(0);
        empty.AttachmentList.Should().BeEmpty();
    }

    [Test]
    [Description("The clean up task removes finished and missing tasks and keeps running ones")]
    public async Task CleanUp_Task_Test()
    {
        await new CleanUpTask().Invoking(t => t.RunOnceAsync()).Should().NotThrowAsync("without a task manager nothing happens");

        var running = Substitute.For<IBackgroundTask>();
        running.IsRunning.Returns(true);
        var finished = Substitute.For<IBackgroundTask>();

        var manager = Substitute.For<ITaskModuleManager>();
        manager.TaskManagerInstances.Returns(["running", "finished", "missing"]);
        manager.TryGetTask("running", out Arg.Any<IBackgroundTask>()).Returns(
            c =>
            {
                c[1] = running;
                return true;
            });
        manager.TryGetTask("finished", out Arg.Any<IBackgroundTask>()).Returns(
            c =>
            {
                c[1] = finished;
                return true;
            });
        manager.TryGetTask("missing", out Arg.Any<IBackgroundTask>()).Returns(false);

        var task = new CleanUpTask { TaskManager = manager };
        task.TaskName.Should().Be(nameof(CleanUpTask));
        task.TaskDescription.Should().NotBeEmpty();

        await task.RunOnceAsync();

        manager.DidNotReceive().TryRemoveTask("running");
        running.DidNotReceive().Dispose();
        manager.Received(1).TryRemoveTask("finished");
        finished.Received(1).Dispose();
        manager.Received(1).TryRemoveTask("missing");
    }

    private static MessageModalNotification Notification(IActionResult result)
    {
        return (result as OkObjectResult)?.Value as MessageModalNotification;
    }

    private static GridDataSet Grid(IActionResult result)
    {
        return (GridDataSet)((OkObjectResult)result).Value;
    }

    private static FormFile File(string content, string contentType)
    {
        var bytes = Encoding.UTF8.GetBytes(content);

        return new FormFile(new MemoryStream(bytes), 0, bytes.Length, "file", "import.txt")
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };
    }

    private async Task<(bool Next, int Status)> RunCheckAsync()
    {
        var nextCalled = false;

        await new CheckBannedIps(
            _ =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            },
            this.board.Resolve<IServiceLocator>()).InvokeAsync(this.board.HttpContext);

        return (nextCalled, this.board.HttpContext.Response.StatusCode);
    }

    private void Insert<T>(T entity)
    {
        using var connection = this.db.Open();
        connection.Insert(entity);
    }

    private int AddAlbum(int userId)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(new UserAlbum { UserID = userId, Title = "Album", Updated = DateTime.UtcNow }, selectIdentity: true);
    }

    private int AddImage(int albumId, string fileName, string caption)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new UserAlbumImage { AlbumID = albumId, FileName = fileName, Caption = caption, ContentType = "image/png", Bytes = 3, Uploaded = DateTime.UtcNow },
            selectIdentity: true);
    }

    private IRepository<T> Repository<T>()
        where T : class, YAF.Types.Interfaces.Data.IEntity, new()
    {
        return this.board.Current.GetRepository<T>();
    }
}
