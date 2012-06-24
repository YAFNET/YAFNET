/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
                    Text = this.GetText("COMMON", "YES"),
                    CssClass = "StandardButton OkButton",
                    ForumPageLink = new DialogBox.ForumLink { ForumPage = ForumPages.logout }
                },
              new DialogBox.DialogButton { Text = this.GetText("COMMON", "NO"), CssClass = "StandardButton CancelButton" });
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

            YafBuildLink.Redirect(ForumPages.search, "search={0}", this.searchInput.Text.TrimWordsOverMaxLengthWordsPreserved(this.Get<YafBoardSettings>().SearchStringMaxLength));
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
        /// <param name="linkToolTip">
        /// The link tool tip.
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
        /// <param name="unread">
        /// The unread.
        /// </param>
        /// <param name="unreadText">
        /// The unread text.
        /// </param>
        private static void RenderMenuItem(
            Control holder, string liCssClass, string linkCssClass, string linkText, string linkToolTip, string linkUrl, bool noFollow, bool showUnread, string unread, string unreadText)
        {
            var liElement = new HtmlGenericControl("li");

            if (!string.IsNullOrEmpty(liCssClass))
            {
                liElement.Attributes.Add("class", liCssClass);
            }

            if (string.IsNullOrEmpty(linkToolTip))
            {
                linkToolTip = linkText;
            }

            var link = new HyperLink
            {
                Target = "_top",
                ToolTip = linkToolTip,
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

            var unreadDiv = new HtmlGenericControl("div");

            if (showUnread)
            {
                unreadDiv.Attributes.Add("class", "UnreadBox");

                liElement.Controls.Add(unreadDiv);
                unreadDiv.Controls.Add(link);
            }
            else
            {
                liElement.Controls.Add(link);
            } 

            if (showUnread)
            {
                var unreadLabel = new HtmlGenericControl("span");

                unreadLabel.Attributes.Add("class", "Unread");

                var unreadlink = new HyperLink
                {
                    Target = "_top",
                    ToolTip = unreadText,
                    NavigateUrl = linkUrl,
                    Text = unread
                };

                unreadLabel.Controls.Add(unreadlink);

                unreadDiv.Controls.Add(unreadLabel);
            }

            holder.Controls.Add(liElement);
        }

        /// <summary>
        ///  Render the Quick Search
        /// </summary>
        private void RenderQuickSearch()
        {
            if ((!this.Get<YafBoardSettings>().ShowQuickSearch ||
                 !this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ExternalSearchPermissions)) &&
                (!this.Get<YafBoardSettings>().ShowQuickSearch ||
                 !this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions)))
            {
                return;
            }

            this.quickSearch.Visible = true;

            var searchIcon = this.Get<ITheme>().GetItem("ICONS", "SEARCH");

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
                    this.menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "ADMIN"),
                    this.GetText("TOOLBAR", "ADMIN_TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_admin),
                    false,
                    false,
                    null,
                    null);
            }

            // Host
            if (this.PageContext.IsHostAdmin)
            {
                this.AdminModHolder.Visible = true;

                // Admin
                RenderMenuItem(
                    this.menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "HOST"),
                    this.GetText("TOOLBAR", "HOST_TITLE"),
                    YafBuildLink.GetLink(ForumPages.admin_hostsettings),
                    false,
                    false,
                    null,
                    null);
            }

            // Moderate
            if (this.PageContext.IsModeratorInAnyForum)
            {
                this.AdminModHolder.Visible = true;

                // Admin
                RenderMenuItem(
                    this.menuAdminItems,
                    "menuAdmin",
                    null,
                    this.GetText("TOOLBAR", "MODERATE"),
                    this.GetText("TOOLBAR", "MODERATE_TITLE"),
                    YafBuildLink.GetLink(ForumPages.moderate_index),
                    false,
                    false,
                    null,
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
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("DEFAULT", "FORUM"),
                    this.GetText("TOOLBAR", "FORUM_TITLE"),
                    YafBuildLink.GetLink(ForumPages.forum),
                    false,
                    false,
                    null,
                    null);

            // Active Topics
            if (this.PageContext.IsGuest)
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "ACTIVETOPICS"),
                    this.GetText("TOOLBAR", "ACTIVETOPICS_TITLE"),
                    YafBuildLink.GetLink(ForumPages.mytopics),
                    false,
                    false,
                    null,
                    null);
            }

            // Search
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ExternalSearchPermissions) || this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().SearchPermissions))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "SEARCH"),
                    this.GetText("TOOLBAR", "SEARCH_TITLE"),
                    YafBuildLink.GetLink(ForumPages.search),
                    false,
                    false,
                    null,
                    null);
            }

            // Members
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().MembersListViewPermissions))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "MEMBERS"),
                    this.GetText("TOOLBAR", "MEMBERS_TITLE"),
                    YafBuildLink.GetLink(ForumPages.members),
                    false, 
                    false,
                    null,
                    null);
            }

            // Team
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowTeamTo))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "TEAM"),
                    this.GetText("TOOLBAR", "TEAM_TITLE"),
                    YafBuildLink.GetLink(ForumPages.team),
                    false, 
                    false,
                    null,
                    null);
            }

            // Help
            if (this.Get<IPermissions>().Check(this.Get<YafBoardSettings>().ShowHelpTo))
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "HELP"),
                    this.GetText("TOOLBAR", "HELP_TITLE"),
                    YafBuildLink.GetLink(ForumPages.help_index),
                    false, 
                    false,
                    null,
                    null);
            }

            // Login
            if (this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                if (this.Get<YafBoardSettings>().UseLoginBox && !(this.Get<IYafSession>().UseMobileTheme ?? false))
                {
                    RenderMenuItem(
                        this.menuListItems,
                        "menuAccount",
                        "LoginLink",
                        this.GetText("TOOLBAR", "LOGIN"),
                        this.GetText("TOOLBAR", "LOGIN_TITLE"),
                        "javascript:void(0);",
                        true,
                        false,
                        null,
                        null);
                }
                else
                {
                    string returnUrl = this.GetReturnUrl().IsSet()
                                           ? "ReturnUrl={0}".FormatWith(this.GetReturnUrl())
                                           : string.Empty;

                    RenderMenuItem(
                        this.menuListItems,
                        "menuAccount",
                        null,
                        this.GetText("TOOLBAR", "LOGIN"),
                        this.GetText("TOOLBAR", "LOGIN_TITLE"),
                        !this.Get<YafBoardSettings>().UseSSLToLogIn
                            ? YafBuildLink.GetLink(ForumPages.login, returnUrl)
                            : YafBuildLink.GetLink(ForumPages.login, true, returnUrl).Replace("http:", "https:"),
                        true,
                        false,
                        null,
                        null);
                }
            }

            // Register
            if (this.PageContext.IsGuest && !this.Get<YafBoardSettings>().DisableRegistrations && !Config.IsAnyPortal)
            {
                RenderMenuItem(
                    this.menuListItems,
                    "menuGeneral",
                    null,
                    this.GetText("TOOLBAR", "REGISTER"),
                    this.GetText("TOOLBAR", "REGISTER_TITLE"),
                    this.Get<YafBoardSettings>().ShowRulesForRegistration
                        ? YafBuildLink.GetLink(ForumPages.rules)
                        : (!this.Get<YafBoardSettings>().UseSSLToRegister
                               ? YafBuildLink.GetLink(ForumPages.register)
                               : YafBuildLink.GetLink(ForumPages.register, true).Replace("http:", "https:")),
                    true,
                    false,
                    null,
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
            this.MyProfile.ToolTip = this.GetText("TOOLBAR", "MYPROFILE_TITLE");
            this.MyProfile.NavigateUrl = YafBuildLink.GetLink(ForumPages.cp_profile);
            this.MyProfile.Text = this.GetText("TOOLBAR", "MYPROFILE");

            // My Inbox
            if (this.Get<YafBoardSettings>().AllowPrivateMessages)
            {
                RenderMenuItem(
                    this.MyInboxItem,
                    "menuMy myPm",
                    null,
                    this.GetText("TOOLBAR", "INBOX"),
                    this.GetText("TOOLBAR", "INBOX_TITLE"),
                    YafBuildLink.GetLink(ForumPages.cp_pm),
                    false,
                    this.PageContext.UnreadPrivate > 0,
                    this.PageContext.UnreadPrivate.ToString(),
                    this.GetText("TOOLBAR", "NEWPM").FormatWith(this.PageContext.UnreadPrivate));
            }

            // My Buddies
            if (this.Get<YafBoardSettings>().EnableBuddyList && this.PageContext.UserHasBuddies)
            {
                RenderMenuItem(
                    this.MyBuddiesItem,
                    "menuMy myBuddies",
                    null,
                    this.GetText("TOOLBAR", "BUDDIES"),
                    this.GetText("TOOLBAR", "BUDDIES_TITLE"),
                    YafBuildLink.GetLink(ForumPages.cp_editbuddies),
                    false,
                    this.PageContext.PendingBuddies > 0,
                    this.PageContext.PendingBuddies.ToString(),
                    this.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(this.PageContext.PendingBuddies));
            }

            // My Albums
            if (this.Get<YafBoardSettings>().EnableAlbum && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
            {
                RenderMenuItem(
                    this.MyAlbumsItem,
                    "menuMy myAlbums",
                    null,
                    this.GetText("TOOLBAR", "MYALBUMS"),
                    this.GetText("TOOLBAR", "MYALBUMS_TITLE"),
                    YafBuildLink.GetLinkNotEscaped(ForumPages.albums, "u={0}", this.PageContext.PageUserID),
                    false,
                    false,
                    null,
                    null);
            }

            bool unread = this.PageContext.UnreadPrivate > 0 || this.PageContext.PendingBuddies > 0/* ||
                          this.PageContext.UnreadTopics > 0*/;

            // My Topics
            RenderMenuItem(
                this.MyTopicItem,
                "menuMy myTopics",
                null,
                this.GetText("TOOLBAR", "MYTOPICS"),
                this.GetText("TOOLBAR", "MYTOPICS"),
                YafBuildLink.GetLink(ForumPages.mytopics),
                false,
                false,//this.PageContext.UnreadTopics > 0,
                string.Empty,//this.PageContext.UnreadTopics.ToString(),
                string.Empty);//this.GetText("TOOLBAR", "UNREADTOPICS").FormatWith(this.PageContext.UnreadTopics));
            
            // Logout
            if (!Config.IsAnyPortal && Config.AllowLoginAndLogoff)
            {
                this.LogutItem.Visible = true;
                this.LogOutButton.Text = this.GetText("TOOLBAR", "LOGOUT");
                this.LogOutButton.ToolTip = this.GetText("TOOLBAR", "LOGOUT");
            }

            // Logged in as : username
            this.LoggedInUserPanel.Visible = true;

            if (unread)
            {
                this.LoggedInUserPanel.CssClass = "loggedInUser unread";
            }

            this.LoggedInUserPanel.Controls.Add(
                new Label { Text = this.GetText("TOOLBAR", "LOGGED_IN_AS").FormatWith("&nbsp;") });

            var userLink = new UserLink
                {
                    ID = "UserLoggedIn", 
                    UserID = this.PageContext.PageUserID,
                    CssClass = "currentUser",
                    EnableHoverCard = false
                };

            this.LoggedInUserPanel.Controls.Add(userLink);
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

            this.GuestMessage.Text = this.GetText("TOOLBAR", "WELCOME_GUEST_FULL");

            var endPoint = new Label { Text = "." };

            var isLoginAllowed = false;
            var isRegisterAllowed = false;

            if (Config.IsAnyPortal)
            {
              this.GuestMessage.Text = this.GetText("TOOLBAR", "WELCOME_GUEST");
            }
            else
            {
                if (Config.AllowLoginAndLogoff)
                {
                    // show login
                    var loginLink = new HyperLink
                    {
                        Text = this.GetText("TOOLBAR", "LOGIN"),
                        ToolTip = this.GetText("TOOLBAR", "LOGIN")
                    };

                    if (this.Get<YafBoardSettings>().UseLoginBox && !(this.Get<IYafSession>().UseMobileTheme ?? false))
                    {
                        loginLink.NavigateUrl = "javascript:void(0);";

                        loginLink.CssClass = "LoginLink";
                    }
                    else
                    {
                        string returnUrl = this.GetReturnUrl().IsSet()
                                               ? "ReturnUrl={0}".FormatWith(this.GetReturnUrl())
                                               : string.Empty;

                        loginLink.NavigateUrl = !this.Get<YafBoardSettings>().UseSSLToLogIn
                                                    ? YafBuildLink.GetLink(ForumPages.login, returnUrl)
                                                    : YafBuildLink.GetLink(ForumPages.login, true, returnUrl).Replace(
                                                        "http:", "https:");
                    }

                    this.GuestUserMessage.Controls.Add(loginLink);

                    isLoginAllowed = true;
                }

                if (!this.Get<YafBoardSettings>().DisableRegistrations)
                {
                    if (isLoginAllowed)
                    {
                        this.GuestUserMessage.Controls.Add(
                            new Label { Text = "&nbsp;{0}&nbsp;".FormatWith(this.GetText("COMMON", "OR")) });
                    }

                    // show register link
                    var registerLink = new HyperLink
                    {
                        Text = this.GetText("TOOLBAR", "REGISTER"),
                        NavigateUrl =
                            this.Get<YafBoardSettings>().ShowRulesForRegistration
                                ? YafBuildLink.GetLink(ForumPages.rules)
                                : (!this.Get<YafBoardSettings>().UseSSLToRegister
                                       ? YafBuildLink.GetLink(ForumPages.register)
                                       : YafBuildLink.GetLink(ForumPages.register, true).Replace("http:", "https:"))
                    };

                    this.GuestUserMessage.Controls.Add(registerLink);

                    this.GuestUserMessage.Controls.Add(endPoint);

                    isRegisterAllowed = true;
                }
                else
                {
                    this.GuestUserMessage.Controls.Add(endPoint);

                    this.GuestUserMessage.Controls.Add(
                          new Label { Text = this.GetText("TOOLBAR", "DISABLED_REGISTER") });
                }

                // If both disallowed
                if (!isLoginAllowed && !isRegisterAllowed)
                {
                    this.GuestUserMessage.Controls.Clear();
                    this.GuestUserMessage.Controls.Add(
                          new Label { Text = this.GetText("TOOLBAR", "WELCOME_GUEST_NO") });
                }
            }           
        }

        #endregion
    }
}