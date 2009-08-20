<%@ Control Language="c#" CodeFile="mail.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.Admin.mail" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Compose Email</td>
        </tr>
        <tr>
            <td class="postheader">
                To:</td>
            <td class="post">
                <asp:DropDownList ID="ToList" runat="server" DataValueField="GroupID" DataTextField="Name">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td class="postheader">
                Subject:</td>
            <td class="post">
                <asp:TextBox ID="Subject" runat="server" CssClass="edit" Style="Width:250px"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="postheader" valign="top">
                Message:</td>
            <td class="post">
                <asp:TextBox ID="Body" runat="server" TextMode="MultiLine" CssClass="edit" Rows="16" Style="Width:100%"></asp:TextBox></td>
        </tr>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Send" runat="server" Text="Send" OnClick="Send_Click" OnClientClick="return confirm('Are you sure you want to send this mail?');"></asp:Button></td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
