<%@ Page language="c#" Codebehind="attachments.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.attachments" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<form runat="server" enctype="multipart/form-data">

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
			<td><a target="_top" href='../topics.aspx?f=<%# DataBinder.Eval(Container.DataItem,"ForumID") %>'><%# DataBinder.Eval(Container.DataItem,"ForumName") %></a></td>
			<td><a target="_top" href='../posts.aspx?t=<%# DataBinder.Eval(Container.DataItem,"TopicID") %>'><%# DataBinder.Eval(Container.DataItem,"TopicName") %></a></td>
			<td><%# FormatDateTimeShort(DataBinder.Eval(Container.DataItem, "Posted")) %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "FileName") %></td>
			<td align="right"><%# DataBinder.Eval(Container.DataItem, "Downloads") %></td>
			<td><%# DataBinder.Eval(Container.DataItem, "ContentType") %></td>
			<td align="right"><%# DataBinder.Eval(Container.DataItem, "Bytes") %></td>
			<td>
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "AttachmentID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

</table>

<yaf:savescrollpos runat="server"/>
</form>