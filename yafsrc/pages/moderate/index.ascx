<%@ Control language="c#" Codebehind="index.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.moderate.index" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>

<YAF:PageLinks runat="server" id="PageLinks"/>

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
				<b><a href='<%# YAF.Classes.Utils.yaf_BuildLink.GetLink(YAF.Classes.Utils.ForumPages.moderate_forum,"f={0}",Eval("ForumID")) %>'><%# Eval("ForumName") %></a></b><br/>
				<%# GetText("MODERATE_DEFAULT","CATEGORY") %>: <%# Eval("CategoryName") %>
			</td>
			<td align="center"><%# Eval("MessageCount") %></td>
		</tr>
</ItemTemplate>
</asp:repeater>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
