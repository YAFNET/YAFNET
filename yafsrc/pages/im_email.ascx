<%@ Control language="c#" Codebehind="im_email.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.im_email" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

	<table class="content" width="400px" border="0" cellpadding="0" cellspacing="1" align="center">
	<tr class="header2">
		<td colspan="2"><%=GetText("TITLE")%></td>
	</tr>
	<tr>
		<td class="postheader"><%=GetText("SUBJECT")%></td>
		<td class="post"><asp:textbox runat="server" id="Subject"/></td>
	</tr>
	<tr>
		<td class="postheader" valign='top'><%=GetText("BODY")%></td>
		<td class="post">
			<asp:textbox runat="server" id="Body" textmode="multiline" rows='10' style='width:100%'/>
		</td>
	</tr>
	<tr class="postfooter">
		<td colspan="2" align="center">
			<asp:button runat="server" id="Send"/>
		</td>
	</tr>
	</table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
