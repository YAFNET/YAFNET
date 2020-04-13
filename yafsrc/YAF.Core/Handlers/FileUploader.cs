/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Core.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.SessionState;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Context;
    using YAF.Core.Model;
    using YAF.Core.Services.Startup;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    /// <summary>
    /// The File Upload Handler
    /// </summary>
    public class FileUploader : IHttpHandler, IReadOnlySessionState, IHaveServiceLocator
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        public bool IsReusable => false;

        /// <summary>
        /// Gets the ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

        #endregion

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            // resource no longer works with dynamic compile...
            if (HttpContext.Current.Request["allowedUpload"] == null)
            {
                return;
            }

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
            var forumId = HttpContext.Current.Request["forumID"].ToType<int>();
            var boardId = HttpContext.Current.Request["boardID"].ToType<int>();
            var yafUserId = HttpContext.Current.Request["userID"].ToType<int>();
            var uploadFolder = HttpContext.Current.Request["uploadFolder"];

            if (!this.CheckAccessRights(boardId, forumId))
            {
                throw new HttpRequestValidationException("No Access");
            }

            try
            {
                for (var i = 0; i < context.Request.Files.Count; i++)
                {
                    var file = context.Request.Files[i];

                    var fileName = Path.GetFileName(file.FileName);

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
                        throw new HttpRequestValidationException("File does not have a name");
                    }

                    if (fileName.Length > 220)
                    {
                        fileName = fileName.Substring(fileName.Length - 220);
                    }

                    // verify the size of the attachment
                    if (this.Get<BoardSettings>().MaxFileSize > 0
                        && file.ContentLength > this.Get<BoardSettings>().MaxFileSize)
                    {
                        throw new HttpRequestValidationException(
                            this.Get<ILocalization>().GetTextFormatted(
                                "UPLOAD_TOOBIG",
                                file.ContentLength / 1024,
                                this.Get<BoardSettings>().MaxFileSize / 1024));
                    }

                    int newAttachmentId;

                    if (this.Get<BoardSettings>().UseFileTable)
                    {
                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            0,
                            yafUserId,
                            fileName,
                            file.ContentLength,
                            file.ContentType,
                            file.InputStream.ToArray());
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

                        newAttachmentId = this.GetRepository<Attachment>().Save(
                            0,
                            yafUserId,
                            fileName,
                            file.ContentLength,
                            file.ContentType);

                        file.SaveAs($"{previousDirectory}/u{yafUserId}-{newAttachmentId}.{fileName}.yafupload");
                    }

                    var fullName = Path.GetFileName(fileName);
                    statuses.Add(new FilesUploadStatus(fullName, file.ContentLength, newAttachmentId));
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger>().Error(ex, "Error during Attachment upload");
            }
        }

        /// <summary>
        /// Checks the access rights.
        /// </summary>
        /// <param name="boardId">The board id.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <returns>
        /// The check access rights.
        /// </returns>
        private bool CheckAccessRights([NotNull] int boardId, [NotNull] int forumId)
        {
            if (forumId == 0)
            {
                // is private message upload
                return true;
            }

            // Find user name
            var user = UserMembershipHelper.GetUser();

            var browser =
                $"{HttpContext.Current.Request.Browser.Browser} {HttpContext.Current.Request.Browser.Version}";
            var platform = HttpContext.Current.Request.Browser.Platform;
            var isMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;
            var userAgent = HttpContext.Current.Request.UserAgent;

            // try and get more verbose platform name by ref and other parameters
            UserAgentHelper.Platform(
                userAgent,
                HttpContext.Current.Request.Browser.Crawler,
                ref platform,
                ref browser,
                out var isSearchEngine,
                out var doNotTrack);

            this.Get<StartupInitializeDb>().Run();

            object userKey = DBNull.Value;

            if (user != null)
            {
                userKey = user.ProviderUserKey;
            }

            var pageRow = this.GetRepository<ActiveAccess>().PageLoadAsDataRow(
                HttpContext.Current.Session.SessionID,
                boardId,
                userKey,
                HttpContext.Current.Request.GetUserRealIPAddress(),
                HttpContext.Current.Request.FilePath,
                HttpContext.Current.Request.QueryString.ToString(),
                browser,
                platform,
                null,
                forumId,
                null,
                null,
                isSearchEngine,
                isMobileDevice,
                doNotTrack);

            return pageRow["UploadAccess"].ToType<bool>() || pageRow["ModeratorAccess"].ToType<bool>();
        }
    }
}