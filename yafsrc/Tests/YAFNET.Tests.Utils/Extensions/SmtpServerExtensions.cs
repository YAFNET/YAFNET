/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
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

using System;

namespace YAF.Tests.Utils.Extensions;

using System.Linq;

using netDumbster.smtp;

/// <summary>
/// Helpers for the test mail server.
/// </summary>
public static class SmtpServerExtensions
{
    /// <param name="server">
    /// The test mail server.
    /// </param>
    extension(SimpleSmtpServer server)
    {
        /// <summary>
        /// Waits until a mail to the given address is received, the forum sends the mails in the background.
        /// </summary>
        /// <param name="toAddress">The recipient address.</param>
        /// <param name="timeout">How long to wait (default 60 seconds).</param>
        /// <returns>The newest mail to that address, or null if none arrived in time.</returns>
        public async Task<SmtpMessage> WaitForEmailAsync(string toAddress, TimeSpan? timeout = null)
        {
            var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(60));

            while (DateTime.UtcNow < deadline)
            {
                var mail = server.ReceivedEmail.LastOrDefault(
                    m => m.ToAddresses.Any(
                        a => string.Equals(a.Address, toAddress, StringComparison.OrdinalIgnoreCase)));

                if (mail is not null)
                {
                    return mail;
                }

                await Task.Delay(500);
            }

            return null;
        }

        /// <summary>
        /// Waits for a mail to the given address and returns the first link in it.
        /// </summary>
        /// <param name="toAddress">The recipient address.</param>
        /// <returns>The first link of the mail, or null if no mail or link was found.</returns>
        public async Task<string> WaitForEmailLinkAsync(string toAddress)
        {
            var mail = await server.WaitForEmailAsync(toAddress);

            return mail is null ? null : StringUtils.GetLinks(mail.MessageParts[0].BodyData).FirstOrDefault();
        }
    }
}
