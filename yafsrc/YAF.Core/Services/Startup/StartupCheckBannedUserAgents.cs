/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

namespace YAF.Core.Services.Startup;

using System;
using System.Text.RegularExpressions;

using YAF.Types.Constants;
using YAF.Types.Models;

/// <summary>
/// The YAF check for banned IPs.
/// </summary>
public class StartupCheckBannedUserAgents : BaseStartupService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupCheckBannedUserAgents"/> class.
    /// </summary>
    /// <param name="dataCache">The data cache.</param>
    /// <param name="httpResponseBase">The HTTP response base.</param>
    /// <param name="httpRequestBase">The HTTP request base.</param>
    /// <param name="bannedUserAgentRepository">The banned user agent repository.</param>
    /// <param name="logger">The logger.</param>
    public StartupCheckBannedUserAgents(
        IDataCache dataCache,
        HttpResponseBase httpResponseBase,
        HttpRequestBase httpRequestBase,
        IRepository<BannedUserAgent> bannedUserAgentRepository,
        ILoggerService logger)
    {
        this.DataCache = dataCache;
        this.HttpResponseBase = httpResponseBase;
        this.HttpRequestBase = httpRequestBase;
        this.BannedUserAgentRepository = bannedUserAgentRepository;
        this.Logger = logger;
    }

    /// <summary>
    ///   Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets or sets HttpRequestBase.
    /// </summary>
    public HttpRequestBase HttpRequestBase { get; set; }

    /// <summary>
    /// Gets or sets the banned IP repository.
    /// </summary>
    /// <value>
    /// The banned IP repository.
    /// </value>
    public IRepository<BannedUserAgent> BannedUserAgentRepository { get; set; }

    /// <summary>
    ///   Gets or sets HttpResponseBase.
    /// </summary>
    public HttpResponseBase HttpResponseBase { get; set; }

    /// <summary>
    /// Gets or sets Logger.
    /// </summary>
    public ILoggerService Logger { get; set; }

    public override int Priority => 3000;

    /// <summary>
    ///   Gets the service name.
    /// </summary>
    
    protected override string ServiceName => "CheckBannedUserAgents_Init";

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    protected override bool RunService()
    {
        try
        {
            var userAgent = this.HttpRequestBase.UserAgent;

            if (userAgent.IsNotSet())
            {
                this.HttpResponseBase.Clear();

                this.HttpResponseBase.StatusCode = 500;

                return false;
            }

            var bannedUserAgents = this.DataCache.GetOrSet(
                Constants.Cache.BannedUserAgent,
                () => this.BannedUserAgentRepository.Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Select(x => x.UserAgent.Trim()).ToList());

            // check for this user in the list...
            if (bannedUserAgents == null || !bannedUserAgents.Any(row => MachUserAgent(userAgent, row)))
            {
                return true;
            }

            this.HttpResponseBase.Clear();

            this.HttpResponseBase.StatusCode = 500;

            return false;
        }
        catch (Exception)
        {
            //this.Logger.Error(exception, "Error during IP Check");

            // Fails if YAF is not installed
            return true;
        }
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