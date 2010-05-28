<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.reindex" Codebehind="reindex.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
	<table cellpadding="0" cellspacing="1" class="content" width="100%">
		<tr>
			<td colspan="2" class="header1">
				YAF DB Operation Report:
			</td>
		</tr>
		<tr class="post">
			<td colspan="2">
				<asp:TextBox ID="txtIndexStatistics" runat="server" Height="400px" TextMode="MultiLine"
					Width="100%"></asp:TextBox>
			</td>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelGetStats" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnGetStats" runat="server" OnClick="btnGetStats_Click" Text="Table Index Statistics"
						Width="200px" />
					<br />
					Show statistical information about YAF table indexes.
				</td>
			</asp:Placeholder>
			<asp:Placeholder ID="PanelRecoveryMode" runat="server" Visible="False">
				<td rowspan="3">
					<asp:Button ID="btnRecoveryMode" runat="server" OnClick="btnRecoveryMode_Click" OnClientClick="return confirm('Are you sure you want to change your database recovery mode?\nThe operation may make the DB inaccessible and may take a little while.\nDO THIS ONLY IF YOU KNOW WHAT YOU ARE DOING!');"
						Text="Set Recovery Mode" Width="200px" />
					<asp:RadioButtonList ID="RadioButtonList1" runat="server">
						<asp:ListItem Selected="True"> Full (Full Recovery allows the database to 
		be recovered to the point of failure.)</asp:ListItem>
						<asp:ListItem> Simple (Simple Recovery allows the database to be 
		recovered to the most recent backup.You need to backup your DB regularly.)</asp:ListItem>
						<asp:ListItem> Bulk-Logged (Bulk-Logged Recovery allows bulk-logged 
		operations.)</asp:ListItem>
					</asp:RadioButtonList>
				</td>
			</asp:Placeholder>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelReindex" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnReindex" runat="server" OnClick="btnReindex_Click" OnClientClick="return confirm('Are you sure you want to reindex all YAF tables?\nThe operation may make the DB inaccessible and may take a little while.');"
						Text="Reindex Tables" Width="200px" />
					<br />
					With any data modification operations, table fragmentation can occur. This command
					can be used to rebuild all the indexes on all the tables in database to boost performance.
				</td>
			</asp:Placeholder>
		</tr>
		<tr class="footer1">
			<asp:Placeholder ID="PanelShrink" runat="server" Visible="False">
				<td valign="top">
					<asp:Button ID="btnShrink" runat="server" OnClick="btnShrink_Click" OnClientClick="return confirm('Are you sure you want to Shrink database?\nThe operation may make the DB inaccessible and may take a little while.');"
						Text="Shrink Database" Width="200px" />
					<br />
					You can use the Shrink method to reduce the size of the files that make up the database
					manually. The data is stored more densely and unused pages are removed
				</td>
			</asp:Placeholder>
		</tr>
	</table>
</YAF:AdminMenu>
