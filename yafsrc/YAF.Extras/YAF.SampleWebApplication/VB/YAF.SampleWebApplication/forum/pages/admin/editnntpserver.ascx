<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editnntpserver" Codebehind="editnntpserver.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="11">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPSERVER" />
             </td>
		</tr>
        <tr>
			    <td class="header2" height="30" colspan="11"></td>
		</tr>
        <tr>
			<td class="postheader" colspan="4">
                <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="NNTP_NAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox Width="250" ID="Name" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NNTP_ADRESS" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox Width="250" ID="Address" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NNTP_PORT" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox Width="250" ID="Port" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NNTP_USERNAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox Width="250" ID="UserName" runat="server" Enabled="true" /></td>
		</tr>
		<tr>
			<td class="postheader" colspan="4">
				<YAF:HelpLabel ID="LocalizedLabel6" runat="server" LocalizedTag="NNTP_PASSWORD" LocalizedPage="ADMIN_EDITNNTPSERVER" />
            </td>
			<td class="post" colspan="7">
				<asp:TextBox Width="250" ID="UserPass" runat="server" Enabled="true" /></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="11">
				<asp:Button ID="Save" runat="server" CssClass="pbutton" OnClick="Save_Click"></asp:Button>&nbsp;
				<asp:Button ID="Cancel" runat="server" CssClass="pbutton" OnClick="Cancel_Click"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
