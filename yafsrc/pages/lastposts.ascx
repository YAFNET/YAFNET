<%@ Control Language="c#" AutoEventWireup="True" Codebehind="lastposts.ascx.cs" Inherits="YAF.Pages.lastposts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<asp:repeater id="repLastPosts" runat="server" visible="true">
<HeaderTemplate>
	<table class="content" cellSpacing="1" cellPadding="0" width="100%" align="center">
		<tr>
			<td class=header2 align=middle colspan="2"><%# GetText("last10") %></td>
		</tr>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<ItemTemplate>
		<tr class="postheader">
			<td width="140"><b><a href="<%# YAF.Forum.GetLink(YAF.ForumPages.profile,"u={0}",Eval( "UserID")) %>"><%# Eval( "UserName") %></a></b>
			</td>
			<td width="80%" class="small" align="left"><b><%# GetText("posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %></td>
		</tr>
		<tr class="post">
			<td>&nbsp;</td>
			<td valign="top" class="message">
				<%# FormatBody(Container.DataItem) %>
			</td>
		</tr>
</ItemTemplate>
<AlternatingItemTemplate>
		<tr class="postheader">
			<td width="140"><b><a href="<%# YAF.Forum.GetLink(YAF.ForumPages.profile,"u={0}",Eval( "UserID")) %>"><%# Eval( "UserName") %></a></b>
			</td>
			<td width="80%" class="small" align="left"><b><%# GetText("posted") %></b> <%# FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %></td>
		</tr>
		<tr class="post_alt">
			<td>&nbsp;</td>
			<td valign="top" class="message">
				<%# FormatBody(Container.DataItem) %>
			</td>
		</tr>
</AlternatingItemTemplate>
</asp:repeater>