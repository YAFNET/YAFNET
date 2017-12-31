/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
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

namespace YAF.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Web;
    using System.Web.Script.Serialization;
    using System.Web.SessionState;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Core.Services.Startup;
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
        public IServiceLocator ServiceLocator => YafContext.Current.ServiceLocator;

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

            this.WriteJsonIframeSafe(context, statuses);
        }

        /// <summary>
        /// Uploads the whole file.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="statuses">The statuses.</param>
        private void UploadWholeFile(HttpContext context, List<FilesUploadStatus> statuses)
        {
            var forumID = HttpContext.Current.Request["forumID"].ToType<int>();
            var boardID = HttpContext.Current.Request["boardID"].ToType<int>();
            var yafUserID = HttpContext.Current.Request["userID"].ToType<int>();
            var uploadFolder = HttpContext.Current.Request["uploadFolder"];

            if (!this.CheckAccessRights(boardID, forumID))
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

                        //fileName = fileName.Unidecode();
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
                    if (this.Get<YafBoardSettings>().MaxFileSize > 0
                        && file.ContentLength > this.Get<YafBoardSettings>().MaxFileSize)
                    {
                        throw new HttpRequestValidationException(
                            this.Get<ILocalization>()
                                .GetTextFormatted(
                                    "UPLOAD_TOOBIG",
                                    file.ContentLength / 1024,
                                    this.Get<YafBoardSettings>().MaxFileSize / 1024));
                    }

                    int newAttachmentID;

                    if (this.Get<YafBoardSettings>().UseFileTable)
                    {
                        newAttachmentID = this.GetRepository<Attachment>()
                            .Save(
                                messageId: 0,
                                userId: yafUserID,
                                fileName: fileName,
                                bytes: file.ContentLength,
                                contentType: file.ContentType,
                                fileData: file.InputStream.ToArray());
                    }
                    else
                    {
                        var previousDirectory =
                            this.Get<HttpRequestBase>()
                                .MapPath(Path.Combine(BaseUrlBuilder.ServerFileRoot, uploadFolder));

                        // check if Uploads folder exists
                        if (!Directory.Exists(previousDirectory))
                        {
                            Directory.CreateDirectory(previousDirectory);
                        }

                        newAttachmentID = this.GetRepository<Attachment>()
                            .Save(
                                messageId: 0,
                                userId: yafUserID,
                                fileName: fileName,
                                bytes: file.ContentLength,
                                contentType: file.ContentType);

                        file.SaveAs(
                            "{0}/u{1}-{2}.{3}.yafupload".FormatWith(
                                previousDirectory,
                                yafUserID,
                                newAttachmentID,
                                fileName));
                    }

                    var fullName = Path.GetFileName(fileName);
                    statuses.Add(new FilesUploadStatus(fullName, file.ContentLength, newAttachmentID));
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger>().Error(ex, "Error during Attachment upload");
            }
        }

        /// <summary>
        /// Writes the JSON iFrame safe.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="statuses">The statuses.</param>
        private void WriteJsonIframeSafe(HttpContext context, List<FilesUploadStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");

            context.Response.ContentType = "application/json";

            var jsonObject = new JavaScriptSerializer().Serialize(statuses.ToArray());
            context.Response.Write(jsonObject);
        }

        /// <summary>
        /// Checks the access rights.
        /// </summary>
        /// <param name="boardID">The board id.</param>
        /// <param name="forumID">The forum identifier.</param>
        /// <returns>
        /// The check access rights.
        /// </returns>
        private bool CheckAccessRights([NotNull] int boardID, [NotNull] int forumID)
        {
            // Find user name
            var user = UserMembershipHelper.GetUser();

            var browser = "{0} {1}".FormatWith(
                HttpContext.Current.Request.Browser.Browser, HttpContext.Current.Request.Browser.Version);
            var platform = HttpContext.Current.Request.Browser.Platform;
            var isMobileDevice = HttpContext.Current.Request.Browser.IsMobileDevice;
            bool isSearchEngine;
            bool dontTrack;
            var userAgent = HttpContext.Current.Request.UserAgent;

            // try and get more verbose platform name by ref and other parameters
            UserAgentHelper.Platform(
                userAgent,
                HttpContext.Current.Request.Browser.Crawler,
                ref platform,
                ref browser,
                out isSearchEngine,
                out dontTrack);

            this.Get<StartupInitializeDb>().Run();

            object userKey = DBNull.Value;

            if (user != null)
            {
                userKey = user.ProviderUserKey;
            }

            DataRow pageRow = LegacyDb.pageload(
                HttpContext.Current.Session.SessionID,
                boardID,
                userKey,
                HttpContext.Current.Request.GetUserRealIPAddress(),
                HttpContext.Current.Request.FilePath,
                HttpContext.Current.Request.QueryString.ToString(),
                browser,
                platform,
                null,
                forumID,
                null,
                null,
                isSearchEngine, // don't track if this is a search engine
                isMobileDevice,
                dontTrack);

            return pageRow["UploadAccess"].ToType<bool>() || pageRow["ModeratorAccess"].ToType<bool>();
        }
    }
}