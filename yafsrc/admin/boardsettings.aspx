<%@ Page language="c#" Codebehind="boardsettings.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.boardsettings" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<table class=content cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td colspan=2 class=header1>Forum Settings</td>
	</tr>
	<tr>
		<td class="header2" colspan="2">Forum Setup</td>
	</tr>
	<tr>
		<td class=postheader width="50%"><b>Forum Name:</b><br>The name of the forum.</td>
		<td class=post width="50%"><asp:textbox style="width:300px" id=Name runat=server></asp:textbox></td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server" Text="Save"/></td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
