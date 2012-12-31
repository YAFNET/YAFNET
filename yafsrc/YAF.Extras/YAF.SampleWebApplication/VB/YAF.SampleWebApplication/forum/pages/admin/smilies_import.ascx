<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies_import" Codebehind="smilies_import.ascx.cs" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class="content" width="100%" cellspacing="1" cellpadding="0">
<tr>
	<td class="header1" colspan="2"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_SMILIES_IMPORT" /></td>
</tr>
<tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
<tr>
	<td class="postheader" width="50%"><YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="CHOOSE_PAK" LocalizedPage="ADMIN_SMILIES_IMPORT" /></td>
	<td class="post" width="50"><asp:dropdownlist id="File" runat="server" Width="250"/></td>
</tr>
<tr>
	<td class="postheader" width="50%"><YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE_EXISTING" LocalizedPage="ADMIN_SMILIES_IMPORT" /></td>
	<td class="post" width="50%"><asp:checkbox id="DeleteExisting" runat="server"/></td>
</tr>
<tr>
	<td class="footer1" colspan="2" align="center">
		<asp:button id="import" runat="server"  CssClass="pbutton"/>
		<asp:button id="cancel" runat="server"  CssClass="pbutton"/>
	</td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
