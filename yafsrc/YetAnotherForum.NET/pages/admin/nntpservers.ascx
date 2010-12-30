<%@ Control language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpservers" Codebehind="nntpservers.ascx.cs" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class="content" width="100%" cellspacing="1" cellpadding="0">
<tr>
	<td class="header1" colspan="6">
    <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" /></td>
</tr>

<asp:Repeater id="RankList" runat="server">
	<HeaderTemplate>
		<tr>
			<td class="header2"><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" /></td>
			<td class="header2"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADRESS" LocalizedPage="ADMIN_NNTPSERVERS" /></td>
			<td class="header2"><YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" LocalizedPage="ADMIN_NNTPSERVERS" /></td>
			<td class="header2">&nbsp;</td>
		</tr>
	</HeaderTemplate>
	<ItemTemplate>
		<tr>
			<td class="post">
				<%# Eval( "Name") %>
			</td>
			<td class="post">
				<%# Eval( "Address") %>
			</td>
			<td class="post">
				<%# Eval( "UserName") %>
			</td>
			<td class="post">
				<asp:linkbutton runat="server" commandname="edit" commandargument='<%# Eval( "NntpServerID") %>'><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="EDIT" LocalizedPage="ADMIN_NNTPFORUMS" /></asp:linkbutton>
				|
				<asp:linkbutton runat="server" onload="Delete_Load" commandname="delete" commandargument='<%# Eval( "NntpServerID") %>'><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE" LocalizedPage="ADMIN_NNTPFORUMS" /></asp:linkbutton>
			</td>
		</tr>
	</ItemTemplate>
</asp:Repeater>

<tr>
	<td class="footer1" colspan="5"><asp:linkbutton id="NewServer" runat="server" text="New Server" onclick="NewServer_Click" /></td>
</tr>
</table>

</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
