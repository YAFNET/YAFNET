<%@ Page language="c#" Codebehind="printtopic.aspx.cs" AutoEventWireup="false" Inherits="yaf.printtopic" %>

<p class="navlinks">
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=CategoryLink runat="server"/>
	&#187; <asp:hyperlink id=ForumLink runat="server"/>
	&#187; <asp:hyperlink id=TopicLink runat="server"/>
</p>

<asp:repeater id=Posts runat=server>
<ItemTemplate>
<table class=print width=100% cellspacing=0 cellpadding=0>
<tr>
	<td class=printheader><%# GetPrintHeader(Container.DataItem) %></td>
</tr>
<tr>
	<td class=printbody><%# GetPrintBody(Container.DataItem) %></td>
</tr>
</table>
</ItemTemplate>
<SeparatorTemplate>
<br/>
</SeparatorTemplate>
</asp:repeater>