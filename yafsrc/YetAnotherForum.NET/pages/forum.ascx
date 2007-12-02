<%@ Control Language="c#" CodeFile="forum.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.forum" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="../controls/ForumList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumWelcome" Src="../controls/ForumWelcome.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumIconLegend" Src="../controls/ForumIconLegend.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumStatistics" Src="../controls/ForumStatistics.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumActiveDiscussion" Src="../controls/ForumActiveDiscussion.ascx" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:ForumWelcome runat="server" ID="Welcome" />
<br />
<asp:Repeater ID="CategoryList" runat="server" OnItemCommand="categoryList_ItemCommand"
	OnItemDataBound="CategoryList_ItemDataBound">
	<HeaderTemplate>
		<table class="content" cellspacing="1" cellpadding="0" width="100%">
			<tr>
				<td class="header1" width="1%">
					&nbsp;</td>
				<td class="header1" align="left">
					<YAF:LocalizedLabel ID="ForumHeaderLabel" runat="server" LocalizedTag="FORUM" />
				</td>
				<td class="header1" align="center" width="15%">
					<YAF:LocalizedLabel ID="ModeratorsHeaderLabel" runat="server" LocalizedTag="MODERATORS" />
				</td>
				<td class="header1" align="center" width="4%">
					<YAF:LocalizedLabel ID="TopicsHeaderLabel" runat="server" LocalizedTag="TOPICS" />
				</td>
				<td class="header1" align="center" width="4%">
					<YAF:LocalizedLabel ID="PostsHeaderLabel" runat="server" LocalizedTag="POSTS" />
				</td>
				<td class="header1" align="center" width="25%">
					<YAF:LocalizedLabel ID="LastPostHeaderLabel" runat="server" LocalizedTag="LASTPOST" />
				</td>
			</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class="header2" colspan="6">
				<asp:ImageButton runat="server" ID="expandCategory" BorderWidth="0" ImageAlign="Bottom"
					CommandName="panel" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "CategoryID") %>'>
				</asp:ImageButton>
				&nbsp;&nbsp; <a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(YAF.Classes.Utils.ForumPages.forum,"c={0}",DataBinder.Eval(Container.DataItem, "CategoryID")) %>'>
					<%# DataBinder.Eval(Container.DataItem, "Name") %>
				</a>
			</td>
		</tr>
		<YAF:ForumList runat="server" Visible="true" ID="forumList" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>' />
	</ItemTemplate>
	<FooterTemplate>
		<tr><td colspan="6" align="right" class="footer1">
			<asp:LinkButton runat="server" OnClick="MarkAll_Click" ID="MarkAll" Text='<%# GetText("MARKALL") %>' />
			<span id="RSSLinkSpacer" runat="server" visible='<%# PageContext.BoardSettings.ShowRSSLink %>'>|</span>
      <asp:HyperLink ID="RssFeed" runat="server" NavigateUrl='<%# YafBuildLink.GetLink( ForumPages.rsstopic, "pg=forum" ) %>' visible='<%# PageContext.BoardSettings.ShowRSSLink %>'><%# GetText( "RSSFEED" ) %></asp:HyperLink>
		</td></tr>
		</table>
	</FooterTemplate>
</asp:Repeater>
<br />

<YAF:ForumActiveDiscussion ID="ActiveDiscussions" runat="server" OnNeedDataBind="OnNeedDataBind" />
<br />

<YAF:ForumStatistics ID="ForumStats" runat="Server" OnNeedDataBind="OnNeedDataBind" />

<YAF:ForumIconLegend ID="IconLegend" runat="server" />

<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>