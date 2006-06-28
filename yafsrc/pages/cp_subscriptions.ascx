<%@ Control language="c#" Codebehind="cp_subscriptions.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.cp_subscriptions" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

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
		<asp:label id=tfid runat=server text='<%# Eval("WatchForumID") %>' visible="false"/>


		<tr>
			<td class=post><%# Eval("ForumName") %></td>
			<td class=post align=center><%# Eval("Topics") %></td>
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
		<asp:label id=ttid runat=server text='<%# Eval("WatchTopicID") %>' visible="false"/>
		<tr>
			<td class=post><%# Eval("TopicName") %></td>
			<td class=post align=center><%# Eval("Replies") %></td>
			<td class=post align=center><%# Eval("Views") %></td>
			<td class=post><%# FormatLastPosted(Container.DataItem) %></td>
			<td class=post align=center><asp:checkbox id=unsubx runat="server"/></td>
		</tr>
	</itemtemplate>
</asp:repeater>
<tr>
	<td class=footer1 colspan=5 align=middle><asp:button id=UnsubscribeTopics runat="server"/></td>
</tr>
</table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
