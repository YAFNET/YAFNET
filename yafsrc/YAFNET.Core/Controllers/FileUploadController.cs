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

namespace YAF.Core.Controllers;

using Microsoft.AspNetCore.Hosting;

using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Extensions.Logging;

using Model;

using Types.Models;

using YAF.Types.Objects;

using System.Threading.Tasks;

using SixLabors.ImageSharp;

using YAF.Core.BasePages;

/// <summary>
/// The File Upload controller.
/// </summary>
[Route("api/[controller]")]
[EnableRateLimiting("fixed")]
public class FileUpload : ForumBaseController
{
    /// <summary>
    /// Uploads the files
    /// </summary>
    /// <returns>ActionResult&lt;List&lt;FilesUploadStatus&gt;&gt;.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FilesUploadStatus>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPost("Upload")]
    public async Task<ActionResult<List<FilesUploadStatus>>> Upload([FromForm] IFormFile file)
    {
        var statuses = new List<FilesUploadStatus>();

        if (file == null)
        {
            statuses.Add(
                new FilesUploadStatus
                {
                    error = "No File"
                });

            return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(statuses);
        }

        var yafUserId = this.PageBoardContext.PageUserID;
        var uploadFolder = Path.Combine(
            this.Get<IWebHostEnvironment>().WebRootPath,
            this.Get<BoardFolders>().Uploads);

        if (!this.PageBoardContext.UploadAccess)
        {
            statuses.Add(
                new FilesUploadStatus
                    {
                        error = "Invalid File"
                    });

            return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(statuses);
        }

        try
        {
            var allowedExtensions =
                this.Get<BoardSettings>().AllowedFileExtensions.ToLower().Split(',');

            var fileName = Path.GetFileName(file.FileName);

            var extension = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();

            if (!allowedExtensions.Contains(extension) || !MimeTypes.FileMatchContentType(file))
            {
                statuses.Add(
                    new FilesUploadStatus
                    {
                        error = "Invalid File"
                    });

                return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(statuses);
            }

            if (fileName.IsSet())
            {
                // Check for Illegal Chars
                if (FileHelper.ValidateFileName(fileName))
                {
                    fileName = FileHelper.CleanFileName(fileName);
                }
            }
            else
            {
                statuses.Add(
                    new FilesUploadStatus
                    {
                        error = "File does not have a name"
                    });

                return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(statuses);
            }

            if (fileName.Length > 220)
            {
                fileName = fileName[^220..];
            }

            // verify the size of the attachment
            if (this.PageBoardContext.BoardSettings.MaxFileSize > 0
                && file.Length > this.PageBoardContext.BoardSettings.MaxFileSize)
            {
                statuses.Add(new FilesUploadStatus
                {
                    error = this.Get<ILocalization>().GetTextFormatted(
                                         "UPLOAD_TOOBIG",
                                         file.Length / 1024,
                                         this.Get<BoardSettings>().MaxFileSize / 1024)
                });

                return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(statuses);
            }

            MemoryStream resized = null;

            try
            {
                // resize image ?!
                using var img = await Image.LoadAsync(file.OpenReadStream());

                if (img.Width > this.Get<BoardSettings>().ImageAttachmentResizeWidth
                    || img.Height > this.Get<BoardSettings>().ImageAttachmentResizeHeight)
                {
                    resized = ImageHelper.GetResizedImage(
                        img,
                        this.Get<BoardSettings>().ImageAttachmentResizeWidth,
                        this.Get<BoardSettings>().ImageAttachmentResizeHeight);
                }
            }
            catch (Exception)
            {
                resized = null;
            }

            int newAttachmentId;

            if (this.PageBoardContext.BoardSettings.UseFileTable)
            {
                if (resized is null)
                {
                    using var memoryStream = new MemoryStream();
                    await file.OpenReadStream().CopyToAsync(memoryStream);

                    newAttachmentId = this.GetRepository<Attachment>().Save(
                        yafUserId,
                        fileName,
                        file.Length.ToType<int>(),
                        file.ContentType,
                        memoryStream.ToArray());
                }
                else
                {
                    var extensionOld = Path.GetExtension(fileName);

                    fileName = fileName.Replace(extensionOld, ".webp");

                    newAttachmentId = this.GetRepository<Attachment>().Save(
                        yafUserId,
                        fileName,
                        resized.Length.ToType<int>(),
                        file.ContentType,
                        resized.ToArray());
                }
            }
            else
            {
                // check if Uploads folder exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                if (resized is null)
                {
                    newAttachmentId = this.GetRepository<Attachment>().Save(
                        yafUserId,
                        fileName,
                        file.Length.ToType<int>(),
                        file.ContentType);

                    await using var fileStream = new FileStream(
                        $"{uploadFolder}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload",
                        FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
                else
                {
                    newAttachmentId = this.GetRepository<Attachment>().Save(
                        yafUserId,
                        fileName,
                        resized.Length.ToType<int>(),
                        file.ContentType);

                    await using var fs = new FileStream($"{uploadFolder}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload", FileMode.Create, FileAccess.ReadWrite);
                    var bytes = resized.ToArray();
                    await fs.WriteAsync(bytes);
                }
            }

            var fullName = Path.GetFileName(fileName);
            statuses.Add(new FilesUploadStatus(fullName, file.Length.ToType<int>(), newAttachmentId));
        }
        catch (Exception ex)
        {
            statuses.Add(new FilesUploadStatus
                             {
                                 error = ex.Message
                             });

            this.Get<ILogger<FileUpload>>().Error(ex, "Error during Attachment upload");
        }

        return await Task.FromResult<ActionResult<List<FilesUploadStatus>>>(this.Ok(statuses));
    }
}