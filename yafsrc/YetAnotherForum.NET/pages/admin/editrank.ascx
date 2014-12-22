<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editrank" Codebehind="editrank.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="11">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITRANK" />
             </td>
		</tr>
        <tr>
	      <td class="header2" colspan="11" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="RANK_NAME" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox  Style="width: 250px" ID="Name" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsStart" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="LADDER_GROUP" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="7">
				<asp:CheckBox ID="IsLadder" runat="server"></asp:CheckBox></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MIN_POSTS" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox ID="MinPosts"  Style="width: 250px" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="PRIVATE_MESSAGES" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox ID="PMLimit"  Style="width: 250px" Text="0" runat="server" CssClass="Numeric" /></td>
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="RANK_DESC" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="Description" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SIG_LENGTH" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="UsrSigChars" runat="server" CssClass="Numeric" /></td>
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODE" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="UsrSigBBCodes" runat="server" /></td>
		
		</tr>
			<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="UsrSigHTMLTags" runat="server" /></td>
		</tr>				
			<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="ALBUMS_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="UsrAlbums" Text="0" runat="server" CssClass="Numeric" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox Style="width: 250px" ID="UsrAlbumImages" Text="0" runat="server" CssClass="Numeric" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="RANK_PRIO" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox ID="RankPriority" Style="width: 250px" Text="0" runat="server" CssClass="Numeric" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="RANK_STYLE" LocalizedPage="ADMIN_EDITRANK" />
            </td>
			<td class="post" colspan="6">
				<asp:TextBox ID="Style" Text="" Style="width: 250px" TextMode="MultiLine" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="RANK_IMAGE" LocalizedPage="ADMIN_EDITRANK" />
			</td>
			<td class="post" colspan="7">
				<asp:DropDownList Style="width: 250px" ID="RankImage" runat="server" CssClass="standardSelectMenu" />
				<img style="vertical-align:middle" src="" alt="Rank Image" runat="server" id="Preview" />
			</td>
		</tr>
		<tr>
			<td class="footer1"  align="center" colspan="11">
				<asp:Button ID="Save" runat="server" OnClick="Save_Click" CssClass="pbutton"></asp:Button>&nbsp;
				<asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CssClass="pbutton"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
