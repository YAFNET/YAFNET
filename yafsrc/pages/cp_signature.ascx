<%@ Control language="c#" Codebehind="cp_signature.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.cp_signature" %>
<%@ Register TagPrefix="rte" Namespace="yaf" Assembly="yaf" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader valign=top><%= GetText("signature") %></td>
	<td class=post>
		<rte:RichEdit runat="server" id="sig"/>
	</td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=center>
		<asp:button id=save runat="server"/>
		<asp:button id=cancel runat="server"/>
	</td>
</tr>
</table>

<yaf:savescrollpos runat="server"/>
