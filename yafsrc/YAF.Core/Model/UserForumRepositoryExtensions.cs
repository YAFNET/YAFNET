/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The UserForum Repository Extensions
    /// </summary>
    public static class UserForumRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Deletes the specified user identifier.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="forumId">The forum identifier.</param>
        /// <returns>Returns if deleting was successful or not</returns>
        public static bool Delete(this IRepository<UserForum> repository, int? userId, int? forumId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var success = repository.DbAccess.Execute(
                              db => db.Connection.Delete<UserForum>(x => x.UserID == userId && x.ForumID == forumId))
                          == 1;

            if (success)
            {
                repository.FireDeleted();
            }

            return success;
        }

        /// <summary>
        /// The userforum_list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable ListAsDataTable(
            this IRepository<UserForum> repository,
            [NotNull] object userID,
            [NotNull] object forumID)
        {
            return repository.DbFunction.GetData.userforum_list(UserID: userID, ForumID: forumID);
        }

        /// <summary>
        /// The userforum_save.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="accessMaskID">
        /// The access mask id.
        /// </param>
        public static void Save(
            this IRepository<UserForum> repository,
            [NotNull] object userID,
            [NotNull] object forumID,
            [NotNull] object accessMaskID)
        {
            repository.DbFunction.Scalar.userforum_save(
                UserID: userID,
                ForumID: forumID,
                AccessMaskID: accessMaskID,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        #endregion
    }
}