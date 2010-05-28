<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pm" Codebehind="pm.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				Private messages</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				Number of private messages:</td>
			<td class="post" width="50%">
				<asp:Label runat="server" ID="Count" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				Delete read messages older than:</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Days1" />
				days</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				Delete unread messages older than:</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Days2" />
				days</td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="commit" runat="server" Text="Delete" OnLoad="DeleteButton_Load" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
