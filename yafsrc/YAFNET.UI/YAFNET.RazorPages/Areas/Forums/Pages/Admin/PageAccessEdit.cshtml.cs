
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;
using System.Linq;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Admin Edit Admin Page Access Page
/// </summary>
public class PageAccessEditModel : AdminPage
{
    /// <summary>
    /// Gets or sets the access list.
    /// </summary>
    /// <value>The access list.</value>
    [BindProperty]
    public List<AdminPageUserAccess> AccessList { get; set; }

    /// <summary>
    /// Gets or sets the user identifier.
    /// </summary>
    /// <value>The user identifier.</value>
    [BindProperty]
    public int UserId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageAccessEditModel"/> class.
    /// </summary>
    public PageAccessEditModel()
        : base("ADMIN_PAGEACCESSEDIT", ForumPages.Admin_PageAccessEdit)
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
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_PAGEACCESSEDIT", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Cancels the click.
    /// </summary>
    public IActionResult OnPostCancel()
    {
        // get back to access admin list
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_PageAccessList);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? u)
    {
        if (!u.HasValue)
        {
            return this.Get<ILinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.BindData(u.Value);

        return this.Page();
    }

    /// <summary>
    /// Handles the Click event of the Save control.
    /// </summary>
    public IActionResult OnPostSave()
    {
        this.AccessList.ForEach(
            userAccess =>
            {
                var readAccess = userAccess.ReadAccess;
                var pageName = userAccess.PageName;

                if (readAccess || string.Equals("Admin_Admin", pageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // save it
                    this.GetRepository<AdminPageUserAccess>().Save(this.UserId, pageName);
                }
                else
                {
                    this.GetRepository<AdminPageUserAccess>().Delete(this.UserId, pageName);
                }
            });

        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.AdminPageAccess, this.UserId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_PageAccessList);
    }

    /// <summary>
    /// Grants all click.
    /// </summary>
    public IActionResult OnPostGrantAll()
    {
        // save permissions to table - checked only
        this.AccessList.ForEach(
            userAccess => this.GetRepository<AdminPageUserAccess>().Save(
                this.UserId,
                userAccess.PageName));

        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.AdminPageAccess, this.UserId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_PageAccessList);
    }

    /// <summary>
    /// The RevokeAll
    /// </summary>
    public IActionResult OnPostRevokeAll()
    {
        this.AccessList.ForEach(
            userAccess =>
            {
                var pageName = userAccess.PageName;

                // save it - admin index should be always available
                if (!string.Equals("Admin_Admin", pageName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.GetRepository<AdminPageUserAccess>().Delete(
                        this.UserId,
                        pageName);
                }
            });

        this.Get<IDataCache>().Remove(string.Format(Constants.Cache.AdminPageAccess, this.UserId));

        return this.Get<ILinkBuilder>().Redirect(ForumPages.Admin_PageAccessList);
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    /// <param name="userId"></param>
    private void BindData(int userId)
    {
        this.UserId = userId;

        var found = false;

        // Load the page access list.
        var dt = this.GetRepository<AdminPageUserAccess>().List(userId);

        // Get admin pages by page prefixes.
        var listPages = Enum.GetNames<ForumPages>().Where(e => e.StartsWith("Admin_"));

        // Initialize list with a helper class.
        var pagesAll = new List<AdminPageUserAccess>();

        // Protected host-admin pages
        var hostPages = new[] {
                                  "Admin_Boards", "Admin_HostSettings", "Admin_PageAccessList", "Admin_PageAccessEdit"
                              };

        // Iterate through all admin pages
        listPages.ToList().ForEach(
            listPage =>
            {
                if (dt != null && dt.Any(a => a.PageName == listPage && Array.TrueForAll(hostPages, s => s != a.PageName)))
                {
                    found = true;
                    pagesAll.Add(
                        new AdminPageUserAccess {
                                                    UserID = userId,
                                                    PageName = listPage,
                                                    ReadAccess = true
                                                });
                }

                // If it doesn't contain page for the user add it.
                if (!found && Array.TrueForAll(hostPages, s => s != listPage))
                {
                    pagesAll.Add(
                        new AdminPageUserAccess {
                                                    UserID = userId,
                                                    PageName = listPage,
                                                    ReadAccess = false
                                                });
                }

                // Reset flag in the end of the outer loop
                found = false;
            });

        // get admin pages list with access flags.
        this.AccessList = pagesAll;
    }
}