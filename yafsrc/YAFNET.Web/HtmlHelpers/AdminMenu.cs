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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.HtmlHelpers;

/// <summary>
/// The admin menu html helper.
/// </summary>
public static class AdminMenuHtmlHelper
{
    /// <summary>
    /// The admin menu.
    /// </summary>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent AdminMenu(this IHtmlHelper _)
    {
        var content = new HtmlContentBuilder();
        var context = BoardContext.Current;

        return RenderMenuItems(content, context);
    }

    /// <summary>
    /// Renders the menu items.
    /// </summary>
    /// <param name="content">
    /// The content.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static HtmlContentBuilder RenderMenuItems(HtmlContentBuilder content, BoardContext context)
    {
        var pagesAccess = context.Get<IDataCache>().GetOrSet(
            string.Format(Constants.Cache.AdminPageAccess, context.PageUserID),
            () => context.GetRepository<AdminPageUserAccess>().List(context.PageUserID).ToList());

        // Admin Admin
        content.AppendHtml(
            RenderMenuItem(
                "dropdown-item",
                context.Get<ILocalization>().GetText("ADMINMENU", "ADMIN_ADMIN"),
                context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Admin),
                context.CurrentForumPage.PageName == ForumPages.Admin_Admin,
                false,
                "tachometer-alt"));

        // Admin - Settings Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(
                x => x.PageName is "Admin_BoardAnnouncement" or "Admin_Settings" or "Admin_Forums" or "Admin_ReplaceWords" or "Admin_BBCodes" or "Admin_Languages"))
        {
            content.AppendHtml(RenderAdminSettings(pagesAccess, context));
        }

        // Admin - Spam Protection Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(
                x => x.PageName is "Admin_SpamLog" or "Admin_SpamWords" or "Admin_BannedEmails" or "Admin_BannedIps" or "Admin_BannedNames" or "Admin_BannedUserAgents"))
        {
            content.AppendHtml(RenderAdminSpamProtection(pagesAccess, context));
        }

        // Admin - Users and Roles Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(
                x => x.PageName is "Admin_ProfileDefinitions" or "Admin_AccessMasks" or "Admin_Groups" or "Admin_Ranks" or "Admin_Users" or "Admin_Medals" or "Admin_Mail" or "Admin_Digest"))
        {
            content.AppendHtml(RenderAdminUsersAndRoles(pagesAccess, context));
        }

        // Admin - Maintenance Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(
                x => x.PageName is "Admin_Attachments" or "Admin_Tags" or "Admin_Prune" or "Admin_Restore" or "Admin_TaskManager" or "Admin_EventLog" or "Admin_RestartApp"))
        {
            content.AppendHtml(RenderAdminMaintenance(pagesAccess, context));
        }

        // Admin - Database Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(
                x => x.PageName is "Admin_ReIndex" or "Admin_RunSql"))
        {
            content.AppendHtml(RenderAdminDatabase(pagesAccess, context));
        }

        // Admin - Upgrade Menu
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Exists(x => x.PageName == "Admin_Version"))
        {
            content.AppendHtml(RenderAdminUpgrade(pagesAccess, context));
        }

#if DEBUG
        // Admin Admin
        content.AppendHtml(
            RenderMenuItem(
                "dropdown-item",
                "Test Generator",
                context.Get<LinkBuilder>().GetLink(ForumPages.Admin_TestData),
                context.CurrentForumPage.PageName == ForumPages.Admin_TestData,
                false,
                "vial-virus"));
