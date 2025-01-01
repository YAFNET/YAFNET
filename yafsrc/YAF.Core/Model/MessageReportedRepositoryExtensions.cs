﻿/* Yet Another Forum.NET
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
///     The MessageReported repository extensions.
/// </summary>
public static class MessageReportedRepositoryExtensions
{
    /// <summary>
    /// Retrieve all reported messages with the correct forumID argument.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="forumId">
    /// The forum Id.
    /// </param>
    public static
        List<ReportedMessage> ListReported(this IRepository<MessageReported> repository, int forumId)
    {
        return repository.DbAccess.Execute(
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

                var q = db.Connection.From<MessageReportedAudit>(db.Connection.TableAlias("x"));
                q.Where(
                    $"x.{q.Column<MessageReportedAudit>(a => a.MessageID)}={expression.Column<MessageReported>(a => a.ID, true)}");
                var subSql = q.Select(Sql.Count($"{q.Column<MessageReportedAudit>(a => a.LogID)}"))
                    .ToSelectStatement();

                expression.Select<MessageReported, MessageReportedAudit, Message, Topic, User>(
                    (a, b, m, t, u) => new {
                        MessageID = a.ID,
                        a.Message,
                        a.ResolvedBy,
                        a.ResolvedDate,
                        a.Resolved,
                        OriginalMessage = m.MessageText,
                        m.Flags,
                        m.IsModeratorChanged,
                        UserName = m.UserName == null ? u.Name : m.UserName,
                        UserDisplayName = m.UserDisplayName == null ? u.DisplayName : m.UserDisplayName,
                        m.UserID,
                        u.Suspended,
                        u.UserStyle,
                        m.Posted,
                        m.TopicID,
                        t.TopicName,
                        NumberOfReports = Sql.Custom($"({subSql})")
                    });

                return db.Connection
                    .Select<ReportedMessage>(expression);
            });
    }

    /// <summary>
    /// Save reported message back to the database.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
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
    public static void Report(
        this IRepository<MessageReported> repository,
        Tuple<Topic, Message, User, Forum> message,
        int userId,
        DateTime reportedDateTime,
        string reportText)
    {
        reportText ??= string.Empty;

        if (!repository.Exists(m => m.ID == message.Item2.ID))
        {
            repository.Insert(new MessageReported { ID = message.Item2.ID, Message = message.Item2.MessageText });
        }

        var reportAudit = BoardContext.Current.GetRepository<MessageReportedAudit>()
            .GetSingle(m => m.MessageID == message.Item2.ID && m.UserID == userId);

        if (reportAudit == null)
        {
            BoardContext.Current.GetRepository<MessageReportedAudit>().Insert(
                new MessageReportedAudit {
                    MessageID = message.Item2.ID,
                    UserID = userId,
                    Reported = reportedDateTime,
                    ReportText = $"{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}??{reportText}"
                });
        }
        else
        {
            BoardContext.Current.GetRepository<MessageReportedAudit>().UpdateOnly(
                () => new MessageReportedAudit {
                    ReportedNumber = reportAudit.ReportedNumber < 2147483647
                        ? reportAudit.ReportedNumber + 1
                        : reportAudit.ReportedNumber,
                    Reported = reportedDateTime,
                    ReportText = $"{reportAudit.ReportText}|{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}??{reportText}"
                },
                m => m.MessageID == message.Item2.ID && m.UserID == userId);
        }

        var flags = message.Item2.MessageFlags;

        flags.IsReported = true;

        BoardContext.Current.GetRepository<Message>().UpdateFlags(message.Item2.ID, flags.BitValue);
    }

    /// <summary>
    /// Copy current Message text over reported Message text.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public static void ReportCopyOver(this IRepository<MessageReported> repository, Message message)
    {
        repository.UpdateOnly(() => new MessageReported { Message = message.MessageText }, m => m.ID == message.ID);
    }
}