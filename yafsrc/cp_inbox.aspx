<%@ Page language="c#" Codebehind="cp_inbox.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp.cp_inbox" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=UserLink runat="server"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<table class=content cellspacing=1 cellpadding=0 width=100%>
<tr>
	<td class=header1 colspan=6><%# GetText(IsSentItems ? "cp_inbox_sentitems" : "cp_inbox_title") %></td>
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
		<td colspan="6" align="right"><asp:button runat="server" onload="DeleteSelected_Load" commandname="delete" text='<%# GetText("cp_inbox_deleteselected") %>'/></td>
	</tr>
	</table>
</FooterTemplate>
<ItemTemplate>
	<tr class=post>
		<td align="center"><img src='<%# GetImage(Container.DataItem) %>'/></td>
		<td><a href='cp_message.aspx?m=<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'><%# DataBinder.Eval(Container.DataItem,"Subject") %></a></td>
		<td><%# DataBinder.Eval(Container.DataItem,IsSentItems ? "ToUser" : "FromUser") %></td>
		<td><%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %></td>
		<td align="center"><asp:checkbox runat="server" id="ItemCheck" /></td>
		<asp:label runat="server" id="PMessageID" visible="false" text='<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'/>
	</tr>
</ItemTemplate>
</asp:repeater>

</form>
