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

using System;
using System.Net.Http;
using System.Threading;

using Microsoft.AspNetCore.Mvc.Testing;

using Microsoft.Extensions.DependencyInjection;

namespace YAF.Tests.Utils;

using Microsoft.AspNetCore.Hosting;

using YAF.ViewComponents;

/// <summary>
/// 
/// </summary>
public static class WebTestingHostFactoryUtils
{
    private readonly static Lock SyncRoot = new();

    /// <summary>
    /// The root host factory, shared by all tests, because the Kestrel host binds to a fixed url
    /// and the app keeps its Autofac container in a static.
    /// </summary>
    private static WebTestingHostFactory<CultureSwitcherViewComponent> hostFactory;

    private static WebApplicationFactory<CultureSwitcherViewComponent> configuredFactory;

    private static bool kestrelReady;

    /// <summary>
    /// Creates a new instance of an <see cref="HttpClient"/> that can be used to
    /// send <see cref="HttpRequestMessage"/> to the server. The base address of the <see cref="HttpClient"/>
    /// instance will be set to <c>http://localhost</c>.
    /// </summary>
    /// <param name="testSettings">The test settings.</param>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns></returns>
    public static HttpClient CreateClient(TestConfig testSettings, string ipAddress)
    {
        HttpClient client;

        lock (SyncRoot)
        {
            client = GetFactory(testSettings).CreateDefaultClient();
        }

        client.DefaultRequestHeaders.Add(FakeRemoteIpAddressMiddleware.HeaderName, ipAddress);

        return client;
    }

    /// <summary>
    /// Starts the shared host (only once), so the forum can be reached via the <see cref="TestConfig.TestForumUrl"/>.
    /// </summary>
    /// <param name="testSettings">The test settings.</param>
    /// <returns>The services of the running forum application.</returns>
    public static IServiceProvider EnsureHostStarted(TestConfig testSettings)
    {
        lock (SyncRoot)
        {
            // accessing the server creates and starts the host
            var services = GetFactory(testSettings).Services;

            if (!kestrelReady)
            {
                // the kestrel host can start listening shortly after the start call returned,
                // so wait until the url actually answers, before tests navigate to it.
                WaitUntilListening(testSettings.TestForumUrl, TimeSpan.FromMinutes(2));

                kestrelReady = true;
            }

            return services;
        }
    }

    /// <summary>
    /// Waits until the url answers with any http response.
    /// </summary>
    /// <param name="url">The url.</param>
    /// <param name="timeout">The timeout.</param>
    /// <exception cref="TimeoutException">If the url does not answer within the timeout.</exception>
    private static void WaitUntilListening(string url, TimeSpan timeout)
    {
        using var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        using var client = new HttpClient(handler);
        client.Timeout = timeout;

        var deadline = DateTime.UtcNow + timeout;

        while (true)
        {
            try
            {
                using var response = client.Send(new HttpRequestMessage(HttpMethod.Get, url));
                return;
            }
            catch (HttpRequestException) when (DateTime.UtcNow < deadline)
            {
                // not listening yet
                Thread.Sleep(250);
            }
            catch (HttpRequestException ex)
            {
                throw new TimeoutException($"The test host did not start listening on {url} within {timeout}.", ex);
            }
        }
    }

    /// <summary>
    /// Gets the configured factory, must be called inside the lock.
    /// </summary>
    /// <param name="testSettings">The test settings.</param>
    /// <returns>The configured factory.</returns>
    private static WebApplicationFactory<CultureSwitcherViewComponent> GetFactory(TestConfig testSettings)
    {
        if (configuredFactory is null)
        {
            // Create the host factory with the App class as parameter and the
            // url we are going to use.
            hostFactory = new WebTestingHostFactory<CultureSwitcherViewComponent>();

            configuredFactory = hostFactory
                // Override host configuration to mock stuff if required.
                .WithWebHostBuilder(
                    builder =>
                    {
                        // Setup the url to use.
                        builder.UseUrls(testSettings.TestForumUrl);
                        builder.ConfigureServices(services =>
                        {
                            services.AddSingleton<IStartupFilter, CustomStartupFilter>();
                        });
                    });
        }

        return configuredFactory;
    }

    /// <summary>
    /// Stops and disposes the shared host.
    /// </summary>
    public static void DisposeHost()
    {
        lock (SyncRoot)
        {
            // disposes the derived configured factory too
            hostFactory?.Dispose();

            hostFactory = null;
            configuredFactory = null;
            kestrelReady = false;
        }
    }
}