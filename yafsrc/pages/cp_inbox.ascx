<%@ Control language="c#" Codebehind="cp_inbox.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.cp_inbox" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=6><%# GetText(IsSentItems ? "sentitems" : "title") %></td>
</tr>
<tr class=header2>
	<td>&nbsp;</td>
	<td><img runat="server" id="SortSubject" align="absmiddle"/> <asp:linkbutton runat="server" id="SubjectLink"/></td>
	<td><img runat="server" id="SortFrom" align="absmiddle"/> <asp:linkbutton runat="server" id="FromLink"/></td>
	<td><img runat="server" id="SortDate" align="absmiddle"/> <asp:linkbutton runat="server" id="DateLink"/></td>
	<td>&nbsp;</td>
</tr>

<asp:repeater id=Inbox runat=server>
<FooterTemplate>
	<tr class=footer1>
		<td colspan="6" align="right"><asp:button runat="server" onload="DeleteSelected_Load" commandname="delete" text='<%# GetText("deleteselected") %>'/></td>
	</tr>
	</table>
</FooterTemplate>
<ItemTemplate>
	<tr class=post>
		<td align="center"><img src='<%# GetImage(Container.DataItem) %>'/></td>
		<td><a href='<%# yaf.Forum.GetLink(yaf.Pages.cp_message,"pm={0}",DataBinder.Eval(Container.DataItem,"UserPMessageID")) %>'><%# DataBinder.Eval(Container.DataItem,"Subject") %></a></td>
		<td><%# DataBinder.Eval(Container.DataItem,IsSentItems ? "ToUser" : "FromUser") %></td>
		<td><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %></td>
		<td align="center"><asp:checkbox runat="server" id="ItemCheck" /></td>
		<asp:label runat="server" id="UserPMessageID" visible="false" text='<%# DataBinder.Eval(Container.DataItem,"UserPMessageID") %>'/>
	</tr>
</ItemTemplate>
</asp:repeater>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
