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

namespace YAF.Core.Services;

using System.IO;

using Model;

using Types.Models;

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
    public Album(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Deletes an album with all its images, only the owner's albums.
    /// </summary>
    /// <param name="uploadFolder">The upload folder.</param>
    /// <param name="albumId">The album id.</param>
    /// <param name="userId">The user id (the owner of the album).</param>
    public async Task AlbumDeleteAsync(string uploadFolder,
        int albumId,
        int userId)
    {
        var album = await this.GetRepository<UserAlbum>().GetByIdAsync(albumId);

        // unknown album or not the own one
        if (album is null || album.UserID != userId)
        {
            return;
        }

        foreach (var image in this.GetRepository<UserAlbumImage>().List(albumId))
        {
            DeleteFile($"{uploadFolder}/{userId}.{albumId}.{image.FileName}.yafalbum");

            await this.GetRepository<UserAlbum>().DeleteCoverAsync(image.ID);
        }

        await this.GetRepository<UserAlbumImage>().DeleteAsync(a => a.AlbumID == albumId);
        await this.GetRepository<UserAlbum>().DeleteAsync(a => a.ID == albumId);
    }

    /// <summary>
    /// Deletes an image of an album, only images in the owner's albums.
    /// </summary>
    /// <param name="uploadFolder">The upload folder.</param>
    /// <param name="imageId">The image id.</param>
    /// <param name="userId">The user id (the owner of the album).</param>
    public async Task AlbumImageDeleteAsync(string uploadFolder,
        int imageId,
        int userId)
    {
        var image = await this.GetRepository<UserAlbumImage>().GetByIdAsync(imageId, true).ConfigureAwait(false);

        // unknown image or not in an own album
        if (image?.UserAlbum is null || image.UserAlbum.UserID != userId)
        {
            return;
        }

        DeleteFile($"{uploadFolder}/{userId}.{image.AlbumID}.{image.FileName}.yafalbum");

        await this.GetRepository<UserAlbumImage>().DeleteByIdAsync(imageId);
        await this.GetRepository<UserAlbum>().DeleteCoverAsync(imageId);
    }

    /// <summary>
    /// Changes the album title.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newTitle">
    /// The New title.
    /// </param>
    public void ChangeAlbumTitle(int imageId, string newTitle)
    {
        var album = this.GetRepository<UserAlbum>().GetById(imageId);

        // unknown album or not the own one
        if (album is null || album.UserID != BoardContext.Current.PageUserID)
        {
            return;
        }

        album.Title = newTitle;

        this.GetRepository<UserAlbum>().Update(album);
    }

    /// <summary>
    /// Change the album image caption.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    public void ChangeImageCaption(int imageId, string newCaption)
    {
        var image = this.GetRepository<UserAlbumImage>().GetById(imageId, true);

        // unknown image or not in an own album
        if (image?.UserAlbum is null || image.UserAlbum.UserID != BoardContext.Current.PageUserID)
        {
            return;
        }

        image.Caption = newCaption;

        this.GetRepository<UserAlbumImage>().Update(image);
    }

    /// <summary>
    /// Deletes the file of an album image if it exists (images can also be stored in the database).
    /// </summary>
    /// <param name="fullName">The full file name.</param>
    private static void DeleteFile(string fullName)
    {
        if (!File.Exists(fullName))
        {
            return;
        }

        File.SetAttributes(fullName, FileAttributes.Normal);
        File.Delete(fullName);
    }
}
