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

namespace YAF.Controls;

using System.Web.UI;
using System.Web.UI.HtmlControls;

using YAF.Web.Controls;
using YAF.Types.Models;

/// <summary>
/// The Header.
/// </summary>
public partial class AdminMenu : BaseUserControl
{
    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        this.AdminDropdown.CssClass = "nav-link dropdown-toggle";

        if (this.PageBoardContext.CurrentForumPage.IsAdminPage &&
            this.PageBoardContext.CurrentForumPage.PageType is not (ForumPages.Admin_HostSettings or ForumPages.Admin_Boards or
                ForumPages.Admin_EditBoard or ForumPages.Admin_PageAccessEdit or ForumPages.Admin_PageAccessList))
        {
            this.AdminDropdown.CssClass = "nav-link dropdown-toggle active";
        }

        this.RenderMenuItems();
    }

    /// <summary>
    /// Render Li and a Item
    /// </summary>
    /// <param name="holder">
    /// The holder.
    /// </param>
    /// <param name="cssClass">
    /// The CSS class.
    /// </param>
    /// <param name="linkText">
    /// The link text.
    /// </param>
    /// <param name="linkUrl">
    /// The link URL.
    /// </param>
    /// <param name="isActive">
    /// The is Active.
    /// </param>
    /// <param name="isDropDownToggle">
    /// The is Drop Down Toggle.
    /// </param>
    /// <param name="iconName">
    /// The icon Name.
    /// </param>
    private static void RenderMenuItem(
        Control holder,
        string cssClass,
        string linkText,
        string linkUrl,
        bool isActive,
        bool isDropDownToggle,
        string iconName)
    {
        var link = new ThemeButton
                       {
                           Text = linkText,
                           Icon = iconName,
                           Type = ButtonStyle.None,
                           CssClass = isActive ? $"{cssClass} active" : cssClass,
                           NavigateUrl = linkUrl,
                           DataToggle = isDropDownToggle ? "dropdown" : "tooltip"
                       };

        if (isDropDownToggle)
        {
            holder.Controls.Add(link);
        }
        else
        {
            var listItem = new HtmlGenericControl("li");
            listItem.Controls.Add(link);
            holder.Controls.Add(listItem);
        }
    }

    /// <summary>
    /// Renders the menu items.
    /// </summary>
    private void RenderMenuItems()
    {
        var pagesAccess = this.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.AdminPageAccess, this.PageBoardContext.PageUserID),
            () => this.GetRepository<AdminPageUserAccess>().List(this.PageBoardContext.PageUserID).ToList());

        // Admin Admin
        RenderMenuItem(
            this.MenuHolder,
            "dropdown-item",
            this.GetText("ADMINMENU", "ADMIN_ADMIN"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin),
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Admin,
            false,
            "tachometer-alt");

        // Admin - Settings Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_BoardAnnouncement" or "Admin_Settings" or "Admin_Forums" or
                         "Admin_ReplaceWords" or "Admin_BBCodes" or "Admin_Languages"))
        {
            this.RenderAdminSettings(pagesAccess);
        }

        // Admin - Spam Protection Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_SpamLog" or "Admin_SpamWords" or "Admin_BannedEmails" or "Admin_BannedIps" or
                         "Admin_BannedNames" or "Admin_BannedUserAgents"))
        {
            this.RenderAdminSpamProtection(pagesAccess);
        }

        // Admin - Users and Roles Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_ProfileDefinitions" or "Admin_AccessMasks" or "Admin_Groups" or "Admin_Ranks"
                         or "Admin_Users" or "Admin_Medals" or "Admin_Mail" or "Admin_Digest"))
        {
            this.RenderAdminUsersAndRoles(pagesAccess);
        }

        // Admin - Maintenance Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_Attachments" or "Admin_Tags" or "Admin_Prune" or "Admin_Restore" or
                         "Admin_TaskManager" or "Admin_EventLog" or "Admin_RestartApp"))
        {
            this.RenderAdminMaintenance(pagesAccess);
        }

        // Admin - Database Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_ReIndex" or "Admin_RunSql"))
        {
            this.RenderAdminDatabase(pagesAccess);
        }

        // Admin - Nntp Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName is "Admin_NntpRetrieve" or "Admin_NntpForums" or "Admin_NntpServers"))
        {
            this.RenderAdminNntp(pagesAccess);
        }

        // Admin - Upgrade Menu
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Version"))
        {
            this.RenderAdminUpgrade(pagesAccess);
        }

