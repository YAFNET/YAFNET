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

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The ForumAcess Repository Extensions
    /// </summary>
    public static class ForumAcessRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Save Updated Forum Access
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="accessMaskId">The access mask identifier.</param>
        public static void Save(this IRepository<ForumAccess> repository, [NotNull] int forumId, [NotNull] int groupId, [NotNull] int accessMaskId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(
                () => new ForumAccess { AccessMaskID = accessMaskId },
                f => f.ForumID == forumId && f.GroupID == groupId);
        }

        /// <summary>
        /// Gets the paged list of all users Album Images
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <returns>
        /// Returns the list of entities
        /// </returns>
        public static List<ForumAccessList> GetForumAccessList(
            [NotNull] this IRepository<ForumAccess> repository,
            int forumId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var expression = OrmLiteConfig.DialectProvider.SqlExpression<ForumAccess>();

            expression.Join<ForumAccess, Group>((access, group) => group.ID == access.GroupID)
                .Where<ForumAccess, Group>((access, group) => access.ForumID == forumId)
                .Select<ForumAccess, Group>((access, group) => new { access, GroupName = group.Name });

            var results = new List<ForumAccessList>();

            foreach (var result in repository.DbAccess.Execute(db => db.Connection.Select<dynamic>(expression)))
            {
                var item = new ForumAccessList
                               {
                                   GroupID = result.GroupID,
                                   ForumID = result.ForumID,
                                   AccessMaskID = result.AccessMaskID,
                                   GroupName = result.GroupName
                               };

                results.Add(item);
            }

            return results;
        }

        #endregion
    }

    [Serializable]
    public class ForumAccessList
    {
         public int GroupID { get; set; }

        public int ForumID { get; set; }

        public int AccessMaskID { get; set; }

        public string GroupName { get; set; }
    }
}