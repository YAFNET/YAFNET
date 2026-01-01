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
using System.Threading;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Class WebTestingHostFactory.
/// Implements the <see cref="WebApplicationFactory{TProgram}" />
/// </summary>
/// <typeparam name="TProgram">The type of the t program.</typeparam>
/// <seealso cref="WebApplicationFactory{TProgram}" />
public class WebTestingHostFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    /// <summary>
    /// Override the CreateHost to build our HTTP host server.
    /// <see cref="T:Microsoft.AspNetCore.Hosting.IWebHostBuilder" /> will use <see cref="M:Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory`1.CreateServer(Microsoft.AspNetCore.Hosting.IWebHostBuilder)" /> instead.
    /// </summary>
    /// <param name="builder">The <see cref="T:Microsoft.Extensions.Hosting.IHostBuilder" /> used to create the host.</param>
    /// <returns>The <see cref="T:Microsoft.Extensions.Hosting.IHost" /> with the bootstrapped application.</returns>
    override protected IHost CreateHost(IHostBuilder builder)
    {
        // Create the host that is actually used by the
        // TestServer (In Memory).
        var testHost = base.CreateHost(builder);
        // configure and start the actual host using Kestrel.
        builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());
        var host = builder.Build();
        host.Start();

        // In order to clean up and properly dispose HTTP server
        // resources we return a composite host object that is
        // actually just a way to intercept the StopAsync and Dispose
        // call and relay to our HTTP host.
        return new CompositeHost(testHost, host);
    }

    /// <summary>
    /// Relay the call to both test host and kestrel host.
    /// Implements the <see cref="IHost" />
    /// </summary>
    /// <seealso cref="IHost" />
    public class CompositeHost : IHost
    {
        private readonly IHost testHost;

        private readonly IHost kestrelHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeHost"/> class.
        /// </summary>
        /// <param name="testHost">The test host.</param>
        /// <param name="kestrelHost">The kestrel host.</param>
        public CompositeHost(IHost testHost, IHost kestrelHost)
        {
            this.testHost = testHost;
            this.kestrelHost = kestrelHost;
        }

        /// <summary>
        /// Gets the services configured for the program (for example, using <see cref="M:HostBuilder.ConfigureServices(Action&lt;HostBuilderContext,IServiceCollection&gt;)" />).
        /// </summary>
        /// <value>The services.</value>
        public IServiceProvider Services => this.testHost.Services;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            this.testHost.Dispose();
            this.kestrelHost.Dispose();
        }

        /// <summary>
        /// Start as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">Used to abort program start.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await this.testHost.StartAsync(cancellationToken);
            await this.kestrelHost.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stop as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">Used to indicate when stop should no longer be graceful.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await this.testHost.StopAsync(cancellationToken);
            await this.kestrelHost.StopAsync(cancellationToken);
        }
    }
}