/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

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
    using System.Data;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    /// <summary>
    ///     The Message repository extensions.
    /// </summary>
    public static class MessageRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets all messages by board as Typed Search Message List.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns>Returns all Messages as Typed Search Message List</returns>
        public static IEnumerable<SearchMessage> GetAllMessagesByBoard(this IRepository<Message> repository, int? boardId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetAsDataTable(cdb => cdb.message_list_search(BoardID: boardId ?? repository.BoardID))
                .SelectTypedList(t => new SearchMessage(t));
        }

        /// <summary>
        /// A list of messages.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>Returns Typed Message List</returns>
        public static IList<Message> ListTyped(this IRepository<Message> repository, int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.SqlList(
                "message_list",
                new
                    {
                       MessageID = messageId
                });
        }

        /// <summary>
        /// Get the replies message(s)
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <returns>Returns Typed Message List</returns>
        public static DataTable GetReplies(this IRepository<Message> repository, int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.message_getReplies(MessageID: messageId);
        }

        /// <summary>
        /// Saves the specified message identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="message">The message.</param>
        /// <param name="guestUserName">Name of the guest user.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="posted">The posted.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <param name="blogPostId">The blog post identifier.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>Returns the new message identifier</returns>
        public static int Save(
            this IRepository<Medal> repository,
            int topicId,
            int userId,
            string message,
            string guestUserName,
            string ip,
            DateTime posted,
            int replyTo,
            int blogPostId,
            int flags)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var messageId = (int)repository.DbFunction.Scalar.message_save(
                TopicID: topicId,
                UserID: userId,
                Message: message,
                UserName: guestUserName,
                IP: ip,
                Posted: posted,
                ReplyTo: replyTo,
                BlogPostID: null,
                Flags: flags,
                UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew(messageId);

            return messageId;
        }

        /// <summary>
        /// Approves the message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void ApproveMessage(this IRepository<Message> repository, int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.message_approve(MessageID: messageId);
        }

        /// <summary>
        /// Updates the flags.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="messageId">The message identifier.</param>
        /// <param name="flags">The flags.</param>
        public static void UpdateFlags(this IRepository<Message> repository, int messageId, int flags)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Message { Flags = flags }, where: u => u.ID == messageId);
        }

        #endregion
    }
}