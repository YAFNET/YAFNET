<%@ Page language="c#" Codebehind="approve.aspx.cs" AutoEventWireup="false" Inherits="yaf.approve" %>

<form runat=server method=get>

<p class=navlinks><asp:hyperlink id=HomeLink runat=server>HomeLink</asp:hyperlink></p>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=approved runat=server visible=false>
	<tr>
		<td class=header1 colspan=2>Validate Email Address</td>
	</tr>
	<tr>
		<td class=post colspan=2 align=middle>Your email address has been verified.</td>
	</tr>
</table>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=error runat=server visible=false>
	<tr>
		<td class=header1 colspan=2>Validate Email Address</td>
	</tr>
	<tr>
		<td class=header2 colspan=2>Failed to verify email address.</td>
	</tr>
	<tr>
		<td class=postheader width="50%">Enter validation key:</td>
		<td class=post width="50%"><asp:textbox id=k runat=server></asp:textbox></td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
<asp:Button id=ValidateKey runat="server" Text="Validate"></asp:Button></td>
	</tr>
</table>

</form>
