<%@ Control language="c#" CodeFile="accessmasks.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.accessmasks" %>





<YAF:PageLinks runat="server" id="PageLinks"/>

<YAF:adminmenu runat="server">

<table class=content cellSpacing=1 cellPadding=0 width="100%">
<tr>
	<td class="header1" colspan="13">Access Masks</td>
</tr>
<tr class="header2">
	<td>Name</td>
	<td align="center">Read</td>
	<td align="center">Post</td>
	<td align="center">Reply</td>
	<td align="center">Priority</td>
	<td align="center">Poll</td>
	<td align="center">Vote</td>
	<td align="center">Moderator</td>
	<td align="center">Edit</td>
	<td align="center">Delete</td>
	<td align="center">Upload</td>
	<td align="center">Download</td>
	<td>&nbsp;</td>
</tr>

<asp:repeater id="List" runat="server">
<ItemTemplate>
		<tr class="post">
			<td>
				<%# Eval( "Name") %>
			</td>
			<td align="center"><asp:label ID="Label1" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label2" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PostAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PostAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label3" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReplyAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReplyAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label4" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PriorityAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PriorityAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label5" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PollAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PollAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label6" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.VoteAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.VoteAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label7" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label8" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.EditAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.EditAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label9" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DeleteAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DeleteAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label10" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.UploadAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.UploadAccess) %></asp:label></td>
			<td align="center"><asp:label ID="Label11" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DownloadAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DownloadAccess) %></asp:label></td>
			<td width=15% style="font-weight:normal">
				<asp:linkbutton runat='server' commandname='edit' commandargument='<%# Eval( "AccessMaskID") %>'>Edit</asp:linkbutton>
				|
				<asp:linkbutton runat='server' onload="Delete_Load" commandname='delete' commandargument='<%# Eval( "AccessMaskID") %>'>Delete</asp:linkbutton>
			</td>
		</tr>
	
</ItemTemplate>
</asp:repeater>
<tr class="footer1">
	<td colSpan="13">
		<asp:linkbutton id="New" runat="server" text="New Access Mask" onclick="New_Click" />
	</td>
</tr>
</table>
		
</YAF:adminmenu>

<YAF:SmartScroller id="SmartScroller1" runat = "server" />
