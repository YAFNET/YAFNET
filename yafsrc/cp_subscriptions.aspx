<%@ Page language="c#" Codebehind="cp_subscriptions.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_subscriptions" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=UserLink runat="server"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=5><%= GetText("forums") %></td>
</td>
<tr>
	<td class=header2><%= GetText("forum") %></td>
	<td class=header2 align=center><%= GetText("topics") %></td>
	<td class=header2 align=center><%= GetText("replies") %></td>
	<td class=header2><%= GetText("lastpost") %></td>
	<td class=header2>&nbsp;</td>
</tr>
<asp:repeater id=ForumList runat=server>
	<itemtemplate>
		<asp:label id=tfid runat=server text='<%# DataBinder.Eval(Container.DataItem,"WatchForumID") %>' visible="false"/>


		<tr>
			<td class=post><%# DataBinder.Eval(Container.DataItem,"ForumName") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Topics") %></td>
			<td class=post align=center><%# FormatForumReplies(Container.DataItem) %></td>
			<td class=post><%# FormatLastPosted(Container.DataItem) %></td>
			<td class=post align=center><asp:checkbox id=unsubf runat="server"/></td>
		</tr>
	</itemtemplate>
</asp:repeater>
<tr>
	<td class=footer1 colspan=5 align=center><asp:button id=UnsubscribeForums runat="server"/></td>
</tr>
</table>

<br/>

<table class=content cellspacing=1 cellpadding=0 width="100%">
<tr>
	<td class=header1 colspan=5><%= GetText("topics") %></td>
</td>
<tr>
	<td class=header2><%= GetText("topic") %></td>
	<td class=header2 align=middle><%= GetText("replies") %></td>
	<td class=header2 align=middle><%= GetText("views") %></td>
	<td class=header2><%= GetText("lastpost") %></td>
	<td class=header2>&nbsp;</td>
</tr>
<asp:repeater id=TopicList runat=server>
	<itemtemplate>
		<asp:label id=ttid runat=server text='<%# DataBinder.Eval(Container.DataItem,"WatchTopicID") %>' visible="false"/>
		<tr>
			<td class=post><%# DataBinder.Eval(Container.DataItem,"TopicName") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Replies") %></td>
			<td class=post align=center><%# DataBinder.Eval(Container.DataItem,"Views") %></td>
			<td class=post><%# FormatLastPosted(Container.DataItem) %></td>
			<td class=post align=center><asp:checkbox id=unsubx runat="server"/></td>
		</tr>
	</itemtemplate>
</asp:repeater>
<tr>
	<td class=footer1 colspan=5 align=middle><asp:button id=UnsubscribeTopics runat="server"/></td>
</tr>
</table>

</form>
