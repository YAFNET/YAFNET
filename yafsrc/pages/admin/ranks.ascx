<%@ Control language="c#" Codebehind="ranks.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.ranks" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:adminmenu runat="server">

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
				<%# DataBinder.Eval(Container.DataItem, "Name") %>
			</td>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "IsStart") %>
			</td>
			<td class=post>
				<%# LadderInfo(DataBinder.Eval(Container.DataItem, "IsLadder"),DataBinder.Eval(Container.DataItem, "MinPosts")) %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# DataBinder.Eval(Container.DataItem, "RankID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "RankID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=6><asp:linkbutton id=NewRank runat="server" text="New Rank"/></td>
</tr>
</table>

</yaf:adminmenu>

<yaf:savescrollpos runat="server"/>
