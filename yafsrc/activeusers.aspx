<%@ Page language="c#" Codebehind="activeusers.aspx.cs" AutoEventWireup="false" Inherits="yaf.activeusers" %>

<form runat=server>

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <a href="activeusers.aspx">Active Users</a>
</p>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6>Active Users</td>
</tr>
<tr>
	<td class=header2>User Name</td>
	<td class=header2>Logged In</td>
	<td class=header2>Last Active</td>
	<td class=header2>Active</td>
	<td class=header2>Browser</td>
	<td class=header2>Platform</td>
</tr>

<asp:repeater id=UserList runat=server>
<ItemTemplate>
<tr>
	<td class=post><%# DataBinder.Eval(Container.DataItem,"Name") %></td>
	<td class=post><%# FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["Login"]) %></td>
	<td class=post><%# FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastActive"]) %></td>
	<td class=post><%# String.Format(CustomCulture,"{0:N0} minutes",((System.Data.DataRowView)Container.DataItem)["Active"]) %></td>
	<td class=post><%# DataBinder.Eval(Container.DataItem,"Browser") %></td>
	<td class=post><%# DataBinder.Eval(Container.DataItem,"Platform") %></td>
</tr>
</ItemTemplate>
</asp:repeater>

</table>

</form>
