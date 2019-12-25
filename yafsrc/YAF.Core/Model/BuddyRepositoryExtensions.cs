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

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    /// The Buddy repository extensions.
    /// </summary>
    public static class BuddyRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Adds a buddy request. (Should be approved later by "ToUserID")
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserId">
        /// The from User Id.
        /// </param>
        /// <param name="toUserId">
        /// The to User Id.
        /// </param>
        /// <param name="useDisplayName">
        /// Display name of the use.
        /// </param>
        /// <returns>
        /// The name of the second user + Whether this request is approved or not.
        /// </returns>
        [NotNull]
        public static string[] AddRequest(
            this IRepository<Buddy> repository,
            [NotNull] int fromUserId,
            [NotNull] int toUserId,
            [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;
            IDbDataParameter parameterApproved = null;

            repository.SqlList(
                "buddy_addrequest",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserId);
                        cmd.AddParam("ToUserID", toUserId);
                        cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                        parameterApproved = cmd.AddParam("approved", direction: ParameterDirection.Output);
                    });

            return new[] { parameterOutput.Value.ToString(), parameterApproved.Value.ToString() };
        }

        /// <summary>
        /// Approves a buddy request.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <param name="mutual">
        /// Should the requesting user (ToUserID) be added to FromUserID's buddy list too?
        /// </param>
        /// <param name="useDisplayName">
        /// The use Display Name.
        /// </param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        [NotNull]
        public static string ApproveRequest(
            this IRepository<Buddy> repository,
            [NotNull] int fromUserID,
            [NotNull] int toUserID,
            [NotNull] bool mutual,
            [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;

            repository.SqlList(
                "buddy_approverequest",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserID);
                        cmd.AddParam("ToUserID", toUserID);
                        cmd.AddParam("mutual", mutual);
                        cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        /// <summary>
        /// Denies a buddy request.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <param name="useDisplayName">
        /// Display name of the use.
        /// </param>
        /// <returns>
        /// the name of the second user.
        /// </returns>
        [NotNull]
        public static string DenyRequest(
            this IRepository<Buddy> repository,
            [NotNull] int fromUserID,
            [NotNull] int toUserID,
            [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;

            repository.SqlList(
                "buddy_denyrequest",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserID);
                        cmd.AddParam("ToUserID", toUserID);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        /// <summary>
        /// Removes the "ToUserID" from "FromUserID"'s buddy list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="toUserID">
        /// The to user id.
        /// </param>
        /// <param name="useDisplayName">
        /// Display name of the use.
        /// </param>
        /// <returns>
        /// The name of the second user.
        /// </returns>
        [NotNull]
        public static string Remove(
            this IRepository<Buddy> repository,
            [NotNull] int fromUserID,
            [NotNull] int toUserID,
            [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;

            repository.SqlList(
                "buddy_remove",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserID);
                        cmd.AddParam("ToUserID", toUserID);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        /// <summary>
        /// Gets all the buddies of a certain user.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="fromUserID">From user identifier.</param>
        /// <returns>
        /// The <see cref="DataTable" /> containing the buddy list.
        /// </returns>
        public static DataTable List(this IRepository<Buddy> repository, int fromUserID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.buddy_list(FromUserID: fromUserID);
        }

        /// <summary>
        /// The check is friend.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserId">
        /// The from user id.
        /// </param>
        /// <param name="toUserId">
        /// The to user id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool CheckIsFriend(this IRepository<Buddy> repository, int fromUserId, int toUserId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var buddy = repository.GetSingle(b => b.Approved && b.FromUserID == fromUserId && b.ToUserID == toUserId);

            return buddy != null;
        }

        #endregion
    }
}