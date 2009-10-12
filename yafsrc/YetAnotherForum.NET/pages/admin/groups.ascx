<%@ Control Language="c#" CodeFile="groups.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="AdminMenu">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<asp:Repeater ID="RoleListNet" runat="server" OnItemCommand="RoleListNet_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="6">
						Provider Roles
					</td>
				</tr>
				<tr>
					<td colspan="6" class="post">
						Note: Deleting a role here removes it completely from the provider. "Add to YAF"
						to make this role accessible in the forum.
					</td>
				</tr>
				<tr>
					<td class="header2" colspan="5">
						Name
					</td>
					<td class="header2">
						&nbsp;
					</td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post" colspan="5">
						<%# Container.DataItem %>
						(Unlinked)
					</td>
					<td class="post">
						<asp:LinkButton ID="LinkButtonAdd" runat="server" CommandName="add" CommandArgument='<%# Container.DataItem %>'>Add to YAF</asp:LinkButton>
						|
						<asp:LinkButton ID="LinkButtonDelete" runat="server" OnLoad="Delete_Load" CommandName="delete"
							CommandArgument='<%# Container.DataItem %>'>Delete Role</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<asp:Repeater ID="RoleListYaf" runat="server" OnItemCommand="RoleListYaf_ItemCommand">
			<HeaderTemplate>
				<tr>
					<td class="header1" colspan="7">
						YetAnotherForum Roles
					</td>
				</tr>
				<tr>
					<td colspan="7" class="post">
						Note: Deleting one of these "linked" roles outside of YAF will cause user data loss.
						If you want to delete the role, first "Delete from YAF" then the role can be managed
						outside of YAF.
					</td>
				</tr>
				<tr>
					<td class="header2">
						Name
					</td>
					<td class="header2">
						Is Guest
					</td>
					<td class="header2">
						Is Start
					</td>
					<td class="header2">
						Is Moderator
					</td>
					<td class="header2">
						Is Admin
					</td>
					<td class="header2">
						PMs
					</td>
					<td class="header2">
						&nbsp;
					</td>				
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# Eval( "Name" ) %>
						(<%# GetLinkedStatus( (System.Data.DataRowView) Container.DataItem )%>)
					</td>
					<td class="post">
						<%# General.BinaryAnd(Eval( "Flags" ),2) %>
					</td>
					<td class="post">
						<%# General.BinaryAnd(Eval( "Flags" ),4) %>
					</td>
					<td class="post">
						<%# General.BinaryAnd(Eval( "Flags" ),8) %>
					</td>
					<td class="post">
						<%# General.BinaryAnd(Eval( "Flags" ),1) %>
					</td>
					<td class="post">
						<%# ((Convert.ToInt32(Eval("Flags")) & 1) == 1 ? int.MaxValue.ToString() : Eval("PMLimit").ToString())%>
					</td>
					<td class="post">
						<asp:LinkButton ID="LinkButtonEdit" runat="server" Visible='<%#(General.BinaryAnd(Eval( "Flags" ),2) == true ? false : true)%>'
							CommandName="edit" CommandArgument='<%# Eval( "GroupID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton ID="LinkButtonDelete" runat="server" OnLoad="Delete_Load" Visible='<%#(General.BinaryAnd(Eval( "Flags" ),2) == true ? false : true)%>'
							CommandName="delete" CommandArgument='<%# Eval( "GroupID") %>'>Delete from YAF</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="7">
				<asp:LinkButton ID="NewGroup" runat="server" OnClick="NewGroup_Click">New Role</asp:LinkButton>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
