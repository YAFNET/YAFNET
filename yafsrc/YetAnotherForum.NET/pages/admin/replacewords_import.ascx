<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.replacewords_import" Codebehind="replacewords_import.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" /></td>
		</tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SELECT_IMPORT" LocalizedPage="ADMIN_REPLACEWORDS_IMPORT" />
            </td>
			<td class="post" width="50%">
			<input type="file" id="importFile" class="pbutton" runat="server" />
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
