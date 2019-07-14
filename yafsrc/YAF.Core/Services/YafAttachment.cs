/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2019 Ingo Herbote
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Web;

    using YAF.Configuration;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.Localization;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf attachment.
    /// </summary>
    public class YafAttachment : IAttachment, IHaveServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YafAttachment"/> class.
        /// </summary>
        /// <param name="serviceLocator">
        /// The service locator.
        /// </param>
        public YafAttachment([NotNull] IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;
        }

        #region Properties

        /// <summary>
        /// Gets or sets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator { get; set; }

        #endregion

        /// <summary>
        /// The get response attachment.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void GetResponseAttachment([NotNull] HttpContext context)
        {
            try
            {
                // AttachmentID
                var attachment =
                    this.GetRepository<Attachment>()
                        .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("a"));

                var boardID = context.Request.QueryString.GetFirstOrDefault("b") != null
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : YafContext.Current.BoardSettings.BoardID;

                if (!this.Get<IPermissions>().CheckAccessRights(boardID, attachment.MessageID))
                {
                    // tear it down
                    // no permission to download
                    context.Response.Write(
                        "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                    return;
                }

                byte[] data;


                this.GetRepository<Attachment>()
                    .IncrementDownloadCounter(attachment.ID);

                if (attachment.FileData == null)
                {
                    var uploadFolder = YafBoardFolders.Current.Uploads;

                    var oldFileName =
                        context.Server.MapPath(
                            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                    var newFileName =
                        context.Server.MapPath(
                            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                    var fileName = oldFileName;

                    if (File.Exists(oldFileName))
                    {
                        fileName = oldFileName;
                    }
                    else
                    {
                        oldFileName =
                        context.Server.MapPath(
                            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload");

                        // use the new fileName (with extension) if it exists...
                        fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                        // its an old extension
                        if (!File.Exists(fileName))
                        {
                            fileName = context.Server.MapPath(
                                $"{uploadFolder}/{attachment.MessageID.ToString()}.{attachment.FileName}.yafupload");
                        }
                    }

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        data = input.ToArray();
                        input.Close();
                    }
                }
                else
                {
                    data = attachment.FileData;
                }

                context.Response.ContentType = attachment.ContentType;
                context.Response.AppendHeader(
                    "Content-Disposition",
                    $"attachment; filename={HttpUtility.UrlPathEncode(attachment.FileName).Replace("+", "_")}");
                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                        EventLogTypes.Information);
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Gets the response image.
        /// </summary>
        /// <param name="context">The context.</param>
        public void GetResponseImage([NotNull] HttpContext context)
        {
            try
            {
                var eTag = $@"""{context.Request.QueryString.GetFirstOrDefault("i")}""";

                // AttachmentID
                var attachment =
                    this.GetRepository<Attachment>()
                        .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("i"));


                if (context.Request.QueryString.GetFirstOrDefault("editor") == null)
                {
                    // add a download count...
                    this.GetRepository<Attachment>()
                        .IncrementDownloadCounter(attachment.ID);
                }

                var boardID = context.Request.QueryString.GetFirstOrDefault("b") != null
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : YafContext.Current.BoardSettings.BoardID;

                // check download permissions here
                if (!this.Get<IPermissions>().CheckAccessRights(boardID, attachment.MessageID))
                {
                    // tear it down
                    // no permission to download
                    context.Response.Write(
                        "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                    return;
                }

                byte[] data;

                if (attachment.FileData == null)
                {
                    var uploadFolder = YafBoardFolders.Current.Uploads;

                    var oldFileName =
                         context.Server.MapPath(
                             $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                    var newFileName =
                        context.Server.MapPath(
                            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                    var fileName = oldFileName;

                    if (File.Exists(oldFileName))
                    {
                        fileName = oldFileName;

                    }
                    else
                    {
                        oldFileName =
                        context.Server.MapPath(
                            $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload");

                        // use the new fileName (with extension) if it exists...
                        fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                    }

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        data = input.ToArray();
                        input.Close();
                    }
                }
                else
                {
                    data = attachment.FileData;
                }

                context.Response.ContentType = attachment.ContentType;
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetETag(eTag);
                context.Response.OutputStream.Write(data, 0, data.Length);
            }
            catch (Exception x)
            {
                this.Get<ILogger>()
                    .Log(
                        YafContext.Current.PageUserID,
                        this,
                        $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                        EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }
    }
}