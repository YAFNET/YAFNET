<%@ Page language="c#" Codebehind="cp_message.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp.cp_message" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<yaf:PageLinks runat="server" id="PageLinks"/>

<asp:repeater id=Inbox runat=server>
<HeaderTemplate>
	<table class=content cellspacing=1 cellpadding=0 width=100%>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<SeparatorTemplate>
	<tr class="postsep"><td colspan="2" style="height:7px"></td></tr>
</SeparatorTemplate>
<ItemTemplate>
	<tr>
		<td class=header1 colspan=2><%# DataBinder.Eval(Container.DataItem,"Subject") %></td>
	</tr>
	<tr>
		<td class=postheader><%# DataBinder.Eval(Container.DataItem,"FromUser") %></td>
		<td class=postheader>
			<table cellspacing=0 cellpadding=0 width=100%><tr>
			<td>
				<b><%# GetText("posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %>
			</td>
			<td align=right>
				<asp:linkbutton tooltip="Delete this message" runat=server commandname=delete commandargument='<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'><%# GetThemeContents("BUTTONS","DELETEPOST") %></asp:linkbutton>
				<asp:linkbutton tooltip="Reply to this message" runat=server commandname=reply commandargument='<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'><%# GetThemeContents("BUTTONS","QUOTEPOST") %></asp:linkbutton>
			</td>
			</tr></table>
		</td>
	</tr>
	<tr>
		<td class=post>&nbsp;</td>
		<td class=post valign=top><%# FormatBody(Container.DataItem) %></td>
	</tr>
</ItemTemplate>
</asp:repeater>

<yaf:savescrollpos runat="server"/>
</form>
