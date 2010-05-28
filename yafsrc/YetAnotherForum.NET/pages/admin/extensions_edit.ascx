<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions_edit" Codebehind="extensions_edit.ascx.cs" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">Add/Edit Allowed File Extensions</td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><b>File Extension:</b><br/>(Example: Enter "jpg" for a JPEG graphic file.)</td>
			<td class="post" width="50%">
				<asp:textbox id="extension" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
				<asp:button id="save" runat="server" text="Save"></asp:button>
				<asp:button id="cancel" runat="server" text="Cancel"></asp:button></td>
		</tr>
	</table>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
