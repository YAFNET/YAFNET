<%@ Control language="c#" Codebehind="cp_editprofile.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.cp_editprofile" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table width="100%" class=content cellspacing=1 cellpadding=4>
	<tr>
		<td class=header1 colspan=2><%= GetText("title") %></td>
	</tr>
		<tr>
			<td colspan="2" class="header2"><b><%= GetText("aboutyou") %></b></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("realname2") %></td>
			<td class="post"><asp:TextBox id="Realname" runat="server" cssclass="edit" /></td>
		</tr>
	<tr>
		<td colspan=2 class=header2><b><%= GetText("location") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("where") %></td>
		<td class=post><asp:TextBox id=Location runat="server" cssclass="edit"/></td>
	</tr>
		<tr>
			<td class="postheader"><%= GetText("occupation") %></td>
			<td class="post"><asp:TextBox id="Occupation" runat="server" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("interests") %></td>
			<td class="post"><asp:TextBox id="Interests" runat="server" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("gender") %></td>
			<td class="post">
				<asp:DropDownList id="Gender" runat="server" cssclass="edit" /></td>
		</tr>
	<tr>
		<td colspan=2 class=header2><b><%= GetText("homepage") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("homepage2") %></td>
		<td class=post><asp:TextBox runat="server" id="HomePage" cssclass="edit"/></td>
	</tr>
	
		<tr>
			<td class="postheader"><%= GetText("weblog2") %></td>
			<td class="post"><asp:TextBox runat="server" id="Weblog" cssclass="edit" /></td>
		</tr>
		<tr>
			<td colspan="2" class="header2"><b><%= GetText("messenger") %></b></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("msn") %></td>
			<td class="post"><asp:TextBox runat="server" id="MSN" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("yim") %></td>
			<td class="post"><asp:TextBox runat="server" id="YIM" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("aim") %></td>
			<td class="post"><asp:TextBox runat="server" id="AIM" cssclass="edit" /></td>
		</tr>
		<tr>
			<td class="postheader"><%= GetText("icq") %></td>
			<td class="post"><asp:TextBox runat="server" id="ICQ" cssclass="edit" /></td>
		</tr>
	
	
	<tr>
		<td colspan=2 class=header2><b><%= GetText("timezone") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("timezone2") %></td>
		<td class=post><asp:DropDownList runat="server" id="TimeZones" DataTextField="Name" DataValueField="Value"/></td>
	</tr>

	<asp:placeholder runat="server" id="ForumSettingsRows">
	<tr>
		<td colspan=2 class=header2><b><%= GetText("FORUM_SETTINGS") %></b></td>
	</tr>
	<tr runat="server" id="UserThemeRow">
		<td class="postheader"><%= GetText("SELECT_THEME") %></td>
		<td class="post"><asp:dropdownlist runat="server" id="Theme"/></td>
	</tr>
	<tr runat="server" id="UserLanguageRow">
		<td class="postheader"><%= GetText("SELECT_LANGUAGE") %></td>
		<td class="post"><asp:dropdownlist runat="server" id="Language"/></td>
	</tr>
	</asp:placeholder>

	<tr runat="server" id="AvatarRow">
		<td class=header2 colspan=2><%= GetText("avatar") %></td>
	</tr>
	<tr runat="server" id="AvatarUploadRow">
		<td class=postheader><%= GetText("avatarupload") %></td>
		<td class=post><input type="file" id="File" runat="server"/></td>
	</tr>
	<tr runat="server" id="AvatarDeleteRow">
		<td class=postheader><%= GetText("avatardelete") %></td>
		<td class=post><asp:button runat="server" id="DeleteAvatar"/></td>
	</tr>
	<tr runat="server" id="AvatarRemoteRow">
		<td class=postheader><%= GetText("avatarremote") %></td>
		<td class=post><asp:textbox cssclass=edit id=Avatar runat="server"/></td>
	</tr>

	<tr>
		<td class=header2 colspan=2><%= GetText("change_password") %></td>
	</tr>
	<tr>
		<td class=postheader width="50%"><%= GetText("oldpassword") %></td>
		<td class=post width="50%"><asp:TextBox cssclass="edit" ID="OldPassword" Runat="server" TextMode="Password"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("newpassword") %></td>
		<td class=post><asp:TextBox cssclass=edit ID=NewPassword1 Runat=server TextMode="Password"/></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("confirmpassword") %></td>
		<td class=post><asp:TextBox cssclass=edit ID=NewPassword2 Runat=server TextMode="Password"/></td>
	</tr>

	<tr>
		<td colspan=2 class=header2><%= GetText("change_email") %></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("email") %></td>
		<td class=post><asp:TextBox id=Email cssclass=edit runat="server"/></td>
	</tr>

	<tr>
		<td class=footer1 colspan=2 align=middle>
			<asp:Button id=UpdateProfile runat="server"/>
		</td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
