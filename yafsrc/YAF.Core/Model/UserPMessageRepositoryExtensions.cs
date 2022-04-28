/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The UserPMessage Repository Extensions
/// </summary>
public static class UserPMessageRepositoryExtensions
{
    #region Public Methods and Operators

    /// <summary>
    /// Mark Private Message as read.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="messageFlags">
    /// The message Flags.
    /// </param>
    public static void MarkAsRead(
        this IRepository<UserPMessage> repository,
        [NotNull] int messageId,
        [NotNull] PMessageFlags messageFlags)
    {
        CodeContracts.VerifyNotNull(repository);

        if (messageFlags.IsRead)
        {
            return;
        }

        messageFlags.IsRead = true;

        repository.UpdateOnly(() => new UserPMessage { Flags = messageFlags.BitValue }, m => m.PMessageID == messageId);
    }

    /// <summary>
    /// archive message.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="messageFlags">
    /// The message Flags.
    /// </param>
    public static void Archive(
        this IRepository<UserPMessage> repository,
        [NotNull] int messageId,
        [NotNull] PMessageFlags messageFlags)
    {
        CodeContracts.VerifyNotNull(repository);

        messageFlags.IsArchived = true;

        repository.UpdateOnly(() => new UserPMessage { Flags = messageFlags.BitValue }, m => m.ID == messageId);
    }

    /// <summary>
    /// Get Messages by To User Id
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="view">
    /// The view.
    /// </param>
    /// <returns>
    /// The <see cref="List"/>.
    /// </returns>
    public static List<UserPMessage> List(
        this IRepository<UserPMessage> repository,
        [NotNull] int userId,
        [NotNull] PmView view)
    {
        CodeContracts.VerifyNotNull(repository);

        return view == PmView.Archive
                   ? repository.Get(p => p.UserID == userId && (p.Flags & 1) != 1 && (p.Flags & 4) == 4)
                   : repository.Get(
                       p => p.UserID == userId && (p.Flags & 1) != 1 && (p.Flags & 8) != 8 && (p.Flags & 4) != 4);
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
    /// The delete From Outbox.
    /// </param>
    public static void Delete(
        this IRepository<UserPMessage> repository,
        [NotNull] int userPmMessageId,
        [NotNull] bool deleteFromOutbox)
    {
        CodeContracts.VerifyNotNull(repository);

        var message = repository.GetById(userPmMessageId);

        var flags = message.PMessageFlags;

        /*if (deleteFromOutbox && message.PMessageFlags.IsInOutbox)
        {
            // -- remove IsInOutbox bit which will remove it from the senders outbox
            flags.IsInOutbox = false;

            repository.UpdateOnly(() => new UserPMessage { Flags = flags.BitValue }, x => x.ID == userPmMessageId);
        }
        else
        {
            if (message.PMessageFlags.IsInOutbox && message.PMessageFlags.IsArchived && !message.PMessageFlags.IsDeleted)
            {
                // -- The message is in archive but still is in sender outbox
                flags.IsInOutbox = false;
                flags.IsArchived = false;
                flags.IsDeleted = true;

                repository.UpdateOnly(() => new UserPMessage { Flags = flags.BitValue }, x => x.ID == userPmMessageId);
            }
        }*/

        flags.IsInOutbox = false;
        flags.IsArchived = false;
        flags.IsDeleted = true;

        repository.UpdateOnly(() => new UserPMessage { Flags = flags.BitValue }, x => x.ID == userPmMessageId);

        // -- see if there are no longer references to this PM.
        if (!repository.Exists(p => p.ID == userPmMessageId && (p.Flags & 2) != 2 && (p.Flags & 8) == 8))
        {
            return;
        }

        repository.Delete(p => p.PMessageID == message.PMessageID);
        BoardContext.Current.GetRepository<PMessage>().DeleteById(message.PMessageID);
    }

    #endregion
}