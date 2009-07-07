<%@ Control Language="c#" AutoEventWireup="True" CodeFile="ForumList.ascx.cs" Inherits="YAF.Controls.ForumList"	EnableViewState="false" %>
<%@ Register TagPrefix="YAF" TagName="ForumLastPost" Src="ForumLastPost.ascx" %>	
<%@ Register TagPrefix="YAF" TagName="ForumModeratorList" Src="ForumModeratorList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="ForumSubForumList.ascx" %>
<asp:Repeater ID="ForumList1" runat="server" OnItemCreated="ForumList1_ItemCreated">
	<ItemTemplate>
		<tr class="forumRow post">
			<td>
			    <YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div><div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</div>
				<YAF:ForumSubForumList ID="SubForumList" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
			<td align="center" class="moderatorListCol" id="ModeratorListTD" runat="server">
				<YAF:ForumModeratorList ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>' />
			</td>
			<td align="center" class="topicCountCol">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center" class="postCountCol">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="lastPostCol" style="white-space:nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>' ID="lastPost" runat="server" />
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="forumRow_Alt post_alt">
			<td>
			    <YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div>
				<div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</div>
				<YAF:ForumSubForumList ID="ForumSubForumListAlt" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
			<td align="center" class="moderatorListCol" id="ModeratorListTD" runat="server">
				<YAF:ForumModeratorList ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>' />
			</td>
			<td align="center" class="topicCountCol">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center" class="postCountCol">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="lastPostCol" nowrap="nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>' ID="lastPost" runat="server" />
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
