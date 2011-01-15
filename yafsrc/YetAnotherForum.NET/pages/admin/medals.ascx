<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.medals" Codebehind="medals.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="6">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_MEDALS" />
            </td>
		</tr>
		<asp:Repeater ID="MedalList" OnItemCommand="MedalList_ItemCommand" runat="server">
			<HeaderTemplate>
				<tr>
					<td class="header2" style="width: 20px;">
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ORDER" /></td>
					<td class="header2" style="width: 50px;">
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IMAGE_TEXT" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NAME" LocalizedPage="COMMON" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="CATEGORY" /></td>
					<td class="header2">
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_BBCODE" /></td>
					<td class="header2" style="width: 125px;">
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="COMMAND" /></td>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td class="post">
						<%# Eval( "SortOrder") %>
					</td>
					<td class="post">
						<%# RenderImages(Container.DataItem) %>
					</td>
					<td class="post">
						<%# Eval( "Name") %>
					</td>
					<td class="post">
						<%# Eval( "Category") %>
					</td>
					<td class="post">
						<%# ((string)Eval( "Description")).Substring(0, Math.Min(Eval( "Description").ToString().Length, 100)) + "..." %>
					</td>
					<td class="post">
						<asp:LinkButton ID="EditMedal" runat="server" CommandName="edit" CommandArgument='<%# Eval("MedalID") %>'>
                          <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="EDIT" />
                        </asp:LinkButton>
						|
						<asp:LinkButton ID="MoveUp" runat="server" CommandName="moveup" CommandArgument='<%# Eval("MedalID") %>'
							Text="▲" />
						<asp:LinkButton ID="MoveDown" runat="server" CommandName="movedown" CommandArgument='<%# Eval("MedalID") %>'
							Text="▼" />
						|
						<asp:LinkButton ID="DeleteMedal" runat="server" OnLoad="Delete_Load" CommandName="delete"
							CommandArgument='<%# Eval("MedalID") %>'>
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETE" />
                        </asp:LinkButton>
					</td>
				</tr>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="6" align="center">
				<asp:Button ID="NewMedal" runat="server" OnClick="NewMedal_Click" CssClass="pbutton" />
            </td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
