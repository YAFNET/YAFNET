<%@ Control Language="C#" AutoEventWireup="true" CodeFile="medals.ascx.cs" Inherits="YAF.Pages.Admin.medals" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="5">
				Medals</td>
		</tr>
		<asp:Repeater ID="MedalList" OnItemCommand="MedalList_ItemCommand" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2">
						Order</td>
					<td class="header2">
						Name</td>
					<td class="header2">
						Category</td>
					<td class="header2">
						Image</td>
					<td class="header2">
						Command</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# Eval( "SortOrder") %>
						<td class="post">
							<%# Eval( "Name") %>
						</td>
					<td class="post">
						<%# Eval( "Category") %>
					</td>
					<td class="post">
						<%# RenderImages(Container.DataItem) %>
					</td>
					<td class="post">
						<asp:LinkButton ID="EditMedal" runat="server" CommandName="edit" CommandArgument='<%# Eval("MedalID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton ID="MoveUp" runat="server" CommandName="moveup" CommandArgument='<%# Eval("MedalID") %>'
							Text="▲" />
						<asp:LinkButton ID="MoveDown" runat="server" CommandName="movedown" CommandArgument='<%# Eval("MedalID") %>'
							Text="▼" />
						|
						<asp:LinkButton ID="DeleteMedal" runat="server" OnLoad="Delete_Load" CommandName="delete"
							CommandArgument='<%# Eval("MedalID") %>'>Delete</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="6">
				<asp:LinkButton ID="NewMedal" runat="server" Text="New Medal" OnClick="NewMedal_Click" /></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
