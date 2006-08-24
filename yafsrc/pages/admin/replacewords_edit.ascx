<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Codebehind="replacewords_edit.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.replacewords_edit" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<table class="content" cellSpacing="1" cellPadding="0" width="100%">
		<tr>
			<td class="header1" colSpan="2">Add/Edit Word Replace</td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><B>Bad Word</B></td>
			<td class="post" width="50%">
				<asp:textbox id="badword" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postheader" width="50%"><B>Good Word</B></td>
			<td class="post" width="50%">
				<asp:textbox id="goodword" runat="server"></asp:textbox></td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colSpan="2">
				<asp:button id="save" runat="server" text="Save"></asp:button>
				<asp:button id="cancel" runat="server" text="Cancel"></asp:button></td>
		</tr>
	</table>
</yaf:adminmenu>
<yaf:SmartScroller id="SmartScroller1" runat = "server" />
