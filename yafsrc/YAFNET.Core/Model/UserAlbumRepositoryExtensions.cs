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

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Models;

/// <summary>
/// The UserAlbum Repository Extensions
/// </summary>
public static class UserAlbumRepositoryExtensions
{
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<UserAlbum> repository)
    {
        /// <summary>
        /// Lists all the albums associated with the UserID or gets all the
        ///   specifications for the specified album id.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        public List<UserAlbum> ListByUser(int userId)
        {
            return [.. repository.Get(userAlbum => userAlbum.UserID == userId).OrderByDescending(u => u.Updated)];
        }

        /// <summary>
        /// Lists all the albums associated with the UserID or gets all the
        ///   specifications for the specified album id.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="pageIndex">
        /// The page Index.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        public List<UserAlbum> ListByUserPaged(int userId, int pageIndex, int pageSize)
        {
            return [.. repository.GetPaged(userAlbum => userAlbum.UserID == userId, pageIndex, pageSize).OrderByDescending(u => u.Updated)];
        }

        /// <summary>
        /// The get title.
        /// </summary>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetTitle(int albumId)
        {
            return repository.GetById(albumId).Title;
        }

        /// <summary>
        /// The delete cover.
        /// </summary>
        /// <param name="imageId">
        /// The image id.
        /// </param>
        public Task DeleteCoverAsync(int imageId)
        {
            return repository.UpdateOnlyAsync(() => new UserAlbum { CoverImageID = null }, u => u.CoverImageID == imageId);
        }

        /// <summary>
        /// The count user album.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public long CountUserAlbum(int userId)
        {
            return repository.Count(album => album.UserID == userId);
        }

        /// <summary>
        /// The update title.
        /// </summary>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        public void UpdateTitle(int albumId,
            string title)
        {
            repository.UpdateOnly(
                () => new UserAlbum { Title = title },
                album => album.ID == albumId);
        }

        /// <summary>
        /// The update cover.
        /// </summary>
        /// <param name="albumId">
        /// The album id.
        /// </param>
        /// <param name="coverImageId">
        /// The cover image id.
        /// </param>
        public void UpdateCover(int albumId,
            int? coverImageId)
        {
            repository.UpdateOnly(
                () => new UserAlbum { CoverImageID = coverImageId },
                album => album.ID == albumId);
        }

        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="coverImageId">
        /// The cover image id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Save(int userId,
            string title,
            int? coverImageId)
        {
            var entity = new UserAlbum
            {
                UserID = userId, Title = title, CoverImageID = coverImageId, Updated = DateTime.UtcNow
            };

            var newId = repository.Insert(entity);

            return newId;
        }
    }
}