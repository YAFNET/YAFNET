<%@ Page language="c#" Codebehind="register.aspx.cs" AutoEventWireup="false" Inherits="yaf.register" %>

<form runat="server">

<p class="navlinks">
	<asp:hyperlink runat="server" id="HomeLink"/>
</p>

<table class="content" cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1 colspan=2><%# GetText("register_title") %></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("register_details") %></td>
	</tr>
	<tr>
		<td width="50%" class=postheader><%# GetText("register_username") %>:</td>
		<td class=post>
			<asp:TextBox id=UserName runat="server"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("register_need_username") %>' ControlToValidate="UserName" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password runat="server" TextMode="Password"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("register_need_password") %>' ControlToValidate="Password" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_retype_password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password2 runat="server" TextMode="Password"/>
			<asp:CompareValidator runat="server" ErrorMessage='<%# GetText("register_need_match") %>' ControlToValidate="Password2" ControlToCompare="Password" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_email") %>:</td>
		<td class=post><asp:TextBox id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("register_profile") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_location") %>:</td>
		<td class=post><asp:TextBox id=Location runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_homepage") %>:</td>
		<td class=post><asp:TextBox id=HomePage runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2 align="middle"><%# GetText("register_preferences") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("register_timezone") %>:</td>
		<td class=post><asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"/></td>
	</tr>
	
	<tr>
		<td align="middle" colspan="2" class=footer1>
			<asp:Button id=ForumRegister runat="server"/>
			<asp:button id=cancel runat="server"/>
		</td>
	</tr>
</table>

</form>
