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
/// The PollVote Repository Extensions
/// </summary>
public static class PollVoteRepositoryExtensions
{
    /// <summary>
    /// Checks for a vote in the database
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="pollId">
    /// The poll Id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    public static List<PollVote> VoteCheck(
        this IRepository<PollVote> repository,
        int pollId,
        int userId)
    {
        return repository.Get(p => p.UserID == userId && p.PollID == pollId);
    }

    /// <summary>
    /// Get all Voters for the Current Poll
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    public static List<Tuple<PollVote, User>> Voters(
        this IRepository<PollVote> repository,
        int pollId)
    {
        var expression = OrmLiteConfig.DialectProvider.SqlExpression<PollVote>();

        expression.Join<User>((p, u) => u.ID == p.UserID).Where<PollVote>(p => p.PollID == pollId);

        return repository.DbAccess.Execute(db => db.Connection.SelectMulti<PollVote, User>(expression));
    }

    /// <summary>
    /// Adds a Vote
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="choiceId">
    /// The choice id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="pollId">
    /// The poll id.
    /// </param>
    public static void Vote(
        this IRepository<PollVote> repository,
        int choiceId,
        int userId,
        int pollId)
    {
        var entity = new PollVote { PollID = pollId, UserID = userId, ChoiceID = choiceId };

        repository.Insert(entity);
    }
}