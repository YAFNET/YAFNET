<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bbcode_import" Codebehind="bbcode_import.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BBCODE_IMPORT" />
            </td>
		</tr>
        <tr>
	      <td class="header2" colspan="2" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_EXTENSIONS_IMPORT" />
            </td>
			<td class="post" width="50%">
			    <input type="file" id="importFile" class="pbutton" runat="server" style="width:250px" />
	        </td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="Import" runat="server" OnClick="Import_OnClick" CssClass="pbutton"></asp:button>
				<asp:button id="cancel" runat="server" OnClick="Cancel_OnClick" CssClass="pbutton"></asp:button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />