<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.rules" Codebehind="rules.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="0" cellpadding="0" width="100%">
	<tr>
		<td class="header1" align="center">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
		</td>
	</tr>
	<tr>
		<td>
			<YAF:LocalizedLabel runat="server" LocalizedTag="RULES_TEXT" EnableBBCode="true" />
		</td>
	</tr>
	<tr>
		<td align="center">
			<asp:Button ID="Accept" runat="server" Text="Accept" CssClass="pbutton" OnClick="Accept_Click" />
			<asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="pbutton" OnClick="Cancel_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
