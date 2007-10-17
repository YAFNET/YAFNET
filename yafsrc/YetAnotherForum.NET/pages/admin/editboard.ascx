<%@ Control Language="c#" CodeFile="editboard.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editboard" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				Edit Board</td>
		</tr>
		<tr>
			<td width="50%" class="postheader">
				<b>Name:</b><br />
				The name of the board.</td>
			<td width="50%" class="post">
				<asp:TextBox ID="Name" runat="server" Style="width: 100%"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Allow Threaded:</b><br />
				Allow threaded view for posts.</td>
			<td class="post">
				<asp:CheckBox runat="server" ID="AllowThreaded" /></td>
		</tr>
		<asp:PlaceHolder runat="server" ID="AdminInfo">
			<tr>
				<td class="postheader">
					<b>User Name:</b><br />
					This will be the administrator for the board.</td>
				<td class="post">
					<asp:TextBox runat="server" ID="UserName" /></td>
			</tr>
			<tr>
				<td class="postheader">
					<b>User Email:</b><br />
					Email address for administrator.</td>
				<td class="post">
					<asp:TextBox runat="server" ID="UserEmail" /></td>
			</tr>
			<tr>
				<td class="postheader">
					<b>Password:</b><br />
					Enter password for administrator here.</td>
				<td class="post">
					<asp:TextBox runat="server" ID="UserPass1" TextMode="password" /></td>
			</tr>
			<tr>
				<td class="postheader">
					<b>Verify Password:</b><br />
					Verify the password.</td>
				<td class="post">
					<asp:TextBox runat="server" ID="UserPass2" TextMode="password" /></td>
			</tr>
		</asp:PlaceHolder>
		<tr>
			<td class="postfooter" align="middle" colspan="2">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
				<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
