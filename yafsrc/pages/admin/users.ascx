<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="users.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.users" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellSpacing="0" cellPadding="0" width="100%">
		<tr>
			<td class="post" vAlign="top">
				<table cellSpacing="0" cellPadding="0" width="100%">
					<tr>
						<td class="header2" noWrap colSpan="4"><B>Filter</B></td>
					</tr>
					<tr class="post">
						<td>Group:</td>
						<td>Rank:</td>
						<td>Name Contains:</td>
						<td width="99%">&nbsp;</td>
					</tr>
					<tr class="post">
						<td>
							<asp:dropdownlist id="group" runat="server"></asp:dropdownlist></td>
						<td>
							<asp:dropdownlist id="rank" runat="server"></asp:dropdownlist></td>
						<td>
							<asp:textbox id="name" runat="server"></asp:textbox></td>
						<td align="right">
							<asp:button id="search" runat="server" text="Search"></asp:button></td>
					</tr>
				</table>
			</td>
		</tr>
	</table>
	<BR>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%">
		<tr>
			<td class="header1" colSpan="6">Users</td>
		</tr>
		<tr>
			<td class="header2">Name</td>
			<td class="header2">Rank</td>
			<td class="header2" align="center">Posts</td>
			<td class="header2" align="center">Approved</td>
			<td class="header2">Last Visit</td>
			<td class="header2">&nbsp;</td>
		</tr>
		<asp:repeater id="UserList" runat="server">
			<ItemTemplate>
				<tr>
					<td class="post"><%# Eval( "Name") %></td>
					<td class="post"><%# Eval("RankName") %></td>
					<td class="post" align="center"><%# Eval( "NumPosts") %></td>
					<td class="post" align="center"><%# BitSet(Eval( "Flags"),2) %></td>
					<td class="post"><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["LastVisit"]) %></td>
					<td class="post" align="center">
						<asp:linkbutton runat=server commandname=edit commandargument='<%# Eval( "UserID") %>'>Edit</asp:linkbutton>
						|
						<asp:linkbutton onload="Delete_Load" runat=server commandname=delete commandargument='<%# Eval( "UserID") %>'>Delete</asp:linkbutton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:repeater><!--- Added BAI 07.01.2003 -->
		<tr>
			<td class="footer1" colSpan="6">
				<asp:linkbutton id="NewUser" runat="server">New User</asp:linkbutton></td>
		</tr> <!--- Added BAI 07.01.2003 --></table>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat="server" />
