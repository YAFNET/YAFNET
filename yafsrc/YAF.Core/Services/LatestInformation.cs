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

using ServiceStack.OrmLite.Base.Text;

namespace YAF.Core.Services;

using System;
using System.Dynamic;
using System.Net;

/// <summary>
/// LatestInformation service class
/// </summary>
public class LatestInformation : IHaveServiceLocator, ILatestInformation
{
    /// <summary>
    /// The major version of this forum, only releases of it are checked.
    /// </summary>
    private const int MajorVersion = 3;

    /// <summary>
    /// Initializes a new instance of the <see cref="LatestInformation"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public LatestInformation(IServiceLocator serviceLocator)
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
    public dynamic GetLatestVersion()
    {
        dynamic version = new ExpandoObject();

        try
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault |
                                                   SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            // releases/latest returns the newest release of any line (e.g. 4.x),
            // this branch has to compare itself with the newest release of its own major version
            var releasesJson = "https://api.github.com/repos/YAFNET/YAFNET/releases?per_page=100".GetJsonFromUrl(
                x => x.UserAgent = "YAF.NET");

            var tagPrefix = $"v{MajorVersion}.";

            var release = JsonArrayObjects.Parse(releasesJson)
                .Where(r => r.Child("tag_name")?.StartsWith(tagPrefix, StringComparison.OrdinalIgnoreCase) == true
                            && r.Child("draft") != "true" && r.Child("prerelease") != "true")
                .Select(r => new
                                 {
                                     Tag = r.Child("tag_name"),
                                     Date = DateTime.Parse(r.Child("published_at"), CultureInfo.InvariantCulture)
                                 })
                .OrderByDescending(r => r.Date)
                .First();

            var tagName = release.Tag;
            var date = DateTime.SpecifyKind(release.Date, DateTimeKind.Unspecified);

#pragma warning disable S1075 // the fixed download address of the project
            version.UpgradeUrl = "https://yetanotherforum.net/download";
#pragma warning restore S1075
            version.VersionDate = date;
            version.Version = tagName.Replace("v", string.Empty);
        }
        catch (Exception x)
        {
            this.Get<ILoggerService>().Error(x, "Exception In LatestInformationService");
        }

        return version;
    }
}