<%@ Control Language="c#" CodeFile="editaccessmask.ascx.cs" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editaccessmask" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr class="header1">
			<td colspan="2">
				Edit Access Mask</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Name:</b><br />
				Name of this access mask.</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Name" CssClass="edit" /><asp:RequiredFieldValidator
					runat="server" Text="<br />Enter name please!" ControlToValidate="Name" Display="Dynamic" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Order:</b><br />
				Sort order for this access mask.</td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="SortOrder" CssClass="edit" /><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
					runat="server" Text="<br />Enter sort order please!" ControlToValidate="SortOrder" Display="Dynamic" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Read Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ReadAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Post Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PostAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Reply Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ReplyAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Priority Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PriorityAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Poll Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="PollAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Vote Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="VoteAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Moderator Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="ModeratorAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Edit Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="EditAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Delete Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="DeleteAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Upload Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="UploadAccess" /></td>
		</tr>
		<tr>
			<td class="postheader">
				<b>Download Access:</b></td>
			<td class="post">
				<asp:CheckBox runat="server" ID="DownloadAccess" /></td>
		</tr>
		<tr class="postfooter">
			<td align="center" colspan="2">
				<asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
				<asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" CausesValidation="false" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
