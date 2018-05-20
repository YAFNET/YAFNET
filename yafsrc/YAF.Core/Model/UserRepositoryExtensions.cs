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

    using ServiceStack;
    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
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
            return repository.SqlList(
                "user_find",
                new
                    {
                        BoardID = boardId ?? repository.BoardID,
                        Filter = filter,
                        UserName = userName,
                        Email = email,
                        DisplayName = displayName,
                        NotificationType = notificationType,
                        DailyDigest = dailyDigest
                    });
        }

        /// <summary>
        /// Gets the user signature.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Returns the user points</returns>
        public static string GetSignature(
            this IRepository<User> repository,
            int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.GetById(userId).Signature;
        }

        /// <summary>
        /// Saves the signature.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="signature">The signature.</param>
        public static void SaveSignature(
            this IRepository<User> repository,
            [NotNull] int userId,
            [CanBeNull] string signature)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new User { Signature = signature }, where: u => u.ID == userId);
        }

        /// <summary>
        /// Gets the user points.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Returns the user points</returns>
        public static int GetPoints(
            this IRepository<User> repository,
            int userId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.GetById(userId).Points;
        }

        /// <summary>
        /// Sets the points.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="points">The points.</param>
        public static void SetPoints(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull] int points)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(() => new User { Points = points }, where: u => u.ID == userId);
        }

        /// <summary>
        /// Suspends the User
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="suspend">The suspend.</param>
        /// <param name="suspendReason">The suspend reason.</param>
        /// <param name="suspendBy">The suspend by.</param>
        public static void Suspend(
            this IRepository<User> repository, [NotNull] int userId, [NotNull] DateTime? suspend = null, string suspendReason = null, [NotNull] int suspendBy = 0)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.UpdateOnly(
                () => new User { Suspended = suspend, SuspendedReason = suspendReason, SuspendedBy = suspendBy },
                where: u => u.ID == userId);
        }

        /// <summary>
        /// Updates the authentication service status.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="authService">The authentication service.</param>
        public static void UpdateAuthServiceStatus(
            this IRepository<User> repository,
            [NotNull] int userId,
            [NotNull]AuthService authService)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            bool isFacebookUser = false, isTwitterUser = false, isGoogleUser = false;

            switch (authService)
            {
                case AuthService.facebook:
                    isFacebookUser = true;
                    break;
                case AuthService.twitter:
                    isTwitterUser = true;
                    break;
                case AuthService.google:
                    isGoogleUser = true;
                    break;
            }

            repository.UpdateOnly(
                () => new User
                          {
                              IsFacebookUser = isFacebookUser,
                              IsTwitterUser = isTwitterUser,
                              IsGoogleUser = isGoogleUser
                          },
                where: u => u.ID == userId);
        }
    }
}