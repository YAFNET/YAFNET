<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.bannedip_import" Codebehind="bannedip_import.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
              <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_BANNEDIP_IMPORT" />
            </td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
              <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="IMPORT_FILE" LocalizedPage="ADMIN_BANNEDIP_IMPORT" />
            </td>
			<td class="post" width="50%">
			  <input type="file" id="importFile" class="pbutton" runat="server" style="width:250px" />
			</td>
		</tr>
		<tr>
			<td class="postfooter" style="text-align:center" colspan="2">
				<asp:button id="Import" runat="server" CssClass="pbutton" OnClick="Import_OnClick"></asp:button>
				<asp:button id="cancel" runat="server" CssClass="pbutton" OnClick="Cancel_OnClick"></asp:button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
