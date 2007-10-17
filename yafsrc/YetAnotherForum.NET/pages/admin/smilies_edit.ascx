<%@ Control Language="c#" CodeFile="smilies_edit.ascx.cs" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.smilies_edit" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="2">
				Edit Smiley</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Code:</b></td>
			<td class="post" width="50%">
				<asp:TextBox ID="Code" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Icon:</b></td>
			<td class="post" width="50%">
				<asp:DropDownList ID="Icon" runat="server" />
				&nbsp;
				<img style="vertical-align: middle" runat="server" id="Preview" src="" alt="" />
			</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Emotion:</b></td>
			<td class="post" width="50%">
				<asp:TextBox ID="Emotion" runat="server" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<b>Sort Order:</b></td>
			<td class="post" width="50%">
				<asp:TextBox ID="SortOrder" runat="server" Text="0" MaxLength="3" /></td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="save" runat="server" Text="Save" />
				<asp:Button ID="cancel" runat="server" Text="Cancel" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
