<%@ Page language="c#" Codebehind="topics.aspx.cs" AutoEventWireup="false" Inherits="yaf.topics" %>
<%@ Register TagPrefix="uc1" TagName="forumjump" Src="forumjump.ascx" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=CategoryLink runat="server">Category</asp:hyperlink>
	&#187; <asp:hyperlink id=ForumLink runat="server">Forum</asp:hyperlink>
</p>

<table class=command cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class=navlinks align=left id=PageLinks1 runat="server"/>
		<td align="right">
			<asp:linkbutton id=moderate1 runat=server cssclass="imagelink"/>
			<asp:linkbutton id=NewTopic1 runat="server" cssclass="imagelink"/>
		</td>
	</tr>
</table>

<table class=content cellSpacing=1 cellPadding=0 width="100%">
	<tr>
		<td class=header1 colspan=6><asp:label id=PageTitle runat="server"></asp:label></td>
	</tr>
  <tr>
    <td class=header2 width="1%">&nbsp;</td>
    <td class=header2 align=left><%# GetText("Topics") %></td>
    <td class=header2 align=left width="20%"><%# GetText("Topic_Starter") %></td>
    <td class=header2 align=middle width="7%"><%# GetText("Replies") %></td>
    <td class=header2 align=middle width="7%"><%# GetText("Views") %></td>
    <td class=header2 align=middle width="25%"><%# GetText("Last_Post") %></td>
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
					<%# GetText("Show_Topics") %>
					<asp:DropDownList id=ShowList runat="server" AutoPostBack="True"/>
					</nobr>
				</td>
				<td align="right">
				<asp:linkbutton id="WatchForum" runat="server"/>
				</td></tr></table>

</td></tr></table>

<table class=command width="100%" cellspacing=0 cellpadding=0>
	<tr>
		<td align="left" class=navlinks id=PageLinks2 runat="server"/>
		<td align="right">
			<asp:linkbutton id=moderate2 runat=server cssclass="imagelink"/>
			<asp:linkbutton id=NewTopic2 runat="server" cssclass="imagelink"/>
		</td>
	</tr>
</table>

<table width=100% cellspacing=0 cellpadding=0>
<tr>
	<td align=right colspan=2>
		<%# GetText("Forum_Jump") %> <uc1:forumjump id=Forumjump1 runat="server"/>
	</td>
</tr>
<tr><td valign=top>
	<table cellspacing=1 cellpadding=1>
		<tr>
			<td><img align=absMiddle src='<% =ThemeFile("topic_new.png") %>'> <%# GetText("New_Posts") %></td>
			<td><img align=absMiddle src='<% =ThemeFile("topic.png") %>'> <%# GetText("No_New_Posts") %></td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_announce.png") %>'> <%# GetText("Announcement") %></td>
		</tr>
		<tr>
			<td><img align=absMiddle src='<% =ThemeFile("topic_lock_new.png") %>'> <%# GetText("New_Posts_Locked") %></td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_lock.png") %>'> <%# GetText("No_New_Posts_Locked") %></td>
			<td><img align=absMiddle src='<% =ThemeFile("topic_sticky.png") %>'> <%# GetText("Sticky") %></td>
		</tr>
		<tr>
			<td colspan=3><img align=absMiddle src='<% =ThemeFile("topic_moved.png") %>'> <%# GetText("Moved") %></td>
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
