<%@ Control language="c#" Codebehind="search.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.search" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

	<yaf:PageLinks runat="server" id="PageLinks"/>
	
	<table class="command" cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td class="navlinks" align="left" id="PageLinks1" runat="server"></td>
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
			<td class="postheader" colspan="2" align="center"><asp:textbox id="txtSearchString" runat="server" Width="293px"></asp:textbox><asp:button id="btnSearch" runat="server"/></td>
		</tr>
		<asp:repeater id="SearchRes" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2" colspan="2"><%= GetText("results") %></td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
					<td class="largefont" id="CounterCol" rowspan="2" runat="server"></td>
					<td class="postheader">
						<b><%= GetText("topic") %></b> <a href='<%# yaf.Forum.GetLink(yaf.Pages.posts,"t={0}",DataBinder.Eval(Container.DataItem, "TopicID")) %>'><%# DataBinder.Eval(Container.DataItem, "Topic") %></a><br/>
						<b><%= GetText("posted") %></b> <%# DataBinder.Eval(Container.DataItem, "Posted") %>
						<%= GetText("SEARCH","BY") %> <a href='<%# yaf.Forum.GetLink(yaf.Pages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
					</td>
				</tr>
				<tr class="post">
					<td class="largefont" width="99%">
						<%# FormatMessage(Container.DataItem) %>
					</td>
				</tr>
			</ItemTemplate>
		</asp:repeater>
	</table>
	<table class="command" width="100%" cellspacing="0" cellpadding="0">
		<tr>
			<td align="left" class="navlinks" id="PageLinks2" runat="server"></td>
		</tr>
	</table>

<yaf:savescrollpos runat="server"/>
