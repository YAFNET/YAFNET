<%@ Page language="c#" Codebehind="pmessage.aspx.cs" AutoEventWireup="false" Inherits="yaf.pmessage" %>
<form runat=server>

<p class=navlinks><asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink></p>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=2>Post a Private Message</td>
	</tr>
	<tr id=ToRow runat=server>
		<td class=postheader>To:</td>
		<td class=post>
<asp:TextBox id=To runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td class=postheader>Subject:</td>
		<td class=post><asp:TextBox id=Subject runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td class=postheader valign=top>Message:</td>
		<td class=post>
<asp:TextBox id=Editor runat="server" TextMode="MultiLine" CssClass="posteditor"></asp:TextBox></td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
<asp:Button id=Save runat="server" Text="Save"></asp:Button>&nbsp;
<asp:Button id=Cancel runat="server" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>

</form>
