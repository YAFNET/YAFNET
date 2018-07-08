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
    #region Using

    using System.Collections.Generic;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;

    #endregion

    /// <summary>
    ///     The Topic repository extensions.
    /// </summary>
    public static class TopicRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the similar topics.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="searchInput">The search input.</param>
        /// <returns>Get List of similar topics</returns>
        public static List<SearchMessage> GetSimilarTopics(this IRepository<Topic> repository, [NotNull] int userId, [NotNull] string searchInput)
        {
            return YafContext.Current.Get<ISearch>().SearchSimilar(userId, searchInput, "Topic");
        }
        
        /// <summary>
        /// Sets the answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="messageId">The message identifier.</param>
        public static void SetAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int messageId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = messageId }, where: t => t.ID == topicId);
        }

        /// <summary>
        /// Removes answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        public static void RemoveAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { AnswerMessageId = null }, where: t => t.ID == topicId);
        }

        /// <summary>
        /// Gets the answer message.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <returns>Returns the Answer Message identifier</returns>
        public static int? GetAnswerMessage(this IRepository<Topic> repository, [NotNull] int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.GetSingle(t => t.ID == topicId);

            return topic?.AnswerMessageId;
        }

        /// <summary>
        /// Lock's/Unlock's the topic.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <param name="flags">The topic flags.</param>
        public static void LockTopic(this IRepository<Topic> repository, [NotNull] int topicId, [NotNull] int flags)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new Topic { Flags = flags }, where: t => t.ID == topicId);
        }

        #endregion
    }
}