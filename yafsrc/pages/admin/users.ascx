<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="users.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.users" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<TABLE class="content" cellSpacing="0" cellPadding="0" width="100%">
		<TR>
			<TD class="post" vAlign="top">
				<TABLE cellSpacing="0" cellPadding="0" width="100%">
					<TR>
						<TD class="header2" noWrap colSpan="4"><B>Filter</B></TD>
					</TR>
					<TR class="post">
						<TD>Group:</TD>
						<TD>Rank:</TD>
						<TD>Name Contains:</TD>
						<TD width="99%">&nbsp;</TD>
					</TR>
					<TR class="post">
						<TD>
							<asp:dropdownlist id="group" runat="server"></asp:dropdownlist></TD>
						<TD>
							<asp:dropdownlist id="rank" runat="server"></asp:dropdownlist></TD>
						<TD>
							<asp:textbox id="name" runat="server"></asp:textbox></TD>
						<TD align="right">
							<asp:button id="search" runat="server" text="Search"></asp:button></TD>
					</TR>
				</TABLE>
			</TD>
		</TR>
	</TABLE>
	<BR>
	<TABLE class="content" cellSpacing="1" cellPadding="0" width="100%">
		<TR>
			<TD class="header1" colSpan="6">Users</TD>
		</TR>
		<TR>
			<TD class="header2">Name</TD>
			<TD class="header2">Rank</TD>
			<TD class="header2" align="center">Posts</TD>
			<TD class="header2" align="center">Approved</TD>
			<TD class="header2">Last Visit</TD>
			<TD class="header2">&nbsp;</TD>
		</TR>
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
		<TR>
			<TD class="footer1" colSpan="6">
				<asp:linkbutton id="NewUser" runat="server">New User</asp:linkbutton></TD>
		</TR> <!--- Added BAI 07.01.2003 --></TABLE>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat="server" />
