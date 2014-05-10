<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.spamwords_edit" Codebehind="spamwords_edit.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		
		<tr>
			<td class="header1" colspan="2">
			  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
             </td>
		</tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader" width="50%">
              <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SPAM" LocalizedPage="ADMIN_SPAMWORDS_EDIT" />
            </td>
			<td class="post" width="50%">
			  <asp:TextBox ID="spamword" runat="server" Width="250"></asp:TextBox>
            </td>
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
			  <asp:Button ID="save" runat="server" CssClass="pbutton"></asp:Button>
			  <asp:Button ID="cancel" runat="server" CssClass="pbutton"></asp:Button></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
