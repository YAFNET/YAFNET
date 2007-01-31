<%@ Control language="c#" Codebehind="emailtopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.emailtopic" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="2"><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader><%= GetText("to") %></td>
	<td class=post><asp:textbox id=EmailAddress runat=server cssclass=edit></asp:textbox></td>
</tr>
<tr>
	<td class=postheader><%= GetText("subject") %></td>
	<td class=post><asp:textbox id=Subject runat=server cssclass=edit></asp:textbox></td>
</tr>
<tr>
	<td class=postheader valign=top><%= GetText("message") %></td>
	<td class=post valign=top><asp:textbox id=Message runat=server cssclass=edit TextMode="MultiLine" Rows="12"></asp:textbox></td>
</tr>
<tr>
	<td class=footer1 colspan="2" align=center><asp:button id=SendEmail runat="server" onclick="SendEmail_Click" /></td>
</tr>
</table>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
