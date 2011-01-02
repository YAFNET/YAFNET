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
                    <td class="post">
                        <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "NntpForumID") %>'><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="EDIT" LocalizedPage="ADMIN_NNTPFORUMS" /></asp:LinkButton>
                        |
                        <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "NntpForumID") %>'><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE" LocalizedPage="ADMIN_NNTPFORUMS" /></asp:LinkButton>
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
