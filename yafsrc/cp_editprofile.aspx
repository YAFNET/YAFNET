<%@ Page language="c#" Codebehind="cp_editprofile.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp_editprofile" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server">Home</asp:hyperlink>
	&#187; <asp:hyperlink id=UserLink runat="server">UserLink</asp:hyperlink>
	&#187; <a href="cp_editprofile.aspx">Edit Profile</a>
</p>


<table width="100%" class=content cellspacing=1 cellpadding=4>
	<tr>
		<td class=header1 colspan=2>Edit Profile</td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b>Location</b></td>
	</tr>
	<tr>
		<td class=post>Where do you live?</td>
		<td class=post><asp:TextBox id=Location runat="server" cssclass="edit"/></td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b>Home Page</b></td>
	</tr>
	<tr>
		<td class=post>Your home page:</td>
		<td class=post><asp:TextBox id=HomePage runat="server" cssclass="edit"/></td>
	</tr>
	<tr>
		<td colspan=2 class=header2><b>Time Zone</b></td>
	</tr>
	<tr>
		<td class=post>To give you times and dates in your local time, we need to know your time zone.</td>
		<td class=post><asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"/></td>
	</tr>

<tr>
	<td class=header2 colspan=2>Avatar</td>
</tr>
<tr>
	<td class=post>
		Enter the url of an avatar that will be displayed next to your posts, or
		<asp:linkbutton id="UploadAvatar" runat="server" text="upload"/> an image
		from your local computer.
	</td>
	<td class=post><asp:textbox cssclass=edit id=Avatar runat="server"/></td>
</tr>

<tr>
	<td class=header2 colspan=2>Change Password</td>
</tr>
<tr>
	<td class=postheader width="50%">Old Password:</td>
	<td class=post width="50%"><asp:TextBox cssclass=edit ID="OldPassword" Runat=server TextMode=Password></asp:TextBox></td>
</tr>
<tr>
	<td class=postheader>New Password:</td>
	<td class=post><asp:TextBox cssclass=edit ID=NewPassword1 Runat=server TextMode=Password></asp:TextBox></td>
</tr>
<tr>
	<td class=postheader>Re-Type New Password:</td>
	<td class=post><asp:TextBox cssclass=edit ID=NewPassword2 Runat=server TextMode=Password></asp:TextBox></td>
</tr>

<tr>
	<td colspan=2 class=header2><b>Change Email Address</b></td>
</tr>
<tr>
	<td class=postheader>You must always have a valid email address.</td>
	<td class=post><asp:TextBox id=Email cssclass=edit runat="server"></asp:TextBox></td>
</tr>

<tr>
	<td class=footer1 colspan=2 align=middle>
<asp:Button id=UpdateProfile runat="server" Text="Save"></asp:Button></td>
</tr>
</table>

</form>
