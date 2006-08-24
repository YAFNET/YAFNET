<%@ Control language="c#" Codebehind="nntpservers.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.nntpservers" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6>NNTP Servers</td>
</tr>

<asp:repeater id=RankList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2>Name</td>
			<td class=header2>Address</td>
			<td class=header2>User Name</td>
			<td class=header2>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<%# Eval( "Name") %>
			</td>
			<td class=post>
				<%# Eval( "Address") %>
			</td>
			<td class=post>
				<%# Eval( "UserName") %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval( "NntpServerID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "NntpServerID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=5><asp:linkbutton id=NewServer runat="server" text="New Server" onclick="NewServer_Click" /></td>
</tr>
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
