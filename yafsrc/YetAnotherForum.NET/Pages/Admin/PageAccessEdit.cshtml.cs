
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
using YAF.Types.Extensions;
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
    /// The pages only the host admin can use (they can't be granted to the admins of a board).
    /// </summary>
    private readonly static string[] HostPages = ForumPagesExtensions.HostAdminPages;

    /// <summary>
    /// The sections of the admin menu (title tag of the admin menu, icon, pages).
    /// </summary>
    public readonly static (string Tag, string Icon, string[] Pages)[] Sections =
    [
        ("ADMIN_ADMIN", "tachometer-alt", ["Admin_Admin"]),
        ("SETTINGS", "cogs", ["Admin_BoardAnnouncement", "Admin_Settings", "Admin_Forums", "Admin_EditForum",
            "Admin_EditCategory", "Admin_DeleteForum", "Admin_ReplaceWords", "Admin_BBCodes", "Admin_EditBBCode"]),
        ("SPAM_PROTECTION", "shield-alt", ["Admin_SpamLog", "Admin_SpamWords", "Admin_BannedEmails", "Admin_BannedIps",
            "Admin_BannedNames", "Admin_BannedUserAgents", "Admin_BannedCountries"]),
        ("USERSANDROLES", "users", ["Admin_ProfileDefinitions", "Admin_AccessMasks", "Admin_EditAccessMask", "Admin_Groups",
            "Admin_EditGroup", "Admin_Users", "Admin_EditUser", "Admin_RegisterUser", "Admin_Ranks", "Admin_EditRank",
            "Admin_Medals", "Admin_EditMedal", "Admin_Mail", "Admin_Digest"]),
        ("MAINTENANCE", "toolbox", ["Admin_Attachments", "Admin_Tags", "Admin_Prune", "Admin_Restore", "Admin_Pm",
            "Admin_EventLog"])
    ];

    /// <summary>
    /// The titles of the pages that are not in the admin menu (localization page, tag).
    /// </summary>
    private readonly static Dictionary<string, (string Page, string Tag)> PageTitles = new()
    {
        ["Admin_Settings"] = ("ADMINMENU", "ADMIN_BOARDSETTINGS"),
        ["Admin_BBCodes"] = ("ADMINMENU", "ADMIN_BBCODE"),
        ["Admin_BannedEmails"] = ("ADMINMENU", "ADMIN_BANNEDEMAIL"),
        ["Admin_BannedIps"] = ("ADMINMENU", "ADMIN_BANNEDIP"),
        ["Admin_BannedNames"] = ("ADMINMENU", "ADMIN_BANNEDNAME"),
        ["Admin_BannedUserAgents"] = ("ADMINMENU", "ADMIN_BANNED_USERAGENTS"),
        ["Admin_BannedCountries"] = ("ADMINMENU", "ADMIN_BANNED_COUNTRIES"),
        ["Admin_EditForum"] = ("ADMIN_EDITFORUM", "TITLE"),
        ["Admin_EditCategory"] = ("ADMIN_EDITCATEGORY", "TITLE"),
        ["Admin_DeleteForum"] = ("ADMIN_DELETEFORUM", "TITLE"),
        ["Admin_EditBBCode"] = ("ADMIN_BBCODE_EDIT", "TITLE"),
        ["Admin_EditAccessMask"] = ("ADMIN_EDITACCESSMASKS", "TITLE"),
        ["Admin_EditGroup"] = ("ADMIN_EDITGROUP", "TITLE"),
        ["Admin_EditUser"] = ("ADMIN_EDITUSER", "TITLE"),
        ["Admin_RegisterUser"] = ("ADMIN_REGUSER", "TITLE"),
        ["Admin_EditRank"] = ("ADMIN_EDITRANK", "TITLE"),
        ["Admin_EditMedal"] = ("ADMIN_EDITMEDAL", "TITLE")
    };

    /// <summary>
    /// The readable title of an admin page (the entry of the admin menu or the title of the page).
    /// </summary>
    /// <param name="pageName">The name of the page (e.g. Admin_Forums).</param>
    public string GetPageTitle(string pageName)
    {
        var (page, tag) = PageTitles.TryGetValue(pageName, out var title) ? title : ("ADMINMENU", pageName.ToUpperInvariant());

        // the titles of the edit pages have the name of the item ("Edit User "{0}"")
        var text = this.GetText(page, tag).Replace("{0}", string.Empty).Trim().Trim('"').Trim();

        return text.IsSet() ? text : pageName;
    }

    /// <summary>
    /// The index of the section of a page (the pages that are in no section are the last).
    /// </summary>
    /// <param name="pageName">The name of the page.</param>
    public static int GetSectionIndex(string pageName)
    {
        var index = Array.FindIndex(Sections, s => s.Pages.Contains(pageName));

        return index < 0 ? Sections.Length : index;
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

                // the pages of the host admin can't be granted (the list is sent by the client)
                if (HostPages.Contains(pageName))
                {
                    readAccess = false;
                }

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
        // save permissions to table - the pages of the host admin can't be granted (the list is sent by the client)
        this.AccessList.Where(userAccess => !HostPages.Contains(userAccess.PageName)).ToList().ForEach(
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
        var hostPages = HostPages;

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
        // in the order of the admin menu
        this.AccessList = [.. pagesAll.OrderBy(a => GetSectionIndex(a.PageName))
            .ThenBy(a => Array.FindIndex(Sections.ElementAtOrDefault(GetSectionIndex(a.PageName)).Pages ?? [], p => p == a.PageName))];
    }
}