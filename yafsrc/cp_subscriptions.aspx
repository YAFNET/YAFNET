<%@ Page language="c#" Codebehind="cp_subscriptions.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_subscriptions" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	» <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
	» <a href="cp_subscriptions.aspx">Subscriptions</a>
</p>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=6>Forums</td>
</td>
<tr>
	<td class=header2>&nbsp;</td>
	<td class=header2>Forum</td>
	<td class=header2 align=center>Topics</td>
	<td class=header2 align=center>Replies</td>
	<td class=header2>Last Post</td>
	<td class=header2>&nbsp;</td>
</tr>
<asp:repeater id=ForumList runat=server>
	<itemtemplate>
		<tr>
			<td class=post>&nbsp;<asp:label id=tfid runat=server text='<%# DataBinder.Eval(Container.DataItem,"WatchForumID") %>'></asp:label></td>
			<td class=post><%# DataBinder.Eval(Container.DataItem,"ForumName") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Topics") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Replies") %></td>
			<td class=post><%# FormatLastPostedForum(Container.DataItem) %></td>
			<td class=post align=center><asp:checkbox id=unsubf runat=server></asp:checkbox></td>
		</tr>
	</itemtemplate>
</asp:repeater>
<tr>
	<td class=footer1 colspan=6 align=center><asp:button id=UnsubscribeForums runat=server text=Unsubscribe></asp:button></td>
</tr>
</table>

<br>

<table class=content cellspacing=1 cellpadding=0 width="100%">
<tr>
	<td class=header1 colspan=6>Topics</td>
</td>
<tr>
	<td class=header2>&nbsp;</td>
	<td class=header2>Topic</td>
	<td class=header2 align=middle>Replies</td>
	<td class=header2 align=middle>Views</td>
	<td class=header2>Last Post</td>
	<td class=header2>&nbsp;</td>
</tr>
<asp:repeater id=TopicList runat=server>
	<itemtemplate>
		<tr>
			<td class=post>&nbsp;
<asp:label id=ttid runat=server text='<%# DataBinder.Eval(Container.DataItem,"WatchTopicID") %>'></asp:label></td>
			<td class=post><%# DataBinder.Eval(Container.DataItem,"TopicName") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Replies") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Views") %></td>
			<td class=post><%# FormatLastPosted(Container.DataItem) %></td>
			<td class=post align=center>
<asp:checkbox id=unsubx runat=server></asp:checkbox></td>
		</tr>
	</itemtemplate>
</asp:repeater>
<tr>
	<td class=footer1 colspan=6 align=middle><asp:button id=UnsubscribeTopics runat=server text=Unsubscribe></asp:button></td>
</tr>
</table>

</form>
