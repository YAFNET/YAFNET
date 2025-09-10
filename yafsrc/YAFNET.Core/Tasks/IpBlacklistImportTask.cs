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

namespace YAF.Core.Tasks;

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using YAF.Configuration;

/// <summary>
/// The Ip blacklist import task.
/// Implements the <see cref="YAF.Core.Tasks.LongBackgroundTask" />
/// </summary>
/// <seealso cref="YAF.Core.Tasks.LongBackgroundTask" />
public class IpBlacklistImportTask : LongBackgroundTask
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "IpBlacklistImportTask" /> class.
    /// </summary>
    public IpBlacklistImportTask()
    {
        this.RunPeriodMs = 300 * 1000;
        this.StartDelayMs = 30 * 1000;
    }

    /// <summary>
    ///   Gets TaskName.
    /// </summary>
    public static string TaskName => nameof(IpBlacklistImportTask);

    /// <summary>
    /// The run once.
    /// </summary>
    public override Task RunOnceAsync()
    {
        return this.ImportBannedIpListAsync();
    }

    /// <summary>
    /// Determines whether [is time to import blacklist].
    /// </summary>
    /// <returns><c>true</c> if [is time to import blacklist]; otherwise, <c>false</c>.</returns>
    private bool IsTimeToImportBlacklist()
    {
        var lastImport = DateTime.MinValue;

        if (this.Get<BoardSettings>().LastIpListImport.IsSet())
        {
            try
            {
                lastImport = Convert.ToDateTime(this.Get<BoardSettings>().LastIpListImport, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                lastImport = DateTime.MinValue;
            }
        }

        // haven't sent in X hours or more and it's 12 to 5 am.
        var importList = lastImport < DateTime.Now.AddHours(-24)
                         && DateTime.Now < DateTime.Today.AddHours(4);

        if (!importList)
        {
            return false;
        }

        // we're good to send -- update latest send so no duplication...
        this.Get<BoardSettings>().LastIpListImport = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(this.Get<BoardSettings>());

        return true;
    }

    /// <summary>
    /// Import List
    /// </summary>
    private async Task ImportBannedIpListAsync()
    {
        if (!this.IsTimeToImportBlacklist())
        {
            return;
        }

        if (this.Get<BoardSettings>().AbuseIpDbApiKey.IsNotSet())
        {
            return;
        }

        var blackList = await this.Get<IIpInfoService>()
            .GetAbuseIpDbBlackListAsync(this.Get<BoardSettings>().AbuseIpDbApiKey, 55, 10000);

        var importedCount = await this.Get<IDataImporter>()
            .BannedIpAddressesImportAsync(BoardContext.Current.PageBoardID, 0,
                blackList.Data);

        if (importedCount > 0)
        {
            this.Get<ILogger<IpBlacklistImportTask>>()
                .Info(string.Format(this.Get<ILocalization>().GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS"),
                    importedCount));
        }
    }
}