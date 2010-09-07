<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editnntpserver" Codebehind="editnntpserver.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="11">
				Edit NNTP Server</td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Name:</strong><br />
				Name of this server.</td>
			<td class="post" colspan="7">
				<asp:TextBox Style="width: 300px" ID="Name" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Address:</strong><br />
				The host name of the server.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="Address" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Port:</strong><br />
				The port number to connect to.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="Port" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>User Name:</strong><br />
				The user name used to log on to the nntp server.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="UserName" runat="server" Enabled="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Password:</strong><br />
				The password used to log on to the nntp server.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="UserPass" runat="server" Enabled="true" /></td>
		</tr>
		<tr>
			<td class="postfooter" align="middle" colspan="11">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button>&nbsp;
				<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
