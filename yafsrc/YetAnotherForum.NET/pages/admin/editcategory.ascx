<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.editcategory" Codebehind="editcategory.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		
		<tr>
			<td class="header1" colspan="2">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITCATEGORY" />
			<asp:Label ID="CategoryNameTitle" runat="server"></asp:Label></td>
		</tr>
        <tr>
	      <td class="header2" height="30" colspan="2"></td>
		</tr>
		<tr>
			<td class="postheader">
			  <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CATEGORY_NAME" LocalizedPage="ADMIN_EDITCATEGORY" />
			</td>
			<td class="post">
			<asp:TextBox ID="Name" runat="server" MaxLength="50" Width="250"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postheader">
			  <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CATEGORY_IMAGE" LocalizedPage="ADMIN_EDITCATEGORY" />
			</td>
			<td class="post">
			<asp:DropDownList ID="CategoryImages" Width="250" runat="server" CssClass="standardSelectMenu" />
			<img align="middle" alt="Preview" runat="server" id="Preview" />
			</td>
		</tr>
		<tr>
			<td class="postheader">
			  <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITCATEGORY" />
			</td>
			<td class="post">
			<asp:TextBox ID="SortOrder" runat="server" Width="250" MaxLength="5" CssClass="Numeric"></asp:TextBox></td>
		</tr>
		<tr>
			<td class="postfooter" colspan="2" align="center">
			  <asp:Button ID="Save" runat="server" OnClick="Save_Click" CssClass="pbutton"></asp:Button>
			  <asp:Button ID="Cancel" runat="server" OnClick="Cancel_Click" CssClass="pbutton"></asp:Button>
            </td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
