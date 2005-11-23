<%@ Control language="c#" Codebehind="nntpretrieve.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.nntpretrieve" %>
<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>

<yaf:PageLinks runat="server" id="PageLinks"/>

<yaf:adminmenu runat="server">

<table class=content width="100%" cellspacing=1 cellpadding=0>
<tr class="header1">
	<td colspan="3">Retrieve NNTP Articles</td>
</tr>

<asp:repeater runat="server" id="List">
<HeaderTemplate>
<tr class="header2">
	<td>Groups ready for retrieval</td>
	<td align="right">Last Message</td>
	<td>Last Update</td>
</tr>
</HeaderTemplate>
<ItemTemplate>
<tr class="post">
	<td><%# DataBinder.Eval(Container.DataItem,"GroupName") %></td>
	<td align="right"><%# LastMessageNo(Container.DataItem) %></td>
	<td><%# FormatDateTime(DataBinder.Eval(Container.DataItem,"LastUpdate")) %></td>
</tr>
</ItemTemplate>
</asp:repeater>
<tr>
	<td colspan="2" class="postheader" width="50%">Specify how much time article retrieval should use.</td>
	<td class="post" width="50%"><asp:textbox runat="server" id="Seconds" text="30"/>&nbsp;seconds</td>
</tr>
<tr class="footer1">
	<td colspan="3" align="center"><asp:button runat="server" id="Retrieve" text="Retrieve"/></td>
</tr>
</table>

<p style="color:red">
The NNTP feature of Yet Another Forum.net is still beta, and <b>will</b> have bugs. Bugs you should
look for is character set conversion, and incorrect time on posts. Usenet articles will be posted to
the forum with the date from the article header, meaning that you well could get future dates. Please let
me know of any unknown time zones that are encountered.
</p>

<p style="color:red">
The forums usenet artiles are posted to should be read-only, as messages will <b>not</b> be posted
back to the usenet. For speed purposes usenet articles are automatically approved, meaning that if the
forum is moderated, the posts will be automatically approved.
</p>

<p style="color:red">
To protect usenet servers, a newsgroup can only be updated once every 10 minutes.
</p>

</yaf:adminmenu>

<yaf:SmartScroller id="SmartScroller1" runat = "server" />
