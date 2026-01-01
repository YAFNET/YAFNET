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

using System.Linq;
using System.Threading.Tasks;

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using YAF.Core.Extensions;
using YAF.Core.Helpers;
using YAF.Types.Modals;
using YAF.Types.Models;

/// <summary>
/// The Admin Banned Countries Page.
/// </summary>
public class BannedCountriesModel : AdminPage
{
    /// <summary>
    /// Gets or sets the list.
    /// </summary>
    /// <value>The list.</value>
    [BindProperty]
    public List<BannedCountry> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BannedCountriesModel"/> class.
    /// </summary>
    public BannedCountriesModel()
        : base("ADMIN_BANNED_COUNTRIES", ForumPages.Admin_BannedCountries)
    {
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_BANNED_COUNTRIES", "TITLE"), string.Empty);
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
    /// Handles the Click event of the Search control.
    /// </summary>
    public void OnPost()
    {
        this.BindData();
    }

    /// <summary>
    /// Delete Item.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await this.GetRepository<BannedCountry>().DeleteByIdAsync(id);

        this.BindData();

        return this.PageBoardContext.Notify(
            this.GetText("ADMIN_BANNED_COUNTRIES", "MSG_REMOVEBAN_NAME"),
            MessageTypes.success);
    }

    /// <summary>
    /// Opens the add new Entry dialog.
    /// </summary>
    /// <returns>PartialViewResult.</returns>
    public PartialViewResult OnGetAdd()
    {
        var countries = StaticDataHelper.Countries().ToList();

        return new PartialViewResult
        {
            ViewName = "Dialogs/BannedCountryEdit",
            ViewData = new ViewDataDictionary<BannedCountryModal>(
                this.ViewData,
                new BannedCountryModal { Id = 0, Countries = countries })
        };
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData()
    {
        this.PageSizeList = new SelectList(StaticDataHelper.PageEntries(), nameof(SelectListItem.Value),
            nameof(SelectListItem.Text));

        this.List = this.GetRepository<BannedCountry>().GetPaged(
            x => x.BoardID == this.PageBoardContext.PageBoardID,
            this.PageBoardContext.PageIndex,
            this.Size);
    }
}