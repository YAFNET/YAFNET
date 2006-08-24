<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersInfo.ascx.cs" Inherits="yaf.controls.EditUsersInfo" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2">Edit User</td>
	</tr>
	<tr>
		<td class="postheader">Name:</td>
		<td class="post"><asp:TextBox style="width:300px" id="Name" runat="server" /></td>
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


