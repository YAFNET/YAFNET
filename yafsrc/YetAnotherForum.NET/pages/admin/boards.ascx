<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boards" Codebehind="boards.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table cellspacing="1" cellpadding="0" width="100%" class="content">
		<tr>
			<td class="header1" colspan="3">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BOARDS" />
			</td>
		</tr>
		<tr>
			<td class="header2">
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="ID" LocalizedPage="ADMIN_BOARDS" />
			</td>
			<td class="header2" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_BOARDS" />
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server">
			<ItemTemplate>
				<tr id="BoardRow" class='<%# Convert.ToInt32(Eval( "BoardID")) != PageContext.PageBoardID ? "post" : "post_res" %>' runat="server">
					<td >
						<%# Eval( "BoardID") %>
					</td>
					<td>
						<%# HtmlEncode(Eval( "Name")) %>
					</td>
					<td align="center">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "BoardID") %>'><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" /></asp:LinkButton>
						|
						<asp:LinkButton OnLoad="Delete_Load" runat="server" CommandName="delete" CommandArgument='<%# Eval( "BoardID") %>'><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" /></asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1" align="center">
			<td colspan="3">
				<asp:Button ID="New" runat="server" CssClass="pbutton" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
