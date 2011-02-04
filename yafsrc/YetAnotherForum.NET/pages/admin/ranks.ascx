<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ranks" Codebehind="ranks.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="6">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_RANKS" />
            </td>
		</tr>
		<asp:Repeater ID="RankList" OnItemCommand="RankList_ItemCommand" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="COMMON" />
                    </td>			
					<td class="header2">
						&nbsp;
                    </td>                   
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="header2">
						<%# Eval( "Name") %>
					</td>
                    <td class="header2">
						&nbsp;
                    </td>
                 </tr>
                 <tr>
					<td class="post">
                     <YAF:LocalizedLabel ID="HelpLabel6" Visible='<%# Eval("Description").ToString().IsSet() %>' runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITGROUP">
                         </YAF:LocalizedLabel>
                          &nbsp;<%# Eval("Description").ToString() %>&nbsp; 
                    <br />
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_RANKS" />
                     <asp:Label ID="Label4" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %></asp:Label>&nbsp;|&nbsp;				
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="IS_LADDER" LocalizedPage="ADMIN_RANKS" />
                    <asp:Label ID="Label1" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(RankFlags.Flags.IsLadder)) %>'><%# LadderInfo(Container.DataItem) %></asp:Label>&nbsp;|&nbsp;				
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_LIMIT" LocalizedPage="ADMIN_RANKS" />
						<%# Eval( "PMLimit" ) %>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel ID="HelpLabel7" runat="server" LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />                   
                    <%# Convert.ToInt32(Eval("UsrSigChars")) %>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
                    <%# Eval("UsrSigBBCodes").ToString()%>&nbsp;|&nbsp; 
                    <YAF:LocalizedLabel ID="HelpLabel9" runat="server"  LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />                
                    <%# Eval("UsrSigHTMLTags").ToString()%>&nbsp;|&nbsp; 
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel10" runat="server" LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <%# Convert.ToInt32(Eval("UsrAlbums")) %>&nbsp;|&nbsp;                   
                    <YAF:LocalizedLabel  ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <%# Convert.ToInt32(Eval("UsrAlbumImages")) %>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel12" runat="server" LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />
                    <%# Convert.ToInt32(Eval("SortOrder")) %>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel  ID="HelpLabel13" runat="server" LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;  
                    <%# Eval("Style").ToString() %>&nbsp; 
					</td>
					<td class="post">
						<asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "RankID") %>'>
                          <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
                        </asp:LinkButton>						|
						<asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "RankID") %>'>
                          <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" />
                        </asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="6" align="center">
				<asp:Button ID="NewRank" runat="server" OnClick="NewRank_Click" CssClass="pbutton" /></td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
