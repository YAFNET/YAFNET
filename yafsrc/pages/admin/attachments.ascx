<%@ Control language="c#" Codebehind="attachments.ascx.cs" AutoEventWireup="True" Inherits="yaf.pages.admin.attachments" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="8">Attachments</td>
</tr>

<asp:repeater runat="server" id="List">
	<HeaderTemplate>
		<tr class="header2">
			<td>Forum</td>
			<td>Topic</td>
			<td>Posted</td>
			<td>File Name</td>
			<td align="right">Downloads</td>
			<td>Content Type</td>
			<td align="right">Size</td>
			<td>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr class=post>
			<td><a target="_top" href='<%# yaf.Forum.GetLink(yaf.Pages.topics,"f={0}",Eval("ForumID")) %>'><%# Eval("ForumName") %></a></td>
			<td><a target="_top" href='<%# yaf.Forum.GetLink(yaf.Pages.posts,"t={0}",Eval("TopicID")) %>'><%# Eval("TopicName") %></a></td>
			<td><%# FormatDateTimeShort(Eval( "Posted")) %></td>
			<td><%# Eval( "FileName") %></td>
			<td align="right"><%# Eval( "Downloads") %></td>
			<td><%# Eval( "ContentType") %></td>
			<td align="right"><%# Eval( "Bytes") %></td>
			<td>
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "AttachmentID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

</table>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
