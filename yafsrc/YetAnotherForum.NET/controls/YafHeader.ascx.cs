/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The yaf header.
    /// </summary>
    public partial class YafHeader : BaseUserControl
    {
        #region Methods

        /// <summary>
        /// The get return url.
        /// </summary>
        /// <returns>
        /// The url.
        /// </returns>
        protected string GetReturnUrl()
        {
            string returnUrl = string.Empty;

            if (this.PageContext.ForumPageType != ForumPages.login)
            {
                returnUrl = HttpContext.Current.Server.UrlEncode(General.GetSafeRawUrl());
            }
            else
            {
                // see if there is already one since we are on the login page
                if (HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl").IsSet())
                {
                    returnUrl =
                      HttpContext.Current.Server.UrlEncode(
                        General.GetSafeRawUrl(HttpContext.Current.Request.QueryString.GetFirstOrDefault("ReturnUrl")));
                }
            }

            return returnUrl;
        }

        /// <summary>
        /// Do Logout Dialog
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void LogOutClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            var notification = (DialogBox)this.PageContext.CurrentForumPage.Notification;

            notification.Show(
              this.GetText("TOOLBAR", "LOGOUT_QUESTION"),
              "Logout?",
              DialogBox.DialogIcon.Question,
              new DialogBox.DialogButton
                {
                    Text = "Yes",
                    CssClass = "StandardButton",
                    ForumPageLink = new DialogBox.ForumLink { ForumPage = ForumPages.logout }
                },
              new DialogBox.DialogButton { Text = "No", CssClass = "StandardButton" });
        }

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
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.RenderGuestControls();

            this.RenderUserContainer();

            this.RenderMainHeaderMenu();

            this.RenderAdminModMenu();
        }
        
        /// <summary>
        /// Do Quick Search
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void QuickSearchClick([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (string.IsNullOrEmpty(this.searchInput.Text))
            {
                return;
            }

            YafBuildLink.Redirect(ForumPages.search, "search={0}", this.searchInput.Text);
        }

        /// <summary>
        /// Render Li and a Item
        /// </summary>
        /// <param name="holder">
        /// The holder.
        /// </param>
        /// <param name="liCssClass">
        /// The li Css Class.
        /// </param>
        /// <param name="linkCssClass">
        /// The link Css Class.
        /// </param>
        /// <param name="linkText">
        /// The link text.
        /// </param>
        /// <param name="linkUrl">
        /// The link url.
        /// </param>
        /// <param name="noFollow">
        /// Add rel="nofollow" to the link
        /// </param>
        /// <param name="showUnread">
        /// The show unread.
        /// </param>
        /// <param name="unreadText">
        /// The unread text.
        /// </param>
        private static void RenderMenuItem(
            Control holder, string liCssClass, string linkCssClass, string linkText, string linkUrl, bool noFollow, bool showUnread, string unreadText)
        {
            var liElement = new HtmlGenericControl("li");

            if (!string.IsNullOrEmpty(liCssClass))
            {
                liElement.Attributes.Add("class", liCssClass);
            }

            var link = new HyperLink
            {
                Target = "_top",
                ToolTip = linkText,
                NavigateUrl = linkUrl,
                Text = linkText
            };

            if (noFollow)
            {
                link.Attributes.Add("rel", "nofollow");
            }

            if (!string.IsNullOrEmpty(linkCssClass))
            {
                link.CssClass = linkCssClass;
            }

            liElement.Controls.Add(link);

            if (showUnread)
            {
                var unreadLabel = new HtmlGenericControl("span");

                unreadLabel.Attributes.Add("class", "unread");

                unreadLabel.InnerText = unreadText;

                liElement.Controls.Add(unreadLabel);
            }

            holder.Controls.Add(liElement);
        }

        /// <summary>
        ///  Render the Quick Search
        /// </summary>
        private void RenderQuickSearch()
        {
            if ((!this.PageContext.BoardSettings.ShowQuickSearch ||
                 !this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions)) &&
                (!this.PageContext.BoardSettings.ShowQuickSearch ||
                 !this.Get<IPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions)))
            {
                return;
            }

            this.quickSearch.Visible = true;

            var searchIcon = this.PageContext.Get<ITheme>().GetItem("ICONS", "SEARCH");

            if (!string.IsNullOrEmpty(searchIcon))
            {
                this.doQuickSearch.Text = @"<img alt=""{1}"" title=""{1}"" src=""{0}"" /> {1}".FormatWith(
                    searchIcon, this.GetText("SEARCH", "BTNSEARCH"));
            }
            else
            {
                this.doQuickSearch.Text = this.GetText("SEARCH", "BTNSEARCH");
            }

            this.searchInput.Attributes["onkeydown"] =
                "if(event.which || event.keyCode){{if ((event.which == 13) || (event.keyCode == 13)) {{document.getElementById('{0}').click();return false;}}}} else {{return true}}; "
                    .FormatWith(this.doQuickSearch.ClientID);
            this.searchInput.Attributes["onfocus"] =
                "if (this.value == '{0}') {{this.value = '';}}".FormatWith(
                    this.GetText("TOOLBAR", "SEARCHKEYWORD"));
            this.searchInput.Attributes["onblur"] =
                "if (this.value == '') {{this.value = '{0}';}}".FormatWith(
                    this.GetText("TOOLBAR", "SEARCHKEYWORD"));

            this.searchInput.Text = this.GetText("TOOLBAR", "SEARCHKEYWORD");
        }

        /// <summary>
        /// Render the Admin/Moderater Menu Links
        /// </summary>
        private void RenderAdminModMenu()
        {
            // Admin
            if (this.PageContext.IsAdmin)
            {
                this.AdminModHolder.Visible = true;

                // Admin
                RenderMenuItem(
                    menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "ADMIN"),
                    YafBuildLink.GetLink(ForumPages.admin_admin),
                    false,
                    false,
                    null);
            }

            // Host
            if (this.PageContext.IsHostAdmin)
            {
                this.AdminModHolder.Visible = true;

                // Admin
                RenderMenuItem(
                    menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "HOST"),
                    YafBuildLink.GetLink(ForumPages.admin_hostsettings),
                    false,
                    false,
                    null);
            }

            // Moderate
            if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
            {
                this.AdminModHolder.Visible = true;

                // Admin
                RenderMenuItem(
                    menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "MODERATE"),
                    YafBuildLink.GetLink(ForumPages.moderate_index),
                    false,
                    false,
                    null);
            }
        }

        /// <summary>
        /// Render the Main Header Menu Links
        /// </summary>
        private void RenderMainHeaderMenu()
        {
            // Forum
            RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("DEFAULT", "FORUM"),
                    YafBuildLink.GetLink(ForumPages.forum),
                    false,
                    false,
                    null);

            // Active Topics
            if (this.PageContext.IsGuest)
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                     this.GetText("TOOLBAR", "ACTIVETOPICS"),
                    YafBuildLink.GetLink(ForumPages.mytopics),
                    false,
                    false,
                    null);
            }

            // Search
            if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || this.Get<IPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions))
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "SEARCH"),
                    YafBuildLink.GetLink(ForumPages.search),
                    false,
                    false,
                    null);
            }

            // Members
            if (this.Get<IPermissions>().Check(this.PageContext.BoardSettings.MembersListViewPermissions))
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "MEMBERS"),
                    YafBuildLink.GetLink(ForumPages.members),
                    false, 
                    false,
                    null);
            }

            // Team
            if (this.PageContext.BoardSettings.ShowTeam)
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "TEAM"),
                    YafBuildLink.GetLink(ForumPages.team),
                    false, 
                    false,
                    null);
            }

            // Help
            if (this.PageContext.BoardSettings.ShowHelp)
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "HELP"),
                    YafBuildLink.GetLink(ForumPages.help_index),
                    false, 
                    false,
                    null);
            }

            // Login
            if (this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                if (this.PageContext.BoardSettings.UseLoginBox && !(YafContext.Current.Get<IYafSession>().UseMobileTheme ?? false))
                {
                    RenderMenuItem(
                        menuListItems,
                        "menuAccount",
                        "LoginLink",
                        this.GetText("TOOLBAR", "LOGIN"),
                        "{0}#".FormatWith(YafBuildLink.GetLink(this.PageContext.ForumPageType)),
                        true,
                        false,
                        null);
                }
                else
                {
                    string returnUrl = this.GetReturnUrl().IsSet()
                                             ? "ReturnUrl={0}".FormatWith(
                                               this.PageContext.BoardSettings.UseSSLToLogIn
                                                 ? this.GetReturnUrl().Replace("http:", "https:")
                                                 : this.GetReturnUrl())
                                             : string.Empty;

                    RenderMenuItem(
                        menuListItems,
                        "menuAccount",
                        null,
                        this.GetText("TOOLBAR", "LOGIN"),
                        YafBuildLink.GetLink(ForumPages.login, returnUrl),
                        true,
                        false,
                        null);
                }
            }

            // Register
            if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.DisableRegistrations && !Config.IsAnyPortal)
            {
                RenderMenuItem(
                    menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "REGISTER"),
                    this.PageContext.BoardSettings.ShowRulesForRegistration
                        ? YafBuildLink.GetLink(ForumPages.rules)
                        : (!this.PageContext.BoardSettings.UseSSLToRegister
                               ? YafBuildLink.GetLink(ForumPages.register)
                               : YafBuildLink.GetLink(ForumPages.register).Replace("http:", "https:")),
                    true,
                    false,
                    null);
            }
        }

        /// <summary>
        /// Render The User related Links
        /// </summary>
        private void RenderUserContainer()
        {
            if (this.PageContext.IsGuest)
            {
                return;
            }

            this.UserContainer.Visible = true;

            // My Profile
            this.MyProfile.ToolTip = this.GetText("TOOLBAR", "MYPROFILE");
            this.MyProfile.NavigateUrl = YafBuildLink.GetLink(ForumPages.cp_profile);
            this.MyProfile.Text = this.GetText("TOOLBAR", "MYPROFILE");

            // My Inbox
            if (this.PageContext.BoardSettings.AllowPrivateMessages)
            {
                RenderMenuItem(
                    MyInboxItem,
                    "menuMy",
                    null,
                    this.GetText("TOOLBAR", "INBOX"),
                    YafBuildLink.GetLink(ForumPages.cp_pm),
                    false,
                    this.PageContext.PendingBuddies > 0,
                    this.GetText("TOOLBAR", "NEWPM").FormatWith(this.PageContext.UnreadPrivate));
            }

            // My Buddies
            if (this.PageContext.BoardSettings.EnableBuddyList && this.PageContext.UserHasBuddies)
            {
                RenderMenuItem(
                    MyBuddiesItem,
                    "menuMy",
                    null,
                    this.GetText("TOOLBAR", "BUDDIES"),
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    false,
                    this.PageContext.PendingBuddies > 0,
                    this.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(this.PageContext.PendingBuddies));
            }

            // My Albums
            if (this.PageContext.BoardSettings.EnableAlbum && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
            {
                RenderMenuItem(
                    MyAlbumsItem,
                    "menuMy",
                    null,
                    this.GetText("TOOLBAR", "MYALBUMS"),
                    YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                    false,
                    false,
                    null);
            }

            // My Topics
            this.MyTopics.ToolTip = this.GetText("TOOLBAR", "MYTOPICS");
            this.MyTopics.NavigateUrl = YafBuildLink.GetLink(ForumPages.mytopics);
            this.MyTopics.Text = this.GetText("TOOLBAR", "MYTOPICS");

            // Logout
            if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                LogutItem.Visible = true;
                LogOutButton.Text = this.GetText("TOOLBAR", "LOGOUT");
                LogOutButton.ToolTip = this.GetText("TOOLBAR", "LOGOUT");
            }

            // Logged in as : username
            LoggedInUserPanel.Visible = true;

            LoggedInUserPanel.Controls.Add(
                new Label { Text = this.GetText("TOOLBAR", "LOGGED_IN_AS").FormatWith("&nbsp;") });

            var userLink = new UserLink
                {
                    ID = "UserLoggedIn", 
                    UserID = this.PageContext.PageUserID,
                    CssClass = "currentUser"
                };

            LoggedInUserPanel.Controls.Add(userLink);
        }

        /// <summary>
        /// Render The GuestBar
        /// </summary>
        private void RenderGuestControls()
        {
            if (!this.PageContext.IsGuest)
            {
                return;
            }

            this.GuestUserMessage.Visible = true;
        }

        #endregion
    }
}