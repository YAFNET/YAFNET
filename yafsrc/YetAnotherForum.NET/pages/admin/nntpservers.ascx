<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpservers"
    CodeBehind="nntpservers.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" width="100%" cellspacing="1" cellpadding="0">
        <tr>
            <td class="header1" colspan="6">
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPSERVERS" />
            </td>
        </tr>
        <asp:Repeater ID="RankList" runat="server">
            <HeaderTemplate>
                <tr>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ADRESS" LocalizedPage="ADMIN_NNTPSERVERS" />
                    </td>
                    <td class="header2">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="USERNAME" LocalizedPage="ADMIN_NNTPSERVERS" />
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
                        <%# Eval( "Address") %>
                    </td>
                    <td class="post">
                        <%# Eval( "UserName") %>
                    </td>
                    <td class="post rightItem">
                        <asp:LinkButton runat="server" CommandName="edit" CommandArgument='<%# Eval( "NntpServerID") %>'>
                            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="EDIT" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </asp:LinkButton>
                        |
                        <asp:LinkButton runat="server" OnLoad="Delete_Load" CommandName="delete" CommandArgument='<%# Eval( "NntpServerID") %>'>
                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td class="footer1" colspan="5" align="center">
                <asp:Button ID="NewServer" runat="server" CssClass="pbutton" OnClick="NewServer_Click" />
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
