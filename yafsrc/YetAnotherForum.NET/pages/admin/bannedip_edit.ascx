<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.bannedip_edit" Codebehind="bannedip_edit.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDIP_EDIT" />
             </td>
		</tr>
        <tr>
	      <td class="header2" colspan="2" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_EDITACCESSMASKS" />
            </td>
			<td class="post" width="50%">
				<asp:TextBox Style="width: 250px" ID="mask" runat="server"></asp:TextBox></td>
		</tr>
			<tr>
			<td class="postheader" width="50%">
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_EDITACCESSMASKS" />
            </td>
			<td class="post" width="50%">
				<asp:TextBox Style="width: 250px" ID="BanReason" runat="server"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="save" runat="server" OnClick="Save_Click" CssClass="pbutton"></asp:Button>
				<asp:Button ID="cancel" runat="server" OnClick="Cancel_Click" CssClass="pbutton"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
