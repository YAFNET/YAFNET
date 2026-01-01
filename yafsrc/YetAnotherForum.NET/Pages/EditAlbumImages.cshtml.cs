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
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

using YAF.Core.Helpers;

namespace YAF.Pages;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;

/// <summary>
/// The Edit User Album Images Page.
/// </summary>
public class EditAlbumImagesModel : ForumPageRegistered
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EditAlbumImagesModel" /> class.
    /// </summary>
    public EditAlbumImagesModel()
        : base("EDIT_ALBUMIMAGES", ForumPages.EditAlbumImages)
    {
    }

    /// <summary>
    /// Gets or sets the album identifier.
    /// </summary>
    /// <value>The album identifier.</value>
    [BindProperty]
    public int? AlbumId { get; set; }

    /// <summary>
    /// The groups.
    /// </summary>
    [BindProperty]
    public string AlbumTitle { get; set; }

    /// <summary>
    /// Gets or sets the stream.
    /// </summary>
    [BindProperty]
    public List<UserAlbumImage> Images { get; set; }

    /// <summary>
    /// Gets or sets the allowed images.
    /// </summary>
    /// <value>The allowed images.</value>
    [BindProperty]
    public int AllowedImages { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [enable upload].
    /// </summary>
    /// <value><c>true</c> if [enable upload]; otherwise, <c>false</c>.</value>
    [BindProperty]
    public bool EnableUpload { get; set; }

    /// <summary>
    /// Gets or sets the image files.
    /// </summary>
    /// <value>The image files.</value>
    [BindProperty]
    public List<IFormFile> ImageFiles { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        var displayName = this.PageBoardContext.PageUser.DisplayOrUserName();

        this.PageBoardContext.PageLinks.AddUser(this.PageBoardContext.PageUserID, displayName);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="a">
    /// The album Id.
    /// </param>
    public async Task<IActionResult> OnGetAsync(string a)
    {
        if (!this.PageBoardContext.BoardSettings.EnableAlbum)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        int? albumId = null;

        try
        {
            if (a != "new")
            {
                albumId = a.ToType<int>();
            }
        }
        catch (Exception)
        {
            albumId = null;
        }

        this.PageBoardContext.PageLinks.AddLink(
            !albumId.HasValue
                ? this.GetText("EDIT_ALBUMIMAGES", "TITLE_NEW")
                : this.GetText("EDIT_ALBUMIMAGES", "TITLE"),
            string.Empty);

        return await this.BindDataAsync(albumId);
    }

    /// <summary>
    /// Delete Album
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    /// <returns>IActionResult.</returns>
    public Task<IActionResult> OnPostAsync(int albumId)
    {
        return this.DeleteAlbumAsync(albumId);
    }

    /// <summary>
    /// Delete an Image form the Album
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    /// <param name="imageId">The image identifier.</param>
    public async Task OnPostDeleteImageAsync(int? albumId, int imageId)
    {
        var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

        await this.Get<IAlbum>().AlbumImageDeleteAsync(
            path,
            imageId,
            this.PageBoardContext.PageUserID);

        await this.BindDataAsync(albumId);
    }

    /// <summary>
    /// Upload Album Image
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task<IActionResult> OnPostUploadAsync(int? albumId)
    {
        try
        {
            if (this.CheckValidFiles(this.ImageFiles))
            {
                albumId = await this.SaveAttachmentsAsync(this.ImageFiles, albumId);
            }

            return await this.BindDataAsync(albumId);
        }
        catch (Exception x)
        {
            if (x is ThreadAbortException)
            {
                return await this.BindDataAsync(null);
            }

            this.Get<ILogger<EditAlbumImagesModel>>().Log(this.PageBoardContext.PageUserID, this, x);

            this.PageBoardContext.Notify(x.Message, MessageTypes.danger);

            return await this.BindDataAsync(null);
        }
    }

    /// <summary>
    /// Update Album Title
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    public Task OnPostUpdateTitleAsync(int albumId)
    {
        this.AlbumTitle = HttpUtility.HtmlEncode(this.AlbumTitle);

        this.GetRepository<UserAlbum>().UpdateTitle(
            albumId,
            this.AlbumTitle);

        return this.BindDataAsync(albumId);
    }

    /// <summary>
    /// Called when [post back].
    /// </summary>
    /// <param name="albumId">The album identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostBack(int? albumId = null)
    {
        if (albumId.HasValue)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Album,
                new { u = this.PageBoardContext.PageUserID, a = albumId });
        }

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Albums, new { u = this.PageBoardContext.PageUserID });
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private async Task<IActionResult> BindDataAsync(int? albumId)
    {
        var data = await this.GetRepository<User>().MaxAlbumDataAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageBoardID);

        this.AllowedImages = (int)data.UserAlbumImages;

        var albumSize = this.GetRepository<UserAlbum>().CountUserAlbum(this.PageBoardContext.PageUserID);

        if (!albumId.HasValue)
        {
            // Has the user created maximum number of albums?
            if (this.AllowedImages > 0 && albumSize >= this.AllowedImages)
            {
                // Albums count. If we reached limit then we go to info page.
                return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.AccessDenied);
            }

            this.Images = [];
        }
        else
        {
            this.AlbumId = albumId;

            this.AlbumTitle = this.GetRepository<UserAlbum>().GetTitle(this.AlbumId.Value);

            this.Images = this.GetRepository<UserAlbumImage>().List(this.AlbumId.Value);

            // Check if user album is empty
            if (this.Images.NullOrEmpty())
            {
                await this.DeleteAlbumAsync(this.AlbumId.Value);

                return this.Page();
            }
        }

        // Has the user uploaded maximum number of images?
        if (this.AllowedImages > 0)
        {
            this.EnableUpload = this.Images.Count < this.AllowedImages;
        }
        else
        {
            this.EnableUpload = false;
        }

        return this.Page();
    }

    /// <summary>
    /// Deletes the Entire Album
    /// </summary>
    private async Task<IActionResult> DeleteAlbumAsync(int albumId)
    {
        var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

        await this.Get<IAlbum>().AlbumDeleteAsync(
            path,
            albumId,
            this.PageBoardContext.PageUserID);

        // clear the cache for this user to update albums|images stats...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.EditAlbumImages);
    }

    /// <summary>
    /// Check to see if the user is trying to upload  valid file(s).
    /// </summary>
    /// <param name="files">
    /// the uploaded file(s).
    /// </param>
    /// <returns>
    /// true if file is valid for uploading. otherwise false.
    /// </returns>
    private bool CheckValidFiles(List<IFormFile> files)
    {
        if (files is not {Count: > 0})
        {
            return false;
        }

        foreach (var item in files)
        {
            var filePath = item.FileName.Trim();

            if (filePath.IsNotSet() || item.Length == 0)
            {
                return false;
            }

            if (item.ContentType.Contains("text", StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath).ToLower();

            // remove the "period"
            extension = extension.Replace(".", string.Empty);
            var imageExtensions = StaticDataHelper.ImageFormats;

            // If we don't get a match from the db, then the extension is not allowed
            // also, check to see an image is being uploaded.
            if (Array.IndexOf(imageExtensions, extension) != -1)
            {
                return true;
            }

            this.PageBoardContext.Notify(this.GetTextFormatted("FILEERROR", extension), MessageTypes.warning);
        }

        return false;
    }

    /// <summary>
    /// Save the attached file(s) both physically and in the Database
    /// </summary>
    /// <param name="files">the file(s).</param>
    /// <param name="albumId"></param>
    /// <exception cref="Exception">Album Image File is too big</exception>
    private async Task<int?> SaveAttachmentsAsync(List<IFormFile> files, int? albumId)
    {
        var data = await this.GetRepository<User>().MaxAlbumDataAsync(
            this.PageBoardContext.PageUserID,
            this.PageBoardContext.PageBoardID);

        var usrAlbumImagesAllowed = (int)data.UserAlbumImages;

        if (!albumId.HasValue)
        {
            var usrAlbumsAllowed = (int)data.UserAlbum;

            var allStats = this.GetRepository<UserAlbum>().CountUserAlbum(this.PageBoardContext.PageUserID);

            // Albums count. If we reached limit then we exit.
            if (allStats >= usrAlbumsAllowed)
            {
                this.PageBoardContext.Notify(this.GetTextFormatted("ALBUMS_COUNT_LIMIT", usrAlbumImagesAllowed), MessageTypes.warning);
                return null;
            }

            albumId = this.GetRepository<UserAlbum>().Save(
                this.PageBoardContext.PageUserID,
                this.AlbumTitle,
            null);
        }

        foreach (var file in files.Where(x => x != null && x.FileName.Trim().Length > 0 && x.Length > 0))
        {
            var path = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

            // check if Uploads folder exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filename = file.FileName;

            var pos = filename.LastIndexOfAny(['/', '\\']);
            if (pos >= 0)
            {
                filename = filename[(pos + 1)..];
            }

            // filename can be only 255 characters long (due to table column)
            if (filename.Length > 255)
            {
                filename = filename[^255..];
            }

            // verify the size of the attachment
            if (this.PageBoardContext.BoardSettings.AlbumImagesSizeMax > 0
                && file.Length > this.PageBoardContext.BoardSettings.AlbumImagesSizeMax)
            {
                this.PageBoardContext.Notify(this.GetText("ERROR_TOOBIG"), MessageTypes.danger);
                return albumId;
            }

            long allStats;

            try
            {
                allStats = this.GetRepository<UserAlbumImage>().CountAlbumImages(
                    albumId.Value);
            }
            catch (Exception )
            {
                allStats = 0;
            }

            // Images count. If we reached limit then we exit.
            if (allStats >= usrAlbumImagesAllowed)
            {
                this.PageBoardContext.Notify(this.GetTextFormatted("IMAGES_COUNT_LIMIT", usrAlbumImagesAllowed), MessageTypes.warning);
                return albumId;
            }

            var filePath = $"{path}/{this.PageBoardContext.PageUserID}.{albumId.Value}.{filename}.yafalbum";

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            this.GetRepository<UserAlbumImage>().Save(
                null,
                albumId.Value,
                null,
                filename,
                file.Length.ToType<int>(),
                file.ContentType);

            usrAlbumImagesAllowed++;
        }

        return albumId;
    }
}