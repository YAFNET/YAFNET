<%@ Register TagPrefix="uc1" TagName="forumjump" Src="forumjump.ascx" %>
<%@ Page language="c#" Codebehind="active.aspx.cs" AutoEventWireup="false" Inherits="yaf.active" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink id="HomeLink" runat="server"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class=command cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class=navlinks align=left id=PageLinks1 runat=server></td>
		<td align=right><%= GetText("active_since") %> <asp:DropDownList id=Since runat="server" AutoPostBack="True"/></td>
	</tr>
</table>

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class=header1 width="1%">&nbsp;</td>
	<td class=header1 align=left><%= GetText("active_topics") %></td>
	<td class=header1 align=left width="20%"><%= GetText("active_topic_starter") %></td>
	<td class=header1 align=middle width="7%"><%= GetText("active_replies") %></td>
	<td class=header1 align=middle width="7%"><%= GetText("active_views") %></td>
	<td class=header1 align=middle width="20%"><%= GetText("active_lastpost") %></td>
</tr>
<asp:repeater id=TopicList runat="server">
<ItemTemplate>
	<%# PrintForumName((System.Data.DataRowView)Container.DataItem) %>
	<tr class=post>
		<td><img src='<%# GetTopicImage(Container.DataItem) %>'></td>
		<td class=largefont><%# GetPriorityMessage(Container.DataItem) %>
			<a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem, "TopicID") %>'><%# DataBinder.Eval(Container.DataItem, "Subject") %></a></td>
		<td><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Starter") %></a></td>
		<td align=center><%# DataBinder.Eval(Container.DataItem, "Replies") %></td>
		<td align=center><%# DataBinder.Eval(Container.DataItem, "Views") %></td>
		<td align=center class=smallfont><%# FormatLastPost((System.Data.DataRowView)Container.DataItem) %></td>
	</tr>
</ItemTemplate>
</asp:repeater>
<tr>
	<td align=middle colSpan=6 class=footer1>&nbsp;</td>
</tr>
</table>

<table class=command width="100%" cellspacing=0 cellpadding=0>
<tr>
	<td align="left" class=navlinks id=PageLinks2 runat="server"/>
</tr>
</table>

<table width=100% cellspacing=0 cellpadding=0>
<tr>
	<td align=right colspan=2>
		<%= GetText("Forum_Jump") %> <uc1:forumjump runat="server"/>
	</td>
</tr>
<tr><td valign=top>
	<table cellspacing=1 cellpadding=1>
	<tr>
		<td><img align=absMiddle src='<% =ThemeFile("topic_new.png") %>'> <%= GetText("New_Posts") %></td>
		<td><img align=absMiddle src='<% =ThemeFile("topic.png") %>'> <%= GetText("No_New_Posts") %></td>
		<td><img align=absMiddle src='<% =ThemeFile("topic_announce.png") %>'> <%= GetText("Announcement") %></td>
	</tr>
	<tr>
		<td><img align=absMiddle src='<% =ThemeFile("topic_lock_new.png") %>'> <%= GetText("New_Posts_Locked") %></td>
		<td><img align=absMiddle src='<% =ThemeFile("topic_lock.png") %>'> <%= GetText("No_New_Posts_Locked") %></td>
		<td><img align=absMiddle src='<% =ThemeFile("topic_sticky.png") %>'> <%= GetText("Sticky") %></td>
	</tr>
	<tr>
		<td colspan=3><img align=absMiddle src='<% =ThemeFile("topic_moved.png") %>'> <%= GetText("Moved") %></td>
	</tr>
	</table>
</td></tr>
</table>

</form>
