<%@ Page language="c#" Codebehind="movetopic.aspx.cs" AutoEventWireup="false" Inherits="yaf.movetopic" %>

<form runat=server>

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	» <asp:hyperlink id=CategoryLink runat="server">Category</asp:hyperlink>
	» <asp:hyperlink id=ForumLink runat="server">Forum</asp:hyperlink>
	» <asp:hyperlink id=TopicLink runat="server">Topic</asp:hyperlink>
</p>

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2>Move Topic</td>
</tr>
<tr>
	<td class=postheader width="50%">Select the forum you want to move the post to:</td>
	<td class=post width="50%">
<asp:DropDownList id=ForumList runat="server" DataValueField="ForumID" DataTextField="Forum"></asp:DropDownList></td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=middle>
<asp:Button id=Move runat="server" Text="Move Topic"></asp:Button></td>
</tr>
</table>

</form>
