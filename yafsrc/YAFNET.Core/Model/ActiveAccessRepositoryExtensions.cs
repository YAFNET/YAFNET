
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

#pragma warning disable S1125

/// <summary>
/// The active access repository extensions.
/// </summary>
public static class ActiveAccessRepositoryExtensions
{
    /// <summary>
    /// Sets the Page Access for the specified user
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board identifier.
    /// </param>
    /// <param name="userId">
    /// The user identifier.
    /// </param>
    /// <param name="isGuest">
    /// The is guest.
    /// </param>
    public static void InsertPageAccess(
        this IRepository<ActiveAccess> repository,
        int boardId,
        int userId,
        bool isGuest)
    {
        if (repository.Exists(a => a.UserID == userId))
        {
            return;
        }

        var accessList = BoardContext.Current.GetRepository<VAccess>().Get(x => x.UserID == userId)
            .DistinctBy(a => new { a.UserID, a.ForumID });

        var activeList = new List<ActiveAccess>();

        // -- update active access
        // -- ensure that access right are in place
        foreach (var access in accessList.Where(access =>
                     !activeList.Exists(x => x.UserID == access.UserID && x.ForumID == access.ForumID)))
        {
            activeList.Add(
                new ActiveAccess
                {
                    UserID = userId,
                    BoardID = boardId,
                    ForumID = access.ForumID,
                    IsAdmin = access.IsAdmin > 0,
                    IsForumModerator = access.IsForumModerator > 0,
                    IsModerator = access.IsModerator > 0,
                    IsGuestX = isGuest,
                    LastActive = DateTime.UtcNow,
                    ReadAccess = access.ReadAccess > 0,
                    PostAccess = access.PostAccess > 0,
                    ReplyAccess = access.ReplyAccess > 0,
                    PriorityAccess = access.PriorityAccess > 0,
                    PollAccess = access.PollAccess > 0,
                    VoteAccess = access.VoteAccess > 0,
                    ModeratorAccess = access.ModeratorAccess > 0,
                    EditAccess = access.EditAccess > 0,
                    DeleteAccess = access.DeleteAccess > 0
                });
        }

        repository.BulkInsert(activeList /*.DistinctBy(a => new { a.UserID, a.ForumID })*/);
    }

    /// <summary>
    /// Delete all old
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="activeTime">
    /// The active Time.
    /// </param>
    public static void Delete(this IRepository<ActiveAccess> repository, int activeTime)
    {
        repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<ActiveAccess>();

                    expression.Where(
                        $"{OrmLiteConfig.DialectProvider.DateDiffFunction(
                            "minute",
                            expression.Column<ActiveAccess>(x => x.LastActive, true),
                            OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {activeTime} ");

                    expression.And(x => x.IsGuestX == false);

                    return db.Connection.Delete(expression);
                });
    }
}