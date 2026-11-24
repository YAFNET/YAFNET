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
    /// How long no mails are sent after the mail server throttled the sending (hosters limit the mails per hour).
    /// </summary>
    private static readonly TimeSpan ThrottlePause = TimeSpan.FromHours(1);

    /// <summary>
    /// The words of a mail server error that tell the sending is throttled.
    /// </summary>
    private static readonly string[] ThrottleWords =
    [
        "throttl", "messaging limit", "sending limit", "message limit", "rate limit", "too many messages"
    ];

    /// <summary>
    /// Until when no mails are sent (the service is a single instance).
    /// </summary>
    private DateTimeOffset pausedUntil = DateTimeOffset.MinValue;

    /// <summary>
    /// Gets or sets the clock of the throttle pause (the system clock, tests set a fixed time).
    /// </summary>
    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;

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
    /// Gets the encryption of the connection to the mail server: the configured one, or Auto (SSL on connect for port
    /// 465, otherwise STARTTLS when the server offers it). Only STARTTLS was used, a server on port 465 (SSL only)
    /// could not be used.
    /// </summary>
    /// <param name="mailConfiguration">The mail configuration.</param>
    /// <returns>The <see cref="SecureSocketOptions"/>.</returns>
    public static SecureSocketOptions GetSecureSocketOptions(MailConfiguration mailConfiguration)
    {
        return Enum.TryParse<SecureSocketOptions>(mailConfiguration?.Security, true, out var options)
               && Enum.IsDefined(options)
                   ? options
                   : SecureSocketOptions.Auto;
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

        // the mail server throttled the sending: no new connection (each would fail and write an error to the event log)
        if (this.TimeProvider.GetUtcNow() < this.pausedUntil)
        {
            throw new MailThrottledException(this.pausedUntil.ToLocalTime());
        }

        var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(this.mailConfig.Host, this.mailConfig.Port, GetSecureSocketOptions(this.mailConfig));

        if (this.mailConfig.Password.IsSet() && this.mailConfig.UserName.IsSet())
        {
            await smtpClient.AuthenticateAsync(this.mailConfig.UserName, this.mailConfig.Password);
        }

        var sent = 0;
        Exception firstError = null;
        MailThrottledException throttled = null;

        var toSend = mailMessages.Where(m => m != null).ToList();

        foreach (var m in toSend)
        {
            // a refused mail (e.g. an unknown recipient) does not stop the other mails
            try
            {
                await smtpClient.SendAsync(new MailKit.SendRequest(m));
                sent++;
            }
            catch (Exception ex) when (IsThrottled(ex))
            {
                // e.g. the hourly mail limit of the hoster: one warning, the sending is paused, the other mails are not sent
                this.pausedUntil = this.TimeProvider.GetUtcNow().Add(ThrottlePause);

                throttled = new MailThrottledException(this.pausedUntil.ToLocalTime(), ex);

                this.Get<ILogger<MailService>>().Warn(
                    "{0} ({1}). {2} of {3} mail(s) not sent.",
                    throttled.Message,
                    ex.Message,
                    toSend.Count - sent,
                    toSend.Count);

                break;
            }
            catch (Exception ex)
            {
                this.Get<ILogger<MailService>>().Error(ex, "Mail Error");

                firstError ??= ex;

                if (!smtpClient.IsConnected)
                {
                    break;
                }
            }
        }

        if (smtpClient.IsConnected)
        {
            await smtpClient.DisconnectAsync(true);
        }

        // the callers do not log it again (the warning is in the event log), the digest is not reported as sent
        if (throttled is not null)
        {
            throw throttled;
        }

        // no mail was sent (e.g. the only mail, a test mail or a mail to a user): the caller reports the error
        if (sent == 0 && firstError is not null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(firstError).Throw();
        }
    }

    /// <summary>
    /// Determines whether the mail server throttles the sending (e.g. "You have exceeded your session messaging limits",
    /// "Sender is throttled"): a temporary (4xx) refusal of the sender, or a refusal that names a limit.
    /// </summary>
    /// <param name="ex">The error of the mail server.</param>
    /// <returns>Returns if the sending is throttled.</returns>
    private static bool IsThrottled(Exception ex)
    {
        if (ex is not SmtpCommandException command)
        {
            return false;
        }

        if (command.ErrorCode == SmtpErrorCode.SenderNotAccepted && (int)command.StatusCode is >= 400 and < 500)
        {
            return true;
        }

        return ThrottleWords.Any(w => command.Message.Contains(w, StringComparison.OrdinalIgnoreCase));
    }
}