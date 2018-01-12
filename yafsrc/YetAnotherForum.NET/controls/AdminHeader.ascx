<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminHeader.ascx.cs" Inherits="YAF.Controls.AdminHeader" %>
<%@ Import Namespace="YAF.Classes" %>

<a class="nav-link navbar-toggler d-sm-none" data-toggle="collapse" data-target=".navbar-toggleable-xs" aria-expanded="false" aria-label="Toggle navigation" href="#">
               <i class="fa fa-bars fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedPage="COMMON" LocalizedTag="MENU"></YAF:LocalizedLabel>
            </a>

<nav class="navbar navbar-light bg-light" role="navigation" style="margin-bottom: 0">
    <ul class="nav navbar-top-links justify-content-end">
        <!-- /.dropdown -->
        <li class="dropdown nav-item">
             <a class="nav-link" data-toggle="dropdown" href="#">
                 <i class="fa fa-life-ring fa-fw"></i>  <i class="fa fa-caret-down"></i>
             </a>
            <ul class="dropdown-menu">
                <asp:placeholder id="menuListItems" runat="server"></asp:placeholder>
                <li class="dropdown-divider"></li>
                <asp:placeholder id="menuAdminItems" runat="server"></asp:placeholder>
            </ul>
            <!-- /.dropdown-tasks -->
        </li>
        <!-- /.dropdown -->
        <li class="dropdown nav-item"> <a class="nav-link" data-toggle="dropdown" href="#">

                        <i class="fa fa-user fa-fw"></i><span class="hidden-sm-down">&nbsp;<%= this.Get<YafBoardSettings>().EnableDisplayName ? this.PageContext.CurrentUserData.DisplayName : this.PageContext.CurrentUserData.UserName  %></span>  <i class="fa fa-caret-down"></i>

                    </a>
            <ul class="dropdown-menu dropdown-user">
                <asp:PlaceHolder id="MyProfile" runat="server"></asp:PlaceHolder>
                <asp:placeholder id="MyInboxItem" runat="server"></asp:placeholder>
                <asp:placeholder id="MyBuddiesItem" runat="server"></asp:placeholder>
                <asp:placeholder id="MyAlbumsItem" runat="server"></asp:placeholder>
                <asp:placeholder id="MyTopicItem" runat="server"></asp:placeholder>
                <li class="dropdown-divider"></li>
                <asp:placeholder runat="server" id="LogoutItem">
                    <li class="dropdown-item">
                        <asp:linkbutton id="LogOutButton" runat="server" onclick="LogOutClick"></asp:linkbutton>
                    </li>
                </asp:placeholder>
            </ul>
            <!-- /.dropdown-user -->
        </li>
        <!-- /.dropdown -->
    </ul>
