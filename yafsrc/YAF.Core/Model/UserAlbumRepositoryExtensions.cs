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

namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The UserAlbum Repository Extensions
    /// </summary>
    public static class UserAlbumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Lists all the albums associated with the UserID or gets all the
        ///   specifications for the specified album id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="albumID">
        /// the album id.
        /// </param>
        /// <returns>
        /// a Datatable containing the albums.
        /// </returns>
        public static List<UserAlbum> List(
            [NotNull] this IRepository<UserAlbum> repository, int albumId)
        {
            return repository.Get(userAlbum => userAlbum.ID == albumId);
        }

        /// <summary>
        /// Lists all the albums associated with the UserID or gets all the
        ///   specifications for the specified album id.
        /// </summary>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="albumID">
        /// the album id.
        /// </param>
        /// <returns>
        /// a Datatable containing the albums.
        /// </returns>
        public static List<UserAlbum> ListByUser(
            [NotNull] this IRepository<UserAlbum> repository, int userId)
        {
            return repository.Get(userAlbum => userAlbum.UserID == userId).OrderByDescending(u => u.Updated).ToList();
        }


        public static string GetTitle(this IRepository<UserAlbum> repository, [NotNull] int albumId)
        {
            return repository.GetById(albumId).Title;
        }

        public static void DeleteCover(this IRepository<UserAlbum> repository, [NotNull] int imageId)
        {
            repository.UpdateOnly(() => new UserAlbum { CoverImageID = null }, u => u.CoverImageID == imageId);
        }

        [NotNull]
        public static long CountUserAlbum([NotNull] this IRepository<UserAlbum> repository, [NotNull] int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Count(album => album.UserID == userId);
        }

        public static void UpdateTitle(
            this IRepository<UserAlbum> repository,
            [NotNull] int albumId,
            [NotNull] string title)
        {
            repository.UpdateOnly(
                () => new UserAlbum { Title = title },
                album => album.ID == albumId);
        }

        public static void UpdateCover(
            this IRepository<UserAlbum> repository,
            [NotNull] int albumId,
            [NotNull] int? coverImageId)
        {
            repository.UpdateOnly(
                () => new UserAlbum { CoverImageID = coverImageId },
                album => album.ID == albumId);
        }

        public static int Save(
            this IRepository<UserAlbum> repository,
            [NotNull] int userId,
            [NotNull] string title,
            [NotNull] int? coverImageId)
        {
            var entity = new UserAlbum
                             {
                                 UserID = userId, Title = title, CoverImageID = coverImageId, Updated = DateTime.UtcNow
                             };

            var newId = repository.Insert(entity);

            repository.FireNew(entity);

            return newId;
        }

        #endregion
    }
}