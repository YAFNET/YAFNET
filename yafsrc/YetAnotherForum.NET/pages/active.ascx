<%@ Control Language="c#" CodeFile="active.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.active" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<div class="DivTopSeparator">
</div>

	<table class="command" cellspacing="0" cellpadding="0" width="100%" style="padding-bottom: 10px;">
		<tr>
			<td align="right">
				<YAF:LocalizedLabel ID="SinceLabel" runat="server" LocalizedTag="SINCE" />
				<asp:DropDownList ID="Since" runat="server" AutoPostBack="True" OnSelectedIndexChanged="Since_SelectedIndexChanged" />
			</td>
		</tr>
	</table>

<asp:UpdatePanel ID="ActiveUpdatePanel" runat="server">
	<ContentTemplate>
		<table class="command" cellspacing="0" cellpadding="0" width="100%">
			<tr>
				<td>
					<YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
				</td>
			</tr>
		</table>
		<table class="content" cellspacing="1" cellpadding="0" width="100%">
			<tr>
				<td class="header1" width="1%">
					&nbsp;
				</td>
				<td class="header1" align="left">
					<YAF:LocalizedLabel ID="TopicsLabel" runat="server" LocalizedTag="TOPICS" />
				</td>
				<td class="header1" align="left" width="20%">
					<YAF:LocalizedLabel ID="StarterLabel" runat="server" LocalizedTag="TOPIC_STARTER" />
				</td>
				<td class="header1" align="center" width="7%">
					<YAF:LocalizedLabel ID="RepliesLabel" runat="server" LocalizedTag="REPLIES" />
				</td>
				<td class="header1" align="center" width="7%">
					<YAF:LocalizedLabel ID="ViewsLabel" runat="server" LocalizedTag="VIEWS" />
				</td>
				<td class="header1" align="center" width="20%">
					<YAF:LocalizedLabel ID="LastPostLabel" runat="server" LocalizedTag="LASTPOST" />
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
					<YAF:LocalizedLabel ID="Last24Label" runat="server" LocalizedTag="LAST_24" />
				</td>
			</tr>
		</table>
		<table class="command" width="100%" cellspacing="0" cellpadding="0">
			<tr>
				<td>
					<YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
				</td>
			</tr>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>
<asp:PlaceHolder ID="ForumJumpHolder" runat="server">
	<div id="DivForumJump">
		<YAF:LocalizedLabel ID="ForumJumpLabel" runat="server" LocalizedTag="FORUM_JUMP" />
		&nbsp;<YAF:ForumJump ID="ForumJump1" runat="server" />
	</div>
</asp:PlaceHolder>
<div id="DivIconLegend">
	<YAF:IconLegend ID="IconLegend1" runat="server" />
</div>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
