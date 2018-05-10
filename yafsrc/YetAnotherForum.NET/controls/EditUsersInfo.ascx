<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.EditUsersInfo"
    CodeBehind="EditUsersInfo.ascx.cs" %>
<table class="content" width="100%" cellspacing="1" cellpadding="0">
    <tr>
        <td class="header1" colspan="2">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEAD_USER_DETAILS" LocalizedPage="ADMIN_EDITUSER" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="USERINFO_NAME" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:TextBox Style="width: 300px" ID="Name" runat="server" Enabled="false" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="USERINFO_DISPLAYNAME" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:TextBox Style="width: 300px" ID="DisplayName" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="EMAIL" LocalizedPage="PROFILE" />
        </td>
        <td class="post">
            <asp:TextBox Style="width: 300px" ID="Email" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="RANK" LocalizedPage="ADMIN_USERS" />
        </td>
        <td class="post">
            <asp:DropDownList Style="width: 300px" ID="RankID" runat="server" CssClass="standardSelectMenu" />
        </td>
    </tr>


    <tr runat="server" id="IsHostAdminRow">
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="USERINFO_HOST" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsHostAdminX" />
        </td>
    </tr>
    <tr runat="server" id="IsCaptchaExcludedRow">
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="USERINFO_EX_CAPTCHA" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsCaptchaExcluded" />
        </td>
    </tr>
    <tr runat="server" id="IsExcludedFromActiveUsersRow">
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="USERINFO_EX_ACTIVE" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsExcludedFromActiveUsers" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="USERINFO_APPROVED" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsApproved" />
        </td>
    </tr>
    <!-- Easy to enable it if there is major issues (i.e. Guest being deleted). -->
    <tr runat="server" id="IsGuestRow" visible="false">
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="USERINFO_GUEST" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsGuestX" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="JOINED" LocalizedPage="PROFILE" />

        </td>
        <td class="post">
            <asp:TextBox Style="width: 300px" ID="Joined" runat="server" Enabled="False" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="LASTVISIT" LocalizedPage="PROFILE" />
        </td>
        <td class="post">
            <asp:TextBox Style="width: 300px" ID="LastVisit" runat="server" Enabled="False" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="FACEBOOK_USER" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsFacebookUser" Enabled="false" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="TWITTER_USER" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsTwitterUser" Enabled="false" />
        </td>
    </tr>
    <tr>
        <td class="postheader">
            <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="Google_USER" LocalizedPage="ADMIN_EDITUSER" />
        </td>
        <td class="post">
            <asp:CheckBox runat="server" ID="IsGoogleUser" Enabled="false" />
        </td>
    </tr>
    <tr>
        <td class="postfooter" colspan="2" align="center">
            <asp:Button ID="Save" runat="server" CssClass="pbutton" OnClick="Save_Click" />
        </td>
    </tr>
</table>
