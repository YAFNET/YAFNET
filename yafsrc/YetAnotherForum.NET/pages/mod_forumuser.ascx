<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.mod_forumuser" Codebehind="mod_forumuser.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="50%">
			<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="USER" />
		</td>
		<td class="post" width="50%">
			<asp:TextBox runat="server" ID="UserName" /><asp:DropDownList runat="server" ID="ToList"
				Visible="false" CssClass="standardSelectMenu" />
			<asp:Button runat="server" ID="FindUsers" OnClick="FindUsers_Click" /></td>
	</tr>
	<tr>
		<td class="postheader">
			<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCESSMASK" />
		</td>
		<td class="post">
			<asp:DropDownList runat="server" ID="AccessMaskID" CssClass="standardSelectMenu" /></td>
	</tr>
	<tr class="footer1">
		<td colspan="2" align="center">
			<asp:Button runat="server" ID="Update" CssClass="pbutton" OnClick="Update_Click" />
			<asp:Button runat="server" ID="Cancel" CssClass="pbutton" OnClick="Cancel_Click" />
		</td>
	</tr>
</table>
<div id="DivSmartScroller">
	<YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
