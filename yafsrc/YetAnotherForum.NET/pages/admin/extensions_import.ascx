<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions_import" Codebehind="extensions_import.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">Import Extension List</td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><b>Select Import File:</b><br/>(Must be *.xml file)</td>
			
			
			<td class="post" width="50%">
			<input type="file" id="importFile" class="pbutton" runat="server" />
			</td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="Import" runat="server" text="Import" OnClick="Import_OnClick"></asp:button>
			
			
				<asp:button id="cancel" runat="server" text="Cancel" OnClick="Cancel_OnClick"></asp:button></td>
			
			
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
