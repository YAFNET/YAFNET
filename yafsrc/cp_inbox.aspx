<%@ Page language="c#" Codebehind="cp_inbox.aspx.cs" AutoEventWireup="false" Inherits="yaf.cp.cp_inbox" %>

<form runat=server>

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=UserLink runat="server"/>
	&#187; <asp:hyperlink runat="server" id="ThisLink"/>
</p>

<asp:repeater id=Inbox runat=server>
<HeaderTemplate>
	<table class=content cellspacing=1 cellpadding=0 width=100%>
	<tr>
		<td class=header1 colspan=2><%# GetText("cp_inbox_title") %></td>
	</tr>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<SeparatorTemplate>
	<tr class="postsep"><td colspan="2" style="height:7px"></td></tr>
</SeparatorTemplate>
<ItemTemplate>
	<tr>
		<td class=header2 colspan=2><%# DataBinder.Eval(Container.DataItem,"Subject") %></td>
	</tr>
	<tr>
		<td class=postheader><%# DataBinder.Eval(Container.DataItem,"FromUser") %></td>
		<td class=postheader>
			<table cellspacing=0 cellpadding=0 width=100%><tr>
			<td>
				<b><%= GetText("cp_inbox_posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Created"]) %>
			</td>
			<td align=right>
				<asp:linkbutton runat=server commandname=delete commandargument='<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'><img src='<%# ThemeFile("b_delete_post.png") %>' title='Delete this message'></asp:linkbutton>
				<asp:linkbutton runat=server commandname=reply commandargument='<%# DataBinder.Eval(Container.DataItem,"PMessageID") %>'><img src='<%# ThemeFile("b_quote_post.png") %>' title='Reply to this message'></asp:linkbutton>
			</td>
			</tr></table>
		</td>
	</tr>
	<tr>
		<td class=post>&nbsp;</td>
		<td class=post valign=top><%# FormatBody(Container.DataItem) %></td>
	</tr>
</ItemTemplate>
</asp:repeater>

</form>
