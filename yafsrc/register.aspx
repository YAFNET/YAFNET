<%@ Page language="c#" Codebehind="register.aspx.cs" AutoEventWireup="false" Inherits="yaf.register" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"></asp:hyperlink>
</p>

<table class="content" cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1 colspan=2>Register New User</td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle">Registration Details</td>
	</tr>
	<tr>
		<td width="50%" class=postheader>User Name:</td>
		<td class=post><asp:TextBox id=UserName runat="server"></asp:TextBox>
<asp:RequiredFieldValidator id=RequiredFieldValidator1 runat="server" ErrorMessage="User Name is required." ControlToValidate="UserName" EnableClientScript="False"></asp:RequiredFieldValidator></td></tr>
	<tr><td class=postheader>Password:</td><td class=post>
<asp:TextBox id=Password runat="server" TextMode="Password"></asp:TextBox>
<asp:RequiredFieldValidator id=RequiredFieldValidator2 runat="server" ErrorMessage="Password is required." ControlToValidate="Password" EnableClientScript="False"></asp:RequiredFieldValidator></td></tr>
	<tr><td class=postheader>Retype Password:</td><td class=post>
<asp:TextBox id=Password2 runat="server" TextMode="Password"></asp:TextBox>
<asp:CompareValidator id=CompareValidator1 runat="server" ErrorMessage="Passwords didn't match." ControlToValidate="Password2" ControlToCompare="Password" EnableClientScript="False"></asp:CompareValidator>
<asp:RequiredFieldValidator id=RequiredFieldValidator3 runat="server" ErrorMessage="Password is required." ControlToValidate="Password2" EnableClientScript="False"></asp:RequiredFieldValidator></td></tr>
	<tr><td class=postheader>Email Address:</td><td class=post>
<asp:TextBox id=Email runat="server"></asp:TextBox></td></tr>
	<tr>
		<td class=header2 colspan=2 align="middle">Profile Information</td>
	</tr>
	<tr><td class=postheader>Location:</td><td class=post>
<asp:TextBox id=Location runat="server"></asp:TextBox></td></tr>
	<tr><td class=postheader>Home Page:</td><td class=post>
<asp:TextBox id=HomePage runat="server"></asp:TextBox></td></tr>
	<tr>
		<td class="header2" colspan=2 align="middle">Forum Preferences</td>
	</tr>
	<tr><td class=postheader>Time Zone:</td><td class=post>
<asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"></asp:DropDownList></td></tr>
	
	<tr><td align="middle" colspan="2" class=footer1>
<asp:Button id=ForumRegister runat="server" Text="Register"></asp:Button>
<asp:button id=cancel runat=server text=Cancel></asp:button>
</td></tr>
</table>

</form>
