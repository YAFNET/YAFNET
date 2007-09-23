<%@ Control Language="c#" AutoEventWireup="True" CodeFile="ForumList.ascx.cs" Inherits="YAF.Controls.ForumList"
	EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
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
				<asp:Repeater ID="SubforumList" Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>'
					DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' runat="server">
					<HeaderTemplate>
						<span class="smallfont subforumlink"><b>
							<%# PageContext.Localization.GetText("SUBFORUMS")%>
						</b>:
					</HeaderTemplate>
					<ItemTemplate>
						<%# GetSubForumIcon(Container.DataItem) %>
						<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
					<FooterTemplate>
						</span></FooterTemplate>
				</asp:Repeater>
			</td>
			<td align="center" class="smallfont subforumlink">
				<asp:Repeater ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
					<ItemTemplate>
						<%# GetModeratorLink((System.Data.DataRow)Container.DataItem)%>
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
					<FooterTemplate>
						<%# GetModeratorsFooter((Repeater)Container.Parent) %>
					</FooterTemplate>
				</asp:Repeater>
			</td>
			<td align="center">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="smallfont" nowrap="nowrap">
				<%# YAF.Classes.Utils.General.BadWordReplace(FormatLastPost((System.Data.DataRow)Container.DataItem)) %>
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
				<asp:Repeater ID="SubforumList" Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>'
					DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' runat="server">
					<HeaderTemplate>
						<span class="smallfont subforumlink"><b>
							<%# PageContext.Localization.GetText("SUBFORUMS")%>
						</b>:
					</HeaderTemplate>
					<ItemTemplate>
						<%# GetSubForumIcon(Container.DataItem) %>
						<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
					<FooterTemplate>
						</span></FooterTemplate>
				</asp:Repeater>
			</td>
			<td align="center" class="smallfont subforumlink">
				<asp:Repeater ID="ModeratorList" runat="server" DataSource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
					<ItemTemplate>
						<%# GetModeratorLink((System.Data.DataRow)Container.DataItem)%>
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
					<FooterTemplate>
						<%# GetModeratorsFooter((Repeater)Container.Parent)%>
					</FooterTemplate>
				</asp:Repeater>
			</td>
			<td align="center">
				<%# Topics(Container.DataItem) %>
			</td>
			<td align="center">
				<%# Posts(Container.DataItem) %>
			</td>
			<td align="center" class="smallfont" nowrap="nowrap">
				<%# YAF.Classes.Utils.General.BadWordReplace(FormatLastPost((System.Data.DataRow)Container.DataItem)) %>
			</td>
		</tr>
	</AlternatingItemTemplate>
</asp:Repeater>
