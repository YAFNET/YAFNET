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

using System.Collections.Generic;

using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The group repository extensions.
/// </summary>
public static class GroupRepositoryExtensions
{
    /// <summary>
    /// The list.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    public static IList<Group> List(
        this IRepository<Group> repository,
        int? groupId = null,
        int? boardId = null)
    {
        return groupId.HasValue
                   ? repository.Get(g => g.BoardID == boardId && g.ID == groupId.Value)
                   : [.. repository.Get(g => g.BoardID == boardId).OrderBy(o => o.SortOrder)];
    }

    /// <summary>
    /// Gets All Roles by User indicating if User is Member or not
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board Id.
    /// </param>
    /// <param name="userId">
    /// The user Id.
    /// </param>
    public static List<GroupMember> Member(
        this IRepository<Group> repository,
        int boardId,
        int userId)
    {
        return repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<Group>();

                    expression.LeftJoin<UserGroup>((g, u) => u.UserID == userId && u.GroupID == g.ID)
                        .Where(g => g.BoardID == boardId).OrderBy<Group>(a => a.Name).Select<Group, UserGroup>(
                            (g, u) => new { GroupID = g.ID, g.Name, u.UserID });

                    return db.Connection.Select<GroupMember>(expression);
                });
    }

    /// <summary>
    /// Save or Add new Group
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="groupId">
    /// The group Id.
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
    /// <param name="accessMaskId">
    /// The access mask id.
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
    /// The signature BBCodes.
    /// </param>
    /// <param name="userAlbums">
    /// Defines allowed number of albums.
    /// </param>
    /// <param name="userAlbumImages">
    /// Defines number of images allowed for an album.
    /// </param>
    /// <returns>
    /// Returns the group Id
    /// </returns>
    public async static Task<int> SaveAsync(
        this IRepository<Group> repository,
        int? groupId,
        int boardId,
        string name,
        GroupFlags flags,
        int accessMaskId,
        string style,
        short sortOrder,
        string description,
        int signatureChars,
        string signatureBBCodes,
        int userAlbums,
        int userAlbumImages)
    {
        if (groupId.HasValue)
        {
            await repository.UpdateOnlyAsync(
                () => new Group
                          {
                              Name = name,
                              Flags = flags.BitValue,
                              Style = style,
                              SortOrder = sortOrder,
                              Description = description,
                              UsrSigChars = signatureChars,
                              UsrSigBBCodes = signatureBBCodes,
                              UsrAlbums = userAlbums,
                              UsrAlbumImages = userAlbumImages
                          },
                g => g.ID == groupId.Value);
        }
        else
        {
            groupId = await repository.InsertAsync(
                new Group
                    {
                        Name = name,
                        BoardID = boardId,
                        Flags = flags.BitValue,
                        Style = style,
                        SortOrder = sortOrder,
                        Description = description,
                        UsrSigChars = signatureChars,
                        UsrSigBBCodes = signatureBBCodes,
                        UsrAlbums = userAlbums,
                        UsrAlbumImages = userAlbumImages
                    });

            BoardContext.Current.GetRepository<ForumAccess>().InitialAssignGroup(groupId.Value, accessMaskId);
        }

        if (style.IsSet())
        {
            // -- group styles override rank styles
            await BoardContext.Current.Get<IRaiseEventAsync>().RaiseAsync(new UpdateUserStylesEvent(boardId));
        }

        return groupId.Value;
    }
}