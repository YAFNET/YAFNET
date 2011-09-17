<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Controls.ForumList"
	EnableViewState="false" Codebehind="ForumList.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="ForumLastPost" Src="ForumLastPost.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumModeratorList" Src="ForumModeratorList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="ForumSubForumList.ascx" %>
<asp:Repeater ID="ForumList1" runat="server" OnItemCreated="ForumList1_ItemCreated">
	<ItemTemplate>
		<tr class="forumRow post">
			<td class="forumIconCol">
				<YAF:ThemeImage  ID="ThemeForumIcon" Visible="false" runat="server" />	
				<img id="ForumImage1" class="" src="" alt="image" visible="false" runat="server" style="border-width:0px;" />	
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div>
				<div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# Page.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]")) %>
				</div>
                <span id="ModListMob_Span" class="description" Visible="false" runat="server"><YAF:ForumModeratorList ID="ForumModeratorListMob" Visible="false" runat="server"  /></span>
				<YAF:ForumSubForumList ID="SubForumList" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
					Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>' />
			</td>
			<td class="moderatorListCol" id="ModeratorListTD" Visible="false" runat="server">
				<YAF:ForumModeratorList ID="ModeratorList" Visible="false" runat="server" />
			</td>
			<td class="topicCountCol">
				<%# Topics(Container.DataItem) %>
			</td>
			<td class="postCountCol">
				<%# Posts(Container.DataItem) %>
			</td>
			<td class="lastPostCol" style="white-space: nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>'
					ID="lastPost" runat="server" />
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="forumRow_Alt post_alt">
			<td>
				<YAF:ThemeImage ID="ThemeForumIcon" runat="server" />
				<img id="ForumImage1" src="" alt="" visible="false" runat="server" style="border-width:0px;" />	
			</td>
			<td class="forumLinkCol">
				<div class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</div>
				<div class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</div>
				<div class="subforumheading">
					<%# Page.HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]"))%>
				</div> 
				<YAF:ForumSubForumList ID="ForumSubForumListAlt" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>'
					Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>' />
                <span id="ModListMob_Span" Visible="false" class="description"  runat="server"><YAF:ForumModeratorList ID="ForumModeratorListMob" runat="server" Visible="false" /></span>
			</td>
			<td class="moderatorListCol" id="ModeratorListTD" Visible="false" runat="server">
				<YAF:ForumModeratorList ID="ModeratorList" Visible="false" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>' />
			</td>
			<td class="topicCountCol">
				<%#  Topics(Container.DataItem) %>
			</td>
			<td class="postCountCol">
				<%# Posts(Container.DataItem) %>
			</td>
			<td class="lastPostCol" nowrap="nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>'
					ID="lastPost" runat="server" />
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
