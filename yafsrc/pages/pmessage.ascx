<%@ Control language="c#" Codebehind="pmessage.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.pmessage" %>
<%@ Register TagPrefix="editor" Namespace="yaf.editor" Assembly="yaf" %>
<%@ Register TagPrefix="uc1" TagName="smileys" Src="../controls/smileys.ascx" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class="content" width="100%" cellspacing="1" cellpadding="0">
	<tr>
		<td class="header1" colspan="2"><%= GetText("title") %></td>
	</tr>
	<tr id=ToRow runat=server>
		<td width="30%" class="postformheader"><%= GetText("to") %></td>
		<td width="70%" class="post">
			<asp:TextBox id="To" runat="server"/>
			<asp:DropDownList runat="server" id="ToList" visible="false"/>
			<asp:button runat="server" id="FindUsers"/>
			<asp:button runat="server" id="AllUsers"/>
		</td>
	</tr>
	<tr>
		<td class="postformheader"><%= GetText("subject") %></td>
		<td class="post"><asp:TextBox id=Subject runat="server"/></td>
	</tr>
	<tr>
		<td class="postformheader" valign="top">
			<%= GetText("message") %>
			<uc1:smileys runat="server" onclick="insertsmiley"/>
		</td>
		<td id="EditorLine" class="post" runat="server">
			<!-- editor goes here -->	
		</td>
	</tr>
	<tr>
		<td class="postfooter" colspan=2 align=middle>
			<asp:Button id=Save cssclass="pbutton" runat="server"/>
			<asp:Button id=Cancel cssclass="pbutton" runat="server"/>
		</td>
	</tr>
</table>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
