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

namespace YAF.Core.Handlers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.SessionState;

using YAF.Core.Context.Start;
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
                // the upload script of the forum sends this header, a form of another web site can not send it
                // (it would upload files with the login cookie of the user)
                if (context.Request.Headers["X-Requested-With"].IsNotSet())
                {
                    context.Response.StatusCode = 403;
                    break;
                }

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
        // suspended users can't upload files (like the api requests), users without the upload access are
        // forbidden (not a server error)
        if (!BoardContext.Current.UploadAccess || SuspendedUserFilter.IsSuspended(BoardContext.Current))
        {
            context.Response.StatusCode = 403;
            return;
        }

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

        try
        {
            // the setting can contain spaces and dots ("jpg, .png")
            var allowedExtensions = this.Get<BoardSettings>().AllowedFileExtensions.ToLower().Split(',')
                .Select(x => x.Trim().TrimStart('.')).Where(x => x.Length > 0).ToList();

            for (var i = 0; i < context.Request.Files.Count; i++)
            {
                var file = context.Request.Files[i];

                var fileName = Path.GetFileName(file.FileName);

                var extension = Path.GetExtension(fileName).Replace(".", string.Empty).ToLower();

                if (!allowedExtensions.Contains(extension) || !MimeTypes.FileMatchContentType(file))
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
                    if (!FileHelper.ValidateFileName(fileName))
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
                    fileName = fileName[^220..];
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

                // the image check has read the upload, the file is read from the start again
                file.InputStream.Position = 0;

                int newAttachmentId;

                if (this.Get<BoardSettings>().UseFileTable)
                {
                    if (resized is null)
                    {
                        byte[] fileData;
                        // the stream stays open: its length is the size of the upload status
                        using (var binaryReader = new BinaryReader(file.InputStream, Encoding.UTF8, true))
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
                        // the resized image is already encoded in the format of the upload
                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            resized.Length.ToType<int>(),
                            file.ContentType,
                            resized.ToArray());

                        resized.Dispose();
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

                        file.SaveAs($"{previousDirectory}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload");
                    }
                    else
                    {
                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            yafUserId,
                            fileName,
                            resized.Length.ToType<int>(),
                            file.ContentType);

                        // the resized image is already encoded in the format of the upload
                        var bytes = resized.ToArray();

                        using var fs = new FileStream($"{previousDirectory}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload", FileMode.Create, FileAccess.ReadWrite);
                        fs.Write(bytes, 0, bytes.Length);

                        resized.Dispose();
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