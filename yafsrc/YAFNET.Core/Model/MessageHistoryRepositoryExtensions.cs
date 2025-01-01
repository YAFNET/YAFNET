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

using System.Collections.Generic;

using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
///     The MessageHistory repository extensions.
/// </summary>
public static class MessageHistoryRepositoryExtensions
{
    /// <summary>
    /// Gets the List of all message changes.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The Message ID.
    /// </param>
    /// <param name="daysToClean">
    /// Days to clean.
    /// </param>
    /// <returns>
    /// Returns the List of all message changes.
    /// </returns>
    public static List<MessageHistoryTopic> List(
        this IRepository<MessageHistory> repository,
        int messageId,
        int daysToClean)
    {
        repository.DeleteOlderThen(daysToClean);

        var list = repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageHistory>();

                    expression.LeftJoin<Message>((mh, m) => mh.MessageID == m.ID)
                        .LeftJoin<Message, Topic>((m, t) => t.ID == m.TopicID)
                        .LeftJoin<MessageHistory, User>((mh, u) => u.ID == mh.EditedBy)
                        .Where<MessageHistory>(mh => mh.MessageID == messageId).OrderBy(mh => mh.Edited)
                        .ThenBy(mh => mh.MessageID).Select<MessageHistory, Message, Topic, User>(
                            (mh, m, t, u) => new
                                                 {
                                                     mh.EditReason,
                                                     mh.Edited,
                                                     mh.EditedBy,
                                                     mh.Flags,
                                                     mh.IP,
                                                     mh.IsModeratorChanged,
                                                     mh.MessageID,
                                                     mh.Message,
                                                     u.Name,
                                                     u.DisplayName,
                                                     u.UserStyle,
                                                     u.Suspended,
                                                     t.ForumID,
                                                     TopicID = t.ID,
                                                     Topic = t.TopicName,
                                                     m.Posted,
                                                     MessageIP = m.IP
                                                 });

                    return db.Connection
                        .Select<MessageHistoryTopic>(expression);
                });

        // Load Current Message
        var currentMessage = BoardContext.Current.GetRepository<Message>().GetMessageAsTuple(messageId);

        var current = new MessageHistoryTopic
                          {
                              EditReason = currentMessage.Item2.EditReason,
                              Edited = currentMessage.Item2.Edited.Value,
                              EditedBy = currentMessage.Item2.UserID,
                              Flags = currentMessage.Item2.Flags,
                              IP = currentMessage.Item2.IP,
                              IsModeratorChanged = currentMessage.Item2.IsModeratorChanged,
                              MessageID = currentMessage.Item2.ID,
                              Message = currentMessage.Item2.MessageText,
                              Name = currentMessage.Item3.Name,
                              DisplayName = currentMessage.Item3.DisplayName,
                              UserStyle = currentMessage.Item3.UserStyle,
                              Suspended = currentMessage.Item3.Suspended,
                              ForumID = currentMessage.Item1.ForumID,
                              TopicID = currentMessage.Item1.ID,
                              Topic = currentMessage.Item1.TopicName,
                              Posted = currentMessage.Item2.Posted,
                              MessageIP = currentMessage.Item2.IP
                          };

        list.Add(current);

        return list;
    }

    /// <summary>
    /// Delete all message variants older then DaysToClean
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="daysToClean">
    /// The days to clean.
    /// </param>
    private static void DeleteOlderThen(this IRepository<MessageHistory> repository, int daysToClean)
    {
        repository.DbAccess.Execute(
            db =>
                {
                    var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageHistory>();

                    expression.Where(
                        $"{OrmLiteConfig.DialectProvider.DateDiffFunction("day", expression.Column<MessageHistory>(x => x.Edited, true), OrmLiteConfig.DialectProvider.GetUtcDateFunction())} > {daysToClean}");

                    return db.Connection.Delete(expression);
                });
    }
}