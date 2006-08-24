<%@ Control language="c#" Codebehind="editnntpserver.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.editnntpserver" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
	<tr>
		<td class=header1 colspan=11>Edit NNTP Server</td>
	</tr>
  <tr>
    <td class=postheader colspan=4><b>Name:</b><br/>Name of this server.</td>
    <td class=post colspan=7><asp:textbox style="width:300px" id=Name runat="server"/></td></tr>
 
  <tr>
    <td class=postheader colspan=4><b>Address:</b><br/>The host name of the server.</td>
    <td class=post colspan=7><asp:textbox id="Address" runat="server"/></td></tr>

	<tr>
		<td class=postheader colspan=4><b>Port:</b><br/>The port number to connect to.</td>
		<td class=post colspan=7><asp:textbox id="Port" runat="server"/></td>
	</tr>
    
  <tr>
    <td class=postheader colspan=4><b>User Name:</b><br/>The user name used to log on to the nntp server.</td>
    <td class=post colspan=7><asp:textbox id="UserName" runat="server" enabled="false"/></td></tr>

	<tr>
		<td class=postheader colspan=4><b>Password:</b><br/>The password used to log on to the nntp server.</td>
		<td class=post colspan=7><asp:textbox id="UserPass" runat="server" enabled="false"/></td>
	</tr>
  <tr>
    <td class=postfooter align=middle colspan=11><asp:button id=Save runat="server" Text="Save" onclick="Save_Click"></asp:button>&nbsp; 
<asp:button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click"></asp:button></td></tr></table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
