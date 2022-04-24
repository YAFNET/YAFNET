/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Core.Services;

#region Using

using System;
using System.IO;
using System.Linq;
using System.Web;

using YAF.Core.Context;
using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Core.Model;
using YAF.Core.Services.Startup;
using YAF.Core.Utilities;
using YAF.Types.Constants;
using YAF.Types.Models;
using YAF.Types.Objects;

#endregion

/// <summary>
/// Album Service for the current user.
/// </summary>
public class Album : IAlbum, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Album"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Album([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    #region Properties

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Deletes the specified album/image.
    /// </summary>
    /// <param name="uploadFolder">
    /// The Upload folder.
    /// </param>
    /// <param name="albumId">
    /// The album id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="imageId">
    /// The image id.
    /// </param>
    public void AlbumImageDelete(
        [NotNull] string uploadFolder,
        [CanBeNull] int? albumId,
        int userId,
        [NotNull] int? imageId)
    {
        if (albumId.HasValue)
        {
            var albums = BoardContext.Current.GetRepository<UserAlbumImage>().List(albumId.Value);

            albums.ForEach(
                dr =>
                    {
                        var fullName = $"{uploadFolder}/{userId}.{albumId}.{dr.FileName}.yafalbum";
                        var file = new FileInfo(fullName);

                        try
                        {
                            if (!file.Exists)
                            {
                                return;
                            }

                            File.SetAttributes(fullName, FileAttributes.Normal);
                            File.Delete(fullName);
                        }
                        finally
                        {
                            var imageIdDelete = dr.ID;
                            BoardContext.Current.GetRepository<UserAlbumImage>().DeleteById(imageIdDelete);
                            BoardContext.Current.GetRepository<UserAlbum>().DeleteCover(imageIdDelete);
                        }
                    });

            this.GetRepository<UserAlbumImage>().Delete(a => a.AlbumID == albumId.ToType<int>());

            this.GetRepository<UserAlbum>().Delete(a => a.ID == albumId.ToType<int>());
        }
        else
        {
            var image = this.GetRepository<UserAlbumImage>().GetImage(imageId.Value);

            var fileName = image.Item1.FileName;
            var imgAlbumId = image.Item1.AlbumID.ToString();
            var fullName = $"{uploadFolder}/{userId}.{imgAlbumId}.{fileName}.yafalbum";
            var file = new FileInfo(fullName);

            try
            {
                if (!file.Exists)
                {
                    return;
                }

                File.SetAttributes(fullName, FileAttributes.Normal);
                File.Delete(fullName);
            }
            finally
            {
                this.GetRepository<UserAlbumImage>().DeleteById(imageId.Value);
                this.GetRepository<UserAlbum>().DeleteCover(imageId.Value);
            }
        }
    }

    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public ReturnClass ChangeImageCaption(int imageId, [NotNull] string newCaption)
    {
        // load the DB so BoardContext can work...
        CodeContracts.VerifyNotNull(newCaption);

        this.Get<StartupInitializeDb>().Run();

        this.GetRepository<UserAlbumImage>().UpdateCaption(imageId, newCaption);
        var returnObject = new ReturnClass { NewTitle = newCaption };

        returnObject.NewTitle = newCaption == string.Empty
                                    ? this.Get<ILocalization>().GetText(
                                        "ALBUM",
                                        "ALBUM_IMAGE_CHANGE_CAPTION")
                                    : newCaption;
        returnObject.Id = imageId.ToString();
        return returnObject;
    }

    /// <summary>
    /// The get album image preview.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="localizationFile">The localization file.</param>
    /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
    public void GetAlbumImagePreview([NotNull] HttpContext context, string localizationFile, bool previewCropped)
    {
        // Check QueryString first
        if (!ValidationHelper.IsNumeric(context.Request.QueryString.GetFirstOrDefault("imgprv")))
        {
            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            return;
        }

        var etag =
            $@"""{context.Request.QueryString.GetFirstOrDefault("imgprv")}{localizationFile.GetHashCode()}""";

        try
        {
            // ImageID
            var image = this.GetRepository<UserAlbumImage>()
                .GetImage(context.Request.QueryString.GetFirstOrDefaultAs<int>("imgprv"));

            var uploadFolder = this.Get<BoardFolders>().Uploads;

            var oldFileName = context.Server.MapPath(
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}");
            var newFileName = context.Server.MapPath(
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum");

            // use the new fileName (with extension) if it exists...
            var fileName = File.Exists(newFileName) ? newFileName : oldFileName;

            var ms = new MemoryStream();

            using var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            input.CopyTo(ms);

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.Cache.SetETag(etag);

            ms.Dispose();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(
                BoardContext.Current.PageUser.ID,
                this,
                $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                EventLogTypes.Information);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// The get album cover.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="localizationFile">The localization file.</param>
    /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
    public void GetAlbumCover([NotNull] HttpContext context, string localizationFile, bool previewCropped)
    {
        // Check QueryString first
        if (!ValidationHelper.IsNumeric(context.Request.QueryString.GetFirstOrDefault("album")))
        {
            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

            return;
        }

        // Check QueryString first
        if (!ValidationHelper.IsNumeric(context.Request.QueryString.GetFirstOrDefault("cover")))
        {
            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

            return;
        }

        var etag = $@"""{context.Request.QueryString.GetFirstOrDefault("cover")}{localizationFile.GetHashCode()}""";

        try
        {
            // CoverID
            var fileName = string.Empty;
            var data = new MemoryStream();
            if (context.Request.QueryString.GetFirstOrDefault("cover") == "0")
            {
                var album = this.GetRepository<UserAlbumImage>().List(context.Request.QueryString.GetFirstOrDefaultAs<int>("album"));

                var random = new RandomGenerator();

                if (album != null && album.Any())
                {
                    var image = this.GetRepository<UserAlbumImage>().GetImage(album[random.Next(1, album.Count)].ID);

                    var uploadFolder = this.Get<BoardFolders>().Uploads;

                    var oldFileName = context.Server.MapPath(
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}");
                    var newFileName = context.Server.MapPath(
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum");

                    // use the new fileName (with extension) if it exists...
                    fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                }
            }
            else
            {
                var image = this.GetRepository<UserAlbumImage>()
                    .GetImage(context.Request.QueryString.GetFirstOrDefaultAs<int>("cover"));

                if (image != null)
                {
                    var uploadFolder = this.Get<BoardFolders>().Uploads;

                    var oldFileName = context.Server.MapPath(
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}");
                    var newFileName = context.Server.MapPath(
                        $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum");

                    // use the new fileName (with extension) if it exists...
                    fileName = File.Exists(newFileName) ? newFileName : oldFileName;
                }
            }

            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                input.CopyTo(data);
            }

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(data.ToArray(), 0, data.Length.ToType<int>());
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.Cache.SetETag(etag);

            data.Dispose();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(
                BoardContext.Current.PageUser.ID,
                this,
                x,
                EventLogTypes.Information);
            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// The get album image.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    public void GetAlbumImage([NotNull] HttpContext context)
    {
        try
        {
            // Check QueryString first
            if (!ValidationHelper.IsNumeric(context.Request.QueryString.GetFirstOrDefault("image")))
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

                return;
            }

            // ImageID
            var image = this.GetRepository<UserAlbumImage>()
                .GetImage(context.Request.QueryString.GetFirstOrDefaultAs<int>("image"));

            byte[] data;

            var uploadFolder = this.Get<BoardFolders>().Uploads;

            var oldFileName = context.Server.MapPath(
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}");
            var newFileName = context.Server.MapPath(
                $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum");

            // use the new fileName (with extension) if it exists...
            var fileName = File.Exists(newFileName) ? newFileName : oldFileName;

            using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                data = new byte[input.Length];
                input.Read(data, 0, data.Length);
                input.Close();
            }

            context.Response.ContentType = image.Item1.ContentType;

            if (context.Response.ContentType.Contains("text"))
            {
                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
            else
            {
                context.Response.OutputStream.Write(data, 0, data.Length);

                // add a download count...
                this.GetRepository<UserAlbumImage>().IncrementDownload(
                    context.Request.QueryString.GetFirstOrDefaultAs<int>("image"));
            }
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(
                BoardContext.Current.PageUser.ID,
                this,
                $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                EventLogTypes.Information);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    /// <summary>
    /// Gets the Preview Image as Response
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="localizationFile">The localization file.</param>
    /// <param name="previewCropped">if set to <c>true</c> [preview cropped].</param>
    public void GetResponseImagePreview([NotNull] HttpContext context, string localizationFile, bool previewCropped)
    {
        // Check QueryString first
        if (!ValidationHelper.IsNumeric(context.Request.QueryString.GetFirstOrDefault("p")))
        {
            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");

            return;
        }

        var etag = $@"""{context.Request.QueryString.GetFirstOrDefault("p")}{localizationFile.GetHashCode()}""";

        try
        {
            // AttachmentID
            var attachment = this.GetRepository<Attachment>()
                .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("p"));

            if (!BoardContext.Current.DownloadAccess)
            {
                // tear it down
                // no permission to download
                context.Response.Write(
                    "You have insufficient rights to download this resource. Contact forum administrator for further details.");
                return;
            }

            var data = new MemoryStream();

            if (attachment.FileData == null)
            {
                var uploadFolder = this.Get<BoardFolders>().Uploads;

                var oldFileName = context.Server.MapPath(
                    $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                var newFileName = context.Server.MapPath(
                    $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                string fileName;

                if (File.Exists(oldFileName))
                {
                    fileName = oldFileName;
                }
                else
                {
                    oldFileName = context.Server.MapPath(
                        $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}.yafupload");

                    // use the new fileName (with extension) if it exists...
                    fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                    // Find wrongly converted attachments
                    if (!File.Exists(fileName) && attachment.MessageID.Equals(0))
                    {
                        var file = Directory.EnumerateFiles(context.Server.MapPath(uploadFolder)).FirstOrDefault(
                            f => f.Contains($"{attachment.FileName}.yafupload"));

                        fileName = file;
                    }
                }

                using var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                input.CopyTo(data);
            }
            else
            {
                var buffer = attachment.FileData;
                data.Write(buffer, 0, buffer.Length);
            }

            // reset position...
            data.Position = 0;

            context.Response.ContentType = "image/png";

            // output stream...
            context.Response.OutputStream.Write(data.ToArray(), 0, data.Length.ToType<int>());
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetMaxAge(TimeSpan.FromHours(2));
            context.Response.Cache.SetLastModified(DateTime.UtcNow);
            context.Response.Cache.SetETag(etag);

            data.Dispose();
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Log(
                BoardContext.Current.PageUser.ID,
                this,
                $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                EventLogTypes.Information);

            context.Response.Write(
                "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
        }
    }

    #endregion
}