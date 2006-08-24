<%@ Control language="c#" Codebehind="editboard.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.editboard" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class=header1 colspan="2">Edit Board</td>
</tr>
<tr>
    <td width="50%" class="postheader"><b>Name:</b><br />The name of the board.</td>
    <td width="50%" class="post"><asp:textbox id=Name runat="server" style="width:100%"></asp:textbox></td>
</tr>
	<tr>
		<td class="postheader"><b>Allow Threaded:</b><br/>Allow threaded view for posts.</td>
		<td class="post"><asp:checkbox runat="server" id="AllowThreaded"/></td>
	</tr>
<asp:placeholder runat="server" id="AdminInfo">
<tr>
	<td class="postheader"><b>User Name:</b><br/>This will be the administrator for the board.</td>
	<td class="post"><asp:textbox runat="server" id="UserName"/></td>
</tr>
<tr>
	<td class="postheader"><b>User Email:</b><br/>Email address for administrator.</td>
	<td class="post"><asp:textbox runat="server" id="UserEmail"/></td>
</tr>
<tr>
	<td class="postheader"><b>Password:</b><br/>Enter password for administrator here.</td>
	<td class="post"><asp:textbox runat="server" id="UserPass1" textmode="password"/></td>
</tr>
<tr>
	<td class="postheader"><b>Verify Password:</b><br/>Verify the password.</td>
	<td class="post"><asp:textbox runat="server" id="UserPass2" textmode="password"/></td>
</tr>
</asp:placeholder>
<tr>
	<td class=postfooter align=middle colSpan="2">
		<asp:button id=Save runat="server" Text="Save" onclick="Save_Click" />
		<asp:Button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click" />
	</td>
</tr>
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
