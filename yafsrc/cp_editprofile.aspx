<%@ Page language="c#" Codebehind="cp_editprofile.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_editprofile" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>


<table width="100%" class=content cellspacing=1 cellpadding=4>
	<tr>
		<td class=header1 colspan=2><%= GetText("title") %></td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b><%= GetText("location") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("where") %></td>
		<td class=post><asp:TextBox id=Location runat="server" cssclass="edit"/></td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b><%= GetText("homepage") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("homepage2") %></td>
		<td class=post><asp:TextBox runat="server" id="HomePage" cssclass="edit"/></td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b><%= GetText("timezone") %></b></td>
	</tr>
	<tr>
		<td class=postheader><%= GetText("timezone2") %></td>
		<td class=post><asp:DropDownList runat="server" id="TimeZones" DataTextField="Name" DataValueField="Value"/></td>
	</tr>

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

</form>
