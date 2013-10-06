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

    using System;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The mail message extensions.
    /// </summary>
    public static class MailMessageExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Does the string contain any unicode characters?
        /// From http://stackoverflow.com/questions/4459571/how-to-recognize-if-a-string-contains-unicode-chars
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The contains unicode character.
        /// </returns>
        public static bool ContainsUnicodeCharacter([NotNull] string input)
        {
            CodeContracts.VerifyNotNull(input, "input");

            const int MaxAnsiCode = 255;

            return input.ToCharArray().Any(c => c > MaxAnsiCode);
        }

        /// <summary>
        /// The populate.
        /// </summary>
        /// <param name="mailMessage">
        /// The mail message. 
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
        [NotNull]
        public static void Populate(
            [NotNull] this MailMessage mailMessage, 
            [NotNull] MailAddress fromAddress, 
            [NotNull] MailAddress toAddress, 
            [CanBeNull] string subject, 
            [CanBeNull] string bodyText, 
            [CanBeNull] string bodyHtml)
        {
            CodeContracts.VerifyNotNull(mailMessage, "mailMessage");
            CodeContracts.VerifyNotNull(fromAddress, "fromAddress");
            CodeContracts.VerifyNotNull(toAddress, "toAddress");

            mailMessage.To.Add(toAddress);
            mailMessage.From = fromAddress;
            mailMessage.Subject = subject;

            // add default text view
            mailMessage.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(
                    bodyText, ContainsUnicodeCharacter(bodyText) ? Encoding.Unicode : Encoding.UTF8, MediaTypeNames.Text.Plain));

            // see if html alternative is also desired...
            if (bodyHtml.IsSet())
            {
                mailMessage.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        bodyHtml, ContainsUnicodeCharacter(bodyHtml) ? Encoding.Unicode : Encoding.UTF8, MediaTypeNames.Text.Html));
            }
        }

        /// <summary>
        /// Creates a SmtpClient and sends a MailMessage.
        /// </summary>
        /// <param name="message">
        /// The message. 
        /// </param>
        public static void Send([NotNull] this MailMessage message)
        {
            CodeContracts.VerifyNotNull(message, "message");

            var smtpSend = new SmtpClient { EnableSsl = Config.UseSMTPSSL };

            // Tommy: solve random failure problem. Don't set this value to 1.
            // See this: http://stackoverflow.com...tem-net-mail-has-issues 
            smtpSend.ServicePoint.MaxIdleTime = 10;
            smtpSend.ServicePoint.ConnectionLimit = 1;

            // send the message...
            smtpSend.Send(message);
        }

        #endregion
    }
}