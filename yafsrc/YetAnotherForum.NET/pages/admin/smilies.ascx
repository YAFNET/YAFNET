<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.smilies"CodeBehind="smilies.ascx.cs" %>
<%@ Import Namespace="YAF.Utils" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <asp:UpdatePanel ID="SmilesUpdatePanel" runat="server">
        <ContentTemplate>
            <YAF:Pager ID="Pager" runat="server" />
            <table width="100%" cellspacing="1" cellpadding="0" class="content">
                <asp:Repeater runat="server" ID="List">
                    <HeaderTemplate>
                        <tr>
                            <td class="header1" colspan="5">
                                Smilies
                            </td>
                        </tr>
                        <tr>
                            <td class="header2">
                                Order
                            </td>
                            <td class="header2">
                                Code
                            </td>
                            <td class="header2" align="center">
                                Smile
                            </td>
                            <td class="header2">
                                Emotion
                            </td>
                            <td class="header2">
                                Commands
                            </td>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td class="post">
                                <%# Eval("SortOrder") %>
                            </td>
                            <td class="post">
                                <%# Eval("Code") %>
                            </td>
                            <td class="post" align="center">
                                <img src="<%# YafForumInfo.ForumClientFileRoot + YafBoardFolders.Current.Emoticons %>/<%# Eval("Icon") %>" />
                            </td>
                            <td class="post">
                                <%# Eval("Emoticon") %>
                            </td>
                            <td class="post">
                                <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval("SmileyID") %>'
                                    Text="Edit" />
                                |
                                <asp:LinkButton ID="MoveUp" runat="server" CommandName="moveup" CommandArgument='<%# Eval("SmileyID") %>'
                                    Text="▲" />
                                <asp:LinkButton ID="MoveDown" runat="server" CommandName="movedown" CommandArgument='<%# Eval("SmileyID") %>'
                                    Text="▼" />
                                |
                                <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval("SmileyID") %>'
                                    Text="Delete" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr>
                            <td class="footer1" colspan="5">
                                <asp:LinkButton runat="server" CommandName="add">Add Smiley</asp:LinkButton>
                                |
                                <asp:LinkButton runat="server" CommandName="import">Import Smiley Pack</asp:LinkButton>
                            </td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </table>
            <YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" />
        </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
