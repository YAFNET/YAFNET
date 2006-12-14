<%@ Control language="c#" Codebehind="editgroup.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editgroup" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class=header1 colspan=11>Edit Group</td>
</tr>
<tr>
	<td class=postheader width="50%"><b>Name:</b><br/>Name of this group.</td>
	<td class=post width="50%"><asp:textbox style="width:300px" id=Name runat="server"/></td>
</tr>
<tr>
	<td class=postheader><b>Is Admin:</b><br/>Means that users in this group are admins.</td>
	<td class=post><asp:checkbox id=IsAdminX runat="server"></asp:checkbox></td>
</tr>
<tr>
	<td class=postheader><b>Is Start:</b><br/>If this is checked, all new users will be a member of this group.</td>
	<td class=post><asp:checkbox id=IsStart runat="server"></asp:checkbox></td>
</tr>
<tr>
	<td class=postheader><b>Is Forum Moderator:</b><br/>When this is checked, members of this group will have some admin access rights.</td>
	<td class=post><asp:checkbox id=IsModeratorX runat="server"></asp:checkbox></td>
</tr>
<tr runat="server" id="NewGroupRow">
	<td class="postheader"><b>Initial Access Mask:</b><br/>The initial access mask for all forums.</td>
	<td class="post"><asp:dropdownlist runat="server" id="AccessMaskID" ondatabinding="BindData_AccessMaskID"/></td>
</tr>
    
<asp:repeater id=AccessList runat="server">
<HeaderTemplate>
	<tr>
		<td class=header1 colspan=11>Access</td>
	</tr>
	<tr>
		<td class=header2>Forum</td>
		<td class=header2>Access Mask</td>
	</tr>
</HeaderTemplate>
<ItemTemplate>
	<tr>
		<td class="postheader">
			<asp:label id=ForumID visible=false runat="server" text='<%# Eval( "ForumID") %>'></asp:label>
			<b><%# Eval( "ForumName") %></b><br>
			Category: <%# Eval( "CategoryName") %>
		</td>
		<td class="post">
			<asp:dropdownlist runat="server" id="AccessMaskID" ondatabinding="BindData_AccessMaskID" onprerender="SetDropDownIndex" value='<%# Eval("AccessMaskID") %>'/>
			...
		</td>
	</tr>
</ItemTemplate>
</asp:repeater>
  <tr>
    <td class=postfooter align=middle colspan=11><asp:button id=Save runat="server" Text="Save" onclick="Save_Click"></asp:button>&nbsp; 
<asp:button id=Cancel runat="server" Text="Cancel" onclick="Cancel_Click"></asp:button></td></tr></table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
