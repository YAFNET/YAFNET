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

using YAF.Types.Models;

/// <summary>
/// Middleware to Check For Banned Countries
/// </summary>
public class CheckBannedCountries : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckBannedCountries"/> class.
    /// </summary>
    /// <param name="next">
    /// The next.
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public CheckBannedCountries(RequestDelegate next, IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
        this.requestDelegate = next;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Invokes the asynchronous.
    /// </summary>
    /// <param name="context">The context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!this.Get<IGeoIpCountryService>().DatabaseExists())
        {
            await this.requestDelegate(context);

            return;
        }

        List<string> bannedCountries;

        try
        {
            bannedCountries = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.BannedCountry,
                () => this.GetRepository<BannedCountry>().Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Select(x => x.Country.Trim()).ToList());
        }
        catch (Exception)
        {
            await this.requestDelegate(context);

            return;
        }

        if (bannedCountries == null)
        {
            await this.requestDelegate(context);

            return;
        }

        var ipAddress = context.GetUserRealIPAddress();

        var country = this.Get<IGeoIpCountryService>().GetCountry(ipAddress).CountryCode;

        if (country.IsNotSet())
        {
            await this.requestDelegate(context);

            return;
        }

        // check for this user in the list...
        if (!bannedCountries.Exists(row => row.Equals(country, StringComparison.InvariantCultureIgnoreCase)))
        {
            await this.requestDelegate(context);

            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    }
}