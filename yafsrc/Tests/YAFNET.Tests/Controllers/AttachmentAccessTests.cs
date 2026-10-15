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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Helpers;
using YAF.Core.Extensions;
using YAF.Core.Utilities;
using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Interfaces;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// Attachment downloads: only for users with download access and only with the signed links of the forum (an
/// attachment can be shown in any post, the ids are counted up).
/// </summary>
[TestFixture]
[NonParallelizable]
public class AttachmentAccessTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    private int publicForum;

    private int staffForum;

    [SetUp]
    public void SetUp()
    {
        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(Path.GetTempPath());

        var urlHelper = Substitute.For<IUrlHelper>();
        urlHelper.Action(Arg.Any<Microsoft.AspNetCore.Mvc.Routing.UrlActionContext>()).Returns("/api/Attachments/GetAttachment?attachmentId=1");

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(urlHelper).As<IUrlHelper>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");

        this.publicForum = this.db.AddForum("Public");
        this.staffForum = this.db.AddForum("Staff");

        this.board.GrantAccess(this.alice, this.publicForum);
        this.board.GrantAccess(this.alice, this.staffForum);
        this.board.GrantAccess(this.bob, this.publicForum);

        SetMimeTypes(("png", "image/png"), ("txt", "text/plain"));

        this.board.PageLoad.DownloadAccess = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        SetMimeTypes();
    }

    [Test]
    [Description("Attachments of posts the user can read can be downloaded")]
    public async Task Readable_Post_Test()
    {
        var attachment = this.AddAttachment(this.alice, this.AddPost(this.publicForum));
        this.board.LoginAs(this.bob);

        (await this.DownloadAsync(attachment)).Should().Be("file content");
        (await this.DownloadAsync(attachment, response: true)).Should().Be("file content");
    }

    [Test]
    [Description("An attachment is only served with the signature of its link: any id could be downloaded by counting up")]
    public async Task Unsigned_Link_Test()
    {
        var attachment = this.AddAttachment(this.alice, this.AddPost(this.staffForum));
        var other = this.AddAttachment(this.bob, this.AddPost(this.publicForum));
        this.board.LoginAs(this.bob);

        (await this.DownloadAsync(attachment, signature: string.Empty)).Should().BeNull("the id alone is not enough");
        (await this.DownloadAsync(attachment, response: true, signature: string.Empty)).Should().BeNull();
        (await this.DownloadAsync(attachment, signature: AttachmentLinkHelper.Sign(other))).Should()
            .BeNull("the signature of another attachment");
        (await this.DownloadAsync(attachment, signature: "not+base64%")).Should().BeNull();
        (await this.DownloadAsync(attachment, signature: new string('A', 200))).Should().BeNull();

        (await this.DownloadAsync(attachment)).Should().Be("file content", "the link shown by the forum");
    }

    [Test]
    [Description("The key of the signatures is created once and stored, the signatures stay the same")]
    public void Signature_Key_Is_Stored_Test()
    {
        var signature = AttachmentLinkHelper.Sign(42);

        signature.Should().NotBe(AttachmentLinkHelper.Sign(43));
        signature.Length.Should().BeGreaterThan(20);

        this.board.Current.GetRepository<Registry>().Count(r => r.Name == "attachmentlinkkey" && r.BoardID == null)
            .Should().Be(1);

        // the cached key is loaded again from the database
        this.board.Cache.Clear();

        AttachmentLinkHelper.Sign(42).Should().Be(signature);
        AttachmentLinkHelper.IsValid(42, signature).Should().BeTrue();
        AttachmentLinkHelper.IsValid(43, signature).Should().BeFalse();
        AttachmentLinkHelper.IsValid(42, null).Should().BeFalse();
    }

    [Test]
    [Description("Without download access nothing is served")]
    public async Task No_Download_Access_Test()
    {
        var attachment = this.AddAttachment(this.alice, this.AddPost(this.publicForum));
        this.board.LoginAs(this.alice);
        this.board.PageLoad.DownloadAccess = false;

        (await this.DownloadAsync(attachment)).Should().BeNull();
    }

    [Test]
    [Description("Unknown attachments and files whose content type does not match the name are not served")]
    public async Task Invalid_Attachments_Test()
    {
        this.board.LoginAs(this.alice);
        var mismatch = this.AddAttachment(this.alice, this.AddPost(this.publicForum), "evil.png", "text/html");

        (await this.DownloadAsync(99999)).Should().BeNull();
        (await this.DownloadAsync(mismatch)).Should().BeNull();
    }

    [Test]
    [Description("Downloading through a file link counts the download, a refused request does not")]
    public async Task Download_Counter_Test()
    {
        var attachment = this.AddAttachment(this.alice, this.AddPost(this.publicForum));
        this.board.LoginAs(this.bob);

        await this.DownloadAsync(attachment, editor: true, signature: string.Empty);
        this.Downloads(attachment).Should().Be(0, "a request without signature is no download");

        this.board.PageLoad.DownloadAccess = false;
        await this.DownloadAsync(attachment, editor: true);
        this.Downloads(attachment).Should().Be(0, "a request without download access is no download");

        this.board.PageLoad.DownloadAccess = true;
        await this.DownloadAsync(attachment, editor: true);
        this.Downloads(attachment).Should().Be(1);
    }

    [Test]
    [Description("The editor list only shows the user's own uploads")]
    public void GetAttachments_Own_Only_Test()
    {
        this.AddAttachment(this.alice, messageId: 0, fileName: "mine.png", contentType: "image/png");
        this.AddAttachment(this.bob, messageId: 0, fileName: "bobs.txt");
        this.board.LoginAs(this.alice);

        var result = (OkObjectResult)this.board.CreateController<AttachmentController>()
            .GetAttachments(new PagedResults { PageNumber = 0, PageSize = 10 });

        var data = (GridDataSet)result.Value;
        data.TotalRecords.Should().Be(1);
        data.AttachmentList.Should().ContainSingle().Which.DataURL.Should().NotBeNull("images get a preview");
    }

    [Test]
    [Description("All guests are the same user: a guest gets no list, it would show the uploads of the other guests")]
    public void GetAttachments_Not_For_Guests_Test()
    {
        this.AddAttachment(this.alice, messageId: 0, fileName: "guest.png", contentType: "image/png");
        this.board.LoginAs(this.alice);
        this.board.User.IsGuest = true;

        var result = (OkObjectResult)this.board.CreateController<AttachmentController>()
            .GetAttachments(new PagedResults { PageNumber = 0, PageSize = 10 });

        var data = (GridDataSet)result.Value;
        data.TotalRecords.Should().Be(0);
        data.AttachmentList.Should().BeEmpty();
    }

    private int AddPost(int forumId)
    {
        return this.db.AddMessage(this.db.AddTopic(forumId, this.alice), this.alice);
    }

    private int AddAttachment(int userId, int messageId, string fileName = "notes.txt", string contentType = "text/plain")
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new Attachment
            {
                MessageID = messageId,
                UserID = userId,
                FileName = fileName,
                ContentType = contentType,
                Bytes = 12,
                FileData = Encoding.UTF8.GetBytes("file content")
            },
            selectIdentity: true);
    }

    private int Downloads(int attachmentId)
    {
        return this.board.Current.GetRepository<Attachment>().GetById(attachmentId).Downloads;
    }

    /// <summary>
    /// Downloads the attachment with the signed link of the forum, or with another signature.
    /// </summary>
    private async Task<string> DownloadAsync(int attachmentId, bool response = false, bool editor = false, string signature = null)
    {
        var controller = this.board.CreateController<Attachments>();

        signature ??= AttachmentLinkHelper.Sign(attachmentId);

        var result = response
            ? await controller.GetResponseAttachment(attachmentId, editor, signature)
            : await controller.GetAttachment(attachmentId, editor, signature);

        if (result is not FileStreamResult file)
        {
            return null;
        }

        using var reader = new StreamReader(file.FileStream);
        return await reader.ReadToEndAsync();
    }

    private static void SetMimeTypes(params (string Extension, string Type)[] types)
    {
        // MimeTypes caches the list in a static field (same as FileAndAlbumControllerTests)
        typeof(MimeTypes).GetField("_mimeTypes", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, types.Length == 0 ? null : types.Select(t => new MimeType { Extension = t.Extension, Type = t.Type }).ToList());
    }
}
