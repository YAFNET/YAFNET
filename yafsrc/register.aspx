<%@ Page language="c#" Codebehind="register.aspx.cs" AutoEventWireup="false" Inherits="yaf.register" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"></asp:hyperlink>
</p>

<table class="content" cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1 colspan=2><%# GetText("Register_New_User") %></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("Registration_Details") %></td>
	</tr>
	<tr>
		<td width="50%" class=postheader><%# GetText("User_Name") %>:</td>
		<td class=post>
			<asp:TextBox id=UserName runat="server"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("User_Name_required") %>' ControlToValidate="UserName" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password runat="server" TextMode="Password"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("Password_required") %>' ControlToValidate="Password" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Retype_Password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password2 runat="server" TextMode="Password"/>
			<asp:CompareValidator runat="server" ErrorMessage='<%# GetText("Password_dont_match") %>' ControlToValidate="Password2" ControlToCompare="Password" EnableClientScript="False"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("Password_required") %>' ControlToValidate="Password2" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Email_Address") %>:</td>
		<td class=post><asp:TextBox id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("Profile_Information") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Location") %>:</td>
		<td class=post><asp:TextBox id=Location runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Home_Page") %>:</td>
		<td class=post><asp:TextBox id=HomePage runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2 align="middle"><%# GetText("Forum_Preferences") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("Time_Zone") %>:</td>
		<td class=post><asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"/></td>
	</tr>
	
	<tr>
		<td align="middle" colspan="2" class=footer1>
			<asp:Button id=ForumRegister runat="server" Text='<%# GetText("Register") %>'/>
			<asp:button id=cancel runat=server text='<%# GetText("Cancel") %>'/>
		</td>
	</tr>
</table>

</form>
