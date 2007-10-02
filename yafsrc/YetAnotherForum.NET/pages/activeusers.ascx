<%@ Control Language="c#" CodeFile="activeusers.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.activeusers" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="6">
			<%= GetText("title") %>
		</td>
	</tr>
	<tr>
		<td class="header2">
			<%= GetText("username") %>
		</td>
		<td class="header2">
			<%= GetText("logged_in") %>
		</td>
		<td class="header2">
			<%= GetText("last_active") %>
		</td>
		<td class="header2">
			<%= GetText("active") %>
		</td>
		<td class="header2">
			<%= GetText("browser") %>
		</td>
		<td class="header2">
			<%= GetText("platform") %>
		</td>
	</tr>
	<asp:Repeater ID="UserList" runat="server">
		<ItemTemplate>
			<tr>
				<td class="post">
					<YAF:UserLink ID="NameLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>' UserName='<%# Eval("Name").ToString() %>' />
				</td>
				<td class="post">
					<%# YafDateTime.FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["Login"]) %>
				</td>
				<td class="post">
					<%# YafDateTime.FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastActive"]) %>
				</td>
				<td class="post">
					<%# String.Format(GetText("minutes"),((System.Data.DataRowView)Container.DataItem)["Active"]) %>
				</td>
				<td class="post">
					<%# Eval("Browser") %>
				</td>
				<td class="post">
					<%# Eval("Platform") %>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
</table>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
