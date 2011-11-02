<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.forums" Codebehind="forums.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="header1" colspan="3">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FORUMS" LocalizedPage="TEAM" />
			</td>
		</tr>
		<asp:Repeater ID="CategoryList" runat="server">
			<ItemTemplate>
				<tr>
					<td class="header2">
						<%# HtmlEncode(Eval( "Name"))%>
					</td>
					<td class="header2" width="8%" align="center">
						<%# Eval( "SortOrder") %>
					</td>
					<td class="header2" width="17%" style="font-weight: normal">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval( "CategoryID") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" OnLoad="DeleteCategory_Load"  CommandName='delete' CommandArgument='<%# Eval( "CategoryID") %>' TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></YAF:ThemeButton>
                    </td>
				</tr>
				<asp:Repeater ID="ForumList" OnItemCommand="ForumList_ItemCommand" runat="server"
					DataSource='<%# ((System.Data.DataRowView)Container.DataItem).Row.GetChildRows("FK_Forum_Category") %>'>
					<ItemTemplate>
						<tr class="post">
							<td align="left">
								<strong>
									<%# HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Name\"]")) %></strong><br />
								<%# HtmlEncode(DataBinder.Eval(Container.DataItem, "[\"Description\"]")) %>
							</td>
							<td align="center">
								<%# DataBinder.Eval(Container.DataItem, "[\"SortOrder\"]") %>
							</td>
							<td>
                             <YAF:ThemeButton ID="btnEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>								
							 <YAF:ThemeButton ID="btnDuplicate" CssClass="yaflittlebutton" CommandName='copy' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' TitleLocalizedTag="COPY" ImageThemePage="ICONS" ImageThemeTag="COPY_SMALL_ICON" runat="server"></YAF:ThemeButton>
                             <YAF:ThemeButton ID="btnDelete" CssClass="yaflittlebutton" CommandName='delete' CommandArgument='<%# Eval( "[\"ForumID\"]") %>' TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></YAF:ThemeButton>
                            </td>
						</tr>
					</ItemTemplate>
				</asp:Repeater>
			</ItemTemplate>
		</asp:Repeater>
		<tr>
			<td class="footer1" colspan="3" align="center">
				<asp:Button ID="NewCategory" runat="server" OnClick="NewCategory_Click" CssClass="pbutton"></asp:Button>
				|
				<asp:Button ID="NewForum" runat="server" OnClick="NewForum_Click" CssClass="pbutton"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>

<YAF:SmartScroller ID="SmartScroller1" runat="server" />
