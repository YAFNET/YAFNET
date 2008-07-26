<%@ Control Language="c#" CodeFile="activeusers.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.activeusers" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="6">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
		</td>
	</tr>
	<tr>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="username" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="logged_in" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="last_active" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="active" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="browser" />
		</td>
		<td class="header2">
			<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="platform" />
		</td>
	</tr>
	<asp:Repeater ID="UserList" runat="server">
		<ItemTemplate>
			<tr>
				<td class="post">
					<YAF:UserLink ID="NameLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>'
						UserName='<%# Eval("UserName").ToString() %>' />
				    <asp:PlaceHolder ID="HiddenPlaceHolder" runat="server" Visible=<%# Convert.ToBoolean(Eval("IsHidden"))  %>>
				    (<YAF:LocalizedLabel ID="Hidden" LocalizedTag="HIDDEN" runat="server" />)
				    </asp:PlaceHolder>				    
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
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
