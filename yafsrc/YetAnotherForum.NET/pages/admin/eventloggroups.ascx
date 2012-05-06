<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.eventloggroups" Codebehind="eventloggroups.ascx.cs" %>
<%@ Import Namespace="YAF.Types" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="3">
				  <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EVENTLOGGROUPS" />
			</td>
		</tr>
		<tr>
			<td class="header2">
				<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="GROUPNAME"  LocalizedPage="ADMIN_EVENTLOGGROUPS" />
			</td>	
            <td class="header2">
				<YAF:LocalizedLabel ID="BoardNameLabel" runat="server" LocalizedTag="BOARDNAME"  LocalizedPage="ADMIN_EVENTLOGGROUPS" />
			</td>	
			<td class="header2">&nbsp;
				
			</td>
		</tr>
		<asp:Repeater ID="List" runat="server" OnItemCommand="List_ItemCommand">
			<ItemTemplate>
				<tr class="post">
					<td>
					    <!-- Group Name -->
					  <img alt='<%# this.HtmlEncode(Eval("Name")) %>'
                                    title='<%# this.HtmlEncode(Eval("Name")) %>'
                                    src='<%# this.Get<ITheme>().GetItem("VOTE","VOTE_USERS")  %>' />&nbsp;<%# this.HtmlEncode(Eval("Name"))%>
					</td>
                    	<td>
                    	 <%# this.HtmlEncode(Eval( "BoardName")) %>
                        </td>		
					<td width="15%">
						  <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" TitleLocalizedPage="ADMIN_EVENTLOGGROUPS" CommandName='edit' CommandArgument='<%# Eval( "GroupID") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr class="footer1" style="text-align: center;">
			<td colspan="3">
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
