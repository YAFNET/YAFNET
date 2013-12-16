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

namespace YAF.Core.Extensions
{
    #region Using

    using System.Net.Mail;
    using System.Net.Mime;
    using System.Text;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Extensions;

    #endregion

    /// <summary>
    /// The mail message extensions.
    /// </summary>
    public static class MailMessageExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Populates the specified mail message.
        /// </summary>
        /// <param name="mailMessage">The mail message.</param>
        /// <param name="fromAddress">The from address.</param>
        /// <param name="toAddress">The to address.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="bodyText">The body text.</param>
        /// <param name="bodyHtml">The body html.</param>
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

            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;

            // add default text view
            mailMessage.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(
                    bodyText, Encoding.UTF8, MediaTypeNames.Text.Plain));

            // see if html alternative is also desired...
            if (bodyHtml.IsSet())
            {
                mailMessage.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        bodyHtml, Encoding.UTF8, MediaTypeNames.Text.Html));
            }
        }

        #endregion
    }
}