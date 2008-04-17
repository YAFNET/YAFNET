<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditUsersGroups.ascx.cs"
	Inherits="YAF.Controls.EditUsersGroups" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<asp:Repeater ID="UserGroups" runat="server">
		<HeaderTemplate>
			<tr>
				<td class="header1" colspan="2">
					User Groups</td>
			</tr>
			<tr>
				<td class="header2">
					Member</td>
				<td class="header2">
					Group</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
					<asp:CheckBox runat="server" ID="GroupMember" Checked='<%# IsMember(DataBinder.Eval(Container.DataItem,"Member")) %>' /></td>
				<td class="post">
					<asp:Label ID="GroupID" Visible="false" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:Label>
					<b>
						<%# DataBinder.Eval(Container.DataItem, "Name") %>
					</b>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
		</td>
	</tr>
</table>
