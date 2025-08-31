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

using System.Linq;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using YAF.Configuration;

namespace YAF.RazorPages;

/// <summary>
/// Provides extension methods for the Microsoft.Extensions.DependencyInjection.IMvcCoreBuilder and Microsoft.Extensions.DependencyInjection.IMvcBuilder interfaces.
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Adds PineBlog Razor Pages services to the services collection.
    /// </summary>
    /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IMvcCoreBuilder.</param>
    /// <param name="env">The webhost environment.</param>
    public static IMvcCoreBuilder AddYafRazorPages(this IMvcCoreBuilder builder, IWebHostEnvironment env)
    {
        ConfigureServices(builder.Services, env);

        return builder;
    }

    /// <summary>
    /// Adds PineBlog Razor Pages services to the services collection.
    /// </summary>
    /// <param name="builder">The Microsoft.Extensions.DependencyInjection.IMvcBuilder.</param>
    /// <param name="env">The webhost environment.</param>
    public static IMvcBuilder AddYafRazorPages(this IMvcBuilder builder, IWebHostEnvironment env)
    {
        ConfigureServices(builder.Services, env);

        return builder;
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="env">The webhost environment.</param>
    private static void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
    {
        services.ConfigureOptions<StaticFilePostConfigureOptions>();

        if (!env.IsDevelopment())
        {
            return;
        }

        // Get wwwroot from nuget cache
        var dir = env.WebRootFileProvider.GetDirectoryContents("languages").ToList()[0].PhysicalPath;

        if (dir != null)
        {
            Config.WebRootPath = dir[..dir.IndexOf("languages", StringComparison.Ordinal)];
        }
    }
}