#if DEBUG
        // Admin - Test Data
        RenderMenuItem(
            this.MenuHolder,
            "dropdown-item",
            "Test Generator",
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_TestData),
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_TestData,
            false,
            "vial-virus");
#endif
    }

    /// <summary>
    /// Render Admin Settings Sub Menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminSettings(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "SETTINGS"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_BoardAnnouncement or ForumPages.Admin_Settings or
                ForumPages.Admin_Forums or ForumPages.Admin_EditForum or ForumPages.Admin_EditCategory or
                ForumPages.Admin_ReplaceWords or ForumPages.Admin_BBCodes or ForumPages.Admin_BBCode_Edit or
                ForumPages.Admin_Languages or ForumPages.Admin_EditLanguage,
            true,
            "cogs");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin Board Announcement
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BoardAnnouncement"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_BoardAnnouncement"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BoardAnnouncement),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_BoardAnnouncement,
                false,
                "bullhorn");
        }

        // Admin Settings
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Settings"))
        {
            RenderMenuItem(
                list,
                "dropdown-item dropdown",
                this.GetText("ADMINMENU", "admin_boardsettings"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Settings),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Settings,
                false,
                "cogs");
        }

        // Admin Forums
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Forums"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_forums"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Forums or ForumPages.Admin_EditForum or
                    ForumPages.Admin_EditCategory,
                false,
                "comments");
        }

        // Admin ReplaceWords
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_ReplaceWords"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_replacewords"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_ReplaceWords),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_ReplaceWords,
                false,
                "sticky-note");
        }

        // Admin BBCodes
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BBCodes"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_bbcode"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BBCodes),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_BBCodes or ForumPages.Admin_BBCode_Edit,
                false,
                "plug");
        }

        // Admin Languages
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Languages"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Languages"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Languages),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Languages or ForumPages.Admin_EditLanguage,
                false,
                "language");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin spam protection sub menu.
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminSpamProtection(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "Spam_Protection"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_SpamLog or ForumPages.Admin_SpamWords or
                ForumPages.Admin_BannedEmails or ForumPages.Admin_BannedIps or ForumPages.Admin_BannedNames or ForumPages.Admin_BannedUserAgents,
            true,
            "shield-alt");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin SpamLog
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_SpamLog"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_spamlog"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_SpamLog),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_SpamLog,
                false,
                "book");
        }

        // Admin SpamWords
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_SpamWords"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_SpamWords"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_SpamWords),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_SpamWords,
                false,
                "hand-paper");
        }

        // Admin BannedEmails
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedEmails"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_BannedEmail"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedEmails),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_BannedEmails,
                false,
                "hand-paper");
        }

        // Admin BannedIps
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedIps"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_BannedIp"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedIps),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_BannedIps,
                false,
                "hand-paper");
        }

        // Admin BannedNames
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedNames"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_BannedName"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedNames),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_BannedNames,
                false,
                "hand-paper");
        }

        // Admin BannedUserAgents
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedUserAgents"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "ADMIN_BANNED_USERAGENTS"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedUserAgents),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_BannedUserAgents,
                false,
                "user-secret");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin users and roles sub menu.
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminUsersAndRoles(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "UsersandRoles"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_ProfileDefinitions or ForumPages.Admin_AccessMasks or
                ForumPages.Admin_EditAccessMask or ForumPages.Admin_Groups or ForumPages.Admin_EditGroup or
                ForumPages.Admin_Ranks or ForumPages.Admin_Users or ForumPages.Admin_EditUser or
                ForumPages.Admin_EditRank or ForumPages.Admin_Medals or ForumPages.Admin_EditMedal or
                ForumPages.Admin_Mail or ForumPages.Admin_Digest,
            true,
            "users");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin ProfileDefinitions
        if ((this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ProfileDefinitions")) && !Config.IsDotNetNuke)
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_ProfileDefinitions"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_ProfileDefinitions),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_ProfileDefinitions,
                false,
                "id-card");
        }

        // Admin AccessMasks
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_AccessMasks"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_AccessMasks"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_AccessMasks),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_AccessMasks or ForumPages.Admin_EditAccessMask,
                false,
                "universal-access");
        }

        // Admin Groups
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Groups"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Groups"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Groups),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Groups or ForumPages.Admin_EditGroup,
                false,
                "users");
        }

        // Admin Users
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Users"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Users"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_EditUser or ForumPages.Admin_Users,
                false,
                "users");
        }

        // Admin Ranks
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Ranks"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Ranks"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Ranks),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Ranks or ForumPages.Admin_EditRank,
                false,
                "graduation-cap");
        }

        // Admin Medals
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Medals"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Medals"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Medals),
                this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Medals or ForumPages.Admin_EditMedal,
                false,
                "medal");
        }

        // Admin Mail
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Mail"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Mail"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Mail),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Mail,
                false,
                "at");
        }

        // Admin Digest
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Digest"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Digest"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Digest),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Digest,
                false,
                "envelope");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin Maintenance sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminMaintenance(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "Maintenance"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_Attachments or ForumPages.Admin_Tags or
                ForumPages.Admin_Prune or ForumPages.Admin_Restore or ForumPages.Admin_TaskManager or
                ForumPages.Admin_EventLog or ForumPages.Admin_RestartApp,
            true,
            "toolbox");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin Attachments
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Attachments"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Attachments"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Attachments),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Attachments,
                false,
                "paperclip");
        }

        // Admin Tags
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Tags"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Tags"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Tags),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Tags,
                false,
                "tags");
        }

        // Admin Prune
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Prune"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Prune"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Prune),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Prune,
                false,
                "trash");
        }

        // Admin Restore
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Restore"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Restore"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Restore),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Restore,
                false,
                "trash-restore");
        }

        // Admin Pm
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Pm"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_Pm"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Pm),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Pm,
                false,
                "envelope-square");
        }

        // Admin TaskManager
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_TaskManager"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_TaskManager"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_TaskManager),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_TaskManager,
                false,
                "tasks");
        }

        // Admin EventLog
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_EventLog"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_EventLog"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_EventLog),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_EventLog,
                false,
                "book");
        }

        // Admin RestartApp
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_RestartApp"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_RestartApp"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_RestartApp),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_RestartApp,
                false,
                "sync");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin database sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminDatabase(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "Database"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_ReIndex or ForumPages.Admin_RunSql,
            true,
            "database");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin ReIndex
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_ReIndex"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_ReIndex"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_ReIndex),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_ReIndex,
                false,
                "database");
        }

        // Admin RunSql
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_RunSql"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_RunSql"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_RunSql),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_RunSql,
                false,
                "database");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin NNTP sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminNntp(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "NNTP"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_NntpRetrieve or ForumPages.Admin_NntpForums or
                ForumPages.Admin_NntpServers,
            true,
            "newspaper");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin NntpServers
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_NntpServers"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_NntpServers"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_NntpServers),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_NntpServers,
                false,
                "newspaper");
        }

        // Admin NntpForums
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_NntpForums"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_NntpForums"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_NntpForums),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_NntpForums,
                false,
                "newspaper");
        }

        // Admin NntpRetrieve
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_NntpRetrieve"))
        {
            RenderMenuItem(
                list,
                "dropdown-item",
                this.GetText("ADMINMENU", "admin_NntpRetrieve"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_NntpRetrieve),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_NntpRetrieve,
                false,
                "newspaper");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }

    /// <summary>
    /// Render Admin Upgrade sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    private void RenderAdminUpgrade(IEnumerable<AdminPageUserAccess> pagesAccess)
    {
        var listItem = new HtmlGenericControl("li");

        listItem.Attributes.Add("class", "dropdown dropend");

        RenderMenuItem(
            listItem,
            "dropdown-item dropdown-toggle subdropdown-toggle",
            this.GetText("ADMINMENU", "Upgrade"),
            "#",
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Version,
            true,
            "download");

        var list = new HtmlGenericControl("ul");

        list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

        // Admin Version
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Version"))
        {
            RenderMenuItem(
                list,
                "dropdown-item dropdown",
                this.GetText("ADMINMENU", "admin_Version"),
                this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Version),
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Version,
                false,
                "info");
        }

        listItem.Controls.Add(list);

        this.MenuHolder.Controls.Add(listItem);
    }
}