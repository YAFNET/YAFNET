<%@ Control language="c#" Codebehind="eventlog.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.eventlog" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="8">Event Log</td>
</tr>

<asp:repeater runat="server" id="List">
	<HeaderTemplate>
		<tr class="header2">
			<td width="1%">Type</td>
			<td>User</td>
			<td>Time</td>
			<td>Source</td>
			<td>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class=postheader>
			<td align="center"><%# EventImageCode(Container.DataItem) %></td>
			<td><%# Eval( "Name") %></td>
			<td><%# Eval( "EventTime") %></td>
			<td><%# Eval( "Source") %></td>
			<td>
				<asp:linkbutton runat="server" id="showbutton" commandname="show">Show</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "EventLogID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
		<tr class="post" runat="server" visible="false" id="details">
			<td colspan="5"><pre style="overflow:scroll"><%# Eval( "Description") %></pre></td>
		</tr>
	</ItemTemplate>
</asp:repeater>

</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
