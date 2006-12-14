<%@ Control language="c#" Codebehind="groups.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.groups" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="5">Groups</td>
</tr>

<asp:repeater id=GroupList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2>Name</td>
			<td class=header2>Is Admin</td>
			<td class=header2>Is Start</td>
			<td class=header2>Is Forum Moderator</td>
			<td class=header2>Command</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<%# Eval( "Name") %>
			</td>
			<td class=post>
				<%# BitSet(Eval( "Flags"),1) %>
			</td>
			<td class=post>
				<%# BitSet(Eval( "Flags"),4) %>
			</td>
			<td class=post>
				<%# BitSet(Eval( "Flags"),8) %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval( "GroupID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "GroupID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=6><asp:linkbutton id=NewGroup runat="server" onclick="NewGroup_Click">New Group</asp:linkbutton></td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
