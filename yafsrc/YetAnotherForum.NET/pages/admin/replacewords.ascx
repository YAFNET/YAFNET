<%@ Control Language="c#" Debug="true" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.replacewords" Codebehind="replacewords.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<asp:Repeater ID="list" runat="server">
		<HeaderTemplate>
			<table class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="3">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_REPLACEWORDS" />
					</td>
				</tr>
				<tr>
					<td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BAD" LocalizedPage="ADMIN_REPLACEWORDS" />
					</td>
					<td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GOOD" LocalizedPage="ADMIN_REPLACEWORDS" />
					</td>
					<td class="header2">
						&nbsp;
					</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
					<%# HtmlEncode(Eval("badword")) %>
				</td>
				<td class="post">
					<%# HtmlEncode(Eval("goodword")) %>
				</td>
				<td class="post" align="right">
					<YAF:ThemeButton ID="btnEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval("ID") %>' 
                        TextLocalizedTag="EDIT"
                        TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server">					    
					</YAF:ThemeButton>	
					<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" OnLoad="Delete_Load"  CommandName='delete' 
                        TextLocalizedTag="DELETE"
                        CommandArgument='<%# Eval( "ID") %>' TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server">
					</YAF:ThemeButton>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="3" align="center">
					<asp:Button runat="server" CommandName='add' ID="Linkbutton3" CssClass="pbutton" OnLoad="addLoad"> </asp:Button>
					|
					<asp:Button runat="server" CommandName='import' ID="Linkbutton5" CssClass="pbutton" OnLoad="importLoad"></asp:Button>
					|
					<asp:Button runat="server" CommandName='export' ID="Linkbutton4" CssClass="pbutton" OnLoad="exportLoad"></asp:Button>
				</td>
			</tr>
			</table>
		</FooterTemplate>
	</asp:Repeater>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
