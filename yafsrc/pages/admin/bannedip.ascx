<%@ Control language="c#" Codebehind="bannedip.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.bannedip" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<asp:repeater id=list runat=server>
<HeaderTemplate>
	<table class=content cellspacing=1 cellpadding=0 width=100%>
	<tr>
		<td class=header1 colspan=3>Banned IP Addresses</td>
	</tr>
	<tr>
		<td class=header2>Mask</td>
		<td class=header2>Since</td>
		<td class=header2>&nbsp;</td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class=post><%# Eval("Mask") %></td>
		<td class=post><%# FormatDateTime(Eval("Since")) %></td>
		<td class=post>
			<asp:linkbutton runat=server text=Edit commandname='edit' commandargument='<%# Eval("ID") %>'></asp:linkbutton>
			| <asp:linkbutton runat=server text=Delete commandname='delete' commandargument='<%# Eval("ID") %>'></asp:linkbutton>
		</td>
	</tr>
</ItemTemplate>
<FooterTemplate>
	<tr>
		<td class=footer1 colspan=3><asp:linkbutton runat=server text=Add commandname='add'></asp:linkbutton></td>
	</tr>
	</table>
</FooterTemplate>
</asp:repeater>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
