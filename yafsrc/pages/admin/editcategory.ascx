<%@ Control language="c#" Codebehind="editcategory.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.editcategory" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellspacing=1 cellpadding=0 width="100%">
	<tr>
		<td class=header1 colspan="2">Edit Category: <asp:label id=CategoryNameTitle runat=server></asp:label></td>
	</tr>
	<tr>
		<td class=post>Name:</td>
		<td class=post>
<asp:TextBox id=Name runat="server" cssclass=edit></asp:TextBox></td>
	</tr>
	<tr>
		<td class=post>Sort Order:</td>
		<td class=post>
<asp:TextBox id=SortOrder runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<td class=postfooter colspan="2" align=middle>
<asp:Button id=Save runat="server" Text="Save" onclick="Save_Click"></asp:Button>&nbsp;
<asp:Button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click"></asp:Button></td>
	</tr>
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
