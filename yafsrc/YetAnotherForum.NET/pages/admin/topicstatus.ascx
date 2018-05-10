<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Pages.Admin.topicstatus" Codebehind="topicstatus.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu ID="Adminmenu1" runat="server">
	  <asp:Repeater ID="list" runat="server">
        <HeaderTemplate>
      	<table class="content" cellspacing="1" cellpadding="0" width="100%">
     
                <tr>
                    <td class="header1" colspan="4">
                          <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_TOPICSTATUS" />
                     </td>
                </tr>
                <tr>
                    <td class="header2" style="width:16px">
                        &nbsp;</td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TOPICSTATUS_NAME" LocalizedPage="ADMIN_TOPICSTATUS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="DEFAULT_DESCRIPTION" LocalizedPage="ADMIN_TOPICSTATUS" />
                     </td>
                     <td class="header2">
                        &nbsp;</td>
                </tr>
        	 </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td class="post">
                        <YAF:ThemeImage runat="server" ID="TopicStatusIcon" ThemePage="TOPIC_STATUS" ThemeTag='<%# HtmlEncode(Eval("TopicStatusName")) %>'></YAF:ThemeImage>
                        
                </td>
                <td class="post">
                        <%# HtmlEncode(Eval("TopicStatusName")) %>
                </td>
                <td class="post">
						<%# HtmlEncode(Eval("DefaultDescription"))%>
				</td>
                <td class="post" align="right">
                    <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" CommandName='edit' CommandArgument='<%# Eval("TopicStatusId") %>' 
                        TextLocalizedTag="EDIT" TitleLocalizedTag="EDIT" ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON" runat="server"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" OnLoad="Delete_Load"  CommandName='delete' CommandArgument='<%# Eval("TopicStatusId") %>' 
                        TextLocalizedTag="DELETE" TitleLocalizedTag="DELETE" ImageThemePage="ICONS" ImageThemeTag="DELETE_SMALL_ICON" runat="server"></YAF:ThemeButton>
                </td>
            </tr>
        	 </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td class="footer1" colspan="4" style="text-align:center">
                    <asp:Button runat="server" CommandName='add' ID="Linkbutton3" CssClass="pbutton" OnLoad="addLoad"></asp:Button>
                    |
                    <asp:Button runat="server" CommandName='import' ID="Linkbutton5" CssClass="pbutton" OnLoad="importLoad"> </asp:Button>
                    |
                    <asp:Button runat="server" CommandName='export' ID="Linkbutton4" CssClass="pbutton" OnLoad="exportLoad"></asp:Button>
                </td>
            </tr>
           	</table>
        	 </FooterTemplate>
    	 </asp:Repeater>

</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
