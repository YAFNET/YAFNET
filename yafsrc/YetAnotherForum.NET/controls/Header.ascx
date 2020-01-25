<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="YAF.Controls.Header" %>
<%@ Import Namespace="YAF.Types.Constants" %>

<%@ Register TagPrefix="YAF" TagName="AdminMenu" Src="AdminMenu.ascx" %>
<%@ Register TagPrefix="YAF" TagName="UserMenu" Src="UserMenu.ascx" %>

<YAF:Alert runat="server" Visible="False" ID="GuestUserMessage" 
           Type="info" 
           Dismissing="True">
    <asp:Label id="GuestMessage" runat="server"></asp:Label>
</YAF:Alert>
    
<header class="mb-3">
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <a class="navbar-brand mb-1" href="<%=BuildLink.GetLink(ForumPages.forum) %>">
            <%= this.PageContext.BoardSettings.Name %>
        </a>
        
        <button class="navbar-toggler" type="button" 
                data-toggle="collapse" 
                data-target="#navbarSupportedContent" 
                aria-controls="navbarSupportedContent" 
                aria-expanded="false" 
                aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav mr-auto">
                <asp:PlaceHolder ID="menuListItems" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="AdminModHolder" runat="server" Visible="false">
                    <asp:PlaceHolder ID="menuAdminItems" runat="server"></asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="AdminAdminHolder" Visible="False">
                        <YAF:AdminMenu runat="server"></YAF:AdminMenu>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="HostMenuHolder" Visible="False">
                        <li class="nav-item dropdown">
                            <YAF:ThemeButton runat="server" ID="hostDropdown"
                                             DataToggle="dropdown"
                                             Type="None"
                                             CssClass="nav-link dropdown-toggle"
                                             TextLocalizedTag="HOST"
                                             TextLocalizedPage="TOOLBAR" />
                            <div class="dropdown-menu" aria-labelledby="hostDropdown">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_hostsettings) %>"
                               class="<%= this.PageContext.ForumPageType == ForumPages.admin_hostsettings ? "dropdown-item active" : "dropdown-item" %>">
                            <i class="fa fa-cog fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_hostsettings"></YAF:LocalizedLabel>
                            </a>
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_boards) %>"
                               class="<%= this.PageContext.ForumPageType == ForumPages.admin_boards || this.PageContext.ForumPageType == ForumPages.admin_editboard ? "dropdown-item active" : "dropdown-item" %>">
                                <i class="fa fa-globe fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_boards" LocalizedPage="adminmenu"></YAF:LocalizedLabel>
                            </a>
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_pageaccesslist) %>"
                               class="<%= this.PageContext.ForumPageType == ForumPages.admin_pageaccesslist || this.PageContext.ForumPageType == ForumPages.admin_pageaccessedit ? "dropdown-item active" : "dropdown-item" %>">
                                <i class="fa fa-building fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="admin_pageaccesslist"></YAF:LocalizedLabel>
                            </a>
                        </div>
                  </li>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="ModerateHolder" Visible="False">
                        <li class="nav-item dropdown">
                        <a class="nav-link dropdown-toggle" id="moderateDropdown" data-toggle="dropdown" 
                           href="<%= BuildLink.GetLink(ForumPages.admin_hostsettings) %>" 
                           role="button" 
                           aria-haspopup="true" aria-expanded="false">
                           <%= this.GetText("TOOLBAR", "HOST")  %>
                        </a>
                        <div class="dropdown-menu" aria-labelledby="moderateDropdown">
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_hostsettings) %>"
                               class="dropdown-item">
                            <i class="fa fa-cog fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" LocalizedTag="admin_hostsettings"></YAF:LocalizedLabel>
                            </a>
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_boards) %>"
                               class="dropdown-item">
                                <i class="fa fa-globe fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" LocalizedTag="admin_boards"></YAF:LocalizedLabel>
                            </a>
                            <a href="<%= BuildLink.GetLink(ForumPages.admin_pageaccesslist) %>"
                               class="dropdown-item">
                                <i class="fa fa-building fa-fw"></i>&nbsp;
                                <YAF:LocalizedLabel runat="server" LocalizedTag="admin_pageaccesslist"></YAF:LocalizedLabel>
                            </a>
                        </div>
                  </li>
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
                <asp:PlaceHolder id="LoggedInUserPanel" runat="server" Visible="false">
                    <YAF:UserMenu runat="server"></YAF:UserMenu>
                </asp:PlaceHolder>
            </ul>
            <asp:Panel ID="quickSearch" runat="server" CssClass="form-inline" Visible="false">
                <asp:TextBox ID="searchInput" Type="Search" runat="server" 
                             CssClass="form-control"
                             aria-label="Search"></asp:TextBox>&nbsp;
                <YAF:ThemeButton ID="doQuickSearch" runat="server"
                                 CssClass="my-2 my-sm-0"
                                 Type="OutlineInfo"
                                 OnClick="QuickSearchClick"
                                 Icon="search">
                </YAF:ThemeButton>
            </asp:Panel>
        </div>
    </nav>
</header>