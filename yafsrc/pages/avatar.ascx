<%@ Control Language="c#" AutoEventWireup="false" Codebehind="avatar.ascx.cs" Inherits="yaf.pages.avatar" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<p class="navlinks"><yaf:pager runat="server" id="pager"/></p>

<table class="content" cellSpacing="1" cellPadding="0" width="100%">
	<tr>
		<td class="header1" colSpan="10"><%= GetText("TITLE") %></td>
	</tr>
	<asp:Literal id="DirResults" Runat="server" />
	<asp:Literal id="AvatarResults" Runat="server" />
</table>

<p class="navlinks"><yaf:pager runat="server" linkedpager="pager"/></p>

<asp:linkbutton runat="server" id="GoDir" visible="false"/>
