<%@ Control language="c#" Codebehind="login.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.login" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table align=center cellspacing="1" cellpadding="0" class=content width=100% id=LoginView runat=server>
<tr>
	<td class=header1 colspan=2><%= GetText("title") %></td>
</tr>
<tr>
	<td class=postheader width=50%><%= GetText("username") %></td>
	<td class=post width=50%><asp:TextBox id=UserName runat="server"/></td>
</tr>
<tr>
	<td class=postheader><%= GetText("password") %></td>
	<td class=post><asp:TextBox id=Password runat="server" TextMode="Password"/></td>
</tr>
<tr>
	<td class=postheader><%= GetText("auto") %></td>
	<td class=post><asp:CheckBox id="AutoLogin" runat="server" Checked="true"/></td>
</tr>
<tr>
	<td class=postfooter colspan=2 align=middle>
		<asp:Button id=ForumLogin runat="server"/>
		<asp:button id=LostPassword runat="server"/>
	</td>
</tr>
</table>

<table class=content width=100% cellspacing=1 cellpadding=0 id=RecoverView runat=server visible=false>
<tr>
	<td class=header1 colspan=2><%= GetText("recover") %></td>
</tr>
<tr>
	<td class=postheader width=50%><%= GetText("username") %></td>
	<td class=post width=50%><asp:textbox id=LostUserName runat="server"/></td>
</tr>
<tr>
	<td class=postheader><%= GetText("email") %></td>
	<td class=post><asp:textbox id=LostEmail runat="server"/></td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=middle><asp:button id="Recover" runat="server"/></td>
</tr>
</table>

<yaf:savescrollpos runat="server"/>
