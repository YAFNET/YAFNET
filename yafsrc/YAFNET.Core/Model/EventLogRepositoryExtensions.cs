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
using YAF.Types.Objects.Model;

/// <summary>
///     The event log repository extensions.
/// </summary>
public static class EventLogRepositoryExtensions
{
    /// <summary>
    /// Gets the Event Log (Paged)
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="maxRows">
    /// The max rows.
    /// </param>
    /// <param name="maxDays">
    /// The max days.
    /// </param>
    /// <param name="pageIndex">
    /// The page index.
    /// </param>
    /// <param name="pageSize">
    /// The page size.
    /// </param>
    /// <param name="sinceDate">
    /// The since date.
    /// </param>
    /// <param name="toDate">
    /// The to date.
    /// </param>
    /// <param name="eventType">
    /// The event Type.
    /// </param>
    /// <param name="spamOnly">
    /// The spam Only.
    /// </param>
    /// <returns>
    /// Returns a Paged List of the Event Log
    /// </returns>
    public static List<PagedEventLog> ListPaged(
        this IRepository<EventLog> repository,
        int? boardId,
        int maxRows,
        int maxDays,
        int pageIndex,
        int pageSize,
        DateTime sinceDate,
        DateTime toDate,
        int? eventType,
        bool spamOnly = false)
    {
        repository.DeleteOld(maxRows, maxDays);

        return repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<EventLog>();

                    var guestUserId = BoardContext.Current.GuestUserID;

                    expression.Join<User>((e, u) => u.ID == (e.UserID ?? guestUserId));

                    expression.Where<EventLog, User>(
                        (a, b) => b.BoardID == (boardId ?? repository.BoardID) && a.EventTime >= sinceDate &&
                                  a.EventTime <= toDate);

                    if (eventType.HasValue)
                    {
                        expression.And(a => a.Type == eventType);
                    }

                    if (spamOnly)
                    {
                        expression.And(
                            a => a.Type == 1003 || a.Type == 1004 || a.Type == 1005 || a.Type == 2000 ||
                                 a.Type == 2001 || a.Type == 2002 || a.Type == 2003);
                    }

                    // -- count total
                    var countTotalSql = expression
                        .Select(Sql.Count($"{expression.Column<EventLog>(x => x.ID)}")).ToSelectStatement();

                    expression.Select<EventLog, User>(
                        (a, b) => new
                                      {
                                          a.UserID,
                                          a.ID,
                                          a.EventTime,
                                          a.Source,
                                          a.Description,
                                          a.Type,
                                          b.Name,
                                          b.DisplayName,
                                          b.Suspended,
                                          b.UserStyle,
                                          b.Flags,
                                          TotalRows = Sql.Custom($"({countTotalSql})")
                                      });

                    expression.OrderByDescending(a => a.ID);

                    // Set Paging
                    expression.Page(pageIndex + 1, pageSize);

                    return db.Connection.Select<PagedEventLog>(expression);
                });
    }

    /// <summary>
    /// Delete Old Entries
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="maxRows">
    /// The max rows.
    /// </param>
    /// <param name="maxDays">
    /// The max days.
    /// </param>
    public static void DeleteOld(
        this IRepository<EventLog> repository,
        int maxRows,
        int maxDays)
    {
        // -- delete entries older than 10 days
        var agesAgo = DateTime.Today.AddDays(-maxDays);

        repository.Delete(x => x.EventTime < agesAgo);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<EventLog>();

        expression.OrderBy(x => x.EventTime).Limit(100);

        var entries = repository.DbAccess.Execute(db => db.Connection.Select(expression));

        // -- or if there are more then 1000
        if (entries.Count >= maxRows + 50)
        {
            repository.DbAccess.Execute(db => db.Connection.DeleteAll(entries));
        }
    }
}