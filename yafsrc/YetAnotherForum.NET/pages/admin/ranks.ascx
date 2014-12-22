<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ranks" Codebehind="ranks.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
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
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_RANKS" />
                    </td>			
					<td class="header2">
						&nbsp;
                    </td>                   
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="header2">
                    <img alt="" title="" src='<%# this.Get<ITheme>().GetItem("VOTE","VOTE_USERS") %>' />&nbsp;
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
                    <YAF:LocalizedLabel  ID="HelpLabel12" runat="server" LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label11" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "SortOrder" ).ToString()) %>'><%# this.Eval("SortOrder").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_RANKS" />
                     <asp:Label ID="Label4" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'><%# GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %></asp:Label>&nbsp;|&nbsp;				
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="IS_LADDER" LocalizedPage="ADMIN_RANKS" />
                    <asp:Label ID="Label1" runat="server" ForeColor='<%# GetItemColor(this.Eval( "Flags" ).BinaryAnd(RankFlags.Flags.IsLadder)) %>'><%# LadderInfo(Container.DataItem) %></asp:Label>&nbsp;|&nbsp;				
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_LIMIT" LocalizedPage="ADMIN_RANKS" />
					<asp:Label ID="Label6" runat="server" ForeColor='<%# GetItemColorString((Convert.ToInt32(Eval("PMLimit")) == int.MaxValue) ? "\u221E" : Eval("PMLimit").ToString()) %>'><%# ((Convert.ToInt32(Eval("PMLimit")) == int.MaxValue) ? "\u221E": Eval("PMLimit").ToString())%></asp:Label>&nbsp;|&nbsp;
                    <br />                    
                    <YAF:LocalizedLabel  ID="HelpLabel10" runat="server" LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label9" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "UsrAlbums" ).ToString()) %>'><%# this.Eval("UsrAlbums").ToString()%></asp:Label>&nbsp;|&nbsp;                   
                    <YAF:LocalizedLabel  ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label10" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "UsrAlbumImages" ).ToString()) %>'><%# this.Eval("UsrAlbumImages").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <br />                  
                    <YAF:LocalizedLabel  ID="HelpLabel13" runat="server" LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;  
                    <asp:Label ID="Label12" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "Style" ).ToString()) %>'><%# this.Eval("Style").ToString().IsSet() && (this.Eval("Style").ToString().Trim().Length > 0) ? "" : this.GetItemName(false)%></asp:Label>&nbsp;
                    <YAF:RoleRankStyles ID="RoleRankStylesRanks" RawStyles='<%# this.Eval( "Style" ).ToString() %>' runat="server" /> 
                    <br />
					<YAF:LocalizedLabel ID="HelpLabel7" runat="server" LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />                   
                    <asp:Label ID="Label5" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "UsrSigChars" ).ToString()) %>'><%# this.Eval("UsrSigChars").ToString().IsSet() ? this.Eval("UsrSigChars").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label7" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "UsrSigBBCodes" ).ToString()) %>'><%# this.Eval("UsrSigBBCodes").ToString().IsSet() ? this.Eval("UsrSigBBCodes").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;  
                    <YAF:LocalizedLabel ID="HelpLabel9" runat="server"  LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />                
                    <asp:Label ID="Label8" runat="server" ForeColor='<%# GetItemColorString(this.Eval( "UsrSigHTMLTags" ).ToString()) %>'><%#  this.Eval("UsrSigHTMLTags").ToString().IsSet() ? this.Eval("UsrSigHTMLTags").ToString() : this.GetItemName(false)%></asp:Label>&nbsp;|&nbsp; 
                    </td>
					<td class="post" align="right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" 
                            CommandName='edit' CommandArgument='<%# Eval( "RankID") %>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON"
                            TextLocalizedTag="EDIT" 
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# Eval( "RankID") %>' 
                                    TitleLocalizedTag="DELETE" 
                                    ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
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
