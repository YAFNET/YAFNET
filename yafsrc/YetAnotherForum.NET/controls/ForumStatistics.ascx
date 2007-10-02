<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" CodeFile="ForumStatistics.ascx.cs"
	Inherits="YAF.Controls.ForumStatistics" %>
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="2">
			<asp:ImageButton runat="server" ID="expandInformation" BorderWidth="0" ImageAlign="Baseline"
				OnClick="expandInformation_Click" />&nbsp;&nbsp;<%# PageContext.Localization.GetText("INFORMATION") %></td>
	</tr>
	<tbody id="InformationTBody" runat="server">
		<tr>
			<td class="header2" colspan="2">
				<%# PageContext.Localization.GetText( "ACTIVE_USERS" )%>
			</td>
		</tr>
		<tr>
			<td class="post" width="1%">
				<img src="<%# PageContext.Theme.GetItem("ICONS","FORUM_USERS") %>" alt="" /></td>
			<td class="post">
				<asp:Label runat="server" ID="activeinfo" /><br />
				<asp:Repeater runat="server" ID="ActiveList">
					<ItemTemplate>
						<YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>' UserName='<%# Eval("Name").ToString() %>' />
					</ItemTemplate>
					<SeparatorTemplate>
						,
					</SeparatorTemplate>
				</asp:Repeater>
			</td>
		</tr>
		<tr>
			<td class="header2" colspan="2">
				<%# PageContext.Localization.GetText( "STATS" )%>
			</td>
		</tr>
		<tr>
			<td class="post" width="1%">
				<img src="<%# PageContext.Theme.GetItem("ICONS","FORUM_STATS") %>" alt="" /></td>
			<td class="post">
				<asp:Label ID="Stats" runat="server">Label</asp:Label></td>
		</tr>
	</tbody>
</table>
