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

using System.Net.Http;

using Microsoft.Extensions.DependencyInjection;

namespace YAF.Tests.Utils;

using Microsoft.AspNetCore.Hosting;

using YAF.ViewComponents;

/// <summary>
/// 
/// </summary>
public class WebTestingHostFactoryUtils
{
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
        // Create the host factory with the App class as parameter and the
        // url we are going to use.
        var hostFactory = new WebTestingHostFactory<CultureSwitcherViewComponent>();

       return hostFactory
            // Override host configuration to mock stuff if required.
            .WithWebHostBuilder(
                builder =>
                {
                    // Setup the url to use.
                    builder.UseUrls(testSettings.TestForumUrl);
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IStartupFilter, CustomStartupFilter>(x => new CustomStartupFilter(ipAddress));
                    });
                })
            // Create the host using the CreateDefaultClient method.
            .CreateDefaultClient();
    }

}