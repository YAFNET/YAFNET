<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.bannedip" Codebehind="bannedip.ascx.cs" %>
<%@ Import Namespace="YAF.Core"%>
<%@ Import Namespace="YAF.Core.Services" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
		<asp:Repeater ID="list" runat="server" OnItemCommand="list_ItemCommand">
		<HeaderTemplate>
				<table class="content" cellspacing="1" cellpadding="0" width="100%">

				<tr>
					<td class="header1" colspan="5">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_BANNEDIP" />
                     </td>
				</tr>
				<tr>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MASK" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="SINCE" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="REASON" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>	
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="BAN_BY" LocalizedPage="ADMIN_BANNEDIP" />
                    </td>		
					<td class="header2">
						&nbsp;</td>
				</tr>
			</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post">
					<%# Eval("Mask") %>
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
				<td class="post">
					<asp:LinkButton runat="server" CommandName='edit' CommandArgument='<%# Eval("ID") %>'>
                      <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
                    </asp:LinkButton>
					|
					<asp:LinkButton runat="server" CommandName='delete' CommandArgument='<%# Eval("ID") %>'>
                      <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" />
                    </asp:LinkButton>
				</td>
			</tr>
			</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="5" align="center">
					<asp:Button runat="server" OnLoad="Add_Load" CommandName='add' CssClass="pbutton"></asp:Button></td>
			</tr>
			</table>
			</FooterTemplate>
		</asp:Repeater>
	
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
