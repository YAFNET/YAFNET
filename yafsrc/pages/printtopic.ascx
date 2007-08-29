<%@ Control language="c#" CodeFile="printtopic.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.printtopic" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

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