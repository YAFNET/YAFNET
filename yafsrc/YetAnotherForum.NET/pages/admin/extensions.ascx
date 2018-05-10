<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.extensions" Codebehind="extensions.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	  <asp:Repeater ID="list" runat="server">
        <HeaderTemplate>
      	<table class="content" cellspacing="1" cellpadding="0" width="100%">
     
                <tr>
                    <td class="header1" colspan="2">
                        <asp:Label ID="ExtensionTitle" runat="server" OnLoad="ExtensionTitle_Load">
                          <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                        </asp:Label>
                     </td>
                </tr>
                <tr>
                    <td class="header2" width="90%">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EXTENSIONS" />
                    </td>
                    <td class="header2">
                        &nbsp;</td>
                </tr>
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                    <strong>*.<%# HtmlEncode(Eval("extension")) %></strong></td>
                <td class="post" align="right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" 
                            CommandName='edit' CommandArgument='<%# Eval( "extensionId") %>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON"
                            TextLocalizedTag="EDIT" 
                            runat="server">
					    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# Eval( "extensionId") %>' 
                                    TitleLocalizedTag="DELETE" 
                                    ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td class="footer1" colspan="3" align="center">
                    <asp:Button runat="server" CommandName='add' ID="Linkbutton3" CssClass="pbutton" OnLoad="addLoad"></asp:Button>
                    |
                    <asp:Button runat="server" CommandName='import' ID="Linkbutton5" CssClass="pbutton" OnLoad="importLoad"> </asp:Button>
                    |
                    <asp:Button runat="server" CommandName='export' ID="Linkbutton4" CssClass="pbutton" OnLoad="exportLoad"></asp:Button>
                </td>
            </tr>
           	</table>
        	 </FooterTemplate>
    	 </asp:Repeater>

</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
