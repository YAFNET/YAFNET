/* Yet Another Forum.NET
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

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The UserAlbumImage Repository Extensions
/// </summary>
public static class UserAlbumImageRepositoryExtensions
{
    /// <summary>
    /// Gets the number of images on the album with AlbumID.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="albumId">
    /// The album Id.
    /// </param>
    public static long CountAlbumImages(this IRepository<UserAlbumImage> repository, int albumId)
    {
        return repository.Count(albumImage => albumImage.AlbumID == albumId);
    }

    /// <summary>
    /// Lists all the images associated with the Album Id.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="albumId">
    /// The album Id.
    /// </param>
    /// <returns>
    /// a Data table containing the image(s).
    /// </returns>
    public static List<UserAlbumImage> List(
        this IRepository<UserAlbumImage> repository,
        int albumId)
    {
        return [.. repository.Get(albumImage => albumImage.AlbumID == albumId).OrderByDescending(a => a.Uploaded)];
    }

    /// <summary>
    /// Lists all the images associated with the Album Id.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="albumId">
    /// The album Id.
    /// </param>
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    /// <returns>
    /// a Data table containing the image(s).
    /// </returns>
    public static List<UserAlbumImage> ListPaged(
        this IRepository<UserAlbumImage> repository,
        int albumId,
        int pageIndex,
        int pageSize)
    {
        return [.. repository.GetPaged(albumImage => albumImage.AlbumID == albumId, pageIndex, pageSize).OrderByDescending(a => a.Uploaded)];
    }

    /// <summary>
    /// Lists all the images associated with the AlbumID or
    ///   the image with the ImageID.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="imageId">
    /// The image Id.
    /// </param>
    /// <returns>
    /// a Data table containing the image(s).
    /// </returns>
    public static Tuple<UserAlbumImage, UserAlbum> GetImage(
        this IRepository<UserAlbumImage> repository,
        int imageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserAlbumImage>();

        expression.Join<UserAlbumImage, UserAlbum>((image, userAlbum) => userAlbum.ID == image.AlbumID)
            .Where<UserAlbumImage, UserAlbum>((image, userAlbum) => image.ID == imageId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<UserAlbumImage, UserAlbum>(expression)).FirstOrDefault();
    }

    /// <summary>
    /// Gets the paged list of all users Album Images
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="pageIndex">Index of the page.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <returns>
    /// Returns the list of entities
    /// </returns>
    public static List<UserAlbumImage> GetUserAlbumImagesPaged(
        this IRepository<UserAlbumImage> repository,
        int userId,
        int pageIndex = 0,
        int pageSize = 10000000)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserAlbumImage>();

        expression.Join<UserAlbumImage, UserAlbum>((image, userAlbum) => userAlbum.ID == image.AlbumID)
            .Where<UserAlbumImage, UserAlbum>((image, userAlbum) => userAlbum.UserID == userId)
            .OrderByDescending<UserAlbumImage>(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression));
    }

    /// <summary>
    /// Gets the user album image count.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>
    /// Returns the list of entities
    /// </returns>
    public static int GetUserAlbumImageCount(this IRepository<UserAlbumImage> repository, int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserAlbumImage>();

        expression.Join<UserAlbumImage, UserAlbum>((image, userAlbum) => userAlbum.ID == image.AlbumID)
            .Where<UserAlbumImage, UserAlbum>((image, userAlbum) => userAlbum.UserID == userId);

        return repository.DbAccess.Execute(db => db.Connection.Select(expression)).Count;
    }

    /// <summary>
    /// Increments the image's download times.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="imageId">The image identifier.</param>
    public static void IncrementDownload(this IRepository<UserAlbumImage> repository, int imageId)
    {
        repository.UpdateAdd(() => new UserAlbumImage { Downloads = 1 }, u => u.ID == imageId);
    }

    /// <summary>
    /// Inserts/Saves a user image.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="imageId">
    /// The image Id.
    /// </param>
    /// <param name="albumId">
    /// the album id for adding a new image.
    /// </param>
    /// <param name="caption">
    /// the caption of the existing/new image.
    /// </param>
    /// <param name="fileName">
    /// the file name of the new image.
    /// </param>
    /// <param name="bytes">
    /// the size of the new image.
    /// </param>
    /// <param name="contentType">
    /// the content type.
    /// </param>
    public static void Save(
        this IRepository<UserAlbumImage> repository,
        int? imageId,
        int albumId,
        string caption,
        string fileName,
        int bytes,
        string contentType)
    {
        repository.Insert(
            new UserAlbumImage
                {
                    ID = imageId ?? 0,
                    AlbumID = albumId,
                    Caption = caption,
                    Bytes = bytes,
                    ContentType = contentType,
                    Uploaded = DateTime.UtcNow,
                    FileName = fileName,
                    Downloads = 0
                });
    }
}