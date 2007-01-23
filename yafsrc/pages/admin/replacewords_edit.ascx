<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Control language="c#" Codebehind="replacewords_edit.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.replacewords_edit" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
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
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
