<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions_edit" Codebehind="extensions_edit.ascx.cs" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
		</tr>
        <tr>
	      <td class="header2" style="height:30px" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="FILE_EXTENSION" LocalizedPage="ADMIN_EXTENSIONS_EDIT" />
            </td>
			<td class="post" width="50%">
				<asp:textbox id="extension" runat="server" Width="250"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="save" runat="server" CssClass="pbutton"></asp:button>
				<asp:button id="cancel" runat="server" CssClass="pbutton"></asp:button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
