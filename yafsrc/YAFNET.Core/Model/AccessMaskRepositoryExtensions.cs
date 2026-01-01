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

using YAF.Types.Models;

/// <summary>
/// The access mask repository extensions.
/// </summary>
public static class AccessMaskRepositoryExtensions
{
    /// <summary>
    /// The save.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="accessMaskId">The access mask id.</param>
    /// <param name="name">The name.</param>
    /// <param name="flags">The Access Mask Flags</param>
    /// <param name="sortOrder">The sort order.</param>
    /// <param name="boardId">The board id.</param>
    public static Task SaveAsync(
        this IRepository<AccessMask> repository,
        int? accessMaskId,
        string name,
        AccessFlags flags,
        short sortOrder,
        int? boardId = null)
    {
        return repository.UpsertAsync(
            new AccessMask
            {
                BoardID = boardId ?? repository.BoardID,
                ID = accessMaskId ?? 0,
                Name = name,
                Flags = flags.BitValue,
                SortOrder = sortOrder
            });
    }
}