﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Services.Startup;

using YAF.Core.Model;
using YAF.Types.Constants;
using YAF.Types.Interfaces.Tasks;
using YAF.Types.Models;

/// <summary>
/// The startup initialize Database.
/// </summary>
public class StartupInitializeDb : BaseStartupService, ICriticalStartupService, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StartupInitializeDb"/> class.
    /// </summary>
    /// <param name="httpResponseBase">
    /// The http response base.
    /// </param>
    /// <param name="serviceLocator">
    /// The service Locator.
    /// </param>
    public StartupInitializeDb(
        HttpResponseBase httpResponseBase,
        IServiceLocator serviceLocator)
    {
        this.HttpResponseBase = httpResponseBase;
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets the ServiceLocator.
    /// </summary>
    /// <value>The service locator.</value>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    ///   Gets or sets HttpResponseBase.
    /// </summary>
    public HttpResponseBase HttpResponseBase { get; set; }

    /// <summary>
    ///     Gets the service name.
    /// </summary>
    override protected string ServiceName => "YafInitializeDb_Init";

    /// <summary>
    /// The run service.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    override protected bool RunService()
    {
        if (HttpContext.Current == null)
        {
            return true;
        }

        if (Config.ConnectionString == null)
        {
            // attempt to create a connection string...
            this.HttpResponseBase.Redirect($"{BoardInfo.ForumClientFileRoot}install/default.aspx");

            return false;
        }

        // attempt to init the db...
        if (!this.Get<IDbAccess>().TestConnection(out var errorString))
        {
            // unable to connect to the DB...
            this.Get<HttpSessionStateBase>()["StartupException"] = errorString;

            this.HttpResponseBase.Redirect($"{BoardInfo.ForumClientFileRoot}error.aspx");

            return false;
        }

        // step 2: validate the database version...
        var versionType = this.GetRepository<Registry>().ValidateVersion(BoardInfo.AppVersion);

        switch (versionType)
        {
            case DbVersionType.Current:
                return true;
            case DbVersionType.Upgrade:
                // Run Auto Upgrade
                this.Get<UpgradeService>().Upgrade();
                return false;
            case DbVersionType.NewInstall:
                this.HttpResponseBase.Redirect($"{BoardInfo.ForumClientFileRoot}install/default.aspx", true);
                return false;
            default:
                return false;
        }
    }
}