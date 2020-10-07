/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Collections.Generic;
    using System.Dynamic;
    
    using ServiceStack.OrmLite;

    using YAF.Core.Context;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The MessageHistory repository extensions.
    /// </summary>
    public static class MessageHistoryRepositoryExtensions
    {
        #region Public Methods and Operators

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
        public static List<dynamic> List(this IRepository<MessageHistory> repository, int messageId, int daysToClean)
        {
            CodeContracts.VerifyNotNull(repository);

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

                    return db.Connection.Select<dynamic>(expression);
                });

            // Load Current Message
            var currentMessage = BoardContext.Current.GetRepository<Message>().GetMessage(messageId);

            dynamic current = new ExpandoObject();

            current.EditReason = currentMessage.Item2.EditReason;
            current.Edited = currentMessage.Item2.Posted;
            current.EditedBy = currentMessage.Item2.UserID;
            current.Flags = currentMessage.Item2.Flags;
            current.IP = currentMessage.Item2.IP;
            current.IsModeratorChanged = currentMessage.Item2.IsModeratorChanged;
            current.MessageID = currentMessage.Item2.ID;
            current.Message = currentMessage.Item2.MessageText;
            current.DisplayName = currentMessage.Item3.Name;
            current.DisplayName = currentMessage.Item3.DisplayName;
            current.UserStyle = currentMessage.Item3.UserStyle;
            current.Suspended = currentMessage.Item3.Suspended;
            current.ForumID = currentMessage.Item1.ForumID;
            current.TopicID = currentMessage.Item1.ID;
            current.Topic = currentMessage.Item1.TopicName;
            current.Posted = currentMessage.Item2.Posted;
            current.MessageIP = currentMessage.Item2.IP;

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
        private static void DeleteOlderThen(
            this IRepository<MessageHistory> repository,
            [NotNull] int daysToClean)
        {
            CodeContracts.VerifyNotNull(repository);

            repository.DbAccess.Execute(db =>
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<MessageHistory>();

                expression.Where(
                    $"DATEDIFF(day, {expression.Column<MessageHistory>(x => x.Edited, true)}, GETUTCDATE()) > {daysToClean}");

                return db.Connection.Delete(expression);
            });
        }

        #endregion
    }
}