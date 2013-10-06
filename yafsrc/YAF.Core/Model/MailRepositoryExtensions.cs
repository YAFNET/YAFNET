/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */
namespace YAF.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data;

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
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="bodyHtml">
        /// The body html.
        /// </param>
        public static void Create(
            this IRepository<Mail> repository, string from, string fromName, string to, string toName, string subject, string body, string bodyHtml)
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
                UTCTIMESTAMP: DateTime.UtcNow);

            repository.FireNew();
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="to">
        /// The to.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        public static void Create(this IRepository<Mail> repository, string from, string to, string subject, string body)
        {
            repository.Create(from, null, to, null, subject, body, null);
        }

        /// <summary>
        /// The createwatch.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="from">
        /// The from.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        /// <param name="bodyHtml">
        /// The body html.
        /// </param>
        /// <param name="userID">
        /// The user id.
        /// </param>
        public static void CreateWatch(
            this IRepository<Mail> repository, int topicID, string from, string fromName, string subject, string body, string bodyHtml, int userID)
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
        /// The list.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="processID">
        /// The process id.
        /// </param>
        /// <returns>
        /// The <see cref="DataTable"/>.
        /// </returns>
        public static DataTable List(this IRepository<Mail> repository, int? processID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            return repository.DbFunction.GetData.mail_list(ProcessID: processID, UTCTIMESTAMP: DateTime.UtcNow);
        }

        /// <summary>
        /// The list typed.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="processID">
        /// The process id.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public static IList<Mail> ListTyped(this IRepository<Mail> repository, int? processID)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            using (var session = repository.DbFunction.CreateSession())
            {
                return session.GetTyped<Mail>(
                    r => r.mail_list(ProcessID: processID, UTCTIMESTAMP: DateTime.UtcNow));
            }
        }

        /// <summary>
        /// Save Updated Mail
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mailMessage">The mail message.</param>
        public static void Save(
            this IRepository<Mail> repository, Mail mailMessage)
        {
            CodeContracts.VerifyNotNull(repository, "repository");

            repository.DbFunction.Scalar.mail_save(
                MailID: mailMessage.ID, SendTries: mailMessage.SendTries, SendAttempt: DateTime.UtcNow);

            repository.FireUpdated(mailMessage.ID);
        }

        #endregion
    }
}