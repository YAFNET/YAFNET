<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.EditUsersInfo" Codebehind="EditUsersInfo.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">
			User Details
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>Username:</strong>
			<br />
			Cannot be modified.
		</td>
		<td class="post">
			<asp:TextBox Style="width: 300px" ID="Name" runat="server" Enabled="false" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>E-mail:</strong>
		</td>
		<td class="post">
			<asp:TextBox Style="width: 300px" ID="Email" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>Rank:</strong>
		</td>
		<td class="post">
			<asp:DropDownList ID="RankID" runat="server" />
		</td>
	</tr>
	<tr runat="server" id="IsHostAdminRow">
		<td class="postheader">
			<strong>Host Admin:</strong>
			<br />
			Gives user access to modify "Host Settings" section.
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="IsHostAdminX" />
		</td>
	</tr>
	<tr runat="server" id="IsCaptchaExcludedRow">
		<td class="postheader">
			<strong>Exclude from CAPTCHA:</strong><br />
			CAPTCHA is disabled for this user specifically.
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="IsCaptchaExcluded" />
		</td>
	</tr>
	<tr runat="server" id="IsExcludedFromActiveUsersRow">
		<td class="postheader">
			<strong>Exclude from Active Users:</strong><br />
			User is not shown in Active User lists.
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="IsExcludedFromActiveUsers" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>Is Approved:</strong>
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="IsApproved" />
		</td>
	</tr>
	<!-- Easy to enable it if there is major issues (i.e. Guest being deleted). -->
	<tr runat="server" id="IsGuestRow" visible="false">
		<td class="postheader">
			<strong>Is Guest:</strong>
		</td>
		<td class="post">
			<asp:CheckBox runat="server" ID="IsGuestX" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>Joined:</strong>
		</td>
		<td class="post">
			<asp:TextBox ID="Joined" runat="server" Enabled="False" />
		</td>
	</tr>
	<tr>
		<td class="postheader">
			<strong>Last Visit:</strong>
		</td>
		<td class="post">
			<asp:TextBox ID="LastVisit" runat="server" Enabled="False" />
		</td>
	</tr>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button ID="Save" runat="server" Text="Save" CssClass="pbutton" OnClick="Save_Click" />
		</td>
	</tr>
</table>
