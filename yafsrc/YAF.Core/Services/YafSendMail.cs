/* Yet Another Forum.net
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

namespace YAF.Core.Services
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mail;

    using YAF.Classes;
    using YAF.Types;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    ///     Functions to send email via SMTP
    /// </summary>
    public class YafSendMail : ISendMail
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sends all MailMessages via the SmtpClient. Doesn't handle any exceptions.
        /// </summary>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        public void SendAll([NotNull] IEnumerable<MailMessage> messages)
        {
            CodeContracts.VerifyNotNull(messages, "messages");

            using (var smtpClient = new SmtpClient { EnableSsl = Config.UseSMTPSSL })
            {
                // send the message...
                foreach (var m in messages.ToList())
                {
                    smtpClient.Send(m);
                }
            }
        }

        /// <summary>
        ///     The send all isolated.
        /// </summary>
        /// <param name="messages">
        ///     The messages.
        /// </param>
        /// <param name="handleExceptionAction">
        ///     The handle exception action.
        /// </param>
        public void SendAllIsolated([NotNull] IEnumerable<MailMessage> messages, [CanBeNull] Action<MailMessage, Exception> handleExceptionAction)
        {
            CodeContracts.VerifyNotNull(messages, "messages");

            using (var smtpClient = new SmtpClient { EnableSsl = Config.UseSMTPSSL })
            {
                // send the message...
                foreach (var m in messages.ToList())
                {
                    try
                    {
                        // send the message...
                        smtpClient.Send(m);
                    }
                    catch (Exception ex)
                    {
                        if (handleExceptionAction != null)
                        {
                            handleExceptionAction(m, ex);
                        }
                    }
                }
            }
        }

        #endregion
    }
}