<%@ Page language="c#" Codebehind="cp_signature.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_signature" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader valign=top><%= GetText("signature") %></td>
	<td class=post>
		<asp:textbox id=sig runat="server" cssclass="posteditor" TextMode="MultiLine" Rows="12"/>
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
