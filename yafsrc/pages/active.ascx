<%@ Control language="c#" Codebehind="active.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.active" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<table class="command" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td class="navlinks"><YAF:pager runat="server" id="Pager"/></td>
		<td align="right"><%= GetText("since") %> <asp:DropDownList id="Since" runat="server" AutoPostBack="True" onselectedindexchanged="Since_SelectedIndexChanged" /></td>
	</tr>
</table>

<table class="content" cellspacing="1" cellpadding="0" width="100%">
<tr>
	<td class="header1" width="1%">&nbsp;</td>
	<td class="header1" align="left"><%= GetText("topics") %></td>
	<td class="header1" align="left" width="20%"><%= GetText("topic_starter") %></td>
	<td class="header1" align="middle" width="7%"><%= GetText("replies") %></td>
	<td class="header1" align="middle" width="7%"><%= GetText("views") %></td>
	<td class="header1" align="middle" width="20%"><%= GetText("lastpost") %></td>
</tr>
<tr>
	<td class="header1" align="right" width="100%" colspan="6"><asp:hyperlink id="RssFeed" runat="server" /> <%= GetText("last_24") %></td>
</tr>
<asp:repeater id="TopicList" runat="server">
<ItemTemplate>
	<%# PrintForumName((System.Data.DataRowView)Container.DataItem) %>
	<YAF:TopicLine runat="server" findunread="true" DataRow="<%# Container.DataItem %>"/>
</ItemTemplate>
</asp:repeater>
<tr>
	<td align="middle" colspan="6" class="footer1">&nbsp;</td>
</tr>
</table>

<table class="command" width="100%" cellspacing="0" cellpadding="0">
<tr>
	<td class="navlinks"><YAF:pager runat="server" linkedpager="Pager"/></td>
</tr>
</table>

<table width="100%" cellspacing="0" cellpadding="0">
<tr>
	<td align="right" colspan="2">
		<%= GetText("Forum_Jump") %> <YAF:forumjump runat="server"/>
	</td>
</tr>
<tr>
	<td valign="top"><YAF:IconLegend runat="server"/></td>
</tr>
</table>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
