<%@ Control language="c#" Codebehind="activeusers.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.activeusers" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6><%= GetText("title") %></td>
</tr>
<tr>
	<td class=header2><%= GetText("username") %></td>
	<td class=header2><%= GetText("logged_in") %></td>
	<td class=header2><%= GetText("last_active") %></td>
	<td class=header2><%= GetText("active") %></td>
	<td class=header2><%= GetText("browser") %></td>
	<td class=header2><%= GetText("platform") %></td>
</tr>

<asp:repeater id=UserList runat=server>
<ItemTemplate>
<tr>
	<td class=post><asp:HyperLink id="Name" NavigateUrl='<%# YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.profile,"u={0}",Eval("UserID")) %>' Text='<%# Server.HtmlEncode(Convert.ToString(Eval("Name"))) %>' runat="server" /></td>
	<td class=post><%# yaf_DateTime.FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["Login"]) %></td>
	<td class=post><%# yaf_DateTime.FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastActive"]) %></td>
	<td class=post><%# String.Format(GetText("minutes"),((System.Data.DataRowView)Container.DataItem)["Active"]) %></td>
	<td class=post><%# Eval("Browser") %></td>
	<td class=post><%# Eval("Platform") %></td>
</tr>
</ItemTemplate>
</asp:repeater>

</table>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
