/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System.Collections.Generic;
using System.Net.Mail;

using YAF.Types.Constants;

/// <summary>
///     Functions to send email via SMTP
/// </summary>
public class MailService : IMailService, IHaveServiceLocator
{
    /// <summary>
    ///     Gets the service locator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Sends all MailMessages via the SMTP Client. Doesn't handle any exceptions.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    public void SendAll(
        [NotNull] IEnumerable<MailMessage> messages)
    {
        var mailMessages = messages.ToList();

        CodeContracts.VerifyNotNull(mailMessages);

        using var smtpClient = new SmtpClient();

        // send the message...
        mailMessages.ToList().ForEach(
            m =>
                {
                    try
                    {
                        if (m != null)
                        {
                            // send the message...
                            smtpClient.Send(m);
                        }
                    }
                    catch (SmtpException ex)
                    {
                        this.Get<ILoggerService>().Log("Mail Error", EventLogTypes.Error, null, null, ex);
                    }
                });

        smtpClient.Dispose();
    }
}