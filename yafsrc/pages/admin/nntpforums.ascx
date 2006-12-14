<%@ Control language="c#" Codebehind="nntpforums.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6>NNTP Forums</td>
</tr>

<asp:repeater id=RankList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2>Server</td>
			<td class=header2>Group</td>
			<td class=header2>Forum</td>
			<td class=header2>Active</td>
			<td class=header2>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<%# Eval( "Name") %>
			</td>
			<td class=post>
				<%# Eval( "GroupName") %>
			</td>
			<td class=post>
				<%# Eval( "ForumName") %>
			</td>
			<td class=post>
				<%# Eval( "Active") %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval( "NntpForumID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "NntpForumID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=5><asp:linkbutton id=NewForum runat="server" text="New Forum" onclick="NewForum_Click" /></td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
