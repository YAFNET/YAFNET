/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System;
    using System.Collections.Generic;
    
    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects.Model;

    /// <summary>
    ///     The event log repository extensions.
    /// </summary>
    public static class EventLogRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The list.
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
        /// The <see cref="List"/>.
        /// </returns>
        public static List<PagedEventLog> ListPaged(
            this IRepository<EventLog> repository,
            [NotNull] int? boardId,
            [NotNull] int maxRows,
            [NotNull] int maxDays,
            [NotNull] int pageIndex,
            [NotNull] int pageSize,
            [NotNull] DateTime sinceDate,
            [NotNull] DateTime toDate,
            [CanBeNull] int? eventType,
            [NotNull] bool spamOnly = false)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DeleteOld(maxRows, maxDays);

            return repository.DbAccess.Execute(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<EventLog>();

                    expression.Join<User>((e, u) => u.ID == e.UserID);

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
                    var countTotalExpression = expression;

                    var countTotalSql = countTotalExpression
                        .Select(Sql.Count($"{countTotalExpression.Column<EventLog>(x => x.ID)}")).ToSelectStatement();

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
           [NotNull] int maxRows,
           [NotNull] int maxDays)
        {
            CodeContracts.VerifyNotNull(repository);

            // -- delete entries older than 10 days
            var agesAgo = DateTime.Today.AddDays(-maxDays);

            repository.Delete(x => x.EventTime < agesAgo);

            // -- or if there are more then 1000
            var total = repository.DbAccess.Execute(
                db =>
                {
                    var expression = db.Connection.From<EventLog>().Select(Sql.Count("1"));

                    return db.Connection.SqlScalar<int>(expression);
                });

            if (total >= maxRows + 50)
            {
                repository.DbAccess.Execute(
                    db =>
                    {
                        var expression = db.Connection.From<EventLog>(db.Connection.TableAlias("x"));

                        expression.UnsafeWhere(
                            $@"{expression.Column<EventLog>(x => x.ID, true)} in
                                        (select top 100 x.{expression.Column<EventLog>(x => x.ID)}
                                         from {expression.Table<EventLog>()} x 
                                         order by x.{expression.Column<EventLog>(x => x.EventTime)})");

                        return db.Connection.Delete(expression);
                    });
            }
        }

        #endregion
    }
}