<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.mail" Codebehind="mail.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2">
			  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_MAIL" />
            </td>
		</tr>
        <tr>
	      <td class="header2" colspan="2" style="height:30px"></td>
		</tr>
		<tr>
			<td class="postheader">
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="MAIL_TO" LocalizedPage="ADMIN_MAIL" /></strong>
            </td>
			<td class="post">
			  <asp:DropDownList ID="ToList" runat="server" DataValueField="GroupID" DataTextField="Name" Style="Width:250px">
                </asp:DropDownList>
            </td>
		</tr>
		<tr>
			<td class="postheader">
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="MAIL_SUBJECT" LocalizedPage="ADMIN_MAIL" /></strong>
            </td>
			<td class="post">
			  <asp:TextBox ID="Subject" runat="server" CssClass="edit" Style="Width:250px"></asp:TextBox>
            </td>
		</tr>
		<tr>
			<td class="postheader" valign="top">
			  <strong><YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MAIL_MESSAGE" LocalizedPage="ADMIN_MAIL" /></strong>
            </td>
			<td class="post">
			  <asp:TextBox ID="Body" runat="server" TextMode="MultiLine" CssClass="edit" Rows="16" Style="Width:99%"></asp:TextBox>
            </td>
		</tr>
		<tr>
			<td class="footer1" align="center" colspan="2">
			  <asp:Button ID="Send" runat="server" Text="Send" OnClick="Send_Click" CssClass="pbutton"></asp:Button>
            </td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
