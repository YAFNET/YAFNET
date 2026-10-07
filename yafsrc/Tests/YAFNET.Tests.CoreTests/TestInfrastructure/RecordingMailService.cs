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

namespace YAF.Tests.CoreTests.TestInfrastructure;

using System.Collections.Concurrent;
using System.Threading.Tasks;

using MimeKit;

using YAF.Types.Interfaces.Services;

/// <summary>
/// An <see cref="IMailService"/> that records the mails instead of sending them.
/// All the SendAsync / CreateMessage extension methods end up in <see cref="SendAllAsync"/>.
/// </summary>
public class RecordingMailService : IMailService
{
    private readonly ConcurrentQueue<MimeMessage> sent = new();

    /// <summary>
    /// Gets the sent mails.
    /// </summary>
    public IReadOnlyList<MimeMessage> Sent => [.. this.sent];

    /// <summary>
    /// Gets or sets a task every send waits for; set it to an uncompleted task to simulate a slow mail server.
    /// </summary>
    public Task Delay { get; set; } = Task.CompletedTask;

    /// <summary>
    /// Gets the recipient addresses of all sent mails.
    /// </summary>
    public IEnumerable<string> Recipients => this.Sent.SelectMany(m => m.To.Mailboxes).Select(a => a.Address);

    public async Task SendAllAsync(IEnumerable<MimeMessage> messages)
    {
        await this.Delay;

        foreach (var message in messages)
        {
            this.sent.Enqueue(message);
        }
    }
}
