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

using YAF.Types.Models;

/// <summary>
/// The ForumReadTracking Repository Extensions
/// </summary>
public static class ForumReadTrackingRepositoryExtensions
{
    /// <summary>
    /// The add or update.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    public static void AddOrUpdate(
        this IRepository<ForumReadTracking> repository,
        int userId,
        int forumId)
    {
        var item = repository.GetSingle(x => x.ForumID == forumId && x.UserID == userId);

        if (item != null)
        {
            repository.UpdateOnly(
                () => new ForumReadTracking { LastAccessDate = DateTime.UtcNow },
                x => x.LastAccessDate == item.LastAccessDate && x.ForumID == forumId && x.UserID == userId);
        }
        else
        {
            repository.Insert(
                new ForumReadTracking { UserID = userId, ForumID = forumId, LastAccessDate = DateTime.UtcNow });
        }

        // -- Delete TopicReadTracking for forum...
        // Remark : not needed ?!
        // BoardContext.Current.GetRepository<TopicReadTracking>().Delete(x => x.UserID == userId)
    }

    /// <summary>
    /// The delete.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public static bool Delete(this IRepository<ForumReadTracking> repository, int userId)
    {
        var success = repository.Delete(x => x.UserID == userId) == 1;

        if (success)
        {
            repository.FireDeleted();
        }

        return success;
    }

    /// <summary>
    /// The last read.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <returns>
    /// The <see cref="DateTime"/>.
    /// </returns>
    public static DateTime? LastRead(
        this IRepository<ForumReadTracking> repository,
        int userId,
        int forumId)
    {
        var forum = repository.GetSingle(t => t.UserID == userId && t.ForumID == forumId);

        return forum?.LastAccessDate;
    }
}