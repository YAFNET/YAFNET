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
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Types.Models;

/// <summary>
/// Middleware to Check For Banned IP
/// </summary>
public class CheckBannedIps : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckBannedIps"/> class.
    /// </summary>
    /// <param name="next">
    /// The next.
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public CheckBannedIps(RequestDelegate next, IServiceLocator serviceLocator)
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
        List<string> bannedIPs;

        try
        {
            bannedIPs = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BannedIP,
                () => this.GetRepository<BannedIP>().Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Select(x => x.Mask.Trim()).ToList());
        }
        catch (Exception)
        {
            await this.requestDelegate(context);

            return;
        }

        if (context.Connection.RemoteIpAddress is null)
        {
            await this.requestDelegate(context);

            return;
        }

        var ipToCheck = context.Connection.RemoteIpAddress.ToString();

        // check for this user in the list...
        if (bannedIPs is null || !bannedIPs.Exists(row => IPHelper.IsBanned(row, ipToCheck)))
        {
            await this.requestDelegate(context);

            return;
        }

        if (BoardContext.Current.BoardSettings.LogBannedIP)
        {
            this.Get<ILogger<CheckBannedIps>>().Log(
                null,
                "Banned IP Blocked",
                $"""
                 Ending Response for Banned PageUser at IP "{ipToCheck}"
                 """,
                EventLogTypes.IpBanDetected);
        }

        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    }
}