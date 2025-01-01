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

namespace YAF.Core.Middleware;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using YAF.Types.Models;

/// <summary>
/// Middleware to Check For Banned User Agents
/// </summary>
public class CheckBannedUserAgents : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckBannedUserAgents"/> class.
    /// </summary>
    /// <param name="next">
    /// The next.
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public CheckBannedUserAgents(RequestDelegate next, IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.requestDelegate = next;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The invoke.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task InvokeAsync(HttpContext context)
    {
        List<string> bannedUserAgents;

        try
        {
            bannedUserAgents = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BannedUserAgent,
                () => this.GetRepository<BannedUserAgent>().Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Select(x => x.UserAgent.Trim()).ToList());
        }
        catch (Exception)
        {
            await this.requestDelegate(context);

            return;
        }

        var userAgent = context.Request.Headers.UserAgent.ToString();

        if (userAgent.IsNotSet())
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
        }

        // check for this user in the list...
        if (bannedUserAgents == null || !bannedUserAgents.Exists(row => MachUserAgent(userAgent, row)))
        {
            await this.requestDelegate(context);

            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    }

    /// <summary>
    /// Matches the user agent.
    /// </summary>
    /// <param name="userAgent">The user agent.</param>
    /// <param name="pattern">The pattern.</param>
    /// <returns>bool.</returns>
    private static bool MachUserAgent(string userAgent, string pattern)
    {
        var check = Regex.Match(userAgent, pattern, RegexOptions.None, TimeSpan.FromMilliseconds(100));

        return check.Success;
    }
}