<%@ Control language="c#" Codebehind="users.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.users" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:adminmenu runat="server">

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
	<td class=header2>&nbsp;</td>
</tr>

<asp:repeater id=UserList runat=server>
<ItemTemplate>
	<tr>
		<td class=post><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
		<td class=post><%# DataBinder.Eval(Container.DataItem,"RankName") %></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "NumPosts") %></td>
		<td class=post align=center><%# DataBinder.Eval(Container.DataItem, "Approved") %></td>
		<td class=post><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %></td>
		<td class=post align="center">
			<asp:linkbutton runat=server commandname=edit commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Edit</asp:linkbutton>
			|
			<asp:linkbutton onload="Delete_Load" runat=server commandname=delete commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Delete</asp:linkbutton>
		</td>
	</tr>
</ItemTemplate>
</asp:repeater>

</table>

</yaf:adminmenu>

<yaf:savescrollpos runat="server"/>
