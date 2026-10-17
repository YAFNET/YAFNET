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

namespace YAF.Tests.TestInfrastructure;

using System.Collections.Concurrent;
using System.IO;
using System.Net.Mail;

using YAF.Types.Interfaces.Services;

/// <summary>
/// An <see cref="IMailService"/> that records the mails instead of sending them.
/// All the Send / CreateMessage extension methods end up in <see cref="SendAll"/>.
/// </summary>
public class RecordingMailService : IMailService
{
    private readonly ConcurrentQueue<MailMessage> sent = new();

    /// <summary>
    /// Gets the sent mails.
    /// </summary>
    public IReadOnlyList<MailMessage> Sent => [.. this.sent];

    /// <summary>
    /// Gets the recipient addresses of all sent mails.
    /// </summary>
    public IEnumerable<string> Recipients => this.Sent.SelectMany(m => m.To).Select(a => a.Address);

    /// <summary>
    /// Gets or sets an error every send throws (e.g. to simulate a mail server that is down).
    /// </summary>
    public Exception Error { get; set; }

    public void SendAll(IEnumerable<MailMessage> messages)
    {
        if (this.Error != null)
        {
            throw this.Error;
        }

        foreach (var message in messages)
        {
            this.sent.Enqueue(message);
        }
    }

    /// <summary>
    /// The text of the mail (the plain text view, or the body).
    /// </summary>
    public static string TextOf(MailMessage message)
    {
        return ViewText(message, "text/plain") ?? message.Body;
    }

    /// <summary>
    /// The html of the mail (the html view, or the body of an html mail).
    /// </summary>
    public static string HtmlOf(MailMessage message)
    {
        return ViewText(message, "text/html") ?? (message.IsBodyHtml ? message.Body : null);
    }

    private static string ViewText(MailMessage message, string mediaType)
    {
        var view = message.AlternateViews.FirstOrDefault(v => v.ContentType.MediaType == mediaType);

        if (view is null)
        {
            return null;
        }

        view.ContentStream.Position = 0;

        return new StreamReader(view.ContentStream).ReadToEnd();
    }
}
