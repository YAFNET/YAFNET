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

using YAF.Types.Objects;

namespace YAF.Core.Services;

using System;
using System.Dynamic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

/// <summary>
/// Latest Information service class
/// </summary>
public class LatestInformationService : IHaveServiceLocator, ILatestInformationService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LatestInformationService"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public LatestInformationService(IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Gets the latest version information.
    /// </summary>
    /// <returns>Returns the LatestVersionInformation</returns>
    public async Task<dynamic> GetLatestVersionAsync()
    {
        dynamic version = new ExpandoObject();

        try
        {
            var client = new HttpClient(new HttpClientHandler());

            client.DefaultRequestHeaders.UserAgent.ParseAdd("YAF.NET");

#pragma warning disable S1075
            const string url = "https://api.github.com/repos/YAFNET/YAFNET/releases/latest";
#pragma warning restore S1075

            var json = await client.GetFromJsonAsync<GitHubRelease>(url);

            version.UpgradeUrl = "https://yetanotherforum.net/download";
            version.VersionDate = json.PublishedAt;
            version.Version = json.TagName.Replace("v", string.Empty);
        }
        catch (Exception x)
        {
            this.Get<ILogger<LatestInformationService>>().Error(x, "Exception In LatestInformationService");
        }

        return version;
    }
}