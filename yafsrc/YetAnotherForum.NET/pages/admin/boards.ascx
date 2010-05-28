<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.boards" Codebehind="boards.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table cellspacing="1" cellpadding="0" width="100%" class="content">
		<tr>
			<td class="header1" colspan="3">
				Boards
			</td>
		</tr>
		<tr>
			<td class="header2">
				ID
			</td>
			<td class="header2">
				Name
			</td>
			<td class="header2">
				&nbsp;
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server">
			<ItemTemplate>
				<tr id="BoardRow" class='<%# Convert.ToInt32(Eval( "BoardID")) != PageContext.PageBoardID ? "post" : "post_res" %>' runat="server">
					<td >
						<%# Eval( "BoardID") %>
					</td>
					<td>
						<%# Eval( "Name") %>
					</td>
					<td align="center">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "BoardID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton OnLoad="Delete_Load" runat="server" CommandName="delete" CommandArgument='<%# Eval( "BoardID") %>'>Delete</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1">
			<td colspan="3">
				<asp:LinkButton ID="New" runat="server" Text="New Board" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
