<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version"
    CodeBehind="version.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
    <table style="width:100%" class="content">
        <tr>
            <td class="header1">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_VERSION" />
            </td>
        </tr>
        <tr>
            <td class="post">
                <asp:PlaceHolder runat="server" ID="UpgradeVersionHolder" Visible="false">
                <div class="ui-widget">
                    <div class="ui-state-highlight ui-corner-all">
                        <p>
                            <span class="ui-icon ui-icon-info" style="float: left; margin-right: .3em;"></span>
                            <YAF:LocalizedLabel ID="Upgrade" runat="server" LocalizedTag="UPGRADE_VERSION" 
                                LocalizedPage="ADMIN_VERSION" />
                        </p>
                    </div>
                </div> 
                </asp:PlaceHolder>
                <div style="font-size: 11pt;">
                    <img src="~/Images/YafLogoSmall.png" alt="YAF.NET" style="float:left; padding: 10px" runat="server" />
                    <p>
                        <asp:Label ID="RunningVersion" runat="server"></asp:Label></p>
                    <p>
                        <asp:Label ID="LatestVersion" runat="server"></asp:Label></p>
                </div>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
