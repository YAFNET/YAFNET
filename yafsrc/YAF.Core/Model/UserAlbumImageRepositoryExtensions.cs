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

namespace YAF.Core.Model
{
    using System.Collections.Generic;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The UserAlbumImage Repository Extensions
    /// </summary>
    public static class UserAlbumImageRepositoryExtensions
    {
        #region Public Methods and Operators

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
            [NotNull] this IRepository<UserAlbumImage> repository,
            int userId,
            int? pageIndex = 0,
            int? pageSize = 10000000)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
        public static int GetUserAlbumImageCount([NotNull] this IRepository<UserAlbumImage> repository, int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

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
        public static void IncrementDownload(this IRepository<UserAlbumImage> repository, [NotNull] int imageId)
        {
            repository.UpdateAdd(() => new UserAlbumImage { Downloads = 1 }, where: u => u.ID == imageId);
        }

        #endregion
    }
}