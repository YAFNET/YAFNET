<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.languages" Codebehind="languages.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
	<table class="content" width="100%" cellspacing="1" cellpadding="0">
		<tr>
			<td class="header1" colspan="8">
				<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_LANGUAGES" />
			</td>
		</tr>
        <tr>
            <td style="padding:0">
		<asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
			    <table style="width:100%"  cellspacing="1" cellpadding="0" class="sortable tablesorter">
                    <thead>
                        <tr class="header2">
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="LANG_NAME" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="CULTURE_TAG" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NATIVE_NAME" LocalizedPage="ADMIN_LANGUAGES" />
                            </th>
                            <th>
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="FILENAME" />
                            </th>
                            <th>
                                &nbsp;
                            </th>
                        </tr>
                    </thead>
                <tbody>
			</HeaderTemplate>
			<ItemTemplate>
				<tr class="post">
                    <td>
						<%# Eval("CultureEnglishName")%>
					</td>
                    <td>
						<%# Eval("CultureTag")%>
					</td>
                     <td>
						<%# Eval("CultureNativeName")%>
					</td>
					<td>
						<%# Eval("CultureFile")%>
					</td>
                    <td class="rightItem">
                        <YAF:ThemeButton ID="btnEdit" 
                            CssClass="yaflittlebutton" 
                            CommandName='edit' 
                            CommandArgument='<%# Eval("CultureFile")%>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" 
                            TextLocalizedTag="EDIT"
                            ImageThemeTag="EDIT_SMALL_ICON" 
                            runat="server">
                        </YAF:ThemeButton>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>
                </tbody>
        </table>
                </FooterTemplate>
		</asp:Repeater>
                </td>
         </tr>
         <tr>
           <td class="footer1" align="center" colspan="8" style="height:30px"></td>
         </tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
