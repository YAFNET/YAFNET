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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace YAF.RazorPages;

/// <summary>
/// Configures the StaticFileOptions type.
/// </summary>
public class StaticFilePostConfigureOptions : IPostConfigureOptions<StaticFileOptions>
{
    /// <summary>
    /// The environment
    /// </summary>
    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Initializes the StaticFilePostConfigureOptions.
    /// </summary>
    public StaticFilePostConfigureOptions(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    /// <summary>
    /// Invoked to configure a TOptions instance.
    /// </summary>
    /// <param name="name">The name of the options instance being configured.</param>
    /// <param name="options">The options instance to configured.</param>
    public void PostConfigure(string name, StaticFileOptions options)
    {
        options = options ?? throw new ArgumentNullException(nameof(options));

        // Basic initialization in case the options weren't initialized by any other component
#pragma warning disable S1656
        options.ContentTypeProvider = options.ContentTypeProvider;
#pragma warning restore S1656

        if (options.FileProvider == null && this.environment.WebRootFileProvider == null)
        {
            throw new InvalidOperationException("Missing FileProvider.");
        }

        options.FileProvider ??= this.environment.WebRootFileProvider;

        const string basePath = "wwwroot";

        var filesProvider = new ManifestEmbeddedFileProvider(this.GetType().Assembly, basePath);
        options.FileProvider = new CompositeFileProvider(options.FileProvider, filesProvider);
    }
}