<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Header.ascx.cs" Inherits="YAF.Controls.Header" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Core.Services" %>

<%@ Register TagPrefix="YAF" TagName="AdminMenu" Src="AdminMenu.ascx" %>
<%@ Register TagPrefix="YAF" TagName="UserMenu" Src="UserMenu.ascx" %>

<header class="mb-2">
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <div class="container-fluid">
        <a class="navbar-brand mb-1" href="<%=this.Get<LinkBuilder>().GetLink(ForumPages.Board) %>">
            <%= this.PageBoardContext.BoardSettings.Name %>
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
                                       class="<%= this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_HostSettings ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="cog" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_hostsettings" />
                                    </a>
                                </li>
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Boards) %>"
                                       class="<%= this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_Boards || this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_EditBoard ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="globe" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_boards" LocalizedPage="adminmenu"></YAF:LocalizedLabel>
                                    </a>
                                </li>
                                <li>
                                    <a href="<%= this.Get<LinkBuilder>().GetLink(ForumPages.Admin_PageAccessList) %>"
                                       class="<%= this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_PageAccessList || this.PageBoardContext.CurrentForumPage.PageType == ForumPages.Admin_PageAccessEdit ? "dropdown-item active" : "dropdown-item" %>">
                                        <YAF:Icon runat="server" IconName="building" />
                                        <YAF:LocalizedLabel runat="server" 
                                                            LocalizedTag="admin_pageaccesslist"/>
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
            <asp:PlaceHolder runat="server" ID="ThemeModeSelector" Visible="False">
	            <div class="py-2 py-lg-1 col-12 col-lg-auto">
		            <div class="vr d-none d-lg-flex h-100 mx-lg-2 text-white"></div>
		            <hr class="d-lg-none my-2 text-white-50">
	            </div>
                        
	            <div class="d-flex">
		            <ul class="navbar-nav">
                        <li class="nab-item dropdown">
	                        <button class="btn btn-link nav-link py-2 px-0 px-lg-2 dropdown-toggle d-flex align-items-center" type="button" id="bd-theme" data-bs-toggle="dropdown" aria-expanded="false" title='<%= this.GetText("SELECT_THEME_MODE") %>'>
		                        <YAF:Icon runat="server" IconName="moon theme-icon-active"></YAF:Icon>
		                        <span class="d-lg-none ms-2" id="bd-theme-text"><YAF:LocalizedLabel runat="server" LocalizedTag="SELECT_THEME_MODE"></YAF:LocalizedLabel></span>
	                        </button>
	                        <ul class="dropdown-menu dropdown-menu-end">
		                        <li>
			                        <button class="dropdown-item d-flex align-items-center" data-bs-theme-value="light" type="button">
				                        <YAF:Icon runat="server" IconName="sun" />
				                        <YAF:LocalizedLabel runat="server" 
				                                            LocalizedTag="LIGHT" LocalizedPage="THEME_MODE"/>
				                        <YAF:Icon runat="server" IconName="check ms-4 d-none"></YAF:Icon>
			                        </button>
		                        </li>
		                        <li>
			                        <button class="dropdown-item d-flex align-items-center" data-bs-theme-value="dark" type="button">
				                        <YAF:Icon runat="server" IconName="moon" />
				                        <YAF:LocalizedLabel runat="server" 
				                                            LocalizedTag="DARK" LocalizedPage="THEME_MODE"/>
				                        <YAF:Icon runat="server" IconName="check ms-4 d-none"></YAF:Icon>
			                        </button>
		                        </li>
		                        <li>
			                        <button class="dropdown-item d-flex align-items-center" data-bs-theme-value="auto" type="button">
				                        <YAF:Icon runat="server" IconName="circle-half-stroke" />
				                        <YAF:LocalizedLabel runat="server" 
				                                            LocalizedTag="AUTO" LocalizedPage="THEME_MODE"/>
				                        <YAF:Icon runat="server" IconName="check ms-4 d-none"></YAF:Icon>
			                        </button>
		                        </li>
	                        </ul>
                        </li>
		            </ul>
	            </div>
            </asp:PlaceHolder>
        </div>
        </div>
    </nav>
</header>