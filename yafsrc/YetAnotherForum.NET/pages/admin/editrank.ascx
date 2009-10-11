<%@ Control Language="c#" CodeFile="editrank.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editrank" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="11">
				Edit Rank</td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Name:</b><br />
				Name of this rank.</td>
			<td class="post" colspan="7">
				<asp:TextBox Style="width: 300px" ID="Name" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Is Start:</b><br />
				Means that this is the rank that new users belong to. Only one rank should have
				this checked.</td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsStart" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Is Ladder Group:</b><br />
				If this is checked, this rank should be part of the ladder system where users advance
				as they post messages.</td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsLadder" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Minimum Posts:</b><br />
				Minimum number of posts before users are advanced to this rank.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="MinPosts" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Private Messages:</b><br />
				Max Private Messages allowed to Rank members.</td>
			<td class="post" colspan="6">
				<asp:TextBox ID="PMLimit" Text="0" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<b>Rank Image:</b><br />
				This image will be shown next to users of this rank.</td>
			<td class="post" colspan="7">
				<asp:DropDownList ID="RankImage" runat="server" />
				<img style="vertical-align:middle" runat="server" id="Preview" />
			</td>
		</tr>
		<tr>
			<td class="postfooter" align="middle" colspan="11">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button>&nbsp;
				<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
