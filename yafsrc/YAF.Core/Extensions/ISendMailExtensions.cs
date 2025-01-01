﻿/* Yet Another Forum.NET
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
namespace YAF.Core.Extensions;

using System.Collections.Generic;
using System.Net.Mail;

/// <summary>
///   The YAF send mail extensions.
/// </summary>
public static class ISendMailExtensions
{
    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="sendMail">The send Mail.</param>
    /// <param name="fromEmail">The from email.</param>
    /// <param name="toEmail">The to email.</param>
    /// <param name="senderEmail">The sender email.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="body">The body.</param>
    public static void Send(
        this IMailService sendMail,
        string fromEmail,
        string toEmail,
        string senderEmail,
        string subject,
        string body)
    {
        sendMail.Send(
            new MailAddress(fromEmail),
            new MailAddress(toEmail),
            new MailAddress(senderEmail),
            subject,
            body);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="sendMail">The send Mail.</param>
    /// <param name="fromAddress">The from address.</param>
    /// <param name="toAddress">The to address.</param>
    /// <param name="senderAddress">The sender address.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="bodyText">The body text.</param>
    public static void Send(
        this IMailService sendMail,
        MailAddress fromAddress,
        MailAddress toAddress,
        MailAddress senderAddress,
        string subject,
        string bodyText)
    {
        sendMail.Send(fromAddress, toAddress, senderAddress, subject, bodyText, null);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="sendMail">The send mail.</param>
    /// <param name="fromAddress">The from address.</param>
    /// <param name="toAddress">The to address.</param>
    /// <param name="senderAddress">The sender address.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="bodyText">The body text.</param>
    /// <param name="bodyHtml">The body html.</param>
    public static void Send(
        this IMailService sendMail,
        MailAddress fromAddress,
        MailAddress toAddress,
        MailAddress senderAddress,
        string subject,
        string bodyText,
        string bodyHtml)
    {
        var mailMessage = new MailMessage();

        mailMessage.Populate(fromAddress, toAddress, senderAddress, subject, bodyText, bodyHtml);

        sendMail.SendAll(new List<MailMessage> { mailMessage });
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
    /// <param name="senderAddress">
    /// The sender address.
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
    /// <returns>
    /// The <see cref="MailMessage"/>.
    /// </returns>
    public static MailMessage CreateMessage(
        this IMailService sendMail,
        MailAddress fromAddress,
        MailAddress toAddress,
        MailAddress senderAddress,
        string subject,
        string bodyText,
        string bodyHtml)
    {
        var mailMessage = new MailMessage();

        mailMessage.Populate(fromAddress, toAddress, senderAddress, subject, bodyText, bodyHtml);

        return mailMessage;
    }
}