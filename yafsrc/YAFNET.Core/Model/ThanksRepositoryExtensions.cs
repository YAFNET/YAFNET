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

/// <summary>
///     The Thanks repository extensions.
/// </summary>
public static class ThanksRepositoryExtensions
{
    /// <summary>
    /// The thanks from user.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="thanksFromUserId">
    /// The thanks from user id.
    /// </param>
    /// <returns>
    /// The <see cref="long"/>.
    /// </returns>
    public static long ThanksFromUser(this IRepository<Thanks> repository, int thanksFromUserId)
    {
        return repository.Count(thanks => thanks.ThanksFromUserID == thanksFromUserId);
    }

    /// <summary>
    /// Gets the number of times and posts that other users have thanked the
    /// user with the provided userID.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="thanksToUserId">
    /// The thanks To User Id.
    /// </param>
    /// <returns>
    /// Returns the number of times and posts that other users have thanked the
    /// user with the provided userID.
    /// </returns>
    public static (int Posts, string ThanksReceived) ThanksToUser(
        this IRepository<Thanks> repository,
        int thanksToUserId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Thanks>();

        expression.Where<Thanks>(t => t.ThanksToUserID == thanksToUserId).Select(
            u => new { ThankesPosts = Sql.CountDistinct(u.MessageID), ThankesReceived = Sql.Count("*") });

        return repository.DbAccess
            .Execute(db => db.Connection.Single<(int Posts, string ThanksReceived)>(expression));
    }

    /// <summary>
    /// Add thanks to the Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public static void AddMessageThanks(
        this IRepository<Thanks> repository,
        int fromUserId,
        int toUserId,
        int messageId)
    {
        repository.Insert(
            new Thanks
            {
                ThanksFromUserID = fromUserId,
                ThanksToUserID = toUserId,
                MessageID = messageId,
                ThanksDate = DateTime.UtcNow
            });
    }

    /// <summary>
    /// Gets the UserIDs and UserNames who have thanked the message
    ///   with the provided messageID.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    /// <returns>
    /// Returns the UserIDs and UserNames who have thanked the message
    ///   with the provided messageID.
    /// </returns>
    public static List<Tuple<Thanks, User>> MessageGetThanksList(
        this IRepository<Thanks> repository,
        int messageId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Thanks>();

        expression.Join<User>((a, b) => a.ThanksFromUserID == b.ID).Where<Thanks>(b => b.MessageID == messageId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Thanks, User>(expression));
    }

    /// <summary>
    /// The message remove thanks.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    /// <param name="messageId">
    /// The message id.
    /// </param>
    public static void RemoveMessageThanks(
        this IRepository<Thanks> repository,
        int fromUserId,
        int messageId)
    {
        repository.Delete(t => t.ThanksFromUserID == fromUserId && t.MessageID == messageId);
    }

    /// <summary>
    /// Has User Thanked the current Message
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="messageId">
    /// The message Id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <returns>
    /// If the User Thanked the Current Message
    /// </returns>
    public static bool ThankedMessage(
        this IRepository<Thanks> repository,
        int messageId,
        int userId)
    {
        var thankCount = repository.Count(t => t.MessageID == messageId && t.ThanksFromUserID == userId);

        return thankCount > 0;
    }
}