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
namespace YAF.Core
{
    #region Using

    using System.Collections.Generic;
    using System.Net.Mail;

    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     The yaf send mail extensions.
    /// </summary>
    public static class ISendMailExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="sendMail">
        /// The send Mail.
        /// </param>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="body">
        /// The body.
        /// </param>
        public static void Send(
            [NotNull] this ISendMail sendMail, 
            [NotNull] string fromEmail, 
            [NotNull] string toEmail, 
            [CanBeNull] string subject, 
            [CanBeNull] string body)
        {
            CodeContracts.VerifyNotNull(fromEmail, "fromEmail");
            CodeContracts.VerifyNotNull(toEmail, "toEmail");

            sendMail.Send(new MailAddress(fromEmail), new MailAddress(toEmail), subject, body);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="sendMail">
        /// The send mail.
        /// </param>
        /// <param name="fromEmail">
        /// The from email.
        /// </param>
        /// <param name="fromName">
        /// The from name.
        /// </param>
        /// <param name="toEmail">
        /// The to email.
        /// </param>
        /// <param name="toName">
        /// The to name.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        /// <param name="bodyHtml">
        /// The body html.
        /// </param>
        public static void Send(
            [NotNull] this ISendMail sendMail, 
            [NotNull] string fromEmail, 
            [CanBeNull] string fromName, 
            [NotNull] string toEmail, 
            [CanBeNull] string toName, 
            [CanBeNull] string subject, 
            [CanBeNull] string bodyText, 
            [CanBeNull] string bodyHtml)
        {
            sendMail.Send(new MailAddress(fromEmail, fromName), new MailAddress(toEmail, toName), subject, bodyText, bodyHtml);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="sendMail">
        /// The send Mail.
        /// </param>
        /// <param name="fromAddress">
        /// The from address.
        /// </param>
        /// <param name="toAddress">
        /// The to address.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        public static void Send(
            [NotNull] this ISendMail sendMail, 
            [NotNull] MailAddress fromAddress, 
            [NotNull] MailAddress toAddress, 
            [CanBeNull] string subject, 
            [CanBeNull] string bodyText)
        {
            sendMail.Send(fromAddress, toAddress, subject, bodyText, null);
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="sendMail">
        /// The send mail.
        /// </param>
        /// <param name="fromAddress">
        /// The from address.
        /// </param>
        /// <param name="toAddress">
        /// The to address.
        /// </param>
        /// <param name="subject">
        /// The subject.
        /// </param>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        /// <param name="bodyHtml">
        /// The body html.
        /// </param>
        public static void Send(
            [NotNull] this ISendMail sendMail, 
            [NotNull] MailAddress fromAddress, 
            [NotNull] MailAddress toAddress, 
            [CanBeNull] string subject, 
            [CanBeNull] string bodyText, 
            [CanBeNull] string bodyHtml)
        {
            CodeContracts.VerifyNotNull(sendMail, "sendMail");
            CodeContracts.VerifyNotNull(fromAddress, "fromAddress");
            CodeContracts.VerifyNotNull(toAddress, "toAddress");

            var mailMessage = new MailMessage();
            mailMessage.Populate(fromAddress, toAddress, subject, bodyText, bodyHtml);
            sendMail.SendAll(new List<MailMessage> { mailMessage });
        }

        #endregion
    }
}