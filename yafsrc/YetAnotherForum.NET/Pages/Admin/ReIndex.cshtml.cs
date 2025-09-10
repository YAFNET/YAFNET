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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Mvc.Rendering;

using YAF.Core.Extensions;
using YAF.Types.Extensions;
using YAF.Types.Extensions.Data;
using YAF.Types.Interfaces.Data;
using YAF.Types.Objects;

/// <summary>
/// The Admin Database Maintenance Page
/// </summary>
public class ReIndexModel : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReIndexModel"/> class.
    /// </summary>
    public ReIndexModel()
        : base("ADMIN_REINDEX", ForumPages.Admin_ReIndex)
    {
    }

    /// <summary>
    /// Gets or sets the index statistics.
    /// </summary>
    /// <value>The index statistics.</value>
    [BindProperty]
    public string IndexStatistics { get; set; }

    /// <summary>
    /// Gets or sets the recovery modes.
    /// </summary>
    /// <value>The recovery modes.</value>
    public List<SelectListItem> RecoveryModes { get; set; }

    /// <summary>
    /// Gets or sets the recovery mode.
    /// </summary>
    /// <value>The recovery mode.</value>
    [BindProperty]
    public string RecoveryMode { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_REINDEX", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.BindData();
    }

    /// <summary>
    /// Gets the stats click.
    /// </summary>
    public async Task OnPostGetStatsAsync()
    {
        this.BindData();

        try
        {
            this.IndexStatistics = await this.Get<IDbAccess>().GetDatabaseFragmentationInfoAsync();
        }
        catch (Exception ex)
        {
            this.IndexStatistics = $"Failure: {ex}";
        }
    }

    /// <summary>
    /// Sets the Recovery mode
    /// </summary>
    public async Task<IActionResult> OnPostRecoveryModeAsync()
    {
        this.BindData();

        const string result = "Done";

        this.IndexStatistics = string.Empty;

        var stats = this.IndexStatistics = await this.Get<IDbAccess>().ChangeRecoveryModeAsync(this.RecoveryMode);

        return this.PageBoardContext.Notify(stats.IsSet() ? stats : result, MessageTypes.success);
    }

    /// <summary>
    /// Re-indexing the Database
    /// </summary>
    public async Task<IActionResult> OnPostReIndexAsync()
    {
        this.BindData();

        const string result = "Done";

        var stats = await this.Get<IDbAccess>().ReIndexDatabaseAsync(this.Get<BoardConfiguration>().DatabaseObjectQualifier);

        this.IndexStatistics = string.Empty;

        return this.PageBoardContext.Notify(stats.IsSet() ? stats : result, MessageTypes.success);
    }

    /// <summary>
    /// Mod By Touradg (herman_herman) 2009/10/19
    /// Shrinking Database
    /// </summary>
    public async Task<IActionResult> OnPostShrinkAsync()
    {
        this.BindData();

        try
        {
            var result = new StringBuilder();

            result.Append(await this.Get<IDbAccess>().ShrinkDatabaseAsync());

            result.Append("&nbsp;");

            result.AppendLine(this.GetTextFormatted("INDEX_SHRINK", await this.Get<IDbAccess>().GetDatabaseSizeAsync()));

            result.Append("&nbsp;");

            this.IndexStatistics = string.Empty;

            return this.PageBoardContext.Notify(result.ToString(), MessageTypes.success);
        }
        catch (Exception error)
        {
            return this.PageBoardContext.Notify(
                this.GetTextFormatted("INDEX_STATS_FAIL", error.Message),
                MessageTypes.danger);
        }
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.RecoveryModes = [
            new SelectListItem(this.GetText("ADMIN_REINDEX", "RECOVERY1"), "FULL", true),
            new SelectListItem(this.GetText("ADMIN_REINDEX", "RECOVERY2"), "SIMPLE"),
            new SelectListItem(this.GetText("ADMIN_REINDEX", "RECOVERY3"), "BULK_LOGGED")
        ];
    }
}