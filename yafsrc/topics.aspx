<%@ Page language="c#" Codebehind="topics.aspx.cs" AutoEventWireup="false" Inherits="yaf.topics" %>
<%@ Register TagPrefix="uc1" TagName="forumjump" Src="forumjump.ascx" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	» <asp:hyperlink id=CategoryLink runat="server">Category</asp:hyperlink>
	» <asp:hyperlink id=ForumLink runat="server">Forum</asp:hyperlink>
</p>

<table class=command cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class=navlinks align=left id=PageLinks1 runat=server></td>
		<td align="right">
			<asp:linkbutton id=moderate1 runat=server ToolTip="Moderate this forum" cssclass=imagelink><img align=absmiddle title="Moderate this forum" src='<%# ThemeFile("topic_moderate.png") %>'></asp:linkbutton>
			<asp:linkbutton id=NewTopic1 runat="server" ToolTip="New Topic" cssclass=imagelink><img align=absmiddle title="Post new topic" src='<%# ThemeFile("b_post_topic.png") %>'></asp:linkbutton>
		</td>
	</tr>
</table>

<table class=content cellSpacing=1 cellPadding=0 width="100%">
	<tr>
		<td class=header1 colspan=6><asp:label id=PageTitle runat="server"></asp:label></td>
	</tr>
  <tr>
    <td class=header2 width="1%">&nbsp;</td>
    <td class=header2 align=left>Topics</td>
    <td class=header2 align=left width="20%">Topic Starter</td>
    <td class=header2 align=middle width="7%">Replies</td>
    <td class=header2 align=middle width="7%">Views</td>
    <td class=header2 align=middle width="25%">Last Post</td>
   </tr>
<asp:repeater id=Announcements runat="server">
	<ItemTemplate>
		<tr class=post>
			<td><img src='<%# GetTopicImage(Container.DataItem) %>'></td>
			<td class=largefont><%# GetPriorityMessage((System.Data.DataRowView)Container.DataItem) %>
				<a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem, "LinkTopicID") %>'><%# DataBinder.Eval(Container.DataItem, "Subject") %></a></td>
			<td><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Starter") %></a></td>
			<td align=center><%# DataBinder.Eval(Container.DataItem, "Replies") %></td>
			<td align=center><%# DataBinder.Eval(Container.DataItem, "Views") %></td>
			<td align=center class=smallfont><%# FormatLastPost((System.Data.DataRowView)Container.DataItem) %></td>
		</tr>
	</ItemTemplate>
</asp:repeater>
<asp:repeater id=TopicList runat="server">
	<ItemTemplate>
		<tr class=post>
			<td><img src='<%# GetTopicImage(Container.DataItem) %>'></td>
			<td class=largefont><%# GetPriorityMessage((System.Data.DataRowView)Container.DataItem) %>
				<a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem, "LinkTopicID") %>'><%# DataBinder.Eval(Container.DataItem, "Subject") %></a></td>
			<td><a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Starter") %></a></td>
			<td align=center><%# DataBinder.Eval(Container.DataItem, "Replies") %></td>
			<td align=center><%# DataBinder.Eval(Container.DataItem, "Views") %></td>
			<td align=center class=smallfont><%# FormatLastPost((System.Data.DataRowView)Container.DataItem) %></td>
		</tr>
	</ItemTemplate>
</asp:repeater>
  <tr>
    <td align=middle colSpan=6 class=footer1>
      <table cellSpacing=0 cellPadding=0 width="100%">
        <tr>
				<td width="1%">
					<nobr>
					<asp:Label id=Label1 runat="server">Show Topics</asp:Label>
					<asp:DropDownList id=ShowList runat="server" AutoPostBack="True">
<asp:ListItem Value="0" Selected="True">All</asp:ListItem>
<asp:ListItem Value="1">from the last week</asp:ListItem>
<asp:ListItem Value="2">from the last two weeks</asp:ListItem>
<asp:ListItem Value="3">from the last month</asp:ListItem>
<asp:ListItem Value="4">from the last two months</asp:ListItem>
<asp:ListItem Value="5">from the last six months</asp:ListItem>
<asp:ListItem Value="6">from the last year</asp:ListItem></asp:DropDownList>		
					</nobr>
				</td>
				<td align="right">
				<asp:linkbutton id=WatchForum runat=server>Watch This Forum</asp:linkbutton>
				</td></tr></table>

</td></tr></table>

<table class=command width="100%" cellspacing=0 cellpadding=0>
	<tr>
		<td align="left" class=navlinks id=PageLinks2 runat=server></td>
		<td align="right">
			<asp:linkbutton id=moderate2 runat=server ToolTip="Moderate this forum" cssclass=imagelink><img align=absmiddle title="Moderate this forum" src='<%# ThemeFile("topic_moderate.png") %>'></asp:linkbutton>
			<asp:linkbutton id=NewTopic2 runat="server" ToolTip="New Topic" cssclass=imagelink><img align=absmiddle title="Post new topic" src='<%# ThemeFile("b_post_topic.png") %>'></asp:linkbutton>
		</td>
	</tr>
</table>

<table width=100% cellspacing=0 cellpadding=0>
<tr>
	<td align=right colspan=2>
		Forum Jump <uc1:forumjump id=Forumjump1 runat="server"></uc1:forumjump>
	</td>
</tr>
<tr><td valign=top>
	<table cellspacing=1 cellpadding=1>
		<tr>
			<td><img align=absMiddle src='<% =ThemeFile("topic_new.png") %>'> New Posts</td>
			<td><img align=absMiddle src='<% =ThemeFile("topic.png") %>'> No New Posts</td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_announce.png") %>'> Announcement</td>
		</tr>
		<tr>
			<td><img align=absMiddle src='<% =ThemeFile("topic_lock_new.png") %>'> New Posts (Locked)</td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_lock.png") %>'> No New Posts (Locked)</td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_sticky.png") %>'> Sticky</td>
		</tr>
		<tr>
			<td colspan=3><img align=absMiddle src='<% =ThemeFile("topic_moved.png") %>'> Moved</td>
		</tr>
	</table>
</td><td align=right>
	<table cellspacing=1 cellpadding=1>
		<tr>
			<td align=right valign=top id=AccessCell class=smallfont runat=server></td>
		</tr>
	</table>
</td></tr>
</table>

</form>
