/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

#region Using

using System;

using YAF.Types.Constants;
using YAF.Types.Models;

#endregion

/// <summary>
/// The YAF check for banned IPs.
/// </summary>
public class StartupCheckBannedIps : BaseStartupService
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StartupCheckBannedIps" /> class.
    /// </summary>
    /// <param name="dataCache">The data cache.</param>
    /// <param name="httpResponseBase">The http response base.</param>
    /// <param name="httpRequestBase">The http request base.</param>
    /// <param name="bannedIpRepository">The banned IP repository.</param>
    /// <param name="logger">The logger.</param>
    public StartupCheckBannedIps(
        [NotNull] IDataCache dataCache,
        [NotNull] HttpResponseBase httpResponseBase,
        [NotNull] HttpRequestBase httpRequestBase,
        IRepository<BannedIP> bannedIpRepository,
        [NotNull] ILoggerService logger)
    {
        this.DataCache = dataCache;
        this.HttpResponseBase = httpResponseBase;
        this.HttpRequestBase = httpRequestBase;
        this.BannedIpRepository = bannedIpRepository;
        this.Logger = logger;
    }

    #endregion

    #region Properties

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
    public IRepository<BannedIP> BannedIpRepository { get; set; }

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
    [NotNull]
    protected override string ServiceName => "CheckBannedIps_Init";

    #endregion

    #region Methods

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
            var bannedIPs = this.DataCache.GetOrSet(
                Constants.Cache.BannedIP,
                () => this.BannedIpRepository.Get(x => x.BoardID == BoardContext.Current.PageBoardID)
                    .Select(x => x.Mask.Trim()).ToList());

            var ipToCheck = this.HttpRequestBase.ServerVariables["REMOTE_ADDR"];

            // check for this user in the list...
            if (bannedIPs == null || !bannedIPs.Any(row => IPHelper.IsBanned(row, ipToCheck)))
            {
                return true;
            }

            if (BoardContext.Current.BoardSettings.LogBannedIP)
            {
                this.Logger.Log(
                    null,
                    "Banned IP Blocked",
                    $@"Ending Response for Banned User at IP ""{ipToCheck}""",
                    EventLogTypes.IpBanDetected);
            }

            if (Config.BannedIpRedirectUrl.IsSet())
            {
                this.HttpResponseBase.Redirect(Config.BannedIpRedirectUrl);
            }

            this.HttpResponseBase.End();

            return false;
        }
        catch (Exception)
        {
            //this.Logger.Error(exception, "Error during IP Check");

            // Fails if YAF is not installed
            return true;
        }
    }

    #endregion
}