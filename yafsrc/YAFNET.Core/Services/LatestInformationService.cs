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

using System.Collections.Generic;

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
    /// The release package name for each database provider.
    /// </summary>
    private static readonly Dictionary<string, string> PackageNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Microsoft.Data.SqlClient"] = "SqlServer",
        ["MySql.Data.MySqlClient"] = "MySql",
        ["Npgsql Data Provider"] = "PostgreSQL",
        ["System.Data.SQLite"] = "Sqlite",
        ["Microsoft.Data.Sqlite"] = "SqliteData"
    };

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
            var client = this.Get<IHttpClientFactory>().CreateClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd("YAF.NET");

#pragma warning disable S1075
            const string url = "https://api.github.com/repos/YAFNET/YAFNET/releases";

            var releases = await client.GetFromJsonAsync<List<GitHubRelease>>(url);

            var release = releases.First(x => x.TagName.StartsWith("v4"));

            version.UpgradeUrl = this.GetDownloadUrl(release) ?? "https://yetanotherforum.net/downloads";
            version.VersionDate = release.PublishedAt;
            version.Version = release.TagName.Replace("v", string.Empty);
#pragma warning restore S1075
        }
        catch (Exception x)
        {
            this.Get<ILogger<LatestInformationService>>().Error(x, "Exception In LatestInformationService");
        }

        return version;
    }

    /// <summary>
    /// Gets the download url of the release package for the database this forum runs on
    /// (e.g. YAF.SqlServer-v4.0.11.zip), or the release page when there is no matching package.
    /// </summary>
    /// <param name="release">The release.</param>
    /// <returns>The download url, or null when the release has no url at all.</returns>
    private string GetDownloadUrl(GitHubRelease release)
    {
        string providerName = null;

        try
        {
            providerName = this.Get<IDbAccess>().Information.ProviderName;
        }
        catch (Exception x)
        {
            this.Get<ILogger<LatestInformationService>>().Error(x, "Unable to get the database provider");
        }

        if (providerName != null && PackageNames.TryGetValue(providerName, out var packageName))
        {
            var fileName = $"YAF.{packageName}-{release.TagName}.zip";

            var asset = release.Assets?.Find(
                a => string.Equals(a.Name, fileName, StringComparison.OrdinalIgnoreCase));

            if (asset?.BrowserDownloadUrl.IsSet() == true)
            {
                return asset.BrowserDownloadUrl;
            }
        }

        return release.HtmlUrl.IsSet() ? release.HtmlUrl : null;
    }
}