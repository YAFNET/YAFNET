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

using System.Net.Mail;
using System.Net.Mime;
using System.Text;

/// <summary>
/// The mail message extensions.
/// </summary>
public static class MailMessageExtensions
{
    /// <summary>
    /// Populates the specified mail message.
    /// </summary>
    /// <param name="mailMessage">The mail message.</param>
    /// <param name="fromAddress">The from address.</param>
    /// <param name="toAddress">The to address.</param>
    /// <param name="senderAddress">The sender address.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="bodyText">The body text.</param>
    /// <param name="bodyHtml">The body html.</param>
    public static void Populate(
        this MailMessage mailMessage,
        MailAddress fromAddress,
        MailAddress toAddress,
        MailAddress senderAddress,
        string subject,
        string bodyText,
        string bodyHtml)
    {
        mailMessage.To.Add(toAddress);
        mailMessage.From = fromAddress;

        mailMessage.Sender = senderAddress;

        mailMessage.Subject = subject;

        mailMessage.HeadersEncoding = Encoding.UTF8;
        mailMessage.BodyEncoding = Encoding.UTF8;
        mailMessage.SubjectEncoding = Encoding.UTF8;

        // add default text view
        mailMessage.AlternateViews.Add(
            AlternateView.CreateAlternateViewFromString(bodyText, Encoding.UTF8, MediaTypeNames.Text.Plain));

        // see if html alternative is also desired...
        if (bodyHtml.IsSet())
        {
            mailMessage.AlternateViews.Add(
                AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, MediaTypeNames.Text.Html));
        }
    }
}