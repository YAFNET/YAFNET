<%@ Control Language="c#" CodeFile="prune.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.prune" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				Prune Topics</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Select forum to prune:</b></td>
			<td class="post" width="50%">
				<asp:DropDownList ID="forumlist" runat="server">
				</asp:DropDownList>
		</tr>
		<tr>
			<td class="postheader">
				<b>Enter minimum age in days:</b><br />
				Topics with the last post older than this will be deleted.</td>
			<td class="post">
				<asp:TextBox ID="days" runat="server"></asp:TextBox>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="commit" runat="server" Text="Prune" OnLoad="PruneButton_Load"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
