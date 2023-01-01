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
 * to you under the Apache License, Version 2.0 (thea
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

/// <summary>
/// The Header.
/// </summary>
public partial class Header : BaseUserControl
{
    /// <summary>
    /// The On PreRender event.
    /// </summary>
    /// <param name="e">
    /// the Event Arguments
    /// </param>
    protected override void OnPreRender([NotNull] EventArgs e)
    {
        this.RenderQuickSearch();

        base.OnPreRender(e);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        this.RenderGuestControls();

        this.RenderMainHeaderMenu();

        this.RenderAdminModMenu();
    }

    /// <summary>
    /// Do Quick Search
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void QuickSearchClick([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.searchInput.Text.IsNotSet())
        {
            return;
        }

        this.Get<LinkBuilder>().Redirect(ForumPages.Search, new { search = this.Server.UrlEncode(this.searchInput.Text) });
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
    /// <param name="linkToolTip">
    /// The link tool tip.
    /// </param>
    /// <param name="linkUrl">
    /// The link URL.
    /// </param>
    /// <param name="noFollow">
    /// Add no follow to the link
    /// </param>
    /// <param name="showUnread">
    /// The show unread.
    /// </param>
    /// <param name="unread">
    /// The unread.
    /// </param>
    /// <param name="unreadText">
    /// The unread text.
    /// </param>
    /// <param name="isActive">
    /// The is Active.
    /// </param>
    /// <param name="icon">
    /// The icon.
    /// </param>
    private static void RenderMenuItem(
        Control holder,
        string cssClass,
        string linkText,
        string linkToolTip,
        string linkUrl,
        bool noFollow,
        bool showUnread,
        string unread,
        string unreadText,
        bool isActive,
        string icon = "")
    {
        var element = new HtmlGenericControl("li");

        if (cssClass.IsSet())
        {
            element.Attributes.Add("class", "nav-item");
        }

        if (isActive)
        {
            cssClass = $"{cssClass} active";
        }

        if (linkToolTip.IsNotSet())
        {
            linkToolTip = linkText;
        }

        var link = new ThemeButton
                       {
                           TitleLocalizedTag = linkToolTip,
                           Type = ButtonStyle.None,
                           TitleLocalizedPage = "TOOLBAR",
                           NavigateUrl = linkUrl,
                           Text = icon.IsSet()
                                      ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}"
                                      : linkText,
                           CssClass = cssClass
                       };

        if (noFollow)
        {
            link.Attributes.Add("rel", "nofollow");
        }

        link.DataToggle = "tooltip";

        if (showUnread)
        {
            /*link.Controls.Add(new LiteralControl(icon.IsSet()
                                                     ? $"<i class=\"fa fa-{icon} fa-fw\"></i>&nbsp;{linkText}&nbsp;"
                                                     : $"{linkText}&nbsp;"));*/

            var unreadLabel = new Label { CssClass = "badge bg-danger ms-1", ToolTip = unreadText, Text = unread };

            unreadLabel.Attributes.Add("data-bs-toggle", "tooltip");

            var unreadLabelText = new Label { CssClass = "visually-hidden", Text = unreadText };

            link.Controls.Add(unreadLabel);

            link.Controls.Add(unreadLabelText);
        }

        if (cssClass.StartsWith("nav-link"))
        {
            element.Controls.Add(link);
            holder.Controls.Add(element);
        }
        else
        {
            holder.Controls.Add(link);
        }
    }

    /// <summary>
    ///  Render the Quick Search
    /// </summary>
    private void RenderQuickSearch()
    {
        if (!this.PageBoardContext.BoardSettings.ShowQuickSearch
            || !this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.SearchPermissions))
        {
            return;
        }

        this.quickSearch.Visible = true;

        this.searchInput.Attributes.Add(
            "onkeydown",
            JavaScriptBlocks.ClickOnEnterJs(this.doQuickSearch.ClientID));

        this.searchInput.Attributes["onfocus"] =
            $"if (this.value == '{this.GetText("TOOLBAR", "SEARCHKEYWORD")}') {{this.value = '';}}";
        this.searchInput.Attributes["onblur"] =
            $"if (this.value == '') {{this.value = '{this.GetText("TOOLBAR", "SEARCHKEYWORD")}';}}";

        this.searchInput.Text = this.GetText("TOOLBAR", "SEARCHKEYWORD");
    }

    /// <summary>
    /// Render the Admin/Moderator Menu Links
    /// </summary>
    private void RenderAdminModMenu()
    {
        // Admin
        if (this.PageBoardContext.IsAdmin)
        {
            this.AdminModHolder.Visible = true;

            this.AdminAdminHolder.Visible = true;
        }

        // Host
        if (this.PageBoardContext.PageUser.UserFlags.IsHostAdmin)
        {
            this.AdminModHolder.Visible = true;
            this.HostMenuHolder.Visible = true;
        }

        // Moderate
        if (!this.PageBoardContext.IsModeratorInAnyForum)
        {
            return;
        }

        this.AdminModHolder.Visible = true;

        // Admin
        RenderMenuItem(
            this.menuModerateItems,
            "nav-link",
            this.GetText("TOOLBAR", "MODERATE"),
            "MODERATE_TITLE",
            this.Get<LinkBuilder>().GetLink(ForumPages.Moderate_Index),
            false,
            this.PageBoardContext.ModeratePosts > 0,
            this.PageBoardContext.ModeratePosts.ToString(),
            this.GetTextFormatted("MODERATE_NEW", this.PageBoardContext.ModeratePosts),
            this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Moderate_Index);

        this.hostDropdown.CssClass =
            this.PageBoardContext.CurrentForumPage.PageType is ForumPages.Admin_HostSettings or ForumPages.Admin_Boards or
                ForumPages.Admin_EditBoard or ForumPages.Admin_PageAccessEdit or ForumPages.Admin_PageAccessList
                ? "nav-link dropdown-toggle active"
                : "nav-link dropdown-toggle";
    }

    /// <summary>
    /// Render the Main Header Menu Links
    /// </summary>
    private void RenderMainHeaderMenu()
    {
        // Search
        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.SearchPermissions))
        {
            RenderMenuItem(
                this.menuListItems,
                "nav-link",
                this.GetText("TOOLBAR", "SEARCH"),
                "SEARCH_TITLE",
                this.Get<LinkBuilder>().GetLink(ForumPages.Search),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Search,
                string.Empty);
        }

        // Members
        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.MembersListViewPermissions))
        {
            RenderMenuItem(
                this.menuListItems,
                "nav-link",
                this.GetText("TOOLBAR", "MEMBERS"),
                "MEMBERS_TITLE",
                this.Get<LinkBuilder>().GetLink(ForumPages.Members),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Members,
                string.Empty);
        }

        // Team
        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowTeamTo))
        {
            RenderMenuItem(
                this.menuListItems,
                "nav-link",
                this.GetText("TOOLBAR", "TEAM"),
                "TEAM_TITLE",
                this.Get<LinkBuilder>().GetLink(ForumPages.Team),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Team,
                string.Empty);
        }

        // Help
        if (this.Get<IPermissions>().Check(this.PageBoardContext.BoardSettings.ShowHelpTo))
        {
            RenderMenuItem(
                this.menuListItems,
                "nav-link",
                this.GetText("TOOLBAR", "HELP"),
                "HELP_TITLE",
                this.Get<LinkBuilder>().GetLink(ForumPages.Help),
                false,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Help,
                string.Empty);
        }

        if (!this.PageBoardContext.IsGuest || Config.IsAnyPortal)
        {
            return;
        }

        // Login
        if (Config.AllowLoginAndLogoff)
        {
            var navigateUrl = "javascript:void(0);";

            if (this.PageBoardContext.CurrentForumPage.IsAccountPage)
            {
                navigateUrl = this.Get<LinkBuilder>().GetLink(ForumPages.Account_Login);
            }

            RenderMenuItem(
                this.menuListItems,
                "nav-link LoginLink",
                this.GetText("TOOLBAR", "LOGIN"),
                "LOGIN_TITLE",
                navigateUrl,
                true,
                false,
                null,
                null,
                false,
                string.Empty);
        }

        // Register
        if (!this.PageBoardContext.BoardSettings.DisableRegistrations)
        {
            RenderMenuItem(
                this.menuListItems,
                "nav-link",
                this.GetText("TOOLBAR", "REGISTER"),
                "REGISTER_TITLE",
                this.Get<LinkBuilder>().GetLink(
                    this.PageBoardContext.BoardSettings.ShowRulesForRegistration
                        ? ForumPages.RulesAndPrivacy
                        : ForumPages.Account_Register),
                true,
                false,
                null,
                null,
                this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Account_Register,
                string.Empty);
        }
    }

    /// <summary>
    /// Render The GuestBar
    /// </summary>
    private void RenderGuestControls()
    {
        if (!this.PageBoardContext.IsGuest)
        {
            // Logged in as : username
            this.LoggedInUserPanel.Visible = true;
        }
    }
}