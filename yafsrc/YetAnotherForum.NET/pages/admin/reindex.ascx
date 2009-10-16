<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reindex.ascx.cs" Inherits="YAF.Pages.Admin.reindex" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu ID="AdminMenu1" runat="server">
    <table cellpadding="0" cellspacing="1" class="content" width="100%">
		<tr>
			<td colspan="2" class="header1" >YAF DB operation report:</td>
		</tr>
		<tr class="post">
			<td colspan="2">
			<asp:TextBox ID="txtIndexStatistics" runat="server" Height="400px" TextMode="MultiLine" Width="100%"></asp:TextBox>
			</td>
		</tr>
		<tr class="footer1">
			<td valign="top">
			<asp:Button id="btnGetStats" runat="server" onclick="btnGetStats_Click" text="Table Index Statistics" Width="200px" />
			<br />
			Show statistical information about YAF table indexes.</td>
			<td rowspan="3">
			<asp:Button id="btnRecoveryMode" runat="server" onclick="btnRecoveryMode_Click" onclientclick="return confirm('Are you sure you want to change your database recovery mode?\nThe operation may make the DB inaccessible and may take a little while.\nDO THIS ONLY IF YOU KNOW WHAT YOU ARE DOING!');" text="Set Recovery Mode" Width="200px" />
			<asp:RadioButtonList id="RadioButtonList1" runat="server">
		<asp:listitem Selected="True">Full (Full Recovery allows the database to 
		be recovered to the point of failure.)</asp:listitem>
		<asp:listitem>Simple (Simple Recovery allows the database to be 
		recovered to the most recent backup.You need to backup your DB regularly.)</asp:listitem>
		<asp:listitem>Bulk-Logged (Bulk-Logged Recovery allows bulk-logged 
		operations.)</asp:listitem>
			</asp:RadioButtonList>
			</td>
		</tr>
		<tr class="footer1">
			<td valign="top">
			<asp:Button id="btnReindex" runat="server" onclick="btnReindex_Click" onclientclick="return confirm('Are you sure you want to reindex all YAF tables?\nThe operation may make the DB inaccessible and may take a little while.');" text="Reindex Tables" Width="200px" />
			<br />
			With any data modification operations, table fragmentation can 
			occur. This command can be used to rebuild all the indexes on all 
			the tables in database to boost performance.</td>
		</tr>
		<tr class="footer1">
			<td valign="top">
			<asp:Button id="btnShrink" runat="server" onclick="btnShrink_Click" onclientclick="return confirm('Are you sure you want to Shrink database?\nThe operation may make the DB inaccessible and may take a little while.');" text="Shrink Database" Width="200px" />
			<br />
			You can use the Shrink method to reduce the size of the files that 
			make up the database manually. The data is stored more densely and 
			unused pages are removed
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
