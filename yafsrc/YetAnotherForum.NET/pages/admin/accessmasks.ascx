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
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PostAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PriorityAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PollAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.VoteAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.EditAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DeleteAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.UploadAccess) %></td>
			<td align="center"><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DownloadAccess) %></td>
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
