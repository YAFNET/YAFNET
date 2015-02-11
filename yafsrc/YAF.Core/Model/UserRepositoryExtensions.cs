/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2015 Ingo Herbote
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

    using YAF.Types;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The User Repository Extensions
    /// </summary>
    public static class UserRepositoryExtensions
    {
        /// <summary>
        /// Gets the List of Administrators
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="useStyledNicks">The use styled nicks.</param>
        /// <param name="boardId">The board identifier.</param>
        /// <returns>Returns a Data Table of Administrators</returns>
        public static DataTable AdminList(
            this IRepository<User> repository,
            bool? useStyledNicks = null,
            int? boardId = null)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.admin_list(
                BoardID: boardId ?? repository.BoardID,
                StyledNicks: useStyledNicks,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// Finds the user typed.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="filter">if set to <c>true</c> [filter].</param>
        /// <param name="boardId">The board identifier.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="email">The email.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="dailyDigest">The daily digest.</param>
        /// <returns>Returns List of found user(s).</returns>
        public static IList<User> FindUserTyped(
            this IRepository<User> repository,
            bool filter,
            int? boardId = null,
            string userName = null,
            string email = null,
            string displayName = null,
            int? notificationType = null,
            bool? dailyDigest = null)
        {
            using (var session = repository.DbFunction.CreateSession())
            {
                return
                    session.GetTyped<User>(
                        r =>
                        r.user_find(
                            BoardID: boardId ?? repository.BoardID,
                            Filter: filter,
                            UserName: userName,
                            Email: email,
                            DisplayName: displayName,
                            NotificationType: notificationType,
                            DailyDigest: dailyDigest));
            }
        }
    }
}