<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="yaf._default" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<yaf:PageLinks runat="server" id="PageLinks"/>

<p id=Welcome runat=server>
	<table cellSpacing=0 cellPadding=0>
	<tr>
		<td>
			<asp:label id=TimeNow runat="server"/><br/>
			<asp:label id=TimeLastVisit runat="server"/><br/>
			<asp:hyperlink runat="server" id="UnreadMsgs" visible="false"/>
		</td>
	</tr>
	</table>
</p>

<asp:repeater id=CategoryList runat="server">
<HeaderTemplate>
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class=header1 width=1%>&nbsp;</td>
			<td class=header1 align=left><%# GetText("FORUM") %></td>
			<td class=header1 align=center width=7%><%# GetText("topics") %></td>
			<td class=header1 align=center width=7%><%# GetText("posts") %></td>
			<td class=header1 align=center width=25%><%# GetText("lastpost") %></td>
		</tr>
</HeaderTemplate>
<ItemTemplate>
		<tr>
			<td class=header2 colspan=5><a href='default.aspx?c=<%# DataBinder.Eval(Container.DataItem, "CategoryID") %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a></td>
		</tr>
		<asp:Repeater id=ForumList runat="server" onitemcommand='ForumList_ItemCommand' datasource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("myrelation") %>'>
			<ItemTemplate>
				<tr class=post>
					<td><img src='<%# GetForumIcon(DataBinder.Eval(Container.DataItem, "[\"LastPosted\"]"),DataBinder.Eval(Container.DataItem, "[\"Locked\"]")) %>'></td>
					<td>
						<asp:linkbutton runat="server" cssclass=largefont commandname="forum" commandargument='<%# DataBinder.Eval(Container.DataItem, "[\"ForumID\"]") %>'><%# DataBinder.Eval(Container.DataItem, "[\"Forum\"]") %></asp:linkbutton><%# GetViewing(Container.DataItem) %><br>
						<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
						<br>
						<asp:repeater visible='<%# DataBinder.Eval(Container.DataItem, "[\"Moderated\"]") %>' id=ModeratorList runat=server onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("rel2") %>'>
							<HeaderTemplate><span class=smallfont><%# GetText("moderators") %>: </HeaderTemplate>
							<ItemTemplate><%# DataBinder.Eval(Container.DataItem, "[\"GroupName\"]") %></ItemTemplate>
							<SeparatorTemplate>, </SeparatorTemplate>
							<FooterTemplate></span></FooterTemplate>
						</asp:repeater>
					</td>
					<td align=center><%# DataBinder.Eval(Container.DataItem, "[\"Topics\"]") %></td>
					<td align=center><%# DataBinder.Eval(Container.DataItem, "[\"Posts\"]") %></td>
					<td align=center class=smallfont><%# FormatLastPost((System.Data.DataRow)Container.DataItem) %></td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
</ItemTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
</asp:repeater>

<br>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=2><%= GetText("INFORMATION") %></td>
</tr>
<tr>
	<td class=header2 colspan=2><%= GetText("ACTIVE_USERS") %></td>
</tr>
<tr>
	<td class=post width=1%><img src='<%# GetThemeContents("ICONS","FORUM_USERS") %>'></td>
	<td class=post>
		<asp:label runat="server" id="activeinfo"/><br>
		<asp:repeater runat="server" id="ActiveList">
			<ItemTemplate><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a></ItemTemplate>
			<SeparatorTemplate>, </SeparatorTemplate>
		</asp:repeater>
	</td>
</tr>

<tr>
    <td class=header2 colspan=2><%= GetText("STATS") %></td>
</tr>
<tr>
	<td class=post width=1%><img src='<%# GetThemeContents("ICONS","FORUM_STATS") %>'></td>
	<td class=post><asp:label id=Stats runat="server">Label</asp:label></td>
</tr>
</table>

<table cellspacing=1 cellpadding=1>
	<tr>
		<td><img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM_NEW") %>'> <%# GetText("ICONLEGEND","New_Posts") %></td>
		<td><img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM") %>'> <%# GetText("ICONLEGEND","No_New_Posts") %></td>
		<td><img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM_LOCKED") %>'> <%# GetText("ICONLEGEND","Forum_Locked") %></td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
