<%@ Page language="c#" Codebehind="moderate.aspx.cs" AutoEventWireup="false" Inherits="yaf.moderate0" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=4><%=GetText("MEMBERS")%></td>
</tr>

<tr class="header2">
	<td><%=GetText("USER")%></td>
	<td align="center"><%=GetText("ACCEPTED")%></td>
	<td><%=GetText("ACCESSMASK")%></td>
	<td>&nbsp;</td>
</tr>
<asp:repeater runat="server" id="UserList">
<ItemTemplate>
<tr class="post">
	<td><%# DataBinder.Eval(Container.DataItem,"Name") %></td>
	<td align="center"><%# DataBinder.Eval(Container.DataItem,"Accepted") %></td>
	<td><%# DataBinder.Eval(Container.DataItem,"Access") %></td>
	<td>
		<asp:linkbutton runat="server" text='<%#GetText("EDIT")%>' commandname="edit" commandargument='<%# DataBinder.Eval(Container.DataItem,"UserID") %>'/>
		|
		<asp:linkbutton runat="server" text='<%#GetText("REMOVE")%>' onload="DeleteUser_Load" commandname="remove" commandargument='<%# DataBinder.Eval(Container.DataItem,"UserID") %>'/>
	</td>
</tr>
</ItemTemplate>
</asp:repeater>
<tr class="footer1">
	<td colspan="4"><asp:linkbutton runat="server" id="AddUser" text="Invite User"/></td>
</tr>
</table>

<br/>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=7><%= GetText("title") %></td>
</tr>
<tr>
	<td class=header2 width="1%">&nbsp;</td>
	<td class=header2 align=left><%= GetText("topics") %></td>
	<td class=header2 align=left width="20%"><%= GetText("topic_starter") %></td>
	<td class=header2 align=middle width="7%"><%= GetText("replies") %></td>
	<td class=header2 align=middle width="7%"><%= GetText("views") %></td>
	<td class=header2 align=middle width="25%"><%= GetText("lastpost") %></td>
	<td class=header2>&nbsp;</td>
</tr>

<asp:repeater id=topiclist runat=server>
<itemtemplate>
	<yaf:TopicLine runat="server" DataRow=<%# Container.DataItem %>>
		<td class="postheader" align="center">
			<asp:linkbutton runat=server onload="Delete_Load" commandargument='<%# DataBinder.Eval(Container.DataItem, "TopicID") %>' commandname='delete'><%# GetThemeContents("BUTTONS","DELETETOPIC") %></asp:linkbutton>
		</td>
	</yaf:TopicLine>
</itemtemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=7>&nbsp;</td>
</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
