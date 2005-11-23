<%@ Control language="c#" Inherits="yaf.pages.admin.accessmasks" CodeFile="accessmasks.ascx.cs" CodeFileBaseClass="yaf.AdminPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class="header1" colspan="12">Access Masks</td>
</tr>
<tr class="header2">
	<td>Name</td>
	<td align="center">Read</td>
	<td align="center">Post</td>
	<td align="center">Reply</td>
	<td align="center">Priority</td>
	<td align="center">Poll</td>
	<td align="center">Vote</td>
	<td align="center">Moderator</td>
	<td align="center">Edit</td>
	<td align="center">Delete</td>
	<td align="center">Upload</td>
	<td>&nbsp;</td>
</tr>

<asp:repeater id="List" runat="server">
<ItemTemplate>
		<tr class="post">
			<td>
				<%# DataBinder.Eval(Container.DataItem, "Name") %>
			</td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),1) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),2) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),4) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),8) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),16) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),32) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),64) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),128) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),256) %></td>
			<td align="center"><%# BitSet(DataBinder.Eval(Container.DataItem, "Flags"),512) %></td>
			<td width=15% style="font-weight:normal">
				<asp:linkbutton runat='server' commandname='edit' commandargument='<%# DataBinder.Eval(Container.DataItem, "AccessMaskID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat='server' onload="Delete_Load" commandname='delete' commandargument='<%# DataBinder.Eval(Container.DataItem, "AccessMaskID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	
</ItemTemplate>
</asp:repeater>
<tr class="footer1">
	<td colSpan="12">
		<asp:linkbutton id="New" runat="server" text="New Access Mask" onclick="New_Click" />
	</td>
</tr>
</table>
		
</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
