<%@ Control language="c#" Codebehind="eventlog.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.eventlog" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="8">Event Log</td>
</tr>

<asp:repeater runat="server" id="List">
	<HeaderTemplate>
		<tr class="header2">
			<td>User</td>
			<td>Time</td>
			<td>Source</td>
			<td>Description</td>
			<td>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class=post>
			<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Name") %></td>
			<td valign="top"><%# DataBinder.Eval(Container.DataItem, "EventTime") %></td>
			<td valign="top"><%# DataBinder.Eval(Container.DataItem, "Source") %></td>
			<td valign="top"><pre style="overflow:scroll"><%# DataBinder.Eval(Container.DataItem, "Description") %></pre></td>
			<td valign="top">
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "EventLogID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
