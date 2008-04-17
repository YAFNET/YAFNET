<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditUsersInfo.ascx.cs" Inherits="YAF.Controls.EditUsersInfo" %>





<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">User Details</td>
	</tr>
	<tr>
		<td class="postheader"><b>Username:</b>
		<br />
		Cannot be modified.		
		</td>
		<td class="post"><asp:TextBox style="width:300px" id="Name" runat="server" Enabled="false" /></td>
	</tr>
	<tr>
		<td class="postheader"><b>E-mail:</b>
		</td>
		<td class="post"><asp:TextBox style="width:300px" id="Email" runat="server"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Rank:</b>
		</td>
		<td class="post"><asp:dropdownlist id="RankID" runat="server"/></td>
	</tr>
	<tr runat="server" id="IsHostAdminRow">
		<td class="postheader"><b>Host Admin:</b>
		<br />
		Gives user access to modify "Host Settings" section.
		</td>
		<td class="post"><asp:checkbox runat="server" id="IsHostAdminX"/></td>
	</tr>
	<tr runat="server" id="IsCaptchaExcludedRow">
		<td class="postheader"><b>Exclude from CAPTCHA:</b><br />
		CAPTCHA is disabled for this user specifically.
		</td>
		<td class="post"><asp:checkbox runat="server" id="IsCaptchaExcluded"/></td>
	</tr>
	<!-- Easy to enable it if there is major issues (i.e. Guest being deleted). -->	     
	<tr runat="server" id="IsGuestRow" visible="false">
	  <td class="postheader"><b>Is Guest:</b></td>
	  <td class="post"><asp:checkbox runat="server" id="IsGuestX"/></td>
  </tr>
	<tr>
		<td class="postheader"><b>Joined:</b></td>
		<td class="post"><asp:TextBox id="Joined" runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class="postheader"><b>Last Visit:</b></td>
		<td class="post"><asp:TextBox id="LastVisit" runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class="postfooter" colspan="2" align="center">
			<asp:Button id="Save" runat="server" Text="Save" onclick="Save_Click" />
		</td>
	</tr>

</table>


