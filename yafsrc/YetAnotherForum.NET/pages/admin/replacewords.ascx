<%@ Control language="c#" Debug="true" CodeFile="replacewords.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.replacewords" %>




<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<asp:repeater id="list" runat="server">
		<HeaderTemplate>
			<table class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="3">Replace Words</td>
				</tr>
				<tr>
					<td class="header2">"Bad" (Find) Expression</td>
					<td class="header2">"Good" (Replace) Expression</td>
					<td class="header2">&nbsp;</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# HtmlEncode(Eval("badword")) %></td>
				<td class="post"><%# HtmlEncode(Eval("goodword")) %></td>
				<td class="post">
					<asp:linkbutton runat=server text=Edit commandname='edit' commandargument='<%# Eval("ID") %>' ID="Linkbutton1">
					</asp:linkbutton>
					|
					<asp:linkbutton runat=server text=Delete commandname='delete' OnLoad="Delete_Load" commandargument='<%# Eval("ID") %>' ID="Linkbutton2">
					</asp:linkbutton>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="3">
					<asp:linkbutton runat="server" text="Add" commandname='add' ID="Linkbutton3"></asp:linkbutton>
                    |
                    <asp:LinkButton runat="server" Text="Import from XML" CommandName='import' ID="Linkbutton5"></asp:LinkButton>
                    |
                    <asp:LinkButton runat="server" Text="Export to XML" CommandName='export' ID="Linkbutton4"></asp:LinkButton>					
                    </td>
			</tr>
			</table>
		</FooterTemplate>
	</asp:repeater>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat="server" />
