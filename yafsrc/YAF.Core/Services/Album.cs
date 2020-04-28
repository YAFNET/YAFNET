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
    using YAF.Core.Context;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Core.Services.Startup;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
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
        /// The change album title.
        /// </summary>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <param name="newTitle">
        /// The New title.
        /// </param>
        /// <returns>
        /// the return object.
        /// </returns>
        public ReturnClass ChangeAlbumTitle(int albumId, [NotNull] string newTitle)
        {
            // load the DB so BoardContext can work...
            CodeContracts.VerifyNotNull(newTitle, "newTitle");

            this.Get<StartupInitializeDb>().Run();

            // newTitle = System.Web.HttpUtility.HtmlEncode(newTitle);
            this.GetRepository<UserAlbum>().UpdateTitle(albumId, newTitle);

            var returnObject = new ReturnClass { NewTitle = newTitle };

            returnObject.NewTitle = newTitle == string.Empty
                                        ? this.Get<ILocalization>().GetText("ALBUM", "ALBUM_CHANGE_TITLE")
                                        : newTitle;
            returnObject.Id = $"0{albumId.ToString()}";
            return returnObject;
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
            CodeContracts.VerifyNotNull(newCaption, "newCaption");

            this.Get<StartupInitializeDb>().Run();

            // newCaption = System.Web.HttpUtility.HtmlEncode(newCaption);
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
            var etag =
                $@"""{context.Request.QueryString.GetFirstOrDefault("imgprv")}{localizationFile.GetHashCode()}""";

            try
            {
                // ImageID
                var image = this.GetRepository<UserAlbumImage>()
                    .GetImage(context.Request.QueryString.GetFirstOrDefaultAs<int>("imgprv"));

                var data = new MemoryStream();

                var uploadFolder = BoardFolders.Current.Uploads;

                var oldFileName = context.Server.MapPath(
                    $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}");
                var newFileName = context.Server.MapPath(
                    $"{uploadFolder}/{image.Item2.UserID}.{image.Item1.AlbumID}.{image.Item1.FileName}.yafalbum");

                // use the new fileName (with extension) if it exists...
                var fileName = File.Exists(newFileName) ? newFileName : oldFileName;

                using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var buffer = new byte[input.Length];
                    input.Read(buffer, 0, buffer.Length);
                    data.Write(buffer, 0, buffer.Length);
                    input.Close();
                }

                // reset position...
                data.Position = 0;

                var ms = GetAlbumOrAttachmentImageResized(
                    data,
                    this.Get<BoardSettings>().ImageAttachmentResizeWidth,
                    this.Get<BoardSettings>().ImageAttachmentResizeHeight,
                    previewCropped,
                    image.Item1.Downloads.ToType<int>(),
                    localizationFile,
                    "POSTS");

                context.Response.ContentType = "image/png";

                // output stream...
                context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(System.DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);
                context.Response.Cache.SetETag(etag);

                data.Dispose();
                ms.Dispose();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
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
            var etag = $@"""{context.Request.QueryString.GetFirstOrDefault("cover")}{localizationFile.GetHashCode()}""";

            try
            {
                // CoverID
                var fileName = string.Empty;
                var data = new MemoryStream();
                if (context.Request.QueryString.GetFirstOrDefault("cover") == "0")
                {
                    var album = this.GetRepository<UserAlbumImage>().List(context.Request.QueryString.GetFirstOrDefaultAs<int>("album"));

                    var random = new Random();

                    if (album != null && album.Any())
                    {
                        var image = this.GetRepository<UserAlbumImage>().GetImage(album[random.Next(album.Count)].ID);

                        var uploadFolder = BoardFolders.Current.Uploads;

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
                        var uploadFolder = BoardFolders.Current.Uploads;

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
                    var buffer = new byte[input.Length];
                    input.Read(buffer, 0, buffer.Length);
                    data.Write(buffer, 0, buffer.Length);
                    input.Close();
                }

                // reset position...
                data.Position = 0;
                var imagesNumber = this.GetRepository<UserAlbumImage>()
                    .CountAlbumImages(context.Request.QueryString.GetFirstOrDefaultAs<int>("album"));
                var ms = GetAlbumOrAttachmentImageResized(
                    data,
                    this.Get<BoardSettings>().ImageAttachmentResizeWidth,
                    this.Get<BoardSettings>().ImageAttachmentResizeHeight,
                    previewCropped,
                    imagesNumber.ToType<int>(),
                    localizationFile,
                    "ALBUM");

                context.Response.ContentType = "image/png";

                // output stream...
                context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(System.DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);
                context.Response.Cache.SetETag(etag);

                data.Dispose();
                ms.Dispose();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(BoardContext.Current.PageUserID, this, x, EventLogTypes.Information);
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
                // ImageID
                var image = this.GetRepository<UserAlbumImage>()
                    .GetImage(context.Request.QueryString.GetFirstOrDefaultAs<int>("image"));

                byte[] data;

                var uploadFolder = BoardFolders.Current.Uploads;

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
                this.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
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
            var etag = $@"""{context.Request.QueryString.GetFirstOrDefault("p")}{localizationFile.GetHashCode()}""";

            // defaults
            var previewMaxWidth = this.Get<BoardSettings>().ImageThumbnailMaxWidth;
            var previewMaxHeight = this.Get<BoardSettings>().ImageThumbnailMaxHeight;

            try
            {
                // AttachmentID
                var attachment = this.GetRepository<Attachment>()
                    .GetById(context.Request.QueryString.GetFirstOrDefaultAs<int>("p"));

                var boardID = context.Request.QueryString.Exists("b")
                                  ? context.Request.QueryString.GetFirstOrDefaultAs<int>("b")
                                  : BoardContext.Current.BoardSettings.BoardID;

                if (!this.Get<IPermissions>().CheckAccessRights(boardID, attachment.MessageID))
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
                    var uploadFolder = BoardFolders.Current.Uploads;

                    var oldFileName = context.Server.MapPath(
                        $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}")}.{attachment.FileName}");

                    var newFileName = context.Server.MapPath(
                        $"{uploadFolder}/{(attachment.MessageID > 0 ? attachment.MessageID.ToString() : $"u{attachment.UserID}-{attachment.ID}")}.{attachment.FileName}.yafupload");

                    var fileName = oldFileName;

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

                    using (var input = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var buffer = new byte[input.Length];
                        input.Read(buffer, 0, buffer.Length);
                        data.Write(buffer, 0, buffer.Length);
                        input.Close();
                    }
                }
                else
                {
                    var buffer = attachment.FileData;
                    data.Write(buffer, 0, buffer.Length);
                }

                // reset position...
                data.Position = 0;

                var ms = GetAlbumOrAttachmentImageResized(
                    data,
                    previewMaxWidth,
                    previewMaxHeight,
                    previewCropped,
                    attachment.Downloads,
                    localizationFile,
                    "POSTS");

                context.Response.ContentType = "image/png";

                // output stream...
                context.Response.OutputStream.Write(ms.ToArray(), 0, ms.Length.ToType<int>());
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetExpires(System.DateTime.UtcNow.AddHours(2));
                context.Response.Cache.SetLastModified(System.DateTime.UtcNow);
                context.Response.Cache.SetETag(etag);

                data.Dispose();
                ms.Dispose();
            }
            catch (Exception x)
            {
                this.Get<ILogger>().Log(
                    BoardContext.Current.PageUserID,
                    this,
                    $"URL: {context.Request.Url}<br />Referer URL: {(context.Request.UrlReferrer != null ? context.Request.UrlReferrer.AbsoluteUri : string.Empty)}<br />Exception: {x}",
                    EventLogTypes.Information);

                context.Response.Write(
                    "Error: Resource has been moved or is unavailable. Please contact the forum admin.");
            }
        }

        /// <summary>
        /// Get the Album Or Image Attachment Preview
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="previewWidth">The preview width.</param>
        /// <param name="previewHeight">The preview height.</param>
        /// <param name="previewCropped">The preview Cropped</param>
        /// <param name="downloads">The downloads.</param>
        /// <param name="localizationFile">The localization file.</param>
        /// <param name="localizationPage">The localization page.</param>
        /// <returns>
        /// Resized Image Stream
        /// </returns>
        [NotNull]
        private static MemoryStream GetAlbumOrAttachmentImageResized(
            [NotNull] Stream data,
            int previewWidth,
            int previewHeight,
            bool previewCropped,
            int downloads,
            [NotNull] string localizationFile,
            string localizationPage)
        {
            const int PixelPadding = 6;
            const int BottomSize = 26;

            var localization = new Localization.Localization(localizationPage);
            localization.LoadTranslation(localizationFile);

            using (var src = new Bitmap(data))
            {
                var ms = new MemoryStream();

                var newImgSize = new Size(previewWidth, previewHeight);

                if (previewCropped)
                {
                    var width = (float)newImgSize.Width;
                    var height = (float)newImgSize.Height;

                    var xRatio = width / src.Width;
                    var yRatio = height / src.Height;

                    var ratio = Math.Min(xRatio, yRatio);

                    newImgSize = new Size(
                        Math.Min(
                            newImgSize.Width,
                            Math.Round(src.Width * ratio, MidpointRounding.AwayFromZero).ToType<int>()),
                        Math.Min(
                            newImgSize.Height,
                            Math.Round(src.Height * ratio, MidpointRounding.AwayFromZero).ToType<int>()));

                    newImgSize.Width = newImgSize.Width - PixelPadding;
                    newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;
                }
                else
                {
                    var finalHeight = Math.Abs(src.Height * newImgSize.Width / src.Width);

                    // Height resize if necessary
                    if (finalHeight > newImgSize.Height)
                    {
                        newImgSize.Width = src.Width * newImgSize.Height / src.Height;
                        finalHeight = newImgSize.Height;
                    }

                    newImgSize.Height = finalHeight;
                    newImgSize.Width = newImgSize.Width - PixelPadding;
                    newImgSize.Height = newImgSize.Height - BottomSize - PixelPadding;

                    if (newImgSize.Height <= BottomSize + PixelPadding)
                    {
                        newImgSize.Height = finalHeight;
                    }
                }

                var heightToSmallFix = newImgSize.Height <= BottomSize + PixelPadding;

                using (var dst = new Bitmap(
                    newImgSize.Width + PixelPadding,
                    newImgSize.Height + BottomSize + PixelPadding,
                    PixelFormat.Format24bppRgb))
                {
                    var srcImg = new Rectangle(
                        0,
                        0,
                        src.Width,
                        src.Height + (heightToSmallFix ? BottomSize + PixelPadding : 0));

                    if (previewCropped)
                    {
                        srcImg = new Rectangle(0, 0, newImgSize.Width, newImgSize.Height);
                    }

                    var destRect = new Rectangle(3, 3, dst.Width - PixelPadding, dst.Height - PixelPadding - BottomSize);
                    var rDstTxt1 = new Rectangle(3, destRect.Height + 3, newImgSize.Width, BottomSize - 13);
                    var rDstTxt2 = new Rectangle(3, destRect.Height + 16, newImgSize.Width, BottomSize - 13);

                    using (var g = Graphics.FromImage(dst))
                    {
                        g.Clear(Color.FromArgb(64, 64, 64));
                        g.FillRectangle(Brushes.White, destRect);

                        g.CompositingMode = CompositingMode.SourceOver;
                        g.CompositingQuality = CompositingQuality.GammaCorrected;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                        g.DrawImage(src, destRect, srcImg, GraphicsUnit.Pixel);

                        using (var f = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel))
                        {
                            using (var brush = new SolidBrush(Color.FromArgb(191, 191, 191)))
                            {
                                var sf = new StringFormat
                                             {
                                                 Alignment = StringAlignment.Near,
                                                 LineAlignment = StringAlignment.Center
                                             };

                                g.DrawString(localization.GetText("IMAGE_RESIZE_ENLARGE"), f, brush, rDstTxt1, sf);

                                sf.Alignment = StringAlignment.Far;
                                g.DrawString(
                                    string.Format(localization.GetText("IMAGE_RESIZE_VIEWS"), downloads),
                                    f,
                                    brush,
                                    rDstTxt2,
                                    sf);
                            }
                        }
                    }

                    // save the bitmap to the stream...
                    dst.Save(ms, ImageFormat.Png);
                    ms.Position = 0;

                    return ms;
                }
            }
        }

        #endregion
    }
}