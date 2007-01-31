<%@ Control language="c#" Codebehind="cp_message.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.cp_message" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

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
		<td class=header1 colspan="2"><%# HtmlEncode(DataBinder.Eval(Container.DataItem,"Subject")) %></td>
	</tr>
	<tr>
		<td class=postheader><%# Eval("FromUser") %></td>
		<td class=postheader>
			<table cellspacing=0 cellpadding=0 width=100%><tr>
			<td>
				<b><%# GetText("posted") %></b> <%# yaf_DateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %>
			</td>
			<td align=right>
				<asp:linkbutton id="DeleteMessage" onload="DeleteMessage_Load" tooltip="Delete this message" runat=server commandname=delete commandargument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","DELETEPOST") %></asp:linkbutton>
				<asp:linkbutton id="ReplyMessage" tooltip="Reply to this message" runat=server commandname=reply commandargument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","REPLYPM") %></asp:linkbutton>
				<asp:LinkButton id="QuoteMessage" tooltip="Reply with quote" Runat=server CommandName=quote CommandArgument='<%# Eval("UserPMessageID") %>'><%# GetThemeContents("BUTTONS","QUOTEPOST") %></asp:LinkButton>
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

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
