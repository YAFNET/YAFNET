<%@ Control Language="c#" AutoEventWireup="false" Codebehind="avatar.ascx.cs" Inherits="yaf.pages.avatar" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" cellSpacing="1" cellPadding="0" width="100%">
	<tr>
		<td class="header1" colSpan="10"><%= GetText("TITLE") %></td>
	</tr>
	<asp:Literal id="DirResults" Runat="server" />
	<asp:Literal id="AvatarResults" Runat="server" />
</table>

<yaf:pager runat="server" id="pager"/>

<asp:linkbutton runat="server" id="GoDir" visible="false"/>
