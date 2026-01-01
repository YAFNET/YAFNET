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
/// The UserForum Repository Extensions
/// </summary>
public static class UserForumRepositoryExtensions
{
    /// <param name="repository">The repository.</param>
    extension(IRepository<UserForum> repository)
    {
        /// <summary>
        /// Deletes the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <returns>Returns if deleting was successful or not</returns>
        public bool Delete(int userId, int forumId)
        {
            var success = repository.DbAccess.Execute(
                db => db.Connection.Delete<UserForum>(x => x.UserID == userId && x.ForumID == forumId)) == 1;

            return success;
        }

        /// <summary>
        /// Gets the User (Moderator) Forum List
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        public List<Tuple<User, UserForum, AccessMask>> List(int? userId,
            int forumId)
        {
            var expression = OrmLiteConfig.DialectProvider.SqlExpression<User>();

            if (userId.HasValue)
            {
                expression.Join<UserForum>((a, b) => b.UserID == a.ID)
                    .Join<UserForum, AccessMask>((b, c) => c.ID == b.AccessMaskID)
                    .Where<UserForum, User>((b, a) => b.ForumID == forumId && a.ID == userId)
                    .Select<User, UserForum, AccessMask>((user, b, c) => new { user, b, c });
            }
            else
            {
                expression.Join<UserForum>((a, b) => b.UserID == a.ID).Join<UserForum, AccessMask>((b, c) => c.ID == b.AccessMaskID)
                    .Where<UserForum>((b) => b.ForumID == forumId)
                    .Select<User, UserForum, AccessMask>((user, b, c) => new { user, b, c });
            }

            return repository.DbAccess.Execute(
                db => db.Connection.SelectMulti<User, UserForum, AccessMask>(expression));
        }

        /// <summary>
        /// Save the User Forum
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="accessMaskId">
        /// The access mask id.
        /// </param>
        public void Save(int userId,
            int forumId,
            int accessMaskId)
        {
            var userForum = repository.GetSingle(x => x.UserID == userId && x.ForumID == forumId);

            if (userForum != null)
            {
                // update
                repository.UpdateOnly(
                    () => new UserForum { AccessMaskID = accessMaskId },
                    x => x.UserID == userId && x.ForumID == forumId);
            }
            else
            {
                // add
                repository.Insert(
                    new UserForum
                    {
                        AccessMaskID = accessMaskId,
                        UserID = userId,
                        ForumID = forumId,
                        Invited = DateTime.UtcNow,
                        Accepted = true
                    });
            }
        }
    }
}