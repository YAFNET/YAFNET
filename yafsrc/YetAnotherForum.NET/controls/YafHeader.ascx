<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YafHeader.ascx.cs" Inherits="YAF.Controls.YafHeader" %>
<%@ Import Namespace="YAF.Classes" %>


<div id="yafheader">
   <% if (this.PageContext.IsGuest) {%>
    <div class="guestUser">
      <%=this.PageContext.Localization.GetText("TOOLBAR", this.PageContext.IsGuest ? "WELCOME_GUEST" : "LOGGED_IN_AS").FormatWith("&nbsp;")%>
    </div>
    <%
     }%>
   
   <div class="outerMenuContainer">   
    <div class="menuMyContainer">
      <ul class="menuMyList">
      <% if (!this.PageContext.IsGuest)
               {%>
       <li class="menuMy"><a target='_top'  title="<%=this.PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")%>" href="<%=YafBuildLink.GetLink(ForumPages.cp_profile) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYPROFILE")%></a> </li>
                 <%
                    }%>
       <% if (!this.PageContext.IsGuest && this.PageContext.BoardSettings.AllowPrivateMessages)
               {%>
            <li class="menuMy"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "INBOX")%>" href="<%=YafBuildLink.GetLink(ForumPages.cp_pm)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "INBOX")%>
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
            <li class="menuMy"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "BUDDIES")%>" href="<%=YafBuildLink.GetLink(ForumPages.cp_editbuddies)%>">
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
            <li class="menuMy"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "MYALBUMS")%>" href="<%=YafBuildLink.GetLink(ForumPages.albums, "u={0}", this.PageContext.PageUserID)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYALBUMS")%></a> </li>
            <%
                }                    
            %>
           
            <%if (!this.PageContext.IsGuest)
              {%>

            <li class="menuMy"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")%>" href="<%=YafBuildLink.GetLink(ForumPages.mytopics) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MYTOPICS")%></a> </li>
            <%
                }%>
                 <%
                if (!this.PageContext.IsGuest && !YAF.Classes.Config.IsAnyPortal && YAF.Classes.Config.AllowLoginAndLogoff)
                {%>
            <li class="menuAccount"><asp:LinkButton ID="LogOutButton" runat="server" OnClick="LogOutClick"><%=this.PageContext.Localization.GetText("TOOLBAR", "LOGOUT")%></asp:LinkButton></li>
            <%
                }    
            %>
      </ul>
    </div>
    <% if (!this.PageContext.IsGuest) {%>
    <div class="loggedInUser">
     
        <%=this.PageContext.Localization.GetText("TOOLBAR", "LOGGED_IN_AS").FormatWith("&nbsp;")%>
        <%= new UserLink()
{
    ID = "UserLoggedIn",
    Visible = !this.PageContext.IsGuest,
    UserID =  this.PageContext.PageUserID,
    CssClass = "currentUser"
    ,
}.RenderToString() %>
 </div>
 <%
     }%>
    <div class="menuContainer">
        <ul class="menuList">
           <li class="menuGeneral"><a target='_top' title="<%=this.PageContext.Localization.GetText("DEFAULT", "FORUM")%>" href="<%=YafBuildLink.GetLink(ForumPages.forum) %>">
                 <%=this.PageContext.Localization.GetText("DEFAULT", "FORUM")%></a> </li>
           <%if (this.PageContext.IsGuest)
              {%>
             <li class="menuGeneral"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")%>" href="<%=YafBuildLink.GetLink(ForumPages.mytopics) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "ACTIVETOPICS")%></a> </li>
                 <%
                 }%>
            <%
                if (this.Get<YafPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || this.Get<YafPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions))
                {%>
            <li class="menuGeneral"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")%>" href="<%=YafBuildLink.GetLink(ForumPages.search)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "SEARCH")%></a> </li>
            <%
                }                    
            %>
            <%
                if (this.Get<YafPermissions>().Check(this.PageContext.BoardSettings.MembersListViewPermissions))
                {%>
            <li class="menuGeneral"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")%>" href="<%=YafBuildLink.GetLink(ForumPages.members)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MEMBERS")%></a> </li>
                <li class="menuGeneral"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "TEAM")%>" href="<%=YafBuildLink.GetLink(ForumPages.team)%>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "TEAM")%></a> </li>
            <%
                }                    
            %>
             <%
                if (this.PageContext.BoardSettings.ShowHelp)
                {%>
            <li class="menuGeneral"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "HELP")%>" href="<%=YafBuildLink.GetLink(ForumPages.help_index) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "HELP")%></a> </li>
                 <%
                }                    
            %>
            <%
                if (this.PageContext.IsGuest && !YAF.Classes.Config.IsAnyPortal && YAF.Classes.Config.AllowLoginAndLogoff)
                {
                    string returnUrl = this.GetReturnUrl().IsSet()
                                         ? "ReturnUrl={0}".FormatWith(
                                           this.PageContext.BoardSettings.UseSSLToLogIn
                                             ? this.GetReturnUrl().Replace("http:", "https:")
                                             : this.GetReturnUrl())
                                         : string.Empty;

                    if (this.PageContext.BoardSettings.UseLoginBox && !(YafContext.Current.Get<YafSession>().UseMobileTheme ?? false))
                {%>
            <li class="menuAccount"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")%>" class="LoginLink" href="#">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")%>
            </a></li>
             <%
                }
                else
                {%>
                  <li class="menuAccount"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")%>" href="<%= YafBuildLink.GetLink(ForumPages.login, returnUrl) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "LOGIN")%></a></li>
                    
                <%}%>
            <%
                }%>
            <%
                if (this.PageContext.IsGuest && !this.PageContext.BoardSettings.DisableRegistrations && !YAF.Classes.Config.IsAnyPortal)
                {%>
            <li class="menuAccount"><a title="<%=this.PageContext.Localization.GetText("TOOLBAR", "REGISTER")%>" href="<%=this.PageContext.BoardSettings.ShowRulesForRegistration
                                                        ? YafBuildLink.GetLink(ForumPages.rules)
                                                        : (!this.PageContext.BoardSettings.UseSSLToRegister ? YafBuildLink.GetLink(ForumPages.register) : YafBuildLink.GetLink(ForumPages.register).Replace("http:", "https:")) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "REGISTER")%></a></li>
            <%
                }%>
        </ul>
         <%
             if (this.PageContext.BoardSettings.ShowQuickSearch && this.Get<YafPermissions>().Check(this.PageContext.BoardSettings.ExternalSearchPermissions) || this.PageContext.BoardSettings.ShowQuickSearch && this.Get<YafPermissions>().Check(this.PageContext.BoardSettings.SearchPermissions))
                {%>
                
        <div id="quickSearch" class="QuickSearch" runat="server">
          <asp:TextBox ID="searchInput" runat="server" ></asp:TextBox>&nbsp;
          <asp:LinkButton ID="doQuickSearch" onkeydown="" runat="server" CssClass="QuickSearchButton" OnClick="QuickSearchClick"></asp:LinkButton>
        </div>
         <%
                }
                if (this.PageContext.IsAdmin)
                {%>
        <ul class="menuAdminList">    
            <li class="menuAdmin"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "ADMIN")%>" href="<%=YafBuildLink.GetLink(ForumPages.admin_admin) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "ADMIN")%></a> </li>
            <%
                }

                if (this.PageContext.IsHostAdmin)
                {%>
            <li class="menuAdmin"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "HOST")%>" href="<%=YafBuildLink.GetLink(ForumPages.admin_hostsettings) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "HOST")%></a> </li>
            <%
                }%>
            <%if (this.PageContext.IsModerator || this.PageContext.IsForumModerator)
              {%>
            <li class="menuAdmin"><a target='_top' title="<%=this.PageContext.Localization.GetText("TOOLBAR", "MODERATE")%>" href="<%=YafBuildLink.GetLink(ForumPages.moderate_index) %>">
                <%=this.PageContext.Localization.GetText("TOOLBAR", "MODERATE")%></a> </li>
        </ul>
         <% 
                }%>
    </div>
    </div>
    <div id="yafheaderEnd"></div>
</div>
