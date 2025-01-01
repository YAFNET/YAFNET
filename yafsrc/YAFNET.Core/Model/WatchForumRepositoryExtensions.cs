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
/// The WatchForum Repository Extensions
/// </summary>
public static class WatchForumRepositoryExtensions
{
    /// <summary>
    /// Add a new WatchForum
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
    public static void Add(this IRepository<WatchForum> repository, int userId, int forumId)
    {
        var watchForum = new WatchForum { ForumID = forumId, UserID = userId, Created = DateTime.UtcNow };

        repository.Insert(watchForum);

        repository.FireNew(watchForum);
    }

    /// <summary>
    /// Checks if Watch Forum Exists and Returns WatchForum ID
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
    /// The <see cref="int"/>.
    /// </returns>
    public static int? Check(this IRepository<WatchForum> repository, int userId, int forumId)
    {
        var forum = repository.GetSingle(w => w.UserID == userId && w.ForumID == forumId);

        return forum?.ID;
    }

    /// <summary>
    /// List all Watch Forums by User
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pageIndex">
    /// The page Index.
    /// </param>
    /// <param name="pageSize">
    /// The page Size.
    /// </param>
    public static List<WatchForum> List(
        this IRepository<WatchForum> repository,
        int userId,
        int pageIndex = 0,
        int pageSize = 10000000)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<WatchForum>();

        expression.Join<Forum>((a, b) => b.ID == a.ForumID).Where<WatchForum>(b => b.UserID == userId)
            .OrderByDescending(item => item.ID).Page(pageIndex + 1, pageSize);

        return repository.DbAccess.Execute(db => db.Connection.LoadSelect(expression));
    }
}