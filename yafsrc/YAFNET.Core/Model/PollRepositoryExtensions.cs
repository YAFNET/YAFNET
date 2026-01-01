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

using YAF.Types.Models;

/// <summary>
///     The Poll repository extensions.
/// </summary>
public static class PollRepositoryExtensions
{
    /// <param name="repository">
    /// The repository.
    /// </param>
    extension(IRepository<Poll> repository)
    {
        /// <summary>
        /// Get the Poll with all Choices
        /// </summary>
        /// <param name="pollId">
        /// The poll id.
        /// </param>
        public List<Tuple<Poll, Choice>> GetPollAndChoices(int pollId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<Poll>();

            expression.Join<Choice>((p, c) => c.PollID == p.ID).Where<Poll>(p => p.ID == pollId);

            return repository.DbAccess.Execute(db => db.Connection.SelectMulti<Poll, Choice>(expression));
        }

        /// <summary>
        /// Remove Poll
        /// </summary>
        /// <param name="pollId">
        /// The poll Id.
        /// </param>
        public void Remove(int pollId)
        {
            // delete vote records first
            BoardContext.Current.GetRepository<PollVote>().Delete(p => p.PollID == pollId);

            // delete choices
            BoardContext.Current.GetRepository<Choice>().Delete(p => p.PollID == pollId);

            // update topics
            BoardContext.Current.GetRepository<Topic>().UpdateOnly(
                () => new Topic
                {
                    PollID = null
                },
                t => t.PollID == pollId);

            repository.DeleteById(pollId);
        }

        /// <summary>
        /// Update an existing Poll
        /// </summary>
        /// <param name="pollId">
        /// The poll id.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="closes">
        /// The closes.
        /// </param>
        /// <param name="isClosedBounded">
        /// The is closed bounded.
        /// </param>
        /// <param name="allowMultipleChoices">
        /// The allow multiple choices.
        /// </param>
        /// <param name="showVoters">
        /// The show voters.
        /// </param>
        /// <param name="questionPath">
        /// The question path.
        /// </param>
        public void Update(int pollId,
            string question,
            DateTime? closes,
            bool isClosedBounded,
            bool allowMultipleChoices,
            bool showVoters,
            string questionPath)
        {
            var flags = new PollFlags
            {
                IsClosedBound = isClosedBounded,
                AllowMultipleChoice = allowMultipleChoices,
                ShowVoters = showVoters,
                AllowSkipVote = false
            };

            repository.UpdateOnly(
                () => new Poll
                {
                    Question = question,
                    Closes = closes,
                    ObjectPath = questionPath,
                    Flags = flags.BitValue
                },
                p => p.ID == pollId);
        }

        /// <summary>
        /// Create new Poll
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="question">
        /// The question.
        /// </param>
        /// <param name="closes">
        /// The closes.
        /// </param>
        /// <param name="isClosedBounded">
        /// The is closed bounded.
        /// </param>
        /// <param name="allowMultipleChoices">
        /// The allow multiple choices.
        /// </param>
        /// <param name="showVoters">
        /// The show voters.
        /// </param>
        /// <param name="questionPath">
        /// The question path.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Create(int userId,
            string question,
            DateTime? closes,
            bool isClosedBounded,
            bool allowMultipleChoices,
            bool showVoters,
            string questionPath)
        {
            var flags = new PollFlags
            {
                IsClosedBound = isClosedBounded,
                AllowMultipleChoice = allowMultipleChoices,
                ShowVoters = showVoters,
                AllowSkipVote = false
            };

            return repository.Insert(
                new Poll
                {
                    UserID = userId,
                    Question = question,
                    Closes = closes,
                    ObjectPath = questionPath,
                    Flags = flags.BitValue
                });
        }
    }
}