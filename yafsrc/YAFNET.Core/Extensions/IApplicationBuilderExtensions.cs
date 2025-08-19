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

using Autofac.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using YAF.Core.Middleware;
using YAF.Types;
using YAF.Types.Objects;

namespace YAF.Core.Extensions;

/// <summary>
/// Extensions for the <see cref="IApplicationBuilder"/>.
/// </summary>
public static class IApplicationBuilderExtensions
{
    /// <summary>
    /// Registers the autofac.
    /// </summary>
    /// <param name="app">The application.</param>
    public static void RegisterAutofac(this IApplicationBuilder app)
    {
        GlobalContainer.AutoFacContainer = app.ApplicationServices.GetAutofacRoot();

        ServiceLocatorAccess.CurrentServiceProvider = GlobalContainer.AutoFacContainer.Resolve<IServiceLocator>();
    }

    /// <summary>
    /// Uses the yaf core.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="serviceLocator">The service locator.</param>
    public static void UseYafCore(this IApplicationBuilder app, IServiceLocator serviceLocator)
    {
        app.UseAntiXssMiddleware();

        app.UseSecurityHeader(serviceLocator.Get<BoardConfiguration>());

        app.UseStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = ctx =>
            {
                ctx.Context.Response.Headers.CacheControl = "max-age: 31536000";
            }
        });

        app.UseSession();

        if (serviceLocator.Get<BoardConfiguration>().UseHttpsRedirection)
        {
            app.UseHttpsRedirection();
        }

        // Database Connection String
        Config.DatabaseObjectQualifier = serviceLocator.Get<BoardConfiguration>().DatabaseObjectQualifier;
        Config.SqlCommandTimeout = serviceLocator.Get<BoardConfiguration>().SqlCommandTimeout;
        Config.DatabaseOwner = serviceLocator.Get<BoardConfiguration>().DatabaseOwner;
        Config.BoardID = serviceLocator.Get<BoardConfiguration>().BoardID;
        Config.CategoryID = serviceLocator.Get<BoardConfiguration>().CategoryID;
        Config.UrlRewritingMode = serviceLocator.Get<BoardConfiguration>().UrlRewritingMode;

        // Legacy Settings
        Config.LegacyMembershipHashAlgorithmType =
            serviceLocator.Get<BoardConfiguration>().LegacyMembershipHashAlgorithmType;
        Config.LegacyMembershipHashCase = serviceLocator.Get<BoardConfiguration>().LegacyMembershipHashCase;
        Config.LegacyMembershipHashHex = serviceLocator.Get<BoardConfiguration>().LegacyMembershipHashHex;
        Config.UseRateLimiter = serviceLocator.Get<BoardConfiguration>().UseRateLimiter;

        app.UseMiddleware<InitializeDb>();

        app.Use(
            (httpContext, nextMiddleware) =>
            {
                // app init notification...
                serviceLocator.Get<IRaiseEvent>().RaiseIsolated(new HttpContextInitEvent(httpContext), null);

                return nextMiddleware();
            });

        app.UseMiddleware<CheckBannedIps>();
        app.UseMiddleware<CheckBannedUserAgents>();

        app.UseRouting();

        app.UseOutputCache();

        var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
        app.UseRequestLocalization(localizationOptions);

        app.UseResponseCaching();

        app.UseAuthentication();
        app.UseAuthorization();


        if (Config.UseRateLimiter)
        {
            app.UseRateLimiter();
        }
    }

    /// <summary>
    /// Uses the robots text.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="env">The env.</param>
    /// <returns>IApplicationBuilder.</returns>
    public static IApplicationBuilder UseRobotsTxt(
        this IApplicationBuilder builder,
        IWebHostEnvironment env
    )
    {
        return builder.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/robots.txt"), b =>
            b.UseMiddleware<RobotsTxtMiddleware>(env.EnvironmentName));
    }
}