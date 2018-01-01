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
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The TopicStatus repository extensions.
    /// </summary>
    public static class TopicStatusRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Add Or Update the entity
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicStatusId">The topic status identifier.</param>
        /// <param name="topicStatusName">Name of the topic status.</param>
        /// <param name="defaultDescription">The default description.</param>
        /// <param name="boardId">The board identifier.</param>
        public static void Save(
            this IRepository<TopicStatus> repository,
            int? topicStatusId,
            string topicStatusName,
            string defaultDescription,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.Upsert(
                new TopicStatus
                    {
                        BoardID = boardId ?? repository.BoardID,
                        ID = topicStatusId ?? 0,
                        TopicStatusName = topicStatusName,
                        DefaultDescription = defaultDescription
                    });
        }

        #endregion
    }
}