<%@ Register TagPrefix="yaf" Namespace="yaf.controls" Assembly="yaf" %>
<%@ Control language="c#" Debug="true" Codebehind="replacewords.ascx.cs" AutoEventWireup="false" Inherits="yaf.pages.admin.replacewords" %>
<yaf:PageLinks runat="server" id="PageLinks" />
<yaf:adminmenu runat="server" id="Adminmenu1">
	<asp:repeater id="list" runat="server">
		<HeaderTemplate>
			<table class="content" cellspacing="1" cellpadding="0" width="100%">
				<tr>
					<td class="header1" colspan="3">Replace Words</td>
				</tr>
				<tr>
					<td class="header2">Badword</td>
					<td class="header2">Goodword</td>
					<td class="header2">&nbsp;</td>
				</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr>
				<td class="post"><%# DataBinder.Eval(Container.DataItem,"badword") %></td>
				<td class="post"><%# DataBinder.Eval(Container.DataItem,"goodword") %></td>
				<td class="post">
					<asp:linkbutton runat=server text=Edit commandname='edit' commandargument='<%# DataBinder.Eval(Container.DataItem,"ID") %>' ID="Linkbutton1">
					</asp:linkbutton>
					|
					<asp:linkbutton runat=server text=Delete commandname='delete' commandargument='<%# DataBinder.Eval(Container.DataItem,"ID") %>' ID="Linkbutton2">
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
</yaf:adminmenu>
<yaf:savescrollpos runat="server" id="Savescrollpos1" />
