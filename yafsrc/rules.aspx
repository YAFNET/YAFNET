<%@ Page language="c#" Codebehind="rules.aspx.cs" AutoEventWireup="false" Inherits="yaf.rules" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" cellspacing=0 cellpadding=0 width="100%">
	<tr>
		<td class="header1" align="middle">Forum Rules and Policies</td>
	</tr>
	<tr>
		<td><asp:Label id=ForumRules runat="server"/></td>
	</tr>
	<tr>
		<td align="middle">
			<asp:Button id=Accept runat="server" Text="Accept"/>
			<asp:Button id=Cancel runat="server" Text="Cancel"/>
		</td>
	</tr>
</table>

</form>
