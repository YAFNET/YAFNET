<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YafHeader.ascx.cs" Inherits="YAF.Controls.YafHeader" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Classes" %>
    <asp:Panel id="GuestUserMessage" CssClass="alert alert-info alert-dismissible fade show" runat="server" Visible="false" role="alert">
       <asp:Label id="GuestMessage" runat="server"></asp:Label>
        <button type="button" class="close" data-dismiss="alert" aria-label="Close">
            <span aria-hidden="true">&times;</span>
        </button>
    </asp:Panel>
    
<header class="mb-3">
    <nav class="navbar navbar-expand-lg navbar-dark bg-dark">
        <a class="navbar-brand" href="#">
            <%= this.PageContext.BoardSettings.Name %>
        </a>
        
        <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarSupportedContent" aria-controls="navbarSupportedContent" aria-expanded="false" aria-label="Toggle navigation">
            <span class="navbar-toggler-icon"></span>
        </button>

        <div class="collapse navbar-collapse" id="navbarSupportedContent">
            <ul class="navbar-nav mr-auto">
                <asp:PlaceHolder ID="menuListItems" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="AdminModHolder" runat="server" Visible="false">
                    <asp:PlaceHolder ID="menuAdminItems" runat="server"></asp:PlaceHolder>
                </asp:PlaceHolder>
                <li class="nav-item dropdown">
                    <asp:PlaceHolder id="LoggedInUserPanel" runat="server" Visible="false">
                        <a class="nav-link dropdown-toggle" id="userDropdown" data-toggle="dropdown" 
                           href="<%= YafBuildLink.GetLink(ForumPages.profile, "u={0}&name={1}", this.PageContext.PageUserID, 
                                         this.Get<YafBoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName) %>" 
                           role="button" 
                           aria-haspopup="true" aria-expanded="false">
                            <i class="fa fa-user fa-fw"></i>&nbsp;<%= this.Get<YafBoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName  %>
                            <asp:PlaceHolder runat="server" id="UnreadPlaceHolder">
                                <span class="badge badge-danger">
                                    <%= (this.PageContext.UnreadPrivate + this.PageContext.PendingBuddies).ToString() %>
                                </span>
                            </asp:PlaceHolder>
                        </a>
                        <div class="dropdown-menu" aria-labelledby="userDropdown">
                            <asp:HyperLink id="MyProfile" runat="server" Target="_top"
                                           CssClass="dropdown-item"></asp:HyperLink>
                            <asp:PlaceHolder ID="MyInboxItem" runat="server">
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="MyBuddiesItem" runat="server">
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="MyAlbumsItem" runat="server">
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="MyTopicItem" runat="server">
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="LogutItem" runat="server" Visible="false">
                                <div class="dropdown-divider"></div>
                                <asp:LinkButton ID="LogOutButton" runat="server" 
                                            CssClass="dropdown-item"
                                            OnClick="LogOutClick">
                                </asp:LinkButton>
                            </asp:PlaceHolder>
                        </div>
                    </asp:PlaceHolder>
                </li>
            </ul>
            <asp:Panel ID="quickSearch" runat="server" CssClass="QuickSearch" Visible="false">
                <YAF:ModernTextBox ID="searchInput" Type="Search" runat="server"></YAF:ModernTextBox>&nbsp;
                <YAF:ThemeButton ID="doQuickSearch" runat="server"
                                 CssClass="btn btn-outline-success my-2 my-sm-0"
                                 OnClick="QuickSearchClick"
                                 TextLocalizedTag="BTNSEARCH" TitleLocalizedTag="SEARCH"
                                 Icon="search">
                </YAF:ThemeButton>
            </asp:Panel>
        </div>
    </nav>
</header>