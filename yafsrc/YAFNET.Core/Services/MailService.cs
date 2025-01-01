/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit.Security;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MimeKit;

using YAF.Types.Objects;

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
    /// The mail config.
    /// </summary>
    private readonly MailConfiguration mailConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailService"/> class.
    /// </summary>
    /// <param name="mailConfiguration">
    /// The mail configuration.
    /// </param>
    public MailService(IOptions<MailConfiguration> mailConfiguration)
    {
        this.mailConfig = mailConfiguration.Value;
    }

    /// <summary>
    /// Sends all MailMessages via the SMTP Client. Doesn't handle any exceptions.
    /// </summary>
    /// <param name="messages">
    ///     The messages.
    /// </param>
    public async Task SendAllAsync(IEnumerable<MimeMessage> messages)
    {
        var mailMessages = messages.ToList();

        ArgumentNullException.ThrowIfNull(mailMessages);

        var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(this.mailConfig.Host, this.mailConfig.Port, SecureSocketOptions.StartTlsWhenAvailable);

        if (this.mailConfig.Password.IsSet() && this.mailConfig.Mail.IsSet())
        {
            await smtpClient.AuthenticateAsync(this.mailConfig.Mail, this.mailConfig.Password);
        }

        // send the message...
        foreach (var m in mailMessages.ToList())
        {
            try
            {
                if (m != null)
                {
                    // send the message...
                    await smtpClient.SendAsync(m);
                }
            }
            catch (Exception ex)
            {
                this.Get<ILogger<MailService>>().Error(ex, "Mail Error");
            }
        }

        await smtpClient.DisconnectAsync(true);
    }
}