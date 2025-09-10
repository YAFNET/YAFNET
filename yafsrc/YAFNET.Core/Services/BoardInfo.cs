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

using Microsoft.AspNetCore.Hosting;

namespace YAF.Core.Services;

using System;

using YAF.Types.Objects;

/// <summary>
/// Class provides helper functions related to the forum path and URLs as well as forum version information.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BoardInfo"/> class.
/// </remarks>
/// <param name="serviceLocator">The service locator.</param>
public class BoardInfo(IServiceLocator serviceLocator) : IHaveServiceLocator
{
    /// <summary>
    /// Gets or sets the ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; } = serviceLocator;

    /// <summary>
    /// Gets the Current YAF Application Version string
    /// </summary>
    public string AppVersionName => this.Get<BoardInfo>().AppVersionNameFromCode();

    /// <summary>
    /// Gets the Current YAF Database Version
    /// </summary>
    public int AppVersion { get; set; } = 1001;

    /// <summary>
    /// Gets the Current YAF Build Date
    /// </summary>
    public DateTime AppVersionDate { get; set; } = new (2025, 09, 03, 09, 14, 00, DateTimeKind.Utc);

    /// <summary>
    /// Creates a string that is the YAF Application Version from a long value
    /// </summary>
    /// <returns>
    /// Application Version String
    /// </returns>
    public string AppVersionNameFromCode()
    {
        var version = new YafVersion
        {
            Major = 4,
            Minor = 0,
            Build = 2,
            ReleaseType = ReleaseType.BETA,
            ReleaseNumber = 0
        };

        var versionString = new StringBuilder();

        versionString.Append($"{version.Major}.{version.Minor}.{version.Build}");

        if (version.ReleaseType == ReleaseType.Regular)
        {
            return versionString.ToString();
        }

        var number = version.ReleaseNumber >= 1
                         ? version.ReleaseNumber.ToString()
                         : this.Get<BoardInfo>().AppVersionDate.ToString("yyyyMMddHHmm");

        versionString.Append($" {version.ReleaseType.ToString().ToUpper()} {number}");

        return versionString.ToString();
    }

    /// <summary>
    /// Helper function that creates the URL to the themes folder.
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>
    /// Returns the URL including the themes path
    /// </returns>
    public string GetUrlToContentThemes(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        return this.Get<IUrlHelper>().Content($"~/css/themes/{resourceName}");
    }

    /// <summary>
    /// Helper function that creates the URL to the css folder.
    /// </summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>System.String.</returns>
    public string GetUrlToCss(string resourceName)
    {
        ArgumentNullException.ThrowIfNull(resourceName);

        return this.Get<IUrlHelper>().Content($"~/css/{resourceName}");
    }

    /// <summary>
    /// Gets complete application external (client-side) URL of the forum. (e.g. http://domain.com/forum
    /// </summary>
    public string ForumBaseUrl
    {
        get
        {
            var baseUrlMask = this.Get<IHttpContextAccessor>().HttpContext?.Request.BaseUrl();

            return TreatBaseUrl(baseUrlMask);
        }
    }

    /// <summary>
    /// Gets the web root path.
    /// </summary>
    /// <value>The web root path.</value>
    public string WebRootPath => Config.WebRootPath.IsSet() ? Config.WebRootPath : this.Get<IWebHostEnvironment>().WebRootPath;

    /// <summary>
    /// Treats the base URL.
    /// </summary>
    /// <param name="baseUrl">The base url.</param>
    /// <returns>
    /// The treat base url.
    /// </returns>
    private static string TreatBaseUrl(string baseUrl)
    {
        if (baseUrl.EndsWith('/'))
        {
            // remove ending slash...
            baseUrl = baseUrl[..^1];
        }

        return baseUrl;
    }
}