<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.smilies_edit" Codebehind="smilies_edit.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SMILIES_EDIT" />
            </td>
		</tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong><YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="CODE" LocalizedPage="ADMIN_SMILIES_EDIT" /></strong></td>
			<td class="post" width="50%">
				<asp:TextBox ID="Code" runat="server" Width="250" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ICON" LocalizedPage="ADMIN_SMILIES_EDIT" /></strong>
            </td>
			<td class="post" width="50%">
				<asp:DropDownList ID="Icon" runat="server" Width="250" />
				&nbsp;
				<img style="vertical-align: middle" runat="server" id="Preview" src="" alt="" />
			</td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="EMOTION" LocalizedPage="ADMIN_SMILIES_EDIT" /></strong>
            </td>
			<td class="post" width="50%">
				<asp:TextBox ID="Emotion" runat="server" Width="250" /></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
				<strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_SMILIES_EDIT" /></strong>
            </td>
			<td class="post" width="50%">
				<asp:TextBox ID="SortOrder" runat="server" Text="0" MaxLength="3" Width="250" /></td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="save" runat="server" Text="Save" CssClass="pbutton" />
				<asp:Button ID="cancel" runat="server" Text="Cancel" CssClass="pbutton" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
