<%@ Page language="c#" Codebehind="edituser.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.edituser" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat=server>

<table class=content width="100%" cellspacing=1 cellpadding=0>
	<tr>
		<td class=header1 colspan=2>Edit User</td>
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
		<td class=post>Rank:</td>
		<td class=post><asp:dropdownlist id=RankID runat="server"/></td>
	</tr>
	<tr>
		<td class=post>Joined:</td>
		<td class=post><asp:TextBox id=Joined runat="server" Enabled="False"/></td>
	</tr>
	<tr>
		<td class=post>Last Visit:</td>
		<td class=post><asp:TextBox id=LastVisit runat="server" Enabled="False"/></td>
	</tr>
	
    <asp:repeater id=UserGroups runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header1 colspan=2>User Groups</td>
		</tr>
		<tr>
			<td class=header2>Member</td>
			<td class=header2>Group</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post><asp:checkbox runat="server" id=GroupMember checked='<%# IsMember(DataBinder.Eval(Container.DataItem,"Member")) %>'/></td>
			<td class=post><asp:label id=GroupID visible=false runat="server" text='<%# DataBinder.Eval(Container.DataItem, "GroupID") %>'></asp:label>
				<b><%# DataBinder.Eval(Container.DataItem, "Name") %></b>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>
	
	<tr>
		<td class=postfooter colspan=2 align=middle>
			<asp:Button id=Save runat="server" Text="Save"/>
			<asp:Button id=Cancel runat="server" Text="Cancel"/>
		</td>
	</tr>

</table>

<yaf:savescrollpos runat="server"/>
</form>
