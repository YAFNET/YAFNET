<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.reindex" Codebehind="reindex.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
	<table cellpadding="0" cellspacing="1" class="content" width="100%">
		<tr>
			<td colspan="2" class="header1">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REINDEX" />
			</td>
		</tr>
        <tr>
			    <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr class="post">
			<td colspan="2">
				<asp:TextBox ID="txtIndexStatistics" runat="server" Height="400px" TextMode="MultiLine"
					Width="99%"></asp:TextBox>
			</td>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelGetStats" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnGetStats" CssClass="pbutton" runat="server" OnClick="btnGetStats_Click" Text="Table Index Statistics"
						Width="200px" />
					<br />
					<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SHOW_STATS" LocalizedPage="ADMIN_REINDEX" />
				</td>
			</asp:Placeholder>
			<asp:Placeholder ID="PanelRecoveryMode" runat="server" Visible="False">
				<td rowspan="3">
					<asp:Button ID="btnRecoveryMode" CssClass="pbutton" runat="server" OnClick="btnRecoveryMode_Click" Text="Set Recovery Mode" Width="200px" />
					<asp:RadioButtonList ID="RadioButtonList1" runat="server">
					</asp:RadioButtonList>
				</td>
			</asp:Placeholder>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelReindex" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnReindex" CssClass="pbutton" runat="server" OnClick="btnReindex_Click" Text="Reindex Tables" Width="200px" />
					<br />
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="REINDEX" LocalizedPage="ADMIN_REINDEX" />
				</td>
			</asp:Placeholder>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelShrink" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnShrink" CssClass="pbutton" runat="server" OnClick="btnShrink_Click" Text="Shrink Database" Width="200px" />
					<br />
					
                    <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="SHRINK" LocalizedPage="ADMIN_REINDEX" />
				</td>
			</asp:Placeholder>
		</tr>
	</table>
</YAF:AdminMenu>
