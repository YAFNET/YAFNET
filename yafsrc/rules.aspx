<%@ Page language="c#" Codebehind="rules.aspx.cs" AutoEventWireup="false" Inherits="yaf.rules" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"></asp:hyperlink>
</p>

<table class="content" cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class="header1" align="middle">Forum Rules and Policies</td>
	</tr>
	<tr>
		<td>
<asp:Label id=ForumRules runat="server">Label</asp:Label></td>
	</tr>
	<tr>
		<td align="middle">
<asp:Button id=Accept runat="server" Text="Accept"></asp:Button>&nbsp;
<asp:Button id=Cancel runat="server" Text="Cancel"></asp:Button></td>
	</tr>
</table>

</form>
