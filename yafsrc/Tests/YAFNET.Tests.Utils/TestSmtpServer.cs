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

namespace YAF.Tests.Utils;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using MimeKit;

using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

using ServiceProvider = SmtpServer.ComponentModel.ServiceProvider;

/// <summary>
/// An in-process SMTP server for the tests, it keeps every received mail in memory.
/// </summary>
public sealed class TestSmtpServer : IDisposable
{
    /// <summary>
    /// The received mails.
    /// </summary>
    private readonly List<MimeMessage> receivedEmail = [];

    /// <summary>
    /// Stops the server.
    /// </summary>
    private readonly CancellationTokenSource cancellation = new();

    /// <summary>
    /// The SMTP server.
    /// </summary>
    private readonly SmtpServer server;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestSmtpServer"/> class.
    /// </summary>
    /// <param name="port">The port to listen on (all interfaces, the forum may run in a docker container).</param>
    private TestSmtpServer(int port)
    {
        this.Port = port;

        var options = new SmtpServerOptionsBuilder()
            .ServerName("localhost")
            .Endpoint(b => b.Endpoint(new IPEndPoint(IPAddress.Any, port)))
            .Build();

        var serviceProvider = new ServiceProvider();
        serviceProvider.Add(new Store(this));

        this.server = new SmtpServer(options, serviceProvider);
    }

    /// <summary>
    /// Gets the port.
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// Gets a snapshot of the received mails.
    /// </summary>
    public IReadOnlyList<MimeMessage> ReceivedEmail
    {
        get
        {
            lock (this.receivedEmail)
            {
                return [.. this.receivedEmail];
            }
        }
    }

    /// <summary>
    /// Gets the number of received mails.
    /// </summary>
    public int ReceivedEmailCount
    {
        get
        {
            lock (this.receivedEmail)
            {
                return this.receivedEmail.Count;
            }
        }
    }

    /// <summary>
    /// Starts a server on the given port and waits until it accepts connections.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <returns>The running server.</returns>
    public static TestSmtpServer Start(int port)
    {
        var smtpServer = new TestSmtpServer(port);

        var serverTask = smtpServer.server.StartAsync(smtpServer.cancellation.Token);

        // the listener is opened in the background, wait until it accepts connections
        var deadline = DateTime.UtcNow.AddSeconds(10);

        while (true)
        {
            if (serverTask.IsFaulted)
            {
                serverTask.GetAwaiter().GetResult();
            }

            try
            {
                using var client = new TcpClient();
                client.Connect(IPAddress.Loopback, port);
                return smtpServer;
            }
            catch (SocketException) when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(50);
            }
        }
    }

    /// <summary>
    /// Removes all received mails.
    /// </summary>
    public void ClearReceivedEmail()
    {
        lock (this.receivedEmail)
        {
            this.receivedEmail.Clear();
        }
    }

    /// <summary>
    /// Stops the server, the port is free again afterwards.
    /// </summary>
    public void Stop()
    {
        if (this.cancellation.IsCancellationRequested)
        {
            return;
        }

        this.server.Shutdown();
        this.cancellation.Cancel();

        try
        {
            this.server.ShutdownTask.Wait(TimeSpan.FromSeconds(10));
        }
        catch (AggregateException)
        {
            // cancelled sessions
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.Stop();
        this.cancellation.Dispose();
    }

    /// <summary>
    /// Stores the received mails in the server.
    /// </summary>
    private sealed class Store(TestSmtpServer owner) : MessageStore
    {
        /// <inheritdoc />
        public override async Task<SmtpResponse> SaveAsync(
            ISessionContext context,
            IMessageTransaction transaction,
            ReadOnlySequence<byte> buffer,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(buffer.ToArray());

            var message = await MimeMessage.LoadAsync(stream, cancellationToken);

            lock (owner.receivedEmail)
            {
                owner.receivedEmail.Add(message);
            }

            return SmtpResponse.Ok;
        }
    }
}
