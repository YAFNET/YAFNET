/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2021 Ingo Herbote
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Topic Tag repository extensions.
    /// </summary>
    public static class TopicTagRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds Topic Tag
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="tagId">
        /// The tag Id.
        /// </param>
        /// <param name="topicId">
        /// The topic ID.
        /// </param>
        public static void Add(
            this IRepository<TopicTag> repository,
            int tagId,
            int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var newId = repository.Insert(
                new TopicTag
                    {
                        TagID = tagId,
                        TopicID = topicId
                    });

            repository.FireNew(newId);
        }

        /// <summary>
        /// List all Topic Tags
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Tuple<TopicTag, Tag>> List(this IRepository<TopicTag> repository, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<TopicTag>();

            expression.Join<Tag>((topicTag, tag) => tag.ID == topicTag.TagID)
                .Where<TopicTag>(t => t.TopicID == topicId)
                .Select();

            return repository.DbAccess.Execute(
                db => db.Connection.SelectMulti<TopicTag, Tag>(expression));
        }

        /// <summary>
        /// List all Topic Tags
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicId">
        /// The topic Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static string ListAsDelimitedString(this IRepository<TopicTag> repository, int topicId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");
            
            return  repository.List(topicId).Select(t => t.Item2.TagName).ToDelimitedString(",");
        }

        #endregion
    }
}