<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ForumList.ascx.cs" Inherits="yaf.controls.ForumList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>

<asp:Repeater id=forumList runat="server" onitemcommand='ForumList_ItemCommand'>
<ItemTemplate>
	<tr class=post>
		<td><%# GetForumIcon(Container.DataItem) %></td>
		<td>
			<asp:linkbutton runat="server" commandname="forum" commandargument='<%# DataBinder.Eval(Container.DataItem, "[\"ForumID\"]") %>'><%# DataBinder.Eval(Container.DataItem, "[\"Forum\"]") %></asp:linkbutton><%# GetViewing(Container.DataItem) %><br>
			<span class="smallfont"><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></span>
			<br>
			<asp:repeater visible='<%# DataBinder.Eval(Container.DataItem, "[\"Moderated\"]") %>' id=ModeratorList runat=server onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
				<HeaderTemplate><span class=smallfont><%# ForumPage.GetText("moderators") %>: </HeaderTemplate>
				<ItemTemplate><%# DataBinder.Eval(Container.DataItem, "[\"GroupName\"]") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
				<FooterTemplate></span></FooterTemplate>
			</asp:repeater>
		</td>
		<td align=center><%# DataBinder.Eval(Container.DataItem, "[\"Topics\"]") %></td>
		<td align=center><%# DataBinder.Eval(Container.DataItem, "[\"Posts\"]") %></td>
		<td align=center class=smallfont nowrap="nowrap"><%# FormatLastPost((System.Data.DataRow)Container.DataItem) %></td>
	</tr>
</ItemTemplate>
</asp:Repeater>
