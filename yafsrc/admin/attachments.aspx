<%@ Page language="c#" Codebehind="attachments.aspx.cs" AutoEventWireup="false" Inherits="yaf.admin.attachments" %>
<form runat="server" enctype="multipart/form-data">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan="6">Attachments</td>
</tr>

<asp:repeater runat="server" id="List">
	<HeaderTemplate>
		<tr class="header2">
			<td>Forum</td>
			<td>Topic</td>
			<td>Posted</td>
			<td>File Name</td>
			<td align="right">Size</td>
			<td>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class="post"><a target="_top" href='../topics.aspx?f=<%# DataBinder.Eval(Container.DataItem,"ForumID") %>'><%# DataBinder.Eval(Container.DataItem,"ForumName") %></a></td>
			<td class="post"><a target="_top" href='../posts.aspx?t=<%# DataBinder.Eval(Container.DataItem,"TopicID") %>'><%# DataBinder.Eval(Container.DataItem,"TopicName") %></a></td>
			<td class=post><%# FormatDateTime(DataBinder.Eval(Container.DataItem, "Posted")) %></td>
			<td class=post><%# DataBinder.Eval(Container.DataItem, "FileName") %></td>
			<td class=post align="right"><%# DataBinder.Eval(Container.DataItem, "Bytes") %></td>
			<td class=post>
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "AttachmentID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

</table>


</form>