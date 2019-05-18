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
        /// Gets All the Thanks for the Message IDs which are in the
        ///   delimited string variable MessageIDs
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageIdsSeparatedWithColon">
        /// The message i ds.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
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

        /// <summary>
        /// The thanks from user.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="thanksFromUserId">
        /// The thanks from user id.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        public static long ThanksFromUser(this IRepository<Thanks> repository, int thanksFromUserId)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.Count(thanks => thanks.ThanksFromUserID == thanksFromUserId);
        }

        /// <summary>
        /// Gets the number of times and posts that other users have thanked the
        /// user with the provided userID.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User Id.
        /// </param>
        /// <returns>
        /// Returns the number of times and posts that other users have thanked the
        /// user with the provided userID.
        /// </returns>
        [NotNull]
        public static int[] GetUserThanksTo(
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
                        cmd.CommandType = CommandType.StoredProcedure;

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
        /// Gets the posts which is thanked by the user + the posts which are posted by the user and
        ///   are thanked by other users.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="pageUserId">
        /// The page User ID.
        /// </param>
        /// <returns>
        /// Returns the posts which is thanked by the user + the posts which are posted by the user and
        ///   are thanked by other users.
        /// </returns>
        public static DataTable ViewAllThanksByUserAsDataTable(
            this IRepository<Thanks> repository,
            [NotNull] int userId,
            [NotNull] int pageUserId)
        {
            return repository.DbFunction.GetData.user_viewallthanks(UserID: userId, PageUserID: pageUserId);
        }

        /// <summary>
        /// Add thanks to the Message
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserId">
        /// The from user id.
        /// </param>
        /// <param name="messageId">
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
            this IRepository<Thanks> repository, [NotNull] int fromUserId, [NotNull] int messageId, [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;
            IDbDataParameter parameterThanksToPostsNumber = null;

            repository.SqlList(
                "message_addthanks",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserId);
                        cmd.AddParam("MessageID", messageId);
                        cmd.AddParam("UTCTIMESTAMP", DateTime.UtcNow);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        /// <summary>
        /// Gets the UserIDs and UserNames who have thanked the message
        ///   with the provided messageID.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <returns>
        /// Returns the UserIDs and UserNames who have thanked the message
        ///   with the provided messageID.
        /// </returns>
        public static DataTable MessageGetThanksAsDataTable(
            this IRepository<Thanks> repository, [NotNull] int messageId)
        {
            return repository.DbFunction.GetData.message_getthanks(MessageID: messageId);
        }

        /// <summary>
        /// The message_ remove thanks.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="fromUserId">
        /// The from user id.
        /// </param>
        /// <param name="messageId">
        /// The message id.
        /// </param>
        /// <param name="useDisplayName">
        /// use the display name.
        /// </param>
        /// <returns>
        /// Returns the name of the user
        /// </returns>
        [NotNull]
        public static string RemoveMessageThanks(
            this IRepository<Thanks> repository, [NotNull] int fromUserId, [NotNull] int messageId, [NotNull] bool useDisplayName)
        {
            IDbDataParameter parameterOutput = null;

            repository.SqlList(
                "message_Removethanks",
                cmd =>
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.AddParam("FromUserID", fromUserId);
                        cmd.AddParam("MessageID", messageId);
                        cmd.AddParam("UseDisplayName", useDisplayName);

                        parameterOutput = cmd.AddParam("paramOutput", direction: ParameterDirection.Output);
                    });

            return parameterOutput.Value.ToString();
        }

        #endregion
    }
}