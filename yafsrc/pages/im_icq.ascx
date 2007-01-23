<%@ Control language="c#" Codebehind="im_icq.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.im_icq" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

	<table class="content" width="400px" border="0" cellpadding="0" cellspacing="1" align="center">
	<tr class="header2">
		<td colspan="2"><img runat="server" id="Status" align="absmiddle"/><%=GetText("TITLE")%></td>
	</tr>
	<tr>
		<td class="postheader"><%=GetText("NAME")%></td>
		<td class="post">
			<asp:textbox runat="server" id="From" size="15" maxlength="40" style="width:100%" enabled="false"/>
		</td>
	</tr>
	<tr>
		<td class="postheader"><%=GetText("EMAIL")%></td>
		<td class="post">
			<asp:textbox runat="server" id="Email" size="15" maxlength="50" style="width:100%" enabled="false"/>
		</td>
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

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
