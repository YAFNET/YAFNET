<%@ Page language="c#" Codebehind="attachments.aspx.cs" AutoEventWireup="false" Inherits="yaf.attachments" %>
<form runat="server" enctype="multipart/form-data">

<p class=navlinks>
	<asp:hyperlink id=HomeLink runat="server"/>
	&#187; <asp:hyperlink id=CategoryLink runat="server"/>
	&#187; <asp:hyperlink id=ForumLink runat="server"/>
	&#187; <asp:hyperlink id=TopicLink runat="server"/>
	&#187; <asp:hyperlink id=ThisLink runat="server"/>
</P>

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr>
	<td class=header1 colspan=3><%= GetText("TITLE") %></td>
</tr>

<asp:repeater runat="server" id="List">
	<HeaderTemplate>
		<tr>
			<td class=header2><%# GetText("FILENAME") %></td>
			<td class=header2 align="right"><%# GetText("SIZE") %></td>
			<td class=header2>&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class=post>
				<%# DataBinder.Eval(Container.DataItem, "FileName") %>
			</td>
			<td class=post align="right">
				<%# DataBinder.Eval(Container.DataItem, "Bytes") %>
			</td>
			<td class=post>
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# DataBinder.Eval(Container.DataItem, "AttachmentID") %>'><%# GetText("DELETE") %></asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:repeater>

<tr>
	<td class=header2><%= GetText("UPLOAD_TITLE") %></td>
	<td class=header2>&nbsp;</td>
	<td class=header2>&nbsp;</td>
</tr>
<tr>
	<td class=postheader><%= GetText("SELECT_FILE") %></td>
	<td class=post><input type="file" id="File" runat="server"/></td>
	<td class=post><asp:button runat="server" id="Upload"/></td>
</tr>

<tr class="footer1">
	<td colspan="3" align="center"><asp:button runat="server" id="Back"/></td>
</tr>
</table>


</form>