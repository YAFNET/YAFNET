<%@ Control language="c#" Codebehind="users.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.users" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class="content" width="100%" cellspacing="0" cellpadding="0"><tr><td class="post" valign="top">
	<table width="100%" cellspacing="0" cellpadding="0">
		<tr><td nowrap colspan="4" class="header2"><b>Filter</b></td></tr>
		<tr class="post">
			<td>Group:</td>
			<td>Rank:</td>
			<td>Name Contains:</td>
			<td width="99%">&nbsp;</td>
		</tr>
		<tr class="post">
			<td><asp:dropdownlist runat="server" id="group"/></td>
			<td><asp:dropdownlist runat="server" id="rank"/></td>
			<td><asp:textbox runat="server" id="name"/></td>
			<td align="right"><asp:button runat="server" id="search" text="Search"/></td>
		</tr>
	</table>
</td></tr></table>

<br/>

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
		<td class=post align=center><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),2) %></td>
		<td class=post><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %></td>
		<td class=post align="center">
			<asp:linkbutton runat=server commandname=edit commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Edit</asp:linkbutton>
			|
			<asp:linkbutton onload="Delete_Load" runat=server commandname=delete commandargument='<%# DataBinder.Eval(Container.DataItem, "UserID") %>'>Delete</asp:linkbutton>
		</td>
	</tr>
</ItemTemplate>
</asp:repeater>

    <!--- Added BAI 07.01.2003 -->
    <TR>
      <TD class="footer1" colSpan="6"><asp:linkbutton id="NewUser" runat="server">New User</asp:linkbutton></TD>
    </TR>
    <!--- Added BAI 07.01.2003 -->
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
