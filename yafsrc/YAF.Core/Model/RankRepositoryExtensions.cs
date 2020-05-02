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
namespace YAF.Core.Model
{
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The Rank repository extensions.
    /// </summary>
    public static class RankRepositoryExtensions
    {
        /// <summary>
        /// The rank_save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="rankID">
        /// The rank id.
        /// </param>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isStart">
        /// The is start.
        /// </param>
        /// <param name="isLadder">
        /// The is ladder.
        /// </param>
        /// <param name="minPosts">
        /// The min posts.
        /// </param>
        /// <param name="pmLimit">
        /// The pm limit.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="sortOrder">
        /// The sort order.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="usrSigChars">
        /// The usrSigChars defines number of allowed characters in user signature.
        /// </param>
        /// <param name="usrSigBBCodes">
        /// The UsrSigBBCodes.defines comma separated bbcodes allowed for a rank, i.e in a user signature
        /// </param>
        /// <param name="usrSigHTMLTags">
        /// The UsrSigHTMLTags defines comma separated tags allowed for a rank, i.e in a user signature
        /// </param>
        /// <param name="usrAlbums">
        /// The UsrAlbums defines allowed number of albums.
        /// </param>
        /// <param name="usrAlbumImages">
        /// The UsrAlbumImages defines number of images allowed for an album.
        /// </param>
        public static void Save(
            this IRepository<Rank> repository,
            [NotNull] object rankID,
            [NotNull] object boardID,
            [NotNull] object name,
            [NotNull] object isStart,
            [NotNull] object isLadder,
            [NotNull] object minPosts,
            [NotNull] object pmLimit,
            [NotNull] object style,
            [NotNull] object sortOrder,
            [NotNull] object description,
            [NotNull] object usrSigChars,
            [NotNull] object usrSigBBCodes,
            [NotNull] object usrSigHTMLTags,
            [NotNull] object usrAlbums,
            [NotNull] object usrAlbumImages)
        {
            repository.DbFunction.Scalar.rank_save(
                RankID: rankID,
                BoardID: boardID,
                Name: name,
                IsStart: isStart,
                IsLadder: isLadder,
                MinPosts: minPosts,
                PMLimit: pmLimit,
                Style: style,
                SortOrder: sortOrder,
                Description: description,
                UsrSigChars: usrSigChars,
                UsrSigBBCodes: usrSigBBCodes,
                UsrSigHTMLTags: usrSigHTMLTags,
                UsrAlbums: usrAlbums,
                UsrAlbumImages: usrAlbumImages);
        }
    }
}