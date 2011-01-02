<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.version" Codebehind="version.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="adminmenu1" runat="server">
	<table width="100%" cellspacing="0" cellpadding="0" class="content">
		<tr>
			<td class="header1">
				 <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_VERSION" />
			</td>
		</tr>
        <tr>
			<td class="header2" height="30">
			</td>
		</tr>
		<tr>
			<td class="post">
                 <p><asp:Label id="RunningVersion" runat="server"></asp:Label></p>
                 <p><asp:Label id="LatestVersion" runat="server"></asp:Label></p>
                 <p><YAF:LocalizedLabel ID="Upgrade" runat="server" LocalizedTag="UPGRADE_VERSION" LocalizedPage="ADMIN_VERSION" Visible="false" /></p>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
