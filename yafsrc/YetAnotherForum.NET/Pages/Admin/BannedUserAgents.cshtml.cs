/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using System.Collections.Generic;
using System.IO;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Extensions;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin Banned UserAgents Page.
/// </summary>
public class BannedUserAgentsModel : AdminPage
{
    [BindProperty]
    public List<BannedUserAgent> List { get; set; }

    [BindProperty]
    public string SearchInput{ get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BannedUserAgentsModel"/> class. 
    /// </summary>
    public BannedUserAgentsModel()
        : base("ADMIN_BANNED_USERAGENTS", ForumPages.Admin_BannedUserAgents)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_USERAGENTS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value), nameof(SelectListItem.Text));
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
    /// Handles the Click event of the Search control.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Export Banned Name(s)
    /// </summary>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostExport()
    {
        var bannedUserAgents = this.GetRepository<BannedUserAgent>()
            .Get(x => x.BoardID == this.PageBoardContext.PageBoardID);

        const string FileName = "BannedEmailsExport.txt";

        var stream = new MemoryStream();
        var streamWriter = new StreamWriter(stream);

        bannedUserAgents.ForEach(
            name =>
            {
                streamWriter.Write(name.UserAgent);
                streamWriter.Write(streamWriter.NewLine);
            });

        streamWriter.Close();

        return this.File(stream.ToArray(), "application/vnd.text", FileName);
    }

    public IActionResult OnPostDelete(int id)
    {
        this.GetRepository<BannedUserAgent>().DeleteById(id);

        this.SearchInput = string.Empty;

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_BANNED_USERAGENTS", "MSG_REMOVEBAN_NAME"),
            MessageTypes.success);
    }

    public PartialViewResult OnGetImport()
    {
        return new PartialViewResult
               {
                   ViewName = "Dialogs/BannedUserAgentImport",
                   ViewData = new ViewDataDictionary<ImportModal>(
                       this.ViewData,
                       new ImportModal())
               };
    }

    public PartialViewResult OnGetAdd()
    {
        return new PartialViewResult {
                                         ViewName = "Dialogs/BannedUserAgentEdit",
                                         ViewData = new ViewDataDictionary<BannedEmailEditModal>(
                                             this.ViewData,
                                             new BannedEmailEditModal { Id = 0 })
                                     };
    }

    public PartialViewResult OnGetEdit(int id)
    {
        // Edit
        var banned = this.GetRepository<BannedUserAgent>().GetById(id);

        return new PartialViewResult
               {
                   ViewName = "Dialogs/BannedUserAgentEdit",
                   ViewData = new ViewDataDictionary<BannedEmailEditModal>(
                       this.ViewData,
                       new BannedEmailEditModal
                       {
                           Id = banned.ID,
                           Mask = banned.UserAgent
                       })
               };
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        var searchText = this.SearchInput;

        List<BannedUserAgent> bannedList;

        if (searchText.IsSet())
        {
            bannedList = this.GetRepository<BannedUserAgent>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID && x.UserAgent == searchText,
                this.PageBoardContext.PageIndex,
                this.Size);
        }
        else
        {
            bannedList = this.GetRepository<BannedUserAgent>().GetPaged(
                x => x.BoardID == this.PageBoardContext.PageBoardID,
                this.PageBoardContext.PageIndex,
                this.Size);
        }

        this.List = bannedList;
    }
}