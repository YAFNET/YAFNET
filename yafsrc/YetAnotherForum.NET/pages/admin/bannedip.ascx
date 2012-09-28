<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedip" Codebehind="bannedip.ascx.cs" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
  <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
		<asp:Repeater ID="list" runat="server" OnItemCommand="List_ItemCommand">
		<HeaderTemplate>
				<table class="content" cellspacing="1" cellpadding="0" width="100%">

				<tr>
					<td class="header1" colspan="5">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDIP" />
                     </td>
				</tr>
				<tr>
					<td class="header2" width="15%">
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>
					<td class="header2" width="15%">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>
					<td class="header2" width="15%">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>	
					<td class="header2" width="10%">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BAN_BY" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>		
					<td class="header2">&nbsp;
						</td>
				</tr>
			</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
				<asp:HiddenField ID="fID" Value='<%# Eval("ID") %>' runat="server"/>
				<asp:Label ID="MaskBox" Text='<%# Eval("Mask") %>' runat="server"></asp:Label>	
				</td>
				<td class="post">
					<%# this.Get<IDateTime>().FormatDateTime(Eval("Since")) %>
				</td>
				<td class="post">
					<%# Eval("Reason") %>
				</td>
				<td class="post">
				<YAF:UserLink ID="UserLink1" runat="server" UserID='<%# string.IsNullOrEmpty(Eval("UserID").ToString())? -1 :Eval("UserID") %>' />
				</td>
				<td class="post" style="text-align:right">
				<YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval("ID") %>' 
                    TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" CommandName='delete' CommandArgument='<%# Eval("ID") %>' 
                    TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></YAF:ThemeButton>
				</td>
			</tr>
			</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="5" style="text-align:center">
					<asp:Button runat="server" OnLoad="Import_Load" CommandName='import' CssClass="pbutton"></asp:Button>&nbsp;|&nbsp;<asp:Button 
                    runat="server" OnLoad="Add_Load" CommandName='add' CssClass="pbutton"></asp:Button></td></td>
			</tr>
			</table>
			</FooterTemplate>
		</asp:Repeater>
	 <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
