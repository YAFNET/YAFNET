<%@ Page language="c#" Codebehind="smilies_edit.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.smilies_edit" %>
<form runat="server">

<table class=content width=100% cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2>Edit Smiley</td>
</tr>
<tr>
	<td class=postheader width=50%><b>Code:</b></td>
	<td class=post width=50%><asp:textbox id=Code runat="server"/></td>
</tr>
<tr>
	<td class=postheader width=50%><b>Icon:</b></td>
	<td class=post width=50%><asp:dropdownlist id=Icon runat="server"/></td>
</tr>
<tr>
	<td class=postheader width=50%><b>Emotion:</b></td>
	<td class=post width=50%><asp:textbox id=Emotion runat="server"/></td>
</tr>
<tr>
	<td class=footer1 colspan=2 align=center>
		<asp:button id=save runat=server text="Save"/>
		<asp:button id=cancel runat=server text="Cancel"/>
	</td>
</tr>
</table>

</form>
