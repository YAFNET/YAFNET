<%@ Page language="c#" Codebehind="pmessage.aspx.cs" AutoEventWireup="false" Inherits="yaf.pmessage" %>
<form runat=server>

<p class=navlinks><asp:hyperlink id=HomeLink runat="server"/></p>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=2><%= GetText("title") %></td>
	</tr>
	<tr id=ToRow runat=server>
		<td class=postheader><%= GetText("to") %></td>
		<td class=post><asp:TextBox id=To runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("subject") %></td>
		<td class=post><asp:TextBox id=Subject runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader valign=top><%= GetText("message") %></td>
		<td class=post>
			<asp:TextBox id=Editor runat="server" TextMode="MultiLine" CssClass="posteditor"/>
		</td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server"/>
			<asp:Button id=Cancel runat="server"/>
		</td>
	</tr>
</table>

</form>
