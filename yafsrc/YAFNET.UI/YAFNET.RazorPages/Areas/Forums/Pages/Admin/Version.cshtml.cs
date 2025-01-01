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

namespace YAF.Pages.Admin;

using YAF.Core.Extensions;
using YAF.Core.Services;

/// <summary>
/// The Admin Restart App Page.
/// </summary>
public class VersionModel : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionModel"/> class.
    /// </summary>
    public VersionModel()
        : base("ADMIN_Version", ForumPages.Admin_Version)
    {
    }

    /// <summary>
    /// Gets or sets the latest version.
    /// </summary>
    [TempData]
    public string LatestVersion { get; set; }

    /// <summary>
    /// Gets or sets the running version.
    /// </summary>
    [TempData]
    public string RunningVersion { get; set; }

    /// <summary>
    /// Gets or sets the download url.
    /// </summary>
    [TempData]
    public string DownloadUrl { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether show upgrade.
    /// </summary>
    [TempData]
    public bool ShowUpgrade { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_VERSION", "TITLE"), string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        try
        {
            var version = this.Get<IDataCache>().GetOrSet(
                "LatestVersion",
                () => this.Get<ILatestInformationService>().GetLatestVersionAsync().Result,
                TimeSpan.FromDays(1));

            string lastVersion = version.Version;
            var lastVersionDate = (DateTime)version.VersionDate;

            this.LatestVersion = this.GetTextFormatted(
                "LATEST_VERSION",
                lastVersion,
                this.Get<IDateTimeService>().FormatDateShort(lastVersionDate));

            this.ShowUpgrade = lastVersionDate.ToUniversalTime().Date > this.Get<BoardInfo>().AppVersionDate.ToUniversalTime().Date;

            this.DownloadUrl = version.UpgradeUrl;
        }
        catch (Exception)
        {
            this.LatestVersion = string.Empty;
        }

        this.RunningVersion = this.GetTextFormatted(
            "RUNNING_VERSION",
            this.Get<BoardInfo>().AppVersionName,
            this.Get<IDateTimeService>().FormatDateShort(this.Get<BoardInfo>().AppVersionDate));

        return this.Page();
    }
}