<%@ Page language="c#" Codebehind="movetopic.aspx.cs" AutoEventWireup="false" Inherits="yaf.movetopic" %>

<form runat=server>

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=CategoryLink runat="server"/>
	&#187; <asp:hyperlink id=ForumLink runat="server"/>
	&#187; <asp:hyperlink id=TopicLink runat="server"/>
</p>

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
		<asp:Button id=Move runat="server"/>
	</td>
</tr>
</table>

</form>
