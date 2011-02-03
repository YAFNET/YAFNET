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
				<td class="post">
					<asp:LinkButton runat="server" CommandName='edit' CommandArgument='<%# Eval("ID") %>'
						ID="Linkbutton1">
  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
					</asp:LinkButton>
					|
					<asp:LinkButton runat="server"  CommandName='delete' OnLoad="Delete_Load"
						CommandArgument='<%# Eval("ID") %>' ID="Linkbutton2">
                          <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="DELETE" />
					</asp:LinkButton>
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
