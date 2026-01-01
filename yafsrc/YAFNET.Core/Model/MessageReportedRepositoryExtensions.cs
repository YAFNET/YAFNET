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

using System;
using System.Collections.Generic;

using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
///     The MessageReported repository extensions.
/// </summary>
public static class MessageReportedRepositoryExtensions
{
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<MessageReported> repository)
    {
        /// <summary>
        /// Retrieve all reported messages with the correct forumID argument.
        /// </summary>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        public Task<List<ReportedMessage>> ListReportedAsync(int forumId)
        {
            return repository.DbAccess.ExecuteAsync(
                db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageReported>();

                    expression
                        .Join<Message>((a, m) => m.ID == a.ID)
                        .Join<Message, Topic>((m, t) => t.ID == m.TopicID)
                        .Join<Message, User>((m, u) => u.ID == m.UserID)
                        .Where<Message, Topic>(
                            (m, t) => t.ForumID == forumId && (m.Flags & 8) != 8 && (t.Flags & 8) != 8 &&
                                      (m.Flags & 128) == 128);

                    var q = db.From<MessageReportedAudit>(db.TableAlias("x"));
                    q.Where(
                        $"x.{q.Column<MessageReportedAudit>(a => a.MessageID)}={expression.Column<MessageReported>(a => a.ID, true)}");
                    var subSql = q.Select(Sql.Count($"{q.Column<MessageReportedAudit>(a => a.LogID)}"))
                        .ToSelectStatement();

                    expression.Select<MessageReported, MessageReportedAudit, Message, Topic, User>(
                        (a, b, m, t, u) => new
                        {
                            MessageID = a.ID,
                            a.Message,
                            a.ResolvedBy,
                            a.ResolvedDate,
                            a.Resolved,
                            OriginalMessage = m.MessageText,
                            m.Flags,
                            m.IsModeratorChanged,
                            UserName = m.UserName ?? u.Name,
                            UserDisplayName = m.UserDisplayName ?? u.DisplayName,
                            m.UserID,
                            u.Suspended,
                            u.UserStyle,
                            m.Posted,
                            m.TopicID,
                            t.TopicName,
                            NumberOfReports = Sql.Custom($"({subSql})")
                        });

                    return db.SelectAsync<ReportedMessage>(expression);
                });
        }

        /// <summary>
        /// Save reported message back to the database.
        /// </summary>
        /// <param name="message">
        /// The Message
        /// </param>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="reportedDateTime">
        /// The reported date time.
        /// </param>
        /// <param name="reportText">
        /// The report text.
        /// </param>
        public async Task ReportAsync(Message message,
            int userId,
            DateTime reportedDateTime,
            string reportText)
        {
            reportText ??= string.Empty;

            if (!await repository.ExistsAsync(m => m.ID == message.ID))
            {
                await repository.InsertAsync(new MessageReported { ID = message.ID, Message = message.MessageText });
            }

            var reportAudit = await BoardContext.Current.GetRepository<MessageReportedAudit>()
                .GetSingleAsync(m => m.MessageID == message.ID && m.UserID == userId);

            if (reportAudit == null)
            {
                await BoardContext.Current.GetRepository<MessageReportedAudit>().InsertAsync(
                    new MessageReportedAudit
                    {
                        MessageID = message.ID,
                        UserID = userId,
                        Reported = reportedDateTime,
                        ReportText = $"{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}??{reportText}"
                    });
            }
            else
            {
                await BoardContext.Current.GetRepository<MessageReportedAudit>().UpdateOnlyAsync(
                    () => new MessageReportedAudit
                    {
                        ReportedNumber = reportAudit.ReportedNumber < 2147483647 ? reportAudit.ReportedNumber + 1 : reportAudit.ReportedNumber,
                        Reported = reportedDateTime,
                        ReportText = $"{reportAudit.ReportText}|{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}??{reportText}"
                    },
                    m => m.MessageID == message.ID && m.UserID == userId);
            }

            var flags = message.MessageFlags;

            flags.IsReported = true;

            await BoardContext.Current.GetRepository<Message>().UpdateFlagsAsync(message.ID, flags.BitValue);
        }

        /// <summary>
        /// Copy current Message text over reported Message text.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public Task ReportCopyOverAsync(Message message)
        {
            return repository.UpdateOnlyAsync(() => new MessageReported { Message = message.MessageText }, m => m.ID == message.ID);
        }
    }
}