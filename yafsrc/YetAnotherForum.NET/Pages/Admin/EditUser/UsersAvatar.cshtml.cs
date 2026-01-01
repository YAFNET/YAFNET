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

using YAF.Core.Model;

namespace YAF.Pages.Admin.EditUser;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using YAF.Core.Context;
using YAF.Core.Helpers;
using YAF.Core.Services;
using YAF.Pages.Profile;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// Class UsersAvatarModel.
/// Implements the <see cref="YAF.Core.BasePages.AdminPage" />
/// </summary>
/// <seealso cref="YAF.Core.BasePages.AdminPage" />
public class UsersAvatarModel : AdminPage
{
    /// <summary>
    /// Gets or sets the User Data.
    /// </summary>
    public Tuple<User, AspNetUsers, Rank, VAccess> EditUser { get; set; }

    /// <summary>
    /// Gets or sets the gallery.
    /// </summary>
    /// <value>The gallery.</value>
    public List<SelectListItem> Gallery { get; set; }

    /// <summary>
    /// Gets or sets the upload.
    /// </summary>
    /// <value>The upload.</value>
    [BindProperty]
    public IFormFile Upload { get; set; }

    /// <summary>
    /// Gets or sets the avatar gallery.
    /// </summary>
    /// <value>The avatar gallery.</value>
    [BindProperty]
    public string AvatarGallery { get; set; }

    /// <summary>
    /// Gets or sets the avatar URL.
    /// </summary>
    /// <value>The avatar URL.</value>
    [BindProperty]
    public string AvatarUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public UsersSignatureInputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UsersAvatarModel"/> class.
    /// </summary>
    public UsersAvatarModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    /// <summary>Called when [get].</summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnGet(int userId)
    {
        if (!BoardContext.Current.IsAdmin)
        {
            return this.Get<ILinkBuilder>().AccessDenied();
        }

        this.Input = new UsersSignatureInputModel {
            UserId = userId
        };

        return this.BindData(userId);
    }

    /// <summary>
    /// Delete Current Avatar
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAvatarAsync()
    {
        await this.GetRepository<User>().DeleteAvatarAsync(this.Input.UserId);

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View4" });
    }

