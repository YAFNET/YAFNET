<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version"
    CodeBehind="version.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
    <table width="100%" cellspacing="0" cellpadding="0" class="content">
        <tr>
            <td class="header1">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_VERSION" />
            </td>
        </tr>
        <tr>
            <td class="post">
                <div style="font-size: 11pt;">
                    <img src="<%=YafForumInfo.ForumClientFileRoot + "/images/YafLogo.png" %>" alt="YAF.NET" style="float:left; padding: 10px" />
                    <p>
                        <asp:Label ID="RunningVersion" runat="server"></asp:Label></p>
                    <p>
                        <asp:Label ID="LatestVersion" runat="server"></asp:Label></p>
                    <p>
                        <YAF:LocalizedLabel ID="Upgrade" runat="server" LocalizedTag="UPGRADE_VERSION" LocalizedPage="ADMIN_VERSION"
                            Visible="false" />
                    </p>
                </div>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
