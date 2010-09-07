<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editboard" Codebehind="editboard.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <asp:UpdatePanel ID="UppdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                Edit Board</td>
        </tr>
        <tr>
            <td width="50%" class="postheader">
                <strong>Name:</strong><br />
                The name of the board.</td>
            <td width="50%" class="post">
                <asp:TextBox ID="Name" runat="server" Style="width: 100%"></asp:TextBox></td>
        </tr>
              <tr>
            <td width="50%" class="postheader">
                <strong>Culture:</strong><br />
                The Culture of the board.</td>
            <td width="50%" class="post">
            <asp:DropDownList ID="Culture" runat="server" />               
        </tr>
        <tr>
            <td class="postheader">
                <strong>Allow Threaded:</strong><br />
                Allow threaded view for posts.</td>
            <td class="post">
                <asp:CheckBox runat="server" ID="AllowThreaded" /></td>
        </tr>                  
        <tr>
            <td width="50%" class="postheader">
                <strong>Membership Application Name:</strong><br />
                Application name required for provider, blank will use ApplicationName in web.config.</td>
            <td width="50%" class="post">
                <asp:TextBox ID="BoardMembershipAppName" runat="server" Style="width: 100%"></asp:TextBox></td>
        </tr>
        <asp:PlaceHolder runat="server" ID="CreateNewAdminHolder">
        <tr>
            <td class="postheader">
                <strong>Create New Admin User:</strong><br />
                Only required when creating a board using a new &amp; different membership application name.</td>
            <td class="post">
                <asp:CheckBox runat="server" ID="CreateAdminUser" AutoPostBack="true" OnCheckedChanged="CreateAdminUser_CheckedChanged" /></td>
        </tr>
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="AdminInfo" Visible="false">
            <tr>
                <td colspan="2" class="header2">New Administrator Information</td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>User Name:</strong><br />
                    This will be the administrator for the board.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserName" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>User Email:</strong><br />
                    Email address for administrator.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserEmail" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>Password:</strong><br />
                    Enter password for administrator here.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPass1" TextMode="password" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>Verify Password:</strong><br />
                    Verify the password.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPass2" TextMode="password" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>Security Question:</strong><br />
                    The question you will be asked when you need to retrieve your lost password.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPasswordQuestion" /></td>
            </tr>
            <tr>
                <td class="postheader">
                    <strong>Security Answer:</strong><br />
                    The answer to the security question.</td>
                <td class="post">
                    <asp:TextBox runat="server" ID="UserPasswordAnswer" /></td>
            </tr>
        </asp:PlaceHolder>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click" />
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click" />
            </td>
        </tr>
    </table>
    </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
