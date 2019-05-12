/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
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

    using System;
    using System.Collections.Generic;
    using System.Data;

    using ServiceStack.OrmLite;

    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    ///     The Thanks repository extensions.
    /// </summary>
    public static class ThanksRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Retuns All the Thanks for the Message IDs which are in the
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="messageIdsSeparatedWithColon">
        /// The message i ds.
        /// </param>
        [NotNull]
        public static IEnumerable<TypedAllThanks> MessageGetAllThanks(
            this IRepository<Thanks> repository,
            [NotNull] string messageIdsSeparatedWithColon)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction
                .GetAsDataTable(cdb => cdb.message_getallthanks(MessageIDs: messageIdsSeparatedWithColon))
                .SelectTypedList(t => new TypedAllThanks(t));
        }

        public static long ThanksFromUser(this IRepository<Thanks> repository, int thanksFromUserId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Count(thanks => thanks.ThanksFromUserID == thanksFromUserId);
        }

        // <summary> Returns the number of times and posts that other users have thanked the
        // user with the provided userID.
        /// <summary>
        /// The user_getthanks_to.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// </returns>
        [NotNull]
        public static int[] GetuserThanksTo(
            this IRepository<Thanks> repository,
            [NotNull] int userId,
            [NotNull] int pageUserId)
        {

            IDbDataParameter parameterThanksToNumber = null;
            IDbDataParameter parameterThanksToPostsNumber = null;

            repository.SqlList(
                "user_getthanks_to",
                cmd =>
                    {


                        cmd.AddParam("UserID", userId);

                        cmd.AddParam("PageUserID", pageUserId);

                        parameterThanksToNumber = cmd.AddParam("ThanksToNumber", direction: ParameterDirection.Output);
                        parameterThanksToPostsNumber = cmd.AddParam(
                            "ThanksToPostsNumber",
                            direction: ParameterDirection.Output);
                    });

            return new[]
                       {
                           parameterThanksToNumber.Value.ToType<int>(), parameterThanksToPostsNumber.Value.ToType<int>()
                       };
        }

        /// <summary>
        /// Returns the posts which is thanked by the user + the posts which are posted by the user and
        ///   are thanked by other users.
        /// </summary>
        /// <param name="UserID">
        /// The user id.
        /// </param>
        /// <param name="pageUserID">
        /// The page User ID.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable ViewAllThanksByUserAsDataTable(
            this IRepository<Thanks> repository,
            [NotNull] int UserID,
            [NotNull] int pageUserID)
        {
            return repository.DbFunction.GetData.user_viewallthanks(UserID: UserID, PageUserID: pageUserID);
        }

        /// <summary>
        /// The message_ add thanks.
        /// </summary>
        /// <param name="fromUserID">
        /// The from user id.
        /// </param>
        /// <param name="messageID">
        /// The message id.
        /// </param>
        /// <param name="useDisplayName">
        /// Use Display Name.
        /// </param>
        /// <returns>
        /// Returns the Name of the User
        /// </returns>
        [NotNull]
        public static string AddMessageThanks(
            this IRepository<Thanks> repository, [NotNull] int fromUserID, [NotNull] int messageID, [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;
            IDbDataParameter parameterThanksToPostsNumber = null;

            repository.SqlList(
                "message_addthanks",
                cmd =>
                    {
                        cmd.AddParam("FromUserID", fromUserID);
                        cmd.AddParam("MessageID", messageID);
                        cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        /// <summary>
        /// Returns the UserIDs and UserNames who have thanked the message
        ///   with the provided messageID.
        /// </summary>
        /// <param name="MessageID">
        /// The message id.
        /// </param>
        /// <returns>
        /// </returns>
        public static DataTable MessageGetThanksAsDataTable(
            this IRepository<Thanks> repository, [NotNull] int MessageID)
        {
            return repository.DbFunction.GetData.message_getthanks(MessageID: MessageID);
        }

        /// <summary>
        /// The message_ remove thanks.
        /// </summary>
        /// <param name="FromUserID">The from user id.</param>
        /// <param name="MessageID">The message id.</param>
        /// <param name="useDisplayName">use the display name.</param>
        /// <returns>
        /// Returns the name of the user
        /// </returns>
        [NotNull]
        public static string RemoveMessageThanks(
            this IRepository<Thanks> repository, [NotNull] int FromUserID, [NotNull] int MessageID, [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;

            repository.SqlList(
                "message_Removethanks",
                cmd =>
                    {
                        cmd.AddParam("FromUserID", FromUserID);
                        cmd.AddParam("MessageID", MessageID);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        #endregion
    }
}