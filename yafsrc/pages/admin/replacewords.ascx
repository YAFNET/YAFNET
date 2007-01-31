<%@ Control language="c#" Debug="true" Codebehind="replacewords.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.replacewords" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.UI" Assembly="YAF.Classes.UI" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Classes.Utils" Assembly="YAF.Classes.Utils" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" Assembly="YAF.Controls" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<asp:repeater id="list" runat="server">
		<HeaderTemplate>
			<table class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="3">Replace Words</td>
				</tr>
				<tr>
					<td class="header2">Bad Word</td>
					<td class="header2">Good Word</td>
					<td class="header2">&nbsp;</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# Server.HtmlEncode(Convert.ToString(Eval("badword"))) %></td>
				<td class="post"><%# Server.HtmlEncode(Convert.ToString(Eval("goodword"))) %></td>
				<td class="post">
					<asp:linkbutton runat=server text=Edit commandname='edit' commandargument='<%# Eval("ID") %>' ID="Linkbutton1">
					</asp:linkbutton>
					|
					<asp:linkbutton runat=server text=Delete commandname='delete' commandargument='<%# Eval("ID") %>' ID="Linkbutton2">
					</asp:linkbutton>
				</td>
			</tr>
		</ItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="3">
					<asp:linkbutton runat="server" text="Add" commandname='add' ID="Linkbutton3"></asp:linkbutton></td>
			</tr>
			</table>
		</FooterTemplate>
	</asp:repeater>
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat="server" />
