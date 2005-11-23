<%@ Control language="c#" Inherits="yaf.pages.moderate.index" CodeFile="index.ascx.cs" CodeFileBaseClass="yaf.pages.ForumPage" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<asp:repeater id="List" runat="server">
<HeaderTemplate>
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class=header1 width=1%>&nbsp;</td>
			<td class=header1 align=left><%# GetText("MODERATE_DEFAULT","FORUM") %></td>
			<td class=header1 align=center width=7%><%# GetText("MODERATE_DEFAULT","UNAPPROVED") %></td>
		</tr>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<ItemTemplate>
		<tr class="post">
			<td><img src='<%# GetThemeContents("ICONS","FORUM") %>'/></td>
			<td>
				<b><a href='<%# yaf.Forum.GetLink(yaf.Pages.moderate_forum,"f={0}",DataBinder.Eval(Container.DataItem,"ForumID")) %>'><%# DataBinder.Eval(Container.DataItem,"ForumName") %></a></b><br/>
				<%# GetText("MODERATE_DEFAULT","CATEGORY") %>: <%# DataBinder.Eval(Container.DataItem,"CategoryName") %>
			</td>
			<td align="center"><%# DataBinder.Eval(Container.DataItem,"MessageCount") %></td>
		</tr>
</ItemTemplate>
</asp:repeater>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
