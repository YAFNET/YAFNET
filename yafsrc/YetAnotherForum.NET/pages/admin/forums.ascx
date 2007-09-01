<%@ Control language="c#" CodeFile="forums.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.forums" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class=header1 colspan=3>Forums</td>
</tr>

<asp:repeater id=CategoryList runat="server">
<ItemTemplate>
		<tr>
			<td class=header2>
				<%# Eval( "Name") %>
			</td>
			<td class=header2 width=10% align=center><%# Eval( "SortOrder") %></td>
			<td class=header2 width=15% style="font-weight:normal">
				<asp:linkbutton runat='server' commandname='edit' commandargument='<%# Eval( "CategoryID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat='server' onload="DeleteCategory_Load" commandname='delete' commandargument='<%# Eval( "CategoryID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
		<asp:Repeater id=ForumList OnItemCommand="ForumList_ItemCommand" runat="server" datasource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
			<ItemTemplate>
				<tr class=post>
					<td align=left><b><%# DataBinder.Eval(Container.DataItem, "[\"Name\"]") %></b><br /><%# DataBinder.Eval(Container.DataItem, "[\"Description\"]") %></td>
					<td align=center><%# DataBinder.Eval(Container.DataItem, "[\"SortOrder\"]") %></td>
					<td>
						<asp:linkbutton runat='server' commandname='edit' commandargument='<%# Eval( "[\"ForumID\"]") %>'>Edit</asp:linkbutton>
						|
						<asp:linkbutton runat='server' onload="DeleteForum_Load" commandname='delete' commandargument='<%# Eval( "[\"ForumID\"]") %>'>Delete</asp:linkbutton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
	
</ItemTemplate>
</asp:repeater>
  <tr>
    <td class=footer1 colSpan=3><asp:linkbutton id=NewCategory runat="server" onclick="NewCategory_Click">New Category</asp:linkbutton>
		|
		<asp:LinkButton id=NewForum runat="server" onclick="NewForum_Click">New Forum</asp:LinkButton></td></tr></table>
		
</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
