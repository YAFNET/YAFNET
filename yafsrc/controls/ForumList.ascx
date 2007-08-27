<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ForumList.ascx.cs" Inherits="YAF.Controls.ForumList" EnableViewState="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<asp:Repeater id="forumList" runat="server">
<ItemTemplate>
	<tr class="post">
		<td><%# GetForumIcon(Container.DataItem) %></td>
		<td>
			<span class="forumheading"><%# GetForumLink((System.Data.DataRow)Container.DataItem) %></span>
			<span class="forumviewing"><%# GetViewing(Container.DataItem) %></span><br />
			<span class="subforumheading"><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></span>
			<br/>
			<asp:Repeater ID="SubforumList" Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>' DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' runat="server">
			  <HeaderTemplate><span class="smallfont subforumlink"><b><%# PageContext.Localization.GetText("SUBFORUMS")%></b>: </HeaderTemplate>			  
			  <ItemTemplate><%# GetSubForumIcon(Container.DataItem) %> <%# GetForumLink((System.Data.DataRow)Container.DataItem) %></ItemTemplate>
			  <SeparatorTemplate>, </SeparatorTemplate>
			  <FooterTemplate></span></FooterTemplate>
			</asp:Repeater>
			<asp:repeater visible='<%# GetModerated(Container.DataItem) %>' id="ModeratorList" runat="server" onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
				<HeaderTemplate><span class="smallfont"><%# PageContext.Localization.GetText("moderators") %>: </HeaderTemplate>
				<ItemTemplate><%# Eval( "[\"GroupName\"]") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
				<FooterTemplate></span></FooterTemplate>
			</asp:repeater>
		</td>
		<td align="center"><%# Topics(Container.DataItem) %></td>
		<td align="center"><%# Posts(Container.DataItem) %></td>
		<td align="center" class="smallfont" nowrap="nowrap"><%# YAF.Classes.Utils.General.BadWordReplace(FormatLastPost((System.Data.DataRow)Container.DataItem)) %></td>
	</tr>
</ItemTemplate>
<AlternatingItemTemplate>
	<tr class="post_alt">
		<td><%# GetForumIcon(Container.DataItem) %></td>
		<td>
			<span class="forumheading"><%# GetForumLink((System.Data.DataRow)Container.DataItem) %></span>
			<span class="forumviewing"><%# GetViewing(Container.DataItem) %></span><br />
			<span class="subforumheading"><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></span>
			<br/>
			<asp:Repeater ID="SubforumList" Visible='<%# HasSubforums((System.Data.DataRow)Container.DataItem) %>' DataSource='<%# GetSubforums( (System.Data.DataRow)Container.DataItem ) %>' runat="server">
			  <HeaderTemplate><span class="smallfont subforumlink"><b><%# PageContext.Localization.GetText("SUBFORUMS")%></b>: </HeaderTemplate>			  
			  <ItemTemplate><%# GetSubForumIcon(Container.DataItem) %> <%# GetForumLink((System.Data.DataRow)Container.DataItem) %></ItemTemplate>
			  <SeparatorTemplate>, </SeparatorTemplate>
			  <FooterTemplate></span></FooterTemplate>
			</asp:Repeater>
			<asp:repeater visible='<%# GetModerated(Container.DataItem) %>' id="Repeater1" runat="server" onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("FK_Moderator_Forum") %>'>
				<HeaderTemplate><span class="smallfont"><%# PageContext.Localization.GetText("moderators") %>: </HeaderTemplate>
				<ItemTemplate><%# Eval( "[\"GroupName\"]") %></ItemTemplate>
				<SeparatorTemplate>, </SeparatorTemplate>
				<FooterTemplate></span></FooterTemplate>
			</asp:repeater>
		</td>
		<td align="center"><%# Topics(Container.DataItem) %></td>
		<td align="center"><%# Posts(Container.DataItem) %></td>
		<td align="center" class="smallfont" nowrap="nowrap"><%# YAF.Classes.Utils.General.BadWordReplace(FormatLastPost((System.Data.DataRow)Container.DataItem)) %></td>
	</tr>
</AlternatingItemTemplate>
</asp:Repeater>
