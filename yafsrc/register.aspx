<%@ Page language="c#" Codebehind="register.aspx.cs" AutoEventWireup="false" Inherits="yaf.register" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server">

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1 colspan=2><%# GetText("title") %></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("details") %></td>
	</tr>
	<tr>
		<td width="50%" class=postheader><%# GetText("username") %>:</td>
		<td class=post>
			<asp:TextBox id=UserName runat="server"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("need_username") %>' ControlToValidate="UserName" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password runat="server" TextMode="Password"/>
			<asp:RequiredFieldValidator runat="server" ErrorMessage='<%# GetText("need_password") %>' ControlToValidate="Password" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("retype_password") %>:</td>
		<td class=post>
			<asp:TextBox id=Password2 runat="server" TextMode="Password"/>
			<asp:CompareValidator runat="server" ErrorMessage='<%# GetText("need_match") %>' ControlToValidate="Password2" ControlToCompare="Password" EnableClientScript="False"/>
		</td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("email") %>:</td>
		<td class=post><asp:TextBox id=Email runat="server"/></td>
	</tr>
	<tr>
		<td class=header2 colspan=2 align="middle"><%# GetText("profile") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("location") %>:</td>
		<td class=post><asp:TextBox id=Location runat="server"/></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("homepage") %>:</td>
		<td class=post><asp:TextBox id=HomePage runat="server"/></td>
	</tr>
	<tr>
		<td class="header2" colspan=2 align="middle"><%# GetText("preferences") %></td>
	</tr>
	<tr>
		<td class=postheader><%# GetText("timezone") %>:</td>
		<td class=post><asp:DropDownList id=TimeZones runat="server" DataTextField="Name" DataValueField="Value"/></td>
	</tr>
	
	<tr>
		<td align="middle" colspan="2" class=footer1>
			<asp:Button id=ForumRegister runat="server"/>
			<asp:button id=cancel runat="server"/>
		</td>
	</tr>
</table>

<yaf:savescrollpos runat="server"/>
</form>
