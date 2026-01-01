
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

using YAF.Core.Extensions;
using YAF.Types.Models;

/// <summary>
/// The Admin Access Masks Page.
/// </summary>
public class AccessMasksModel : AdminPage
{
    /// <summary>
    /// Gets or sets the access mask list.
    /// </summary>
    /// <value>The access mask list.</value>
    [BindProperty]
    public IList<AccessMask> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AccessMasksModel"/> class.
    /// </summary>
    public AccessMasksModel()
        : base("ADMIN_ACCESSMASKS", ForumPages.Admin_AccessMasks)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ACCESSMASKS", "TITLE"));
    }

    /// <summary>
    /// Called when [post edit].
    /// </summary>
    /// <param name="maskId">The mask identifier.</param>
    /// <returns>IActionResult.</returns>
    public IActionResult OnPostEdit(int maskId)
    {
        // redirect to editing page
        return this.Get<ILinkBuilder>().Redirect(
            ForumPages.Admin_EditAccessMask,
            new {
                    i = maskId
                });
    }

    /// <summary>
    /// Called when [post delete].
    /// </summary>
    /// <param name="maskId">The mask identifier.</param>
    /// <returns>IActionResult.</returns>
    public async Task<IActionResult> OnPostDeleteAsync(int maskId)
    {
        var isInUse = await this.GetRepository<ForumAccess>().ExistsAsync(x => x.AccessMaskID == maskId)
                      || await this.GetRepository<UserForum>().ExistsAsync(x => x.AccessMaskID == maskId);

        // attempt to delete access masks
        if (isInUse)
        {
            // used masks cannot be deleted
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_ACCESSMASKS", "MSG_NOT_DELETE"),
                MessageTypes.warning);
        }

        await this.GetRepository<AccessMask>().DeleteByIdAsync(maskId);

        await this.BindDataAsync();

        return this.Page();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        // bind data
        return this.BindDataAsync();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        // list all access masks for this board
        this.List = await this.GetRepository<AccessMask>().GetByBoardIdAsync();
    }
}