<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersKill" Codebehind="EditUsersKill.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr runat="server" id="trHeader">
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_KILL_USER" LocalizedPage="ADMIN_EDITUSER" />
		</td>
	</tr>
    <tr>
			<td class="header2" height="30" colspan="2">
			</td>
		</tr>
	<tr>
		<td class="postheader">
			<strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="IP_ADRESSES" LocalizedPage="ADMIN_EDITUSER" /></strong>
		</td>
		<td class="post">
			<asp:Literal ID="IpAddresses" runat="server"></asp:Literal>
		</td>
	</tr>	
	<tr>
		<td class="postheader">
			<strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BAN_IP_OFUSER" LocalizedPage="ADMIN_EDITUSER" /></strong>
		</td>
		<td class="post">
			<asp:CheckBox ID="BanIps" runat="server" />
		</td>
	</tr>	
	<tr>
		<td class="postheader">
			<strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_POSTS_USER" LocalizedPage="ADMIN_EDITUSER" /></strong>
		</td>
		<td class="post">
			<strong><asp:Literal ID="PostCount" runat="server"></asp:Literal></strong> (<asp:HyperLink ID="ViewPostsLink" runat="server" Target="_blank"><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEW_ALL" LocalizedPage="ADMIN_EDITUSER" /></asp:HyperLink>)
		</td>
	</tr>	
	<tr>
		<td colspan="2" class="footer1" align="center">
			<asp:Button runat="server" ID="Kill" Text="Kill User" CssClass="pbutton" OnClientClick="return confirm('Are you sure you want to delete all posts by and optionally ban all IP addreess for this user?');" OnClick="Kill_OnClick" />
		</td>
	</tr>
</table>