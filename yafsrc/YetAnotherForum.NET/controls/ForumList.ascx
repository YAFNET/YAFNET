<%@ Control Language="c#" AutoEventWireup="True" CodeFile="ForumList.ascx.cs" Inherits="YAF.Controls.ForumList"	EnableViewState="false" %>
<%@ Register TagPrefix="YAF" TagName="ForumLastPost" Src="ForumLastPost.ascx" %>	
<%@ Register TagPrefix="YAF" TagName="ForumModeratorList" Src="ForumModeratorList.ascx" %>
<%@ Register TagPrefix="YAF" TagName="ForumSubForumList" Src="ForumSubForumList.ascx" %>
<asp:Repeater ID="forumList" runat="server">
	<ItemTemplate>
		<tr class="post">
			<td>
				<%# GetForumIcon(Container.DataItem) %>
			</td>
			<td>
				<span class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</span><span class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</span>
				<br />
				<span class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</span>
				<br />
				<YAF:ForumSubForumList ID="SubForumList" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
			<td align="center" class="smallfont subforumlink">
				<YAF:ForumModeratorList ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>' />
			</td>
			<td align="center">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="smallfont" nowrap="nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>' ID="lastPost" runat="server" />
			</td>
		</tr>
	</ItemTemplate>
	<AlternatingItemTemplate>
		<tr class="post_alt">
			<td>
				<%# GetForumIcon(Container.DataItem) %>
			</td>
			<td>
				<span class="forumheading">
					<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
				</span><span class="forumviewing">
					<%# GetViewing(Container.DataItem) %>
				</span>
				<br />
				<span class="subforumheading">
					<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
				</span>
				<br />
				<YAF:ForumSubForumList ID="ForumSubForumListAlt" runat="server" DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' Visible='<%# HasSubforums( (System.Data.DataRow)Container.DataItem ) %>' />
			</td>
			<td align="center" class="smallfont subforumlink">
				<YAF:ForumModeratorList ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>' />
			</td>
			<td align="center">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="smallfont" nowrap="nowrap">
				<YAF:ForumLastPost DataRow="<%# Container.DataItem %>" Visible='<%# (((System.Data.DataRow)Container.DataItem)["RemoteURL"] == DBNull.Value) %>' ID="lastPost" runat="server" />
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
