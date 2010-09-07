<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.prune" Codebehind="prune.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				Prune Topics
			</td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
				<asp:Label ID="lblPruneInfo" runat="server"></asp:Label>
			</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong>Select forum to prune:</strong>
			</td>
			<td class="post" width="50%">
				<asp:DropDownList ID="forumlist" runat="server">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Enter minimum age in days:</strong><br />
				Topics with the last post older than this will be deleted.
			</td>
			<td class="post">
				<asp:TextBox ID="days" runat="server"></asp:TextBox>
			</td>
		</tr>
		<tr>
			<td class="postheader">
				<strong>Permanently remove from DB:</strong><br />
				All Topics marked with the Deleted flag will be permanently deleted.
			</td>
			<td class="post">
				<asp:CheckBox ID="permDeleteChkBox" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="commit" runat="server" class="pbutton" Text="Start Prune Task" OnLoad="PruneButton_Load">
				</asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
