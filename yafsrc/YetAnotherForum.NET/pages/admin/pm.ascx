<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pm" Codebehind="pm.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_PM" />
            </td>
		</tr>
        <tr>
			    <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_NUMBER" LocalizedPage="ADMIN_PM" />
            </td>
			<td class="post" width="50%">
				<asp:Label runat="server" ID="Count" />
            </td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DELETE_READ" LocalizedPage="ADMIN_PM" />
                </td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Days1" CssClass="Numeric" />
				<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DAYS" LocalizedPage="ADMIN_PM" />
            </td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE_UNREAD" LocalizedPage="ADMIN_PM" />
            </td>
			<td class="post" width="50%">
				<asp:TextBox runat="server" ID="Days2" CssClass="Numeric" />
				<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DAYS" LocalizedPage="ADMIN_PM" />
            </td>
		</tr>
		<tr>
			<td class="footer1" colspan="2" align="center">
				<asp:Button ID="commit" CssClass="pbutton" runat="server" OnLoad="DeleteButton_Load" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
