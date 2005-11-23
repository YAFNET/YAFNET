<%@ Control language="c#" Inherits="yaf.pages.printtopic" CodeFile="printtopic.ascx.cs" CodeFileBaseClass="yaf.pages.ForumPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

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