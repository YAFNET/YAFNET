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

namespace YAF.Tests.CoreTests.Services;

using System.IO;
using System.Threading.Tasks;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

using NSubstitute;

using ServiceStack.OrmLite;

using YAF.Core.Controllers;
using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Tests.CoreTests.TestInfrastructure;
using YAF.Types.Constants;
using YAF.Types.Interfaces;
using YAF.Types.Interfaces.Services;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The <see cref="Album"/> service (deleting albums and images) and the album image endpoints.
/// </summary>
[TestFixture]
[NonParallelizable]
public class AlbumServiceTests
{
    private TestBoardContext board;

    private TestDatabase db;

    private IPermissions permissions;

    private string uploads;

    private IAlbum album;

    private int alice;

    private int bob;

    [SetUp]
    public void SetUp()
    {
        var webRoot = Path.Combine(Path.GetTempPath(), $"yaf-album-{Guid.NewGuid():N}");
        this.uploads = Path.Combine(webRoot, "uploads");
        Directory.CreateDirectory(this.uploads);

        var environment = Substitute.For<IWebHostEnvironment>();
        environment.WebRootPath.Returns(webRoot);

        this.permissions = Substitute.For<IPermissions>();
        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(true);

        this.board = new TestBoardContext(
            b =>
            {
                b.RegisterType<Album>().As<IAlbum>();
                b.RegisterInstance(environment).As<IWebHostEnvironment>();
                b.RegisterType<BoardFolders>().AsSelf();
                b.RegisterInstance(new BoardConfiguration());
                b.RegisterInstance(this.permissions).As<IPermissions>();
            },
            useDatabase: true);

        this.db = this.board.Database;
        this.alice = this.db.AddUser("Alice");
        this.bob = this.db.AddUser("Bob");
        this.album = this.board.Resolve<IAlbum>();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
        Directory.Delete(Path.GetDirectoryName(this.uploads)!, true);
    }

    [Test]
    [Description("The owner deletes an album with all images and files")]
    public async Task Delete_Album_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        var first = this.AddImage(albumId, "a.png", withFile: true, userId: this.alice);
        this.AddImage(albumId, "b.png", withFile: true, userId: this.alice);

        await this.album.AlbumDeleteAsync(this.uploads, albumId, this.alice);

        this.Albums().Should().BeEmpty();
        this.Images().Should().BeEmpty();
        File.Exists(this.FilePath(this.alice, albumId, "a.png")).Should().BeFalse();
        this.Images().Should().NotContain(i => i.ID == first);
    }

    [Test]
    [Description("An album is deleted completely also when image files are missing (e.g. stored in the database)")]
    public async Task Delete_Album_Missing_Files_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        this.AddImage(albumId, "a.png");
        this.AddImage(albumId, "b.png");
        this.AddImage(albumId, "c.png");

        await this.album.AlbumDeleteAsync(this.uploads, albumId, this.alice);

        this.Images().Should().BeEmpty();
        this.Albums().Should().BeEmpty();
    }

    [Test]
    [Description("Users can not delete the albums of other users")]
    public async Task Delete_Album_Not_Owner_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        this.AddImage(albumId, "a.png");

        await this.album.AlbumDeleteAsync(this.uploads, albumId, this.bob);

        this.Albums().Should().ContainSingle();
        this.Images().Should().ContainSingle();
    }

    [Test]
    [Description("The owner deletes an image, its file and the album cover pointing to it")]
    public async Task Delete_Image_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        var image = this.AddImage(albumId, "a.png", withFile: true, userId: this.alice);
        var kept = this.AddImage(albumId, "b.png");
        this.board.Current.GetRepository<UserAlbum>().UpdateOnly(() => new UserAlbum { CoverImageID = image }, a => a.ID == albumId);

        await this.album.AlbumImageDeleteAsync(this.uploads, image, this.alice);

        this.Images().Select(i => i.ID).Should().Equal(kept);
        File.Exists(this.FilePath(this.alice, albumId, "a.png")).Should().BeFalse();
        this.Albums().Single().CoverImageID.Should().BeNull();
    }

    [Test]
    [Description("Users can not delete the images of other users")]
    public async Task Delete_Image_Not_Owner_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        var image = this.AddImage(albumId, "a.png");

        await this.album.AlbumImageDeleteAsync(this.uploads, image, this.bob);

        this.Images().Should().ContainSingle();
    }

    [Test]
    [Description("Deleting an unknown image does nothing")]
    public async Task Delete_Unknown_Image_Test()
    {
        await this.album.Invoking(a => a.AlbumImageDeleteAsync(this.uploads, 99999, this.alice)).Should().NotThrowAsync();
    }

    [Test]
    [Description("Album images are served to users who may view profiles, not to others")]
    public async Task Image_Permission_Test()
    {
        var albumId = this.AddAlbum(this.alice);
        var image = this.AddImage(albumId, "a.png", withFile: true, userId: this.alice);

        await using ((await this.board.CreateController<Albums>().GetImage(image)).Should().BeOfType<FileStreamResult>().Subject.FileStream)
        {
        }

        await using ((await this.board.CreateController<Albums>().GetImagePreview(image)).Should().BeOfType<FileStreamResult>().Subject.FileStream)
        {
        }

        this.permissions.Check(Arg.Any<ViewPermissions>()).Returns(false);

        var denied = await this.board.CreateController<Albums>().GetImage(image);
        (denied as FileStreamResult)?.FileStream.Dispose();
        denied.Should().BeOfType<NotFoundResult>();

        var deniedPreview = await this.board.CreateController<Albums>().GetImagePreview(image);
        (deniedPreview as FileStreamResult)?.FileStream.Dispose();
        deniedPreview.Should().BeOfType<NotFoundResult>();
    }

    private int AddAlbum(int userId)
    {
        using var connection = this.db.Open();

        return (int)connection.Insert(
            new UserAlbum { UserID = userId, Title = "Holiday", Updated = DateTime.UtcNow },
            selectIdentity: true);
    }

    private int AddImage(int albumId, string fileName, bool withFile = false, int userId = 0)
    {
        if (withFile)
        {
            File.WriteAllBytes(this.FilePath(userId, albumId, fileName), [1, 2, 3]);
        }

        using var connection = this.db.Open();

        return (int)connection.Insert(
            new UserAlbumImage
            {
                AlbumID = albumId,
                FileName = fileName,
                ContentType = "image/png",
                Bytes = 3,
                Uploaded = DateTime.UtcNow
            },
            selectIdentity: true);
    }

    private string FilePath(int userId, int albumId, string fileName)
    {
        return Path.Combine(this.uploads, $"{userId}.{albumId}.{fileName}.yafalbum");
    }

    private List<UserAlbum> Albums()
    {
        return this.board.Current.GetRepository<UserAlbum>().Get(_ => true);
    }

    private List<UserAlbumImage> Images()
    {
        return this.board.Current.GetRepository<UserAlbumImage>().Get(_ => true);
    }
}
