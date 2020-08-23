<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMenu.ascx.cs" Inherits="YAF.Controls.AdminMenu" %>

<li class="nav-item dropdown">
    <YAF:ThemeButton runat="server" ID="AdminDropdown"
                     Type="None"
                     DataToggle="dropdown"
                     TextLocalizedTag="ADMIN"
                     TextLocalizedPage="ADMINMENU"
                     NavigateUrl="#">
    </YAF:ThemeButton>
    <ul class="dropdown-menu" aria-labelledby="adminDropdown">
        <asp:PlaceHolder runat="Server" ID="MenuHolder"></asp:PlaceHolder>
    </ul>
</li>