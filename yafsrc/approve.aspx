<%@ Page language="c#" Codebehind="approve.aspx.cs" AutoEventWireup="false" Inherits="yaf.approve" %>

<form runat=server method=get>

<p class=navlinks><asp:hyperlink runat="server" id="HomeLink"/></p>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=approved runat=server visible=false>
	<tr>
		<td class=header1 colspan=2><%= GetText("approve_title") %></td>
	</tr>
	<tr>
		<td class=post colspan=2 align=middle><%= GetText("approve_email_verified") %></td>
	</tr>
</table>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=error runat=server visible=false>
	<tr>
		<td class=header1 colspan=2><%= GetText("approve_title") %></td>
	</tr>
	<tr>
		<td class=header2 colspan=2><%= GetText("approve_email_verify_failed") %></td>
	</tr>
	<tr>
		<td class=postheader width="50%"><%= GetText("approve_enter_key") %>:</td>
		<td class=post width="50%"><asp:textbox id=k runat="server"/></td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=ValidateKey runat="server"/>
		</td>
	</tr>
</table>

</form>
