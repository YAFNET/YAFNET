<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.pageaccesslist" Codebehind="pageaccesslist.ascx.cs" %>
<%@ Import Namespace="YAF.Types" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="3">
				  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PAGEACCESSLIST" />
			</td>
		</tr>
		<tr class="header2">
			<td>
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER"  LocalizedPage="ADMIN_PAGEACCESSLIST" />
			</td>	
            	<td>
				<YAF:LocalizedLabel ID="BoardNameLabel" runat="server" LocalizedTag="BOARDnAME"  LocalizedPage="ADMIN_PAGEACCESSLIST" />
			</td>	
			<td>
				&nbsp;
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server" OnItemCommand="List_ItemCommand">
			<ItemTemplate>
				<tr class="postheader">
					<td>
					    <!-- User Name -->
					  <img alt='<%# this.HtmlEncode(this.Get<YafBoardSettings>().EnableDisplayName ? Eval( "DisplayName") : Eval( "Name")) %>'
                                    title='<%# this.HtmlEncode(this.Get<YafBoardSettings>().EnableDisplayName ? Eval( "DisplayName") : Eval( "Name")) %>'
                                    src='<%# this.Get<ITheme>().GetItem("ICONS","USER_BUSINESS") %>' />&nbsp;<%# this.HtmlEncode(this.Get<YafBoardSettings>().EnableDisplayName ? Eval("DisplayName") : Eval("Name"))%>
					</td>
                    	<td>
                    	 <%# this.HtmlEncode(Eval( "BoardName")) %>
                        </td>		
					<td width="15%" style="font-weight: normal">
						<asp:LinkButton runat='server' CommandName='edit' CommandArgument='<%# Eval( "UserID") %>'>
						  <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" TitleLocalizedPage="ADMIN_PAGEACCESSLIST"   CommandName='edit' CommandArgument='<%# Eval( "UserID") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
                        </asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1" style="text-align: center;">
			<td colspan="2">
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
