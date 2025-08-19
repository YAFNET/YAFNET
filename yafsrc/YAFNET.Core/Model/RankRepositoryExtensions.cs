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

using System.Threading.Tasks;

namespace YAF.Core.Model;

using System;

using YAF.Types.Models;

/// <summary>
///     The Rank repository extensions.
/// </summary>
public static class RankRepositoryExtensions
{
    /// <summary>
    /// Saves or adds a New Rank
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="rankId">
    /// The rank Id.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="flags">
    /// The flags.
    /// </param>
    /// <param name="minPosts">
    /// The min posts.
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
    /// <param name="signatureChars">
    /// Defines number of allowed characters in user signature.
    /// </param>
    /// <param name="signatureBBCodes">
    /// Defines comma separated BBCodes allowed for a rank, i.e in a user signature
    /// </param>
    /// <param name="userAlbums">
    /// Defines allowed number of albums.
    /// </param>
    /// <param name="userAlbumImages">
    /// Defines number of images allowed for an album.
    /// </param>
    public async static Task SaveAsync(
        this IRepository<Rank> repository,
        int? rankId,
        int boardId,
        string name,
        RankFlags flags,
        int? minPosts,
        string style,
        short sortOrder,
        string description,
        int signatureChars,
        string signatureBBCodes,
        int userAlbums,
        int userAlbumImages)
    {
        minPosts = flags.IsLadder switch
        {
            false => null,
            true when !minPosts.HasValue => 0,
            _ => minPosts
        };

        if (rankId.HasValue)
        {
            await repository.UpdateOnlyAsync(
                () => new Rank
                          {
                              Name = name,
                              Flags = flags.BitValue,
                              MinPosts = minPosts,
                              Style = style,
                              SortOrder = sortOrder,
                              Description = description,
                              UsrSigChars = signatureChars,
                              UsrSigBBCodes = signatureBBCodes,
                              UsrAlbums = userAlbums,
                              UsrAlbumImages = userAlbumImages
                          },
                g => g.ID == rankId.Value);
        }
        else
        {
            rankId = await repository.InsertAsync(
                new Rank
                    {
                        Name = name,
                        BoardID = boardId,
                        Flags = flags.BitValue,
                        MinPosts = minPosts,
                        Style = style,
                        SortOrder = sortOrder,
                        Description = description,
                        UsrSigChars = signatureChars,
                        UsrSigBBCodes = signatureBBCodes,
                        UsrAlbums = userAlbums,
                        UsrAlbumImages = userAlbumImages
                    });
        }

        if (style.IsSet())
        {
            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateUserStylesEvent(boardId));
        }
    }

    /// <summary>
    /// Get the User with the Current Rank
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    /// <returns>
    /// The <see cref="Tuple"/>.
    /// </returns>
    public async static Task<Tuple<User, Rank>> GetUserAndRankAsync(this IRepository<Rank> repository, int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

        expression.Join<Rank>((u, r) => r.ID == u.RankID).Where<User>(u => u.ID == userId);

        var results = await repository.DbAccess.ExecuteAsync(db => db.SelectMultiAsync<User, Rank>(expression));

        return results.FirstOrDefault();
    }

    /// <summary>
    /// Gets the Style from Current User Rank.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>Returns the Style if the Rank has one</returns>
    public async static Task<string> GetRankStyleForUserAsync(
        this IRepository<Rank> repository,
        int userId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Rank>();

        expression.Join<User>((rank, user) => rank.ID == user.RankID)
            .Where<Rank, User>((rank, user) => rank.Style != null && user.ID == userId);

        var results = await repository.DbAccess.ExecuteAsync(db => db.SingleAsync(expression));

        return results?.Style;
    }
}