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

namespace YAF.Core.Model;

using System;
using System.Collections.Generic;

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The UserPMessage Repository Extensions
/// </summary>
public static class UserPMessageRepositoryExtensions
{
    /// <summary>
    /// Mark the Private Message of a recipient as read.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userPMessageId">
    /// The id of the message of the recipient (not of the message, a message to all users has one per user).
    /// </param>
    /// <param name="userMessageFlags">
    /// The flags of the message of the recipient (read, outbox, deleted).
    /// </param>
    public static void MarkAsRead(
        this IRepository<UserPMessage> repository,
        int userPMessageId,
        PMessageFlags userMessageFlags)
    {
        if (userMessageFlags.IsRead)
        {
            return;
        }

        userMessageFlags.IsRead = true;

        repository.UpdateOnly(() => new UserPMessage { Flags = userMessageFlags.BitValue }, m => m.ID == userPMessageId);
    }

    /// <summary>
     /// Mark Private Message as read.
     /// </summary>
     /// <param name="repository">
     /// The repository.
     /// </param>
     /// <param name="message">
     /// The message.
     /// </param>
    public static void MarkAsRead(
        this IRepository<UserPMessage> repository,
        UserPMessage message)
    {
        

        var flags = message.PMessageFlags;

        if (flags.IsRead)
        {
            return;
        }

        flags.IsRead = true;

        repository.UpdateOnly(() => new UserPMessage {Flags = flags.BitValue}, m => m.ID == message.ID);
    }

    public static List<UserPMessage> List(
        this IRepository<UserPMessage> repository,
        int userId,
        PmView view)
    {
        switch (view)
        {
            case PmView.Outbox:
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserPMessage>();

                expression.Join<PMessage>((a, b) => a.PMessageID == b.ID)
                    .Where<UserPMessage, PMessage>((a, b) => b.FromUserID == userId && (a.Flags & 2) == 2);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
            case PmView.Inbox:
            {
                var expression = OrmLiteConfig.DialectProvider.SqlExpression<UserPMessage>();

                expression.Join<PMessage>((a, b) => a.PMessageID == b.ID)
                    .Where<UserPMessage, PMessage>((a, b) => a.UserID == userId && (a.Flags & 8) != 8);

                return repository.DbAccess.Execute(db => db.Connection.Select(expression));
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(view), view, null);
        }
    }

    /// <summary>
    /// Deletes the Private Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userPmMessageId">
    /// The user Pm Message Id.
    /// </param>
    /// <param name="deleteFromOutbox">
    /// Delete From Outbox?
    /// </param>
    public static long Delete(
        this IRepository<UserPMessage> repository,
        int userPmMessageId,
        bool deleteFromOutbox)
    {
        

        var message = repository.GetById(userPmMessageId);

        return Delete(repository, message, deleteFromOutbox);
    }

    /// <summary>
    /// Deletes the Private Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="message">
    /// The user Pm Message.
    /// </param>
    /// <param name="deleteFromOutbox">
    /// Delete From Outbox?
    /// </param>
    public static int Delete(
        this IRepository<UserPMessage> repository,
        UserPMessage message,
        bool deleteFromOutbox)
    {
        

        var flags = message.PMessageFlags;

        if (deleteFromOutbox)
        {
            // -- remove IsInOutbox bit which will remove it from the senders outbox
            flags.IsInOutbox = false;
        }
        else
        {
            // -- set is deleted, which removes it from the inbox of the recipient
            flags.IsDeleted = true;
        }

        repository.UpdateOnly(() => new UserPMessage { Flags = flags.BitValue }, x => x.ID == message.ID);

        // -- the sender and the recipient deleted it
        if (flags.IsInOutbox || !flags.IsDeleted)
        {
            return 0;
        }

        var deleteCount = repository.DeleteById(message.ID) ? 1 : 0;

        // -- see if there are no longer references to this PM (a message to all users has a recipient for each user)
        if (!repository.Exists(p => p.PMessageID == message.PMessageID))
        {
            BoardContext.Current.GetRepository<PMessage>().DeleteById(message.PMessageID);
        }

        return deleteCount;
    }
}