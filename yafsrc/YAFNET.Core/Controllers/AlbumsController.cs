/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using System.Threading.Tasks;

namespace YAF.Core.Controllers;

using Microsoft.AspNetCore.Hosting;

using System;
using System.IO;
using System.Security.Cryptography;

using Microsoft.Extensions.Logging;

using Model;

using Types.Models;

using YAF.Core.BasePages;

/// <summary>
/// The Albums controller.
/// </summary>
[Route("api/[controller]")]
public class Albums : ForumBaseController
{
    /// <summary>
    /// Gets the album image.
    /// </summary>
    /// <param name="imageId">The image identifier.</param>
    /// <returns>The album image</returns>
    [Produces("image/png")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetImage")]
    public async Task<ActionResult> GetImage(int imageId)
    {
        try
        {
            // ImageID
            var image = await this.GetRepository<UserAlbumImage>()
                .GetImageAsync(imageId);

            var uploadFolder = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

            var oldFileName =
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}";
            var newFileName =
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum";

            // use the new fileName (with extension) if it exists...
            var fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;

            if (image.Item1.ContentType.Contains("text"))
            {
                return this.NotFound();
            }

            // add a download count...
            await this.GetRepository<UserAlbumImage>().IncrementDownloadAsync(
                imageId);

            // output stream...
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = new FileStreamResult(stream, "image/png");

            return result;
        }
        catch (Exception x)
        {
            this.Get<ILogger<Albums>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }

    /// <summary>
    /// Gets the album image preview.
    /// </summary>
    /// <param name="imageId">The image identifier.</param>
    /// <returns>The album image preview</returns>
    [Produces("image/png")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetImagePreview")]
    public async Task<ActionResult> GetImagePreview(int imageId)
    {
        try
        {
            // ImageID
            var image = await this.GetRepository<UserAlbumImage>()
                .GetImageAsync(imageId);

            var uploadFolder = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

            var oldFileName = $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}";
            var newFileName =
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum";

            // use the new fileName (with extension) if it exists...
            var fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;

            // output stream...
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = new FileStreamResult(stream, "image/png");

            return result;
        }
        catch (Exception x)
        {
            this.Get<ILogger<Albums>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }

    /// <summary>
    /// Gets the album cover.
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    /// <param name="coverId">The cover identifier.</param>
    /// <returns>The Album Cover Image</returns>
    [Produces("image/png")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileStreamResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetAlbumCover")]
    public async Task<ActionResult> GetAlbumCover(int albumId, int coverId)
    {
        try
        {
            // Check if user has access
            if (!this.Get<IPermissions>().Check(this.Get<BoardSettings>().ProfileViewPermissions))
            {
                return this.NotFound();
            }

            // CoverID
            var fileName = string.Empty;
            if (coverId == 0)
            {
                var album = this.GetRepository<UserAlbumImage>().List(albumId);

                if (!album.NullOrEmpty())
                {
                    var image = album.Count > 1
                                    ? await this.GetRepository<UserAlbumImage>()
                                        .GetImageAsync(album[RandomNumberGenerator.GetInt32(1, album.Count)].ID)
                                    : await this.GetRepository<UserAlbumImage>()
                                        .GetImageAsync(album[0].ID);

                    var uploadFolder = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

                    var oldFileName =
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}";
                    var newFileName =
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum";

                    // use the new fileName (with extension) if it exists...
                    fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;
                }
            }
            else
            {
                var image = await this.GetRepository<UserAlbumImage>()
                    .GetImageAsync(coverId);

                if (image != null)
                {
                    var uploadFolder = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

                    var oldFileName =
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}";
                    var newFileName =
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum";

                    // use the new fileName (with extension) if it exists...
                    fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;
                }
            }

            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            var result = new FileStreamResult(stream, "image/png");

            return result;
        }
        catch (Exception x)
        {
            this.Get<ILogger<Albums>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }
}