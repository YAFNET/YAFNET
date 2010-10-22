<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums"
    CodeBehind="nntpforums.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1" colspan="6">
                NNTP Forums
            </td>
        </tr>
        <asp:Repeater ID="RankList" OnItemCommand="RankList_ItemCommand" runat="server">
            <HeaderTemplate>
                <tr>
                    <td class="header2">
                        Server
                    </td>
                    <td class="header2">
                        Group
                    </td>
                    <td class="header2">
                        Forum
                    </td>
                    <td class="header2">
                        Active
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
                    <td class="post">
                        <asp:LinkButton runat="server"  CommandName="edit" CommandArgument='<%# Eval( "NntpForumID") %>'>Edit</asp:LinkButton>
                        |
                        <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "NntpForumID") %>'>Delete</asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td class="footer1" colspan="5">
                <asp:LinkButton ID="NewForum" runat="server" Text="New Forum" OnClick="NewForum_Click" />
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
