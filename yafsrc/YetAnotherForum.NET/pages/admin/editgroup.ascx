<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editgroup"
    CodeBehind="editgroup.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="11">
                Add/Edit Role
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Name:</strong><br />
                Name of this role.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 300px" ID="Name" runat="server" />
                <asp:RequiredFieldValidator ID="postNameRequired" runat="server" Display="Dynamic" ControlToValidate="Name" ErrorMessage="Role name is required."></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Is Start:</strong><br />
                If this is checked, all new users will be a member of this role.
            </td>
            <td class="post">
                <asp:CheckBox ID="IsStartX" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Is Forum Moderator:</strong><br />
                When this is checked, members of this role will have some admin access rights in
                YAF.
            </td>
            <td class="post">
                <asp:CheckBox ID="IsModeratorX" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <strong>Is Admin:</strong><br />
                Means that users in this role are admins in YAF.
            </td>
            <td class="post">
                <asp:CheckBox ID="IsAdminX" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>PMessages:</strong><br />
                Max Private Messages allowed to Group members.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 80px" ID="PMLimit" Text="0" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Description:</strong><br />
                Enter here a role description.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 300px" ID="Description" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Max number of chars in a user signature:</strong><br />
                Max number of chars in a user signature in the role.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 80px" ID="UsrSigChars" runat="server"  Text="128" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>User signature BBCodes:</strong><br />
                Comma separated BBCodes allowed in a user signature in the role.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 300px" ID="UsrSigBBCodes" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>User signature HTML tags:</strong><br />
                Comma separated HTML tags allowed in a user signature in the group.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 300px" ID="UsrSigHTMLTags" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>User Albums Number:</strong><br />
                Integer value for a user allowed albums number.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 80px" ID="UsrAlbums" runat="server" Text="0" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Total Album Images Number:</strong><br />
                Integer value for a user allowed images number in ALL albums.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 80px" ID="UsrAlbumImages" runat="server" Text="0" />
            </td>
        </tr>
        <tr>
            <td class="postheader" style="width: 50%">
                <strong>Priority:</strong><br />
                Enter here priority for different tasks.
            </td>
            <td class="post" style="width: 50%">
                <asp:TextBox Style="width: 50px" ID="Priority" MaxLength="5" Text="0" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="postheader" width="50%">
                <strong>Style:</strong><br />
                Enter here a combined style string for coloured nicks.
            </td>
            <td class="post" width="50%">
                <asp:TextBox Style="width: 100%" ID="StyleTextBox" TextMode="MultiLine" runat="server" />
            </td>
        </tr>
        <tr runat="server" visible="false" id="IsGuestTR">
            <td class="postheader">
                <strong>Is Guest:</strong><br />
                This flag is internal and makes the role unavailable to .NET membership. Never assign
                this role to any users except the (1) guest user. If you do flag this role as IsGuest,
                the guest user must a member of it. Never use this flag in conjunction with any
                other flags.
            </td>
            <td class="post">
                <asp:CheckBox ID="IsGuestX" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr runat="server" id="NewGroupRow">
            <td class="postheader">
                <strong>Initial Access Mask:</strong><br />
                The initial access mask for all forums.
            </td>
            <td class="post">
                <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID" />
            </td>
        </tr>
        <asp:Repeater ID="AccessList" runat="server">
            <HeaderTemplate>
                <tr>
                    <td class="header1" colspan="11">
                        Access
                    </td>
                </tr>
                <tr>
                    <td class="header2">
                        Forum
                    </td>
                    <td class="header2">
                        Access Mask
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="postheader">
                        <asp:Label ID="ForumID" Visible="false" runat="server" Text='<%# Eval( "ForumID") %>'></asp:Label>
                        <strong>
                            <%# Eval( "ForumName") %>
                        </strong>
                        <br />
                        Board:
                        <%# Eval( "BoardName") %>
                        <br />
                        Category:
                        <%# Eval( "CategoryName") %>
                    </td>
                    <td class="post">
                        <asp:DropDownList runat="server" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID"
                            OnPreRender="SetDropDownIndex" value='<%# Eval("AccessMaskID") %>' />
                        ...
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td class="postfooter" align="center" colspan="11">
                <asp:Button ID="Save" runat="server" Text="Save" OnClick="Save_Click"></asp:Button>&nbsp;
                <asp:Button ID="Cancel" runat="server" Text="Cancel" OnClick="Cancel_Click"></asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
