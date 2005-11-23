<%@ Control language="c#" Inherits="yaf.pages.admin.nntpforums" CodeFile="nntpforums.ascx.cs" CodeFileBaseClass="yaf.AdminPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=6>NNTP Forums</td>
</tr>

<asp:repeater id=RankList runat="server">
	<HeaderTemplate>
		<tr>
			<td class=header2>Server</td>
			<td class=header2>Group</td>
			<td class=header2>Forum</td>
			<td class=header2>Active</td>
			<td class=header2>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "Name") %>
			</td>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "GroupName") %>
			</td>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "ForumName") %>
			</td>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "Active") %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# DataBinder.Eval(Container.DataItem, "NntpForumID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "NntpForumID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=footer1 colspan=5><asp:linkbutton id=NewForum runat="server" text="New Forum" onclick="NewForum_Click" /></td>
</tr>
</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
