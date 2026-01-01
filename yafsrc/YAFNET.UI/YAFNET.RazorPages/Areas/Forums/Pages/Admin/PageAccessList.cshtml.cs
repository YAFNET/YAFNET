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
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Admin Page Admin Access list page
/// </summary>
public class PageAccessListModel : AdminPage
{
    /// <summary>
    /// Gets or sets the attachments.
    /// </summary>
    [BindProperty]
    public IEnumerable<User> List { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageAccessListModel"/> class.
    /// </summary>
    public PageAccessListModel()
        : base("ADMIN_PAGEACCESSLIST", ForumPages.Admin_PageAccessList)
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
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSLIST", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public Task OnGetAsync()
    {
        return this.BindDataAsync();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private async Task BindDataAsync()
    {
        // list admins but not host admins
        this.List = (await this.GetRepository<User>().ListAdminsAsync(this.PageBoardContext.PageBoardID))
            .Where(u => !u.UserFlags.IsHostAdmin);
    }
}