#endif

        return content;
    }

    /// <summary>
    /// Render Li and a Item
    /// </summary>
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
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderMenuItem(
        string cssClass,
        string linkText,
        string linkUrl,
        bool isActive,
        bool isDropDownToggle,
        string iconName)
    {
        var link = new TagBuilder(HtmlTag.A);

        link.AddCssClass(isActive ? $"{cssClass} active" : cssClass);

        link.MergeAttribute(HtmlAttribute.Rel, "nofollow");
        link.MergeAttribute(HtmlAttribute.Role, HtmlTag.Button);
        link.MergeAttribute("data-bs-toggle", isDropDownToggle ? "dropdown" : "tooltip");
        link.MergeAttribute(HtmlAttribute.Href, linkUrl);
        link.MergeAttribute(HtmlAttribute.Title, linkText);

        // Icon
        var iconTag = new TagBuilder(HtmlTag.I);

        iconTag.AddCssClass($"fas fa-{iconName} fa-fw me-1");

        iconTag.TagRenderMode = TagRenderMode.Normal;

        link.InnerHtml.AppendHtml(iconTag);

        // Text
        link.InnerHtml.Append(linkText);

        if (isDropDownToggle)
        {
            return link;
        }

        var listItem = new TagBuilder(HtmlTag.Li);
        listItem.InnerHtml.AppendHtml(link);

        return listItem;
    }

    /// <summary>
    /// Render Admin Settings Sub Menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminSettings(
        IReadOnlyCollection<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "SETTINGS"),
                "#",
                context.CurrentForumPage.PageName is ForumPages.Admin_BoardAnnouncement or ForumPages.Admin_Settings
                    or ForumPages.Admin_Forums or ForumPages.Admin_EditForum or ForumPages.Admin_EditCategory or
                    ForumPages.Admin_ReplaceWords or ForumPages.Admin_BBCodes or ForumPages.Admin_EditBBCode or
                    ForumPages.Admin_Languages or ForumPages.Admin_EditLanguage,
                true,
                "cogs"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin Board Announcement
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BoardAnnouncement"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_BoardAnnouncement"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BoardAnnouncement),
                    context.CurrentForumPage.PageName == ForumPages.Admin_BoardAnnouncement,
                    false,
                    "bullhorn"));
        }

        // Admin Settings
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Settings"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item dropdown",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_boardsettings"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Settings),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Settings,
                    false,
                    "cogs"));
        }

        // Admin Forums
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Forums"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_forums"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Forums),
                    context.CurrentForumPage.PageName is ForumPages.Admin_Forums or ForumPages.Admin_EditForum or ForumPages.Admin_EditCategory,
                    false,
                    "comments"));
        }

        // Admin ReplaceWords
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_ReplaceWords"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_replacewords"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_ReplaceWords),
                    context.CurrentForumPage.PageName == ForumPages.Admin_ReplaceWords,
                    false,
                    "sticky-note"));
        }

        // Admin BBCodes
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BBCodes"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_bbcode"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BBCodes),
                    context.CurrentForumPage.PageName is ForumPages.Admin_BBCodes or ForumPages.Admin_EditBBCode,
                    false,
                    "plug"));
        }

        // Admin Languages
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Languages"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Languages"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Languages),
                    context.CurrentForumPage.PageName is ForumPages.Admin_Languages or ForumPages.Admin_EditLanguage,
                    false,
                    "language"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }

    /// <summary>
    /// Render Admin spam protection sub menu.
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminSpamProtection(
        IReadOnlyCollection<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "Spam_Protection"),
                "#",
                context.CurrentForumPage.PageName is ForumPages.Admin_SpamLog or ForumPages.Admin_SpamWords
                    or ForumPages.Admin_BannedEmails or ForumPages.Admin_BannedIps or ForumPages.Admin_BannedNames
                    or ForumPages.Admin_BannedUserAgents,
                true,
                "shield-alt"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin SpamLog
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_SpamLog"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_spamlog"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_SpamLog),
                    context.CurrentForumPage.PageName == ForumPages.Admin_SpamLog,
                    false,
                    "book"));
        }

        // Admin SpamWords
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_SpamWords"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_SpamWords"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_SpamWords),
                    context.CurrentForumPage.PageName == ForumPages.Admin_SpamWords,
                    false,
                    "hand-paper"));
        }

        // Admin BannedEmails
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedEmails"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_BannedEmail"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedEmails),
                    context.CurrentForumPage.PageName == ForumPages.Admin_BannedEmails,
                    false,
                    "hand-paper"));
        }

        // Admin BannedIps
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedIps"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_BannedIp"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedIps),
                    context.CurrentForumPage.PageName == ForumPages.Admin_BannedIps,
                    false,
                    "hand-paper"));
        }

        // Admin BannedNames
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedNames"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_BannedName"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedNames),
                    context.CurrentForumPage.PageName == ForumPages.Admin_BannedNames,
                    false,
                    "hand-paper"));
        }

        // Admin BannedUserAgents
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_BannedUserAgents"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "ADMIN_BANNED_USERAGENTS"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_BannedUserAgents),
                    context.CurrentForumPage.PageName == ForumPages.Admin_BannedUserAgents,
                    false,
                    "user-secret"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }

    /// <summary>
    /// Render Admin users and roles sub menu.
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminUsersAndRoles(
        IReadOnlyCollection<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "UsersandRoles"),
                "#",
                context.CurrentForumPage.PageName is ForumPages.Admin_ProfileDefinitions or ForumPages.Admin_AccessMasks or ForumPages.Admin_EditAccessMask or ForumPages.Admin_Groups or ForumPages.Admin_EditGroup or ForumPages.Admin_Ranks or ForumPages.Admin_Users or ForumPages.Admin_EditUser or ForumPages.Admin_EditRank or ForumPages.Admin_Medals or ForumPages.Admin_EditMedal or ForumPages.Admin_Mail or ForumPages.Admin_Digest,
                true,
                "users"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin ProfileDefinitions
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_ProfileDefinitions"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_ProfileDefinitions"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_ProfileDefinitions),
                    context.CurrentForumPage.PageName == ForumPages.Admin_ProfileDefinitions,
                    false,
                    "id-card"));
        }

        // Admin AccessMasks
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_AccessMasks"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_AccessMasks"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_AccessMasks),
                    context.CurrentForumPage.PageName is ForumPages.Admin_AccessMasks or ForumPages.Admin_EditAccessMask,
                    false,
                    "universal-access"));
        }

        // Admin Groups
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Groups"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Groups"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Groups),
                    context.CurrentForumPage.PageName is ForumPages.Admin_Groups or ForumPages.Admin_EditGroup,
                    false,
                    "users"));
        }

        // Admin Users
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Users"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Users"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users),
                    context.CurrentForumPage.PageName is ForumPages.Admin_EditUser or ForumPages.Admin_Users,
                    false,
                    "users"));
        }

        // Admin Ranks
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Ranks"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Ranks"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Ranks),
                    context.CurrentForumPage.PageName is ForumPages.Admin_Ranks or ForumPages.Admin_EditRank,
                    false,
                    "graduation-cap"));
        }

        // Admin Medals
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Medals"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Medals"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Medals),
                    context.CurrentForumPage.PageName is ForumPages.Admin_Medals or ForumPages.Admin_EditMedal,
                    false,
                    "medal"));
        }

        // Admin Mail
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Mail"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Mail"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Mail),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Mail,
                    false,
                    "at"));
        }

        // Admin Digest
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Digest"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Digest"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Digest),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Digest,
                    false,
                    "envelope"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }

    /// <summary>
    /// Render Admin Maintenance sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminMaintenance(
        IReadOnlyCollection<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "Maintenance"),
                "#",
                context.CurrentForumPage.PageName is ForumPages.Admin_Attachments or ForumPages.Admin_Tags or
                    ForumPages.Admin_Prune or ForumPages.Admin_Restore or ForumPages.Admin_TaskManager or
                    ForumPages.Admin_EventLog or ForumPages.Admin_RestartApp,
                true,
                "toolbox"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin Attachments
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Attachments"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Attachments"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Attachments),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Attachments,
                    false,
                    "paperclip"));
        }

        // Admin Tags
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Tags"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Tags"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Tags),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Tags,
                    false,
                    "tags"));
        }

        // Admin Prune
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Prune"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Prune"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Prune),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Prune,
                    false,
                    "trash"));
        }

        // Admin Restore
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Restore"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Restore"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Restore),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Restore,
                    false,
                    "trash-restore"));
        }

        // Admin Pm
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Pm"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Pm"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Pm),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Pm,
                    false,
                    "envelope-square"));
        }

        // Admin TaskManager
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_TaskManager"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_TaskManager"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_TaskManager),
                    context.CurrentForumPage.PageName == ForumPages.Admin_TaskManager,
                    false,
                    "tasks"));
        }

        // Admin EventLog
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_EventLog"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_EventLog"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_EventLog),
                    context.CurrentForumPage.PageName == ForumPages.Admin_EventLog,
                    false,
                    "book"));
        }

        // Admin RestartApp
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_RestartApp"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_RestartApp"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_RestartApp),
                    context.CurrentForumPage.PageName == ForumPages.Admin_RestartApp,
                    false,
                    "sync"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }

    /// <summary>
    /// Render Admin database sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminDatabase(
        IReadOnlyCollection<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "Database"),
                "#",
                context.CurrentForumPage.PageName is ForumPages.Admin_ReIndex or ForumPages.Admin_RunSql,
                true,
                "database"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin ReIndex
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_ReIndex"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_ReIndex"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_ReIndex),
                    context.CurrentForumPage.PageName == ForumPages.Admin_ReIndex,
                    false,
                    "database"));
        }

        // Admin RunSql
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_RunSql"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_RunSql"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_RunSql),
                    context.CurrentForumPage.PageName == ForumPages.Admin_RunSql,
                    false,
                    "database"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }

    /// <summary>
    /// Render Admin Upgrade sub menu
    /// </summary>
    /// <param name="pagesAccess">
    /// The pages access.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderAdminUpgrade(
        IEnumerable<AdminPageUserAccess> pagesAccess,
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", "Upgrade"),
                "#",
                context.CurrentForumPage.PageName == ForumPages.Admin_Version,
                true,
                "download"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");

        // Admin Version
        if (context.PageUser.UserFlags.IsHostAdmin || pagesAccess.Any(x => x.PageName == "Admin_Version"))
        {
            list.InnerHtml.AppendHtml(
                RenderMenuItem(
                    "dropdown-item dropdown",
                    context.Get<ILocalization>().GetText("ADMINMENU", "admin_Version"),
                    context.Get<LinkBuilder>().GetLink(ForumPages.Admin_Version),
                    context.CurrentForumPage.PageName == ForumPages.Admin_Version,
                    false,
                    "info"));
        }

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }
}