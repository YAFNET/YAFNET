<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ForumCategoryList.ascx.cs"
	Inherits="YAF.Controls.ForumCategoryList" %>
<%@ Register TagPrefix="YAF" TagName="ForumList" Src="ForumList.ascx" %>
<asp:UpdatePanel ID="UpdatePanelCategory" runat="server" UpdateMode="Conditional">
	<ContentTemplate>
	<table class="content" width="100%">
		<asp:Repeater ID="CategoryList" runat="server">
			<HeaderTemplate>
				
					<tr class="forumRowTitle">
						<td colspan="2" align="left" class="header1">
							<YAF:LocalizedLabel ID="ForumHeaderLabel" runat="server" LocalizedTag="FORUM" />
						</td>
						<td id="Td1" class="header1" align="center" width="15%" runat="server" visible="<%# PageContext.BoardSettings.ShowModeratorList %>">
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
				<tr class="forumRowCat header2">
					<td colspan="<%# (PageContext.BoardSettings.ShowModeratorList ? "6" : "5" ) %>">
						<YAF:CollapsibleImage ID="CollapsibleImage" runat="server" BorderWidth="0" ImageAlign="Bottom"
							PanelID='<%# "categoryPanel" + DataBinder.Eval(Container.DataItem, "CategoryID").ToString() %>'
							AttachedControlID="forumList" />
						&nbsp;&nbsp; <a href='<%# YAF.Classes.Utils.YafBuildLink.GetLink(ForumPages.forum,"c={0}",DataBinder.Eval(Container.DataItem, "CategoryID")) %>'>
							
							<asp:Image ID="uxCategoryImage" CssClass="category_image" AlternateText=" " ImageUrl='<%# YafForumInfo.ForumRoot + YafBoardFolders.Current.Categories + "/" + DataBinder.Eval(Container.DataItem, "CategoryImage") %>' Visible='<%# !String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "CategoryImage" ).ToString()) %>' runat="server" />
							
							<%# DataBinder.Eval(Container.DataItem, "Name") %>
						</a>
					</td>
				</tr>
				<YAF:ForumList runat="server" Visible="true" ID="forumList" DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>' />
			</ItemTemplate>
			<FooterTemplate>
				<tr class="forumRowFoot footer1">
					<td colspan="<%# (PageContext.BoardSettings.ShowModeratorList ? "6" : "5" ) %>" align="right">
						<asp:LinkButton runat="server" OnClick="MarkAll_Click" ID="MarkAll" Text='<%# PageContext.Localization.GetText("MARKALL") %>' />
						<span id="RSSLinkSpacer" runat="server" visible='<%# PageContext.BoardSettings.ShowRSSLink %>'>
							|</span>
						<asp:HyperLink ID="RssFeed" runat="server" NavigateUrl='<%# YafBuildLink.GetLinkNotEscaped( ForumPages.rsstopic, "pg=forum" ) %>'
							Visible='<%# PageContext.BoardSettings.ShowRSSLink %>'>
							<YAF:LocalizedLabel ID="RSSFeedLabel" runat="server" LocalizedTag="RSSFEED" />							
					</asp:HyperLink>
					</td>
				</tr>
				
			</FooterTemplate>
		</asp:Repeater>
		</table>
	</ContentTemplate>
</asp:UpdatePanel>
