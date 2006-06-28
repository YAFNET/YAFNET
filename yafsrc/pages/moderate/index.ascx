<%@ Control language="c#" Codebehind="index.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.moderate.index" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

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
				<b><a href='<%# yaf.Forum.GetLink(yaf.Pages.moderate_forum,"f={0}",Eval("ForumID")) %>'><%# Eval("ForumName") %></a></b><br/>
				<%# GetText("MODERATE_DEFAULT","CATEGORY") %>: <%# Eval("CategoryName") %>
			</td>
			<td align="center"><%# Eval("MessageCount") %></td>
		</tr>
</ItemTemplate>
</asp:repeater>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
