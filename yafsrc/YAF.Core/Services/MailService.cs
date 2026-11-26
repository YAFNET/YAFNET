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
using System.Linq;
using System.Net.Mail;
using System.Runtime.ExceptionServices;

using YAF.Types.Constants;
using YAF.Types.Exceptions;

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
    /// Until when no mails are sent (UTC, for all boards of the application).
    /// </summary>
    private static DateTime pausedUntil = DateTime.MinValue;

    /// <summary>
    /// Sends all MailMessages via the SMTP Client. Doesn't handle any exceptions.
    /// </summary>
    /// <param name="messages">
    /// The messages.
    /// </param>
    public void SendAll(
        IEnumerable<MailMessage> messages)
    {
        var mailMessages = messages.Where(m => m != null).ToList();

        // the mail server throttled the sending: no new connection (each mail would fail and write an error to the
        // event log)
        if (DateTime.UtcNow < pausedUntil)
        {
            throw new MailThrottledException(pausedUntil.ToLocalTime());
        }

        // the smtp settings (incl. EnableSsl) come from the web.config (system.net/mailSettings)
#pragma warning disable S5332
        using var smtpClient = new SmtpClient();
#pragma warning restore S5332

        var sent = 0;
        Exception firstError = null;

        MailThrottledException throttled = null;

        // a refused mail (e.g. an unknown recipient) does not stop the other mails
        foreach (var m in mailMessages)
        {
            try
            {
                smtpClient.Send(m);
                sent++;
            }
            catch (Exception ex) when (IsThrottled(ex))
            {
                // e.g. the hourly mail limit of the hoster: one warning, the sending is paused, the other mails are
                // not sent (each wrote an error to the event log)
                pausedUntil = DateTime.UtcNow.Add(ThrottlePause);

                throttled = new MailThrottledException(pausedUntil.ToLocalTime(), ex);

                this.Get<ILoggerService>().Log(
                    $"{throttled.Message} ({ex.Message}). {mailMessages.Count - sent} of {mailMessages.Count} mail(s) not sent.",
                    EventLogTypes.Warning,
                    null,
                    "Mail Error");

                break;
            }
            catch (Exception ex)
            {
                this.Get<ILoggerService>().Log("Mail Error", EventLogTypes.Error, null, null, ex);

                firstError ??= ex;
            }
        }

        // the callers do not log it again (the warning is in the event log), the digest is not reported as sent
        if (throttled != null)
        {
            throw throttled;
        }

        // no mail was sent (e.g. the only mail, a test mail or a mail to a user): the caller reports the error
        if (sent == 0 && firstError != null)
        {
            ExceptionDispatchInfo.Capture(firstError).Throw();
        }
    }

    /// <summary>
    /// Determines whether the mail server throttles the sending (e.g. "You have exceeded your session messaging limits",
    /// "Sender is throttled"): a temporary (4xx) refusal of the server, or a refusal that names a limit.
    /// </summary>
    /// <param name="ex">The error of the mail server.</param>
    /// <returns>Returns if the sending is throttled.</returns>
    private static bool IsThrottled(Exception ex)
    {
        // a refused recipient is not a limit of the sender
        if (ex is not SmtpException smtp || ex is SmtpFailedRecipientException)
        {
            return false;
        }

        if ((int)smtp.StatusCode is >= 400 and < 500)
        {
            return true;
        }

        var message = $"{smtp.Message} {smtp.InnerException?.Message}";

        return ThrottleWords.Any(w => message.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
    }
}