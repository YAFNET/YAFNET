<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editrank" Codebehind="editrank.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="11">
				Edit Rank</td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Name:</strong><br />
				Name of this rank.</td>
			<td class="post" colspan="7">
				<asp:TextBox Style="width: 300px" ID="Name" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Is Start:</strong><br />
				Means that this is the rank that new users belong to. Only one rank should have
				this checked.</td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsStart" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Is Ladder Group:</strong><br />
				If this is checked, this rank should be part of the ladder system where users advance
				as they post messages.</td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsLadder" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Minimum Posts:</strong><br />
				Minimum number of posts before users are advanced to this rank.</td>
			<td class="post" colspan="7">
				<asp:TextBox ID="MinPosts" Style="width: 80px" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Private Messages:</strong><br />
				Max Private Messages allowed to Rank members.</td>
			<td class="post" colspan="6">
				<asp:TextBox ID="PMLimit" Style="width: 80px" Text="0" runat="server" /></td>
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<strong>Description:</strong><br />
				Enter here a role description.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 300px" ID="Description" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Max number of chars in a user signature:</strong><br />
				Max number of chars in a user signature in the rank.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 80px" ID="UsrSigChars" runat="server" /></td>
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<strong>User signature BBCodes:</strong><br />
				Comma separated  BBCodes allowed in a user signature in the rank.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 300px" ID="UsrSigBBCodes" runat="server" /></td>
		
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<strong>User signature HTML tags:</strong><br />
				Comma separated HTML tags allowed in a user signature in the group.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 300px" ID="UsrSigHTMLTags" runat="server" /></td>
		</tr>				
			<tr>
			<td class="postheader" colspan="4">
				<strong>User Albums Number:</strong><br />
				Integer value for a user allowed albums number.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 80px" ID="UsrAlbums" runat="server" /></td>
		</tr>
			</tr>
				</tr>
			<tr>
			<td class="postheader" colspan="4">
				<strong>Total Album Images Number:</strong><br />
				Integer value for a user allowed images number in ALL albums.</td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 80px" ID="UsrAlbumImages" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Priority:</strong><br />
				Priority of rank is various things.</td>
			<td class="post" colspan="6">
				<asp:TextBox ID="RankPriority" Style="width: 80px" Text="0" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Style:</strong><br />
				Style of users links in active users, colore, font size...</td>
			<td class="post" colspan="6">
				<asp:TextBox ID="Style" Text="" Style="width: 100%" TextMode="MultiLine" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<strong>Rank Image:</strong><br />
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
