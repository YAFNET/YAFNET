<%@ Control language="c#" Codebehind="pm.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.pm" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=2>Private messages</td>
</tr>
<tr>
	<td class="postheader" width="50%">Number of private messages:</td>
	<td class="post" width="50%"><asp:label runat="server" id="Count"/></td>
</td>
<tr>
	<td class="postheader" width="50%">Delete read messages older than:</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="Days1"/> days</td>
</td>
<tr>
	<td class="postheader" width="50%">Delete unread messages older than:</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="Days2"/> days</td>
</td>

<tr>
	<td class=footer1 colspan=2 align=center>
		<asp:button id=commit runat=server text="Delete"/>
	</td>
</tr>
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />

