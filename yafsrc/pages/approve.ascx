<%@ Control language="c#" Codebehind="approve.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.approve" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=approved runat=server visible=false>
	<tr>
		<td class=header1 colspan="2"><%= GetText("title") %></td>
	</tr>
	<tr>
		<td class=post colspan="2" align=middle><%= GetText("email_verified") %></td>
	</tr>
</table>

<table class=content width="100%" cellspacing=1 cellpadding=0 id=error runat=server visible=false>
	<tr>
		<td class=header1 colspan="2"><%= GetText("title") %></td>
	</tr>
	<tr>
		<td class=header2 colspan="2"><%= GetText("email_verify_failed") %></td>
	</tr>
	<tr>
		<td class=postheader width="50%"><%= GetText("enter_key") %>:</td>
		<td class=post width="50%"><asp:textbox style="width:300px" id="key" runat="server"/></td>
	</tr>
	<tr>
		<td class=postfooter colspan="2" align=middle>
			<asp:Button id=ValidateKey runat="server"/>
		</td>
	</tr>
</table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