    /// <summary>
    /// Save selected Avatar from Gallery
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostGalleryAsync()
    {
        if (!this.AvatarGallery.IsNotSet() || this.AvatarGallery == this.GetText("OURAVATAR"))
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {u = this.Input.UserId, tab = "View4"});
        }

        // save the avatar right now...
        await this.GetRepository<User>().SaveAvatarAsync(
            this.Input.UserId,
            this.AvatarGallery,
            null,
            null);

        // clear the cache for this user...
        this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View4" });
    }

    /// <summary>
    /// Upload selected avatar
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostUploadUpdateAsync()
    {
        if (this.Upload.FileName.Trim().Length <= 0 || !this.Upload.FileName.Trim().IsImageName())
        {
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View4" });
        }

        long x = this.PageBoardContext.BoardSettings.AvatarWidth;
        long y = this.PageBoardContext.BoardSettings.AvatarHeight;
        var avatarSize = this.PageBoardContext.BoardSettings.AvatarSize;

        MemoryStream resizedImage = null;

        try
        {
            using var image = await Image.LoadAsync(this.Upload.OpenReadStream());

            if (image.Width > x || image.Height > y)
            {
                this.PageBoardContext.SessionNotify(
                    $"{this.GetTextFormatted("WARN_TOOBIG", x, y)} {this.GetTextFormatted("WARN_SIZE", image.Width, image.Height)} {this.GetText("EDIT_AVATAR", "WARN_RESIZED")}",
                    MessageTypes.warning);

                resizedImage = ImageHelper.GetResizedImage(image, x, y);
            }

            // Delete old first...
            await this.GetRepository<User>().DeleteAvatarAsync(this.Input.UserId);

            if (this.PageBoardContext.BoardSettings.UseFileTable)
            {
                await this.SaveAvatarToTableAsync(resizedImage);
            }
            else
            {
                await this.SaveAvatarToFolderAsync(resizedImage);
            }

            // clear the cache for this user...
            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.Input.UserId));

            if (avatarSize > 0 && this.Upload.Length >= avatarSize && resizedImage is null)
            {
                this.PageBoardContext.SessionNotify(
                    $"{this.GetTextFormatted("WARN_BIGFILE", avatarSize)} {this.GetTextFormatted("WARN_FILESIZE", this.Upload.Length)}",
                    MessageTypes.warning);
                return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View4" });
            }

            return this.Get<ILinkBuilder>().Redirect(ForumPages.Profile_EditAvatar);
        }
        catch (Exception exception)
        {
            this.Get<ILogger<EditAvatarModel>>().Error(exception, exception.Message);

            // image is probably invalid...
            this.PageBoardContext.SessionNotify(this.GetText("EDIT_AVATAR", "INVALID_FILE"), MessageTypes.danger);
            return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_EditUser, new { u = this.Input.UserId, tab = "View4" });
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private IActionResult BindData(int userId)
    {
        if (this.Get<IDataCache>()[string.Format(Constants.Cache.EditUser, userId)] is not Tuple<User, AspNetUsers, Rank, VAccess> user)
        {
            return this.Get<ILinkBuilder>().Redirect(
                ForumPages.Admin_EditUser,
                new {
                    u = userId
                });
        }

        this.EditUser = user;

        var currentUser = this.EditUser.Item1;

        var avatars = this.LoadAvatarGallery();

        if (avatars.Count != 0)
        {
            this.Gallery = avatars;
        }

        var showNoAvatar = false;

        if (!currentUser.AvatarImage.IsNullOrEmptyField())
        {
            this.AvatarUrl = this.Get<IUrlHelper>().Action(
                "GetResponseLocalAvatar",
                "Avatar",
                new {userId = this.Input.UserId, v = DateTime.Now.Ticks.ToString()});
        }
        else if (currentUser.Avatar.IsSet() && currentUser.Avatar.StartsWith('/'))
        {
            if (currentUser.Avatar.Contains(this.Get<BoardFolders>().Avatars))
            {
                var item = avatars.Find(x => x.Value == currentUser.Avatar);

                if (item != null)
                {
                    item.Selected = true;

                    this.AvatarUrl = currentUser.Avatar;
                }
                else
                {
                    showNoAvatar = true;
                }
            }
            else
            {
                this.AvatarUrl = currentUser.Avatar;
            }
        }
        else
        {
            showNoAvatar = true;
        }

        if (showNoAvatar)
        {
            this.AvatarUrl = this.Get<IUrlHelper>().Action(
                "GetTextAvatar",
                "Avatar",
                new {userId = this.Input.UserId, v = DateTime.Now.Ticks.ToString()});
        }

        return this.Page();
    }

    /// <summary>
    /// Loads the avatar gallery.
    /// </summary>
    /// <returns>List&lt;SelectListItem&gt;</returns>
    private List<SelectListItem> LoadAvatarGallery()
    {
        var avatars = new List<SelectListItem>();

        if (!this.PageBoardContext.BoardSettings.AvatarGallery)
        {
            return avatars;
        }

        avatars.Add(new SelectListItem(this.GetText("OURAVATAR"), ""));

        var dir = new DirectoryInfo(
            Path.Combine(this.Get<BoardInfo>().WebRootPath, this.Get<BoardFolders>().Avatars));

        var files = dir.GetFiles("*.*").ToList();

        avatars.AddImageFiles(files, this.Get<BoardFolders>().Avatars);

        return avatars;
    }

    /// <summary>
    /// Saves the avatar to DB table.
    /// </summary>
    /// <param name="resized">The resized.</param>
    private async Task SaveAvatarToTableAsync(Stream resized)
    {
        if (resized is null)
        {
            using var image = await Image.LoadAsync(this.Upload.OpenReadStream());

            var memoryStream = new MemoryStream();

            await image.SaveAsync(memoryStream, image.Metadata.DecodedImageFormat);

            await this.GetRepository<User>().SaveAvatarAsync(
                this.Input.UserId,
                null,
                memoryStream,
                this.Upload.ContentType);
        }
        else
        {
            await this.GetRepository<User>().SaveAvatarAsync(
                this.Input.UserId,
                null,
                resized,
                this.Upload.ContentType);
        }
    }

    /// <summary>
    /// Saves the avatar to folder.
    /// </summary>
    /// <param name="resized">The resized.</param>
    private async Task SaveAvatarToFolderAsync(MemoryStream resized)
    {
        var uploadFolderPath = Path.Combine(this.Get<IWebHostEnvironment>().WebRootPath, this.Get<BoardFolders>().Uploads);

        var extensionOld = Path.GetExtension(this.Upload.FileName);

        var fileName = Path.Combine(uploadFolderPath, this.Upload.FileName.Replace(extensionOld, ".webp"));

        var pos = fileName.LastIndexOfAny(['/', '\\']);

        if (pos >= 0)
        {
            fileName = fileName[(pos + 1)..];
        }

        // filename can be only 255 characters long (due to table column)
        if (fileName.Length > 255)
        {
            fileName = fileName[^255..];
        }

        var newFileName = $"{this.Input.UserId}{Path.GetExtension(fileName)}";

        var filePath = Path.Combine(uploadFolderPath, newFileName);

        // Delete old avatar
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        if (resized is null)
        {
            using var avatarImage = await Image.LoadAsync(this.Upload.OpenReadStream());

            using var memory = new MemoryStream();
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            await avatarImage.SaveAsync(memory, new WebpEncoder());
            var bytes = memory.ToArray();
            await fs.WriteAsync(bytes);

            await this.GetRepository<User>().SaveAvatarAsync(
                this.Input.UserId,
                this.Url.Content($"~/{this.Get<BoardFolders>().Uploads}/{newFileName}"),
                null,
                null);
        }
        else
        {
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite);
            var bytes = resized.ToArray();
            await fs.WriteAsync(bytes);

            await this.GetRepository<User>().SaveAvatarAsync(
                this.Input.UserId,
                this.Url.Content($"~/{this.Get<BoardFolders>().Uploads}/{newFileName}"),
                null,
                null);
        }
    }
}