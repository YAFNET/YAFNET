<%@ Page language="c#" Codebehind="nntpretrieve.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.nntpretrieve" %>

<form runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr class="header1">
	<td colspan="2">Retrieve NNTP Articles</td>
</tr>
<tr>
	<td class="postheader" width="50%">Specify how much time article retrieval should use.</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="Seconds" text="10"/>&nbsp;seconds</td>
</tr>
<tr class="footer1">
	<td colspan="2" align="center"><asp:button runat="server" id="Retrieve" text="Retrieve"/></td>
</tr>
</table>

</form>
