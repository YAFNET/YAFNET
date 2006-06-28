<%@ Control language="c#" Codebehind="forum.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.moderate.forum" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<asp:repeater id="List" runat="server">
<HeaderTemplate>
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td colspan="2" class=header1 align=left><%# GetText("MODERATE_FORUM","UNAPPROVED") %></td>
		</tr>
</HeaderTemplate>
<FooterTemplate>
	</table>
</FooterTemplate>
<ItemTemplate>
		<tr class="header2">
			<td colspan="2"><%# Eval("Topic") %></td>
		</tr>
		<tr class="postheader">
			<td><%# Eval("UserName") %></td>
			<td><b>Posted:</b> <%# Eval("Posted") %></td>
		</tr>
		<tr class="post">
			<td valign="top" width="140">&nbsp;</td>
			<td valign="top" class="message">
				<%# FormatMessage((System.Data.DataRowView)Container.DataItem) %>
			</td>
		</tr>
		<tr class=postfooter>
			<td class=small>
				<a href="javascript:scroll(0,0)"><%# GetText("MODERATE_FORUM","TOP") %></a>
			</td>
			<td class="postfooter">
				<asp:linkbutton runat="server" text='<%# GetText("MODERATE_FORUM","APPROVE") %>' commandname="Approve" commandargument='<%# Eval("MessageID") %>'/>&nbsp;
				<asp:linkbutton runat="server" text='<%# GetText("MODERATE_FORUM","DELETE") %>' commandname="Delete" commandargument='<%# Eval("MessageID") %>' onload="Delete_Load"/>&nbsp;
			</td>
		</tr>
</ItemTemplate>
<SeparatorTemplate>
		<tr class="postsep"><td colspan="2" style="height:7px"></td></tr>
</SeparatorTemplate>
</asp:repeater>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
