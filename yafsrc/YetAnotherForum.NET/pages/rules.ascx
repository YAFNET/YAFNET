<%@ Control language="c#" CodeFile="rules.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.rules" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<table class="content" cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class="header1" align="center">Forum Rules and Policies</td>
	</tr>
	<tr>
		<td><asp:Label id=ForumRules runat="server"/></td>
	</tr>
	<tr>
		<td align="center">
			<asp:Button id=Accept runat="server" Text="Accept" onclick="Accept_Click" />
			<asp:Button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click" />
		</td>
	</tr>
</table>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
