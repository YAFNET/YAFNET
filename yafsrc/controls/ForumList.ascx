<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ForumList.ascx.cs" Inherits="yaf.controls.ForumList" EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<asp:Repeater id="forumList" runat="server">
<ItemTemplate>
	<tr class=post>
		<td><%# GetForumIcon(Container.DataItem) %></td>
		<td>
			<%# GetForumLink((System.Data.DataRow)Container.DataItem) %>
			<%# GetViewing(Container.DataItem) %><br>
			<span class="smallfont"><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></span>
			<br>
			<asp:repeater visible='<%# GetModerated(Container.DataItem) %>' id="ModeratorList" runat="server" onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
				<HeaderTemplate><span class="smallfont"><%# ForumPage.GetText("moderators") %>: </HeaderTemplate>
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem, "[\"GroupName\"]") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
				<FooterTemplate></span></FooterTemplate>
			</asp:repeater>
		</td>
		<td align="center"><%# Topics(Container.DataItem) %></td>
		<td align="center"><%# Posts(Container.DataItem) %></td>
		<td align="center" class="smallfont" nowrap="nowrap"><%# yaf.Utils.BadWordReplace(FormatLastPost((System.Data.DataRow)Container.DataItem)) %></td>
	</tr>
</ItemTemplate>
</asp:Repeater>
