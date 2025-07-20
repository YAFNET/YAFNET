/* Yet Another Forum.NET
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

using System;

namespace YAF.Core.Middleware;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Class RobotsTxtMiddleware.
/// Implements the <see cref="IHaveServiceLocator" />
/// </summary>
/// <seealso cref="IHaveServiceLocator" />
public class RobotsTxtMiddleware : IHaveServiceLocator
{
    /// <summary>
    /// The default robots.txt content
    /// </summary>
    private const string Default =
        @"User-Agent: *\nAllow: /";

    private readonly string environmentName;

    /// <summary>
    /// The next.
    /// </summary>
    private readonly RequestDelegate next;

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RobotsTxtMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next.</param>
    /// <param name="environmentName">Name of the environment.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public RobotsTxtMiddleware(RequestDelegate next,
        string environmentName,
        IServiceLocator serviceLocator)
    {
        this.next = next;
        this.environmentName = environmentName;
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Invoke as an asynchronous operation.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/robots.txt"))
        {
            var generalRobotsTxt = Path.Combine(BoardContext.Current.Get<BoardInfo>().WebRootPath, "resources", "robots.txt");
            var environmentRobotsTxt = Path.Combine(BoardContext.Current.Get<BoardInfo>().WebRootPath, "resources", $"robots.{this.environmentName}.txt");

            string output;

            // try environment first
            if (File.Exists(environmentRobotsTxt))
            {
                output = await File.ReadAllTextAsync(environmentRobotsTxt);
            }
            // then robots.txt
            else if (File.Exists(generalRobotsTxt))
            {
                output = await File.ReadAllTextAsync(generalRobotsTxt);
            }
            // then just a general default
            else
            {
                output = Default;
            }

            // replace default sitemap url
            if (output.Contains("https://www.mydomain.com/", StringComparison.CurrentCultureIgnoreCase))
            {
                output = output.Replace("https://www.mydomain.com", $"{this.Get<BoardInfo>().ForumBaseUrl}");
            }

            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(output);
        }
        else
        {
            await this.next(context);
        }
    }
}