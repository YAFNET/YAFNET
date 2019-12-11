/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Core.WebService
{
    using System;
    using System.Web.Services;

    using YAF.Configuration;
    using YAF.Core.Model;
    using YAF.Core.UsersRoles;
    using YAF.Types;
    using YAF.Types.EventProxies;
    using YAF.Types.Exceptions;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Events;
    using YAF.Types.Models;

    using WebService = System.Web.Services.WebService;

    /// <summary>
    /// Forum WebService for various Functions
    /// </summary>
    [Obsolete]
    [WebService(Namespace = "https://yetanotherforum.net/services")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class YafWebService : WebService, IHaveServiceLocator
    {
        /// <summary>
        /// Gets ServiceLocator.
        /// </summary>
        public IServiceLocator ServiceLocator => YafContext.Current.ServiceLocator;

        #region Public Methods

        /// <summary>
        /// Creates the new topic.
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <param name="forumId">
        /// The forum Id.
        /// </param>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="styles">
        /// The styles.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="post">
        /// The post.
        /// </param>
        /// <param name="ip">
        /// The IP address.
        /// </param>
        /// <param name="priority">
        /// The priority.
        /// </param>
        /// <param name="flags">
        /// The flags.
        /// </param>
        /// <returns>
        /// Returns the new Message Id
        /// </returns>
        /// <exception cref="SecurityFailureInvalidWebServiceTokenException">
        /// Invalid Secure Web Service Token: Operation Failed
        /// </exception>
        [WebMethod]

        [Obsolete]
        public long CreateNewTopic(
            [NotNull] string token,
            int forumId,
            int userId,
            [NotNull] string username,
            [CanBeNull] string status,
            [CanBeNull] string styles,
            [CanBeNull] string description,
            [NotNull] string subject,
            [NotNull] string post,
            [NotNull] string ip,
            int priority,
            int flags)
        {
            // validate token...
            if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
            {
                throw new SecurityFailureInvalidWebServiceTokenException(
                    "Invalid Secure Web Service Token: Operation Failed");
            }

            long messageId = 0;
            var subjectEncoded = this.Server.HtmlEncode(subject);

            return this.GetRepository<Topic>().Save(
                forumId,
                subjectEncoded,
                status,
                styles,
                description,
                post,
                userId,
                priority,
                username,
                ip,
                DateTime.UtcNow,
                string.Empty,
                flags,
                ref messageId);
        }

        /// <summary>
        /// Sets the display name from username.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="username">The username.</param>
        /// <param name="displayName">The display Name.</param>
        /// <returns>
        /// The set display name from username.
        /// </returns>
        /// <exception cref="Exception">
        ///   <c>Exception</c>.
        ///   </exception>
        /// <exception cref="NonUniqueDisplayNameException">
        ///   <c>NonUniqueDisplayNameException</c>.
        ///   </exception>
        /// <exception cref="SecurityFailureInvalidWebServiceTokenException">Invalid Secure Web Service Token: Operation Failed</exception>
        [WebMethod]
        [Obsolete]
        public bool SetDisplayNameFromUsername(
            [NotNull] string token, [NotNull] string username, [NotNull] string displayName)
        {
            // validate token...
            if (token != YafContext.Current.Get<YafBoardSettings>().WebServiceToken)
            {
                throw new SecurityFailureInvalidWebServiceTokenException(
                    "Invalid Secure Web Service Token: Operation Failed");
            }

            // get the user id...
            var membershipUser = UserMembershipHelper.GetMembershipUserByName(username);

            if (membershipUser == null)
            {
                return false;
            }

            var userId = UserMembershipHelper.GetUserIDFromProviderUserKey(membershipUser.ProviderUserKey);

            var displayNameId = this.Get<IUserDisplayName>().GetId(displayName);

            if (displayNameId.HasValue && displayNameId.Value != userId)
            {
                // problem...
                throw new NonUniqueDisplayNameException(
                    $"Display Name must be unique. {displayName} display name already exists in the database.");
            }

            var userFields = YafContext.Current.GetRepository<User>().ListAsDataTable(Config.BoardID.ToType<int>(), userId, null).Rows[0];

            this.GetRepository<User>().Save(
                userId,
                Config.BoardID,
                null,
                displayName,
                null,
                userFields["TimeZone"],
                userFields["LanguageFile"],
                userFields["Culture"],
                userFields["ThemeFile"],
                userFields["TextEditor"],
                null,
                null,
                null,
                null,
                null,
                null);

            this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(userId));

            return true;
        }

        #endregion
    }
}