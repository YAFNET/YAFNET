<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums"
    CodeBehind="nntpforums.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1" colspan="6">
                 <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" />
            </td>
        </tr>
        <asp:Repeater ID="RankList" OnItemCommand="RankList_ItemCommand" runat="server">
            <HeaderTemplate>
                <tr>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="Server" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Forum" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Active" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </td>
                    <td class="header2">
                        &nbsp;
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="post">
                        <%# Eval( "Name") %>
                    </td>
                    <td class="post">
                        <%# Eval( "GroupName") %>
                    </td>
                    <td class="post">
                        <%# Eval( "ForumName") %>
                    </td>
                    <td class="post">
                        <%# Eval( "Active") %>
                    </td>
                    <td class="post" align="right">
                        <YAF:ThemeButton ID="ThemeButtonEdit" CssClass="yaflittlebutton" 
                            CommandName='edit' CommandArgument='<%# Eval( "NntpForumID") %>' 
                            TitleLocalizedTag="EDIT" 
                            ImageThemePage="ICONS" ImageThemeTag="EDIT_SMALL_ICON"
                            TextLocalizedTag="EDIT" 
                            runat="server">
					    </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete" CssClass="yaflittlebutton" 
                                    CommandName='delete' CommandArgument='<%# Eval( "NntpForumID") %>' 
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
            <td class="footer1" colspan="5" align="center">
                <asp:Button ID="NewForum" runat="server" CssClass="pbutton" OnClick="NewForum_Click" />
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
