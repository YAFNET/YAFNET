/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utils;
    using YAF.Utils.Helpers;
    using YAF.Web.Controls;

    #endregion

    /// <summary>
    /// The Header.
    /// </summary>
    public partial class AdminMenu : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.AdminDropdown.CssClass = "nav-link dropdown-toggle";

            if (this.PageContext.CurrentForumPage.IsAdminPage
                && !(this.PageContext.ForumPageType == ForumPages.Admin_HostSettings
                     || this.PageContext.ForumPageType == ForumPages.Admin_Boards
                     || this.PageContext.ForumPageType == ForumPages.Admin_EditBoard
                     || this.PageContext.ForumPageType == ForumPages.Admin_PageAccessEdit
                     || this.PageContext.ForumPageType == ForumPages.Admin_PageAccessList
                     || this.PageContext.ForumPageType == ForumPages.Admin_Profiler))
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
            [NotNull] Control holder,
            [NotNull] string cssClass,
            [NotNull] string linkText,
            [NotNull] string linkUrl,
            [NotNull] bool isActive,
            [NotNull] bool isDropDownToggle,
            [NotNull] string iconName)
        {
            var icon = new Icon { IconName = iconName };

            var link = new HyperLink
            {
                Target = "_top",
                ToolTip = linkText,
                Text = $"{icon.RenderToString()}{linkText}",
                CssClass = isActive ? $"{cssClass} active" : cssClass
            };

            link.Attributes.Add("href", linkUrl);

            link.Attributes.Add("rel", "nofollow");

            link.Attributes.Add("data-toggle", isDropDownToggle ? "dropdown" : "tooltip");

            holder.Controls.Add(link);
        }

        /// <summary>
        /// Renders the menu items.
        /// </summary>
        private void RenderMenuItems()
        {
            var pagesAccess = this.Get<IDataCache>().GetOrSet(
                string.Format(Constants.Cache.AdminPageAccess, this.PageContext.PageUserID),
                () => this.GetRepository<AdminPageUserAccess>().List(this.PageContext.PageUserID).ToList());

            // Admin Admin
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item",
                this.GetText("ADMINMENU", "ADMIN_ADMIN"),
                BuildLink.GetLink(ForumPages.Admin_Admin),
                this.PageContext.ForumPageType == ForumPages.Admin_Admin,
                false,
                "tachometer-alt");

            // Admin - Settings Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BoardAnnouncement" || x.PageName == "Admin_Settings" ||
                     x.PageName == "Admin_Forums" || x.PageName == "Admin_ReplaceWords" ||
                     x.PageName == "Admin_BBCodes" || x.PageName == "Admin_Languages"))
            {
                this.RenderAdminSettings(pagesAccess);
            }

            // Admin - Spam Protection Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_SpamLog" || x.PageName == "Admin_SpamWords" ||
                     x.PageName == "Admin_BannedEmails" || x.PageName == "Admin_BannedIps" ||
                     x.PageName == "Admin_BannedNames"))
            {
                this.RenderAdminSpamProtection(pagesAccess);
            }

            // Admin - Users and Roles Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ProfileDefinitions" ||
                     x.PageName == "Admin_AccessMasks" ||
                     x.PageName == "Admin_Groups" ||
                     x.PageName == "Admin_Ranks" ||
                     x.PageName == "Admin_Users" ||
                     x.PageName == "Admin_Medals" ||
                     x.PageName == "Admin_Mail" ||
                     x.PageName == "Admin_Digest"))
            {
                this.RenderAdminUsersAndRoles(pagesAccess);
            }

            // Admin - Maintenance Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Prune" ||
                     x.PageName == "Admin_Restore" ||
                     x.PageName == "Admin_TaskManager" ||
                     x.PageName == "Admin_EventLog" ||
                     x.PageName == "Admin_RestartApp"))
            {
                this.RenderAdminMaintenance(pagesAccess);
            }

            // Admin - Database Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ReIndex" || x.PageName == "Admin_RunSql"))
            {
                this.RenderAdminDatabase(pagesAccess);
            }

            // Admin - Nntp Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_NntpRetrieve" ||
                     x.PageName == "Admin_NntpForums" ||
                     x.PageName == "Admin_NntpServers"))
            {
                this.RenderAdminNntp(pagesAccess);
            }

            // Admin - Upgrade Menu
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Version"))
            {
                this.RenderAdminUpgrade(pagesAccess);
            }
        }

        /// <summary>
        /// Render Admin Settings Sub Menu
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminSettings(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "SETTINGS"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_BoardAnnouncement ||
                this.PageContext.ForumPageType == ForumPages.Admin_Settings ||
                this.PageContext.ForumPageType == ForumPages.Admin_Forums ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditForum ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditCategory ||
                this.PageContext.ForumPageType == ForumPages.Admin_ReplaceWords ||
                this.PageContext.ForumPageType == ForumPages.Admin_BBCodes ||
                this.PageContext.ForumPageType == ForumPages.Admin_BBCode_Edit ||
                this.PageContext.ForumPageType == ForumPages.Admin_Languages ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditLanguage,
                true,
                "cogs");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin Board Announcement
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BoardAnnouncement"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_BoardAnnouncement"),
                    BuildLink.GetLink(ForumPages.Admin_BoardAnnouncement),
                    this.PageContext.ForumPageType == ForumPages.Admin_BoardAnnouncement,
                    false,
                    "bullhorn");
            }

            // Admin Settings
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Settings"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_boardsettings"),
                    BuildLink.GetLink(ForumPages.Admin_Settings),
                    this.PageContext.ForumPageType == ForumPages.Admin_Settings,
                    false,
                    "cogs");
            }

            // Admin Forums
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Forums"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_forums"),
                    BuildLink.GetLink(ForumPages.Admin_Forums),
                    this.PageContext.ForumPageType == ForumPages.Admin_Forums ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditForum ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditCategory,
                    false,
                    "comments");
            }

            // Admin ReplaceWords
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ReplaceWords"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_replacewords"),
                    BuildLink.GetLink(ForumPages.Admin_ReplaceWords),
                    this.PageContext.ForumPageType == ForumPages.Admin_ReplaceWords,
                    false,
                    "sticky-note");
            }

            // Admin BBCodes
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BBCodes"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_bbcode"),
                    BuildLink.GetLink(ForumPages.Admin_BBCodes),
                    this.PageContext.ForumPageType == ForumPages.Admin_BBCodes ||
                    this.PageContext.ForumPageType == ForumPages.Admin_BBCode_Edit,
                    false,
                    "plug");
            }

            // Admin Languages
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Languages"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Languages"),
                    BuildLink.GetLink(ForumPages.Admin_Languages),
                    this.PageContext.ForumPageType == ForumPages.Admin_Languages ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditLanguage,
                    false,
                    "language");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin spam protection sub menu.
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminSpamProtection(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "Spam_Protection"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_SpamLog ||
                this.PageContext.ForumPageType == ForumPages.Admin_SpamWords ||
                this.PageContext.ForumPageType == ForumPages.Admin_BannedEmails ||
                this.PageContext.ForumPageType == ForumPages.Admin_BannedIps ||
                this.PageContext.ForumPageType == ForumPages.Admin_BannedNames,
                true,
                "shield-alt");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin SpamLog
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_SpamLog"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_spamlog"),
                    BuildLink.GetLink(ForumPages.Admin_SpamLog),
                    this.PageContext.ForumPageType == ForumPages.Admin_SpamLog,
                    false,
                    "book");
            }

            // Admin SpamWords
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_SpamWords"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_SpamWords"),
                    BuildLink.GetLink(ForumPages.Admin_SpamWords),
                    this.PageContext.ForumPageType == ForumPages.Admin_SpamWords,
                    false,
                    "hand-paper");
            }

            // Admin BannedEmails
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BannedEmails"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_BannedEmail"),
                    BuildLink.GetLink(ForumPages.Admin_BannedEmails),
                    this.PageContext.ForumPageType == ForumPages.Admin_BannedEmails,
                    false,
                    "hand-paper");
            }

            // Admin BannedIps
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BannedIps"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_BannedIp"),
                    BuildLink.GetLink(ForumPages.Admin_BannedIps),
                    this.PageContext.ForumPageType == ForumPages.Admin_BannedIps,
                    false,
                    "hand-paper");
            }

            // Admin BannedNames
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_BannedNames"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_BannedName"),
                    BuildLink.GetLink(ForumPages.Admin_BannedNames),
                    this.PageContext.ForumPageType == ForumPages.Admin_BannedNames,
                    false,
                    "hand-paper");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin users and roles sub menu.
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminUsersAndRoles(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "UsersandRoles"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_ProfileDefinitions ||
                this.PageContext.ForumPageType == ForumPages.Admin_AccessMasks ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditAccessMask ||
                this.PageContext.ForumPageType == ForumPages.Admin_Groups ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditGroup ||
                this.PageContext.ForumPageType == ForumPages.Admin_Ranks ||
                this.PageContext.ForumPageType == ForumPages.Admin_Users ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditUser ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditRank ||
                this.PageContext.ForumPageType == ForumPages.Admin_Medals ||
                this.PageContext.ForumPageType == ForumPages.Admin_EditMedal ||
                this.PageContext.ForumPageType == ForumPages.Admin_Mail ||
                this.PageContext.ForumPageType == ForumPages.Admin_Digest,
                true,
                "users");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin ProfileDefinitions
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ProfileDefinitions"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_ProfileDefinitions"),
                    BuildLink.GetLink(ForumPages.Admin_ProfileDefinitions),
                    this.PageContext.ForumPageType == ForumPages.Admin_ProfileDefinitions,
                    false,
                    "id-card");
            }

            // Admin AccessMasks
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_AccessMasks"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_AccessMasks"),
                    BuildLink.GetLink(ForumPages.Admin_AccessMasks),
                    this.PageContext.ForumPageType == ForumPages.Admin_AccessMasks ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditAccessMask,
                    false,
                    "universal-access");
            }

            // Admin Groups
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Groups"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Groups"),
                    BuildLink.GetLink(ForumPages.Admin_Groups),
                    this.PageContext.ForumPageType == ForumPages.Admin_Groups ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditGroup,
                    false,
                    "users");
            }

            // Admin Users
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Users"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Users"),
                    BuildLink.GetLink(ForumPages.Admin_Users),
                    this.PageContext.ForumPageType == ForumPages.Admin_EditUser ||
                    this.PageContext.ForumPageType == ForumPages.Admin_Users,
                    false,
                    "users");
            }

            // Admin Ranks
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Ranks"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Ranks"),
                    BuildLink.GetLink(ForumPages.Admin_Ranks),
                    this.PageContext.ForumPageType == ForumPages.Admin_Ranks ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditRank,
                    false,
                    "graduation-cap");
            }

            // Admin Medals
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Medals"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Medals"),
                    BuildLink.GetLink(ForumPages.Admin_Medals),
                    this.PageContext.ForumPageType == ForumPages.Admin_Medals ||
                    this.PageContext.ForumPageType == ForumPages.Admin_EditMedal,
                    false,
                    "medal");
            }

            // Admin Mail
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Mail"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Mail"),
                    BuildLink.GetLink(ForumPages.Admin_Mail),
                    this.PageContext.ForumPageType == ForumPages.Admin_Mail,
                    false,
                    "at");
            }

            // Admin Digest
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Digest"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Digest"),
                    BuildLink.GetLink(ForumPages.Admin_Digest),
                    this.PageContext.ForumPageType == ForumPages.Admin_Digest,
                    false,
                    "envelope");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin Maintenance sub menu
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminMaintenance(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "Maintenance"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_Prune ||
                this.PageContext.ForumPageType == ForumPages.Admin_Restore ||
                this.PageContext.ForumPageType == ForumPages.Admin_TaskManager ||
                this.PageContext.ForumPageType == ForumPages.Admin_EventLog ||
                this.PageContext.ForumPageType == ForumPages.Admin_RestartApp,
                true,
                "toolbox");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin Prune
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Prune"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Prune"),
                    BuildLink.GetLink(ForumPages.Admin_Prune),
                    this.PageContext.ForumPageType == ForumPages.Admin_Prune,
                    false,
                    "trash");
            }

            // Admin Restore
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Restore"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Restore"),
                    BuildLink.GetLink(ForumPages.Admin_Restore),
                    this.PageContext.ForumPageType == ForumPages.Admin_Restore,
                    false,
                    "trash-restore");
            }

            // Admin Pm
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Pm"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Pm"),
                    BuildLink.GetLink(ForumPages.Admin_Pm),
                    this.PageContext.ForumPageType == ForumPages.Admin_Pm,
                    false,
                    "envelope-square");
            }

            // Admin TaskManager
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_TaskManager"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_TaskManager"),
                    BuildLink.GetLink(ForumPages.Admin_TaskManager),
                    this.PageContext.ForumPageType == ForumPages.Admin_TaskManager,
                    false,
                    "tasks");
            }

            // Admin EventLog
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_EventLog"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_EventLog"),
                    BuildLink.GetLink(ForumPages.Admin_EventLog),
                    this.PageContext.ForumPageType == ForumPages.Admin_EventLog,
                    false,
                    "book");
            }

            // Admin RestartApp
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_RestartApp"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_RestartApp"),
                    BuildLink.GetLink(ForumPages.Admin_RestartApp),
                    this.PageContext.ForumPageType == ForumPages.Admin_RestartApp,
                    false,
                    "sync");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin database sub menu
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminDatabase(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "Database"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_ReIndex ||
                this.PageContext.ForumPageType == ForumPages.Admin_RunSql,
                true,
                "database");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin ReIndex
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_ReIndex"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_ReIndex"),
                    BuildLink.GetLink(ForumPages.Admin_ReIndex),
                    this.PageContext.ForumPageType == ForumPages.Admin_ReIndex,
                    false,
                    "database");
            }

            // Admin RunSql
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_RunSql"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_RunSql"),
                    BuildLink.GetLink(ForumPages.Admin_RunSql),
                    this.PageContext.ForumPageType == ForumPages.Admin_RunSql,
                    false,
                    "database");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin NNTP sub menu
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminNntp(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "NNTP"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_NntpRetrieve ||
                this.PageContext.ForumPageType == ForumPages.Admin_NntpForums ||
                this.PageContext.ForumPageType == ForumPages.Admin_NntpServers,
                true,
                "newspaper");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin NntpServers
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_NntpServers"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_NntpServers"),
                    BuildLink.GetLink(ForumPages.Admin_NntpServers),
                    this.PageContext.ForumPageType == ForumPages.Admin_NntpServers,
                    false,
                    "newspaper");
            }

            // Admin NntpForums
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_NntpForums"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_NntpForums"),
                    BuildLink.GetLink(ForumPages.Admin_NntpForums),
                    this.PageContext.ForumPageType == ForumPages.Admin_NntpForums,
                    false,
                    "newspaper");
            }

            // Admin NntpRetrieve
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_NntpRetrieve"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_NntpRetrieve"),
                    BuildLink.GetLink(ForumPages.Admin_NntpRetrieve),
                    this.PageContext.ForumPageType == ForumPages.Admin_NntpRetrieve,
                    false,
                    "newspaper");
            }

            this.MenuHolder.Controls.Add(list);
        }

        /// <summary>
        /// Render Admin Upgrade sub menu
        /// </summary>
        /// <param name="pagesAccess">
        /// The pages access.
        /// </param>
        private void RenderAdminUpgrade(IReadOnlyCollection<AdminPageUserAccess> pagesAccess)
        {
            RenderMenuItem(
                this.MenuHolder,
                "dropdown-item dropdown-toggle subdropdown-toggle",
                this.GetText("ADMINMENU", "Upgrade"),
                "#",
                this.PageContext.ForumPageType == ForumPages.Admin_Version,
                true,
                "download");

            var list = new HtmlGenericControl("ul");

            list.Attributes.Add("class", "dropdown-menu dropdown-submenu");

            // Admin Version
            if (this.PageContext.IsHostAdmin || pagesAccess.Any(
                x => x.PageName == "Admin_Version"))
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "admin_Version"),
                    BuildLink.GetLink(ForumPages.Admin_Version),
                    this.PageContext.ForumPageType == ForumPages.Admin_Version,
                    false,
                    "info");
            }

            // Admin Version
            if (this.PageContext.IsHostAdmin)
            {
                RenderMenuItem(
                    list,
                    "dropdown-item dropdown",
                    this.GetText("ADMINMENU", "Upgrade"),
                    this.ResolveUrl("~/install/default.aspx"),
                    false,
                    false,
                    "download");
            }

            this.MenuHolder.Controls.Add(list);
        }

        #endregion
    }
}