<%@ Control language="c#" Codebehind="search.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.search" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

	<yaf:PageLinks runat="server" id="PageLinks"/>
	<script language="javascript">
function doSearch() {
   if (window.event.keyCode == 13) _ctl0._ctl1__ctl0_btnSearch.focus();
}
</script>
	<table class="command" cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td class="navlinks"><yaf:pager runat="server" id="Pager"/></td>
		</tr>
	</table>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2"><%= GetText("title") %></td>
		</tr>
		<tr>
			<td class="postheader" colspan="2" align="center">
				<asp:dropdownlist id="listForum" runat="server"/>
				<asp:dropdownlist id="listResInPage" runat="server"/>
				<asp:dropdownlist id="listSearchWhere" runat="server"/>
				<asp:dropdownlist id="listSearchWath" runat="server"/>
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2" align="center"><asp:textbox id="txtSearchString" runat="server" Width="293px" onkeypress="doSearch();"></asp:textbox><asp:button id="btnSearch" runat="server"/></td>
		</tr>
		<asp:repeater id="SearchRes" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2" colspan="2"><%= GetText("results") %></td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
					<td class="largefont" id="CounterCol" align="center" rowspan="2" runat="server"><%# DataBinder.Eval(Container.DataItem, "MessageID") %></td>
					<td class="postheader" width="99%">
						<b><%= GetText("topic") %></b> <a href='<%# yaf.Forum.GetLink(yaf.Pages.posts,"t={0}",DataBinder.Eval(Container.DataItem, "TopicID")) %>'><%# DataBinder.Eval(Container.DataItem, "Topic") %></a><br/>
						<b><%= GetText("posted") %></b> <%# DataBinder.Eval(Container.DataItem, "Posted") %>
						<%= GetText("SEARCH","BY") %> <a href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
					</td>
				</tr>
				<tr class="post">
					<td width="99%">
						<%# FormatMessage(Container.DataItem) %>
					</td>
				</tr>
			</ItemTemplate>
		</asp:repeater>
	</table>
	<table class="command" width="100%" cellspacing="0" cellpadding="0">
		<tr>
			<td class="navlinks"><yaf:pager runat="server" linkedpager="Pager"/></td>
		</tr>
	</table>

<yaf:savescrollpos runat="server"/>
