<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="yaf._default" %>

<form runat="server">

<p class="navlinks" id=NavLinks runat=server>
	<asp:hyperlink id=HomeLink runat=server>HomeLink</asp:hyperlink>
	&#187; <asp:hyperlink id=CategoryLink runat=server>CategoryLink</asp:hyperlink>
</p>
<p class="navlinks" id="NavLinks2" runat=server>
	<asp:hyperlink id="HomeLink2" runat=server>HomeLink</asp:hyperlink>
</p> 

<p id=Welcome runat=server><table class=title1 cellSpacing=0 cellPadding=0>
  <tr>
    <td><asp:label id=TimeNow runat="server">Label</asp:label><br><asp:label id=TimeLastVisit runat="server"></asp:label></td></tr></table>
</p>

<asp:repeater id=CategoryList runat="server">
<HeaderTemplate>
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class=header1 width=1%>&nbsp;</td>
			<td class=header1 align=left>Forum</td>
			<td class=header1 align=center width=7%>Topics</td>
			<td class=header1 align=center width=7%>Posts</td>
			<td class=header1 align=center width=25%>Last Post</td>
		</tr>
</HeaderTemplate>
<ItemTemplate>
		<tr>
			<td class=header2 colspan=5><a href='default.aspx?c=<%# DataBinder.Eval(Container.DataItem, "CategoryID") %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a></td>
		</tr>
		<asp:Repeater id=ForumList runat="server" onitemcommand='ForumList_ItemCommand' datasource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("myrelation") %>'>
			<ItemTemplate>
				<tr class=post>
					<td><img src='<%# GetForumIcon(DataBinder.Eval(Container.DataItem, "[\"LastPosted\"]"),DataBinder.Eval(Container.DataItem, "[\"Locked\"]"),((System.Data.DataRow)Container.DataItem)["PostAccess"],((System.Data.DataRow)Container.DataItem)["ReplyAccess"],((System.Data.DataRow)Container.DataItem)["ReadAccess"]) %>'></td>
					<td>
						<asp:linkbutton runat="server" cssclass=largefont commandname="forum" commandargument='<%# DataBinder.Eval(Container.DataItem, "[\"ForumID\"]") %>'><%# DataBinder.Eval(Container.DataItem, "[\"Forum\"]") %></asp:linkbutton><br>
						<%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %>
						<br>
						<asp:repeater id=ModeratorList runat=server onitemcommand='ModeratorList_ItemCommand' datasource='<%# ((System.Data.DataRow)Container.DataItem).GetChildRows("rel2") %>'>
							<HeaderTemplate>
								<span class=smallfont>Moderators:
							</HeaderTemplate>
							<ItemTemplate>
								<%# DataBinder.Eval(Container.DataItem, "[\"GroupName\"]") %>
							</ItemTemplate>
							<FooterTemplate>
								</span>
							</FooterTemplate>
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
	<td class=header1 colspan=2>Information</td>
</tr>
<tr>
	<td class=header2 colspan=2>Active Users</td>
</tr>
<tr>
	<td class=post width=1%><img src='<%# ThemeFile("folder_who.png") %>'></td>
	<td class=post>
		<asp:label id=activeinfo runat=server></asp:label><br>

<asp:repeater runat=server id=ActiveList>
<ItemTemplate>
	<a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
</ItemTemplate>
<SeparatorTemplate>
-
</SeparatorTemplate>
</asp:repeater>

	</td>
</tr>

<tr>
    <td class=header2 colspan=2>Statistics</td>
</tr>
<tr>
	<td class=post width=1%><img src='<%# ThemeFile("folder_stats.png") %>'></td>
	<td class=post><asp:label id=Stats runat="server">Label</asp:label></td>
</tr>
</table>

<table cellspacing=1 cellpadding=1>
	<tr>
		<td><img align=absMiddle src='<% =ThemeFile("topic_new.png") %>'> New Posts</td>
		<td><img align=absMiddle src='<% =ThemeFile("topic.png") %>'> No New Posts</td>
		<td><img align=absMiddle src='<% =ThemeFile("topic_lock.png") %>'> Forum Locked</td>
	</tr>
</table>

</form>
