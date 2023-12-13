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

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using YAF.Core.Hubs;

namespace YAF.Core.Extensions;

/// <summary>
///     The Service Collection extensions.
/// </summary>
public static class ServiceCollectionExtensionsExtensions
{
    /// <summary>
    ///  Adds YAF.NET core services to the specified services collection.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddYAFCore(this IServiceCollection services)
    {
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>().AddScoped(
            x => x.GetRequiredService<IUrlHelperFactory>()
                .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));

        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        services.AddOptions();

        return services;
    }

    /// <summary>
    /// Adds the yaf install languages.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <returns>IServiceCollection.</returns>
    public static IServiceCollection AddYAFInstallLanguages(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] {
                new CultureInfo("ar"),
                new CultureInfo("zh-CN"),
                new CultureInfo("zh-TW"),
                new CultureInfo("cs"),
                new CultureInfo("da"),
                new CultureInfo("nl"),
                new CultureInfo("en-US"),
                new CultureInfo("et"),
                new CultureInfo("fi"),
                new CultureInfo("fr"),
                new CultureInfo("de-DE"),
                new CultureInfo("he"),
                new CultureInfo("it"),
                new CultureInfo("lt"),
                new CultureInfo("no"),
                new CultureInfo("fa"),
                new CultureInfo("pl"),
                new CultureInfo("pt"),
                new CultureInfo("ro"),
                new CultureInfo("ru"),
                new CultureInfo("sk"),
                new CultureInfo("es"),
                new CultureInfo("sv"),
                new CultureInfo("tr"),
                new CultureInfo("vi")
            };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });


        return services;
    }
}