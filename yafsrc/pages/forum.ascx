<%@ Control language="c#" Codebehind="forum.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.forum" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" TagName="ForumList" Src="../controls/ForumList.ascx" %>

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
		<td class=header2 colspan=5><a href='<%# yaf.Forum.GetLink(yaf.Pages.forum,"c={0}",DataBinder.Eval(Container.DataItem, "CategoryID")) %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a></td>
	</tr>
	<yaf:forumlist runat="server" datasource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'/>
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
			<ItemTemplate><a href='<%#yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID"))%>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a></ItemTemplate>
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

<table cellspacing=1 cellpadding=1 width="100%">
	<tr>
		<td>
			<img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM_NEW") %>'> <%# GetText("ICONLEGEND","New_Posts") %>
			<img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM") %>'> <%# GetText("ICONLEGEND","No_New_Posts") %>
			<img align=absMiddle src='<% =GetThemeContents("ICONS","FORUM_LOCKED") %>'> <%# GetText("ICONLEGEND","Forum_Locked") %>
		</td>
		<td align="right"><asp:linkbutton runat="server" id="MarkAll"/></td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
