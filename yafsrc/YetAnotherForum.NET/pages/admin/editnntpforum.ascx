<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editnntpforum"
    CodeBehind="editnntpforum.ascx.cs" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Edit NNTP Forum
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Server:</strong><br />
                What server this groups is located.
            </td>
            <td class="post" width="50%">
                <asp:DropDownList ID="NntpServerID" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Group:</strong><br />
                The name of the newsgroup.
            </td>
            <td class="post">
                <asp:TextBox ID="GroupName" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Forum:</strong><br />
                The forum messages will be inserted into.
            </td>
            <td class="post">
                <asp:DropDownList ID="ForumID" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Active:</strong><br />
                Check this to make the forum active.
            </td>
            <td class="post">
                <asp:CheckBox ID="Active" runat="server" Checked="true" />
            </td>
        </tr>
        <tr>
            <td class="postfooter" align="middle" colspan="2">
                <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
