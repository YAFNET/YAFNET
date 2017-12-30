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

    using Omu.ValueInjecter;

    using YAF.Core.Data;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Data;
    using YAF.Types.Models;

    /// <summary>
    ///     The mail repository extensions.
    /// </summary>
    public static class MailRepositoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="to">The to.</param>
        /// <param name="toName">To name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        public static void Create(
            this IRepository<Mail> repository,
            string to,
            string toName,
            string subject,
            string body)
        {
            repository.Create(
                YafContext.Current.BoardSettings.ForumEmail,
                YafContext.Current.BoardSettings.Name,
                to,
                toName,
                subject,
                body,
                null,
                0,
                null);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="to">The to.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodyHtml">The body html.</param>
        public static void Create(
            this IRepository<Mail> repository,
            string to,
            string toName,
            string subject,
            string body,
            string bodyHtml)
        {
            repository.Create(
                YafContext.Current.BoardSettings.ForumEmail,
                YafContext.Current.BoardSettings.Name,
                to,
                toName,
                subject,
                body,
                bodyHtml,
                0,
                null);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="from">The from.</param>
        /// <param name="fromName">The from name.</param>
        /// <param name="to">The to.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodyHtml">The body html.</param>
        public static void Create(
            this IRepository<Mail> repository,
            string from,
            string fromName,
            string to,
            string toName,
            string subject,
            string body,
            string bodyHtml)
        {
            repository.Create(from, fromName, to, toName, subject, body, bodyHtml, 0, null);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="from">The from.</param>
        /// <param name="fromName">The from name.</param>
        /// <param name="to">The to.</param>
        /// <param name="toName">The to name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodyHtml">The body html.</param>
        /// <param name="sendTries">The send tries.</param>
        /// <param name="sendAttempt">The send attempt.</param>
        public static void Create(
            this IRepository<Mail> repository,
            string from,
            string fromName,
            string to,
            string toName,
            string subject,
            string body,
            string bodyHtml,
            int sendTries,
            DateTime? sendAttempt)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.mail_create(
                From: from,
                FromName: fromName,
                To: to,
                ToName: toName,
                Subject: subject,
                Body: body,
                BodyHtml: bodyHtml,
                SendTries: sendTries,
                SendAttempt: sendAttempt,
                UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();
        }

        /// <summary>
        /// Creates the watch email.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="topicID">The topic id.</param>
        /// <param name="from">The from.</param>
        /// <param name="fromName">The from name.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="bodyHtml">The body html.</param>
        /// <param name="userID">The user id.</param>
        public static void CreateWatch(
            this IRepository<Mail> repository,
            int topicID,
            string from,
            string fromName,
            string subject,
            string body,
            string bodyHtml,
            int userID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Query.mail_createwatch(
                TopicID: topicID,
                From: from,
                FromName: fromName,
                Subject: subject,
                Body: body,
                BodyHtml: bodyHtml,
                UserID: userID,
                UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the Mail List
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="processID">
        /// The process id.
        /// </param>
        /// <returns>
        /// Returns the Mail List as <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<Mail> repository, int? processID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.mail_list(ProcessID: processID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the Mail List Typed
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="processID">
        /// The process id.
        /// </param>
        /// <returns>
        ///  Returns the Mail List as Typed List
        /// </returns>
        public static IList<Mail> ListTyped(this IRepository<Mail> repository, int? processID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            var mailList = new List<Mail>();

            foreach (DataRow dr in
                repository.DbFunction.GetData.mail_list(ProcessID: processID, UTCTIMESTAMP: DateTime.UtcNow).Rows)
            {
                var mail = new Mail();
                mail.InjectFrom<DataRowInjection>(dr);
                mailList.Add(mail);
            }

            return mailList;
        }

        /// <summary>
        /// Save Updated Mail
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mailMessage">The mail message.</param>
        public static void Save(this IRepository<Mail> repository, Mail mailMessage)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.mail_save(
                MailID: mailMessage.ID,
                SendTries: mailMessage.SendTries,
                SendAttempt: DateTime.UtcNow);

            repository.FireUpdated(mailMessage.ID);
        }

        #endregion
    }
}