<%@ Page language="c#" Codebehind="users.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.users" %>

<form runat="server">

<table cellspacing=1 cellpadding=0 width="100%" class=content>
<tr>
	<td class=header1 colspan=6>Users</td>
</tr>
<tr>
	<td class=header2>Name</td>
	<td class=header2>Rank</td>
	<td class=header2 align=center>Posts</td>
	<td class=header2 align=center>Approved</td>
	<td class=header2>Last Visit</td>
	<td class=header2>Commands</td>
</tr>

<asp:repeater id=UserList runat=server>
<ItemTemplate>
	<tr>
		<td class=post><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
		<td class=post><%# DataBinder.Eval(Container.DataItem,"RankName") %></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "NumPosts") %></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "Approved") %></td>
		<td class=post><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %></td>
		<td class=post>
			<asp:linkbutton runat=server commandname=edit commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Edit</asp:linkbutton>
			|
			<asp:linkbutton runat=server commandname=delete commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Delete</asp:linkbutton>
		</td>
	</tr>
</ItemTemplate>
</asp:repeater>

</table>

</form>
