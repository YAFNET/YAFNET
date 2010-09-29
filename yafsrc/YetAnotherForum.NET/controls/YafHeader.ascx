<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YafHeader.ascx.cs" Inherits="YAF.Controls.YafHeader" %>
<div id="yafheader">
    <div class="loggedInUser">
        <%=this.PageContext.Localization.GetText("TOOLBAR", this.PageContext.IsGuest ? "WELCOME_GUEST" : "LOGGED_IN_AS").FormatWith(String.Empty) %>
        <%= new UserLabel()
{
    ID = "UserLoggedIn",
    Visible = !this.PageContext.IsGuest,
    UserID =  this.PageContext.PageUserID,
    CssClass = "currentUser"
}.RenderToString() %>
    </div>
    <div class="menuContainer">
        <ul class="menuList">
            <% if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.AllowPrivateMessages)
               {%>
            <li class="menuMy"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.cp_pm)%>">
                <%=this.PageContext.Localization.GetText("CP_PM", "INBOX")%>
            </a>
                <%
                    if (this.PageContext.UnreadPrivate > 0)
                    {%>
                <span class="unread">
                    <%=
                     this.PageContext.Localization.GetText("TOOLBAR", "NEWPM").FormatWith(
                       this.PageContext.UnreadPrivate)%></span>
                <%
                    }%>
            </li>
            <%
                }
               if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableBuddyList && this.PageContext.UserHasBuddies)
               {%>
            <li class="menuMy"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.cp_editbuddies)%>">
                <%= this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES")%></a>
                <%
                    if (this.PageContext.PendingBuddies > 0)
                    {%>
                <span class="unread">
                    <%=
                           this.PageContext.Localization.GetText("TOOLBAR", "BUDDYREQUEST").FormatWith(
                             this.PageContext.PendingBuddies)%></span>
                <% } %>
            </li>
            <% } %>
            <%
                if (!this.PageContext.IsGuest && YafContext.Current.BoardSettings.EnableAlbum && (this.PageContext.UsrAlbums > 0 || this.PageContext.NumAlbums > 0))
                {%>
            <li class="menuMy"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYALBUMS")%></a> </li>
            <%
                }                    
            %>
            <li class="menuMy"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.mytopics) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")%></a> </li>
            <%if (!this.PageContext.IsGuest)
              {%>
            <li class="menuMy"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.cp_profile) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")%></a> </li>
            <%
                }%>
            <%
                if (YafServices.Permissions.Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || YafServices.Permissions.Check(this.PageContext.BoardSettings.SearchPermissions))
                {%>
            <li class="menuGeneral"><a href="<%=YafBuildLink.GetLink(ForumPages.search)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")%></a> </li>
            <%
                }                    
            %>
            <%
                if (YafServices.Permissions.Check(this.PageContext.BoardSettings.MembersListViewPermissions))
                {%>
            <li class="menuGeneral"><a href="<%=YafBuildLink.GetLink(ForumPages.members)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")%></a> </li>
            <%
                }                    
            %>

            <li class="menuGeneral"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.help_index) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "HELP")%></a> </li>
            <%
                if (this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
                {
                    string returnUrl = this.GetReturnUrl().IsSet()
                                         ? "ReturnUrl={0}".FormatWith(
                                           this.PageContext.BoardSettings.UseSSLToLogIn
                                             ? this.GetReturnUrl().Replace("http:", "https:")
                                             : this.GetReturnUrl())
                                         : string.Empty;
            %>
            <li class="menuAccount"><a href="<%= YafBuildLink.GetLink(ForumPages.login, returnUrl) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")%>
            </a></li>
            <%
                }%>
            <%
                if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.DisableRegistrations)
                {%>
            <li class="menuAccount"><a href="<%=this.PageContext.BoardSettings.ShowRulesForRegistration
                                                        ? YafBuildLink.GetLink(ForumPages.rules)
                                                        : (!this.PageContext.BoardSettings.UseSSLToRegister ? YafBuildLink.GetLink(ForumPages.register) : YafBuildLink.GetLink(ForumPages.register).Replace("http:", "https:")) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "REGISTER")%></a></li>
            <%
                }%>
            <%
                if (!this.PageContext.IsGuest && !Config.IsAnyPortal && Config.AllowLoginAndLogoff)
                {%>
            <li class="menuAccount"><a href="<%=YafBuildLink.GetLink(ForumPages.logout)%>" onclick="return confirm('<%=this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT_QUESTION")%>');">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT")%></a></li>
            <%
                }    
            %>

            <%
                if (this.PageContext.IsAdmin)
                {%>
            <li class="menuAdmin"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.admin_admin) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "ADMIN")%></a> </li>
            <%
                }%>
            <%if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
              {%>
            <li class="menuAdmin"><a target='_top' href="<%=YafBuildLink.GetLink(ForumPages.moderate_index) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MODERATE")%></a> </li>
            <% 
                }%>
        </ul>
    </div>
</div>
