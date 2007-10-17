<%@ Control Language="C#" AutoEventWireup="true" CodeFile="extensions.ascx.cs" Inherits="YAF.Pages.Admin.extensions" %>

<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">
	<asp:repeater id="list" runat="server">
		<HeaderTemplate>
			<table class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="2">Allowed File Extensions</td>
				</tr>
				<tr>
					<td class="header2" width="90%">File Extensions</td>
					<td class="header2">&nbsp;</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# HtmlEncode(Eval("extension")) %></td>
				<td class="post">
					<asp:linkbutton runat="server" text="Edit" commandname="edit" commandargument='<%# Eval("extensionId") %>' ID="Linkbutton1">
					</asp:linkbutton>
					|
					<asp:linkbutton runat="server" text="Delete" onload="Delete_Load" commandname="delete" commandargument='<%# Eval("extensionId") %>' ID="Linkbutton2">
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