<%@ Control language="c#" Codebehind="members.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.members" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
<tr runat="server" id="LetterRow"/>
</table>

<table class=command><tr><td class="navlinks"><yaf:pager runat="server" id="Pager"/></td></tr></table>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=4><%= GetText("title") %></td>
	</tr>
	<tr>
		<td class=header2><img runat="server" id="SortUserName" align="absmiddle"/> <asp:linkbutton runat=server id="UserName"/></td>
		<td class=header2><img runat="server" id="SortRank" align="absmiddle"/> <asp:linkbutton runat=server id="Rank"/></td>
		<td class=header2><img runat="server" id="SortJoined" align="absmiddle"/> <asp:linkbutton runat=server id="Joined"/></td>
		<td class=header2 align=center><img runat="server" id="SortPosts" align="absmiddle"/> <asp:linkbutton runat=server id="Posts"/></td>
	</tr>
	
	<asp:repeater id=MemberList runat=server>
		<ItemTemplate>
			<tr>
				<td class=post><a href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",Eval("UserID")) %>'><%# Server.HtmlEncode(Convert.ToString(Eval("Name"))) %></a></td>
				<td class=post><%# Eval("RankName") %></td>
				<td class=post><%# FormatDateLong((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Joined"]) %></td>
				<td class=post align=center><%# String.Format("{0:N0}",((System.Data.DataRowView)Container.DataItem)["NumPosts"]) %></td>
			</tr>
		</ItemTemplate>
	</asp:repeater>
</table>

<table class=command><tr><td class=navlinks><yaf:pager runat="server" linkedpager="Pager"/></td></tr></table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
