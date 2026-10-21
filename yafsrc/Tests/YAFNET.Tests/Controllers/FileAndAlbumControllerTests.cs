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
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using SkiaSharp;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Core.Utilities;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Flags;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Data;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// File upload, attachment download, album images and the moderator topic lookup.
/// </summary>
[TestFixture]
[NonParallelizable]
public class FileAndAlbumControllerTests
{
    private string webRoot;

    private TestBoardContext board;

    private TestDatabase db;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        this.webRoot = Path.Combine(Path.GetTempPath(), $"yaf-files-{Guid.NewGuid():N}");
        Directory.CreateDirectory(this.webRoot);

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(this.webRoot);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterType<Album>().As<IAlbum>();

                var permissions = Substitute.For<IPermissions>();
                permissions.Check(Arg.Any<YAF.Types.Constants.ViewPermissions>()).Returns(true);
                b.RegisterInstance(permissions).As<IPermissions>();
            },
            useDatabase: true);

        this.db = this.board.Database;

        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.board.LoginAs(this.alice);

        // MimeTypes caches the list in a static field, set it for these tests
        SetMimeTypes(("png", "image/png"), ("webp", "image/webp"), ("jpg", "image/jpeg"), ("txt", "text/plain"));

        this.board.BoardSettings.AllowedFileExtensions = "png,jpg,webp";
        this.board.BoardSettings.ImageAttachmentResizeWidth = 1000;
        this.board.BoardSettings.ImageAttachmentResizeHeight = 1000;
        this.board.PageLoad.UploadAccess = true;
        this.board.PageLoad.DownloadAccess = true;
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        SetMimeTypes();

        try
        {
            Directory.Delete(this.webRoot, true);
        }
        catch (IOException)
        {
            // ignore
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    [Description("A resized image is stored as webp with the matching name, content type and size, and can be downloaded")]
    public async Task Upload_Resized_Image_Test(bool useFileTable)
    {
        this.board.BoardSettings.UseFileTable = useFileTable;
        this.board.BoardSettings.ImageAttachmentResizeWidth = 20;
        this.board.BoardSettings.ImageAttachmentResizeHeight = 20;

        var status = await this.UploadAsync(Png(200, 100), "my.photo.png", "image/png");

        status.error.Should().BeNull();

        var attachment = await this.Repository<Attachment>().GetByIdAsync(status.fileID);
        attachment.FileName.Should().Be("my.photo.webp");
        attachment.ContentType.Should().Be("image/webp");

        var stored = await this.DownloadAsync(attachment.ID);
        stored.Should().NotBeNull("the download check must accept the stored name and content type");
        attachment.Bytes.Should().Be(stored.Length).And.Be(status.size);

        using var decoded = SKBitmap.Decode(stored);
        decoded.Width.Should().Be(20, "the image was resized");
    }

    [Test]
    [Description("Small images are stored unchanged")]
    public async Task Upload_Not_Resized_Test()
    {
        this.board.BoardSettings.UseFileTable = true;
        var png = Png(10, 10);

        var status = await this.UploadAsync(png, "small.png", "image/png");

        var attachment = await this.Repository<Attachment>().GetByIdAsync(status.fileID);
        attachment.FileName.Should().Be("small.png");
        attachment.ContentType.Should().Be("image/png");
        attachment.Bytes.Should().Be(png.Length);
        (await this.DownloadAsync(attachment.ID)).Should().Equal(png);
    }

    [Test]
    [Description("The allowed extensions can be written with spaces and dots (\"png, .jpg\"), empty entries allow nothing")]
    public async Task Upload_Allowed_Extensions_With_Spaces_Test()
    {
        this.board.BoardSettings.UseFileTable = true;
        this.board.BoardSettings.AllowedFileExtensions = "txt, .png, ,";

        (await this.UploadAsync(Png(10, 10), "photo.png", "image/png")).error.Should().BeNull();
        (await this.UploadAsync([1, 2, 3], "photo", "image/png")).error.Should().Be("Invalid File");
        (await this.UploadAsync(Png(10, 10), "photo.jpg", "image/jpeg")).error.Should().Be("Invalid File");

        (await this.Repository<Attachment>().CountAsync(a => true)).Should().Be(1);
    }

    [Test]
    [Description("Uploads need upload access, an allowed extension and a matching content type")]
    public async Task Upload_Rejected_Test()
    {
        this.board.BoardSettings.UseFileTable = true;

        (await this.UploadAsync(Png(10, 10), "photo.png", "text/plain")).error.Should().Be("Invalid File");
        (await this.UploadAsync([1, 2, 3], "notes.txt", "text/plain")).error.Should().Be("Invalid File");

        this.board.PageLoad.UploadAccess = false;
        (await this.UploadAsync(Png(10, 10), "photo.png", "image/png")).error.Should().Be("Invalid File");

        (await this.Repository<Attachment>().CountAsync(a => true)).Should().Be(0);
    }

    [Test]
    [Description("A request without a file is answered with an error")]
    public async Task Upload_No_File_Test()
    {
        var result = await this.board.CreateController<FileUpload>().Upload(null);

        result.Value.Single().error.Should().Be("No File");
    }

    [Test]
    [Description("Illegal characters are removed from the name, long names are shortened keeping the extension")]
    public async Task Upload_File_Name_Test()
    {
        this.board.BoardSettings.UseFileTable = true;

        var cleaned = await this.UploadAsync(Png(10, 10), "my\0photo.png", "image/png");
        (await this.Repository<Attachment>().GetByIdAsync(cleaned.fileID)).FileName.Should().Be("myphoto.png");

        var longName = $"{new string('a', 300)}.png";
        var shortened = await this.UploadAsync(Png(10, 10), longName, "image/png");
        var stored = (await this.Repository<Attachment>().GetByIdAsync(shortened.fileID)).FileName;

        stored.Should().HaveLength(220).And.EndWith(".png");
    }

    [Test]
    [Description("Files larger than the maximum file size are rejected")]
    public async Task Upload_Too_Big_Test()
    {
        this.board.BoardSettings.UseFileTable = true;
        this.board.BoardSettings.MaxFileSize = 10;

        var status = await this.UploadAsync(Png(10, 10), "photo.png", "image/png");

        status.error.Should().Contain("UPLOAD_TOOBIG");
        (await this.Repository<Attachment>().CountAsync(_ => true)).Should().Be(0);
    }

    [Test]
    [Description("Without the file table the upload is written to the uploads folder of the user")]
    public async Task Upload_To_Folder_Test()
    {
        this.board.BoardSettings.UseFileTable = false;
        var png = Png(10, 10);

        var status = await this.UploadAsync(png, "folder.png", "image/png");

        var path = Path.Combine(this.webRoot, "uploads", $"u{this.alice}-{status.fileID}.folder.png.yafupload");
        (await File.ReadAllBytesAsync(path)).Should().Equal(png);
    }

    [Test]
    [Description("An error while storing the upload is returned to the upload script (html encoded), nothing is saved")]
    public async Task Upload_Storage_Error_Test()
    {
        this.board.BoardSettings.UseFileTable = false;

        // the uploads folder can not be created
        await File.WriteAllTextAsync(Path.Combine(this.webRoot, "uploads"), "<not a folder>");

        var status = await this.UploadAsync(Png(10, 10), "folder.png", "image/png");

        status.error.Should().NotBeNullOrEmpty();
        status.fileID.Should().Be(0);
        (await this.Repository<Attachment>().CountAsync(a => a.UserID == this.alice)).Should().Be(0);
    }

    [Test]
    [Description("Attachments stored in the uploads folder are found under the current and the legacy file names")]
    [TestCase(0, "u{user}-{id}.{name}.yafupload")]
    [TestCase(0, "u{user}.{name}.yafupload")]
    [TestCase(0, "u{user}.{name}")]
    [TestCase(77, "{message}.{name}.yafupload")]
    [TestCase(77, "{message}.{name}")]
    public async Task Download_File_Names_Test(int messageId, string pattern)
    {
        var id = (int)await this.Repository<Attachment>().InsertAsync(
            new Attachment { UserID = this.alice, MessageID = messageId, FileName = "pic.png", ContentType = "image/png", Bytes = 3 },
            true);

        var uploads = Path.Combine(this.webRoot, "uploads");
        Directory.CreateDirectory(uploads);

        var name = pattern.Replace("{user}", this.alice.ToString()).Replace("{id}", id.ToString())
            .Replace("{message}", messageId.ToString()).Replace("{name}", "pic.png");
        await File.WriteAllBytesAsync(Path.Combine(uploads, name), [1, 2, 3]);

        (await this.DownloadAsync(id)).Should().Equal(1, 2, 3);

        var response = await this.board.CreateController<Attachments>().GetResponseAttachment(id, editor: true, sig: AttachmentLinkHelper.Sign(id));
        var file = response.Should().BeOfType<FileStreamResult>().Subject;
        await file.FileStream.DisposeAsync();

        file.ContentType.Should().Be("image/png");
        file.FileDownloadName.Should().Be("pic.png");
        (await this.Repository<Attachment>().GetByIdAsync(id)).Downloads.Should().Be(1, "only the editor download is counted");
    }

    [Test]
    [Description("Unknown attachments and attachments without a file are not found")]
    public async Task Download_Missing_Test()
    {
        (await this.board.CreateController<Attachments>().GetAttachment(9999, sig: AttachmentLinkHelper.Sign(9999))).Should().BeOfType<NotFoundResult>();

        var id = (int)await this.Repository<Attachment>().InsertAsync(
            new Attachment { UserID = this.alice, FileName = "gone.png", ContentType = "image/png", Bytes = 3 },
            true);

        (await this.board.CreateController<Attachments>().GetResponseAttachment(id, sig: AttachmentLinkHelper.Sign(id))).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Album images are served with their raster image type, other types (e.g. svg) as png")]
    [TestCase("image/jpeg", "image/jpeg")]
    [TestCase("IMAGE/WEBP", "image/webp")]
    [TestCase("image/gif", "image/gif")]
    [TestCase("image/svg+xml", "image/png")]
    [TestCase(null, "image/png")]
    public async Task Album_Image_Content_Type_Test(string stored, string served)
    {
        var album = this.AddAlbum(this.alice);
        var image = this.AddAlbumImage(album, "picture.img", stored);
        var albums = this.board.CreateController<Albums>();

        foreach (var result in new[]
                 {
                     await albums.GetImage(image), await albums.GetImagePreview(image), await albums.GetAlbumCover(album, image)
                 })
        {
            var file = result.Should().BeOfType<FileStreamResult>().Subject;
            file.ContentType.Should().Be(served);
            await file.FileStream.DisposeAsync();
        }
    }

    [Test]
    [Description("Only the full image view is counted, the cover is picked from the album, an empty album has none")]
    public async Task Album_Downloads_And_Cover_Test()
    {
        var album = this.AddAlbum(this.alice);
        var first = this.AddAlbumImage(album, "one.png", "image/png");
        var second = this.AddAlbumImage(album, "two.png", "image/png");
        var albums = this.board.CreateController<Albums>();

        await ((FileStreamResult)await albums.GetImage(first)).FileStream.DisposeAsync();
        await ((FileStreamResult)await albums.GetImagePreview(first)).FileStream.DisposeAsync();

        (await this.Repository<UserAlbumImage>().GetByIdAsync(first)).Downloads.Should().Be(1);

        // without a cover one of the album images is used
        for (var i = 0; i < 3; i++)
        {
            await ((FileStreamResult)await albums.GetAlbumCover(album, 0)).FileStream.DisposeAsync();
        }

        (await this.Repository<UserAlbumImage>().GetByIdAsync(second)).Downloads.Should().Be(0, "covers are not counted");

        (await albums.GetAlbumCover(this.AddAlbum(this.alice), 0)).Should().BeOfType<NotFoundResult>();
        (await albums.GetAlbumCover(album, 9999)).Should().BeOfType<NotFoundResult>();
        (await albums.GetImage(9999)).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Album images stored under the old file name (without .yafalbum) are still served")]
    public async Task Album_Old_File_Name_Test()
    {
        var album = this.AddAlbum(this.alice);
        var image = this.AddAlbumImage(album, "old.png", "image/png");

        var uploads = Path.Combine(this.webRoot, this.board.Resolve<BoardFolders>().Uploads);
        File.Move(
            Path.Combine(uploads, $"{this.alice}.{album}.old.png.yafalbum"),
            Path.Combine(uploads, $"{this.alice}.{album}.old.png"));

        var result = (FileStreamResult)await this.board.CreateController<Albums>().GetImagePreview(image);
        await using var stream = result.FileStream;
        stream.Length.Should().Be(3);
    }

    [Test]
    [Description("Album images with a text content type are never served, also not as preview or cover")]
    public async Task Album_Text_Content_Type_Test()
    {
        var album = this.AddAlbum(this.alice);
        var image = this.AddAlbumImage(album, "page.png", "text/html");
        var cover = this.AddAlbumImage(album, "cover.png", "image/png");

        var albums = this.board.CreateController<Albums>();

        (await albums.GetImage(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetImagePreview(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetAlbumCover(album, image)).Should().BeOfType<NotFoundResult>();

        (await albums.GetImagePreview(cover)).Should().BeOfType<FileStreamResult>();
        (await albums.GetAlbumCover(album, cover)).Should().BeOfType<FileStreamResult>();
    }

    [Test]
    [Description("An album image without its file is not found")]
    public async Task Album_Image_Missing_File_Test()
    {
        var album = this.AddAlbum(this.alice);
        var image = this.AddAlbumImage(album, "gone.png", "image/png");

        File.Delete(
            Path.Combine(this.webRoot, this.board.Resolve<BoardFolders>().Uploads, $"{this.alice}.{album}.gone.png.yafalbum"));

        var albums = this.board.CreateController<Albums>();

        (await albums.GetImage(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetImagePreview(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetAlbumCover(album, image)).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Album images need the permission to view profiles and a valid request")]
    public async Task Album_Image_Permission_Test()
    {
        var album = this.AddAlbum(this.alice);
        var image = this.AddAlbumImage(album, "pic.png", "image/png");

        var albums = this.board.CreateController<Albums>();
        albums.ModelState.AddModelError("imageId", "invalid");

        (await albums.GetImage(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetImagePreview(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetAlbumCover(album, image)).Should().BeOfType<NotFoundResult>();

        this.board.Resolve<IPermissions>().Check(Arg.Any<YAF.Types.Constants.ViewPermissions>()).Returns(false);

        albums = this.board.CreateController<Albums>();

        (await albums.GetImage(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetImagePreview(image)).Should().BeOfType<NotFoundResult>();
        (await albums.GetAlbumCover(album, image)).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Renaming albums and captions only works for own albums and ignores unknown ids")]
    public void Album_Rename_Test()
    {
        var own = this.AddAlbum(this.alice);
        var foreign = this.AddAlbum(this.bob);
        var ownImage = this.AddAlbumImage(own, "a.png", "image/png");
        var foreignImage = this.AddAlbumImage(foreign, "b.png", "image/png");

        var controller = this.board.CreateController<AlbumController>();

        controller.ChangeAlbumTitle(own, "Holidays").Should().BeOfType<OkResult>();
        controller.ChangeAlbumTitle(foreign, "Hacked").Should().BeOfType<OkResult>();
        controller.ChangeAlbumTitle(int.MaxValue, "Unknown").Should().BeOfType<OkResult>();

        controller.ChangeImageCaption(ownImage, "Beach").Should().BeOfType<OkResult>();
        controller.ChangeImageCaption(foreignImage, "Hacked").Should().BeOfType<OkResult>();
        controller.ChangeImageCaption(int.MaxValue, "Unknown").Should().BeOfType<OkResult>();

        this.Repository<UserAlbum>().GetById(own).Title.Should().Be("Holidays");
        this.Repository<UserAlbum>().GetById(foreign).Title.Should().Be("Album");
        this.Repository<UserAlbumImage>().GetById(ownImage).Caption.Should().Be("Beach");
        this.Repository<UserAlbumImage>().GetById(foreignImage).Caption.Should().BeNull();
    }

    [Test]
    [Description("The topic lookup only lists topics of forums the user is admin or moderator of")]
    public void GetTopics_Moderator_Only_Test()
    {
        var moderated = this.db.AddForum("Moderated");
        var readOnly = this.db.AddForum("Read only");
        this.AddTopicWithPost(moderated, "Moderated topic");
        this.AddTopicWithPost(readOnly, "Staff topic");

        this.board.GrantAccess(this.alice, moderated, TestDatabase.MemberAccess | AccessFlags.Flags.ModeratorAccess);
        this.board.GrantAccess(this.alice, readOnly);

        // the page is a forum Alice moderates, but the request asks for another one
        this.board.SetPage(moderated);
        this.board.PageLoad.ModeratorAccess = true;

        Titles(this.GetTopics(moderated, "topic")).Should().Equal("Moderated topic");
        Titles(this.GetTopics(moderated, null)).Should().Equal("Moderated topic");

        this.GetTopics(readOnly, "topic").Should().BeOfType<NotFoundResult>();
        this.GetTopics(readOnly, null).Should().BeOfType<NotFoundResult>();

        // admins see every forum of their board
        this.board.PageLoad.IsAdmin = true;
        Titles(this.GetTopics(readOnly, null)).Should().Equal("Staff topic");

        // but not the forums of another board
        var otherBoardForum = this.db.AddForum(
            "Other board",
            this.db.AddCategory("Other board", c => c.BoardID = TestDatabase.BoardId + 1));
        this.AddTopicWithPost(otherBoardForum, "Other board topic");

        this.GetTopics(otherBoardForum, "topic").Should().BeOfType<NotFoundResult>();
        this.GetTopics(otherBoardForum, null).Should().BeOfType<NotFoundResult>();
    }

    [Test]
    [Description("Deleted topics and the links of moved topics are no targets for moving a message")]
    public void GetTopics_No_Deleted_Or_Moved_Topics_Test()
    {
        var forum = this.db.AddForum("Moderated");
        this.AddTopicWithPost(forum, "Open topic");
        this.AddTopicWithPost(forum, "Deleted topic");
        this.AddTopicWithPost(forum, "Moved topic");

        var deleted = this.Repository<Topic>().GetSingle(t => t.TopicName == "Deleted topic");
        this.Repository<Topic>().UpdateOnly(() => new Topic { Flags = deleted.Flags | (int)TopicFlags.Flags.IsDeleted }, t => t.ID == deleted.ID);
        this.Repository<Topic>().UpdateOnly(() => new Topic { TopicMovedID = deleted.ID }, t => t.TopicName == "Moved topic");

        this.board.PageLoad.IsAdmin = true;

        Titles(this.GetTopics(forum, "topic")).Should().Equal("Open topic");
        Titles(this.GetTopics(forum, null)).Should().NotContain("Moved topic").And.NotContain("Deleted topic");
    }

    [Test]
    [Description("The search skips the current topic without removing it from the cached topic list")]
    public void GetTopics_Search_Keeps_Cached_List_Test()
    {
        var forum = this.db.AddForum("Moderated");
        var first = this.db.AddTopic(forum, this.bob, "First topic");
        this.db.AddTopic(forum, this.bob, "Second topic");

        this.board.PageLoad.IsAdmin = true;

        Titles(this.board.CreateController<TopicController>().GetTopics(
            new SearchTopic { ForumId = forum, TopicId = first, SearchTerm = "topic", Page = 0 })).Should().Equal("Second topic");

        Titles(this.GetTopics(forum, "topic")).Should().BeEquivalentTo("First topic", "Second topic");
    }

    private IRepository<T> Repository<T>()
        where T : IEntity
    {
        return this.board.Current.GetRepository<T>();
    }

    private static void SetMimeTypes(params (string Extension, string Type)[] types)
    {
        typeof(MimeTypes).GetField("_mimeTypes", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, types.Length == 0 ? null : types.Select(t => new MimeType { Extension = t.Extension, Type = t.Type }).ToList());
    }

    private static byte[] Png(int width, int height)
    {
        using var bitmap = new SKBitmap(width, height);
        bitmap.Erase(SKColors.CornflowerBlue);

        using var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }

    private async Task<FilesUploadStatus> UploadAsync(byte[] content, string fileName, string contentType)
    {
        await using var stream = new MemoryStream(content);

        var file = new FormFile(stream, 0, content.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(), ContentType = contentType
        };

        var result = await this.board.CreateController<FileUpload>().Upload(file);

        var statuses = (List<FilesUploadStatus>)((result.Result as OkObjectResult)?.Value ?? result.Value);

        return statuses.Single();
    }

    private async Task<byte[]> DownloadAsync(int attachmentId)
    {
        var result = await this.board.CreateController<Attachments>().GetAttachment(attachmentId, sig: AttachmentLinkHelper.Sign(attachmentId));

        if (result is not FileStreamResult file)
        {
            return null;
        }

        await using var stream = file.FileStream;
        using var copy = new MemoryStream();
        await stream.CopyToAsync(copy);

        return copy.ToArray();
    }

    private int AddAlbum(int userId)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(new UserAlbum { UserID = userId, Title = "Album", Updated = DateTime.UtcNow }, selectIdentity: true);
    }

    private int AddAlbumImage(int albumId, string fileName, string contentType)
    {
        using var connection = this.db.Open();

        var id = (int)connection.Insert(
            new UserAlbumImage { AlbumID = albumId, FileName = fileName, ContentType = contentType, Bytes = 3, Uploaded = DateTime.UtcNow },
            selectIdentity: true);

        var userId = connection.SingleById<UserAlbum>(albumId).UserID;
        var uploads = Path.Combine(this.webRoot, this.board.Resolve<BoardFolders>().Uploads);
        Directory.CreateDirectory(uploads);
        File.WriteAllBytes(Path.Combine(uploads, $"{userId}.{albumId}.{fileName}.yafalbum"), [1, 2, 3]);

        return id;
    }

    private void AddTopicWithPost(int forumId, string subject)
    {
        var topic = this.db.AddTopic(forumId, this.bob, subject);
        this.db.AddMessage(topic, this.bob);
    }

    private IActionResult GetTopics(int forumId, string searchTerm)
    {
        return this.board.CreateController<TopicController>()
            .GetTopics(new SearchTopic { ForumId = forumId, SearchTerm = searchTerm, Page = 0 });
    }

    private static IEnumerable<string> Titles(IActionResult result)
    {
        var options = result.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeOfType<SelectPagedOptions>().Subject;

        return options.Results.Select(r => r.text);
    }
}
