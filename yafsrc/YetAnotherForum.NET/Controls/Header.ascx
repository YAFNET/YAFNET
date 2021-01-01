<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="YAF.Controls.Header" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Services" %>

<%@ Register TagPrefix="YAF" TagName="AdminMenu" Src="AdminMenu.ascx" %>
<%@ Register TagPrefix="YAF" TagName="UserMenu" Src="UserMenu.ascx" %>

<YAF:Alert runat="server" Visible="False" ID="GuestUserMessage" 
           Type="info" 
           Dismissing="True">
    <asp:Label id="GuestMessage" runat="server"></asp:Label>
</YAF:Alert>
    
<header class="mb-2">
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container-fluid">
        <a class="navbar-brand mb-1" href="<%=this.Get<LinkBuilder>().GetLink(ForumPages.Board) %>">
            <%= this.PageContext.BoardSettings.Name %>
        </a>
        
        <button class="navbar-toggler" type="button" 
                data-bs-toggle="collapse" 
                data-bs-target="#navbarSupportedContent" 
                aria-controls="navbarSupportedContent" 
                aria-expanded="false" 
                aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav me-auto">
                <asp:PlaceHolder ID="menuListItems" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="AdminModHolder" runat="server" 
                                 Visible="false">
                    <asp:PlaceHolder ID="menuModerateItems" runat="server"/>
                    <asp:PlaceHolder runat="server" ID="AdminAdminHolder" Visible="False">
                        <YAF:AdminMenu runat="server"></YAF:AdminMenu>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <asp:PlaceHolder runat="server" ID="HostMenuHolder" Visible="False">
                        <li class="nav-item dropdown">
                            <YAF:ThemeButton runat="server" ID="hostDropdown"
                                             DataToggle="dropdown"
                                             Type="None"
                                             CssClass="nav-link dropdown-toggle"
                                             TextLocalizedTag="HOST"
                                             TextLocalizedPage="TOOLBAR"
                                             NavigateUrl="#"/>
                            <ul class="dropdown-menu" aria-labelledby="hostDropdown">
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_HostSettings) %>"
                                       class="<%= this.PageContext.ForumPageType == ForumPages.Admin_HostSettings ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="cog" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_hostsettings" />
                                    </a>
                                </li>
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Boards) %>"
                                       class="<%= this.PageContext.ForumPageType == ForumPages.Admin_Boards || this.PageContext.ForumPageType == ForumPages.Admin_EditBoard ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="globe" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_boards" LocalizedPage="adminmenu"></YAF:LocalizedLabel>
                                    </a>
                                </li>
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_PageAccessList) %>"
                                       class="<%= this.PageContext.ForumPageType == ForumPages.Admin_PageAccessList || this.PageContext.ForumPageType == ForumPages.Admin_PageAccessEdit ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="building" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_pageaccesslist"/>
                                    </a>
                                </li>
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Profiler) %>"
                                       class="<%= this.PageContext.ForumPageType == ForumPages.Admin_Profiler ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="diagnoses" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_profiler"/>
                                    </a>
                                </li>
                        </ul>
                  </li>
                    </asp:PlaceHolder>
                <asp:PlaceHolder id="LoggedInUserPanel" runat="server" Visible="false">
                    <YAF:UserMenu runat="server"></YAF:UserMenu>
                </asp:PlaceHolder>
            </ul>
            <asp:Panel ID="quickSearch" runat="server" CssClass="d-flex" Visible="false">
                <asp:TextBox ID="searchInput" Type="Search" runat="server" 
                             CssClass="form-control me-2"
                             aria-label="Search"></asp:TextBox>
                <YAF:ThemeButton ID="doQuickSearch" runat="server"
                                 Type="OutlineInfo"
                                 OnClick="QuickSearchClick"
                                 Icon="search">
                </YAF:ThemeButton>
            </asp:Panel>
        </div>
        </div>
    </nav>
</header>