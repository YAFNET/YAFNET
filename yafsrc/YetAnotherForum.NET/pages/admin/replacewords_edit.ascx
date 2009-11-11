<%@ Control Language="c#" CodeFile="replacewords_edit.ascx.cs" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.replacewords_edit" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		
		<tr>
			<td class="header1" colspan="2">
			
			Add/Edit Word Replace</td>
			
		</tr>
		<tr>
			<td class="postheader" width="50%">
			
			<b>&quot;Bad&quot; Expression:</b>
			<br />Regular expression statement. Escape puncutation with a preceeding slash (e.g. &apos;\.&apos;).</td>
			
			
			
			
			<td class="post" width="50%">
			
			<asp:TextBox ID="badword" runat="server"></asp:TextBox></td>
			
			
			
		</tr>
		<tr>
			<td class="postheader" width="50%">
			
			<b>&quot;Good&quot; Expression:</b>
			<br />Regular expression statement. Escape puncutation with a preceeding slash (e.g. &apos;\.&apos;).</td>
			
			
			
			
			<td class="post" width="50%">
			
			<asp:TextBox ID="goodword" runat="server"></asp:TextBox></td>
			
			
			
		</tr>
		<tr>
			<td class="postfooter" align="center" colspan="2">
			
			<asp:Button ID="save" runat="server" Text="Save"></asp:Button>
			
			
			
			
			<asp:Button ID="cancel" runat="server" Text="Cancel"></asp:Button></td>
			
			
			
			
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
