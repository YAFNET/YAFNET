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

using System.Linq;
using System.Threading.Tasks;

using YAF.Core.Extensions;
using YAF.Types.Flags;

namespace YAF.Core.Model;

using YAF.Types.Models;

/// <summary>
/// The access mask repository extensions.
/// </summary>
public static class AccessMaskRepositoryExtensions
{
    /// <summary>
    /// Gets the access mask new forums give to the roles (and a new role gets in the forums): the mask of the
    /// board setting if it belongs to the board (the default is the id of the "Member Access" of the first board,
    /// on another board it was a mask of the first board), otherwise the mask of the board with the most rights
    /// without moderator access ("Member Access" of a new board).
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="defaultMaskId">The access mask of the board setting.</param>
    /// <param name="boardId">The board id.</param>
    /// <returns>The access mask id, 0 if the board has no masks.</returns>
    public static int GetDefaultForumAccessMaskId(
        this IRepository<AccessMask> repository,
        int defaultMaskId,
        int? boardId = null)
    {
        var masks = repository.GetByBoardId(boardId);

        if (masks.Any(m => m.ID == defaultMaskId))
        {
            return defaultMaskId;
        }

        return masks.Where(m => !new AccessFlags(m.Flags).ModeratorAccess)
                   .OrderByDescending(m => System.Numerics.BitOperations.PopCount((uint)m.Flags))
                   .Select(m => m.ID)
                   .FirstOrDefault();
    }

    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="accessMaskId">The access mask id.</param>
    /// <param name="name">The name.</param>
    /// <param name="flags">The Access Mask Flags</param>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="boardId">The board id.</param>
    public async static Task SaveAsync(
        this IRepository<AccessMask> repository,
        int? accessMaskId,
        string name,
        AccessFlags flags,
        short sortOrder,
        int? boardId = null)
    {
        // an id of another board is not changed
        var saveBoardId = boardId ?? repository.BoardID;

        if (accessMaskId is > 0 && !await repository.ExistsAsync(x => x.ID == accessMaskId && x.BoardID == saveBoardId))
        {
            return;
        }

        await repository.UpsertAsync(
            new AccessMask
            {
                BoardID = saveBoardId,
                ID = accessMaskId ?? 0,
                Name = name,
                Flags = flags.BitValue,
                SortOrder = sortOrder
            });
    }
}