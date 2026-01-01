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

using System.Threading.Tasks;

namespace YAF.Core.Controllers;

using Microsoft.AspNetCore.Hosting;

using System;
using System.IO;

using Microsoft.Extensions.Logging;

using Model;

using Types.Models;

using YAF.Core.BasePages;

/// <summary>
/// The Attachments controller.
/// </summary>
[Route("api/[controller]")]
public class Attachments : ForumBaseController
{
    /// <summary>
    /// Gets the attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <param name="editor">if set to <c>true</c> [editor].</param>
    /// <returns>ActionResult.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetAttachment")]
    public async Task<ActionResult> GetAttachment(int attachmentId, bool editor = false)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return this.NotFound();
            }

            // AttachmentID
            var attachment = await this.GetRepository<Attachment>().GetByIdAsync(attachmentId);

            if (editor)
            {
                // add a download count...
                await this.GetRepository<Attachment>().IncrementDownloadCounterAsync(attachment.ID);
            }

            if (!MimeTypes.FileMatchContentType(attachment.FileName, attachment.ContentType))
            {
                // Illegal File
                return this.NotFound();
            }

            // check download permissions here
            if (!BoardContext.Current.DownloadAccess)
            {
                // tear it down
                // no permission to download
                return this.NotFound();
            }

            if (attachment.FileData == null)
            {
                var uploadFolder = Path.Combine(
                    this.Get<IWebHostEnvironment>().WebRootPath,
                    this.Get<BoardFolders>().Uploads);

                var oldFileName = Path.Combine(uploadFolder,
                    $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                var newFileName = Path.Combine(uploadFolder,
                    $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                string fileName;

                if (System.IO.File.Exists(oldFileName))
                {
                    fileName = oldFileName;
                }
                else
                {
                    oldFileName =
                        Path.Combine(uploadFolder,
                            $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload");

                    // use the new fileName (with extension) if it exists...
                    fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;
                }

                // output stream...
                var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(stream, attachment.ContentType, attachment.FileName);
            }
            else
            {
                var stream = new MemoryStream(attachment.FileData);

                return File(stream, attachment.ContentType, attachment.FileName);
            }
        }
        catch (Exception x)
        {
            this.Get<ILogger<Attachments>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }

    /// <summary>
    /// Gets the response attachment.
    /// </summary>
    /// <param name="attachmentId">The attachment identifier.</param>
    /// <param name="editor">if set to <c>true</c> [editor].</param>
    /// <returns>ActionResult.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ActionResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetResponseAttachment")]
    public async Task<ActionResult> GetResponseAttachment(int attachmentId, bool editor = false)
    {
        try
        {
            if (!this.ModelState.IsValid)
            {
                return this.NotFound();
            }

            // AttachmentID
            var attachment = await this.GetRepository<Attachment>().GetByIdAsync(attachmentId);

            if (editor)
            {
                // add a download count...
                await this.GetRepository<Attachment>().IncrementDownloadCounterAsync(attachment.ID);
            }

            if (!MimeTypes.FileMatchContentType(attachment.FileName, attachment.ContentType))
            {
                // Illegal File
                return this.NotFound();
            }

            // check download permissions here
            if (!this.PageBoardContext.DownloadAccess)
            {
                // tear it down
                // no permission to download
                return this.NotFound();
            }

            if (attachment.FileData == null)
            {
                var uploadFolder = Path.Combine(
                    this.Get<IWebHostEnvironment>().WebRootPath,
                    this.Get<BoardFolders>().Uploads);

                var oldFileName = Path.Combine(uploadFolder,
                    $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                var newFileName = Path.Combine(uploadFolder,
                    $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                string fileName;

                if (System.IO.File.Exists(oldFileName))
                {
                    fileName = oldFileName;
                }
                else
                {
                    oldFileName = Path.Combine(uploadFolder,
                        $"{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload");

                    // use the new fileName (with extension) if it exists...
                    fileName = System.IO.File.Exists(newFileName) ? newFileName : oldFileName;
                }

                // output stream...
                var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                return File(stream, attachment.ContentType, attachment.FileName);
            }
            else
            {
                var stream = new MemoryStream(attachment.FileData);

                return File(stream, attachment.ContentType, attachment.FileName);
            }
        }
        catch (Exception x)
        {
            this.Get<ILogger<Attachments>>().Log(this.PageBoardContext.PageUserID, this, x, EventLogTypes.Information);

            return this.NotFound();
        }
    }
}