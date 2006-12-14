<%@ Control language="c#" Codebehind="ranks.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ranks" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6>Ranks</td>
</tr>

<asp:repeater id=RankList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2>Name</td>
			<td class=header2>Is Start</td>
			<td class=header2>Is Ladder</td>
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
				<%# LadderInfo(Container.DataItem) %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval( "RankID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "RankID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=6><asp:linkbutton id=NewRank runat="server" text="New Rank" onclick="NewRank_Click" /></td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
