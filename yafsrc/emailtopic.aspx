<%@ Page language="c#" Codebehind="emailtopic.aspx.cs" AutoEventWireup="false" Inherits="yaf.emailtopic" %>

<form runat=server>

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=CategoryLink runat="server">Category</asp:hyperlink>
	&#187; <asp:hyperlink id=ForumLink runat="server">Forum</asp:hyperlink>
	&#187; <asp:hyperlink id=TopicLink runat="server">Topic</asp:hyperlink>
</p>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2>Send a page to a friend</td>
</tr>
<tr>
	<td class=postheader>Send To (Email Address):</td>
	<td class=post><asp:textbox id=EmailAddress runat=server cssclass=edit></asp:textbox></td>
</tr>
<tr>
	<td class=postheader>Subject:</td>
	<td class=post><asp:textbox id=Subject runat=server cssclass=edit></asp:textbox></td>
</tr>
<tr>
	<td class=postheader valign=top>Message:</td>
	<td class=post valign=top><asp:textbox id=Message runat=server cssclass=edit TextMode="MultiLine" Rows="12"></asp:textbox></td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=center><asp:button id=SendEmail runat=server text="Send Email"></asp:button></td>
</tr>
</table>

</form>
