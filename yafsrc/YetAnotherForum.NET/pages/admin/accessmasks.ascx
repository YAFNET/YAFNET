<%@ Control Language="c#" CodeFile="accessmasks.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.accessmasks" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="13">
				Access Masks
			</td>
		</tr>
		<tr class="header2">
			<td>
				Name
			</td>
			<td align="center">
				Read
			</td>
			<td align="center">
				Post
			</td>
			<td align="center">
				Reply
			</td>
			<td align="center">
				Priority
			</td>
			<td align="center">
				Poll
			</td>
			<td align="center">
				Vote
			</td>
			<td align="center">
				Moderator
			</td>
			<td align="center">
				Edit
			</td>
			<td align="center">
				Delete
			</td>
			<td align="center">
				Upload
			</td>
			<td align="center">
				Download
			</td>
			<td>
				&nbsp;
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server" OnItemCommand="List_ItemCommand">
			<ItemTemplate>
				<tr class="post">
					<td>
						<%# Eval( "Name") %>
					</td>
					<td align="center">
						<asp:Label ID="Label1" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReadAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label2" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PostAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PostAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label3" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReplyAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ReplyAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label4" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PriorityAccess)) %>'><%# BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PriorityAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label5" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PollAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.PollAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label6" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.VoteAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.VoteAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label7" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.ModeratorAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label8" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.EditAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.EditAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label9" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DeleteAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DeleteAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label10" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.UploadAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.UploadAccess) %></asp:Label>
					</td>
					<td align="center">
						<asp:Label ID="Label11" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DownloadAccess)) %>'><%# BitSet(Eval( "Flags"),(int)YAF.Classes.Data.AccessFlags.Flags.DownloadAccess) %></asp:Label>
					</td>
					<td width="15%" style="font-weight: normal">
						<asp:LinkButton runat='server' CommandName='edit' CommandArgument='<%# Eval( "AccessMaskID") %>'>Edit</asp:LinkButton>
						|
						<asp:LinkButton runat='server' OnLoad="Delete_Load" CommandName='delete' CommandArgument='<%# Eval( "AccessMaskID") %>'>Delete</asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1">
			<td colspan="13">
				<asp:LinkButton ID="New" runat="server" Text="New Access Mask" OnClick="New_Click" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
