<%@ Page language="c#" Codebehind="edituser.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.edituser" %>

<form runat=server>

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=2>Edit User</td>
</tr>
	<tr>
		<td class=post>Group:</td>
		<td class=post><asp:DropDownList id=GroupList runat="server" DataValueField="GroupID" DataTextField="Name"/></td>
	</tr>
	<tr>
		<td class=post>Name:</td>
		<td class=post><asp:TextBox style="width:300px" id=Name runat="server"/></td>
	</tr>
	<tr>
		<td class=post>E-mail:</td>
		<td class=post><asp:TextBox style="width:300px" id=Email runat="server" ReadOnly="True"/></td>
	</tr>
	<tr>
		<td class=post>Joined:</td>
		<td class=post><asp:TextBox id=Joined runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class=post>Last Visit:</td>
		<td class=post><asp:TextBox id=LastVisit runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server" Text="Save"/>
			<asp:Button id=Cancel runat="server" Text="Cancel"/>
		</td>
	</tr>

</table>

</form>
