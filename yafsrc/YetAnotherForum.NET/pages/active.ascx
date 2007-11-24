<%@ Control Language="c#" CodeFile="active.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.active" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="command" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td>
			<YAF:Pager runat="server" ID="Pager" />
		</td>
		<td align="right">
			<%= GetText("since") %>
			<asp:DropDownList ID="Since" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" /></td>
	</tr>
</table>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" width="1%">
			&nbsp;</td>
		<td class="header1" align="left">
			<%= GetText("topics") %>
		</td>
		<td class="header1" align="left" width="20%">
			<%= GetText("topic_starter") %>
		</td>
		<td class="header1" align="center" width="7%">
			<%= GetText("replies") %>
		</td>
		<td class="header1" align="center" width="7%">
			<%= GetText("views") %>
		</td>
		<td class="header1" align="center" width="20%">
			<%= GetText("lastpost") %>
		</td>
	</tr>
	<asp:Repeater ID="TopicList" runat="server">
		<ItemTemplate>
			<%# PrintForumName((System.Data.DataRowView)Container.DataItem) %>
			<YAF:TopicLine runat="server" FindUnread="true" DataRow="<%# Container.DataItem %>" />
		</ItemTemplate>
	</asp:Repeater>
	<tr>
		<td class="footer1" align="right" width="100%" colspan="6">
			<asp:HyperLink ID="RssFeed" runat="server" />
			<%= GetText("last_24") %>
		</td>
	</tr>	
</table>
<table class="command" width="100%" cellspacing="0" cellpadding="0">
	<tr>
		<td>
			<YAF:Pager runat="server" LinkedPager="Pager" />
		</td>
	</tr>
</table>
<table width="100%" cellspacing="0" cellpadding="0">
	<tr>
		<td align="right" colspan="2">
			<%= GetText("Forum_Jump") %>
			<YAF:ForumJump runat="server" />
		</td>
	</tr>
	<tr>
		<td valign="top">
			<YAF:IconLegend runat="server" />
		</td>
	</tr>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
