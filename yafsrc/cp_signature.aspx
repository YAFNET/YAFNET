<%@ Page language="c#" Codebehind="cp_signature.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_signature" %>
<%@ Register TagPrefix="RichEdit" TagName="rte" Src="rte/rte.ascx" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader valign=top><%= GetText("signature") %></td>
	<td class=post>
		<RichEdit:rte runat="server" id="sig" cssclass="posteditor"/>
	</td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=center>
		<asp:button id=save runat="server"/>
		<asp:button id=cancel runat="server"/>
	</td>
</tr>
</table>

</form>
