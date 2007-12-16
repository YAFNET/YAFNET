<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditUsersInfo.ascx.cs" Inherits="YAF.Controls.EditUsersInfo" %>





<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">Edit User</td>
	</tr>
	<tr>
		<td class="postheader">Name:</td>
		<td class="post"><asp:TextBox style="width:300px" id="Name" runat="server" Enabled="false" /></td>
	</tr>
	<tr>
		<td class="postheader">E-mail:</td>
		<td class="post"><asp:TextBox style="width:300px" id="Email" runat="server"/></td>
	</tr>
	<tr>
		<td class="postheader">Rank:</td>
		<td class="post"><asp:dropdownlist id="RankID" runat="server"/></td>
	</tr>
	<tr runat="server" id="IsHostAdminRow">
		<td class="postheader">Is Host Admin:</td>
		<td class="post"><asp:checkbox runat="server" id="IsHostAdminX"/></td>
	</tr>
	<tr runat="server" id="IsCaptchaExcludedRow">
		<td class="postheader">Exclude from CAPTCHA:</td>
		<td class="post"><asp:checkbox runat="server" id="IsCaptchaExcluded"/> (if checked, CAPTCHA check for this user is bypassed)</td>
	</tr>
	<!-- Easy to enable it if there is major issues (i.e. Guest being deleted). -->	     
	<tr runat="server" id="IsGuestRow" visible="false">
	  <td class="postheader">Is Guest:</td>
	  <td class="post"><asp:checkbox runat="server" id="IsGuestX"/></td>
  </tr>
	<tr>
		<td class="postheader">Joined:</td>
		<td class="post"><asp:TextBox id="Joined" runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class="postheader">Last Visit:</td>
		<td class="post"><asp:TextBox id="LastVisit" runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button id="Save" runat="server" Text="Save" onclick="Save_Click" />
			<asp:Button id="Cancel" runat="server" Text="Cancel" onclick="Cancel_Click" />
		</td>
	</tr>

</table>


