<%@ Page language="c#" Codebehind="mod_forumuser.aspx.cs" AutoEventWireup="false" Inherits="yaf.mod_forumuser" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content cellspacing=1 cellpadding=0 w_idth=100% align="center">
<tr>
	<td class=header1 colspan="2"><%= GetText("title") %></td>
</tr>
<tr>
	<td class="postheader" width="50%">User:</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="UserName"/><asp:dropdownlist runat="server" id="ToList" visible="false"/> <asp:button runat="server" id="FindUsers" text="Find Users"/></td>
</tr>
<tr>
	<td class="postheader">Access Mask:</td>
	<td class="post"><asp:dropdownlist runat="server" id="AccessMaskID"/></td>
</tr>

<tr class="footer1">
	<td colspan="2" align="center">
		<asp:button runat="server" id="Update" text="Update"/>
		<asp:button runat="server" id="Cancel" text="Cancel"/>
	</td>
</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
