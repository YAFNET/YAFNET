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

namespace YAF.Tests.Services;

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using MimeKit;

using YAF.Core.Services;
using YAF.Tests.TestInfrastructure;
using YAF.Types.Objects;

/// <summary>
/// Sending several mails at once (<see cref="MailService.SendAllAsync"/>) against a small smtp server.
/// </summary>
[TestFixture]
[NonParallelizable]
public class MailServiceTests
{
    private TestBoardContext board;

    [SetUp]
    public void SetUp()
    {
        this.board = new TestBoardContext();
    }

    [TearDown]
    public void TearDown()
    {
        this.board?.Dispose();
    }

    [Test]
    [Description("A mail the server refuses (e.g. an unknown recipient) does not stop the other mails (digest, watch notifications)")]
    public async Task Refused_Mail_Does_Not_Stop_The_Others_Test()
    {
        using var server = new FakeSmtpServer("refused@example.com");

        var service = new MailService(
            Options.Create(new MailConfiguration { Host = "127.0.0.1", Port = server.Port }));

        await service.SendAllAsync(
            [
                Mail("first@example.com"),
                Mail("refused@example.com"),
                null,
                Mail("third@example.com")
            ]);

        server.Delivered.Should().Equal("first@example.com", "third@example.com");
    }

    private static MimeMessage Mail(string to)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse("forum@example.com"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = "Digest";
        message.Body = new TextPart("plain") { Text = "text" };

        return message;
    }

    /// <summary>
    /// A smtp server for one connection, it refuses one recipient.
    /// </summary>
    private sealed class FakeSmtpServer : IDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);

        private readonly string refused;

        public FakeSmtpServer(string refused)
        {
            this.refused = refused;
            this.listener.Start();
            _ = Task.Run(this.ServeAsync);
        }

        public int Port => ((IPEndPoint)this.listener.LocalEndpoint).Port;

        public List<string> Delivered { get; } = [];

        public void Dispose()
        {
            this.listener.Stop();
        }

        private async Task ServeAsync()
        {
            using var client = await this.listener.AcceptTcpClientAsync();
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII);
            await using var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };

            await writer.WriteLineAsync("220 localhost");

            string recipient = null;

            while (await reader.ReadLineAsync() is { } line)
            {
                var command = line.ToUpperInvariant();

                if (command.StartsWith("EHLO") || command.StartsWith("HELO"))
                {
                    await writer.WriteLineAsync("250 localhost");
                }
                else if (command.StartsWith("RCPT TO:"))
                {
                    var address = line[8..].Trim('<', '>', ' ');

                    if (address.Equals(this.refused, StringComparison.OrdinalIgnoreCase))
                    {
                        await writer.WriteLineAsync("550 unknown user");
                    }
                    else
                    {
                        recipient = address;
                        await writer.WriteLineAsync("250 ok");
                    }
                }
                else if (command == "DATA")
                {
                    await writer.WriteLineAsync("354 go ahead");

                    while (await reader.ReadLineAsync() is { } data && data != ".")
                    {
                    }

                    this.Delivered.Add(recipient);
                    await writer.WriteLineAsync("250 ok");
                }
                else if (command == "QUIT")
                {
                    await writer.WriteLineAsync("221 bye");
                    break;
                }
                else
                {
                    // MAIL FROM, RSET, NOOP
                    recipient = command.StartsWith("MAIL") || command == "RSET" ? null : recipient;
                    await writer.WriteLineAsync("250 ok");
                }
            }
        }
    }
}
