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

    /// <summary>
    /// The WatchTopic Repository Extensions
    /// </summary>
    public static class WatchTopicRepositoryExtensions
    {
        /// <summary>
        /// Adds the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userID">The user identifier.</param>
        /// <param name="topicID">The topic identifier.</param>
        public static void Add(this IRepository<WatchTopic> repository, int userID, int topicID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            if (repository.Check(userID, topicID).HasValue)
            {
                return;
            }

            repository.DbFunction.Query.watchtopic_add(UserID: userID, TopicID: topicID, UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();
        }

        /// <summary>
        /// Checks the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="topicId">The topic identifier.</param>
        /// <returns></returns>
        public static int? Check(this IRepository<WatchTopic> repository, int userId, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var topic = repository.GetSingle(w => w.UserID == userId && w.TopicID == topicId);

            return topic?.ID;
        }

        /// <summary>
        /// Lists the specified repository.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userID">The user identifier.</param>
        /// <returns></returns>
        public static DataTable List(this IRepository<WatchTopic> repository, int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.watchtopic_list(UserID: userID);
        }
    }
}