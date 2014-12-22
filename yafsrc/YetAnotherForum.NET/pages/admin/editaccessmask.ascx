<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editaccessmask" Codebehind="editaccessmask.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITACCESSMASKS" />
            </td>
		</tr>
        <tr>
	      <td class="header2" colspan="2" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="MASK_NAME" LocalizedPage="ADMIN_EDITACCESSMASKS" />
			</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Name" CssClass="edit" style="width:250px" /><asp:RequiredFieldValidator
					runat="server" Text="<br />Enter name please!" ControlToValidate="Name" Display="Dynamic" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="MASK_ORDER" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                <strong></strong><br />
				</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="SortOrder" MaxLength="5" style="width:250px" CssClass="Numeric" /><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
					runat="server" Text="<br />Enter sort order please!" ControlToValidate="SortOrder" Display="Dynamic" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="READ_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ReadAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="POST_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PostAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="REPLY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ReplyAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="PRIORITY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PriorityAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="POLL_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PollAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="VOTE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="VoteAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="MODERATOR_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ModeratorAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="EDIT_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="EditAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="DELETE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="DeleteAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="UPLOAD_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="UploadAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="DOWNLOAD_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" Suffix=":" />
            </td>
			<td class="post">
				<asp:CheckBox runat="server" ID="DownloadAccess" /></td>
		</tr>
		<tr class="footer1">
			<td align="center" colspan="2">
				<asp:Button ID="Save" runat="server" OnClick="Save_Click" CssClass="pbutton" />
				<asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="pbutton" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
