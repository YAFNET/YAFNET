<%@ Page language="c#" Codebehind="login.aspx.cs" AutoEventWireup="false" Inherits="yaf.login" %>

<form runat="server">

<p class="navlinks"><asp:hyperlink runat=server id=HomeLink></asp:hyperlink></p>

<asp:Label id=ErrorMsg runat="server" Font-Bold="True" ForeColor="Red" Visible="False">Label</asp:Label>

<table align=center cellspacing="1" cellpadding="0" class=content width=100% id=LoginView runat=server>
<tr>
	<td class=header1 colspan=2>Login</td>
</tr>
<tr>
	<td class=postheader width=50%>User Name:</td>
	<td class=post width=50%><asp:TextBox id=UserName runat="server"></asp:TextBox></td>
</tr>
<tr>
	<td class=postheader>Password:</td>
	<td class=post><asp:TextBox id=Password runat="server" TextMode="Password"></asp:TextBox></td>
</tr>
<tr>
	<td class=postheader>&nbsp;</td>
	<td class=post><asp:CheckBox id=AutoLogin runat="server" Text="Auto Login" Checked="True"></asp:CheckBox></td>
</tr>
<tr>
	<td class=postheader>&nbsp;</td>
	<td class=post><asp:LinkButton id=LostPassword runat=server Text="Lost Password"></asp:LinkButton></td>
</tr>
<tr>
	<td class=postfooter colspan=2 align=middle>
<asp:Button id=ForumLogin runat="server" Text="Forum Login"></asp:Button></td></tr>
</table>

<table class=content width=100% cellspacing=1 cellpadding=0 id=RecoverView runat=server visible=false>
<tr>
	<td class=header1 colspan=2>Recover Password</td>
</tr>
<tr>
	<td class=postheader width=50%>Enter your user name:</td>
	<td class=post width=50%><asp:textbox id=LostUserName runat=server></asp:textbox></td>
</tr>
<tr>
	<td class=postheader>Enter your email address:</td>
	<td class=post><asp:textbox id=LostEmail runat=server></asp:textbox></td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=middle><asp:button id=Recover runat=server text="Send Password"></asp:button></td>
</tr>
</table>

</form>
