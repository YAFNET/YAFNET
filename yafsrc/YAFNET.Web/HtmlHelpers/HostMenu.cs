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
/// The host menu html helper.
/// </summary>
public static class HostMenuHtmlHelper
{
    /// <summary>
    /// The Host menu.
    /// </summary>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    public static IHtmlContent HostMenu(this IHtmlHelper _)
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
        // Host - Settings Menu
        if (context.PageUser.UserFlags.IsHostAdmin)
        {
            content.AppendHtml(RenderSettingsMenu(context));
        }

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

        iconTag.AddCssClass($"fas fa-{iconName} me-1");

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
    /// Render Host Settings Sub Menu
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The <see cref="IHtmlContent"/>.
    /// </returns>
    private static TagBuilder RenderSettingsMenu(
        BoardContext context)
    {
        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("dropdown dropend");

        var currentPageName = Enum.GetName(context.CurrentForumPage.PageName);

        var isActive = currentPageName != null && currentPageName.StartsWith("Host_");

        const string cssClassItem = "dropdown-item";
        const string page = "ADMIN_HOSTSETTINGS";

		listItem.InnerHtml.AppendHtml(
            RenderMenuItem(
                "dropdown-item dropdown-toggle subdropdown-toggle",
                context.Get<ILocalization>().GetText("ADMINMENU", page),
                "#",
                isActive,
                true,
                "cogs"));

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("dropdown-menu dropdown-submenu");


		// Host - Server Info
		list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HEADER_SERVER_INFO"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "ServerInfo" }),
                context.CurrentForumPage.PageName == ForumPages.Host_ServerInfo,
                false,
                "server"));

        // Host - Setup
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HEADER_SETUP"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Setup" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Setup,
                false,
                "gears"));

        // Host - Features
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_FEATURES"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Features" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Features,
                false,
                "wand-sparkles"));

        // Host - Display
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_DISPLAY"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Display" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Display,
                false,
                "display"));

        // Host - Adverts
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_ADVERTS"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Adverts" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Adverts,
                false,
                "rectangle-ad"));

        // Host - Permissions
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_PERMISSION"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Permission" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Permission,
                false,
                "user-lock"));

        // Host - Avatars
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_AVATARS"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Avatars" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Avatars,
                false,
                "user-tie"));

        // Host - Cache
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_CACHE"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Cache" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Cache,
                false,
                "memory"));

        // Host - Search
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_SEARCH"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Search" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Search,
                false,
                "magnifying-glass"));

        // Host - Logs
        list.InnerHtml.AppendHtml(
            RenderMenuItem(
                cssClassItem,
                context.Get<ILocalization>().GetText(page, "HOST_LOG"),
                context.Get<ILinkBuilder>().GetLink(ForumPages.Admin_HostSettings, new { tab = "Log" }),
                context.CurrentForumPage.PageName == ForumPages.Host_Log,
                false,
                "book"));

        listItem.InnerHtml.AppendHtml(list);

        return listItem;
    }
}