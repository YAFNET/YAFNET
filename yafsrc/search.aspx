<%@ Page language="c#" Codebehind="search.aspx.cs" AutoEventWireup="false" Inherits="yaf.search" %>
<%@ Register TagPrefix="uc1" TagName="forumjump" Src="forumjump.ascx" %>
<form runat="server">
	<p class="navlinks">
		<asp:hyperlink id="HomeLink" runat="server"/>
		&#187; <asp:hyperlink runat="server" id="ThisLink"/>
	</p>
	<table class="command" cellspacing="0" cellpadding="0" width="100%">
		<tr>
			<td class="navlinks" align="left" id="PageLinks1" runat="server"></td>
		</tr>
	</table>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%">
		<tr>
			<td class="header1" colspan="2"><%= GetText("search_title") %></td>
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
					<td class="header2" colspan="2"><%= GetText("search_results") %></td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
					<td class="largefont" id="CounterCol" rowspan="2" runat="server"></td>
					<td class="postheader">
						<b><%= GetText("search_topic") %></b> <a href='posts.aspx?t=<%# DataBinder.Eval(Container.DataItem, "TopicID") %>'><%# DataBinder.Eval(Container.DataItem, "Topic") %></a><br/>
						<b><%= GetText("search_posted") %></b> <%# DataBinder.Eval(Container.DataItem, "Posted") %>
						by <a href='profile.aspx?u=<%# DataBinder.Eval(Container.DataItem, "UserID") %>'><%# DataBinder.Eval(Container.DataItem, "Name") %></a>
					</td>
				</tr>
				<tr class="post">
					<td class="largefont" width="99%">
						<%# FormatMessage( DataBinder.Eval(Container.DataItem, "Message") ) %>
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
</form>
