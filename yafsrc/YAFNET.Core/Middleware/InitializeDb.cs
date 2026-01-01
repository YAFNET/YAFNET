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

using Microsoft.Extensions.Configuration;

namespace YAF.Core.Middleware;

using System.Threading.Tasks;

using YAF.Core.Model;
using YAF.Types.Models;
using YAF.Types.Objects;

/// <summary>
/// The startup initialize Database.
/// </summary>
public class InitializeDb : IHaveServiceLocator
{
    /// <summary>
    /// The request delegate.
    /// </summary>
    private readonly RequestDelegate requestDelegate;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeDb"/> class.
    /// </summary>
    /// <param name="next">
    /// The next.
    /// </param>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public InitializeDb(RequestDelegate next, IServiceLocator serviceLocator)
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
        // init the modules and run them immediately...
        var baseModules = this.Get<IModuleManager<IBaseForumModule>>();
        baseModules.GetAll().ForEach(module => module.Init());

        var response = context.Response;

        var path  = context.Request.Path.ToString();

        if (!path.Contains("/Install") && !path.Contains("/Error"))
        {
            if (this.Get<IDataCache>().Get("Install") != null)
            {
                return;
            }

            if (this.Get<IConfiguration>().GetConnectionString("DefaultConnection") == null)
            {
                // attempt to create a connection string...
                response.Redirect(this.Get<BoardConfiguration>().Area.IsSet() ? $"/{this.Get<BoardConfiguration>().Area}/Install/Install"
                    : "/Install/Install");

                return;
            }

            // attempt to init the db...
            if (!this.Get<IDbAccess>().TestConnection(out var errorString))
            {
                // unable to connect to the DB...
                context.Session.SetString("StartupException", errorString);

                response.Redirect(ForumPages.Error.GetPageName());
                return;
            }

            // step 2: validate the database version...
            var versionType = this.GetRepository<Registry>().ValidateVersion(this.Get<BoardInfo>().AppVersion);

            switch (versionType)
            {
                case DbVersionType.Upgrade:
                    // Run Auto Upgrade
                    await this.Get<UpgradeService>().UpgradeAsync();
                    break;
                case DbVersionType.NewInstall:
                    // fake the board settings
                    BoardContext.Current.BoardSettings = new BoardSettings();
                    response.Redirect(this.Get<BoardConfiguration>().Area.IsSet()
                        ? $"/{this.Get<BoardConfiguration>().Area}/Install/Install"
                        : "/Install/Install");
                    return;
            }
        }

        await this.requestDelegate(context);
    }
}