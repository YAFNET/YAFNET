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

using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin Banned IP Page.
/// </summary>
public class BannedIpsModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<BannedIP> List { get; set; }

    /// <summary>
    /// Gets or sets the search input.
    /// </summary>
    /// <value>The search input.</value>
    [BindProperty]
    public string SearchInput { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BannedIpsModel"/> class.
    /// </summary>
    public BannedIpsModel()
        : base("ADMIN_BANNEDIP", ForumPages.Admin_BannedIps)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BANNEDIP", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
         this.BindData();
    }

    /// <summary>
    /// Handles the Click event of the Search control.
    /// </summary>
    public void OnPostSearch()
    {
        this.BindData();
    }

    /// <summary>
    /// Clear Search
    /// </summary>
    public void OnPostClear()
    {
        this.SearchInput = string.Empty;
        this.BindData();
    }

    /// <summary>
    /// The page size on selected index changed.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Export Banned IP(s)
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostExportAsync()
    {
        var bannedIps = await this.GetRepository<BannedIP>().GetByBoardIdAsync();

        const string fileName = "BannedIpsExport.txt";

        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        bannedIps.ForEach(
            ip =>
            {
                streamWriter.Write(ip.Mask);
                streamWriter.Write(streamWriter.NewLine);
            });

        streamWriter.Close();

        return this.File(stream.ToArray(), "application/vnd.text", fileName);
    }

    /// <summary>
    /// Delete Item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var ipAddress = this.GetIpFromId(id);

        await this.GetRepository<BannedIP>().DeleteByIdAsync(id);

        this.SearchInput = string.Empty;

        this.BindData();

        if (this.PageBoardContext.BoardSettings.LogBannedIP)
        {
            this.Get<ILogger<BannedIpsModel>>().Log(
                this.PageBoardContext.PageUserID,
                " YAF.Pages.Admin.bannedip",
                $"IP or mask {ipAddress} was deleted by {this.PageBoardContext.PageUser.DisplayOrUserName()}.",
                EventLogTypes.IpBanLifted);
        }

        return this.PageBoardContext.Notify(this.GetTextFormatted("MSG_REMOVEBAN_IP", ipAddress), MessageTypes.success);
    }

    /// <summary>
    /// Opens the import dialog.
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/BannedIpImport",
                   ViewData = new ViewDataDictionary<ImportModal>(
                       this.ViewData,
                       new ImportModal())
               };
    }

    /// <summary>
    /// Opens the add new Entry dialog.
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/BannedIpEdit",
                                         ViewData = new ViewDataDictionary<BannedEmailEditModal>(
                                             this.ViewData,
                                             new BannedEmailEditModal { Id = 0 })
                                     };
    }

    /// <summary>
    /// Opens the edit dialog.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var banned = this.GetRepository<BannedIP>().GetById(id);

        return new PartialViewResult
               {
                   ViewName = "Dialogs/BannedIpEdit",
                   ViewData = new ViewDataDictionary<BannedEmailEditModal>(
                       this.ViewData,
                       new BannedEmailEditModal
                       {
                           Id = banned.ID,
                           Mask = banned.Mask, BanReason = banned.Reason
                       })
               };
    }

    /// <summary>
    /// Import most recent Ip Addresses from AbuseIpDb.com
    /// </summary>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostImportMostRecentAsync()
    {
        if (this.PageBoardContext.BoardSettings.AbuseIpDbApiKey.IsNotSet())
        {
            return this.Page();
        }

        var blackList = await this.Get<IIpInfoService>()
            .GetAbuseIpDbBlackListAsync(this.PageBoardContext.BoardSettings.AbuseIpDbApiKey, 55, 10000);

        var importedCount = await this.Get<IDataImporter>()
            .BannedIpAddressesImportAsync(this.PageBoardContext.PageBoardID, this.PageBoardContext.PageUserID,
                blackList.Data);

        this.BindData();

        return this.PageBoardContext.Notify(importedCount > 0
            ? string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS"), importedCount)
            : this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_NOTHING"), MessageTypes.success);
    }

    /// <summary>
    /// Helper to get mask from ID.
    /// </summary>
    /// <param name="id">The ID.</param>
    /// <returns>
    /// Returns the IP
    /// </returns>
    private string GetIpFromId(int id)
    {
        return this.GetRepository<BannedIP>().GetById(id).Mask;
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));

        var searchText = this.SearchInput;

        List<BannedIP> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<BannedIP>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.Mask == searchText,
                this.PageBoardContext.PageIndex,
                this.Size);
        }
        else
        {
            bannedList = this.GetRepository<BannedIP>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageIndex,
                this.Size);
        }

        this.List = bannedList;
    }
}