<%@ Control language="c#" Inherits="yaf.pages.movetopic" CodeFile="movetopic.ascx.cs" CodeFileBaseClass="yaf.pages.ForumPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader width="50%"><%= GetText("select_forum") %></td>
	<td class=post width="50%">
		<asp:DropDownList id=ForumList runat="server" DataValueField="ForumID" DataTextField="Forum"/>
	</td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=middle>
		<asp:Button id=Move runat="server" onclick="Move_Click" />
	</td>
</tr>
</table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
