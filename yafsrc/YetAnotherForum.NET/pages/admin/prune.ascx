<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.prune" CodeBehind="prune.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_PRUNE" />
            </td>
        </tr>
        <tr>
            <td class="header2" colspan="2" height="30px">
                <asp:Label ID="lblPruneInfo" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="PRUNE_FORUM" LocalizedPage="ADMIN_PRUNE" />
            </td>
            <td class="post" width="50%">
                <asp:DropDownList ID="forumlist" runat="server" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="PRUNE_DAYS" LocalizedPage="ADMIN_PRUNE" />
            </td>
            <td class="post">
                <asp:TextBox ID="days" runat="server" CssClass="Numeric"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="PRUNE_PERMANENT" LocalizedPage="ADMIN_PRUNE" />
            </td>
            <td class="post">
                <asp:CheckBox ID="permDeleteChkBox" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="footer1" colspan="2" align="center">
                <asp:Button ID="commit" runat="server" class="pbutton" OnClick="commit_Click" OnLoad="PruneButton_Load"></asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
