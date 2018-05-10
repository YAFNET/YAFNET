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
					<td class="header2">
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
					<td class="post" align="right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" 
                            CommandName='edit' CommandArgument='<%# Eval( "MedalID") %>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON"
                            TextLocalizedTag="EDIT" 
                            runat="server">
					    </YAF:ThemeButton>
					    <YAF:ThemeButton ID="ThemeButtonMoveUp" CssClass="yaflittlebutton" 
                            CommandName='moveup' CommandArgument='<%# Eval("MedalID") %>' 
					        TitleLocalizedTag="MOVE_UP" 
                            TitleLocalizedPage="ADMIN_SMILIES"
					        ImageThemePage="SORT" ImageThemeTag="ASCENDING"
					        TextLocalizedTag="MOVE_UP" 
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server"/>
					    <YAF:ThemeButton ID="ThemeButtonMoveDown" CssClass="yaflittlebutton" 
					        CommandName='movedown' CommandArgument='<%# Eval("MedalID") %>' 
					        TitleLocalizedTag="MOVE_DOWN" 
                            TitleLocalizedPage="ADMIN_SMILIES"
					        ImageThemePage="SORT" ImageThemeTag="DESCENDING"
					        TextLocalizedTag="MOVE_DOWN" 
                            TextLocalizedPage="ADMIN_SMILIES"
					        runat="server" />
						<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# Eval( "MedalID") %>' 
                                    TitleLocalizedTag="DELETE" 
                                    ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
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
