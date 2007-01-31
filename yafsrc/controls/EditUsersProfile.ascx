<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditUsersProfile.ascx.cs" Inherits="YAF.Controls.EditUsersProfile" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<%@ Register TagPrefix="editor" Namespace="YAF.Editor" Assembly="YAF" %>

<table width="100%" class="content" cellspacing="1" cellpadding="4">
	<tr>
		<td class="header1" colspan="2"><%= PageContext.Localization.GetText("CP_EDITPROFILE","title") %></td>
	</tr>
		<tr>
			<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","aboutyou") %></b></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","realname2") %></td>
			<td class="post"><asp:TextBox id="Realname" runat="server" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","occupation") %></td>
			<td class="post"><asp:TextBox id="Occupation" runat="server" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","interests") %></td>
			<td class="post"><asp:TextBox id="Interests" runat="server" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","gender") %></td>
			<td class="post">
				<asp:DropDownList id="Gender" runat="server" cssclass="edit" /></td>
		</tr>		
	<tr>
		<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","location") %></b></td>
	</tr>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","where") %></td>
		<td class="post"><asp:TextBox id="Location" runat="server" cssclass="edit"/></td>
	</tr>
	<tr>
		<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","homepage") %></b></td>
	</tr>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","homepage2") %></td>
		<td class="post"><asp:TextBox runat="server" id="HomePage" cssclass="edit"/></td>
	</tr>
	
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","weblog2") %></td>
			<td class="post"><asp:TextBox runat="server" id="Weblog" cssclass="edit" /></td>
		</tr>
		<tr>
			<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","messenger") %></b></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","msn") %></td>
			<td class="post"><asp:TextBox runat="server" id="MSN" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","yim") %></td>
			<td class="post"><asp:TextBox runat="server" id="YIM" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","aim") %></td>
			<td class="post"><asp:TextBox runat="server" id="AIM" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","icq") %></td>
			<td class="post"><asp:TextBox runat="server" id="ICQ" cssclass="edit" /></td>
		</tr>
	
	
	<tr>
		<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","timezone") %></b></td>
	</tr>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","timezone2") %></td>
		<td class="post"><asp:DropDownList runat="server" id="TimeZones" DataTextField="Name" DataValueField="Value"/></td>
	</tr>

	<tr runat="server" id="ForumSettingsRows">
		<td colspan="2" class="header2"><b><%= PageContext.Localization.GetText("CP_EDITPROFILE","FORUM_SETTINGS") %></b></td>
	</tr>
	<tr runat="server" id="UserThemeRow">
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","SELECT_THEME") %></td>
		<td class="post"><asp:dropdownlist runat="server" id="Theme"/></td>
	</tr>
	<tr runat="server" id="OverrideForumThemeRow">
        <td class="postheader">
            <%= PageContext.Localization.GetText("CP_EDITPROFILE","OVERRIDE_DEFAULT_THEMES") %>
        </td>
        <td class="post">
            <asp:CheckBox ID="OverrideDefaultThemes" runat="server" /></td>
    </tr>
	<tr runat="server" id="UserLanguageRow">
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","SELECT_LANGUAGE") %></td>
		<td class="post"><asp:dropdownlist runat="server" id="Language"/></td>
	</tr>
	
	<tr runat="server" id="PMNotificationRow">
	    <td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","PM_EMAIL_NOTIFICATION") %></td>
	    <td class="post"><asp:CheckBox id="PMNotificationEnabled" runat="server"/></td>
	</tr>
	<asp:placeholder runat="server" id="LoginInfo" visible="false">
	<tr>
		<td class="header2" colspan="2"><%= PageContext.Localization.GetText("CP_EDITPROFILE","change_password") %></td>
	</tr>
	<asp:PlaceHolder runat="server" ID="ShowOldPassword" Visible="true">
	    <tr>
		    <td class="postheader" width="50%"><%= PageContext.Localization.GetText("CP_EDITPROFILE","oldpassword") %></td>
		    <td class="post" width="50%"><asp:TextBox cssclass="edit" ID="OldPassword" Runat="server" TextMode="Password"/></td>
	    </tr>
	</asp:PlaceHolder>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","newpassword") %></td>
		<td class="post"><asp:TextBox cssclass="edit" ID="NewPassword1" Runat="server" TextMode="Password"/></td>
	</tr>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","confirmpassword") %></td>
		<td class="post"><asp:TextBox cssclass="edit" ID="NewPassword2" Runat="server" TextMode="Password"/></td>
	</tr>

	<tr>
		<td colspan="2" class="header2"><%= PageContext.Localization.GetText("CP_EDITPROFILE","change_email") %></td>
	</tr>
	<tr>
		<td class="postheader"><%= PageContext.Localization.GetText("CP_EDITPROFILE","email") %></td>
		<td class="post"><asp:TextBox id="Email" cssclass="edit" runat="server" ontextchanged="Email_TextChanged" /></td>
	</tr>
	</asp:placeholder>

	<tr>
		<td class="footer1" colspan="2" align="center">
            <asp:Button ID="UpdateProfile" CssClass="pbutton" runat="server" OnClick="UpdateProfile_Click" />
            |
            <asp:Button ID="Cancel" CssClass="pbutton" runat="server" OnClick="Cancel_Click" />            
		</td>
	</tr>
</table>