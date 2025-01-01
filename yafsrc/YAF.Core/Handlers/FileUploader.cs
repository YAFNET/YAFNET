﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Handlers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web.Script.Serialization;
using System.Web.SessionState;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

using MimeTypes = Utilities.MimeTypes;

/// <summary>
/// The File Upload Handler
/// </summary>
public class FileUploader : IHttpHandler, IReadOnlySessionState, IHaveServiceLocator
{
    /// <summary>
    /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
    /// </summary>
    public bool IsReusable => false;

    /// <summary>
    /// Gets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
    /// </summary>
    /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
    public void ProcessRequest(HttpContext context)
    {
        context.Response.AddHeader("Pragma", "no-cache");
        context.Response.AddHeader("Cache-Control", "private, no-cache");

        this.HandleMethod(context);
    }

    /// <summary>
    /// Returns the options.
    /// </summary>
    /// <param name="context">The context.</param>
    private static void ReturnOptions(HttpContext context)
    {
        context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
        context.Response.StatusCode = 200;
    }

    /// <summary>
    /// Writes the JSON iFrame safe.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="statuses">The statuses.</param>
    private static void WriteJsonIframeSafe(HttpContext context, List<FilesUploadStatus> statuses)
    {
        context.Response.AddHeader("Vary", "Accept");

        context.Response.ContentType = "application/json";

        var jsonObject = new JavaScriptSerializer().Serialize(statuses.ToArray());
        context.Response.Write(jsonObject);
    }

    /// <summary>
    /// Handle request based on method
    /// </summary>
    /// <param name="context">The context.</param>
    private void HandleMethod(HttpContext context)
    {
        switch (context.Request.HttpMethod)
        {
            case "POST":
            case "PUT":
                this.UploadFile(context);
                break;

            case "OPTIONS":
                ReturnOptions(context);
                break;

            default:
                context.Response.ClearHeaders();
                context.Response.StatusCode = 405;
                break;
        }
    }

    /// <summary>
    /// Uploads the file.
    /// </summary>
    /// <param name="context">The context.</param>
    private void UploadFile(HttpContext context)
    {
        var statuses = new List<FilesUploadStatus>();

        this.UploadWholeFile(context, statuses);

        WriteJsonIframeSafe(context, statuses);
    }

    /// <summary>
    /// Uploads the whole file.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="statuses">The statuses.</param>
    private void UploadWholeFile(HttpContext context, ICollection<FilesUploadStatus> statuses)
    {
        var yafUserId = BoardContext.Current.PageUserID;
        var uploadFolder = BoardContext.Current.Get<BoardFolders>().Uploads;

        if (!BoardContext.Current.UploadAccess)
        {
            throw new HttpRequestValidationException("No Access");
        }

        try
        {
            var allowedExtensions = this.Get<BoardSettings>().AllowedFileExtensions.ToLower().Split(',');

            for (var i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);

                var extension = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    statuses.Add(
                        new FilesUploadStatus {
                                                  error = "Invalid File"
                                              });

                    return;
                }

                if (!MimeTypes.FileMatchContentType(file))
                {
                    statuses.Add(
                        new FilesUploadStatus {
                                                  error = "Invalid File"
                                              });

                    return;
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
                        new FilesUploadStatus {
                                                  error = "File does not have a name"
                                              });

                    return;
                }

                if (fileName.Length > 220)
                {
                    fileName = fileName.Substring(fileName.Length - 220);
                }

                // verify the size of the attachment
                if (this.Get<BoardSettings>().MaxFileSize > 0
                    && file.ContentLength > this.Get<BoardSettings>().MaxFileSize)
                {
                    statuses.Add(new FilesUploadStatus { error = this.Get<ILocalization>().GetTextFormatted(
                                                           "UPLOAD_TOOBIG",
                                                           file.ContentLength / 1024,
                                                           this.Get<BoardSettings>().MaxFileSize / 1024)
                                                       });

                    return;
                }

                Stream resized = null;

                try
                {
                    // resize image ?!
                    using var img = Image.FromStream(file.InputStream);
                    if (img.Width > this.Get<BoardSettings>().ImageAttachmentResizeWidth ||
                        img.Height > this.Get<BoardSettings>().ImageAttachmentResizeHeight)
                    {
                        resized = ImageHelper.GetResizedImageStreamFromImage(img,
                            this.Get<BoardSettings>().ImageAttachmentResizeWidth,
                            this.Get<BoardSettings>().ImageAttachmentResizeHeight);
                    }
                }
                catch (Exception)
                {
                    resized = null;
                }

                int newAttachmentId;

                if (this.Get<BoardSettings>().UseFileTable)
                {
                    if (resized is null)
                    {
                        byte[] fileData;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            fileData.Length.ToType<int>(),
                            file.ContentType,
                            fileData.ToArray());
                    }
                    else
                    {
                        var image = Image.FromStream(resized);

                        var memoryStream = new MemoryStream();
                        image.Save(memoryStream, image.RawFormat);
                        memoryStream.Position = 0;

                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            resized.Length.ToType<int>(),
                            file.ContentType,
                            resized.ToArray());

                        image.Dispose();
                    }
                }
                else
                {
                    var previousDirectory = this.Get<HttpRequestBase>()
                        .MapPath(Path.Combine(BaseUrlBuilder.ServerFileRoot, uploadFolder));

                    // check if Uploads folder exists
                    if (!Directory.Exists(previousDirectory))
                    {
                        Directory.CreateDirectory(previousDirectory);
                    }

                    if (resized is null)
                    {
                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            file.ContentLength,
                            file.ContentType);

                        using var fs = new FileStream($"{previousDirectory}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload", FileMode.Create, FileAccess.ReadWrite);
                        byte[] fileData;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        fs.Write(fileData, 0, fileData.Length);
                    }
                    else
                    {
                        var newFile = Image.FromStream(resized);

                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            resized.Length.ToType<int>(),
                            file.ContentType);

                        using var memory = new MemoryStream();
                        using var fs = new FileStream($"{previousDirectory}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload", FileMode.Create, FileAccess.ReadWrite);
                        newFile.Save(memory, newFile.RawFormat);
                        var bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);

                        newFile.Dispose();
                    }
                }

                var fullName = Path.GetFileName(fileName);
                statuses.Add(new FilesUploadStatus(fullName, file.ContentLength, newAttachmentId));
            }
        }
        catch (Exception ex)
        {
            statuses.Add(new FilesUploadStatus
                         {
                             error = ex.Message
                         });

            this.Get<ILoggerService>().Error(ex, "Error during Attachment upload");
        }
    }
}