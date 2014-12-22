<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.accessmasks" Codebehind="accessmasks.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="13">
				  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ACCESSMASKS" />
			</td>
		</tr>
		<tr class="header2">
			<td>
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NAME"  LocalizedPage="ADMIN_ACCESSMASKS" />
			</td>			
			<td>
				&nbsp;
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server" OnItemCommand="List_ItemCommand">
			<ItemTemplate>
				<tr class="postheader">
					<td>
					  <img alt='<%# Eval( "Name") %>'
                                    title='<%# Eval( "Name") %>'
                                    src='<%# this.Get<ITheme>().GetItem("VOTE","POLL_MASK") %>' />&nbsp;<%# Eval( "Name") %>
					</td>					
					<td width="15%" style="font-weight: normal" align="right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" 
                            CommandName='edit' CommandArgument='<%# Eval( "AccessMaskID") %>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON"
                            TextLocalizedTag="EDIT" 
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# Eval( "AccessMaskID") %>' 
                                    TitleLocalizedTag="DELETE" 
                                    ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
					</td>
				</tr>
                <tr class="post">
                    <td align="center" colspan = 2>
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="READ"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
					    <asp:Label ID="Label1" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %>'><%# GetItemName(BitSet(Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %></asp:Label>&nbsp;|&nbsp; 
                       
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="POST"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label2" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %>'><%# GetItemName(BitSet(Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %></asp:Label>&nbsp;|&nbsp;
                        
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="REPLY"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
                        <asp:Label ID="Label3" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %>'><%# GetItemName(BitSet(Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %></asp:Label>&nbsp;|&nbsp;
					 	
                        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="PRIORITY"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
                        <asp:Label ID="Label4" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %>'><%# GetItemName(BitSet(Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %></asp:Label>&nbsp;|&nbsp;
                   <br />
				
                    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="POLL"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
					<asp:Label ID="Label5" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.PollAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.PollAccess)) %></asp:Label>&nbsp;|&nbsp;
				
					<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="VOTE"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label6" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.VoteAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.VoteAccess)) %></asp:Label>&nbsp;|&nbsp;
					
				<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="MODERATOR"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label7" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %></asp:Label>&nbsp;|&nbsp;
				
					<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="EDIT"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label8" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.EditAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.EditAccess)) %></asp:Label>&nbsp;|&nbsp;
				 <br />	
				<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="DELETE"  LocalizedPage="ADMIN_ACCESSMASKS"/>&nbsp;
						<asp:Label ID="Label9" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.DeleteAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.DeleteAccess)) %></asp:Label>&nbsp;|&nbsp;
				
				<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="UPLOAD"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label10" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.UploadAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.UploadAccess)) %></asp:Label>&nbsp;|&nbsp;
					
				<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="DOWNLOAD"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label11" runat="server" ForeColor='<%# GetItemColor(BitSet(Eval("Flags"),(int)AccessFlags.Flags.DownloadAccess)) %>'><%# GetItemName(BitSet(Eval( "Flags"),(int)AccessFlags.Flags.DownloadAccess)) %></asp:Label>
					</td>
                </tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1" align="center">
			<td colspan="13">
				<asp:Button ID="New" runat="server" OnClick="New_Click" CssClass="pbutton" />
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
